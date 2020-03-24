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

/* --------------------------------------------------------------------------
 * Copyrights
 * 
 * Portions created by or assigned to Cursive Systems, Inc. are 
 * Copyright (c) 2002-2005 Cursive Systems, Inc.  All Rights Reserved.  Contact
 * information for Cursive Systems, Inc. is available at
 * http://www.cursive.net/.
 *
 * License
 * 
 * Jabber-Net can be used under either JOSL or the GPL.  
 * See LICENSE.txt for details.
 * --------------------------------------------------------------------------*/

using System;

namespace AgsXMPP.Xml.Xpnet
{
	/// <summary>
	/// Represents information returned by <code>Encoding.tokenizeContent</code>.
	/// @see Encoding#tokenizeContent
	/// </summary>
	public class ContentToken : Token
	{
		private const int INIT_ATT_COUNT = 8;
		private int attCount = 0;
		private int[] attNameStart = new int[INIT_ATT_COUNT];
		private int[] attNameEnd = new int[INIT_ATT_COUNT];
		private int[] attValueStart = new int[INIT_ATT_COUNT];
		private int[] attValueEnd = new int[INIT_ATT_COUNT];
		private bool[] attNormalized = new bool[INIT_ATT_COUNT];


		/// <summary>
		/// Returns the number of attributes specified in the start-tag or empty element tag.
		/// </summary>
		/// <returns></returns>
		public int getAttributeSpecifiedCount()
		{
			return this.attCount;
		}

		/// <summary>
		/// Returns the index of the first character of the name of the
		/// attribute index <code>i</code>.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public int getAttributeNameStart(int i)
		{
			if (i >= this.attCount)
				throw new IndexOutOfRangeException();
			return this.attNameStart[i];
		}

		/**
		 * Returns the index following the last character of the name of the
		 * attribute index <code>i</code>.
		 */
		public int getAttributeNameEnd(int i)
		{
			if (i >= this.attCount)
				throw new IndexOutOfRangeException();
			return this.attNameEnd[i];
		}

		/**
		 * Returns the index of the character following the opening quote of
		 * attribute index <code>i</code>.
		 */
		public int getAttributeValueStart(int i)
		{
			if (i >= this.attCount)
				throw new IndexOutOfRangeException();
			return this.attValueStart[i];
		}

		/**
		 * Returns the index of the closing quote attribute index <code>i</code>.
		 */
		public int getAttributeValueEnd(int i)
		{
			if (i >= this.attCount)
				throw new IndexOutOfRangeException();
			return this.attValueEnd[i];
		}

		/**
		 * Returns true if attribute index <code>i</code> does not need to
		 * be normalized.  This is an optimization that allows further processing
		 * of the attribute to be avoided when it is known that normalization
		 * cannot change the value of the attribute.
		 */
		public bool isAttributeNormalized(int i)
		{
			if (i >= this.attCount)
				throw new IndexOutOfRangeException();
			return this.attNormalized[i];
		}


		/// <summary>
		/// Clear out all of the current attributes
		/// </summary>
		public void clearAttributes()
		{
			this.attCount = 0;
		}

		/// <summary>
		/// Add a new attribute
		/// </summary>
		/// <param name="nameStart"></param>
		/// <param name="nameEnd"></param>
		/// <param name="valueStart"></param>
		/// <param name="valueEnd"></param>
		/// <param name="normalized"></param>
		public void appendAttribute(int nameStart, int nameEnd,
			int valueStart, int valueEnd,
			bool normalized)
		{
			if (this.attCount == this.attNameStart.Length)
			{
				this.attNameStart = grow(this.attNameStart);
				this.attNameEnd = grow(this.attNameEnd);
				this.attValueStart = grow(this.attValueStart);
				this.attValueEnd = grow(this.attValueEnd);
				this.attNormalized = grow(this.attNormalized);
			}
			this.attNameStart[this.attCount] = nameStart;
			this.attNameEnd[this.attCount] = nameEnd;
			this.attValueStart[this.attCount] = valueStart;
			this.attValueEnd[this.attCount] = valueEnd;
			this.attNormalized[this.attCount] = normalized;
			++this.attCount;
		}

		/// <summary>
		/// Is the current attribute unique?
		/// </summary>
		/// <param name="buf"></param>
		public void checkAttributeUniqueness(byte[] buf)
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
								throw new InvalidTokenException(this.attNameStart[i],
									InvalidTokenException.DUPLICATE_ATTRIBUTE);
						} while (buf[s1++] == buf[s2++]);
					}
				}
			}
		}

		private static int[] grow(int[] v)
		{
			var tem = v;
			v = new int[tem.Length << 1];
			Array.Copy(tem, 0, v, 0, tem.Length);
			return v;
		}

		private static bool[] grow(bool[] v)
		{
			var tem = v;
			v = new bool[tem.Length << 1];
			Array.Copy(tem, 0, v, 0, tem.Length);
			return v;
		}
	}
}