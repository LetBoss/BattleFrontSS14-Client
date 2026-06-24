// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleDeployActionComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (VehicleDeploySystem)})]
public sealed class VehicleDeployActionComponent : 
  Component,
  ISerializationGenerated<VehicleDeployActionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "ActionVehicleDeploy";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Vehicle;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleDeployActionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleDeployActionComponent) target1;
    if (serialization.TryCustomCopy<VehicleDeployActionComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Vehicle, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Vehicle, hookCtx, context);
    target.Vehicle = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleDeployActionComponent target,
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
    VehicleDeployActionComponent target1 = (VehicleDeployActionComponent) target;
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
    VehicleDeployActionComponent target1 = (VehicleDeployActionComponent) target;
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
    VehicleDeployActionComponent target1 = (VehicleDeployActionComponent) target;
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
  virtual VehicleDeployActionComponent Component.Instantiate()
  {
    return new VehicleDeployActionComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleDeployActionComponent_AutoState : IComponentState
  {
    public EntProtoId ActionId;
    public NetEntity? Action;
    public NetEntity? Vehicle;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleDeployActionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleDeployActionComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleDeployActionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleDeployActionComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleDeployActionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleDeployActionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleDeployActionComponent.VehicleDeployActionComponent_AutoState()
      {
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        Vehicle = this.GetNetEntity(component.Vehicle)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleDeployActionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleDeployActionComponent.VehicleDeployActionComponent_AutoState current))
        return;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<VehicleDeployActionComponent>(current.Action, uid);
      component.Vehicle = this.EnsureEntity<VehicleDeployActionComponent>(current.Vehicle, uid);
    }
  }
}
