/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2020 by AG-Software, FRNathan13								 *
 * All Rights Reserved.																 *
 * Contact information for AG-Software is available at http://www.ag-software.de	 *
 *																					 *
 * Licence:																			 *
 * The agsXMPP SDK is released under a dual licence									 *
 * agsXMPP can be used under either of two licences									 *
 * 																					 *
 * A commercial licence which is probably the most appropriate for commercial 		 *
 * corporate use and closed source projects. 										 *
 *																					 *
 * The GNU Public License (GPL) is probably most appropriate for inclusion in		 *
 * other open source projects.														 *
 *																					 *
 * See README.html for details.														 *
 *																					 *
 * For general enquiries visit our website at:										 *
 * http://www.ag-software.de														 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections;
using AgsXMPP.Events;
using AgsXMPP.Factory;
using AgsXMPP.Xml.Dom;
using AgsXMPP.Xml.Xpnet;

namespace AgsXMPP.Xml
{
	public delegate void StreamError(object sender, Exception ex);
	public delegate void StreamHandler(object sender, Node e);

	/// <summary>
	/// Stream Parser is a lighweight Streaming XML Parser.
	/// </summary>
	public class StreamParser
	{
		protected internal EventEmitter<StreamHandler> m_OnStreamStart = new EventEmitter<StreamHandler>();
		protected internal EventEmitter<StreamHandler> m_OnStreamEnd = new EventEmitter<StreamHandler>();
		protected internal EventEmitter<StreamHandler> m_OnStreamElement = new EventEmitter<StreamHandler>();
		protected internal EventEmitter<StreamError> m_OnStreamError = new EventEmitter<StreamError>();

		// Stream Event Handlers
		public event StreamHandler OnStreamStart
		{
			add => this.m_OnStreamStart.Register(value);
			remove => this.m_OnStreamStart.Unregister(value);
		}

		public event StreamHandler OnStreamEnd
		{
			add => this.m_OnStreamEnd.Register(value);
			remove => this.m_OnStreamEnd.Unregister(value);
		}

		public event StreamHandler OnStreamElement
		{
			add => this.m_OnStreamElement.Register(value);
			remove => this.m_OnStreamElement.Unregister(value);
		}

		/// <summary>
		/// Event for XML-Stream errors
		/// </summary>
		public event StreamError OnStreamError
		{
			add => this.m_OnStreamError.Register(value);
			remove => this.m_OnStreamError.Unregister(value);
		}

		private int m_Depth = 0;
		private Node m_root = null;
		//private Node					current				= null;
		private Element current = null;

		private static System.Text.Encoding utf = System.Text.Encoding.UTF8;
		private Encoding m_enc = new UTF8Encoding();
		private BufferAggregate m_buf = new BufferAggregate();
		private NS m_ns = new NS();
		private bool m_cdata = false;

		public StreamParser()
		{
		}

		/// <summary>
		/// Reset the XML Stream
		/// </summary>
		public void Reset()
		{
			this.m_Depth = 0;
			this.m_root = null;
			this.current = null;
			this.m_cdata = false;

			this.m_buf = null;
			this.m_buf = new BufferAggregate();

			//m_buf.Clear(0);
			this.m_ns.Clear();
		}

		/// <summary>
		/// Reset the XML Stream
		/// </summary>
		/// <param name="sr">new Stream that is used for parsing</param>
		public long Depth
		{
			get { return this.m_Depth; }
		}

		private object thisLock = new object();

		/// <summary>
		/// Put bytes into the parser.
		/// </summary>
		/// <param name="buf">The bytes to put into the parse stream</param>
		/// <param name="offset">Offset into buf to start at</param>
		/// <param name="length">Number of bytes to write</param>
		public void Push(byte[] buf, int offset, int length)
		{

			// or assert, really, but this is a little nicer.
			if (length == 0)
				return;

			// No locking is required.  Read() won't get called again
			// until this method returns.

			// TODO: only do this copy if we have a partial token at the
			// end of parsing.
			var copy = new byte[length];
			Buffer.BlockCopy(buf, offset, copy, 0, length);
			this.m_buf.Write(copy);

			var b = this.m_buf.GetBuffer();
			var off = 0;
			var tok = TOK.END_TAG;
			var ct = new ContentToken();
			try
			{
				while (off < b.Length)
				{
					if (this.m_cdata)
						tok = this.m_enc.tokenizeCdataSection(b, off, b.Length, ct);
					else
						tok = this.m_enc.tokenizeContent(b, off, b.Length, ct);

					switch (tok)
					{
						case TOK.EMPTY_ELEMENT_NO_ATTS:
						case TOK.EMPTY_ELEMENT_WITH_ATTS:
							this.StartTag(b, off, ct, tok);
							this.EndTag(b, off, ct, tok);
							break;
						case TOK.START_TAG_NO_ATTS:
						case TOK.START_TAG_WITH_ATTS:
							this.StartTag(b, off, ct, tok);
							break;
						case TOK.END_TAG:
							this.EndTag(b, off, ct, tok);
							break;
						case TOK.DATA_CHARS:
						case TOK.DATA_NEWLINE:
							this.AddText(utf.GetString(b, off, ct.TokenEnd - off));
							break;
						case TOK.CHAR_REF:
						case TOK.MAGIC_ENTITY_REF:
							this.AddText(new string(new char[] { ct.RefChar1 }));
							break;
						case TOK.CHAR_PAIR_REF:
							this.AddText(new string(new char[] {ct.RefChar1,
															ct.RefChar2}));
							break;
						case TOK.COMMENT:
							if (this.current != null)
							{
								// <!-- 4
								//  --> 3
								var start = off + 4 * this.m_enc.MinBytesPerChar;
								var end = ct.TokenEnd - off -
									7 * this.m_enc.MinBytesPerChar;
								var text = utf.GetString(b, start, end);
								this.current.AddChild(new Comment(text));
							}
							break;
						case TOK.CDATA_SECT_OPEN:
							this.m_cdata = true;
							break;
						case TOK.CDATA_SECT_CLOSE:
							this.m_cdata = false;
							break;
						case TOK.XML_DECL:
							// thou shalt use UTF8, and XML version 1.
							// i shall ignore evidence to the contrary...

							// TODO: Throw an exception if these assuptions are
							// wrong
							break;
						case TOK.ENTITY_REF:
						case TOK.PI:
#if CF
					    throw new util.NotImplementedException("Token type not implemented: " + tok);
#else
							throw new NotImplementedException("Token type not implemented: " + tok);
#endif
					}
					off = ct.TokenEnd;
				}
			}
			catch (PartialTokenException)
			{
				// ignored;
			}
			catch (ExtensibleTokenException)
			{
				// ignored;
			}
			catch (Exception ex)
			{
				this.m_OnStreamError.Invoke(this, ex);
			}
			finally
			{
				this.m_buf.Clear(off);
			}
		}

		private void StartTag(byte[] buf, int offset,
			ContentToken ct, TOK tok)
		{
			this.m_Depth++;
			int colon;
			string name;
			string prefix;
			var ht = new Hashtable();

			this.m_ns.PushScope();

			// if i have attributes
			if ((tok == TOK.START_TAG_WITH_ATTS) ||
				(tok == TOK.EMPTY_ELEMENT_WITH_ATTS))
			{
				int start;
				int end;
				string val;
				for (var i = 0; i < ct.getAttributeSpecifiedCount(); i++)
				{
					start = ct.getAttributeNameStart(i);
					end = ct.getAttributeNameEnd(i);
					name = utf.GetString(buf, start, end - start);

					start = ct.getAttributeValueStart(i);
					end = ct.getAttributeValueEnd(i);
					//val = utf.GetString(buf, start, end - start);

					val = this.NormalizeAttributeValue(buf, start, end - start);
					// <foo b='&amp;'/>
					// <foo b='&amp;amp;'
					// TODO: if val includes &amp;, it gets double-escaped
					if (name.StartsWith("xmlns:"))
					{
						colon = name.IndexOf(':');
						prefix = name.Substring(colon + 1);
						this.m_ns.AddNamespace(prefix, val);
					}
					else if (name == "xmlns")
					{
						this.m_ns.AddNamespace(string.Empty, val);
					}
					else
					{
						ht.Add(name, val);
					}
				}
			}

			name = utf.GetString(buf,
				offset + this.m_enc.MinBytesPerChar,
				ct.NameEnd - offset - this.m_enc.MinBytesPerChar);

			colon = name.IndexOf(':');
			var ns = "";
			prefix = null;
			if (colon > 0)
			{
				prefix = name.Substring(0, colon);
				name = name.Substring(colon + 1);
				ns = this.m_ns.LookupNamespace(prefix);
			}
			else
			{
				ns = this.m_ns.DefaultNamespace;
			}

			var newel = ElementFactory.GetElement(prefix, name, ns);

			foreach (string attrname in ht.Keys)
			{
				newel.SetAttribute(attrname, (string)ht[attrname]);
			}

			if (this.m_root == null)
			{
				this.m_root = newel;
				this.m_OnStreamStart.Invoke(this, this.m_root);
			}
			else
			{
				if (this.current != null)
					this.current.AddChild(newel);
				this.current = newel;
			}
		}

		private void EndTag(byte[] buf, int offset, ContentToken ct, TOK tok)
		{
			this.m_Depth--;
			this.m_ns.PopScope();

			if (this.current == null)
			{
				this.m_OnStreamEnd.Invoke(this, this.m_root);
				return;
			}

			string name = null;

			if ((tok == TOK.EMPTY_ELEMENT_WITH_ATTS) ||
				(tok == TOK.EMPTY_ELEMENT_NO_ATTS))
				name = utf.GetString(buf,
					offset + this.m_enc.MinBytesPerChar,
					ct.NameEnd - offset -
					this.m_enc.MinBytesPerChar);
			else
				name = utf.GetString(buf,
					offset + this.m_enc.MinBytesPerChar * 2,
					ct.NameEnd - offset -
					this.m_enc.MinBytesPerChar * 2);


			//			if (current.Name != name)
			//				throw new Exception("Invalid end tag: " + name +
			//					" != " + current.Name);

			var parent = (Element)this.current.Parent;
			if (parent == null)
			{
				this.DoRaiseOnStreamElement(this.current);
				//if (OnStreamElement!=null)
				//    OnStreamElement(this, current);
				//FireOnElement(current);
			}
			this.current = parent;
		}

		/// <summary>
		/// If users didnt use the library correctly and had no local error handles
		/// it always crashed here and disconencted the socket.
		/// Catch this errors here now and foreward them.
		/// </summary>
		/// <param name="el"></param>
		internal void DoRaiseOnStreamElement(Element el)
		{
			try
			{
				this.m_OnStreamElement?.Invoke(this, this.current);
			}
			catch (Exception ex)
			{
				this.m_OnStreamError.Invoke(this, ex);
			}
		}

		private string NormalizeAttributeValue(byte[] buf, int offset, int length)
		{
			if (length == 0)
				return null;

			string val = null;
			var buffer = new BufferAggregate();
			var copy = new byte[length];
			Buffer.BlockCopy(buf, offset, copy, 0, length);
			buffer.Write(copy);
			var b = buffer.GetBuffer();
			var off = 0;
			var tok = TOK.END_TAG;
			var ct = new ContentToken();
			try
			{
				while (off < b.Length)
				{
					//tok = m_enc.tokenizeContent(b, off, b.Length, ct);
					tok = this.m_enc.tokenizeAttributeValue(b, off, b.Length, ct);

					switch (tok)
					{
						case TOK.ATTRIBUTE_VALUE_S:
						case TOK.DATA_CHARS:
						case TOK.DATA_NEWLINE:
							val += (utf.GetString(b, off, ct.TokenEnd - off));
							break;
						case TOK.CHAR_REF:
						case TOK.MAGIC_ENTITY_REF:
							val += new string(new char[] { ct.RefChar1 });
							break;
						case TOK.CHAR_PAIR_REF:
							val += new string(new char[] { ct.RefChar1, ct.RefChar2 });
							break;
						case TOK.ENTITY_REF:
#if CF
						    throw new util.NotImplementedException("Token type not implemented: " + tok);
#else
							throw new NotImplementedException("Token type not implemented: " + tok);
#endif
					}
					off = ct.TokenEnd;
				}
			}
			catch (PartialTokenException)
			{
				// ignored;
			}
			catch (ExtensibleTokenException)
			{
				// ignored;
			}
			catch (Exception ex)
			{
				this.m_OnStreamError.Invoke(this, ex);
			}
			finally
			{
				buffer.Clear(off);
			}
			return val;
		}

		private void AddText(string text)
		{
			if (text == "")
				return;

			//Console.WriteLine("AddText:" + text);
			//Console.WriteLine(lastTOK);

			if (this.current != null)
			{
				var last = this.current.LastNode;
				if (last != null && last.NodeType == NodeType.Text)
					last.Value += text;
				else
					this.current.AddChild(new Text(text));
			}
		}

	}
}