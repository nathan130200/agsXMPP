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

namespace AgsXMPP.Protocol.Query.Disco
{
	/*
	Example 10. Requesting all items

	<iq type='get'
	from='romeo@montague.net/orchard'
	to='shakespeare.lit'
	id='items1'>
	<query xmlns='http://jabber.org/protocol/disco#items'/>
	</iq>
	
	
	Example 11. Result-set for all items

	<iq type='result'
		from='shakespeare.lit'
		to='romeo@montague.net/orchard'
		id='items1'>
	<query xmlns='http://jabber.org/protocol/disco#items'>
		<item jid='people.shakespeare.lit'
			name='Directory of Characters'/>
		<item jid='plays.shakespeare.lit'
			name='Play-Specific Chatrooms'/>
		<item jid='mim.shakespeare.lit'
			name='Gateway to Marlowe IM'/>
		<item jid='words.shakespeare.lit'
			name='Shakespearean Lexicon'/>
		<item jid='globe.shakespeare.lit'
			name='Calendar of Performances'/>
		<item jid='headlines.shakespeare.lit'
			name='Latest Shakespearean News'/>
		<item jid='catalog.shakespeare.lit'
			name='Buy Shakespeare Stuff!'/>
		<item jid='en2fr.shakespeare.lit'
			name='French Translation Service'/>
	</query>
	</iq>
	
	
	Example 12. Empty result set

	<iq type='result'
		from='shakespeare.lit'
		to='romeo@montague.net/orchard'
		id='items1'>
	<query xmlns='http://jabber.org/protocol/disco#items'/>
	</iq>
      
    */

	/// <summary>
	/// Discovering the Items Associated with a Jabber Entity
	/// </summary>
	public class DiscoItems : Client.IQ
	{
		public DiscoItems()
		{
			this.TagName = "query";
			this.Namespace = URI.DISCO_ITEMS;
		}

		/// <summary>
		/// The node to discover (Optional)
		/// </summary>
		public string Node
		{
			get { return this.GetAttribute("node"); }
			set { this.SetAttribute("node", value); }
		}

		public DiscoItem AddDiscoItem()
		{
			var item = new DiscoItem();
			this.AddChild(item);
			return item;
		}

		public void AddDiscoItem(DiscoItem item)
		{
			this.AddChild(item);
		}

		public DiscoItem[] GetDiscoItems()
		{
			var nl = this.SelectElements(typeof(DiscoItem));
			var items = new DiscoItem[nl.Count];
			var i = 0;
			foreach (Element e in nl)
			{
				items[i] = (DiscoItem)e;
				i++;
			}
			return items;
		}
	}
}
