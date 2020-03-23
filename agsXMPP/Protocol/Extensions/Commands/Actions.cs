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

namespace AgsXMPP.Protocol.Extensions.Commands
{
	/*
      <xs:element name='actions'>
        <xs:complexType>
          <xs:sequence>
            <xs:element name='prev' type='empty' minOccurs='0'/>
            <xs:element name='next' type='empty' minOccurs='0'/>
            <xs:element name='complete' type='empty' minOccurs='0'/>
          </xs:sequence>
          <xs:attribute name='execute' use='optional'>
            <xs:simpleType>
              <xs:restriction base='xs:NCName'>
                <xs:enumeration value='complete'/>
                <xs:enumeration value='next'/>
                <xs:enumeration value='prev'/>
              </xs:restriction>
            </xs:simpleType>
          </xs:attribute>
        </xs:complexType>
      </xs:element>
     
      <actions execute='complete'>
        <prev/>
        <complete/>
      </actions>
    */
	public class Actions : Element
	{
		public Actions()
		{
			this.TagName = "actions";
			this.Namespace = Namespaces.COMMANDS;
		}

		/// <summary>
		/// Optional Execute Action, only complete, next and previous is allowed
		/// </summary>
		public Action Execute
		{
			get
			{
				return this.GetAttributeEnum<Action>("execute");
			}
			set
			{
				if (value == Action.None)
					this.RemoveAttribute("execute");
				else
					this.SetAttribute("execute", value.ToString());
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public bool Complete
		{
			get { return this.HasTag("complete"); }
			set
			{
				if (value)
					this.SetTag("complete");
				else
					this.RemoveTag("complete");
			}
		}

		public bool Next
		{
			get { return this.HasTag("next"); }
			set
			{
				if (value)
					this.SetTag("next");
				else
					this.RemoveTag("next");
			}
		}

		public bool Previous
		{
			get { return this.HasTag("prev"); }
			set
			{
				if (value)
					this.SetTag("prev");
				else
					this.RemoveTag("prev");
			}
		}

		/// <summary>
		/// Actions, only complete, prev and next are allowed here and can be combined
		/// </summary>
		public Action Action
		{
			get
			{
				Action res = 0;

				if (this.Complete)
					res |= Action.complete;
				if (this.Previous)
					res |= Action.prev;
				if (this.Next)
					res |= Action.next;

				if (res == 0)
					return Action.None;
				else
					return res;
			}
			set
			{
				if (value == Action.None)
				{
					this.Complete = false;
					this.Previous = false;
					this.Next = false;
				}
				else
				{
					this.Complete = ((value & Action.complete) == Action.complete);
					this.Previous = ((value & Action.prev) == Action.prev);
					this.Next = ((value & Action.next) == Action.next);
				}
			}
		}
	}
}