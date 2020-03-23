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

namespace AgsXMPP.Protocol.Extensions.jivesoftware.Phone
{
	/// <summary>
	/// Events are sent to the user when their phone is ringing, 
	/// when a call ends, etc. As with presence, 
	/// pubsub should probably be the mechanism used for sending this information, 
	/// but message packets are used to send events for the time being
	/// </summary>
	public enum PhoneStatusType
	{
		[XmppEnumMember("RING")]
		Ring,

		[XmppEnumMember("DIALED")]
		Dialed,

		[XmppEnumMember("ON_PHONE")]
		OnPhone,

		[XmppEnumMember("HANG_UP")]
		HangUp
	}
}
