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

namespace AgsXMPP.Protocol.X.Muc.iq.admin
{
	/*
        <query xmlns='http://jabber.org/protocol/muc#admin'>
            <item nick='pistol' role='none'>
              <reason>Avaunt, you cullion!</reason>
            </item>
        </query>
    */

	/// <summary>
	/// 
	/// </summary>
	public class Admin : Element
	{
		public Admin()
		{
			this.TagName = "query";
			this.Namespace = URI.MUC_ADMIN;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void AddItem(Item item)
		{
			this.AddChild(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void AddItems(Item[] items)
		{
			foreach (var itm in items)
			{
				this.AddItem(itm);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Item[] GetItems()
		{
			var nl = this.SelectElements(typeof(Item));
			var items = new Item[nl.Count];
			var i = 0;
			foreach (Item itm in nl)
			{
				items[i] = itm;
				i++;
			}
			return items;
		}

	}
}
