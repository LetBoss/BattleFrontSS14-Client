// Decompiled with JetBrains decompiler
// Type: Content.Shared.Morgue.Components.MorgueComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Morgue.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class MorgueComponent : 
  Component,
  ISerializationGenerated<MorgueComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool DoSoulBeep = true;
  [DataField(null, false, 1, false, false, null)]
  public float AccumulatedFrameTime;
  [DataField(null, false, 1, false, false, null)]
  public float BeepTime = 10f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier OccupantHasSoulAlarmSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/EmptyAlarm/smg_empty_alarm.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MorgueComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MorgueComponent) target1;
    if (serialization.TryCustomCopy<MorgueComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.DoSoulBeep, ref target2, hookCtx, false, context))
      target2 = this.DoSoulBeep;
    target.DoSoulBeep = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AccumulatedFrameTime, ref target3, hookCtx, false, context))
      target3 = this.AccumulatedFrameTime;
    target.AccumulatedFrameTime = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BeepTime, ref target4, hookCtx, false, context))
      target4 = this.BeepTime;
    target.BeepTime = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.OccupantHasSoulAlarmSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OccupantHasSoulAlarmSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.OccupantHasSoulAlarmSound, hookCtx, context);
    target.OccupantHasSoulAlarmSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MorgueComponent target,
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
    MorgueComponent target1 = (MorgueComponent) target;
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
    MorgueComponent target1 = (MorgueComponent) target;
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
    MorgueComponent target1 = (MorgueComponent) target;
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
  virtual MorgueComponent Component.Instantiate() => new MorgueComponent();
}
