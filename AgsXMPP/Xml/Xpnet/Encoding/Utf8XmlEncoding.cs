using System;
using AgsXMPP.Xml.Xpnet.Exceptions;

namespace AgsXMPP.Xml.Xpnet.Encoding
{
	/// <summary>
	/// UTF-8 specific tokenizer.
	/// </summary>
	public class Utf8XmlEncoding : XmlEncoding

	{
		internal static int[] Utf8HiTypeTable { get; set; } = new int[]
		{
            /* 0x80 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0x84 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0x88 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0x8C */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0x90 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0x94 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0x98 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0x9C */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0xA0 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0xA4 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0xA8 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0xAC */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0xB0 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0xB4 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0xB8 */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0xBC */ BT_MALFORM, BT_MALFORM, BT_MALFORM, BT_MALFORM,
            /* 0xC0 */ BT_LEAD2, BT_LEAD2, BT_LEAD2, BT_LEAD2,
            /* 0xC4 */ BT_LEAD2, BT_LEAD2, BT_LEAD2, BT_LEAD2,
            /* 0xC8 */ BT_LEAD2, BT_LEAD2, BT_LEAD2, BT_LEAD2,
            /* 0xCC */ BT_LEAD2, BT_LEAD2, BT_LEAD2, BT_LEAD2,
            /* 0xD0 */ BT_LEAD2, BT_LEAD2, BT_LEAD2, BT_LEAD2,
            /* 0xD4 */ BT_LEAD2, BT_LEAD2, BT_LEAD2, BT_LEAD2,
            /* 0xD8 */ BT_LEAD2, BT_LEAD2, BT_LEAD2, BT_LEAD2,
            /* 0xDC */ BT_LEAD2, BT_LEAD2, BT_LEAD2, BT_LEAD2,
            /* 0xE0 */ BT_LEAD3, BT_LEAD3, BT_LEAD3, BT_LEAD3,
            /* 0xE4 */ BT_LEAD3, BT_LEAD3, BT_LEAD3, BT_LEAD3,
            /* 0xE8 */ BT_LEAD3, BT_LEAD3, BT_LEAD3, BT_LEAD3,
            /* 0xEC */ BT_LEAD3, BT_LEAD3, BT_LEAD3, BT_LEAD3,
            /* 0xF0 */ BT_LEAD4, BT_LEAD4, BT_LEAD4, BT_LEAD4,
            /* 0xF4 */ BT_LEAD4, BT_LEAD4, BT_LEAD4, BT_LEAD4,
            /* 0xF8 */ BT_NONXML, BT_NONXML, BT_NONXML, BT_NONXML,
            /* 0xFC */ BT_NONXML, BT_NONXML, BT_MALFORM, BT_MALFORM
		};

		internal static int[] Utf8TypeTable { get; set; } = new int[256];

		static Utf8XmlEncoding()
		{
			Array.Copy(AsciiTypeTable, 0, Utf8TypeTable, 0, 128);
			Array.Copy(Utf8HiTypeTable, 0, Utf8TypeTable, 128, 128);
		}

		/// <summary>
		/// New tokenizer
		/// </summary>
		public Utf8XmlEncoding() : base(1)
		{

		}

		/// <summary>
		/// What is the type of the current byte?
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <returns></returns>
		protected override int GetByteType(byte[] buf, int off)
		{
			return Utf8TypeTable[buf[off] & 0xFF];
		}

		/// <summary>
		/// Current byte to ASCII char
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <returns></returns>
		protected override char BytesToAscii(byte[] buf, int off)
		{
			return (char)buf[off];
		}

		/// <summary>
		/// c is a significant ASCII character
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		protected override bool IsCharMatches(byte[] buf, int off, char c)
		{
			return ((char)buf[off]) == c;
		}

		/// <summary>
		/// A 2 byte UTF-8 representation splits the characters 11 bits
		/// between the bottom 5 and 6 bits of the bytes.
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <returns></returns>
		protected override int GetByteType2(byte[] buf, int off)
		{
			var page = CharTypeTable[(buf[off] >> 2) & 0x7];
			return page[((buf[off] & 3) << 6) | (buf[off + 1] & 0x3F)];
		}

		protected override void Check3(byte[] buf, int off)
		{
			switch (buf[off])
			{
				case 0xEF:
					/* 0xFFFF 0xFFFE */
					if ((buf[off + 1] == 0xBF) &&
						((buf[off + 2] == 0xBF) ||
						 (buf[off + 2] == 0xBE)))
						throw new InvalidTokenException(off);
					return;
				case 0xED:
					/* 0xD800..0xDFFF <=> top 5 bits are 11011 */
					if ((buf[off + 1] & 0x20) != 0)
						throw new InvalidTokenException(off);
					return;
				default:
					return;
			}
		}

		protected override void Check4(byte[] buf, int off)
		{
			switch (buf[off] & 0x7)
			{
				default:
					return;
				case 5:
				case 6:
				case 7:
					break;
				case 4:
					if ((buf[off + 1] & 0x30) == 0)
						return;
					break;
			}

			throw new InvalidTokenException(off);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceBuf"></param>
		/// <param name="sourceStart"></param>
		/// <param name="sourceEnd"></param>
		/// <param name="targetBuf"></param>
		/// <param name="targetStart"></param>
		/// <returns></returns>
		protected override int Convert(byte[] sourceBuf,
									   int sourceStart, int sourceEnd,
									   char[] targetBuf, int targetStart)
		{
			var initTargetStart = targetStart;
			int c;
			while (sourceStart != sourceEnd)
			{
				var b = sourceBuf[sourceStart++];
				if (b >= 0)
					targetBuf[targetStart++] = (char)b;
				else
				{
					switch (Utf8TypeTable[b & 0xFF])
					{
						case BT_LEAD2:
							/* 5, 6 */
							targetBuf[targetStart++]
								= (char)(((b & 0x1F) << 6) | (sourceBuf[sourceStart++] & 0x3F));
							break;
						case BT_LEAD3:
							/* 4, 6, 6 */
							c = (b & 0xF) << 12;
							c |= (sourceBuf[sourceStart++] & 0x3F) << 6;
							c |= (sourceBuf[sourceStart++] & 0x3F);
							targetBuf[targetStart++] = (char)c;
							break;
						case BT_LEAD4:
							/* 3, 6, 6, 6 */
							c = (b & 0x7) << 18;
							c |= (sourceBuf[sourceStart++] & 0x3F) << 12;
							c |= (sourceBuf[sourceStart++] & 0x3F) << 6;
							c |= (sourceBuf[sourceStart++] & 0x3F);
							c -= 0x10000;
							targetBuf[targetStart++] = (char)((c >> 10) | 0xD800);
							targetBuf[targetStart++] = (char)((c & ((1 << 10) - 1)) | 0xDC00);
							break;
					}
				}
			}
			return targetStart - initTargetStart;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="off"></param>
		/// <param name="end"></param>
		/// <param name="pos"></param>
		protected override void MovePosition(byte[] buf, int off, int end, Position pos)
		{
			/* Maintain the invariant: off - colDiff == colNumber. */
			var colDiff = off - pos.ColumnNumber;
			var lineNumber = pos.LineNumber;
			while (off != end)
			{
				var b = buf[off];
				if (b >= 0)
				{
					++off;
					switch (b)
					{
						case (byte)'\n':
							lineNumber += 1;
							colDiff = off;
							break;
						case (byte)'\r':
							lineNumber += 1;
							if (off != end && buf[off] == '\n')
								off++;
							colDiff = off;
							break;
					}
				}
				else
				{
					switch (Utf8TypeTable[b & 0xFF])
					{
						default:
							off += 1;
							break;
						case BT_LEAD2:
							off += 2;
							colDiff++;
							break;
						case BT_LEAD3:
							off += 3;
							colDiff += 2;
							break;
						case BT_LEAD4:
							off += 4;
							colDiff += 3;
							break;
					}
				}
			}
			pos.ColumnNumber = off - colDiff;
			pos.LineNumber = lineNumber;
		}
	}
}