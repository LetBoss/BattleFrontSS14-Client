// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemToggle.Components.ItemToggleSizeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Item.ItemToggle.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ItemToggleSizeComponent : 
  Component,
  ISerializationGenerated<ItemToggleSizeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ItemSizePrototype>? ActivatedSize;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<Box2i>? ActivatedShape;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ItemSizePrototype>? DeactivatedSize;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<Box2i>? DeactivatedShape;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemToggleSizeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemToggleSizeComponent) target1;
    if (serialization.TryCustomCopy<ItemToggleSizeComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ItemSizePrototype>? target2 = new ProtoId<ItemSizePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>?>(this.ActivatedSize, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ItemSizePrototype>?>(this.ActivatedSize, hookCtx, context);
    target.ActivatedSize = target2;
    List<Box2i> target3 = (List<Box2i>) null;
    if (!serialization.TryCustomCopy<List<Box2i>>(this.ActivatedShape, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<Box2i>>(this.ActivatedShape, hookCtx, context);
    target.ActivatedShape = target3;
    ProtoId<ItemSizePrototype>? target4 = new ProtoId<ItemSizePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>?>(this.DeactivatedSize, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<ItemSizePrototype>?>(this.DeactivatedSize, hookCtx, context);
    target.DeactivatedSize = target4;
    List<Box2i> target5 = (List<Box2i>) null;
    if (!serialization.TryCustomCopy<List<Box2i>>(this.DeactivatedShape, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<Box2i>>(this.DeactivatedShape, hookCtx, context);
    target.DeactivatedShape = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemToggleSizeComponent target,
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
    ItemToggleSizeComponent target1 = (ItemToggleSizeComponent) target;
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
    ItemToggleSizeComponent target1 = (ItemToggleSizeComponent) target;
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
    ItemToggleSizeComponent target1 = (ItemToggleSizeComponent) target;
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
  virtual ItemToggleSizeComponent Component.Instantiate() => new ItemToggleSizeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ItemToggleSizeComponent_AutoState : IComponentState
  {
    public ProtoId<ItemSizePrototype>? ActivatedSize;
    public List<Box2i>? ActivatedShape;
    public ProtoId<ItemSizePrototype>? DeactivatedSize;
    public List<Box2i>? DeactivatedShape;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ItemToggleSizeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ItemToggleSizeComponent, ComponentGetState>(new ComponentEventRefHandler<ItemToggleSizeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ItemToggleSizeComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemToggleSizeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ItemToggleSizeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ItemToggleSizeComponent.ItemToggleSizeComponent_AutoState()
      {
        ActivatedSize = component.ActivatedSize,
        ActivatedShape = component.ActivatedShape,
        DeactivatedSize = component.DeactivatedSize,
        DeactivatedShape = component.DeactivatedShape
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ItemToggleSizeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ItemToggleSizeComponent.ItemToggleSizeComponent_AutoState current))
        return;
      component.ActivatedSize = current.ActivatedSize;
      component.ActivatedShape = current.ActivatedShape == null ? (List<Box2i>) null : new List<Box2i>((IEnumerable<Box2i>) current.ActivatedShape);
      component.DeactivatedSize = current.DeactivatedSize;
      component.DeactivatedShape = current.DeactivatedShape == null ? (List<Box2i>) null : new List<Box2i>((IEnumerable<Box2i>) current.DeactivatedShape);
    }
  }
}
