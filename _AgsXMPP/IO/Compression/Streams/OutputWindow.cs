// OutputWindow.cs
//
// Copyright (C) 2001 Mike Krueger
//
// This file was translated from java, it was part of the GNU Classpath
// Copyright (C) 2001 Free Software Foundation, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// Linking this library statically or dynamically with other modules is
// making a combined work based on this library.  Thus, the terms and
// conditions of the GNU General Public License cover the whole
// combination.
// 
// As a special exception, the copyright holders of this library give you
// permission to link this library with independent modules to produce an
// executable, regardless of the license terms of these independent
// modules, and to copy and distribute the resulting executable under
// terms of your choice, provided that you also meet, for each linked
// independent module, the terms and conditions of the license of that
// module.  An independent module is a module which is not derived from
// or based on this library.  If you modify this library, you may extend
// this exception to your version of the library, but you are not
// obligated to do so.  If you do not wish to do so, delete this
// exception statement from your version.

using System;

namespace AgsXMPP.IO.Compression.Streams
{

	/// <summary>
	/// Contains the output from the Inflation process.
	/// We need to have a window so that we can refer backwards into the output stream
	/// to repeat stuff.<br/>
	/// Author of the original java version : John Leuner
	/// </summary>
	public class OutputWindow
	{
		private static int WINDOW_SIZE = 1 << 15;
		private static int WINDOW_MASK = WINDOW_SIZE - 1;

		private byte[] window = new byte[WINDOW_SIZE]; //The window is 2^15 bytes
		private int windowEnd = 0;
		private int windowFilled = 0;

		/// <summary>
		/// Write a byte to this output window
		/// </summary>
		/// <param name="abyte">value to write</param>
		/// <exception cref="InvalidOperationException">
		/// if window is full
		/// </exception>
		public void Write(int abyte)
		{
			if (this.windowFilled++ == WINDOW_SIZE)
			{
				throw new InvalidOperationException("Window full");
			}
			this.window[this.windowEnd++] = (byte)abyte;
			this.windowEnd &= WINDOW_MASK;
		}


		private void SlowRepeat(int repStart, int len, int dist)
		{
			while (len-- > 0)
			{
				this.window[this.windowEnd++] = this.window[repStart++];
				this.windowEnd &= WINDOW_MASK;
				repStart &= WINDOW_MASK;
			}
		}

		/// <summary>
		/// Append a byte pattern already in the window itself
		/// </summary>
		/// <param name="len">length of pattern to copy</param>
		/// <param name="dist">distance from end of window pattern occurs</param>
		/// <exception cref="InvalidOperationException">
		/// If the repeated data overflows the window
		/// </exception>
		public void Repeat(int len, int dist)
		{
			if ((this.windowFilled += len) > WINDOW_SIZE)
			{
				throw new InvalidOperationException("Window full");
			}

			var rep_start = (this.windowEnd - dist) & WINDOW_MASK;
			var border = WINDOW_SIZE - len;
			if (rep_start <= border && this.windowEnd < border)
			{
				if (len <= dist)
				{
					Array.Copy(this.window, rep_start, this.window, this.windowEnd, len);
					this.windowEnd += len;
				}
				else
				{
					/* We have to copy manually, since the repeat pattern overlaps. */
					while (len-- > 0)
					{
						this.window[this.windowEnd++] = this.window[rep_start++];
					}
				}
			}
			else
			{
				this.SlowRepeat(rep_start, len, dist);
			}
		}

		/// <summary>
		/// Copy from input manipulator to internal window
		/// </summary>
		/// <param name="input">source of data</param>
		/// <param name="len">length of data to copy</param>
		/// <returns>the number of bytes copied</returns>
		public int CopyStored(StreamManipulator input, int len)
		{
			len = Math.Min(Math.Min(len, WINDOW_SIZE - this.windowFilled), input.AvailableBytes);
			int copied;

			var tailLen = WINDOW_SIZE - this.windowEnd;
			if (len > tailLen)
			{
				copied = input.CopyBytes(this.window, this.windowEnd, tailLen);
				if (copied == tailLen)
				{
					copied += input.CopyBytes(this.window, 0, len - tailLen);
				}
			}
			else
			{
				copied = input.CopyBytes(this.window, this.windowEnd, len);
			}

			this.windowEnd = (this.windowEnd + copied) & WINDOW_MASK;
			this.windowFilled += copied;
			return copied;
		}

		/// <summary>
		/// Copy dictionary to window
		/// </summary>
		/// <param name="dict">source dictionary</param>
		/// <param name="offset">offset of start in source dictionary</param>
		/// <param name="len">length of dictionary</param>
		/// <exception cref="InvalidOperationException">
		/// If window isnt empty
		/// </exception>
		public void CopyDict(byte[] dict, int offset, int len)
		{
			if (this.windowFilled > 0)
			{
				throw new InvalidOperationException();
			}

			if (len > WINDOW_SIZE)
			{
				offset += len - WINDOW_SIZE;
				len = WINDOW_SIZE;
			}
			Array.Copy(dict, offset, this.window, 0, len);
			this.windowEnd = len & WINDOW_MASK;
		}

		/// <summary>
		/// Get remaining unfilled space in window
		/// </summary>
		/// <returns>Number of bytes left in window</returns>
		public int GetFreeSpace()
		{
			return WINDOW_SIZE - this.windowFilled;
		}

		/// <summary>
		/// Get bytes available for output in window
		/// </summary>
		/// <returns>Number of bytes filled</returns>
		public int GetAvailable()
		{
			return this.windowFilled;
		}

		/// <summary>
		/// Copy contents of window to output
		/// </summary>
		/// <param name="output">buffer to copy to</param>
		/// <param name="offset">offset to start at</param>
		/// <param name="len">number of bytes to count</param>
		/// <returns>The number of bytes copied</returns>
		/// <exception cref="InvalidOperationException">
		/// If a window underflow occurs
		/// </exception>
		public int CopyOutput(byte[] output, int offset, int len)
		{
			var copy_end = this.windowEnd;
			if (len > this.windowFilled)
			{
				len = this.windowFilled;
			}
			else
			{
				copy_end = (this.windowEnd - this.windowFilled + len) & WINDOW_MASK;
			}

			var copied = len;
			var tailLen = len - copy_end;

			if (tailLen > 0)
			{
				Array.Copy(this.window, WINDOW_SIZE - tailLen, output, offset, tailLen);
				offset += tailLen;
				len = copy_end;
			}
			Array.Copy(this.window, copy_end - len, output, offset, len);
			this.windowFilled -= copied;
			if (this.windowFilled < 0)
			{
				throw new InvalidOperationException();
			}
			return copied;
		}

		/// <summary>
		/// Reset by clearing window so <see cref="GetAvailable">GetAvailable</see> returns 0
		/// </summary>
		public void Reset()
		{
			this.windowFilled = this.windowEnd = 0;
		}
	}
}
