using Content.Shared.Chemistry.Components.SolutionManager;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Chemistry;

[ByRefEvent]
public readonly record struct VaporHitEvent(Entity<SolutionContainerManagerComponent> Solution, int Power);
