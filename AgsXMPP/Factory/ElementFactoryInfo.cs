using System;

namespace AgsXMPP.Factory
{
	public class ElementFactoryInfo : IEquatable<ElementFactoryInfo>
	{
		internal ElementFactoryInfo(string name, string ns)
		{
			this.Name = name;
			this.Namespace = ns;
		}

		public string Name { get; set; }
		public string Namespace { get; set; }

		public override int GetHashCode()
		{
			return HashCode.Combine(this.Name, this.Namespace);
		}

		public override bool Equals(object obj)
		{
			return obj is ElementFactoryInfo other && this.Equals(other);
		}

		public bool Equals(ElementFactoryInfo other)
		{
			return this.ToString().Equals(other.ToString(), StringComparison.InvariantCultureIgnoreCase);
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.Namespace))
				return string.Concat(this.Namespace, "#", this.Name);

			return this.Name;
		}

		public static ElementFactoryInfo Create(string name, string ns)
			=> new ElementFactoryInfo(name, ns);
	}
}