// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Inventory.CMItemSlotsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Inventory;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedCMInventorySystem)})]
public sealed class CMItemSlotsComponent : 
  Component,
  ISerializationGenerated<CMItemSlotsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastEjectAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? Cooldown;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? CooldownPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? Count;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ItemSlot? Slot;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? StartingItem;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId>? StartingItems;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMItemSlotsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMItemSlotsComponent) target1;
    if (serialization.TryCustomCopy<CMItemSlotsComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastEjectAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.LastEjectAt, hookCtx, context);
    target.LastEjectAt = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.Cooldown, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.Cooldown, hookCtx, context);
    target.Cooldown = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.CooldownPopup, ref target4, hookCtx, false, context))
      target4 = this.CooldownPopup;
    target.CooldownPopup = target4;
    int? target5 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Count, ref target5, hookCtx, false, context))
      target5 = this.Count;
    target.Count = target5;
    ItemSlot target6 = (ItemSlot) null;
    if (!serialization.TryCustomCopy<ItemSlot>(this.Slot, ref target6, hookCtx, false, context))
    {
      if (this.Slot == null)
        target6 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.Slot, ref target6, hookCtx, context);
    }
    target.Slot = target6;
    EntProtoId? target7 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.StartingItem, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId?>(this.StartingItem, hookCtx, context);
    target.StartingItem = target7;
    List<EntProtoId> target8 = (List<EntProtoId>) null;
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.StartingItems, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<EntProtoId>>(this.StartingItems, hookCtx, context);
    target.StartingItems = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMItemSlotsComponent target,
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
    CMItemSlotsComponent target1 = (CMItemSlotsComponent) target;
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
    CMItemSlotsComponent target1 = (CMItemSlotsComponent) target;
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
    CMItemSlotsComponent target1 = (CMItemSlotsComponent) target;
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
  virtual CMItemSlotsComponent Component.Instantiate() => new CMItemSlotsComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMItemSlotsComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMItemSlotsComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<CMItemSlotsComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      CMItemSlotsComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastEjectAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMItemSlotsComponent_AutoState : IComponentState
  {
    public TimeSpan LastEjectAt;
    public TimeSpan? Cooldown;
    public 
    #nullable enable
    string? CooldownPopup;
    public int? Count;
    public ItemSlot? Slot;
    public EntProtoId? StartingItem;
    public List<EntProtoId>? StartingItems;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMItemSlotsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMItemSlotsComponent, ComponentGetState>(new ComponentEventRefHandler<CMItemSlotsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMItemSlotsComponent, ComponentHandleState>(new ComponentEventRefHandler<CMItemSlotsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMItemSlotsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMItemSlotsComponent.CMItemSlotsComponent_AutoState()
      {
        LastEjectAt = component.LastEjectAt,
        Cooldown = component.Cooldown,
        CooldownPopup = component.CooldownPopup,
        Count = component.Count,
        Slot = component.Slot,
        StartingItem = component.StartingItem,
        StartingItems = component.StartingItems
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMItemSlotsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMItemSlotsComponent.CMItemSlotsComponent_AutoState current))
        return;
      component.LastEjectAt = current.LastEjectAt;
      component.Cooldown = current.Cooldown;
      component.CooldownPopup = current.CooldownPopup;
      component.Count = current.Count;
      component.Slot = current.Slot;
      component.StartingItem = current.StartingItem;
      component.StartingItems = current.StartingItems == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.StartingItems);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CMItemSlotsComponent>(uid, component, ref args1);
    }
  }
}
