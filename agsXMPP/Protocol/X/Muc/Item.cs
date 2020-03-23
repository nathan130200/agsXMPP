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

namespace AgsXMPP.Protocol.x.muc
{
	/// <summary>
	/// Summary description for Item.
	/// </summary>
	public class Item : Base.Item
	{
		/*
        <x xmlns='http://jabber.org/protocol/muc#user'>
             <item affiliation='admin' role='moderator'/>
        </x>
         
        <item nick='pistol' role='none'>
            <reason>Avaunt, you cullion!</reason>
        </item>
        
        <presence
                from='darkcave@macbeth.shakespeare.lit/thirdwitch'
                to='crone1@shakespeare.lit/desktop'>
                <x xmlns='http://jabber.org/protocol/muc#user'>
                    <item   affiliation='none'
                            jid='hag66@shakespeare.lit/pda'
                            role='participant'/>
                </x>
        </presence>
        */

		/// <summary>
		/// 
		/// </summary>
		public Item() : base()
		{
			this.TagName = "item";
			this.Namespace = Namespaces.MUC_USER;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="affiliation"></param>
		public Item(Affiliation affiliation) : this()
		{
			this.Affiliation = affiliation;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		public Item(Role role) : this()
		{
			this.Role = role;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="affiliation"></param>
		/// <param name="role"></param>
		public Item(Affiliation affiliation, Role role) : this(affiliation)
		{
			this.Role = role;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="affiliation"></param>
		/// <param name="role"></param>
		/// <param name="reason"></param>
		public Item(Affiliation affiliation, Role role, string reason) : this(affiliation, role)
		{
			this.Reason = reason;
		}

		/// <summary>
		/// 
		/// </summary>
		public Role Role
		{
			get { return (Role)this.GetAttributeEnum("role", typeof(Role)); }
			set { this.SetAttribute("role", value.ToString()); }
		}

		/// <summary>
		/// 
		/// </summary>
		public Affiliation Affiliation
		{
			get { return (Affiliation)this.GetAttributeEnum("affiliation", typeof(Affiliation)); }
			set { this.SetAttribute("affiliation", value.ToString()); }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Nickname
		{
			get { return this.GetAttribute("nick"); }
			set { this.SetAttribute("nick", value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Reason
		{
			set { this.SetTag("reason", value); }
			get { return this.GetTag("reason"); }
		}

		public Actor Actor
		{
			get
			{
				return this.SelectSingleElement(typeof(Actor)) as Actor;
			}
			set
			{
				if (this.HasTag(typeof(Actor)))
					this.RemoveTag(typeof(Actor));

				if (value != null)
					this.AddChild(value);
			}
		}
	}
}