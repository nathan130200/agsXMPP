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

using agsXMPP.Xml.Dom;

namespace agsXMPP.Protocol.x.muc
{
	/*
        Example 29. User Requests Limit on Number of Messages in History

        <presence
            from='hag66@shakespeare.lit/pda'
            to='darkcave@macbeth.shakespeare.lit/thirdwitch'>
          <x xmlns='http://jabber.org/protocol/muc'>
            <history maxstanzas='20'/>
          </x>
        </presence>
              

        Example 30. User Requests History in Last 3 Minutes

        <presence
            from='hag66@shakespeare.lit/pda'
            to='darkcave@macbeth.shakespeare.lit/thirdwitch'>
          <x xmlns='http://jabber.org/protocol/muc'>
            <history seconds='180'/>
          </x>
        </presence>
              

        Example 31. User Requests All History Since the Beginning of the Unix Era

        <presence
            from='hag66@shakespeare.lit/pda'
            to='darkcave@macbeth.shakespeare.lit/thirdwitch'>
          <x xmlns='http://jabber.org/protocol/muc'>
            <history since='1970-01-01T00:00Z'/>
          </x>
        </presence>
    */

	/// <summary>
	/// This is used to get the history of a muc room
	/// </summary>
	public class History : Element
	{
		#region << Constructors >>
		/// <summary>
		/// Empty default constructor
		/// </summary>
		public History()
		{
			this.TagName = "history";
			this.Namespace = Namespaces.MUC;
		}

		/// <summary>
		/// get the history starting from a given date when available
		/// </summary>
		/// <param name="date"></param>
		public History(DateTime date) : this()
		{
			this.Since = date;
		}

		/// <summary>
		/// Specify the maximum nunber of messages to retrieve from the history
		/// </summary>
		/// <param name="max"></param>
		public History(int max) : this()
		{
			this.MaxStanzas = max;
		}
		#endregion


		/// <summary>
		/// request the last xxx seconds of history when available
		/// </summary>
		public int Seconds
		{
			get { return this.GetAttributeInt("seconds"); }
			set { this.SetAttribute("seconds", value); }
		}

		/// <summary>
		/// Request maximum stanzas of history when available
		/// </summary>
		public int MaxStanzas
		{
			get { return this.GetAttributeInt("maxstanzas"); }
			set { this.SetAttribute("maxstanzas", value); }
		}

		/// <summary>
		/// Request history from a given date when available
		/// </summary>
		public DateTime Since
		{
			get { return Util.Time.FromISO(this.GetAttribute("since")); }
			set { this.SetAttribute("since", Util.Time.ToISO(value)); }
		}

		/// <summary>
		/// Limit the total number of characters in the history to "X" 
		/// (where the character count is the characters of the complete XML stanzas, 
		/// not only their XML character data).
		/// </summary>
		public int MaxCharacters
		{
			get { return this.GetAttributeInt("maxchars"); }
			set { this.SetAttribute("maxchars", value); }
		}
	}

}


