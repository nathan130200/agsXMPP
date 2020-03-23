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

using AgsXMPP.Protocol.Extensions.nickname;
using AgsXMPP.Protocol.Extensions.primary;

namespace AgsXMPP.Protocol.Client
{
	/// <summary>
	/// Zusammenfassung für Presence.
	/// </summary>
	public class Presence : Base.Stanza
	{
		#region << Constructors >>
		public Presence()
		{
			this.TagName = "presence";
			this.Namespace = Namespaces.CLIENT;
		}

		public Presence(ShowType show, string status) : this()
		{
			this.Show = show;
			this.Status = status;
		}

		public Presence(ShowType show, string status, int priority) : this(show, status)
		{
			this.Priority = priority;
		}
		#endregion

		/// <summary>
		/// The OPTIONAL statuc contains a natural-language description of availability status. 
		/// It is normally used in conjunction with the show element to provide a detailed description of an availability state 
		/// (e.g., "In a meeting").
		/// </summary>
		public string Status
		{
			get { return this.GetTag("status"); }
			set { this.SetTag("status", value); }
		}

		/// <summary>
		/// The type of a presence stanza is OPTIONAL. 
		/// A presence stanza that does not possess a type attribute is used to signal to the server that the sender is online and available 
		/// for communication. If included, the type attribute specifies a lack of availability, a request to manage a subscription 
		/// to another entity's presence, a request for another entity's current presence, or an error related to a previously-sent 
		/// presence stanza.
		/// </summary>
		public PresenceType Type
		{
			get
			{
				return this.GetAttributeEnum<PresenceType>("type");
			}
			set
			{
				// dont add type="available"
				if (value == PresenceType.Available)
					this.RemoveAttribute("type");
				else
					this.SetAttributeEnum("type", value);
			}

		}

		/// <summary>
		/// Error Child Element
		/// </summary>
		public Error Error
		{
			get
			{
				return this.SelectSingleElement(typeof(Error)) as Error;

			}
			set
			{
				if (this.HasTag(typeof(Error)))
					this.RemoveTag(typeof(Error));

				if (value != null)
					this.AddChild(value);
			}
		}

		/// <summary>
		/// The OPTIONAL show element contains non-human-readable XML character data that specifies the particular availability
		/// status of an entity or specific resource.
		/// </summary>
		public ShowType Show
		{
			get { return (ShowType)this.GetTagEnum("show", typeof(ShowType)); }
			set
			{
				if (value != ShowType.NONE)
					this.SetTag("show", value.ToString());
				else
					this.RemoveAttribute("show");
			}
		}

		/// <summary>
		/// The priority level of the resource. The value MUST be an integer between -128 and +127. 
		/// If no priority is provided, a server SHOULD consider the priority to be zero.         
		/// </summary>
		/// <remarks>
		/// For information regarding the semantics of priority values in stanza routing 
		/// within instant messaging and presence applications, refer to Server Rules 
		/// for Handling XML StanzasServer Rules for Handling XML Stanzas.
		/// </remarks>
		public int Priority
		{
			get
			{
				try
				{
					return int.Parse(this.GetTag("priority"));
				}
				catch
				{
					return 0;
				}
			}
			set { this.SetTag("priority", value.ToString()); }
		}

		public x.Delay XDelay
		{
			get { return this.SelectSingleElement(typeof(x.Delay)) as x.Delay; }
			set
			{
				if (this.HasTag(typeof(x.Delay)))
					this.RemoveTag(typeof(x.Delay));

				if (value != null)
					this.AddChild(value);
			}
		}

		public bool IsPrimary
		{
			get
			{
				return this.GetTag(typeof(Primary)) == null ? false : true;
			}
			set
			{
				if (value)
					this.SetTag(typeof(Primary));
				else
					this.RemoveTag(typeof(Primary));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public x.muc.User MucUser
		{
			get { return this.SelectSingleElement(typeof(x.muc.User)) as x.muc.User; }
			set
			{
				if (this.HasTag(typeof(x.muc.User)))
					this.RemoveTag(typeof(x.muc.User));

				if (value != null)
					this.AddChild(value);
			}
		}

		/// <summary>
		/// Nickname Element
		/// </summary>
		public Nickname Nickname
		{
			get
			{
				return this.SelectSingleElement(typeof(Nickname)) as Nickname;
			}
			set
			{
				if (this.HasTag(typeof(Nickname)))
					this.RemoveTag(typeof(Nickname));

				if (value != null)
					this.AddChild(value);
			}
		}
	}
}
