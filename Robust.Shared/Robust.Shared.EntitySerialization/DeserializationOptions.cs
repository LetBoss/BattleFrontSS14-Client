namespace Robust.Shared.EntitySerialization;

public record struct DeserializationOptions()
{
	public static readonly DeserializationOptions Default = new DeserializationOptions();

	public bool StoreYamlUids = false;

	public bool InitializeMaps = false;

	public bool PauseMaps = false;

	public bool LogOrphanedGrids = true;

	public bool LogInvalidEntities = true;

	public bool AssignMapIds = true;
}
