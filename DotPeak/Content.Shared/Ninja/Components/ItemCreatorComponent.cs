// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Components.ItemCreatorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions.Components;
using Content.Shared.Ninja.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Ninja.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedItemCreatorSystem)})]
public sealed class ItemCreatorComponent : 
  Component,
  ISerializationGenerated<ItemCreatorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Battery;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId<InstantActionComponent> Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ActionEntity;
  [DataField(null, false, 1, true, false, null)]
  public float Charge = 14.4f;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId SpawnedPrototype = (EntProtoId) string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public LocId NoPowerPopup = (LocId) string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemCreatorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemCreatorComponent) target1;
    if (serialization.TryCustomCopy<ItemCreatorComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Battery, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Battery, hookCtx, context);
    target.Battery = target2;
    EntProtoId<InstantActionComponent> target3 = new EntProtoId<InstantActionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<InstantActionComponent>>(this.Action, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId<InstantActionComponent>>(this.Action, hookCtx, context);
    target.Action = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionEntity, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.ActionEntity, hookCtx, context);
    target.ActionEntity = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Charge, ref target5, hookCtx, false, context))
      target5 = this.Charge;
    target.Charge = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.SpawnedPrototype, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.SpawnedPrototype, hookCtx, context);
    target.SpawnedPrototype = target6;
    LocId target7 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.NoPowerPopup, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId>(this.NoPowerPopup, hookCtx, context);
    target.NoPowerPopup = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemCreatorComponent target,
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
    ItemCreatorComponent target1 = (ItemCreatorComponent) target;
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
    ItemCreatorComponent target1 = (ItemCreatorComponent) target;
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
    ItemCreatorComponent target1 = (ItemCreatorComponent) target;
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
  virtual ItemCreatorComponent Component.Instantiate() => new ItemCreatorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ItemCreatorComponent_AutoState : IComponentState
  {
    public NetEntity? Battery;
    public NetEntity? ActionEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ItemCreatorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ItemCreatorComponent, ComponentGetState>(new ComponentEventRefHandler<ItemCreatorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ItemCreatorComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemCreatorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ItemCreatorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ItemCreatorComponent.ItemCreatorComponent_AutoState()
      {
        Battery = this.GetNetEntity(component.Battery),
        ActionEntity = this.GetNetEntity(component.ActionEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ItemCreatorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ItemCreatorComponent.ItemCreatorComponent_AutoState current))
        return;
      component.Battery = this.EnsureEntity<ItemCreatorComponent>(current.Battery, uid);
      component.ActionEntity = this.EnsureEntity<ItemCreatorComponent>(current.ActionEntity, uid);
    }
  }
}
