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

#region Using directives


#endregion

namespace AgsXMPP.Protocol.Component
{
	/// <summary>
	/// Summary description for Iq.
	/// </summary>
	public class IQ : Client.IQ
	{
		#region << Constructors >>
		public IQ() : base()
		{
			this.Namespace = URI.ACCEPT;
		}

		public IQ(Client.IQType type) : base(type)
		{
			this.Namespace = URI.ACCEPT;
		}

		public IQ(Jid from, Jid to) : base(from, to)
		{
			this.Namespace = URI.ACCEPT;
		}

		public IQ(Client.IQType type, Jid from, Jid to) : base(type, from, to)
		{
			this.Namespace = URI.ACCEPT;
		}
		#endregion

		/// <summary>
		/// Error Child Element
		/// </summary>
		public new Error Error
		{
			get
			{
				return this.SelectSingleElement(typeof(Error)) as Error;

			}
			set
			{
				if (this.HasTag(typeof(Error)))
					this.RemoveTag(typeof(Error));

				if (value != null)
					this.AddChild(value);
			}
		}
	}
}
