using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Interaction;

public sealed class BeforeRangedInteractEvent : HandledEntityEventArgs
{
	public EntityUid User { get; }

	public EntityUid Used { get; }

	public EntityUid? Target { get; }

	public EntityCoordinates ClickLocation { get; }

	public bool CanReach { get; }

	public BeforeRangedInteractEvent(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool canReach)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Used = used;
		Target = target;
		ClickLocation = clickLocation;
		CanReach = canReach;
	}
}
