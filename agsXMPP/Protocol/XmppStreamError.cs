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

using agsXMPP.Xml.Dom;

namespace agsXMPP.Protocol
{
	// <stream:error>Invalid handshake</stream:error>
	// <stream:error>Socket override by another connection.</stream:error>

	/// <summary>
	/// Stream Errors &lt;stream:error&gt;
	/// </summary>
	public class XmppStreamError : Element
	{
		public XmppStreamError()
		{
			this.TagName = "error";
			this.Namespace = Namespaces.STREAM;
		}

		public XmppStreamError(XmppStreamErrorCondition condition) : this()
		{
			this.Condition = condition;
		}

		/*
		public Error(string msg) : this()
		{
			Message = msg;
		}

		/// <summary>
		/// The error Description
		/// </summary>
		public string Message
		{
			get	{ return this.Value;  }
			set	{ this.Value = value; }
		}
        */

		public XmppStreamErrorCondition Condition
		{
			get
			{
				if (this.HasTag("bad-format"))
					return XmppStreamErrorCondition.BadFormat;
				else if (this.HasTag("bad-namespace-prefix"))
					return XmppStreamErrorCondition.BadNamespacePrefix;
				else if (this.HasTag("conflict"))
					return XmppStreamErrorCondition.Conflict;
				else if (this.HasTag("connection-timeout"))
					return XmppStreamErrorCondition.ConnectionTimeout;
				else if (this.HasTag("host-gone"))
					return XmppStreamErrorCondition.HostGone;
				else if (this.HasTag("host-unknown"))
					return XmppStreamErrorCondition.HostUnknown;
				else if (this.HasTag("improper-addressing"))
					return XmppStreamErrorCondition.ImproperAddressing;
				else if (this.HasTag("internal-server-error"))
					return XmppStreamErrorCondition.InternalServerError;
				else if (this.HasTag("invalid-from"))
					return XmppStreamErrorCondition.InvalidFrom;
				else if (this.HasTag("invalid-id"))
					return XmppStreamErrorCondition.InvalidId;
				else if (this.HasTag("invalid-namespace"))
					return XmppStreamErrorCondition.InvalidNamespace;
				else if (this.HasTag("invalid-xml"))
					return XmppStreamErrorCondition.InvalidXml;
				else if (this.HasTag("not-authorized"))
					return XmppStreamErrorCondition.NotAuthorized;
				else if (this.HasTag("policy-violation"))
					return XmppStreamErrorCondition.PolicyViolation;
				else if (this.HasTag("remote-connection-failed"))
					return XmppStreamErrorCondition.RemoteConnectionFailed;
				else if (this.HasTag("resource-constraint"))
					return XmppStreamErrorCondition.ResourceConstraint;
				else if (this.HasTag("restricted-xml"))
					return XmppStreamErrorCondition.RestrictedXml;
				else if (this.HasTag("see-other-host"))
					return XmppStreamErrorCondition.SeeOtherHost;
				else if (this.HasTag("system-shutdown"))
					return XmppStreamErrorCondition.SystemShutdown;
				else if (this.HasTag("undefined-condition"))
					return XmppStreamErrorCondition.UndefinedCondition;
				else if (this.HasTag("unsupported-encoding"))
					return XmppStreamErrorCondition.UnsupportedEncoding;
				else if (this.HasTag("unsupported-stanza-type"))
					return XmppStreamErrorCondition.UnsupportedStanzaType;
				else if (this.HasTag("unsupported-version"))
					return XmppStreamErrorCondition.UnsupportedVersion;
				else if (this.HasTag("xml-not-well-formed"))
					return XmppStreamErrorCondition.XmlNotWellFormed;
				else
					return XmppStreamErrorCondition.UnknownCondition;
			}

			set
			{
				switch (value)
				{
					case XmppStreamErrorCondition.BadFormat:
						this.SetTag("bad-format", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.BadNamespacePrefix:
						this.SetTag("bad-namespace-prefix", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.Conflict:
						this.SetTag("conflict", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.ConnectionTimeout:
						this.SetTag("connection-timeout", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.HostGone:
						this.SetTag("host-gone", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.HostUnknown:
						this.SetTag("host-unknown", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.ImproperAddressing:
						this.SetTag("improper-addressing", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.InternalServerError:
						this.SetTag("internal-server-error", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.InvalidFrom:
						this.SetTag("invalid-from", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.InvalidId:
						this.SetTag("invalid-id", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.InvalidNamespace:
						this.SetTag("invalid-namespace", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.InvalidXml:
						this.SetTag("invalid-xml", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.NotAuthorized:
						this.SetTag("not-authorized", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.PolicyViolation:
						this.SetTag("policy-violation", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.RemoteConnectionFailed:
						this.SetTag("remote-connection-failed", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.ResourceConstraint:
						this.SetTag("resource-constraint", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.RestrictedXml:
						this.SetTag("restricted-xml", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.SeeOtherHost:
						this.SetTag("see-other-host", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.SystemShutdown:
						this.SetTag("system-shutdown", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.UndefinedCondition:
						this.SetTag("undefined-condition", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.UnsupportedEncoding:
						this.SetTag("unsupported-encoding", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.UnsupportedStanzaType:
						this.SetTag("unsupported-stanza-type", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.UnsupportedVersion:
						this.SetTag("unsupported-version", "", Namespaces.STREAMS);
						break;
					case XmppStreamErrorCondition.XmlNotWellFormed:
						this.SetTag("xml-not-well-formed", "", Namespaces.STREAMS);
						break;
					default:
						return;

				}
			}
		}

		/// <summary>
		/// <para>
		/// The &lt;text/&gt; element is OPTIONAL. If included, it SHOULD be used only to provide descriptive or diagnostic information
		/// that supplements the meaning of a defined condition or application-specific condition. 
		/// </para>
		/// <para>
		/// It SHOULD NOT be interpreted programmatically by an application.
		/// It SHOULD NOT be used as the error message presented to a user, but MAY be shown in addition to the error message 
		/// associated with the included condition element (or elements).
		/// </para>
		/// </summary>
		public string Text
		{
			get { return this.GetTag("text"); }
			set
			{
				this.SetTag("text", value, Namespaces.STREAMS);
			}
		}

	}
}
