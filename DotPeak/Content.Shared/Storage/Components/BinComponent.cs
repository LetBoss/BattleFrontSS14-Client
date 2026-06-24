// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Components.BinComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
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
namespace Content.Shared.Storage.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (BinSystem)})]
public sealed class BinComponent : 
  Component,
  ISerializationGenerated<BinComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container ItemContainer;
  [DataField(null, false, 1, false, false, null)]
  public string ContainerId = "bin-container";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> Items = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> InitialContents = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxItems = 20;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BinComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BinComponent) target1;
    if (serialization.TryCustomCopy<BinComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target2, hookCtx, false, context))
      target2 = this.ContainerId;
    target.ContainerId = target2;
    List<EntityUid> target3 = (List<EntityUid>) null;
    if (this.Items == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Items, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<EntityUid>>(this.Items, hookCtx, context);
    target.Items = target3;
    List<EntProtoId> target4 = (List<EntProtoId>) null;
    if (this.InitialContents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.InitialContents, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntProtoId>>(this.InitialContents, hookCtx, context);
    target.InitialContents = target4;
    EntityWhitelist target5 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target5, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target5 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target5, hookCtx, context);
    }
    target.Whitelist = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxItems, ref target6, hookCtx, false, context))
      target6 = this.MaxItems;
    target.MaxItems = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BinComponent target,
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
    BinComponent target1 = (BinComponent) target;
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
    BinComponent target1 = (BinComponent) target;
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
    BinComponent target1 = (BinComponent) target;
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
  virtual BinComponent Component.Instantiate() => new BinComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BinComponent_AutoState : IComponentState
  {
    public List<NetEntity> Items;
    public EntityWhitelist? Whitelist;
    public int MaxItems;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BinComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BinComponent, ComponentGetState>(new ComponentEventRefHandler<BinComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BinComponent, ComponentHandleState>(new ComponentEventRefHandler<BinComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, BinComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new BinComponent.BinComponent_AutoState()
      {
        Items = this.GetNetEntityList(component.Items),
        Whitelist = component.Whitelist,
        MaxItems = component.MaxItems
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BinComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BinComponent.BinComponent_AutoState current))
        return;
      this.EnsureEntityList<BinComponent>(current.Items, uid, component.Items);
      component.Whitelist = current.Whitelist;
      component.MaxItems = current.MaxItems;
    }
  }
}
