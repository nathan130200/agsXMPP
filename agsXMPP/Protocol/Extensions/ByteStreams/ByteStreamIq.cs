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

using agsXMPP.Protocol.client;

namespace agsXMPP.Protocol.extensions.bytestreams
{
	/// <summary>
	/// a Bytestream IQ
	/// </summary>
	public class ByteStreamIq : client.IQ
	{
		private ByteStream m_ByteStream = new ByteStream();

		public ByteStreamIq()
		{
			base.Query = this.m_ByteStream;
			this.GenerateId();
		}

		public ByteStreamIq(IqType type) : this()
		{
			this.Type = type;
		}

		public ByteStreamIq(IqType type, Jid to)
			: this(type)
		{
			this.To = to;
		}

		public ByteStreamIq(IqType type, Jid to, Jid from)
			: this(type, to)
		{
			this.From = from;
		}

		public ByteStreamIq(IqType type, Jid to, Jid from, string Id)
			: this(type, to, from)
		{
			this.Id = Id;
		}

		public new ByteStream Query
		{
			get
			{
				return this.m_ByteStream;
			}
		}
	}
}
