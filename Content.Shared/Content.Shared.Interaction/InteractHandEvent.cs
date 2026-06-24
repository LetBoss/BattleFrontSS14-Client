using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction;

public sealed class InteractHandEvent : HandledEntityEventArgs, ITargetedInteractEventArgs
{
	public EntityUid User { get; }

	public EntityUid Target { get; }

	public InteractHandEvent(EntityUid user, EntityUid target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Target = target;
	}
}
