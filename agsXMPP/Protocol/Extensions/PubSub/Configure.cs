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

using AgsXMPP.Protocol.x.data;

namespace AgsXMPP.Protocol.Extensions.PubSub
{
	public class Configure : PubSubAction
	{
		#region << Constructors >>
		public Configure() : base()
		{
			this.TagName = "configure";
		}

		public Configure(string node) : this()
		{
			this.Node = node;
		}

		public Configure(Type type) : this()
		{
			this.Type = type;
		}

		public Configure(string node, Type type) : this(node)
		{
			this.Type = type;
		}
		#endregion

		public Access Access
		{
			get => this.GetAttributeEnum<Access>("access");
			set
			{
				if (value == Access.None)
					this.RemoveAttribute("access");
				else
					this.SetAttributeEnum("access", value);
			}
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
