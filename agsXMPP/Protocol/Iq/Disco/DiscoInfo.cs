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

/*
Example 1. Querying for further information

<iq type='get'
from='romeo@montague.net/orchard'
to='plays.shakespeare.lit'
id='info1'>
<query xmlns='http://jabber.org/protocol/disco#info'/>
</iq>


Example 2. Result-set for information request

<iq type='result'
    from='plays.shakespeare.lit'
    to='romeo@montague.net/orchard'
    id='info1'>
  <query xmlns='http://jabber.org/protocol/disco#info'>
    <identity
        category='conference'
        type='text'
        name='Play-Specific Chatrooms'/>
    <identity
        category='directory'
        type='chatroom'
        name='Play-Specific Chatrooms'/>
    <feature var='http://jabber.org/protocol/disco#info'/>
    <feature var='http://jabber.org/protocol/disco#items'/>
    <feature var='http://jabber.org/protocol/muc'/>
    <feature var='jabber:iq:register'/>
    <feature var='jabber:iq:search'/>
    <feature var='jabber:iq:time'/>
    <feature var='jabber:iq:version'/>
  </query>
</iq>
    

Example 3. Target entity does not exist

<iq type='error'
    from='plays.shakespeare.lit'
    to='romeo@montague.net/orchard'
    id='info1'>
  <query xmlns='http://jabber.org/protocol/disco#info'/>
  <error code='404' type='cancel'>
    <item-not-found xmlns='urn:ietf:xml:params:ns:xmpp-stanzas'/>
  </error>
</iq>
    
 */
namespace agsXMPP.Protocol.iq.disco
{
	/// <summary>
	/// Discovering Information About a Jabber Entity
	/// </summary>
	public class DiscoInfo : Element
	{
		public DiscoInfo()
		{
			this.TagName = "query";
			this.Namespace = Namespaces.DISCO_INFO;
		}

		/// <summary>
		/// Optional node Attrib
		/// </summary>
		public string Node
		{
			get { return this.GetAttribute("node"); }
			set { this.SetAttribute("node", value); }
		}

		public DiscoIdentity AddIdentity()
		{
			var id = new DiscoIdentity();
			this.AddChild(id);
			return id;
		}

		public void AddIdentity(DiscoIdentity id)
		{
			this.AddChild(id);
		}

		public DiscoFeature AddFeature()
		{
			var f = new DiscoFeature();
			this.AddChild(f);
			return f;
		}

		public void AddFeature(DiscoFeature f)
		{
			this.AddChild(f);
		}

		public DiscoIdentity[] GetIdentities()
		{
			var nl = this.SelectElements(typeof(DiscoIdentity));
			var items = new DiscoIdentity[nl.Count];
			var i = 0;
			foreach (Element e in nl)
			{
				items[i] = (DiscoIdentity)e;
				i++;
			}
			return items;
		}

		/// <summary>
		/// Gets all Features
		/// </summary>
		/// <returns></returns>
		public DiscoFeature[] GetFeatures()
		{
			var nl = this.SelectElements(typeof(DiscoFeature));
			var items = new DiscoFeature[nl.Count];
			var i = 0;
			foreach (Element e in nl)
			{
				items[i] = (DiscoFeature)e;
				i++;
			}
			return items;
		}

		/// <summary>
		/// Check if a feature is supported
		/// </summary>
		/// <param name="var"></param>
		/// <returns></returns>
		public bool HasFeature(string var)
		{
			var features = this.GetFeatures();
			foreach (var feat in features)
			{
				if (feat.Var == var)
					return true;
			}
			return false;
		}
	}
}
