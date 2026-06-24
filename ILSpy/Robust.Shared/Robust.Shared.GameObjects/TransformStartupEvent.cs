namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly struct TransformStartupEvent(Entity<TransformComponent> entity)
{
	public readonly Entity<TransformComponent> Entity = entity;

	public TransformComponent Component => Entity.Comp;
}
