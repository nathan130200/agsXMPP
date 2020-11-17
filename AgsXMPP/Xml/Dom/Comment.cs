namespace AgsXMPP.Xml.Dom
{
	public class Comment : Node
	{
		public Comment() : base(NodeType.Comment)
		{

		}

		public Comment(string value) : this()
		{
			this.Value = value;
		}
	}

	public class Cdata : Node
	{
		public Cdata() : base(NodeType.Cdata)
		{

		}

		public Cdata(string value) : this()
		{
			this.Value = value;
		}
	}
}
