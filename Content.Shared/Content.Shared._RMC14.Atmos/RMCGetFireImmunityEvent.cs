using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Atmos;

[ByRefEvent]
public record struct RMCGetFireImmunityEvent(EntityUid? Fire, bool Ignite = true, bool Immune = false);
