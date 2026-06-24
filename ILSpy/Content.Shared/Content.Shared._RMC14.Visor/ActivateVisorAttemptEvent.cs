using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Visor;

[ByRefEvent]
public sealed class ActivateVisorAttemptEvent : CancellableEntityEventArgs
{
	public readonly EntityUid User;

	public ActivateVisorAttemptEvent(EntityUid user)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		User = user;
	}
}
