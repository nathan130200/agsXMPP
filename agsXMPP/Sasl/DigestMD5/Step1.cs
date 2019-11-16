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

using System;

//encoded challenge to client: 
//
//<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
//cmVhbG09InNvbWVyZWFsbSIsbm9uY2U9Ik9BNk1HOXRFUUdtMmhoIixxb3A9ImF1dGgi
//LGNoYXJzZXQ9dXRmLTgsYWxnb3JpdGhtPW1kNS1zZXNzCg==
//</challenge>The decoded challenge is: 
//
//realm="somerealm",nonce="OA6MG9tEQGm2hh",qop="auth",charset=utf-8,algorithm=md5-sess


namespace agsXMPP.Sasl.DigestMD5
{
	/// <summary>
	/// Summary description for Step1.
	/// </summary>
	public class Step1 : DigestMD5Mechanism // Mechanism 
	{
		/// <summary>
		/// Exception occurs when we were unable to parse the challenge
		/// </summary>
		public class ChallengeParseException : Exception
		{
			public ChallengeParseException(string message) : base(message)
			{
			}
		}

		#region << Constructors >>
		public Step1()
		{

		}

		public Step1(string message)
		{
			this.m_Message = message;
			this.Parse(message);
		}
		#endregion

		#region << Properties >>
		private string m_Realm;
		private string m_Nonce;
		private string m_Qop;//			= "auth";		
		private string m_Charset = "utf-8";
		private string m_Algorithm;

		private string m_Rspauth = null;

		private string m_Message;

		public string Realm
		{
			get { return this.m_Realm; }
			set { this.m_Realm = value; }
		}

		public string Nonce
		{
			get { return this.m_Nonce; }
			set { this.m_Nonce = value; }
		}

		public string Qop
		{
			get { return this.m_Qop; }
			set { this.m_Qop = value; }
		}

		public string Charset
		{
			get { return this.m_Charset; }
			set { this.m_Charset = value; }
		}

		public string Algorithm
		{
			get { return this.m_Algorithm; }
			set { this.m_Algorithm = value; }
		}

		public string Rspauth
		{
			get { return this.m_Rspauth; }
			set { this.m_Rspauth = value; }
		}
		#endregion

		/*
            nonce="deqOGux/N6hDPtf9vkGMU5Vzae+zfrqpBIvh6LovbBM=",
            realm="amessage.de",
            qop="auth,auth-int,auth-conf",
            cipher="rc4-40,rc4-56,rc4,des,3des",
            maxbuf=1024,
            charset=utf-8,
            algorithm=md5-sess
        */
		private void Parse(string message)
		{
			try
			{
				var start = 0;
				var end = 0;
				while (start < message.Length)
				{
					var equalPos = message.IndexOf('=', start);
					if (equalPos > 0)
					{
						// look if the next char is a quote
						if (message.Substring(equalPos + 1, 1) == "\"")
						{
							// quoted value, find the end now
							end = message.IndexOf('"', equalPos + 2);
							this.ParsePair(message.Substring(start, end - start + 1));
							start = end + 2;
						}
						else
						{
							// value is not quoted, ends at the next comma or end of string   
							end = message.IndexOf(',', equalPos + 1);
							if (end == -1)
								end = message.Length;

							this.ParsePair(message.Substring(start, end - start));

							start = end + 1;
						}
					}
				}
			}
			catch
			{
				throw new ChallengeParseException("Unable to parse challenge");
			}
		}

		private void ParsePair(string pair)
		{
			var equalPos = pair.IndexOf("=");
			if (equalPos > 0)
			{
				var key = pair.Substring(0, equalPos);
				string data;
				// is the value quoted?
				if (pair.Substring(equalPos + 1, 1) == "\"")
					data = pair.Substring(equalPos + 2, pair.Length - equalPos - 3);
				else
					data = pair.Substring(equalPos + 1);

				switch (key)
				{
					case "realm":
						this.m_Realm = data;
						break;
					case "nonce":
						this.m_Nonce = data;
						break;
					case "qop":
						this.m_Qop = data;
						break;
					case "charset":
						this.m_Charset = data;
						break;
					case "algorithm":
						this.m_Algorithm = data;
						break;
					case "rspauth":
						this.m_Rspauth = data;
						break;
				}
			}
		}
	}
}
