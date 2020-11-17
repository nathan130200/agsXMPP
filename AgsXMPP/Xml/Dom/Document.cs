using System;
using System.IO;

namespace AgsXMPP.Xml.Dom
{
	public class Document : Node
	{
		public string Encoding { get; set; }
		public string Version { get; set; }

		public Document() : base(NodeType.Document)
		{
			// TODO: Make document ctor.
		}

		public class Loader
		{
			protected Document Document;
			protected StreamParser Parser;
			protected byte[] Buffer;

			Loader(Document document)
			{
				this.Parser = new StreamParser();
				this.Document = document;
			}

			public Loader(Document document, string xml) : this(document)
			{
				this.Buffer = xml.GetBytes();
			}

			public Loader(Document document, Stream stream) : this(document)
			{
				var buffer = new byte[stream.Length];
				var count = stream.Read(buffer, 0, buffer.Length);
				Array.Copy(buffer, 0, this.Buffer, 0, count);
			}

			public Loader(Document document, StreamReader stream) : this(document, stream.ReadToEnd())
			{

			}
		}
	}
}
