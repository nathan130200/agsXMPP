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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using AgsXMPP.Events;

namespace AgsXMPP.Net
{
	public delegate void SocketDataEventHandler(object sender, byte[] data, int count);
	public delegate void SocketRemoteCertificateValidation(object sender, RemoteCertificateValidationEventArgs e);

	public class RemoteCertificateValidationEventArgs
	{
		public X509Certificate Certificate { get; internal set; }
		public X509Chain Chain { get; internal set; }
		public SslPolicyErrors Erros { get; internal set; }
		public bool Cancel { get; internal set; }
	}

	/// <summary>
	/// Base Socket class
	/// </summary>
	public abstract class BaseSocket
	{
		protected internal EventEmitter<SocketRemoteCertificateValidation> m_OnValidateCertificate = new EventEmitter<SocketRemoteCertificateValidation>();
		protected internal EventEmitter<SocketDataEventHandler> m_OnReceive = new EventEmitter<SocketDataEventHandler>();
		protected internal EventEmitter<SocketDataEventHandler> m_OnSend = new EventEmitter<SocketDataEventHandler>();
		protected internal EventEmitter<ObjectHandler> m_OnConnect = new EventEmitter<ObjectHandler>();
		protected internal EventEmitter<ObjectHandler> m_OnDisconnect = new EventEmitter<ObjectHandler>();
		protected internal EventEmitter<ErrorHandler> m_OnError = new EventEmitter<ErrorHandler>();

		public event SocketRemoteCertificateValidation OnCertificateValidation
		{
			add => this.m_OnValidateCertificate.Register(value);
			remove => this.m_OnValidateCertificate.Unregister(value);
		}

		public event SocketDataEventHandler OnReceive
		{
			add => this.m_OnReceive.Register(value);
			remove => this.m_OnReceive.Unregister(value);
		}

		public event SocketDataEventHandler OnSend
		{
			add => this.m_OnSend.Register(value);
			remove => this.m_OnSend.Unregister(value);
		}

		public event ObjectHandler OnConnect
		{
			add => this.m_OnConnect.Register(value);
			remove => this.m_OnConnect.Unregister(value);
		}

		public event ObjectHandler OnDisconnect
		{
			add => this.m_OnDisconnect.Register(value);
			remove => this.m_OnDisconnect.Unregister(value);
		}

		public event ErrorHandler OnError
		{
			add => this.m_OnError.Register(value);
			remove => this.m_OnError.Unregister(value);
		}

		private string m_Address = null;
		private int m_Port = 0;
		private long m_ConnectTimeout = 10000; // 10 seconds is default

		internal XmppConnection m_XmppCon = null;

		public BaseSocket()
		{

		}

		public string Address
		{
			get { return this.m_Address; }
			set { this.m_Address = value; }
		}

		public int Port
		{
			get { return this.m_Port; }
			set { this.m_Port = value; }
		}

		protected void FireOnConnect()
		{
			this.m_OnConnect.Invoke(this);
		}

		protected void FireOnDisconnect()
		{
			this.m_OnDisconnect.Invoke(this);
		}

		protected void FireOnReceive(byte[] b, int length)
		{
			this.m_OnReceive.Invoke(this, b, length);
		}

		protected void FireOnSend(byte[] b, int length)
		{
			this.m_OnSend.Invoke(this, b, length);
		}

		protected void FireOnError(Exception ex)
		{
			this.m_OnError.Invoke(this, ex);
		}

		protected bool FireOnValidateCertificate(
			  object sender,
			  X509Certificate certificate,
			  X509Chain chain,
			  SslPolicyErrors erros)
		{
			var evt = new RemoteCertificateValidationEventArgs
			{
				Certificate = certificate,
				Chain = chain,
				Erros = erros
			};

			this.m_OnValidateCertificate.Invoke(this, evt);

			if (evt.Cancel)
				return false;

			return true;
		}

		public virtual bool Connected
		{
			get { return false; }
		}

		public virtual bool SupportsStartTls
		{
			get { return false; }
		}

		public virtual long ConnectTimeout
		{
			get { return this.m_ConnectTimeout; }
			set { this.m_ConnectTimeout = value; }
		}

		#region << Methods >>
		public virtual void Connect()
		{

		}

		public virtual void Disconnect()
		{

		}

		public virtual void StartTls()
		{

		}

		public virtual void StartCompression()
		{

		}

		/// <summary>
		/// Added for Bosh because we have to tell the BoshClientSocket when to reset the stream
		/// </summary>
		public virtual void Reset()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		public virtual void Send(string data)
		{

		}

		/// <summary>
		/// Send data to the server.
		/// </summary>
		public virtual void Send(byte[] bData)
		{

		}

		#endregion
	}
}