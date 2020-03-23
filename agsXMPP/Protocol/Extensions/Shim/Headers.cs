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

namespace AgsXMPP.Protocol.Extensions.Shim
{
	/// <summary>
	/// JEP-0131: Stanza Headers and Internet Metadata (SHIM)
	/// </summary>
	public class Headers : Element
	{
		// <headers xmlns='http://jabber.org/protocol/shim'>
		//	 <header name='In-Reply-To'>123456789@capulet.com</header>
		// <header name='Keywords'>shakespeare,&lt;xmpp/&gt;</header>
		// </headers>
		public Headers()
		{
			this.TagName = "headers";
			this.Namespace = Namespaces.SHIM;
		}

		/// <summary>
		/// Adds a new Header
		/// </summary>
		/// <returns></returns>
		public Header AddHeader()
		{
			var h = new Header();
			this.AddChild(h);
			return h;
		}

		/// <summary>
		/// Adds the given Header
		/// </summary>
		/// <param name="header"></param>
		/// <returns>returns the given Header</returns>
		public Header AddHeader(Header header)
		{
			this.AddChild(header);
			return header;
		}

		/// <summary>
		/// Adds a new Header
		/// </summary>
		/// <param name="name">header name</param>
		/// <param name="val">header value</param>
		/// <returns>returns the new added header</returns>
		public Header AddHeader(string name, string val)
		{
			var header = new Header(name, val);
			this.AddChild(header);
			return header;
		}

		public void SetHeader(string name, string val)
		{
			var header = this.GetHeader(name);
			if (header != null)
				header.Value = val;
			else
				this.AddHeader(name, val);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public Header GetHeader(string name)
		{
			return (Header)this.SelectSingleElement("header", "name", name);
		}

		public Header[] GetHeaders()
		{
			var nl = this.SelectElements("header");
			var headers = new Header[nl.Count];

			var i = 0;
			foreach (Element e in nl)
			{
				headers[i] = (Header)e;
				i++;
			}
			return headers;
		}
	}
}
