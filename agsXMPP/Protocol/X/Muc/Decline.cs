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

namespace AgsXMPP.Protocol.x.muc
{
	/// <summary>
	/// <br/>
	/// <list type="bullet">
	/// <item>
	/// Example 45. Invitee Declines Invitation
	/// <code>
	/// &lt;message from='hecate@shakespeare.lit/broom' to='darkcave@macbeth.shakespeare.lit'&gt;<br/>
	///   &lt;x xmlns='http://jabber.org/protocol/muc#user'&gt;<br/>
	///     &lt;decline to='crone1@shakespeare.lit'&gt;<br/>
	///       &lt;reason&gt;<br/>
	///         Sorry, I'm too busy right now.<br/>
	///       &lt;/reason&gt;<br/>
	///     &lt;/decline&gt;<br/>
	///   &lt;/x&gt;<br/>
	/// &lt;/message&gt;<br/>
	/// </code>
	/// </item>
	/// <br/>
	/// <item>
	/// Example 46. Room Informs Invitor that Invitation Was Declined
	/// <code>
	/// &lt;message from='darkcave@macbeth.shakespeare.lit' to='crone1@shakespeare.lit/desktop'&gt;<br/>
	/// &lt;x xmlns='http://jabber.org/protocol/muc#user'&gt;<br/>
	///     &lt;decline from='hecate@shakespeare.lit'&gt;<br/>
	///       &lt;reason&gt;<br/>
	///      Sorry, I'm too busy right now.<br/>
	///       &lt;/reason&gt;<br/>
	///     &lt;/decline&gt;<br/>
	///   &lt;/x&gt;<br/>
	/// &lt;/message&gt;<br/>
	/// </code>
	/// </item>
	/// </list>
	/// </summary>
	public class Decline : Invitation
	{
		#region << Constructors >>
		public Decline() : base()
		{
			this.TagName = "decline";
		}

		public Decline(string reason) : this()
		{
			this.Reason = reason;
		}

		public Decline(Jid to) : this()
		{
			this.To = to;
		}

		public Decline(Jid to, string reason) : this()
		{
			this.To = to;
			this.Reason = reason;
		}
		#endregion
	}
}
