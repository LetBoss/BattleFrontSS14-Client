// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.Firewalk.FirewalkArmorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Armor.Firewalk;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (FirewalkSystem)})]
public sealed class FirewalkArmorComponent : 
  Component,
  ISerializationGenerated<FirewalkArmorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist Whitelist = new EntityWhitelist();
  [DataField(null, false, 1, true, false, null)]
  public ComponentRegistry AddComponentsOnFirewalk = new ComponentRegistry();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionFireShield";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags Slots = SlotFlags.OUTERCLOTHING;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FirewalkTime = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color AuraColor = Color.Teal;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FirewalkArmorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FirewalkArmorComponent) target1;
    if (serialization.TryCustomCopy<FirewalkArmorComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context, true);
    }
    target.Whitelist = target2;
    ComponentRegistry target3 = (ComponentRegistry) null;
    if (this.AddComponentsOnFirewalk == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.AddComponentsOnFirewalk, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ComponentRegistry>(this.AddComponentsOnFirewalk, hookCtx, context);
    target.AddComponentsOnFirewalk = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target5;
    SlotFlags target6 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slots, ref target6, hookCtx, false, context))
      target6 = this.Slots;
    target.Slots = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FirewalkTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.FirewalkTime, hookCtx, context);
    target.FirewalkTime = target7;
    Color target8 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.AuraColor, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Color>(this.AuraColor, hookCtx, context);
    target.AuraColor = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FirewalkArmorComponent target,
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
    FirewalkArmorComponent target1 = (FirewalkArmorComponent) target;
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
    FirewalkArmorComponent target1 = (FirewalkArmorComponent) target;
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
    FirewalkArmorComponent target1 = (FirewalkArmorComponent) target;
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
  virtual FirewalkArmorComponent Component.Instantiate() => new FirewalkArmorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FirewalkArmorComponent_AutoState : IComponentState
  {
    public EntityWhitelist Whitelist;
    public EntProtoId ActionId;
    public NetEntity? Action;
    public SlotFlags Slots;
    public TimeSpan FirewalkTime;
    public Color AuraColor;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FirewalkArmorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FirewalkArmorComponent, ComponentGetState>(new ComponentEventRefHandler<FirewalkArmorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FirewalkArmorComponent, ComponentHandleState>(new ComponentEventRefHandler<FirewalkArmorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      FirewalkArmorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new FirewalkArmorComponent.FirewalkArmorComponent_AutoState()
      {
        Whitelist = component.Whitelist,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        Slots = component.Slots,
        FirewalkTime = component.FirewalkTime,
        AuraColor = component.AuraColor
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FirewalkArmorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FirewalkArmorComponent.FirewalkArmorComponent_AutoState current))
        return;
      component.Whitelist = current.Whitelist;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<FirewalkArmorComponent>(current.Action, uid);
      component.Slots = current.Slots;
      component.FirewalkTime = current.FirewalkTime;
      component.AuraColor = current.AuraColor;
    }
  }
}
