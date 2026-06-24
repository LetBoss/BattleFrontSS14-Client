// Decompiled with JetBrains decompiler
// Type: Content.Shared.Timing.UseDelayComponent
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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Timing;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (UseDelaySystem)})]
public sealed class UseDelayComponent : 
  Component,
  ISerializationGenerated<UseDelayComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, UseDelayInfo> Delays = new Dictionary<string, UseDelayInfo>();
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Delay = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UseDelayComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UseDelayComponent) target1;
    if (serialization.TryCustomCopy<UseDelayComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, UseDelayInfo> target2 = (Dictionary<string, UseDelayInfo>) null;
    if (this.Delays == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, UseDelayInfo>>(this.Delays, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, UseDelayInfo>>(this.Delays, hookCtx, context);
    target.Delays = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UseDelayComponent target,
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
    UseDelayComponent target1 = (UseDelayComponent) target;
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
    UseDelayComponent target1 = (UseDelayComponent) target;
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
    UseDelayComponent target1 = (UseDelayComponent) target;
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
  virtual UseDelayComponent Component.Instantiate() => new UseDelayComponent();
}
