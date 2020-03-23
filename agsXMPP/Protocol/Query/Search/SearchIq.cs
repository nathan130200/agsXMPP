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

using AgsXMPP.Protocol.Client;

//	Example 1. Requesting Search Fields
//
//	<iq type='get'
//		from='romeo@montague.net/home'
//		to='characters.shakespeare.lit'
//		id='search1'
//		xml:lang='en'>
//		<query xmlns='jabber:iq:search'/>
//	</iq>

namespace AgsXMPP.Protocol.Query.search
{
	/// <summary>
	/// Summary description for SearchIq.
	/// </summary>
	public class SearchIq : IQ
	{
		private Search m_Search = new Search();

		public SearchIq()
		{
			base.Query = this.m_Search;
			this.GenerateId();
		}

		public SearchIq(IQType type) : this()
		{
			this.Type = type;
		}

		public SearchIq(IQType type, Jid to) : this(type)
		{
			this.To = to;
		}

		public SearchIq(IQType type, Jid to, Jid from) : this(type, to)
		{
			this.From = from;
		}

		public new Search Query
		{
			get
			{
				return this.m_Search;
			}
		}

	}
}
