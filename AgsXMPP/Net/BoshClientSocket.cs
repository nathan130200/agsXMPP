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
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using AgsXMPP.Protocol.Extensions.Bosh;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Net
{

	public class WebRequestState
	{
		public WebRequestState(WebRequest request)
		{
			this.m_WebRequest = request;
		}

		DateTime m_Started;


		WebRequest m_WebRequest = null;
		Stream m_RequestStream = null;
		string m_Output = null;
		bool m_IsSessionRequest = false;
		Timer m_TimeOutTimer = null;
		private bool m_Aborted = false;

		public int WebRequestId;

		/// <summary>
		/// when was this request started (timestamp)?
		/// </summary>
		public DateTime Started
		{
			get { return this.m_Started; }
			set { this.m_Started = value; }
		}

		public bool IsSessionRequest
		{
			get { return this.m_IsSessionRequest; }
			set { this.m_IsSessionRequest = value; }
		}

		public string Output
		{
			get { return this.m_Output; }
			set { this.m_Output = value; }
		}

		public WebRequest WebRequest
		{
			get { return this.m_WebRequest; }
			set { this.m_WebRequest = value; }
		}

		public Stream RequestStream
		{
			get { return this.m_RequestStream; }
			set { this.m_RequestStream = value; }
		}

		public Timer TimeOutTimer
		{
			get { return this.m_TimeOutTimer; }
			set { this.m_TimeOutTimer = value; }
		}

		public bool Aborted
		{
			get { return this.m_Aborted; }
			set { this.m_Aborted = value; }
		}
	}

	public class BoshClientSocket : BaseSocket
	{
		private const string CONTENT_TYPE = "text/xml; charset=utf-8";
		private const string METHOD = "POST";
		private const string BOSH_VERSION = "1.6";
		private const int WEBREQUEST_TIMEOUT = 5000;
		private string[] Keys;                                   // Array of keys
		private int waitingRequests = 0;                    // currently active (waiting) WebRequests
		private int CurrentKeyIdx;                          // index of the currect key
		private Queue m_SendQueue = new Queue();          // Queue for stanzas to send
		private bool streamStarted = false;                // is the stream started? received stream header?
		private int polling = 0;
		private bool terminate = false;
		private bool terminated = false;
		private DateTime lastSend = DateTime.MinValue;    // DateTime of the last activity/response

		private bool m_KeepAlive = true;

		private long rid;
		private bool restart = false;                // stream state, are we currently restarting the stream?
		private string sid;

		private int webRequestId = 1;

		public BoshClientSocket(XmppConnection con)
		{
			this.m_XmppCon = con;
		}

		private void Init()
		{
			this.Keys = null;
			this.streamStarted = false;
			this.terminate = false;
			this.terminated = false;
		}

		#region << Properties >>
		private Jid m_To;
		private int m_Wait = 300;  // 5 minutes by default, if you think this is to long change it over the public property
		private int m_Requests = 2;

#if !CF && !CF_2
		private int m_MinCountKeys = 1000;
		private int m_MaxCountKeys = 9999;
#else
		// set this lower on embedded devices because the key generation is slow there		
        private int             m_MinCountKeys  = 10;
        private int             m_MaxCountKeys  = 99;
#endif
		private int m_Hold = 1;    // should be 1
		private int m_MaxPause = 0;

		public Jid To
		{
			get { return this.m_To; }
			set { this.m_To = value; }
		}

		/// <summary>
		/// The longest time (in seconds) that the connection manager is allowed to wait before responding to any request during the session.
		/// This enables the client to prevent its TCP connection from expiring due to inactivity, as well as to limit the delay before 
		/// it discovers any network failure.
		/// </summary>
		public int Wait
		{
			get { return this.m_Wait; }
			set { this.m_Wait = value; }
		}

		public int Requests
		{
			get { return this.m_Requests; }
			set { this.m_Requests = value; }
		}

		public int MaxCountKeys
		{
			get { return this.m_MaxCountKeys; }
			set { this.m_MaxCountKeys = value; }
		}

		public int MinCountKeys
		{
			get { return this.m_MinCountKeys; }
			set { this.m_MinCountKeys = value; }
		}

		/// <summary>
		/// This attribute specifies the maximum number of requests the connection manager is allowed to keep waiting 
		/// at any one time during the session. If the client is not able to use HTTP Pipelining then this SHOULD be set to "1".
		/// </summary>
		public int Hold
		{
			get { return this.m_Hold; }
			set { this.m_Hold = value; }
		}

		/// <summary>
		/// Keep Alive for HTTP Webrequests, its disables by default because not many BOSH implementations support Keep Alives
		/// </summary>
		public bool KeepAlive
		{
			get { return this.m_KeepAlive; }
			set { this.m_KeepAlive = value; }
		}

		/// <summary>
		/// If the connection manager supports session pausing (see Inactivity) then it SHOULD advertise that to the client 
		/// by including a 'maxpause' attribute in the session creation response element. 
		/// The value of the attribute indicates the maximum length of a temporary session pause (in seconds) that a client MAY request.
		/// 0 is the default value and indicated that the connection manager supports no session pausing.
		/// </summary>
		public int MaxPause
		{
			get { return this.m_MaxPause; }
			set { this.m_MaxPause = value; }
		}

		public bool SupportsSessionPausing
		{
			get { return !(this.m_MaxPause == 0); }
		}
		#endregion

		private string DummyStreamHeader
		{
			get
			{
				// <stream:stream xmlns='jabber:client' xmlns:stream='http://etherx.jabber.org/streams' id='1075705237'>
				// create dummy stream header
				var sb = new StringBuilder();

				sb.Append("<stream:stream");

				sb.Append(" xmlns='");
				sb.Append(URI.CLIENT);

				sb.Append("' xmlns:stream='");
				sb.Append(URI.STREAM);

				sb.Append("' id='");
				sb.Append(this.sid);

				sb.Append("' version='");
				sb.Append("1.0");

				sb.Append("'>");

				return sb.ToString();
			}
		}

		/// <summary>
		/// Generates a bunch of keys
		/// </summary>
		private void GenerateKeys()
		{
			/*
            13.3 Generating the Key Sequence

            Prior to requesting a new session, the client MUST select an unpredictable counter ("n") and an unpredictable value ("seed").
            The client then processes the "seed" through a cryptographic hash and converts the resulting 160 bits to a hexadecimal string K(1).
            It does this "n" times to arrive at the initial key K(n). The hashing algorithm MUST be SHA-1 as defined in RFC 3174.

            Example 25. Creating the key sequence

                    K(1) = hex(SHA-1(seed))
                    K(2) = hex(SHA-1(K(1)))
                    ...
                    K(n) = hex(SHA-1(K(n-1)))

            */
			var countKeys = this.GetRandomNumber(this.m_MinCountKeys, this.m_MaxCountKeys);

			this.Keys = new string[countKeys];
			var prev = this.GenerateSeed();

			for (var i = 0; i < countKeys; i++)
			{
				this.Keys[i] = Util.Hash.Sha1Hash(prev);
				prev = this.Keys[i];
			}
			this.CurrentKeyIdx = countKeys - 1;
		}

		private string GenerateSeed()
		{
			var m_lenght = 10;

			var buf = new byte[m_lenght];

			using (var rng = RandomNumberGenerator.Create())
				rng.GetBytes(buf);

			return Util.Hash.HexToString(buf);
		}

		private int GenerateRid()
		{
			var min = 1;
			var max = int.MaxValue;

			var rnd = new Random();

			return rnd.Next(min, max);
		}

		private int GetRandomNumber(int min, int max)
		{
			var rnd = new Random();
			return rnd.Next(min, max);
		}

		public override void Reset()
		{
			base.Reset();

			this.streamStarted = false;
			this.restart = true;
		}

		public void RequestBoshSession()
		{
			/*
            Example 1. Requesting a BOSH session

            POST /webclient HTTP/1.1
            Host: httpcm.jabber.org
            Accept-Encoding: gzip, deflate
            Content-Type: text/xml; charset=utf-8
            Content-Length: 104

            <body content='text/xml; charset=utf-8'
                  hold='1'
                  rid='1573741820'
                  to='jabber.org'
                  route='xmpp:jabber.org:9999'
                  secure='true'
                  ver='1.6'
                  wait='60'
                  ack='1'
                  xml:lang='en'
                  xmlns='http://jabber.org/protocol/httpbind'/>
             */

			this.lastSend = DateTime.Now;

			// Generate the keys
			this.GenerateKeys();
			this.rid = this.GenerateRid();
			var body = new Body();
			/*
             * <body hold='1' xmlns='http://jabber.org/protocol/httpbind' 
             *  to='vm-2k' 
             *  wait='300' 
             *  rid='782052' 
             *  newkey='8e7d6cec12004e2bfcf7fc000310fda87bc8337c' 
             *  ver='1.6' 
             *  xmpp:xmlns='urn:xmpp:xbosh' 
             *  xmpp:version='1.0'/>
             */

			body.Version = BOSH_VERSION;
			body.XmppVersion = "1.0";
			body.Hold = this.m_Hold;
			body.Wait = this.m_Wait;
			body.Rid = this.rid;
			body.Polling = 0;
			body.Requests = this.m_Requests;
			body.To = new Jid(this.m_XmppCon.Server);

			body.NewKey = this.Keys[this.CurrentKeyIdx];

			body.SetAttribute("xmpp:xmlns", "urn:xmpp:xbosh");

			this.waitingRequests++;

			var req = (HttpWebRequest)WebRequest.Create(this.Address);

			var state = new WebRequestState(req);
			state.Started = DateTime.Now;
			state.Output = body.ToString();
			state.IsSessionRequest = true;

			req.Method = METHOD;
			req.ContentType = CONTENT_TYPE;
			req.Timeout = this.m_Wait * 1000;
			req.KeepAlive = this.m_KeepAlive;
			req.ContentLength = state.Output.Length;

			try
			{
				var result = req.BeginGetRequestStream(new AsyncCallback(this.OnGetSessionRequestStream), state);
			}
			catch (Exception)
			{
			}
		}

		private void OnGetSessionRequestStream(IAsyncResult ar)
		{
			var state = ar.AsyncState as WebRequestState;

			var req = state.WebRequest as HttpWebRequest;

			var outputStream = req.EndGetRequestStream(ar);

			var bytes = Encoding.UTF8.GetBytes(state.Output);

			state.RequestStream = outputStream;
			var result = outputStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.OnEndWrite), state);
		}

		private void OnGetSessionRequestResponse(IAsyncResult result)
		{
			// grab the custom state object
			var state = (WebRequestState)result.AsyncState;
			var request = (HttpWebRequest)state.WebRequest;

			//state.TimeOutTimer.Dispose();

			// get the Response
			var resp = (HttpWebResponse)request.EndGetResponse(result);

			// The server must always return a 200 response code,
			// sending any session errors as specially-formatted identifiers.
			if (resp.StatusCode != HttpStatusCode.OK)
			{
				//FireOnError(new PollSocketException("unexpected status code " + resp.StatusCode.ToString()));
				return;
			}

			var rs = resp.GetResponseStream();

			int readlen;
			var readbuf = new byte[1024];
			var ms = new MemoryStream();
			while ((readlen = rs.Read(readbuf, 0, readbuf.Length)) > 0)
			{
				ms.Write(readbuf, 0, readlen);
			}

			var recv = ms.ToArray();

			if (recv.Length > 0)
			{
				string body = null;
				string stanzas = null;

				var res = Encoding.UTF8.GetString(recv, 0, recv.Length);

				this.ParseResponse(res, ref body, ref stanzas);

				var doc = new Document();
				doc.LoadXml(body);
				var boshBody = doc.RootElement as Body;

				this.sid = boshBody.Sid;
				this.polling = boshBody.Polling;
				this.m_MaxPause = boshBody.MaxPause;

				var bin = Encoding.UTF8.GetBytes(this.DummyStreamHeader + stanzas);

				this.FireOnReceive(bin, bin.Length);

				// cleanup webrequest resources
				ms.Close();
				rs.Close();
				resp.Close();

				this.waitingRequests--;

				if (this.waitingRequests == 0)
					this.StartWebRequest();
			}
		}

		/// <summary>
		/// This is ugly code, but currently all BOSH server implementaions are not namespace correct,
		/// which means we can't use the XML parser here and have to spit it with string functions.
		/// </summary>
		/// <param name="res"></param>
		/// <param name="body"></param>
		/// <param name="stanzas"></param>
		private void ParseResponse(string res, ref string body, ref string stanzas)
		{
			res = res.Trim();
			if (res.EndsWith("/>"))
			{
				// <body ..../>
				// empty response
				body = res;
				stanzas = null;
			}
			else
			{
				/* 
                 * <body .....>
                 *  <message/>
                 *  <presence/>
                 * </body>  
                 */

				// find position of the first closing angle bracket
				var startPos = res.IndexOf(">");
				// find position of the last opening angle bracket
				var endPos = res.LastIndexOf("<");

				body = res.Substring(0, startPos) + "/>";
				stanzas = res.Substring(startPos + 1, endPos - startPos - 1);
			}
		}

		#region << Public Methods and Functions >>
		public override void Connect()
		{
			base.Connect();

			this.Init();
			this.FireOnConnect();

			this.RequestBoshSession();
		}

		public override void Disconnect()
		{
			base.Disconnect();

			this.FireOnDisconnect();
			//m_Connected = false;
		}

		public override void Send(byte[] bData)
		{
			base.Send(bData);

			this.Send(Encoding.UTF8.GetString(bData, 0, bData.Length));
		}


		public override void Send(string data)
		{
			base.Send(data);

			// This are hacks because we send no stream headers and footer in Bosh
			if (data.StartsWith("<stream:stream"))
			{
				if (!this.streamStarted && !this.restart)
					this.streamStarted = true;
				else
				{
					var bin = Encoding.UTF8.GetBytes(this.DummyStreamHeader);
					this.FireOnReceive(bin, bin.Length);
				}
				return;
			}

			if (data.EndsWith("</stream:stream>"))
			{
				var pres = new Protocol.Client.Presence();
				pres.Type = Protocol.Client.PresenceType.Unavailable;
				data = pres.ToString(); //= "<presence type='unavailable' xmlns='jabber:client'/>";
				this.terminate = true;
			}
			//    return;

			lock (this.m_SendQueue)
			{
				this.m_SendQueue.Enqueue(data);
			}

			if (this.waitingRequests <= 1)
			{
				this.StartWebRequest();
			}
		}
		#endregion

		private string BuildPostData()
		{
			this.CurrentKeyIdx--;
			this.rid++;

			var sb = new StringBuilder();

			var body = new Body();

			body.Rid = this.rid;
			body.Key = this.Keys[this.CurrentKeyIdx];

			if (this.CurrentKeyIdx == 0)
			{
				// this is our last key
				// Generate a new key sequence
				this.GenerateKeys();
				body.NewKey = this.Keys[this.CurrentKeyIdx];
			}

			body.Sid = this.sid;
			//body.Polling    = 0;
			body.To = new Jid(this.m_XmppCon.Server);

			if (this.restart)
			{
				body.XmppRestart = true;
				this.restart = false;
			}

			lock (this.m_SendQueue)
			{
				if (this.terminate && this.m_SendQueue.Count == 1)
					body.Type = BoshType.Terminate;

				if (this.m_SendQueue.Count > 0)
				{
					sb.Append(body.StartTag());

					while (this.m_SendQueue.Count > 0)
					{
						var data = this.m_SendQueue.Dequeue() as string;
						sb.Append(data);
					}

					sb.Append(body.EndTag());

					return sb.ToString();
				}
				else
					return body.ToString();
			}
		}

		private void StartWebRequest()
		{
			this.StartWebRequest(false, null);
		}

		private void StartWebRequest(bool retry, string content)
		{
			lock (this)
			{
				this.webRequestId++;
			}

			this.waitingRequests++;

			this.lastSend = DateTime.Now;

			var req = (HttpWebRequest)WebRequest.Create(this.Address);

			var state = new WebRequestState(req);
			state.Started = DateTime.Now;
			state.WebRequestId = this.webRequestId;

			if (!retry)
				state.Output = this.BuildPostData();
			else
				state.Output = content;

			req.Method = METHOD;
			req.ContentType = CONTENT_TYPE;
			req.Timeout = this.m_Wait * 1000;
			req.KeepAlive = this.m_KeepAlive;
			req.ContentLength = state.Output.Length;

			// Create the delegate that invokes methods for the timer.            
			var timerDelegate = new TimerCallback(this.TimeOutGetRequestStream);
			var timeoutTimer = new Timer(timerDelegate, state, WEBREQUEST_TIMEOUT, WEBREQUEST_TIMEOUT);
			state.TimeOutTimer = timeoutTimer;

			//Console.WriteLine(String.Format("Start Webrequest: id:{0}", webRequestId.ToString()));
			try
			{
				var result = req.BeginGetRequestStream(new AsyncCallback(this.OnGetRequestStream), state);
			}
			catch (Exception)
			{
				//Console.WriteLine(ex.Message);
			}
		}

		public void TimeOutGetRequestStream(object stateObj)
		{

			//Console.WriteLine("Web Request timed out");

			var state = stateObj as WebRequestState;
			state.TimeOutTimer.Dispose();
			state.Aborted = true;
			state.WebRequest.Abort();
		}

		//        public void TimeOutGetResponseStream(Object stateObj)
		//        {
		//#if DEBUG
		//            Console.WriteLine("Web Response timed out");
		//#endif
		//            WebRequestState state = stateObj as WebRequestState;
		//            state.TimeOutTimer.Dispose();
		//            state.Aborted = true;
		//            state.WebRequest.Abort();
		//        }

		private void OnGetRequestStream(IAsyncResult ar)
		{
			try
			{
				var state = ar.AsyncState as WebRequestState;

				//Console.WriteLine(String.Format("OnGetRequestStream: id:{0}", state.WebRequestId.ToString()));

				if (state.Aborted)
				{
					this.waitingRequests--;
					this.StartWebRequest(true, state.Output);
				}
				else
				{
					state.TimeOutTimer.Dispose();
					var req = state.WebRequest as HttpWebRequest;

					var requestStream = req.EndGetRequestStream(ar);
					state.RequestStream = requestStream;
					//byte[] bytes = Encoding.UTF8.GetBytes(BuildPostData());
					var bytes = Encoding.UTF8.GetBytes(state.Output);
					//Console.WriteLine("Write Request:");
					//Console.WriteLine(state.Output);
					var result = requestStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.OnEndWrite), state);
				}
			}
			catch (Exception)
			{
				//Console.WriteLine(ex.Message);
				this.waitingRequests--;

				var state = ar.AsyncState as WebRequestState;
				this.StartWebRequest(true, state.Output);
			}
		}

		private void OnEndWrite(IAsyncResult ar)
		{
			var state = ar.AsyncState as WebRequestState;

			//Console.WriteLine(String.Format("OnEndWrite: id:{0}", state.WebRequestId.ToString()));

			var req = state.WebRequest as HttpWebRequest;
			var requestStream = state.RequestStream;

			requestStream.EndWrite(ar);
			requestStream.Close();

			IAsyncResult result;

			try
			{
				if (state.IsSessionRequest)
					result = req.BeginGetResponse(new AsyncCallback(this.OnGetSessionRequestResponse), state);
				else
					result = req.BeginGetResponse(new AsyncCallback(this.OnGetResponse), state);

			}
			catch (Exception)
			{
				//Console.WriteLine(ex.Message);
			}
		}

		private void OnGetResponse(IAsyncResult ar)
		{
			try
			{
				// grab the custom state object
				var state = (WebRequestState)ar.AsyncState;

				//Console.WriteLine(String.Format("OnGetResponse: id:{0}", state.WebRequestId.ToString()));

				//if (state.Aborted)
				//{
				//    waitingRequests--;
				//    if (waitingRequests == 0 && !terminated)
				//    {
				//        StartWebRequest();
				//    }
				//    return;
				//}


				var request = (HttpWebRequest)state.WebRequest;


				HttpWebResponse resp = null;

				if (request.HaveResponse)
				{
					// TODO, its crashing mostly here
					// get the Response
					try
					{
						resp = (HttpWebResponse)request.EndGetResponse(ar);
					}
					catch (WebException ex)
					{
						this.waitingRequests--;
						if (ex.Response == null)
						{
							this.StartWebRequest();
						}
						else
						{
							var res = ex.Response as HttpWebResponse;
							if (res.StatusCode == HttpStatusCode.NotFound)
							{
								this.TerminateBoshSession();
							}
							//if (waitingRequests == 0)
							//{
							//    StartWebRequest();                    
							//}  
						}
						return;
					}

					// The server must always return a 200 response code,
					// sending any session errors as specially-formatted identifiers.
					if (resp.StatusCode != HttpStatusCode.OK)
					{
						this.waitingRequests--;
						if (resp.StatusCode == HttpStatusCode.NotFound)
						{
							//Console.WriteLine("Not Found");
							this.TerminateBoshSession();
						}
						//FireOnError(new PollSocketException("unexpected status code " + resp.StatusCode.ToString()));
						return;
					}
				}
				else
				{
					//Console.WriteLine("No response");
				}

				var rs = resp.GetResponseStream();

				int readlen;
				var readbuf = new byte[1024];
				var ms = new MemoryStream();
				while ((readlen = rs.Read(readbuf, 0, readbuf.Length)) > 0)
				{
					ms.Write(readbuf, 0, readlen);
				}

				var recv = ms.ToArray();

				if (recv.Length > 0)
				{
					string sbody = null;
					string stanzas = null;

					this.ParseResponse(Encoding.UTF8.GetString(recv, 0, recv.Length), ref sbody, ref stanzas);
					//string res = Encoding.UTF8.GetString(recv, 0, recv.Length);

					if (stanzas != null)
					{
						var bStanzas = Encoding.UTF8.GetBytes(stanzas);
						this.FireOnReceive(bStanzas, bStanzas.Length);
					}
					else
					{
						if (this.terminate && !this.terminated)
						{
							// empty teminate response
							this.TerminateBoshSession();
						}
						//Console.WriteLine("Empty Response");
					}
				}

				// cleanup webrequest resources
				ms.Close();
				rs.Close();
				resp.Close();

				this.waitingRequests--;

				if (this.waitingRequests == 0 && !this.terminated)
				{
					this.StartWebRequest();
				}
			}
			catch (Exception)
			{
				//Console.WriteLine("Error in OnGetResponse");
				//Console.WriteLine(ex.Message);
			}
		}

		private void TerminateBoshSession()
		{
			// empty teminate response
			var bStanzas = Encoding.UTF8.GetBytes("</stream:stream>");
			this.FireOnReceive(bStanzas, bStanzas.Length);
			this.terminated = true;
		}
	}
}