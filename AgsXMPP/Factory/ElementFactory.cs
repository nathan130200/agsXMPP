using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AgsXMPP.Attributes;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Factory
{
	public static class ElementFactory
	{
		internal static bool IsInternalLibraryFactoryInitialized = false;

		static ElementFactory()
		{
			RegisterElementTypesFromCurrentAssembly();
		}

		internal static void RegisterElementTypesFromCurrentAssembly()
		{
			if (!IsInternalLibraryFactoryInitialized)
			{

				IsInternalLibraryFactoryInitialized = true;
			}
		}

		internal static readonly ConcurrentDictionary<Type, ElementFactoryInfo> Types
			 = new ConcurrentDictionary<Type, ElementFactoryInfo>();

		static bool TryExtractElementsFromType(Type type, out IEnumerable<ElementFactoryInfo> elements)
		{
			elements = type.GetCustomAttributes<XmppElementFactoryAttribute>()
				.Select(x => ElementFactoryInfo.Create(x.Name, x.Namespace));

			return elements.Any();
		}

		public static void RegisterElementTypesFromAssembly(Assembly assembly)
		{
			foreach (var type in from type in assembly.GetTypes()
								 where type.IsSubclassOf(typeof(Element))
								 where !type.IsAbstract
								 select type)
			{
				if (TryExtractElementsFromType(type, out var elements))
					RegisterElementsFromType(type, elements);
			}
		}

		public static void RegisterElementsFrom<T>()
			where T : Element, new()
		{
			var type = typeof(T);

			if (type.GetTypeInfo().IsSubclassOf(typeof(Element)))
				throw new InvalidOperationException($"Type {typeof(T)} is not valid element type.");

			if (TryExtractElementsFromType(type, out var elements))
				RegisterElementsFromType(type, elements);
		}

		public static void RegisterElementsFromType(Type type, IEnumerable<ElementFactoryInfo> elements)
		{
			foreach (var element in elements)
				Types[type] = element;
		}
	}
}