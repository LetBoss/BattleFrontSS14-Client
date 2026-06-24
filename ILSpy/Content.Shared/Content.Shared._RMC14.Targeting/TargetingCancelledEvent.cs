using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Targeting;

[ByRefEvent]
public record struct TargetingCancelledEvent(bool Handled = false);
