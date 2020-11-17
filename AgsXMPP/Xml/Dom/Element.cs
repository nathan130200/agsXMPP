using System.Collections.Generic;
using System.Linq;

namespace AgsXMPP.Xml.Dom
{
	public class Element : Node
	{
		public string Prefix { get; set; }
		public string TagName { get; set; }
		public bool IsRootElement => this.Parent == null;
		protected Dictionary<string, string> RawAttributes;

		public Element(string tagname, string xmlns = default, string text = default) : base(NodeType.Element)
		{
			this.RawAttributes = new Dictionary<string, string>();
			this.TagName = tagname;
			this.Namespace = xmlns;
			this.Value = text;
		}

		public Element Root
		{
			get
			{
				Node node;

				for (node = this; node != null; node = node.Parent)
					;

				return (Element)node;
			}
		}

		public IReadOnlyDictionary<string, string> Attributes
		{
			get
			{
				KeyValuePair<string, string>[] temp;

				lock (this.RawAttributes)
					temp = this.RawAttributes.ToArray();

				return temp.ToDictionary(x => x.Key, x => x.Value);
			}
		}
	}
}
