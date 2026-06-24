using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Weapons.Ranged.Events;

[ByRefEvent]
public record struct GunRefreshModifiersEvent(Entity<GunComponent> Gun, SoundSpecifier? SoundGunshot, float CameraRecoilScalar, Angle AngleIncrease, Angle AngleDecay, Angle MaxAngle, Angle MinAngle, int ShotsPerBurst, float FireRate, float ProjectileSpeed);
