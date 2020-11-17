using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Base
{
	public abstract class Stanza : DirectionalElement
	{
		public string Id
		{
			get => this.GetAttributeRaw("id");
			set => this.SetAttributeRaw("id", value);
		}

		public Stanza(string name, string xmlns = null) : base(name, xmlns)
		{

		}
	}

	public abstract class DirectionalElement : Element
	{
		public DirectionalElement(string name, string xmlns = null) : base(name, xmlns)
		{

		}

		public Jid From { get; set; }
		public Jid To { get; set; }
	}
}