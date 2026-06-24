// Decompiled with JetBrains decompiler
// Type: Content.Shared.Implants.Components.ImplanterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
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
namespace Content.Shared.Implants.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class ImplanterComponent : 
  Component,
  ISerializationGenerated<ImplanterComponent>,
  ISerializationGenerated
{
  public const string ImplanterSlotId = "implanter_slot";
  public const string ImplantSlotId = "implant";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? Implant;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public float ImplantTime = 5f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public float DrawTime = 25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ImplantOnly;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ImplanterToggleMode CurrentMode;
  [DataField(null, false, 1, false, false, null)]
  public (string, string) ImplantData;
  [DataField(null, false, 1, false, false, null)]
  public bool AllowMultipleImplants;
  [DataField(null, false, 1, true, false, null)]
  public ItemSlot ImplanterSlot = new ItemSlot();
  [DataField(null, false, 1, false, false, null)]
  public bool AllowDeimplantAll;
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> DeimplantWhitelist = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier DeimplantFailureDamage = new DamageSpecifier();
  [AutoNetworkedField]
  public EntProtoId? DeimplantChosen;
  public bool UiUpdateNeeded;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ImplanterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ImplanterComponent) target1;
    if (serialization.TryCustomCopy<ImplanterComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    target.Whitelist = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, context);
    }
    target.Blacklist = target3;
    EntProtoId? target4 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Implant, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId?>(this.Implant, hookCtx, context);
    target.Implant = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ImplantTime, ref target5, hookCtx, false, context))
      target5 = this.ImplantTime;
    target.ImplantTime = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DrawTime, ref target6, hookCtx, false, context))
      target6 = this.DrawTime;
    target.DrawTime = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.ImplantOnly, ref target7, hookCtx, false, context))
      target7 = this.ImplantOnly;
    target.ImplantOnly = target7;
    ImplanterToggleMode target8 = ImplanterToggleMode.Inject;
    if (!serialization.TryCustomCopy<ImplanterToggleMode>(this.CurrentMode, ref target8, hookCtx, false, context))
      target8 = this.CurrentMode;
    target.CurrentMode = target8;
    (string, string) target9 = ();
    if (!serialization.TryCustomCopy<(string, string)>(this.ImplantData, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<(string, string)>(this.ImplantData, hookCtx, context);
    target.ImplantData = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowMultipleImplants, ref target10, hookCtx, false, context))
      target10 = this.AllowMultipleImplants;
    target.AllowMultipleImplants = target10;
    ItemSlot target11 = (ItemSlot) null;
    if (this.ImplanterSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.ImplanterSlot, ref target11, hookCtx, false, context))
    {
      if (this.ImplanterSlot == null)
        target11 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.ImplanterSlot, ref target11, hookCtx, context, true);
    }
    target.ImplanterSlot = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowDeimplantAll, ref target12, hookCtx, false, context))
      target12 = this.AllowDeimplantAll;
    target.AllowDeimplantAll = target12;
    List<EntProtoId> target13 = (List<EntProtoId>) null;
    if (this.DeimplantWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.DeimplantWhitelist, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<List<EntProtoId>>(this.DeimplantWhitelist, hookCtx, context);
    target.DeimplantWhitelist = target13;
    DamageSpecifier target14 = (DamageSpecifier) null;
    if (this.DeimplantFailureDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.DeimplantFailureDamage, ref target14, hookCtx, false, context))
    {
      if (this.DeimplantFailureDamage == null)
        target14 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.DeimplantFailureDamage, ref target14, hookCtx, context, true);
    }
    target.DeimplantFailureDamage = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ImplanterComponent target,
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
    ImplanterComponent target1 = (ImplanterComponent) target;
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
    ImplanterComponent target1 = (ImplanterComponent) target;
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
    ImplanterComponent target1 = (ImplanterComponent) target;
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
  virtual ImplanterComponent Component.Instantiate() => new ImplanterComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ImplanterComponent_AutoState : IComponentState
  {
    public EntityWhitelist? Whitelist;
    public EntityWhitelist? Blacklist;
    public bool ImplantOnly;
    public ImplanterToggleMode CurrentMode;
    public EntProtoId? DeimplantChosen;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ImplanterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ImplanterComponent, ComponentGetState>(new ComponentEventRefHandler<ImplanterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ImplanterComponent, ComponentHandleState>(new ComponentEventRefHandler<ImplanterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ImplanterComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ImplanterComponent.ImplanterComponent_AutoState()
      {
        Whitelist = component.Whitelist,
        Blacklist = component.Blacklist,
        ImplantOnly = component.ImplantOnly,
        CurrentMode = component.CurrentMode,
        DeimplantChosen = component.DeimplantChosen
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ImplanterComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ImplanterComponent.ImplanterComponent_AutoState current))
        return;
      component.Whitelist = current.Whitelist;
      component.Blacklist = current.Blacklist;
      component.ImplantOnly = current.ImplantOnly;
      component.CurrentMode = current.CurrentMode;
      component.DeimplantChosen = current.DeimplantChosen;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, ImplanterComponent>(uid, component, ref args1);
    }
  }
}
