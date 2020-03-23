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

#region Using directives


#endregion

namespace AgsXMPP.Protocol.Component
{
	/// <summary>
	/// Summary description for Message.
	/// </summary>
	public class Message : Client.Message
	{
		#region << Constructors >>
		public Message()
			: base()
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to)
			: base(to)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, string body)
			: base(to, body)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, Jid from)
			: base(to, from)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(string to, string body)
			: base(to, body)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, string body, string subject)
			: base(to, body, subject)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(string to, string body, string subject)
			: base(to, body, subject)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(string to, string body, string subject, string thread)
			: base(to, body, subject, thread)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, string body, string subject, string thread)
			: base(to, body, subject, thread)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(string to, Client.MessageType type, string body)
			: base(to, type, body)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, Client.MessageType type, string body)
			: base(to, type, body)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(string to, Client.MessageType type, string body, string subject)
			: base(to, type, body, subject)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, Client.MessageType type, string body, string subject)
			: base(to, type, body, subject)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(string to, Client.MessageType type, string body, string subject, string thread)
			: base(to, type, body, subject, thread)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, Client.MessageType type, string body, string subject, string thread)
			: base(to, type, body, subject, thread)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, Jid from, string body)
			: base(to, from, body)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, Jid from, string body, string subject)
			: base(to, from, body, subject)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, Jid from, string body, string subject, string thread)
			: base(to, from, body, subject, thread)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, Jid from, Client.MessageType type, string body)
			: base(to, from, type, body)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, Jid from, Client.MessageType type, string body, string subject)
			: base(to, from, type, body, subject)
		{
			this.Namespace = Namespaces.ACCEPT;
		}

		public Message(Jid to, Jid from, Client.MessageType type, string body, string subject, string thread)
			: base(to, from, type, body, subject, thread)
		{
			this.Namespace = Namespaces.ACCEPT;
		}
		#endregion

		/// <summary>
		/// Error Child Element
		/// </summary>
		public new Error Error
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
	}
}
