namespace AgsXMPP.Idn
{
	public class IDNAException : System.Exception
	{
		public static string CONTAINS_NON_LDH = "Contains non-LDH characters.";
		public static string CONTAINS_HYPHEN = "Leading or trailing hyphen not allowed.";
		public static string CONTAINS_ACE_PREFIX = "ACE prefix (xn--) not allowed.";
		public static string TOO_LONG = "String too long.";

		public IDNAException(string m) : base(m)
		{

		}

		// TODO
		public IDNAException(StringprepException e) : base("", e)
		{
		}

		public IDNAException(PunycodeException e) : base("", e)
		{
		}
	}
}