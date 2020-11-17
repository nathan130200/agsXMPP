using System.Globalization;

namespace AgsXMPP.Xml.Converters
{
	public class UInt64AttributeConverter : IAttributeConverter<ulong>
	{
		public Optional<ulong> Deserialize(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<ulong>();

			return ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var temp)
				? Optional.FromValue(temp)
				: Optional.FromNonValue<ulong>();
		}

		public string Serialize(ulong value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
