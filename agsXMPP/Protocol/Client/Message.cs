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

using System;
using AgsXMPP.Protocol.Extensions.ChatStates;
using AgsXMPP.Protocol.Extensions.nickname;
using AgsXMPP.Protocol.Extensions.Shim;
using AgsXMPP.Protocol.Extensions.XHtml;
using AgsXMPP.Protocol.x;

namespace AgsXMPP.Protocol.Client
{
	/// <summary>
	/// This class represents a XMPP message.
	/// </summary>
	public class Message : Base.Stanza
	{
		#region << Constructors >>
		public Message()
		{
			this.TagName = "message";
			this.Namespace = Namespaces.CLIENT;
		}

		public Message(Jid to) : this()
		{
			this.To = to;
		}
		public Message(Jid to, string body) : this(to)
		{
			this.Body = body;
		}

		public Message(Jid to, Jid from) : this()
		{
			this.To = to;
			this.From = from;
		}

		public Message(string to, string body) : this()
		{
			this.To = new Jid(to);
			this.Body = body;
		}

		public Message(Jid to, string body, string subject) : this()
		{
			this.To = to;
			this.Body = body;
			this.Subject = subject;
		}

		public Message(string to, string body, string subject) : this()
		{
			this.To = new Jid(to);
			this.Body = body;
			this.Subject = subject;
		}

		public Message(string to, string body, string subject, string thread) : this()
		{
			this.To = new Jid(to);
			this.Body = body;
			this.Subject = subject;
			this.Thread = thread;
		}

		public Message(Jid to, string body, string subject, string thread) : this()
		{
			this.To = to;
			this.Body = body;
			this.Subject = subject;
			this.Thread = thread;
		}

		public Message(string to, MessageType type, string body) : this()
		{
			this.To = new Jid(to);
			this.Type = type;
			this.Body = body;
		}

		public Message(Jid to, MessageType type, string body) : this()
		{
			this.To = to;
			this.Type = type;
			this.Body = body;
		}

		public Message(string to, MessageType type, string body, string subject) : this()
		{
			this.To = new Jid(to);
			this.Type = type;
			this.Body = body;
			this.Subject = subject;
		}

		public Message(Jid to, MessageType type, string body, string subject) : this()
		{
			this.To = to;
			this.Type = type;
			this.Body = body;
			this.Subject = subject;
		}

		public Message(string to, MessageType type, string body, string subject, string thread) : this()
		{
			this.To = new Jid(to);
			this.Type = type;
			this.Body = body;
			this.Subject = subject;
			this.Thread = thread;
		}

		public Message(Jid to, MessageType type, string body, string subject, string thread) : this()
		{
			this.To = to;
			this.Type = type;
			this.Body = body;
			this.Subject = subject;
			this.Thread = thread;
		}

		public Message(Jid to, Jid from, string body) : this()
		{
			this.To = to;
			this.From = from;
			this.Body = body;
		}

		public Message(Jid to, Jid from, string body, string subject) : this()
		{
			this.To = to;
			this.From = from;
			this.Body = body;
			this.Subject = subject;
		}

		public Message(Jid to, Jid from, string body, string subject, string thread) : this()
		{
			this.To = to;
			this.From = from;
			this.Body = body;
			this.Subject = subject;
			this.Thread = thread;
		}

		public Message(Jid to, Jid from, MessageType type, string body) : this()
		{
			this.To = to;
			this.From = from;
			this.Type = type;
			this.Body = body;
		}

		public Message(Jid to, Jid from, MessageType type, string body, string subject) : this()
		{
			this.To = to;
			this.From = from;
			this.Type = type;
			this.Body = body;
			this.Subject = subject;
		}

		public Message(Jid to, Jid from, MessageType type, string body, string subject, string thread) : this()
		{
			this.To = to;
			this.From = from;
			this.Type = type;
			this.Body = body;
			this.Subject = subject;
			this.Thread = thread;
		}

		#endregion

		#region << Properties >>
		/// <summary>
		/// The body of the message. This contains the message text.
		/// </summary>
		public string Body
		{
			set { this.SetTag("body", value); }
			get { return this.GetTag("body"); }
		}

		/// <summary>
		/// subject of this message. Its like a subject in a email. The Subject is optional.
		/// </summary>
		public string Subject
		{
			set { this.SetTag("subject", value); }
			get { return this.GetTag("subject"); }
		}

		/// <summary>
		/// messages and conversations could be threaded. You can compare this with threads in newsgroups or forums.
		/// Threads are optional.
		/// </summary>
		public string Thread
		{
			set { this.SetTag("thread", value); }
			get { return this.GetTag("thread"); }
		}

		/// <summary>
		/// message type (chat, groupchat, normal, headline or error).
		/// </summary>
		public MessageType Type
		{
			get
			{
				return this.GetAttributeEnum<MessageType>("type");
			}
			set
			{
				if (value == MessageType.None)
					this.RemoveAttribute("type");
				else
					this.SetAttribute("type", value.ToString());
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
		/// The html part of the message if you want to support the html-im Jep. This part of the message is optional.
		/// </summary>
		public Html Html
		{
			get { return (Html)this.SelectSingleElement(typeof(Html)); }
			set
			{
				this.RemoveTag(typeof(Html));
				if (value != null)
					this.AddChild(value);
			}
		}

		/// <summary>
		/// The event Element for JEP-0022 Message events
		/// </summary>
		public Event XEvent
		{
			get
			{
				return this.SelectSingleElement(typeof(Event)) as Event;
			}
			set
			{
				if (this.HasTag(typeof(Event)))
					this.RemoveTag(typeof(Event));

				if (value != null)
					this.AddChild(value);
			}
		}


		/// <summary>
		/// The event Element for JEP-0022 Message events
		/// </summary>
		public Delay XDelay
		{
			get
			{
				return this.SelectSingleElement(typeof(Delay)) as Delay;
			}
			set
			{
				if (this.HasTag(typeof(Delay)))
					this.RemoveTag(typeof(Delay));

				if (value != null)
					this.AddChild(value);
			}
		}


		/// <summary>
		/// Stanza Headers and Internet Metadata
		/// </summary>
		public Headers Headers
		{
			get
			{
				return this.SelectSingleElement(typeof(Headers)) as Headers;
			}
			set
			{
				if (this.HasTag(typeof(Headers)))
					this.RemoveTag(typeof(Headers));

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

		#region << Chatstate Properties >>   

		public ChatStateType Chatstate
		{
			get
			{
				if (this.HasTag(typeof(Active)))
					return ChatStateType.active;
				else if (this.HasTag(typeof(Inactive)))
					return ChatStateType.inactive;
				else if (this.HasTag(typeof(Composing)))
					return ChatStateType.composing;
				else if (this.HasTag(typeof(Paused)))
					return ChatStateType.paused;
				else if (this.HasTag(typeof(Gone)))
					return ChatStateType.gone;
				else
					return ChatStateType.None;
			}
			set
			{
				this.RemoveChatstate();
				switch (value)
				{
					case ChatStateType.active:
						this.AddChild(new Active());
						break;
					case ChatStateType.inactive:
						this.AddChild(new Inactive());
						break;
					case ChatStateType.composing:
						this.AddChild(new Composing());
						break;
					case ChatStateType.paused:
						this.AddChild(new Paused());
						break;
					case ChatStateType.gone:
						this.AddChild(new Gone());
						break;
				}
			}
		}

		private void RemoveChatstate()
		{
			this.RemoveTag(typeof(Active));
			this.RemoveTag(typeof(Inactive));
			this.RemoveTag(typeof(Composing));
			this.RemoveTag(typeof(Paused));
			this.RemoveTag(typeof(Gone));
		}
		#endregion

		#endregion

		#region << Methods and Functions >>
#if !CF
		/// <summary>
		/// Create a new unique Thread indendifier
		/// </summary>
		/// <returns></returns>
		public string CreateNewThread()
		{
			var guid = Guid.NewGuid().ToString().ToLower();
			this.Thread = guid;

			return guid;
		}
#endif
		#endregion
	}
}