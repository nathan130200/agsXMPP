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

/* --------------------------------------------------------------------------
 * Copyrights
 * 
 * Portions created by or assigned to Cursive Systems, Inc. are 
 * Copyright (c) 2002-2005 Cursive Systems, Inc.  All Rights Reserved.  Contact
 * information for Cursive Systems, Inc. is available at
 * http://www.cursive.net/.
 *
 * License
 * 
 * Jabber-Net can be used under either JOSL or the GPL.  
 * See LICENSE.txt for details.
 * --------------------------------------------------------------------------*/
namespace AgsXMPP.Xml.Xpnet
{

	/**
	 * Represents a position in an entity.
	 * A position can be modified by <code>Encoding.movePosition</code>.
	 * @see Encoding#movePosition
	 * @version $Revision: 1.2 $ $Date: 1998/02/17 04:24:15 $
	 */
	public class Position : System.ICloneable
	{
		private int lineNumber;
		private int columnNumber;

		/**
		 * Creates a position for the start of an entity: the line number is
		 * 1 and the column number is 0.
		 */
		public Position()
		{
			this.lineNumber = 1;
			this.columnNumber = 0;
		}

		/**
		 * Returns the line number.
		 * The first line number is 1.
		 */
		public int LineNumber
		{
			get { return this.lineNumber; }
			set { this.lineNumber = value; }
		}

		/**
		 * Returns the column number.
		 * The first column number is 0.
		 * A tab character is not treated specially.
		 */
		public int ColumnNumber
		{
			get { return this.columnNumber; }
			set { this.columnNumber = value; }
		}

		/**
		 * Returns a copy of this position.
		 */
		public object Clone()
		{
#if CF
	  throw new util.NotImplementedException();
#else
			throw new System.NotImplementedException();
#endif
		}
	}
}