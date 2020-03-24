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
using System.Collections;
using System.Net.Sockets;
using System.Text;

using AgsXMPP.Exceptions;
using AgsXMPP.Net;
using AgsXMPP.Net.Dns;
using AgsXMPP.Protocol.Client;
using AgsXMPP.Protocol.Extensions.Caps;
using AgsXMPP.Protocol.Extensions.Compression;
using AgsXMPP.Protocol.Query.Agent;
using AgsXMPP.Protocol.Query.Auth;
using AgsXMPP.Protocol.Query.Disco;
using AgsXMPP.Protocol.Query.Register;
using AgsXMPP.Protocol.Query.Roster;
using AgsXMPP.Protocol.Stream;
using AgsXMPP.Sasl;
using AgsXMPP.Xml.Dom;

#if MONOSSL
using  Mono.Security.Protocol.Tls;
#endif

#if SSL || MONOSSL || BCCRYPTO
using AgsXMPP.Protocol.Tls;
#endif

namespace AgsXMPP
{
	public delegate void ObjectHandler(object sender);
	public delegate void XmppElementHandler(object sender, Element e);

	/// <summary>
	/// Summary description for XmppClient.
	/// </summary>
	public class XmppClientConnection : XmppConnection
	{

		//const string SRV_RECORD_PREFIX = "_xmpp-client._tcp.";

		// Delegates		
		public delegate void RosterHandler(object sender, RosterItem item);
		public delegate void AgentHandler(object sender, Agent agent);

		private SaslHandler m_SaslHandler = null;

		private bool m_CleanUpDone;
		private bool m_StreamStarted;

		private SRVRecord[] _SRVRecords;
		private SRVRecord _currentSRVRecord;


		#region << Properties and Member Variables >>
		private string m_ClientLanguage = "en";
		private string m_ServerLanguage = null;
		private string m_Username = "";
		private string m_Password = "";
		private string m_Resource = "agsXMPP";
		private string m_Status = "";
		private int m_Priority = 5;
		private ShowType m_Show = ShowType.NONE;
		private bool m_AutoRoster = true;
		private bool m_AutoAgents = true;
		private bool m_AutoPresence = true;

		private bool m_UseSSL = false;
#if (CF || CF_2) && !BCCRYPTO
        private     bool                    m_UseStartTLS       = false;
#else
		private bool m_UseStartTLS = true;
#endif
		private bool m_UseCompression = false;
		internal bool m_Binded = false;
		private bool m_Authenticated = false;

		private IqGrabber m_IqGrabber = null;
		private MessageGrabber m_MessageGrabber = null;
		private PresenceGrabber m_PresenceGrabber = null;
		private bool m_RegisterAccount = false;
		private PresenceManager m_PresenceManager;
		private RosterManager m_RosterManager;


		private Capabilities m_Capabilities = new Capabilities();
		private string m_ClientVersion = "1.0";
		private bool m_EnableCapabilities = false;

		private DiscoInfo m_DiscoInfo = new DiscoInfo();


		/// <summary>
		/// The prefered Client Language Attribute
		/// </summary>
		/// <seealso cref="Base.XmppPacket.Language"/>
		public string ClientLanguage
		{
			get { return this.m_ClientLanguage; }
			set { this.m_ClientLanguage = value; }
		}

		/// <summary>
		/// The language which the server decided to use.
		/// </summary>
		/// <seealso cref="Base.XmppPacket.Language"/>
		public string ServerLanguage
		{
			get { return this.m_ServerLanguage; }
		}

		/// <summary>
		/// the username that is used to authenticate to the xmpp server
		/// </summary>
		public string Username
		{
			get { return this.m_Username; }
			set
			{
				// first Encode the user/node
				this.m_Username = value;

				var tmpUser = Jid.EscapeNode(value);
#if !STRINGPREP
				if (value != null)
					this.m_Username = tmpUser.ToLower();
				else
					this.m_Username = null;
#else
                if (value != null)
                    m_Username = Stringprep.NodePrep(tmpUser);
                else
                    m_Username = null;
#endif

			}
		}

		/// <summary>
		/// the password that is used to authenticate to the xmpp server
		/// </summary>
		public string Password
		{
			get { return this.m_Password; }
			set { this.m_Password = value; }
		}

		/// <summary>
		/// the resource for this connection each connection to the server with the same jid needs a unique resource.
		/// You can also set <code>Resource = null</code> and the server will assign a random Resource for you.
		/// </summary>
		public string Resource
		{
			get { return this.m_Resource; }
			set { this.m_Resource = value; }
		}

		/// <summary>
		/// our XMPP id build from Username, Server and Resource Property (user@server/resourcee)
		/// </summary>
		public Jid MyJID
		{
			get
			{
				return this.BuildMyJid();
			}
		}

		/// <summary>
		/// The status message of this connection which is sent with the presence packets.
		/// </summary>
		/// <remarks>
		/// you have to call the method <b>SendMyPresence</b> to send your updated presence to the server.        
		/// </remarks>
		public string Status
		{
			get
			{
				return this.m_Status;
			}
			set
			{
				this.m_Status = value;
			}
		}

		/// <summary>
		/// The priority of this connection send with the presence packets.
		/// The OPTIONAL priority element contains non-human-readable XML character data that specifies the priority level 
		/// of the resource. The value MUST be an integer between -128 and +127. If no priority is provided, a server 
		/// SHOULD consider the priority to be zero.        
		/// </summary>
		/// <remarks>you have to call the method <b>SendMyPresence</b> to send your updated presence to the server.</remarks>
		public int Priority
		{
			get { return this.m_Priority; }
			set
			{
				if (value > -128 && value < 128)
					this.m_Priority = value;
				else
					throw new ArgumentException("The value MUST be an integer between -128 and +127");
			}
		}

		/// <summary>
		/// change the showtype. 
		/// </summary>
		/// <remarks>you have to call the method <b>SendMyPresence</b> to send your updated presence to the server.</remarks>
		public ShowType Show
		{
			get { return this.m_Show; }
			set { this.m_Show = value; }
		}

		/// <summary>
		/// If set to true then the Roster (contact list) is requested automatically after sucessful login. 
		/// Set this property to false if you don't want to receive your contact list, or request it manual. 
		/// To save bandwidth is makes sense to cache the contact list and don't receive it on each login.
		/// </summary>
		/// <remarks>default value is <b>true</b></remarks>
		public bool AutoRoster
		{
			get { return this.m_AutoRoster; }
			set { this.m_AutoRoster = value; }
		}

		/// <summary>
		/// Sends the presence Automatically after successful login.
		/// This property works only in combination with AutoRoster (AutoRoster = true).
		/// </summary>
		public bool AutoPresence
		{
			get { return this.m_AutoPresence; }
			set { this.m_AutoPresence = value; }
		}



		/// <summary>
		/// If set to true then the Agents are requested automatically after sucessful login. 
		/// Set this property to false if you don't use agents at all, or if you request them manual.
		/// </summary>
		/// <remarks>default value is <b>true</b></remarks>
		public bool AutoAgents
		{
			get { return this.m_AutoAgents; }
			set { this.m_AutoAgents = value; }
		}

		/// <summary>
		/// use "old style" ssl for this connection (Port 5223).
		/// </summary>
		public bool UseSSL
		{
			get { return this.m_UseSSL; }

#if SSL || MONOSSL
			set
			{
				// Only one of both can be true
				this.m_UseSSL = value;
				if (value == true)
					this.m_UseStartTLS = false;
			}
#endif
		}

		/// <summary>
		/// use Start-TLS on this connection when the server supports it. Make sure UseSSL is false when 
		/// you want to use this feature.
		/// </summary>
		public bool UseStartTLS
		{
			get { return this.m_UseStartTLS; }

#if SSL || MONOSSL || BCCRYPTO
			set
			{
				// Only one of both can be true
				this.m_UseStartTLS = value;
				if (value == true)
					this.m_UseSSL = false;
			}
#endif
		}

		/// <summary>
		/// Use Stream compression to save bandwidth?
		/// This should not be used in combination with StartTLS,
		/// because TLS has build in compression (see RFC 2246, http://www.ietf.org/rfc/rfc2246.txt)
		/// </summary>
		public bool UseCompression
		{
			get { return this.m_UseCompression; }
			set { this.m_UseCompression = value; }
		}

		/// <summary>
		/// Are we Authenticated to the server? This is readonly and set by the library
		/// </summary>
		public bool Authenticated
		{
			get { return this.m_Authenticated; }
		}

		/// <summary>
		/// is the resource binded? This is readonly and set by the library
		/// </summary>
		public bool Binded
		{
			get { return this.m_Binded; }
		}

		/// <summary>
		/// Should the library register a new account on the server
		/// </summary>
		public bool RegisterAccount
		{
			get { return this.m_RegisterAccount; }
			set { this.m_RegisterAccount = value; }
		}

		public IqGrabber IqGrabber
		{
			get { return this.m_IqGrabber; }
		}

		public MessageGrabber MessageGrabber
		{
			get { return this.m_MessageGrabber; }
		}

		public PresenceGrabber PresenceGrabber
		{
			get { return this.m_PresenceGrabber; }
		}

		public RosterManager RosterManager
		{
			get { return this.m_RosterManager; }
		}

		public PresenceManager PresenceManager
		{
			get { return this.m_PresenceManager; }
		}

		public bool EnableCapabilities
		{
			get { return this.m_EnableCapabilities; }
			set { this.m_EnableCapabilities = value; }
		}

		public string ClientVersion
		{
			get { return this.m_ClientVersion; }
			set { this.m_ClientVersion = value; }
		}

		public Capabilities Capabilities
		{
			get { return this.m_Capabilities; }
			set { this.m_Capabilities = value; }
		}

		/// <summary>
		/// The DiscoInfo object is used to respond to DiscoInfo request if AutoAnswerDiscoInfoRequests == true in DisoManager objects,
		/// it's also used to build the Caps version when EnableCapabilities is set to true.
		/// <remarks>
		/// When EnableCapailities == true call UpdateCapsVersion after each update of the DiscoInfo object
		/// </remarks>
		/// </summary>
		public DiscoInfo DiscoInfo
		{
			get { return this.m_DiscoInfo; }
			set { this.m_DiscoInfo = value; }
		}
		#endregion

		#region << Events >>			

		/// <summary>
		/// We are authenticated to the server now.
		/// </summary>	
		public event ObjectHandler OnLogin;
		/// <summary>
		/// This event occurs after the resource was binded
		/// </summary>
		public event ObjectHandler OnBinded;

		/// <summary>
		/// This event is fired when we get register information.
		/// You ca use this event for custom registrations.
		/// </summary>
		public event RegisterEventHandler OnRegisterInformation;

		/// <summary>
		/// This event gets fired after a new account is registered
		/// </summary>
		public event ObjectHandler OnRegistered;

		/// <summary>
		/// This event ets fired after a ChangePassword Request was successful
		/// </summary>
		public event ObjectHandler OnPasswordChanged;

		/*
        was never used, comment ot until we need it
		public event XmppElementHandler			OnXmppError;
		*/

		/// <summary>
		/// Event that occurs on registration errors
		/// </summary>
		public event XmppElementHandler OnRegisterError;

		/// <summary>
		/// Event occurs on Xmpp Stream error elements
		/// </summary>
		public event XmppElementHandler OnStreamError;

		/// <summary>
		/// Event that occurs on authentication errors
		/// e.g. wrong password, user doesnt exist etc...
		/// </summary>
		public event XmppElementHandler OnAuthError;

		/// <summary>
		/// Event occurs on Socket Errors
		/// </summary>
		public event ErrorHandler OnSocketError;

		public event ObjectHandler OnClose;


		/// <summary>
		/// This event is raised when a response to a roster query is received. The roster query contains the contact list.
		/// This lost could be very large and could contain hundreds of contacts. The are all send in a single XML element from 
		/// the server. Normally you show the contact list in a GUI control in you application (treeview, listview). 
		/// When this event occurs you couls Suspend the GUI for faster drawing and show change the mousepointer to the hourglass
		/// </summary>
		/// <remarks>see also OnRosterItem and OnRosterEnd</remarks>
		public event ObjectHandler OnRosterStart;

		/// <summary>
		/// This event is raised when a response to a roster query is received. It notifies you that all RosterItems (contacts) are
		/// received now.
		/// When this event occurs you could Resume the GUI and show the normal mousepointer again.
		/// </summary>
		/// <remarks>see also OnRosterStart and OnRosterItem</remarks>
		public event ObjectHandler OnRosterEnd;

		/// <summary>
		/// This event is raised when a response to a roster query is received. This event always contains a single RosterItem. 
		/// e.g. you have 150 friends on your contact list, then this event is called 150 times.
		/// </summary>
		/// <remarks>see also OnRosterItem and OnRosterEnd</remarks>
		public event RosterHandler OnRosterItem;

		/// <summary>
		/// This event is raised when a response to an agents query which could contain multiple agentitems.
		/// Normally you show the items in a GUI. This event could be used to suspend the UI for faster drawing.
		/// </summary>
		/// <remarks>see also OnAgentItem and OnAgentEnd</remarks>
		public event ObjectHandler OnAgentStart;

		/// <summary>
		/// This event is raised when a response to an agents query which could contain multiple agentitems.
		/// Normally you show the items in a GUI. This event could be used to resume the suspended userinterface.
		/// </summary>
		/// <remarks>see also OnAgentStart and OnAgentItem</remarks>
		public event ObjectHandler OnAgentEnd;

		/// <summary>
		/// This event returns always a single AgentItem from a agents query result.
		/// This is from the old jabber protocol. Instead of agents Disco (Service Discovery) should be used in modern
		/// application. But still lots of servers use Agents.
		/// <seealso cref=""/>
		/// </summary>
		/// <remarks>see also OnAgentStart and OnAgentEnd</remarks>
		public event AgentHandler OnAgentItem;

		/// <summary>
		/// 
		/// </summary>        
		public event IqHandler OnIq;

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

		//public event ErrorHandler				OnError;

		public event SaslEventHandler OnSaslStart;
		public event ObjectHandler OnSaslEnd;


		#endregion

		#region << Constructors >>
		public XmppClientConnection() : base()
		{
			this.m_IqGrabber = new IqGrabber(this);
			this.m_MessageGrabber = new MessageGrabber(this);
			this.m_PresenceGrabber = new PresenceGrabber(this);
			this.m_PresenceManager = new PresenceManager(this);
			this.m_RosterManager = new RosterManager(this);
		}

		public XmppClientConnection(SocketConnectionType type) : this()
		{
			this.SocketType = type;
		}

		public XmppClientConnection(string server) : this()
		{
			this.Server = server;
		}

		public XmppClientConnection(string server, int port) : this(server)
		{
			this.Port = port;
		}
		#endregion

		/// <summary>
		/// This method open the connections to the xmpp server and authenticates you to ther server.
		/// This method is async, don't assume you are already connected when it returns. You have to wait for the OnLogin Event
		/// </summary>
		public void Open()
		{
			this._Open();
		}

		/// <summary>
		/// This method open the connections to the xmpp server and authenticates you to ther server.
		/// This method is async, don't assume you are already connected when it returns. You have to wait for the OnLogin Event
		/// </summary>
		/// <param name="username">your username</param>
		/// <param name="password">your password</param>
		public void Open(string username, string password)
		{
			this.Username = username;
			this.Password = password;

			this._Open();
		}

		/// <summary>
		/// This method open the connections to the xmpp server and authenticates you to ther server.
		/// This method is async, don't assume you are already connected when it returns. You have to wait for the OnLogin Event
		/// </summary>
		/// <param name="username">your username</param>
		/// <param name="password">your passowrd</param>
		/// <param name="resource">resource for this connection</param>
		public void Open(string username, string password, string resource)
		{
			this.m_Username = username;
			this.m_Password = password;
			this.m_Resource = resource;
			this._Open();
		}

		/// <summary>
		/// This method open the connections to the xmpp server and authenticates you to ther server.
		/// This method is async, don't assume you are already connected when it returns. You have to wait for the OnLogin Event
		/// </summary>
		/// <param name="username">your username</param>
		/// <param name="password">your password</param>
		/// <param name="resource">resource for this connection</param>
		/// <param name="priority">priority which will be sent with presence packets</param>
		public void Open(string username, string password, string resource, int priority)
		{
			this.m_Username = username;
			this.m_Password = password;
			this.m_Resource = resource;
			this.m_Priority = priority;
			this._Open();
		}

		/// <summary>
		/// This method open the connections to the xmpp server and authenticates you to ther server.
		/// This method is async, don't assume you are already connected when it returns. You have to wait for the OnLogin Event
		/// </summary>
		/// <param name="username">your username</param>
		/// <param name="password">your password</param>
		/// <param name="priority">priority which will be sent with presence packets</param>
		public void Open(string username, string password, int priority)
		{
			this.m_Username = username;
			this.m_Password = password;
			this.m_Priority = priority;
			this._Open();
		}

		#region << Socket handers >>
		public override void SocketOnConnect(object sender)
		{
			base.SocketOnConnect(sender);

			this.SendStreamHeader(true);
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

			if ((ex.GetType() == typeof(ConnectTimeoutException)
				|| (ex.GetType() == typeof(SocketException) && ((SocketException)ex).ErrorCode == 10061))
				&& this._SRVRecords != null
				&& this._SRVRecords.Length > 1)
			{
				// connect failed. We are using SRV records and have multiple results.
				// remove the current record
				this.RemoveSrvRecord(this._currentSRVRecord);
				// find and set a new record
				this.SetConnectServerFromSRVRecords();
				// connect again
				this.OpenSocket();
			}
			else
			{
				// Fires the socket error
				OnSocketError?.Invoke(this, ex);

				// Only cleaneUp Session and raise on close if the stream already has started
				// if teh stream gets closed because of a socket error we have to raise both errors fo course
				if (this.m_StreamStarted && !this.m_CleanUpDone)
					this.CleanupSession();
			}
		}
		#endregion

		private void _Open()
		{
			this.m_CleanUpDone = false;
			this.m_StreamStarted = false;

			this.StreamParser.Reset();
#if SSL || MONOSSL
			if (this.ClientSocket.GetType() == typeof(ClientSocket))
				((ClientSocket)this.ClientSocket).SSL = this.m_UseSSL;
#endif
			// this should start later
			//if (m_KeepAlive)
			//    CreateKeepAliveTimer();

			if (this.SocketType == SocketConnectionType.Direct && this.AutoResolveConnectServer)
				this.ResolveSrv();

			this.OpenSocket();
		}

		private void OpenSocket()
		{
			if (this.ConnectServer == null)
				this.SocketConnect(this.Server, this.Port);
			else
				this.SocketConnect(this.ConnectServer, this.Port);
		}

		#region << SRV functions >>
		/// <summary>
		/// Resolves the connection host of a xmpp domain when SRV records are set
		/// </summary>
		private void ResolveSrv()
		{
#if WIN32
            try
            {
                // get the machine's default DNS servers
                string[] dnsServers = IPConfigurationInformation.DnsServers;

                if (dnsServers[0] == "")
                {
                    FireOnError(this, new Exception("No DNS Servers found"));
                    return;
                }

                // Take the 1st DNS Server for our query
                IPAddress dnsServer = IPAddress.Parse(dnsServers[0]);

                // Information
                string queryDomain = SRV_RECORD_PREFIX + this.Server;
                //QuerySrvRecord(dnsServer, queryDomain, DnsType.SRV);

                _SRVRecords = Resolver.SRVLookup(queryDomain, dnsServer);

                SetConnectServerFromSRVRecords();
            }
            catch (Exception ex)
            {
                FireOnError(this, ex);                
            }
#endif
		}

		private void SetConnectServerFromSRVRecords()
		{
			// check we have a response
			if (this._SRVRecords != null && this._SRVRecords.Length > 0)
			{
				//SRVRecord srv = _SRVRecords[0];
				this._currentSRVRecord = this.PickSRVRecord();

				this.Port = this._currentSRVRecord.Port;
				this.ConnectServer = this._currentSRVRecord.Target;
			}
			else
			{
				// no SRV-Records set
				this._currentSRVRecord = null;
				this.ConnectServer = null;
			}
		}

		private void RemoveSrvRecord(SRVRecord rec)
		{
			var i = 0;
			var recs = new SRVRecord[this._SRVRecords.Length - 1];
			foreach (var srv in this._SRVRecords)
			{
				if (!srv.Equals(rec))
				{
					recs[i] = srv;
					i++;
				}
			}
			this._SRVRecords = recs;
		}

		/// <summary>
		/// Picks one of the SRV records.
		/// priority and weight are evaluated by the following algorithm.
		/// </summary>
		/// <returns>SRVRecord</returns>
		private SRVRecord PickSRVRecord()
		{
			SRVRecord ret = null;

			// total weight of all servers with the same priority
			var totalWeight = 0;

			// ArrayList for the servers with the lowest priority
			var lowServers = new ArrayList();
			// check we have a response
			if (this._SRVRecords != null && this._SRVRecords.Length > 0)
			{
				// Find server(s) with the highest priority (could be multiple)
				foreach (var srv in this._SRVRecords)
				{
					if (ret == null)
					{
						ret = srv;
						lowServers.Add(ret);
						totalWeight = ret.Weight;
					}
					else
					{
						if (srv.Priority == ret.Priority)
						{
							lowServers.Add(srv);
							totalWeight += srv.Weight;
						}
						else if (srv.Priority < ret.Priority)
						{
							// found a servr with a lower priority
							// clear the lowServers Array and start with this server
							lowServers.Clear();
							lowServers.Add(ret);
							ret = srv;
							totalWeight = ret.Weight;
						}
						else if (srv.Priority > ret.Priority)
						{
							// exit the loop, because servers are already sorted by priority
							break;
						}
					}
				}
			}

			// if we have multiple lowServers then we have to pick a random one
			// BUT we have too involve the weight which can be used for "Load Balancing" here
			if (lowServers.Count > 1)
			{
				if (totalWeight > 0)
				{
					// Create a random value between 1 - total Weight
					var rnd = new Random().Next(1, totalWeight);
					var i = 0;
					foreach (SRVRecord sr in lowServers)
					{
						if (rnd > i && rnd <= (i + sr.Weight))
						{
							ret = sr;
							break;
						}
						else
						{
							i += sr.Weight;
						}
					}
				}
				else
				{
					// Servers have no weight, they are all equal, pick a random server
					var rnd = new Random().Next(lowServers.Count);
					ret = (SRVRecord)lowServers[rnd];
				}
			}

			return ret;
		}

		#endregion

		private void SendStreamHeader(bool _)
		{
			var sb = new StringBuilder();


			sb.Append("<stream:stream");
			sb.Append(" to='" + this.Server + "'");
			sb.Append(" xmlns='jabber:client'");
			sb.Append(" xmlns:stream='http://etherx.jabber.org/streams'");

			if (this.StreamVersion != null)
				sb.Append(" version='" + this.StreamVersion + "'");

			if (this.m_ClientLanguage != null)
				sb.Append(" xml:lang='" + this.m_ClientLanguage + "'");

			// xml:lang="en"<stream:stream to="coversant.net" xmlns="jabber:client" xmlns:stream="http://etherx.jabber.org/streams"  xml:lang="en" version="1.0" >
			// sb.Append(" xml:lang='" + "en" + "' ");


			sb.Append(">");

			this.Open(sb.ToString());
		}


		/// <summary>
		/// Sends our Presence, the packet is built of Status, Show and Priority
		/// </summary>
		public void SendMyPresence()
		{
			var pres = new Presence(this.m_Show, this.m_Status, this.m_Priority);

			// Add client caps when enabled
			if (this.m_EnableCapabilities)
			{
				if (this.m_Capabilities.Version == null)
					this.UpdateCapsVersion();

				pres.AddChild(this.m_Capabilities);
			}

			this.Send(pres);
		}

		/// <summary>
		/// Sets the caps version automatically from the DiscoInfo object.
		/// Call this member after each change of the DiscoInfo object
		/// </summary>
		public void UpdateCapsVersion()
		{
			this.m_Capabilities.SetVersion(this.m_DiscoInfo);
		}

		internal void RequestLoginInfo()
		{
			var iq = new AuthIq(IQType.Get, new Jid(this.Server));
			iq.Query.Username = this.m_Username;

			this.IqGrabber.SendIq(iq, new IqCB(this.OnGetAuthInfo), null);
		}

		/// <summary>
		/// Changing the Password. You should use this function only when connected with SSL or TLS
		/// because the password is sent in plain text over the connection.		
		/// </summary>
		/// /// <remarks>
		///		<para>
		///			After this request was successful the new password is set automatically in the Username Property
		///		</para>
		/// </remarks>		
		/// <param name="newPass">value of the new password</param>
		public void ChangePassword(string newPass)
		{
			/*
			
			Example 10. Password Change
			<iq type='set' to='somehost' id='change1'>
			<query xmlns='jabber:iq:register'>
				<username>bill</username>
				<password>newpass</password>
			</query>
			</iq>			    

			Because the password change request contains the password in plain text,
			a client SHOULD NOT send such a request unless the underlying stream is encrypted 
			(using SSL or TLS) and the client has verified that the server certificate is signed 
			by a trusted certificate authority. A given domain MAY choose to disable password 
			changes if the stream is not properly encrypted, or to disable in-band password 
			changes entirely.

			If the user provides an empty password element or a password element that contains 
			no XML character data (i.e., either <password/> or <password></password>),
			the server or service MUST NOT change the password to a null value, 
			but instead MUST maintain the existing password.

			Example 11. Host Informs Client of Successful Password Change

			<iq type='result' id='change1'/>			
			*/

			var regIq = new RegisterIq(IQType.Set, new Jid(this.Server));
			regIq.Query.Username = this.m_Username;
			regIq.Query.Password = newPass;

			this.IqGrabber.SendIq(regIq, new IqCB(this.OnChangePasswordResult), newPass);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="iq"></param>
		/// <param name="data">contains the new password</param>
		private void OnChangePasswordResult(object sender, IQ iq, object data)
		{
			if (iq.Type == IQType.Result)
			{
				OnPasswordChanged?.Invoke(this);

				// Set the new password in the Password property on sucess
				this.m_Password = (string)data;
			}
			else if (iq.Type == IQType.Error)
			{
				/*
				The server or service SHOULD NOT return the original XML sent in 
				IQ error stanzas related to password changes.

				Example 12. Host Informs Client of Failed Password Change (Bad Request)

				<iq type='error' from='somehost' to='user@host/resource' id='change1'>
				<error code='400' type='modify'>
					<conflict xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/>
				</error>
				</iq>
				    

				Example 13. Host Informs Client of Failed Password Change (Not Authorized)

				<iq type='error' from='somehost' to='user@host/resource' id='change1'>
				<error code='401' type='cancel'>
					<not-authorized xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/>
				</error>
				</iq>
				    

				Example 14. Server Informs Client of Failed Password Change (Not Allowed)

				<iq type='error' from='somehost' to='user@host/resource' id='change1'>
				<error code='405' type='cancel'>
					<not-allowed xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/>
				</error>
				</iq>
								  
				*/

				OnRegisterError?.Invoke(this, iq);
			}
		}

		#region << Register new Account >>


		/// <summary>
		/// requests the registration fields
		/// </summary>
		/// <param name="obj">object which contains the features node which we need later for login again</param>
		private void GetRegistrationFields(object data)
		{
			// <iq type='get' id='reg1'>
			//  <query xmlns='jabber:iq:register'/>
			// </iq>

			var regIq = new RegisterIq(IQType.Get, new Jid(this.Server));
			this.IqGrabber.SendIq(regIq, new IqCB(this.OnRegistrationFieldsResult), data);
		}

		private void OnRegistrationFieldsResult(object sender, IQ iq, object data)
		{
			if (iq.Type != IQType.Error)
			{
				if (iq.Query != null && iq.Query.GetType() == typeof(Register))
				{
					var args = new RegisterEventArgs(iq.Query as Register);
					OnRegisterInformation?.Invoke(this, args);

					this.DoChangeXmppConnectionState(XmppConnectionState.Registering);

					var regIq = new IQ(IQType.Set);
					regIq.GenerateId();
					regIq.To = new Jid(this.Server);

					//RegisterIq regIq = new RegisterIq(IqType.set, new Jid(base.Server));
					if (args.Auto)
					{
						var reg = new Register(this.m_Username, this.m_Password);
						regIq.Query = reg;
					}
					else
					{
						regIq.Query = args.Register;
					}
					this.IqGrabber.SendIq(regIq, new IqCB(this.OnRegisterResult), data);
				}
			}
			else
			{
				OnRegisterError?.Invoke(this, iq);
			}
		}

		private void OnRegisterResult(object sender, IQ iq, object data)
		{
			/*
			Example 6. Host Informs Entity of Failed Registration (Username Conflict)

			<iq type='error' id='reg2'>
			<query xmlns='jabber:iq:register'>
				<username>bill</username>
				<password>m1cro$oft</password>
				<email>billg@bigcompany.com</email>
			</query>
			<error code='409' type='cancel'>
				<conflict xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/>
			</error>
			</iq>
    

			Example 7. Host Informs Entity of Failed Registration (Some Required Information Not Provided)

			<iq type='error' id='reg2'>
			<query xmlns='jabber:iq:register'>
				<username>bill</username>
				<password>Calliope</password>
			</query>
			<error code='406' type='modify'>
				<not-acceptable xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/>
			</error>
			</iq>
			*/
			if (iq.Type == IQType.Result)
			{
				this.DoChangeXmppConnectionState(XmppConnectionState.Registered);
				OnRegistered?.Invoke(this);

				if (this.StreamVersion != null && this.StreamVersion.StartsWith("1."))
				{
					// init sasl login
					this.InitSaslHandler();
					this.m_SaslHandler.OnStreamElement(this, data as Node);
				}
				else
				{
					// old jabber style login
					this.RequestLoginInfo();
				}
			}
			else if (iq.Type == IQType.Error)
			{
				OnRegisterError?.Invoke(this, iq);
			}
		}
		#endregion

		private void OnGetAuthInfo(object sender, IQ iq, object data)
		{
			// We get smth like this and should add password (digest) and ressource
			// Recv:<iq type="result" id="MX_7"><query xmlns="jabber:iq:auth"><username>gnauck</username><password/><digest/><resource/></query></iq>
			// Send:<iq type='set' id='mx_login'>
			//			<query xmlns='jabber:iq:auth'><username>gnauck</username><digest>27c05d464e3f908db3b2ca1729674bfddb28daf2</digest><resource>Office</resource></query>
			//		</iq>
			// Recv:<iq id="mx_login" type="result"/> 

			iq.GenerateId();
			iq.SwitchDirection();
			iq.Type = IQType.Set;

			var auth = (Auth)iq.Query;

			auth.Resource = this.m_Resource;
			auth.SetAuth(this.m_Username, this.m_Password, this.StreamId);

			this.IqGrabber.SendIq(iq, new IqCB(this.OnAuthenticate), null);
		}

		/// <summary>
		/// Refreshes the myJid Member Variable
		/// </summary>
		private Jid BuildMyJid()
		{
			var jid = new Jid(null);

			jid.m_User = this.m_Username;
			jid.m_Server = this.Server;
			jid.m_Resource = this.m_Resource;

			jid.BuildJid();

			return jid;
		}

		#region << RequestAgents >>
		public void RequestAgents()
		{
			var iq = new AgentsIq(IQType.Get, new Jid(this.Server));
			this.IqGrabber.SendIq(iq, new IqCB(this.OnAgents), null);
		}

		private void OnAgents(object sender, IQ iq, object data)
		{
			OnAgentStart?.Invoke(this);

			var agents = iq.Query as Agents;
			if (agents != null)
			{
				foreach (Agent a in agents.GetAgents())
				{
					OnAgentItem?.Invoke(this, a);
				}
			}

			OnAgentEnd?.Invoke(this);
		}
		#endregion

		#region << RequestRoster >>
		public void RequestRoster()
		{
			var iq = new RosterIq(IQType.Get);
			this.Send(iq);
		}

		private void OnRosterIQ(IQ iq)
		{
			// if type == result then it must be the "FullRoster" we requested
			// in this case we raise OnRosterStart and OnRosterEnd
			// 
			// if type == set its a new added r updated rosteritem. Here we dont raise
			// OnRosterStart and OnRosterEnd
			if (iq.Type == IQType.Result && OnRosterStart != null)
				OnRosterStart(this);

			var r = iq.Query as Roster;
			if (r != null)
			{
				foreach (var i in r.GetRoster())
				{
					OnRosterItem?.Invoke(this, i);
				}
			}

			if (iq.Type == IQType.Result && OnRosterEnd != null)
				OnRosterEnd(this);

			if (this.m_AutoPresence && iq.Type == IQType.Result)
				this.SendMyPresence();
		}
		#endregion

		private void OnAuthenticate(object sender, IQ iq, object data)
		{
			if (iq.Type == IQType.Result)
			{
				this.m_Authenticated = true;
				this.RaiseOnLogin();
			}
			else if (iq.Type == IQType.Error)
			{
				/* 
				 * <iq xmlns="jabber:client" id="agsXMPP_2" type="error">
				 *		<query xmlns="jabber:iq:auth">
				 *			<username>test</username>
				 *			<digest sid="842070264">dc7e472abb95b65c2b75129ade607170be478b16</digest>
				 *			<resource>MiniClient</resource>
				 *		</query>
				 *		<error code="401">Unauthorized</error>
				 * </iq>
				 * 
				 */
				OnAuthError?.Invoke(this, iq);
			}

		}

		internal void FireOnAuthError(Element e)
		{
			OnAuthError?.Invoke(this, e);
		}

		#region << StreamParser Events >>
		public override void StreamParserOnStreamStart(object sender, Node e)
		{
			base.StreamParserOnStreamStart(this, e);

			this.m_StreamStarted = true;

			//m_CleanUpDone = false; moved that to _Open();

			var xst = (Protocol.XmppStream)e;
			if (xst == null)
				return;

			// Read the server language string
			this.m_ServerLanguage = xst.Language;


			// Auth stuff
			if (!this.RegisterAccount)
			{
				if (this.StreamVersion != null && this.StreamVersion.StartsWith("1."))
				{
					if (!this.Authenticated)
					{
						// we assume server supports SASL here, because it advertised a StreamVersion 1.X
						// and wait for the stream features and initialize the SASL Handler
						this.InitSaslHandler();
					}
				}
				else
				{
					// old auth stuff
					this.RequestLoginInfo();
				}
			}
			else
			{
				// Register on "old" jabber servers without stream features
				if (this.StreamVersion == null)
					this.GetRegistrationFields(null);
			}

		}

		private void InitSaslHandler()
		{
			if (this.m_SaslHandler == null)
			{
				this.m_SaslHandler = new SaslHandler(this);
				this.m_SaslHandler.OnSaslStart += new SaslEventHandler(this.m_SaslHandler_OnSaslStart);
				this.m_SaslHandler.OnSaslEnd += new ObjectHandler(this.m_SaslHandler_OnSaslEnd);
			}
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

			if (e.GetType() == typeof(IQ))
			{
				OnIq?.Invoke(this, e as IQ);

				var iq = e as IQ;
				if (iq != null && iq.Query != null)
				{
					// Roster
					if (iq.Query is Roster)
						this.OnRosterIQ(iq);
				}
			}
			else if (e.GetType() == typeof(Message))
			{
				OnMessage?.Invoke(this, e as Message);
			}
			else if (e.GetType() == typeof(Presence))
			{
				OnPresence?.Invoke(this, e as Presence);
			}
			else if (e.GetType() == typeof(StreamFeatures))
			{
				// Stream Features
				// StartTLS stuff
				var f = e as StreamFeatures;
				if (this.m_UseCompression &&
					f.SupportsCompression &&
					f.Compression.SupportsMethod(CompressionMethod.zlib))
				{
					// Check for Stream Compression
					// we support only ZLIB because its a free algorithm without patents
					// yes ePatents suck                                       
					this.DoChangeXmppConnectionState(XmppConnectionState.StartCompression);
					this.Send(new Compress(CompressionMethod.zlib));
				}
#if SSL || MONOSSL || BCCRYPTO
				else if (f.SupportsStartTls && this.m_UseStartTLS)
				{
					this.DoChangeXmppConnectionState(XmppConnectionState.Securing);
					this.Send(new StartTls());
				}
#endif
				else if (f.SupportsRegistration && this.m_RegisterAccount)
				{
					// Do registration after TLS when possible
					if (f.SupportsRegistration)
						this.GetRegistrationFields(e);
					else
					{
						// registration is not enabled on this server                        
						this.FireOnError(this, new RegisterException("Registration is not allowed on this server"));
						this.Close();
						// Close the stream
					}
				}
			}
#if SSL || MONOSSL || BCCRYPTO
			else if (e.GetType() == typeof(Proceed))
			{
				this.StreamParser.Reset();
				this.ClientSocket.StartTls();
				this.SendStreamHeader(false);
				this.DoChangeXmppConnectionState(XmppConnectionState.Authenticating);
			}
#endif
			else if (e.GetType() == typeof(Compressed))
			{
				//DoChangeXmppConnectionState(XmppConnectionState.StartCompression);
				this.StreamParser.Reset();
				this.ClientSocket.StartCompression();
				// Start new Stream Header compressed.
				this.SendStreamHeader(false);

				this.DoChangeXmppConnectionState(XmppConnectionState.Compressed);
			}
			else if (e is Protocol.XmppStreamError)
			{
				OnStreamError?.Invoke(this, e as Element);
			}

		}

		public override void StreamParserOnStreamError(object sender, Exception ex)
		{
			base.StreamParserOnStreamError(sender, ex);

			this.SocketDisconnect();
			this.CleanupSession();

			//this._NetworkStream.Close();

			this.FireOnError(this, ex);

			if (!this.m_CleanUpDone)
				this.CleanupSession();
		}
		#endregion



		public override void Send(Element e)
		{
			if (!(this.ClientSocket is BoshClientSocket))
			{
				// this is a hack to not send the xmlns="jabber:client" with all packets
				var dummyEl = new Element("a");
				dummyEl.Namespace = URI.CLIENT;

				dummyEl.AddChild(e);
				var toSend = dummyEl.ToString();

				this.Send(toSend.Substring(25, toSend.Length - 25 - 4));
			}
			else
				base.Send(e);
		}

		/// <summary>
		/// Does the Clieanup of the Session and sends the OnClose Event
		/// </summary>
		private void CleanupSession()
		{
			this.m_CleanUpDone = true;

			// TODO, check if this is always OK
			if (this.ClientSocket.Connected)
				this.ClientSocket.Disconnect();

			this.DoChangeXmppConnectionState(XmppConnectionState.Disconnected);

			this.StreamParser.Reset();

			this.m_IqGrabber.Clear();
			this.m_MessageGrabber.Clear();

			if (this.m_SaslHandler != null)
			{
				this.m_SaslHandler.Dispose();
				this.m_SaslHandler = null;
			}

			this.m_Authenticated = false;
			this.m_Binded = false;

			this.DestroyKeepAliveTimer();

			OnClose?.Invoke(this);
		}

		internal void Reset()
		{
			// tell also the socket that we need to reset the stream, this is needed for BOSH
			this.ClientSocket.Reset();

			this.StreamParser.Reset();
			this.SendStreamHeader(false);
		}

		internal void DoRaiseEventBinded()
		{
			OnBinded?.Invoke(this);
		}

		#region << SASL Handler Events >>
		private void m_SaslHandler_OnSaslStart(object sender, SaslEventArgs args)
		{
			// This acts only as a tunnel to the client
			OnSaslStart?.Invoke(this, args);
		}

		internal void RaiseOnLogin()
		{
			if (this.KeepAlive)
				this.CreateKeepAliveTimer();

			OnLogin?.Invoke(this);

			if (this.m_AutoAgents)
				this.RequestAgents();

			if (this.m_AutoRoster)
				this.RequestRoster();
		}

		private void m_SaslHandler_OnSaslEnd(object sender)
		{
			OnSaslEnd?.Invoke(this);

			this.m_Authenticated = true;
		}
		#endregion
	}
}