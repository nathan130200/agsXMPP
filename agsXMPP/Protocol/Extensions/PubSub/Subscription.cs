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

using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Extensions.PubSub
{
	public class Subscription : Element
	{
		/*
            Example 23. Service replies with success

            <iq type='result'
                from='pubsub.shakespeare.lit'
                to='francisco@denmark.lit/barracks'
                id='sub1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                <subscription 
                    node='blogs/princely_musings'
                    jid='francisco@denmark.lit'
                    subid='ba49252aaa4f5d320c24d3766f0bdcade78c78d3'
                    subscription='subscribed'/>
              </pubsub>
            </iq>
         
            
            Example 36. Service replies with success and indicates that subscription configuration is required

            <iq type='result'
                from='pubsub.shakespeare.lit'
                to='francisco@denmark.lit/barracks'
                id='sub1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                <subscription 
                    node='blogs/princely_musings'
                    jid='francisco@denmark.lit'
                    subscription='unconfigured'>
                  <subscribe-options>
                    <required/>
                  </subscribe-options>
                </subscription>
              </pubsub>
            </iq>
                
    
            <xs:element name='subscription'>
                <xs:complexType>
                  <xs:sequence>
                    <xs:element ref='subscribe-options' minOccurs='0'/>
                  </xs:sequence>
                  <xs:attribute name='jid' type='xs:string' use='required'/>
                  <xs:attribute name='node' type='xs:string' use='optional'/>
                  <xs:attribute name='subid' type='xs:string' use='optional'/>
                  <xs:attribute name='subscription' use='optional'>
                    <xs:simpleType>
                      <xs:restriction base='xs:NCName'>
                        <xs:enumeration value='pending'/>
                        <xs:enumeration value='subscribed'/>
                        <xs:enumeration value='unconfigured'/>
                      </xs:restriction>
                    </xs:simpleType>
                  </xs:attribute>
                </xs:complexType>
            </xs:element>
        */

		#region << Constructors >>
		public Subscription()
		{
			this.TagName = "subscription";
			this.Namespace = URI.PUBSUB;
		}

		public Subscription(Jid jid) : this()
		{
			this.Jid = jid;
		}

		public Subscription(Jid jid, SubscriptionState subType) : this(jid)
		{
			this.SubscriptionState = subType;
		}

		public Subscription(Jid jid, string node) : this()
		{
			this.Node = node;
		}

		public Subscription(Jid jid, string node, SubscriptionState subType) : this(jid, node)
		{
			this.SubscriptionState = subType;
		}

		public Subscription(Jid jid, string node, string subId) : this(jid, node)
		{
			this.SubId = subId;
		}

		public Subscription(Jid jid, string node, string subId, SubscriptionState subType) : this(jid, node, subId)
		{
			this.SubscriptionState = subType;
		}
		#endregion

		/// <summary>
		/// Node (optional)
		/// </summary>
		public string Node
		{
			get { return this.GetAttribute("node"); }
			set { this.SetAttribute("node", value); }
		}

		public Jid Jid
		{
			get
			{
				if (this.HasAttribute("jid"))
					return new Jid(this.GetAttribute("jid"));
				else
					return null;
			}
			set
			{
				if (value != null)
					this.SetAttribute("jid", value.ToString());
				else
					this.RemoveAttribute("jid");
			}
		}

		/// <summary>
		/// Subscription ID (optional)
		/// </summary>
		public string SubId
		{
			get { return this.GetAttribute("subid"); }
			set
			{
				if (value != null)
					this.SetAttribute("subid", value);
				else
					this.RemoveAttribute("subid");
			}
		}

		public AffiliationType Affiliation
		{
			get => this.GetAttributeEnum<AffiliationType>("affiliation");
			set => this.SetAttributeEnum("affiliation", value);
		}

		public SubscriptionState SubscriptionState
		{
			get => this.GetAttributeEnum<SubscriptionState>("subscription");
			set => this.SetAttributeEnum("subscription", value);
		}

		public SubscribeOptions SubscribeOptions
		{
			get => this.SelectSingleElement(typeof(SubscribeOptions)) as SubscribeOptions;
			set
			{
				if (this.HasTag(typeof(SubscribeOptions)))
					this.RemoveTag(typeof(SubscribeOptions));

				if (value != null)
					this.AddChild(value);
			}
		}

	}
}