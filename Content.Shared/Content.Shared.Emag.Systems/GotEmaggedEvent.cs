using Robust.Shared.GameObjects;

namespace Content.Shared.Emag.Systems;

[ByRefEvent]
public record struct GotEmaggedEvent(EntityUid UserUid, EmagType Type, bool Handled = false, bool Repeatable = false);
