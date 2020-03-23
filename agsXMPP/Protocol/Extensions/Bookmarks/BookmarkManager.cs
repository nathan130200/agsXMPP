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
using AgsXMPP.Protocol.Client;

namespace AgsXMPP.Protocol.Extensions.Bookmarks
{
	public class BookmarkManager
	{
		private XmppClientConnection m_connection = null;


		public BookmarkManager(XmppClientConnection con)
		{
			this.m_connection = con;
		}

		#region << Request Bookmarks >>
		/// <summary>
		/// Request the bookmarks from the storage on the server
		/// </summary>
		public void RequestBookmarks()
		{
			this.RequestBookmarks(null, null);
		}

		/// <summary>
		/// Request the bookmarks from the storage on the server
		/// </summary>
		/// <param name="cb"></param>
		public void RequestBookmarks(IqCB cb)
		{
			this.RequestBookmarks(cb, null);
		}

		/// <summary>
		/// Request the bookmarks from the storage on the server
		/// </summary>
		/// <param name="cb"></param>
		/// <param name="cbArgs"></param>
		public void RequestBookmarks(IqCB cb, object cbArgs)
		{
			var siq = new StorageIq(IQType.Get);

			if (cb == null)
				this.m_connection.Send(siq);
			else
				this.m_connection.IqGrabber.SendIq(siq, cb, cbArgs);
		}
		#endregion


		#region << Store Bookmarks >>
		/// <summary>
		/// Send booksmarks to the server storage
		/// </summary>
		/// <param name="urls"></param>
		public void StoreBookmarks(Url[] urls)
		{
			this.StoreBookmarks(urls, null, null, null);
		}

		/// <summary>
		/// Send booksmarks to the server storage
		/// </summary>
		/// <param name="urls"></param>
		/// <param name="cb"></param>
		public void StoreBookmarks(Url[] urls, IqCB cb)
		{
			this.StoreBookmarks(urls, null, cb, null);
		}

		/// <summary>
		/// Send booksmarks to the server storage
		/// </summary>
		/// <param name="urls"></param>
		/// <param name="cb"></param>
		/// <param name="cbArgs"></param>
		public void StoreBookmarks(Url[] urls, IqCB cb, object cbArgs)
		{
			this.StoreBookmarks(urls, null, cb, cbArgs);
		}

		/// <summary>
		/// Send booksmarks to the server storage
		/// </summary>
		/// <param name="conferences"></param>
		public void StoreBookmarks(Conference[] conferences)
		{
			this.StoreBookmarks(null, conferences, null, null);
		}

		/// <summary>
		/// Send booksmarks to the server storage
		/// </summary>
		/// <param name="conferences"></param>
		/// <param name="cb"></param>
		public void StoreBookmarks(Conference[] conferences, IqCB cb)
		{
			this.StoreBookmarks(null, conferences, cb, null);
		}

		/// <summary>
		/// Send booksmarks to the server storage
		/// </summary>
		/// <param name="conferences"></param>
		/// <param name="cb"></param>
		/// <param name="cbArgs"></param>
		public void StoreBookmarks(Conference[] conferences, IqCB cb, object cbArgs)
		{
			this.StoreBookmarks(null, conferences, cb, cbArgs);
		}

		/// <summary>
		/// Send booksmarks to the server storage
		/// </summary>
		/// <param name="urls"></param>
		/// <param name="conferences"></param>
		public void StoreBookmarks(Url[] urls, Conference[] conferences)
		{
			this.StoreBookmarks(urls, conferences, null, null);
		}

		/// <summary>
		/// Send booksmarks to the server storage
		/// </summary>
		/// <param name="urls"></param>
		/// <param name="conferences"></param>
		/// <param name="cb"></param>
		public void StoreBookmarks(Url[] urls, Conference[] conferences, IqCB cb)
		{
			this.StoreBookmarks(urls, conferences, cb, null);
		}

		/// <summary>
		/// Send booksmarks to the server storage
		/// </summary>
		/// <param name="urls"></param>
		/// <param name="conferences"></param>
		/// <param name="cb"></param>
		/// <param name="cbArgs"></param>
		public void StoreBookmarks(Url[] urls, Conference[] conferences, IqCB cb, object cbArgs)
		{
			var siq = new StorageIq(IQType.Set);

			if (urls != null)
				siq.Query.Storage.AddUrls(urls);

			if (conferences != null)
				siq.Query.Storage.AddConferences(conferences);

			if (cb == null)
				this.m_connection.Send(siq);
			else
				this.m_connection.IqGrabber.SendIq(siq, cb, cbArgs);
		}
		#endregion
	}
}
