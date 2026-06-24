// Decompiled with JetBrains decompiler
// Type: Content.Shared.Glue.GlueComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Glue;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedGlueSystem)})]
public sealed class GlueComponent : 
  Component,
  ISerializationGenerated<GlueComponent>,
  ISerializationGenerated
{
  [DataField("squeeze", false, 1, false, false, null)]
  public SoundSpecifier Squeeze = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/squeezebottle.ogg");
  [DataField("solution", false, 1, false, false, null)]
  public string Solution = "drink";
  [DataField("reagent", false, 1, false, false, typeof (PrototypeIdSerializer<ReagentPrototype>))]
  public string Reagent = "SpaceGlue";
  [DataField("consumptionUnit", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public FixedPoint2 ConsumptionUnit = FixedPoint2.New(5);
  [DataField("durationPerUnit", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan DurationPerUnit = TimeSpan.FromSeconds(6L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GlueComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GlueComponent) target1;
    if (serialization.TryCustomCopy<GlueComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.Squeeze == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Squeeze, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.Squeeze, hookCtx, context);
    target.Squeeze = target2;
    string target3 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target3, hookCtx, false, context))
      target3 = this.Solution;
    target.Solution = target3;
    string target4 = (string) null;
    if (this.Reagent == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Reagent, ref target4, hookCtx, false, context))
      target4 = this.Reagent;
    target.Reagent = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ConsumptionUnit, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.ConsumptionUnit, hookCtx, context);
    target.ConsumptionUnit = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DurationPerUnit, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.DurationPerUnit, hookCtx, context);
    target.DurationPerUnit = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GlueComponent target,
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
    GlueComponent target1 = (GlueComponent) target;
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
    GlueComponent target1 = (GlueComponent) target;
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
    GlueComponent target1 = (GlueComponent) target;
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
  virtual GlueComponent Component.Instantiate() => new GlueComponent();
}
