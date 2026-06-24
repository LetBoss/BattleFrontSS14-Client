using Robust.Shared.GameObjects;

namespace Content.Shared.Damage;

[ByRefEvent]
public record struct BeforeDamageChangedEvent(DamageSpecifier Damage, EntityUid? Origin = null, bool Cancelled = false);
