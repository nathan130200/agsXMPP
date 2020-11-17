using System;
using AgsXMPP.Xml.Xpnet.Exceptions;

namespace AgsXMPP.Xml.Xpnet
{
	public class ContentToken : Token
	{
		private const int INIT_ATT_COUNT = 8;
		private int attCount = 0;
		private int[] attNameStart = new int[INIT_ATT_COUNT];
		private int[] attNameEnd = new int[INIT_ATT_COUNT];
		private int[] attValueStart = new int[INIT_ATT_COUNT];
		private int[] attValueEnd = new int[INIT_ATT_COUNT];
		private bool[] attNormalized = new bool[INIT_ATT_COUNT];

		internal ContentToken()
		{

		}

		public int GetAttributeSpecifiedCount()
		{
			return this.attCount;
		}

		public int GetAttributeNameStart(int i)
		{
			if (i >= this.attCount)
				throw new IndexOutOfRangeException();
			return this.attNameStart[i];
		}

		public int GetAttributeNameEnd(int i)
		{
			if (i >= this.attCount)
				throw new IndexOutOfRangeException();
			return this.attNameEnd[i];
		}

		public int GetAttributeValueStart(int i)
		{
			if (i >= this.attCount)
				throw new IndexOutOfRangeException();
			return this.attValueStart[i];
		}

		public int GetAttributeValueEnd(int i)
		{
			if (i >= this.attCount)
				throw new IndexOutOfRangeException();
			return this.attValueEnd[i];
		}

		public bool IsAttributeNormalized(int i)
		{
			if (i >= this.attCount)
				throw new IndexOutOfRangeException();
			return this.attNormalized[i];
		}

		public void ClearAttributes()
		{
			this.attCount = 0;
		}

		public void AppendAttribute(int nameStart, int nameEnd,
			int valueStart, int valueEnd,
			bool normalized)
		{
			if (this.attCount == this.attNameStart.Length)
			{
				this.attNameStart = Grow(this.attNameStart);
				this.attNameEnd = Grow(this.attNameEnd);
				this.attValueStart = Grow(this.attValueStart);
				this.attValueEnd = Grow(this.attValueEnd);
				this.attNormalized = Grow(this.attNormalized);
			}
			this.attNameStart[this.attCount] = nameStart;
			this.attNameEnd[this.attCount] = nameEnd;
			this.attValueStart[this.attCount] = valueStart;
			this.attValueEnd[this.attCount] = valueEnd;
			this.attNormalized[this.attCount] = normalized;
			++this.attCount;
		}

		public void CheckAttributeUniqueness(byte[] buf)
		{
			for (var i = 1; i < this.attCount; i++)
			{
				var len = this.attNameEnd[i] - this.attNameStart[i];
				for (var j = 0; j < i; j++)
				{
					if (this.attNameEnd[j] - this.attNameStart[j] == len)
					{
						var n = len;
						var s1 = this.attNameStart[i];
						var s2 = this.attNameStart[j];
						do
						{
							if (--n < 0)
								throw new InvalidTokenException(this.attNameStart[i], InvalidTokenType.DuplicatedAttribute);
						} while (buf[s1++] == buf[s2++]);
					}
				}
			}
		}

		private static int[] Grow(int[] v)
		{
			var tem = v;
			v = new int[tem.Length << 1];
			Array.Copy(tem, 0, v, 0, tem.Length);
			return v;
		}

		private static bool[] Grow(bool[] v)
		{
			var tem = v;
			v = new bool[tem.Length << 1];
			Array.Copy(tem, 0, v, 0, tem.Length);
			return v;
		}
	}
}