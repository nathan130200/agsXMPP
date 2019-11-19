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
using agsXMPP.Protocol.extensions.pubsub;

namespace agsXMPP.Protocol.client
{
	/// <summary>
	/// IQ Stanza Type.
	/// </summary>
	public readonly struct IqType : IEquatable<IqType>
	{
		private readonly string _type;

		internal IqType(string text) : this()
		{
			this._type = text;
		}

		public static IqType Of(string value)
		{
			switch (value)
			{
				case "get": return Get;
				case "set": return Set;
				case "result": return Result;
				case "error": return Error;
				default: throw new ArgumentOutOfRangeException(nameof(value));
			}
		}

		/// <summary>
		/// IQ: Get
		/// </summary>
		public static IqType Get { get; } = new IqType("get");

		/// <summary>
		/// IQ: Set
		/// </summary>
		public static IqType Set { get; } = new IqType("set");

		/// <summary>
		/// IQ: Result
		/// </summary>
		public static IqType Result { get; } = new IqType("result");

		/// <summary>
		/// IQ: Error
		/// </summary>
		public static IqType Error { get; } = new IqType("error");

		public override string ToString()
		{
			return this._type;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return false;

			if (ReferenceEquals(obj, this))
				return true;

			return this.Equals((IqType)obj);
		}

		public bool Equals(IqType other)
		{
			return other._type == this._type;
		}

		public static bool operator ==(IqType m1, IqType m2)
			=> Equals(m1, m2);

		public static bool operator !=(IqType m1, IqType m2)
			=> !(m1 == m2);
	}
}