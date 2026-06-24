// Decompiled with JetBrains decompiler
// Type: Content.Client.Movement.Components.EyeCursorOffsetComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Movement.Components;

[RegisterComponent]
public sealed class EyeCursorOffsetComponent : 
  SharedEyeCursorOffsetComponent,
  ISerializationGenerated<EyeCursorOffsetComponent>,
  ISerializationGenerated
{
  public Vector2 TargetPosition = Vector2.Zero;
  public Vector2 CurrentPosition = Vector2.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EyeCursorOffsetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedEyeCursorOffsetComponent target1 = (SharedEyeCursorOffsetComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EyeCursorOffsetComponent) target1;
    serialization.TryCustomCopy<EyeCursorOffsetComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EyeCursorOffsetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SharedEyeCursorOffsetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EyeCursorOffsetComponent target1 = (EyeCursorOffsetComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SharedEyeCursorOffsetComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EyeCursorOffsetComponent target1 = (EyeCursorOffsetComponent) target;
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
    EyeCursorOffsetComponent target1 = (EyeCursorOffsetComponent) target;
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
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EyeCursorOffsetComponent SharedEyeCursorOffsetComponent.Instantiate()
  {
    return new EyeCursorOffsetComponent();
  }
}
