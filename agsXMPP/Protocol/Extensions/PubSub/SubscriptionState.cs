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

namespace AgsXMPP.Protocol.Extensions.PubSub
{
	/// <summary>
	/// Subscription State
	/// </summary>
	public enum SubscriptionState
	{
		/// <summary>
		/// The node MUST NOT send event notifications or payloads to the Entity.
		/// </summary>
		[XmppEnumMember("none")]
		None,

		/// <summary>
		/// An entity has requested to subscribe to a node and the request has not yet been approved 
		/// by a node owner. The node MUST NOT send event notifications or payloads to the entity 
		/// while it is in this state.
		/// </summary>
		[XmppEnumMember("pending")]
		Pending,

		/// <summary>
		/// An entity has subscribed but its subscription options have not yet been configured. 
		/// The node MAY send event notifications or payloads to the entity while it is in this state. 
		/// The service MAY timeout unconfigured subscriptions.
		/// </summary>
		[XmppEnumMember("unconfigured")]
		Unconfigured,

		/// <summary>
		/// An entity has subscribed and its subscription options have been configured. 
		/// </summary>
		[XmppEnumMember("subscribed")]
		Subscribed
	}
}
