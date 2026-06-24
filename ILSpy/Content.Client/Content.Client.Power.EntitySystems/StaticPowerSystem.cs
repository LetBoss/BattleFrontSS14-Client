using Content.Client.Power.Components;
using Robust.Shared.GameObjects;

namespace Content.Client.Power.EntitySystems;

public static class StaticPowerSystem
{
	public static bool IsPowered(this EntitySystem system, EntityUid uid, IEntityManager entManager, ApcPowerReceiverComponent? receiver = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		if (receiver == null && !entManager.TryGetComponent<ApcPowerReceiverComponent>(uid, ref receiver))
		{
			return false;
		}
		return receiver.Powered;
	}
}
