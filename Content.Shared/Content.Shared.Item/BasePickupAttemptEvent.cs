using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Shared.Item;

[Virtual]
public class BasePickupAttemptEvent : CancellableEntityEventArgs
{
	public readonly EntityUid User;

	public readonly EntityUid Item;

	public BasePickupAttemptEvent(EntityUid user, EntityUid item)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Item = item;
	}
}
