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

namespace AgsXMPP.Protocol.Query.Privacy
{
	/// <summary>
	/// This class represents a rule which is used for blocking communication
	/// </summary>
	public class Item : Element
	{

		#region << Constructors >>
		/// <summary>
		/// Default Contructor
		/// </summary>
		public Item()
		{
			this.TagName = "item";
			this.Namespace = URI.IQ_PRIVACY;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="order"></param>
		public Item(ActionType action, int order) : this()
		{
			this.Action = action;
			this.Order = order;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="order"></param>
		/// <param name="block"></param>
		public Item(ActionType action, int order, StanzaType stanza) : this(action, order)
		{
			this.Stanza = stanza;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="order"></param>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public Item(ActionType action, int order, ItemType type, string value) : this(action, order)
		{
			this.Type = type;
			this.RuleValue = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="order"></param>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <param name="block"></param>
		public Item(ActionType action, int order, ItemType type, string value, StanzaType stanza) : this(action, order, type, value)
		{
			this.Stanza = stanza;
		}
		#endregion

		public ActionType Action
		{
			get => this.GetAttributeEnum<ActionType>("action");
			set => this.SetAttributeEnum("action", value);
		}

		public ItemType Type
		{
			get => this.GetAttributeEnum<ItemType>("type");
			set
			{
				if (value == ItemType.None)
					this.RemoveAttribute("type");
				else
					this.SetAttributeEnum("type", value);
			}
		}

		/// <summary>
		/// The order of this rule
		/// </summary>
		public int Order
		{
			get { return this.GetAttributeInt("order"); }
			set { this.SetAttribute("order", value); }
		}

		/// <summary>
		/// The value to match of this rule
		/// </summary>
		public string RuleValue
		{
			get { return this.GetAttribute("value"); }
			set { this.SetAttribute("value", value); }
		}

		/// <summary>
		/// Block Iq stanzas
		/// </summary>
		public bool BlockIq
		{
			get { return this.HasTag("iq"); }
			set
			{
				if (value)
					this.SetTag("iq");
				else
					this.RemoveTag("iq");
			}
		}

		/// <summary>
		/// Block messages
		/// </summary>
		public bool BlockMessage
		{
			get { return this.HasTag("message"); }
			set
			{
				if (value)
					this.SetTag("message");
				else
					this.RemoveTag("message");
			}
		}

		/// <summary>
		/// Block incoming presence
		/// </summary>
		public bool BlockIncomingPresence
		{
			get { return this.HasTag("presence-in"); }
			set
			{
				if (value)
					this.SetTag("presence-in");
				else
					this.RemoveTag("presence-in");
			}
		}

		/// <summary>
		/// Block outgoing presence
		/// </summary>
		public bool BlockOutgoingPresence
		{
			get { return this.HasTag("presence-out"); }
			set
			{
				if (value)
					this.SetTag("presence-out");
				else
					this.RemoveTag("presence-out");
			}
		}

		/// <summary>
		/// which stanzas should be blocked?
		/// </summary>
		public StanzaType Stanza
		{
			get
			{
				var result = StanzaType.All;

				if (this.BlockIq)
					result |= StanzaType.Iq;
				if (this.BlockMessage)
					result |= StanzaType.Message;
				if (this.BlockIncomingPresence)
					result |= StanzaType.IncomingPresence;
				if (this.BlockOutgoingPresence)
					result |= StanzaType.OutgoingPresence;

				return result;
			}
			set
			{
				if (value == StanzaType.All)
				{
					// Block All Communications
					this.BlockIq = false;
					this.BlockMessage = false;
					this.BlockIncomingPresence = false;
					this.BlockOutgoingPresence = false;
				}
				else
				{
					this.BlockIq = ((value & StanzaType.Iq) == StanzaType.Iq);
					this.BlockMessage = ((value & StanzaType.Message) == StanzaType.Message);
					this.BlockIncomingPresence = ((value & StanzaType.IncomingPresence) == StanzaType.IncomingPresence);
					this.BlockOutgoingPresence = ((value & StanzaType.OutgoingPresence) == StanzaType.OutgoingPresence);
				}
			}
		}
	}
}