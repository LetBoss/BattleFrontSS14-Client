// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.HoloTargeting.HoloTargetingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.HoloTargeting;

[RegisterComponent]
[Access(new Type[] {typeof (RMCHoloTargetingSystem)})]
public sealed class HoloTargetingComponent : 
  Component,
  ISerializationGenerated<HoloTargetingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Stacks = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxStacks = 100f;
  [DataField(null, false, 1, false, false, null)]
  public float Decay = 5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HoloTargetingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HoloTargetingComponent) target1;
    if (serialization.TryCustomCopy<HoloTargetingComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Stacks, ref target2, hookCtx, false, context))
      target2 = this.Stacks;
    target.Stacks = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxStacks, ref target3, hookCtx, false, context))
      target3 = this.MaxStacks;
    target.MaxStacks = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Decay, ref target4, hookCtx, false, context))
      target4 = this.Decay;
    target.Decay = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HoloTargetingComponent target,
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
    HoloTargetingComponent target1 = (HoloTargetingComponent) target;
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
    HoloTargetingComponent target1 = (HoloTargetingComponent) target;
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
    HoloTargetingComponent target1 = (HoloTargetingComponent) target;
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
  virtual HoloTargetingComponent Component.Instantiate() => new HoloTargetingComponent();
}
