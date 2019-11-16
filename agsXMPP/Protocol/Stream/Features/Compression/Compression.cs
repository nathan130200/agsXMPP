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

using agsXMPP.Protocol.extensions.compression;
using agsXMPP.Xml.Dom;

namespace agsXMPP.Protocol.stream.features.Compression
{
	public class Compression : Element
	{
		/*
         *  <compression xmlns='http://jabber.org/features/compress'>
         *      <method>zlib</method>
         *  </compression>
         * 
         * <stream:features>
         *      <starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>
         *      <compression xmlns='http://jabber.org/features/compress'>
         *          <method>zlib</method>
         *          <method>lzw</method>
         *      </compression>
         * </stream:features>
         */

		public Compression()
		{
			this.TagName = "compression";
			this.Namespace = Namespaces.FEATURE_COMPRESS;
		}

		/// <summary>
		/// method/algorithm used to compressing the stream
		/// </summary>
		public CompressionMethod Method
		{
			set
			{
				if (value != extensions.compression.CompressionMethod.Unknown)
					this.SetTag("method", value.ToString());
			}
			get
			{
				return this.GetTagEnum<extensions.compression.CompressionMethod>("method");
			}
		}

		/// <summary>
		/// Add a compression method/algorithm
		/// </summary>
		/// <param name="method"></param>
		public void AddMethod(extensions.compression.CompressionMethod method)
		{
			if (!this.SupportsMethod(method))
				this.AddChild(new Method(method));
		}

		/// <summary>
		/// Is the given compression method/algrithm supported?
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public bool SupportsMethod(extensions.compression.CompressionMethod method)
		{
			var nList = this.SelectElements(typeof(Method));
			foreach (Method m in nList)
			{
				if (m.CompressionMethod == method)
					return true;
			}
			return false;
		}

		public Method[] GetMethods()
		{
			var methods = this.SelectElements(typeof(Method));

			var items = new Method[methods.Count];
			var i = 0;
			foreach (Method m in methods)
			{
				items[i] = m;
				i++;
			}
			return items;
		}

	}
}
