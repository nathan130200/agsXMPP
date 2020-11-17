using System;
using System.Diagnostics;
using System.Xml;
using AgsXMPP.Xml.Dom;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgsXMPP.Test
{
	[TestClass]
	public class ElementAttributesTest
	{
		[TestMethod]
		[DataRow("foo", "urn:foo", "<foo xmlns=\"urn:foo\" />")]
		[DataRow("bar", "urn:baz", "<bar xmlns=\"urn:baz\" />")]
		[DataRow("baz", "urn:bar", "<baz xmlns=\"urn:bar\" />")]
		[DataRow("query", "urn:my", "<query xmlns=\"urn:my\" />")]
		[DataRow("my", "urn:test", "<my xmlns=\"urn:test\" />")]
		public void EnsureValidElement(string name, string xmlns, string excepted)
		{
			var current = new Element(name, xmlns).ToString(Formatting.None);
			Assert.AreEqual(excepted, current);
		}

		[TestMethod]
		public void SetAttributesToElement()
		{
			var el = new Element("foo", "urn:xmpp:foo");
			el.SetAttribute("my_int32", 1234);
			el.SetAttribute("my_float", 1.234567f);

			var excepted = "<foo xmlns=\"urn:xmpp:foo\" my_int32=\"1234\" my_float=\"1.234567\" />";
			Assert.AreEqual(el.ToString(), excepted);
			Console.WriteLine(el.ToString());
		}
	}
}
