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

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections;

#if SSL
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
#endif

#if MONOSSL
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Protocol.Tls;
#endif

#if BCCRYPTO
using Org.BouncyCastle.Crypto.Tls;
#endif

using AgsXMPP.IO.Compression;

namespace AgsXMPP.Net
{
	public class ConnectTimeoutException : Exception
	{
		public ConnectTimeoutException(string message) : base(message)
		{
		}
	}

	/// <summary>
	/// Use async sockets to connect, send and receive data over TCP sockets.
	/// </summary>
	public class ClientSocket : BaseSocket
	{
		Socket _socket;
#if SSL
		SslStream m_SSLStream;
#endif
#if MONOSSL
        SslClientStream		m_SSLStream;
#endif
		NetworkStream m_Stream;
		Stream m_NetworkStream = null;


		const int BUFFERSIZE = 1024;
		private byte[] m_ReadBuffer = null;

		private bool m_SSL = false;

		private bool m_PendingSend = false;
		private Queue m_SendQueue = new Queue();

		/// <summary>
		/// is compression used for this connection
		/// </summary>
		private bool m_Compressed = false;

		private bool m_ConnectTimedOut = false;
		/// <summary>
		/// is used to compress data
		/// </summary>
		private Deflater deflater = null;
		/// <summary>
		/// is used to decompress data
		/// </summary>
		private Inflater inflater = null;

		private Timer connectTimeoutTimer;


		#region << Constructor >>
		public ClientSocket()
		{

		}
		#endregion

		#region << Properties >>
		public bool SSL
		{
			get { return this.m_SSL; }
#if SSL || MONOSSL
			set { this.m_SSL = value; }
#endif
		}

		public override bool SupportsStartTls
		{
#if SSL || MONOSSL
			get
			{
				return true;
			}
#else
			get
			{
				return false;
			}
#endif
		}

		/// <summary>
		/// Returns true if the socket is connected to the server. The property 
		/// Socket.Connected does not always indicate if the socket is currently 
		/// connected, this polls the socket to determine the latest connection state.
		/// </summary>
		public override bool Connected
		{
			get
			{
				// return right away if have not created socket
				if (this._socket == null)
					return false;

				return this._socket.Connected;

				// commented this out because it caused problems on some machines.
				// return the connected property of the socket now

				//the socket is not connected if the Connected property is false
				//if (!_socket.Connected)
				//    return false;

				//// there is no guarantee that the socket is connected even if the
				//// Connected property is true
				//try
				//{
				//    // poll for error to see if socket is connected
				//    return !_socket.Poll(1, SelectMode.SelectError);
				//}
				//catch
				//{
				//    return false;
				//}
			}
		}

		public bool Compressed
		{
			get { return this.m_Compressed; }
			set { this.m_Compressed = value; }
		}
		#endregion

		/// <summary>
		/// Connect to the specified address and port number.
		/// </summary>
		public void Connect(string address, int port)
		{
			this.Address = address;
			this.Port = port;

			this.Connect();
		}

		public override void Connect()
		{
			base.Connect();

			// Socket is never compressed at startup
			this.m_Compressed = false;

			this.m_ReadBuffer = null;
			this.m_ReadBuffer = new byte[BUFFERSIZE];

			try
			{
				var ipHostInfo = System.Net.Dns.GetHostEntry(this.Address);
				var ipAddress = ipHostInfo.AddressList[0];// IPAddress.Parse(address);
				var endPoint = new IPEndPoint(ipAddress, this.Port);

				// Timeout
				// .NET supports no timeout for connect, and the default timeout is very high, so it could
				// take very long to establish the connection with the default timeout. So we handle custom
				// connect timeouts with a timer
				this.m_ConnectTimedOut = false;
				var timerDelegate = new TimerCallback(this.connectTimeoutTimerDelegate);
				this.connectTimeoutTimer = new Timer(timerDelegate, null, this.ConnectTimeout, this.ConnectTimeout);

				this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this._socket.BeginConnect(endPoint, new AsyncCallback(this.EndConnect), null);
			}
			catch (Exception ex)
			{
				this.FireOnError(ex);
			}
		}

		private void EndConnect(IAsyncResult ar)
		{
			if (this.m_ConnectTimedOut)
			{
				this.FireOnError(new ConnectTimeoutException("Attempt to connect timed out"));
			}
			else
			{
				try
				{
					// stop the timeout timer
					this.connectTimeoutTimer.Dispose();

					// pass connection status with event
					this._socket.EndConnect(ar);

					this.m_Stream = new NetworkStream(this._socket, false);

					this.m_NetworkStream = this.m_Stream;

#if SSL || MONOSSL
					if (this.m_SSL)
						this.InitSSL();
#endif

					this.FireOnConnect();

					// Setup Receive Callback
					this.Receive();
				}
				catch (Exception ex)
				{
					this.FireOnError(ex);
				}
			}
		}

		/// <summary>
		/// Connect Timeout Timer Callback
		/// </summary>
		/// <param name="stateInfo"></param>
		private void connectTimeoutTimerDelegate(object stateInfo)
		{
			// for compression debug statisticsConsole.WriteLine("Connect Timeout");
			this.connectTimeoutTimer.Dispose();
			this.m_ConnectTimedOut = true;
			this._socket.Close();
		}

#if SSL
		/// <summary>
		/// Starts TLS on a "normal" connection
		/// </summary>
		public override void StartTls()
		{
			base.StartTls();
			this.InitSSL();
		}

		/// <summary>
		/// 
		/// </summary>
		private void InitSSL()
		{
			this.InitSSL(SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="protocol"></param>		
		private void InitSSL(SslProtocols protocol)
		{
			this.m_SSLStream = new SslStream(
				this.m_Stream,
				false,
				new RemoteCertificateValidationCallback(this.ValidateCertificate),
				null
				);
			try
			{
				this.m_SSLStream.AuthenticateAsClient(this.Address, null, protocol, true);
				// Display the properties and settings for the authenticated stream.
				//DisplaySecurityLevel(m_SSLStream);
				//DisplaySecurityServices(m_SSLStream);
				//DisplayCertificateInformation(m_SSLStream);
				//DisplayStreamProperties(m_SSLStream);

			}
			catch (AuthenticationException e)
			{
				//Console.WriteLine("Exception: {0}", e.Message);
				if (e.InnerException != null)
				{
					//Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
				}
				//Console.WriteLine ("Authentication failed - closing the connection.");
				//client.Close();
				return;
			}

			this.m_NetworkStream = this.m_SSLStream;
			this.m_SSL = true;
		}


		#region << SSL Properties Display stuff >>

		/*
		private void DisplaySecurityLevel(SslStream stream)
        {
            Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
            Console.WriteLine("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength);
            Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
            Console.WriteLine("Protocol: {0}", stream.SslProtocol);
        }

        private void DisplaySecurityServices(SslStream stream)
        {
            Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
            Console.WriteLine("IsSigned: {0}", stream.IsSigned);
            Console.WriteLine("Is Encrypted: {0}", stream.IsEncrypted);
        }
        
        private void DisplayStreamProperties(SslStream stream)
        {
            Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
            Console.WriteLine("Can timeout: {0}", stream.CanTimeout);
        }

        private void DisplayCertificateInformation(SslStream stream)
        {
            //Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);
            // Display the properties of the client's certificate.
            var remoteCertificate = stream.RemoteCertificate;
            if (stream.RemoteCertificate != null)
            {
                Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                    remoteCertificate.Subject,
                    remoteCertificate.GetEffectiveDateString(),
                    remoteCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Remote certificate is null.");
            }
        }
		*/

		#endregion

		/// <summary>
		/// Validate the SSL certificate here
		/// for now we dont stop the SSL connection an return always true
		/// </summary>
		/// <param name="certificate"></param>
		/// <param name="certificateErrors"></param>
		/// <returns></returns>
		//private bool ValidateCertificate (X509Certificate certificate, int[] certificateErrors) 
		private bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return this.FireOnValidateCertificate(sender, certificate, chain, sslPolicyErrors);
		}
#endif

#if MONOSSL
        /// <summary>
		/// Starts TLS on a "normal" connection
		/// </summary>
		public override void StartTls()
		{
			base.StartTls();

			Mono.Security.Protocol.Tls.SecurityProtocolType protocol = Mono.Security.Protocol.Tls.SecurityProtocolType.Tls;
			InitSSL(protocol);
		}

		/// <summary>
		/// 
		/// </summary>
		private void InitSSL()
		{
			InitSSL(Mono.Security.Protocol.Tls.SecurityProtocolType.Default);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="protocol"></param>
		private void InitSSL(Mono.Security.Protocol.Tls.SecurityProtocolType protocol)
		{
			m_SSLStream = new SslClientStream(m_Stream, Address, false, protocol, null);				
			m_SSLStream.ServerCertValidationDelegate = new Mono.Security.Protocol.Tls.CertificateValidationCallback(ValidateCertificate);
			m_NetworkStream = m_SSLStream;	
			m_SSL = true;
			// Send a whitespace to start the encryption of the connection now
			Send(" ");
		}

		/// <summary>
		/// Validate the SSL certificate here
		/// for now we dont stop the SSL connection an return always true
		/// </summary>
		/// <param name="certificate"></param>
		/// <param name="certificateErrors"></param>
		/// <returns></returns>
		private bool ValidateCertificate (X509Certificate certificate, int[] certificateErrors) 
		{
			return base.FireOnValidateCertificate(certificate, certificateErrors);
		}
#endif

#if BCCRYPTO
        /// <summary>
        /// Starts TLS on a "normal" connection
        /// </summary>
        public override void StartTls()
        {
            base.StartTls();

            //TlsProtocolHandler protocolHandler = new TlsProtocolHandler(m_NetworkStream, m_NetworkStream);
            //Stream st = new NetworkStream(_socket, false);
            TlsProtocolHandler protocolHandler = new TlsProtocolHandler(m_Stream, m_Stream);
            //TlsProtocolHandler protocolHandler = new TlsProtocolHandler(st, st);

            CertificateVerifier certVerify = new CertificateVerifier();
            certVerify.OnVerifyCertificate += new CertificateValidationCallback(certVerify_OnVerifyCertificate);

            protocolHandler.Connect(certVerify);

            m_NetworkStream = new SslStream(protocolHandler.InputStream, protocolHandler.OutputStream);
            m_SSL = true;
        }

        internal bool certVerify_OnVerifyCertificate(Org.BouncyCastle.Asn1.X509.X509CertificateStructure[] certs)
        {
            return base.FireOnValidateCertificate(certs);
        }
#endif

		/// <summary>
		/// Start Compression on the socket
		/// </summary>
		public override void StartCompression()
		{
			this.InitCompression();
		}

		/// <summary>
		/// Initialize compression stuff (Inflater, Deflater)
		/// </summary>
		private void InitCompression()
		{
			base.StartCompression();

			this.inflater = new Inflater();
			this.deflater = new Deflater();

			// Set the compressed flag to true when we init compression
			this.m_Compressed = true;
		}

		/// <summary>
		/// Disconnect from the server.
		/// </summary>
		public override void Disconnect()
		{
			base.Disconnect();

			lock (this)
			{
				// TODO maybe we should notify the user which packets were not sent.
				this.m_PendingSend = false;
				this.m_SendQueue.Clear();
			}

			// return right away if have not created socket
			if (this._socket == null)
				return;

			try
			{
				// first, shutdown the socket
				this._socket.Shutdown(SocketShutdown.Both);
			}
			catch { }

			try
			{
				// next, close the socket which terminates any pending
				// async operations
				this._socket.Close();
			}
			catch { }

			this.FireOnDisconnect();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		public override void Send(string data)
		{
			this.Send(Encoding.UTF8.GetBytes(data));
		}

		/// <summary>
		/// Send data to the server.
		/// </summary>
		public override void Send(byte[] bData)
		{
			lock (this)
			{
				try
				{
					this.FireOnSend(bData, bData.Length);

					//Console.WriteLine("Socket OnSend: " + System.Text.Encoding.UTF8.GetString(bData, 0, bData.Length));

					// compress bytes if we are on a compressed socket
					if (this.m_Compressed)
					{
						var tmpData = new byte[bData.Length];
						bData.CopyTo(tmpData, 0);

						bData = this.Compress(bData);

						// for compression debug statistics
						// base.FireOnOutgoingCompressionDebug(this, bData, bData.Length, tmpData, tmpData.Length);
					}

					// .NET 2.0 SSL Stream issues when sending multiple async packets
					// http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=124213&SiteID=1
					if (this.m_PendingSend)
					{
						this.m_SendQueue.Enqueue(bData);
					}
					else
					{
						this.m_PendingSend = true;
						try
						{
							this.m_NetworkStream.BeginWrite(bData, 0, bData.Length, new AsyncCallback(this.EndSend), null);
						}
						catch (Exception)
						{
							this.Disconnect();
						}
					}
				}
				catch (Exception)
				{

				}
			}

		}

		/// <summary>
		/// Read data from server.
		/// </summary>
		private void Receive()
		{
			this.m_NetworkStream.BeginRead(this.m_ReadBuffer, 0, BUFFERSIZE, new AsyncCallback(this.EndReceive), null);
		}

		private void EndReceive(IAsyncResult ar)
		{
			try
			{
				int nBytes;
				nBytes = this.m_NetworkStream.EndRead(ar);
				if (nBytes > 0)
				{
					// uncompress Data if we are on a compressed socket
					if (this.m_Compressed)
					{
						var buf = this.Decompress(this.m_ReadBuffer, nBytes);
						this.FireOnReceive(buf, buf.Length);
						// for compression debug statistics
						//base.FireOnInComingCompressionDebug(this, m_ReadBuffer, nBytes, buf, buf.Length);
					}
					else
					{
						//Console.WriteLine("Socket OnReceive: " + System.Text.Encoding.UTF8.GetString(m_ReadBuffer, 0, nBytes));                        
						// Raise the receive event
						this.FireOnReceive(this.m_ReadBuffer, nBytes);
					}
					// Setup next Receive Callback
					if (this.Connected)
						this.Receive();
				}
				else
				{
					this.Disconnect();
				}
			}
			catch (ObjectDisposedException)
			{
				//object already disposed, just exit
				return;
			}
			catch (IOException ex)
			{
				Console.WriteLine("\nSocket Exception: " + ex.Message);
				this.Disconnect();
			}
		}

		private void EndSend(IAsyncResult ar)
		{
			lock (this)
			{
				try
				{
					this.m_NetworkStream.EndWrite(ar);
					if (this.m_SendQueue.Count > 0)
					{
						var bData = (byte[])this.m_SendQueue.Dequeue();
						this.m_NetworkStream.BeginWrite(bData, 0, bData.Length, new AsyncCallback(this.EndSend), null);
					}
					else
					{
						this.m_PendingSend = false;
					}
				}
				catch (Exception)
				{
					this.Disconnect();
				}
			}
		}

		#region << compression functions >>
		/// <summary>
		/// Compress bytes
		/// </summary>
		/// <param name="bIn"></param>
		/// <returns></returns>
		private byte[] Compress(byte[] bIn)
		{
			int ret;

			// The Flush SHOULD be after each STANZA
			// The libds sends always one complete XML Element/stanza,
			// it doesn't cache stanza and send them in groups, and also doesnt send partial
			// stanzas. So everything should be ok here.
			this.deflater.SetInput(bIn);
			this.deflater.Flush();

			var ms = new MemoryStream();
			do
			{
				var buf = new byte[BUFFERSIZE];
				ret = this.deflater.Deflate(buf);
				if (ret > 0)
					ms.Write(buf, 0, ret);

			} while (ret > 0);

			using (ms)
				return ms.ToArray();

		}

		/// <summary>
		/// Decompress bytes
		/// </summary>
		/// <param name="bIn"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		private byte[] Decompress(byte[] bIn, int length)
		{
			int ret;

			this.inflater.SetInput(bIn, 0, length);

			var ms = new MemoryStream();
			do
			{
				var buf = new byte[BUFFERSIZE];
				ret = this.inflater.Inflate(buf);
				if (ret > 0)
					ms.Write(buf, 0, ret);

			} while (ret > 0);

			using (ms)
				return ms.ToArray();
		}

		#endregion
	}
}