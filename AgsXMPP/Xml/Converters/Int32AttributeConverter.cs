using System.Globalization;

namespace AgsXMPP.Xml.Converters
{
	public class Int32AttributeConverter : IAttributeConverter<int>
	{
		public Optional<int> Deserialize(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<int>();

			return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var temp)
				? Optional.FromValue(temp)
				: Optional.FromNonValue<int>();
		}

		public string Serialize(int value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}