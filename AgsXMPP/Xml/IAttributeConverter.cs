namespace AgsXMPP.Xml
{
	public interface IAttributeConverter
	{

	}

	public interface IAttributeConverter<T> : IAttributeConverter
	{
		Optional<T> Deserialize(string value);
		string Serialize(T value);
	}
}