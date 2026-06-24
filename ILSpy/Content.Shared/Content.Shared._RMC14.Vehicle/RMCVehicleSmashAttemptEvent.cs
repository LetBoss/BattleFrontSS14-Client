using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Vehicle;

[ByRefEvent]
public record struct RMCVehicleSmashAttemptEvent(EntityUid Vehicle, bool Handled = false);
