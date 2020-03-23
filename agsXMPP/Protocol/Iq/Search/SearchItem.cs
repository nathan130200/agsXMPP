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

namespace AgsXMPP.Protocol.Iq.search
{
	// jabber:iq:search
	//	Example 4. Receiving Search Results
	//
	//	<iq type='result'
	//		from='characters.shakespeare.lit'
	//		to='romeo@montague.net/home'
	//		id='search2'
	//		xml:lang='en'>
	//		<query xmlns='jabber:iq:search'>
	//			<item jid='juliet@capulet.com'>
	//				<first>Juliet</first>
	//				<last>Capulet</last>
	//				<nick>JuliC</nick>
	//				<email>juliet@shakespeare.lit</email>
	//			</item>
	//			<item jid='stpeter@jabber.org'>
	//				<first>Tybalt</first>
	//				<last>Capulet</last>
	//				<nick>ty</nick>
	//				<email>tybalt@shakespeare.lit</email>
	//			</item>
	//		</query>
	//	</iq>

	/// <summary>
	///
	/// </summary>
	public class SearchItem : Element
	{
		public SearchItem()
		{
			this.TagName = "item";
			this.Namespace = Namespaces.IQ_SEARCH;
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

		/// <summary>
		/// Nickname, null when not available
		/// </summary>
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
	}
}
