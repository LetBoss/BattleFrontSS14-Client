// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.Cartridges.NanoTaskPrintedComponent
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
namespace Content.Shared.CartridgeLoader.Cartridges;

[RegisterComponent]
public sealed class NanoTaskPrintedComponent : 
  Component,
  ISerializationGenerated<NanoTaskPrintedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public NanoTaskItem? Task;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NanoTaskPrintedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (NanoTaskPrintedComponent) component;
    if (serialization.TryCustomCopy<NanoTaskPrintedComponent>(this, ref target, hookCtx, false, context))
      return;
    NanoTaskItem nanoTaskItem = (NanoTaskItem) null;
    if (!serialization.TryCustomCopy<NanoTaskItem>(this.Task, ref nanoTaskItem, hookCtx, false, context))
      nanoTaskItem = serialization.CreateCopy<NanoTaskItem>(this.Task, hookCtx, context, false);
    target.Task = nanoTaskItem;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NanoTaskPrintedComponent target,
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
    NanoTaskPrintedComponent target1 = (NanoTaskPrintedComponent) target;
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
    NanoTaskPrintedComponent target1 = (NanoTaskPrintedComponent) target;
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
    NanoTaskPrintedComponent target1 = (NanoTaskPrintedComponent) target;
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
  virtual NanoTaskPrintedComponent Component.Instantiate() => new NanoTaskPrintedComponent();
}
