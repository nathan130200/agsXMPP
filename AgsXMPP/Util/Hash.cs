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

using System.Text;
using System;

#if !CF
using System.Security.Cryptography;
#endif

namespace AgsXMPP.Util
{
	/// <summary>
	/// Summary description for Hash.
	/// </summary>
	/// 

	public class Hash
	{

		#region << SHA1 Hash Desktop Framework and Mono >>		
#if !CF
		public static string Sha1Hash(string password)
		{

			byte[] hash;

			using (var sha1 = SHA1.Create())
				hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));

			return HexToString(hash);
		}

		public static byte[] Sha1HashBytes(string password)
		{
			using (var sha1 = SHA1.Create())
				return sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
		}
#endif

		/// <summary>
		/// Converts all bytes in the Array to a string representation.
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns>string representation</returns>
		public static string HexToString(byte[] buffer)
		{
#if CF || CF_2
			var sb = new StringBuilder();
			
			foreach (var b in buffer)
				sb.Append(b.ToString("x2"));

			return sb.ToString();
#else
			return BitConverter.ToString(buffer)
				.Replace("-", string.Empty);
#endif
		}

		#endregion


		#region << SHA1 Hash Compact Framework >>
#if CF
		

		/// <summary>
		/// return a SHA1 Hash on PPC and Smartphone
		/// </summary>
		/// <param name="pass"></param>
		/// <returns></returns>
		public static byte[] Sha1Hash(byte[] pass)
		{
			IntPtr hProv;
			bool retVal = WinCeApi.CryptAcquireContext( out hProv, null, null, (int) WinCeApi.SecurityProviderType.RSA_FULL, 0 );
			IntPtr hHash;
			retVal = WinCeApi.CryptCreateHash( hProv, (int) WinCeApi.SecurityProviderType.CALG_SHA1, IntPtr.Zero, 0, out hHash );
			
			byte [] publicKey = pass;
			int publicKeyLen = publicKey.Length;
			retVal = WinCeApi.CryptHashData( hHash, publicKey, publicKeyLen, 0 );
			int bufferLen = 20; //SHA1 size
			byte [] buffer = new byte[bufferLen];
			retVal = WinCeApi.CryptGetHashParam( hHash, (int) WinCeApi.SecurityProviderType.HP_HASHVAL, buffer, ref bufferLen, 0 );
			retVal = WinCeApi.CryptDestroyHash( hHash );
			retVal = WinCeApi.CryptReleaseContext( hProv, 0 );
			
			return buffer;
		}

		/// <summary>
		/// return a SHA1 Hash on PPC and Smartphone
		/// </summary>
		/// <param name="pass"></param>
		/// <returns></returns>
		public static string Sha1Hash(string pass)
		{
			return HexToString(Sha1Hash(System.Text.Encoding.ASCII.GetBytes(pass)));		
		}

		/// <summary>
		/// return a SHA1 Hash on PPC and Smartphone
		/// </summary>
		/// <param name="pass"></param>
		/// <returns></returns>
		public static byte[] Sha1HashBytes(string pass)
		{
			return Sha1Hash(System.Text.Encoding.UTF8.GetBytes(pass));
		}

		/// <summary>
		/// omputes the MD5 hash value for the specified byte array.		
		/// </summary>
		/// <param name="pass">The input for which to compute the hash code.</param>
		/// <returns>The computed hash code.</returns>
		public static byte[] MD5Hash(byte[] pass)
		{
			IntPtr hProv;
			bool retVal = WinCeApi.CryptAcquireContext( out hProv, null, null, (int) WinCeApi.SecurityProviderType.RSA_FULL, 0 );
			IntPtr hHash;
			retVal = WinCeApi.CryptCreateHash( hProv, (int) WinCeApi.SecurityProviderType.CALG_MD5, IntPtr.Zero, 0, out hHash );
			
			byte [] publicKey = pass;
			int publicKeyLen = publicKey.Length;
			retVal = WinCeApi.CryptHashData( hHash, publicKey, publicKeyLen, 0 );
			int bufferLen = 16; //SHA1 size
			byte [] buffer = new byte[bufferLen];
			retVal = WinCeApi.CryptGetHashParam( hHash, (int) WinCeApi.SecurityProviderType.HP_HASHVAL, buffer, ref bufferLen, 0 );
			retVal = WinCeApi.CryptDestroyHash( hHash );
			retVal = WinCeApi.CryptReleaseContext( hProv, 0 );

			return buffer;
		}

#endif
		#endregion

	}
}
