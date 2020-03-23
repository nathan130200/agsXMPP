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
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Query.Disco
{
	/// <summary>
	/// Disco feature
	/// </summary>
	/// <remarks>
	/// see: http://www.jabber.org/registrar/disco-features.html
	/// </remarks>
	public class DiscoFeature : Element
	{
		/*
        <iq type='result'
            from='plays.shakespeare.lit'
            to='romeo@montague.net/orchard'
            id='info1'>
        <query xmlns='http://jabber.org/protocol/disco#info'>
            <identity
                category='conference'
                type='text'
                name='Play-Specific Chatrooms'/>
            <identity
                category='directory'
                type='chatroom'
                name='Play-Specific Chatrooms'/>
            <feature var='http://jabber.org/protocol/disco#info'/>
            <feature var='http://jabber.org/protocol/disco#items'/>
            <feature var='http://jabber.org/protocol/muc'/>
            <feature var='jabber:iq:register'/>
            <feature var='jabber:iq:search'/>
            <feature var='jabber:iq:time'/>
            <feature var='jabber:iq:version'/>
        </query>
        </iq>
        */
		#region << Constructors >>
		public DiscoFeature()
		{
			this.TagName = "feature";
			this.Namespace = URI.DISCO_INFO;
		}

		public DiscoFeature(string var) : this()
		{
			this.Var = var;
		}
		#endregion

		/// <summary>
		/// feature name or namespace
		/// </summary>
		public string Var
		{
			get { return this.GetAttribute("var"); }
			set { this.SetAttribute("var", value); }
		}
	}
}
