// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Item;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedItemSystem)})]
[AutoGenerateComponentState(true, false)]
public sealed class ItemComponent : 
  Component,
  ISerializationGenerated<ItemComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedItemSystem)})]
  public ProtoId<ItemSizePrototype> Size = (ProtoId<ItemSizePrototype>) "Small";
  [Access(new Type[] {typeof (SharedItemSystem)})]
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<HandLocation, List<PrototypeLayerData>> InhandVisuals = new Dictionary<HandLocation, List<PrototypeLayerData>>();
  [Access(new Type[] {typeof (SharedItemSystem)})]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? HeldPrefix;
  [Access(new Type[] {typeof (SharedItemSystem)})]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("sprite", false, 1, false, false, null)]
  public string? RsiPath;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<Box2i>? Shape;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier? StoredSprite;
  [AutoNetworkedField]
  public float StoredRotation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i StoredOffset;
  [DataField("storedRotation", false, 1, false, false, null)]
  private float _unusedStoredRotation;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemComponent) target1;
    if (serialization.TryCustomCopy<ItemComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ItemSizePrototype> target2 = new ProtoId<ItemSizePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>>(this.Size, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ItemSizePrototype>>(this.Size, hookCtx, context);
    target.Size = target2;
    Dictionary<HandLocation, List<PrototypeLayerData>> target3 = (Dictionary<HandLocation, List<PrototypeLayerData>>) null;
    if (this.InhandVisuals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HandLocation, List<PrototypeLayerData>>>(this.InhandVisuals, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<HandLocation, List<PrototypeLayerData>>>(this.InhandVisuals, hookCtx, context);
    target.InhandVisuals = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.HeldPrefix, ref target4, hookCtx, false, context))
      target4 = this.HeldPrefix;
    target.HeldPrefix = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.RsiPath, ref target5, hookCtx, false, context))
      target5 = this.RsiPath;
    target.RsiPath = target5;
    List<Box2i> target6 = (List<Box2i>) null;
    if (!serialization.TryCustomCopy<List<Box2i>>(this.Shape, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<Box2i>>(this.Shape, hookCtx, context);
    target.Shape = target6;
    SpriteSpecifier target7 = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.StoredSprite, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SpriteSpecifier>(this.StoredSprite, hookCtx, context);
    target.StoredSprite = target7;
    Vector2i target8 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.StoredOffset, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Vector2i>(this.StoredOffset, hookCtx, context);
    target.StoredOffset = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this._unusedStoredRotation, ref target9, hookCtx, false, context))
      target9 = this._unusedStoredRotation;
    target._unusedStoredRotation = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemComponent target,
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
    ItemComponent target1 = (ItemComponent) target;
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
    ItemComponent target1 = (ItemComponent) target;
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
    ItemComponent target1 = (ItemComponent) target;
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
  virtual ItemComponent Component.Instantiate() => new ItemComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ItemComponent_AutoState : IComponentState
  {
    public ProtoId<ItemSizePrototype> Size;
    public string? HeldPrefix;
    public List<Box2i>? Shape;
    public SpriteSpecifier? StoredSprite;
    public float StoredRotation;
    public Vector2i StoredOffset;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ItemComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ItemComponent, ComponentGetState>(new ComponentEventRefHandler<ItemComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ItemComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, ItemComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new ItemComponent.ItemComponent_AutoState()
      {
        Size = component.Size,
        HeldPrefix = component.HeldPrefix,
        Shape = component.Shape,
        StoredSprite = component.StoredSprite,
        StoredRotation = component.StoredRotation,
        StoredOffset = component.StoredOffset
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ItemComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ItemComponent.ItemComponent_AutoState current))
        return;
      component.Size = current.Size;
      component.HeldPrefix = current.HeldPrefix;
      component.Shape = current.Shape == null ? (List<Box2i>) null : new List<Box2i>((IEnumerable<Box2i>) current.Shape);
      component.StoredSprite = current.StoredSprite;
      component.StoredRotation = current.StoredRotation;
      component.StoredOffset = current.StoredOffset;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, ItemComponent>(uid, component, ref args1);
    }
  }
}
