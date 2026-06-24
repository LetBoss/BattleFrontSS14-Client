using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Medical.Wounds;

[ByRefEvent]
public record struct CMBleedEvent(DamageChangedEvent Damage, bool Handled = false);
