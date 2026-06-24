using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Weapons.Ranged.Events;

public sealed class TakeAmmoEvent : EntityEventArgs
{
	public readonly EntityUid? User;

	public int Shots;

	public List<(EntityUid? Entity, IShootable Shootable)> Ammo;

	public string? Reason;

	public EntityCoordinates Coordinates;

	public TakeAmmoEvent(int shots, List<(EntityUid? Entity, IShootable Shootable)> ammo, EntityCoordinates coordinates, EntityUid? user)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Shots = shots;
		Ammo = ammo;
		Coordinates = coordinates;
		User = user;
	}
}
