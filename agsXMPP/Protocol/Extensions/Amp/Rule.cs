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

namespace agsXMPP.Protocol.extensions.amp
{
	public class Rule : Element
	{
		public Rule()
		{
			this.TagName = "rule";
			this.Namespace = Namespaces.AMP;
		}

		public Rule(Condition condition, string val, Action action)
			: this()
		{
			this.Condition = condition;
			this.Val = val;
			this.Action = action;
		}

		/// <summary>
		/// The 'value' attribute defines how the condition is matched. 
		/// This attribute MUST be present, and MUST NOT be an empty string (""). 
		/// The interpretation of this attribute's value is determined by the 'condition' attribute.
		/// </summary>
		public string Val
		{
			get { return this.GetAttribute("value"); }
			set { this.SetAttribute("value", value); }
		}

		/// <summary>
		/// The 'action' attribute defines the result for this rule. 
		/// This attribute MUST be present, and MUST be either a value defined in the Defined Actions section, 
		/// or one registered with the XMPP Registrar.
		/// </summary>
		public Action Action
		{
			get
			{
				return (Action)this.GetAttributeEnum("action", typeof(Action));
			}
			set
			{
				if (value == Action.Unknown)
					this.RemoveAttribute("action");
				else
					this.SetAttribute("action", value.ToString());
			}
		}

		/// <summary>
		/// The 'condition' attribute defines the overall condition this rule applies to. 
		/// This attribute MUST be present, and MUST be either a value defined in the Defined Conditions section, 
		/// or one registered with the XMPP Registrar.
		/// </summary>
		public Condition Condition
		{
			get
			{
				switch (this.GetAttribute("condition"))
				{
					case "deliver":
						return Condition.Deliver;
					case "expire-at":
						return Condition.ExprireAt;
					case "match-resource":
						return Condition.MatchResource;
					default:
						return Condition.Unknown;
				}
			}

			set
			{
				switch (value)
				{
					case Condition.Deliver:
						this.SetAttribute("condition", "deliver");
						break;
					case Condition.ExprireAt:
						this.SetAttribute("condition", "expire-at");
						break;
					case Condition.MatchResource:
						this.SetAttribute("condition", "match-resource");
						break;
				}
			}
		}
	}
}