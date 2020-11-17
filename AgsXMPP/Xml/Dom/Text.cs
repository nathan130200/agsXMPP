namespace AgsXMPP.Xml.Dom
{
	public class Text : Node
	{
		public Text(string value) : base(NodeType.Text)
		{
			this.Value = value;
		}
	}
}
