// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Events.TakeAmmoEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Events;

public sealed class TakeAmmoEvent : EntityEventArgs
{
  public readonly EntityUid? User;
  public int Shots;
  public List<(EntityUid? Entity, IShootable Shootable)> Ammo;
  public string? Reason;
  public EntityCoordinates Coordinates;

  public TakeAmmoEvent(
    int shots,
    List<(EntityUid? Entity, IShootable Shootable)> ammo,
    EntityCoordinates coordinates,
    EntityUid? user)
  {
    this.Shots = shots;
    this.Ammo = ammo;
    this.Coordinates = coordinates;
    this.User = user;
  }
}
