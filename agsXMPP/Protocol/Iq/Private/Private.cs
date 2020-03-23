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

using AgsXMPP.Protocol.Extensions.Bookmarks;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Iq.@private
{
	/// <summary>
	/// Private XML Storage JEP-0049
	/// </summary>
	/// <remarks>
	/// A Jabber client can store any arbitrary XML on the server side by sending an
	/// iq stanza of type "set" to the server with a query child scoped by the 'jabber:iq:private' namespace.
	/// The query element MAY contain any arbitrary XML fragment as long as the root element of that 
	/// fragment is scoped by its own namespace. The data can then be retrieved by sending an iq stanza 
	/// of type "get" with a query child scoped by the 'jabber:iq:private' namespace, 
	/// which in turn contains a child element scoped by the namespace used for storage of that fragment.
	/// Using this method, Jabber entities can store private data on the server and retrieve it 
	/// whenever necessary. The data stored might be anything, as long as it is valid XML.
	/// One typical usage for this namespace is the server-side storage of client-specific preferences; 
	/// another is Bookmark Storage.
	/// </remarks>
	public class Private : Element
	{
		public Private()
		{
			this.TagName = "query";
			this.Namespace = URI.IQ_PRIVATE;
		}

		/// <summary>
		/// The <see cref="Extensions.Bookmarks.Storage">Storage</see> object 
		/// </summary>
		public Storage Storage
		{
			get
			{
				return this.SelectSingleElement(typeof(Extensions.Bookmarks.Storage)) as Extensions.Bookmarks.Storage;
			}
			set
			{
				if (this.HasTag(typeof(Extensions.Bookmarks.Storage)))
					this.RemoveTag(typeof(Extensions.Bookmarks.Storage));

				if (value != null)
					this.AddChild(value);
			}
		}
	}
}