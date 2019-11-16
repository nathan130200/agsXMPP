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

namespace agsXMPP.Protocol.extensions.pubsub
{
	/*
        <iq type='result'
            from='pubsub.shakespeare.lit'
            to='francisco@denmark.lit'
            id='affil1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub'>
            <affiliations>
              <affiliation node='node1' jid='francisco@denmark.lit' affiliation='owner'/>
              <affiliation node='node2' jid='francisco@denmark.lit' affiliation='publisher'/>
              <affiliation node='node5' jid='francisco@denmark.lit' affiliation='outcast'/>
              <affiliation node='node6' jid='francisco@denmark.lit' affiliation='owner'/>
            </affiliations>
          </pubsub>
        </iq>
    */

	public class Affiliations : Element
	{
		#region << Consrtuctors >>
		public Affiliations()
		{
			this.TagName = "affiliations";
			this.Namespace = Namespaces.PUBSUB;
		}
		#endregion

		public Affiliation AddAffiliation()
		{
			var aff = new Affiliation();
			this.AddChild(aff);
			return aff;
		}


		public Affiliation AddAffiliation(Affiliation aff)
		{
			this.AddChild(aff);
			return aff;
		}

		public Affiliation[] GetAffiliations()
		{
			var nl = this.SelectElements(typeof(Affiliation));
			var items = new Affiliation[nl.Count];
			var i = 0;
			foreach (Element e in nl)
			{
				items[i] = (Affiliation)e;
				i++;
			}
			return items;
		}
	}
}
