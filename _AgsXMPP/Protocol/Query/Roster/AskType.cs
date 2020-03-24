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

namespace AgsXMPP.Protocol.Query.Roster
{
	// jabber:iq:roster
	// <iq from="user@server.com/Office" id="doroster_1" type="result">
	//		<query xmlns="jabber:iq:roster">
	//			<item subscription="both" name="juiliet" jid="11111@icq.myjabber.net"><group>ICQ</group></item>
	//			<item subscription="both" name="roman" jid="22222@icq.myjabber.net"><group>ICQ</group></item>
	//			<item subscription="both" name="angie" jid="33333@icq.myjabber.net"><group>ICQ</group></item>
	//			<item subscription="both" name="bob" jid="44444@icq.myjabber.net"><group>ICQ</group></item>
	//		</query>
	// </iq> 

	public enum AskType
	{
		None = -1,

		[XmppEnumMember("subscribe")]
		Subscribe,

		[XmppEnumMember("unsubscribe")]
		Unsubscribe
	}
}
