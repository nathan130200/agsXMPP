namespace AgsXMPP.Xml.Xpnet.Exceptions
{
	/// <summary>
	/// Thrown to indicate that the subarray being tokenized is not the
	/// complete encoding of one or more characters, but might be if
	/// more bytes were added.
	/// </summary>
	public class PartialCharException : PartialTokenException
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="leadByteIndex"></param>
		public PartialCharException(int leadByteIndex)
		{
			this.LeadByteIndex = leadByteIndex;
		}

		/// <summary>
		/// Returns the index of the first byte that is not part of the complete
		/// encoding of a character.
		/// </summary>
		public int LeadByteIndex { get; }
	}
}
