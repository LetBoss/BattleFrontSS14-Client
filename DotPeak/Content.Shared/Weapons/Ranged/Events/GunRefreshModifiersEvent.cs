// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Events.GunRefreshModifiersEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Events;

[ByRefEvent]
public record struct GunRefreshModifiersEvent(
  Entity<GunComponent> Gun,
  SoundSpecifier? SoundGunshot,
  float CameraRecoilScalar,
  Angle AngleIncrease,
  Angle AngleDecay,
  Angle MaxAngle,
  Angle MinAngle,
  int ShotsPerBurst,
  float FireRate,
  float ProjectileSpeed)
;
