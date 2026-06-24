// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.ClothingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (ClothingSystem), typeof (InventorySystem)})]
public sealed class ClothingComponent : 
  Component,
  ISerializationGenerated<ClothingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, List<PrototypeLayerData>> ClothingVisuals = new Dictionary<string, List<PrototypeLayerData>>();
  [DataField(null, false, 1, false, false, null)]
  public string? MappedLayer;
  [DataField(null, false, 1, false, false, null)]
  public bool QuickEquip = true;
  [DataField(null, false, 1, true, false, null)]
  [Access(new Type[] {typeof (ClothingSystem), typeof (InventorySystem)})]
  public SlotFlags Slots;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? EquipSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? UnequipSound;
  [Access(new Type[] {typeof (ClothingSystem)})]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? EquippedPrefix;
  [Access(new Type[] {typeof (ClothingSystem)})]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? EquippedState;
  [DataField("sprite", false, 1, false, false, null)]
  public string? RsiPath;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? InSlot;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags? InSlotFlag;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan EquipDelay = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UnequipDelay = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan StripDelay = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ClothingComponent) component;
    if (serialization.TryCustomCopy<ClothingComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, List<PrototypeLayerData>> dictionary = (Dictionary<string, List<PrototypeLayerData>>) null;
    if (this.ClothingVisuals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, List<PrototypeLayerData>>>(this.ClothingVisuals, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, List<PrototypeLayerData>>>(this.ClothingVisuals, hookCtx, context, false);
    target.ClothingVisuals = dictionary;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.MappedLayer, ref str1, hookCtx, false, context))
      str1 = this.MappedLayer;
    target.MappedLayer = str1;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.QuickEquip, ref flag, hookCtx, false, context))
      flag = this.QuickEquip;
    target.QuickEquip = flag;
    SlotFlags slotFlags = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slots, ref slotFlags, hookCtx, false, context))
      slotFlags = this.Slots;
    target.Slots = slotFlags;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EquipSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.EquipSound, hookCtx, context, false);
    target.EquipSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnequipSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.UnequipSound, hookCtx, context, false);
    target.UnequipSound = soundSpecifier2;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.EquippedPrefix, ref str2, hookCtx, false, context))
      str2 = this.EquippedPrefix;
    target.EquippedPrefix = str2;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.EquippedState, ref str3, hookCtx, false, context))
      str3 = this.EquippedState;
    target.EquippedState = str3;
    string str4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.RsiPath, ref str4, hookCtx, false, context))
      str4 = this.RsiPath;
    target.RsiPath = str4;
    string str5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.InSlot, ref str5, hookCtx, false, context))
      str5 = this.InSlot;
    target.InSlot = str5;
    SlotFlags? nullable = new SlotFlags?();
    if (!serialization.TryCustomCopy<SlotFlags?>(this.InSlotFlag, ref nullable, hookCtx, false, context))
      nullable = this.InSlotFlag;
    target.InSlotFlag = nullable;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EquipDelay, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.EquipDelay, hookCtx, context, false);
    target.EquipDelay = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnequipDelay, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.UnequipDelay, hookCtx, context, false);
    target.UnequipDelay = timeSpan2;
    TimeSpan timeSpan3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StripDelay, ref timeSpan3, hookCtx, false, context))
      timeSpan3 = serialization.CreateCopy<TimeSpan>(this.StripDelay, hookCtx, context, false);
    target.StripDelay = timeSpan3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ClothingComponent target,
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
    ClothingComponent target1 = (ClothingComponent) target;
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
    ClothingComponent target1 = (ClothingComponent) target;
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
    ClothingComponent target1 = (ClothingComponent) target;
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
  virtual ClothingComponent Component.Instantiate() => new ClothingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ClothingComponent_AutoState : IComponentState
  {
    public string? EquippedPrefix;
    public string? EquippedState;
    public string? InSlot;
    public SlotFlags? InSlotFlag;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ClothingComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ClothingComponent, ComponentGetState>(new ComponentEventRefHandler<ClothingComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ClothingComponent, ComponentHandleState>(new ComponentEventRefHandler<ClothingComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, ClothingComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ClothingComponent.ClothingComponent_AutoState()
      {
        EquippedPrefix = component.EquippedPrefix,
        EquippedState = component.EquippedState,
        InSlot = component.InSlot,
        InSlotFlag = component.InSlotFlag
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ClothingComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ClothingComponent.ClothingComponent_AutoState current))
        return;
      component.EquippedPrefix = current.EquippedPrefix;
      component.EquippedState = current.EquippedState;
      component.InSlot = current.InSlot;
      component.InSlotFlag = current.InSlotFlag;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, ClothingComponent>(uid, component, ref handleStateEvent);
    }
  }
}
