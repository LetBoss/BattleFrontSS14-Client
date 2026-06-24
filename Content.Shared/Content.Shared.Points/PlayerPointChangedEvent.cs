using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;

namespace Content.Shared.Points;

[ByRefEvent]
public readonly record struct PlayerPointChangedEvent(NetUserId Player, FixedPoint2 Points);
