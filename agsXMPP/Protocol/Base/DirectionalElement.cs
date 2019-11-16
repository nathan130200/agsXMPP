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

using agsXMPP.Xml.Dom;

namespace agsXMPP.Protocol.@base
{
	/// <summary>
	/// Base XMPP Element
	/// This must ne used to build all other new packets
	/// </summary>
	public abstract class DirectionalElement : Element
	{
		public DirectionalElement()
			: base()
		{
		}

		public DirectionalElement(string tag)
			: base(tag)
		{
		}

		public DirectionalElement(string tag, string ns)
			: base(tag)
		{
			this.Namespace = ns;
		}

		public DirectionalElement(string tag, string text, string ns)
			: base(tag, text)
		{
			this.Namespace = ns;
		}

		public Jid From
		{
			get
			{
				if (this.HasAttribute("from"))
					return new Jid(this.GetAttribute("from"));
				else
					return null;
			}
			set
			{
				if (value != null)
					this.SetAttribute("from", value.ToString());
				else
					this.RemoveAttribute("from");
			}
		}

		public Jid To
		{
			get
			{
				if (this.HasAttribute("to"))
					return new Jid(this.GetAttribute("to"));
				else
					return null;
			}
			set
			{
				if (value != null)
					this.SetAttribute("to", value.ToString());
				else
					this.RemoveAttribute("to");
			}
		}

		/// <summary>
		/// Switches the from and to attributes when existing
		/// </summary>
		public void SwitchDirection()
		{
			var from = this.From;
			var to = this.To;

			// Remove from and to now
			this.RemoveAttribute("from");
			this.RemoveAttribute("to");

			Jid helper = null;

			helper = from;
			from = to;
			to = helper;

			this.From = from;
			this.To = to;
		}
	}
}