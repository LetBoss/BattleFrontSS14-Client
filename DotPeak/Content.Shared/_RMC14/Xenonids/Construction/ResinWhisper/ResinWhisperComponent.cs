// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.ResinWhisper.ResinWhispererComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.ResinWhisper;

[RegisterComponent]
[NetworkedComponent]
public sealed class ResinWhispererComponent : 
  Component,
  ISerializationGenerated<ResinWhispererComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? StandardConstructDelay;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2? MaxConstructDistance;
  [DataField(null, false, 1, false, false, null)]
  public float MaxRemoteConstructDistance = 100f;
  [DataField(null, false, 1, false, false, null)]
  public float RemoteConstructDelayMultiplier = 2.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ResinWhispererComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ResinWhispererComponent) target1;
    if (serialization.TryCustomCopy<ResinWhispererComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan? target2 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.StandardConstructDelay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan?>(this.StandardConstructDelay, hookCtx, context);
    target.StandardConstructDelay = target2;
    FixedPoint2? target3 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.MaxConstructDistance, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2?>(this.MaxConstructDistance, hookCtx, context);
    target.MaxConstructDistance = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxRemoteConstructDistance, ref target4, hookCtx, false, context))
      target4 = this.MaxRemoteConstructDistance;
    target.MaxRemoteConstructDistance = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RemoteConstructDelayMultiplier, ref target5, hookCtx, false, context))
      target5 = this.RemoteConstructDelayMultiplier;
    target.RemoteConstructDelayMultiplier = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ResinWhispererComponent target,
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
    ResinWhispererComponent target1 = (ResinWhispererComponent) target;
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
    ResinWhispererComponent target1 = (ResinWhispererComponent) target;
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
    ResinWhispererComponent target1 = (ResinWhispererComponent) target;
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
  virtual ResinWhispererComponent Component.Instantiate() => new ResinWhispererComponent();
}
