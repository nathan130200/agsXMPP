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
using AgsXMPP.Protocol.Client;
using AgsXMPP.Protocol.x.data;
using AgsXMPP.Protocol.x.muc.iq.admin;
using AgsXMPP.Protocol.x.muc.iq.owner;

namespace AgsXMPP.Protocol.x.muc
{
	/// <summary>
	/// A helper class for Multi User Chat
	/// </summary>
	public class MucManager
	{
		private XmppClientConnection m_connection = null;

		public MucManager(XmppClientConnection con)
		{
			this.m_connection = con;
		}


		#region << Invite >>
		/*
        <message
            from='crone1@shakespeare.lit/desktop'
            to='darkcave@macbeth.shakespeare.lit'>
          <x xmlns='http://jabber.org/protocol/muc#user'>
            <invite to='hecate@shakespeare.lit'>
              <reason>
                Hey Hecate, this is the place for all good witches!
              </reason>
            </invite>
          </x>
        </message>
        */

		/// <summary>
		/// Invite a contact to join a chatroom
		/// </summary>
		/// <param name="to">The Jid of the contact to invite</param>
		/// <param name="room">The Jid of the chatroom</param>
		public void Invite(Jid to, Jid room)
		{
			this.Invite(to, room, null);
		}

		/// <summary>
		/// Invite a contact to join a chatroom
		/// </summary>
		/// <param name="to">The Jid of the contact to invite</param>
		/// <param name="room">The Jid of the chatroom</param>
		public void Invite(Jid to, Jid room, string reason)
		{
			this.Invite(new Jid[] { to }, room, reason);
		}

		/// <summary>
		/// Invite multiple contacts to a chatroom
		/// </summary>
		/// <param name="jids"></param>
		/// <param name="room"></param>
		/// <param name="reason"></param>
		public void Invite(Jid[] jids, Jid room, string reason)
		{
			var msg = new Message();
			msg.To = room;

			var user = new User();
			foreach (var jid in jids)
			{
				if (reason != null)
					user.AddChild(new Invite(jid, reason));
				else
					user.AddChild(new Invite(jid));
			}

			msg.AddChild(user);

			this.m_connection.Send(msg);
		}
		#endregion


		#region << Decline Invite >>
		/// <summary>
		/// Decline a groupchat invitation
		/// </summary>
		/// <param name="to">the jid which invited us</param>
		/// <param name="room">to room to which we send the decline (this is normally the same room we were invited to)</param>
		public void Decline(Jid to, Jid room)
		{
			this.Decline(to, room, null);
		}

		/// <summary>
		/// Decline a groupchat invitation
		/// </summary>
		/// <param name="to">the jid which invited us</param>
		/// <param name="room">to room to which we send the decline (this is normally the same room we were invited to)</param>
		/// <param name="reason">reason why we decline the invitation</param>
		public void Decline(Jid to, Jid room, string reason)
		{
			var msg = new Message();
			msg.To = room;

			var user = new User();
			if (reason != null)
				user.Decline = new Decline(to, reason);
			else
				user.Decline = new Decline(to);

			msg.AddChild(user);

			this.m_connection.Send(msg);
		}
		#endregion

		/*
            <message
                from='darkcave@macbeth.shakespeare.lit/secondwitch'
                to='crone1@shakespeare.lit/desktop'
                type='groupchat'>
              <subject>Fire Burn and Cauldron Bubble!</subject>
            </message>
        */
		/// <summary>
		/// Change the subject of a room
		/// </summary>
		/// <param name="room"></param>
		/// <param name="newSubject"></param>
		public void ChangeSubject(Jid room, string newSubject)
		{
			this.ChangeSubject(room, newSubject, null);
		}

		/// <summary>
		/// Change the subject of a room
		/// </summary>
		/// <param name="room"></param>
		/// <param name="newSubject"></param>
		/// <param name="body"></param>
		public void ChangeSubject(Jid room, string newSubject, string body)
		{
			var msg = new Message();
			msg.Type = MessageType.GroupChat;
			msg.To = room;
			msg.Subject = newSubject;

			if (body != null)
				msg.Body = body;

			this.m_connection.Send(msg);
		}

		/// <summary>
		/// Change the Nickname in a room
		/// </summary>
		/// <param name="room"></param>
		/// <param name="newNick"></param>
		public void ChangeNickname(Jid room, string newNick)
		{
			var to = new Jid(room.ToString());
			to.Resource = newNick;

			var pres = new Presence();
			pres.To = to;

			this.m_connection.Send(pres);
		}

		/*
            <presence
                from='hag66@shakespeare.lit/pda'
                to='darkcave@macbeth.shakespeare.lit/thirdwitch'>
              <x xmlns='http://jabber.org/protocol/muc'/>
            </presence>
         */
		/// <summary>
		/// Join a chatroom
		/// </summary>
		/// <param name="room">jid of the room to join</param>
		/// <param name="nickname">nickname to use in the room</param>
		public void JoinRoom(Jid room, string nickname)
		{
			this.JoinRoom(room, nickname, null, false);
		}

		/// <summary>
		/// Join a chatroom
		/// </summary>
		/// <param name="room">jid of the room to join</param>
		/// <param name="nickname">nickname to use in the room</param>
		/// <param name="disableHistory">true for joining without chat room history</param>
		public void JoinRoom(Jid room, string nickname, bool disableHistory)
		{
			this.JoinRoom(room, nickname, null, disableHistory);
		}

		public void JoinRoom(Jid room, string nickname, string password)
		{
			this.JoinRoom(room, nickname, password, false);
		}

		/// <summary>
		/// Join a chatroom
		/// </summary>
		/// <param name="room">jid of the room to join</param>
		/// <param name="nickname">nickname to use in the room</param>
		/// <param name="password">password for password protected chat rooms</param>
		/// <param name="disableHistory">true for joining without chat room history</param>
		public void JoinRoom(Jid room, string nickname, string password, bool disableHistory)
		{
			/*
            <presence
                from='hag66@shakespeare.lit/pda'
                to='darkcave@macbeth.shakespeare.lit/thirdwitch'>
              <x xmlns='http://jabber.org/protocol/muc'>
                <password>cauldron</password>
              </x>
            </presence>

            join room and request no history
            <presence
                from='hag66@shakespeare.lit/pda'
                to='darkcave@macbeth.shakespeare.lit/thirdwitch'>
              <x xmlns='http://jabber.org/protocol/muc'>
                <history maxchars='0'/>
              </x>
            </presence>
            */

			var to = new Jid(room.ToString());
			to.Resource = nickname;

			var pres = new Presence();
			pres.To = to;
			var x = new Muc();
			if (password != null)
				x.Password = password;

			if (disableHistory)
			{
				var hist = new History();
				hist.MaxCharacters = 0;
				x.History = hist;
			}

			pres.AddChild(x);

			this.m_connection.Send(pres);
		}


		/// <summary>
		/// Leave a conference room
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		public void LeaveRoom(Jid room, string nickname)
		{
			var to = new Jid(room.ToString());
			to.Resource = nickname;

			var pres = new Presence();
			pres.To = to;
			pres.Type = PresenceType.Unavailable;

			this.m_connection.Send(pres);
		}

		/*
            9.1.2 Creating an Instant Room

            If the initial room owner wants to accept the default room configuration (i.e., create an "instant room"), the room owner MUST decline an initial configuration form by sending an IQ set to the <room@service> itself containing a <query/> element qualified by the 'http://jabber.org/protocol/muc#owner' namespace, where the only child of the <query/> is an empty <x/> element that is qualified by the 'jabber:x:data' namespace and that possesses a 'type' attribute whose value is "submit":

            Example 129. Owner Requests Instant Room

            <iq from='crone1@shakespeare.lit/desktop'
                id='create1'
                to='darkcave@macbeth.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#owner'>
                <x xmlns='jabber:x:data' type='submit'/>
              </query>
            </iq>
        */

		/// <summary>
		/// create an "instant room". This means you accept the default configuration and dont want to configure the room.
		/// </summary>
		/// <param name="room"></param>
		public void AcceptDefaultConfiguration(Jid room)
		{
			this.AcceptDefaultConfiguration(room, null, null);
		}

		/// <summary>
		/// create an "instant room". This means you accept the default configuration and dont want to configure the room.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		public void AcceptDefaultConfiguration(Jid room, IqCB cb)
		{
			this.AcceptDefaultConfiguration(room, cb, null);
		}

		/// <summary>
		/// create an "instant room". This means you accept the default configuration and dont want to configure the room.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		/// <param name="cbArgs"></param>
		public void AcceptDefaultConfiguration(Jid room, IqCB cb, object cbArgs)
		{
			var oIq = new OwnerIq(IQType.Set, room);
			oIq.Query.AddChild(new Data(XDataFormType.submit));

			if (cb == null)
				this.m_connection.Send(oIq);
			else
				this.m_connection.IqGrabber.SendIq(oIq, cb, cbArgs);
		}

		/*
            
            <iq from='crone1@shakespeare.lit/desktop'
                id='create1'
                to='darkcave@macbeth.shakespeare.lit'
                type='get'>
              <query xmlns='http://jabber.org/protocol/muc#owner'/>
            </iq>
      
         */

		/// <summary>
		/// Request the configuration form of a chatroom.
		/// You can request the from when creating a new room. or at any time later if you want to change the room configuration.
		/// Only room owners can request this from. Otherwise the service must return a 403 forbidden error
		/// </summary>
		/// <param name="room"></param>
		public void RequestConfigurationForm(Jid room)
		{
			this.RequestConfigurationForm(room, null, null);
		}

		/// <summary>
		/// Request the configuration form of a chatroom.
		/// You can request the from when creating a new room. or at any time later if you want to change the room configuration.
		/// Only room owners can request this from. Otherwise the service must return a 403 forbidden error
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		public void RequestConfigurationForm(Jid room, IqCB cb)
		{
			this.RequestConfigurationForm(room, cb, null);
		}

		/// <summary>
		/// Request the configuration form of a chatroom.
		/// You can request the from when creating a new room. or at any time later if you want to change the room configuration.
		/// Only room owners can request this from. Otherwise the service must return a 403 forbidden error
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		/// <param name="cbArgs"></param>
		public void RequestConfigurationForm(Jid room, IqCB cb, object cbArgs)
		{
			var oIq = new OwnerIq(IQType.Get, room);

			this.m_connection.IqGrabber.SendIq(oIq, cb, cbArgs);
		}

		/*
            <iq from='fluellen@shakespeare.lit/pda'
                id='kick1'
                to='harfleur@henryv.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item nick='pistol' role='none'>
                  <reason>Avaunt, you cullion!</reason>
                </item>
              </query>
            </iq>
         */

		/// <summary>
		/// Kick a accupant
		/// A moderator has permissions kick a visitor or participant from a room.
		/// The kick is normally performed based on the occupant's room nickname (though it MAY be based on the full JID)
		/// and is completed by setting the role of a participant or visitor to a value of "none".
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname">Nickname od the occupant to kick</param>
		public void KickOccupant(Jid room, string nickname)
		{
			this.KickOccupant(room, nickname, null, null, null);
		}

		/// <summary>
		/// Kick a accupant
		/// A moderator has permissions kick a visitor or participant from a room.
		/// The kick is normally performed based on the occupant's room nickname (though it MAY be based on the full JID)
		/// and is completed by setting the role of a participant or visitor to a value of "none".
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname">Nickname od the occupant to kick</param>
		/// <param name="reason">A optional reason why you kick this occupant</param>
		public void KickOccupant(Jid room, string nickname, string reason)
		{
			this.KickOccupant(room, nickname, reason, null, null);
		}


		/// <summary>
		/// Kick a accupant
		/// A moderator has permissions kick a visitor or participant from a room.
		/// The kick is normally performed based on the occupant's room nickname (though it MAY be based on the full JID)
		/// and is completed by setting the role of a participant or visitor to a value of "none".
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname">Nickname od the occupant to kick</param>
		/// <param name="reason">A optional reason why you kick this occupant</param>
		/// <param name="cb">Callback which is invoked with the result to this iq</param>        
		public void KickOccupant(Jid room, string nickname, string reason, IqCB cb)
		{
			this.KickOccupant(room, nickname, reason, cb, null);
		}

		/// <summary>
		/// Kick a accupant
		/// A moderator has permissions kick a visitor or participant from a room.
		/// The kick is normally performed based on the occupant's room nickname (though it MAY be based on the full JID)
		/// and is completed by setting the role of a participant or visitor to a value of "none".
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname">Nickname od the occupant to kick</param>
		/// <param name="reason">A optional reason why you kick this occupant</param>
		/// <param name="cb">Callback which is invoked with the result to this iq</param>
		/// <param name="cbArg">Callback which is invoked with the result to this iq</param>
		public void KickOccupant(Jid room, string nickname, string reason, IqCB cb, object cbArg)
		{
			this.ChangeRole(Role.none, room, nickname, reason, cb, cbArg);
		}

		/*
            Example 77. Moderator Grants Voice to a Visitor

            <iq from='crone1@shakespeare.lit/desktop'
                id='voice1'
                to='darkcave@macbeth.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item nick='thirdwitch'
                      role='participant'>
                  <reason>A worthy witch indeed!</reason>
                </item>
              </query>
            </iq>
        */

		/// <summary>
		/// 
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname"></param>
		public void GrantVoice(Jid room, string nickname)
		{
			this.GrantVoice(room, nickname, null, null, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		public void GrantVoice(Jid room, string nickname, string reason)
		{
			this.GrantVoice(room, nickname, reason, null, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>        
		public void GrantVoice(Jid room, string nickname, string reason, IqCB cb)
		{
			this.GrantVoice(room, nickname, reason, cb, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void GrantVoice(Jid room, string nickname, string reason, IqCB cb, object cbArg)
		{
			this.ChangeRole(Role.participant, room, nickname, reason, cb, cbArg);
		}

		/*
            Example 80. Moderator Revokes Voice from a Participant

            <iq from='crone1@shakespeare.lit/desktop'
                id='voice2'
                to='darkcave@macbeth.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item nick='thirdwitch'
                      role='visitor'/>
              </query>
            </iq>
        */

		/// <summary>
		/// In a moderated room, a moderator may want to revoke a participant's privileges to speak.
		/// The moderator can revoke voice from a participant by changing the participant's role to "visitor":
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname"></param>
		public void RevokeVoice(Jid room, string nickname)
		{
			this.RevokeVoice(room, nickname, null, null, null);
		}

		/// <summary>
		/// In a moderated room, a moderator may want to revoke a participant's privileges to speak.
		/// The moderator can revoke voice from a participant by changing the participant's role to "visitor":
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		public void RevokeVoice(Jid room, string nickname, string reason)
		{
			this.RevokeVoice(room, nickname, reason, null, null);
		}

		/// <summary>
		/// In a moderated room, a moderator may want to revoke a participant's privileges to speak.
		/// The moderator can revoke voice from a participant by changing the participant's role to "visitor":
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		public void RevokeVoice(Jid room, string nickname, string reason, IqCB cb)
		{
			this.RevokeVoice(room, nickname, reason, cb, null);
		}

		/// <summary>
		/// In a moderated room, a moderator may want to revoke a participant's privileges to speak.
		/// The moderator can revoke voice from a participant by changing the participant's role to "visitor":
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void RevokeVoice(Jid room, string nickname, string reason, IqCB cb, object cbArg)
		{
			this.ChangeRole(Role.visitor, room, nickname, reason, cb, cbArg);
		}

		/*
            A moderator in a moderated room may want to modify the voice list. To do so, the moderator first requests the voice list by querying the room for all occupants with a role of 'participant'.

            Example 84. Moderator Requests Voice List

            <iq from='bard@shakespeare.lit/globe'
                id='voice3'
                to='goodfolk@chat.shakespeare.lit'
                type='get'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item role='participant'/>
              </query>
            </iq>
        */

		/// <summary>
		/// A moderator in a moderated room may want to modify the voice list. 
		/// To do so, the moderator first requests the voice list by querying the room for all occupants 
		/// with a role of 'participant'.
		/// The service MUST then return the voice list to the moderator; each item MUST include 
		/// the 'nick' and 'role' attributes and SHOULD include the 'affiliation' and 'jid' attributes.
		/// The moderator MAY then modify the voice list. In order to do so, the moderator MUST send the 
		/// changed items (i.e., only the "delta") back to the service; each item MUST include 
		/// the 'nick' attribute and 'role' attribute (normally set to a value of "participant" or "visitor") 
		/// but SHOULD NOT include the 'jid' attribute and MUST NOT include the 'affiliation' attribute 
		/// (which is used to manage affiliations such as owner rather than the participant role),
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		public void RequestVoiceList(Jid room)
		{
			this.RequestVoiceList(room, null, null);
		}

		/// <summary>
		/// A moderator in a moderated room may want to modify the voice list. 
		/// To do so, the moderator first requests the voice list by querying the room for all occupants 
		/// with a role of 'participant'.
		/// The service MUST then return the voice list to the moderator; each item MUST include 
		/// the 'nick' and 'role' attributes and SHOULD include the 'affiliation' and 'jid' attributes.
		/// The moderator MAY then modify the voice list. In order to do so, the moderator MUST send the 
		/// changed items (i.e., only the "delta") back to the service; each item MUST include 
		/// the 'nick' attribute and 'role' attribute (normally set to a value of "participant" or "visitor") 
		/// but SHOULD NOT include the 'jid' attribute and MUST NOT include the 'affiliation' attribute 
		/// (which is used to manage affiliations such as owner rather than the participant role),        
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="cb"></param>
		public void RequestVoiceList(Jid room, IqCB cb)
		{
			this.RequestVoiceList(room, cb, null);
		}

		/// <summary>
		/// A moderator in a moderated room may want to modify the voice list. 
		/// To do so, the moderator first requests the voice list by querying the room for all occupants 
		/// with a role of 'participant'.
		/// The service MUST then return the voice list to the moderator; each item MUST include 
		/// the 'nick' and 'role' attributes and SHOULD include the 'affiliation' and 'jid' attributes.
		/// The moderator MAY then modify the voice list. In order to do so, the moderator MUST send the 
		/// changed items (i.e., only the "delta") back to the service; each item MUST include 
		/// the 'nick' attribute and 'role' attribute (normally set to a value of "participant" or "visitor") 
		/// but SHOULD NOT include the 'jid' attribute and MUST NOT include the 'affiliation' attribute 
		/// (which is used to manage affiliations such as owner rather than the participant role),        
		/// </summary>
		/// <param name="room">Jid of the room to which this iq is sent</param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void RequestVoiceList(Jid room, IqCB cb, object cbArg)
		{
			this.RequestList(Role.participant, room, cb, cbArg);
		}

		/*
         Example 89. Admin Bans User

        <iq from='kinghenryv@shakespeare.lit/throne'
            id='ban1'
            to='southampton@henryv.shakespeare.lit'
            type='set'>
          <query xmlns='http://jabber.org/protocol/muc#admin'>
            <item affiliation='outcast'
                  jid='earlofcambridge@shakespeare.lit'>
              <reason>Treason</reason>
            </item>
          </query>
        </iq>
        */

		/// <summary>
		/// 
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		public void BanUser(Jid room, Jid user)
		{
			this.BanUser(room, user, null, null, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		/// <param name="reason"></param>
		public void BanUser(Jid room, Jid user, string reason)
		{
			this.BanUser(room, user, reason, null, null);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		public void BanUser(Jid room, Jid user, string reason, IqCB cb)
		{
			this.BanUser(room, user, reason, cb, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void BanUser(Jid room, Jid user, string reason, IqCB cb, object cbArg)
		{
			this.ChangeAffiliation(Affiliation.outcast, room, user, null, reason, cb, cbArg);
		}

		/*
            A room admin may want to modify the ban list. Note: The ban list is always based on a user's bare JID, although a nick (perhaps the last room nickname associated with that JID) MAY be included for convenience. To modify the list of banned JIDs, the admin first requests the ban list by querying the room for all users with an affiliation of 'outcast'.

            Example 94. Admin Requests Ban List

            <iq from='kinghenryv@shakespeare.lit/throne'
                id='ban2'
                to='southampton@henryv.shakespeare.lit'
                type='get'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item affiliation='outcast'/>
              </query>
            </iq>
    
         */

		/// <summary>
		/// A room admin may want to modify the ban list. 
		/// <remarks>
		/// Note: The ban list is always based on a user's bare JID, 
		/// although a nick (perhaps the last room nickname associated with that JID) MAY be included for convenience. 
		/// To modify the list of banned JIDs, the admin first requests the ban list by querying the room for all 
		/// users with an affiliation of 'outcast'.
		/// </remarks>
		/// </summary>
		/// <param name="room"></param>
		public void RequestBanList(Jid room)
		{
			this.RequestBanList(room, null, null);
		}

		/// <summary>
		/// A room admin may want to modify the ban list. 
		/// <remarks>
		/// Note: The ban list is always based on a user's bare JID, 
		/// although a nick (perhaps the last room nickname associated with that JID) MAY be included for convenience. 
		/// To modify the list of banned JIDs, the admin first requests the ban list by querying the room for all 
		/// users with an affiliation of 'outcast'.
		/// </remarks>
		/// </summary>
		/// <param name="room"></param>        
		/// <param name="cb"></param>
		public void RequestBanList(Jid room, IqCB cb)
		{
			this.RequestBanList(room, cb, null);
		}

		/// <summary>
		/// A room admin may want to modify the ban list. 
		/// <remarks>
		/// Note: The ban list is always based on a user's bare JID, 
		/// although a nick (perhaps the last room nickname associated with that JID) MAY be included for convenience. 
		/// To modify the list of banned JIDs, the admin first requests the ban list by querying the room for all 
		/// users with an affiliation of 'outcast'.
		/// </remarks>
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void RequestBanList(Jid room, IqCB cb, object cbArg)
		{
			this.RequestList(Affiliation.outcast, room, cb, cbArg);
		}


		/*
            An owner can grant administrative privileges to a member or unaffiliated user; this is done by changing the user's affiliation to "admin":

            Example 155. Owner Grants Admin Privileges

            <iq from='crone1@shakespeare.lit/desktop'
                id='admin1'
                to='darkcave@macbeth.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item affiliation='admin'
                      jid='wiccarocks@shakespeare.lit'/>
              </query>
            </iq>
        */

		/// <summary>
		/// Grant administrative privileges to a member or unaffiliated user.
		/// This could be done by an room owner
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		public void GrantAdminPrivileges(Jid room, Jid user)
		{
			this.GrantAdminPrivileges(room, user, null, null);
		}

		/// <summary>
		/// Grant administrative privileges to a member or unaffiliated user.
		/// This could be done by an room owner
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		/// <param name="cb"></param>
		public void GrantAdminPrivileges(Jid room, Jid user, IqCB cb)
		{
			this.GrantAdminPrivileges(room, user, cb, null);
		}

		/// <summary>
		/// Grant administrative privileges to a member or unaffiliated user.
		/// This could be done by an room owner
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void GrantAdminPrivileges(Jid room, Jid user, IqCB cb, object cbArg)
		{
			this.ChangeAffiliation(Affiliation.admin, room, user, cb, cbArg);
		}


		/*
            An admin can grant membership to a user; 
            this is done by changing the user's affiliation to "member" 
            (normally based on nick if the user is in the room, or on bare JID if not; 
            in either case, if the nick is provided, that nick becomes the user's default nick in the room
            if that functionality is supported by the implementation)

            Example 98. Admin Grants Membership

            <iq from='crone1@shakespeare.lit/desktop'
                id='member1'
                to='darkcave@macbeth.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item affiliation='member'
                      jid='hag66@shakespeare.lit'/>
              </query>
            </iq>
        */

		/// <summary>
		/// An admin can grant membership to a user; 
		/// this is done by changing the user's affiliation to "member" 
		/// (normally based on nick if the user is in the room, or on bare JID if not; 
		/// in either case, if the nick is provided, that nick becomes the user's default nick in the room
		/// if that functionality is supported by the implementation)
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		public void GrantMembership(Jid room, Jid user)
		{
			this.GrantMembership(room, user, null, null, null);
		}

		/// <summary>
		/// An admin can grant membership to a user; 
		/// this is done by changing the user's affiliation to "member" 
		/// (normally based on nick if the user is in the room, or on bare JID if not; 
		/// in either case, if the nick is provided, that nick becomes the user's default nick in the room
		/// if that functionality is supported by the implementation)
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		/// <param name="reason"></param>
		public void GrantMembership(Jid room, Jid user, string reason)
		{
			this.GrantMembership(room, user, reason, null, null);
		}

		/// <summary>
		/// An admin can grant membership to a user; 
		/// this is done by changing the user's affiliation to "member" 
		/// (normally based on nick if the user is in the room, or on bare JID if not; 
		/// in either case, if the nick is provided, that nick becomes the user's default nick in the room
		/// if that functionality is supported by the implementation)
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		public void GrantMembership(Jid room, Jid user, string reason, IqCB cb)
		{
			this.GrantMembership(room, user, reason, cb, null);
		}

		/// <summary>
		/// An admin can grant membership to a user; 
		/// this is done by changing the user's affiliation to "member" 
		/// (normally based on nick if the user is in the room, or on bare JID if not; 
		/// in either case, if the nick is provided, that nick becomes the user's default nick in the room
		/// if that functionality is supported by the implementation)
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void GrantMembership(Jid room, Jid user, string reason, IqCB cb, object cbArg)
		{
			this.ChangeAffiliation(Affiliation.member, room, user, null, reason, cb, cbArg);
		}

		/// <summary>
		/// An admin can grant membership to a user; 
		/// this is done by changing the user's affiliation to "member" 
		/// (normally based on nick if the user is in the room, or on bare JID if not; 
		/// in either case, if the nick is provided, that nick becomes the user's default nick in the room
		/// if that functionality is supported by the implementation)
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		public void GrantMembership(Jid room, string nickname)
		{
			this.GrantMembership(room, nickname, null, null, null);
		}

		/// <summary>
		/// An admin can grant membership to a user; 
		/// this is done by changing the user's affiliation to "member" 
		/// (normally based on nick if the user is in the room, or on bare JID if not; 
		/// in either case, if the nick is provided, that nick becomes the user's default nick in the room
		/// if that functionality is supported by the implementation)
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		public void GrantMembership(Jid room, string nickname, string reason)
		{
			this.GrantMembership(room, nickname, reason, null, null);
		}

		/// <summary>
		/// An admin can grant membership to a user; 
		/// this is done by changing the user's affiliation to "member" 
		/// (normally based on nick if the user is in the room, or on bare JID if not; 
		/// in either case, if the nick is provided, that nick becomes the user's default nick in the room
		/// if that functionality is supported by the implementation)
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		public void GrantMembership(Jid room, string nickname, string reason, IqCB cb)
		{
			this.GrantMembership(room, nickname, reason, cb, null);
		}

		/// <summary>
		/// An admin can grant membership to a user; 
		/// this is done by changing the user's affiliation to "member" 
		/// (normally based on nick if the user is in the room, or on bare JID if not; 
		/// in either case, if the nick is provided, that nick becomes the user's default nick in the room
		/// if that functionality is supported by the implementation)
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void GrantMembership(Jid room, string nickname, string reason, IqCB cb, object cbArg)
		{
			this.ChangeAffiliation(Affiliation.member, room, nickname, reason, cb, cbArg);
		}


		/*
            Example 144. Owner Grants Ownership Privileges

            <iq from='crone1@shakespeare.lit/desktop'
                id='owner1'
                to='darkcave@macbeth.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item affiliation='owner'
                      jid='hecate@shakespeare.lit'/>
              </query>
            </iq>                
         */

		/// <summary>
		/// If allowed by an implementation, an owner MAY grant ownership privileges to another user.        
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		public void GrantOwnershipPrivileges(Jid room, Jid user)
		{
			this.ChangeAffiliation(Affiliation.owner, room, user, null, null);
		}

		/// <summary>
		/// If allowed by an implementation, an owner MAY grant ownership privileges to another user.        
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		/// <param name="cb"></param>
		public void GrantOwnershipPrivileges(Jid room, Jid user, IqCB cb)
		{
			this.ChangeAffiliation(Affiliation.owner, room, user, cb, null);
		}

		/// <summary>
		/// If allowed by an implementation, an owner MAY grant ownership privileges to another user.        
		/// </summary>
		/// <param name="room"></param>
		/// <param name="user"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void GrantOwnershipPrivileges(Jid room, Jid user, IqCB cb, object cbArg)
		{
			this.ChangeAffiliation(Affiliation.owner, room, user, cb, cbArg);
		}

		/*
            8.4 Revoking Membership

            An admin may want to revoke a user's membership; this is done by changing the user's affiliation to "none":

            Example 101. Admin Revokes Membership

            <iq from='crone1@shakespeare.lit/desktop'
                id='member2'
                to='darkcave@macbeth.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item affiliation='none'
                      nick='thirdwitch'/>
              </query>
            </iq>
        */

		/// <summary>
		/// An admin may want to revoke a user's membership
		/// this is done by changing the user's affiliation to "none"
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		public void RevokeMembership(Jid room, string nickname)
		{
			this.RevokeMembership(room, nickname, null, null);
		}

		/// <summary>
		/// An admin may want to revoke a user's membership
		/// this is done by changing the user's affiliation to "none"
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		public void RevokeMembership(Jid room, string nickname, string reason)
		{
			this.RevokeMembership(room, nickname, reason, null, null);
		}

		/// <summary>
		/// An admin may want to revoke a user's membership
		/// this is done by changing the user's affiliation to "none"
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		public void RevokeMembership(Jid room, string nickname, string reason, IqCB cb)
		{
			this.RevokeMembership(room, nickname, reason, cb, null);
		}

		/// <summary>
		/// An admin may want to revoke a user's membership
		/// this is done by changing the user's affiliation to "none"
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void RevokeMembership(Jid room, string nickname, string reason, IqCB cb, object cbArg)
		{
			this.ChangeAffiliation(Affiliation.none, room, nickname, reason, cb, cbArg);
		}


		/*
            9.8 Modifying the Admin List

            A room owner may want to modify the admin list. 
            To do so, the owner first requests the admin list by querying the room for all users with an affiliation of 'admin'.

            Example 161. Owner Requests Admin List

            <iq from='bard@shakespeare.lit/desktopaffiliation
                id='admin3'
                to='darkcave@macbeth.shakespeare.lit'
                type='get'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item affiliation='admin'/>
              </query>
            </iq>
    
        */

		/// <summary>
		/// Request the list of admins. This could be done by the room owner
		/// </summary>
		/// <param name="room"></param>
		public void RequestAdminList(Jid room)
		{
			this.RequestAdminList(room, null, null);
		}

		/// <summary>
		/// Request the list of admins. This could be done by the room owner
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		public void RequestAdminList(Jid room, IqCB cb)
		{
			this.RequestAdminList(room, cb, null);
		}

		/// <summary>
		/// Request the list of admins. This could be done by the room owner
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void RequestAdminList(Jid room, IqCB cb, object cbArg)
		{
			this.RequestList(Affiliation.admin, room, cb, cbArg);
		}

		/*
            If allowed by an implementation, a room owner may want to modify the owner list. 
            To do so, the owner first requests the owner list by querying the room for all users with an affiliation of 'owner'.

            Example 150. Owner Requests Owner List

            <iq from='bard@shakespeare.lit/globe'
                id='owner3'
                to='darkcave@macbeth.shakespeare.lit'
                type='get'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item affiliation='owner'/>
              </query>
            </iq>
    
        */

		/// <summary>
		/// Request the owner list of a room
		/// </summary>
		/// <param name="room"></param>
		public void RequestOwnerList(Jid room)
		{
			this.RequestOwnerList(room, null, null);
		}

		/// <summary>
		/// Request the owner list of a room
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		public void RequestOwnerList(Jid room, IqCB cb)
		{
			this.RequestOwnerList(room, cb, null);
		}

		/// <summary>
		/// Request the owner list of a room
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void RequestOwnerList(Jid room, IqCB cb, object cbArg)
		{
			this.RequestList(Affiliation.owner, room, cb, cbArg);
		}

		/*
            Example 105. Admin Requests Member List

            <iq from='crone1@shakespeare.lit/desktop'
                id='member3'
                to='darkcave@macbeth.shakespeare.lit'
                type='get'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item affiliation='member'/>
              </query>
            </iq>
                
         */

		/// <summary>
		/// In the context of a members-only room, the member list is essentially a "whitelist" of people 
		/// who are allowed to enter the room. Anyone who is not a member is effectively banned from entering the room, 
		/// even if their affiliation is not "outcast".
		/// In the context of an open room, the member list is simply a list of users (bare JID and reserved nick) 
		/// who are registered with the room. Such users may appear in a room roster, have their room nickname reserved, 
		/// be returned in search results or FAQ queries, and the like.
		/// It is RECOMMENDED that only room admins have the privilege to modify the member list in members-only rooms. 
		/// To do so, the admin first requests the member list by querying the room for all users with an affiliation of "member"
		/// </summary>
		/// <param name="room"></param>
		public void RequestMemberList(Jid room)
		{
			this.RequestMemberList(room, null, null);
		}

		/// <summary>
		/// In the context of a members-only room, the member list is essentially a "whitelist" of people 
		/// who are allowed to enter the room. Anyone who is not a member is effectively banned from entering the room, 
		/// even if their affiliation is not "outcast".
		/// In the context of an open room, the member list is simply a list of users (bare JID and reserved nick) 
		/// who are registered with the room. Such users may appear in a room roster, have their room nickname reserved, 
		/// be returned in search results or FAQ queries, and the like.
		/// It is RECOMMENDED that only room admins have the privilege to modify the member list in members-only rooms. 
		/// To do so, the admin first requests the member list by querying the room for all users with an affiliation of "member"
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		public void RequestMemberList(Jid room, IqCB cb)
		{
			this.RequestMemberList(room, cb, null);
		}

		/// <summary>
		/// In the context of a members-only room, the member list is essentially a "whitelist" of people 
		/// who are allowed to enter the room. Anyone who is not a member is effectively banned from entering the room, 
		/// even if their affiliation is not "outcast".
		/// In the context of an open room, the member list is simply a list of users (bare JID and reserved nick) 
		/// who are registered with the room. Such users may appear in a room roster, have their room nickname reserved, 
		/// be returned in search results or FAQ queries, and the like.
		/// It is RECOMMENDED that only room admins have the privilege to modify the member list in members-only rooms. 
		/// To do so, the admin first requests the member list by querying the room for all users with an affiliation of "member"
		/// </summary>
		/// <param name="room"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void RequestMemberList(Jid room, IqCB cb, object cbArg)
		{
			this.RequestList(Affiliation.member, room, cb, cbArg);
		}

		/*
            8.6 Granting Moderator Privileges

            An admin may want to grant moderator privileges to a participant or visitor; this is done by changing the user's role to "moderator":

            Example 113. Admin Grants Moderator Privileges

            <iq from='crone1@shakespeare.lit/desktop'
                id='mod1'
                to='darkcave@macbeth.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item nick='thirdwitch'
                      role='moderator'/>
              </query>
            </iq>
                
        */

		/// <summary>
		/// An admin may want to grant moderator privileges to a participant or visitor
		/// this is done by changing the user's role to "moderator"
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		public void GrantModeratorPrivileges(Jid room, string nickname)
		{
			this.GrantModeratorPrivileges(room, nickname, null, null, null);
		}

		/// <summary>
		/// An admin may want to grant moderator privileges to a participant or visitor
		/// this is done by changing the user's role to "moderator"
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		public void GrantModeratorPrivileges(Jid room, string nickname, string reason)
		{
			this.GrantModeratorPrivileges(room, nickname, reason, null, null);
		}

		/// <summary>
		/// An admin may want to grant moderator privileges to a participant or visitor
		/// this is done by changing the user's role to "moderator"
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		public void GrantModeratorPrivileges(Jid room, string nickname, string reason, IqCB cb)
		{
			this.GrantModeratorPrivileges(room, nickname, reason, cb, null);
		}

		/// <summary>
		/// An admin may want to grant moderator privileges to a participant or visitor
		/// this is done by changing the user's role to "moderator"
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nickname"></param>
		/// <param name="reason"></param>
		/// <param name="cb"></param>
		/// <param name="cbArg"></param>
		public void GrantModeratorPrivileges(Jid room, string nickname, string reason, IqCB cb, object cbArg)
		{
			this.ChangeRole(Role.moderator, room, nickname, reason, cb, cbArg);
		}

		/*
            8.7 Revoking Moderator Privileges

            An admin may want to revoke a user's moderator privileges. An admin MAY revoke moderator privileges only from a user whose affiliation is "member" or "none" (i.e., not from an owner or admin). The privilege is revoked by changing the user's role to "participant":

            Example 116. Admin Revokes Moderator Privileges

            <iq from='crone1@shakespeare.lit/desktop'
                id='mod2'
                to='darkcave@macbeth.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item nick='thirdwitch'
                      role='participant'/>
              </query>
            </iq>
        */

		public void RevokeModerator(Jid room, string nickname)
		{
			this.RevokeModerator(room, nickname, null, null, null);
		}

		public void RevokeModerator(Jid room, string nickname, string reason)
		{
			this.RevokeModerator(room, nickname, reason, null, null);
		}

		public void RevokeModerator(Jid room, string nickname, string reason, IqCB cb)
		{
			this.RevokeModerator(room, nickname, reason, cb, null);
		}

		public void RevokeModerator(Jid room, string nickname, string reason, IqCB cb, object cbArg)
		{
			this.ChangeRole(Role.participant, room, nickname, reason, cb, cbArg);
		}

		/*
            8.8 Modifying the Moderator List

            An admin may want to modify the moderator list. To do so, the admin first requests the moderator list by querying the room for all users with a role of 'moderator'.

            Example 120. Admin Requests Moderator List

            <iq from='crone1@shakespeare.lit/desktop'
                id='mod3'
                to='darkcave@macbeth.shakespeare.lit'
                type='get'>
              <query xmlns='http://jabber.org/protocol/muc#admin'>
                <item role='moderator'/>
              </query>
            </iq>
                
        */

		public void RequestModeratorList(Jid room)
		{
			this.RequestModeratorList(room, null, null);
		}

		public void RequestModeratorList(Jid room, IqCB cb)
		{
			this.RequestModeratorList(room, cb, null);
		}

		public void RequestModeratorList(Jid room, IqCB cb, object cbArg)
		{
			this.RequestList(Role.moderator, room, cb, cbArg);
		}


		public void RequestList(Affiliation affiliation, Jid room, IqCB cb, object cbArg)
		{
			var aIq = new AdminIq();
			aIq.To = room;
			aIq.Type = IQType.Get;

			aIq.Query.AddItem(new iq.admin.Item(affiliation));

			if (cb == null)
				this.m_connection.Send(aIq);
			else
				this.m_connection.IqGrabber.SendIq(aIq, cb, cbArg);
		}

		public void RequestList(Role role, Jid room, IqCB cb, object cbArg)
		{
			var aIq = new AdminIq();
			aIq.To = room;
			aIq.Type = IQType.Get;

			aIq.Query.AddItem(new iq.admin.Item(role));

			if (cb == null)
				this.m_connection.Send(aIq);
			else
				this.m_connection.IqGrabber.SendIq(aIq, cb, cbArg);
		}

		#region << Create Reserved Room >>
		/// <summary>
		/// Creates a reserved room. The MUC server replies to this request either with an error if the room already exists 
		/// or another error occured. Or with the configuration for, for the reserved room which you have submit in the
		/// second step
		/// </summary>
		/// <param name="room">Jid of the room to create</param>
		public void CreateReservedRoom(Jid room)
		{
			this.CreateReservedRoom(room, null, null);
		}

		/// <summary>
		/// <para>
		/// Creates a reserved room. The MUC server replies to this request either with an error if the room already exists 
		/// or another error occured. Or with the configuration for, for the reserved room which you have submit in the
		/// second step.
		/// </para>        
		/// </summary>
		/// <param name="room">Jid of the room to create</param>
		/// <param name="cb">callback for the response</param>
		public void CreateReservedRoom(Jid room, IqCB cb)
		{
			this.CreateReservedRoom(room, cb, null);
		}

		/// <summary>
		/// <para>
		/// Creates a reserved room. The MUC server replies to this request either with an error if the room already exists 
		/// or another error occured. Or with the configuration for, for the reserved room which you have submit in the
		/// second step.
		/// </para>        
		/// </summary>
		/// <param name="room">Jid of the room to create</param>
		/// <param name="cb">callback for the response</param>
		/// <param name="cbArg">optional callback arguments</param>
		public void CreateReservedRoom(Jid room, IqCB cb, object cbArg)
		{
			/*
            <iq from='crone1@shakespeare.lit/desktop'
                id='create1'
                to='darkcave@macbeth.shakespeare.lit'
                type='get'>
                <query xmlns='http://jabber.org/protocol/muc#owner'/>
            </iq>
            */

			var iq = new OwnerIq();
			iq.Type = IQType.Get;
			iq.To = room;

			if (cb == null)
				this.m_connection.Send(iq);
			else
				this.m_connection.IqGrabber.SendIq(iq, cb, cbArg);
		}
		#endregion


		#region << Destroy Room >>
		public void DestroyRoom(Jid room, Jid altVenue)
		{
			this.DestroyRoom(room, altVenue, null, null, null);
		}

		public void DestroyRoom(Jid room, Jid altVenue, IqCB cb)
		{
			this.DestroyRoom(room, altVenue, null, cb, null);
		}

		public void DestroyRoom(Jid room, Jid altVenue, IqCB cb, object cbArg)
		{
			this.DestroyRoom(room, altVenue, null, cb, cbArg);
		}

		public void DestroyRoom(Jid room, string reason)
		{
			this.DestroyRoom(room, null, reason, null, null);
		}

		public void DestroyRoom(Jid room, string reason, IqCB cb)
		{
			this.DestroyRoom(room, null, reason, cb, null);
		}

		public void DestroyRoom(Jid room, string reason, IqCB cb, object cbArg)
		{
			this.DestroyRoom(room, null, reason, cb, cbArg);
		}

		public void DestroyRoom(Jid room, Jid altVenue, string reason)
		{
			this.DestroyRoom(room, altVenue, reason, null, null);
		}

		public void DestroyRoom(Jid room, Jid altVenue, string reason, IqCB cb)
		{
			this.DestroyRoom(room, altVenue, reason, cb, null);
		}

		public void DestroyRoom(Jid room, Jid altVenue, string reason, IqCB cb, object cbArg)
		{
			/*
             Example 177. Owner Submits Room Destruction Request

            <iq from='crone1@shakespeare.lit/desktop'
                id='begone'
                to='heath@macbeth.shakespeare.lit'
                type='set'>
              <query xmlns='http://jabber.org/protocol/muc#owner'>
                <destroy jid='darkcave@macbeth.shakespeare.lit'>
                  <reason>Macbeth doth come.</reason>
                </destroy>
              </query>
            </iq>
            */

			var iq = new OwnerIq();
			iq.Type = IQType.Set;
			iq.To = room;

			var destroy = new Destroy();

			if (reason != null)
				destroy.Reason = reason;

			if (altVenue != null)
				destroy.AlternateVenue = altVenue;

			iq.Query.AddChild(destroy);

			if (cb == null)
				this.m_connection.Send(iq);
			else
				this.m_connection.IqGrabber.SendIq(iq, cb, cbArg);

		}
		#endregion

		public void ModifyList(Jid room, iq.admin.Item[] items)
		{
			this.ModifyList(room, items, null, null);
		}

		public void ModifyList(Jid room, iq.admin.Item[] items, IqCB cb)
		{
			this.ModifyList(room, items, cb, null);
		}

		public void ModifyList(Jid room, iq.admin.Item[] items, IqCB cb, object cbArg)
		{
			var aIq = new AdminIq();
			aIq.To = room;
			aIq.Type = IQType.Set;

			foreach (var itm in items)
			{
				aIq.Query.AddItem(itm);
			}

			if (cb == null)
				this.m_connection.Send(aIq);
			else
				this.m_connection.IqGrabber.SendIq(aIq, cb, cbArg);
		}


		#region << private functions >>

		private void ChangeRole(Role role, Jid room, string nickname, string reason, IqCB cb, object cbArg)
		{
			var aIq = new AdminIq();
			aIq.To = room;
			aIq.Type = IQType.Set;

			var itm = new iq.admin.Item();
			itm.Role = role;
			itm.Nickname = nickname;

			if (reason != null)
				itm.Reason = reason;

			aIq.Query.AddItem(itm);

			if (cb == null)
				this.m_connection.Send(aIq);
			else
				this.m_connection.IqGrabber.SendIq(aIq, cb, cbArg);
		}

		private void ChangeAffiliation(Affiliation affiliation, Jid room, string nickname, string reason, IqCB cb, object cbArg)
		{
			var aIq = new AdminIq();
			aIq.To = room;
			aIq.Type = IQType.Set;

			var itm = new iq.admin.Item();
			itm.Affiliation = affiliation;

			if (nickname != null)
				itm.Nickname = nickname;

			if (reason != null)
				itm.Reason = reason;

			aIq.Query.AddItem(itm);

			if (cb == null)
				this.m_connection.Send(aIq);
			else
				this.m_connection.IqGrabber.SendIq(aIq, cb, cbArg);
		}

		private void ChangeAffiliation(Affiliation affiliation, Jid room, Jid user, IqCB cb, object cbArg)
		{
			this.ChangeAffiliation(affiliation, room, user, null, null, cb, cbArg);
		}

		private void ChangeAffiliation(Affiliation affiliation, Jid room, Jid user, string nickname, string reason, IqCB cb, object cbArg)
		{
			var aIq = new AdminIq();
			aIq.To = room;
			aIq.Type = IQType.Set;

			var itm = new iq.admin.Item();
			itm.Affiliation = affiliation;

			if (user != null)
				itm.Jid = user;

			if (nickname != null)
				itm.Nickname = nickname;

			if (reason != null)
				itm.Reason = reason;

			aIq.Query.AddItem(itm);

			if (cb == null)
				this.m_connection.Send(aIq);
			else
				this.m_connection.IqGrabber.SendIq(aIq, cb, cbArg);
		}
		#endregion
	}
}
