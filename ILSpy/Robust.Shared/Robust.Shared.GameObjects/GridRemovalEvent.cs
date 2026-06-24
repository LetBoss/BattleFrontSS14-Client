namespace Robust.Shared.GameObjects;

public sealed class GridRemovalEvent : EntityEventArgs
{
	public EntityUid EntityUid { get; }

	public GridRemovalEvent(EntityUid uid)
	{
		EntityUid = uid;
	}
}
