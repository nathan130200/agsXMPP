using System;
using System.Reflection;
using AgsXMPP.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgsXMPP.Test
{
	[TestClass]
	public class AttributeConvertersTest
	{
		[TestMethod]
		[DataRow(typeof(bool), true, "1")]
		[DataRow(typeof(bool), false, "0")]
		[DataRow(typeof(ushort), (ushort)1234, "1234")]
		[DataRow(typeof(short), (short)1234, "1234")]
		[DataRow(typeof(uint), (uint)1234, "1234")]
		[DataRow(typeof(int), (int)1234, "1234")]
		[DataRow(typeof(ulong), (ulong)1234, "1234")]
		[DataRow(typeof(long), (long)1234, "1234")]
		[DataRow(typeof(double), (double)1.569221, "1.569221")]
		[DataRow(typeof(float), (float)3.745641, "3.745641")]
		public void SerializePrimitives(Type type, object currentValue, string exceptedValue)
		{
			var xMethod = this.GetType().GetTypeInfo().GetMethod(nameof(SerializeTest), BindingFlags.NonPublic | BindingFlags.Instance)?
				.MakeGenericMethod(type);

			xMethod?.Invoke(this, new[] { currentValue, exceptedValue });
		}

		[TestMethod]
		[DataRow(typeof(bool?), true, "1")]
		[DataRow(typeof(bool?), false, "0")]
		[DataRow(typeof(ushort?), (ushort)1234, "1234")]
		[DataRow(typeof(short?), (short)1234, "1234")]
		[DataRow(typeof(uint?), (uint)1234, "1234")]
		[DataRow(typeof(int?), (int)1234, "1234")]
		[DataRow(typeof(ulong?), (ulong)1234, "1234")]
		[DataRow(typeof(long?), (long)1234, "1234")]
		[DataRow(typeof(double?), (double)1.569221, "1.569221")]
		[DataRow(typeof(float?), (float)3.745641, "3.745641")]
		public void SerializeNullablePrimitives(Type type, object currentValue, string exceptedValue)
		{
			var xMethod = this.GetType().GetTypeInfo().GetMethod(nameof(SerializeTest), BindingFlags.NonPublic | BindingFlags.Instance)?
				.MakeGenericMethod(type);

			xMethod?.Invoke(this, new[] { currentValue, exceptedValue });
		}

		[TestMethod]
		[DataRow(typeof(bool?), null, "0")]
		[DataRow(typeof(short?), null, "0")]
		[DataRow(typeof(ushort?), null, "0")]
		[DataRow(typeof(int?), null, "0")]
		[DataRow(typeof(uint?), null, "0")]
		[DataRow(typeof(long?), null, "0")]
		[DataRow(typeof(ulong?), null, "0")]
		[DataRow(typeof(double?), null, "0.000000")]
		[DataRow(typeof(float?), null, "0.000000")]
		public void SerializeNullablePrimitivesWithNoValue(Type type, object currentValue, string exceptedValue)
		{
			var xMethod = this.GetType().GetTypeInfo().GetMethod(nameof(SerializeTest), BindingFlags.NonPublic | BindingFlags.Instance)?
				.MakeGenericMethod(type);

			xMethod?.Invoke(this, new[] { currentValue, exceptedValue });
		}

		[TestMethod]
		[DataRow("foo@bar")]
		[DataRow("foo@bar/baz")]
		[DataRow("@bar/baz")]
		[DataRow("bar.baz")]
		[DataRow("foo@bar.baz/bar")]
		public void SerializeJid(string name)
		{
			this.SerializeTest(new Jid(name), name);
		}

		protected virtual void SerializeTest<T>(T value, string exceptedValue)
		{
			var serialized = AttributeConverterFactory.SerializeValue<T>(value);
			Assert.IsTrue(string.Equals(exceptedValue, serialized, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
