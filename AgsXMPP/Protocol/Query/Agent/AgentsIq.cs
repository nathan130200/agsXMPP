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

// Request Agents:
// <iq id='someid' to='myjabber.net' type='get'>
//		<query xmlns='jabber:iq:agents'/>
// </iq>

namespace AgsXMPP.Protocol.Query.Agent
{
	/// <summary>
	/// Summary description for AgentsIq.
	/// </summary>
	public class AgentsIq : IQ
	{
		private Agents m_Agents = new Agents();

		public AgentsIq()
		{
			base.Query = this.m_Agents;
			this.GenerateId();
		}

		public AgentsIq(IQType type) : this()
		{
			this.Type = type;
		}

		public AgentsIq(IQType type, Jid to) : this(type)
		{
			this.To = to;
		}

		public AgentsIq(IQType type, Jid to, Jid from) : this(type, to)
		{
			this.From = from;
		}

		public new Agents Query
		{
			get { return this.m_Agents; }
		}
	}
}
