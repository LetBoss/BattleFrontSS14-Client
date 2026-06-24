// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Events.SelfBeforeGunShotEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Events;

public sealed class SelfBeforeGunShotEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
  public readonly EntityUid Shooter;
  public readonly Entity<GunComponent> Gun;
  public readonly List<(EntityUid? Entity, IShootable Shootable)> Ammo;

  public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

  public SelfBeforeGunShotEvent(
    EntityUid shooter,
    Entity<GunComponent> gun,
    List<(EntityUid? Entity, IShootable Shootable)> ammo)
  {
    this.Shooter = shooter;
    this.Gun = gun;
    this.Ammo = ammo;
  }
}
