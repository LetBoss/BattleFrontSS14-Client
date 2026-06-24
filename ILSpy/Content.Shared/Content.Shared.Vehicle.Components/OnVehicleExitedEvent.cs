using Robust.Shared.GameObjects;

namespace Content.Shared.Vehicle.Components;

[ByRefEvent]
public readonly record struct OnVehicleExitedEvent(Entity<VehicleComponent> Vehicle, EntityUid Operator);
