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

using agsXMPP.Protocol.iq.bind;
using agsXMPP.Protocol.iq.session;
using agsXMPP.Xml.Dom;

namespace agsXMPP.Protocol.client
{
	// a i know that i shouldnt use keywords for Enums. But its much easier this way
	// because of enum.ToString() and enum.Parse() Members
	public enum IqType
	{
		get,
		set,
		result,
		error
	}

	/// <summary>
	/// Iq Stanza.
	/// </summary>
	public class Iq : @base.Stanza
	{
		#region << Constructors >>
		public Iq()
		{
			this.TagName = "iq";
			this.Namespace = Namespaces.CLIENT;
		}

		public Iq(IqType type) : this()
		{
			this.Type = type;
		}

		public Iq(Jid from, Jid to) : this()
		{
			this.From = from;
			this.To = to;
		}

		public Iq(IqType type, Jid from, Jid to) : this()
		{
			this.Type = type;
			this.From = from;
			this.To = to;
		}
		#endregion

		public IqType Type
		{
			set
			{
				this.SetAttribute("type", value.ToString());
			}
			get
			{
				return (IqType)this.GetAttributeEnum("type", typeof(IqType));
			}
		}

		/// <summary>
		/// The query Element. Value can also be null which removes the Query tag when existing
		/// </summary>
		public Element Query
		{
			get
			{
				return this.SelectSingleElement("query");
			}
			set
			{
				if (value != null)
					this.ReplaceChild(value);
				else
					this.RemoveTag("query");
			}
		}

		/// <summary>
		/// Error Child Element
		/// </summary>
		public Error Error
		{
			get
			{
				return this.SelectSingleElement(typeof(Error)) as Error;

			}
			set
			{
				if (this.HasTag(typeof(Error)))
					this.RemoveTag(typeof(Error));

				if (value != null)
					this.AddChild(value);
			}
		}

		/// <summary>
		/// Get or Set the Bind ELement if it is a BingIq
		/// </summary>
		public virtual Bind Bind
		{
			get
			{
				return this.SelectSingleElement(typeof(Bind)) as Bind;
			}
			set
			{
				this.RemoveTag(typeof(Bind));

				if (value != null)
					this.AddChild(value);
			}
		}


		/// <summary>
		/// Get or Set the Session Element if it is a SessionIq
		/// </summary>
		public virtual Session Session
		{
			get
			{
				return this.SelectSingleElement(typeof(Session)) as Session;
			}
			set
			{
				this.RemoveTag(typeof(Session));

				if (value != null)
					this.AddChild(value);
			}
		}
	}
}
