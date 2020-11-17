using System;
using AgsXMPP.Xml.Xpnet.Exceptions;

namespace AgsXMPP.Xml.Xpnet.Encoding
{
	public abstract partial class XmlEncoding
	{
		static XmlEncoding()
		{
			CharTypeTable = new int[256][];

			foreach (var c in NameSingles)
				SetCharType(c, BT_NAME);
			for (var i = 0; i < NameRanges.Length; i += 2)
				SetCharType(NameRanges[i], NameRanges[i + 1], BT_NAME);
			for (var i = 0; i < NameStartSingles.Length; i++)
				SetCharType(NameStartSingles[i], BT_NMSTRT);
			for (var i = 0; i < NameStartRanges.Length; i += 2)
				SetCharType(NameStartRanges[i], NameStartRanges[i + 1],
							BT_NMSTRT);
			SetCharType('\uD800', '\uDBFF', BT_LEAD4);
			SetCharType('\uDC00', '\uDFFF', BT_MALFORM);
			SetCharType('\uFFFE', '\uFFFF', BT_NONXML);

			var other = new int[256];

			for (var i = 0; i < 256; i++)
				other[i] = BT_OTHER;

			for (var i = 0; i < 256; i++)
				if (CharTypeTable[i] == null)
					CharTypeTable[i] = other;

			Array.Copy(AsciiTypeTable, 0, CharTypeTable[0], 0, 128);
		}

		protected abstract int Convert(byte[] sourceBuf,
									   int sourceStart, int sourceEnd,
									   char[] targetBuf, int targetStart);

		protected const byte UTF8_ENCODING = 0;

		public static Utf8XmlEncoding UTF8 { get; } = new Utf8XmlEncoding();

		[Obsolete("Use UTF8 static instead.")]
		protected static XmlEncoding GetEncoding(byte enc)
		{
			return enc switch
			{
				UTF8_ENCODING => UTF8,
				_ => null,
			};
		}

		/// <summary>
		/// Returns the minimum number of bytes required to represent a single
		/// character in this encoding. The value will be 1, 2 or 4.
		/// </summary>
		public int MinBytesPerCharacter { get; }

		/// <summary>
		/// Constructor called by subclasses.
		/// </summary>
		/// <param name="minBytesPerCharacter">The minimum bytes per character.</param>
		protected XmlEncoding(int minBytesPerCharacter)
		{
			this.MinBytesPerCharacter = minBytesPerCharacter;
		}

		/// <summary>
		/// Get the byte type of the next byte.  There are guaranteed to be minBPC available bytes starting at off.
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <returns></returns>
		protected abstract int GetByteType(byte[] buf, int off);

		/// <summary>
		/// Really only works for ASCII7.
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <returns></returns>
		protected abstract char BytesToAscii(byte[] buf, int off);

		/// <summary>
		/// This must only be called when c is an (XML significant)
		/// ASCII character.
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		protected abstract bool IsCharMatches(byte[] buf, int off, char c);

		/// <summary>
		/// Called only when byteType(buf, off) == BT_LEAD2
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <returns></returns>
		protected virtual int GetByteType2(byte[] buf, int off)
		{
			return BT_OTHER;
		}

		/// <summary>
		/// Called only when byteType(buf, off) == BT_LEAD3
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <returns></returns>
		protected virtual int GetByteType3(byte[] buff, int off)
		{
			return BT_OTHER;
		}

		/// <summary>
		/// Called only when byteType(buf, off) == BT_LEAD4
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <returns></returns>
		protected virtual int GetByteType4(byte[] buff, int off)
		{
			return BT_OTHER;
		}

		protected virtual void Check2(byte[] buf, int off) { }
		protected virtual void Check3(byte[] buf, int off) { }
		protected virtual void Check4(byte[] buf, int off) { }

		/**
         * Moves a position forward.  On entry, <code>pos</code> gives
         * the position of the byte at index <code>off</code> in
         * <code>buf</code>.  On exit, it <code>pos</code> will give
         * the position of the byte at index <code>end</code>, which
         * must be greater than or equal to <code>off</code>.  The
         * bytes between <code>off</code> and <code>end</code> must
         * encode one or more complete characters.  A carriage return
         * followed by a line feed will be treated as a single line
         * delimiter provided that they are given to
         * <code>movePosition</code> together.
         */
		protected abstract void MovePosition(byte[] buf, int off, int end, Position pos);

		protected void CheckIfCharMatches(byte[] buf, int off, char c)
		{
			if (!this.IsCharMatches(buf, off, c))
				throw new InvalidTokenException(off);
		}

		/* off points to character following "<!-" */
		protected TokenType ScanComment(byte[] buf, int off, int end, Token token)
		{
			if (off != end)
			{
				this.CheckIfCharMatches(buf, off, '-');
				off += this.MinBytesPerCharacter;
				while (off != end)
				{
					switch (this.GetByteType(buf, off))
					{
						case BT_LEAD2:
							if (end - off < 2)
								throw new PartialCharException(off);
							this.Check2(buf, off);
							off += 2;
							break;
						case BT_LEAD3:
							if (end - off < 3)
								throw new PartialCharException(off);
							this.Check3(buf, off);
							off += 3;
							break;
						case BT_LEAD4:
							if (end - off < 4)
								throw new PartialCharException(off);
							this.Check4(buf, off);
							off += 4;
							break;
						case BT_NONXML:
						case BT_MALFORM:
							throw new InvalidTokenException(off);
						case BT_MINUS:
							if ((off += this.MinBytesPerCharacter) == end)
								throw new PartialTokenException();
							if (this.IsCharMatches(buf, off, '-'))
							{
								if ((off += this.MinBytesPerCharacter) == end)
									throw new PartialTokenException();
								this.CheckIfCharMatches(buf, off, '>');
								token.TokenEnd = off + this.MinBytesPerCharacter;
								return TokenType.COMMENT;
							}
							break;
						default:
							off += this.MinBytesPerCharacter;
							break;
					}
				}
			}
			throw new PartialTokenException();
		}

		/* off points to character following "<!" */
		protected TokenType ScanDecl(byte[] buf, int off, int end, Token token)
		{
			if (off == end)
				throw new PartialTokenException();
			switch (this.GetByteType(buf, off))
			{
				case BT_MINUS:
					return this.ScanComment(buf, off + this.MinBytesPerCharacter, end, token);
				case BT_LSQB:
					token.TokenEnd = off + this.MinBytesPerCharacter;
					return TokenType.COND_SECT_OPEN;
				case BT_NMSTRT:
					off += this.MinBytesPerCharacter;
					break;
				default:
					throw new InvalidTokenException(off);
			}
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_PERCNT:
						if (off + this.MinBytesPerCharacter == end)
							throw new PartialTokenException();
						/* don't allow <!ENTITY% foo "whatever"> */
						switch (this.GetByteType(buf, off + this.MinBytesPerCharacter))
						{
							case BT_S:
							case BT_CR:
							case BT_LF:
							case BT_PERCNT:
								throw new InvalidTokenException(off);
						}
						/* fall through */
						goto case BT_S;
					case BT_S:
					case BT_CR:
					case BT_LF:
						token.TokenEnd = off;
						return TokenType.DECL_OPEN;
					case BT_NMSTRT:
						off += this.MinBytesPerCharacter;
						break;
					default:
						throw new InvalidTokenException(off);
				}
			}
			throw new PartialTokenException();
		}

		protected bool TargetIsXml(byte[] buf, int off, int end)
		{
			var upper = false;
			if (end - off != this.MinBytesPerCharacter * 3)
				return false;
			switch (this.BytesToAscii(buf, off))
			{
				case 'x':
					break;
				case 'X':
					upper = true;
					break;
				default:
					return false;
			}
			off += this.MinBytesPerCharacter;
			switch (this.BytesToAscii(buf, off))
			{
				case 'm':
					break;
				case 'M':
					upper = true;
					break;
				default:
					return false;
			}
			off += this.MinBytesPerCharacter;
			switch (this.BytesToAscii(buf, off))
			{
				case 'l':
					break;
				case 'L':
					upper = true;
					break;
				default:
					return false;
			}
			if (upper)
				throw new InvalidTokenException(off, InvalidTokenType.XmlTarget);
			return true;
		}

		/* off points to character following "<?" */

		protected TokenType ScanPi(byte[] buf, int off, int end, Token token)
		{
			var target = off;
			if (off == end)
				throw new PartialTokenException();
			switch (this.GetByteType(buf, off))
			{
				case BT_NMSTRT:
					off += this.MinBytesPerCharacter;
					break;

				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (this.GetByteType2(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 2;
					break;

				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (this.GetByteType3(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 3;
					break;

				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (this.GetByteType4(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 4;
					break;

				default:
					throw new InvalidTokenException(off);
			}
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_NMSTRT:
					case BT_NAME:
					case BT_MINUS:
						off += this.MinBytesPerCharacter;
						break;
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						if (!this.IsNameChar2(buf, off))
							throw new InvalidTokenException(off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						if (!this.IsNameChar3(buf, off))
							throw new InvalidTokenException(off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						if (!this.IsNameChar4(buf, off))
							throw new InvalidTokenException(off);
						off += 4;
						break;
					case BT_S:
					case BT_CR:
					case BT_LF:
						var isXml = this.TargetIsXml(buf, target, off);
						token.NameEnd = off;
						off += this.MinBytesPerCharacter;
						while (off != end)
						{
							switch (this.GetByteType(buf, off))
							{
								case BT_LEAD2:
									if (end - off < 2)
										throw new PartialCharException(off);
									this.Check2(buf, off);
									off += 2;
									break;
								case BT_LEAD3:
									if (end - off < 3)
										throw new PartialCharException(off);
									this.Check3(buf, off);
									off += 3;
									break;
								case BT_LEAD4:
									if (end - off < 4)
										throw new PartialCharException(off);
									this.Check4(buf, off);
									off += 4;
									break;
								case BT_NONXML:
								case BT_MALFORM:
									throw new InvalidTokenException(off);
								case BT_QUEST:
									off += this.MinBytesPerCharacter;
									if (off == end)
										throw new PartialTokenException();
									if (this.IsCharMatches(buf, off, '>'))
									{
										token.TokenEnd = off + this.MinBytesPerCharacter;
										if (isXml)
											return TokenType.XML_DECL;
										else
											return TokenType.PI;
									}
									break;
								default:
									off += this.MinBytesPerCharacter;
									break;
							}
						}
						throw new PartialTokenException();
					case BT_QUEST:
						token.NameEnd = off;
						off += this.MinBytesPerCharacter;
						if (off == end)
							throw new PartialTokenException();
						this.CheckIfCharMatches(buf, off, '>');
						token.TokenEnd = off + this.MinBytesPerCharacter;
						return this.TargetIsXml(buf, target, token.NameEnd)
								? TokenType.XML_DECL
								: TokenType.PI;
					default:
						throw new InvalidTokenException(off);
				}
			}
			throw new PartialTokenException();
		}

		/* off points to character following "<![" */
		const string CDATA = "CDATA[";

		protected TokenType ScanCdataSection(byte[] buf, int off, int end, Token token)
		{
			/* "CDATA[".length() == 6 */
			if (end - off < 6 * this.MinBytesPerCharacter)
				throw new PartialTokenException();

			for (var i = 0; i < CDATA.Length; i++, off += this.MinBytesPerCharacter)
				this.CheckIfCharMatches(buf, off, CDATA[i]);

			token.TokenEnd = off;
			return TokenType.CDATA_SECT_OPEN;
		}

		/// <summary>
		/// Scans the first token of a byte subarrary that starts with the
		/// content of a CDATA section.
		/// Returns one of the following integers according to the type of token
		/// that the subarray starts with:
		/// <ul>
		/// <li><code>TOK.DATA_CHARS</code></li>
		/// <li><code>TOK.DATA_NEWLINE</code></li>
		/// <li><code>TOK.CDATA_SECT_CLOSE</code></li>
		/// </ul>
		/// <p>
		/// Information about the token is stored in <code>token</code>.
		/// </p>
		/// After <b>TOK.CDATA_SECT_CLOSE</b> is returned, the application
		/// should use <see cref="TokenizeContent(byte[], int, int, ContentToken)"/>.
		/// </summary>
		/// <exception cref="EmptyTokenException">If the subarray is empty</exception>
		/// <exception cref="PartialTokenException">If the subarray contains only part of a legal token</exception>
		/// <exception cref="InvalidTokenException">If the subarrary does not start with a legal token or part of one</exception>
		/// <exception cref="ExtensibleTokenException">If the subarray encodes just a carriage</exception>
		/// return ('\r')
		public TokenType TokenizeCdataSection(byte[] buf, int off, int end,
										Token token)
		{
			if (this.MinBytesPerCharacter > 1)
				end = this.AdjustEnd(off, end);
			if (off == end)
				throw new EmptyTokenException();
			switch (this.GetByteType(buf, off))
			{
				case BT_RSQB:
					off += this.MinBytesPerCharacter;
					if (off == end)
						throw new PartialTokenException();
					if (!this.IsCharMatches(buf, off, ']'))
						break;
					off += this.MinBytesPerCharacter;
					if (off == end)
						throw new PartialTokenException();
					if (!this.IsCharMatches(buf, off, '>'))
					{
						off -= this.MinBytesPerCharacter;
						break;
					}
					token.TokenEnd = off + this.MinBytesPerCharacter;
					return TokenType.CDATA_SECT_CLOSE;
				case BT_CR:
					off += this.MinBytesPerCharacter;
					if (off == end)
						throw new ExtensibleTokenException(TokenType.DATA_NEWLINE);
					if (this.GetByteType(buf, off) == BT_LF)
						off += this.MinBytesPerCharacter;
					token.TokenEnd = off;
					return TokenType.DATA_NEWLINE;
				case BT_LF:
					token.TokenEnd = off + this.MinBytesPerCharacter;
					return TokenType.DATA_NEWLINE;
				case BT_NONXML:
				case BT_MALFORM:
					throw new InvalidTokenException(off);
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					this.Check2(buf, off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					this.Check3(buf, off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					this.Check4(buf, off);
					off += 4;
					break;
				default:
					off += this.MinBytesPerCharacter;
					break;
			}
			token.TokenEnd = this.ExtendCdata(buf, off, end);
			return TokenType.DATA_CHARS;
		}

		protected int ExtendCdata(byte[] buf, int off, int end)
		{
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_LEAD2:
						if (end - off < 2)
							return off;
						this.Check2(buf, off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							return off;
						this.Check3(buf, off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							return off;
						this.Check4(buf, off);
						off += 4;
						break;
					case BT_RSQB:
					case BT_NONXML:
					case BT_MALFORM:
					case BT_CR:
					case BT_LF:
						return off;
					default:
						off += this.MinBytesPerCharacter;
						break;
				}
			}
			return off;
		}


		/* off points to character following "</" */
		protected TokenType ScanEndTag(byte[] buf, int off, int end, Token token)
		{
			if (off == end)
				throw new PartialTokenException();
			switch (this.GetByteType(buf, off))
			{
				case BT_NMSTRT:
					off += this.MinBytesPerCharacter;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (this.GetByteType2(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (this.GetByteType3(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (this.GetByteType4(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 4;
					break;
				default:
					throw new InvalidTokenException(off);
			}
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_NMSTRT:
					case BT_NAME:
					case BT_MINUS:
						off += this.MinBytesPerCharacter;
						break;
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						if (!this.IsNameChar2(buf, off))
							throw new InvalidTokenException(off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						if (!this.IsNameChar3(buf, off))
							throw new InvalidTokenException(off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						if (!this.IsNameChar4(buf, off))
							throw new InvalidTokenException(off);
						off += 4;
						break;
					case BT_S:
					case BT_CR:
					case BT_LF:
						token.NameEnd = off;
						for (off += this.MinBytesPerCharacter; off != end; off += this.MinBytesPerCharacter)
						{
							switch (this.GetByteType(buf, off))
							{
								case BT_S:
								case BT_CR:
								case BT_LF:
									break;
								case BT_GT:
									token.TokenEnd = off + this.MinBytesPerCharacter;
									return TokenType.END_TAG;
								default:
									throw new InvalidTokenException(off);
							}
						}
						throw new PartialTokenException();
					case BT_GT:
						token.NameEnd = off;
						token.TokenEnd = off + this.MinBytesPerCharacter;
						return TokenType.END_TAG;
					default:
						throw new InvalidTokenException(off);
				}
			}
			throw new PartialTokenException();
		}

		protected TokenType ScanHexCharRef(byte[] buf, int off, int end, Token token)
		{
			if (off != end)
			{
				int c = this.BytesToAscii(buf, off);
				int num;
				switch (c)
				{
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						num = c - '0';
						break;
					case 'A':
					case 'B':
					case 'C':
					case 'D':
					case 'E':
					case 'F':
						num = c - ('A' - 10);
						break;
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
						num = c - ('a' - 10);
						break;
					default:
						throw new InvalidTokenException(off);
				}
				for (off += this.MinBytesPerCharacter; off != end; off += this.MinBytesPerCharacter)
				{
					c = this.BytesToAscii(buf, off);
					switch (c)
					{
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							num = (num << 4) + c - '0';
							break;
						case 'A':
						case 'B':
						case 'C':
						case 'D':
						case 'E':
						case 'F':
							num = (num << 4) + c - ('A' - 10);
							break;
						case 'a':
						case 'b':
						case 'c':
						case 'd':
						case 'e':
						case 'f':
							num = (num << 4) + c - ('a' - 10);
							break;
						case ';':
							token.TokenEnd = off + this.MinBytesPerCharacter;
							return this.SetRefChar(num, token);
						default:
							throw new InvalidTokenException(off);
					}
					if (num >= 0x110000)
						throw new InvalidTokenException(off);
				}
			}
			throw new PartialTokenException();
		}

		/* off points to character following "&#" */
		protected TokenType ScanCharRef(byte[] buf, int off, int end, Token token)
		{
			if (off != end)
			{
				int c = this.BytesToAscii(buf, off);
				switch (c)
				{
					case 'x':
						return this.ScanHexCharRef(buf, off + this.MinBytesPerCharacter, end, token);
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						break;
					default:
						throw new InvalidTokenException(off);
				}

				var num = c - '0';
				for (off += this.MinBytesPerCharacter; off != end; off += this.MinBytesPerCharacter)
				{
					c = this.BytesToAscii(buf, off);
					switch (c)
					{
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							num = num * 10 + (c - '0');
							if (num < 0x110000)
								break;
							/* fall through */
							goto default;
						default:
							throw new InvalidTokenException(off);
						case ';':
							token.TokenEnd = off + this.MinBytesPerCharacter;
							return this.SetRefChar(num, token);
					}
				}
			}
			throw new PartialTokenException();
		}

		/* num is known to be < 0x110000; return the token code */
		protected TokenType SetRefChar(int num, Token token)
		{
			if (num < 0x10000)
			{
				switch (CharTypeTable[num >> 8][num & 0xFF])
				{
					case BT_NONXML:
					case BT_LEAD4:
					case BT_MALFORM:
						throw new InvalidTokenException(token.TokenEnd - this.MinBytesPerCharacter);
				}
				token.RefChar1 = (char)num;
				return TokenType.CHAR_REF;
			}
			else
			{
				num -= 0x10000;
				token.RefChar1 = (char)((num >> 10) + 0xD800);
				token.RefChar2 = (char)((num & (1 << 10) - 1) + 0xDC00);
				return TokenType.CHAR_PAIR_REF;
			}
		}

		protected bool IsMagicEntityRef(byte[] buf, int off, int end, Token token)
		{
			switch (this.BytesToAscii(buf, off))
			{
				case 'a':
					if (end - off < this.MinBytesPerCharacter * 4)
						break;
					switch (this.BytesToAscii(buf, off + this.MinBytesPerCharacter))
					{
						case 'm':
							if (this.IsCharMatches(buf, off + this.MinBytesPerCharacter * 2, 'p')
								&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter * 3, ';'))
							{
								token.TokenEnd = off + this.MinBytesPerCharacter * 4;
								token.RefChar1 = '&';
								return true;
							}
							break;
						case 'p':
							if (end - off >= this.MinBytesPerCharacter * 5
								&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter * 2, 'o')
								&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter * 3, 's')
								&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter * 4, ';'))
							{
								token.TokenEnd = off + this.MinBytesPerCharacter * 5;
								token.RefChar1 = '\'';
								return true;
							}
							break;
					}
					break;
				case 'l':
					if (end - off >= this.MinBytesPerCharacter * 3
						&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter, 't')
						&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter * 2, ';'))
					{
						token.TokenEnd = off + this.MinBytesPerCharacter * 3;
						token.RefChar1 = '<';
						return true;
					}
					break;
				case 'g':
					if (end - off >= this.MinBytesPerCharacter * 3
						&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter, 't')
						&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter * 2, ';'))
					{
						token.TokenEnd = off + this.MinBytesPerCharacter * 3;
						token.RefChar1 = '>';
						return true;
					}
					break;
				case 'q':
					if (end - off >= this.MinBytesPerCharacter * 5
						&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter, 'u')
						&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter * 2, 'o')
						&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter * 3, 't')
						&& this.IsCharMatches(buf, off + this.MinBytesPerCharacter * 4, ';'))
					{
						token.TokenEnd = off + this.MinBytesPerCharacter * 5;
						token.RefChar1 = '"';
						return true;
					}
					break;
			}
			return false;
		}

		/* off points to character following "&" */
		protected TokenType ScanRef(byte[] buf, int off, int end, Token token)
		{
			if (off == end)
				throw new PartialTokenException();
			if (this.IsMagicEntityRef(buf, off, end, token))
				return TokenType.MAGIC_ENTITY_REF;
			switch (this.GetByteType(buf, off))
			{
				case BT_NMSTRT:
					off += this.MinBytesPerCharacter;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (this.GetByteType2(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (this.GetByteType3(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (this.GetByteType4(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_NUM:
					return this.ScanCharRef(buf, off + this.MinBytesPerCharacter, end, token);
				default:
					throw new InvalidTokenException(off);
			}
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_NMSTRT:
					case BT_NAME:
					case BT_MINUS:
						off += this.MinBytesPerCharacter;
						break;
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						if (!this.IsNameChar2(buf, off))
							throw new InvalidTokenException(off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						if (!this.IsNameChar3(buf, off))
							throw new InvalidTokenException(off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						if (!this.IsNameChar4(buf, off))
							throw new InvalidTokenException(off);
						off += 4;
						break;
					case BT_SEMI:
						token.NameEnd = off;
						token.TokenEnd = off + this.MinBytesPerCharacter;
						return TokenType.ENTITY_REF;
					default:
						throw new InvalidTokenException(off);
				}
			}
			throw new PartialTokenException();
		}

		/* off points to character following first character of
           attribute name */
		protected TokenType ScanAtts(int nameStart, byte[] buf, int off, int end,
							 ContentToken token)
		{
			var NameEnd = -1;
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_NMSTRT:
					case BT_NAME:
					case BT_MINUS:
						off += this.MinBytesPerCharacter;
						break;
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						if (!this.IsNameChar2(buf, off))
							throw new InvalidTokenException(off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						if (!this.IsNameChar3(buf, off))
							throw new InvalidTokenException(off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						if (!this.IsNameChar4(buf, off))
							throw new InvalidTokenException(off);
						off += 4;
						break;
					case BT_S:
					case BT_CR:
					case BT_LF:
						NameEnd = off;
						for (; ; )
						{
							off += this.MinBytesPerCharacter;
							if (off == end)
								throw new PartialTokenException();
							switch (this.GetByteType(buf, off))
							{
								case BT_EQUALS:
									goto loop;
								case BT_S:
								case BT_LF:
								case BT_CR:
									break;
								default:
									throw new InvalidTokenException(off);
							}
						}
					loop:;
						/* fall through */
						goto case BT_EQUALS;
					case BT_EQUALS:
						{
							if (NameEnd < 0)
								NameEnd = off;
							int open;
							for (; ; )
							{

								off += this.MinBytesPerCharacter;
								if (off == end)
									throw new PartialTokenException();
								open = this.GetByteType(buf, off);
								if (open == BT_QUOT || open == BT_APOS)
									break;
								switch (open)
								{
									case BT_S:
									case BT_LF:
									case BT_CR:
										break;
									default:
										throw new InvalidTokenException(off);
								}
							}
							off += this.MinBytesPerCharacter;
							var valueStart = off;
							var normalized = true;
							int t;
							/* in attribute value */
							for (; ; )
							{
								if (off == end)
									throw new PartialTokenException();
								t = this.GetByteType(buf, off);
								if (t == open)
									break;
								switch (t)
								{
									case BT_NONXML:
									case BT_MALFORM:
										throw new InvalidTokenException(off);
									case BT_LEAD2:
										if (end - off < 2)
											throw new PartialCharException(off);
										this.Check2(buf, off);
										off += 2;
										break;
									case BT_LEAD3:
										if (end - off < 3)
											throw new PartialCharException(off);
										this.Check3(buf, off);
										off += 3;
										break;
									case BT_LEAD4:
										if (end - off < 4)
											throw new PartialCharException(off);
										this.Check4(buf, off);
										off += 4;
										break;
									case BT_AMP:
										{
											normalized = false;
											var saveNameEnd = token.NameEnd;
											this.ScanRef(buf, off + this.MinBytesPerCharacter, end, token);
											token.NameEnd = saveNameEnd;
											off = token.TokenEnd;
											break;
										}
									case BT_S:
										if (normalized
											&& (off == valueStart
												|| this.BytesToAscii(buf, off) != ' '
												|| off + this.MinBytesPerCharacter != end
													&& (this.BytesToAscii(buf, off + this.MinBytesPerCharacter) == ' '
														|| this.GetByteType(buf, off + this.MinBytesPerCharacter) == open)))
											normalized = false;
										off += this.MinBytesPerCharacter;
										break;
									case BT_LT:
										throw new InvalidTokenException(off);
									case BT_LF:
									case BT_CR:
										normalized = false;
										/* fall through */
										goto default;
									default:
										off += this.MinBytesPerCharacter;
										break;
								}
							}
							token.AppendAttribute(nameStart, NameEnd, valueStart,
												  off,
												  normalized);
							off += this.MinBytesPerCharacter;
							if (off == end)
								throw new PartialTokenException();
							t = this.GetByteType(buf, off);
							switch (t)
							{
								case BT_S:
								case BT_CR:
								case BT_LF:
									off += this.MinBytesPerCharacter;
									if (off == end)
										throw new PartialTokenException();
									t = this.GetByteType(buf, off);
									break;
								case BT_GT:
								case BT_SOL:
									break;
								default:
									throw new InvalidTokenException(off);
							}
							/* off points to closing quote */
							for (; ; )
							{
								switch (t)
								{
									case BT_NMSTRT:
										nameStart = off;
										off += this.MinBytesPerCharacter;
										goto skipToName;
									case BT_LEAD2:
										if (end - off < 2)
											throw new PartialCharException(off);
										if (this.GetByteType2(buf, off) != BT_NMSTRT)
											throw new InvalidTokenException(off);
										nameStart = off;
										off += 2;
										goto skipToName;
									case BT_LEAD3:
										if (end - off < 3)
											throw new PartialCharException(off);
										if (this.GetByteType3(buf, off) != BT_NMSTRT)
											throw new InvalidTokenException(off);
										nameStart = off;
										off += 3;
										goto skipToName;
									case BT_LEAD4:
										if (end - off < 4)
											throw new PartialCharException(off);
										if (this.GetByteType4(buf, off) != BT_NMSTRT)
											throw new InvalidTokenException(off);
										nameStart = off;
										off += 4;
										goto skipToName;
									case BT_S:
									case BT_CR:
									case BT_LF:
										break;
									case BT_GT:
										token.CheckAttributeUniqueness(buf);
										token.TokenEnd = off + this.MinBytesPerCharacter;
										return TokenType.START_TAG_WITH_ATTS;
									case BT_SOL:
										off += this.MinBytesPerCharacter;
										if (off == end)
											throw new PartialTokenException();
										this.CheckIfCharMatches(buf, off, '>');
										token.CheckAttributeUniqueness(buf);
										token.TokenEnd = off + this.MinBytesPerCharacter;
										return TokenType.EMPTY_ELEMENT_WITH_ATTS;
									default:
										throw new InvalidTokenException(off);
								}
								off += this.MinBytesPerCharacter;
								if (off == end)
									throw new PartialTokenException();
								t = this.GetByteType(buf, off);
							}

						skipToName:
							NameEnd = -1;
							break;
						}
					default:
						throw new InvalidTokenException(off);
				}
			}
			throw new PartialTokenException();
		}

		/* off points to character following "<" */
		protected TokenType ScanLt(byte[] buf, int off, int end, ContentToken token)
		{
			if (off == end)
				throw new PartialTokenException();
			switch (this.GetByteType(buf, off))
			{
				case BT_NMSTRT:
					off += this.MinBytesPerCharacter;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (this.GetByteType2(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (this.GetByteType3(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (this.GetByteType4(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_EXCL:
					if ((off += this.MinBytesPerCharacter) == end)
						throw new PartialTokenException();

					return (this.GetByteType(buf, off)) switch
					{
						BT_MINUS => this.ScanComment(buf, off + this.MinBytesPerCharacter, end, token),
						BT_LSQB => this.ScanCdataSection(buf, off + this.MinBytesPerCharacter, end, token),
						_ => throw new InvalidTokenException(off),
					};
				case BT_QUEST:
					return this.ScanPi(buf, off + this.MinBytesPerCharacter, end, token);
				case BT_SOL:
					return this.ScanEndTag(buf, off + this.MinBytesPerCharacter, end, token);
				default:
					throw new InvalidTokenException(off);
			}
			/* we have a start-tag */
			token.NameEnd = -1;
			token.ClearAttributes();
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_NMSTRT:
					case BT_NAME:
					case BT_MINUS:
						off += this.MinBytesPerCharacter;
						break;
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						if (!this.IsNameChar2(buf, off))
							throw new InvalidTokenException(off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						if (!this.IsNameChar3(buf, off))
							throw new InvalidTokenException(off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						if (!this.IsNameChar4(buf, off))
							throw new InvalidTokenException(off);
						off += 4;
						break;
					case BT_S:
					case BT_CR:
					case BT_LF:
						token.NameEnd = off;
						off += this.MinBytesPerCharacter;
						for (; ; )
						{
							if (off == end)
								throw new PartialTokenException();
							switch (this.GetByteType(buf, off))
							{
								case BT_NMSTRT:
									return this.ScanAtts(off, buf, off + this.MinBytesPerCharacter, end, token);
								case BT_LEAD2:
									if (end - off < 2)
										throw new PartialCharException(off);
									if (this.GetByteType2(buf, off) != BT_NMSTRT)
										throw new InvalidTokenException(off);
									return this.ScanAtts(off, buf, off + 2, end, token);
								case BT_LEAD3:
									if (end - off < 3)
										throw new PartialCharException(off);
									if (this.GetByteType3(buf, off) != BT_NMSTRT)
										throw new InvalidTokenException(off);
									return this.ScanAtts(off, buf, off + 3, end, token);
								case BT_LEAD4:
									if (end - off < 4)
										throw new PartialCharException(off);
									if (this.GetByteType4(buf, off) != BT_NMSTRT)
										throw new InvalidTokenException(off);
									return this.ScanAtts(off, buf, off + 4, end, token);
								case BT_GT:
								case BT_SOL:
									goto loop;
								case BT_S:
								case BT_CR:
								case BT_LF:
									off += this.MinBytesPerCharacter;
									break;
								default:
									throw new InvalidTokenException(off);
							}
						}
					loop:
						break;
					case BT_GT:
						if (token.NameEnd < 0)
							token.NameEnd = off;
						token.TokenEnd = off + this.MinBytesPerCharacter;
						return TokenType.START_TAG_NO_ATTS;
					case BT_SOL:
						if (token.NameEnd < 0)
							token.NameEnd = off;
						off += this.MinBytesPerCharacter;
						if (off == end)
							throw new PartialTokenException();
						this.CheckIfCharMatches(buf, off, '>');
						token.TokenEnd = off + this.MinBytesPerCharacter;
						return TokenType.EMPTY_ELEMENT_NO_ATTS;
					default:
						throw new InvalidTokenException(off);
				}
			}
			throw new PartialTokenException();
		}

		// Ensure that we always scan a multiple of minBPC bytes.
		protected int AdjustEnd(int off, int end)
		{
			var n = end - off;
			if ((n & this.MinBytesPerCharacter - 1) != 0)
			{
				n &= ~(this.MinBytesPerCharacter - 1);
				if (n == 0)
					throw new PartialCharException(off);
				return off + n;
			}
			else
				return end;
		}

		/**
         * Scans the first token of a byte subarrary that contains content.
         * Returns one of the following integers according to the type of token
         * that the subarray starts with:
         * <ul>
         * <li><code>TOK.START_TAG_NO_ATTS</code></li>
         * <li><code>TOK.START_TAG_WITH_ATTS</code></li>
         * <li><code>TOK.EMPTY_ELEMENT_NO_ATTS</code></li>
         * <li><code>TOK.EMPTY_ELEMENT_WITH_ATTS</code></li>
         * <li><code>TOK.END_TAG</code></li>
         * <li><code>TOK.DATA_CHARS</code></li>
         * <li><code>TOK.DATA_NEWLINE</code></li>
         * <li><code>TOK.CDATA_SECT_OPEN</code></li>
         * <li><code>TOK.ENTITY_REF</code></li>
         * <li><code>TOK.MAGIC_ENTITY_REF</code></li>
         * <li><code>TOK.CHAR_REF</code></li>
         * <li><code>TOK.CHAR_PAIR_REF</code></li>
         * <li><code>TOK.PI</code></li>
         * <li><code>TOK.XML_DECL</code></li>
         * <li><code>TOK.COMMENT</code></li>
         * </ul>
         * <p>
         * Information about the token is stored in <code>token</code>.
         * </p>
         * When <code>TOK.CDATA_SECT_OPEN</code> is returned,
         * <code>tokenizeCdataSection</code> should be called until
         * it returns <code>TOK.CDATA_SECT</code>.
         *
         * @exception EmptyTokenException if the subarray is empty
         * @exception PartialTokenException if the subarray contains only part of
         * a legal token
         * @exception InvalidTokenException if the subarrary does not start
         * with a legal token or part of one
         * @exception ExtensibleTokenException if the subarray encodes just a carriage
         * return ('\r')
         */
		public TokenType TokenizeContent(byte[] buf, int off, int end,
								   ContentToken token)
		{
			if (this.MinBytesPerCharacter > 1)
				end = this.AdjustEnd(off, end);

			if (off == end)
				throw new EmptyTokenException();

			switch (this.GetByteType(buf, off))
			{
				case BT_LT:
					return this.ScanLt(buf, off + this.MinBytesPerCharacter, end, token);
				case BT_AMP:
					return this.ScanRef(buf, off + this.MinBytesPerCharacter, end, token);
				case BT_CR:
					off += this.MinBytesPerCharacter;
					if (off == end)
						throw new ExtensibleTokenException(TokenType.DATA_NEWLINE);
					if (this.GetByteType(buf, off) == BT_LF)
						off += this.MinBytesPerCharacter;
					token.TokenEnd = off;
					return TokenType.DATA_NEWLINE;
				case BT_LF:
					token.TokenEnd = off + this.MinBytesPerCharacter;
					return TokenType.DATA_NEWLINE;
				case BT_RSQB:
					off += this.MinBytesPerCharacter;
					if (off == end)
						throw new ExtensibleTokenException(TokenType.DATA_CHARS);
					if (!this.IsCharMatches(buf, off, ']'))
						break;
					off += this.MinBytesPerCharacter;
					if (off == end)
						throw new ExtensibleTokenException(TokenType.DATA_CHARS);
					if (!this.IsCharMatches(buf, off, '>'))
					{
						off -= this.MinBytesPerCharacter;
						break;
					}
					throw new InvalidTokenException(off);
				case BT_NONXML:
				case BT_MALFORM:
					throw new InvalidTokenException(off);
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					this.Check2(buf, off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					this.Check3(buf, off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					this.Check4(buf, off);
					off += 4;
					break;
				default:
					off += this.MinBytesPerCharacter;
					break;
			}
			token.TokenEnd = this.GetExtendData(buf, off, end);
			return TokenType.DATA_CHARS;
		}

		protected int GetExtendData(byte[] buf, int off, int end)
		{
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_LEAD2:
						if (end - off < 2)
							return off;
						this.Check2(buf, off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							return off;
						this.Check3(buf, off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							return off;
						this.Check4(buf, off);
						off += 4;
						break;
					case BT_RSQB:
					case BT_AMP:
					case BT_LT:
					case BT_NONXML:
					case BT_MALFORM:
					case BT_CR:
					case BT_LF:
						return off;
					default:
						off += this.MinBytesPerCharacter;
						break;
				}
			}
			return off;
		}

		/* off points to character following "%" */
		protected TokenType ScanPercent(byte[] buf, int off, int end, Token token)
		{
			if (off == end)
				throw new PartialTokenException();
			switch (this.GetByteType(buf, off))
			{
				case BT_NMSTRT:
					off += this.MinBytesPerCharacter;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (this.GetByteType2(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (this.GetByteType3(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (this.GetByteType4(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_S:
				case BT_LF:
				case BT_CR:
				case BT_PERCNT:
					token.TokenEnd = off;
					return TokenType.PERCENT;
				default:
					throw new InvalidTokenException(off);
			}
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_NMSTRT:
					case BT_NAME:
					case BT_MINUS:
						off += this.MinBytesPerCharacter;
						break;
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						if (!this.IsNameChar2(buf, off))
							throw new InvalidTokenException(off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						if (!this.IsNameChar3(buf, off))
							throw new InvalidTokenException(off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						if (!this.IsNameChar4(buf, off))
							throw new InvalidTokenException(off);
						off += 4;
						break;
					case BT_SEMI:
						token.NameEnd = off;
						token.TokenEnd = off + this.MinBytesPerCharacter;
						return TokenType.PARAM_ENTITY_REF;
					default:
						throw new InvalidTokenException(off);
				}
			}
			throw new PartialTokenException();
		}


		protected TokenType ScanPoundName(byte[] buf, int off, int end, Token token)
		{
			if (off == end)
				throw new PartialTokenException();
			switch (this.GetByteType(buf, off))
			{
				case BT_NMSTRT:
					off += this.MinBytesPerCharacter;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (this.GetByteType2(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (this.GetByteType3(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (this.GetByteType4(buf, off) != BT_NMSTRT)
						throw new InvalidTokenException(off);
					off += 4;
					break;
				default:
					throw new InvalidTokenException(off);
			}
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_NMSTRT:
					case BT_NAME:
					case BT_MINUS:
						off += this.MinBytesPerCharacter;
						break;
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						if (!this.IsNameChar2(buf, off))
							throw new InvalidTokenException(off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						if (!this.IsNameChar3(buf, off))
							throw new InvalidTokenException(off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						if (!this.IsNameChar4(buf, off))
							throw new InvalidTokenException(off);
						off += 4;
						break;
					case BT_CR:
					case BT_LF:
					case BT_S:
					case BT_RPAR:
					case BT_GT:
					case BT_PERCNT:
					case BT_VERBAR:
						token.TokenEnd = off;
						return TokenType.POUND_NAME;
					default:
						throw new InvalidTokenException(off);
				}
			}
			throw new ExtensibleTokenException(TokenType.POUND_NAME);
		}

		protected TokenType ScanLit(int open, byte[] buf, int off, int end, Token token)
		{
			while (off != end)
			{
				var t = this.GetByteType(buf, off);
				switch (t)
				{
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialTokenException();
						this.Check2(buf, off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialTokenException();
						this.Check3(buf, off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialTokenException();
						this.Check4(buf, off);
						off += 4;
						break;
					case BT_NONXML:
					case BT_MALFORM:
						throw new InvalidTokenException(off);
					case BT_QUOT:
					case BT_APOS:
						off += this.MinBytesPerCharacter;
						if (t != open)
							break;
						if (off == end)
							throw new ExtensibleTokenException(TokenType.LITERAL);
						switch (this.GetByteType(buf, off))
						{
							case BT_S:
							case BT_CR:
							case BT_LF:
							case BT_GT:
							case BT_PERCNT:
							case BT_LSQB:
								token.TokenEnd = off;
								return TokenType.LITERAL;
							default:
								throw new InvalidTokenException(off);
						}
					default:
						off += this.MinBytesPerCharacter;
						break;
				}
			}
			throw new PartialTokenException();
		}

		/**
         * Returns an encoding object to be used to start parsing an
         * external entity.  The encoding is chosen based on the
         * initial 4 bytes of the entity.
         * 
         /// @param buf the byte array containing the initial bytes of
         the entity @param off the index in <code>buf</code> of the
         first byte of the entity 
         * 
         * @param end the index in
         * <code>buf</code> following the last available byte of the
         * entity; <code>end - off</code> must be greater than or
         * equal to 4 unless the entity has fewer that 4 bytes, in
         * which case it must be equal to the length of the entity
         * 
         * @param token receives information about the presence of a
         * byte order mark; if the entity starts with a byte order
         * mark then <code>token.getTokenEnd()</code> will return
         * <code>off + 2</code>, otherwise it will return
         * <code>off</code>
         */

		/// <summary>
		/// Returns an encoding object to be used to start parsing an
		/// external entity.The encoding is chosen based on the
		/// initial 4 bytes of the entity.
		/// </summary>
		/// <param name="buf">The byte array containing the initial bytes of the entity.</param>
		/// <param name="off">The index in buf of the first byte of the entity.</param>
		/// <param name="end">The index in buf following the last available byte of the entity; end - off must be greater than or equal to 4 unless the entity has fewer that 4 bytes, in which case it must be equal to the length of the entity</param>
		/// <param name="token">Receives information about the presence of a byte order mark; if the entity starts with a byte order mark then <see cref="Token.TokenEnd" /> will return <paramref name="off"/> + 2, otherwise it will return <paramref name="off"/></param>
		public static XmlEncoding GetInitialEncoding(byte[] buf, int off, int end, Token token)
		{
			token.TokenEnd = off;

			switch (end - off)
			{
				case 0:
					break;
				case 1:
					if (buf[off] > 127)
						return null;
					break;
				default:
					var b0 = buf[off] & 0xFF;
					var b1 = buf[off + 1] & 0xFF;
					switch (b0 << 8 | b1)
					{
						case 0xFEFF:
							token.TokenEnd = off + 2;
							/* fall through */
							goto case '<';
						case '<': /* not legal; but not a fatal error */
							throw new NotImplementedException(); //return GetEncoding(UTF16_BIG_ENDIAN_ENCODING);
						case 0xFFFE:
							token.TokenEnd = off + 2;
							/* fall through */
							goto case '<' << 8;
						case '<' << 8:  /* not legal; but not a fatal error */
							throw new NotImplementedException(); // return GetEncoding(UTF16_LITTLE_ENDIAN_ENCODING);
					}
					break;
			}
			//return GetEncoding(UTF8_ENCODING);
			return UTF8;
		}

		/**
         * Returns an <code>Encoding</code> corresponding to the
         * specified IANA character set name.  Returns this
         * <code>Encoding</code> if the name is null.  Returns null if
         * the specified encoding is not supported.  Note that there
         * are two distinct <code>Encoding</code> objects associated
         * with the name <code>UTF-16</code>, one for each possible
         * byte order; if this <code>Encoding</code> is UTF-16 with
         * little-endian byte ordering, then
         * <code>getEncoding("UTF-16")</code> will return this,
         * otherwise it will return an <code>Encoding</code> for
         * UTF-16 with big-endian byte ordering.  @param name a string
         * specifying the IANA name of the encoding; this is case
         * insensitive
         */
		[Obsolete]
		public XmlEncoding GetEncoding(string name)
		{
			return UTF8;

			//if (name == null)
			//	return this;

			//switch (name.ToUpper())
			//{
			//	case "UTF-8":
			//		return GetEncoding(UTF8_ENCODING);
			//		/*
			//	case "UTF-16":
			//		return getUTF16Encoding();
			//	case "ISO-8859-1":
			//		return getEncoding(ISO8859_1_ENCODING);
			//	case "US-ASCII":
			//		return getEncoding(ASCII_ENCODING);
			//		*/
			//}
			//return null;
		}

		/**
         * Returns an <code>Encoding</code> for entities encoded with
         * a single-byte encoding (an encoding in which each byte
         * represents exactly one character).  @param map a string
         * specifying the character represented by each byte; the
         * string must have a length of 256;
         * <code>map.charAt(b)</code> specifies the character encoded
         * by byte <code>b</code>; bytes that do not represent any
         * character should be mapped to <code>\uFFFD</code>
         */
		[Obsolete("Use UTF8 static instead.")]
		public XmlEncoding GetSingleByteEncoding(string map)
			=> throw new NotImplementedException();

		/// <summary>
		/// Returns an <see cref="XmlEncoding"/> object for use with
		/// internal entities.  This is a UTF-16 big endian encoding,
		/// except that newlines are assumed to have been normalized
		/// into line feed, so carriage return is treated like a space.
		/// </summary>
		[Obsolete("Use UTF8 static instead.")]
		public static XmlEncoding GetInternalEncoding()
			=> throw new NotImplementedException();

		/// <summary>
		/// Scans the first token of a byte subarray that contains part of a prolog.
		/// <summary>
		public TokenType TokenizeProlog(byte[] buf, int off, int end, Token token)
		{
			TokenType tok;
			if (this.MinBytesPerCharacter > 1)
				end = this.AdjustEnd(off, end);
			if (off == end)
				throw new EmptyTokenException();
			switch (this.GetByteType(buf, off))
			{
				case BT_QUOT:
					return this.ScanLit(BT_QUOT, buf, off + this.MinBytesPerCharacter, end, token);
				case BT_APOS:
					return this.ScanLit(BT_APOS, buf, off + this.MinBytesPerCharacter, end, token);
				case BT_LT:
					{
						off += this.MinBytesPerCharacter;
						if (off == end)
							throw new PartialTokenException();
						switch (this.GetByteType(buf, off))
						{
							case BT_EXCL:
								return this.ScanDecl(buf, off + this.MinBytesPerCharacter, end, token);
							case BT_QUEST:
								return this.ScanPi(buf, off + this.MinBytesPerCharacter, end, token);
							case BT_NMSTRT:
							case BT_LEAD2:
							case BT_LEAD3:
							case BT_LEAD4:
								token.TokenEnd = off - this.MinBytesPerCharacter;
								throw new EndOfPrologException();
						}
						throw new InvalidTokenException(off);
					}
				case BT_CR:
					if (off + this.MinBytesPerCharacter == end)
						throw new ExtensibleTokenException(TokenType.PROLOG_S);
					/* fall through */
					goto case BT_S;
				case BT_S:
				case BT_LF:
					for (; ; )
					{
						off += this.MinBytesPerCharacter;
						if (off == end)
							break;
						switch (this.GetByteType(buf, off))
						{
							case BT_S:
							case BT_LF:
								break;
							case BT_CR:
								/* don't split CR/LF pair */
								if (off + this.MinBytesPerCharacter != end)
									break;
								/* fall through */
								goto default;
							default:
								token.TokenEnd = off;
								return TokenType.PROLOG_S;
						}
					}
					token.TokenEnd = off;
					return TokenType.PROLOG_S;
				case BT_PERCNT:
					return this.ScanPercent(buf, off + this.MinBytesPerCharacter, end, token);
				case BT_COMMA:
					token.TokenEnd = off + this.MinBytesPerCharacter;
					return TokenType.COMMA;
				case BT_LSQB:
					token.TokenEnd = off + this.MinBytesPerCharacter;
					return TokenType.OPEN_BRACKET;
				case BT_RSQB:
					off += this.MinBytesPerCharacter;
					if (off == end)
						throw new ExtensibleTokenException(TokenType.CLOSE_BRACKET);
					if (this.IsCharMatches(buf, off, ']'))
					{
						if (off + this.MinBytesPerCharacter == end)
							throw new PartialTokenException();
						if (this.IsCharMatches(buf, off + this.MinBytesPerCharacter, '>'))
						{
							token.TokenEnd = off + 2 * this.MinBytesPerCharacter;
							return TokenType.COND_SECT_CLOSE;
						}
					}
					token.TokenEnd = off;
					return TokenType.CLOSE_BRACKET;
				case BT_LPAR:
					token.TokenEnd = off + this.MinBytesPerCharacter;
					return TokenType.OPEN_PAREN;
				case BT_RPAR:
					off += this.MinBytesPerCharacter;
					if (off == end)
						throw new ExtensibleTokenException(TokenType.CLOSE_PAREN);
					switch (this.GetByteType(buf, off))
					{
						case BT_AST:
							token.TokenEnd = off + this.MinBytesPerCharacter;
							return TokenType.CLOSE_PAREN_ASTERISK;
						case BT_QUEST:
							token.TokenEnd = off + this.MinBytesPerCharacter;
							return TokenType.CLOSE_PAREN_QUESTION;
						case BT_PLUS:
							token.TokenEnd = off + this.MinBytesPerCharacter;
							return TokenType.CLOSE_PAREN_PLUS;
						case BT_CR:
						case BT_LF:
						case BT_S:
						case BT_GT:
						case BT_COMMA:
						case BT_VERBAR:
						case BT_RPAR:
							token.TokenEnd = off;
							return TokenType.CLOSE_PAREN;
					}
					throw new InvalidTokenException(off);
				case BT_VERBAR:
					token.TokenEnd = off + this.MinBytesPerCharacter;
					return TokenType.OR;
				case BT_GT:
					token.TokenEnd = off + this.MinBytesPerCharacter;
					return TokenType.DECL_CLOSE;
				case BT_NUM:
					return this.ScanPoundName(buf, off + this.MinBytesPerCharacter, end, token);
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					switch (this.GetByteType2(buf, off))
					{
						case BT_NMSTRT:
							off += 2;
							tok = TokenType.NAME;
							break;
						case BT_NAME:
							off += 2;
							tok = TokenType.NMTOKEN;
							break;
						default:
							throw new InvalidTokenException(off);
					}
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					switch (this.GetByteType3(buf, off))
					{
						case BT_NMSTRT:
							off += 3;
							tok = TokenType.NAME;
							break;
						case BT_NAME:
							off += 3;
							tok = TokenType.NMTOKEN;
							break;
						default:
							throw new InvalidTokenException(off);
					}
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					switch (this.GetByteType4(buf, off))
					{
						case BT_NMSTRT:
							off += 4;
							tok = TokenType.NAME;
							break;
						case BT_NAME:
							off += 4;
							tok = TokenType.NMTOKEN;
							break;
						default:
							throw new InvalidTokenException(off);
					}
					break;
				case BT_NMSTRT:
					tok = TokenType.NAME;
					off += this.MinBytesPerCharacter;
					break;
				case BT_NAME:
				case BT_MINUS:
					tok = TokenType.NMTOKEN;
					off += this.MinBytesPerCharacter;
					break;
				default:
					throw new InvalidTokenException(off);
			}
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_NMSTRT:
					case BT_NAME:
					case BT_MINUS:
						off += this.MinBytesPerCharacter;
						break;
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						if (!this.IsNameChar2(buf, off))
							throw new InvalidTokenException(off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						if (!this.IsNameChar3(buf, off))
							throw new InvalidTokenException(off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						if (!this.IsNameChar4(buf, off))
							throw new InvalidTokenException(off);
						off += 4;
						break;
					case BT_GT:
					case BT_RPAR:
					case BT_COMMA:
					case BT_VERBAR:
					case BT_LSQB:
					case BT_PERCNT:
					case BT_S:
					case BT_CR:
					case BT_LF:
						token.TokenEnd = off;
						return tok;
					case BT_PLUS:
						if (tok != TokenType.NAME)
							throw new InvalidTokenException(off);
						token.TokenEnd = off + this.MinBytesPerCharacter;
						return TokenType.NAME_PLUS;
					case BT_AST:
						if (tok != TokenType.NAME)
							throw new InvalidTokenException(off);
						token.TokenEnd = off + this.MinBytesPerCharacter;
						return TokenType.NAME_ASTERISK;
					case BT_QUEST:
						if (tok != TokenType.NAME)
							throw new InvalidTokenException(off);
						token.TokenEnd = off + this.MinBytesPerCharacter;
						return TokenType.NAME_QUESTION;
					default:
						throw new InvalidTokenException(off);
				}
			}
			throw new ExtensibleTokenException(tok);
		}

		/**
         * Scans the first token of a byte subarrary that contains part of
         * literal attribute value.  The opening and closing delimiters
         * are not included in the subarrary.
         * Returns one of the following integers according to the type of
         * token that the subarray starts with:
         * <ul>
         * <li><code>TOK.DATA_CHARS</code></li>
         * <li><code>TOK.DATA_NEWLINE</code></li>
         * <li><code>TOK.ATTRIBUTE_VALUE_S</code></li>
         * <li><code>TOK.MAGIC_ENTITY_REF</code></li>
         * <li><code>TOK.ENTITY_REF</code></li>
         * <li><code>TOK.CHAR_REF</code></li>
         * <li><code>TOK.CHAR_PAIR_REF</code></li>
         * </ul>
         * @exception EmptyTokenException if the subarray is empty
         * @exception PartialTokenException if the subarray contains only part of
         * a legal token
         * @exception InvalidTokenException if the subarrary does not start
         * with a legal token or part of one
         * @exception ExtensibleTokenException if the subarray encodes just a carriage
         * return ('\r')
         * @see #TOK.DATA_CHARS
         * @see #TOK.DATA_NEWLINE
         * @see #TOK.ATTRIBUTE_VALUE_S
         * @see #TOK.MAGIC_ENTITY_REF
         * @see #TOK.ENTITY_REF
         * @see #TOK.CHAR_REF
         * @see #TOK.CHAR_PAIR_REF
         * @see Token
         * @see EmptyTokenException
         * @see PartialTokenException
         * @see InvalidTokenException
         * @see ExtensibleTokenException
         */
		public TokenType TokenizeAttributeValue(byte[] buf, int off, int end, Token token)
		{
			if (this.MinBytesPerCharacter > 1)
				end = this.AdjustEnd(off, end);
			if (off == end)
				throw new EmptyTokenException();
			var start = off;
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						off += 4;
						break;
					case BT_AMP:
						if (off == start)
							return this.ScanRef(buf, off + this.MinBytesPerCharacter, end, token);
						token.TokenEnd = off;
						return TokenType.DATA_CHARS;
					case BT_LT:
						/* this is for inside entity references */
						throw new InvalidTokenException(off);
					case BT_S:
						if (off == start)
						{
							token.TokenEnd = off + this.MinBytesPerCharacter;
							return TokenType.ATTRIBUTE_VALUE_S;
						}
						token.TokenEnd = off;
						return TokenType.DATA_CHARS;
					case BT_LF:
						if (off == start)
						{
							token.TokenEnd = off + this.MinBytesPerCharacter;
							return TokenType.DATA_NEWLINE;
						}
						token.TokenEnd = off;
						return TokenType.DATA_CHARS;
					case BT_CR:
						if (off == start)
						{
							off += this.MinBytesPerCharacter;
							if (off == end)
								throw new ExtensibleTokenException(TokenType.DATA_NEWLINE);
							if (this.GetByteType(buf, off) == BT_LF)
								off += this.MinBytesPerCharacter;
							token.TokenEnd = off;
							return TokenType.DATA_NEWLINE;
						}
						token.TokenEnd = off;
						return TokenType.DATA_CHARS;
					default:
						off += this.MinBytesPerCharacter;
						break;
				}
			}
			token.TokenEnd = off;
			return TokenType.DATA_CHARS;
		}

		/**
         * Scans the first token of a byte subarrary that contains part of
         * literal entity value.  The opening and closing delimiters
         * are not included in the subarrary.
         * Returns one of the following integers according to the type of
         * token that the subarray starts with:
         * <ul>
         * <li><code>TOK.DATA_CHARS</code></li>
         * <li><code>TOK.DATA_NEWLINE</code></li>
         * <li><code>TOK.PARAM_ENTITY_REF</code></li>
         * <li><code>TOK.MAGIC_ENTITY_REF</code></li>
         * <li><code>TOK.ENTITY_REF</code></li>
         * <li><code>TOK.CHAR_REF</code></li>
         * <li><code>TOK.CHAR_PAIR_REF</code></li>
         * </ul>
         * @exception EmptyTokenException if the subarray is empty
         * @exception PartialTokenException if the subarray contains only part of
         * a legal token
         * @exception InvalidTokenException if the subarrary does not start
         * with a legal token or part of one
         * @exception ExtensibleTokenException if the subarray encodes just a carriage
         * return ('\r')
         * @see #TOK.DATA_CHARS
         * @see #TOK.DATA_NEWLINE
         * @see #TOK.MAGIC_ENTITY_REF
         * @see #TOK.ENTITY_REF
         * @see #TOK.PARAM_ENTITY_REF
         * @see #TOK.CHAR_REF
         * @see #TOK.CHAR_PAIR_REF
         * @see Token
         * @see EmptyTokenException
         * @see PartialTokenException
         * @see InvalidTokenException
         * @see ExtensibleTokenException
         */
		public TokenType TokenizeEntityValue(byte[] buf, int off, int end,
									   Token token)
		{
			if (this.MinBytesPerCharacter > 1)
				end = this.AdjustEnd(off, end);
			if (off == end)
				throw new EmptyTokenException();
			var start = off;
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						off += 4;
						break;
					case BT_AMP:
						if (off == start)
							return this.ScanRef(buf, off + this.MinBytesPerCharacter, end, token);
						token.TokenEnd = off;
						return TokenType.DATA_CHARS;
					case BT_PERCNT:
						if (off == start)
							return this.ScanPercent(buf, off + this.MinBytesPerCharacter, end, token);
						token.TokenEnd = off;
						return TokenType.DATA_CHARS;
					case BT_LF:
						if (off == start)
						{
							token.TokenEnd = off + this.MinBytesPerCharacter;
							return TokenType.DATA_NEWLINE;
						}
						token.TokenEnd = off;
						return TokenType.DATA_CHARS;
					case BT_CR:
						if (off == start)
						{
							off += this.MinBytesPerCharacter;
							if (off == end)
								throw new ExtensibleTokenException(TokenType.DATA_NEWLINE);
							if (this.GetByteType(buf, off) == BT_LF)
								off += this.MinBytesPerCharacter;
							token.TokenEnd = off;
							return TokenType.DATA_NEWLINE;
						}
						token.TokenEnd = off;
						return TokenType.DATA_CHARS;
					default:
						off += this.MinBytesPerCharacter;
						break;
				}
			}
			token.TokenEnd = off;
			return TokenType.DATA_CHARS;
		}

		/**
         * Skips over an ignored conditional section.
         * The subarray starts following the <code>&lt;![ IGNORE [</code>.
         *
         * @return the index of the character following the closing
         * <code>]]&gt;</code>
         *
         * @exception PartialTokenException if the subarray does not contain the
         * complete ignored conditional section
         * @exception InvalidTokenException if the ignored conditional section
         * contains illegal characters
         */
		public int SkipIgnoreSect(byte[] buf, int off, int end)
		{
			if (this.MinBytesPerCharacter > 1)
				end = this.AdjustEnd(off, end);
			var level = 0;
			while (off != end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						this.Check2(buf, off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						this.Check3(buf, off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						this.Check4(buf, off);
						off += 4;
						break;
					case BT_NONXML:
					case BT_MALFORM:
						throw new InvalidTokenException(off);
					case BT_LT:
						off += this.MinBytesPerCharacter;
						if (off == end)
							goto loop;
						if (!this.IsCharMatches(buf, off, '!'))
							break;
						off += this.MinBytesPerCharacter;
						if (off == end)
							goto loop;
						if (!this.IsCharMatches(buf, off, '['))
							break;
						level++;
						off += this.MinBytesPerCharacter;
						break;
					case BT_RSQB:
						off += this.MinBytesPerCharacter;
						if (off == end)
							goto loop;
						if (!this.IsCharMatches(buf, off, ']'))
							break;
						off += this.MinBytesPerCharacter;
						if (off == end)
							goto loop;
						if (this.IsCharMatches(buf, off, '>'))
						{
							if (level == 0)
								return off + this.MinBytesPerCharacter;
							level--;
						}
						else if (this.IsCharMatches(buf, off, ']'))
							break;
						off += this.MinBytesPerCharacter;
						break;
					default:
						off += this.MinBytesPerCharacter;
						break;
				}
			}

		loop:
			throw new PartialTokenException();
		}

		/// <summary>
		/// Checks that a literal contained in the specified byte subarray
		/// is a legal public identifier and returns a string with
		/// the normalized content of the public id.
		/// The subarray includes the opening and closing quotes.
		/// </summary>
		/// <exception cref="InvalidTokenException">If it is not a legal public identifier.</exception>
		public string GetPublicId(byte[] buf, int off, int end)
		{
			var sbuf = new System.Text.StringBuilder();
			off += this.MinBytesPerCharacter;
			end -= this.MinBytesPerCharacter;
			for (; off != end; off += this.MinBytesPerCharacter)
			{
				var c = this.BytesToAscii(buf, off);
				switch (this.GetByteType(buf, off))
				{
					case BT_MINUS:
					case BT_APOS:
					case BT_LPAR:
					case BT_RPAR:
					case BT_PLUS:
					case BT_COMMA:
					case BT_SOL:
					case BT_EQUALS:
					case BT_QUEST:
					case BT_SEMI:
					case BT_EXCL:
					case BT_AST:
					case BT_PERCNT:
					case BT_NUM:
						sbuf.Append(c);
						break;
					case BT_S:
						if (this.IsCharMatches(buf, off, '\t'))
							throw new InvalidTokenException(off);
						/* fall through */
						goto case BT_CR;
					case BT_CR:
					case BT_LF:
						if (sbuf.Length > 0 && sbuf[^1] != ' ')
							sbuf.Append(' ');
						break;
					case BT_NAME:
					case BT_NMSTRT:
						if ((c & ~0x7f) == 0)
						{
							sbuf.Append(c);
							break;
						}
						// fall through
						goto default;
					default:
						switch (c)
						{
							case '$':
							case '@':
								break;
							default:
								throw new InvalidTokenException(off);
						}
						break;
				}
			}
			if (sbuf.Length > 0 && sbuf[^1] == ' ')
				sbuf.Length -= 1;
			return sbuf.ToString();
		}

		/**
         * Returns true if the specified byte subarray is equal to the string.
         * The string must contain only XML significant characters.
         */
		public bool MatchesXmlstring(byte[] buf, int off, int end, string str)
		{
			var len = str.Length;
			if (len * this.MinBytesPerCharacter != end - off)
				return false;
			for (var i = 0; i < len; off += this.MinBytesPerCharacter, i++)
			{
				if (!this.IsCharMatches(buf, off, str[i]))
					return false;
			}
			return true;
		}

		/**
         * Skips over XML whitespace characters at the start of the specified
         * subarray.
         *
         * @return the index of the first non-whitespace character,
         * <code>end</code> if there is the subarray is all whitespace
         */
		public int SkipSpace(byte[] buf, int off, int end)
		{
			while (off < end)
			{
				switch (this.GetByteType(buf, off))
				{
					case BT_S:
					case BT_CR:
					case BT_LF:
						off += this.MinBytesPerCharacter;
						break;
					default:
						goto loop;
				}
			}
		loop:
			return off;
		}

		protected bool IsNameChar2(byte[] buf, int off)
		{
			var bt = this.GetByteType2(buf, off);
			return bt == BT_NAME || bt == BT_NMSTRT;
		}

		protected bool IsNameChar3(byte[] buf, int off)
		{
			var bt = this.GetByteType3(buf, off);
			return bt == BT_NAME || bt == BT_NMSTRT;
		}

		protected bool IsNameChar4(byte[] buf, int off)
		{
			var bt = this.GetByteType4(buf, off);
			return bt == BT_NAME || bt == BT_NMSTRT;
		}

		protected static int[][] CharTypeTable;

		protected static void SetCharType(char c, int type)
		{
			if (c < 0x80)
				return;
			var hi = c >> 8;
			if (CharTypeTable[hi] == null)
			{
				CharTypeTable[hi] = new int[256];
				for (var i = 0; i < 256; i++)
					CharTypeTable[hi][i] = BT_OTHER;
			}
			CharTypeTable[hi][c & 0xFF] = type;
		}

		protected static void SetCharType(char min, char max, int type)
		{
			int[] shared = null;
			do
			{
				if ((min & 0xFF) == 0)
				{
					for (; min + (char)0xFF <= max; min += (char)0x100)
					{
						if (shared == null)
						{
							shared = new int[256];
							for (var i = 0; i < 256; i++)
								shared[i] = type;
						}
						CharTypeTable[min >> 8] = shared;
						if (min + 0xFF == max)
							return;
					}
				}
				SetCharType(min, type);
			} while (min++ != max);
		}
	}
}