// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Overwatch.OverwatchDataComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Overwatch;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedOverwatchConsoleSystem)})]
public sealed class OverwatchDataComponent : 
  Component,
  ISerializationGenerated<OverwatchDataComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public OverwatchMarine? Marine;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OverwatchDataComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OverwatchDataComponent) target1;
    if (serialization.TryCustomCopy<OverwatchDataComponent>(this, ref target, hookCtx, false, context))
      return;
    OverwatchMarine? target2 = new OverwatchMarine?();
    if (!serialization.TryCustomCopy<OverwatchMarine?>(this.Marine, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<OverwatchMarine?>(this.Marine, hookCtx, context);
    target.Marine = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OverwatchDataComponent target,
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
    OverwatchDataComponent target1 = (OverwatchDataComponent) target;
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
    OverwatchDataComponent target1 = (OverwatchDataComponent) target;
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
    OverwatchDataComponent target1 = (OverwatchDataComponent) target;
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
  virtual OverwatchDataComponent Component.Instantiate() => new OverwatchDataComponent();
}
