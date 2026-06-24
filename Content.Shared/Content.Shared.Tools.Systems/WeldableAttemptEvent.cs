using Robust.Shared.GameObjects;

namespace Content.Shared.Tools.Systems;

public sealed class WeldableAttemptEvent : CancellableEntityEventArgs
{
	public readonly EntityUid User;

	public readonly EntityUid Tool;

	public WeldableAttemptEvent(EntityUid user, EntityUid tool)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Tool = tool;
	}
}
