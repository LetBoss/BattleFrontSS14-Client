// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.Components.SpillableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Fluids.Components;

[RegisterComponent]
public sealed class SpillableComponent : 
  Component,
  ISerializationGenerated<SpillableComponent>,
  ISerializationGenerated
{
  [DataField("solution", false, 1, false, false, null)]
  public string SolutionName = "puddle";
  [DataField(null, false, 1, false, false, null)]
  public float? SpillDelay;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 MaxMeleeSpillAmount = FixedPoint2.New(20);
  [DataField(null, false, 1, false, false, null)]
  public bool SpillWhenThrown = true;
  [DataField(null, false, 1, false, false, null)]
  public bool PreventMelee = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpillableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpillableComponent) target1;
    if (serialization.TryCustomCopy<SpillableComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.SolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionName, ref target2, hookCtx, false, context))
      target2 = this.SolutionName;
    target.SolutionName = target2;
    float? target3 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.SpillDelay, ref target3, hookCtx, false, context))
      target3 = this.SpillDelay;
    target.SpillDelay = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxMeleeSpillAmount, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.MaxMeleeSpillAmount, hookCtx, context);
    target.MaxMeleeSpillAmount = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.SpillWhenThrown, ref target5, hookCtx, false, context))
      target5 = this.SpillWhenThrown;
    target.SpillWhenThrown = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.PreventMelee, ref target6, hookCtx, false, context))
      target6 = this.PreventMelee;
    target.PreventMelee = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpillableComponent target,
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
    SpillableComponent target1 = (SpillableComponent) target;
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
    SpillableComponent target1 = (SpillableComponent) target;
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
    SpillableComponent target1 = (SpillableComponent) target;
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
  virtual SpillableComponent Component.Instantiate() => new SpillableComponent();
}
