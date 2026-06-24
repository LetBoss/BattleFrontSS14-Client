using Robust.Shared.GameObjects;

namespace Content.Shared.Atmos.Rotting;

[ByRefEvent]
public record struct IsRottingEvent(bool Handled = false);
