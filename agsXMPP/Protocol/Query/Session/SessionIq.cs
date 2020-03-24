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

using AgsXMPP.Protocol.Client;

namespace AgsXMPP.Protocol.Query.Session
{
	/// <summary>
	/// Starting the session, this is done after resource binding
	/// </summary>
	public class SessionIq : IQ
	{
		/*
		SEND:	<iq xmlns="jabber:client" id="agsXMPP_2" type="set" to="jabber.ru">
					<session xmlns="urn:ietf:params:xml:ns:xmpp-session" />
				</iq>
		RECV:	<iq xmlns="jabber:client" from="jabber.ru" type="result" id="agsXMPP_2">
					<session xmlns="urn:ietf:params:xml:ns:xmpp-session" />
				</iq> 
		 */
		private Session m_Session = new Session();

		public SessionIq()
		{
			this.GenerateId();
			this.AddChild(this.m_Session);
		}

		public SessionIq(IQType type) : this()
		{
			this.Type = type;
		}

		public SessionIq(IQType type, Jid to) : this()
		{
			this.Type = type;
			this.To = to;
		}
	}
}
