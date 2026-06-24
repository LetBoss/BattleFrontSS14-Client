using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Components;

[ByRefEvent]
public readonly record struct GotRehydratedEvent(EntityUid Target);
