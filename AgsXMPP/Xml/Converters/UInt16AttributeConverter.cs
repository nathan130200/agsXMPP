using System.Globalization;

namespace AgsXMPP.Xml.Converters
{
	public class UInt16AttributeConverter : IAttributeConverter<ushort>
	{
		public Optional<ushort> Deserialize(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<ushort>();

			return ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var temp)
				? Optional.FromValue(temp)
				: Optional.FromNonValue<ushort>();
		}

		public string Serialize(ushort value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}