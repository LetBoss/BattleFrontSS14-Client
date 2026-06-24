// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.GrantMarineIconsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedMarineSystem)})]
public sealed class GrantMarineIconsComponent : 
  Component,
  IClothingSlots,
  ISerializationGenerated<GrantMarineIconsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<NpcFactionPrototype>>? Factions;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BypassFactionIcons;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags Slots { get; set; } = SlotFlags.EARS;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GrantMarineIconsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GrantMarineIconsComponent) target1;
    if (serialization.TryCustomCopy<GrantMarineIconsComponent>(this, ref target, hookCtx, false, context))
      return;
    SlotFlags target2 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slots, ref target2, hookCtx, false, context))
      target2 = this.Slots;
    target.Slots = target2;
    List<ProtoId<NpcFactionPrototype>> target3 = (List<ProtoId<NpcFactionPrototype>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<NpcFactionPrototype>>>(this.Factions, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<ProtoId<NpcFactionPrototype>>>(this.Factions, hookCtx, context);
    target.Factions = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.BypassFactionIcons, ref target4, hookCtx, false, context))
      target4 = this.BypassFactionIcons;
    target.BypassFactionIcons = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GrantMarineIconsComponent target,
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
    GrantMarineIconsComponent target1 = (GrantMarineIconsComponent) target;
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
    GrantMarineIconsComponent target1 = (GrantMarineIconsComponent) target;
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
    GrantMarineIconsComponent target1 = (GrantMarineIconsComponent) target;
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
  virtual GrantMarineIconsComponent Component.Instantiate() => new GrantMarineIconsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GrantMarineIconsComponent_AutoState : IComponentState
  {
    public SlotFlags Slots;
    public List<ProtoId<NpcFactionPrototype>>? Factions;
    public bool BypassFactionIcons;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GrantMarineIconsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GrantMarineIconsComponent, ComponentGetState>(new ComponentEventRefHandler<GrantMarineIconsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GrantMarineIconsComponent, ComponentHandleState>(new ComponentEventRefHandler<GrantMarineIconsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GrantMarineIconsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GrantMarineIconsComponent.GrantMarineIconsComponent_AutoState()
      {
        Slots = component.Slots,
        Factions = component.Factions,
        BypassFactionIcons = component.BypassFactionIcons
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GrantMarineIconsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GrantMarineIconsComponent.GrantMarineIconsComponent_AutoState current))
        return;
      component.Slots = current.Slots;
      component.Factions = current.Factions == null ? (List<ProtoId<NpcFactionPrototype>>) null : new List<ProtoId<NpcFactionPrototype>>((IEnumerable<ProtoId<NpcFactionPrototype>>) current.Factions);
      component.BypassFactionIcons = current.BypassFactionIcons;
    }
  }
}
