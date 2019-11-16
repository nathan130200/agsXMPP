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

using agsXMPP.Xml.Dom;

namespace agsXMPP.Protocol.extensions.bookmarks
{
	/// <summary>
	/// One of the most common uses of bookmarks will likely be to bookmark 
	/// conference rooms on various Jabber servers
	/// </summary>
	public class Conference : Element
	{
		/*
             <iq type='result' id='2'>
               <query xmlns='jabber:iq:private'>
                 <storage xmlns='storage:bookmarks'>
                   <conference name='Council of Oberon' 
                               autojoin='true'
                               jid='council@conference.underhill.org'>
                     <nick>Puck</nick>
                     <password>titania</password>
                   </conference>
                 </storage>
               </query>
             </iq>   
         */
		public Conference()
		{
			this.TagName = "conference";
			this.Namespace = Namespaces.STORAGE_BOOKMARKS;
		}

		public Conference(Jid jid, string name) : this()
		{
			this.Jid = jid;
			this.Name = name;
		}

		public Conference(Jid jid, string name, string nickname) : this(jid, name)
		{
			this.Nickname = nickname;
		}

		public Conference(Jid jid, string name, string nickname, string password) : this(jid, name, nickname)
		{
			this.Password = password;
		}

		public Conference(Jid jid, string name, string nickname, string password, bool autojoin) : this(jid, name, nickname, password)
		{
			this.AutoJoin = autojoin;
		}

		/// <summary>
		/// A name/description for this bookmarked room
		/// </summary>
		public string Name
		{
			get { return this.GetAttribute("name"); }
			set { this.SetAttribute("name", value); }
		}

		/// <summary>
		/// Should the client join this room automatically after successfuil login?
		/// </summary>
		public bool AutoJoin
		{
			get { return this.GetAttributeBool("autojoin"); }
			set { this.SetAttribute("autojoin", value); }
		}

		/// <summary>
		/// The Jid of the bookmarked room
		/// </summary>
		public Jid Jid
		{
			get { return this.GetAttributeJid("jid"); }
			set { this.SetAttribute("jid", value); }
		}

		/// <summary>
		/// The Nickname for this room
		/// </summary>
		public string Nickname
		{
			get { return this.GetTag("nickname"); }
			set { this.SetTag("nickname", value); }
		}

		/// <summary>
		/// The password for password protected rooms
		/// </summary>
		public string Password
		{
			get { return this.GetTag("password"); }
			set { this.SetTag("password", value); }
		}
	}
}
