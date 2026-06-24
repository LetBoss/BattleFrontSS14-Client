using Robust.Shared.GameObjects;

namespace Content.Shared._CIV14merka.Mortar;

[ByRefEvent]
public readonly record struct CivMortarShellFiredEvent(EntityUid Gunner);
