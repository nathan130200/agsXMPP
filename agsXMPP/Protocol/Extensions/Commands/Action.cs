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

namespace AgsXMPP.Protocol.Extensions.Commands
{
	/*
      <xs:attribute name='action' use='optional'>
        <xs:simpleType>
          <xs:restriction base='xs:NCName'>
            <xs:enumeration value='cancel'/>
            <xs:enumeration value='complete'/>
            <xs:enumeration value='execute'/>
            <xs:enumeration value='next'/>
            <xs:enumeration value='prev'/>
          </xs:restriction>
        </xs:simpleType>
      </xs:attribute>
    */
	public enum Action
	{
		None = -1,

		[XmppEnumMember("next")]
		Next = 1,

		[XmppEnumMember("prev")]
		Prev = 2,

		[XmppEnumMember("complete")]
		Complete = 4,

		[XmppEnumMember("execute")]
		Execute = 8,

		[XmppEnumMember("cancel")]
		Cancel = 16
	}
}
