using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components;

public abstract class BaseAnchoredEvent : EntityEventArgs
{
	public EntityUid User { get; }

	public EntityUid Tool { get; }

	protected BaseAnchoredEvent(EntityUid user, EntityUid tool)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Tool = tool;
	}
}
