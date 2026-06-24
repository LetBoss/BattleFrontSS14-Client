// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Orders.CivSquadLeaderOrdersComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Orders;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class CivSquadLeaderOrdersComponent : 
  Component,
  ISerializationGenerated<CivSquadLeaderOrdersComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
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

  public override bool SessionSpecific => true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivSquadLeaderOrdersComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivSquadLeaderOrdersComponent) target1;
    if (serialization.TryCustomCopy<CivSquadLeaderOrdersComponent>(this, ref target, hookCtx, false, context))
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
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivSquadLeaderOrdersComponent target,
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
    CivSquadLeaderOrdersComponent target1 = (CivSquadLeaderOrdersComponent) target;
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
    CivSquadLeaderOrdersComponent target1 = (CivSquadLeaderOrdersComponent) target;
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
    CivSquadLeaderOrdersComponent target1 = (CivSquadLeaderOrdersComponent) target;
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
  virtual CivSquadLeaderOrdersComponent Component.Instantiate()
  {
    return new CivSquadLeaderOrdersComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CivSquadLeaderOrdersComponent_AutoState : IComponentState
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
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivSquadLeaderOrdersComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivSquadLeaderOrdersComponent, ComponentGetState>(new ComponentEventRefHandler<CivSquadLeaderOrdersComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CivSquadLeaderOrdersComponent, ComponentHandleState>(new ComponentEventRefHandler<CivSquadLeaderOrdersComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CivSquadLeaderOrdersComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CivSquadLeaderOrdersComponent.CivSquadLeaderOrdersComponent_AutoState()
      {
        Duration = component.Duration,
        Cooldown = component.Cooldown,
        OrderRange = component.OrderRange,
        FocusAction = component.FocusAction,
        FocusActionEntity = this.GetNetEntity(component.FocusActionEntity),
        HoldAction = component.HoldAction,
        HoldActionEntity = this.GetNetEntity(component.HoldActionEntity),
        MoveAction = component.MoveAction,
        MoveActionEntity = this.GetNetEntity(component.MoveActionEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CivSquadLeaderOrdersComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CivSquadLeaderOrdersComponent.CivSquadLeaderOrdersComponent_AutoState current))
        return;
      component.Duration = current.Duration;
      component.Cooldown = current.Cooldown;
      component.OrderRange = current.OrderRange;
      component.FocusAction = current.FocusAction;
      component.FocusActionEntity = this.EnsureEntity<CivSquadLeaderOrdersComponent>(current.FocusActionEntity, uid);
      component.HoldAction = current.HoldAction;
      component.HoldActionEntity = this.EnsureEntity<CivSquadLeaderOrdersComponent>(current.HoldActionEntity, uid);
      component.MoveAction = current.MoveAction;
      component.MoveActionEntity = this.EnsureEntity<CivSquadLeaderOrdersComponent>(current.MoveActionEntity, uid);
    }
  }
}
