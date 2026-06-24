// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Dodge.XenoActiveDodgeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Dodge;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (XenoDodgeSystem)})]
public sealed class XenoActiveDodgeComponent : 
  Component,
  ISerializationGenerated<XenoActiveDodgeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 SpeedMult = FixedPoint2.New(0.25);
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 CrowdSpeedAddMult = FixedPoint2.New(0.25);
  [DataField(null, false, 1, false, false, null)]
  public float CrowdRange = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ExpiresAt;
  [DataField(null, false, 1, false, false, null)]
  public bool InCrowd;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoActiveDodgeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoActiveDodgeComponent) target1;
    if (serialization.TryCustomCopy<XenoActiveDodgeComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.SpeedMult, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.SpeedMult, hookCtx, context);
    target.SpeedMult = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.CrowdSpeedAddMult, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.CrowdSpeedAddMult, hookCtx, context);
    target.CrowdSpeedAddMult = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CrowdRange, ref target4, hookCtx, false, context))
      target4 = this.CrowdRange;
    target.CrowdRange = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpiresAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ExpiresAt, hookCtx, context);
    target.ExpiresAt = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.InCrowd, ref target6, hookCtx, false, context))
      target6 = this.InCrowd;
    target.InCrowd = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoActiveDodgeComponent target,
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
    XenoActiveDodgeComponent target1 = (XenoActiveDodgeComponent) target;
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
    XenoActiveDodgeComponent target1 = (XenoActiveDodgeComponent) target;
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
    XenoActiveDodgeComponent target1 = (XenoActiveDodgeComponent) target;
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
  virtual XenoActiveDodgeComponent Component.Instantiate() => new XenoActiveDodgeComponent();
}
