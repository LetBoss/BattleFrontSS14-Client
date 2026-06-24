// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Orders.MarineOrdersComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
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
namespace Content.Shared._RMC14.Marines.Orders;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class MarineOrdersComponent : 
  Component,
  ISerializationGenerated<MarineOrdersComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(80L /*0x50*/);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int OrderRange = 7;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId FocusAction = (EntProtoId) "ActionMarineFocus";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? FocusActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId HoldAction = (EntProtoId) "ActionMarineHold";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? HoldActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId MoveAction = (EntProtoId) "ActionMarineMove";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? MoveActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<LocId> MoveCallouts = new List<LocId>()
  {
    (LocId) "move-order-callout-1",
    (LocId) "move-order-callout-2",
    (LocId) "move-order-callout-3",
    (LocId) "move-order-callout-4",
    (LocId) "move-order-callout-5",
    (LocId) "move-order-callout-6",
    (LocId) "move-order-callout-7",
    (LocId) "move-order-callout-8",
    (LocId) "move-order-callout-9",
    (LocId) "move-order-callout-10",
    (LocId) "move-order-callout-11",
    (LocId) "move-order-callout-12",
    (LocId) "move-order-callout-13",
    (LocId) "move-order-callout-14",
    (LocId) "move-order-callout-15"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<LocId> FocusCallouts = new List<LocId>()
  {
    (LocId) "focus-order-callout-1",
    (LocId) "focus-order-callout-2",
    (LocId) "focus-order-callout-3",
    (LocId) "focus-order-callout-4",
    (LocId) "focus-order-callout-5",
    (LocId) "focus-order-callout-6",
    (LocId) "focus-order-callout-7",
    (LocId) "focus-order-callout-8",
    (LocId) "focus-order-callout-9",
    (LocId) "focus-order-callout-10",
    (LocId) "focus-order-callout-11",
    (LocId) "focus-order-callout-12",
    (LocId) "focus-order-callout-13",
    (LocId) "focus-order-callout-14",
    (LocId) "focus-order-callout-15",
    (LocId) "focus-order-callout-16",
    (LocId) "focus-order-callout-17",
    (LocId) "focus-order-callout-18",
    (LocId) "focus-order-callout-19"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<LocId> HoldCallouts = new List<LocId>()
  {
    (LocId) "hold-order-callout-1",
    (LocId) "hold-order-callout-2",
    (LocId) "hold-order-callout-3",
    (LocId) "hold-order-callout-4",
    (LocId) "hold-order-callout-5",
    (LocId) "hold-order-callout-6",
    (LocId) "hold-order-callout-7",
    (LocId) "hold-order-callout-8",
    (LocId) "hold-order-callout-9",
    (LocId) "hold-order-callout-10",
    (LocId) "hold-order-callout-11",
    (LocId) "hold-order-callout-12",
    (LocId) "hold-order-callout-13",
    (LocId) "hold-order-callout-14",
    (LocId) "hold-order-callout-15",
    (LocId) "hold-order-callout-16",
    (LocId) "hold-order-callout-17"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillLeadership";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Intrinsic = true;

  public override bool SessionSpecific => true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MarineOrdersComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MarineOrdersComponent) target1;
    if (serialization.TryCustomCopy<MarineOrdersComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.OrderRange, ref target4, hookCtx, false, context))
      target4 = this.OrderRange;
    target.OrderRange = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.FocusAction, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.FocusAction, hookCtx, context);
    target.FocusAction = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.FocusActionEntity, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.FocusActionEntity, hookCtx, context);
    target.FocusActionEntity = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HoldAction, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.HoldAction, hookCtx, context);
    target.HoldAction = target7;
    EntityUid? target8 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.HoldActionEntity, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntityUid?>(this.HoldActionEntity, hookCtx, context);
    target.HoldActionEntity = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.MoveAction, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.MoveAction, hookCtx, context);
    target.MoveAction = target9;
    EntityUid? target10 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.MoveActionEntity, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntityUid?>(this.MoveActionEntity, hookCtx, context);
    target.MoveActionEntity = target10;
    List<LocId> target11 = (List<LocId>) null;
    if (this.MoveCallouts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<LocId>>(this.MoveCallouts, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<List<LocId>>(this.MoveCallouts, hookCtx, context);
    target.MoveCallouts = target11;
    List<LocId> target12 = (List<LocId>) null;
    if (this.FocusCallouts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<LocId>>(this.FocusCallouts, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<List<LocId>>(this.FocusCallouts, hookCtx, context);
    target.FocusCallouts = target12;
    List<LocId> target13 = (List<LocId>) null;
    if (this.HoldCallouts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<LocId>>(this.HoldCallouts, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<List<LocId>>(this.HoldCallouts, hookCtx, context);
    target.HoldCallouts = target13;
    EntProtoId<SkillDefinitionComponent> target14 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.Intrinsic, ref target15, hookCtx, false, context))
      target15 = this.Intrinsic;
    target.Intrinsic = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MarineOrdersComponent target,
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
    MarineOrdersComponent target1 = (MarineOrdersComponent) target;
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
    MarineOrdersComponent target1 = (MarineOrdersComponent) target;
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
    MarineOrdersComponent target1 = (MarineOrdersComponent) target;
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
  virtual MarineOrdersComponent Component.Instantiate() => new MarineOrdersComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MarineOrdersComponent_AutoState : IComponentState
  {
    public TimeSpan Duration;
    public TimeSpan Cooldown;
    public int OrderRange;
    public EntProtoId FocusAction;
    public NetEntity? FocusActionEntity;
    public EntProtoId HoldAction;
    public NetEntity? HoldActionEntity;
    public EntProtoId MoveAction;
    public NetEntity? MoveActionEntity;
    public List<LocId> MoveCallouts;
    public List<LocId> FocusCallouts;
    public List<LocId> HoldCallouts;
    public EntProtoId<SkillDefinitionComponent> Skill;
    public bool Intrinsic;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MarineOrdersComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MarineOrdersComponent, ComponentGetState>(new ComponentEventRefHandler<MarineOrdersComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MarineOrdersComponent, ComponentHandleState>(new ComponentEventRefHandler<MarineOrdersComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MarineOrdersComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MarineOrdersComponent.MarineOrdersComponent_AutoState()
      {
        Duration = component.Duration,
        Cooldown = component.Cooldown,
        OrderRange = component.OrderRange,
        FocusAction = component.FocusAction,
        FocusActionEntity = this.GetNetEntity(component.FocusActionEntity),
        HoldAction = component.HoldAction,
        HoldActionEntity = this.GetNetEntity(component.HoldActionEntity),
        MoveAction = component.MoveAction,
        MoveActionEntity = this.GetNetEntity(component.MoveActionEntity),
        MoveCallouts = component.MoveCallouts,
        FocusCallouts = component.FocusCallouts,
        HoldCallouts = component.HoldCallouts,
        Skill = component.Skill,
        Intrinsic = component.Intrinsic
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MarineOrdersComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MarineOrdersComponent.MarineOrdersComponent_AutoState current))
        return;
      component.Duration = current.Duration;
      component.Cooldown = current.Cooldown;
      component.OrderRange = current.OrderRange;
      component.FocusAction = current.FocusAction;
      component.FocusActionEntity = this.EnsureEntity<MarineOrdersComponent>(current.FocusActionEntity, uid);
      component.HoldAction = current.HoldAction;
      component.HoldActionEntity = this.EnsureEntity<MarineOrdersComponent>(current.HoldActionEntity, uid);
      component.MoveAction = current.MoveAction;
      component.MoveActionEntity = this.EnsureEntity<MarineOrdersComponent>(current.MoveActionEntity, uid);
      component.MoveCallouts = current.MoveCallouts == null ? (List<LocId>) null : new List<LocId>((IEnumerable<LocId>) current.MoveCallouts);
      component.FocusCallouts = current.FocusCallouts == null ? (List<LocId>) null : new List<LocId>((IEnumerable<LocId>) current.FocusCallouts);
      component.HoldCallouts = current.HoldCallouts == null ? (List<LocId>) null : new List<LocId>((IEnumerable<LocId>) current.HoldCallouts);
      component.Skill = current.Skill;
      component.Intrinsic = current.Intrinsic;
    }
  }
}
