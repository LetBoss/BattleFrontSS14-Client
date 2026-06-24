using Robust.Shared.GameObjects;

namespace Content.Shared.Mind;

[ByRefEvent]
public record struct GetCharactedDeadIcEvent(bool? Dead);
