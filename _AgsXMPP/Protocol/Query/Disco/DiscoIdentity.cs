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
	*/

	/// <summary>
	/// Summary description for DiscoIdentity.
	/// </summary>
	public class DiscoIdentity : Element
	{
		public DiscoIdentity()
		{
			this.TagName = "identity";
			this.Namespace = URI.DISCO_INFO;
		}

		public DiscoIdentity(string type, string name, string category) : this()
		{
			this.Type = type;
			this.Name = name;
			this.Category = category;
		}

		public DiscoIdentity(string type, string category) : this()
		{
			this.Type = type;
			this.Category = category;
		}

		/// <summary>
		/// type name for the entity
		/// </summary>
		public string Type
		{
			get { return this.GetAttribute("type"); }
			set { this.SetAttribute("type", value); }
		}

		/// <summary>
		/// natural-language name for the entity
		/// </summary>
		public string Name
		{
			get { return this.GetAttribute("name"); }
			set { this.SetAttribute("name", value); }
		}

		/// <summary>
		/// category name for the entity
		/// </summary>
		public string Category
		{
			get { return this.GetAttribute("category"); }
			set { this.SetAttribute("category", value); }
		}


	}
}
