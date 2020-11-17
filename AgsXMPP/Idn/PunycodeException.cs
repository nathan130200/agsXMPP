using System;

namespace AgsXMPP.Idn
{

	public class PunycodeException : Exception
	{
		public static string OVERFLOW = "Overflow.";
		public static string BAD_INPUT = "Bad input.";

		/// <summary>
		/// Creates a new PunycodeException.
		/// </summary>
		/// <param name="message">message</param>
		public PunycodeException(string message) : base(message)
		{
		}
	}
}