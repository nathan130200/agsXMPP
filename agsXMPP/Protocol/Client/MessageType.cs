/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2019 by AG-Software, FRNathan13								 *
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

namespace AgsXMPP.Protocol.Client
{
	/// <summary>
	/// Enumeration that represents the type of a message
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// This in a normal message, much like an email. You dont expect a fast
		/// </summary>
		None = -1,

		/// <summary>
		/// a error messages
		/// </summary>
		[XmppEnumMember("error")]
		Error,

		/// <summary>
		/// is for chat like messages, person to person. Send this if you expect a fast reply. reply or no reply at all.
		/// </summary>
		[XmppEnumMember("chat")]
		Chat,

		/// <summary>
		/// is used for sending/receiving messages from/to a chatroom (IRC style chats) 
		/// </summary>
		[XmppEnumMember("groupchat")]
		GroupChat,

		/// <summary>
		/// Think of this as a news broadcast, or RRS Feed, the message will normally have a URL and Description Associated with it.
		/// </summary>
		[XmppEnumMember("headline")]
		Headline
	}
}