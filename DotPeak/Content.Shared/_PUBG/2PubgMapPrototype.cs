// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.ZoneSettings
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared._PUBG;

[DataDefinition]
public sealed class ZoneSettings : ISerializationGenerated<ZoneSettings>, ISerializationGenerated
{
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
  public FirstZoneRevealSettings? FirstZoneReveal;
  [DataField(null, false, 1, false, false, null)]
  public bool UseDynamicSpeed = true;
  [DataField(null, false, 1, false, false, null)]
  public int DynamicTimeMaxPhase = 5;
  [DataField(null, false, 1, false, false, null)]
  public float DynamicTimeMinMultiplier = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? WarningSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_PUBG/Effects/zone_warning.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ShrinkStartSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_PUBG/Effects/zone_warning.ogg");
  [DataField(null, false, 1, false, false, null)]
  public float[] WarningTimes = new float[2]{ 30f, 10f };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ZoneSettings target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ZoneSettings>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.TotalPhases, ref target1, hookCtx, false, context))
      target1 = this.TotalPhases;
    target.TotalPhases = target1;
    float[] target2 = (float[]) null;
    if (this.PhaseRadiusPercents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.PhaseRadiusPercents, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<float[]>(this.PhaseRadiusPercents, hookCtx, context);
    target.PhaseRadiusPercents = target2;
    float[] target3 = (float[]) null;
    if (this.PhaseWaitDurations == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.PhaseWaitDurations, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<float[]>(this.PhaseWaitDurations, hookCtx, context);
    target.PhaseWaitDurations = target3;
    float[] target4 = (float[]) null;
    if (this.PhaseShrinkDurations == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.PhaseShrinkDurations, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<float[]>(this.PhaseShrinkDurations, hookCtx, context);
    target.PhaseShrinkDurations = target4;
    float[] target5 = (float[]) null;
    if (this.PhaseDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.PhaseDamage, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<float[]>(this.PhaseDamage, hookCtx, context);
    target.PhaseDamage = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.RandomizeCenterFromPhase, ref target6, hookCtx, false, context))
      target6 = this.RandomizeCenterFromPhase;
    target.RandomizeCenterFromPhase = target6;
    FirstZoneRevealSettings target7 = (FirstZoneRevealSettings) null;
    if (!serialization.TryCustomCopy<FirstZoneRevealSettings>(this.FirstZoneReveal, ref target7, hookCtx, false, context))
    {
      if (this.FirstZoneReveal == null)
        target7 = (FirstZoneRevealSettings) null;
      else
        serialization.CopyTo<FirstZoneRevealSettings>(this.FirstZoneReveal, ref target7, hookCtx, context);
    }
    target.FirstZoneReveal = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseDynamicSpeed, ref target8, hookCtx, false, context))
      target8 = this.UseDynamicSpeed;
    target.UseDynamicSpeed = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.DynamicTimeMaxPhase, ref target9, hookCtx, false, context))
      target9 = this.DynamicTimeMaxPhase;
    target.DynamicTimeMaxPhase = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DynamicTimeMinMultiplier, ref target10, hookCtx, false, context))
      target10 = this.DynamicTimeMinMultiplier;
    target.DynamicTimeMinMultiplier = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WarningSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.WarningSound, hookCtx, context);
    target.WarningSound = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ShrinkStartSound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.ShrinkStartSound, hookCtx, context);
    target.ShrinkStartSound = target12;
    float[] target13 = (float[]) null;
    if (this.WarningTimes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.WarningTimes, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<float[]>(this.WarningTimes, hookCtx, context);
    target.WarningTimes = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ZoneSettings target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ZoneSettings target1 = (ZoneSettings) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ZoneSettings Instantiate() => new ZoneSettings();
}
