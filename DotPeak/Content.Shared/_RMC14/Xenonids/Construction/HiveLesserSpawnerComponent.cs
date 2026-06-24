// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.HiveLesserSpawnerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared._RMC14.Xenonids.Construction;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoConstructionSystem), typeof (SharedXenoPylonSystem)})]
public sealed class HiveLesserSpawnerComponent : 
  Component,
  ISerializationGenerated<HiveLesserSpawnerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int MinimumLesserDrones = 2;
  [DataField(null, false, 1, false, false, null)]
  public int XenosPerLesserDrone = 3;
  [DataField(null, false, 1, false, false, null)]
  public int CurrentLesserDrones;
  [DataField(null, false, 1, false, false, null)]
  public int MaxLesserDrones;
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> LiveLesserDrones = new List<EntityUid>();
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextLesserDroneAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NextLesserDroneOviCooldown = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NextLesserDroneCooldown = TimeSpan.FromSeconds(125L);
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 Heal = (FixedPoint2) 100;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan HealEvery = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan HealAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HiveLesserSpawnerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HiveLesserSpawnerComponent) target1;
    if (serialization.TryCustomCopy<HiveLesserSpawnerComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinimumLesserDrones, ref target2, hookCtx, false, context))
      target2 = this.MinimumLesserDrones;
    target.MinimumLesserDrones = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.XenosPerLesserDrone, ref target3, hookCtx, false, context))
      target3 = this.XenosPerLesserDrone;
    target.XenosPerLesserDrone = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.CurrentLesserDrones, ref target4, hookCtx, false, context))
      target4 = this.CurrentLesserDrones;
    target.CurrentLesserDrones = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxLesserDrones, ref target5, hookCtx, false, context))
      target5 = this.MaxLesserDrones;
    target.MaxLesserDrones = target5;
    List<EntityUid> target6 = (List<EntityUid>) null;
    if (this.LiveLesserDrones == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.LiveLesserDrones, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<EntityUid>>(this.LiveLesserDrones, hookCtx, context);
    target.LiveLesserDrones = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextLesserDroneAt, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.NextLesserDroneAt, hookCtx, context);
    target.NextLesserDroneAt = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextLesserDroneOviCooldown, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.NextLesserDroneOviCooldown, hookCtx, context);
    target.NextLesserDroneOviCooldown = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextLesserDroneCooldown, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.NextLesserDroneCooldown, hookCtx, context);
    target.NextLesserDroneCooldown = target9;
    FixedPoint2 target10 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Heal, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<FixedPoint2>(this.Heal, hookCtx, context);
    target.Heal = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HealEvery, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.HealEvery, hookCtx, context);
    target.HealEvery = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HealAt, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.HealAt, hookCtx, context);
    target.HealAt = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HiveLesserSpawnerComponent target,
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
    HiveLesserSpawnerComponent target1 = (HiveLesserSpawnerComponent) target;
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
    HiveLesserSpawnerComponent target1 = (HiveLesserSpawnerComponent) target;
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
    HiveLesserSpawnerComponent target1 = (HiveLesserSpawnerComponent) target;
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
  virtual HiveLesserSpawnerComponent Component.Instantiate() => new HiveLesserSpawnerComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HiveLesserSpawnerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HiveLesserSpawnerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<HiveLesserSpawnerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      HiveLesserSpawnerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextLesserDroneAt += args.PausedTime;
      component.HealAt += args.PausedTime;
    }
  }
}
