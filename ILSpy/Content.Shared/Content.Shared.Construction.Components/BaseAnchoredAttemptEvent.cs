using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components;

public abstract class BaseAnchoredAttemptEvent : CancellableEntityEventArgs
{
	public EntityUid User { get; }

	public EntityUid Tool { get; }

	public float Delay { get; set; }

	protected BaseAnchoredAttemptEvent(EntityUid user, EntityUid tool)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Tool = tool;
	}
}
