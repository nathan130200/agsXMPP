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

namespace AgsXMPP.Protocol.Query.Roster
{
	/// <summary>
	/// <list type="table">
	/// <item><see cref="None"/>: the user does not have a subscription to the contact's presence information, and the contact does not have a subscription to the user's presence information</item>
	/// <item><see cref="To"/>: the user has a subscription to the contact's presence information, but the contact does not have a subscription to the user's presence information</item>
	/// <item><see cref="From"/>:  the contact has a subscription to the user's presence information, but the user does not have a subscription to the contact's presence information</item>
	/// <item><see cref="Both"/>: both the user and the contact have subscriptions to each other's presence information</item>
	/// </list>
	/// </summary>
	public enum SubscriptionType
	{
		[XmppEnumMember("none")]
		None,

		[XmppEnumMember("to")]
		To,

		[XmppEnumMember("from")]
		From,

		[XmppEnumMember("both")]
		Both,

		[XmppEnumMember("remove")]
		Remove
	}
}
