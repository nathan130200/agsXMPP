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

namespace AgsXMPP.Protocol.Extensions.Bookmarks
{
	/// <summary>
	/// 
	/// </summary>
	public class Storage : Element
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
		public Storage()
		{
			this.TagName = "storage";
			this.Namespace = Namespaces.STORAGE_BOOKMARKS;
		}

		/// <summary>
		/// Add a conference bookmark to the storage object
		/// </summary>
		/// <param name="conf"></param>
		/// <returns></returns>
		public Conference AddConference(Conference conf)
		{
			this.AddChild(conf);
			return conf;
		}

		/// <summary>
		/// Add a conference bookmark to the storage object
		/// </summary>
		/// <param name="jid"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public Conference AddConference(Jid jid, string name)
		{
			return this.AddConference(new Conference(jid, name));
		}

		/// <summary>
		/// Add a conference bookmark to the storage object
		/// </summary>
		/// <param name="jid"></param>
		/// <param name="name"></param>
		/// <param name="nickname"></param>
		/// <returns></returns>
		public Conference AddConference(Jid jid, string name, string nickname)
		{
			return this.AddConference(new Conference(jid, name, nickname));
		}

		/// <summary>
		/// Add a conference bookmark to the storage object
		/// </summary>
		/// <param name="jid"></param>
		/// <param name="name"></param>
		/// <param name="nickname"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public Conference AddConference(Jid jid, string name, string nickname, string password)
		{
			return this.AddConference(new Conference(jid, name, nickname, password));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jid"></param>
		/// <param name="name"></param>
		/// <param name="nickname"></param>
		/// <param name="password"></param>
		/// <param name="autojoin"></param>
		/// <returns></returns>
		public Conference AddConference(Jid jid, string name, string nickname, string password, bool autojoin)
		{
			return this.AddConference(new Conference(jid, name, nickname, password, autojoin));
		}

		/// <summary>
		/// add multiple conference bookmarks
		/// </summary>
		/// <param name="confs"></param>
		public void AddConferences(Conference[] confs)
		{
			foreach (var conf in confs)
			{
				this.AddConference(conf);
			}
		}

		/// <summary>
		/// get all conference booksmarks
		/// </summary>
		/// <returns></returns>
		public Conference[] GetConferences()
		{
			var nl = this.SelectElements(typeof(Conference));
			var items = new Conference[nl.Count];
			var i = 0;
			foreach (Element e in nl)
			{
				items[i] = (Conference)e;
				i++;
			}
			return items;
		}

		/// <summary>
		/// add a url bookmark
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public Url AddUrl(Url url)
		{
			this.AddChild(url);
			return url;
		}

		public Url AddUrl(string address, string name)
		{
			return this.AddUrl(new Url(address, name));
		}

		/// <summary>
		/// add multiple url bookmarks
		/// </summary>
		/// <param name="urls"></param>
		public void AddUrls(Url[] urls)
		{
			foreach (var url in urls)
			{
				this.AddUrl(url);
			}
		}

		/// <summary>
		/// Get all url bookmarks
		/// </summary>
		/// <returns></returns>
		public Url[] GetUrls()
		{
			var nl = this.SelectElements(typeof(Url));
			var items = new Url[nl.Count];
			var i = 0;
			foreach (Element e in nl)
			{
				items[i] = (Url)e;
				i++;
			}
			return items;
		}
	}
}
