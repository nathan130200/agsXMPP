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

using agsXMPP.Protocol.x.data;

using agsXMPP.Xml.Dom;

//	Example 1. Requesting Search Fields
//
//	<iq type='get'
//		from='romeo@montague.net/home'
//		to='characters.shakespeare.lit'
//		id='search1'
//		xml:lang='en'>
//		<query xmlns='jabber:iq:search'/>
//	</iq>
//

//	The service MUST then return the possible search fields to the user, and MAY include instructions:
//
//	Example 2. Receiving Search Fields
//
//	<iq type='result'
//		from='characters.shakespeare.lit'
//		to='romeo@montague.net/home'
//		id='search1'
//		xml:lang='en'>
//		<query xmlns='jabber:iq:search'>
//			<instructions>
//			Fill in one or more fields to search
//			for any matching Jabber users.
//			</instructions>
//			<first/>
//			<last/>
//			<nick/>
//			<email/>
//		</query>
//	</iq>    

namespace agsXMPP.Protocol.iq.search
{
	/// <summary>
	/// http://www.jabber.org/jeps/jep-0055.html
	/// </summary>
	public class Search : Element
	{
		public Search()
		{
			this.TagName = "query";
			this.Namespace = Namespaces.IQ_SEARCH;
		}

		public string Instructions
		{
			get
			{
				return this.GetTag("instructions");
			}
			set
			{
				this.SetTag("instructions", value);
			}
		}

		public string Firstname
		{
			get
			{
				return this.GetTag("first");
			}
			set
			{
				this.SetTag("first", value);
			}
		}

		public string Lastname
		{
			get
			{
				return this.GetTag("last");
			}
			set
			{
				this.SetTag("last", value);
			}
		}

		public string Nickname
		{
			get
			{
				return this.GetTag("nick");
			}
			set
			{
				this.SetTag("nick", value);
			}
		}

		public string Email
		{
			get
			{
				return this.GetTag("email");
			}
			set
			{
				this.SetTag("email", value);
			}
		}

		/// <summary>
		/// The X-Data Element
		/// </summary>
		public Data Data
		{
			get
			{
				return this.SelectSingleElement(typeof(Data)) as Data;

			}
			set
			{
				if (this.HasTag(typeof(Data)))
					this.RemoveTag(typeof(Data));

				if (value != null)
					this.AddChild(value);
			}
		}

		/// <summary>
		/// Retrieve the result items of a search
		/// </summary>
		//public ElementList GetItems
		//{
		//    get
		//    {
		//        return this.SelectElements("item");
		//    }			
		//}

		public SearchItem[] GetItems()
		{
			var nl = this.SelectElements(typeof(SearchItem));
			var items = new SearchItem[nl.Count];
			var i = 0;
			foreach (Element e in nl)
			{
				items[i] = (SearchItem)e;
				i++;
			}
			return items;
		}

	}
}
