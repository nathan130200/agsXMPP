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

namespace AgsXMPP.Protocol.Query.RPC
{

	/*         

        Example 1. A typical request

        <iq type='set' to='responder@company-a.com/jrpc-server' id='1'>
          <query xmlns='jabber:iq:rpc'>
            <methodCall>
              <methodName>examples.getStateName</methodName>
              <params>
                <param>
                  <value><i4>6</i4></value>
                </param>
              </params>
            </methodCall>
          </query>
        </iq>

        Example 2. A typical response

        <iq type='result' to='requester@company-b.com/jrpc-client' 
                    from='responder@company-a.com/jrpc-server' id='1'>
          <query xmlns='jabber:iq:rpc'>
            <methodResponse>
              <params>
                <param>
                  <value><string>Colorado</string></value>
                </param>
              </params>
            </methodResponse>
          </query>
        </iq>

    */

	/// <summary>
	/// JEP-0009: Jabber-RPC, transport RPC over Jabber/XMPP
	/// </summary>
	public class Rpc : Element
	{
		public Rpc()
		{
			this.TagName = "query";
			this.Namespace = URI.IQ_RPC;
		}


		/// <summary>
		/// 
		/// </summary>
		public MethodCall MethodCall
		{
			get { return (MethodCall)this.SelectSingleElement(typeof(MethodCall)); }
			set
			{
				this.RemoveTag(typeof(MethodCall));
				if (value != null)
					this.AddChild(value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public MethodResponse MethodResponse
		{
			get { return (MethodResponse)this.SelectSingleElement(typeof(MethodResponse)); }
			set
			{
				this.RemoveTag(typeof(MethodResponse));
				if (value != null)
					this.AddChild(value);
			}
		}

	}
}
