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

// JEP-0086: Error Condition Mappings

// <stanza-kind to='sender' type='error'>
// [RECOMMENDED to include sender XML here]
// <error type='error-type'>
// <defined-condition xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/>
// <text xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'
// xml:lang='langcode'>
// OPTIONAL descriptive text
// </text>
// [OPTIONAL application-specific condition element]
// </error>
// </stanza-kind>

// Legacy Error
// <error code="501">Not Implemented</error>

// XMPP Style Error
// <error code='404' type='cancel'>
//		<item-not-found xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/>
// </error>

namespace agsXMPP.Protocol.client
{
	// XMPP error condition  		XMPP error type  	Legacy error code
	// <bad-request/> 				modify 				400
	// <conflict/> 					cancel 				409
	// <feature-not-implemented/> 	cancel 				501
	// <forbidden/> 				auth 				403
	// <gone/> 						modify 				302 (permanent)
	// <internal-server-error/> 	wait 				500
	// <item-not-found/> 			cancel 				404
	// <jid-malformed/> 			modify 				400
	// <not-acceptable/> 			modify 				406
	// <not-allowed/> 				cancel 				405
	// <not-authorized/> 			auth 				401
	// <payment-required/> 			auth 				402
	// <recipient-unavailable/> 	wait 				404
	// <redirect/> 					modify 				302 (temporary)
	// <registration-required/> 	auth 				407
	// <remote-server-not-found/> 	cancel 				404
	// <remote-server-timeout/> 	wait 				504
	// <resource-constraint/> 		wait 				500
	// <service-unavailable/> 		cancel 				503
	// <subscription-required/> 	auth 				407
	// <undefined-condition/> 		[any] 				500
	// <unexpected-request/> 		wait 				400

	public enum ErrorCondition
	{
		BadRequest,
		Conflict,
		FeatureNotImplemented,
		Forbidden,
		Gone,
		InternalServerError,
		ItemNotFound,
		JidMalformed,
		NotAcceptable,
		NotAllowed,
		NotAuthorized,
		PaymentRequired,
		RecipientUnavailable,
		Redirect,
		RegistrationRequired,
		RemoteServerNotFound,
		RemoteServerTimeout,
		ResourceConstraint,
		ServiceUnavailable,
		SubscriptionRequired,
		UndefinedCondition,
		UnexpectedRequest
	}

	// The value of the <error/> element's 'type' attribute MUST be one of the following:
	// * cancel -- do not retry (the error is unrecoverable)
	// * continue -- proceed (the condition was only a warning)
	// * modify -- retry after changing the data sent
	// * auth -- retry after providing credentials
	// * wait -- retry after waiting (the error is temporary)
	public enum ErrorType
	{
		cancel,
		@continue,
		modify,
		auth,
		wait
	}


	/// <summary>
	/// The legacy Error Code
	/// </summary>
	public enum ErrorCode
	{
		/// <summary>
		/// Bad request
		/// </summary>
		BadRequest = 400,
		/// <summary>
		/// Unauthorized
		/// </summary>
		Unauthorized = 401,
		/// <summary>
		/// Payment required
		/// </summary>
		PaymentRequired = 402,
		/// <summary>
		/// Forbidden
		/// </summary>
		Forbidden = 403,
		/// <summary>
		/// Not found
		/// </summary>
		NotFound = 404,
		/// <summary>
		/// Not allowed
		/// </summary>
		NotAllowed = 405,
		/// <summary>
		/// Not acceptable
		/// </summary>
		NotAcceptable = 406,
		/// <summary>
		/// Registration required 
		/// </summary>
		RegistrationRequired = 407,
		/// <summary>
		/// Request timeout
		/// </summary>
		RequestTimeout = 408,
		/// <summary>
		/// Conflict
		/// </summary>
		Conflict = 409,
		/// <summary>
		/// Internal server error
		/// </summary>
		InternalServerError = 500,
		/// <summary>
		/// Not implemented
		/// </summary>
		NotImplemented = 501,
		/// <summary>
		/// Remote server error
		/// </summary>
		RemoteServerError = 502,
		/// <summary>
		/// Service unavailable
		/// </summary>
		ServiceUnavailable = 503,
		/// <summary>
		/// Remote server timeout
		/// </summary>
		RemoteServerTimeout = 504,
		/// <summary>
		/// Disconnected
		/// </summary>
		Disconnected = 510
	}


	/// <summary>
	/// Summary description for Error.
	/// </summary>
	public class Error : Element
	{

		#region << Constructors >>
		public Error()
		{
			this.Namespace = Namespaces.CLIENT;
			this.TagName = "error";
		}


		public Error(int code) : this()
		{
			this.SetAttribute("code", code.ToString());
		}

		public Error(ErrorCode code) : this()
		{
			this.SetAttribute("code", (int)code);
		}

		public Error(ErrorType type) : this()
		{
			this.Type = type;
		}

		/// <summary>
		/// Creates an error Element according the the condition
		/// The type attrib as added automatically as decribed in the XMPP specs
		/// This is the prefered way to create error Elements
		/// </summary>
		/// <param name="condition"></param>
		public Error(ErrorCondition condition) : this()
		{
			this.Condition = condition;
		}

		public Error(ErrorType type, ErrorCondition condition) : this(type)
		{
			this.Condition = condition;
		}
		#endregion

		/// <summary>
		/// The error Description
		/// </summary>
		public string Message
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.Value = value;
			}
		}

		public ErrorCode Code
		{
			get
			{
				return (ErrorCode)this.GetAttributeInt("code");
			}
			set
			{
				this.SetAttribute("code", (int)value);
			}
		}

		public ErrorType Type
		{
			get
			{
				return (ErrorType)this.GetAttributeEnum("type", typeof(ErrorType));
			}
			set
			{
				this.SetAttribute("type", value.ToString());
			}
		}

		public ErrorCondition Condition
		{
			get
			{
				if (this.HasTag("bad-request"))                  // <bad-request/> 
					return ErrorCondition.BadRequest;
				else if (this.HasTag("conflict"))                // <conflict/> 
					return ErrorCondition.Conflict;
				else if (this.HasTag("feature-not-implemented"))// <feature-not-implemented/>
					return ErrorCondition.FeatureNotImplemented;
				else if (this.HasTag("forbidden"))               // <forbidden/> 
					return ErrorCondition.Forbidden;
				else if (this.HasTag("gone"))                    // <gone/>
					return ErrorCondition.Gone;
				else if (this.HasTag("internal-server-error"))   // <internal-server-error/>
					return ErrorCondition.InternalServerError;
				else if (this.HasTag("item-not-found"))          // <item-not-found/> 
					return ErrorCondition.ItemNotFound;
				else if (this.HasTag("jid-malformed"))           // <jid-malformed/>
					return ErrorCondition.JidMalformed;
				else if (this.HasTag("not-acceptable"))          // <not-acceptable/> 
					return ErrorCondition.NotAcceptable;
				else if (this.HasTag("not-authorized"))          // <not-authorized/>
					return ErrorCondition.NotAuthorized;
				else if (this.HasTag("payment-required"))        // <payment-required/>
					return ErrorCondition.PaymentRequired;
				else if (this.HasTag("recipient-unavailable"))   // <recipient-unavailable/>
					return ErrorCondition.RecipientUnavailable;
				else if (this.HasTag("redirect"))                // <redirect/>
					return ErrorCondition.Redirect;
				else if (this.HasTag("registration-required"))   // <registration-required/>
					return ErrorCondition.RegistrationRequired;
				else if (this.HasTag("remote-server-not-found")) // <remote-server-not-found/> 
					return ErrorCondition.RemoteServerNotFound;
				else if (this.HasTag("remote-server-timeout"))   // <remote-server-timeout/> 
					return ErrorCondition.RemoteServerTimeout;
				else if (this.HasTag("resource-constraint"))     // <resource-constraint/>
					return ErrorCondition.ResourceConstraint;
				else if (this.HasTag("service-unavailable"))     // <service-unavailable/> 
					return ErrorCondition.ServiceUnavailable;
				else if (this.HasTag("subscription-required"))   // <subscription-required/> 
					return ErrorCondition.SubscriptionRequired;
				else if (this.HasTag("undefined-condition"))     // <undefined-condition/> 
					return ErrorCondition.UndefinedCondition;
				else if (this.HasTag("unexpected-request"))      // <unexpected-request/> 
					return ErrorCondition.UnexpectedRequest;
				else
					return ErrorCondition.UndefinedCondition;

			}
			set
			{
				switch (value)
				{
					case ErrorCondition.BadRequest:
						this.SetTag("bad-request", "", Namespaces.STANZAS);
						this.Type = ErrorType.modify;
						break;
					case ErrorCondition.Conflict:
						this.SetTag("conflict", "", Namespaces.STANZAS);
						this.Type = ErrorType.cancel;
						break;
					case ErrorCondition.FeatureNotImplemented:
						this.SetTag("feature-not-implemented", "", Namespaces.STANZAS);
						this.Type = ErrorType.cancel;
						break;
					case ErrorCondition.Forbidden:
						this.SetTag("forbidden", "", Namespaces.STANZAS);
						this.Type = ErrorType.auth;
						break;
					case ErrorCondition.Gone:
						this.SetTag("gone", "", Namespaces.STANZAS);
						this.Type = ErrorType.modify;
						break;
					case ErrorCondition.InternalServerError:
						this.SetTag("internal-server-error", "", Namespaces.STANZAS);
						this.Type = ErrorType.wait;
						break;
					case ErrorCondition.ItemNotFound:
						this.SetTag("item-not-found", "", Namespaces.STANZAS);
						this.Type = ErrorType.cancel;
						break;
					case ErrorCondition.JidMalformed:
						this.SetTag("jid-malformed", "", Namespaces.STANZAS);
						this.Type = ErrorType.modify;
						break;
					case ErrorCondition.NotAcceptable:
						this.SetTag("not-acceptable", "", Namespaces.STANZAS);
						this.Type = ErrorType.modify;
						break;
					case ErrorCondition.NotAllowed:
						this.SetTag("not-allowed", "", Namespaces.STANZAS);
						this.Type = ErrorType.cancel;
						break;
					case ErrorCondition.NotAuthorized:
						this.SetTag("not-authorized", "", Namespaces.STANZAS);
						this.Type = ErrorType.auth;
						break;
					case ErrorCondition.PaymentRequired:
						this.SetTag("payment-required", "", Namespaces.STANZAS);
						this.Type = ErrorType.auth;
						break;
					case ErrorCondition.RecipientUnavailable:
						this.SetTag("recipient-unavailable", "", Namespaces.STANZAS);
						this.Type = ErrorType.wait;
						break;
					case ErrorCondition.Redirect:
						this.SetTag("redirect", "", Namespaces.STANZAS);
						this.Type = ErrorType.modify;
						break;
					case ErrorCondition.RegistrationRequired:
						this.SetTag("registration-required", "", Namespaces.STANZAS);
						this.Type = ErrorType.auth;
						break;
					case ErrorCondition.RemoteServerNotFound:
						this.SetTag("remote-server-not-found", "", Namespaces.STANZAS);
						this.Type = ErrorType.cancel;
						break;
					case ErrorCondition.RemoteServerTimeout:
						this.SetTag("remote-server-timeout", "", Namespaces.STANZAS);
						this.Type = ErrorType.wait;
						break;
					case ErrorCondition.ResourceConstraint:
						this.SetTag("resource-constraint", "", Namespaces.STANZAS);
						this.Type = ErrorType.wait;
						break;
					case ErrorCondition.ServiceUnavailable:
						this.SetTag("service-unavailable", "", Namespaces.STANZAS);
						this.Type = ErrorType.cancel;
						break;
					case ErrorCondition.SubscriptionRequired:
						this.SetTag("subscription-required", "", Namespaces.STANZAS);
						this.Type = ErrorType.auth;
						break;
					case ErrorCondition.UndefinedCondition:
						this.SetTag("undefined-condition", "", Namespaces.STANZAS);
						// could be any
						break;
					case ErrorCondition.UnexpectedRequest:
						this.SetTag("unexpected-request", "", Namespaces.STANZAS);
						this.Type = ErrorType.wait;
						break;

				}
			}
		}
	}
}