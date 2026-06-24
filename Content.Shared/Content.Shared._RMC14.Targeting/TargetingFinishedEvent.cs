using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared._RMC14.Targeting;

[ByRefEvent]
public record struct TargetingFinishedEvent(EntityUid User, EntityCoordinates Coordinates, EntityUid Target, bool Handled = false);
