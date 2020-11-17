using AgsXMPP.Attributes;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Base
{
	[XmppFactory("iq", Namespaces.CLIENT)]
	[XmppFactory("iq", Namespaces.ACCEPT)]
	public class Iq : Stanza
	{
		public Iq() : base("iq")
		{

		}
	}

	public abstract class Stanza : DirectionalElement
	{
		public string Id
		{
			get => this.GetAttribute("id");
			set => this.SetAttribute("id", value);
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