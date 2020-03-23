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

namespace AgsXMPP.Protocol.Iq.register
{
	public delegate void RegisterEventHandler(object sender, RegisterEventArgs args);

	public class RegisterEventArgs
	{
		public RegisterEventArgs()
		{
		}

		public RegisterEventArgs(Register reg)
		{
			this.m_Register = reg;
		}

		// by default we register automatically
		private bool m_Auto = true;
		private Register m_Register;

		/// <summary>
		/// Set Auto to true if the library should register automatically
		/// Set it to false if you want to fill out the registration fields manual
		/// </summary>
		public bool Auto
		{
			get { return this.m_Auto; }
			set { this.m_Auto = value; }
		}

		public Register Register
		{
			get { return this.m_Register; }
			set { this.m_Register = value; }
		}

	}
}