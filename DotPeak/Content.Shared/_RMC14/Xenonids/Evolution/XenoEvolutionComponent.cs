// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Evolution.XenoEvolutionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared._RMC14.Xenonids.Evolution;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoEvolutionSystem)})]
public sealed class XenoEvolutionComponent : 
  Component,
  ISerializationGenerated<XenoEvolutionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RequiresGranter = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanEvolveWithoutGranter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> EvolvesTo = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> EvolvesToWithoutPoints = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> Strains = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan EvolutionDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Points;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Max;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PointsPerSecond = (FixedPoint2) 0.5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 EarlyPointsPerSecond = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastPointsAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<InstantActionComponent> ActionId = (EntProtoId<InstantActionComponent>) "ActionXenoEvolve";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier EvolutionReadySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/xeno_evolveready.ogg", new AudioParams?(AudioParams.Default.WithVolume(-6f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool GotPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan EvolutionJitterDuration = TimeSpan.FromSeconds(10L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoEvolutionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoEvolutionComponent) target1;
    if (serialization.TryCustomCopy<XenoEvolutionComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequiresGranter, ref target2, hookCtx, false, context))
      target2 = this.RequiresGranter;
    target.RequiresGranter = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanEvolveWithoutGranter, ref target3, hookCtx, false, context))
      target3 = this.CanEvolveWithoutGranter;
    target.CanEvolveWithoutGranter = target3;
    List<EntProtoId> target4 = (List<EntProtoId>) null;
    if (this.EvolvesTo == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.EvolvesTo, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntProtoId>>(this.EvolvesTo, hookCtx, context);
    target.EvolvesTo = target4;
    List<EntProtoId> target5 = (List<EntProtoId>) null;
    if (this.EvolvesToWithoutPoints == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.EvolvesToWithoutPoints, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<EntProtoId>>(this.EvolvesToWithoutPoints, hookCtx, context);
    target.EvolvesToWithoutPoints = target5;
    List<EntProtoId> target6 = (List<EntProtoId>) null;
    if (this.Strains == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.Strains, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<EntProtoId>>(this.Strains, hookCtx, context);
    target.Strains = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EvolutionDelay, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.EvolutionDelay, hookCtx, context);
    target.EvolutionDelay = target7;
    FixedPoint2 target8 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Points, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<FixedPoint2>(this.Points, hookCtx, context);
    target.Points = target8;
    FixedPoint2 target9 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Max, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<FixedPoint2>(this.Max, hookCtx, context);
    target.Max = target9;
    FixedPoint2 target10 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PointsPerSecond, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<FixedPoint2>(this.PointsPerSecond, hookCtx, context);
    target.PointsPerSecond = target10;
    FixedPoint2 target11 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.EarlyPointsPerSecond, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<FixedPoint2>(this.EarlyPointsPerSecond, hookCtx, context);
    target.EarlyPointsPerSecond = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastPointsAt, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.LastPointsAt, hookCtx, context);
    target.LastPointsAt = target12;
    EntProtoId<InstantActionComponent> target13 = new EntProtoId<InstantActionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<InstantActionComponent>>(this.ActionId, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId<InstantActionComponent>>(this.ActionId, hookCtx, context);
    target.ActionId = target13;
    EntityUid? target14 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (this.EvolutionReadySound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EvolutionReadySound, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.EvolutionReadySound, hookCtx, context);
    target.EvolutionReadySound = target15;
    bool target16 = false;
    if (!serialization.TryCustomCopy<bool>(this.GotPopup, ref target16, hookCtx, false, context))
      target16 = this.GotPopup;
    target.GotPopup = target16;
    TimeSpan target17 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EvolutionJitterDuration, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<TimeSpan>(this.EvolutionJitterDuration, hookCtx, context);
    target.EvolutionJitterDuration = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoEvolutionComponent target,
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
    XenoEvolutionComponent target1 = (XenoEvolutionComponent) target;
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
    XenoEvolutionComponent target1 = (XenoEvolutionComponent) target;
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
    XenoEvolutionComponent target1 = (XenoEvolutionComponent) target;
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
  virtual XenoEvolutionComponent Component.Instantiate() => new XenoEvolutionComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoEvolutionComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoEvolutionComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoEvolutionComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoEvolutionComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastPointsAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoEvolutionComponent_AutoState : IComponentState
  {
    public bool RequiresGranter;
    public bool CanEvolveWithoutGranter;
    public 
    #nullable enable
    List<EntProtoId> EvolvesTo;
    public List<EntProtoId> EvolvesToWithoutPoints;
    public List<EntProtoId> Strains;
    public TimeSpan EvolutionDelay;
    public FixedPoint2 Points;
    public FixedPoint2 Max;
    public FixedPoint2 PointsPerSecond;
    public FixedPoint2 EarlyPointsPerSecond;
    public TimeSpan LastPointsAt;
    public EntProtoId<InstantActionComponent> ActionId;
    public NetEntity? Action;
    public SoundSpecifier EvolutionReadySound;
    public bool GotPopup;
    public TimeSpan EvolutionJitterDuration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoEvolutionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoEvolutionComponent, ComponentGetState>(new ComponentEventRefHandler<XenoEvolutionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoEvolutionComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoEvolutionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoEvolutionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoEvolutionComponent.XenoEvolutionComponent_AutoState()
      {
        RequiresGranter = component.RequiresGranter,
        CanEvolveWithoutGranter = component.CanEvolveWithoutGranter,
        EvolvesTo = component.EvolvesTo,
        EvolvesToWithoutPoints = component.EvolvesToWithoutPoints,
        Strains = component.Strains,
        EvolutionDelay = component.EvolutionDelay,
        Points = component.Points,
        Max = component.Max,
        PointsPerSecond = component.PointsPerSecond,
        EarlyPointsPerSecond = component.EarlyPointsPerSecond,
        LastPointsAt = component.LastPointsAt,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        EvolutionReadySound = component.EvolutionReadySound,
        GotPopup = component.GotPopup,
        EvolutionJitterDuration = component.EvolutionJitterDuration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoEvolutionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoEvolutionComponent.XenoEvolutionComponent_AutoState current))
        return;
      component.RequiresGranter = current.RequiresGranter;
      component.CanEvolveWithoutGranter = current.CanEvolveWithoutGranter;
      component.EvolvesTo = current.EvolvesTo == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.EvolvesTo);
      component.EvolvesToWithoutPoints = current.EvolvesToWithoutPoints == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.EvolvesToWithoutPoints);
      component.Strains = current.Strains == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.Strains);
      component.EvolutionDelay = current.EvolutionDelay;
      component.Points = current.Points;
      component.Max = current.Max;
      component.PointsPerSecond = current.PointsPerSecond;
      component.EarlyPointsPerSecond = current.EarlyPointsPerSecond;
      component.LastPointsAt = current.LastPointsAt;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<XenoEvolutionComponent>(current.Action, uid);
      component.EvolutionReadySound = current.EvolutionReadySound;
      component.GotPopup = current.GotPopup;
      component.EvolutionJitterDuration = current.EvolutionJitterDuration;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, XenoEvolutionComponent>(uid, component, ref args1);
    }
  }
}
