// Decompiled with JetBrains decompiler
// Type: Content.Shared.Placeable.ItemPlacerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Placeable;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (ItemPlacerSystem)})]
public sealed class ItemPlacerComponent : 
  Component,
  ISerializationGenerated<ItemPlacerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> PlacedEntities = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntityWhitelist? Whitelist;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public uint MaxEntities = 1;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemPlacerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemPlacerComponent) target1;
    if (serialization.TryCustomCopy<ItemPlacerComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<EntityUid> target2 = (HashSet<EntityUid>) null;
    if (this.PlacedEntities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.PlacedEntities, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<EntityUid>>(this.PlacedEntities, hookCtx, context);
    target.PlacedEntities = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, context);
    }
    target.Whitelist = target3;
    uint target4 = 0;
    if (!serialization.TryCustomCopy<uint>(this.MaxEntities, ref target4, hookCtx, false, context))
      target4 = this.MaxEntities;
    target.MaxEntities = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemPlacerComponent target,
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
    ItemPlacerComponent target1 = (ItemPlacerComponent) target;
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
    ItemPlacerComponent target1 = (ItemPlacerComponent) target;
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
    ItemPlacerComponent target1 = (ItemPlacerComponent) target;
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
  virtual ItemPlacerComponent Component.Instantiate() => new ItemPlacerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ItemPlacerComponent_AutoState : IComponentState
  {
    public HashSet<NetEntity> PlacedEntities;
    public uint MaxEntities;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ItemPlacerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ItemPlacerComponent, ComponentGetState>(new ComponentEventRefHandler<ItemPlacerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ItemPlacerComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemPlacerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ItemPlacerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ItemPlacerComponent.ItemPlacerComponent_AutoState()
      {
        PlacedEntities = this.GetNetEntitySet(component.PlacedEntities),
        MaxEntities = component.MaxEntities
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ItemPlacerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ItemPlacerComponent.ItemPlacerComponent_AutoState current))
        return;
      this.EnsureEntitySet<ItemPlacerComponent>(current.PlacedEntities, uid, component.PlacedEntities);
      component.MaxEntities = current.MaxEntities;
    }
  }
}
