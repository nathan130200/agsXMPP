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
	/// Helper class for managing presence and subscriptions
	/// </summary>
	public class PresenceManager
	{
		private XmppClientConnection m_connection = null;

		public PresenceManager(XmppClientConnection con)
		{
			this.m_connection = con;
		}

		/// <summary>
		/// Subscribe to a contact
		/// </summary>
		/// <param name="to">Bare Jid of the rosteritem we want to subscribe</param>
		public void Subscribe(Jid to)
		{
			// <presence to='contact@example.org' type='subscribe'/>
			var pres = new Presence();
			pres.Type = PresenceType.Subscribe;
			pres.To = to;

			this.m_connection.Send(pres);
		}


		/// <summary>        
		/// Subscribe to a contact
		/// </summary>        
		/// <param name="to">Bare Jid of the rosteritem we want to subscribe</param>
		/// <param name="message">a message which normally contains the reason why we want to subscibe to this contact</param>
		public void Subscribe(Jid to, string message)
		{
			var pres = new Presence();
			pres.Type = PresenceType.Subscribe;
			pres.To = to;
			pres.Status = message;

			this.m_connection.Send(pres);
		}


		/// <summary>
		/// Unsubscribe from a contact
		/// </summary>
		/// <param name="to">Bare Jid of the rosteritem we want to unsubscribe</param>
		public void Unsubscribe(Jid to)
		{
			// <presence to='contact@example.org' type='subscribe'/>
			var pres = new Presence();
			pres.Type = PresenceType.Unsubscribe;
			pres.To = to;

			this.m_connection.Send(pres);
		}

		//Example: Approving a subscription request:
		//<presence to='romeo@example.net' type='subscribed'/>

		/// <summary>
		/// Approve a subscription request
		/// </summary>
		/// <param name="to">Bare Jid to approve</param>
		public void ApproveSubscriptionRequest(Jid to)
		{
			// <presence to='contact@example.org' type='subscribe'/>
			var pres = new Presence();
			pres.Type = PresenceType.Subscribed;
			pres.To = to;

			this.m_connection.Send(pres);
		}

		//Example: Refusing a presence subscription request:
		//<presence to='romeo@example.net' type='unsubscribed'/>

		/// <summary>
		/// Refuse  subscription request
		/// </summary>
		/// <param name="to">Bare Jid to approve</param>
		public void RefuseSubscriptionRequest(Jid to)
		{
			// <presence to='contact@example.org' type='subscribe'/>
			var pres = new Presence();
			pres.Type = PresenceType.Unsubscribed;
			pres.To = to;

			this.m_connection.Send(pres);
		}
	}
}