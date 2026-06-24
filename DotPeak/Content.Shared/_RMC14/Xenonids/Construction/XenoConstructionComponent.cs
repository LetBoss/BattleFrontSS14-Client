// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.XenoConstructionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Construction.ResinWhisper;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedXenoConstructionSystem), typeof (ResinWhispererSystem)})]
public sealed class XenoConstructionComponent : 
  Component,
  ISerializationGenerated<XenoConstructionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BuildRange;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> CanBuild;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? BuildChoice;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BuildDelay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 OrderConstructionRange;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> CanOrderConstruction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? OrderConstructionChoice;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OrderConstructionTargeting;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ConfirmOrderConstructionAction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates? OrderingConstructionAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan OrderConstructionDelay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier BuildSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanUpgrade;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DensityConstructionCostModifier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DensityConstructionCostMultiplier;

  public XenoConstructionComponent()
  {
    SoundCollectionSpecifier collectionSpecifier = new SoundCollectionSpecifier("RMCResinBuild");
    collectionSpecifier.Params = AudioParams.Default.WithVolume(-10f);
    this.BuildSound = (SoundSpecifier) collectionSpecifier;
    this.DensityConstructionCostModifier = 0.35f;
    this.DensityConstructionCostMultiplier = 2f;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoConstructionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoConstructionComponent) target1;
    if (serialization.TryCustomCopy<XenoConstructionComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BuildRange, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.BuildRange, hookCtx, context);
    target.BuildRange = target2;
    List<EntProtoId> target3 = (List<EntProtoId>) null;
    if (this.CanBuild == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.CanBuild, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<EntProtoId>>(this.CanBuild, hookCtx, context);
    target.CanBuild = target3;
    EntProtoId? target4 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.BuildChoice, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId?>(this.BuildChoice, hookCtx, context);
    target.BuildChoice = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BuildDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.BuildDelay, hookCtx, context);
    target.BuildDelay = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.OrderConstructionRange, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.OrderConstructionRange, hookCtx, context);
    target.OrderConstructionRange = target6;
    List<EntProtoId> target7 = (List<EntProtoId>) null;
    if (this.CanOrderConstruction == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.CanOrderConstruction, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<EntProtoId>>(this.CanOrderConstruction, hookCtx, context);
    target.CanOrderConstruction = target7;
    EntProtoId? target8 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.OrderConstructionChoice, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId?>(this.OrderConstructionChoice, hookCtx, context);
    target.OrderConstructionChoice = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.OrderConstructionTargeting, ref target9, hookCtx, false, context))
      target9 = this.OrderConstructionTargeting;
    target.OrderConstructionTargeting = target9;
    EntityUid? target10 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ConfirmOrderConstructionAction, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntityUid?>(this.ConfirmOrderConstructionAction, hookCtx, context);
    target.ConfirmOrderConstructionAction = target10;
    EntityCoordinates? target11 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.OrderingConstructionAt, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntityCoordinates?>(this.OrderingConstructionAt, hookCtx, context);
    target.OrderingConstructionAt = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OrderConstructionDelay, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.OrderConstructionDelay, hookCtx, context);
    target.OrderConstructionDelay = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (this.BuildSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BuildSound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.BuildSound, hookCtx, context);
    target.BuildSound = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanUpgrade, ref target14, hookCtx, false, context))
      target14 = this.CanUpgrade;
    target.CanUpgrade = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DensityConstructionCostModifier, ref target15, hookCtx, false, context))
      target15 = this.DensityConstructionCostModifier;
    target.DensityConstructionCostModifier = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DensityConstructionCostMultiplier, ref target16, hookCtx, false, context))
      target16 = this.DensityConstructionCostMultiplier;
    target.DensityConstructionCostMultiplier = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoConstructionComponent target,
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
    XenoConstructionComponent target1 = (XenoConstructionComponent) target;
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
    XenoConstructionComponent target1 = (XenoConstructionComponent) target;
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
    XenoConstructionComponent target1 = (XenoConstructionComponent) target;
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
  virtual XenoConstructionComponent Component.Instantiate() => new XenoConstructionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoConstructionComponent_AutoState : IComponentState
  {
    public FixedPoint2 BuildRange;
    public List<EntProtoId> CanBuild;
    public EntProtoId? BuildChoice;
    public TimeSpan BuildDelay;
    public FixedPoint2 OrderConstructionRange;
    public List<EntProtoId> CanOrderConstruction;
    public EntProtoId? OrderConstructionChoice;
    public bool OrderConstructionTargeting;
    public NetEntity? ConfirmOrderConstructionAction;
    public NetCoordinates? OrderingConstructionAt;
    public TimeSpan OrderConstructionDelay;
    public SoundSpecifier BuildSound;
    public bool CanUpgrade;
    public float DensityConstructionCostModifier;
    public float DensityConstructionCostMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoConstructionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoConstructionComponent, ComponentGetState>(new ComponentEventRefHandler<XenoConstructionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoConstructionComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoConstructionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoConstructionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoConstructionComponent.XenoConstructionComponent_AutoState()
      {
        BuildRange = component.BuildRange,
        CanBuild = component.CanBuild,
        BuildChoice = component.BuildChoice,
        BuildDelay = component.BuildDelay,
        OrderConstructionRange = component.OrderConstructionRange,
        CanOrderConstruction = component.CanOrderConstruction,
        OrderConstructionChoice = component.OrderConstructionChoice,
        OrderConstructionTargeting = component.OrderConstructionTargeting,
        ConfirmOrderConstructionAction = this.GetNetEntity(component.ConfirmOrderConstructionAction),
        OrderingConstructionAt = this.GetNetCoordinates(component.OrderingConstructionAt),
        OrderConstructionDelay = component.OrderConstructionDelay,
        BuildSound = component.BuildSound,
        CanUpgrade = component.CanUpgrade,
        DensityConstructionCostModifier = component.DensityConstructionCostModifier,
        DensityConstructionCostMultiplier = component.DensityConstructionCostMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoConstructionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoConstructionComponent.XenoConstructionComponent_AutoState current))
        return;
      component.BuildRange = current.BuildRange;
      component.CanBuild = current.CanBuild == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.CanBuild);
      component.BuildChoice = current.BuildChoice;
      component.BuildDelay = current.BuildDelay;
      component.OrderConstructionRange = current.OrderConstructionRange;
      component.CanOrderConstruction = current.CanOrderConstruction == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.CanOrderConstruction);
      component.OrderConstructionChoice = current.OrderConstructionChoice;
      component.OrderConstructionTargeting = current.OrderConstructionTargeting;
      component.ConfirmOrderConstructionAction = this.EnsureEntity<XenoConstructionComponent>(current.ConfirmOrderConstructionAction, uid);
      component.OrderingConstructionAt = this.EnsureCoordinates<XenoConstructionComponent>(current.OrderingConstructionAt, uid);
      component.OrderConstructionDelay = current.OrderConstructionDelay;
      component.BuildSound = current.BuildSound;
      component.CanUpgrade = current.CanUpgrade;
      component.DensityConstructionCostModifier = current.DensityConstructionCostModifier;
      component.DensityConstructionCostMultiplier = current.DensityConstructionCostMultiplier;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, XenoConstructionComponent>(uid, component, ref args1);
    }
  }
}
