using System;
using System.ComponentModel;
using AgsXMPP.Factory;
using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Xml
{
	public delegate void StreamNodeHandler(StreamParser parser, Node node);
	public delegate void StreamErrorHandler(StreamParser parser, Exception exception);

	public partial class StreamParser : IDisposable
	{
		static StreamParser()
		{
			ElementFactory.RegisterElementTypesFromCurrentAssembly();
		}



		public void Dispose()
		{
			if (this.HandlerList != null)
			{
				this.HandlerList.Dispose();
				this.HandlerList = null;
			}
		}
	}
}
