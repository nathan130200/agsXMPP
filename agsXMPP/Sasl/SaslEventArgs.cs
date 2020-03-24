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

using AgsXMPP.Protocol.Sasl;

namespace AgsXMPP.Sasl
{
	public delegate void SaslEventHandler(object sender, SaslEventArgs args);

	public class SaslEventArgs
	{
		#region << Constructors >>
		public SaslEventArgs()
		{

		}

		public SaslEventArgs(Mechanisms mechanisms)
		{
			this.m_Mechanisms = mechanisms;
		}
		#endregion

		// by default the library chooses the auth method
		private bool m_Auto = true;
		private string m_Mechanism;
		private Mechanisms m_Mechanisms;

		/// <summary>
		/// Set Auto to true if the library should choose the mechanism
		/// Set it to false for choosing the authentication method yourself
		/// </summary>
		public bool Auto
		{
			get { return this.m_Auto; }
			set { this.m_Auto = value; }
		}

		/// <summary>
		/// SASL Mechanism for authentication as string
		/// </summary>
		public string Mechanism
		{
			get { return this.m_Mechanism; }
			set { this.m_Mechanism = value; }
		}

		public Mechanisms Mechanisms
		{
			get { return this.m_Mechanisms; }
			set { this.m_Mechanisms = value; }
		}
	}
}