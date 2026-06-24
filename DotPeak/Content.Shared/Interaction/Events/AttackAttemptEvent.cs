// Decompiled with JetBrains decompiler
// Type: Content.Shared.Interaction.Events.AttackAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Melee;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Interaction.Events;

public sealed class AttackAttemptEvent : CancellableEntityEventArgs
{
  public EntityUid Uid { get; }

  public EntityUid? Target { get; }

  public Entity<MeleeWeaponComponent>? Weapon { get; }

  public bool Disarm { get; }

  public AttackAttemptEvent(
    EntityUid uid,
    EntityUid? target = null,
    Entity<MeleeWeaponComponent>? weapon = null,
    bool disarm = false)
  {
    this.Uid = uid;
    this.Target = target;
    this.Weapon = weapon;
    this.Disarm = disarm;
  }
}
