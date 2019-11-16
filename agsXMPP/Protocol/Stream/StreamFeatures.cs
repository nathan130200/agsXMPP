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

using agsXMPP.Protocol.iq.bind;
using agsXMPP.Protocol.stream.features;
using agsXMPP.Protocol.stream.features.Compression;
using agsXMPP.Protocol.tls;
using agsXMPP.Xml.Dom;

//</stream:features>
// <stream:features>
//		<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
//			<mechanism>DIGEST-MD5</mechanism>
//			<mechanism>PLAIN</mechanism>
//		</mechanisms>
// </stream:features>

// <stream:features>
//		<starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'>
//			<required/>
//		</starttls>
//		<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
//			<mechanism>DIGEST-MD5</mechanism>
//			<mechanism>PLAIN</mechanism>
//		</mechanisms>
// </stream:features>

namespace agsXMPP.Protocol.stream
{
	/// <summary>
	/// Summary description for Features.
	/// </summary>
	public class StreamFeatures : Element
	{
		public StreamFeatures()
		{
			this.Prefix = "stream";
			this.TagName = "features";
			this.Namespace = Namespaces.STREAM;
		}

		public StartTls StartTls
		{
			get
			{
				return this.SelectSingleElement(typeof(StartTls)) as StartTls;
			}
			set
			{
				if (this.HasTag(typeof(StartTls)))
					this.RemoveTag(typeof(StartTls));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Bind Bind
		{
			get
			{
				return this.SelectSingleElement(typeof(Bind)) as Bind;
			}
			set
			{
				if (this.HasTag(typeof(Bind)))
					this.RemoveTag(typeof(Bind));

				if (value != null)
					this.AddChild(value);
			}
		}

		// <stream:stream from="beta.soapbox.net" xml:lang="de" id="373af7e9-6107-4729-8cea-e8b8ea05ceea" xmlns="jabber:client" version="1.0" xmlns:stream="http://etherx.jabber.org/streams">

		// <stream:features xmlns:stream="http://etherx.jabber.org/streams">
		//      <compression xmlns="http://jabber.org/features/compress"><method>zlib</method></compression>
		//      <starttls xmlns="urn:ietf:params:xml:ns:xmpp-tls" />
		//      <register xmlns="http://jabber.org/features/iq-register" />
		//      <auth xmlns="http://jabber.org/features/iq-auth" />
		//      <mechanisms xmlns="urn:ietf:params:xml:ns:xmpp-sasl">
		//          <mechanism>PLAIN</mechanism>
		//          <mechanism>DIGEST-MD5</mechanism>
		//          <mechanism>ANONYMOUS</mechanism>
		//      </mechanisms>
		// </stream:features>


		public Compression Compression
		{
			get { return this.SelectSingleElement(typeof(Compression)) as Compression; }
			set
			{
				if (this.HasTag(typeof(Compression)))
					this.RemoveTag(typeof(Compression));

				if (value != null)
					this.AddChild(value);
			}
		}

		public Register Register
		{
			get
			{
				return this.SelectSingleElement(typeof(Register)) as Register;
			}
			set
			{
				if (this.HasTag(typeof(Register)))
					this.RemoveTag(typeof(Register));

				if (value != null)
					this.AddChild(value);
			}
		}

		public sasl.Mechanisms Mechanisms
		{
			get
			{
				return this.SelectSingleElement(typeof(sasl.Mechanisms)) as sasl.Mechanisms;
			}
			set
			{
				if (this.HasTag(typeof(sasl.Mechanisms)))
					this.RemoveTag(typeof(sasl.Mechanisms));

				if (value != null)
					this.AddChild(value);
			}
		}

		public bool SupportsBind
		{
			get { return this.Bind != null ? true : false; }
		}

		public bool SupportsStartTls
		{
			get
			{
				return this.StartTls != null ? true : false;
			}
		}

		/// <summary>
		/// Is Stream Compression supported?
		/// </summary>
		public bool SupportsCompression
		{
			get
			{
				return this.Compression != null ? true : false;
			}
		}

		/// <summary>
		/// Is Registration supported?
		/// </summary>
		public bool SupportsRegistration
		{
			get
			{
				return this.Register != null ? true : false;
			}
		}


	}
}
