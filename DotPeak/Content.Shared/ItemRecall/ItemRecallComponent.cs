// Decompiled with JetBrains decompiler
// Type: Content.Shared.ItemRecall.ItemRecallComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.ItemRecall;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedItemRecallSystem)})]
public sealed class ItemRecallComponent : 
  Component,
  ISerializationGenerated<ItemRecallComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public LocId? WhileMarkedName = (LocId?) "item-recall-marked-name";
  [DataField(null, false, 1, false, false, null)]
  public LocId? WhileMarkedDescription = (LocId?) "item-recall-marked-description";
  [DataField(null, false, 1, false, false, null)]
  public string? InitialName;
  [DataField(null, false, 1, false, false, null)]
  public string? InitialDescription;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? MarkedEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemRecallComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemRecallComponent) target1;
    if (serialization.TryCustomCopy<ItemRecallComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId? target2 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.WhileMarkedName, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId?>(this.WhileMarkedName, hookCtx, context);
    target.WhileMarkedName = target2;
    LocId? target3 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.WhileMarkedDescription, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId?>(this.WhileMarkedDescription, hookCtx, context);
    target.WhileMarkedDescription = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.InitialName, ref target4, hookCtx, false, context))
      target4 = this.InitialName;
    target.InitialName = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.InitialDescription, ref target5, hookCtx, false, context))
      target5 = this.InitialDescription;
    target.InitialDescription = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.MarkedEntity, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.MarkedEntity, hookCtx, context);
    target.MarkedEntity = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemRecallComponent target,
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
    ItemRecallComponent target1 = (ItemRecallComponent) target;
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
    ItemRecallComponent target1 = (ItemRecallComponent) target;
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
    ItemRecallComponent target1 = (ItemRecallComponent) target;
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
  virtual ItemRecallComponent Component.Instantiate() => new ItemRecallComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ItemRecallComponent_AutoState : IComponentState
  {
    public NetEntity? MarkedEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ItemRecallComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ItemRecallComponent, ComponentGetState>(new ComponentEventRefHandler<ItemRecallComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ItemRecallComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemRecallComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ItemRecallComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ItemRecallComponent.ItemRecallComponent_AutoState()
      {
        MarkedEntity = this.GetNetEntity(component.MarkedEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ItemRecallComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ItemRecallComponent.ItemRecallComponent_AutoState current))
        return;
      component.MarkedEntity = this.EnsureEntity<ItemRecallComponent>(current.MarkedEntity, uid);
    }
  }
}
