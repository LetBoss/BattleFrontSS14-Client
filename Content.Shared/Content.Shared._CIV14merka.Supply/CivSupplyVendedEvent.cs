using Robust.Shared.GameObjects;

namespace Content.Shared._CIV14merka.Supply;

[ByRefEvent]
public readonly record struct CivSupplyVendedEvent(int Section, int Entry);
