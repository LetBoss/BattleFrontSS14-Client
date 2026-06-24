using Robust.Shared.GameObjects;

namespace Content.Shared.Alert;

public sealed class AlertSyncEvent : EntityEventArgs
{
	public EntityUid Euid { get; }

	public AlertSyncEvent(EntityUid euid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Euid = euid;
	}
}
