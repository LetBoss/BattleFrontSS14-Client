// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.Components.PuddleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Fluids.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedPuddleSystem)})]
public sealed class PuddleComponent : 
  Component,
  ISerializationGenerated<PuddleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SpillSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg");
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 OverflowVolume = FixedPoint2.New(10000);
  [DataField("solution", false, 1, false, false, null)]
  public string SolutionName = "puddle";
  [DataField(null, false, 1, false, false, null)]
  public float DefaultSlippery = 5.5f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<SolutionComponent>? Solution;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PuddleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PuddleComponent) target1;
    if (serialization.TryCustomCopy<PuddleComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.SpillSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SpillSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.SpillSound, hookCtx, context);
    target.SpillSound = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.OverflowVolume, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.OverflowVolume, hookCtx, context);
    target.OverflowVolume = target3;
    string target4 = (string) null;
    if (this.SolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionName, ref target4, hookCtx, false, context))
      target4 = this.SolutionName;
    target.SolutionName = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DefaultSlippery, ref target5, hookCtx, false, context))
      target5 = this.DefaultSlippery;
    target.DefaultSlippery = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PuddleComponent target,
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
    PuddleComponent target1 = (PuddleComponent) target;
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
    PuddleComponent target1 = (PuddleComponent) target;
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
    PuddleComponent target1 = (PuddleComponent) target;
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
  virtual PuddleComponent Component.Instantiate() => new PuddleComponent();
}
