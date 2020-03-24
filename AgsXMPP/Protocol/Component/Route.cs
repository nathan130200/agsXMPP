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

using AgsXMPP.Protocol.Base;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Component
{
	public enum RouteType
	{
		None = -1,

		[XmppEnumMember("error")]
		Error,

		[XmppEnumMember("auth")]
		Auth,

		[XmppEnumMember("session")]
		Session
	}

	/// <summary>
	/// 
	/// </summary>
	public class Route : Stanza
	{
		public Route()
		{
			this.TagName = "route";
			this.Namespace = URI.ACCEPT;
		}

		public Route(Element route) : this()
		{
			this.RouteElement = route;
		}

		public Route(Element route, Jid from, Jid to) : this()
		{
			this.RouteElement = route;
			this.From = from;
			this.To = to;
		}

		public Route(Element route, Jid from, Jid to, RouteType type) : this()
		{
			this.RouteElement = route;
			this.From = from;
			this.To = to;
			this.Type = type;
		}

		/// <summary>
		/// Gets or Sets the logtype
		/// </summary>
		public RouteType Type
		{
			get => this.GetAttributeEnum<RouteType>("type");
			set
			{
				if (value == RouteType.None)
					this.RemoveAttribute("type");
				else
					this.SetAttributeEnum("type", value);
			}
		}

		/// <summary>
		/// sets or gets the element to route
		/// </summary>
		public Element RouteElement
		{
			get { return this.FirstChild as Element; }
			set
			{
				if (this.HasChildElements)
					this.RemoveAllChildNodes();

				if (value != null)
					this.AddChild(value);
			}
		}
	}
}