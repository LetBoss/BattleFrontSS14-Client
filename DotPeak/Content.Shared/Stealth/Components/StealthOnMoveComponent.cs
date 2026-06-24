// Decompiled with JetBrains decompiler
// Type: Content.Shared.Stealth.Components.StealthOnMoveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Stealth.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class StealthOnMoveComponent : 
  Component,
  ISerializationGenerated<StealthOnMoveComponent>,
  ISerializationGenerated
{
  [DataField("passiveVisibilityRate", false, 1, false, false, null)]
  public float PassiveVisibilityRate = -0.15f;
  [DataField("movementVisibilityRate", false, 1, false, false, null)]
  public float MovementVisibilityRate = 0.2f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StealthOnMoveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StealthOnMoveComponent) target1;
    if (serialization.TryCustomCopy<StealthOnMoveComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PassiveVisibilityRate, ref target2, hookCtx, false, context))
      target2 = this.PassiveVisibilityRate;
    target.PassiveVisibilityRate = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MovementVisibilityRate, ref target3, hookCtx, false, context))
      target3 = this.MovementVisibilityRate;
    target.MovementVisibilityRate = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StealthOnMoveComponent target,
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
    StealthOnMoveComponent target1 = (StealthOnMoveComponent) target;
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
    StealthOnMoveComponent target1 = (StealthOnMoveComponent) target;
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
    StealthOnMoveComponent target1 = (StealthOnMoveComponent) target;
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
  virtual StealthOnMoveComponent Component.Instantiate() => new StealthOnMoveComponent();
}
