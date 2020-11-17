using System;

namespace AgsXMPP.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	public class XmppElementFactoryAttribute : Attribute
	{
		public string Name { get; }
		public string Namespace { get; }

		public XmppElementFactoryAttribute(string name, string ns)
		{
			this.Name = name;
			this.Namespace = ns;
		}
	}
}
