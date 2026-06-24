using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Events;

[ByRefEvent]
public record struct GunMuzzleFlashAttemptEvent(bool Cancelled);
