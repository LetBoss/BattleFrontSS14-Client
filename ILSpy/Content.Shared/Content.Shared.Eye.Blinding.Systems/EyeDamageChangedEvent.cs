using Robust.Shared.GameObjects;

namespace Content.Shared.Eye.Blinding.Systems;

[ByRefEvent]
public record struct EyeDamageChangedEvent(int Damage);
