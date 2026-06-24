namespace Robust.Shared.GameObjects;

public sealed class GridInitializeEvent : EntityEventArgs
{
	public EntityUid EntityUid { get; }

	public GridInitializeEvent(EntityUid uid)
	{
		EntityUid = uid;
	}
}
