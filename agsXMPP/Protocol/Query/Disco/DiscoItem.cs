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

using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Query.Disco
{
	public enum DiscoAction
	{
		None = -1,

		[XmppEnumMember("remove")]
		Remove,

		[XmppEnumMember("update")]
		Update
	}

	/// <summary>
	///
	/// </summary>
	public class DiscoItem : Element
	{
		public DiscoItem()
		{
			this.TagName = "item";
			this.Namespace = URI.DISCO_ITEMS;
		}

		public Jid Jid
		{
			get { return new Jid(this.GetAttribute("jid")); }
			set { this.SetAttribute("jid", value.ToString()); }
		}

		public string Name
		{
			get { return this.GetAttribute("name"); }
			set { this.SetAttribute("name", value); }
		}

		public string Node
		{
			get { return this.GetAttribute("node"); }
			set { this.SetAttribute("node", value); }
		}

		public DiscoAction Action
		{
			get => this.GetAttributeEnum<DiscoAction>("action");
			set
			{
				if (value == DiscoAction.None)
					this.RemoveAttribute("action");
				else
					this.SetAttributeEnum("action", value);
			}
		}
	}
}
