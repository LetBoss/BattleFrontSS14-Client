using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Weapons.Melee.Events;

public sealed class AttackedEvent : EntityEventArgs
{
	public DamageSpecifier BonusDamage = new DamageSpecifier();

	public EntityUid Used { get; }

	public EntityUid User { get; }

	public EntityCoordinates ClickLocation { get; }

	public AttackedEvent(EntityUid used, EntityUid user, EntityCoordinates clickLocation)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Used = used;
		User = user;
		ClickLocation = clickLocation;
	}
}
