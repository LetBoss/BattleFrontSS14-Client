// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.SharedEyeCursorOffsetComponent
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

[ComponentProtoName("EyeCursorOffset")]
[NetworkedComponent]
public abstract class SharedEyeCursorOffsetComponent : 
  Component,
  ISerializationGenerated<SharedEyeCursorOffsetComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float MaxOffset = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float OffsetSpeed = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public float PvsIncrease = 0.3f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SharedEyeCursorOffsetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SharedEyeCursorOffsetComponent) target1;
    if (serialization.TryCustomCopy<SharedEyeCursorOffsetComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxOffset, ref target2, hookCtx, false, context))
      target2 = this.MaxOffset;
    target.MaxOffset = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OffsetSpeed, ref target3, hookCtx, false, context))
      target3 = this.OffsetSpeed;
    target.OffsetSpeed = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PvsIncrease, ref target4, hookCtx, false, context))
      target4 = this.PvsIncrease;
    target.PvsIncrease = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SharedEyeCursorOffsetComponent target,
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
    SharedEyeCursorOffsetComponent target1 = (SharedEyeCursorOffsetComponent) target;
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
    SharedEyeCursorOffsetComponent target1 = (SharedEyeCursorOffsetComponent) target;
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
    SharedEyeCursorOffsetComponent target1 = (SharedEyeCursorOffsetComponent) target;
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
  virtual SharedEyeCursorOffsetComponent Component.Instantiate()
  {
    throw new NotImplementedException();
  }
}
