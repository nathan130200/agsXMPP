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
using AgsXMPP.Events;
using AgsXMPP.Protocol.Client;
using AgsXMPP.Protocol.Query.Bind;
using AgsXMPP.Protocol.Query.Session;
using AgsXMPP.Protocol.Sasl;
using AgsXMPP.Protocol.Stream;

using AgsXMPP.Xml;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Sasl
{
	using IqCB = PacketGrabberCallback<IqGrabber, IQ>;

	/// <summary>
	/// Summary description for SaslHandler.
	/// </summary>
	internal class SaslHandler : IDisposable
	{
		protected internal EventEmitter<SaslEventHandler> m_OnSaslStart = new EventEmitter<SaslEventHandler>();
		protected internal EventEmitter<ObjectHandler> m_OnSaslEnd = new EventEmitter<ObjectHandler>();

		public event SaslEventHandler OnSaslStart
		{
			add => this.m_OnSaslStart.Register(value);
			remove => this.m_OnSaslStart.Unregister(value);
		}

		public event ObjectHandler OnSaslEnd
		{
			add => this.m_OnSaslEnd.Register(value);
			remove => this.m_OnSaslEnd.Unregister(value);
		}

		private XmppClientConnection m_connection = null;
		private Mechanism m_Mechanism = null;
		private bool disposed = false;

		public SaslHandler(XmppClientConnection conn)
		{
			this.m_connection = conn;
			this.m_connection.StreamParser.OnStreamElement += new StreamHandler(this.OnStreamElement);
		}

		// Use C# destructor syntax for finalization code.
		// This destructor will run only if the Dispose method 
		// does not get called.
		// It gives your base class the opportunity to finalize.
		// Do not provide destructors in types derived from this class.
		~SaslHandler()
		{
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			this.Dispose(false);
		}

		internal void OnStreamElement(object sender, Node e)
		{
			if (this.m_connection.State == XmppConnectionState.Securing
				|| this.m_connection.State == XmppConnectionState.StartCompression)
				return;

			if (e.GetType() == typeof(StreamFeatures))
			{
				var f = e as StreamFeatures;
				if (!this.m_connection.Authenticated)
				{
					// RECV: <stream:features><mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
					//			<mechanism>DIGEST-MD5</mechanism><mechanism>PLAIN</mechanism>
					//			</mechanisms>
					//			<register xmlns='http://jabber.org/features/iq-register'/>
					//		</stream:features>
					// SENT: <auth mechanism="DIGEST-MD5" xmlns="urn:ietf:params:xml:ns:xmpp-sasl"/>				
					// Select a SASL mechanism

					var args = new SaslEventArgs(f.Mechanisms);

					this.m_OnSaslStart.Invoke(this, args);

					if (args.Auto == true)
					{
						// Library handles the Sasl stuff
						if (f.Mechanisms != null)
						{
							if (this.m_connection.UseStartTLS == false && this.m_connection.UseSSL == false
								&& f.Mechanisms.SupportsMechanism(MechanismType.X_GOOGLE_TOKEN))
							{
								// This is the only way to connect to GTalk on a unsecure Socket for now
								// Secure authentication is done over https requests to pass the
								// authentication credentials on a secure connection
								args.Mechanism = Protocol.Sasl.Mechanism.GetMechanismName(MechanismType.X_GOOGLE_TOKEN);
							}
							else if (f.Mechanisms.SupportsMechanism(MechanismType.DIGEST_MD5))
							{
								args.Mechanism = Protocol.Sasl.Mechanism.GetMechanismName(MechanismType.DIGEST_MD5);
							}
							else if (f.Mechanisms.SupportsMechanism(MechanismType.PLAIN))
							{
								args.Mechanism = Protocol.Sasl.Mechanism.GetMechanismName(MechanismType.PLAIN);
							}
							else
							{
								args.Mechanism = null;
							}
						}
						else
						{
							// Hack for Google
							// TODO: i don't think we need this anymore. This was in an very early version of the gtalk server.
							args.Mechanism = null;
							//args.Mechanism = agsXMPP.protocol.sasl.Mechanism.GetMechanismName(agsXMPP.protocol.sasl.MechanismType.PLAIN);
						}
					}
					if (args.Mechanism != null)
					{
						this.m_Mechanism = Factory.SaslFactory.GetMechanism(args.Mechanism);
						// Set properties for the SASL mechanism
						this.m_Mechanism.Username = this.m_connection.Username;
						this.m_Mechanism.Password = this.m_connection.Password;
						this.m_Mechanism.Server = this.m_connection.Server;
						// Call Init Method on the mechanism
						this.m_Mechanism.Init(this.m_connection);
					}
					else
					{
						this.m_connection.RequestLoginInfo();
					}
				}
				else if (!this.m_connection.Binded)
				{
					if (f.SupportsBind)
					{
						this.m_connection.DoChangeXmppConnectionState(XmppConnectionState.Binding);

						BindIq bIq;
						if (this.m_connection.Resource == null || this.m_connection.Resource.Length == 0)
							bIq = new BindIq(IQType.Set, new Jid(this.m_connection.Server));
						else
							bIq = new BindIq(IQType.Set, new Jid(this.m_connection.Server), this.m_connection.Resource);

						this.m_connection.IqGrabber.SendIq(bIq, new IqCB(this.BindResult), null);
					}
				}

			}
			else if (e.GetType() == typeof(Challenge))
			{
				if (this.m_Mechanism != null && !this.m_connection.Authenticated)
				{
					this.m_Mechanism.Parse(e);
				}
			}
			else if (e.GetType() == typeof(Success))
			{
				// SASL authentication was successfull
				this.m_OnSaslEnd?.Invoke(this);
				this.m_connection.DoChangeXmppConnectionState(XmppConnectionState.Authenticated);
				this.m_Mechanism = null;
				this.m_connection.Reset();
			}
			else if (e.GetType() == typeof(Failure))
			{
				// Authentication failure
				this.m_connection.FireOnAuthError(e as Element);
			}
		}

		internal void DoBind()
		{
			this.m_connection.DoChangeXmppConnectionState(XmppConnectionState.Binding);

			BindIq iq;
			if (this.m_connection.Resource == null || this.m_connection.Resource.Length == 0)
				iq = new BindIq(IQType.Set, new Jid(this.m_connection.Server));
			else
				iq = new BindIq(IQType.Set, new Jid(this.m_connection.Server), this.m_connection.Resource);

			this.m_connection.IqGrabber.SendIq(iq, new IqCB(this.BindResult), null);
		}

		private void BindResult(object sender, IQ iq, object data)
		{
			// Once the server has generated a resource identifier for the client or accepted the resource 
			// identifier provided by the client, it MUST return an IQ stanza of type "result" 
			// to the client, which MUST include a <jid/> child element that specifies the full JID for 
			// the connected resource as determined by the server:

			// Server informs client of successful resource binding: 
			// <iq type='result' id='bind_2'>
			//  <bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'>
			//    <jid>somenode@example.com/someresource</jid>
			//  </bind>
			// </iq>
			if (iq.Type == IQType.Result)
			{
				// i assume the server could assign another resource here to the client
				// so grep the resource assigned by the server now
				var bind = iq.SelectSingleElement(typeof(Bind));
				if (bind != null)
				{
					var jid = ((Bind)bind).Jid;
					this.m_connection.Resource = jid.Resource;
					this.m_connection.Username = jid.User;
				}

				this.m_connection.m_Binded = true;
				this.m_connection.DoChangeXmppConnectionState(XmppConnectionState.Binded);
				this.m_connection.DoRaiseEventBinded();

				// success, so start the session now
				this.m_connection.DoChangeXmppConnectionState(XmppConnectionState.StartSession);
				var sIq = new SessionIq(IQType.Set, new Jid(this.m_connection.Server));
				this.m_connection.IqGrabber.SendIq(sIq, new IqCB(this.SessionResult), null);

			}
			else if (iq.Type == IQType.Error)
			{
				// TODO, handle bind errors
			}
		}

		private void SessionResult(object sender, IQ iq, object data)
		{
			if (iq.Type == IQType.Result)
			{
				this.m_connection.DoChangeXmppConnectionState(XmppConnectionState.SessionStarted);
				this.m_connection.RaiseOnLogin();

			}
			else if (iq.Type == IQType.Error)
			{

			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
		}

		// Dispose(bool disposing) executes in two distinct scenarios.
		// If disposing equals true, the method has been called directly
		// or indirectly by a user's code. Managed and unmanaged resources
		// can be disposed.
		// If disposing equals false, the method has been called by the 
		// runtime from inside the finalizer and you should not reference 
		// other objects. Only unmanaged resources can be disposed.
		private void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (!this.disposed)
			{
				// If disposing equals true, dispose all managed 
				// and unmanaged resources.
				if (disposing)
				{
					// Dispose managed resources.
					// Remove the event handler or we will be in trouble with too many events
					this.m_connection.StreamParser.OnStreamElement -= new StreamHandler(this.OnStreamElement);
					this.m_connection = null;
					this.m_Mechanism = null;
				}

				// Call the appropriate methods to clean up 
				// unmanaged resources here.
				// If disposing is false, 
				// only the following code is executed.

			}
			this.disposed = true;
		}


		#endregion
	}
}