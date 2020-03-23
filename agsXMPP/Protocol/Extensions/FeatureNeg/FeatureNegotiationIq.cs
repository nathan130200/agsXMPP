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

namespace AgsXMPP.Protocol.Extensions.FeatureNeg
{
	/// <summary>
	/// JEP-0020: Feature Negotiation
	/// This JEP defines a A protocol that enables two Jabber entities to mutually negotiate feature options.
	/// </summary>
	public class FeatureNegotiationIq : Client.IQ
	{
		public FeatureNegotiation FeatureNeg { get; } = new FeatureNegotiation();

		public FeatureNegotiationIq()
		{
			this.AddChild(this.FeatureNeg);
			this.GenerateId();
		}

		public FeatureNegotiationIq(IQType type) : this()
		{
			this.Type = type;
		}
	}
}
