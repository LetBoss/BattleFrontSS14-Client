using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Weapons.Ranged.Systems;

[ByRefEvent]
public record struct GunShotEvent(EntityUid User, List<(EntityUid? Uid, IShootable Shootable)> Ammo, EntityCoordinates FromCoordinates, EntityCoordinates ToCoordinates);
