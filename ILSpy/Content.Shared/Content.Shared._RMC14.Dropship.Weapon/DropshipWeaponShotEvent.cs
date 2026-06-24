using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Explosion.Implosion;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Dropship.Weapon;

[ByRefEvent]
public record struct DropshipWeaponShotEvent(float Spread, int BulletSpread, TimeSpan TravelTime, int RoundsPerShot, int ShotsPerVolley, DamageSpecifier? Damage, int ArmorPiercing, TimeSpan SoundTravelTime, SoundSpecifier? SoundCockpit, SoundSpecifier? SoundMarker, SoundSpecifier? SoundGround, SoundSpecifier? SoundImpact, SoundSpecifier? SoundWarning, bool MarkerWarning, List<EntProtoId> ImpactEffect, RMCExplosion? Explosion, RMCImplosion? Implosion, RMCFire? Fire, int SoundEveryShots);
