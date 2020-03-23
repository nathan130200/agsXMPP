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

namespace AgsXMPP.Protocol.X.muc
{
	/*
    <x xmlns='http://jabber.org/protocol/muc#user'>
        <status code='100'/>
    </x>    
    */

	/// <summary>
	/// Summary description for MucUser.
	/// </summary>
	public class Status : Element
	{
		#region << Constructors >>
		public Status()
		{
			this.TagName = "status";
			this.Namespace = URI.MUC_USER;
		}

		public Status(StatusCode code) : this()
		{
			this.Code = code;
		}

		public Status(int code) : this()
		{
			this.SetAttribute("code", code);
		}
		#endregion

		public StatusCode Code
		{
			get
			{
				return (StatusCode)this.GetAttributeEnum("code", typeof(StatusCode));
			}
			set
			{
				this.SetAttribute("code", value.ToString());
			}
		}
	}

}