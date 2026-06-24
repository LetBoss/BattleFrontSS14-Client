using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Damage;

public sealed class DamageModifyEvent : EntityEventArgs, IInventoryRelayEvent
{
	public readonly DamageSpecifier OriginalDamage;

	public DamageSpecifier Damage;

	public EntityUid? Origin;

	public EntityUid? Tool;

	public int ArmorPiercing;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public DamageModifyEvent(DamageSpecifier damage, EntityUid? origin = null, EntityUid? tool = null, int armorPiercing = 0)
	{
		OriginalDamage = damage;
		Damage = damage;
		Origin = origin;
		Tool = tool;
		ArmorPiercing = armorPiercing;
	}
}
