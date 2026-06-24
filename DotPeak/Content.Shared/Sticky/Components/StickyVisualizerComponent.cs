// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sticky.Components.StickyVisualizerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Sticky.Components;

[RegisterComponent]
public sealed class StickyVisualizerComponent : 
  Component,
  ISerializationGenerated<StickyVisualizerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int StuckDrawDepth = 10;
  [DataField(null, false, 1, false, false, null)]
  public int OriginalDrawDepth;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StickyVisualizerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StickyVisualizerComponent) target1;
    if (serialization.TryCustomCopy<StickyVisualizerComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.StuckDrawDepth, ref target2, hookCtx, false, context))
      target2 = this.StuckDrawDepth;
    target.StuckDrawDepth = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.OriginalDrawDepth, ref target3, hookCtx, false, context))
      target3 = this.OriginalDrawDepth;
    target.OriginalDrawDepth = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StickyVisualizerComponent target,
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
    StickyVisualizerComponent target1 = (StickyVisualizerComponent) target;
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
    StickyVisualizerComponent target1 = (StickyVisualizerComponent) target;
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
    StickyVisualizerComponent target1 = (StickyVisualizerComponent) target;
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
  virtual StickyVisualizerComponent Component.Instantiate() => new StickyVisualizerComponent();
}
