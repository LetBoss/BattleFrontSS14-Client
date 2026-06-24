using Content.Shared.Damage;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Damage;

public sealed class DamageModifyAfterResistEvent : EntityEventArgs, IInventoryRelayEvent
{
	public readonly DamageSpecifier OriginalDamage;

	public DamageSpecifier Damage;

	public EntityUid? Origin;

	public EntityUid? Tool;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public DamageModifyAfterResistEvent(DamageSpecifier damage, EntityUid? origin = null, EntityUid? tool = null)
	{
		OriginalDamage = damage;
		Damage = damage;
		Origin = origin;
		Tool = tool;
	}
}
