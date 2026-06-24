using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing;

[ByRefEvent]
public record struct StopThrowEvent(EntityUid? User);
