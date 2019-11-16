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

using agsXMPP.Protocol.x.data;

using agsXMPP.Xml.Dom;

namespace agsXMPP.Protocol.extensions.pubsub.Owner
{
	public class Configure : Element
	{
		#region << Constructor >>
		public Configure()
		{
			this.TagName = "configure";
			this.Namespace = Namespaces.PUBSUB_OWNER;
		}

		public Configure(string node)
		{
			this.Node = node;
		}
		#endregion

		public string Node
		{
			get { return this.GetAttribute("node"); }
			set { this.SetAttribute("node", value); }
		}

		/// <summary>
		/// The x-Data Element
		/// </summary>
		public Data Data
		{
			get
			{
				return this.SelectSingleElement(typeof(Data)) as Data;
			}
			set
			{
				if (this.HasTag(typeof(Data)))
					this.RemoveTag(typeof(Data));

				if (value != null)
					this.AddChild(value);
			}
		}
	}
}
