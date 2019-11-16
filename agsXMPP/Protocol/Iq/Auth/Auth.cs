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

namespace agsXMPP.Protocol.iq.auth
{
	//	Send:<iq type='get' to='myjabber.net' id='MX_7'>
	//			<query xmlns='jabber:iq:auth'><username>gnauck</username></query>
	//		 </iq>
	//	Recv:<iq type="result" id="MX_7"><query xmlns="jabber:iq:auth"><username>gnauck</username><password/><digest/><resource/></query></iq> 
	//
	//	Send:<iq type='set' id='mx_login'><query xmlns='jabber:iq:auth'><username>gnauck</username><digest>27c05d464e3f908db3b2ca1729674bfddb28daf2</digest><resource>Office</resource></query></iq>
	//	Recv:<iq id="mx_login" type="result"/> 


	/// <summary>
	///
	/// </summary>
	public class Auth : Element
	{
		#region << Constructors >>
		public Auth()
		{
			this.TagName = "query";
			this.Namespace = Namespaces.IQ_AUTH;
		}
		#endregion

		#region << Properties >>
		public string Username
		{
			get { return this.GetTag("username"); }
			set { this.SetTag("username", value); }
		}

		public string Password
		{
			get { return this.GetTag("password"); }
			set { this.SetTag("password", value); }
		}

		public string Resource
		{
			get { return this.GetTag("resource"); }
			set { this.SetTag("resource", value); }
		}

		public string Digest
		{
			get { return this.GetTag("digest"); }
			set { this.SetTag("digest", value); }
		}
		#endregion

		#region << Public Methods >>
		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="StreamID"></param>
		public void SetAuthDigest(string username, string password, string StreamID)
		{
			// Jive Messenger has a problem when we dont remove the password Tag
			this.RemoveTag("password");
			this.Username = username;
			this.Digest = Util.Hash.Sha1Hash(StreamID + password);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		public void SetAuthPlain(string username, string password)
		{
			// remove digest Tag when existing
			this.RemoveTag("digest");
			this.Username = username;
			this.Password = password;
		}

		/// <summary>
		/// 
		/// </summary>
		public void SetAuth(string username, string password, string streamId)
		{
			if (this.HasTag("digest"))
				this.SetAuthDigest(username, password, streamId);
			else
				this.SetAuthPlain(username, password);
		}
		#endregion

	}
}