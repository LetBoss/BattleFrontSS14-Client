// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Telephone.RotaryPhoneBackpackComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Telephone;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCTelephoneSystem)})]
public sealed class RotaryPhoneBackpackComponent : 
  Component,
  ISerializationGenerated<RotaryPhoneBackpackComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags Slot = SlotFlags.BACK;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionTelephone";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RotaryPhoneBackpackComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RotaryPhoneBackpackComponent) target1;
    if (serialization.TryCustomCopy<RotaryPhoneBackpackComponent>(this, ref target, hookCtx, false, context))
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
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RotaryPhoneBackpackComponent target,
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
    RotaryPhoneBackpackComponent target1 = (RotaryPhoneBackpackComponent) target;
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
    RotaryPhoneBackpackComponent target1 = (RotaryPhoneBackpackComponent) target;
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
    RotaryPhoneBackpackComponent target1 = (RotaryPhoneBackpackComponent) target;
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
  virtual RotaryPhoneBackpackComponent Component.Instantiate()
  {
    return new RotaryPhoneBackpackComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RotaryPhoneBackpackComponent_AutoState : IComponentState
  {
    public SlotFlags Slot;
    public EntProtoId ActionId;
    public NetEntity? Action;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RotaryPhoneBackpackComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RotaryPhoneBackpackComponent, ComponentGetState>(new ComponentEventRefHandler<RotaryPhoneBackpackComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RotaryPhoneBackpackComponent, ComponentHandleState>(new ComponentEventRefHandler<RotaryPhoneBackpackComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RotaryPhoneBackpackComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RotaryPhoneBackpackComponent.RotaryPhoneBackpackComponent_AutoState()
      {
        Slot = component.Slot,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RotaryPhoneBackpackComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RotaryPhoneBackpackComponent.RotaryPhoneBackpackComponent_AutoState current))
        return;
      component.Slot = current.Slot;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<RotaryPhoneBackpackComponent>(current.Action, uid);
    }
  }
}
