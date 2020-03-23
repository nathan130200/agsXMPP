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

using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Extensions.pubsub.Owner
{
	public class PubSub : Element
	{
		public PubSub()
		{
			this.TagName = "pubsub";
			this.Namespace = Namespaces.PUBSUB_OWNER;
		}

		public Delete Delete
		{
			get
			{
				return this.SelectSingleElement(typeof(Delete)) as Delete;

			}
			set
			{
				if (this.HasTag(typeof(Delete)))
					this.RemoveTag(typeof(Delete));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Purge Purge
		{
			get
			{
				return this.SelectSingleElement(typeof(Purge)) as Purge;

			}
			set
			{
				if (this.HasTag(typeof(Purge)))
					this.RemoveTag(typeof(Purge));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Subscribers Subscribers
		{
			get
			{
				return this.SelectSingleElement(typeof(Subscribers)) as Subscribers;

			}
			set
			{
				if (this.HasTag(typeof(Subscribers)))
					this.RemoveTag(typeof(Subscribers));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Affiliates Affiliates
		{
			get
			{
				return this.SelectSingleElement(typeof(Affiliates)) as Affiliates;

			}
			set
			{
				if (this.HasTag(typeof(Affiliates)))
					this.RemoveTag(typeof(Affiliates));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Configure Configure
		{
			get
			{
				return this.SelectSingleElement(typeof(Configure)) as Configure;

			}
			set
			{
				if (this.HasTag(typeof(Configure)))
					this.RemoveTag(typeof(Configure));

				if (value != null)
					this.AddChild(value);
			}
		}

	}
}