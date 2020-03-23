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

using AgsXMPP.Protocol.Client;

namespace AgsXMPP.Protocol.Iq.@private
{
	/// <summary>
	/// Summary description for PrivateIq.
	/// </summary>
	public class PrivateIq : Client.IQ
	{
		Private m_Private = new Private();

		public PrivateIq()
		{
			base.Query = this.m_Private;
			this.GenerateId();
		}

		public PrivateIq(IQType type) : this()
		{
			this.Type = type;
		}

		public PrivateIq(IQType type, Jid to) : this(type)
		{
			this.To = to;
		}

		public PrivateIq(IQType type, Jid to, Jid from) : this(type, to)
		{
			this.From = from;
		}

		public new Private Query
		{
			get { return this.m_Private; }
		}
	}
}
