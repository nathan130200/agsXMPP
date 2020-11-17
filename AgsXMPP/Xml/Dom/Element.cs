﻿using System;
using System.Collections.Generic;
using System.Linq;
using AgsXMPP.Factory;

namespace AgsXMPP.Xml.Dom
{
	public class Element : Node
	{
		public string Prefix { get; set; }
		public string Name { get; set; }
		public bool IsRootElement => this.Parent == null;
		protected Dictionary<string, string> RawAttributes;

		protected Element() : base(NodeType.Element)
		{
			this.RawAttributes = new Dictionary<string, string>();
		}

		public Element(string name, string xmlns = default) : this()
		{
			this.Name = name;
			this.Namespace = xmlns;
		}

		public Element(string name, string xmlns = default, string text = default) : this(name, xmlns)
		{
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

		public string GetAttributeRaw(string name)
		{
			if (this.Attributes.TryGetValue(name, out var value))
				return value;

			return null;
		}

		public void SetAttributeRaw(string name, string value)
		{
			lock (this.RawAttributes)
				this.RawAttributes[name] = value;
		}

		public void SetAttribute<T>(string name, T value)
		{
			var rawValue = AttributeConverterFactory.SerializeValue(value);

			lock (this.RawAttributes)
				this.RawAttributes.Add(name, rawValue);
		}

		public T GetAttribute<T>(string name)
		{
			var rawValue = this.GetAttributeRaw(name);

			if (!string.IsNullOrEmpty(rawValue))
				throw new NotImplementedException();

			return default;
		}
	}
}
