using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Melee.Events;

public sealed class MeleeHitEvent : HandledEntityEventArgs
{
	public readonly DamageSpecifier BaseDamage;

	public List<DamageModifierSet> ModifiersList = new List<DamageModifierSet>();

	public DamageSpecifier BonusDamage = new DamageSpecifier();

	public IReadOnlyList<EntityUid> HitEntities;

	public SoundSpecifier? HitSoundOverride;

	public readonly EntityUid User;

	public readonly EntityUid Weapon;

	public readonly Vector2? Direction;

	public bool IsHit = true;

	public MeleeHitEvent(List<EntityUid> hitEntities, EntityUid user, EntityUid weapon, DamageSpecifier baseDamage, Vector2? direction)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		HitEntities = hitEntities;
		User = user;
		Weapon = weapon;
		BaseDamage = baseDamage;
		Direction = direction;
	}
}
