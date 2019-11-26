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

using agsXMPP.Xml.Dom;

namespace agsXMPP.Sasl
{
	/// <summary>
	/// Summary description for Mechanism.
	/// </summary>
	public abstract class Mechanism
	{
		/// <summary>
		/// 
		/// </summary>
		public Mechanism()
		{
		}

		#region << Properties and member variables >>
		private XmppClientConnection m_XmppClientConnection;
		private string m_Username;
		private string m_Password;
		private string m_Server;

		public XmppClientConnection XmppClientConnection
		{
			get { return this.m_XmppClientConnection; }
			set { this.m_XmppClientConnection = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Username
		{
			// lower case that until i implement our c# port of libIDN
			get { return this.m_Username; }
			set { this.m_Username = value?.ToLower(); }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Password
		{
			get { return this.m_Password; }
			set { this.m_Password = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Server
		{
			get { return this.m_Server; }
			set { this.m_Server = value.ToLower(); }
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="con"></param>
		public abstract void Init(XmppClientConnection con);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public abstract void Parse(Node e);
	}
}
