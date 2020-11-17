using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AgsXMPP.Factory;

namespace AgsXMPP.Xml.Dom
{
	public abstract class Node
	{
		static Node()
		{
			ElementFactory.RegisterElementTypesFromCurrentAssembly();
		}

		public Node Parent { get; private set; }
		public string Value { get; set; }
		public string Namespace { get; set; }
		public NodeType Type { get; }

		protected List<Node> RawChildNodes;

		internal Node(NodeType type)
		{
			this.RawChildNodes = new List<Node>();
			this.Type = type;
		}

		public bool HasChildNodes
		{
			get
			{
				bool result = false;

				lock (this.RawChildNodes)
					result = this.RawChildNodes.Count > 0;

				return result;
			}
		}

		public IReadOnlyList<Node> ChildNodes
		{
			get
			{
				Node[] result;

				lock (this.RawChildNodes)
					result = this.RawChildNodes.ToArray();

				return result;
			}
		}

		public void AddChild(Node node)
		{
			lock (this.RawChildNodes)
				this.RawChildNodes.Add(node);

			node.Parent = this;
		}

		public void RemoveChild(Node node)
		{
			lock (this.RawChildNodes)
				this.RawChildNodes.Remove(node);

			node.Parent = null;
		}

		public void SetParent(Node parent)
		{
			if (this.Parent != null)
				this.Parent.RemoveChild(this);

			parent.AddChild(this);
		}

		public void Remove()
		{
			if (this.Parent != null)
				this.Parent.RemoveChild(this);
		}

		public void RemoveAllChildNodes()
		{
			lock (this.RawChildNodes)
				this.RawChildNodes.Clear();
		}

		public override string ToString()
			=> this.ToString(Formatting.None);

		public string ToString(Formatting formatting)
			=> Utilities.BuildXml(this, formatting);

		public IReadOnlyList<Cdata> Cdatas
			=> this.ChildNodes.OfType<Cdata>().ToList();

		public IReadOnlyList<Comment> Comments
			=> this.ChildNodes.OfType<Comment>().ToList();
	}
}
