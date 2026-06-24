// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.Events.MeleeHitEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Weapons.Melee.Events;

public sealed class MeleeHitEvent : HandledEntityEventArgs
{
  public readonly DamageSpecifier BaseDamage;
  public List<DamageModifierSet> ModifiersList = new List<DamageModifierSet>();
  public DamageSpecifier BonusDamage = new DamageSpecifier();
  public IReadOnlyList<EntityUid> HitEntities;
  public SoundSpecifier? HitSoundOverride;
  public readonly EntityUid User;
  public readonly EntityUid Weapon;
  public readonly Vector2? Direction;
  public bool IsHit = true;

  public MeleeHitEvent(
    List<EntityUid> hitEntities,
    EntityUid user,
    EntityUid weapon,
    DamageSpecifier baseDamage,
    Vector2? direction)
  {
    this.HitEntities = (IReadOnlyList<EntityUid>) hitEntities;
    this.User = user;
    this.Weapon = weapon;
    this.BaseDamage = baseDamage;
    this.Direction = direction;
  }
}
