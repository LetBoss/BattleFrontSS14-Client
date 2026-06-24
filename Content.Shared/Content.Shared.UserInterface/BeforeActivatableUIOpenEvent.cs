using Robust.Shared.GameObjects;

namespace Content.Shared.UserInterface;

public sealed class BeforeActivatableUIOpenEvent : EntityEventArgs
{
	public EntityUid User { get; }

	public BeforeActivatableUIOpenEvent(EntityUid who)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		User = who;
	}
}
