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
	/*
	<x xmlns='jabber:x:data'
		type='{form-type}'>
		<title/>
		<instructions/>
		<field var='field-name'
				type='{field-type}'
				label='description'>
			<desc/>
			<required/>
			<value>field-value</value>
			<option label='option-label'><value>option-value</value></option>
			<option label='option-label'><value>option-value</value></option>
		</field>
	</x>
	
	
	<xs:element name='option'>
    <xs:complexType>
      <xs:sequence>
        <xs:element ref='value'/>
      </xs:sequence>
      <xs:attribute name='label' type='xs:string' use='optional'/>
    </xs:complexType>
	</xs:element>
	*/

	/// <summary>
	/// Field Option.
	/// </summary>
	public class Option : Element
	{
		#region << Constructors >>
		public Option()
		{
			this.TagName = "option";
			this.Namespace = Namespaces.X_DATA;
		}

		public Option(string label, string val) : this()
		{
			this.Label = label;
			this.SetValue(val);
		}
		#endregion

		#region << Properties >>
		/// <summary>
		/// Label of the option
		/// </summary>
		public string Label
		{
			get { return this.GetAttribute("label"); }
			set { this.SetAttribute("label", value); }
		}
		#endregion


		/// <summary>
		/// Value of the Option
		/// </summary>
		public string GetValue()
		{
			return this.GetTag(typeof(Value));
		}

		public void SetValue(string val)
		{
			this.SetTag(typeof(Value), val);
		}
	}
}