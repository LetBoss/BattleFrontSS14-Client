using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction;

public sealed class BeforeInteractHandEvent : HandledEntityEventArgs
{
	public EntityUid Target { get; }

	public BeforeInteractHandEvent(EntityUid target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Target = target;
	}
}
