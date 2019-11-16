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

using agsXMPP.Xml.Dom;

namespace agsXMPP.Factory
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
			AddElementType("iq", Namespaces.CLIENT, typeof(Protocol.client.Iq));
			AddElementType("message", Namespaces.CLIENT, typeof(Protocol.client.Message));
			AddElementType("presence", Namespaces.CLIENT, typeof(Protocol.client.Presence));
			AddElementType("error", Namespaces.CLIENT, typeof(Protocol.client.Error));

			AddElementType("agent", Namespaces.IQ_AGENTS, typeof(Protocol.iq.agent.Agent));

			AddElementType("item", Namespaces.IQ_ROSTER, typeof(Protocol.iq.roster.RosterItem));
			AddElementType("group", Namespaces.IQ_ROSTER, typeof(Protocol.@base.Group));
			AddElementType("group", Namespaces.X_ROSTERX, typeof(Protocol.@base.Group));

			AddElementType("item", Namespaces.IQ_SEARCH, typeof(Protocol.iq.search.SearchItem));

			// Stream stuff
			AddElementType("stream", Namespaces.STREAM, typeof(Protocol.XmppStream));
			AddElementType("error", Namespaces.STREAM, typeof(Protocol.XmppStreamError));

			AddElementType("query", Namespaces.IQ_AUTH, typeof(Protocol.iq.auth.Auth));
			AddElementType("query", Namespaces.IQ_AGENTS, typeof(Protocol.iq.agent.Agents));
			AddElementType("query", Namespaces.IQ_ROSTER, typeof(Protocol.iq.roster.Roster));
			AddElementType("query", Namespaces.IQ_LAST, typeof(Protocol.iq.last.Last));
			AddElementType("query", Namespaces.IQ_VERSION, typeof(Protocol.iq.version.Version));
			AddElementType("query", Namespaces.IQ_TIME, typeof(Protocol.iq.time.Time));
			AddElementType("query", Namespaces.IQ_OOB, typeof(Protocol.iq.oob.Oob));
			AddElementType("query", Namespaces.IQ_SEARCH, typeof(Protocol.iq.search.Search));
			AddElementType("query", Namespaces.IQ_BROWSE, typeof(Protocol.iq.browse.Browse));
			AddElementType("query", Namespaces.IQ_AVATAR, typeof(Protocol.iq.avatar.Avatar));
			AddElementType("query", Namespaces.IQ_REGISTER, typeof(Protocol.iq.register.Register));
			AddElementType("query", Namespaces.IQ_PRIVATE, typeof(Protocol.iq.@private.Private));

			// Privacy Lists
			AddElementType("query", Namespaces.IQ_PRIVACY, typeof(Protocol.iq.privacy.Privacy));
			AddElementType("item", Namespaces.IQ_PRIVACY, typeof(Protocol.iq.privacy.Item));
			AddElementType("list", Namespaces.IQ_PRIVACY, typeof(Protocol.iq.privacy.List));
			AddElementType("active", Namespaces.IQ_PRIVACY, typeof(Protocol.iq.privacy.Active));
			AddElementType("default", Namespaces.IQ_PRIVACY, typeof(Protocol.iq.privacy.Default));

			// Browse
			AddElementType("service", Namespaces.IQ_BROWSE, typeof(Protocol.iq.browse.Service));
			AddElementType("item", Namespaces.IQ_BROWSE, typeof(Protocol.iq.browse.BrowseItem));

			// Service Discovery			
			AddElementType("query", Namespaces.DISCO_ITEMS, typeof(Protocol.iq.disco.DiscoItems));
			AddElementType("query", Namespaces.DISCO_INFO, typeof(Protocol.iq.disco.DiscoInfo));
			AddElementType("feature", Namespaces.DISCO_INFO, typeof(Protocol.iq.disco.DiscoFeature));
			AddElementType("identity", Namespaces.DISCO_INFO, typeof(Protocol.iq.disco.DiscoIdentity));
			AddElementType("item", Namespaces.DISCO_ITEMS, typeof(Protocol.iq.disco.DiscoItem));

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

			AddElementType("bind", Namespaces.BIND, typeof(Protocol.iq.bind.Bind));
			AddElementType("session", Namespaces.SESSION, typeof(Protocol.iq.session.Session));

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
			AddElementType("handshake", Namespaces.ACCEPT, typeof(Protocol.component.Handshake));
			AddElementType("log", Namespaces.ACCEPT, typeof(Protocol.component.Log));
			AddElementType("route", Namespaces.ACCEPT, typeof(Protocol.component.Route));
			AddElementType("iq", Namespaces.ACCEPT, typeof(Protocol.component.IQ));
			AddElementType("message", Namespaces.ACCEPT, typeof(Protocol.component.Message));
			AddElementType("presence", Namespaces.ACCEPT, typeof(Protocol.component.Presence));
			AddElementType("error", Namespaces.ACCEPT, typeof(Protocol.component.Error));

			//Extensions (JEPS)
			AddElementType("headers", Namespaces.SHIM, typeof(Protocol.extensions.Shim.Header));
			AddElementType("header", Namespaces.SHIM, typeof(Protocol.extensions.Shim.Headers));
			AddElementType("roster", Namespaces.ROSTER_DELIMITER, typeof(Protocol.iq.roster.Delimiter));
			AddElementType("p", Namespaces.PRIMARY, typeof(Protocol.extensions.primary.Primary));
			AddElementType("nick", Namespaces.NICK, typeof(Protocol.extensions.nickname.Nickname));

			AddElementType("item", Namespaces.X_ROSTERX, typeof(Protocol.x.roster.RosterItem));
			AddElementType("x", Namespaces.X_ROSTERX, typeof(Protocol.x.roster.Roster));

			// Filetransfer stuff
			AddElementType("file", Namespaces.SI_FILE_TRANSFER, typeof(Protocol.extensions.filetransfer.File));
			AddElementType("range", Namespaces.SI_FILE_TRANSFER, typeof(Protocol.extensions.filetransfer.Range));

			// FeatureNeg
			AddElementType("feature", Namespaces.FEATURE_NEG, typeof(Protocol.extensions.featureneg.FeatureNeg));

			// Bytestreams
			AddElementType("query", Namespaces.BYTESTREAMS, typeof(Protocol.extensions.bytestreams.ByteStream));
			AddElementType("streamhost", Namespaces.BYTESTREAMS, typeof(Protocol.extensions.bytestreams.StreamHost));
			AddElementType("streamhost-used", Namespaces.BYTESTREAMS, typeof(Protocol.extensions.bytestreams.StreamHostUsed));
			AddElementType("activate", Namespaces.BYTESTREAMS, typeof(Protocol.extensions.bytestreams.Activate));
			AddElementType("udpsuccess", Namespaces.BYTESTREAMS, typeof(Protocol.extensions.bytestreams.UdpSuccess));


			AddElementType("si", Namespaces.SI, typeof(Protocol.extensions.si.SI));

			AddElementType("html", Namespaces.XHTML_IM, typeof(Protocol.extensions.html.Html));
			AddElementType("body", Namespaces.XHTML, typeof(Protocol.extensions.html.Body));

			AddElementType("compressed", Namespaces.COMPRESS, typeof(Protocol.extensions.compression.Compressed));
			AddElementType("compress", Namespaces.COMPRESS, typeof(Protocol.extensions.compression.Compress));
			AddElementType("failure", Namespaces.COMPRESS, typeof(Protocol.extensions.compression.Failure));

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
			AddElementType("query", Namespaces.IQ_RPC, typeof(Protocol.iq.rpc.Rpc));
			AddElementType("methodCall", Namespaces.IQ_RPC, typeof(Protocol.iq.rpc.MethodCall));
			AddElementType("methodResponse", Namespaces.IQ_RPC, typeof(Protocol.iq.rpc.MethodResponse));

			// Chatstates Jep-0085
			AddElementType("active", Namespaces.CHATSTATES, typeof(Protocol.extensions.chatstates.Active));
			AddElementType("inactive", Namespaces.CHATSTATES, typeof(Protocol.extensions.chatstates.Inactive));
			AddElementType("composing", Namespaces.CHATSTATES, typeof(Protocol.extensions.chatstates.Composing));
			AddElementType("paused", Namespaces.CHATSTATES, typeof(Protocol.extensions.chatstates.Paused));
			AddElementType("gone", Namespaces.CHATSTATES, typeof(Protocol.extensions.chatstates.Gone));

			// Jivesoftware Extenstions
			AddElementType("phone-event", Namespaces.JIVESOFTWARE_PHONE, typeof(Protocol.extensions.jivesoftware.Phone.PhoneEvent));
			AddElementType("phone-action", Namespaces.JIVESOFTWARE_PHONE, typeof(Protocol.extensions.jivesoftware.Phone.PhoneAction));
			AddElementType("phone-status", Namespaces.JIVESOFTWARE_PHONE, typeof(Protocol.extensions.jivesoftware.Phone.PhoneStatus));

			// Jingle stuff is in heavy development, we commit this once the most changes on the Jeps are done            
			//AddElementType("jingle",            Uri.JINGLE,                 typeof(agsXMPP.protocol.extensions.jingle.Jingle));
			//AddElementType("candidate",         Uri.JINGLE,                 typeof(agsXMPP.protocol.extensions.jingle.Candidate));

			AddElementType("c", Namespaces.CAPS, typeof(Protocol.extensions.caps.Capabilities));

			AddElementType("geoloc", Namespaces.GEOLOC, typeof(Protocol.extensions.geoloc.GeoLoc));

			// Xmpp Ping
			AddElementType("ping", Namespaces.PING, typeof(Protocol.extensions.ping.Ping));

			//Ad-Hock Commands
			AddElementType("command", Namespaces.COMMANDS, typeof(Protocol.extensions.commands.Command));
			AddElementType("actions", Namespaces.COMMANDS, typeof(Protocol.extensions.commands.Actions));
			AddElementType("note", Namespaces.COMMANDS, typeof(Protocol.extensions.commands.Note));

			// **********
			// * PubSub *
			// **********
			// Owner namespace
			AddElementType("affiliate", Namespaces.PUBSUB_OWNER, typeof(Protocol.extensions.pubsub.Owner.Affiliate));
			AddElementType("affiliates", Namespaces.PUBSUB_OWNER, typeof(Protocol.extensions.pubsub.Owner.Affiliates));
			AddElementType("configure", Namespaces.PUBSUB_OWNER, typeof(Protocol.extensions.pubsub.Owner.Configure));
			AddElementType("delete", Namespaces.PUBSUB_OWNER, typeof(Protocol.extensions.pubsub.Owner.Delete));
			AddElementType("pending", Namespaces.PUBSUB_OWNER, typeof(Protocol.extensions.pubsub.Owner.Pending));
			AddElementType("pubsub", Namespaces.PUBSUB_OWNER, typeof(Protocol.extensions.pubsub.Owner.PubSub));
			AddElementType("purge", Namespaces.PUBSUB_OWNER, typeof(Protocol.extensions.pubsub.Owner.Purge));
			AddElementType("subscriber", Namespaces.PUBSUB_OWNER, typeof(Protocol.extensions.pubsub.Owner.Subscriber));
			AddElementType("subscribers", Namespaces.PUBSUB_OWNER, typeof(Protocol.extensions.pubsub.Owner.Subscribers));

			// Event namespace
			AddElementType("delete", Namespaces.PUBSUB_EVENT, typeof(Protocol.extensions.pubsub.Event.Delete));
			AddElementType("event", Namespaces.PUBSUB_EVENT, typeof(Protocol.extensions.pubsub.Event.Event));
			AddElementType("item", Namespaces.PUBSUB_EVENT, typeof(Protocol.extensions.pubsub.Event.Item));
			AddElementType("items", Namespaces.PUBSUB_EVENT, typeof(Protocol.extensions.pubsub.Event.Items));
			AddElementType("purge", Namespaces.PUBSUB_EVENT, typeof(Protocol.extensions.pubsub.Event.Purge));

			// Main Pubsub namespace
			AddElementType("affiliation", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Affiliation));
			AddElementType("affiliations", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Affiliations));
			AddElementType("configure", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Configure));
			AddElementType("create", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Create));
			AddElementType("configure", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Configure));
			AddElementType("item", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Item));
			AddElementType("items", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Items));
			AddElementType("options", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Options));
			AddElementType("publish", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Publish));
			AddElementType("pubsub", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.PubSub));
			AddElementType("retract", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Retract));
			AddElementType("subscribe", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Subscribe));
			AddElementType("subscribe-options", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.SubscribeOptions));
			AddElementType("subscription", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Subscription));
			AddElementType("subscriptions", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Subscriptions));
			AddElementType("unsubscribe", Namespaces.PUBSUB, typeof(Protocol.extensions.pubsub.Unsubscribe));

			// HTTP Binding XEP-0124
			AddElementType("body", Namespaces.HTTP_BIND, typeof(Protocol.extensions.bosh.Body));

			// Message receipts XEP-0184
			AddElementType("received", Namespaces.MSG_RECEIPT, typeof(Protocol.extensions.msgreceipts.Received));
			AddElementType("request", Namespaces.MSG_RECEIPT, typeof(Protocol.extensions.msgreceipts.Request));

			// Bookmark storage XEP-0048         
			AddElementType("storage", Namespaces.STORAGE_BOOKMARKS, typeof(Protocol.extensions.bookmarks.Storage));
			AddElementType("url", Namespaces.STORAGE_BOOKMARKS, typeof(Protocol.extensions.bookmarks.Url));
			AddElementType("conference", Namespaces.STORAGE_BOOKMARKS, typeof(Protocol.extensions.bookmarks.Conference));

			// XEP-0047: In-Band Bytestreams (IBB)
			AddElementType("open", Namespaces.IBB, typeof(Protocol.extensions.ibb.Open));
			AddElementType("data", Namespaces.IBB, typeof(Protocol.extensions.ibb.Data));
			AddElementType("close", Namespaces.IBB, typeof(Protocol.extensions.ibb.Close));

			// XEP-0153: vCard-Based Avatars
			AddElementType("x", Namespaces.VCARD_UPDATE, typeof(Protocol.x.vcard.VCardUpdate));

			// AMP
			AddElementType("amp", Namespaces.AMP, typeof(Protocol.extensions.amp.Amp));
			AddElementType("rule", Namespaces.AMP, typeof(Protocol.extensions.amp.Rule));

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