namespace AgsXMPP
{
	public static class Optional
	{
		public static Optional<T> FromValue<T>(T value) => new Optional<T>(value);
		public static Optional<T> FromNonValue<T>() => new Optional<T>();
	}

	public struct Optional<T>
	{
		public T Value { get; }
		public bool HasValue { get; }

		public Optional(T value)
		{
			this.Value = value;
			this.HasValue = !(value is null);
		}

		public static implicit operator Optional<T>(T value)
			=> Optional.FromValue(value);
	}
}