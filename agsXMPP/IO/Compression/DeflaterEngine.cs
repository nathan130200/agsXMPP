// DeflaterEngine.cs
//
// Copyright (C) 2001 Mike Krueger
// Copyright (C) 2004 John Reilly
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

using agsXMPP.IO.Compression.Checksums;

namespace agsXMPP.IO.Compression
{

	/// <summary>
	/// Strategies for deflater
	/// </summary>
	public enum DeflateStrategy
	{
		/// <summary>
		/// The default strategy
		/// </summary>
		Default = 0,

		/// <summary>
		/// This strategy will only allow longer string repetitions.  It is
		/// useful for random data with a small character set.
		/// </summary>
		Filtered = 1,


		/// <summary>
		/// This strategy will not look for string repetitions at all.  It
		/// only encodes with Huffman trees (which means, that more common
		/// characters get a smaller encoding.
		/// </summary>
		HuffmanOnly = 2
	}

	// DEFLATE ALGORITHM:
	// 
	// The uncompressed stream is inserted into the window array.  When
	// the window array is full the first half is thrown away and the
	// second half is copied to the beginning.
	//
	// The head array is a hash table.  Three characters build a hash value
	// and they the value points to the corresponding index in window of 
	// the last string with this hash.  The prev array implements a
	// linked list of matches with the same hash: prev[index & WMASK] points
	// to the previous index with the same hash.
	// 


	/// <summary>
	/// Low level compression engine for deflate algorithm which uses a 32K sliding window
	/// with secondary compression from Huffman/Shannon-Fano codes.
	/// </summary>
	public class DeflaterEngine : DeflaterConstants
	{
		static int TOO_FAR = 4096;

		int ins_h;

		/// <summary>
		/// Hashtable, hashing three characters to an index for window, so
		/// that window[index]..window[index+2] have this hash code.  
		/// Note that the array should really be unsigned short, so you need
		/// to and the values with 0xffff.
		/// </summary>
		short[] head;

		/// <summary>
		/// <code>prev[index &amp; WMASK]</code> points to the previous index that has the
		/// same hash code as the string starting at index.  This way 
		/// entries with the same hash code are in a linked list.
		/// Note that the array should really be unsigned short, so you need
		/// to and the values with 0xffff.
		/// </summary>
		short[] prev;

		int matchStart;
		int matchLen;
		bool prevAvailable;
		int blockStart;

		/// <summary>
		/// Points to the current character in the window.
		/// </summary>
		int strstart;

		/// <summary>
		/// lookahead is the number of characters starting at strstart in
		/// window that are valid.
		/// So window[strstart] until window[strstart+lookahead-1] are valid
		/// characters.
		/// </summary>
		int lookahead;

		/// <summary>
		/// This array contains the part of the uncompressed stream that 
		/// is of relevance.  The current character is indexed by strstart.
		/// </summary>
		byte[] window;

		DeflateStrategy strategy;
		int max_chain, max_lazy, niceLength, goodLength;

		/// <summary>
		/// The current compression function.
		/// </summary>
		int comprFunc;

		/// <summary>
		/// The input data for compression.
		/// </summary>
		byte[] inputBuf;

		/// <summary>
		/// The total bytes of input read.
		/// </summary>
		int totalIn;

		/// <summary>
		/// The offset into inputBuf, where input data starts.
		/// </summary>
		int inputOff;

		/// <summary>
		/// The end offset of the input data.
		/// </summary>
		int inputEnd;

		DeflaterPending pending;
		DeflaterHuffman huffman;

		/// <summary>
		/// The adler checksum
		/// </summary>
		Adler32 adler;

		/// <summary>
		/// Construct instance with pending buffer
		/// </summary>
		/// <param name="pending">
		/// Pending buffer to use
		/// </param>>
		public DeflaterEngine(DeflaterPending pending)
		{
			this.pending = pending;
			this.huffman = new DeflaterHuffman(pending);
			this.adler = new Adler32();

			this.window = new byte[2 * WSIZE];
			this.head = new short[HASH_SIZE];
			this.prev = new short[WSIZE];

			// We start at index 1, to avoid an implementation deficiency, that
			// we cannot build a repeat pattern at index 0.
			this.blockStart = this.strstart = 1;
		}

		/// <summary>
		/// Reset internal state
		/// </summary>		
		public void Reset()
		{
			this.huffman.Reset();
			this.adler.Reset();
			this.blockStart = this.strstart = 1;
			this.lookahead = 0;
			this.totalIn = 0;
			this.prevAvailable = false;
			this.matchLen = MIN_MATCH - 1;

			for (var i = 0; i < HASH_SIZE; i++)
			{
				this.head[i] = 0;
			}

			for (var i = 0; i < WSIZE; i++)
			{
				this.prev[i] = 0;
			}
		}

		/// <summary>
		/// Reset Adler checksum
		/// </summary>		
		public void ResetAdler()
		{
			this.adler.Reset();
		}

		/// <summary>
		/// Get current value of Adler checksum
		/// </summary>		
		public int Adler
		{
			get
			{
				return (int)this.adler.Value;
			}
		}

		/// <summary>
		/// Total data processed
		/// </summary>		
		public int TotalIn
		{
			get
			{
				return this.totalIn;
			}
		}

		/// <summary>
		/// Get/set the <see cref="DeflateStrategy">deflate strategy</see>
		/// </summary>		
		public DeflateStrategy Strategy
		{
			get
			{
				return this.strategy;
			}
			set
			{
				this.strategy = value;
			}
		}

		/// <summary>
		/// Set the deflate level (0-9)
		/// </summary>
		public void SetLevel(int lvl)
		{
			this.goodLength = GOOD_LENGTH[lvl];
			this.max_lazy = MAX_LAZY[lvl];
			this.niceLength = NICE_LENGTH[lvl];
			this.max_chain = MAX_CHAIN[lvl];

			if (COMPR_FUNC[lvl] != this.comprFunc)
			{
				/*
								if (DeflaterConstants.DEBUGGING) {
								   Console.WriteLine("Change from " + comprFunc + " to "
														  + DeflaterConstants.COMPR_FUNC[lvl]);
								}
				*/
				switch (this.comprFunc)
				{
					case DEFLATE_STORED:
						if (this.strstart > this.blockStart)
						{
							this.huffman.FlushStoredBlock(this.window, this.blockStart,
								this.strstart - this.blockStart, false);
							this.blockStart = this.strstart;
						}
						this.UpdateHash();
						break;
					case DEFLATE_FAST:
						if (this.strstart > this.blockStart)
						{
							this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart,
								false);
							this.blockStart = this.strstart;
						}
						break;
					case DEFLATE_SLOW:
						if (this.prevAvailable)
						{
							this.huffman.TallyLit(this.window[this.strstart - 1] & 0xff);
						}
						if (this.strstart > this.blockStart)
						{
							this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, false);
							this.blockStart = this.strstart;
						}
						this.prevAvailable = false;
						this.matchLen = MIN_MATCH - 1;
						break;
				}
				this.comprFunc = COMPR_FUNC[lvl];
			}
		}

		void UpdateHash()
		{
			/*
						if (DEBUGGING) {
							Console.WriteLine("updateHash: "+strstart);
						}
			*/
			this.ins_h = (this.window[this.strstart] << HASH_SHIFT) ^ this.window[this.strstart + 1];
		}

		/// <summary>
		/// Inserts the current string in the head hash and returns the previous
		/// value for this hash.
		/// </summary>
		/// <returns>The previous hash value</returns>
		int InsertString()
		{
			short match;
			var hash = ((this.ins_h << HASH_SHIFT) ^ this.window[this.strstart + (MIN_MATCH - 1)]) & HASH_MASK;
			/*
						if (DeflaterConstants.DEBUGGING) {
							if (hash != (((window[strstart] << (2*HASH_SHIFT)) ^ 
											  (window[strstart + 1] << HASH_SHIFT) ^ 
											  (window[strstart + 2])) & HASH_MASK)) {
									throw new SharpZipBaseException("hash inconsistent: " + hash + "/"
															+window[strstart] + ","
															+window[strstart+1] + ","
															+window[strstart+2] + "," + HASH_SHIFT);
								}
						}
			*/
			this.prev[this.strstart & WMASK] = match = this.head[hash];
			this.head[hash] = (short)this.strstart;
			this.ins_h = hash;
			return match & 0xffff;
		}

		void SlideWindow()
		{
			Array.Copy(this.window, WSIZE, this.window, 0, WSIZE);
			this.matchStart -= WSIZE;
			this.strstart -= WSIZE;
			this.blockStart -= WSIZE;

			/* Slide the hash table (could be avoided with 32 bit values
			 * at the expense of memory usage).
			 */
			for (var i = 0; i < HASH_SIZE; ++i)
			{
				var m = this.head[i] & 0xffff;
				this.head[i] = (short)(m >= WSIZE ? (m - WSIZE) : 0);
			}

			/* Slide the prev table. */
			for (var i = 0; i < WSIZE; i++)
			{
				var m = this.prev[i] & 0xffff;
				this.prev[i] = (short)(m >= WSIZE ? (m - WSIZE) : 0);
			}
		}

		/// <summary>
		/// Fill the window
		/// </summary>
		public void FillWindow()
		{
			/* If the window is almost full and there is insufficient lookahead,
			 * move the upper half to the lower one to make room in the upper half.
			 */
			if (this.strstart >= WSIZE + MAX_DIST)
			{
				this.SlideWindow();
			}

			/* If there is not enough lookahead, but still some input left,
			 * read in the input
			 */
			while (this.lookahead < MIN_LOOKAHEAD && this.inputOff < this.inputEnd)
			{
				var more = 2 * WSIZE - this.lookahead - this.strstart;

				if (more > this.inputEnd - this.inputOff)
				{
					more = this.inputEnd - this.inputOff;
				}

				Array.Copy(this.inputBuf, this.inputOff, this.window, this.strstart + this.lookahead, more);
				this.adler.Update(this.inputBuf, this.inputOff, more);

				this.inputOff += more;
				this.totalIn += more;
				this.lookahead += more;
			}

			if (this.lookahead >= MIN_MATCH)
			{
				this.UpdateHash();
			}
		}

		/// <summary>
		/// Find the best (longest) string in the window matching the 
		/// string starting at strstart.
		///
		/// Preconditions:
		/// <code>
		/// strstart + MAX_MATCH &lt;= window.length.</code>
		/// </summary>
		/// <param name="curMatch"></param>
		/// <returns>True if a match greater than the minimum length is found</returns>
		bool FindLongestMatch(int curMatch)
		{
			var chainLength = this.max_chain;
			var niceLength = this.niceLength;
			var prev = this.prev;
			var scan = this.strstart;
			int match;
			var best_end = this.strstart + this.matchLen;
			var best_len = Math.Max(this.matchLen, MIN_MATCH - 1);

			var limit = Math.Max(this.strstart - MAX_DIST, 0);

			var strend = this.strstart + MAX_MATCH - 1;
			var scan_end1 = this.window[best_end - 1];
			var scan_end = this.window[best_end];

			/* Do not waste too much time if we already have a good match: */
			if (best_len >= this.goodLength)
			{
				chainLength >>= 2;
			}

			/* Do not look for matches beyond the end of the input. This is necessary
			* to make deflate deterministic.
			*/
			if (niceLength > this.lookahead)
			{
				niceLength = this.lookahead;
			}

			/*
						if (DeflaterConstants.DEBUGGING && strstart > 2 * WSIZE - MIN_LOOKAHEAD) {
							throw new InvalidOperationException("need lookahead");
						}
			*/
			do
			{
				/*
								if (DeflaterConstants.DEBUGGING && curMatch >= strstart) {
									throw new InvalidOperationException("future match");
								}
				*/
				if (this.window[curMatch + best_len] != scan_end ||
					this.window[curMatch + best_len - 1] != scan_end1 ||
					this.window[curMatch] != this.window[scan] ||
					this.window[curMatch + 1] != this.window[scan + 1])
				{
					continue;
				}

				match = curMatch + 2;
				scan += 2;

				/* We check for insufficient lookahead only every 8th comparison;
				* the 256th check will be made at strstart + 258.
				*/
				while (this.window[++scan] == this.window[++match] &&
					this.window[++scan] == this.window[++match] &&
					this.window[++scan] == this.window[++match] &&
					this.window[++scan] == this.window[++match] &&
					this.window[++scan] == this.window[++match] &&
					this.window[++scan] == this.window[++match] &&
					this.window[++scan] == this.window[++match] &&
					this.window[++scan] == this.window[++match] && scan < strend) ;

				if (scan > best_end)
				{
					/*
										if (DeflaterConstants.DEBUGGING && ins_h == 0)
											System.err.println("Found match: "+curMatch+"-"+(scan-strstart));
					*/
					this.matchStart = curMatch;
					best_end = scan;
					best_len = scan - this.strstart;

					if (best_len >= niceLength)
					{
						break;
					}

					scan_end1 = this.window[best_end - 1];
					scan_end = this.window[best_end];
				}
				scan = this.strstart;
			} while ((curMatch = (prev[curMatch & WMASK] & 0xffff)) > limit && --chainLength != 0);

			this.matchLen = Math.Min(best_len, this.lookahead);
			return this.matchLen >= MIN_MATCH;
		}

		/// <summary>
		/// Set compression dictionary
		/// </summary>
		public void SetDictionary(byte[] buffer, int offset, int length)
		{
			/*
						if (DeflaterConstants.DEBUGGING && strstart != 1) {
							throw new InvalidOperationException("strstart not 1");
						}
			*/
			this.adler.Update(buffer, offset, length);
			if (length < MIN_MATCH)
			{
				return;
			}
			if (length > MAX_DIST)
			{
				offset += length - MAX_DIST;
				length = MAX_DIST;
			}

			Array.Copy(buffer, offset, this.window, this.strstart, length);

			this.UpdateHash();
			--length;
			while (--length > 0)
			{
				this.InsertString();
				this.strstart++;
			}
			this.strstart += 2;
			this.blockStart = this.strstart;
		}

		bool DeflateStored(bool flush, bool finish)
		{
			if (!flush && this.lookahead == 0)
			{
				return false;
			}

			this.strstart += this.lookahead;
			this.lookahead = 0;

			var storedLen = this.strstart - this.blockStart;

			if ((storedLen >= MAX_BLOCK_SIZE) || /* Block is full */
				(this.blockStart < WSIZE && storedLen >= MAX_DIST) ||   /* Block may move out of window */
				flush)
			{
				var lastBlock = finish;
				if (storedLen > MAX_BLOCK_SIZE)
				{
					storedLen = MAX_BLOCK_SIZE;
					lastBlock = false;
				}

				/*
								if (DeflaterConstants.DEBUGGING) {
								   Console.WriteLine("storedBlock["+storedLen+","+lastBlock+"]");
								}
				*/

				this.huffman.FlushStoredBlock(this.window, this.blockStart, storedLen, lastBlock);
				this.blockStart += storedLen;
				return !lastBlock;
			}
			return true;
		}

		private bool DeflateFast(bool flush, bool finish)
		{
			if (this.lookahead < MIN_LOOKAHEAD && !flush)
			{
				return false;
			}

			while (this.lookahead >= MIN_LOOKAHEAD || flush)
			{
				if (this.lookahead == 0)
				{
					/* We are flushing everything */
					this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, finish);
					this.blockStart = this.strstart;
					return false;
				}

				if (this.strstart > 2 * WSIZE - MIN_LOOKAHEAD)
				{
					/* slide window, as findLongestMatch needs this.
					 * This should only happen when flushing and the window
					 * is almost full.
					 */
					this.SlideWindow();
				}

				int hashHead;
				if (this.lookahead >= MIN_MATCH &&
					(hashHead = this.InsertString()) != 0 &&
					this.strategy != DeflateStrategy.HuffmanOnly &&
					this.strstart - hashHead <= MAX_DIST &&
					this.FindLongestMatch(hashHead))
				{
					/* longestMatch sets matchStart and matchLen */
					/*
										if (DeflaterConstants.DEBUGGING) {
											for (int i = 0 ; i < matchLen; i++) {
												if (window[strstart+i] != window[matchStart + i]) {
													throw new SharpZipBaseException("Match failure");
												}
											}
										}
					*/
					// -jr- Hak hak hak this stops problems with fast/low compression and index out of range
					if (this.huffman.TallyDist(this.strstart - this.matchStart, this.matchLen))
					{
						var lastBlock = finish && this.lookahead == 0;
						this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, lastBlock);
						this.blockStart = this.strstart;
					}

					this.lookahead -= this.matchLen;
					if (this.matchLen <= this.max_lazy && this.lookahead >= MIN_MATCH)
					{
						while (--this.matchLen > 0)
						{
							++this.strstart;
							this.InsertString();
						}
						++this.strstart;
					}
					else
					{
						this.strstart += this.matchLen;
						if (this.lookahead >= MIN_MATCH - 1)
						{
							this.UpdateHash();
						}
					}
					this.matchLen = MIN_MATCH - 1;
					continue;
				}
				else
				{
					/* No match found */
					this.huffman.TallyLit(this.window[this.strstart] & 0xff);
					++this.strstart;
					--this.lookahead;
				}

				if (this.huffman.IsFull())
				{
					var lastBlock = finish && this.lookahead == 0;
					this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, lastBlock);
					this.blockStart = this.strstart;
					return !lastBlock;
				}
			}
			return true;
		}

		bool DeflateSlow(bool flush, bool finish)
		{
			if (this.lookahead < MIN_LOOKAHEAD && !flush)
			{
				return false;
			}

			while (this.lookahead >= MIN_LOOKAHEAD || flush)
			{
				if (this.lookahead == 0)
				{
					if (this.prevAvailable)
					{
						this.huffman.TallyLit(this.window[this.strstart - 1] & 0xff);
					}
					this.prevAvailable = false;

					// We are flushing everything
					/*
										if (DeflaterConstants.DEBUGGING && !flush) {
											throw new SharpZipBaseException("Not flushing, but no lookahead");
										}
					*/
					this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart,
						finish);
					this.blockStart = this.strstart;
					return false;
				}

				if (this.strstart >= 2 * WSIZE - MIN_LOOKAHEAD)
				{
					/* slide window, as findLongestMatch need this.
					 * This should only happen when flushing and the window
					 * is almost full.
					 */
					this.SlideWindow();
				}

				var prevMatch = this.matchStart;
				var prevLen = this.matchLen;
				if (this.lookahead >= MIN_MATCH)
				{
					var hashHead = this.InsertString();
					if (this.strategy != DeflateStrategy.HuffmanOnly && hashHead != 0 && this.strstart - hashHead <= MAX_DIST && this.FindLongestMatch(hashHead))
					{
						/* longestMatch sets matchStart and matchLen */

						/* Discard match if too small and too far away */
						if (this.matchLen <= 5 && (this.strategy == DeflateStrategy.Filtered || (this.matchLen == MIN_MATCH && this.strstart - this.matchStart > TOO_FAR)))
						{
							this.matchLen = MIN_MATCH - 1;
						}
					}
				}

				/* previous match was better */
				if (prevLen >= MIN_MATCH && this.matchLen <= prevLen)
				{
					/*
										if (DeflaterConstants.DEBUGGING) {
										   for (int i = 0 ; i < matchLen; i++) {
											  if (window[strstart-1+i] != window[prevMatch + i])
												 throw new SharpZipBaseException();
											}
										}
					*/
					this.huffman.TallyDist(this.strstart - 1 - prevMatch, prevLen);
					prevLen -= 2;
					do
					{
						this.strstart++;
						this.lookahead--;
						if (this.lookahead >= MIN_MATCH)
						{
							this.InsertString();
						}
					} while (--prevLen > 0);
					this.strstart++;
					this.lookahead--;
					this.prevAvailable = false;
					this.matchLen = MIN_MATCH - 1;
				}
				else
				{
					if (this.prevAvailable)
					{
						this.huffman.TallyLit(this.window[this.strstart - 1] & 0xff);
					}
					this.prevAvailable = true;
					this.strstart++;
					this.lookahead--;
				}

				if (this.huffman.IsFull())
				{
					var len = this.strstart - this.blockStart;
					if (this.prevAvailable)
					{
						len--;
					}
					var lastBlock = (finish && this.lookahead == 0 && !this.prevAvailable);
					this.huffman.FlushBlock(this.window, this.blockStart, len, lastBlock);
					this.blockStart += len;
					return !lastBlock;
				}
			}
			return true;
		}

		/// <summary>
		/// Deflate drives actual compression of data
		/// </summary>
		public bool Deflate(bool flush, bool finish)
		{
			bool progress;
			do
			{
				this.FillWindow();
				var canFlush = flush && this.inputOff == this.inputEnd;
				//				if (DeflaterConstants.DEBUGGING) {
				//					//Console.WriteLine("window: ["+blockStart+","+strstart+","
				//					                  +lookahead+"], "+comprFunc+","+canFlush);
				//				}
				switch (this.comprFunc)
				{
					case DEFLATE_STORED:
						progress = this.DeflateStored(canFlush, finish);
						break;
					case DEFLATE_FAST:
						progress = this.DeflateFast(canFlush, finish);
						break;
					case DEFLATE_SLOW:
						progress = this.DeflateSlow(canFlush, finish);
						break;
					default:
						throw new InvalidOperationException("unknown comprFunc");
				}
			} while (this.pending.IsFlushed && progress); /* repeat while we have no pending output and progress was made */
			return progress;
		}


		/// <summary>
		/// Sets input data to be deflated.  Should only be called when <code>NeedsInput()</code>
		/// returns true
		/// </summary>
		/// <param name="buf">The buffer containing input data.</param>
		/// <param name="off">The index of the first byte of data.</param>
		/// <param name="len">The number of bytes of data to use as input.</param>
		public void SetInput(byte[] buf, int off, int len)
		{
			if (this.inputOff < this.inputEnd)
			{
				throw new InvalidOperationException("Old input was not completely processed");
			}

			var end = off + len;

			/* We want to throw an ArrayIndexOutOfBoundsException early.  The
			* check is very tricky: it also handles integer wrap around.
			*/
			if (0 > off || off > end || end > buf.Length)
			{
				throw new ArgumentOutOfRangeException();
			}

			this.inputBuf = buf;
			this.inputOff = off;
			this.inputEnd = end;
		}

		/// <summary>
		/// Return true if input is needed via <see cref="SetInput"> SetInput</see>
		/// </summary>		
		public bool NeedsInput()
		{
			return this.inputEnd == this.inputOff;
		}
	}
}
