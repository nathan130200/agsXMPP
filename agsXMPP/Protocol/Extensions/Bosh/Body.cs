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

using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Extensions.Bosh
{

	public class Body : Element
	{
		public Body()
		{
			this.TagName = "body";
			this.Namespace = URI.HTTP_BIND;
		}

		/*
        POST /webclient HTTP/1.1
        Host: httpcm.jabber.org
        Accept-Encoding: gzip, deflate
        Content-Type: text/xml; charset=utf-8
        Content-Length: 153

        <body rid='1249243564'
              sid='SomeSID'
              type='terminate'
              xmlns='http://jabber.org/protocol/httpbind'>
          <presence type='unavailable'
                    xmlns='jabber:client'/>
        </body>
        
        HTTP/1.1 200 OK
        Content-Type: text/xml; charset=utf-8
        Content-Length: 128

        <body authid='ServerStreamID'
              wait='60'
              inactivity='30'
              polling='5'
              requests='2'
              accept='deflate,gzip'
              sid='SomeSID'
              secure='true'
              stream='firstStreamName'
              charsets='ISO_8859-1 ISO-2022-JP'
              xmlns='http://jabber.org/protocol/httpbind'/>
        */

		public string Sid
		{
			get { return this.GetAttribute("sid"); }
			set { this.SetAttribute("sid", value); }
		}

		public long Rid
		{
			get { return this.GetAttributeLong("rid"); }
			set { this.SetAttribute("rid", value); }
		}

		public long Ack
		{
			get { return this.GetAttributeLong("ack"); }
			set { this.SetAttribute("ack", value); }
		}

		public bool Secure
		{
			get { return this.GetAttributeBool("secure"); }
			set { this.SetAttribute("secure", value); }
		}

		/// <summary>
		/// Specifies the longest time (in seconds) that the connection manager is allowed to wait before responding to any request 
		/// during the session. This enables the client to limit the delay before it discovers any network failure, 
		/// and to prevent its HTTP/TCP connection from expiring due to inactivity.
		/// </summary>
		public int Wait
		{
			get { return this.GetAttributeInt("wait"); }
			set { this.SetAttribute("wait", value); }
		}

		/// <summary>
		/// If the connection manager supports session pausing (inactivity) then it SHOULD advertise that to the client by including a 'maxpause'
		/// attribute in the session creation response element. The value of the attribute indicates the maximum length of a temporary 
		/// session pause (in seconds) that a client MAY request.
		/// </summary>
		public int MaxPause
		{
			get { return this.GetAttributeInt("maxpause"); }
			set { this.SetAttribute("maxpause", value); }
		}

		public int Inactivity
		{
			get { return this.GetAttributeInt("inactivity"); }
			set { this.SetAttribute("inactivity", value); }
		}

		public int Polling
		{
			get { return this.GetAttributeInt("polling"); }
			set { this.SetAttribute("polling", value); }
		}

		public int Requests
		{
			get { return this.GetAttributeInt("requests"); }
			set { this.SetAttribute("requests", value); }
		}

		/// <summary>
		/// Specifies the target domain of the first stream.
		/// </summary>
		public Jid To
		{
			get { return this.GetAttributeJid("to"); }
			set { this.SetAttribute("to", value); }
		}

		public Jid From
		{
			get { return this.GetAttributeJid("from"); }
			set { this.SetAttribute("from", value); }
		}

		/// <summary>
		/// specifies the maximum number of requests the connection manager is allowed to keep waiting at any one time during the session. 
		/// If the client is not able to use HTTP Pipelining then this SHOULD be set to "1".
		/// </summary>
		public int Hold
		{
			get { return this.GetAttributeInt("hold"); }
			set { this.SetAttribute("hold", value); }
		}

		/// <summary>
		/// <para>
		/// Specifies the highest version of the BOSH protocol that the client supports. 
		/// The numbering scheme is "<major>.<minor>" (where the minor number MAY be incremented higher than a single digit, 
		/// so it MUST be treated as a separate integer).
		/// </para>
		/// <remarks>
		/// The 'ver' attribute should not be confused with the version of any protocol being transported.
		/// </remarks>
		/// </summary>
		public string Version
		{
			get { return this.GetAttribute("ver"); }
			set { this.SetAttribute("ver", value); }
		}

		public string NewKey
		{
			get { return this.GetAttribute("newkey"); }
			set { this.SetAttribute("newkey", value); }
		}

		public string Key
		{
			get { return this.GetAttribute("key"); }
			set { this.SetAttribute("key", value); }
		}

		public BoshType Type
		{
			get => this.GetAttributeEnum<BoshType>("type"); }
			set
			{
				if (value == BoshType.NONE)
					this.RemoveAttribute("type");
				else
					this.SetAttribute("type", value.ToString());
			}
		}

		public string XmppVersion
		{
			get { return this.GetAttribute("xmpp:version"); }
			set { this.SetAttribute("xmpp:version", value); }
		}

		public bool XmppRestart
		{
			get { return this.GetAttributeBool("xmpp:restart"); }
			set { this.SetAttribute("xmpp:restart", value); }
		}
	}
}