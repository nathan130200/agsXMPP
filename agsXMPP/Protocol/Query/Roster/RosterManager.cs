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

using AgsXMPP.Protocol.Client;

namespace AgsXMPP.Protocol.Query.Roster
{
	/// <summary>
	/// Helper Class that makes it easier to manage your roster
	/// </summary>
	public class RosterManager
	{
		private XmppClientConnection m_connection = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="con">The XmppClientConnection on which the RosterManager should send the packets</param>
		public RosterManager(XmppClientConnection con)
		{
			this.m_connection = con;
		}

		/// <summary>
		/// Removes a Rosteritem from the Roster
		/// </summary>
		/// <param name="jid">The BARE jid of the rosteritem that should be removed</param>
		public void RemoveRosterItem(Jid jid)
		{
			var riq = new RosterIq();
			riq.Type = IQType.Set;

			var ri = new RosterItem();
			ri.Jid = jid;
			ri.Subscription = SubscriptionType.Remove;

			riq.Query.AddRosterItem(ri);

			this.m_connection.Send(riq);
		}

		/// <summary>
		/// Add a Rosteritem to the Roster
		/// </summary>
		/// <param name="jid">The BARE jid of the rosteritem that should be removed</param>
		public void AddRosterItem(Jid jid)
		{
			this.AddRosterItem(jid, null, new string[] { });
		}

		/// <summary>
		/// Update a Rosteritem
		/// </summary>
		/// <param name="jid"></param>
		public void UpdateRosterItem(Jid jid)
		{
			this.AddRosterItem(jid, null, new string[] { });
		}

		/// <summary>
		/// Add a Rosteritem to the Roster
		/// </summary>
		/// <param name="jid">The BARE jid of the rosteritem that should be removed</param>
		/// <param name="nickname">Nickname for the RosterItem</param>
		public void AddRosterItem(Jid jid, string nickname)
		{
			this.AddRosterItem(jid, nickname, new string[] { });
		}

		/// <summary>
		/// Update a Rosteritem
		/// </summary>
		/// <param name="jid"></param>
		/// <param name="nickname"></param>
		public void UpdateRosterItem(Jid jid, string nickname)
		{
			this.AddRosterItem(jid, nickname, new string[] { });
		}

		/// <summary>
		/// Add a Rosteritem to the Roster
		/// </summary>
		/// <param name="jid">The BARE jid of the rosteritem that should be removed</param>
		/// <param name="nickname">Nickname for the RosterItem</param>
		/// <param name="group">The group to which the roteritem should be added</param>
		public void AddRosterItem(Jid jid, string nickname, string group)
		{
			this.AddRosterItem(jid, nickname, new string[] { group });
		}

		/// <summary>
		/// Update a Rosteritem
		/// </summary>
		/// <param name="jid"></param>
		/// <param name="nickname"></param>
		/// <param name="group"></param>
		public void UpdateRosterItem(Jid jid, string nickname, string group)
		{
			this.AddRosterItem(jid, nickname, new string[] { group });
		}

		/// <summary>
		/// Add a Rosteritem to the Roster
		/// </summary>
		/// <param name="jid">The BARE jid of the rosteritem that should be removed</param>
		/// <param name="nickname">Nickname for the RosterItem</param>
		/// <param name="group">An Array of groups when you want to add the Rosteritem to multiple groups</param>
		public void AddRosterItem(Jid jid, string nickname, string[] group)
		{
			var riq = new RosterIq();
			riq.Type = IQType.Set;

			var ri = new RosterItem();
			ri.Jid = jid;

			if (nickname != null)
				ri.Name = nickname;

			foreach (var g in group)
			{
				ri.AddGroup(g);
			}

			riq.Query.AddRosterItem(ri);

			this.m_connection.Send(riq);
		}

		/// <summary>
		/// Update a Rosteritem
		/// </summary>
		/// <param name="jid"></param>
		/// <param name="nickname"></param>
		/// <param name="group"></param>
		public void UpdateRosterItem(Jid jid, string nickname, string[] group)
		{
			this.AddRosterItem(jid, nickname, group);
		}

	}
}
