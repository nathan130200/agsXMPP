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

using System;
using System.Text;
using System.Threading;
using AgsXMPP.Events;
using AgsXMPP.Net;
using AgsXMPP.Xml;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP
{
	public delegate void XmlHandler(object sender, string xml);
	public delegate void ErrorHandler(object sender, Exception ex);

	public delegate void XmppConnectionStateHandler(object sender, XmppConnectionState state);
	/// <summary>
	/// Abstract base class XmppConnection.
	/// </summary>
	public abstract class XmppConnection : IXmppConnection
	{
		protected internal EventEmitter<XmppConnectionStateHandler> m_OnXmppConnectionStateChanged = new EventEmitter<XmppConnectionStateHandler>();
		protected internal EventEmitter<XmlHandler> m_OnReadXml = new EventEmitter<XmlHandler>();
		protected internal EventEmitter<XmlHandler> m_OnWriteXml = new EventEmitter<XmlHandler>();
		protected internal EventEmitter<ErrorHandler> m_OnError = new EventEmitter<ErrorHandler>();
		protected internal EventEmitter<SocketDataEventHandler> m_OnReadSocketData = new EventEmitter<SocketDataEventHandler>();
		protected internal EventEmitter<SocketDataEventHandler> m_OnWriteSocketData = new EventEmitter<SocketDataEventHandler>();

		private Timer m_KeepaliveTimer = null;

		#region << Events >>
		/// <summary>
		/// This event just informs about the current state of the XmppConnection
		/// </summary>
		public event XmppConnectionStateHandler OnXmppConnectionStateChanged
		{
			add => this.m_OnXmppConnectionStateChanged.Register(value);
			remove => this.m_OnXmppConnectionStateChanged.Unregister(value);
		}

		/// <summary>
		/// a XML packet or text is received. 
		/// This are no winsock events. The Events get generated from the XML parser
		/// </summary>
		public event XmlHandler OnReadXml
		{
			add => this.m_OnReadXml.Register(value);
			remove => this.m_OnReadXml.Unregister(value);
		}

		/// <summary>
		/// XML or Text is written to the Socket this includes also the keep alive packages (a single space)		
		/// </summary>
		public event XmlHandler OnWriteXml
		{
			add => this.m_OnWriteXml.Register(value);
			remove => this.m_OnWriteXml.Unregister(value);
		}

		public event ErrorHandler OnError
		{
			add => this.m_OnError.Register(value);
			remove => this.m_OnError.Unregister(value);
		}

		/// <summary>
		/// Data received from the Socket
		/// </summary>
		public event SocketDataEventHandler OnReadSocketData
		{
			add => this.m_OnReadSocketData.Register(value);
			remove => this.m_OnReadSocketData.Unregister(value);
		}

		/// <summary>
		/// Data was sent to the socket for sending
		/// </summary>
		public event SocketDataEventHandler OnWriteSocketData
		{
			add => this.m_OnWriteSocketData.Register(value);
			remove => this.m_OnWriteSocketData.Unregister(value);
		}

		#endregion

		#region << Constructors >>
		public XmppConnection()
		{
			this.InitSocket();

			this.m_StreamParser = new StreamParser();
			this.m_StreamParser.OnStreamStart += new StreamHandler(this.StreamParserOnStreamStart);
			this.m_StreamParser.OnStreamEnd += new StreamHandler(this.StreamParserOnStreamEnd);
			this.m_StreamParser.OnStreamElement += new StreamHandler(this.StreamParserOnStreamElement);
			this.m_StreamParser.OnStreamError += new StreamError(this.StreamParserOnStreamError);
		}

		public XmppConnection(SocketConnectionType type) : this()
		{
			this.m_SocketConnectionType = type;
		}
		#endregion

		#region << Properties and Member Variables >>
		private int m_Port = 5222;
		private string m_Server = null;
		private string m_ConnectServer = null;
		private string m_StreamId = "";
		private string m_StreamVersion = "1.0";
		private XmppConnectionState m_ConnectionState = XmppConnectionState.Disconnected;
		private BaseSocket m_ClientSocket = null;
		private StreamParser m_StreamParser = null;
		private SocketConnectionType m_SocketConnectionType = SocketConnectionType.Direct;
		private bool m_AutoResolveConnectServer = false;
		private int m_KeepAliveInterval = 120;
		private bool m_KeepAlive = true;
		/// <summary>
		/// The Port of the remote server for the connection
		/// </summary>
		public int Port
		{
			get { return this.m_Port; }
			set { this.m_Port = value; }
		}

		/// <summary>
		/// domain or ip-address of the remote server for the connection
		/// </summary>
		public string Server
		{
			get { return this.m_Server; }
			set
			{
#if !STRINGPREP
				if (value != null)
					this.m_Server = value.ToLower();
				else
					this.m_Server = null;
#else
                if (value != null)
                    m_Server = Stringprep.NamePrep(value);
                else
                    m_Server = null;
#endif
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string ConnectServer
		{
			get { return this.m_ConnectServer; }
			set { this.m_ConnectServer = value; }
		}

		/// <summary>
		/// the id of the current xmpp xml-stream
		/// </summary>
		public string StreamId
		{
			get { return this.m_StreamId; }
			set { this.m_StreamId = value; }
		}

		/// <summary>
		/// Set to null for old Jabber Protocol without SASL andstream features
		/// </summary>
		public string StreamVersion
		{
			get { return this.m_StreamVersion; }
			set { this.m_StreamVersion = value; }
		}

		public XmppConnectionState State
		{
			get { return this.m_ConnectionState; }
		}

		/// <summary>
		/// Read Online Property ClientSocket
		/// returns the SOcket object used for this connection
		/// </summary>
		public BaseSocket ClientSocket
		{
			get { return this.m_ClientSocket; }
		}

		/// <summary>
		/// the underlaying XMPP StreamParser. Normally you don't need it, but we make it accessible for
		/// low level access to the stream
		/// </summary>
		public StreamParser StreamParser
		{
			get { return this.m_StreamParser; }
		}

		public SocketConnectionType SocketType
		{
			get { return this.m_SocketConnectionType; }
			set
			{
				this.m_SocketConnectionType = value;
				this.InitSocket();
			}
		}

		public bool AutoResolveConnectServer
		{
			get { return this.m_AutoResolveConnectServer; }
			set { this.m_AutoResolveConnectServer = value; }
		}

		/// <summary>
		/// <para>
		/// the keep alive interval in seconds.
		/// Default value is 120
		/// </para>
		/// <para>
		/// Keep alive packets prevent disconenct on NAT and broadband connections which often
		/// disconnect if they are idle.
		/// </para>
		/// </summary>
		public int KeepAliveInterval
		{
			get
			{
				return this.m_KeepAliveInterval;
			}
			set
			{
				this.m_KeepAliveInterval = value;
			}
		}
		/// <summary>
		/// Send Keep Alives (for NAT)
		/// </summary>
		public bool KeepAlive
		{
			get
			{
				return this.m_KeepAlive;
			}
			set
			{
				this.m_KeepAlive = value;
			}
		}
		#endregion

		#region << Socket handers >>
		protected virtual void SocketOnConnect(object sender)
		{
			this.DoChangeXmppConnectionState(XmppConnectionState.Connected);
		}

		protected virtual void SocketOnDisconnect(object sender)
		{

		}

		protected virtual void SocketOnReceive(object sender, byte[] data, int count)
		{

			this.m_OnReadSocketData.Invoke(sender, data, count);

			// put the received bytes to the parser
			lock (this)
			{
				this.StreamParser.Push(data, 0, count);
			}
		}

		protected virtual void SocketOnError(object sender, Exception ex)
		{

		}
		#endregion

		#region << StreamParser Events >>
		protected virtual void StreamParserOnStreamStart(object sender, Node e)
		{
			var xst = e as Protocol.XmppStream;

			//var xml = e.ToString().Trim();
			//xml = xml.Substring(0, xml.Length - 2) + ">";

			this.FireOnReadXml(this, xst.StartTag());

			//if (e is Protocol.XmppStreamError error)
			//{
			//	this.FireOnError(this, new InvalidOperationException($"Stream error received: <{error.Condition}/>"));
			//	return;
			//}

			if (xst != null)
			{
				this.m_StreamId = xst.StreamId;
				this.m_StreamVersion = xst.Version;
			}
		}

		protected virtual void StreamParserOnStreamEnd(object sender, Node e)
		{
			//var tag = e as Element;

			//string qName;
			//if (tag.Prefix == null)
			//	qName = tag.TagName;
			//else
			//	qName = tag.Prefix + ":" + tag.TagName;

			//var xml = "</" + qName + ">";

			this.FireOnReadXml(this, ((Protocol.XmppStream)e).EndTag());
		}

		protected virtual void StreamParserOnStreamElement(object sender, Node e)
		{
			this.FireOnReadXml(this, e.ToString(true));
		}

		protected virtual void StreamParserOnStreamError(object sender, Exception ex)
		{
			this.FireOnError(sender, ex);
		}

		protected virtual void StreamParserOnError(object sender, Exception ex)
		{
			this.FireOnError(sender, ex);
		}

		#endregion

		internal void DoChangeXmppConnectionState(XmppConnectionState state)
		{
			this.m_ConnectionState = state;
			this.m_OnXmppConnectionStateChanged.Invoke(this, state);
		}

		private void InitSocket()
		{
			this.m_ClientSocket = null;

			// Socket Stuff
			if (this.m_SocketConnectionType == SocketConnectionType.HttpPolling)
				this.m_ClientSocket = new PollClientSocket();
			else if (this.m_SocketConnectionType == SocketConnectionType.Bosh)
				this.m_ClientSocket = new BoshClientSocket(this);
			else
				this.m_ClientSocket = new ClientSocket();

			this.m_ClientSocket.OnConnect += new ObjectHandler(this.SocketOnConnect);
			this.m_ClientSocket.OnDisconnect += new ObjectHandler(this.SocketOnDisconnect);
			this.m_ClientSocket.OnReceive += new SocketDataEventHandler(this.SocketOnReceive);
			this.m_ClientSocket.OnError += new ErrorHandler(this.SocketOnError);
		}

		/// <summary>
		/// Starts connecting of the socket
		/// </summary>
		protected virtual void SocketConnect()
		{
			this.DoChangeXmppConnectionState(XmppConnectionState.Connecting);
			this.ClientSocket.Connect();
		}

		protected void SocketConnect(string server, int port)
		{
			this.ClientSocket.Address = server;
			this.ClientSocket.Port = port;
			this.SocketConnect();
		}

		protected void SocketDisconnect()
		{
			this.m_ClientSocket.Disconnect();
		}

		/// <summary>
		/// Send a raw xml element over connection.
		/// </summary>
		/// <param name="xml"></param>
		public void Send(string xml)
		{
			this.FireOnWriteXml(this, xml);
			this.m_ClientSocket.Send(xml);

			this.m_OnWriteSocketData.Invoke(this, Encoding.UTF8.GetBytes(xml), xml.Length);

			// reset keep alive timer if active to make sure the interval is always idle time from the last 
			// outgoing packet
			if (this.m_KeepAlive && this.m_KeepaliveTimer != null)
				this.m_KeepaliveTimer.Change(this.m_KeepAliveInterval * 1000, this.m_KeepAliveInterval * 1000);
		}

		/// <summary>
		/// Send a xml element over the connection.
		/// </summary>
		/// <param name="e">Xml element to send.</param>
		public virtual void Send(Element e)
		{
			string xml = e.ToString(false);

			this.FireOnWriteXml(this, e.ToString(true));
			this.m_ClientSocket.Send(xml);

			this.m_OnWriteSocketData.Invoke(this, Encoding.UTF8.GetBytes(xml), xml.Length);

			// reset keep alive timer if active to make sure the interval is always idle time from the last 
			// outgoing packet
			if (this.m_KeepAlive && this.m_KeepaliveTimer != null)
				this.m_KeepaliveTimer.Change(this.m_KeepAliveInterval * 1000, this.m_KeepAliveInterval * 1000);
		}

		public void Open(string xml)
		{
			this.Send(xml);
		}

		/// <summary>
		/// Send the end of stream
		/// </summary>
		public virtual void Close()
		{
			this.Send("</stream:stream>");
		}

		protected void FireOnReadXml(object sender, string xml)
		{
			if (string.IsNullOrEmpty(xml) || string.IsNullOrWhiteSpace(xml))
				return;

			this.m_OnReadXml.Invoke(sender, xml);
		}

		protected void FireOnWriteXml(object sender, string xml)
		{
			if (string.IsNullOrEmpty(xml) || string.IsNullOrWhiteSpace(xml))
				return;

			this.m_OnWriteXml.Invoke(sender, xml);
		}

		protected void FireOnError(object sender, Exception ex)
		{
			this.m_OnError.Invoke(sender, ex);
		}

		#region << Keepalive Timer functions >>
		protected void CreateKeepAliveTimer()
		{
			// Create the delegate that invokes methods for the timer.
			var timerDelegate = new TimerCallback(this.KeepAliveTick);
			var interval = this.m_KeepAliveInterval * 1000;
			// Create a timer that waits x seconds, then invokes every x seconds.
			this.m_KeepaliveTimer = new Timer(timerDelegate, null, interval, interval);
		}

		protected void DestroyKeepAliveTimer()
		{
			if (this.m_KeepaliveTimer == null)
				return;

			this.m_KeepaliveTimer.Dispose();
			this.m_KeepaliveTimer = null;
		}

		private void KeepAliveTick(object state)
		{
			// Send a Space for Keep Alive
			this.Send(" ");
		}
		#endregion
	}
}