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

namespace AgsXMPP.Protocol.Extensions.PubSub
{
	public abstract class PubSubAction : Element
	{
		public PubSubAction()
		{
			this.Namespace = URI.PUBSUB;
		}

		public string Node
		{
			get { return this.GetAttribute("node"); }
			set { this.SetAttribute("node", value); }
		}

		public Type Type
		{
			get => this.GetAttributeEnum<Type>("type");
			set
			{
				if (value == Type.None)
					this.RemoveAttribute("type");
				else
					this.SetAttributeEnum("type", value);
			}
		}
	}
}
