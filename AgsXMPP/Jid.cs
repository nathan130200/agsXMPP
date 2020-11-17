using System;
using System.Text;

namespace AgsXMPP
{
	public class Jid
	{
		internal string Raw;
		internal string RawUser;
		internal string RawServer;
		internal string RawResource;

		public bool IsBare => string.IsNullOrEmpty(this.Resource);
		public bool IsFull => !this.IsBare;

		public Jid()
		{

		}

		public Jid(string jid)
		{
			this.ParseInternal(jid);
		}

		public static Jid Parse(string value)
		{
			var j = new Jid();

			if (!j.ParseInternal(value))
				throw new ArgumentException(nameof(value));

			return j;
		}

		public static bool TryParse(string value, out Jid jid)
		{
			jid = new Jid();
			return jid.ParseInternal(value);
		}

		internal bool ParseInternal(string value)
		{
			string user = null,
				   server = null,
				   resource = null;

			if (string.IsNullOrEmpty(value))
				return false;

			this.Raw = value;

			var span = this.Raw.AsSpan();

			var at_pos = span.IndexOf('@');
			var slash_pos = span.IndexOf('/');

			if (at_pos == 0 || slash_pos == 0)
				return false;

			if (at_pos + 1 == value.Length)
				return false;

			if (at_pos + 1 == slash_pos)
				return false;

			if (at_pos == -1)
			{
				if (slash_pos == -1)
					server = this.Raw;
				else
				{
					server = span.Slice(0, slash_pos).AsString();
					resource = span[(slash_pos + 1)..].AsString();
				}
			}
			else
			{
				if (slash_pos == -1)
				{
					user = span.Slice(0, at_pos).AsString();
					server = span[(at_pos + 1)..].AsString();
				}
				else
				{
					user = span.Slice(0, at_pos).AsString();
					server = span.Slice(at_pos + 1, slash_pos - at_pos - 1).AsString();
					resource = span[(slash_pos + 1)..].AsString();
				}
			}

			if (!string.IsNullOrEmpty(user))
				this.RawUser = user;

			if (!string.IsNullOrEmpty(server))
				this.RawServer = server;

			if (string.IsNullOrEmpty(resource))
				this.RawResource = resource;

			return true;
		}

		public string User
		{
			get => this.RawUser;
			set
			{
				var temp = EscapeNode(value);

				if (!string.IsNullOrEmpty(temp))
					this.RawUser = temp.ToLowerInvariant();
				else
					this.RawUser = null;

				this.BuildJid();
			}
		}

		public string Server
		{
			get => this.RawServer;
			set
			{
				var temp = EscapeNode(value);

				if (!string.IsNullOrEmpty(temp))
					this.RawServer = temp;
				else
					this.RawServer = null;

				this.BuildJid();
			}
		}

		public string Resource
		{
			get => this.RawResource;
			set
			{
				var temp = EscapeNode(value);

				if (!string.IsNullOrEmpty(temp))
					this.RawResource = temp;
				else
					this.RawResource = null;

				this.BuildJid();
			}
		}

		public string Bare
			=> BuildJid(this.RawUser, this.RawServer);

		public string Full
			=> BuildJid(this.RawUser, this.RawServer, this.RawResource);

		protected void BuildJid()
			=> this.Raw = BuildJid(this.RawUser, this.RawServer, this.RawResource);

		static string BuildJid(string user = null, string server = null, string resource = null)
		{
			var stbl = new StringBuilder();

			if (!string.IsNullOrEmpty(user))
				stbl.Append(user).Append('@');

			if (!string.IsNullOrEmpty(server))
				stbl.Append(server);

			if (!string.IsNullOrEmpty(resource))
				stbl.Append('/').Append(resource);

			return stbl.ToString();
		}

		public override int GetHashCode()
			=> HashCode.Combine(this.RawUser, this.RawServer, this.RawResource);

		public override string ToString()
			=> this.Raw;

		public static string EscapeNode(string node)
		{
			if (string.IsNullOrEmpty(node))
				return null;

			var stbl = new StringBuilder();

			for (var i = 0; i < node.Length; i++)
			{
				var ch = node[i];

				switch (ch)
				{
					case ' ': stbl.Append(@"\20"); break;
					case '"': stbl.Append(@"\22"); break;
					case '&': stbl.Append(@"\26"); break;
					case '\'': stbl.Append(@"\27"); break;
					case '/': stbl.Append(@"\2f"); break;
					case ':': stbl.Append(@"\3a"); break;
					case '<': stbl.Append(@"\3c"); break;
					case '>': stbl.Append(@"\3e"); break;
					case '@': stbl.Append(@"\40"); break;
					case '\\': stbl.Append(@"\5c"); break;
					default: stbl.Append(ch); break;
				}
			}

			return stbl.ToString();
		}

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
	}
}