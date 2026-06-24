// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Flamer.RMCBroilerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Flamer;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCFlamerSystem)})]
public sealed class RMCBroilerComponent : 
  Component,
  ISerializationGenerated<RMCBroilerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags Slot = SlotFlags.BACK;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionBroiler";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerPrefix = "Tank";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ActiveTank;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ResPath NumberingResource = new ResPath("Effects/crayondecals.rsi");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCBroilerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCBroilerComponent) target1;
    if (serialization.TryCustomCopy<RMCBroilerComponent>(this, ref target, hookCtx, false, context))
      return;
    SlotFlags target2 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slot, ref target2, hookCtx, false, context))
      target2 = this.Slot;
    target.Slot = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target4;
    string target5 = (string) null;
    if (this.ContainerPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerPrefix, ref target5, hookCtx, false, context))
      target5 = this.ContainerPrefix;
    target.ContainerPrefix = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.ActiveTank, ref target6, hookCtx, false, context))
      target6 = this.ActiveTank;
    target.ActiveTank = target6;
    ResPath target7 = new ResPath();
    if (!serialization.TryCustomCopy<ResPath>(this.NumberingResource, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ResPath>(this.NumberingResource, hookCtx, context);
    target.NumberingResource = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCBroilerComponent target,
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
    RMCBroilerComponent target1 = (RMCBroilerComponent) target;
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
    RMCBroilerComponent target1 = (RMCBroilerComponent) target;
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
    RMCBroilerComponent target1 = (RMCBroilerComponent) target;
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
  virtual RMCBroilerComponent Component.Instantiate() => new RMCBroilerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCBroilerComponent_AutoState : IComponentState
  {
    public SlotFlags Slot;
    public EntProtoId ActionId;
    public NetEntity? Action;
    public string ContainerPrefix;
    public int ActiveTank;
    public ResPath NumberingResource;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCBroilerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCBroilerComponent, ComponentGetState>(new ComponentEventRefHandler<RMCBroilerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCBroilerComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCBroilerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCBroilerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCBroilerComponent.RMCBroilerComponent_AutoState()
      {
        Slot = component.Slot,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        ContainerPrefix = component.ContainerPrefix,
        ActiveTank = component.ActiveTank,
        NumberingResource = component.NumberingResource
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCBroilerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCBroilerComponent.RMCBroilerComponent_AutoState current))
        return;
      component.Slot = current.Slot;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<RMCBroilerComponent>(current.Action, uid);
      component.ContainerPrefix = current.ContainerPrefix;
      component.ActiveTank = current.ActiveTank;
      component.NumberingResource = current.NumberingResource;
    }
  }
}
