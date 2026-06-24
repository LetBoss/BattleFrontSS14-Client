// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.Events.AttackedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

#nullable enable
namespace Content.Shared.Weapons.Melee.Events;

public sealed class AttackedEvent : EntityEventArgs
{
  public DamageSpecifier BonusDamage = new DamageSpecifier();

  public EntityUid Used { get; }

  public EntityUid User { get; }

  public EntityCoordinates ClickLocation { get; }

  public AttackedEvent(EntityUid used, EntityUid user, EntityCoordinates clickLocation)
  {
    this.Used = used;
    this.User = user;
    this.ClickLocation = clickLocation;
  }
}
