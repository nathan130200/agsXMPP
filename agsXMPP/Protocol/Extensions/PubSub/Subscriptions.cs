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

namespace AgsXMPP.Protocol.Extensions.PubSub
{
	public class Subscriptions : Element
	{
		/*
            Example 14. Entity requests all current subscriptions

            <iq type='get'
                from='francisco@denmark.lit/barracks'
                to='pubsub.shakespeare.lit'
                id='subscriptions1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                <subscriptions/>
              </pubsub>
            </iq>
                       

            Example 15. Service returns all current subscriptions

            <iq type='result'
                from='pubsub.shakespeare.lit'
                to='francisco@denmark.lit'
                id='subscriptions1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                <subscriptions>
                  <subscription node='node1' jid='francisco@denmark.lit' subscription='subscribed'/>
                  <subscription node='node2' jid='francisco@denmark.lit' subscription='subscribed'/>
                  <subscription node='node5' jid='francisco@denmark.lit' subscription='unconfigured'/>
                  <subscription node='node6' jid='francisco@denmark.lit' subscription='pending'/>
                </subscriptions>
              </pubsub>
            </iq>
    
        */
		public Subscriptions()
		{
			this.TagName = "subscriptions";
			this.Namespace = URI.PUBSUB;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Subscription AddSubscription()
		{
			var sub = new Subscription();
			this.AddChild(sub);
			return sub;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public Subscription AddSubscription(Subscription sub)
		{
			this.AddChild(sub);
			return sub;
		}

		public Subscription[] GetSubscriptions()
		{
			var nl = this.SelectElements(typeof(Subscription));
			var items = new Subscription[nl.Count];
			var i = 0;
			foreach (Element e in nl)
			{
				items[i] = (Subscription)e;
				i++;
			}
			return items;
		}
	}
}
