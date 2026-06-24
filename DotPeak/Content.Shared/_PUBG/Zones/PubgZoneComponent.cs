// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.PubgZoneComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG;

[RegisterComponent]
public sealed class PubgZoneComponent : 
  Component,
  ISerializationGenerated<PubgZoneComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Vector2 CurrentCenter;
  [DataField(null, false, 1, false, false, null)]
  public float CurrentRadius;
  [DataField(null, false, 1, false, false, null)]
  public float InitialRadius;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 NextCenter;
  [DataField(null, false, 1, false, false, null)]
  public float NextRadius;
  [DataField(null, false, 1, false, false, null)]
  public int CurrentPhase;
  [DataField(null, false, 1, false, false, null)]
  public ZoneState State;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? StateStartTime;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 ShrinkStartCenter;
  [DataField(null, false, 1, false, false, null)]
  public float ShrinkStartRadius;
  [DataField(null, false, 1, false, false, null)]
  public float WaitDuration = 60f;
  [DataField(null, false, 1, false, false, null)]
  public float ShrinkDuration = 120f;
  [DataField(null, false, 1, false, false, null)]
  public bool Active;
  [DataField(null, false, 1, false, false, null)]
  public int TotalPhases = 6;
  [DataField(null, false, 1, false, false, null)]
  public float[] PhaseRadiusPercents = new float[7]
  {
    1f,
    0.75f,
    0.55f,
    0.35f,
    0.2f,
    0.1f,
    0.05f
  };
  [DataField(null, false, 1, false, false, null)]
  public float[] PhaseWaitDurations = new float[6]
  {
    135f,
    113f,
    90f,
    75f,
    60f,
    45f
  };
  [DataField(null, false, 1, false, false, null)]
  public float[] PhaseShrinkDurations = new float[6]
  {
    90f,
    75f,
    60f,
    45f,
    38f,
    30f
  };
  [DataField(null, false, 1, false, false, null)]
  public float[] PhaseDamage = new float[6]
  {
    2f,
    5f,
    8f,
    12f,
    20f,
    30f
  };
  [DataField(null, false, 1, false, false, null)]
  public int RandomizeCenterFromPhase = 1;
  [DataField(null, false, 1, false, false, null)]
  public bool PendingFirstZoneReveal;
  [DataField(null, false, 1, false, false, null)]
  public float FirstZoneRevealDelaySeconds;
  [DataField(null, false, 1, false, false, null)]
  public bool FirstZoneRevealRandomizeCenter = true;
  [DataField(null, false, 1, false, false, null)]
  public bool PendingInitialShrinkToFirstPhase;
  [DataField(null, false, 1, false, false, null)]
  public float DynamicSpeedMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float ZoneTimeModifierPercent;
  [DataField(null, false, 1, false, false, null)]
  public float CurrentStateTimeModifierPercent;
  [DataField(null, false, 1, false, false, null)]
  public bool UseDynamicSpeed = true;
  [DataField(null, false, 1, false, false, null)]
  public int DynamicTimeMaxPhase = 5;
  [DataField(null, false, 1, false, false, null)]
  public float DynamicTimeMinMultiplier = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  public int InitialAlivePlayers;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? WarningSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_PUBG/Effects/zone_warning.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ShrinkStartSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_PUBG/Effects/zone_warning.ogg");
  [DataField(null, false, 1, false, false, null)]
  public float[] WarningTimes = new float[2]{ 30f, 10f };
  [DataField(null, false, 1, false, false, null)]
  public HashSet<float> PlayedWarnings = new HashSet<float>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgZoneComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgZoneComponent) target1;
    if (serialization.TryCustomCopy<PubgZoneComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2 target2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.CurrentCenter, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2>(this.CurrentCenter, hookCtx, context);
    target.CurrentCenter = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CurrentRadius, ref target3, hookCtx, false, context))
      target3 = this.CurrentRadius;
    target.CurrentRadius = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InitialRadius, ref target4, hookCtx, false, context))
      target4 = this.InitialRadius;
    target.InitialRadius = target4;
    Vector2 target5 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.NextCenter, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2>(this.NextCenter, hookCtx, context);
    target.NextCenter = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NextRadius, ref target6, hookCtx, false, context))
      target6 = this.NextRadius;
    target.NextRadius = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.CurrentPhase, ref target7, hookCtx, false, context))
      target7 = this.CurrentPhase;
    target.CurrentPhase = target7;
    ZoneState target8 = ZoneState.Waiting;
    if (!serialization.TryCustomCopy<ZoneState>(this.State, ref target8, hookCtx, false, context))
      target8 = this.State;
    target.State = target8;
    TimeSpan? target9 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.StateStartTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan?>(this.StateStartTime, hookCtx, context);
    target.StateStartTime = target9;
    Vector2 target10 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.ShrinkStartCenter, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Vector2>(this.ShrinkStartCenter, hookCtx, context);
    target.ShrinkStartCenter = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShrinkStartRadius, ref target11, hookCtx, false, context))
      target11 = this.ShrinkStartRadius;
    target.ShrinkStartRadius = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WaitDuration, ref target12, hookCtx, false, context))
      target12 = this.WaitDuration;
    target.WaitDuration = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShrinkDuration, ref target13, hookCtx, false, context))
      target13 = this.ShrinkDuration;
    target.ShrinkDuration = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target14, hookCtx, false, context))
      target14 = this.Active;
    target.Active = target14;
    int target15 = 0;
    if (!serialization.TryCustomCopy<int>(this.TotalPhases, ref target15, hookCtx, false, context))
      target15 = this.TotalPhases;
    target.TotalPhases = target15;
    float[] target16 = (float[]) null;
    if (this.PhaseRadiusPercents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.PhaseRadiusPercents, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<float[]>(this.PhaseRadiusPercents, hookCtx, context);
    target.PhaseRadiusPercents = target16;
    float[] target17 = (float[]) null;
    if (this.PhaseWaitDurations == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.PhaseWaitDurations, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<float[]>(this.PhaseWaitDurations, hookCtx, context);
    target.PhaseWaitDurations = target17;
    float[] target18 = (float[]) null;
    if (this.PhaseShrinkDurations == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.PhaseShrinkDurations, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<float[]>(this.PhaseShrinkDurations, hookCtx, context);
    target.PhaseShrinkDurations = target18;
    float[] target19 = (float[]) null;
    if (this.PhaseDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.PhaseDamage, ref target19, hookCtx, true, context))
      target19 = serialization.CreateCopy<float[]>(this.PhaseDamage, hookCtx, context);
    target.PhaseDamage = target19;
    int target20 = 0;
    if (!serialization.TryCustomCopy<int>(this.RandomizeCenterFromPhase, ref target20, hookCtx, false, context))
      target20 = this.RandomizeCenterFromPhase;
    target.RandomizeCenterFromPhase = target20;
    bool target21 = false;
    if (!serialization.TryCustomCopy<bool>(this.PendingFirstZoneReveal, ref target21, hookCtx, false, context))
      target21 = this.PendingFirstZoneReveal;
    target.PendingFirstZoneReveal = target21;
    float target22 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FirstZoneRevealDelaySeconds, ref target22, hookCtx, false, context))
      target22 = this.FirstZoneRevealDelaySeconds;
    target.FirstZoneRevealDelaySeconds = target22;
    bool target23 = false;
    if (!serialization.TryCustomCopy<bool>(this.FirstZoneRevealRandomizeCenter, ref target23, hookCtx, false, context))
      target23 = this.FirstZoneRevealRandomizeCenter;
    target.FirstZoneRevealRandomizeCenter = target23;
    bool target24 = false;
    if (!serialization.TryCustomCopy<bool>(this.PendingInitialShrinkToFirstPhase, ref target24, hookCtx, false, context))
      target24 = this.PendingInitialShrinkToFirstPhase;
    target.PendingInitialShrinkToFirstPhase = target24;
    float target25 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DynamicSpeedMultiplier, ref target25, hookCtx, false, context))
      target25 = this.DynamicSpeedMultiplier;
    target.DynamicSpeedMultiplier = target25;
    float target26 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ZoneTimeModifierPercent, ref target26, hookCtx, false, context))
      target26 = this.ZoneTimeModifierPercent;
    target.ZoneTimeModifierPercent = target26;
    float target27 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CurrentStateTimeModifierPercent, ref target27, hookCtx, false, context))
      target27 = this.CurrentStateTimeModifierPercent;
    target.CurrentStateTimeModifierPercent = target27;
    bool target28 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseDynamicSpeed, ref target28, hookCtx, false, context))
      target28 = this.UseDynamicSpeed;
    target.UseDynamicSpeed = target28;
    int target29 = 0;
    if (!serialization.TryCustomCopy<int>(this.DynamicTimeMaxPhase, ref target29, hookCtx, false, context))
      target29 = this.DynamicTimeMaxPhase;
    target.DynamicTimeMaxPhase = target29;
    float target30 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DynamicTimeMinMultiplier, ref target30, hookCtx, false, context))
      target30 = this.DynamicTimeMinMultiplier;
    target.DynamicTimeMinMultiplier = target30;
    int target31 = 0;
    if (!serialization.TryCustomCopy<int>(this.InitialAlivePlayers, ref target31, hookCtx, false, context))
      target31 = this.InitialAlivePlayers;
    target.InitialAlivePlayers = target31;
    SoundSpecifier target32 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WarningSound, ref target32, hookCtx, true, context))
      target32 = serialization.CreateCopy<SoundSpecifier>(this.WarningSound, hookCtx, context);
    target.WarningSound = target32;
    SoundSpecifier target33 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ShrinkStartSound, ref target33, hookCtx, true, context))
      target33 = serialization.CreateCopy<SoundSpecifier>(this.ShrinkStartSound, hookCtx, context);
    target.ShrinkStartSound = target33;
    float[] target34 = (float[]) null;
    if (this.WarningTimes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.WarningTimes, ref target34, hookCtx, true, context))
      target34 = serialization.CreateCopy<float[]>(this.WarningTimes, hookCtx, context);
    target.WarningTimes = target34;
    HashSet<float> target35 = (HashSet<float>) null;
    if (this.PlayedWarnings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<float>>(this.PlayedWarnings, ref target35, hookCtx, true, context))
      target35 = serialization.CreateCopy<HashSet<float>>(this.PlayedWarnings, hookCtx, context);
    target.PlayedWarnings = target35;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgZoneComponent target,
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
    PubgZoneComponent target1 = (PubgZoneComponent) target;
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
    PubgZoneComponent target1 = (PubgZoneComponent) target;
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
    PubgZoneComponent target1 = (PubgZoneComponent) target;
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
  virtual PubgZoneComponent Component.Instantiate() => new PubgZoneComponent();
}
