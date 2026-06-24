// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.TailTrip.XenoTailTripComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.TailTrip;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (XenoTailTripSystem)})]
public sealed class XenoTailTripComponent : 
  Component,
  ISerializationGenerated<XenoTailTripComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int PlasmaCost = 30;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan SlowTime = TimeSpan.FromSeconds(0.3);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan StunTime = TimeSpan.FromSeconds(0.2);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MarkedStunTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MarkedDazeTime = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId TailEffect = (EntProtoId) "RMCEffectDisarm";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("XenoTailSwipe");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTailTripComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTailTripComponent) target1;
    if (serialization.TryCustomCopy<XenoTailTripComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = this.PlasmaCost;
    target.PlasmaCost = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.SlowTime, hookCtx, context);
    target.SlowTime = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context);
    target.StunTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MarkedStunTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.MarkedStunTime, hookCtx, context);
    target.MarkedStunTime = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MarkedDazeTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.MarkedDazeTime, hookCtx, context);
    target.MarkedDazeTime = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.TailEffect, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.TailEffect, hookCtx, context);
    target.TailEffect = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTailTripComponent target,
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
    XenoTailTripComponent target1 = (XenoTailTripComponent) target;
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
    XenoTailTripComponent target1 = (XenoTailTripComponent) target;
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
    XenoTailTripComponent target1 = (XenoTailTripComponent) target;
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
  virtual XenoTailTripComponent Component.Instantiate() => new XenoTailTripComponent();
}
