// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.Components.DrainComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Fluids.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedDrainSystem)})]
public sealed class DrainComponent : 
  Component,
  ISerializationGenerated<DrainComponent>,
  ISerializationGenerated
{
  public const string SolutionName = "drainBuffer";
  public static readonly ProtoId<TagPrototype> PlungerTag = (ProtoId<TagPrototype>) "Plunger";
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<SolutionComponent>? Solution;
  [DataField(null, false, 1, false, false, null)]
  public float Accumulator;
  [DataField(null, false, 1, false, false, null)]
  public bool AutoDrain = true;
  [DataField(null, false, 1, false, false, null)]
  public float UnitsPerSecond = 6f;
  [DataField(null, false, 1, false, false, null)]
  public float UnitsDestroyedPerSecond = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float Range = 2.5f;
  [DataField(null, false, 1, false, false, null)]
  public float DrainFrequency = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float UnclogDuration = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float UnclogProbability = 0.75f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier ManualDrainSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Fluids/slosh.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier PlungerSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Janitor/plunger.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier UnclogSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Fluids/glug.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DrainComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DrainComponent) target1;
    if (serialization.TryCustomCopy<DrainComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Accumulator, ref target2, hookCtx, false, context))
      target2 = this.Accumulator;
    target.Accumulator = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoDrain, ref target3, hookCtx, false, context))
      target3 = this.AutoDrain;
    target.AutoDrain = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UnitsPerSecond, ref target4, hookCtx, false, context))
      target4 = this.UnitsPerSecond;
    target.UnitsPerSecond = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UnitsDestroyedPerSecond, ref target5, hookCtx, false, context))
      target5 = this.UnitsDestroyedPerSecond;
    target.UnitsDestroyedPerSecond = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target6, hookCtx, false, context))
      target6 = this.Range;
    target.Range = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DrainFrequency, ref target7, hookCtx, false, context))
      target7 = this.DrainFrequency;
    target.DrainFrequency = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UnclogDuration, ref target8, hookCtx, false, context))
      target8 = this.UnclogDuration;
    target.UnclogDuration = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UnclogProbability, ref target9, hookCtx, false, context))
      target9 = this.UnclogProbability;
    target.UnclogProbability = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.ManualDrainSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ManualDrainSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.ManualDrainSound, hookCtx, context);
    target.ManualDrainSound = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (this.PlungerSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PlungerSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.PlungerSound, hookCtx, context);
    target.PlungerSound = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (this.UnclogSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnclogSound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.UnclogSound, hookCtx, context);
    target.UnclogSound = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DrainComponent target,
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
    DrainComponent target1 = (DrainComponent) target;
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
    DrainComponent target1 = (DrainComponent) target;
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
    DrainComponent target1 = (DrainComponent) target;
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
  virtual DrainComponent Component.Instantiate() => new DrainComponent();
}
