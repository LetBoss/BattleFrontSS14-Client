// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Pheromones.XenoPheromonesComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.HiveLeader;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Pheromones;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoPheromonesSystem), typeof (HiveLeaderSystem)})]
public sealed class XenoPheromonesComponent : 
  Component,
  ISerializationGenerated<XenoPheromonesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int PheromonesPlasmaCost = 35;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PheromonesPlasmaUpkeep = (FixedPoint2) 2.5;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextPheromonesPlasmaUse;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int PheromonesRange = 8;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PheromonesMultiplier = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? PheroSuffix;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoPheromonesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoPheromonesComponent) target1;
    if (serialization.TryCustomCopy<XenoPheromonesComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.PheromonesPlasmaCost, ref target2, hookCtx, false, context))
      target2 = this.PheromonesPlasmaCost;
    target.PheromonesPlasmaCost = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PheromonesPlasmaUpkeep, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.PheromonesPlasmaUpkeep, hookCtx, context);
    target.PheromonesPlasmaUpkeep = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextPheromonesPlasmaUse, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.NextPheromonesPlasmaUse, hookCtx, context);
    target.NextPheromonesPlasmaUse = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.PheromonesRange, ref target5, hookCtx, false, context))
      target5 = this.PheromonesRange;
    target.PheromonesRange = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PheromonesMultiplier, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.PheromonesMultiplier, hookCtx, context);
    target.PheromonesMultiplier = target6;
    string target7 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.PheroSuffix, ref target7, hookCtx, false, context))
      target7 = this.PheroSuffix;
    target.PheroSuffix = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoPheromonesComponent target,
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
    XenoPheromonesComponent target1 = (XenoPheromonesComponent) target;
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
    XenoPheromonesComponent target1 = (XenoPheromonesComponent) target;
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
    XenoPheromonesComponent target1 = (XenoPheromonesComponent) target;
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
  virtual XenoPheromonesComponent Component.Instantiate() => new XenoPheromonesComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoPheromonesComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoPheromonesComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoPheromonesComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoPheromonesComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextPheromonesPlasmaUse += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoPheromonesComponent_AutoState : IComponentState
  {
    public int PheromonesPlasmaCost;
    public FixedPoint2 PheromonesPlasmaUpkeep;
    public TimeSpan NextPheromonesPlasmaUse;
    public int PheromonesRange;
    public FixedPoint2 PheromonesMultiplier;
    public 
    #nullable enable
    string? PheroSuffix;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoPheromonesComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoPheromonesComponent, ComponentGetState>(new ComponentEventRefHandler<XenoPheromonesComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoPheromonesComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoPheromonesComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoPheromonesComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoPheromonesComponent.XenoPheromonesComponent_AutoState()
      {
        PheromonesPlasmaCost = component.PheromonesPlasmaCost,
        PheromonesPlasmaUpkeep = component.PheromonesPlasmaUpkeep,
        NextPheromonesPlasmaUse = component.NextPheromonesPlasmaUse,
        PheromonesRange = component.PheromonesRange,
        PheromonesMultiplier = component.PheromonesMultiplier,
        PheroSuffix = component.PheroSuffix
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoPheromonesComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoPheromonesComponent.XenoPheromonesComponent_AutoState current))
        return;
      component.PheromonesPlasmaCost = current.PheromonesPlasmaCost;
      component.PheromonesPlasmaUpkeep = current.PheromonesPlasmaUpkeep;
      component.NextPheromonesPlasmaUse = current.NextPheromonesPlasmaUse;
      component.PheromonesRange = current.PheromonesRange;
      component.PheromonesMultiplier = current.PheromonesMultiplier;
      component.PheroSuffix = current.PheroSuffix;
    }
  }
}
