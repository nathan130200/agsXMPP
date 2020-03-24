/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2020 by AG-Software, FRNathan13								 *
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
	/// Enumeration for the Presence Type structure. 
	/// This enum is used to describe what type of Subscription Type the current subscription is.
	/// When sending a presence or receiving a subscription this type is used to easily identify the type of subscription it is.
	/// </summary>
	public enum PresenceType
	{
		/// <summary>
		/// Used when one wants to send presence to someone/server/transport that you’re available. 
		/// </summary>
		Available = -1,

		/// <summary>
		/// Used to send a subscription request to someone.
		/// </summary>
		[XmppEnumMember("subscribe")]
		Subscribe,

		/// <summary>
		/// Used to accept a subscription request.
		/// </summary>		
		[XmppEnumMember("subscribed")]
		Subscribed,

		/// <summary>
		/// Used to unsubscribe someone from your presence. 
		/// </summary>
		[XmppEnumMember("unsubscribe")]
		Unsubscribe,

		/// <summary>
		/// Used to deny a subscription request.
		/// </summary>
		[XmppEnumMember("unsubscribed")]
		Unsubscribed,

		/// <summary>
		/// Used when one wants to send presence to someone/server/transport that you’re unavailable.
		/// </summary>
		[XmppEnumMember("unavailable")]
		Unavailable,

		/// <summary>
		/// Used when you want to see your roster, but don't want anyone on you roster to see you
		/// </summary>
		[XmppEnumMember("invisible")]
		Invisible,

		/// <summary>
		/// presence error
		/// </summary>
		[XmppEnumMember("error")]
		Error,

		/// <summary>
		/// used in server to server protocol to request presences
		/// </summary>
		[XmppEnumMember("probe")]
		Probe
	}
}