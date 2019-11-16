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

using agsXMPP.Xml.Dom;

namespace agsXMPP.Protocol.extensions.pubsub.Owner
{
	/*
        <iq type='result'
            from='pubsub.shakespeare.lit'
            to='hamlet@denmark.lit/elsinore'
            id='subman1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
            <subscribers node='blogs/princely_musings'>
              <subscriber jid='hamlet@denmark.lit' subscription='subscribed'/>
              <subscriber jid='polonius@denmark.lit' subscription='unconfigured'/>
            </subscribers>
          </pubsub>
        </iq>
        
        <xs:element name='subscribers'>
            <xs:complexType>
              <xs:sequence>
                <xs:element ref='subscriber' minOccurs='0' maxOccurs='unbounded'/>
              </xs:sequence>
              <xs:attribute name='node' type='xs:string' use='required'/>
            </xs:complexType>
        </xs:element>
    */
	public class Subscribers : Element
	{
		#region << Constructors >>
		public Subscribers()
		{
			this.TagName = "subscribers";
			this.Namespace = Namespaces.PUBSUB_OWNER;
		}

		public Subscribers(string node) : this()
		{
			this.Node = node;
		}
		#endregion

		public string Node
		{
			get { return this.GetAttribute("node"); }
			set { this.SetAttribute("node", value); }
		}

		/// <summary>
		/// Add a Subscriber
		/// </summary>
		/// <returns></returns>
		public Subscriber AddSubscriber()
		{
			var subscriber = new Subscriber();
			this.AddChild(subscriber);
			return subscriber;
		}

		/// <summary>
		/// Add a Subscriber
		/// </summary>
		/// <param name="subscriber">the Subscriber to add</param>
		/// <returns></returns>
		public Subscriber AddSubscriber(Subscriber subscriber)
		{
			this.AddChild(subscriber);
			return subscriber;
		}

		public void AddSubscribers(Subscriber[] subscribers)
		{
			foreach (var subscriber in subscribers)
			{
				this.AddSubscriber(subscriber);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Subscriber[] GetSubscribers()
		{
			var nl = this.SelectElements(typeof(Subscriber));
			var subscribers = new Subscriber[nl.Count];
			var i = 0;
			foreach (Element e in nl)
			{
				subscribers[i] = (Subscriber)e;
				i++;
			}
			return subscribers;
		}
	}
}
