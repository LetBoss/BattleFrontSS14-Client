// Decompiled with JetBrains decompiler
// Type: Content.Shared.Rotatable.RotatableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Rotatable;

[RegisterComponent]
public sealed class RotatableComponent : 
  Component,
  ISerializationGenerated<RotatableComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("rotateWhileAnchored", false, 1, false, false, null)]
  public bool RotateWhileAnchored { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("rotateWhilePulling", false, 1, false, false, null)]
  public bool RotateWhilePulling { get; private set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("increment", false, 1, false, false, null)]
  public Angle Increment { get; private set; } = Angle.FromDegrees(90.0);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RotatableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RotatableComponent) target1;
    if (serialization.TryCustomCopy<RotatableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.RotateWhileAnchored, ref target2, hookCtx, false, context))
      target2 = this.RotateWhileAnchored;
    target.RotateWhileAnchored = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.RotateWhilePulling, ref target3, hookCtx, false, context))
      target3 = this.RotateWhilePulling;
    target.RotateWhilePulling = target3;
    Angle target4 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.Increment, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Angle>(this.Increment, hookCtx, context);
    target.Increment = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RotatableComponent target,
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
    RotatableComponent target1 = (RotatableComponent) target;
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
    RotatableComponent target1 = (RotatableComponent) target;
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
    RotatableComponent target1 = (RotatableComponent) target;
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
  virtual RotatableComponent Component.Instantiate() => new RotatableComponent();
}
