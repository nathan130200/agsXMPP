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

using System.Collections;
using System.Linq;
using AgsXMPP.Collections;
using AgsXMPP.Protocol.Client;

namespace AgsXMPP
{
	using PresenceGrabberCallback = PacketGrabberCallback<PresenceGrabber, Presence>;
	using PresenceGrabberInfo = PacketGrabberInfo<PresenceGrabber, Presence>;

	public class PresenceGrabber : PacketGrabber<XmppClientConnection, Presence, PresenceGrabber>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PresenceGrabber"/> class.
		/// </summary>
		/// <param name="conn">The conn.</param>
		public PresenceGrabber(XmppClientConnection conn)
		{
			this.Connection = conn;
			this.Connection.OnPresence += this.OnPresence;
		}

		public void Add(Jid jid, PresenceGrabberCallback cb, object data)
		{
			if (this.Queue.ContainsKey(jid.ToString()))
				return;

			var info = new PresenceGrabberInfo
			{
				Callback = cb,
				UserData = data,
				Comparer = BareJidComparer.Instance
			};

			this.Queue.AddOrUpdate(jid.ToString(), info, (key, old) => info);
		}

		/// <summary>
		/// Adds the specified jid.
		/// </summary>
		/// <param name="jid">The jid.</param>
		/// <param name="comparer">The comparer.</param>
		/// <param name="callback">The callback.</param>
		/// <param name="data">The callback Arguments.</param>
		public void Add(Jid jid, IComparer comparer, PresenceGrabberCallback callback, object data)
		{
			if (this.Queue.ContainsKey(jid.ToString()))
				return;

			var info = new PresenceGrabberInfo
			{
				Callback = callback,
				UserData = data,
				Comparer = comparer
			};

			this.Queue.AddOrUpdate(jid.ToString(), info, (key, old) => info);
		}

		/// <summary>
		/// Pending request can be removed.
		/// This is useful when a ressource for the callback is destroyed and
		/// we are not interested anymore at the result.
		/// </summary>
		/// <param name="id">ID of the Iq we are not interested anymore</param>
		public void Remove(Jid jid)
			=> this.Queue.TryRemove(jid.ToString(), out var _);

		/// <summary>
		/// A presence is received. Now check if its from a Jid we are looking for and
		/// raise the event in this case.
		/// </summary>
		protected void OnPresence(object sender, Presence presence)
		{
			if (presence == null)
				return;

			foreach (var (jid, item) in this.Queue.Select(x => x))
			{
				if (item.Comparer.Compare(jid, presence.From) == 0)
					item.Callback?.Invoke(this, presence, item.UserData);
			}
		}
	}
}
