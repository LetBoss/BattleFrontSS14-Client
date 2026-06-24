// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.IntelTechTreeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Intel.Tech;
using Content.Shared.FixedPoint;
using Content.Shared.Radio;
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
namespace Content.Shared._RMC14.Intel;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (IntelSystem), typeof (TechSystem)})]
public sealed class IntelTechTreeComponent : 
  Component,
  ISerializationGenerated<IntelTechTreeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public IntelTechTree Tree = new IntelTechTree();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ColonyCommunicationsPoints = FixedPoint2.New(0.7);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PowerPoints = FixedPoint2.New(5);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastAnnounceAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 LastAnnouncePoints;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DoAnnouncements;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<RadioChannelPrototype>> AnnounceIn = new List<ProtoId<RadioChannelPrototype>>()
  {
    (ProtoId<RadioChannelPrototype>) "MarineCommand",
    (ProtoId<RadioChannelPrototype>) "MarineIntel"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HumanoidCorpses;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IntelTechTreeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IntelTechTreeComponent) target1;
    if (serialization.TryCustomCopy<IntelTechTreeComponent>(this, ref target, hookCtx, false, context))
      return;
    IntelTechTree target2 = (IntelTechTree) null;
    if (this.Tree == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IntelTechTree>(this.Tree, ref target2, hookCtx, false, context))
    {
      if (this.Tree == null)
        target2 = (IntelTechTree) null;
      else
        serialization.CopyTo<IntelTechTree>(this.Tree, ref target2, hookCtx, context, true);
    }
    target.Tree = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ColonyCommunicationsPoints, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.ColonyCommunicationsPoints, hookCtx, context);
    target.ColonyCommunicationsPoints = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PowerPoints, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.PowerPoints, hookCtx, context);
    target.PowerPoints = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastAnnounceAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.LastAnnounceAt, hookCtx, context);
    target.LastAnnounceAt = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.LastAnnouncePoints, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.LastAnnouncePoints, hookCtx, context);
    target.LastAnnouncePoints = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.DoAnnouncements, ref target7, hookCtx, false, context))
      target7 = this.DoAnnouncements;
    target.DoAnnouncements = target7;
    List<ProtoId<RadioChannelPrototype>> target8 = (List<ProtoId<RadioChannelPrototype>>) null;
    if (this.AnnounceIn == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<RadioChannelPrototype>>>(this.AnnounceIn, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<ProtoId<RadioChannelPrototype>>>(this.AnnounceIn, hookCtx, context);
    target.AnnounceIn = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.HumanoidCorpses, ref target9, hookCtx, false, context))
      target9 = this.HumanoidCorpses;
    target.HumanoidCorpses = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IntelTechTreeComponent target,
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
    IntelTechTreeComponent target1 = (IntelTechTreeComponent) target;
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
    IntelTechTreeComponent target1 = (IntelTechTreeComponent) target;
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
    IntelTechTreeComponent target1 = (IntelTechTreeComponent) target;
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
  virtual IntelTechTreeComponent Component.Instantiate() => new IntelTechTreeComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IntelTechTreeComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IntelTechTreeComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<IntelTechTreeComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      IntelTechTreeComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastAnnounceAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IntelTechTreeComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    IntelTechTree Tree;
    public FixedPoint2 ColonyCommunicationsPoints;
    public FixedPoint2 PowerPoints;
    public TimeSpan LastAnnounceAt;
    public FixedPoint2 LastAnnouncePoints;
    public bool DoAnnouncements;
    public List<ProtoId<RadioChannelPrototype>> AnnounceIn;
    public int HumanoidCorpses;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IntelTechTreeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IntelTechTreeComponent, ComponentGetState>(new ComponentEventRefHandler<IntelTechTreeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<IntelTechTreeComponent, ComponentHandleState>(new ComponentEventRefHandler<IntelTechTreeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      IntelTechTreeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new IntelTechTreeComponent.IntelTechTreeComponent_AutoState()
      {
        Tree = component.Tree,
        ColonyCommunicationsPoints = component.ColonyCommunicationsPoints,
        PowerPoints = component.PowerPoints,
        LastAnnounceAt = component.LastAnnounceAt,
        LastAnnouncePoints = component.LastAnnouncePoints,
        DoAnnouncements = component.DoAnnouncements,
        AnnounceIn = component.AnnounceIn,
        HumanoidCorpses = component.HumanoidCorpses
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IntelTechTreeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is IntelTechTreeComponent.IntelTechTreeComponent_AutoState current))
        return;
      component.Tree = current.Tree;
      component.ColonyCommunicationsPoints = current.ColonyCommunicationsPoints;
      component.PowerPoints = current.PowerPoints;
      component.LastAnnounceAt = current.LastAnnounceAt;
      component.LastAnnouncePoints = current.LastAnnouncePoints;
      component.DoAnnouncements = current.DoAnnouncements;
      component.AnnounceIn = current.AnnounceIn == null ? (List<ProtoId<RadioChannelPrototype>>) null : new List<ProtoId<RadioChannelPrototype>>((IEnumerable<ProtoId<RadioChannelPrototype>>) current.AnnounceIn);
      component.HumanoidCorpses = current.HumanoidCorpses;
    }
  }
}
