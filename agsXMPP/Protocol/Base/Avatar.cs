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

using System;

using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Base
{
	// Avatar is in multiple Namespaces. So better to work with a Base class

	/// <summary>
	/// Summary description for Avatar.
	/// </summary>
	public class Avatar : Element
	{
		public Avatar()
		{
			this.TagName = "query";
		}

		public byte[] Data
		{
			get
			{
				if (this.HasTag("data"))
					return Convert.FromBase64String(this.GetTag("data"));
				else
					return null;
			}
			set
			{
				this.SetTag("data", Convert.ToBase64String(value, 0, value.Length));
			}
		}

		public string MimeType
		{
			get
			{
				var data = this.SelectSingleElement("data");
				if (data != null)
					return this.GetAttribute("mimetype");
				else
					return null;
			}
			set
			{
				var data = this.SelectSingleElement("data");
				if (data != null)
					this.SetAttribute("mimetype", value);
			}
		}
	}

}
