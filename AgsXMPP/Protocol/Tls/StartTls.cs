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

namespace AgsXMPP.Protocol.Tls
{

	// Step 4: Client sends the STARTTLS command to server:
	// <starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>

	/// <summary>
	/// Summary description for starttls.
	/// </summary>
	public class StartTls : Element
	{
		public StartTls()
		{
			this.TagName = "starttls";
			this.Namespace = URI.TLS;
		}

		public bool Required
		{
			get
			{
				return this.HasTag("required");
			}
			set
			{
				if (value == false)
				{
					if (this.HasTag("required"))
						this.RemoveTag("required");
				}
				else
				{
					if (!this.HasTag("required"))
						this.SetTag("required");
				}
			}
		}
	}
}
