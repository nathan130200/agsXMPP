namespace AgsXMPP.Xml.Xpnet.Exceptions
{
	/// <summary>
	/// Several kinds of token problems.
	/// </summary>
	public class InvalidTokenException : TokenException
	{
		/// <summary>
		/// Some other type of bad token detected
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="type"></param>
		public InvalidTokenException(int offset, InvalidTokenType type)
		{
			this.Offset = offset;
			this.Type = type;
		}

		/// <summary>
		/// Illegal character detected
		/// </summary>
		/// <param name="offset"></param>
		public InvalidTokenException(int offset)
		{
			this.Offset = offset;
			this.Type = InvalidTokenType.IllegalCharacter;
		}

		/// <summary>
		/// Offset into the buffer where the problem ocurred.
		/// </summary>
		public int Offset { get; }

		/// <summary>
		/// Type of exception
		/// </summary>
		public InvalidTokenType Type { get; }
	}
}
