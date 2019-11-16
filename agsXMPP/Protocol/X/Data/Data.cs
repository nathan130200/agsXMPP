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

namespace agsXMPP.Protocol.x.data
{
	/// <summary>
	/// Summary for Data.
	/// </summary>
	public class Data : FieldContainer
	{
		/*
		The base syntax for the 'jabber:x:data' namespace is as follows (a formal description can be found in the XML Schema section below):
		
		<x xmlns='jabber:x:data'
		type='{form-type}'>
		<title/>
		<instructions/>
		<field var='field-name'
				type='{field-type}'
				label='description'>
			<desc/>
			<required/>
			<value>field-value</value>
			<option label='option-label'><value>option-value</value></option>
			<option label='option-label'><value>option-value</value></option>
		</field>
		</x>
		
		*/

		#region << Constructors >>
		public Data()
		{
			this.TagName = "x";
			this.Namespace = Namespaces.X_DATA;
		}

		public Data(XDataFormType type) : this()
		{
			this.Type = type;
		}
		#endregion

		#region << Properties >>
		public string Title
		{
			get { return this.GetTag("title"); }
			set { this.SetTag("title", value); }
		}

		public string Instructions
		{
			get { return this.GetTag("instructions"); }
			set { this.SetTag("instructions", value); }
		}

		/// <summary>
		/// Type of thie XDATA Form.
		/// </summary>
		public XDataFormType Type
		{
			get
			{
				return (XDataFormType)this.GetAttributeEnum("type", typeof(XDataFormType));
			}
			set { this.SetAttribute("type", value.ToString()); }
		}

		public Reported Reported
		{
			get { return this.SelectSingleElement(typeof(Reported)) as Reported; }
			set
			{
				this.RemoveTag(typeof(Reported));
				this.AddChild(value);
			}
		}

		#endregion

		#region << public Methods >>
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Item AddItem()
		{
			var i = new Item();
			this.AddChild(i);
			return i;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public Item AddItem(Item item)
		{
			this.AddChild(item);
			return item;
		}

		/// <summary>
		/// Gets a list of all form fields
		/// </summary>
		/// <returns></returns>
		public Item[] GetItems()
		{
			var nl = this.SelectElements(typeof(Item));
			var items = new Item[nl.Count];
			var i = 0;
			foreach (Element e in nl)
			{
				items[i] = (Item)e;
				i++;
			}
			return items;
		}
		#endregion
	}
}
