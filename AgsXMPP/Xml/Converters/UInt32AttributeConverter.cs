using System.Globalization;

namespace AgsXMPP.Xml.Converters
{
	public class UInt32AttributeConverter : IAttributeConverter<uint>
	{
		public Optional<uint> Deserialize(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<uint>();

			return uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var temp)
				? Optional.FromValue(temp)
				: Optional.FromNonValue<uint>();
		}

		public string Serialize(uint value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
