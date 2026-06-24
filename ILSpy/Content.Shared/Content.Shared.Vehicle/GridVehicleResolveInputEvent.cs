using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Vehicle;

[ByRefEvent]
public record struct GridVehicleResolveInputEvent(MoveButtons Buttons, Vector2i InputDirection, bool Handled = false);
