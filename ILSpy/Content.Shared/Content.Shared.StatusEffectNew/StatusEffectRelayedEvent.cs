using Robust.Shared.GameObjects;

namespace Content.Shared.StatusEffectNew;

[ByRefEvent]
public record struct StatusEffectRelayedEvent<TEvent>(TEvent Args);
