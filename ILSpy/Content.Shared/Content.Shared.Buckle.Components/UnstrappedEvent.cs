using Robust.Shared.GameObjects;

namespace Content.Shared.Buckle.Components;

[ByRefEvent]
public readonly record struct UnstrappedEvent(Entity<StrapComponent> Strap, Entity<BuckleComponent> Buckle);
