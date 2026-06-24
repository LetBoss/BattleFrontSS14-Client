using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Melee;

public sealed class GetMeleeWeaponEvent : HandledEntityEventArgs
{
	public EntityUid? Weapon;
}
