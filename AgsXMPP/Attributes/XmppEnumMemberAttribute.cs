using System;

namespace AgsXMPP.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class XmppEnumMemberAttribute : Attribute
	{
		public string Name { get; }

		public XmppEnumMemberAttribute(string name)
			=> this.Name = name;
	}
}
