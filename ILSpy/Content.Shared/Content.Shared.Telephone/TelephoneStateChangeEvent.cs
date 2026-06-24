using Robust.Shared.GameObjects;

namespace Content.Shared.Telephone;

[ByRefEvent]
public record struct TelephoneStateChangeEvent(TelephoneState OldState, TelephoneState NewState);
