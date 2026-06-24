using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Attachable.Events;

public sealed class AttachableRelayedEvent<TEvent> : EntityEventArgs
{
	public TEvent Args;

	public EntityUid Holder;

	public AttachableRelayedEvent(TEvent args, EntityUid holder)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Args = args;
		Holder = holder;
	}
}
