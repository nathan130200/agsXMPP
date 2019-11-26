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

using agsXMPP.Protocol.client;
using agsXMPP.Protocol.iq.bind;
using agsXMPP.Protocol.iq.session;
using agsXMPP.Protocol.sasl;
using agsXMPP.Protocol.stream;

using agsXMPP.Xml;
using agsXMPP.Xml.Dom;

namespace agsXMPP.Sasl
{
	/// <summary>
	/// Summary description for SaslHandler.
	/// </summary>
	internal class SaslHandler : IDisposable
	{
		public event SaslEventHandler OnSaslStart;
		public event ObjectHandler OnSaslEnd;

		private XmppClientConnection m_XmppClient = null;
		private Mechanism m_Mechanism = null;
		// Track whether Dispose has been called.
		private bool disposed = false;

		public SaslHandler(XmppClientConnection conn)
		{
			this.m_XmppClient = conn;

			this.m_XmppClient.StreamParser.OnStreamElement += new StreamHandler(this.OnStreamElement);
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
			if (this.m_XmppClient.XmppConnectionState == XmppConnectionState.Securing
				|| this.m_XmppClient.XmppConnectionState == XmppConnectionState.StartCompression)
				return;

			if (e.GetType() == typeof(StreamFeatures))
			{
				var f = e as StreamFeatures;
				if (!this.m_XmppClient.Authenticated)
				{
					// RECV: <stream:features><mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
					//			<mechanism>DIGEST-MD5</mechanism><mechanism>PLAIN</mechanism>
					//			</mechanisms>
					//			<register xmlns='http://jabber.org/features/iq-register'/>
					//		</stream:features>
					// SENT: <auth mechanism="DIGEST-MD5" xmlns="urn:ietf:params:xml:ns:xmpp-sasl"/>				
					// Select a SASL mechanism

					var args = new SaslEventArgs(f.Mechanisms);

					OnSaslStart?.Invoke(this, args);

					if (args.Auto == true)
					{
						// Library handles the Sasl stuff
						if (f.Mechanisms != null)
						{
							if (this.m_XmppClient.UseStartTLS == false && this.m_XmppClient.UseSSL == false
								&& f.Mechanisms.SupportsMechanism(MechanismType.X_GOOGLE_TOKEN))
							{
								// This is the only way to connect to GTalk on a unsecure Socket for now
								// Secure authentication is done over https requests to pass the
								// authentication credentials on a secure connection
								args.Mechanism = Protocol.sasl.Mechanism.GetMechanismName(MechanismType.X_GOOGLE_TOKEN);
							}
							else if (f.Mechanisms.SupportsMechanism(MechanismType.DIGEST_MD5))
							{
								args.Mechanism = Protocol.sasl.Mechanism.GetMechanismName(MechanismType.DIGEST_MD5);
							}
							else if (f.Mechanisms.SupportsMechanism(MechanismType.PLAIN))
							{
								args.Mechanism = Protocol.sasl.Mechanism.GetMechanismName(MechanismType.PLAIN);
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
						this.m_Mechanism.Username = this.m_XmppClient.Username;
						this.m_Mechanism.Password = this.m_XmppClient.Password;
						this.m_Mechanism.Server = this.m_XmppClient.Server;
						// Call Init Method on the mechanism
						this.m_Mechanism.Init(this.m_XmppClient);
					}
					else
					{
						this.m_XmppClient.RequestLoginInfo();
					}
				}
				else if (!this.m_XmppClient.Binded)
				{
					if (f.SupportsBind)
					{
						this.m_XmppClient.DoChangeXmppConnectionState(XmppConnectionState.Binding);

						BindIq bIq;
						if (this.m_XmppClient.Resource == null || this.m_XmppClient.Resource.Length == 0)
							bIq = new BindIq(IQType.set, new Jid(this.m_XmppClient.Server));
						else
							bIq = new BindIq(IQType.set, new Jid(this.m_XmppClient.Server), this.m_XmppClient.Resource);

						this.m_Xmppclient.IQGrabber.SendIq(bIq, new IqCB(this.BindResult), null);
					}
				}

			}
			else if (e.GetType() == typeof(Challenge))
			{
				if (this.m_Mechanism != null && !this.m_XmppClient.Authenticated)
				{
					this.m_Mechanism.Parse(e);
				}
			}
			else if (e.GetType() == typeof(Success))
			{
				// SASL authentication was successfull
				OnSaslEnd?.Invoke(this);

				this.m_XmppClient.DoChangeXmppConnectionState(XmppConnectionState.Authenticated);

				this.m_Mechanism = null;

				this.m_XmppClient.Reset();
			}
			else if (e.GetType() == typeof(Failure))
			{
				// Authentication failure
				this.m_XmppClient.FireOnAuthError(e as Element);
			}
		}

		internal void DoBind()
		{
			this.m_XmppClient.DoChangeXmppConnectionState(XmppConnectionState.Binding);

			BindIq bIq;
			if (this.m_XmppClient.Resource == null || this.m_XmppClient.Resource.Length == 0)
				bIq = new BindIq(IQType.set, new Jid(this.m_XmppClient.Server));
			else
				bIq = new BindIq(IQType.set, new Jid(this.m_XmppClient.Server), this.m_XmppClient.Resource);

			this.m_Xmppclient.IQGrabber.SendIq(bIq, new IqCB(this.BindResult), null);
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
			if (iq.Type == IQType.result)
			{
				// i assume the server could assign another resource here to the client
				// so grep the resource assigned by the server now
				var bind = iq.SelectSingleElement(typeof(Bind));
				if (bind != null)
				{
					var jid = ((Bind)bind).Jid;
					this.m_XmppClient.Resource = jid.Resource;
					this.m_XmppClient.Username = jid.User;
				}

				this.m_XmppClient.DoChangeXmppConnectionState(XmppConnectionState.Binded);
				this.m_XmppClient.m_Binded = true;

				this.m_XmppClient.DoRaiseEventBinded();

				// success, so start the session now
				this.m_XmppClient.DoChangeXmppConnectionState(XmppConnectionState.StartSession);
				var sIq = new SessionIq(IQType.set, new Jid(this.m_XmppClient.Server));
				this.m_Xmppclient.IQGrabber.SendIq(sIq, new IqCB(this.SessionResult), null);

			}
			else if (iq.Type == IQType.error)
			{
				// TODO, handle bind errors
			}
		}

		private void SessionResult(object sender, IQ iq, object data)
		{
			if (iq.Type == IQType.result)
			{
				this.m_XmppClient.DoChangeXmppConnectionState(XmppConnectionState.SessionStarted);
				this.m_XmppClient.RaiseOnLogin();

			}
			else if (iq.Type == IQType.error)
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
					this.m_XmppClient.StreamParser.OnStreamElement -= new StreamHandler(this.OnStreamElement);
					this.m_XmppClient = null;
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