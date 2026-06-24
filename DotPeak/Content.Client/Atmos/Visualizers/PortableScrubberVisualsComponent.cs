// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Visualizers.PortableScrubberVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Atmos.Visualizers;

[RegisterComponent]
public sealed class PortableScrubberVisualsComponent : 
  Component,
  ISerializationGenerated<PortableScrubberVisualsComponent>,
  ISerializationGenerated
{
  [DataField("idleState", false, 1, true, false, null)]
  public string IdleState;
  [DataField("runningState", false, 1, true, false, null)]
  public string RunningState;
  [DataField("readyState", false, 1, true, false, null)]
  public string ReadyState;
  [DataField("fullState", false, 1, true, false, null)]
  public string FullState;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PortableScrubberVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PortableScrubberVisualsComponent) component;
    if (serialization.TryCustomCopy<PortableScrubberVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.IdleState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.IdleState, ref str1, hookCtx, false, context))
      str1 = this.IdleState;
    target.IdleState = str1;
    string str2 = (string) null;
    if (this.RunningState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RunningState, ref str2, hookCtx, false, context))
      str2 = this.RunningState;
    target.RunningState = str2;
    string str3 = (string) null;
    if (this.ReadyState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ReadyState, ref str3, hookCtx, false, context))
      str3 = this.ReadyState;
    target.ReadyState = str3;
    string str4 = (string) null;
    if (this.FullState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FullState, ref str4, hookCtx, false, context))
      str4 = this.FullState;
    target.FullState = str4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PortableScrubberVisualsComponent target,
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
    PortableScrubberVisualsComponent target1 = (PortableScrubberVisualsComponent) target;
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
    PortableScrubberVisualsComponent target1 = (PortableScrubberVisualsComponent) target;
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
    PortableScrubberVisualsComponent target1 = (PortableScrubberVisualsComponent) target;
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
  virtual PortableScrubberVisualsComponent Component.Instantiate()
  {
    return new PortableScrubberVisualsComponent();
  }
}
