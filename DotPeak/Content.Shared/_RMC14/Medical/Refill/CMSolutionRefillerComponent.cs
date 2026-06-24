// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Refill.CMSolutionRefillerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Medical.Refill;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (CMRefillableSolutionSystem)})]
public sealed class CMSolutionRefillerComponent : 
  Component,
  ISerializationGenerated<CMSolutionRefillerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<ReagentPrototype>> Reagents = new HashSet<ProtoId<ReagentPrototype>>();
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist Whitelist = new EntityWhitelist();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Current;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Max;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RandomizeReagentsPlanetside;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Recharge;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RechargeCooldown;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan RechargeAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMSolutionRefillerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMSolutionRefillerComponent) target1;
    if (serialization.TryCustomCopy<CMSolutionRefillerComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<ReagentPrototype>> target2 = (HashSet<ProtoId<ReagentPrototype>>) null;
    if (this.Reagents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<ReagentPrototype>>>(this.Reagents, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<ProtoId<ReagentPrototype>>>(this.Reagents, hookCtx, context);
    target.Reagents = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, context, true);
    }
    target.Whitelist = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Current, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.Current, hookCtx, context);
    target.Current = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Max, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.Max, hookCtx, context);
    target.Max = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.RandomizeReagentsPlanetside, ref target6, hookCtx, false, context))
      target6 = this.RandomizeReagentsPlanetside;
    target.RandomizeReagentsPlanetside = target6;
    FixedPoint2 target7 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Recharge, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<FixedPoint2>(this.Recharge, hookCtx, context);
    target.Recharge = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RechargeCooldown, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.RechargeCooldown, hookCtx, context);
    target.RechargeCooldown = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RechargeAt, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.RechargeAt, hookCtx, context);
    target.RechargeAt = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMSolutionRefillerComponent target,
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
    CMSolutionRefillerComponent target1 = (CMSolutionRefillerComponent) target;
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
    CMSolutionRefillerComponent target1 = (CMSolutionRefillerComponent) target;
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
    CMSolutionRefillerComponent target1 = (CMSolutionRefillerComponent) target;
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
  virtual CMSolutionRefillerComponent Component.Instantiate() => new CMSolutionRefillerComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMSolutionRefillerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMSolutionRefillerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<CMSolutionRefillerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      CMSolutionRefillerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.RechargeAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMSolutionRefillerComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    HashSet<ProtoId<ReagentPrototype>> Reagents;
    public EntityWhitelist Whitelist;
    public FixedPoint2 Current;
    public FixedPoint2 Max;
    public bool RandomizeReagentsPlanetside;
    public FixedPoint2 Recharge;
    public TimeSpan RechargeCooldown;
    public TimeSpan RechargeAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMSolutionRefillerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMSolutionRefillerComponent, ComponentGetState>(new ComponentEventRefHandler<CMSolutionRefillerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMSolutionRefillerComponent, ComponentHandleState>(new ComponentEventRefHandler<CMSolutionRefillerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMSolutionRefillerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMSolutionRefillerComponent.CMSolutionRefillerComponent_AutoState()
      {
        Reagents = component.Reagents,
        Whitelist = component.Whitelist,
        Current = component.Current,
        Max = component.Max,
        RandomizeReagentsPlanetside = component.RandomizeReagentsPlanetside,
        Recharge = component.Recharge,
        RechargeCooldown = component.RechargeCooldown,
        RechargeAt = component.RechargeAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMSolutionRefillerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMSolutionRefillerComponent.CMSolutionRefillerComponent_AutoState current))
        return;
      component.Reagents = current.Reagents == null ? (HashSet<ProtoId<ReagentPrototype>>) null : new HashSet<ProtoId<ReagentPrototype>>((IEnumerable<ProtoId<ReagentPrototype>>) current.Reagents);
      component.Whitelist = current.Whitelist;
      component.Current = current.Current;
      component.Max = current.Max;
      component.RandomizeReagentsPlanetside = current.RandomizeReagentsPlanetside;
      component.Recharge = current.Recharge;
      component.RechargeCooldown = current.RechargeCooldown;
      component.RechargeAt = current.RechargeAt;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CMSolutionRefillerComponent>(uid, component, ref args1);
    }
  }
}
