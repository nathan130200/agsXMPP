using System.Globalization;

namespace AgsXMPP.Xml.Converters
{
	public class Int16AttributeConverter : IAttributeConverter<short>
	{
		public Optional<short> Deserialize(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<short>();

			return short.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var temp)
				? Optional.FromValue(temp)
				: Optional.FromNonValue<short>();
		}

		public string Serialize(short value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}