namespace Robust.Shared.GameObjects;

public sealed class GridAddEvent : EntityEventArgs
{
	public EntityUid EntityUid { get; }

	public GridAddEvent(EntityUid uid)
	{
		EntityUid = uid;
	}
}
