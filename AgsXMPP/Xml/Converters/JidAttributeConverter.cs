namespace AgsXMPP.Xml.Converters
{
	public class JidAttributeConverter : IAttributeConverter<Jid>
	{
		public Optional<Jid> Deserialize(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<Jid>();

			return Jid.TryParse(value, out var temp)
				? Optional.FromValue(temp)
				: Optional.FromNonValue<Jid>();
		}

		public string Serialize(Jid value)
			=> value.ToString();
	}
}