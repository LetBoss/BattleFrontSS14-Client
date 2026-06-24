// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleHardpointAmmoComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCVehicleHardpointAmmoComponent : 
  Component,
  ISerializationGenerated<RMCVehicleHardpointAmmoComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MagazineSize = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxStoredMagazines;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int StoredMagazines;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Queue<EntProtoId> MagazineProjectileQueue = new Queue<EntProtoId>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCVehicleHardpointAmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCVehicleHardpointAmmoComponent) target1;
    if (serialization.TryCustomCopy<RMCVehicleHardpointAmmoComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MagazineSize, ref target2, hookCtx, false, context))
      target2 = this.MagazineSize;
    target.MagazineSize = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxStoredMagazines, ref target3, hookCtx, false, context))
      target3 = this.MaxStoredMagazines;
    target.MaxStoredMagazines = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.StoredMagazines, ref target4, hookCtx, false, context))
      target4 = this.StoredMagazines;
    target.StoredMagazines = target4;
    Queue<EntProtoId> target5 = (Queue<EntProtoId>) null;
    if (this.MagazineProjectileQueue == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Queue<EntProtoId>>(this.MagazineProjectileQueue, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Queue<EntProtoId>>(this.MagazineProjectileQueue, hookCtx, context);
    target.MagazineProjectileQueue = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCVehicleHardpointAmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCVehicleHardpointAmmoComponent target1 = (RMCVehicleHardpointAmmoComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCVehicleHardpointAmmoComponent target1 = (RMCVehicleHardpointAmmoComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCVehicleHardpointAmmoComponent target1 = (RMCVehicleHardpointAmmoComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RMCVehicleHardpointAmmoComponent Component.Instantiate()
  {
    return new RMCVehicleHardpointAmmoComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCVehicleHardpointAmmoComponent_AutoState : IComponentState
  {
    public int MagazineSize;
    public int MaxStoredMagazines;
    public int StoredMagazines;
    public Queue<EntProtoId> MagazineProjectileQueue;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCVehicleHardpointAmmoComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCVehicleHardpointAmmoComponent, ComponentGetState>(new ComponentEventRefHandler<RMCVehicleHardpointAmmoComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCVehicleHardpointAmmoComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCVehicleHardpointAmmoComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCVehicleHardpointAmmoComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCVehicleHardpointAmmoComponent.RMCVehicleHardpointAmmoComponent_AutoState()
      {
        MagazineSize = component.MagazineSize,
        MaxStoredMagazines = component.MaxStoredMagazines,
        StoredMagazines = component.StoredMagazines,
        MagazineProjectileQueue = component.MagazineProjectileQueue
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCVehicleHardpointAmmoComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCVehicleHardpointAmmoComponent.RMCVehicleHardpointAmmoComponent_AutoState current))
        return;
      component.MagazineSize = current.MagazineSize;
      component.MaxStoredMagazines = current.MaxStoredMagazines;
      component.StoredMagazines = current.StoredMagazines;
      component.MagazineProjectileQueue = current.MagazineProjectileQueue;
    }
  }
}
