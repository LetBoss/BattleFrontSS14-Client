using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Weapons.Ranged.AimedShot;

[ByRefEvent]
public record struct ShotByAimedShotEvent(EntityUid Gun, EntityUid Target);
