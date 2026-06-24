// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Storage.CMStorageVisualizerComponent
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
namespace Content.Shared._RMC14.Storage;

[RegisterComponent]
public sealed class CMStorageVisualizerComponent : 
  Component,
  ISerializationGenerated<CMStorageVisualizerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string? StorageClosed;
  [DataField(null, false, 1, false, false, null)]
  public string? StorageOpen;
  [DataField(null, false, 1, false, false, null)]
  public string? StorageEmpty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMStorageVisualizerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMStorageVisualizerComponent) target1;
    if (serialization.TryCustomCopy<CMStorageVisualizerComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StorageClosed, ref target2, hookCtx, false, context))
      target2 = this.StorageClosed;
    target.StorageClosed = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StorageOpen, ref target3, hookCtx, false, context))
      target3 = this.StorageOpen;
    target.StorageOpen = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StorageEmpty, ref target4, hookCtx, false, context))
      target4 = this.StorageEmpty;
    target.StorageEmpty = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMStorageVisualizerComponent target,
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
    CMStorageVisualizerComponent target1 = (CMStorageVisualizerComponent) target;
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
    CMStorageVisualizerComponent target1 = (CMStorageVisualizerComponent) target;
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
    CMStorageVisualizerComponent target1 = (CMStorageVisualizerComponent) target;
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
  virtual CMStorageVisualizerComponent Component.Instantiate()
  {
    return new CMStorageVisualizerComponent();
  }
}
