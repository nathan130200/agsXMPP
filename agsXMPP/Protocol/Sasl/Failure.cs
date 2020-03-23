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

using AgsXMPP.Xml.Dom;

// <failure xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
//		<incorrect-encoding/>
// </failure>
namespace AgsXMPP.Protocol.sasl
{
	/// <summary>
	/// Summary description for Failure.
	/// </summary>
	public class Failure : Element
	{
		public Failure()
		{
			this.TagName = "failure";
			this.Namespace = URI.SASL;
		}

		public Failure(FailureCondition cond) : this()
		{
			this.Condition = cond;
		}

		public FailureCondition Condition
		{
			get
			{
				if (this.HasTag("aborted"))
					return FailureCondition.aborted;
				else if (this.HasTag("incorrect-encoding"))
					return FailureCondition.incorrect_encoding;
				else if (this.HasTag("invalid-authzid"))
					return FailureCondition.invalid_authzid;
				else if (this.HasTag("invalid-mechanism"))
					return FailureCondition.invalid_mechanism;
				else if (this.HasTag("mechanism-too-weak"))
					return FailureCondition.mechanism_too_weak;
				else if (this.HasTag("not-authorized"))
					return FailureCondition.not_authorized;
				else if (this.HasTag("temporary-auth-failure"))
					return FailureCondition.temporary_auth_failure;
				else
					return FailureCondition.UnknownCondition;
			}
			set
			{
				if (value == FailureCondition.aborted)
					this.SetTag("aborted");
				else if (value == FailureCondition.incorrect_encoding)
					this.SetTag("incorrect-encoding");
				else if (value == FailureCondition.invalid_authzid)
					this.SetTag("invalid-authzid");
				else if (value == FailureCondition.invalid_mechanism)
					this.SetTag("invalid-mechanism");
				else if (value == FailureCondition.mechanism_too_weak)
					this.SetTag("mechanism-too-weak");
				else if (value == FailureCondition.not_authorized)
					this.SetTag("not-authorized");
				else if (value == FailureCondition.temporary_auth_failure)
					this.SetTag("temporary-auth-failure");
			}
		}
	}
}
