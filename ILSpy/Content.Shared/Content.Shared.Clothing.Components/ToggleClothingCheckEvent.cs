using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing.Components;

[ByRefEvent]
public record struct ToggleClothingCheckEvent(EntityUid User, bool Cancelled = false);
