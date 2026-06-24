// Decompiled with JetBrains decompiler
// Type: Content.Shared.Stacks.StackLayerThresholdComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Stacks;

[RegisterComponent]
[NetworkedComponent]
public sealed class StackLayerThresholdComponent : 
  Component,
  ISerializationGenerated<StackLayerThresholdComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public List<int> Thresholds = new List<int>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StackLayerThresholdComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StackLayerThresholdComponent) target1;
    if (serialization.TryCustomCopy<StackLayerThresholdComponent>(this, ref target, hookCtx, false, context))
      return;
    List<int> target2 = (List<int>) null;
    if (this.Thresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<int>>(this.Thresholds, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<int>>(this.Thresholds, hookCtx, context);
    target.Thresholds = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StackLayerThresholdComponent target,
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
    StackLayerThresholdComponent target1 = (StackLayerThresholdComponent) target;
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
    StackLayerThresholdComponent target1 = (StackLayerThresholdComponent) target;
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
    StackLayerThresholdComponent target1 = (StackLayerThresholdComponent) target;
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
  virtual StackLayerThresholdComponent Component.Instantiate()
  {
    return new StackLayerThresholdComponent();
  }
}
