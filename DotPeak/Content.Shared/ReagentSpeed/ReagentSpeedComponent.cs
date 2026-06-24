// Decompiled with JetBrains decompiler
// Type: Content.Shared.ReagentSpeed.ReagentSpeedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.ReagentSpeed;

[RegisterComponent]
[Access(new Type[] {typeof (ReagentSpeedSystem)})]
public sealed class ReagentSpeedComponent : 
  Component,
  ISerializationGenerated<ReagentSpeedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Solution = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 Cost = (FixedPoint2) 5;
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<ProtoId<ReagentPrototype>, float> Modifiers = new Dictionary<ProtoId<ReagentPrototype>, float>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReagentSpeedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ReagentSpeedComponent) target1;
    if (serialization.TryCustomCopy<ReagentSpeedComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target2, hookCtx, false, context))
      target2 = this.Solution;
    target.Solution = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Cost, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.Cost, hookCtx, context);
    target.Cost = target3;
    Dictionary<ProtoId<ReagentPrototype>, float> target4 = (Dictionary<ProtoId<ReagentPrototype>, float>) null;
    if (this.Modifiers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<ReagentPrototype>, float>>(this.Modifiers, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<ProtoId<ReagentPrototype>, float>>(this.Modifiers, hookCtx, context);
    target.Modifiers = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReagentSpeedComponent target,
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
    ReagentSpeedComponent target1 = (ReagentSpeedComponent) target;
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
    ReagentSpeedComponent target1 = (ReagentSpeedComponent) target;
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
    ReagentSpeedComponent target1 = (ReagentSpeedComponent) target;
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
  virtual ReagentSpeedComponent Component.Instantiate() => new ReagentSpeedComponent();
}
