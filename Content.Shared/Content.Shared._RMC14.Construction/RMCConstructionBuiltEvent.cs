using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Construction;

[ByRefEvent]
public readonly record struct RMCConstructionBuiltEvent(EntityUid Built, EntityUid User);
