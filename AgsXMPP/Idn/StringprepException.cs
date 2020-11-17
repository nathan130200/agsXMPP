using System;

namespace AgsXMPP.Idn
{
	public class StringprepException : Exception
	{
		public static string CONTAINS_UNASSIGNED = "Contains unassigned code points.";
		public static string CONTAINS_PROHIBITED = "Contains prohibited code points.";
		public static string BIDI_BOTHRAL = "Contains both R and AL code points.";
		public static string BIDI_LTRAL = "Leading and trailing code points not both R or AL.";

		public StringprepException(string message) : base(message)
		{
		}
	}
}