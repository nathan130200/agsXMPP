#pragma warning disable

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

using System.Collections;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Factory
{
	/// <summary>
	/// Factory class that implements the factory pattern for builing our Elements.
	/// </summary>
	public class ElementFactory
	{
		/// <summary>
		/// This Hashtable stores Mapping of protocol (tag/namespace) to the agsXMPP objects
		/// </summary>
		private static Hashtable m_table = new Hashtable();

		static ElementFactory()
		{
			AddElementType("iq", Namespaces.CLIENT, typeof(Protocol.Client.IQ));
			AddElementType("message", Namespaces.CLIENT, typeof(Protocol.Client.Message));
			AddElementType("presence", Namespaces.CLIENT, typeof(Protocol.Client.Presence));
			AddElementType("error", Namespaces.CLIENT, typeof(Protocol.Client.Error));

			AddElementType("agent", Namespaces.IQ_AGENTS, typeof(Protocol.Iq.agent.Agent));

			AddElementType("item", Namespaces.IQ_ROSTER, typeof(Protocol.Iq.roster.RosterItem));
			AddElementType("group", Namespaces.IQ_ROSTER, typeof(Protocol.Base.Group));
			AddElementType("group", Namespaces.X_ROSTERX, typeof(Protocol.Base.Group));

			AddElementType("item", Namespaces.IQ_SEARCH, typeof(Protocol.Iq.search.SearchItem));

			// Stream stuff
			AddElementType("stream", Namespaces.STREAM, typeof(Protocol.XmppStream));
			AddElementType("error", Namespaces.STREAM, typeof(Protocol.XmppStreamError));

			AddElementType("query", Namespaces.IQ_AUTH, typeof(Protocol.Iq.auth.Auth));
			AddElementType("query", Namespaces.IQ_AGENTS, typeof(Protocol.Iq.agent.Agents));
			AddElementType("query", Namespaces.IQ_ROSTER, typeof(Protocol.Iq.roster.Roster));
			AddElementType("query", Namespaces.IQ_LAST, typeof(Protocol.Iq.last.Last));
			AddElementType("query", Namespaces.IQ_VERSION, typeof(Protocol.Iq.version.Version));
			AddElementType("query", Namespaces.IQ_TIME, typeof(Protocol.Iq.time.Time));
			AddElementType("query", Namespaces.IQ_OOB, typeof(Protocol.Iq.oob.Oob));
			AddElementType("query", Namespaces.IQ_SEARCH, typeof(Protocol.Iq.search.Search));
			AddElementType("query", Namespaces.IQ_BROWSE, typeof(Protocol.Iq.browse.Browse));
			AddElementType("query", Namespaces.IQ_AVATAR, typeof(Protocol.Iq.avatar.Avatar));
			AddElementType("query", Namespaces.IQ_REGISTER, typeof(Protocol.Iq.register.Register));
			AddElementType("query", Namespaces.IQ_PRIVATE, typeof(Protocol.Iq.@private.Private));

			// Privacy Lists
			AddElementType("query", Namespaces.IQ_PRIVACY, typeof(Protocol.Iq.privacy.Privacy));
			AddElementType("item", Namespaces.IQ_PRIVACY, typeof(Protocol.Iq.privacy.Item));
			AddElementType("list", Namespaces.IQ_PRIVACY, typeof(Protocol.Iq.privacy.List));
			AddElementType("active", Namespaces.IQ_PRIVACY, typeof(Protocol.Iq.privacy.Active));
			AddElementType("default", Namespaces.IQ_PRIVACY, typeof(Protocol.Iq.privacy.Default));

			// Browse
			AddElementType("service", Namespaces.IQ_BROWSE, typeof(Protocol.Iq.browse.Service));
			AddElementType("item", Namespaces.IQ_BROWSE, typeof(Protocol.Iq.browse.BrowseItem));

			// Service Discovery			
			AddElementType("query", Namespaces.DISCO_ITEMS, typeof(Protocol.Iq.Disco.DiscoItems));
			AddElementType("query", Namespaces.DISCO_INFO, typeof(Protocol.Iq.Disco.DiscoInfo));
			AddElementType("feature", Namespaces.DISCO_INFO, typeof(Protocol.Iq.Disco.DiscoFeature));
			AddElementType("identity", Namespaces.DISCO_INFO, typeof(Protocol.Iq.Disco.DiscoIdentity));
			AddElementType("item", Namespaces.DISCO_ITEMS, typeof(Protocol.Iq.Disco.DiscoItem));

			AddElementType("x", Namespaces.X_DELAY, typeof(Protocol.x.Delay));
			AddElementType("x", Namespaces.X_AVATAR, typeof(Protocol.x.Avatar));
			AddElementType("x", Namespaces.X_CONFERENCE, typeof(Protocol.x.Conference));
			AddElementType("x", Namespaces.X_EVENT, typeof(Protocol.x.Event));

			AddElementType("x", Namespaces.STORAGE_AVATAR, typeof(Protocol.storage.Avatar));
			AddElementType("query", Namespaces.STORAGE_AVATAR, typeof(Protocol.storage.Avatar));

			// XData Stuff
			AddElementType("x", Namespaces.X_DATA, typeof(Protocol.x.data.Data));
			AddElementType("field", Namespaces.X_DATA, typeof(Protocol.x.data.Field));
			AddElementType("option", Namespaces.X_DATA, typeof(Protocol.x.data.Option));
			AddElementType("value", Namespaces.X_DATA, typeof(Protocol.x.data.Value));
			AddElementType("reported", Namespaces.X_DATA, typeof(Protocol.x.data.Reported));
			AddElementType("item", Namespaces.X_DATA, typeof(Protocol.x.data.Item));

			AddElementType("features", Namespaces.STREAM, typeof(Protocol.stream.StreamFeatures));

			AddElementType("register", Namespaces.FEATURE_IQ_REGISTER, typeof(Protocol.stream.features.Register));
			AddElementType("compression", Namespaces.FEATURE_COMPRESS, typeof(Protocol.stream.features.Compression.Compression));
			AddElementType("method", Namespaces.FEATURE_COMPRESS, typeof(Protocol.stream.features.Compression.Method));

			AddElementType("bind", Namespaces.BIND, typeof(Protocol.Iq.bind.Bind));
			AddElementType("session", Namespaces.SESSION, typeof(Protocol.Iq.session.Session));

			// TLS stuff
			AddElementType("failure", Namespaces.TLS, typeof(Protocol.tls.Failure));
			AddElementType("proceed", Namespaces.TLS, typeof(Protocol.tls.Proceed));
			AddElementType("starttls", Namespaces.TLS, typeof(Protocol.tls.StartTls));

			// SASL stuff
			AddElementType("mechanisms", Namespaces.SASL, typeof(Protocol.sasl.Mechanisms));
			AddElementType("mechanism", Namespaces.SASL, typeof(Protocol.sasl.Mechanism));
			AddElementType("auth", Namespaces.SASL, typeof(Protocol.sasl.Auth));
			AddElementType("response", Namespaces.SASL, typeof(Protocol.sasl.Response));
			AddElementType("challenge", Namespaces.SASL, typeof(Protocol.sasl.Challenge));

			// TODO, this is a dirty hacks for the buggy BOSH Proxy
			// BEGIN
			AddElementType("challenge", Namespaces.CLIENT, typeof(Protocol.sasl.Challenge));
			AddElementType("success", Namespaces.CLIENT, typeof(Protocol.sasl.Success));
			// END

			AddElementType("failure", Namespaces.SASL, typeof(Protocol.sasl.Failure));
			AddElementType("abort", Namespaces.SASL, typeof(Protocol.sasl.Abort));
			AddElementType("success", Namespaces.SASL, typeof(Protocol.sasl.Success));

			// Server stuff
			AddElementType("stream", Namespaces.SERVER, typeof(Protocol.server.Stream));
			AddElementType("message", Namespaces.SERVER, typeof(Protocol.server.Message));

			// Component stuff
			AddElementType("handshake", Namespaces.ACCEPT, typeof(Protocol.Component.Handshake));
			AddElementType("log", Namespaces.ACCEPT, typeof(Protocol.Component.Log));
			AddElementType("route", Namespaces.ACCEPT, typeof(Protocol.Component.Route));
			AddElementType("iq", Namespaces.ACCEPT, typeof(Protocol.Component.IQ));
			AddElementType("message", Namespaces.ACCEPT, typeof(Protocol.Component.Message));
			AddElementType("presence", Namespaces.ACCEPT, typeof(Protocol.Component.Presence));
			AddElementType("error", Namespaces.ACCEPT, typeof(Protocol.Component.Error));

			//Extensions (JEPS)
			AddElementType("headers", Namespaces.SHIM, typeof(Protocol.Extensions.Shim.Header));
			AddElementType("header", Namespaces.SHIM, typeof(Protocol.Extensions.Shim.Headers));
			AddElementType("roster", Namespaces.ROSTER_DELIMITER, typeof(Protocol.Iq.roster.Delimiter));
			AddElementType("p", Namespaces.PRIMARY, typeof(Protocol.Extensions.primary.Primary));
			AddElementType("nick", Namespaces.NICK, typeof(Protocol.Extensions.nickname.Nickname));

			AddElementType("item", Namespaces.X_ROSTERX, typeof(Protocol.x.roster.RosterItem));
			AddElementType("x", Namespaces.X_ROSTERX, typeof(Protocol.x.roster.Roster));

			// Filetransfer stuff
			AddElementType("file", Namespaces.SI_FILE_TRANSFER, typeof(Protocol.Extensions.FileTransfer.File));
			AddElementType("range", Namespaces.SI_FILE_TRANSFER, typeof(Protocol.Extensions.FileTransfer.Range));

			// FeatureNeg
			AddElementType("feature", Namespaces.FEATURE_NEG, typeof(Protocol.Extensions.FeatureNeg.FeatureNegotiation));

			// Bytestreams
			AddElementType("query", Namespaces.BYTESTREAMS, typeof(Protocol.Extensions.ByteStreams.ByteStream));
			AddElementType("streamhost", Namespaces.BYTESTREAMS, typeof(Protocol.Extensions.ByteStreams.StreamHost));
			AddElementType("streamhost-used", Namespaces.BYTESTREAMS, typeof(Protocol.Extensions.ByteStreams.StreamHostUsed));
			AddElementType("activate", Namespaces.BYTESTREAMS, typeof(Protocol.Extensions.ByteStreams.Activate));
			AddElementType("udpsuccess", Namespaces.BYTESTREAMS, typeof(Protocol.Extensions.ByteStreams.UdpSuccess));


			AddElementType("si", Namespaces.SI, typeof(Protocol.Extensions.SI.SI));

			AddElementType("html", Namespaces.XHTML_IM, typeof(Protocol.Extensions.XHtml.Html));
			AddElementType("body", Namespaces.XHTML, typeof(Protocol.Extensions.XHtml.Body));

			AddElementType("compressed", Namespaces.COMPRESS, typeof(Protocol.Extensions.Compression.Compressed));
			AddElementType("compress", Namespaces.COMPRESS, typeof(Protocol.Extensions.Compression.Compress));
			AddElementType("failure", Namespaces.COMPRESS, typeof(Protocol.Extensions.Compression.Failure));

			// MUC (JEP-0045 Multi User Chat)
			AddElementType("x", Namespaces.MUC, typeof(Protocol.x.muc.Muc));
			AddElementType("x", Namespaces.MUC_USER, typeof(Protocol.x.muc.User));
			AddElementType("item", Namespaces.MUC_USER, typeof(Protocol.x.muc.Item));
			AddElementType("status", Namespaces.MUC_USER, typeof(Protocol.x.muc.Status));
			AddElementType("invite", Namespaces.MUC_USER, typeof(Protocol.x.muc.Invite));
			AddElementType("decline", Namespaces.MUC_USER, typeof(Protocol.x.muc.Decline));
			AddElementType("actor", Namespaces.MUC_USER, typeof(Protocol.x.muc.Actor));
			AddElementType("history", Namespaces.MUC, typeof(Protocol.x.muc.History));
			AddElementType("query", Namespaces.MUC_ADMIN, typeof(Protocol.x.muc.iq.admin.Admin));
			AddElementType("item", Namespaces.MUC_ADMIN, typeof(Protocol.x.muc.iq.admin.Item));
			AddElementType("query", Namespaces.MUC_OWNER, typeof(Protocol.x.muc.iq.owner.Owner));
			AddElementType("destroy", Namespaces.MUC_OWNER, typeof(Protocol.x.muc.Destroy));


			//Jabber RPC JEP 0009            
			AddElementType("query", Namespaces.IQ_RPC, typeof(Protocol.Iq.rpc.Rpc));
			AddElementType("methodCall", Namespaces.IQ_RPC, typeof(Protocol.Iq.rpc.MethodCall));
			AddElementType("methodResponse", Namespaces.IQ_RPC, typeof(Protocol.Iq.rpc.MethodResponse));

			// Chatstates Jep-0085
			AddElementType("active", Namespaces.CHATSTATES, typeof(Protocol.Extensions.ChatStates.Active));
			AddElementType("inactive", Namespaces.CHATSTATES, typeof(Protocol.Extensions.ChatStates.Inactive));
			AddElementType("composing", Namespaces.CHATSTATES, typeof(Protocol.Extensions.ChatStates.Composing));
			AddElementType("paused", Namespaces.CHATSTATES, typeof(Protocol.Extensions.ChatStates.Paused));
			AddElementType("gone", Namespaces.CHATSTATES, typeof(Protocol.Extensions.ChatStates.Gone));

			// Jivesoftware Extenstions
			AddElementType("phone-event", Namespaces.JIVESOFTWARE_PHONE, typeof(Protocol.Extensions.jivesoftware.Phone.PhoneEvent));
			AddElementType("phone-action", Namespaces.JIVESOFTWARE_PHONE, typeof(Protocol.Extensions.jivesoftware.Phone.PhoneAction));
			AddElementType("phone-status", Namespaces.JIVESOFTWARE_PHONE, typeof(Protocol.Extensions.jivesoftware.Phone.PhoneStatus));

			// Jingle stuff is in heavy development, we commit this once the most changes on the Jeps are done            
			//AddElementType("jingle",            Uri.JINGLE,                 typeof(agsXMPP.protocol.extensions.jingle.Jingle));
			//AddElementType("candidate",         Uri.JINGLE,                 typeof(agsXMPP.protocol.extensions.jingle.Candidate));

			AddElementType("c", Namespaces.CAPS, typeof(Protocol.Extensions.Caps.Capabilities));

			AddElementType("geoloc", Namespaces.GEOLOC, typeof(Protocol.Extensions.GeoLoc.GeoLocation));

			// Xmpp Ping
			AddElementType("ping", Namespaces.PING, typeof(Protocol.Extensions.ping.Ping));

			//Ad-Hock Commands
			AddElementType("command", Namespaces.COMMANDS, typeof(Protocol.Extensions.Commands.Command));
			AddElementType("actions", Namespaces.COMMANDS, typeof(Protocol.Extensions.Commands.Actions));
			AddElementType("note", Namespaces.COMMANDS, typeof(Protocol.Extensions.Commands.Note));

			// **********
			// * PubSub *
			// **********
			// Owner namespace
			AddElementType("affiliate", Namespaces.PUBSUB_OWNER, typeof(Protocol.Extensions.pubsub.Owner.Affiliate));
			AddElementType("affiliates", Namespaces.PUBSUB_OWNER, typeof(Protocol.Extensions.pubsub.Owner.Affiliates));
			AddElementType("configure", Namespaces.PUBSUB_OWNER, typeof(Protocol.Extensions.pubsub.Owner.Configure));
			AddElementType("delete", Namespaces.PUBSUB_OWNER, typeof(Protocol.Extensions.pubsub.Owner.Delete));
			AddElementType("pending", Namespaces.PUBSUB_OWNER, typeof(Protocol.Extensions.pubsub.Owner.Pending));
			AddElementType("pubsub", Namespaces.PUBSUB_OWNER, typeof(Protocol.Extensions.pubsub.Owner.PubSub));
			AddElementType("purge", Namespaces.PUBSUB_OWNER, typeof(Protocol.Extensions.pubsub.Owner.Purge));
			AddElementType("subscriber", Namespaces.PUBSUB_OWNER, typeof(Protocol.Extensions.pubsub.Owner.Subscriber));
			AddElementType("subscribers", Namespaces.PUBSUB_OWNER, typeof(Protocol.Extensions.pubsub.Owner.Subscribers));

			// Event namespace
			AddElementType("delete", Namespaces.PUBSUB_EVENT, typeof(Protocol.Extensions.pubsub.Event.Delete));
			AddElementType("event", Namespaces.PUBSUB_EVENT, typeof(Protocol.Extensions.pubsub.Event.Event));
			AddElementType("item", Namespaces.PUBSUB_EVENT, typeof(Protocol.Extensions.pubsub.Event.Item));
			AddElementType("items", Namespaces.PUBSUB_EVENT, typeof(Protocol.Extensions.pubsub.Event.Items));
			AddElementType("purge", Namespaces.PUBSUB_EVENT, typeof(Protocol.Extensions.pubsub.Event.Purge));

			// Main Pubsub namespace
			AddElementType("affiliation", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Affiliation));
			AddElementType("affiliations", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Affiliations));
			AddElementType("configure", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Configure));
			AddElementType("create", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Create));
			AddElementType("configure", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Configure));
			AddElementType("item", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Item));
			AddElementType("items", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Items));
			AddElementType("options", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Options));
			AddElementType("publish", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Publish));
			AddElementType("pubsub", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.PubSub));
			AddElementType("retract", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Retract));
			AddElementType("subscribe", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Subscribe));
			AddElementType("subscribe-options", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.SubscribeOptions));
			AddElementType("subscription", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Subscription));
			AddElementType("subscriptions", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Subscriptions));
			AddElementType("unsubscribe", Namespaces.PUBSUB, typeof(Protocol.Extensions.pubsub.Unsubscribe));

			// HTTP Binding XEP-0124
			AddElementType("body", Namespaces.HTTP_BIND, typeof(Protocol.Extensions.Bosh.Body));

			// Message receipts XEP-0184
			AddElementType("received", Namespaces.MSG_RECEIPT, typeof(Protocol.Extensions.msgreceipts.Received));
			AddElementType("request", Namespaces.MSG_RECEIPT, typeof(Protocol.Extensions.msgreceipts.Request));

			// Bookmark storage XEP-0048         
			AddElementType("storage", Namespaces.STORAGE_BOOKMARKS, typeof(Protocol.Extensions.Bookmarks.Storage));
			AddElementType("url", Namespaces.STORAGE_BOOKMARKS, typeof(Protocol.Extensions.Bookmarks.Url));
			AddElementType("conference", Namespaces.STORAGE_BOOKMARKS, typeof(Protocol.Extensions.Bookmarks.Conference));

			// XEP-0047: In-Band Bytestreams (IBB)
			AddElementType("open", Namespaces.IBB, typeof(Protocol.Extensions.IBB.Open));
			AddElementType("data", Namespaces.IBB, typeof(Protocol.Extensions.IBB.Data));
			AddElementType("close", Namespaces.IBB, typeof(Protocol.Extensions.IBB.Close));

			// XEP-0153: vCard-Based Avatars
			AddElementType("x", Namespaces.VCARD_UPDATE, typeof(Protocol.x.vcard.VCardUpdate));

			// AMP
			AddElementType("amp", Namespaces.AMP, typeof(Protocol.Extensions.Amp.Amp));
			AddElementType("rule", Namespaces.AMP, typeof(Protocol.Extensions.Amp.Rule));

		}

		/// <summary>
		/// Adds new Element Types to the Hashtable
		/// Use this function also to register your own created Elements.
		/// If a element is already registered it gets overwritten. This behaviour is also useful if you you want to overwrite
		/// classes and add your own derived classes to the factory.
		/// </summary>
		/// <param name="tag">FQN</param>
		/// <param name="ns"></param>
		/// <param name="t"></param>
		public static void AddElementType(string tag, string ns, System.Type t)
		{
			var et = new ElementType(tag, ns);
			var key = et.ToString();
			// added thread safety on a user request
			lock (m_table)
			{
				if (m_table.ContainsKey(key))
					m_table[key] = t;
				else
					m_table.Add(et.ToString(), t);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="tag"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		public static Element GetElement(string prefix, string tag, string ns)
		{
			if (ns == null)
				ns = "";

			var et = new ElementType(tag, ns);
			var t = (System.Type)m_table[et.ToString()];

			Element ret;
			if (t != null)
				ret = (Element)System.Activator.CreateInstance(t);
			else
				ret = new Element(tag);

			ret.Prefix = prefix;

			if (ns != "")
				ret.Namespace = ns;

			return ret;
		}
	}
}

#pragma warning restore