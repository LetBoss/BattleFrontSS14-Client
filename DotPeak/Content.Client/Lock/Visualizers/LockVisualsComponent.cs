// Decompiled with JetBrains decompiler
// Type: Content.Client.Lock.Visualizers.LockVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Lock.Visualizers;

[RegisterComponent]
[Access(new Type[] {typeof (LockVisualizerSystem)})]
public sealed class LockVisualsComponent : 
  Component,
  ISerializationGenerated<LockVisualsComponent>,
  ISerializationGenerated
{
  [DataField("stateLocked", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? StateLocked = "locked";
  [DataField("stateUnlocked", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? StateUnlocked = "unlocked";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LockVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (LockVisualsComponent) component;
    if (serialization.TryCustomCopy<LockVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StateLocked, ref str1, hookCtx, false, context))
      str1 = this.StateLocked;
    target.StateLocked = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StateUnlocked, ref str2, hookCtx, false, context))
      str2 = this.StateUnlocked;
    target.StateUnlocked = str2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LockVisualsComponent target,
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
    LockVisualsComponent target1 = (LockVisualsComponent) target;
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
    LockVisualsComponent target1 = (LockVisualsComponent) target;
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
    LockVisualsComponent target1 = (LockVisualsComponent) target;
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
  virtual LockVisualsComponent Component.Instantiate() => new LockVisualsComponent();
}
