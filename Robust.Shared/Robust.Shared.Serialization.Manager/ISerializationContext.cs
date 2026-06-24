namespace Robust.Shared.Serialization.Manager;

public interface ISerializationContext
{
	SerializationManager.SerializerProvider SerializerProvider { get; }

	bool WritingReadingPrototypes { get; }
}
