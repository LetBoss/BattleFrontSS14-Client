using Content.Shared.Damage;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Armor;

public sealed class CoefficientQueryEvent : EntityEventArgs, IInventoryRelayEvent
{
	public SlotFlags TargetSlots { get; set; }

	public DamageModifierSet DamageModifiers { get; set; } = new DamageModifierSet();

	public CoefficientQueryEvent(SlotFlags slots)
	{
		TargetSlots = slots;
	}
}
