using System.IO;
using System.Text;
using System.Xml;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP
{
	public static class Utilities
	{
		internal static string BuildXml(Node e, Formatting formatting)
		{
			if (e == null)
				return string.Empty;
			else
			{
				using var tw = new StringWriter();
				using var w = new XmlTextWriter(tw);
				w.Formatting = formatting;

				if (formatting == Formatting.Indented)
				{
					w.Indentation = 2;
					w.IndentChar = ' ';
				}

				WriteTree(e, w);

				return tw.ToString();
			}
		}

		internal static void WriteTree(Node e, XmlTextWriter tw, Node parent = default)
		{
			if (e.Type == NodeType.Document)
			{
				var doc = e as Document;
				string decl = string.Empty;

				if (doc.Version != null)
					decl += "version='" + doc.Version + "'";

				if (doc.Encoding != null)
				{
					if (decl != null)
						decl += " ";

					decl += "encoding='" + doc.Encoding + "'";
				}

				if (decl != null)
					tw.WriteProcessingInstruction("xml", decl);

				foreach (var n in e.ChildNodes)
				{
					WriteTree(n, tw, e);
				}
			}
			else if (e.Type == NodeType.Text)
				tw.WriteString(e.Value);
			else if (e.Type == NodeType.Comment)
				tw.WriteComment(e.Value);
			else if (e.Type == NodeType.Cdata)
				tw.WriteCData(e.Value);
			else if (e.Type == NodeType.Element)
			{
				var el = e as Element;

				if (string.IsNullOrEmpty(el.Prefix))
					tw.WriteStartElement(el.Name);
				else
					tw.WriteStartElement(el.Prefix + ":" + el.Name);

				if ((parent == null || parent.Namespace != el.Namespace) && !string.IsNullOrEmpty(el.Namespace))
				{
					if (el.Prefix == null)
						tw.WriteAttributeString("xmlns", el.Namespace);
					else
						tw.WriteAttributeString("xmlns:" + el.Prefix, el.Namespace);
				}

				foreach (var (key, value) in el.Attributes)
					tw.WriteAttributeString(key, value);

				if (el.HasChildNodes)
				{
					foreach (var n in el.ChildNodes)
						WriteTree(n, tw, e);
				}

				tw.WriteEndElement();
			}
		}

		public static byte[] GetBytes(this string s)
			=> Encoding.UTF8.GetBytes(s);
	}
}