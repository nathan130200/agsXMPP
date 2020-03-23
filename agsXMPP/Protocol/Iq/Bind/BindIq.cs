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

namespace AgsXMPP.Protocol.Iq.bind
{
	/// <summary>
	/// Summary description for BindIq.
	/// </summary>
	public class BindIq : Client.IQ
	{
		private Bind m_Bind = new Bind();

		public BindIq()
		{
			this.GenerateId();
			this.AddChild(this.m_Bind);
		}

		public BindIq(IQType type) : this()
		{
			this.Type = type;
		}

		public BindIq(IQType type, Jid to) : this()
		{
			this.Type = type;
			this.To = to;
		}

		public BindIq(IQType type, Jid to, string resource) : this(type, to)
		{
			this.m_Bind.Resource = resource;
		}

		public new Bind Query
		{
			get
			{
				return this.m_Bind;
			}
		}
	}
}
