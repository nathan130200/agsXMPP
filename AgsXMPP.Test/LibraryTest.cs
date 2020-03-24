using System;
using System.Diagnostics;
using System.Reflection;
using AgsXMPP.Events;
using AgsXMPP.Protocol.Client;
using AgsXMPP.Xml.Dom;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgsXMPP.Test
{
	[TestClass]
	public class LibraryTest
	{
		XmppClientConnection Client;
		XmppComponentConnection Component;

		[TestInitialize]
		public void InitializeConnection()
		{
			this.Client = new XmppClientConnection();
			this.Component = new XmppComponentConnection();
		}

		[TestMethod]
		[TestCategory("Xml Formatting")]
		public void TestPrintXmlFormatting()
		{
			var e = new Element("foo", ns: "urn:bar");
			e.AddChild(new Element("baz"));

			var xml = e.ToString(true, 2);
			var excepted = @"<foo xmlns=""urn:bar"">
  <baz />
</foo>";

			//Debug.WriteLine("Excepted: " + excepted);
			//Debug.WriteLine("\nCurrent: " + xml);
			//Debug.WriteLine("\nIs Match: " + excepted.Equals(xml));

			Assert.AreEqual(excepted, xml, "Mismatch xml");
		}

		[TestMethod]
		[TestCategory("Xml Formatting")]
		[DataRow(2)]
		[DataRow(4)]
		[DataRow(6)]
		[DataRow(8)]
		[DataRow(10)]
		[DataRow(12)]
		public void TestPrintXmlFormattingSized(int size)
		{
			var e = new Element("foo", ns: "urn:bar");
			e.AddChild(new Element("baz"));

			var xml = e.ToString(true, size);
			var excepted = @$"<foo xmlns=""urn:bar"">
{new string(' ', size)}<baz />
</foo>";

			//Debug.WriteLine("Excepted: " + excepted);
			//Debug.WriteLine("\nCurrent: " + xml);
			//Debug.WriteLine("\nIs Match: " + excepted.Equals(xml));

			Assert.AreEqual(excepted, xml);
		}

		[TestMethod]
		[TestCategory("Xml Formatting")]
		public void TestPrintXmlWithoutFormatting()
		{
			var e = new Element("foo", ns: "urn:bar");
			e.AddChild(new Element("baz"));

			var excepted = @"<foo xmlns=""urn:bar""><baz /></foo>";
			var xml = e.ToString(false);

			Assert.AreEqual(excepted, xml);
		}

		[TestMethod]
		[DataRow(IQType.Get, "get")]
		[DataRow(IQType.Set, "set")]
		[DataRow(IQType.Result, "result")]
		[DataRow(IQType.Error, "error")]
		public void TestXmppEnumAsAttributeValue(IQType type, string exceptedValue)
		{
			var iq = new IQ(type);
			Assert.IsNotNull(iq.GetAttribute("type"));
			Assert.AreEqual(iq.GetAttribute("type"), exceptedValue);
			Assert.AreEqual(iq.GetAttributeEnum<IQType>("type"), type);
		}

		public EventEmitter<T> GetEventEmitter<T>(IXmppConnection connection, string eventName)
			where T : Delegate
		{
			return connection.GetType().GetField($"m_{eventName}", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(connection) as EventEmitter<T>;
		}

		[TestMethod]
		public void TestCorruptedComponentEventHandlers()
		{
			var m_OnIq = this.GetEventEmitter<AgsXMPP.Protocol.Component.IqHandler>(this.Component, "OnIq");
			Assert.IsNotNull(m_OnIq);
			Assert.AreEqual(1, m_OnIq.Handlers.Count);
			Debug.WriteLine("Handlers: " + m_OnIq.Handlers.Count.ToString());

			this.Component.OnIq += this.OnComponentIq;
			this.Component.OnIq += (s, e) => { };

			Assert.AreEqual(3, m_OnIq.Handlers.Count);
			Debug.WriteLine("Handlers: " + m_OnIq.Handlers.Count.ToString());
		}

		void OnComponentIq(object sender, AgsXMPP.Protocol.Component.IQ iq)
		{

		}

		void OnClientIq(object sender, AgsXMPP.Protocol.Client.IQ iq)
		{

		}
	}
}
