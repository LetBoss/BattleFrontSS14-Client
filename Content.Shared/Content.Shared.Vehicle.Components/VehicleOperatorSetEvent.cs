using Robust.Shared.GameObjects;

namespace Content.Shared.Vehicle.Components;

[ByRefEvent]
public readonly record struct VehicleOperatorSetEvent(EntityUid? NewOperator, EntityUid? OldOperator);
