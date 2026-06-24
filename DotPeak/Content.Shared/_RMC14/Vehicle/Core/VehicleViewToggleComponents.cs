// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleViewToggleComponent
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
[Access(new Type[] {typeof (VehicleViewToggleSystem)})]
public sealed class VehicleViewToggleComponent : 
  Component,
  ISerializationGenerated<VehicleViewToggleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "ActionVehicleToggleView";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? OutsideTarget;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? InsideTarget;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Source;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsOutside;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<EntityUid> Sources = new HashSet<EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleViewToggleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleViewToggleComponent) target1;
    if (serialization.TryCustomCopy<VehicleViewToggleComponent>(this, ref target, hookCtx, false, context))
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
    if (!serialization.TryCustomCopy<EntityUid?>(this.OutsideTarget, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.OutsideTarget, hookCtx, context);
    target.OutsideTarget = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.InsideTarget, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.InsideTarget, hookCtx, context);
    target.InsideTarget = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Source, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.Source, hookCtx, context);
    target.Source = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsOutside, ref target7, hookCtx, false, context))
      target7 = this.IsOutside;
    target.IsOutside = target7;
    HashSet<EntityUid> target8 = (HashSet<EntityUid>) null;
    if (this.Sources == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Sources, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<HashSet<EntityUid>>(this.Sources, hookCtx, context);
    target.Sources = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleViewToggleComponent target,
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
    VehicleViewToggleComponent target1 = (VehicleViewToggleComponent) target;
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
    VehicleViewToggleComponent target1 = (VehicleViewToggleComponent) target;
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
    VehicleViewToggleComponent target1 = (VehicleViewToggleComponent) target;
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
  virtual VehicleViewToggleComponent Component.Instantiate() => new VehicleViewToggleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleViewToggleComponent_AutoState : IComponentState
  {
    public EntProtoId ActionId;
    public NetEntity? Action;
    public NetEntity? OutsideTarget;
    public NetEntity? InsideTarget;
    public NetEntity? Source;
    public bool IsOutside;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleViewToggleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleViewToggleComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleViewToggleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleViewToggleComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleViewToggleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleViewToggleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleViewToggleComponent.VehicleViewToggleComponent_AutoState()
      {
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        OutsideTarget = this.GetNetEntity(component.OutsideTarget),
        InsideTarget = this.GetNetEntity(component.InsideTarget),
        Source = this.GetNetEntity(component.Source),
        IsOutside = component.IsOutside
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleViewToggleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleViewToggleComponent.VehicleViewToggleComponent_AutoState current))
        return;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<VehicleViewToggleComponent>(current.Action, uid);
      component.OutsideTarget = this.EnsureEntity<VehicleViewToggleComponent>(current.OutsideTarget, uid);
      component.InsideTarget = this.EnsureEntity<VehicleViewToggleComponent>(current.InsideTarget, uid);
      component.Source = this.EnsureEntity<VehicleViewToggleComponent>(current.Source, uid);
      component.IsOutside = current.IsOutside;
    }
  }
}
