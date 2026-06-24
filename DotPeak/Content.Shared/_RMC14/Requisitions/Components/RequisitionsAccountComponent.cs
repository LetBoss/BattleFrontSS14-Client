// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Requisitions.Components.RequisitionsAccountComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Scaling;
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
namespace Content.Shared._RMC14.Requisitions.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRequisitionsSystem)})]
public sealed class RequisitionsAccountComponent : 
  Component,
  ISerializationGenerated<RequisitionsAccountComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedRequisitionsSystem), typeof (ScalingSystem)})]
  public bool Started;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedRequisitionsSystem), typeof (ScalingSystem)})]
  public int Balance;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextGain;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan GainEvery = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  public List<RequisitionsRandomCrates> RandomCrates = new List<RequisitionsRandomCrates>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RequisitionsAccountComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RequisitionsAccountComponent) target1;
    if (serialization.TryCustomCopy<RequisitionsAccountComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Started, ref target2, hookCtx, false, context))
      target2 = this.Started;
    target.Started = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Balance, ref target3, hookCtx, false, context))
      target3 = this.Balance;
    target.Balance = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextGain, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.NextGain, hookCtx, context);
    target.NextGain = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.GainEvery, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.GainEvery, hookCtx, context);
    target.GainEvery = target5;
    List<RequisitionsRandomCrates> target6 = (List<RequisitionsRandomCrates>) null;
    if (this.RandomCrates == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<RequisitionsRandomCrates>>(this.RandomCrates, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<RequisitionsRandomCrates>>(this.RandomCrates, hookCtx, context);
    target.RandomCrates = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RequisitionsAccountComponent target,
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
    RequisitionsAccountComponent target1 = (RequisitionsAccountComponent) target;
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
    RequisitionsAccountComponent target1 = (RequisitionsAccountComponent) target;
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
    RequisitionsAccountComponent target1 = (RequisitionsAccountComponent) target;
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
  virtual RequisitionsAccountComponent Component.Instantiate()
  {
    return new RequisitionsAccountComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RequisitionsAccountComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RequisitionsAccountComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RequisitionsAccountComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RequisitionsAccountComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextGain += args.PausedTime;
    }
  }
}
