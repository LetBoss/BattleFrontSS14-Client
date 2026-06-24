// Decompiled with JetBrains decompiler
// Type: Content.Client.Eye.LerpingEyeComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Eye;

[RegisterComponent]
public sealed class LerpingEyeComponent : 
  Component,
  ISerializationGenerated<LerpingEyeComponent>,
  ISerializationGenerated
{
  public bool ManuallyAdded;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Angle LastRotation;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Angle TargetRotation;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 LastZoom;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 TargetZoom;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LerpingEyeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (LerpingEyeComponent) component;
    serialization.TryCustomCopy<LerpingEyeComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LerpingEyeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LerpingEyeComponent target1 = (LerpingEyeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LerpingEyeComponent target1 = (LerpingEyeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LerpingEyeComponent target1 = (LerpingEyeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual LerpingEyeComponent Component.Instantiate() => new LerpingEyeComponent();
}
