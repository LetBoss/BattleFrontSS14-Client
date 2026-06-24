// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleXenoPushIgnoreSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleXenoPushIgnoreSystem : EntitySystem
{
  [Dependency]
  private readonly INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleXenoPushIgnoreComponent, PreventCollideEvent>(new EntityEventRefHandler<VehicleXenoPushIgnoreComponent, PreventCollideEvent>(this.OnPreventCollide));
    this.SubscribeLocalEvent<VehicleXenoPushIgnoreComponent, LandEvent>(new EntityEventRefHandler<VehicleXenoPushIgnoreComponent, LandEvent>(this.OnLand));
    this.SubscribeLocalEvent<VehicleXenoPushIgnoreComponent, StopThrowEvent>(new EntityEventRefHandler<VehicleXenoPushIgnoreComponent, StopThrowEvent>(this.OnStopThrow));
  }

  private void OnPreventCollide(
    Entity<VehicleXenoPushIgnoreComponent> ent,
    ref PreventCollideEvent args)
  {
    if (!this.HasComp<ThrownItemComponent>((EntityUid) ent))
      return;
    args.Cancelled = true;
  }

  private void OnLand(Entity<VehicleXenoPushIgnoreComponent> ent, ref LandEvent args)
  {
    if (this._net.IsClient)
      return;
    this.RemCompDeferred<VehicleXenoPushIgnoreComponent>((EntityUid) ent);
  }

  private void OnStopThrow(Entity<VehicleXenoPushIgnoreComponent> ent, ref StopThrowEvent args)
  {
    if (this._net.IsClient)
      return;
    this.RemCompDeferred<VehicleXenoPushIgnoreComponent>((EntityUid) ent);
  }
}
