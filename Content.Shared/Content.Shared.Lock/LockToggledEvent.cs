using Robust.Shared.GameObjects;

namespace Content.Shared.Lock;

[ByRefEvent]
public readonly record struct LockToggledEvent(bool Locked);
