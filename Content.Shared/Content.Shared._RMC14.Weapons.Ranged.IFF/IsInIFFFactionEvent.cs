using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Weapons.Ranged.IFF;

[ByRefEvent]
public record struct IsInIFFFactionEvent(EntProtoId Faction, bool InFaction = false, SlotFlags TargetSlots = SlotFlags.IDCARD) : IInventoryRelayEvent
{
	public void TryHandle(EntProtoId? id)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!InFaction)
		{
			EntProtoId? val = id;
			EntProtoId faction = Faction;
			if (val.HasValue && val.GetValueOrDefault() == faction)
			{
				InFaction = true;
			}
		}
	}
}
