using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.StatusEffectNew;

[ByRefEvent]
public record struct BeforeStatusEffectAddedEvent(EntProtoId Effect, bool Cancelled = false);
