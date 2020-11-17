using System;
using System.IO;

namespace AgsXMPP.Xml.Xpnet
{
	public sealed class BufferAggregate : IDisposable
	{
		internal BufferAggregateNode Head, Tail;
		internal MemoryStream Stream;

		public BufferAggregate()
		{
			this.Stream = new MemoryStream();
		}

		public void Write(byte[] buffer)
			=> this.Write(buffer, 0, buffer.Length);

		public void Write(byte[] buffer, int offset, int length)
		{
			this.Stream.Write(buffer, offset, length);

			var newBuffer = new byte[length];
			Array.Copy(buffer, offset, newBuffer, 0, length);

			if (this.Tail == null)
			{
				this.Head = this.Tail = new BufferAggregateNode();
				this.Head.Buffer = newBuffer;
			}
			else
			{
				var node = new BufferAggregateNode();
				node.Buffer = buffer;
				this.Tail.Next = node;
				this.Tail = node;
			}
		}

		public byte[] GetBuffer()
			=> this.Stream.ToArray();

		public void Clear(int offset)
		{
			var length = 0;
			var saved = -1;

			BufferAggregateNode node;

			for (node = this.Head; node != null; node = node.Next)
			{
				if (length + node.Buffer.Length <= offset)
				{
					if (length + node.Buffer.Length == offset)
					{
						node = node.Next;
						break;
					}

					length += node.Buffer.Length;
				}
				else
				{
					saved = length + node.Buffer.Length - offset;
					break;
				}
			}

			this.Head = node;

			if (this.Head == null)
				this.Tail = null;

			if (saved > 0)
			{
				var newBuffer = new byte[saved];
				Buffer.BlockCopy(this.Head.Buffer, this.Head.Buffer.Length - saved, newBuffer, 0, saved);
				this.Head.Buffer = newBuffer;
			}

			this.Stream.SetLength(0);

			for (node = this.Head; node != null; node = node.Next)
				this.Stream.Write(node.Buffer, 0, node.Buffer.Length);
		}

		public void Dispose()
		{
			if (this.Stream != null)
			{
				this.Stream.Dispose();
				this.Stream = null;
			}

			BufferAggregateNode node;

			for (node = this.Head; node != null; node = node.Next)
				node.Buffer = null;

			this.Head = this.Tail = null;
		}

		public override int GetHashCode()
			=> this.Stream.GetHashCode();

		public override string ToString()
			=> System.Text.Encoding.UTF8.GetString(this.GetBuffer());
	}

	internal sealed class BufferAggregateNode
	{
		public byte[] Buffer;
		public BufferAggregateNode Next;
	}
}
