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

namespace AgsXMPP.Protocol.Base
{
	/// <summary>
	/// Summary description for Stream.
	/// </summary>
	public class Stream : Stanza
	{
		public Stream()
		{
			this.TagName = "stream";
		}

		/// <summary>
		/// The StreamID of the current JabberSession.
		/// Returns null when none available.
		/// </summary>
		public string StreamId
		{
			get
			{
				return this.GetAttribute("id");
			}
			set
			{
				this.SetAttribute("id", value);
			}
		}

		/// <summary>
		/// See XMPP-Core 4.4.1 "Version Support"
		/// </summary>
		public string Version
		{
			get
			{
				return this.GetAttribute("version");
			}
			set
			{
				this.SetAttribute("version", value);
			}
		}
	}
}
