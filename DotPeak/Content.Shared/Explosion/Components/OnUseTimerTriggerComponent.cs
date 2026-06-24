// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.Components.OnUseTimerTriggerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Guidebook;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Explosion.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class OnUseTimerTriggerComponent : 
  Component,
  ISerializationGenerated<OnUseTimerTriggerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Delay = 1f;
  [DataField(null, false, 1, false, false, null)]
  public List<float>? DelayOptions;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? BeepSound;
  [DataField(null, false, 1, false, false, null)]
  public float? InitialBeepDelay;
  [DataField(null, false, 1, false, false, null)]
  public float BeepInterval = 1f;
  [DataField(null, false, 1, false, false, null)]
  public bool UseVerbInstead;
  [DataField(null, false, 1, false, false, null)]
  public bool StartOnStick;
  [DataField("canToggleStartOnStick", false, 1, false, false, null)]
  public bool AllowToggleStartOnStick;
  [DataField(null, false, 1, false, false, null)]
  public bool Examinable = true;
  [DataField(null, false, 1, false, false, null)]
  public bool DoPopup = true;

  [GuidebookData]
  public float? ShortestDelayOption
  {
    get
    {
      List<float> delayOptions = this.DelayOptions;
      return delayOptions == null ? new float?() : new float?(delayOptions.Min());
    }
  }

  [GuidebookData]
  public float? LongestDelayOption
  {
    get
    {
      List<float> delayOptions = this.DelayOptions;
      return delayOptions == null ? new float?() : new float?(delayOptions.Max());
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OnUseTimerTriggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OnUseTimerTriggerComponent) target1;
    if (serialization.TryCustomCopy<OnUseTimerTriggerComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref target2, hookCtx, false, context))
      target2 = this.Delay;
    target.Delay = target2;
    List<float> target3 = (List<float>) null;
    if (!serialization.TryCustomCopy<List<float>>(this.DelayOptions, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<float>>(this.DelayOptions, hookCtx, context);
    target.DelayOptions = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BeepSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.BeepSound, hookCtx, context);
    target.BeepSound = target4;
    float? target5 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.InitialBeepDelay, ref target5, hookCtx, false, context))
      target5 = this.InitialBeepDelay;
    target.InitialBeepDelay = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BeepInterval, ref target6, hookCtx, false, context))
      target6 = this.BeepInterval;
    target.BeepInterval = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseVerbInstead, ref target7, hookCtx, false, context))
      target7 = this.UseVerbInstead;
    target.UseVerbInstead = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.StartOnStick, ref target8, hookCtx, false, context))
      target8 = this.StartOnStick;
    target.StartOnStick = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowToggleStartOnStick, ref target9, hookCtx, false, context))
      target9 = this.AllowToggleStartOnStick;
    target.AllowToggleStartOnStick = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.Examinable, ref target10, hookCtx, false, context))
      target10 = this.Examinable;
    target.Examinable = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.DoPopup, ref target11, hookCtx, false, context))
      target11 = this.DoPopup;
    target.DoPopup = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OnUseTimerTriggerComponent target,
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
    OnUseTimerTriggerComponent target1 = (OnUseTimerTriggerComponent) target;
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
    OnUseTimerTriggerComponent target1 = (OnUseTimerTriggerComponent) target;
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
    OnUseTimerTriggerComponent target1 = (OnUseTimerTriggerComponent) target;
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
  virtual OnUseTimerTriggerComponent Component.Instantiate() => new OnUseTimerTriggerComponent();
}
