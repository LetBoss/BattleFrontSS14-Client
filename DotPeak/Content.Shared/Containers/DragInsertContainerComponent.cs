// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.DragInsertContainerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Containers;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (DragInsertContainerSystem)})]
public sealed class DragInsertContainerComponent : 
  Component,
  ISerializationGenerated<DragInsertContainerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string ContainerId;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool UseVerbs = true;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan EntryDelay = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public bool DelaySelfEntry;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DragInsertContainerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DragInsertContainerComponent) component;
    if (serialization.TryCustomCopy<DragInsertContainerComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref str, hookCtx, false, context))
      str = this.ContainerId;
    target.ContainerId = str;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseVerbs, ref flag1, hookCtx, false, context))
      flag1 = this.UseVerbs;
    target.UseVerbs = flag1;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EntryDelay, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.EntryDelay, hookCtx, context, false);
    target.EntryDelay = timeSpan;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.DelaySelfEntry, ref flag2, hookCtx, false, context))
      flag2 = this.DelaySelfEntry;
    target.DelaySelfEntry = flag2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DragInsertContainerComponent target,
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
    DragInsertContainerComponent target1 = (DragInsertContainerComponent) target;
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
    DragInsertContainerComponent target1 = (DragInsertContainerComponent) target;
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
    DragInsertContainerComponent target1 = (DragInsertContainerComponent) target;
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
  virtual DragInsertContainerComponent Component.Instantiate()
  {
    return new DragInsertContainerComponent();
  }
}
