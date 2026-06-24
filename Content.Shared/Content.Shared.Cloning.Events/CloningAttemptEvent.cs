using Robust.Shared.GameObjects;

namespace Content.Shared.Cloning.Events;

[ByRefEvent]
public record struct CloningAttemptEvent(CloningSettingsPrototype Settings, bool Cancelled = false);
