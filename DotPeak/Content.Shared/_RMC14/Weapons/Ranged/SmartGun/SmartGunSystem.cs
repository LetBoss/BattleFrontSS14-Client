// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.SmartGun.SmartGunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.PowerCell.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.SmartGun;

public sealed class SmartGunSystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<SmartGunBatteryComponent, ContainerGettingInsertedAttemptEvent>(new EntityEventRefHandler<SmartGunBatteryComponent, ContainerGettingInsertedAttemptEvent>(this.OnBatteryInsertedAttempt));
  }

  private void OnBatteryInsertedAttempt(
    Entity<SmartGunBatteryComponent> ent,
    ref ContainerGettingInsertedAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    BaseContainer container = args.Container;
    PowerCellSlotComponent comp;
    if (!this.TryComp<PowerCellSlotComponent>(container.Owner, out comp) || !(container.ID == comp.CellSlotId) || this.HasComp<SmartGunComponent>(container.Owner))
      return;
    args.Cancel();
  }
}
