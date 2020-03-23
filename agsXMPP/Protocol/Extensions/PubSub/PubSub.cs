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

namespace AgsXMPP.Protocol.Extensions.PubSub
{
	public class PubSub : Element
	{
		public PubSub()
		{
			this.TagName = "pubsub";
			this.Namespace = URI.PUBSUB;
		}

		/// <summary>
		/// the Create Element of the Pubsub Element 
		/// </summary>
		public Create Create
		{
			get
			{
				return this.SelectSingleElement(typeof(Create)) as Create;
			}
			set
			{
				if (this.HasTag(typeof(Create)))
					this.RemoveTag(typeof(Create));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Publish Publish
		{
			get
			{
				return this.SelectSingleElement(typeof(Publish)) as Publish;

			}
			set
			{
				if (this.HasTag(typeof(Publish)))
					this.RemoveTag(typeof(Publish));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Retract Retract
		{
			get
			{
				return this.SelectSingleElement(typeof(Retract)) as Retract;

			}
			set
			{
				if (this.HasTag(typeof(Retract)))
					this.RemoveTag(typeof(Retract));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Subscribe Subscribe
		{
			get
			{
				return this.SelectSingleElement(typeof(Subscribe)) as Subscribe;

			}
			set
			{
				if (this.HasTag(typeof(Subscribe)))
					this.RemoveTag(typeof(Subscribe));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Unsubscribe Unsubscribe
		{
			get
			{
				return this.SelectSingleElement(typeof(Unsubscribe)) as Unsubscribe;

			}
			set
			{
				if (this.HasTag(typeof(Unsubscribe)))
					this.RemoveTag(typeof(Unsubscribe));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Subscriptions Subscriptions
		{
			get
			{
				return this.SelectSingleElement(typeof(Subscriptions)) as Subscriptions;

			}
			set
			{
				if (this.HasTag(typeof(Subscriptions)))
					this.RemoveTag(typeof(Subscriptions));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Affiliations Affiliations
		{
			get
			{
				return this.SelectSingleElement(typeof(Affiliations)) as Affiliations;

			}
			set
			{
				if (this.HasTag(typeof(Affiliations)))
					this.RemoveTag(typeof(Affiliations));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Options Options
		{
			get
			{
				return this.SelectSingleElement(typeof(Options)) as Options;

			}
			set
			{
				if (this.HasTag(typeof(Options)))
					this.RemoveTag(typeof(Options));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Items Items
		{
			get
			{
				return this.SelectSingleElement(typeof(Items)) as Items;

			}
			set
			{
				if (this.HasTag(typeof(Items)))
					this.RemoveTag(typeof(Items));

				if (value != null)
					this.AddChild(value);
			}
		}

		/// <summary>
		/// The Configure Element of the PunSub Element
		/// </summary>
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
