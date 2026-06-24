// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Components.StorageFillVisualizerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Storage.Components;

[RegisterComponent]
public sealed class StorageFillVisualizerComponent : 
  Component,
  ISerializationGenerated<StorageFillVisualizerComponent>,
  ISerializationGenerated
{
  [DataField("maxFillLevels", false, 1, true, false, null)]
  public int MaxFillLevels;
  [DataField("fillBaseName", false, 1, true, false, null)]
  public string FillBaseName;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StorageFillVisualizerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StorageFillVisualizerComponent) target1;
    if (serialization.TryCustomCopy<StorageFillVisualizerComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxFillLevels, ref target2, hookCtx, false, context))
      target2 = this.MaxFillLevels;
    target.MaxFillLevels = target2;
    string target3 = (string) null;
    if (this.FillBaseName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FillBaseName, ref target3, hookCtx, false, context))
      target3 = this.FillBaseName;
    target.FillBaseName = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StorageFillVisualizerComponent target,
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
    StorageFillVisualizerComponent target1 = (StorageFillVisualizerComponent) target;
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
    StorageFillVisualizerComponent target1 = (StorageFillVisualizerComponent) target;
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
    StorageFillVisualizerComponent target1 = (StorageFillVisualizerComponent) target;
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
  virtual StorageFillVisualizerComponent Component.Instantiate()
  {
    return new StorageFillVisualizerComponent();
  }
}
