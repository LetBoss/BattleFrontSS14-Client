// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.Generator.GeneratorExhaustGasComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Power.Generator;

[RegisterComponent]
public sealed class GeneratorExhaustGasComponent : 
  Component,
  ISerializationGenerated<GeneratorExhaustGasComponent>,
  ISerializationGenerated
{
  [DataField("gasType", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Gas GasType = Gas.CarbonDioxide;
  [DataField("moleRatio", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float MoleRatio = 1f;
  [DataField("temperature", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float Temperature = 373.15f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GeneratorExhaustGasComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GeneratorExhaustGasComponent) target1;
    if (serialization.TryCustomCopy<GeneratorExhaustGasComponent>(this, ref target, hookCtx, false, context))
      return;
    Gas target2 = Gas.Oxygen;
    if (!serialization.TryCustomCopy<Gas>(this.GasType, ref target2, hookCtx, false, context))
      target2 = this.GasType;
    target.GasType = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MoleRatio, ref target3, hookCtx, false, context))
      target3 = this.MoleRatio;
    target.MoleRatio = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Temperature, ref target4, hookCtx, false, context))
      target4 = this.Temperature;
    target.Temperature = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GeneratorExhaustGasComponent target,
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
    GeneratorExhaustGasComponent target1 = (GeneratorExhaustGasComponent) target;
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
    GeneratorExhaustGasComponent target1 = (GeneratorExhaustGasComponent) target;
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
    GeneratorExhaustGasComponent target1 = (GeneratorExhaustGasComponent) target;
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
  virtual GeneratorExhaustGasComponent Component.Instantiate()
  {
    return new GeneratorExhaustGasComponent();
  }
}
