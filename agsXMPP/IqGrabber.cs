/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2019 by AG-Software, FRNathan13								 *
 * All Rights Reserved.																 *
 * Contact information for AG-Software is available at http://www.ag-software.de	 *
 *																					 *
 * Licence:																			 *
 * The agsXMPP SDK is released under a dual licence									 *
 * agsXMPP can be used under either of two licences									 *
 * 																					 *
 * A commercial licence which is probably the most appropriate for commercial 		 *
 * corporate use and closed source projects. 										 *
 *																					 *
 * The GNU Public License (GPL) is probably most appropriate for inclusion in		 *
 * other open source projects.														 *
 *																					 *
 * See README.html for details.														 *
 *																					 *
 * For general enquiries visit our website at:										 *
 * http://www.ag-software.de														 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Threading;
using agsXMPP.Protocol.client;

namespace agsXMPP
{
	public delegate void IqCB(object sender, Iq iq, object data);

	public class IqGrabber : PacketGrabber
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="conn"></param>
		public IqGrabber(XmppClientConnection conn)
		{
			this.m_connection = conn;
			conn.OnIq += new IqHandler(this.OnIq);
		}

		public IqGrabber(XmppComponentConnection conn)
		{
			this.m_connection = conn;
#if MONOSSL
            conn.OnIq += new agsXMPP.protocol.component.IqHandler(OnIqComponent);
#else
			conn.OnIq += new Protocol.component.IqHandler(this.OnIq);
#endif
		}

#if !CF
		private Iq synchronousResponse = null;

		private int m_SynchronousTimeout = 5000;

		/// <summary>
		/// Timeout for synchronous requests, default value is 5000 (5 seconds)
		/// </summary>
		public int SynchronousTimeout
		{
			get { return this.m_SynchronousTimeout; }
			set { this.m_SynchronousTimeout = value; }
		}
#endif

#if MONOSSL
		private void OnIqComponent(object sender, agsXMPP.protocol.component.IQ iq)
		{
			OnIq(sender, iq);
		}
#endif

		/// <summary>
		/// An IQ Element is received. Now check if its one we are looking for and
		/// raise the event in this case.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnIq(object sender, Iq iq)
		{
			if (iq == null)
				return;

			var id = iq.Id;
			if (id == null)
				return;

			TrackerData td;

			lock (this.m_grabbing)
			{
				td = (TrackerData)this.m_grabbing[id];

				if (td == null)
				{
					return;
				}
				this.m_grabbing.Remove(id);
			}

			td.cb(this, iq, td.data);
		}

		/// <summary>
		/// Send an IQ Request and store the object with callback in the Hashtable
		/// </summary>
		/// <param name="iq">The iq to send</param>
		/// <param name="cb">the callback function which gets raised for the response</param>
		public void SendIq(Iq iq, IqCB cb)
		{
			this.SendIq(iq, cb, null);
		}

		/// <summary>
		/// Send an IQ Request and store the object with callback in the Hashtable
		/// </summary>
		/// <param name="iq">The iq to send</param>
		/// <param name="cb">the callback function which gets raised for the response</param>
		/// <param name="cbArg">additional object for arguments</param>
		public void SendIq(Iq iq, IqCB cb, object cbArg)
		{
			// check if the callback is null, in case of wrong usage of this class
			if (cb != null)
			{
				var td = new TrackerData();
				td.cb = cb;
				td.data = cbArg;

				this.m_grabbing[iq.Id] = td;
			}
			this.m_connection.Send(iq);
		}

#if !CF
		/// <summary>
		/// Sends an Iq synchronous and return the response or null on timeout
		/// </summary>
		/// <param name="iq">The IQ to send</param>
		/// <param name="timeout"></param>
		/// <returns>The response IQ or null on timeout</returns>
		public Iq SendIq(Iq iq, int timeout)
		{
			this.synchronousResponse = null;
			var are = new AutoResetEvent(false);

			this.SendIq(iq, new IqCB(this.SynchronousIqResult), are);

			if (!are.WaitOne(timeout, true))
			{
				// Timed out
				lock (this.m_grabbing)
				{
					if (this.m_grabbing.ContainsKey(iq.Id))
						this.m_grabbing.Remove(iq.Id);
				}
				return null;
			}

			return this.synchronousResponse;
		}

		/// <summary>
		/// Sends an Iq synchronous and return the response or null on timeout.
		/// Timeout time used is <see cref="SynchronousTimeout"/>
		/// </summary>
		/// <param name="iq">The IQ to send</param>        
		/// <returns>The response IQ or null on timeout</returns>
		public Iq SendIq(Iq iq)
		{
			return this.SendIq(iq, this.m_SynchronousTimeout);
		}

		/// <summary>
		/// Callback for synchronous iq grabbing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="iq"></param>
		/// <param name="data"></param>
		private void SynchronousIqResult(object sender, Iq iq, object data)
		{
			this.synchronousResponse = iq;

			var are = data as AutoResetEvent;
			are.Set();
		}
#endif
		private class TrackerData
		{
			public IqCB cb;
			public object data;
		}
	}
}