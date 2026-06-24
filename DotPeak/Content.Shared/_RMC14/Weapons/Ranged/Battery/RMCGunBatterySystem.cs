// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Battery.RMCGunBatterySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Battery;

public sealed class RMCGunBatterySystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  private Robust.Shared.GameObjects.EntityQuery<GunDrainBatteryOnShootComponent> _gunDrainBatteryQuery;

  public override void Initialize()
  {
    this._gunDrainBatteryQuery = this.GetEntityQuery<GunDrainBatteryOnShootComponent>();
    this.SubscribeLocalEvent<GunDrainBatteryOnShootComponent, AttemptShootEvent>(new EntityEventRefHandler<GunDrainBatteryOnShootComponent, AttemptShootEvent>(this.OnDrainBatteryAttemptShoot));
  }

  private void OnDrainBatteryAttemptShoot(
    Entity<GunDrainBatteryOnShootComponent> ent,
    ref AttemptShootEvent args)
  {
    if (args.Cancelled || ent.Comp.Powered)
      return;
    args.Cancelled = true;
    this._popup.PopupClient(this.Loc.GetString("rmc-low-power"), args.User, new EntityUid?(args.User), PopupType.MediumCaution);
  }

  public void SetPowered(Entity<GunDrainBatteryOnShootComponent> gun, bool powered)
  {
    gun.Comp.Powered = powered;
    this.Dirty<GunDrainBatteryOnShootComponent>(gun);
    if (gun.Comp.Powered)
      return;
    GunUnpoweredEvent args = new GunUnpoweredEvent();
    this.RaiseLocalEvent<GunUnpoweredEvent>((EntityUid) gun, ref args);
  }

  public void RefreshBatteryDrain(Entity<GunDrainBatteryOnShootComponent?> gun)
  {
    if (this._timing.ApplyingState || !this._gunDrainBatteryQuery.Resolve((EntityUid) gun, ref gun.Comp, false))
      return;
    GunGetBatteryDrainEvent args = new GunGetBatteryDrainEvent(gun.Comp.BaseDrain);
    this.RaiseLocalEvent<GunGetBatteryDrainEvent>((EntityUid) gun, ref args);
    gun.Comp.Drain = args.Drain;
    this.Dirty<GunDrainBatteryOnShootComponent>(gun);
  }
}
