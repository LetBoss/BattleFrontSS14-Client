// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleGunnerViewSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Camera;
using Content.Shared.Movement.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleGunnerViewSystem : EntitySystem
{
  [Dependency]
  private readonly SharedContentEyeSystem _eye;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleGunnerViewUserComponent, GetEyePvsScaleEvent>(new EntityEventRefHandler<VehicleGunnerViewUserComponent, GetEyePvsScaleEvent>(this.OnGetEyePvsScale));
    this.SubscribeLocalEvent<VehicleGunnerViewUserComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<VehicleGunnerViewUserComponent, AfterAutoHandleStateEvent>(this.OnHandleState));
    this.SubscribeLocalEvent<VehicleGunnerViewUserComponent, ComponentStartup>(new EntityEventRefHandler<VehicleGunnerViewUserComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<VehicleGunnerViewUserComponent, ComponentShutdown>(new EntityEventRefHandler<VehicleGunnerViewUserComponent, ComponentShutdown>(this.OnShutdown));
  }

  private void OnGetEyePvsScale(
    Entity<VehicleGunnerViewUserComponent> ent,
    ref GetEyePvsScaleEvent args)
  {
    args.Scale += ent.Comp.PvsScale + ent.Comp.CursorPvsIncrease;
  }

  private void OnHandleState(
    Entity<VehicleGunnerViewUserComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this._eye.UpdatePvsScale(ent.Owner);
  }

  private void OnStartup(Entity<VehicleGunnerViewUserComponent> ent, ref ComponentStartup args)
  {
    this._eye.UpdatePvsScale(ent.Owner);
  }

  private void OnShutdown(Entity<VehicleGunnerViewUserComponent> ent, ref ComponentShutdown args)
  {
    this._eye.UpdatePvsScale(ent.Owner);
  }
}
