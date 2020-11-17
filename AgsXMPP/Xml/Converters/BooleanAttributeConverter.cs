namespace AgsXMPP.Xml.Converters
{
	public class BooleanAttributeConverter : IAttributeConverter<bool>
	{
		public static bool UseNumerAsBoolean = true;

		public Optional<bool> Deserialize(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<bool>();

			return bool.TryParse(value, out var temp)
				? Optional.FromValue(temp)
				: Optional.FromNonValue<bool>();
		}

		public string Serialize(bool value)
		{
			return UseNumerAsBoolean
				? (value ? "1" : "0")
				: (value ? "true" : "false");
		}
	}
}