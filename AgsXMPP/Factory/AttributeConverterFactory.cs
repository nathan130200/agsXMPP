using System;
using System.Collections.Concurrent;
using AgsXMPP.Xml;

namespace AgsXMPP.Factory
{
	public static class AttributeConverterFactory
	{
		internal static ConcurrentDictionary<Type, IAttributeConverter> Converters
			= new ConcurrentDictionary<Type, IAttributeConverter>();
	}
}