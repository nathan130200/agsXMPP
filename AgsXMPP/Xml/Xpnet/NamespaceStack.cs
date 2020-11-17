using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AgsXMPP.Xml.Xpnet
{
	[DebuggerDisplay("Count: {Stack.Count,nq}")]
	public class NamespaceStack
	{
		protected Stack<Dictionary<string, string>> RawStack;

		public IReadOnlyList<IReadOnlyDictionary<string, string>> Stack
		{
			get
			{
				Dictionary<string, string>[] result;

				lock (this.RawStack)
					result = this.RawStack.ToArray();

				return result;
			}
		}

		public NamespaceStack()
		{
			this.RawStack = new Stack<Dictionary<string, string>>();
			this.PushScope();
			this.AddNamespace("xmlns", "http://www.w3.org/2000/xmlns/");
			this.AddNamespace("xml", "http://www.w3.org/XML/1998/namespace");
		}

		public void PushScope()
		{
			lock (this.RawStack)
				this.RawStack.Push(new Dictionary<string, string>());
		}

		public void PopScope()
		{
			lock (this.RawStack)
				this.RawStack.TryPop(out _);
		}

		public void AddNamespace(string @namespace, string value)
		{
			lock (this.RawStack)
			{
				if (this.RawStack.TryPeek(out var table))
					table[@namespace] = value;
			}
		}

		public string LookupNamespace(string prefix)
		{
			foreach (var ht in this.Stack)
			{
				if (ht.Count > 0 && ht.ContainsKey(prefix))
					return ht[prefix];
			}

			return string.Empty;
		}

		public string DefaultNamespace
			=> this.LookupNamespace(string.Empty);

		public void Clear()
		{
			lock (this.RawStack)
				this.RawStack.Clear();
		}

		public override int GetHashCode()
		{
			lock (this.RawStack)
				return this.RawStack.GetHashCode();
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			{
				var temp = this.Stack;

				foreach (var dict in temp)
				{
					foreach (var (key, value) in dict)
						sb.AppendFormat("{0}: {1}", key, value).AppendLine();

					if (dict != temp.LastOrDefault())
						sb.AppendLine();
				}
			}
			return sb.ToString();
		}
	}
}
