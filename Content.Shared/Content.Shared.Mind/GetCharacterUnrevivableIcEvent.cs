using Robust.Shared.GameObjects;

namespace Content.Shared.Mind;

[ByRefEvent]
public record struct GetCharacterUnrevivableIcEvent(bool? Unrevivable);
