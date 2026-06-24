// Decompiled with JetBrains decompiler
// Type: Content.Shared.Projectiles.ProjectileHitEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Projectiles;

[ByRefEvent]
public record struct ProjectileHitEvent(
  DamageSpecifier Damage,
  EntityUid Target,
  EntityUid? Shooter = null,
  bool Handled = false)
;
