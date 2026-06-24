// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Item.ChangeItemSizeOnTimerTriggerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item;
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
namespace Content.Shared._RMC14.Item;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (ItemSizeChangeSystem)})]
public sealed class ChangeItemSizeOnTimerTriggerComponent : 
  Component,
  ISerializationGenerated<ChangeItemSizeOnTimerTriggerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ItemSizePrototype> Size = (ProtoId<ItemSizePrototype>) "Ginormous";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ItemSizePrototype>? OriginalSize;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChangeItemSizeOnTimerTriggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ChangeItemSizeOnTimerTriggerComponent) target1;
    if (serialization.TryCustomCopy<ChangeItemSizeOnTimerTriggerComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ItemSizePrototype> target2 = new ProtoId<ItemSizePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>>(this.Size, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ItemSizePrototype>>(this.Size, hookCtx, context);
    target.Size = target2;
    ProtoId<ItemSizePrototype>? target3 = new ProtoId<ItemSizePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>?>(this.OriginalSize, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<ItemSizePrototype>?>(this.OriginalSize, hookCtx, context);
    target.OriginalSize = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChangeItemSizeOnTimerTriggerComponent target,
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
    ChangeItemSizeOnTimerTriggerComponent target1 = (ChangeItemSizeOnTimerTriggerComponent) target;
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
    ChangeItemSizeOnTimerTriggerComponent target1 = (ChangeItemSizeOnTimerTriggerComponent) target;
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
    ChangeItemSizeOnTimerTriggerComponent target1 = (ChangeItemSizeOnTimerTriggerComponent) target;
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
  virtual ChangeItemSizeOnTimerTriggerComponent Component.Instantiate()
  {
    return new ChangeItemSizeOnTimerTriggerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ChangeItemSizeOnTimerTriggerComponent_AutoState : IComponentState
  {
    public ProtoId<ItemSizePrototype> Size;
    public ProtoId<ItemSizePrototype>? OriginalSize;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ChangeItemSizeOnTimerTriggerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ChangeItemSizeOnTimerTriggerComponent, ComponentGetState>(new ComponentEventRefHandler<ChangeItemSizeOnTimerTriggerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ChangeItemSizeOnTimerTriggerComponent, ComponentHandleState>(new ComponentEventRefHandler<ChangeItemSizeOnTimerTriggerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ChangeItemSizeOnTimerTriggerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ChangeItemSizeOnTimerTriggerComponent.ChangeItemSizeOnTimerTriggerComponent_AutoState()
      {
        Size = component.Size,
        OriginalSize = component.OriginalSize
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ChangeItemSizeOnTimerTriggerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ChangeItemSizeOnTimerTriggerComponent.ChangeItemSizeOnTimerTriggerComponent_AutoState current))
        return;
      component.Size = current.Size;
      component.OriginalSize = current.OriginalSize;
    }
  }
}
