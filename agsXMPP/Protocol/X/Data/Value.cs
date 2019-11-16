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

namespace agsXMPP.Protocol.x.data
{
	/// <summary>
	/// Summary description for Value.
	/// </summary>
	public class Value : Element
	{
		public Value()
		{
			this.TagName = "value";
			this.Namespace = Namespaces.X_DATA;
		}

		public Value(string val) : this()
		{
			this.Value = val;
		}

		public Value(bool val) : this()
		{
			this.Value = val ? "1" : "0";
		}

	}
}
