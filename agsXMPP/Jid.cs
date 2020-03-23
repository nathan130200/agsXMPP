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

using AgsXMPP.Collections;
using AgsXMPP.Idn;
#if STRINGPREP
using agsXMPP.Idn;
#endif

namespace AgsXMPP
{
	/// <summary>
	/// Class for building and handling XMPP Id's (JID's)
	/// </summary>  
	public class Jid : IComparable, ICloneable, IEquatable<Jid>
#if NET_2 || CF_2
        , IEquatable<Jid>
#endif
	{
		/*		
        14 possible invalid forms of JIDs and some variations on valid JIDs with invalid lengths, viz:

        jidforms = [
            "",
            "@",
            "@/resource",
            "@domain",
            "@domain/",
            "@domain/resource",
            "nodename@",
            "/",
            "nodename@domain/",
            "nodename@/",
            "@/",
            "nodename/",
            "/resource",
            "nodename@/resource",
        ]
        

        TODO
        Each allowable portion of a JID (node identifier, domain identifier, and resource identifier) MUST NOT
        be more than 1023 bytes in length, resulting in a maximum total size
        (including the '@' and '/' separators) of 3071 bytes.
            
        stringprep with libIDN        
        m_User      ==> nodeprep
        m_Server    ==> nameprep
        m_Resource  ==> resourceprep
        */

		// !!! 
		// use this internal variables only if you know what you are doing
		// !!!
		internal string m_Jid = null;
		internal string m_User = null;
		internal string m_Server = null;
		internal string m_Resource = null;

		public object Clone()
			=> new Jid(this.User, this.Server, this.Resource, false);

		public bool Equals(Jid other)
		{
			return this.Equals(other as object);
		}

		/// <summary>
		/// Is bare jid.
		/// <para>node@domain</para>
		/// </summary>
		public bool IsBare => string.IsNullOrEmpty(this.m_Resource);

		/// <summary>
		/// Is full jid.
		/// <para>node@domain/resource</para>
		/// </summary>
		public bool IsFull => !this.IsBare;

		/// <summary>
		/// Create a new JID object from a string. The input string must be a valid jabberId and already prepared with stringprep.
		/// Otherwise use one of the other constructors with escapes the node and prepares the gives balues with the stringprep
		/// profiles
		/// </summary>
		/// <param name="jid">XMPP ID, in string form examples: user@server/Resource, user@server</param>
		public Jid(string jid)
		{
			this.m_Jid = jid;
			this.Parse(jid);
		}

		public static bool IsFullEquals(Jid left, Jid right)
			=> FullJidComparer.Instance.Compare(left, right) == 0;

		public static bool IsBareEquals(Jid left, Jid right)
			=> BareJidComparer.Instance.Compare(left, right) == 0;

		/// <summary>
		/// builds a new Jid object
		/// </summary>
		/// <param name="user">XMPP User part</param>
		/// <param name="server">XMPP Domain part</param>
		/// <param name="resource">XMPP Resource part</param>        
		public Jid(string user, string server, string resource, bool stringprep = false)
		{
			if (!stringprep)
			{
				if (user != null)
				{
					user = EscapeNode(user);

					this.m_User = user.ToLower();
				}

				if (server != null)
					this.m_Server = server.ToLower();

				if (resource != null)
					this.m_Resource = resource;
			}
			else
			{
				if (user != null)
				{
					user = EscapeNode(user);

					this.m_User = Stringprep.NodePrep(user);
				}

				if (server != null)
					this.m_Server = Stringprep.NamePrep(server);

				if (resource != null)
					this.m_Resource = Stringprep.ResourcePrep(resource);
			}

			this.BuildJid();
		}

		/// <summary>
		/// Parses a JabberId from a string. If we parse a jid we assume it's correct and already prepared via stringprep.
		/// </summary>
		/// <param name="fullJid">jis to parse as string</param>
		/// <returns>true if the jid could be parsed, false if an error occured</returns>
		public bool Parse(string fullJid)
		{
			string user = null;
			string server = null;
			string resource = null;

			try
			{
				if (fullJid == null || fullJid.Length == 0)
				{
					return false;
				}

				this.m_Jid = fullJid;

				var atPos = this.m_Jid.IndexOf('@');
				var slashPos = this.m_Jid.IndexOf('/');

				// some more validations
				// @... or /...
				if (atPos == 0 || slashPos == 0)
					return false;

				// nodename@
				if (atPos + 1 == fullJid.Length)
					return false;

				// @/ at followed by resource separator
				if (atPos + 1 == slashPos)
					return false;

				if (atPos == -1)
				{
					user = null;
					if (slashPos == -1)
					{
						// JID Contains only the Server
						server = this.m_Jid;
					}
					else
					{
						// JID Contains only the Server and Resource
						server = this.m_Jid.Substring(0, slashPos);
						resource = this.m_Jid.Substring(slashPos + 1);
					}
				}
				else
				{
					if (slashPos == -1)
					{
						// We have no resource
						// Devide User and Server (user@server)
						server = this.m_Jid.Substring(atPos + 1);
						user = this.m_Jid.Substring(0, atPos);
					}
					else
					{
						// We have all
						user = this.m_Jid.Substring(0, atPos);
						server = this.m_Jid.Substring(atPos + 1, slashPos - atPos - 1);
						resource = this.m_Jid.Substring(slashPos + 1);
					}
				}

				if (user != null)
					this.m_User = user;
				if (server != null)
					this.m_Server = server;
				if (resource != null)
					this.m_Resource = resource;

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal void BuildJid()
		{
			this.m_Jid = this.BuildJid(this.m_User, this.m_Server, this.m_Resource);
		}

		private string BuildJid(string user, string server, string resource)
		{
			var sb = new StringBuilder();
			if (user != null)
			{
				sb.Append(user);
				sb.Append("@");
			}
			sb.Append(server);
			if (resource != null)
			{
				sb.Append("/");
				sb.Append(resource);
			}
			return sb.ToString();
		}

		public override string ToString()
		{
			return this.m_Jid;
		}

		/// <summary>
		/// the user part of the JabberId.
		/// </summary>
		public string User
		{
			get
			{
				return this.m_User;
			}
			set
			{
				// first Encode the user/node
				var tmpUser = EscapeNode(value);
#if !STRINGPREP
				if (value != null)
					this.m_User = tmpUser.ToLower();
				else
					this.m_User = null;
#else
                if (value != null)
                    m_User = Stringprep.NodePrep(tmpUser);
                else
                    m_User = null;
#endif
				this.BuildJid();
			}
		}

		/// <summary>
		/// Only Server
		/// </summary>
		public string Server
		{
			get
			{
				return this.m_Server;
			}
			set
			{
#if !STRINGPREP
				if (value != null)
					this.m_Server = value.ToLower();
				else
					this.m_Server = null;
#else
                if (value != null)
                    m_Server = Stringprep.NamePrep(value);
                else
                    m_Server = null;
#endif
				this.BuildJid();
			}
		}

		/// <summary>
		/// Only the Resource field.
		/// null for none
		/// </summary>        
		public string Resource
		{
			get
			{
				return this.m_Resource;
			}
			set
			{
#if !STRINGPREP
				if (value != null)
					this.m_Resource = value;
				else
					this.m_Resource = null;
#else
                if (value != null)
                    m_Resource = Stringprep.ResourcePrep(value);
                else
                    m_Resource = null;
#endif
				this.BuildJid();
			}
		}

		/// <summary>
		/// The Bare Jid only (user@server).
		/// </summary>		
		public string Bare
		{
			get
			{
				return this.BuildJid(this.m_User, this.m_Server, null);
			}
		}

		#region << Overrides >>
		/// <summary>
		/// This compares the full Jid by default
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return this.Equals(obj, new FullJidComparer());
		}

		public override int GetHashCode()
		{
			var hcode = 0;
			if (this.m_User != null)
				hcode ^= this.m_User.GetHashCode();

			if (this.m_Server != null)
				hcode ^= this.m_Server.GetHashCode();

			if (this.m_Resource != null)
				hcode ^= this.m_Resource.GetHashCode();

			return hcode;
		}
		#endregion

		public bool Equals(object other, System.Collections.IComparer comparer)
		{
			if (comparer.Compare(other, this) == 0)
				return true;
			else
				return false;
		}

		#region IComparable Members
		public int CompareTo(object obj)
		{
			if (obj is Jid)
			{
				var jid = obj as Jid;
				var comparer = new FullJidComparer();
				return comparer.Compare(obj, this);
			}
			throw new ArgumentException("object is not a Jid");
		}
		#endregion

#if NET_2 || CF_2
		#region IEquatable<Jid> Members
        public bool Equals(Jid other)
        {
            FullJidComparer comparer = new FullJidComparer();
            if (comparer.Compare(other, this) == 0)
                return true;
            else
                return false;            
        }
		#endregion
#endif

		#region << XEP-0106: JID Escaping >>
		/// <summary>
		/// <para>
		/// Escape a node according to XEP-0106
		/// </para>
		/// <para>
		/// <a href="http://www.xmpp.org/extensions/xep-0106.html">http://www.xmpp.org/extensions/xep-0106.html</a>
		/// </para>        
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static string EscapeNode(string node)
		{
			if (node == null)
				return null;

			var sb = new StringBuilder();
			for (var i = 0; i < node.Length; i++)
			{
				/*
                <space> \20
                " 	    \22
                & 	    \26
                ' 	    \27
                / 	    \2f
                : 	    \3a
                < 	    \3c
                > 	    \3e
                @ 	    \40
                \ 	    \5c
                */
				var c = node[i];
				switch (c)
				{
					case ' ': sb.Append(@"\20"); break;
					case '"': sb.Append(@"\22"); break;
					case '&': sb.Append(@"\26"); break;
					case '\'': sb.Append(@"\27"); break;
					case '/': sb.Append(@"\2f"); break;
					case ':': sb.Append(@"\3a"); break;
					case '<': sb.Append(@"\3c"); break;
					case '>': sb.Append(@"\3e"); break;
					case '@': sb.Append(@"\40"); break;
					case '\\': sb.Append(@"\5c"); break;
					default: sb.Append(c); break;
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// <para>
		/// unescape a node according to XEP-0106
		/// </para>
		/// <para>
		/// <a href="http://www.xmpp.org/extensions/xep-0106.html">http://www.xmpp.org/extensions/xep-0106.html</a>
		/// </para>        
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static string UnescapeNode(string node)
		{
			if (node == null)
				return null;

			var sb = new StringBuilder();
			for (var i = 0; i < node.Length; i++)
			{
				var c1 = node[i];
				if (c1 == '\\' && i + 2 < node.Length)
				{
					i += 1;
					var c2 = node[i];
					i += 1;
					var c3 = node[i];
					if (c2 == '2')
					{
						switch (c3)
						{
							case '0':
								sb.Append(' ');
								break;
							case '2':
								sb.Append('"');
								break;
							case '6':
								sb.Append('&');
								break;
							case '7':
								sb.Append('\'');
								break;
							case 'f':
								sb.Append('/');
								break;
						}
					}
					else if (c2 == '3')
					{
						switch (c3)
						{
							case 'a':
								sb.Append(':');
								break;
							case 'c':
								sb.Append('<');
								break;
							case 'e':
								sb.Append('>');
								break;
						}
					}
					else if (c2 == '4')
					{
						if (c3 == '0')
							sb.Append("@");
					}
					else if (c2 == '5')
					{
						if (c3 == 'c')
							sb.Append("\\");
					}
				}
				else
					sb.Append(c1);
			}
			return sb.ToString();
		}

		#endregion
	}
}