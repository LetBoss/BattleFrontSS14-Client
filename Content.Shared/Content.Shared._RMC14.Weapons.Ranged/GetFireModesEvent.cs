using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Weapons.Ranged;

[ByRefEvent]
public record struct GetFireModesEvent(SelectiveFire Modes, SelectiveFire Set = SelectiveFire.Invalid);
