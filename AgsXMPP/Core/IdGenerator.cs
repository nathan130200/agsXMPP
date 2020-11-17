using System;

namespace AgsXMPP.Core
{
	public delegate string IdGeneratorFactory();

	public class IdGenerator
	{
		public static IdGenerator Shared { get; } = new IdGenerator(IdGeneratorType.Guid);

		public IdGeneratorType Type { get; }
		public IdGeneratorFactory Factory { internal get; set; }
		public bool IncludePrefix { internal get; set; } = false;
		public string GuidFormat { internal get; set; } = "D";
		public string Prefix { internal get; set; } = "uid";
		public uint Sequence => this.RawSequence;

		protected uint RawSequence;

		public IdGenerator(IdGeneratorType type)
			=> this.Type = type;

		public string GenerateId()
		{
			if (this.Type == IdGeneratorType.Guid)
				return string.Concat(this.IncludePrefix ? this.Prefix : "", Guid.NewGuid().ToString(this.GuidFormat));

			lock (this)
			{
				if (this.Type == IdGeneratorType.Custom)
				{
					var result = this.Factory?.Invoke();

					if (string.IsNullOrEmpty(result))
						throw new ArgumentNullException(nameof(result));

					return string.Concat(this.IncludePrefix ? this.Prefix : "", result);
				}

				var inc = this.RawSequence++;
				return string.Concat(this.Prefix, inc);
			}
		}
	}
}