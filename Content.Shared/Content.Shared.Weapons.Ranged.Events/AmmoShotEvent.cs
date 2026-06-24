using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Events;

public sealed class AmmoShotEvent : EntityEventArgs
{
	public List<EntityUid> FiredProjectiles;
}
