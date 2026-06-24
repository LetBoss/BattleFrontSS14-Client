using Robust.Shared.GameObjects;

namespace Content.Shared.Ninja.Systems;

[ByRefEvent]
public record struct EmaggedSomethingEvent(EntityUid Target);
