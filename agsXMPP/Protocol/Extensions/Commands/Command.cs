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

using AgsXMPP.Protocol.x.data;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Extensions.Commands
{
	public class Command : Element
	{
		#region << Constructors >>
		public Command()
		{
			this.TagName = "command";
			this.Namespace = Namespaces.COMMANDS;
		}

		public Command(string node) : this()
		{
			this.Node = node;
		}

		public Command(Action action) : this()
		{
			this.Action = action;
		}

		public Command(Status status) : this()
		{
			this.Status = status;
		}

		public Command(string node, string sessionId) : this(node)
		{
			this.SessionId = sessionId;
		}

		public Command(string node, string sessionId, Action action) : this(node, sessionId)
		{
			this.Action = action;
		}

		public Command(string node, string sessionId, Status status) : this(node, sessionId)
		{
			this.Status = status;
		}

		public Command(string node, string sessionId, Action action, Status status) : this(node, sessionId, action)
		{
			this.Status = status;
		}
		#endregion

		public Action Action
		{
			get
			{
				return this.GetAttributeEnum<Action>("action");
			}
			set
			{
				if (value == Action.None)
					this.RemoveAttribute("action");
				else
					this.SetAttribute("action", value.ToString());
			}
		}

		public Status Status
		{
			get
			{
				return this.GetAttributeEnum<Status>("status");
			}
			set
			{
				if (value == Status.None)
					this.RemoveAttribute("status");
				else
					this.SetAttributeEnum("status", value);
			}
		}


		// <xs:attribute name='node' type='xs:string' use='required'/>

		/// <summary>
		/// Node is Required
		/// </summary>
		public string Node
		{
			get { return this.GetAttribute("node"); }
			set { this.SetAttribute("node", value); }
		}

		// <xs:attribute name='sessionid' type='xs:string' use='optional'/>
		public string SessionId
		{
			get { return this.GetAttribute("sessionid"); }
			set { this.SetAttribute("sessionid", value); }
		}

		/// <summary>
		/// The X-Data Element
		/// </summary>
		public Data Data
		{
			get
			{
				return this.SelectSingleElement(typeof(Data)) as Data;

			}
			set
			{
				if (this.HasTag(typeof(Data)))
					this.RemoveTag(typeof(Data));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Note Note
		{
			get
			{
				return this.SelectSingleElement(typeof(Note)) as Note;

			}
			set
			{
				if (this.HasTag(typeof(Note)))
					this.RemoveTag(typeof(Note));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Actions Actions
		{
			get
			{
				return this.SelectSingleElement(typeof(Actions)) as Actions;

			}
			set
			{
				if (this.HasTag(typeof(Actions)))
					this.RemoveTag(typeof(Actions));

				if (value != null)
					this.AddChild(value);
			}
		}

	}
}
