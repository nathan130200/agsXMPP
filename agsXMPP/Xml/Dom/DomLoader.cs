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

using System.IO;
//using System.Collections.Specialized;

namespace AgsXMPP.Xml.Dom
{
	/// <summary>
	/// internal class that loads a xml document from a string or stream
	/// </summary>
	internal class DomLoader
	{
		private Document doc;
		private StreamParser sp;

		public DomLoader(string xml, Document d)
		{
			this.doc = d;
			this.sp = new StreamParser();

			this.sp.OnStreamStart += new StreamHandler(this.sp_OnStreamStart);
			this.sp.OnStreamElement += new StreamHandler(this.sp_OnStreamElement);
			this.sp.OnStreamEnd += new StreamHandler(this.sp_OnStreamEnd);

			var b = System.Text.Encoding.UTF8.GetBytes(xml);
			this.sp.Push(b, 0, b.Length);
		}

		public DomLoader(StreamReader sr, Document d) : this(sr.ReadToEnd(), d)
		{

		}

		// ya, the Streamparser is only usable for parsing xmpp stream.
		// it also does a very good job here
		private void sp_OnStreamStart(object sender, Node e)
		{
			this.doc.ChildNodes.Add(e);
		}

		private void sp_OnStreamElement(object sender, Node e)
		{
			this.doc.RootElement.ChildNodes.Add(e);
		}

		private void sp_OnStreamEnd(object sender, Node e)
		{

		}
	}
}