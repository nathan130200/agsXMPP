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

using AgsXMPP.Protocol.Client;

namespace AgsXMPP.Protocol.Extensions.PubSub
{
	public class PubSubManager
	{
		private XmppClientConnection m_connection = null;

		#region << Constructors >>
		public PubSubManager(XmppClientConnection con)
		{
			this.m_connection = con;
		}
		#endregion

		#region << Create Instant Node >>
		/*
            Example 6. Client requests an instant node

            <iq type="set"
                from="pgm@jabber.org"
                to="pubsub.jabber.org"
                id="create2">
                <pubsub xmlns="http://jabber.org/protocol/pubsub">
                    <create/>
                </pubsub>
            </iq>
        */

		public void CreateInstantNode(Jid to)
		{
			this.CreateInstantNode(to, null, null, null);
		}

		public void CreateInstantNode(Jid to, Jid from)
		{
			this.CreateInstantNode(to, from, null, null);
		}

		public void CreateInstantNode(Jid to, Jid from, IqCB cb)
		{
			this.CreateInstantNode(to, from, cb, null);
		}

		public void CreateInstantNode(Jid to, IqCB cb)
		{
			this.CreateInstantNode(to, null, cb, null);
		}

		public void CreateInstantNode(Jid to, Jid from, IqCB cb, object cbArgs)
		{
			var pubsubIq = new PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Create = new Create();

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Create Node >>
		/*
            Example 1. Entity requests a new node with default configuration.

            <iq type="set"
                from="pgm@jabber.org"
                to="pubsub.jabber.org"
                id="create1">
                <pubsub xmlns="http://jabber.org/protocol/pubsub">
                    <create node="generic/pgm-mp3-player"/>
                    <configure/>
                </pubsub>
            </iq>
        */
		/// <summary>
		/// Create a Node with default configuration
		/// </summary>
		/// <param name="to"></param>
		/// <param name="node"></param>
		public void CreateNode(Jid to, string node)
		{
			this.CreateNode(to, null, node, true, null, null);
		}

		public void CreateNode(Jid to, Jid from, string node)
		{
			this.CreateNode(to, from, node, true, null, null);
		}

		/// <summary>
		/// Create a Node
		/// </summary>
		/// <param name="to"></param>
		/// <param name="from"></param>
		/// <param name="defaultConfig"></param>
		public void CreateNode(Jid to, Jid from, string node, bool defaultConfig)
		{
			this.CreateNode(to, from, node, defaultConfig, null, null);
		}

		public void CreateNode(Jid to, string node, bool defaultConfig, IqCB cb)
		{
			this.CreateNode(to, null, node, defaultConfig, cb, null);
		}

		public void CreateNode(Jid to, string node, bool defaultConfig, IqCB cb, object cbArgs)
		{
			this.CreateNode(to, null, node, defaultConfig, cb, cbArgs);
		}

		public void CreateNode(Jid to, Jid from, string node, bool defaultConfig, IqCB cb)
		{
			this.CreateNode(to, from, node, defaultConfig, cb, null);
		}

		public void CreateNode(Jid to, Jid from, string node, bool defaultConfig, IqCB cb, object cbArgs)
		{
			var pubsubIq = new PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Create = new Create(node);

			if (defaultConfig)
				pubsubIq.PubSub.Configure = new Configure();

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << CreateCollection Node >>
		/*
            To create a new collection node, the requesting entity MUST specify a type of "collection" when asking the service to create the node. [20]

            Example 185. Entity requests a new collection node

            <iq type='set'
                from='bard@shakespeare.lit/globe'
                to='pubsub.shakespeare.lit'
                id='create3'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                <create node='announcements' type='collection'/>
              </pubsub>
            </iq>
                
            Example 186. Service responds with success

            <iq type='result'
                from='pubsub.shakespeare.lit'
                to='bard@shakespeare.lit/globe'
                id='create3'/>               
         
        */
		public void CreateCollectionNode(Jid to, string node, bool defaultConfig)
		{
			this.CreateCollectionNode(to, null, node, defaultConfig, null, null);
		}

		public void CreateCollectionNode(Jid to, string node, bool defaultConfig, IqCB cb)
		{
			this.CreateCollectionNode(to, null, node, defaultConfig, cb, null);
		}

		public void CreateCollectionNode(Jid to, string node, bool defaultConfig, IqCB cb, object cbArgs)
		{
			this.CreateCollectionNode(to, null, node, defaultConfig, cb, cbArgs);
		}


		public void CreateCollectionNode(Jid to, Jid from, string node, bool defaultConfig)
		{
			this.CreateCollectionNode(to, from, node, defaultConfig, null, null);
		}

		public void CreateCollectionNode(Jid to, Jid from, string node, bool defaultConfig, IqCB cb)
		{
			this.CreateCollectionNode(to, from, node, defaultConfig, cb, null);
		}

		public void CreateCollectionNode(Jid to, Jid from, string node, bool defaultConfig, IqCB cb, object cbArgs)
		{
			var pubsubIq = new PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Create = new Create(node, Type.collection);

			if (defaultConfig)
				pubsubIq.PubSub.Configure = new Configure();

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Delete Node >>
		/*
            Example 133. Owner deletes a node

            <iq type='set'
                from='hamlet@denmark.lit/elsinore'
                to='pubsub.shakespeare.lit'
                id='delete1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
                <delete node='blogs/princely_musings'/>
              </pubsub>
            </iq>                
        */

		public void DeleteNode(Jid to, string node)
		{
			this.DeleteNode(to, null, node, null, null);
		}

		public void DeleteNode(Jid to, string node, IqCB cb)
		{
			this.DeleteNode(to, null, node, cb, null);
		}

		public void DeleteNode(Jid to, string node, IqCB cb, object cbArgs)
		{
			this.DeleteNode(to, null, node, cb, cbArgs);
		}

		public void DeleteNode(Jid to, Jid from, string node)
		{
			this.DeleteNode(to, from, node, null, null);
		}

		public void DeleteNode(Jid to, Jid from, string node, IqCB cb)
		{
			this.DeleteNode(to, from, node, cb, null);
		}

		public void DeleteNode(Jid to, Jid from, string node, IqCB cb, object cbArgs)
		{
			var pubsubIq = new Owner.PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Delete = new Owner.Delete(node);

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Purge Node >>
		/*
            Example 139. Owner purges all items from a node

            <iq type='set'
                from='hamlet@denmark.lit/elsinore'
                to='pubsub.shakespeare.lit'
                id='purge1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
                <purge node='blogs/princely_musings'/>
              </pubsub>
            </iq>                
        */

		public void PurgeNode(Jid to, string node)
		{
			this.PurgeNode(to, null, node, null, null);
		}

		public void PurgeNode(Jid to, string node, IqCB cb)
		{
			this.PurgeNode(to, null, node, cb, null);
		}

		public void PurgeNode(Jid to, string node, IqCB cb, object cbArgs)
		{
			this.PurgeNode(to, null, node, cb, cbArgs);
		}

		public void PurgeNode(Jid to, Jid from, string node)
		{
			this.PurgeNode(to, from, node, null, null);
		}

		public void PurgeNode(Jid to, Jid from, string node, IqCB cb)
		{
			this.PurgeNode(to, from, node, cb, null);
		}

		public void PurgeNode(Jid to, Jid from, string node, IqCB cb, object cbArgs)
		{
			var pubsubIq = new Owner.PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Purge = new Owner.Purge(node);

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Publish to a Node >>
		/*
            Example 9. Entity publishes an item with an ItemID

            <iq type="set"
                from="pgm@jabber.org"
                to="pubsub.jabber.org"
                id="publish1">
              <pubsub xmlns="http://jabber.org/protocol/pubsub">
                <publish node="generic/pgm-mp3-player">
                  <item id="current">
                    <tune xmlns="http://jabber.org/protocol/tune">
                      <artist>Ralph Vaughan Williams</artist>
                      <title>Concerto in F for Bass Tuba</title>
                      <source>Golden Brass: The Collector's Edition</source>
                    </tune>
                  </item>
                </publish>
              </pubsub>
            </iq>
        */

		/// <summary>
		/// Publish a payload to a Node
		/// </summary>
		/// <param name="to"></param>
		/// <param name="node"></param>
		/// <param name="payload"></param>
		public void PublishItem(Jid to, string node, Item payload)
		{
			this.PublishItem(to, null, node, payload, null, null);
		}

		/// <summary>
		/// Publish a payload to a Node
		/// </summary>
		/// <param name="to"></param>
		/// <param name="node"></param>
		/// <param name="payload"></param>
		/// <param name="cb"></param>
		public void PublishItem(Jid to, string node, Item payload, IqCB cb)
		{
			this.PublishItem(to, null, node, payload, cb, null);
		}

		/// <summary>
		/// Publish a payload to a Node
		/// </summary>
		/// <param name="to"></param>
		/// <param name="node"></param>
		/// <param name="payload"></param>
		/// <param name="cb"></param>
		/// <param name="cbArgs"></param>
		public void PublishItem(Jid to, string node, Item payload, IqCB cb, object cbArgs)
		{
			this.PublishItem(to, null, node, payload, cb, cbArgs);
		}

		/// <summary>
		/// Publish a payload to a Node
		/// </summary>
		/// <param name="to"></param>
		/// <param name="from"></param>
		/// <param name="node"></param>
		/// <param name="payload"></param>
		public void PublishItem(Jid to, Jid from, string node, Item payload)
		{
			this.PublishItem(to, from, node, payload, null, null);
		}

		/// <summary>
		/// Publish a payload to a Node
		/// </summary>
		/// <param name="to"></param>
		/// <param name="from"></param>
		/// <param name="node"></param>
		/// <param name="payload"></param>
		/// <param name="cb"></param>
		public void PublishItem(Jid to, Jid from, string node, Item payload, IqCB cb)
		{
			this.PublishItem(to, from, node, payload, cb, null);
		}

		/// <summary>
		/// Publish a payload to a Node
		/// </summary>
		/// <param name="to"></param>
		/// <param name="from"></param>
		/// <param name="node"></param>
		/// <param name="payload"></param>
		/// <param name="cb"></param>
		/// <param name="cbArgs"></param>
		public void PublishItem(Jid to, Jid from, string node, Item payload, IqCB cb, object cbArgs)
		{
			var pubsubIq = new PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			var publish = new Publish(node);
			publish.AddItem(payload);

			pubsubIq.PubSub.Publish = publish;

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}

		#endregion

		#region << Retract >>
		/*
            <iq type="set"
                from="pgm@jabber.org"
                to="pubsub.jabber.org"
                id="deleteitem1">
              <pubsub xmlns="http://jabber.org/protocol/pubsub">
                <retract node="generic/pgm-mp3-player">
                  <item id="current"/>
                </retract>
              </pubsub>
            </iq>
        */

		public void RetractItem(Jid to, string node, string id)
		{
			this.RetractItem(to, null, node, id, null, null);
		}

		public void RetractItem(Jid to, string node, string id, IqCB cb)
		{
			this.RetractItem(to, null, node, id, cb, null);
		}

		public void RetractItem(Jid to, string node, string id, IqCB cb, object cbArgs)
		{
			this.RetractItem(to, null, node, id, cb, cbArgs);
		}


		public void RetractItem(Jid to, Jid from, string node, string id)
		{
			this.RetractItem(to, from, node, id, null, null);
		}

		public void RetractItem(Jid to, Jid from, string node, string id, IqCB cb)
		{
			this.RetractItem(to, from, node, id, cb, null);
		}

		public void RetractItem(Jid to, Jid from, string node, string id, IqCB cb, object cbArgs)
		{
			var pubsubIq = new PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;


			pubsubIq.PubSub.Retract = new Retract(node, id);

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Subscribe >>
		/*
            <iq type="set"
                from="sub1@foo.com/home"
                to="pubsub.jabber.org"
                id="sub1">
              <pubsub xmlns="http://jabber.org/protocol/pubsub">
                <subscribe
                    node="generic/pgm-mp3-player"
                    jid="sub1@foo.com"/>
              </pubsub>
            </iq>
        */

		/// <summary>
		/// 
		/// </summary>
		/// <param name="to">Jid of the Publish Subscribe Service</param>
		/// <param name="subscribe">Jid which should be subscribed</param>
		/// <param name="node">node to which we want to subscribe</param>
		public void Subscribe(Jid to, Jid subscribe, string node)
		{
			this.Subscribe(to, null, subscribe, node, null, null);
		}

		public void Subscribe(Jid to, Jid subscribe, string node, IqCB cb)
		{
			this.Subscribe(to, null, subscribe, node, cb, null);
		}

		public void Subscribe(Jid to, Jid subscribe, string node, IqCB cb, object cbArgs)
		{
			this.Subscribe(to, null, subscribe, node, cb, cbArgs);
		}

		public void Subscribe(Jid to, Jid from, Jid subscribe, string node)
		{
			this.Subscribe(to, from, subscribe, node, null, null);
		}

		public void Subscribe(Jid to, Jid from, Jid subscribe, string node, IqCB cb)
		{
			this.Subscribe(to, from, subscribe, node, cb, null);
		}

		public void Subscribe(Jid to, Jid from, Jid subscribe, string node, IqCB cb, object cbArgs)
		{
			var pubsubIq = new PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Subscribe = new Subscribe(node, subscribe);

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}

		#endregion

		#region << Unsubscribe >>
		/*
            Example 38. Entity unsubscribes from a node

            <iq type='set'
                from='francisco@denmark.lit/barracks'
                to='pubsub.shakespeare.lit'
                id='unsub1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                 <unsubscribe
                     node='blogs/princely_musings'
                     jid='francisco@denmark.lit'/>
              </pubsub>
            </iq>
    
        */

		/// <summary>
		/// 
		/// </summary>
		/// <param name="to">Jid of the Publish Subscribe Service</param>
		/// <param name="subscribe">Jid which should be subscribed</param>
		/// <param name="node">node to which we want to subscribe</param>
		public void Unsubscribe(Jid to, Jid unsubscribe, string node)
		{
			this.Unsubscribe(to, null, unsubscribe, node, null, null);
		}

		public void Unsubscribe(Jid to, Jid unsubscribe, string node, string subid)
		{
			this.Unsubscribe(to, null, unsubscribe, node, subid, null, null);
		}

		public void Unsubscribe(Jid to, Jid unsubscribe, string node, IqCB cb)
		{
			this.Unsubscribe(to, null, unsubscribe, node, cb, null);
		}

		public void Unsubscribe(Jid to, Jid unsubscribe, string node, string subid, IqCB cb)
		{
			this.Unsubscribe(to, null, unsubscribe, node, subid, cb, null);
		}

		public void Unsubscribe(Jid to, Jid unsubscribe, string node, IqCB cb, object cbArgs)
		{
			this.Unsubscribe(to, null, unsubscribe, node, cb, cbArgs);
		}

		public void Unsubscribe(Jid to, Jid unsubscribe, string node, string subid, IqCB cb, object cbArgs)
		{
			this.Unsubscribe(to, null, unsubscribe, node, subid, cb, cbArgs);
		}

		public void Unsubscribe(Jid to, Jid from, Jid unsubscribe, string node)
		{
			this.Unsubscribe(to, from, unsubscribe, node, null, null);
		}

		public void Unsubscribe(Jid to, Jid from, Jid unsubscribe, string node, string subid)
		{
			this.Unsubscribe(to, from, unsubscribe, node, subid, null, null);
		}

		public void Unsubscribe(Jid to, Jid from, Jid unsubscribe, string node, IqCB cb)
		{
			this.Unsubscribe(to, from, unsubscribe, node, cb, null);
		}

		public void Unsubscribe(Jid to, Jid from, Jid unsubscribe, string node, string subid, IqCB cb)
		{
			this.Unsubscribe(to, from, unsubscribe, node, subid, cb, null);
		}

		public void Unsubscribe(Jid to, Jid from, Jid unsubscribe, string node, IqCB cb, object cbArgs)
		{
			this.Unsubscribe(to, from, unsubscribe, node, null, cb, cbArgs);
		}

		public void Unsubscribe(Jid to, Jid from, Jid unsubscribe, string node, string subid, IqCB cb, object cbArgs)
		{
			var pubsubIq = new PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			var unsub = new Unsubscribe(node, unsubscribe);
			if (subid != null)
				unsub.SubId = subid;

			pubsubIq.PubSub.Unsubscribe = unsub;

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}

		#endregion

		#region << Request Subscriptions >>>
		/*
            <iq type='get'
                from='francisco@denmark.lit/barracks'
                to='pubsub.shakespeare.lit'
                id='subscriptions1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                <subscriptions/>
              </pubsub>
            </iq>
        */
		public void RequestSubscriptions(Jid to)
		{
			this.RequestSubscriptions(to, null, null, null);
		}

		public void RequestSubscriptions(Jid to, IqCB cb)
		{
			this.RequestSubscriptions(to, null, cb, null);
		}

		public void RequestSubscriptions(Jid to, IqCB cb, object cbArgs)
		{
			this.RequestSubscriptions(to, null, cb, cbArgs);
		}

		public void RequestSubscriptions(Jid to, Jid from)
		{
			this.RequestSubscriptions(to, from, null, null);
		}

		public void RequestSubscriptions(Jid to, Jid from, IqCB cb)
		{
			this.RequestSubscriptions(to, from, cb, null);
		}

		public void RequestSubscriptions(Jid to, Jid from, IqCB cb, object cbArgs)
		{
			var pubsubIq = new PubSubIq(IQType.get, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Subscriptions = new Subscriptions();

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Owner Request Affiliations >>
		/*
            <iq type='get'
                from='francisco@denmark.lit/barracks'
                to='pubsub.shakespeare.lit'
                id='affil1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                <affiliations/>
              </pubsub>
            </iq>
        */
		public void RequestAffiliations(Jid to)
		{
			this.RequestAffiliations(to, null, null, null);
		}

		public void RequestAffiliations(Jid to, IqCB cb)
		{
			this.RequestAffiliations(to, null, cb, null);
		}

		public void RequestAffiliations(Jid to, IqCB cb, object cbArgs)
		{
			this.RequestAffiliations(to, null, cb, cbArgs);
		}

		public void RequestAffiliations(Jid to, Jid from)
		{
			this.RequestAffiliations(to, from, null, null);
		}

		public void RequestAffiliations(Jid to, Jid from, IqCB cb)
		{
			this.RequestAffiliations(to, from, cb, null);
		}

		public void RequestAffiliations(Jid to, Jid from, IqCB cb, object cbArgs)
		{
			var pubsubIq = new PubSubIq(IQType.get, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Affiliations = new Affiliations();

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Request Subscription Options >>
		/*
            <iq type='get'
                from='francisco@denmark.lit/barracks'
                to='pubsub.shakespeare.lit'
                id='options1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                <options node='blogs/princely_musings' jid='francisco@denmark.lit'/>
              </pubsub>
            </iq>
        */

		public void RequestSubscriptionOptions(Jid to, Jid subscribe, string node)
		{
			this.RequestSubscriptionOptions(to, null, subscribe, node, null, null);
		}

		public void RequestSubscriptionOptions(Jid to, Jid subscribe, string node, IqCB cb)
		{
			this.RequestSubscriptionOptions(to, null, subscribe, node, cb, null);
		}

		public void RequestSubscriptionOptions(Jid to, Jid subscribe, string node, IqCB cb, object cbArgs)
		{
			this.RequestSubscriptionOptions(to, null, subscribe, node, cb, cbArgs);
		}

		public void RequestSubscriptionOptions(Jid to, Jid from, Jid subscribe, string node)
		{
			this.RequestSubscriptionOptions(to, from, subscribe, node, null, null);
		}

		public void RequestSubscriptionOptions(Jid to, Jid from, Jid subscribe, string node, IqCB cb)
		{
			this.RequestSubscriptionOptions(to, from, subscribe, node, cb, null);
		}

		public void RequestSubscriptionOptions(Jid to, Jid from, Jid subscribe, string node, IqCB cb, object cbArgs)
		{
			var pubsubIq = new PubSubIq(IQType.get, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Options = new Options(subscribe, node);

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Request All Subscribers >>
		/*
            <iq type='get'
                from='hamlet@denmark.lit/elsinore'
                to='pubsub.shakespeare.lit'
                id='subman1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
                <subscribers node='blogs/princely_musings'/>
              </pubsub>
            </iq>
        */

		public void OwnerRequestSubscribers(Jid to, string node)
		{
			this.OwnerRequestSubscribers(to, null, node, null, null);
		}

		public void OwnerRequestSubscribers(Jid to, string node, IqCB cb)
		{
			this.OwnerRequestSubscribers(to, null, node, cb, null);
		}

		public void OwnerRequestSubscribers(Jid to, string node, IqCB cb, object cbArgs)
		{
			this.OwnerRequestSubscribers(to, null, node, cb, cbArgs);
		}

		public void OwnerRequestSubscribers(Jid to, Jid from, string node)
		{
			this.OwnerRequestSubscribers(to, from, node, null, null);
		}

		public void OwnerRequestSubscribers(Jid to, Jid from, string node, IqCB cb)
		{
			this.OwnerRequestSubscribers(to, from, node, cb, null);
		}

		public void OwnerRequestSubscribers(Jid to, Jid from, string node, IqCB cb, object cbArgs)
		{
			var pubsubIq = new Owner.PubSubIq(IQType.get, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Subscribers = new Owner.Subscribers(node);

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Modifying single Subscription State >>
		/*
            Upon receiving the subscribers list, the node owner MAY modify subscription states. 
            The owner MUST send only modified subscription states (i.e., a "delta"), not the complete list.
            (Note: If the 'subscription' attribute is not specified in a modification request, then the value
            MUST NOT be changed.)

            Example 163. Owner modifies subscriptions

            <iq type='set'
                from='hamlet@denmark.lit/elsinore'
                to='pubsub.shakespeare.lit'
                id='subman3'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
                <subscribers node='blogs/princely_musings'>
                  <subscriber jid='polonius@denmark.lit' subscription='none'/>                  
                </subscribers>
              </pubsub>
            </iq>
    
        */

		public void OwnerModifySubscriptionState(Jid to, string node, Jid subscriber, SubscriptionState state)
		{
			this.OwnerModifySubscriptionState(to, null, node, subscriber, state, null, null);
		}

		public void OwnerModifySubscriptionState(Jid to, string node, Jid subscriber, SubscriptionState state, IqCB cb)
		{
			this.OwnerModifySubscriptionState(to, null, node, subscriber, state, cb, null);
		}

		public void OwnerModifySubscriptionState(Jid to, string node, Jid subscriber, SubscriptionState state, IqCB cb, object cbArgs)
		{
			this.OwnerModifySubscriptionState(to, null, node, subscriber, state, cb, cbArgs);
		}


		public void OwnerModifySubscriptionState(Jid to, Jid from, string node, Jid subscriber, SubscriptionState state)
		{
			this.OwnerModifySubscriptionState(to, from, node, subscriber, state, null, null);
		}

		public void OwnerModifySubscriptionState(Jid to, Jid from, string node, Jid subscriber, SubscriptionState state, IqCB cb)
		{
			this.OwnerModifySubscriptionState(to, from, node, subscriber, state, cb, null);
		}

		public void OwnerModifySubscriptionState(Jid to, Jid from, string node, Jid subscriber, SubscriptionState state, IqCB cb, object cbArgs)
		{
			var pubsubIq = new Owner.PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			var subs = new Owner.Subscribers(node);
			subs.AddSubscriber(new Owner.Subscriber(subscriber, state));

			pubsubIq.PubSub.Subscribers = subs;

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Modifying multiple Subscription States >>
		/*
            <iq type='set'
                from='hamlet@denmark.lit/elsinore'
                to='pubsub.shakespeare.lit'
                id='subman3'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
                <subscribers node='blogs/princely_musings'>
                    <subscriber jid='polonius@denmark.lit' subscription='none'/>
                    <subscriber jid='bard@shakespeare.lit' subscription='subscribed'/>                 
                </subscribers>
              </pubsub>
            </iq>
        */

		public void OwnerModifySubscriptionStates(Jid to, string node, Owner.Subscriber[] subscribers)
		{
			this.OwnerModifySubscriptionStates(to, null, node, subscribers, null, null);
		}

		public void OwnerModifySubscriptionStates(Jid to, string node, Owner.Subscriber[] subscribers, IqCB cb)
		{
			this.OwnerModifySubscriptionStates(to, null, node, subscribers, cb, null);
		}

		public void OwnerModifySubscriptionStates(Jid to, string node, Owner.Subscriber[] subscribers, IqCB cb, object cbArgs)
		{
			this.OwnerModifySubscriptionStates(to, null, node, subscribers, cb, cbArgs);
		}


		public void OwnerModifySubscriptionStates(Jid to, Jid from, string node, Owner.Subscriber[] subscribers)
		{
			this.OwnerModifySubscriptionStates(to, from, node, subscribers, null, null);
		}

		public void OwnerModifySubscriptionStates(Jid to, Jid from, string node, Owner.Subscriber[] subscribers, IqCB cb)
		{
			this.OwnerModifySubscriptionStates(to, from, node, subscribers, cb, null);
		}

		public void OwnerModifySubscriptionStates(Jid to, Jid from, string node, Owner.Subscriber[] subscribers, IqCB cb, object cbArgs)
		{
			var pubsubIq = new Owner.PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			var subs = new Owner.Subscribers(node);
			subs.AddSubscribers(subscribers);

			pubsubIq.PubSub.Subscribers = subs;

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Owner Request Affiliations >>
		/*
            Example 168. Owner requests all affiliated entities

            <iq type='get'
                from='hamlet@denmark.lit/elsinore'
                to='pubsub.shakespeare.lit'
                id='ent1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
                <affiliates node='blogs/princely_musings'/>
              </pubsub>
            </iq>                
        */

		public void OwnerRequestAffiliations(Jid to, string node)
		{
			this.OwnerRequestAffiliations(to, null, node, null, null);
		}

		public void OwnerRequestAffiliations(Jid to, string node, IqCB cb)
		{
			this.OwnerRequestAffiliations(to, null, node, cb, null);
		}

		public void OwnerRequestAffiliations(Jid to, string node, IqCB cb, object cbArgs)
		{
			this.OwnerRequestAffiliations(to, null, node, cb, cbArgs);
		}


		public void OwnerRequestAffiliations(Jid to, Jid from, string node)
		{
			this.OwnerRequestAffiliations(to, from, node, null, null);
		}

		public void OwnerRequestAffiliations(Jid to, Jid from, string node, IqCB cb)
		{
			this.OwnerRequestAffiliations(to, from, node, cb, null);
		}

		public void OwnerRequestAffiliations(Jid to, Jid from, string node, IqCB cb, object cbArgs)
		{
			var pubsubIq = new Owner.PubSubIq(IQType.get, to);

			if (from != null)
				pubsubIq.From = from;

			pubsubIq.PubSub.Affiliates = new Owner.Affiliates(node);

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Owner Set/Modify Affiliation >>
		/*
            Owner modifies a single affiliation

            <iq type='set'
                from='hamlet@denmark.lit/elsinore'
                to='pubsub.shakespeare.lit'
                id='ent2'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
                <affiliates node='blogs/princely_musings'/>
                  <affiliate jid='hamlet@denmark.lit' affiliation='owner'/>                 
                </affiliates>
              </pubsub>
            </iq>
    
        */

		public void OwnerModifyAffiliation(Jid to, string node, Jid affiliate, AffiliationType affiliation)
		{
			this.OwnerModifyAffiliation(to, null, node, affiliate, affiliation, null, null);
		}

		public void OwnerModifyAffiliation(Jid to, string node, Jid affiliate, AffiliationType affiliation, IqCB cb)
		{
			this.OwnerModifyAffiliation(to, null, node, affiliate, affiliation, cb, null);
		}

		public void OwnerModifyAffiliation(Jid to, string node, Jid affiliate, AffiliationType affiliation, IqCB cb, object cbArgs)
		{
			this.OwnerModifyAffiliation(to, null, node, affiliate, affiliation, cb, cbArgs);
		}


		public void OwnerModifyAffiliation(Jid to, Jid from, string node, Jid affiliate, AffiliationType affiliation)
		{
			this.OwnerModifyAffiliation(to, from, node, affiliate, affiliation, null, null);
		}

		public void OwnerModifyAffiliation(Jid to, Jid from, string node, Jid affiliate, AffiliationType affiliation, IqCB cb)
		{
			this.OwnerModifyAffiliation(to, from, node, affiliate, affiliation, cb, null);
		}

		public void OwnerModifyAffiliation(Jid to, Jid from, string node, Jid affiliate, AffiliationType affiliation, IqCB cb, object cbArgs)
		{
			var pubsubIq = new Owner.PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			var aff = new Owner.Affiliates(node);
			aff.AddAffiliate(new Owner.Affiliate(affiliate, affiliation));

			pubsubIq.PubSub.Affiliates = aff;

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion

		#region << Owner Modify Affiliations >>
		/*
            Owner modifies a single affiliation

            <iq type='set'
                from='hamlet@denmark.lit/elsinore'
                to='pubsub.shakespeare.lit'
                id='ent2'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
                <affiliates node='blogs/princely_musings'/>
                  <affiliate jid='hamlet@denmark.lit' affiliation='owner'/>
                  <affiliate jid='polonius@denmark.lit' affiliation='none'/>
                  <affiliate jid='bard@shakespeare.lit' affiliation='publisher'/>
                </affiliates>
              </pubsub>
            </iq>
        */

		public void OwnerModifyAffiliations(Jid to, string node, Owner.Affiliate[] affiliates)
		{
			this.OwnerModifyAffiliations(to, null, node, affiliates, null, null);
		}

		public void OwnerModifyAffiliations(Jid to, string node, Owner.Affiliate[] affiliates, IqCB cb)
		{
			this.OwnerModifyAffiliations(to, null, node, affiliates, cb, null);
		}

		public void OwnerModifyAffiliations(Jid to, string node, Owner.Affiliate[] affiliates, IqCB cb, object cbArgs)
		{
			this.OwnerModifyAffiliations(to, null, node, affiliates, cb, cbArgs);
		}


		public void OwnerModifyAffiliations(Jid to, Jid from, string node, Owner.Affiliate[] affiliates)
		{
			this.OwnerModifyAffiliations(to, from, node, affiliates, null, null);
		}

		public void OwnerModifyAffiliations(Jid to, Jid from, string node, Owner.Affiliate[] affiliates, IqCB cb)
		{
			this.OwnerModifyAffiliations(to, from, node, affiliates, cb, null);
		}

		public void OwnerModifyAffiliations(Jid to, Jid from, string node, Owner.Affiliate[] affiliates, IqCB cb, object cbArgs)
		{
			var pubsubIq = new Owner.PubSubIq(IQType.set, to);

			if (from != null)
				pubsubIq.From = from;

			var affs = new Owner.Affiliates(node);
			affs.AddAffiliates(affiliates);

			pubsubIq.PubSub.Affiliates = affs;

			if (cb == null)
				this.m_connection.Send(pubsubIq);
			else
				this.m_connection.IqGrabber.SendIq(pubsubIq, cb, cbArgs);
		}
		#endregion
	}

}