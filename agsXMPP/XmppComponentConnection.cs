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
using System.Text;

using AgsXMPP.Protocol.Component;
using AgsXMPP.Xml.Dom;


namespace AgsXMPP
{
	/// <summary>
	/// <para>
	/// use this class to write components that connect to a Jabebr/XMPP server
	/// </para>
	/// <para>
	/// http://www.xmpp.org/extensions/xep-0114.html
	/// </para>
	/// </summary>
	public class XmppComponentConnection : XmppConnection
	{
		// This route stuff is old undocumented jabberd(2) stuff. hopefully we can get rid of this one day
		// or somebody writes up and XEP
		public delegate void RouteHandler(object sender, Route r);

		public delegate void ComponentIQHandler(object sender, IQ iq);

		private bool m_CleanUpDone;
		private bool m_StreamStarted;

		#region << Constructors >>
		/// <summary>
		/// Creates a new Component Connection to a given server and port
		/// </summary>
		public XmppComponentConnection()
		{
			this.m_IqGrabber = new IqGrabber(this);
		}

		/// <summary>
		/// Creates a new Component Connection to a given server and port
		/// </summary>
		/// <param name="server">host/ip of the listening server</param>
		/// <param name="port">port the server listens for the connection</param>
		public XmppComponentConnection(string server, int port) : this()
		{
			this.Server = server;
			this.Port = port;
		}

		/// <summary>
		/// Creates a new Component Connection to a given server, port and password (secret)
		/// </summary>
		/// <param name="server">host/ip of the listening server</param>
		/// <param name="port">port the server listens for the connection</param>
		/// <param name="password">password</param>
		public XmppComponentConnection(string server, int port, string password) : this(server, port)
		{
			this.Password = password;
		}
		#endregion

		#region << Properties and Member Variables >>

		private string m_Password = null;
		private bool m_Authenticated = false;
		private Jid m_ComponentJid = null;
		private IqGrabber m_IqGrabber = null;

		public string Password
		{
			get { return this.m_Password; }
			set { this.m_Password = value; }
		}

		/// <summary>
		/// Are we Authenticated to the server? This is readonly and set by the library
		/// </summary>
		public bool Authenticated
		{
			get { return this.m_Authenticated; }
		}

		/// <summary>
		/// The Domain of the component.
		/// <para>
		/// eg: <c>jabber.ag-software.de</c>
		/// </para>
		/// </summary>		
		public Jid ComponentJid
		{
			get { return this.m_ComponentJid; }
			set { this.m_ComponentJid = value; }
		}

		public IqGrabber IqGrabber
		{
			get { return this.m_IqGrabber; }
			set { this.m_IqGrabber = value; }
		}
		#endregion

		#region << Events >>
		// public event ErrorHandler			OnError;

		/// <summary>
		/// connection is authenticated now and ready for receiving Route, Log and Xdb Packets
		/// </summary>
		public event ObjectHandler OnLogin;

		/// <summary>
		/// Fired when connection closes.
		/// </summary>
		public event ObjectHandler OnClose;

		/// <summary>
		/// handler for incoming routet packtes from the server
		/// </summary>
		public event RouteHandler OnRoute;

		/// <summary>
		/// Event that occurs on authentication errors
		/// e.g. wrong password, user doesnt exist etc...
		/// </summary>
		public event XmppElementHandler OnAuthError;

		/// <summary>
		/// Stream errors &lt;stream:error/&gt;
		/// </summary>
		public event XmppElementHandler OnStreamError;

		/// <summary>
		/// Event occurs on Socket Errors
		/// </summary>
		public event ErrorHandler OnSocketError;

		/// <summary>
		/// Event occurs on IQ arrives.
		/// </summary>        
		public event ComponentIQHandler OnIq;

		/// <summary>
		/// We received a message. This could be a chat message, headline, normal message or a groupchat message. 
		/// There are also XMPP extension which are embedded in messages. 
		/// e.g. X-Data forms.
		/// </summary>
		public event MessageHandler OnMessage;

		/// <summary>
		/// We received a presence from a contact or chatroom.
		/// Also subscriptions is handles in this event.
		/// </summary>
		public event PresenceHandler OnPresence;

		#endregion

		public void Open()
		{
			this._Open();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="port"></param>
		public void Open(string server, int port)
		{
			this.Server = server;
			this.Port = port;
			this._Open();
		}

		private void _Open()
		{
			this.m_CleanUpDone = false;
			this.m_StreamStarted = false;

			if (this.ConnectServer == null)
				this.SocketConnect(this.Server, this.Port);
			else
				this.SocketConnect(this.ConnectServer, this.Port);
		}

		private void SendOpenStream()
		{
			// <stream:stream
			// xmlns='jabber:component:accept'
			// xmlns:stream='http://etherx.jabber.org/streams'
			// to='shakespeare.lit'>
			var sb = new StringBuilder();

			//sb.Append("<?xml version='1.0'?>");
			sb.Append("<stream:stream");

			if (this.m_ComponentJid != null)
				sb.Append(" to='" + this.m_ComponentJid.ToString() + "'");

			sb.Append(" xmlns='" + URI.ACCEPT + "'");
			sb.Append(" xmlns:stream='" + URI.STREAM + "'");

			sb.Append(">");

			this.Open(sb.ToString());
		}

		private void Login()
		{
			// Send Handshake
			this.Send(new Handshake(this.m_Password, this.StreamId));
		}

		#region << Stream Parser events >>
		public override void StreamParserOnStreamStart(object sender, Node e)
		{
			base.StreamParserOnStreamStart(sender, e);

			this.m_StreamStarted = true;

			this.Login();
		}

		public override void StreamParserOnStreamEnd(object sender, Node e)
		{
			base.StreamParserOnStreamEnd(sender, e);

			if (!this.m_CleanUpDone)
				this.CleanupSession();
		}

		public override void StreamParserOnStreamElement(object sender, Node e)
		{
			base.StreamParserOnStreamElement(sender, e);

			if (e is Handshake)
			{
				this.m_Authenticated = true;

				OnLogin?.Invoke(this);

				if (this.KeepAlive)
					this.CreateKeepAliveTimer();
			}
			else if (e is Route)
			{
				OnRoute?.Invoke(this, e as Route);
			}
			else if (e is Protocol.XmppStreamError)
			{
				var streamErr = e as Protocol.XmppStreamError;
				switch (streamErr.Condition)
				{
					// Auth errors are important for the users here, so throw catch auth errors
					// in a separate event here
					case Protocol.XmppStreamErrorCondition.NotAuthorized:
						// Authentication Error
						OnAuthError?.Invoke(this, e as Element);
						break;
					default:
						OnStreamError?.Invoke(this, e as Element);
						break;
				}
			}
			else if (e is Message)
			{
				OnMessage?.Invoke(this, e as Message);
			}
			else if (e is Presence)
			{
				OnPresence?.Invoke(this, e as Presence);
			}
			else if (e is IQ)
			{
				OnIq?.Invoke(this, e as IQ);
			}

		}
		#endregion

		#region << ClientSocket Events >>
		public override void SocketOnConnect(object sender)
		{
			base.SocketOnConnect(sender);

			this.SendOpenStream();
		}

		public override void SocketOnDisconnect(object sender)
		{
			base.SocketOnDisconnect(sender);

			if (!this.m_CleanUpDone)
				this.CleanupSession();
		}

		public override void SocketOnError(object sender, Exception ex)
		{
			base.SocketOnError(sender, ex);

			if (this.m_StreamStarted && !this.m_CleanUpDone)
				this.CleanupSession();

			OnSocketError?.Invoke(this, ex);

		}
		#endregion

		public override void Send(Element e)
		{
			// this is a hack to not send the xmlns="jabber:component:accept" with all packets                
			//var dummyEl = new Element("a");
			//dummyEl.Namespace = Namespaces.ACCEPT;

			//dummyEl.AddChild(e);
			//var toSend = dummyEl.ToString();

			//this.Send(toSend.Substring(35, toSend.Length - 35 - 4));

			this.Send(e.ToString());
		}

		private void CleanupSession()
		{
			// This cleanup has only to be done if we were able to connect and teh XMPP Stream was started
			this.DestroyKeepAliveTimer();
			this.m_CleanUpDone = true;
			this.StreamParser.Reset();

			this.m_IqGrabber.Clear();

			OnClose?.Invoke(this);
		}
	}
}
