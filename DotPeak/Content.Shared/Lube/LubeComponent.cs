// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lube.LubeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
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
namespace Content.Shared.Lube;

[RegisterComponent]
[NetworkedComponent]
public sealed class LubeComponent : 
  Component,
  ISerializationGenerated<LubeComponent>,
  ISerializationGenerated
{
  [DataField("squeeze", false, 1, false, false, null)]
  public SoundSpecifier Squeeze = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/squeezebottle.ogg");
  [DataField("solution", false, 1, false, false, null)]
  public string Solution = "drink";
  [DataField("reagent", false, 1, false, false, typeof (PrototypeIdSerializer<ReagentPrototype>))]
  public string Reagent = "SpaceLube";
  [DataField("consumption", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public FixedPoint2 Consumption = FixedPoint2.New(3);
  [DataField("minSlips", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int MinSlips = 1;
  [DataField("maxSlips", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int MaxSlips = 6;
  [DataField("slipStrength", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int SlipStrength = 10;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LubeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LubeComponent) target1;
    if (serialization.TryCustomCopy<LubeComponent>(this, ref target, hookCtx, false, context))
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
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Consumption, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.Consumption, hookCtx, context);
    target.Consumption = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinSlips, ref target6, hookCtx, false, context))
      target6 = this.MinSlips;
    target.MinSlips = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxSlips, ref target7, hookCtx, false, context))
      target7 = this.MaxSlips;
    target.MaxSlips = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.SlipStrength, ref target8, hookCtx, false, context))
      target8 = this.SlipStrength;
    target.SlipStrength = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LubeComponent target,
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
    LubeComponent target1 = (LubeComponent) target;
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
    LubeComponent target1 = (LubeComponent) target;
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
    LubeComponent target1 = (LubeComponent) target;
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
  virtual LubeComponent Component.Instantiate() => new LubeComponent();
}
