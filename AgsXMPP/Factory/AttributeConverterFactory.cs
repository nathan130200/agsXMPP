#pragma warning disable

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AgsXMPP.Xml;
using AgsXMPP.Xml.Converters;

namespace AgsXMPP.Factory
{
	public static class AttributeConverterFactory
	{
		static AttributeConverterFactory()
		{
			RegisterAttributeConverter<Jid, JidAttributeConverter>();
			RegisterAttributeConverter<bool, BooleanAttributeConverter>();
			RegisterAttributeConverter<short, Int16AttributeConverter>();
			RegisterAttributeConverter<ushort, UInt16AttributeConverter>();
			RegisterAttributeConverter<int, Int32AttributeConverter>();
			RegisterAttributeConverter<uint, UInt32AttributeConverter>();
			RegisterAttributeConverter<long, Int64AttributeConverter>();
			RegisterAttributeConverter<ulong, UInt64AttributeConverter>();
			RegisterAttributeConverter<double, DoubleAttributeConverter>();
			RegisterAttributeConverter<float, FloatAttributeConverter>();
		}

		internal static ConcurrentDictionary<Type, IAttributeConverter> Converters
			= new ConcurrentDictionary<Type, IAttributeConverter>();

		public static void RegisterAttributeConverter<T, TConverter>()
			where TConverter : IAttributeConverter<T>
		{
			var ctor = typeof(TConverter).GetTypeInfo()
				.DeclaredConstructors
				.FirstOrDefault(x => x.GetParameters().Length == 0);

			if (ctor is null)
				throw new InvalidOperationException($"Type '{typeof(TConverter)}' don't have valid constructors.");

			if (ctor.Invoke(null) is IAttributeConverter<T> instance)
			{
				Debug.WriteLine("Register attribute converter for {0} ({1})", typeof(T).FullName, typeof(TConverter).FullName);
				Converters[typeof(T)] = instance;
			}
		}

		public static void RegisterAttributeConverter<T, TConverter>(TConverter converter) where TConverter : IAttributeConverter<T>
			=> Converters[typeof(T)] = converter;

		static bool TryGetConverter<T>(out IAttributeConverter<T> converter)
		{
			var type = typeof(T);

			if (type == typeof(Nullable<>))
				type = type.GenericTypeArguments.First();

			Converters.TryGetValue(type, out var temp);
			converter = temp as IAttributeConverter<T>;
			return !(converter is null);
		}

		public static string SerializeValue<T>(T value)
		{
			var type = typeof(T);
			var nullableType = default(Type);
			object xArgumentValue = value;

			if ((nullableType = Nullable.GetUnderlyingType(type)) != null)
			{
				var xnullable = typeof(Nullable<>).MakeGenericType(nullableType);
				var _HasValue = xnullable.GetProperty("HasValue");
				var _Value = xnullable.GetProperty("Value");


				if (value is null)
					xArgumentValue = Activator.CreateInstance(type);
				else
				{
					if ((bool)_HasValue.GetValue(value) == true)
						xArgumentValue = _Value.GetValue(value);
					else
						xArgumentValue = Activator.CreateInstance(type);
				}

				type = nullableType;
			}

			var xMethodArgs = new object[1];

			var xMethodResult = (bool)typeof(AttributeConverterFactory).GetTypeInfo()
				.GetMethod(nameof(TryGetConverter), BindingFlags.Static | BindingFlags.NonPublic)
				.MakeGenericMethod(type)
				.Invoke(null, xMethodArgs);

			string result;

			if (xMethodResult)
			{
				result = (string)xMethodArgs[0].GetType().GetTypeInfo()
					.GetMethod("Serialize", BindingFlags.Public | BindingFlags.Instance)
					.Invoke(xMethodArgs[0], new object[] { xArgumentValue });
			}
			else
			{
				result = (string)xMethodArgs[0].GetType()
						.MakeGenericType(type)
						.GetMethod("Serialize", BindingFlags.Public | BindingFlags.Instance)
						.Invoke(xMethodArgs[0], new object[] { value });
			}

			return result;
		}
	}
}

#pragma warning restore