using System;
using AgsXMPP.Xml.Xpnet.Encoding;

namespace AgsXMPP.Xml.Xpnet
{
	/// <summary>
	/// Represents a position in an entity.
	/// A position can be modified by <see cref="XmlEncoding.MovePosition(byte[], int, int, Position)"/>.
	/// </summary>
	public class Position : ICloneable, IEquatable<Position>
	{
		/// <summary>
		/// Creates a position for the start of an entity.
		/// The line number is 1 and the column number is 0.
		/// </summary>
		public Position()
		{
			this.LineNumber = 1;
			this.ColumnNumber = 0;
		}

		/**
		 * Returns the line number.
		 * The first line number is 1.
		 */
		public int LineNumber { get; set; }

		/**
		 * Returns the column number.
		 * The first column number is 0.
		 * A tab character is not treated specially.
		 */
		public int ColumnNumber { get; set; }

		/**
		 * Returns a copy of this position.
		 */
		public object Clone() => new Position
		{
			LineNumber = this.LineNumber,
			ColumnNumber = this.ColumnNumber
		};

		public override int GetHashCode()
			=> HashCode.Combine(this.LineNumber, this.ColumnNumber);

		public override string ToString()
			=> $"Line={this.LineNumber}; Column={this.ColumnNumber}";

		public override bool Equals(object obj)
		{
			return obj is Position other && this.Equals(other);
		}

		public bool Equals(Position other)
		{
			if (ReferenceEquals(other, null))
				return false;

			if (ReferenceEquals(other, this))
				return true;

			return this.LineNumber == other.LineNumber
				&& this.ColumnNumber == other.ColumnNumber;
		}

		public static bool operator ==(Position p1, Position p2)
			=> Equals(p1, p2);

		public static bool operator !=(Position p1, Position p2)
			=> !(p1 == p2);

		public static bool operator <(Position p1, Position p2)
		{
			if (p1.LineNumber == p2.LineNumber)
				return p1.LineNumber < p2.LineNumber;
			else
				return p1.ColumnNumber < p2.ColumnNumber;
		}

		public static bool operator >(Position p1, Position p2)
			=> !(p1 < p2);
	}
}