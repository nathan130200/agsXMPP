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
using AgsXMPP.Net;
using AgsXMPP.Xml;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP
{
	public delegate void XmlHandler(object sender, string xml);
	public delegate void ErrorHandler(object sender, Exception ex);

	public delegate void XmppConnectionStateHandler(object sender, XmppConnectionState state);
	/// <summary>
	/// abstract base class XmppConnection.
	/// </summary>
	public abstract class XmppConnection
	{

		private Timer m_KeepaliveTimer = null;

		#region << Events >>
		/// <summary>
		/// This event just informs about the current state of the XmppConnection
		/// </summary>
		public event XmppConnectionStateHandler OnXmppConnectionStateChanged;

		/// <summary>
		/// a XML packet or text is received. 
		/// This are no winsock events. The Events get generated from the XML parser
		/// </summary>
		public event XmlHandler OnReadXml;
		/// <summary>
		/// XML or Text is written to the Socket this includes also the keep alive packages (a single space)		
		/// </summary>
		public event XmlHandler OnWriteXml;

		public event ErrorHandler OnError;

		/// <summary>
		/// Data received from the Socket
		/// </summary>
		public event BaseSocket.OnSocketDataHandler OnReadSocketData;

		/// <summary>
		/// Data was sent to the socket for sending
		/// </summary>
		public event BaseSocket.OnSocketDataHandler OnWriteSocketData;

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
			this.m_StreamParser.OnError += new ErrorHandler(this.StreamParserOnError);
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

		public XmppConnectionState XmppConnectionState
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

		public SocketConnectionType SocketConnectionType
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
		public virtual void SocketOnConnect(object sender)
		{
			this.DoChangeXmppConnectionState(XmppConnectionState.Connected);
		}

		public virtual void SocketOnDisconnect(object sender)
		{

		}

		public virtual void SocketOnReceive(object sender, byte[] data, int count)
		{

			OnReadSocketData?.Invoke(sender, data, count);

			// put the received bytes to the parser
			lock (this)
			{
				this.StreamParser.Push(data, 0, count);
			}
		}

		public virtual void SocketOnError(object sender, Exception ex)
		{

		}
		#endregion

		#region << StreamParser Events >>
		public virtual void StreamParserOnStreamStart(object sender, Node e)
		{
			var xml = e.ToString().Trim();
			xml = xml.Substring(0, xml.Length - 2) + ">";

			this.FireOnReadXml(this, xml);

			var xst = (Protocol.XmppStream)e;
			if (xst != null)
			{
				this.m_StreamId = xst.StreamId;
				this.m_StreamVersion = xst.Version;
			}
		}

		public virtual void StreamParserOnStreamEnd(object sender, Node e)
		{
			var tag = e as Element;

			string qName;
			if (tag.Prefix == null)
				qName = tag.TagName;
			else
				qName = tag.Prefix + ":" + tag.TagName;

			var xml = "</" + qName + ">";

			this.FireOnReadXml(this, xml);
		}

		public virtual void StreamParserOnStreamElement(object sender, Node e)
		{
			this.FireOnReadXml(this, e.ToString());
		}
		public virtual void StreamParserOnStreamError(object sender, Exception ex)
		{
		}
		public virtual void StreamParserOnError(object sender, Exception ex)
		{
			this.FireOnError(sender, ex);
		}
		#endregion

		internal void DoChangeXmppConnectionState(XmppConnectionState state)
		{
			this.m_ConnectionState = state;

			OnXmppConnectionStateChanged?.Invoke(this, state);
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
			this.m_ClientSocket.OnReceive += new BaseSocket.OnSocketDataHandler(this.SocketOnReceive);
			this.m_ClientSocket.OnError += new ErrorHandler(this.SocketOnError);
		}

		/// <summary>
		/// Starts connecting of the socket
		/// </summary>
		public virtual void SocketConnect()
		{
			this.DoChangeXmppConnectionState(XmppConnectionState.Connecting);
			this.ClientSocket.Connect();
		}

		public void SocketConnect(string server, int port)
		{
			this.ClientSocket.Address = server;
			this.ClientSocket.Port = port;
			this.SocketConnect();
		}

		public void SocketDisconnect()
		{
			this.m_ClientSocket.Disconnect();
		}

		/// <summary>
		/// Send a xml string over the XmppConnection
		/// </summary>
		/// <param name="xml"></param>
		public void Send(string xml)
		{
			this.FireOnWriteXml(this, xml);
			this.m_ClientSocket.Send(xml);

			OnWriteSocketData?.Invoke(this, Encoding.UTF8.GetBytes(xml), xml.Length);

			// reset keep alive timer if active to make sure the interval is always idle time from the last 
			// outgoing packet
			if (this.m_KeepAlive && this.m_KeepaliveTimer != null)
				this.m_KeepaliveTimer.Change(this.m_KeepAliveInterval * 1000, this.m_KeepAliveInterval * 1000);
		}

		/// <summary>
		/// Send a xml element over the XmppConnection
		/// </summary>
		/// <param name="e"></param>
		public virtual void Send(Element e)
		{
			this.Send(e.ToString());
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
			OnReadXml?.Invoke(sender, xml);
		}

		protected void FireOnWriteXml(object sender, string xml)
		{
			OnWriteXml?.Invoke(sender, xml);
		}

		protected void FireOnError(object sender, Exception ex)
		{
			OnError?.Invoke(sender, ex);
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