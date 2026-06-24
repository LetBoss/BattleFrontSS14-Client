// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.ToggleableClothingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clothing.Components;

[Access(new Type[] {typeof (ToggleableClothingSystem)})]
[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ToggleableClothingComponent : 
  Component,
  ISerializationGenerated<ToggleableClothingComponent>,
  ISerializationGenerated
{
  public const string DefaultClothingContainerId = "toggleable-clothing";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Action = EntProtoId.op_Implicit("ActionToggleSuitPiece");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ActionEntity;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId ClothingPrototype;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Slot = "head";
  [DataField("requiredSlot", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags RequiredFlags = SlotFlags.OUTERCLOTHING;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "toggleable-clothing";
  [Robust.Shared.ViewVariables.ViewVariables]
  public ContainerSlot? Container;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ClothingUid;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? StripDelay = new TimeSpan?(TimeSpan.FromSeconds(3L));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? VerbText;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ToggleableClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ToggleableClothingComponent) component;
    if (serialization.TryCustomCopy<ToggleableClothingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId entProtoId1 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Action, ref entProtoId1, hookCtx, false, context))
      entProtoId1 = serialization.CreateCopy<EntProtoId>(this.Action, hookCtx, context, false);
    target.Action = entProtoId1;
    EntityUid? nullable1 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionEntity, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<EntityUid?>(this.ActionEntity, hookCtx, context, false);
    target.ActionEntity = nullable1;
    EntProtoId entProtoId2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ClothingPrototype, ref entProtoId2, hookCtx, false, context))
      entProtoId2 = serialization.CreateCopy<EntProtoId>(this.ClothingPrototype, hookCtx, context, false);
    target.ClothingPrototype = entProtoId2;
    string str1 = (string) null;
    if (this.Slot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Slot, ref str1, hookCtx, false, context))
      str1 = this.Slot;
    target.Slot = str1;
    SlotFlags slotFlags = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.RequiredFlags, ref slotFlags, hookCtx, false, context))
      slotFlags = this.RequiredFlags;
    target.RequiredFlags = slotFlags;
    string str2 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref str2, hookCtx, false, context))
      str2 = this.ContainerId;
    target.ContainerId = str2;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ClothingUid, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.ClothingUid, hookCtx, context, false);
    target.ClothingUid = nullable2;
    TimeSpan? nullable3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.StripDelay, ref nullable3, hookCtx, false, context))
      nullable3 = serialization.CreateCopy<TimeSpan?>(this.StripDelay, hookCtx, context, false);
    target.StripDelay = nullable3;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.VerbText, ref str3, hookCtx, false, context))
      str3 = this.VerbText;
    target.VerbText = str3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ToggleableClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ToggleableClothingComponent target1 = (ToggleableClothingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ToggleableClothingComponent target1 = (ToggleableClothingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ToggleableClothingComponent target1 = (ToggleableClothingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ToggleableClothingComponent Component.Instantiate() => new ToggleableClothingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ToggleableClothingComponent_AutoState : IComponentState
  {
    public EntProtoId Action;
    public NetEntity? ActionEntity;
    public EntProtoId ClothingPrototype;
    public string Slot;
    public SlotFlags RequiredFlags;
    public string ContainerId;
    public NetEntity? ClothingUid;
    public TimeSpan? StripDelay;
    public string? VerbText;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ToggleableClothingComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ToggleableClothingComponent, ComponentGetState>(new ComponentEventRefHandler<ToggleableClothingComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ToggleableClothingComponent, ComponentHandleState>(new ComponentEventRefHandler<ToggleableClothingComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      ToggleableClothingComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ToggleableClothingComponent.ToggleableClothingComponent_AutoState()
      {
        Action = component.Action,
        ActionEntity = this.GetNetEntity(component.ActionEntity, (MetaDataComponent) null),
        ClothingPrototype = component.ClothingPrototype,
        Slot = component.Slot,
        RequiredFlags = component.RequiredFlags,
        ContainerId = component.ContainerId,
        ClothingUid = this.GetNetEntity(component.ClothingUid, (MetaDataComponent) null),
        StripDelay = component.StripDelay,
        VerbText = component.VerbText
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ToggleableClothingComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ToggleableClothingComponent.ToggleableClothingComponent_AutoState current))
        return;
      component.Action = current.Action;
      component.ActionEntity = this.EnsureEntity<ToggleableClothingComponent>(current.ActionEntity, uid);
      component.ClothingPrototype = current.ClothingPrototype;
      component.Slot = current.Slot;
      component.RequiredFlags = current.RequiredFlags;
      component.ContainerId = current.ContainerId;
      component.ClothingUid = this.EnsureEntity<ToggleableClothingComponent>(current.ClothingUid, uid);
      component.StripDelay = current.StripDelay;
      component.VerbText = current.VerbText;
    }
  }
}
