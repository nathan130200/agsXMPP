using System.Globalization;

namespace AgsXMPP.Xml.Converters
{
	public class Int64AttributeConverter : IAttributeConverter<long>
	{
		public Optional<long> Deserialize(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<long>();

			return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var temp)
				? Optional.FromValue(temp)
				: Optional.FromNonValue<long>();
		}

		public string Serialize(long value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
