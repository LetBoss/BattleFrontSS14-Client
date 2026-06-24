using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Shields;

[ByRefEvent]
public readonly record struct RemovedShieldEvent(XenoShieldSystem.ShieldType Type);
