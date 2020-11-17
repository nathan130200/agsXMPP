namespace AgsXMPP.Xml.Xpnet.Exceptions
{
	public enum InvalidTokenType
	{
		/// <summary>
		/// An illegal character
		/// </summary>
		IllegalCharacter = 0,

		/// <summary>
		/// Document prefix wasn't XML
		/// </summary>
		XmlTarget = 1,

		/// <summary>
		/// More than one attribute with the same name on the same element.
		/// </summary>
		DuplicatedAttribute = 2
	}
}
