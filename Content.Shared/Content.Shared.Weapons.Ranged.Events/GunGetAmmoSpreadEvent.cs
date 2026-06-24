using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Weapons.Ranged.Events;

[ByRefEvent]
public record struct GunGetAmmoSpreadEvent(Angle Spread);
