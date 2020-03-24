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

namespace AgsXMPP.Factory
{
	/// <summary>
	/// 
	/// </summary>
	public class ElementType
	{
		private string m_TagName;
		private string m_Namespace;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="TagName"></param>
		/// <param name="Namespace"></param>
		public ElementType(string TagName, string Namespace)
		{
			this.m_TagName = TagName;
			this.m_Namespace = Namespace;
		}

		public override string ToString()
		{
			if ((this.m_Namespace != null) && (this.m_Namespace != string.Empty))
			{
				return (this.m_Namespace + ":" + this.m_TagName);
			}
			return this.m_TagName;
		}
	}
}
