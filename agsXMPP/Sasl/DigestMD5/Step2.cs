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
using System.Text;

#if CF
using agsXMPP.util;
#endif
using System.Security.Cryptography;

namespace AgsXMPP.Sasl.DigestMD5
{
	/// <summary>
	/// Summary description for Step2.
	/// </summary>
	public class Step2 : Step1
	{
		public Step2()
		{

		}

		/// <summary>
		/// builds a step2 message reply to the given step1 message
		/// </summary>
		/// <param name="step1"></param>
		public Step2(Step1 step1, string username, string password, string server)
		{
			this.Nonce = step1.Nonce;

			// fixed for SASL n amessage servers (jabberd 1.x)
			if (this.SupportsAuth(step1.Qop))
				this.Qop = "auth";

			this.Realm = step1.Realm;
			this.Charset = step1.Charset;
			this.Algorithm = step1.Algorithm;

			this.Username = username;
			this.Password = password;
			this.Server = server;

			this.GenerateCnonce();
			this.GenerateNc();
			this.GenerateDigestUri();
			this.GenerateResponse();
		}

		/// <summary>
		/// Does the server support Auth?
		/// </summary>
		/// <param name="qop"></param>
		/// <returns></returns>
		private bool SupportsAuth(string qop)
		{
			var auth = qop.Split(',');
			// This overload was not available in the CF, so updated this to the following
			//bool ret = Array.IndexOf(auth, "auth") < 0 ? false : true;
			var ret = Array.IndexOf(auth, "auth", auth.GetLowerBound(0), auth.Length) < 0 ? false : true;
			return ret;
		}

		/// <summary>
		/// parses a message and returns the step2 object
		/// </summary>
		/// <param name="message"></param>
		public Step2(string _)
		{
			// TODO, important for server stuff
		}

		#region << Properties and member variables >>

		public string Cnonce { get; set; }

		public string Nc { get; set; }

		public string DigestUri { get; set; }

		public string Response { get; set; }

		public string Authzid { get; set; }
		#endregion


		public override string ToString()
		{
			return this.GenerateMessage();
		}


		private void GenerateCnonce()
		{
			// Lenght of the Session ID on bytes,
			// 32 bytes equaly 64 chars
			// 16^64 possibilites for the session IDs (4.294.967.296)
			// This should be unique enough
			var m_lenght = 32;

			var buf = new byte[m_lenght];

			using (var RNG = RandomNumberGenerator.Create())
				RNG.GetBytes(buf);

			this.Cnonce = Util.Hash.HexToString(buf).ToLower();

			//			m_Cnonce = "e163ceed6cfbf8c1559a9ff373b292c2f926b65719a67a67c69f7f034c50aba3";
		}

		private void GenerateNc()
		{
			var nc = 1;
			this.Nc = nc.ToString().PadLeft(8, '0');
		}

		private void GenerateDigestUri()
		{
			this.DigestUri = "xmpp/" + this.Server;
		}


		//	HEX( KD ( HEX(H(A1)),
		//	{
		//		nonce-value, ":" nc-value, ":",
		//		cnonce-value, ":", qop-value, ":", HEX(H(A2)) }))
		//
		//	If authzid is specified, then A1 is
		//
		//	A1 = { H( { username-value, ":", realm-value, ":", passwd } ),
		//	":", nonce-value, ":", cnonce-value, ":", authzid-value }
		//
		//	If authzid is not specified, then A1 is
		//
		//	A1 = { H( { username-value, ":", realm-value, ":", passwd } ),
		//	":", nonce-value, ":", cnonce-value }
		//
		//	where
		//
		//	passwd   = *OCTET
		public void GenerateResponse()
		{
			byte[] H1;
			byte[] H2;
			byte[] H3;
			//byte[] temp;
			string A1;
			string A2;
			string A3;
			string p1;
			string p2;

			var stbl = new StringBuilder();
			stbl.Append(this.Username);
			stbl.Append(":");
			stbl.Append(this.Realm);
			stbl.Append(":");
			stbl.Append(this.Password);

#if !CF
			using (var md5 = new MD5CryptoServiceProvider())
				H1 = md5.ComputeHash(Encoding.UTF8.GetBytes(stbl.ToString()));
#else
			//H1 = Encoding.Default.GetBytes(util.Hash.MD5Hash(sb.ToString()));
			H1 = util.Hash.MD5Hash(Encoding.UTF8.GetBytes(sb.ToString()));
#endif

			stbl.Remove(0, stbl.Length);
			stbl.Append(":");
			stbl.Append(this.Nonce);
			stbl.Append(":");
			stbl.Append(this.Cnonce);

			if (this.Authzid != null)
			{
				stbl.Append(":");
				stbl.Append(this.Authzid);
			}
			A1 = stbl.ToString();


			//			sb.Remove(0, sb.Length);			
			//			sb.Append(Encoding.Default.GetChars(H1));
			//			//sb.Append(Encoding.ASCII.GetChars(H1));
			//			
			//			sb.Append(A1);			
			var bA1 = Encoding.ASCII.GetBytes(A1);
			var bH1A1 = new byte[H1.Length + bA1.Length];

			//Array.Copy(H1, bH1A1, H1.Length);
			Array.Copy(H1, 0, bH1A1, 0, H1.Length);
			Array.Copy(bA1, 0, bH1A1, H1.Length, bA1.Length);
			using (var md5 = new MD5CryptoServiceProvider())
				H1 = md5.ComputeHash(bH1A1);

			stbl.Remove(0, stbl.Length);
			stbl.Append("AUTHENTICATE:");
			stbl.Append(this.DigestUri);
			if (this.Qop.CompareTo("auth") != 0)
			{
				stbl.Append(":00000000000000000000000000000000");
			}
			A2 = stbl.ToString();
			H2 = Encoding.ASCII.GetBytes(A2);

			using (var md5 = new MD5CryptoServiceProvider())
				H2 = md5.ComputeHash(H2);

			// create p1 and p2 as the hex representation of H1 and H2
			p1 = Util.Hash.HexToString(H1).ToLower();
			p2 = Util.Hash.HexToString(H2).ToLower();

			stbl.Remove(0, stbl.Length);
			stbl.Append(p1);
			stbl.Append(":");
			stbl.Append(this.Nonce);
			stbl.Append(":");
			stbl.Append(this.Nc);
			stbl.Append(":");
			stbl.Append(this.Cnonce);
			stbl.Append(":");
			stbl.Append(this.Qop);
			stbl.Append(":");
			stbl.Append(p2);

			A3 = stbl.ToString();

			using (var md5 = new MD5CryptoServiceProvider())
				H3 = md5.ComputeHash(Encoding.ASCII.GetBytes(A3));

			this.Response = Util.Hash.HexToString(H3).ToLower();
		}

		private string GenerateMessage()
		{
			var stbl = new StringBuilder();
			stbl.Append("username=");
			stbl.Append(this.AddQuotes(this.Username));
			stbl.Append(",");
			stbl.Append("realm=");
			stbl.Append(this.AddQuotes(this.Realm));
			stbl.Append(",");
			stbl.Append("nonce=");
			stbl.Append(this.AddQuotes(this.Nonce));
			stbl.Append(",");
			stbl.Append("cnonce=");
			stbl.Append(this.AddQuotes(this.Cnonce));
			stbl.Append(",");
			stbl.Append("nc=");
			stbl.Append(this.Nc);
			stbl.Append(",");
			stbl.Append("qop=");
			stbl.Append(this.Qop);
			stbl.Append(",");
			stbl.Append("digest-uri=");
			stbl.Append(this.AddQuotes(this.DigestUri));
			stbl.Append(",");
			stbl.Append("charset=");
			stbl.Append(this.Charset);
			stbl.Append(",");
			stbl.Append("response=");
			stbl.Append(this.Response);

			return stbl.ToString();
		}

		/// <summary>
		/// return the given string with quotes
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		private string AddQuotes(string s)
		{
			var quote = "\"";
			return quote + s + quote;
		}
	}
}
