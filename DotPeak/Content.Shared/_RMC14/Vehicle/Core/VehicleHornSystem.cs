// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleHornSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Input;
using Content.Shared.Vehicle.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleHornSystem : EntitySystem
{
  [Dependency]
  private readonly IGameTiming _timing;
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly SharedAudioSystem _audio;
  [Dependency]
  private readonly VehicleSystem _rmcVehicles;

  public override void Initialize()
  {
    if (this._net.IsClient)
      CommandBinds.Builder.Bind(ContentKeyFunctions.UseItemInHand, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
      {
        EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
        EntityUid vehicle;
        if (!attachedEntity.HasValue || !this._rmcVehicles.TryResolveControlledVehicle(attachedEntity.GetValueOrDefault(), out vehicle))
          return;
        this.RaiseNetworkEvent((EntityEventArgs) new VehicleHornRequestEvent(this.GetNetEntity(vehicle)));
      }), handle: false)).Register<VehicleHornSystem>();
    this.SubscribeNetworkEvent<VehicleHornRequestEvent>(new EntitySessionEventHandler<VehicleHornRequestEvent>(this.OnHornRequest));
  }

  private void OnHornRequest(VehicleHornRequestEvent ev, EntitySessionEventArgs args)
  {
    if (!this._net.IsServer)
      return;
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid entity = this.GetEntity(ev.Vehicle);
    VehicleComponent comp1;
    if (!this.TryComp<VehicleComponent>(entity, out comp1))
      return;
    attachedEntity = comp1.Operator;
    EntityUid entityUid = valueOrDefault;
    VehicleSoundComponent comp2;
    if ((attachedEntity.HasValue ? (attachedEntity.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0 || !this.TryComp<VehicleSoundComponent>(entity, out comp2) || comp2.HornSound == null)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (comp2.NextHornSound > curTime)
      return;
    comp2.NextHornSound = curTime + TimeSpan.FromSeconds((double) comp2.HornCooldown);
    this._audio.PlayPvs(comp2.HornSound, entity);
    this.Dirty(entity, (IComponent) comp2);
  }
}
