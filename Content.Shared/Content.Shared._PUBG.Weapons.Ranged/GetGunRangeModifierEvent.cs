using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared._PUBG.Weapons.Ranged;

[ByRefEvent]
public record struct GetGunRangeModifierEvent(FixedPoint2 Multiplier);
