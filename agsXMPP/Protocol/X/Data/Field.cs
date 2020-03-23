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

using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.x.data
{

	/*
	 * <x xmlns='jabber:x:data'
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
	*/

	/// <summary>
	/// 
	/// </summary>
	public class Field : Element
	{
		public Field()
		{
			this.TagName = "field";
			this.Namespace = Namespaces.X_DATA;
		}

		public Field(FieldType type) : this()
		{
			this.Type = type;
		}

		public Field(string var, string label, FieldType type) : this()
		{
			this.Type = type;
			this.Var = var;
			this.Label = label;
		}

		#region << Properties >>
		public string Var
		{
			get { return this.GetAttribute("var"); }
			set { this.SetAttribute("var", value); }
		}

		public string Label
		{
			get { return this.GetAttribute("label"); }
			set { this.SetAttribute("label", value); }
		}

		public FieldType Type
		{
			get
			{
				switch (this.GetAttribute("type"))
				{
					case "boolean":
						return FieldType.Boolean;
					case "fixed":
						return FieldType.Fixed;
					case "hidden":
						return FieldType.Hidden;
					case "jid-multi":
						return FieldType.Jid_Multi;
					case "jid-single":
						return FieldType.Jid_Single;
					case "list-multi":
						return FieldType.List_Multi;
					case "list-single":
						return FieldType.List_Single;
					case "text-multi":
						return FieldType.Text_Multi;
					case "text-private":
						return FieldType.Text_Private;
					case "text-single":
						return FieldType.Text_Single;
					default:
						return FieldType.Unknown;
				}
			}

			set
			{
				switch (value)
				{
					case FieldType.Boolean:
						this.SetAttribute("type", "boolean");
						break;
					case FieldType.Fixed:
						this.SetAttribute("type", "fixed");
						break;
					case FieldType.Hidden:
						this.SetAttribute("type", "hidden");
						break;
					case FieldType.Jid_Multi:
						this.SetAttribute("type", "jid-multi");
						break;
					case FieldType.Jid_Single:
						this.SetAttribute("type", "jid-single");
						break;
					case FieldType.List_Multi:
						this.SetAttribute("type", "list-multi");
						break;
					case FieldType.List_Single:
						this.SetAttribute("type", "list-single");
						break;
					case FieldType.Text_Multi:
						this.SetAttribute("type", "text-multi");
						break;
					case FieldType.Text_Private:
						this.SetAttribute("type", "text-private");
						break;
					case FieldType.Text_Single:
						this.SetAttribute("type", "text-single");
						break;
					default:
						this.RemoveAttribute("type");
						break;
				}

			}
		}


		public string Description
		{
			get { return this.GetTag("desc"); }
			set { this.SetTag("desc", value); }
		}

		/// <summary>
		/// Is this field a required field?
		/// </summary>
		public bool IsRequired
		{
			get { return this.HasTag("required"); }
			set
			{
				if (value == true)
					this.SetTag("required");
				else
					this.RemoveTag("required");
			}
		}
		#endregion

		#region << Methods and Functions >>
		/// <summary>
		/// The value of the field.
		/// </summary>
		public string GetValue()
		{
			return this.GetTag(typeof(Value));
			//return GetTag("value");			
		}

		public bool HasValue(string val)
		{
			foreach (var s in this.GetValues())
			{
				if (s == val)
					return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public void SetValue(string val)
		{
			this.SetTag(typeof(Value), val);
		}

		/// <summary>
		/// Set the value of boolean fields
		/// </summary>
		/// <param name="val"></param>
		public void SetValueBool(bool val)
		{
			this.SetValue(val ? "1" : "0");
		}

		/// <summary>
		/// Get the value of boolean fields
		/// </summary>
		/// <returns></returns>
		public bool GetValueBool()
		{
			// only "0" and "1" are valid. We dont care about other buggy implementations
			var val = this.GetValue();
			if (val == null || val == "0")
				return false;
			else
				return true;
		}

		/// <summary>
		/// Returns the value as Jif for the Jid fields. 
		/// Or null when the value is not a valid Jid.
		/// </summary>
		/// <returns></returns>
		public Jid GetValueJid()
		{
			try
			{
				return new Jid(this.GetValue());
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Adds a value
		/// </summary>
		/// <remarks>
		/// you can call this function multiple times to add values to "multi" fields
		/// </remarks> 
		/// <param name="val"></param>
		public void AddValue(string val)
		{
			this.AddChild(new Value(val));
			//AddTag("value", val);
		}

		/// <summary>
		/// Adds multiple values to the already existing values from a string array
		/// </summary>
		/// <param name="vals"></param>
		public void AddValues(string[] vals)
		{
			if (vals.Length > 0)
			{
				foreach (var s in vals)
					this.AddValue(s);
			}
		}

		/// <summary>
		/// Adds multiple values. All already existing values will be removed
		/// </summary>
		/// <param name="vals"></param>
		public void SetValues(string[] vals)
		{
			var nl = this.SelectElements(typeof(Value));

			foreach (Element e in nl)
				e.Remove();

			this.AddValues(vals);
		}

		/// <summary>
		/// Gets all values for multi fields as Array
		/// </summary>
		/// <returns>string Array that contains all the values</returns>
		public string[] GetValues()
		{
			var nl = this.SelectElements(typeof(Value));
			var values = new string[nl.Count];
			var i = 0;
			foreach (Element val in nl)
			{
				values[i] = val.Value;
				i++;
			}
			return values;
		}

		public Option AddOption(string label, string val)
		{
			var opt = new Option(label, val);
			this.AddChild(opt);
			return opt;
		}

		public Option AddOption()
		{
			var opt = new Option();
			this.AddChild(opt);
			return opt;
		}

		public void AddOption(Option opt)
		{
			this.AddChild(opt);
		}

		public Option[] GetOptions()
		{
			var nl = this.SelectElements(typeof(Option));
			var i = 0;
			var result = new Option[nl.Count];
			foreach (Option o in nl)
			{
				result[i] = o;
				i++;
			}
			return result;
		}
		#endregion

	}

}
