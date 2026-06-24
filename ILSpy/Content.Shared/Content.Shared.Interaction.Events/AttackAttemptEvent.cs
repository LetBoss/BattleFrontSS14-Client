using Content.Shared.Weapons.Melee;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

public sealed class AttackAttemptEvent : CancellableEntityEventArgs
{
	public EntityUid Uid { get; }

	public EntityUid? Target { get; }

	public Entity<MeleeWeaponComponent>? Weapon { get; }

	public bool Disarm { get; }

	public AttackAttemptEvent(EntityUid uid, EntityUid? target = null, Entity<MeleeWeaponComponent>? weapon = null, bool disarm = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		Target = target;
		Weapon = weapon;
		Disarm = disarm;
	}
}
