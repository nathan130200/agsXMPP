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

namespace AgsXMPP.Protocol.Query.time
{
	/// <summary>
	/// Summary description for TimeIq.
	/// </summary>
	public class TimeIq : IQ
	{
		private Time m_Time = new Time();

		public TimeIq()
		{
			base.Query = this.m_Time;
			this.GenerateId();
		}

		public TimeIq(IQType type) : this()
		{
			this.Type = type;
		}

		public TimeIq(IQType type, Jid to) : this(type)
		{
			this.To = to;
		}

		public TimeIq(IQType type, Jid to, Jid from) : this(type, to)
		{
			this.From = from;
		}

		public new Time Query
		{
			get
			{
				return this.m_Time;
			}
		}
	}
}