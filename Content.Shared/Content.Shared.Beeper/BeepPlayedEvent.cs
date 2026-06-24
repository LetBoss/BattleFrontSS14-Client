using Robust.Shared.GameObjects;

namespace Content.Shared.Beeper;

[ByRefEvent]
public record struct BeepPlayedEvent(bool Muted);
