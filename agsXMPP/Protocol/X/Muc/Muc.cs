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

namespace AgsXMPP.Protocol.X.Muc
{
	/*
     
        <x xmlns='http://jabber.org/protocol/muc'>
            <password>secret</password>
        </x>
     
     */

	/// <summary>
	/// Summary description for MucUser.
	/// </summary>
	public class Muc : Element
	{
		#region << Constructor >>
		public Muc()
		{
			this.TagName = "x";
			this.Namespace = URI.MUC;
		}
		#endregion

		public string Password
		{
			set { this.SetTag("password", value); }
			get { return this.GetTag("password"); }
		}

		/// <summary>
		/// The History object
		/// </summary>
		public History History
		{
			get
			{
				return this.SelectSingleElement(typeof(History)) as History;
			}
			set
			{
				if (this.HasTag(typeof(History)))
					this.RemoveTag(typeof(History));

				if (value != null)
					this.AddChild(value);
			}
		}
	}
}