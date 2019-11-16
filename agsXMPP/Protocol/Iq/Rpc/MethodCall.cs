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

using System;
using System.Collections;
using System.Globalization;

using agsXMPP.Xml.Dom;

namespace agsXMPP.Protocol.iq.rpc
{

	/// <summary>
	/// The methodCall element.     
	/// </summary>
	public class MethodCall : Element
	{
		/*
        
         <methodCall>
            <methodName>examples.getStateName</methodName>
            <params>
                <param><value><i4>41</i4></value></param>
            </params>
         </methodCall>        
         
        */

		/// <summary>
		/// 
		/// </summary>
		public MethodCall()
		{
			this.TagName = "methodCall";
			this.Namespace = Namespaces.IQ_RPC;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="methodName"></param>
		/// <param name="Params"></param>
		public MethodCall(string methodName, ArrayList Params) : this()
		{
			this.WriteCall(methodName, Params);
		}

		/// <summary>
		/// 
		/// </summary>
		public string MethodName
		{
			set { this.SetTag("methodName", value); }
			get { return this.GetTag("methodName"); }
		}

		/// <summary>
		/// Write the functions call with params to this Element
		/// </summary>
		/// <param name="name"></param>
		/// <param name="Params"></param>
		public void WriteCall(string name, ArrayList Params)
		{
			this.MethodName = name;

			if (Params != null && Params.Count > 0)
			{
				// remote this tag if exists, in case this function gets
				// calles multiple times by some guys
				this.RemoveTag("params");
				var elParams = new Element("params");

				for (var i = 0; i < Params.Count; i++)
				{
					var param = new Element("param");
					this.WriteValue(Params[i], param);
					elParams.AddChild(param);
				}
				this.AddChild(elParams);
			}
		}

		/// <summary>
		/// Writes a single value to a call
		/// </summary>
		/// <param name="param"></param>
		/// <param name="parent"></param>
		private void WriteValue(object param, Element parent)
		{
			var value = new Element("value");

			if (param is string)
			{
				value.AddChild(new Element("string", param as string));
			}
			else if (param is int)
			{
				value.AddChild(new Element("i4", ((int)param).ToString()));
			}
			else if (param is double)
			{
				var numberInfo = new NumberFormatInfo();
				numberInfo.NumberDecimalSeparator = ".";
				//numberInfo.NumberGroupSeparator = ",";
				value.AddChild(new Element("double", ((double)param).ToString(numberInfo)));
			}
			else if (param is bool)
			{
				value.AddChild(new Element("boolean", ((bool)param) ? "1" : "0"));
			}
			// XML-RPC dates are formatted in iso8601 standard, same as xmpp,
			else if (param is DateTime)
			{
				value.AddChild(new Element("dateTime.iso8601", Util.Time.ToISO((DateTime)param)));
			}
			// byte arrays must be encoded in Base64 encoding
			else if (param is byte[])
			{
				var b = (byte[])param;
				value.AddChild(new Element("base64", Convert.ToBase64String(b, 0, b.Length)));
			}
			// Arraylist maps to an XML-RPC array
			else if (param is ArrayList)
			{
				//<array>  
				//    <data>
				//        <value>  <string>one</string>  </value>
				//        <value>  <string>two</string>  </value>
				//        <value>  <string>three</string></value>  
				//    </data> 
				//</array>
				var array = new Element("array");
				var data = new Element("data");

				var list = param as ArrayList;

				for (var i = 0; i < list.Count; i++)
				{
					this.WriteValue(list[i], data);
				}

				array.AddChild(data);
				value.AddChild(array);
			}
			// java.util.Hashtable maps to an XML-RPC struct
			else if (param is Hashtable)
			{
				var elStruct = new Element("struct");

				var ht = (Hashtable)param;
				var myEnumerator = ht.Keys.GetEnumerator();
				while (myEnumerator.MoveNext())
				{
					var member = new Element("member");
					var key = myEnumerator.Current;

					member.AddChild(new Element("name", key.ToString()));
					this.WriteValue(ht[key], member);

					elStruct.AddChild(member);
				}

				value.AddChild(elStruct);
			}
			else
			{
				// Unknown Type
			}
			parent.AddChild(value);
		}


	}
}
