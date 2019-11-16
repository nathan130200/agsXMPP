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

namespace agsXMPP.Protocol.extensions.bytestreams
{
	/*
        <message 
            from='proxy.host3' 
            to='target@host2/bar' 
            id='initiate'>
            <udpsuccess xmlns='http://jabber.org/protocol/bytestreams' dstaddr='Value of Hash'/>
        </message>
    */
	public class UdpSuccess : Element
	{
		public UdpSuccess(string dstaddr)
		{
			this.TagName = "udpsuccess";
			this.Namespace = Namespaces.BYTESTREAMS;

			this.DestinationAddress = dstaddr;
		}

		public string DestinationAddress
		{
			get { return this.GetAttribute("dstaddr"); }
			set { this.SetAttribute("dstaddr", value); }
		}

	}
}
