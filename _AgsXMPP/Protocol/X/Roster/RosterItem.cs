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

namespace AgsXMPP.Protocol.X.Roster
{
	public enum RosterAction
	{
		None = -1,

		[XmppEnumMember("add")]
		Add,

		[XmppEnumMember("remove")]
		Remove,

		[XmppEnumMember("modify")]
		Modify
	}

	/// <summary>
	/// Summary description for RosterItem.
	/// </summary>
	public class RosterItem : Base.RosterItem
	{
		/*
		<item action='delete' jid='rosencrantz@denmark' name='Rosencrantz'>   
			<group>Visitors</group>   
		</item> 
		*/

		public RosterItem() : base()
		{
			this.Namespace = URI.X_ROSTERX;
		}

		public RosterItem(Jid jid) : this()
		{
			this.Jid = jid;
		}

		public RosterItem(Jid jid, string name) : this(jid)
		{
			this.Name = name;
		}

		public RosterItem(Jid jid, string name, RosterAction action) : this(jid, name)
		{
			this.Action = action;
		}

		public RosterAction Action
		{
			get => this.GetAttributeEnum<RosterAction>("action");
			set => this.SetAttributeEnum("action", value);
		}
	}
}
