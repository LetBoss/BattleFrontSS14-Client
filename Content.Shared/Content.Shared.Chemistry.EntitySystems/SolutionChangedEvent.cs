using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public readonly record struct SolutionChangedEvent(Entity<SolutionComponent> Solution);
