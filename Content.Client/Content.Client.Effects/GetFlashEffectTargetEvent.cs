using Robust.Shared.GameObjects;

namespace Content.Client.Effects;

[ByRefEvent]
public record struct GetFlashEffectTargetEvent(EntityUid Target);
