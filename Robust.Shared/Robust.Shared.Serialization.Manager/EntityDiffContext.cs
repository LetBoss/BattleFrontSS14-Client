namespace Robust.Shared.Serialization.Manager;

public sealed class EntityDiffContext : ISerializationContext
{
	public SerializationManager.SerializerProvider SerializerProvider { get; }

	public bool WritingReadingPrototypes { get; set; } = true;

	public EntityDiffContext()
	{
		SerializerProvider = new SerializationManager.SerializerProvider();
		SerializerProvider.RegisterSerializer(this);
	}
}
