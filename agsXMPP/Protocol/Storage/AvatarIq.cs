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

namespace agsXMPP.Protocol.storage
{

	//	Once such data has been set, the avatar can be retrieved by any requesting client from the avatar-generating client's public XML storage:
	//
	//	Example 8.
	//
	//	<iq id='1' type='get' to='user@server'>
	//		<query xmlns='storage:client:avatar'/>
	//	</iq>  

	/// <summary>
	/// Summary description for AvatarIq.
	/// </summary>
	public class AvatarIq : client.IQ
	{
		private Avatar m_Avatar = new Avatar();

		public AvatarIq()
		{
			base.Query = this.m_Avatar;
			this.GenerateId();
		}

		public AvatarIq(IQType type) : this()
		{
			this.Type = type;
		}

		public AvatarIq(IQType type, Jid to) : this(type)
		{
			this.To = to;
		}

		public AvatarIq(IQType type, Jid to, Jid from) : this(type, to)
		{
			this.From = from;
		}

		public new Avatar Query
		{
			get
			{
				return this.m_Avatar;
			}
		}
	}
}
