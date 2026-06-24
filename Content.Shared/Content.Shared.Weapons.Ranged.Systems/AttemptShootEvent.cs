using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Weapons.Ranged.Systems;

[ByRefEvent]
public record struct AttemptShootEvent(EntityUid User, string? Message, EntityCoordinates FromCoordinates, EntityCoordinates? ToCoordinates, bool Cancelled = false, bool ThrowItems = false, bool ResetCooldown = false);
