namespace Robust.Shared.GameObjects;

public sealed class GridStartupEvent : EntityEventArgs
{
	public EntityUid EntityUid { get; }

	public GridStartupEvent(EntityUid uid)
	{
		EntityUid = uid;
	}
}
