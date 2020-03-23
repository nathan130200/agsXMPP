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
			AddElementType("iq", URI.CLIENT, typeof(Protocol.Client.IQ));
			AddElementType("message", URI.CLIENT, typeof(Protocol.Client.Message));
			AddElementType("presence", URI.CLIENT, typeof(Protocol.Client.Presence));
			AddElementType("error", URI.CLIENT, typeof(Protocol.Client.Error));

			AddElementType("agent", URI.IQ_AGENTS, typeof(Protocol.Iq.agent.Agent));

			AddElementType("item", URI.IQ_ROSTER, typeof(Protocol.Iq.roster.RosterItem));
			AddElementType("group", URI.IQ_ROSTER, typeof(Protocol.Base.Group));
			AddElementType("group", URI.X_ROSTERX, typeof(Protocol.Base.Group));

			AddElementType("item", URI.IQ_SEARCH, typeof(Protocol.Iq.search.SearchItem));

			// Stream stuff
			AddElementType("stream", URI.STREAM, typeof(Protocol.XmppStream));
			AddElementType("error", URI.STREAM, typeof(Protocol.XmppStreamError));

			AddElementType("query", URI.IQ_AUTH, typeof(Protocol.Iq.auth.Auth));
			AddElementType("query", URI.IQ_AGENTS, typeof(Protocol.Iq.agent.Agents));
			AddElementType("query", URI.IQ_ROSTER, typeof(Protocol.Iq.roster.Roster));
			AddElementType("query", URI.IQ_LAST, typeof(Protocol.Iq.last.Last));
			AddElementType("query", URI.IQ_VERSION, typeof(Protocol.Iq.version.Version));
			AddElementType("query", URI.IQ_TIME, typeof(Protocol.Iq.time.Time));
			AddElementType("query", URI.IQ_OOB, typeof(Protocol.Iq.oob.Oob));
			AddElementType("query", URI.IQ_SEARCH, typeof(Protocol.Iq.search.Search));
			AddElementType("query", URI.IQ_BROWSE, typeof(Protocol.Iq.browse.Browse));
			AddElementType("query", URI.IQ_AVATAR, typeof(Protocol.Iq.avatar.Avatar));
			AddElementType("query", URI.IQ_REGISTER, typeof(Protocol.Iq.register.Register));
			AddElementType("query", URI.IQ_PRIVATE, typeof(Protocol.Iq.@private.Private));

			// Privacy Lists
			AddElementType("query", URI.IQ_PRIVACY, typeof(Protocol.Iq.privacy.Privacy));
			AddElementType("item", URI.IQ_PRIVACY, typeof(Protocol.Iq.privacy.Item));
			AddElementType("list", URI.IQ_PRIVACY, typeof(Protocol.Iq.privacy.List));
			AddElementType("active", URI.IQ_PRIVACY, typeof(Protocol.Iq.privacy.Active));
			AddElementType("default", URI.IQ_PRIVACY, typeof(Protocol.Iq.privacy.Default));

			// Browse
			AddElementType("service", URI.IQ_BROWSE, typeof(Protocol.Iq.browse.Service));
			AddElementType("item", URI.IQ_BROWSE, typeof(Protocol.Iq.browse.BrowseItem));

			// Service Discovery			
			AddElementType("query", URI.DISCO_ITEMS, typeof(Protocol.Iq.Disco.DiscoItems));
			AddElementType("query", URI.DISCO_INFO, typeof(Protocol.Iq.Disco.DiscoInfo));
			AddElementType("feature", URI.DISCO_INFO, typeof(Protocol.Iq.Disco.DiscoFeature));
			AddElementType("identity", URI.DISCO_INFO, typeof(Protocol.Iq.Disco.DiscoIdentity));
			AddElementType("item", URI.DISCO_ITEMS, typeof(Protocol.Iq.Disco.DiscoItem));

			AddElementType("x", URI.X_DELAY, typeof(Protocol.x.Delay));
			AddElementType("x", URI.X_AVATAR, typeof(Protocol.x.Avatar));
			AddElementType("x", URI.X_CONFERENCE, typeof(Protocol.x.Conference));
			AddElementType("x", URI.X_EVENT, typeof(Protocol.x.Event));

			AddElementType("x", URI.STORAGE_AVATAR, typeof(Protocol.storage.Avatar));
			AddElementType("query", URI.STORAGE_AVATAR, typeof(Protocol.storage.Avatar));

			// XData Stuff
			AddElementType("x", URI.X_DATA, typeof(Protocol.x.data.Data));
			AddElementType("field", URI.X_DATA, typeof(Protocol.x.data.Field));
			AddElementType("option", URI.X_DATA, typeof(Protocol.x.data.Option));
			AddElementType("value", URI.X_DATA, typeof(Protocol.x.data.Value));
			AddElementType("reported", URI.X_DATA, typeof(Protocol.x.data.Reported));
			AddElementType("item", URI.X_DATA, typeof(Protocol.x.data.Item));

			AddElementType("features", URI.STREAM, typeof(Protocol.stream.StreamFeatures));

			AddElementType("register", URI.FEATURE_IQ_REGISTER, typeof(Protocol.stream.features.Register));
			AddElementType("compression", URI.FEATURE_COMPRESS, typeof(Protocol.stream.features.Compression.Compression));
			AddElementType("method", URI.FEATURE_COMPRESS, typeof(Protocol.stream.features.Compression.Method));

			AddElementType("bind", URI.BIND, typeof(Protocol.Iq.bind.Bind));
			AddElementType("session", URI.SESSION, typeof(Protocol.Iq.session.Session));

			// TLS stuff
			AddElementType("failure", URI.TLS, typeof(Protocol.tls.Failure));
			AddElementType("proceed", URI.TLS, typeof(Protocol.tls.Proceed));
			AddElementType("starttls", URI.TLS, typeof(Protocol.tls.StartTls));

			// SASL stuff
			AddElementType("mechanisms", URI.SASL, typeof(Protocol.sasl.Mechanisms));
			AddElementType("mechanism", URI.SASL, typeof(Protocol.sasl.Mechanism));
			AddElementType("auth", URI.SASL, typeof(Protocol.sasl.Auth));
			AddElementType("response", URI.SASL, typeof(Protocol.sasl.Response));
			AddElementType("challenge", URI.SASL, typeof(Protocol.sasl.Challenge));

			// TODO, this is a dirty hacks for the buggy BOSH Proxy
			// BEGIN
			AddElementType("challenge", URI.CLIENT, typeof(Protocol.sasl.Challenge));
			AddElementType("success", URI.CLIENT, typeof(Protocol.sasl.Success));
			// END

			AddElementType("failure", URI.SASL, typeof(Protocol.sasl.Failure));
			AddElementType("abort", URI.SASL, typeof(Protocol.sasl.Abort));
			AddElementType("success", URI.SASL, typeof(Protocol.sasl.Success));

			// Server stuff
			AddElementType("stream", URI.SERVER, typeof(Protocol.server.Stream));
			AddElementType("message", URI.SERVER, typeof(Protocol.server.Message));

			// Component stuff
			AddElementType("handshake", URI.ACCEPT, typeof(Protocol.Component.Handshake));
			AddElementType("log", URI.ACCEPT, typeof(Protocol.Component.Log));
			AddElementType("route", URI.ACCEPT, typeof(Protocol.Component.Route));
			AddElementType("iq", URI.ACCEPT, typeof(Protocol.Component.IQ));
			AddElementType("message", URI.ACCEPT, typeof(Protocol.Component.Message));
			AddElementType("presence", URI.ACCEPT, typeof(Protocol.Component.Presence));
			AddElementType("error", URI.ACCEPT, typeof(Protocol.Component.Error));

			//Extensions (JEPS)
			AddElementType("headers", URI.SHIM, typeof(Protocol.Extensions.Shim.Header));
			AddElementType("header", URI.SHIM, typeof(Protocol.Extensions.Shim.Headers));
			AddElementType("roster", URI.ROSTER_DELIMITER, typeof(Protocol.Iq.roster.Delimiter));
			AddElementType("p", URI.PRIMARY, typeof(Protocol.Extensions.primary.Primary));
			AddElementType("nick", URI.NICK, typeof(Protocol.Extensions.nickname.Nickname));

			AddElementType("item", URI.X_ROSTERX, typeof(Protocol.x.roster.RosterItem));
			AddElementType("x", URI.X_ROSTERX, typeof(Protocol.x.roster.Roster));

			// Filetransfer stuff
			AddElementType("file", URI.SI_FILE_TRANSFER, typeof(Protocol.Extensions.FileTransfer.File));
			AddElementType("range", URI.SI_FILE_TRANSFER, typeof(Protocol.Extensions.FileTransfer.Range));

			// FeatureNeg
			AddElementType("feature", URI.FEATURE_NEG, typeof(Protocol.Extensions.FeatureNeg.FeatureNegotiation));

			// Bytestreams
			AddElementType("query", URI.BYTESTREAMS, typeof(Protocol.Extensions.ByteStreams.ByteStream));
			AddElementType("streamhost", URI.BYTESTREAMS, typeof(Protocol.Extensions.ByteStreams.StreamHost));
			AddElementType("streamhost-used", URI.BYTESTREAMS, typeof(Protocol.Extensions.ByteStreams.StreamHostUsed));
			AddElementType("activate", URI.BYTESTREAMS, typeof(Protocol.Extensions.ByteStreams.Activate));
			AddElementType("udpsuccess", URI.BYTESTREAMS, typeof(Protocol.Extensions.ByteStreams.UdpSuccess));


			AddElementType("si", URI.SI, typeof(Protocol.Extensions.SI.SI));

			AddElementType("html", URI.XHTML_IM, typeof(Protocol.Extensions.XHtml.Html));
			AddElementType("body", URI.XHTML, typeof(Protocol.Extensions.XHtml.Body));

			AddElementType("compressed", URI.COMPRESS, typeof(Protocol.Extensions.Compression.Compressed));
			AddElementType("compress", URI.COMPRESS, typeof(Protocol.Extensions.Compression.Compress));
			AddElementType("failure", URI.COMPRESS, typeof(Protocol.Extensions.Compression.Failure));

			// MUC (JEP-0045 Multi User Chat)
			AddElementType("x", URI.MUC, typeof(Protocol.x.muc.Muc));
			AddElementType("x", URI.MUC_USER, typeof(Protocol.x.muc.User));
			AddElementType("item", URI.MUC_USER, typeof(Protocol.x.muc.Item));
			AddElementType("status", URI.MUC_USER, typeof(Protocol.x.muc.Status));
			AddElementType("invite", URI.MUC_USER, typeof(Protocol.x.muc.Invite));
			AddElementType("decline", URI.MUC_USER, typeof(Protocol.x.muc.Decline));
			AddElementType("actor", URI.MUC_USER, typeof(Protocol.x.muc.Actor));
			AddElementType("history", URI.MUC, typeof(Protocol.x.muc.History));
			AddElementType("query", URI.MUC_ADMIN, typeof(Protocol.x.muc.iq.admin.Admin));
			AddElementType("item", URI.MUC_ADMIN, typeof(Protocol.x.muc.iq.admin.Item));
			AddElementType("query", URI.MUC_OWNER, typeof(Protocol.x.muc.iq.owner.Owner));
			AddElementType("destroy", URI.MUC_OWNER, typeof(Protocol.x.muc.Destroy));


			//Jabber RPC JEP 0009            
			AddElementType("query", URI.IQ_RPC, typeof(Protocol.Iq.rpc.Rpc));
			AddElementType("methodCall", URI.IQ_RPC, typeof(Protocol.Iq.rpc.MethodCall));
			AddElementType("methodResponse", URI.IQ_RPC, typeof(Protocol.Iq.rpc.MethodResponse));

			// Chatstates Jep-0085
			AddElementType("active", URI.CHATSTATES, typeof(Protocol.Extensions.ChatStates.Active));
			AddElementType("inactive", URI.CHATSTATES, typeof(Protocol.Extensions.ChatStates.Inactive));
			AddElementType("composing", URI.CHATSTATES, typeof(Protocol.Extensions.ChatStates.Composing));
			AddElementType("paused", URI.CHATSTATES, typeof(Protocol.Extensions.ChatStates.Paused));
			AddElementType("gone", URI.CHATSTATES, typeof(Protocol.Extensions.ChatStates.Gone));

			// Jivesoftware Extenstions
			AddElementType("phone-event", URI.JIVESOFTWARE_PHONE, typeof(Protocol.Extensions.JiveSoftware.Phone.PhoneEvent));
			AddElementType("phone-action", URI.JIVESOFTWARE_PHONE, typeof(Protocol.Extensions.JiveSoftware.Phone.PhoneAction));
			AddElementType("phone-status", URI.JIVESOFTWARE_PHONE, typeof(Protocol.Extensions.JiveSoftware.Phone.PhoneStatus));

			// Jingle stuff is in heavy development, we commit this once the most changes on the Jeps are done            
			//AddElementType("jingle",            Uri.JINGLE,                 typeof(agsXMPP.protocol.extensions.jingle.Jingle));
			//AddElementType("candidate",         Uri.JINGLE,                 typeof(agsXMPP.protocol.extensions.jingle.Candidate));

			AddElementType("c", URI.CAPS, typeof(Protocol.Extensions.Caps.Capabilities));

			AddElementType("geoloc", URI.GEOLOC, typeof(Protocol.Extensions.GeoLoc.GeoLocation));

			// Xmpp Ping
			AddElementType("ping", URI.PING, typeof(Protocol.Extensions.ping.Ping));

			//Ad-Hock Commands
			AddElementType("command", URI.COMMANDS, typeof(Protocol.Extensions.Commands.Command));
			AddElementType("actions", URI.COMMANDS, typeof(Protocol.Extensions.Commands.Actions));
			AddElementType("note", URI.COMMANDS, typeof(Protocol.Extensions.Commands.Note));

			// **********
			// * PubSub *
			// **********
			// Owner namespace
			AddElementType("affiliate", URI.PUBSUB_OWNER, typeof(Protocol.Extensions.PubSub.Owner.Affiliate));
			AddElementType("affiliates", URI.PUBSUB_OWNER, typeof(Protocol.Extensions.PubSub.Owner.Affiliates));
			AddElementType("configure", URI.PUBSUB_OWNER, typeof(Protocol.Extensions.PubSub.Owner.Configure));
			AddElementType("delete", URI.PUBSUB_OWNER, typeof(Protocol.Extensions.PubSub.Owner.Delete));
			AddElementType("pending", URI.PUBSUB_OWNER, typeof(Protocol.Extensions.PubSub.Owner.Pending));
			AddElementType("pubsub", URI.PUBSUB_OWNER, typeof(Protocol.Extensions.PubSub.Owner.PubSub));
			AddElementType("purge", URI.PUBSUB_OWNER, typeof(Protocol.Extensions.PubSub.Owner.Purge));
			AddElementType("subscriber", URI.PUBSUB_OWNER, typeof(Protocol.Extensions.PubSub.Owner.Subscriber));
			AddElementType("subscribers", URI.PUBSUB_OWNER, typeof(Protocol.Extensions.PubSub.Owner.Subscribers));

			// Event namespace
			AddElementType("delete", URI.PUBSUB_EVENT, typeof(Protocol.Extensions.PubSub.Event.Delete));
			AddElementType("event", URI.PUBSUB_EVENT, typeof(Protocol.Extensions.PubSub.Event.Event));
			AddElementType("item", URI.PUBSUB_EVENT, typeof(Protocol.Extensions.PubSub.Event.Item));
			AddElementType("items", URI.PUBSUB_EVENT, typeof(Protocol.Extensions.PubSub.Event.Items));
			AddElementType("purge", URI.PUBSUB_EVENT, typeof(Protocol.Extensions.PubSub.Event.Purge));

			// Main Pubsub namespace
			AddElementType("affiliation", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Affiliation));
			AddElementType("affiliations", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Affiliations));
			AddElementType("configure", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Configure));
			AddElementType("create", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Create));
			AddElementType("configure", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Configure));
			AddElementType("item", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Item));
			AddElementType("items", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Items));
			AddElementType("options", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Options));
			AddElementType("publish", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Publish));
			AddElementType("pubsub", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.PubSub));
			AddElementType("retract", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Retract));
			AddElementType("subscribe", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Subscribe));
			AddElementType("subscribe-options", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.SubscribeOptions));
			AddElementType("subscription", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Subscription));
			AddElementType("subscriptions", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Subscriptions));
			AddElementType("unsubscribe", URI.PUBSUB, typeof(Protocol.Extensions.PubSub.Unsubscribe));

			// HTTP Binding XEP-0124
			AddElementType("body", URI.HTTP_BIND, typeof(Protocol.Extensions.Bosh.Body));

			// Message receipts XEP-0184
			AddElementType("received", URI.MSG_RECEIPT, typeof(Protocol.Extensions.MsgReceipts.Received));
			AddElementType("request", URI.MSG_RECEIPT, typeof(Protocol.Extensions.MsgReceipts.Request));

			// Bookmark storage XEP-0048         
			AddElementType("storage", URI.STORAGE_BOOKMARKS, typeof(Protocol.Extensions.Bookmarks.Storage));
			AddElementType("url", URI.STORAGE_BOOKMARKS, typeof(Protocol.Extensions.Bookmarks.Url));
			AddElementType("conference", URI.STORAGE_BOOKMARKS, typeof(Protocol.Extensions.Bookmarks.Conference));

			// XEP-0047: In-Band Bytestreams (IBB)
			AddElementType("open", URI.IBB, typeof(Protocol.Extensions.IBB.Open));
			AddElementType("data", URI.IBB, typeof(Protocol.Extensions.IBB.Data));
			AddElementType("close", URI.IBB, typeof(Protocol.Extensions.IBB.Close));

			// XEP-0153: vCard-Based Avatars
			AddElementType("x", URI.VCARD_UPDATE, typeof(Protocol.x.vcard.VCardUpdate));

			// AMP
			AddElementType("amp", URI.AMP, typeof(Protocol.Extensions.Amp.Amp));
			AddElementType("rule", URI.AMP, typeof(Protocol.Extensions.Amp.Rule));

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