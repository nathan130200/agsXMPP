using System.Globalization;

namespace AgsXMPP.Xml.Converters
{
	public class FloatAttributeConverter : IAttributeConverter<float>
	{
		public static string DefaultFormat = "F6";

		public Optional<float> Deserialize(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<float>();

			return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var temp)
				? Optional.FromValue(temp)
				: Optional.FromNonValue<float>();
		}

		public string Serialize(float value)
		{
			return value.ToString(DefaultFormat, CultureInfo.InvariantCulture);
		}
	}
}