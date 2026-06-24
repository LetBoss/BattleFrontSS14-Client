// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Weapons.CivSuppressionEmitterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Weapons;

[RegisterComponent]
[Access(new Type[] {typeof (SharedCivSuppressionSystem)})]
public sealed class CivSuppressionEmitterComponent : 
  Component,
  ISerializationGenerated<CivSuppressionEmitterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public CivSuppressionVisualProfile VisualProfile;
  [DataField(null, false, 1, false, false, null)]
  public float NearMissRadius = 1.75f;
  [DataField(null, false, 1, false, false, null)]
  public float NearMissAmount = 0.14f;
  [DataField(null, false, 1, false, false, null)]
  public float HitAmount = 0.32f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DecayTime = TimeSpan.FromSeconds(2.7999999523162842);
  [DataField(null, false, 1, false, false, null)]
  public float MaxIntensity = 1f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NearMissCooldown = TimeSpan.FromSeconds(0.18000000715255737);
  [DataField(null, false, 1, false, false, null)]
  public float ShotPenaltyDegrees = 4f;
  [DataField(null, false, 1, false, false, null)]
  public float FreshTargetSuppressionMultiplier = 0.6f;
  [DataField(null, false, 1, false, false, null)]
  public float HighStressThreshold = 0.65f;
  [DataField(null, false, 1, false, false, null)]
  public float HighStressShotPenaltyMultiplier = 1.75f;
  [DataField(null, false, 1, false, false, null)]
  public float NearMissMinimumFactor = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  public float NearMissExponent = 1.6f;
  [DataField(null, false, 1, false, false, null)]
  public float OccludedNearMissMultiplier = 0.35f;
  [DataField(null, false, 1, false, false, null)]
  public float WhizzDistance = 2.25f;
  [DataField(null, false, 1, false, false, null)]
  public float WhizzAudioRange = 24f;
  [DataField(null, false, 1, false, false, null)]
  public float WhizzMinVolume = -6f;
  [DataField(null, false, 1, false, false, null)]
  public float WhizzMaxVolume = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float CrackThreshold = 0.78f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier WhizzSound = (SoundSpecifier) new SoundCollectionSpecifier("CivSuppressionWhizz");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier CrackSound = (SoundSpecifier) new SoundCollectionSpecifier("CivSuppressionCrack");
  [DataField(null, false, 1, false, false, null)]
  public float VisualShockMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float VisualSwayMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan VisualRecoveryDelay = TimeSpan.FromSeconds(0.44999998807907104);
  [DataField(null, false, 1, false, false, null)]
  public float VisualRingThreshold = 0.72f;
  [DataField(null, false, 1, false, false, null)]
  public float VisualRingVolume = -11f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan VisualRingCooldown = TimeSpan.FromSeconds(1.3500000238418579);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivSuppressionEmitterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivSuppressionEmitterComponent) target1;
    if (serialization.TryCustomCopy<CivSuppressionEmitterComponent>(this, ref target, hookCtx, false, context))
      return;
    CivSuppressionVisualProfile target2 = CivSuppressionVisualProfile.IncomingFire;
    if (!serialization.TryCustomCopy<CivSuppressionVisualProfile>(this.VisualProfile, ref target2, hookCtx, false, context))
      target2 = this.VisualProfile;
    target.VisualProfile = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NearMissRadius, ref target3, hookCtx, false, context))
      target3 = this.NearMissRadius;
    target.NearMissRadius = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NearMissAmount, ref target4, hookCtx, false, context))
      target4 = this.NearMissAmount;
    target.NearMissAmount = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HitAmount, ref target5, hookCtx, false, context))
      target5 = this.HitAmount;
    target.HitAmount = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DecayTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.DecayTime, hookCtx, context);
    target.DecayTime = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxIntensity, ref target7, hookCtx, false, context))
      target7 = this.MaxIntensity;
    target.MaxIntensity = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NearMissCooldown, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.NearMissCooldown, hookCtx, context);
    target.NearMissCooldown = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShotPenaltyDegrees, ref target9, hookCtx, false, context))
      target9 = this.ShotPenaltyDegrees;
    target.ShotPenaltyDegrees = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FreshTargetSuppressionMultiplier, ref target10, hookCtx, false, context))
      target10 = this.FreshTargetSuppressionMultiplier;
    target.FreshTargetSuppressionMultiplier = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HighStressThreshold, ref target11, hookCtx, false, context))
      target11 = this.HighStressThreshold;
    target.HighStressThreshold = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HighStressShotPenaltyMultiplier, ref target12, hookCtx, false, context))
      target12 = this.HighStressShotPenaltyMultiplier;
    target.HighStressShotPenaltyMultiplier = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NearMissMinimumFactor, ref target13, hookCtx, false, context))
      target13 = this.NearMissMinimumFactor;
    target.NearMissMinimumFactor = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NearMissExponent, ref target14, hookCtx, false, context))
      target14 = this.NearMissExponent;
    target.NearMissExponent = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OccludedNearMissMultiplier, ref target15, hookCtx, false, context))
      target15 = this.OccludedNearMissMultiplier;
    target.OccludedNearMissMultiplier = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WhizzDistance, ref target16, hookCtx, false, context))
      target16 = this.WhizzDistance;
    target.WhizzDistance = target16;
    float target17 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WhizzAudioRange, ref target17, hookCtx, false, context))
      target17 = this.WhizzAudioRange;
    target.WhizzAudioRange = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WhizzMinVolume, ref target18, hookCtx, false, context))
      target18 = this.WhizzMinVolume;
    target.WhizzMinVolume = target18;
    float target19 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WhizzMaxVolume, ref target19, hookCtx, false, context))
      target19 = this.WhizzMaxVolume;
    target.WhizzMaxVolume = target19;
    float target20 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CrackThreshold, ref target20, hookCtx, false, context))
      target20 = this.CrackThreshold;
    target.CrackThreshold = target20;
    SoundSpecifier target21 = (SoundSpecifier) null;
    if (this.WhizzSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WhizzSound, ref target21, hookCtx, true, context))
      target21 = serialization.CreateCopy<SoundSpecifier>(this.WhizzSound, hookCtx, context);
    target.WhizzSound = target21;
    SoundSpecifier target22 = (SoundSpecifier) null;
    if (this.CrackSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CrackSound, ref target22, hookCtx, true, context))
      target22 = serialization.CreateCopy<SoundSpecifier>(this.CrackSound, hookCtx, context);
    target.CrackSound = target22;
    float target23 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VisualShockMultiplier, ref target23, hookCtx, false, context))
      target23 = this.VisualShockMultiplier;
    target.VisualShockMultiplier = target23;
    float target24 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VisualSwayMultiplier, ref target24, hookCtx, false, context))
      target24 = this.VisualSwayMultiplier;
    target.VisualSwayMultiplier = target24;
    TimeSpan target25 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.VisualRecoveryDelay, ref target25, hookCtx, false, context))
      target25 = serialization.CreateCopy<TimeSpan>(this.VisualRecoveryDelay, hookCtx, context);
    target.VisualRecoveryDelay = target25;
    float target26 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VisualRingThreshold, ref target26, hookCtx, false, context))
      target26 = this.VisualRingThreshold;
    target.VisualRingThreshold = target26;
    float target27 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VisualRingVolume, ref target27, hookCtx, false, context))
      target27 = this.VisualRingVolume;
    target.VisualRingVolume = target27;
    TimeSpan target28 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.VisualRingCooldown, ref target28, hookCtx, false, context))
      target28 = serialization.CreateCopy<TimeSpan>(this.VisualRingCooldown, hookCtx, context);
    target.VisualRingCooldown = target28;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivSuppressionEmitterComponent target,
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
    CivSuppressionEmitterComponent target1 = (CivSuppressionEmitterComponent) target;
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
    CivSuppressionEmitterComponent target1 = (CivSuppressionEmitterComponent) target;
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
    CivSuppressionEmitterComponent target1 = (CivSuppressionEmitterComponent) target;
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
  virtual CivSuppressionEmitterComponent Component.Instantiate()
  {
    return new CivSuppressionEmitterComponent();
  }
}
