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

namespace AgsXMPP.Protocol.X.muc
{
	/// <summary>
	/// A base class vor Decline and Invite
	/// We need From, To and SwitchDirection here. This is why we inherit from XmppPacket Base
	/// </summary>
	public abstract class Invitation : Base.Stanza
	{
		public Invitation()
		{
			this.Namespace = URI.MUC_USER;
		}

		/// <summary>
		/// A reason why you want to invite this contact
		/// </summary>
		public string Reason
		{
			set { this.SetTag("reason", value); }
			get { return this.GetTag("reason"); }
		}
	}
}
