using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing;

public abstract class ThrowEvent : HandledEntityEventArgs
{
	public readonly EntityUid Thrown;

	public readonly EntityUid Target;

	public ThrownItemComponent Component;

	public ThrowEvent(EntityUid thrown, EntityUid target, ThrownItemComponent component)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Thrown = thrown;
		Target = target;
		Component = component;
	}
}
