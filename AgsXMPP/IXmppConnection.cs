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

using System;
using AgsXMPP.Net;
using AgsXMPP.Xml;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP
{
	public interface IXmppConnection
	{
		bool AutoResolveConnectServer { get; set; }
		BaseSocket ClientSocket { get; }
		string ConnectServer { get; set; }
		bool KeepAlive { get; set; }
		int KeepAliveInterval { get; set; }
		int Port { get; set; }
		string Server { get; set; }
		SocketConnectionType SocketType { get; set; }
		string StreamId { get; set; }
		StreamParser StreamParser { get; }
		string StreamVersion { get; set; }
		XmppConnectionState State { get; }

		event ErrorHandler OnError;
		event BaseSocket.OnSocketDataHandler OnReadSocketData;
		event XmlHandler OnReadXml;
		event BaseSocket.OnSocketDataHandler OnWriteSocketData;
		event XmlHandler OnWriteXml;
		event XmppConnectionStateHandler OnXmppConnectionStateChanged;

		void Close();
		void Open(string xml);
		void Send(Element e);
		void Send(string xml);
	}
}