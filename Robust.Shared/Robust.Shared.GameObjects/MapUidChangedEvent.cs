using Robust.Shared.Map;

namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly record struct MapUidChangedEvent(Entity<TransformComponent, MetaDataComponent> Entity, EntityUid? OldMap, MapId OldMapId)
{
	public EntityUid? NewMap => Entity.Comp1.MapUid;

	public MapId? NewMapId => Entity.Comp1.MapID;

	public EntityUid Uid => Entity.Owner;

	public TransformComponent Transform => Entity.Comp1;

	public MetaDataComponent Meta => Entity.Comp2;
}
