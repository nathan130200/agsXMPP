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

namespace AgsXMPP.Protocol.X.data
{
	/// <summary>
	/// Form Types
	/// </summary>
	public enum XDataFormType
	{
		/// <summary>
		/// The forms-processing entity is asking the forms-submitting entity to complete a form.
		/// </summary>
		[XmppEnumMember("form")]
		Form,

		/// <summary>
		/// The forms-submitting entity is submitting data to the forms-processing entity.
		/// </summary>
		[XmppEnumMember("submit")]
		Submit,

		/// <summary>
		/// The forms-submitting entity has cancelled submission of data to the forms-processing entity.
		/// </summary>
		[XmppEnumMember("cancel")]
		Cancel,

		/// <summary>
		/// The forms-processing entity is returning data (e.g., search results) to the forms-submitting entity, or the data is a generic data set.
		/// </summary>
		[XmppEnumMember("result")]
		Result
	}
}
