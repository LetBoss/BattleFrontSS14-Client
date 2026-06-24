using Robust.Shared.GameObjects;

namespace Content.Shared.Beam.Components;

public sealed class CreateBeamSuccessEvent : EntityEventArgs
{
	public readonly EntityUid User;

	public readonly EntityUid Target;

	public CreateBeamSuccessEvent(EntityUid user, EntityUid target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Target = target;
	}
}
