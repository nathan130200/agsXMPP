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

namespace AgsXMPP.Protocol.Extensions.ping
{
	/// <summary>
	/// 
	/// </summary>
	public class PingIq : Client.IQ
	{
		private Ping m_Ping = new Ping();

		#region << Constructors >>
		public PingIq()
		{
			base.Query = this.m_Ping;
			this.GenerateId();
		}

		public PingIq(Jid to) : this()
		{
			this.To = to;
		}

		public PingIq(Jid to, Jid from) : this()
		{
			this.To = to;
			this.From = from;
		}
		#endregion


		public new Ping Query
		{
			get { return this.m_Ping; }
		}
	}
}
