using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Weapons.Ranged.Overheat;

[ByRefEvent]
public record struct HeatGainedEvent(float CurrentHeat);
