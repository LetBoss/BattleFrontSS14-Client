using Robust.Shared.GameObjects;

namespace Content.Shared.Mind;

[ByRefEvent]
public record struct MindRelayedEvent<TEvent>(TEvent Args);
