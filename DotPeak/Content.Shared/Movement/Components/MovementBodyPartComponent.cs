// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.MovementBodyPartComponent
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
namespace Content.Shared.Movement.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class MovementBodyPartComponent : 
  Component,
  ISerializationGenerated<MovementBodyPartComponent>,
  ISerializationGenerated
{
  [DataField("walkSpeed", false, 1, false, false, null)]
  public float WalkSpeed = 2.5f;
  [DataField("sprintSpeed", false, 1, false, false, null)]
  public float SprintSpeed = 4.5f;
  [DataField("acceleration", false, 1, false, false, null)]
  public float Acceleration = 20f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MovementBodyPartComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MovementBodyPartComponent) target1;
    if (serialization.TryCustomCopy<MovementBodyPartComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WalkSpeed, ref target2, hookCtx, false, context))
      target2 = this.WalkSpeed;
    target.WalkSpeed = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprintSpeed, ref target3, hookCtx, false, context))
      target3 = this.SprintSpeed;
    target.SprintSpeed = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Acceleration, ref target4, hookCtx, false, context))
      target4 = this.Acceleration;
    target.Acceleration = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MovementBodyPartComponent target,
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
    MovementBodyPartComponent target1 = (MovementBodyPartComponent) target;
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
    MovementBodyPartComponent target1 = (MovementBodyPartComponent) target;
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
    MovementBodyPartComponent target1 = (MovementBodyPartComponent) target;
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
  virtual MovementBodyPartComponent Component.Instantiate() => new MovementBodyPartComponent();
}
