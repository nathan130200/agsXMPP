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

// Note: This JEP is superseded by JEP-0030: Service Discovery.

// WARNING: This JEP has been deprecated by the Jabber Software Foundation. 
// Implementation of the protocol described herein is not recommended. Developers desiring similar functionality should 
// implement the protocol that supersedes this one (if any).

// Most components and gateways still dont implement Service discovery. So we must use jabber:iq:browse for them until everything is replaced with JEP 30 (Service Discovery).
namespace AgsXMPP.Protocol.Iq.browse
{
	/// <summary>
	/// JEP-0011: Jabber Browsing.
	/// <para>
	/// This JEP defines a way to describe information about Jabber entities and the relationships between entities. </para>
	/// </summary>
	public class Browse : Element
	{
		public Browse()
		{
			this.TagName = "query";
			this.Namespace = Namespaces.IQ_BROWSE;
		}

		public string Category
		{
			get { return this.GetAttribute("category"); }
			set { this.SetAttribute("category", value); }
		}

		public string Type
		{
			get { return this.GetAttribute("type"); }
			set { this.SetAttribute("type", value); }
		}

		public string Name
		{
			get { return this.GetAttribute("name"); }
			set { this.SetAttribute("name", value); }
		}

		public string[] GetNamespaces()
		{
			var elements = this.SelectElements("ns");
			var nss = new string[elements.Count];

			var i = 0;
			foreach (Element ns in elements)
			{
				nss[i] = ns.Value;
				i++;
			}

			return nss;
		}

		public BrowseItem[] GetItems()
		{
			var nl = this.SelectElements(typeof(BrowseItem));
			var items = new BrowseItem[nl.Count];
			var i = 0;
			foreach (Element item in nl)
			{
				items[i] = item as BrowseItem;
				i++;
			}
			return items;
		}
	}
}
