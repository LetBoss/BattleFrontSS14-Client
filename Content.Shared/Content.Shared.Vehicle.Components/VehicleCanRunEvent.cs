using Robust.Shared.GameObjects;

namespace Content.Shared.Vehicle.Components;

[ByRefEvent]
public record struct VehicleCanRunEvent(Entity<VehicleComponent> Vehicle, bool CanRun = true);
