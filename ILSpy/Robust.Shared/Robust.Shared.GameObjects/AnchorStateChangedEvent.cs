namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly struct AnchorStateChangedEvent(EntityUid entity, TransformComponent transform, bool detaching = false)
{
	public readonly TransformComponent Transform = transform;

	public readonly bool Detaching = detaching;

	public EntityUid Entity { get; } = entity;

	public bool Anchored => Transform.Anchored;
}
