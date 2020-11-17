namespace AgsXMPP.Xml.Converters
{
	public class JidAttributeConverter : IAttributeConverter<Jid>
	{
		public Optional<Jid> ConvertFrom(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Optional.FromNonValue<Jid>();

			return new Jid(value);
		}

		public string ConvertTo(Jid value)
			=> value.ToString();
	}
}