using System.Text;

namespace AgsXMPP.Idn
{
	public class NFKC
	{
		/// <summary>
		/// Applies NFKC normalization to a string.
		/// </summary>
		/// <param name="in">The string to normalize.</param>
		/// <returns> An NFKC normalized string.</returns>
		public static string NormalizeNFKC(string sbIn)
		{
			var sbOut = new StringBuilder();

			for (var i = 0; i < sbIn.Length; i++)
			{
				var code = sbIn[i];

				// In Unicode 3.0, Hangul was defined as the block from U+AC00
				// to U+D7A3, however, since Unicode 3.2 the block extends until
				// U+D7AF. The decomposeHangul function only decomposes until
				// U+D7A3. Should this be changed?
				if (code >= 0xAC00 && code <= 0xD7AF)
				{
					sbOut.Append(DecomposeHangul(code));
				}
				else
				{
					var index = DecomposeIndex(code);
					if (index == -1)
					{
						sbOut.Append(code);
					}
					else
					{
						sbOut.Append(DecompositionMappings.m[index]);
					}
				}
			}

			// Bring the stringbuffer into canonical order.
			CanonicalOrdering(sbOut);

			// Do the canonical composition.
			var last_cc = 0;
			var last_start = 0;

			for (var i = 0; i < sbOut.Length; i++)
			{
				var cc = CombiningClass(sbOut[i]);

				if (i > 0 && (last_cc == 0 || last_cc != cc))
				{
					// Try to combine characters
					var a = sbOut[last_start];
					var b = sbOut[i];

					var c = Compose(a, b);

					if (c != -1)
					{
						sbOut[last_start] = (char)c;
						//sbOut.deleteCharAt(i);
						sbOut.Remove(i, 1);
						i--;

						if (i == last_start)
						{
							last_cc = 0;
						}
						else
						{
							last_cc = CombiningClass(sbOut[i - 1]);
						}
						continue;
					}
				}

				if (cc == 0)
				{
					last_start = i;
				}

				last_cc = cc;
			}

			return sbOut.ToString();
		}


		/// <summary>
		/// Returns the index inside the decomposition table, implemented
		/// using a binary search.
		/// </summary>
		/// <param name="c">Character to look up.</param>
		/// <returns> Index if found, -1 otherwise.</returns>
		internal static int DecomposeIndex(char c)
		{
			var start = 0;
			var end = DecompositionKeys.k.Length / 2;

			while (true)
			{
				var half = (start + end) / 2;
				var code = DecompositionKeys.k[half * 2];

				if (c == code)
				{
					return DecompositionKeys.k[half * 2 + 1];
				}
				if (half == start)
				{
					// Character not found
					return -1;
				}
				else if (c > code)
				{
					start = half;
				}
				else
				{
					end = half;
				}
			}
		}

		/// <summary>
		/// Returns the combining class of a given character.
		/// </summary>
		/// <param name="c">The character.</param>
		/// <returns> The combining class.</returns>
		internal static int CombiningClass(char c)
		{
			var h = c >> 8;
			var l = c & 0xff;

			var i = Idn.CombiningClass.i[h];
			if (i > -1)
			{
				return Idn.CombiningClass.c[i, l];
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Rearranges characters in a stringbuffer in order to respect the
		/// canonical ordering properties.
		/// </summary>
		/// <param name="The">StringBuffer to rearrange.</param>
		internal static void CanonicalOrdering(StringBuilder sbIn)
		{
			var isOrdered = false;

			while (!isOrdered)
			{
				isOrdered = true;


				// 24.10.2005
				var lastCC = 0;
				if (sbIn.Length > 0)
					lastCC = CombiningClass(sbIn[0]);

				for (var i = 0; i < sbIn.Length - 1; i++)
				{
					var nextCC = CombiningClass(sbIn[i + 1]);
					if (nextCC != 0 && lastCC > nextCC)
					{
						for (var j = i + 1; j > 0; j--)
						{
							if (CombiningClass(sbIn[j - 1]) <= nextCC)
							{
								break;
							}
							var t = sbIn[j];
							sbIn[j] = sbIn[j - 1];
							sbIn[j - 1] = t;
							isOrdered = false;
						}
						nextCC = lastCC;
					}
					lastCC = nextCC;
				}
			}
		}

		/// <summary>
		/// Returns the index inside the composition table.		
		/// </summary>
		/// <param name="a">Character to look up.</param>
		/// <returns> Index if found, -1 otherwise.</returns>
		internal static int ComposeIndex(char a)
		{
			if (a >> 8 >= Composition.composePage.Length)
			{
				return -1;
			}
			var ap = Composition.composePage[a >> 8];
			if (ap == -1)
			{
				return -1;
			}
			return Composition.composeData[ap, a & 0xff];
		}

		/// <summary>
		/// Tries to compose two characters canonically.
		/// </summary>
		/// <param name="a">First character.</param>
		/// <param name="b">Second character.</param>
		/// <returns> The composed character or -1 if no composition could be found.</returns>
		internal static int Compose(char a, char b)
		{
			var h = ComposeHangul(a, b);
			if (h != -1)
			{
				return h;
			}

			var ai = ComposeIndex(a);

			if (ai >= Composition.singleFirstStart && ai < Composition.singleSecondStart)
			{
				if (b == Composition.singleFirst[ai - Composition.singleFirstStart, 0])
				{
					return Composition.singleFirst[ai - Composition.singleFirstStart, 1];
				}
				else
				{
					return -1;
				}
			}


			var bi = ComposeIndex(b);

			if (bi >= Composition.singleSecondStart)
			{
				if (a == Composition.singleSecond[bi - Composition.singleSecondStart, 0])
				{
					return Composition.singleSecond[bi - Composition.singleSecondStart, 1];
				}
				else
				{
					return -1;
				}
			}

			if (ai >= 0 && ai < Composition.multiSecondStart && bi >= Composition.multiSecondStart && bi < Composition.singleFirstStart)
			{
				var f = Composition.multiFirst[ai];

				if (bi - Composition.multiSecondStart < f.Length)
				{
					var r = f[bi - Composition.multiSecondStart];
					if (r == 0)
					{
						return -1;
					}
					else
					{
						return r;
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Entire hangul code copied from:
		/// http://www.unicode.org/unicode/reports/tr15/
		/// Several hangul specific constants
		/// </summary>
		internal const int SBase = 0xAC00;
		internal const int LBase = 0x1100;
		internal const int VBase = 0x1161;
		internal const int TBase = 0x11A7;
		internal const int LCount = 19;
		internal const int VCount = 21;
		internal const int TCount = 28;

		internal static readonly int NCount = VCount * TCount;

		internal static readonly int SCount = LCount * NCount;

		/// <summary>
		/// Decomposes a hangul character.
		/// </summary>
		/// <param name="s">A character to decompose.</param>
		/// <returns> A string containing the hangul decomposition of the input
		/// character. If no hangul decomposition can be found, a string
		/// containing the character itself is returned.</returns>
		internal static string DecomposeHangul(char s)
		{
			var SIndex = s - SBase;
			if (SIndex < 0 || SIndex >= SCount)
			{
				return s.ToString();
			}
			var result = new StringBuilder();
			var L = LBase + SIndex / NCount;
			var V = VBase + (SIndex % NCount) / TCount;
			var T = TBase + SIndex % TCount;
			result.Append((char)L);
			result.Append((char)V);
			if (T != TBase)
				result.Append((char)T);
			return result.ToString();
		}

		/// <summary>
		/// Composes two hangul characters.
		/// </summary>
		/// <param name="a">First character.</param>
		/// <param name="b">Second character.</param>
		/// <returns> Returns the composed character or -1 if the two characters cannot be composed.</returns>
		internal static int ComposeHangul(char a, char b)
		{
			// 1. check to see if two current characters are L and V
			var LIndex = a - LBase;
			if (0 <= LIndex && LIndex < LCount)
			{
				var VIndex = b - VBase;
				if (0 <= VIndex && VIndex < VCount)
				{
					// make syllable of form LV
					return SBase + (LIndex * VCount + VIndex) * TCount;
				}
			}

			// 2. check to see if two current characters are LV and T
			var SIndex = a - SBase;
			if (0 <= SIndex && SIndex < SCount && (SIndex % TCount) == 0)
			{
				var TIndex = b - TBase;
				if (0 <= TIndex && TIndex <= TCount)
				{
					// make syllable of form LVT
					return a + TIndex;
				}
			}
			return -1;
		}
	}
}