/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2019 by AG-Software, FRNathan13								 *
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using agsXMPP.IO;

namespace agsXMPP.Xml.Dom
{
	public enum NodeType
	{
		Document,   // xmlDocument
		Element,    // normal Element
		Text,       // Textnode
		Cdata,      // CDATA Section
		Comment,    // comment
		Declaration // processing instruction
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class Node : IFormattable
	{
		public Node Parent { get; internal set; }
		public NodeList ChildNodes { get; private set; }

		private NodeType m_NodeType;
		private string m_Value = null;
		private string m_Namespace = null;
		internal int m_Index = 0;

		public Node()
		{
			this.ChildNodes = new NodeList(this);
		}

		public NodeType NodeType
		{
			get { return this.m_NodeType; }
			set { this.m_NodeType = value; }
		}

		public virtual string Value
		{
			get { return this.m_Value; }
			set { this.m_Value = value; }
		}

		public string Namespace
		{
			get { return this.m_Namespace; }
			set { this.m_Namespace = value; }
		}

		public int Index
		{
			get { return this.m_Index; }
		}

		public void Remove()
		{
			if (this.Parent != null)
			{
				var index = this.m_Index;
				this.Parent.ChildNodes.RemoveAt(index);
				this.Parent.ChildNodes.RebuildIndex(index);
			}
		}

		public void RemoveAllChildNodes()
		{
			this.ChildNodes.Clear();
		}

		/// <summary>
		/// Appends the given Element as child element
		/// </summary>
		/// <param name="e"></param>
		public virtual void AddChild(Node e)
		{
			this.ChildNodes.Add(e);
		}

		/// <summary>
		/// Returns the Xml of the current Element (Node) as string
		/// </summary>
		public override string ToString()
		{
			return this.BuildXml(this, Formatting.None, 0, ' ');
		}

		public string ToString(Encoding enc)
		{
			if (this != null)
			{
				var tw = new StringWriterWithEncoding(enc);
				var w = new XmlTextWriter(tw);
				w.Formatting = Formatting.Indented;
				w.Indentation = 2;

				this.WriteTree(this, w, null);

				return tw.ToString();
			}
			else
			{
				return "";
			}
		}

		/// <summary>
		/// returns the Xml, difference to the Xml property is that you can set formatting porperties
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public string ToString(Formatting format)
		{
			return this.BuildXml(this, format, 3, ' ');
		}

		/// <summary>
		/// returns the Xml, difference to the Xml property is that you can set formatting properties
		/// </summary>
		/// <param name="format"></param>
		/// <param name="indent"></param>
		/// <returns></returns>
		public string ToString(Formatting format, int indent)
		{
			return this.BuildXml(this, format, indent, ' ');
		}

		#region << Xml Serializer Functions >>

		private string BuildXml(Node e, Formatting format, int indent, char indentchar)
		{
			if (e == null)
				return string.Empty;
			else
			{
				var tw = new StringWriter();
				var w = new XmlTextWriter(tw);
				w.Formatting = format;
				w.Indentation = indent;
				w.IndentChar = indentchar;

				this.WriteTree(this, w, null);

				return tw.ToString();
			}
		}

		private void WriteTree(Node e, XmlTextWriter tw, Node parent)
		{
			if (e.NodeType == NodeType.Document)
			{
				var doc = e as Document;
				string decl = string.Empty;

				if (doc.Version != null)
					decl += "version='" + doc.Version + "'";

				if (doc.Encoding != null)
				{
					if (decl != null)
						decl += " ";

					decl += "encoding='" + doc.Encoding + "'";
				}

				if (decl != null)
					tw.WriteProcessingInstruction("xml", decl);

				foreach (var n in e.ChildNodes.GetNodes())
				{
					this.WriteTree(n, tw, e);
				}
			}
			else if (e.NodeType == NodeType.Text)
			{
				tw.WriteString(e.Value);
			}
			else if (e.NodeType == NodeType.Comment)
			{
				tw.WriteComment(e.Value);
			}
			else if (e.NodeType == NodeType.Element)
			{
				var el = e as Element;

				if (el.Prefix == null)
					tw.WriteStartElement(el.TagName);
				else
					tw.WriteStartElement(el.Prefix + ":" + el.TagName);

				// Write Namespace
				if ((parent == null || parent.Namespace != el.Namespace) && el.Namespace != null && el.Namespace.Length != 0)
				{
					if (el.Prefix == null)
						tw.WriteAttributeString("xmlns", el.Namespace);
					else
						tw.WriteAttributeString("xmlns:" + el.Prefix, el.Namespace);
				}

				foreach (var att in el.GetAttributes().Select(x => x.Key))
					tw.WriteAttributeString(att, el.Attribute(att));

				if (el.ChildNodes.Count > 0)
				{
					foreach (var n in el.ChildNodes.GetNodes())
						this.WriteTree(n, tw, e);
				}

				tw.WriteEndElement();
			}
		}

		public string ToString(string format, IFormatProvider provider)
		{
			switch (format.ToUpperInvariant())
			{
				case "I":
					return this.ToString(Formatting.Indented);

				default:
					return this.ToString(Formatting.None);
			}
		}

		#endregion

	}
}