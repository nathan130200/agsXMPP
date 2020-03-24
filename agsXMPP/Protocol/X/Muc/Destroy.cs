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
     
        <iq from='crone1@shakespeare.lit/desktop'
            id='begone'
            to='heath@macbeth.shakespeare.lit'
            type='set'>
          <query xmlns='http://jabber.org/protocol/muc#owner'>
            <destroy jid='darkcave@macbeth.shakespeare.lit'>
              <reason>Macbeth doth come.</reason>
            </destroy>
          </query>
        </iq>
     
     */

	/// <summary>
	/// 
	/// </summary>
	public class Destroy : Element
	{
		#region << Constructor >>
		public Destroy()
		{
			this.TagName = "destroy";
			this.Namespace = URI.MUC_OWNER;
		}

		public Destroy(string reason) : this()
		{
			this.Reason = reason;
		}

		public Destroy(Jid altVenue) : this()
		{
			this.AlternateVenue = altVenue;
		}

		public Destroy(string reason, Jid altVenue) : this()
		{
			this.Reason = reason;
			this.AlternateVenue = altVenue;
		}
		#endregion


		/// <summary>
		/// Pptional attribute for a alternate venue
		/// </summary>
		public Jid AlternateVenue
		{
			get
			{
				if (this.HasAttribute("jid"))
					return new Jid(this.GetAttribute("jid"));
				else
					return null;
			}
			set
			{
				if (value != null)
					this.SetAttribute("jid", value.ToString());
			}
		}

		public string Reason
		{
			set { this.SetTag("reason", value); }
			get { return this.GetTag("reason"); }
		}

		public string Password
		{
			set { this.SetTag("password", value); }
			get { return this.GetTag("password"); }
		}

	}
}