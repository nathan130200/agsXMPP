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
using System.Collections.Concurrent;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP
{
	public delegate void PacketGrabberCallback<in TGrabber, in TPacket>(TGrabber sender, TPacket packet, object data)
		where TPacket : Element;

	public sealed class PacketGrabberInfo<TGrabber, TPacket>
		where TPacket : Element
	{
		public PacketGrabberCallback<TGrabber, TPacket> Callback { get; internal set; }
		public object UserData { get; internal set; }
		public IComparer Comparer { get; internal set; }
	}

	/// <summary>
	/// Summary description for Grabber.
	/// </summary>
	public class PacketGrabber<TConnection, TPacket, TGrabber>
		where TPacket : Element
		where TConnection : XmppConnection
	{
		public ConcurrentDictionary<string, PacketGrabberInfo<TGrabber, TPacket>> Queue { get; internal set; }
		public TConnection Connection { get; internal set; }

		public PacketGrabber()
		{
			this.Queue = new ConcurrentDictionary<string, PacketGrabberInfo<TGrabber, TPacket>>();
		}

		public void Clear()
			=> this.Queue.Clear();

		/// <summary>
		/// Pending request can be removed.
		/// This is useful when a ressource for the callback is destroyed and
		/// we are not interested anymore at the result.
		/// </summary>
		/// <param name="id">ID of the Iq we are not interested anymore</param>
		public virtual void Remove(string id)
			=> this.Queue.TryRemove(id, out var _);
	}
}
