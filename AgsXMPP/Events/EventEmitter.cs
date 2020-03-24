using System;
using System.Collections.Generic;

namespace AgsXMPP.Events
{
	public class EventEmitter<T>
		where T : Delegate
	{
		private List<T> HandlersInternal;

		public IReadOnlyList<T> Handlers
		{
			get
			{
				T[] temp;

				lock (this.HandlersInternal)
					temp = this.HandlersInternal.ToArray();

				return temp;
			}
		}

		public EventEmitter()
		{
			this.HandlersInternal = new List<T>();
		}

		public void Register(T handler)
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			lock (this.HandlersInternal)
				this.HandlersInternal.Add(handler);
		}

		public void Unregister(T handler)
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			lock (this.HandlersInternal)
				this.HandlersInternal.Remove(handler);
		}

		public void Clear()
		{
			lock (this.HandlersInternal)
				this.HandlersInternal.Clear();
		}

		public void Invoke(params object[] args)
		{
			var exceptions = new List<Exception>();

			foreach (var handler in this.Handlers)
			{
				try
				{
					handler.DynamicInvoke(args);
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
				}
			}

			if (exceptions.Count != 0)
				throw new AggregateException(exceptions);
		}
	}
}
