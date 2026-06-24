using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands;

public abstract class EquippedHandEvent : HandledEntityEventArgs
{
	public EntityUid User { get; }

	public EntityUid Equipped { get; }

	public Hand Hand { get; }

	public EquippedHandEvent(EntityUid user, EntityUid equipped, Hand hand)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Equipped = equipped;
		Hand = hand;
	}
}
