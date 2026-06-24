using Robust.Shared.GameObjects;

namespace Content.Shared.IgnitionSource;

[ByRefEvent]
public readonly record struct IgnitionEvent(bool Ignite = false);
