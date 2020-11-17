using System.ComponentModel;

namespace AgsXMPP.Xml
{
	public partial class StreamParser
	{
		protected EventHandlerList HandlerList = new EventHandlerList();

		static readonly object StreamStartEvent = new object();
		static readonly object StreamEndEvent = new object();
		static readonly object StreamElementEvent = new object();
		static readonly object StreamErrorEvent = new object();

		public event StreamNodeHandler StreamStarted
		{
			add => this.HandlerList.AddHandler(StreamStartEvent, value);
			remove => this.HandlerList.RemoveHandler(StreamStartEvent, value);
		}

		public event StreamNodeHandler StreamEnded
		{
			add => this.HandlerList.AddHandler(StreamEndEvent, value);
			remove => this.HandlerList.RemoveHandler(StreamEndEvent, value);
		}

		public event StreamNodeHandler StreamElementReceived
		{
			add => this.HandlerList.AddHandler(StreamElementEvent, value);
			remove => this.HandlerList.RemoveHandler(StreamElementEvent, value);
		}

		public event StreamErrorHandler StreamErrored
		{
			add => this.HandlerList.AddHandler(StreamErrorEvent, value);
			remove => this.HandlerList.RemoveHandler(StreamErrorEvent, value);
		}
	}
}