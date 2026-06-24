using Robust.Shared.GameObjects;

namespace Content.Shared.StatusEffectNew;

[ByRefEvent]
public readonly record struct StatusEffectRemovedEvent(EntityUid Target);
