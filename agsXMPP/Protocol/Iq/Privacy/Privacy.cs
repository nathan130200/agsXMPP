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

namespace agsXMPP.Protocol.iq.privacy
{
	public class Privacy : Element
	{
		public Privacy()
		{
			this.TagName = "query";
			this.Namespace = Namespaces.IQ_PRIVACY;
		}

		/// <summary>
		/// Add a provacy list
		/// </summary>
		/// <param name="list"></param>
		public void AddList(List list)
		{
			this.AddChild(list);
		}

		/// <summary>
		/// Get all Lists
		/// </summary>
		/// <returns>Array of all privacy lists</returns>
		public List[] GetList()
		{
			var el = this.SelectElements(typeof(List));
			var i = 0;
			var result = new List[el.Count];
			foreach (List list in el)
			{
				result[i] = list;
				i++;
			}
			return result;
		}

		/// <summary>
		/// The active list
		/// </summary>
		public Active Active
		{
			get
			{
				return this.SelectSingleElement(typeof(Active)) as Active;
			}
			set
			{
				if (this.HasTag(typeof(Active)))
					this.RemoveTag(typeof(Active));

				if (value != null)
					this.AddChild(value);
			}
		}

		/// <summary>
		/// The default list
		/// </summary>
		public Default Default
		{
			get
			{
				return this.SelectSingleElement(typeof(Default)) as Default;
			}
			set
			{
				if (this.HasTag(typeof(Default)))
					this.RemoveTag(typeof(Default));

				this.AddChild(value);
			}
		}
	}
}
