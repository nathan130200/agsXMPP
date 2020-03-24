using System;
using System.Diagnostics;
using System.Reflection;
using AgsXMPP.Events;
using AgsXMPP.Protocol.Client;
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
		public void TestNewEnumFieldAttributes()
		{
			var iq = new IQ(IQType.Set);
			Assert.IsNotNull(iq.GetAttribute("type"));
			Assert.AreEqual(iq.GetAttribute("type"), "set");
			Debug.WriteLine(iq.ToString(true, 4));

			iq = new IQ(IQType.Error);
			Assert.IsNotNull(iq.GetAttribute("type"));
			Assert.AreEqual(iq.GetAttribute("type"), "error");
			Debug.WriteLine(iq.ToString(true, 4));

			iq.SetAttributeEnum("type", IQType.Result);
			Assert.IsNotNull(iq.GetAttribute("type"));
			Assert.AreEqual(iq.GetAttribute("type"), "result");
			Debug.WriteLine(iq.ToString(true, 4));
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
