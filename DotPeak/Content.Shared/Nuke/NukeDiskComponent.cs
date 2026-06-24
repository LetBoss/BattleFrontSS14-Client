// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nuke.NukeDiskComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nuke;

[RegisterComponent]
[NetworkedComponent]
public sealed class NukeDiskComponent : 
  Component,
  ISerializationGenerated<NukeDiskComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? TimeModifier;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MicrowaveMean = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MicrowaveStd = TimeSpan.FromSeconds(27.35);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NukeDiskComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NukeDiskComponent) target1;
    if (serialization.TryCustomCopy<NukeDiskComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan? target2 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.TimeModifier, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan?>(this.TimeModifier, hookCtx, context);
    target.TimeModifier = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MicrowaveMean, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.MicrowaveMean, hookCtx, context);
    target.MicrowaveMean = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MicrowaveStd, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.MicrowaveStd, hookCtx, context);
    target.MicrowaveStd = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NukeDiskComponent target,
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
    NukeDiskComponent target1 = (NukeDiskComponent) target;
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
    NukeDiskComponent target1 = (NukeDiskComponent) target;
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
    NukeDiskComponent target1 = (NukeDiskComponent) target;
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
  virtual NukeDiskComponent Component.Instantiate() => new NukeDiskComponent();
}
