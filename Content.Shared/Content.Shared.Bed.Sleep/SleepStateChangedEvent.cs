using Robust.Shared.GameObjects;

namespace Content.Shared.Bed.Sleep;

[ByRefEvent]
public record struct SleepStateChangedEvent(bool FellAsleep);
