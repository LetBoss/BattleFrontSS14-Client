using Robust.Shared.GameObjects;

namespace Content.Shared.Implants;

public sealed class AddImplantAttemptEvent : CancellableEntityEventArgs
{
	public readonly EntityUid User;

	public readonly EntityUid Target;

	public readonly EntityUid Implant;

	public readonly EntityUid Implanter;

	public AddImplantAttemptEvent(EntityUid user, EntityUid target, EntityUid implant, EntityUid implanter)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Target = target;
		Implant = implant;
		Implanter = implanter;
	}
}
