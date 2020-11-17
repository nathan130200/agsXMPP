using System.Globalization;

namespace AgsXMPP.Xml.Converters
{
	public class DoubleAttributeConverter : IAttributeConverter<double>
	{
		public static string DefaultFormat = "F6";

		public Optional<double> Deserialize(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<double>();

			return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var temp)
				? Optional.FromValue(temp)
				: Optional.FromNonValue<double>();
		}

		public string Serialize(double value)
		{
			return value.ToString(DefaultFormat, CultureInfo.InvariantCulture);
		}
	}
}