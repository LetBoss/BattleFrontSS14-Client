using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

public sealed class DroppedEvent : HandledEntityEventArgs
{
	public EntityUid User { get; }

	public DroppedEvent(EntityUid user)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		User = user;
	}
}
