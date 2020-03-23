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

namespace AgsXMPP.Protocol.Query.Oob
{


	//	<iq type="set" to="horatio@denmark" from="sailor@sea" id="i_oob_001">
	//		<query xmlns="jabber:iq:oob">
	//			<url>http://denmark/act4/letter-1.html</url>
	//			<desc>There's a letter for you sir.</desc>
	//		</query>
	// </iq>	

	/// <summary>
	/// Summary for Oob.
	/// </summary>
	public class Oob : Element
	{
		public Oob()
		{
			this.TagName = "query";
			this.Namespace = URI.IQ_OOB;
		}

		public string Url
		{
			set
			{
				this.SetTag("url", value);
			}
			get
			{
				return this.GetTag("url");
			}
		}

		public string Description
		{
			set
			{
				this.SetTag("desc", value);
			}
			get
			{
				return this.GetTag("desc");
			}

		}
	}
}
