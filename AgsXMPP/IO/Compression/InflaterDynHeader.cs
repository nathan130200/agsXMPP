// InflaterDynHeader.cs
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

using AgsXMPP.IO.Compression.Streams;

namespace AgsXMPP.IO.Compression
{

	class InflaterDynHeader
	{
		const int LNUM = 0;
		const int DNUM = 1;
		const int BLNUM = 2;
		const int BLLENS = 3;
		const int LENS = 4;
		const int REPS = 5;

		static readonly int[] repMin = { 3, 3, 11 };
		static readonly int[] repBits = { 2, 3, 7 };

		byte[] blLens;
		byte[] litdistLens;

		InflaterHuffmanTree blTree;

		int mode;
		int lnum, dnum, blnum, num;
		int repSymbol;
		byte lastLen;
		int ptr;

		static readonly int[] BL_ORDER =
		{ 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 };

		public InflaterDynHeader()
		{
		}

		public bool Decode(StreamManipulator input)
		{
		decode_loop:
			for (; ; )
			{
				switch (this.mode)
				{
					case LNUM:
						this.lnum = input.PeekBits(5);
						if (this.lnum < 0)
						{
							return false;
						}
						this.lnum += 257;
						input.DropBits(5);
						//  	    System.err.println("LNUM: "+lnum);
						this.mode = DNUM;
						goto case DNUM; // fall through
					case DNUM:
						this.dnum = input.PeekBits(5);
						if (this.dnum < 0)
						{
							return false;
						}
						this.dnum++;
						input.DropBits(5);
						//  	    System.err.println("DNUM: "+dnum);
						this.num = this.lnum + this.dnum;
						this.litdistLens = new byte[this.num];
						this.mode = BLNUM;
						goto case BLNUM; // fall through
					case BLNUM:
						this.blnum = input.PeekBits(4);
						if (this.blnum < 0)
						{
							return false;
						}
						this.blnum += 4;
						input.DropBits(4);
						this.blLens = new byte[19];
						this.ptr = 0;
						//  	    System.err.println("BLNUM: "+blnum);
						this.mode = BLLENS;
						goto case BLLENS; // fall through
					case BLLENS:
						while (this.ptr < this.blnum)
						{
							var len = input.PeekBits(3);
							if (len < 0)
							{
								return false;
							}
							input.DropBits(3);
							//  		System.err.println("blLens["+BL_ORDER[ptr]+"]: "+len);
							this.blLens[BL_ORDER[this.ptr]] = (byte)len;
							this.ptr++;
						}
						this.blTree = new InflaterHuffmanTree(this.blLens);
						this.blLens = null;
						this.ptr = 0;
						this.mode = LENS;
						goto case LENS; // fall through
					case LENS:
						{
							int symbol;
							while (((symbol = this.blTree.GetSymbol(input)) & ~15) == 0)
							{
								/* Normal case: symbol in [0..15] */

								//  		  System.err.println("litdistLens["+ptr+"]: "+symbol);
								this.litdistLens[this.ptr++] = this.lastLen = (byte)symbol;

								if (this.ptr == this.num)
								{
									/* Finished */
									return true;
								}
							}

							/* need more input ? */
							if (symbol < 0)
							{
								return false;
							}

							/* otherwise repeat code */
							if (symbol >= 17)
							{
								/* repeat zero */
								//  		  System.err.println("repeating zero");
								this.lastLen = 0;
							}
							else
							{
								if (this.ptr == 0)
								{
									throw new SharpZipBaseException();
								}
							}
							this.repSymbol = symbol - 16;
						}
						this.mode = REPS;
						goto case REPS; // fall through
					case REPS:
						{
							var bits = repBits[this.repSymbol];
							var count = input.PeekBits(bits);
							if (count < 0)
							{
								return false;
							}
							input.DropBits(bits);
							count += repMin[this.repSymbol];
							//  	      System.err.println("litdistLens repeated: "+count);

							if (this.ptr + count > this.num)
							{
								throw new SharpZipBaseException();
							}
							while (count-- > 0)
							{
								this.litdistLens[this.ptr++] = this.lastLen;
							}

							if (this.ptr == this.num)
							{
								/* Finished */
								return true;
							}
						}
						this.mode = LENS;
						goto decode_loop;
				}
			}
		}

		public InflaterHuffmanTree BuildLitLenTree()
		{
			var litlenLens = new byte[this.lnum];
			Array.Copy(this.litdistLens, 0, litlenLens, 0, this.lnum);
			return new InflaterHuffmanTree(litlenLens);
		}

		public InflaterHuffmanTree BuildDistTree()
		{
			var distLens = new byte[this.dnum];
			Array.Copy(this.litdistLens, this.lnum, distLens, 0, this.dnum);
			return new InflaterHuffmanTree(distLens);
		}
	}
}
