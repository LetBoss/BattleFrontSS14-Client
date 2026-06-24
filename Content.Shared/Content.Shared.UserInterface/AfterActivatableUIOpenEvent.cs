using Robust.Shared.GameObjects;

namespace Content.Shared.UserInterface;

public sealed class AfterActivatableUIOpenEvent : EntityEventArgs
{
	public readonly EntityUid Actor;

	public EntityUid User { get; }

	public AfterActivatableUIOpenEvent(EntityUid who, EntityUid actor)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = who;
		Actor = actor;
	}
}
