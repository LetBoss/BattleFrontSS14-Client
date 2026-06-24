using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Stealth;

[ByRefEvent]
public record struct ActiveInvisibleToggledEvent(bool enabled);
