// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Medicine.PubgHealOverTimeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Medicine;

[RegisterComponent]
public sealed class PubgHealOverTimeComponent : 
  Component,
  ISerializationGenerated<PubgHealOverTimeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 HealPerSecond;
  [DataField(null, false, 1, false, false, null)]
  public float SecondsRemaining;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NextHealTime;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Source;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgHealOverTimeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgHealOverTimeComponent) target1;
    if (serialization.TryCustomCopy<PubgHealOverTimeComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.HealPerSecond, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.HealPerSecond, hookCtx, context);
    target.HealPerSecond = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SecondsRemaining, ref target3, hookCtx, false, context))
      target3 = this.SecondsRemaining;
    target.SecondsRemaining = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextHealTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.NextHealTime, hookCtx, context);
    target.NextHealTime = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Source, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Source, hookCtx, context);
    target.Source = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgHealOverTimeComponent target,
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
    PubgHealOverTimeComponent target1 = (PubgHealOverTimeComponent) target;
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
    PubgHealOverTimeComponent target1 = (PubgHealOverTimeComponent) target;
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
    PubgHealOverTimeComponent target1 = (PubgHealOverTimeComponent) target;
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
  virtual PubgHealOverTimeComponent Component.Instantiate() => new PubgHealOverTimeComponent();
}
