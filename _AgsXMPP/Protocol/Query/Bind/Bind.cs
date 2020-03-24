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

using System;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Query.Bind
{
	/// <summary>
	/// Summary description for Bind.
	/// </summary>
	public class Bind : Element
	{
		// SENT: <iq id="jcl_1" type="set">
		//			<bind xmlns="urn:ietf:params:xml:ns:xmpp-bind"><resource>Exodus</resource></bind>
		//		 </iq>
		// RECV: <iq id='jcl_1' type='result'>
		//			<bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'><jid>user@server.org/agsxmpp</jid></bind>
		//		 </iq>
		public Bind()
		{
			this.TagName = "bind";
			this.Namespace = URI.BIND;
		}

		public Bind(string resource) : this()
		{
			this.Resource = resource;
		}

		public Bind(Jid jid) : this()
		{
			this.Jid = jid;
		}

		/// <summary>
		/// The resource to bind
		/// </summary>
		public string Resource
		{
			get { return this.GetTag("resource"); }
			set { this.SetTag("resource", value); }
		}

		/// <summary>
		/// The jid the server created
		/// </summary>
		public Jid Jid
		{
			get { return this.GetTagJid("jid"); }
			set { this.SetTag("jid", value.ToString()); }
		}

		/// <summary>
		/// The jid from requested.
		/// <para>Backwards attribute compatibility.</para>
		/// </summary>
		[Obsolete]
		public Jid From
		{
			get
			{
				if (!this.HasAttribute("from"))
					return null;

				return this.GetAttributeJid("from");
			}
			set
			{
				if (value == null)
					this.RemoveAttribute("from");
				else
					this.SetAttribute("from", value);
			}
		}
	}
}
