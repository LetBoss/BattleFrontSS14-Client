// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Weapon.DropshipWeaponShotEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Explosion.Implosion;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Weapon;

[ByRefEvent]
public record struct DropshipWeaponShotEvent(
  float Spread,
  int BulletSpread,
  TimeSpan TravelTime,
  int RoundsPerShot,
  int ShotsPerVolley,
  DamageSpecifier? Damage,
  int ArmorPiercing,
  TimeSpan SoundTravelTime,
  SoundSpecifier? SoundCockpit,
  SoundSpecifier? SoundMarker,
  SoundSpecifier? SoundGround,
  SoundSpecifier? SoundImpact,
  SoundSpecifier? SoundWarning,
  bool MarkerWarning,
  List<EntProtoId> ImpactEffect,
  RMCExplosion? Explosion,
  RMCImplosion? Implosion,
  RMCFire? Fire,
  int SoundEveryShots)
;
