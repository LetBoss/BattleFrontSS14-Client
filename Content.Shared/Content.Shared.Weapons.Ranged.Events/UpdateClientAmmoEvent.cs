using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Events;

[ByRefEvent]
public readonly record struct UpdateClientAmmoEvent(int AritifialIncrease = 0);
