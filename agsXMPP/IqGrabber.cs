/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2020 by AG-Software, FRNathan13								 *
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

using AgsXMPP.Protocol.Client;

namespace AgsXMPP
{
	using IqGrabberCallback = PacketGrabberCallback<IqGrabber, IQ>;
	using IqGrabberInfo = PacketGrabberInfo<IqGrabber, IQ>;

	public class IqGrabber : PacketGrabber<XmppConnection, IQ, IqGrabber>
	{
		public IqGrabber(XmppClientConnection connection)
		{
			this.Connection = connection;
			((XmppClientConnection)this.Connection).OnIq += this.OnIq;
		}

		public IqGrabber(XmppComponentConnection connection)
		{
			this.Connection = connection;
			((XmppComponentConnection)this.Connection).OnIq += this.OnComponentIq;
		}

		void OnComponentIq(object sender, Protocol.Component.IQ iq)
			=> this.OnIq(sender, iq as IQ);

		void OnIq(object sender, IQ iq)
		{
			if (iq == null)
				return;

			if (iq.Id == null)
				return;

			if (!this.Queue.TryRemove(iq.Id, out var info))
				return;

			if (info == null || info.Callback == null)
				return;

			info.Callback(this, iq, info.UserData);
		}

		/// <summary>
		/// Send an IQ Request and store the object with callback in the Hashtable
		/// </summary>
		/// <param name="iq">The iq to send</param>
		/// <param name="callback">the callback function which gets raised for the response</param>
		public void SendIq(IQ iq, IqGrabberCallback callback)
		{
			this.SendIq(iq, callback, null);
		}

		/// <summary>
		/// Send an IQ Request and store the object with callback in the Hashtable
		/// </summary>
		/// <param name="iq">The iq to send</param>
		/// <param name="callback">the callback function which gets raised for the response</param>
		/// <param name="data">additional object for arguments</param>
		public void SendIq(IQ iq, IqGrabberCallback callback, object data)
		{
			if (callback != null)
			{
				var info = new IqGrabberInfo
				{
					Callback = callback,
					UserData = data
				};

				this.Queue.AddOrUpdate(iq.Id, info, (key, old) => info);
			}

			this.Connection.Send(iq);
		}
	}
}