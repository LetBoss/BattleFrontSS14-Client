namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly struct EntityTerminatingEvent(Entity<MetaDataComponent> entity)
{
	public readonly Entity<MetaDataComponent> Entity = entity;
}
