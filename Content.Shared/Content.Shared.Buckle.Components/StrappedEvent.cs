using Robust.Shared.GameObjects;

namespace Content.Shared.Buckle.Components;

[ByRefEvent]
public readonly record struct StrappedEvent(Entity<StrapComponent> Strap, Entity<BuckleComponent> Buckle);
