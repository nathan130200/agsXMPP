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
	/// Item is used in jabber:iq:roster, jabber:iq:search
	/// </summary>
	public class RosterItem : Base.RosterItem
	{
		public RosterItem() : base()
		{
			this.Namespace = URI.IQ_ROSTER;
		}

		public RosterItem(Jid jid) : this()
		{
			this.Jid = jid;
		}

		public RosterItem(Jid jid, string name) : this(jid)
		{
			this.Name = name;
		}

		public SubscriptionType Subscription
		{
			get => this.GetAttributeEnum<SubscriptionType>("subscription");
			set => this.SetAttributeEnum("subscription", value);
		}

		public AskType Ask
		{
			get => this.GetAttributeEnum<AskType>("ask");
			set
			{
				if (value == AskType.None) this.RemoveAttribute("ask");
				else this.SetAttributeEnum("ask", value);
			}
		}
	}
}
