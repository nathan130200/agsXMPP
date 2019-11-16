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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace agsXMPP.Xml.Dom
{
	/// <summary>
	/// 
	/// </summary>	
	public class NodeList : CollectionBase
	{
		/// <summary>
		/// Owner (Parent) of the ChildElement Collection
		/// </summary>
		private Node m_Owner = null;
		private readonly object m_Locker = new object();

		internal NodeList()
		{
		}

		public NodeList(Node owner)
		{
			this.m_Owner = owner;
		}

		public void Add(Node e)
		{
			lock (this.m_Locker)
			{
				// can't add a empty node, so return immediately
				// Some people tried this which caused an error
				if (e == null)
					return;

				if (this.m_Owner != null)
				{
					e.Parent = this.m_Owner;
					if (e.Namespace == null)
						e.Namespace = this.m_Owner.Namespace;
				}

				e.m_Index = this.Count;
				this.List.Add(e);
			}
		}

		// Method implementation from the CollectionBase class
		public void Remove(int index)
		{
			lock (this.m_Locker)
			{
				if (index > this.Count - 1 || index < 0)
					return;

				this.List.RemoveAt(index);
				this.RebuildIndex(index);
			}
		}

		public void Remove(Element e)
		{
			lock (this.m_Locker)
			{
				var offset = e.Index;
				this.List.Remove(e);
				this.RebuildIndex(offset);
			}
		}

		public IEnumerable<Node> GetNodes()
		{
			return this.ToArray()
				.Cast<Node>();
		}

		public Node Item(int index)
		{
			lock (this.m_Locker)
				return (Node)this.List[index];
		}

		public object[] ToArray()
		{
			lock (this.m_Locker)
			{
				var temp = new object[this.List.Count];

				for (var i = 0; i < this.List.Count; i++)
					temp[i] = this.List[i];

				return temp;
			}
		}

		internal void RebuildIndex(int start = 0)
		{
			lock (this.m_Locker)
			{
				for (var i = start; i < this.Count; i++)
				{
					//Element e = (Element) List[i];
					var node = (Node)this.List[i];
					node.m_Index = i;
				}
			}
		}
	}
}