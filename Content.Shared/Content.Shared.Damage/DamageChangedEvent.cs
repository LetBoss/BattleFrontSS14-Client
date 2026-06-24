using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared.Damage;

public sealed class DamageChangedEvent : EntityEventArgs
{
	public readonly DamageableComponent Damageable;

	public readonly DamageSpecifier? DamageDelta;

	public readonly bool DamageIncreased;

	public readonly bool InterruptsDoAfters;

	public readonly EntityUid? Origin;

	public readonly EntityUid? Tool;

	public DamageChangedEvent(DamageableComponent damageable, DamageSpecifier? damageDelta, bool interruptsDoAfters, EntityUid? origin, EntityUid? tool)
	{
		Damageable = damageable;
		DamageDelta = damageDelta;
		Origin = origin;
		Tool = tool;
		if (DamageDelta == null)
		{
			return;
		}
		foreach (FixedPoint2 value in DamageDelta.DamageDict.Values)
		{
			if (value > 0)
			{
				DamageIncreased = true;
				break;
			}
		}
		InterruptsDoAfters = interruptsDoAfters && DamageIncreased;
	}
}
