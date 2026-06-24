namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly record struct GridUidChangedEvent(Entity<TransformComponent, MetaDataComponent> Entity, EntityUid? OldGrid)
{
	public EntityUid? NewGrid => Entity.Comp1.GridUid;

	public EntityUid Uid => Entity.Owner;

	public TransformComponent Transform => Entity.Comp1;

	public MetaDataComponent Meta => Entity.Comp2;
}
