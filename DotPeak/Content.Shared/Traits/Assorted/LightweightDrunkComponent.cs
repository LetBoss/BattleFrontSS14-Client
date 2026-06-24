// Decompiled with JetBrains decompiler
// Type: Content.Shared.Traits.Assorted.LightweightDrunkComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Drunk;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Traits.Assorted;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedDrunkSystem)})]
public sealed class LightweightDrunkComponent : 
  Component,
  ISerializationGenerated<LightweightDrunkComponent>,
  ISerializationGenerated
{
  [DataField("boozeStrengthMultiplier", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float BoozeStrengthMultiplier = 4f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LightweightDrunkComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LightweightDrunkComponent) target1;
    if (serialization.TryCustomCopy<LightweightDrunkComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BoozeStrengthMultiplier, ref target2, hookCtx, false, context))
      target2 = this.BoozeStrengthMultiplier;
    target.BoozeStrengthMultiplier = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LightweightDrunkComponent target,
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
    LightweightDrunkComponent target1 = (LightweightDrunkComponent) target;
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
    LightweightDrunkComponent target1 = (LightweightDrunkComponent) target;
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
    LightweightDrunkComponent target1 = (LightweightDrunkComponent) target;
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
  virtual LightweightDrunkComponent Component.Instantiate() => new LightweightDrunkComponent();
}
