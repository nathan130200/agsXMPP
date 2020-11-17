namespace AgsXMPP.Xml.Xpnet.Exceptions
{
	/// <summary>
	/// Thrown to indicate that the byte subarray being tokenized is a legal XML token,
	/// but that subsequent bytes in the same entity could be part of the token.
	/// </summary>
	public class ExtensibleTokenException : TokenException
	{
		/// <summary>
		/// Ctor
		/// </summary>
		public ExtensibleTokenException(TokenType tokenType)
		{
			this.TokenType = tokenType;
		}

		/// <summary>
		/// Returns the type of token in the byte subarrary.
		/// </summary>
		public TokenType TokenType { get; }
	}
}
