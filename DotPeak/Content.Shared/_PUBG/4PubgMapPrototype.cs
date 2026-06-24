// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.RedZoneSettings
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared._PUBG;

[DataDefinition]
public sealed class RedZoneSettings : 
  ISerializationGenerated<RedZoneSettings>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int BaseChancePercent = 10;
  [DataField(null, false, 1, false, false, null)]
  public int ChanceIncreaseMin = 10;
  [DataField(null, false, 1, false, false, null)]
  public int ChanceIncreaseMax = 30;
  [DataField(null, false, 1, false, false, null)]
  public float MinIntervalSeconds = 30f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxIntervalSeconds = 300f;
  [DataField(null, false, 1, false, false, null)]
  public float ZoneRadiusMinPercent = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  public float ZoneRadiusMaxPercent = 0.8f;
  [DataField(null, false, 1, false, false, null)]
  public float ZoneDurationMinSeconds = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float ZoneDurationMaxSeconds = 30f;
  [DataField(null, false, 1, false, false, null)]
  public float BombInitialRadius = 1.2f;
  [DataField(null, false, 1, false, false, null)]
  public float BombFinalRadius = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  public float BombShrinkDuration = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float DelayBetweenBombs = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public bool UsePlayerDensity;
  [DataField(null, false, 1, false, false, null)]
  public int DensityMinPlayers = 6;
  [DataField(null, false, 1, false, false, null)]
  public float DensityWeight = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public int PatternRandomWeight = 100;
  [DataField(null, false, 1, false, false, null)]
  public int PatternClusterWeight;
  [DataField(null, false, 1, false, false, null)]
  public int PatternLineWeight;
  [DataField(null, false, 1, false, false, null)]
  public int PatternRingWeight;
  [DataField(null, false, 1, false, false, null)]
  public int ClusterCountMin = 3;
  [DataField(null, false, 1, false, false, null)]
  public int ClusterCountMax = 6;
  [DataField(null, false, 1, false, false, null)]
  public float ClusterRadius = 6f;
  [DataField(null, false, 1, false, false, null)]
  public int LineCountMin = 4;
  [DataField(null, false, 1, false, false, null)]
  public int LineCountMax = 8;
  [DataField(null, false, 1, false, false, null)]
  public float LineSpacing = 4f;
  [DataField(null, false, 1, false, false, null)]
  public int RingCount = 8;
  [DataField(null, false, 1, false, false, null)]
  public float RingRadiusMin = 8f;
  [DataField(null, false, 1, false, false, null)]
  public float RingRadiusMax = 14f;
  [DataField(null, false, 1, false, false, null)]
  public bool ShakeEnabled = true;
  [DataField(null, false, 1, false, false, null)]
  public int ShakeShakes = 2;
  [DataField(null, false, 1, false, false, null)]
  public int ShakeStrength = 1;
  [DataField(null, false, 1, false, false, null)]
  public float ShakeSpacingSeconds = 0.12f;
  [DataField(null, false, 1, false, false, null)]
  public float ShakeRadiusMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float ProtectedRadiusTiles = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public bool RestrictToSafeZone = true;
  [DataField(null, false, 1, false, false, null)]
  public bool RequireZoneVisible = true;
  [DataField(null, false, 1, false, false, null)]
  public int MinZonePhase;
  [DataField(null, false, 1, false, false, null)]
  public int MaxZonePhase = 6;
  [DataField(null, false, 1, false, false, null)]
  public bool AllowDuringZoneShrink = true;
  [DataField(null, false, 1, false, false, null)]
  public bool UseNextZoneOnShrink = true;
  [DataField(null, false, 1, false, false, null)]
  public float SafeZoneMarginTiles = 6f;
  [DataField(null, false, 1, false, false, null)]
  public float SafeZoneMarginPercent = 0.08f;
  [DataField(null, false, 1, false, false, null)]
  public bool FallbackToMapWhenNoZone = true;
  [DataField(null, false, 1, false, false, null)]
  public bool FallbackToMapWhenZoneActive;
  [DataField(null, false, 1, false, false, null)]
  public float StartDelaySeconds = 45f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RedZoneSettings target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RedZoneSettings>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.BaseChancePercent, ref target1, hookCtx, false, context))
      target1 = this.BaseChancePercent;
    target.BaseChancePercent = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.ChanceIncreaseMin, ref target2, hookCtx, false, context))
      target2 = this.ChanceIncreaseMin;
    target.ChanceIncreaseMin = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.ChanceIncreaseMax, ref target3, hookCtx, false, context))
      target3 = this.ChanceIncreaseMax;
    target.ChanceIncreaseMax = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinIntervalSeconds, ref target4, hookCtx, false, context))
      target4 = this.MinIntervalSeconds;
    target.MinIntervalSeconds = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxIntervalSeconds, ref target5, hookCtx, false, context))
      target5 = this.MaxIntervalSeconds;
    target.MaxIntervalSeconds = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ZoneRadiusMinPercent, ref target6, hookCtx, false, context))
      target6 = this.ZoneRadiusMinPercent;
    target.ZoneRadiusMinPercent = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ZoneRadiusMaxPercent, ref target7, hookCtx, false, context))
      target7 = this.ZoneRadiusMaxPercent;
    target.ZoneRadiusMaxPercent = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ZoneDurationMinSeconds, ref target8, hookCtx, false, context))
      target8 = this.ZoneDurationMinSeconds;
    target.ZoneDurationMinSeconds = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ZoneDurationMaxSeconds, ref target9, hookCtx, false, context))
      target9 = this.ZoneDurationMaxSeconds;
    target.ZoneDurationMaxSeconds = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BombInitialRadius, ref target10, hookCtx, false, context))
      target10 = this.BombInitialRadius;
    target.BombInitialRadius = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BombFinalRadius, ref target11, hookCtx, false, context))
      target11 = this.BombFinalRadius;
    target.BombFinalRadius = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BombShrinkDuration, ref target12, hookCtx, false, context))
      target12 = this.BombShrinkDuration;
    target.BombShrinkDuration = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DelayBetweenBombs, ref target13, hookCtx, false, context))
      target13 = this.DelayBetweenBombs;
    target.DelayBetweenBombs = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.UsePlayerDensity, ref target14, hookCtx, false, context))
      target14 = this.UsePlayerDensity;
    target.UsePlayerDensity = target14;
    int target15 = 0;
    if (!serialization.TryCustomCopy<int>(this.DensityMinPlayers, ref target15, hookCtx, false, context))
      target15 = this.DensityMinPlayers;
    target.DensityMinPlayers = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DensityWeight, ref target16, hookCtx, false, context))
      target16 = this.DensityWeight;
    target.DensityWeight = target16;
    int target17 = 0;
    if (!serialization.TryCustomCopy<int>(this.PatternRandomWeight, ref target17, hookCtx, false, context))
      target17 = this.PatternRandomWeight;
    target.PatternRandomWeight = target17;
    int target18 = 0;
    if (!serialization.TryCustomCopy<int>(this.PatternClusterWeight, ref target18, hookCtx, false, context))
      target18 = this.PatternClusterWeight;
    target.PatternClusterWeight = target18;
    int target19 = 0;
    if (!serialization.TryCustomCopy<int>(this.PatternLineWeight, ref target19, hookCtx, false, context))
      target19 = this.PatternLineWeight;
    target.PatternLineWeight = target19;
    int target20 = 0;
    if (!serialization.TryCustomCopy<int>(this.PatternRingWeight, ref target20, hookCtx, false, context))
      target20 = this.PatternRingWeight;
    target.PatternRingWeight = target20;
    int target21 = 0;
    if (!serialization.TryCustomCopy<int>(this.ClusterCountMin, ref target21, hookCtx, false, context))
      target21 = this.ClusterCountMin;
    target.ClusterCountMin = target21;
    int target22 = 0;
    if (!serialization.TryCustomCopy<int>(this.ClusterCountMax, ref target22, hookCtx, false, context))
      target22 = this.ClusterCountMax;
    target.ClusterCountMax = target22;
    float target23 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ClusterRadius, ref target23, hookCtx, false, context))
      target23 = this.ClusterRadius;
    target.ClusterRadius = target23;
    int target24 = 0;
    if (!serialization.TryCustomCopy<int>(this.LineCountMin, ref target24, hookCtx, false, context))
      target24 = this.LineCountMin;
    target.LineCountMin = target24;
    int target25 = 0;
    if (!serialization.TryCustomCopy<int>(this.LineCountMax, ref target25, hookCtx, false, context))
      target25 = this.LineCountMax;
    target.LineCountMax = target25;
    float target26 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LineSpacing, ref target26, hookCtx, false, context))
      target26 = this.LineSpacing;
    target.LineSpacing = target26;
    int target27 = 0;
    if (!serialization.TryCustomCopy<int>(this.RingCount, ref target27, hookCtx, false, context))
      target27 = this.RingCount;
    target.RingCount = target27;
    float target28 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RingRadiusMin, ref target28, hookCtx, false, context))
      target28 = this.RingRadiusMin;
    target.RingRadiusMin = target28;
    float target29 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RingRadiusMax, ref target29, hookCtx, false, context))
      target29 = this.RingRadiusMax;
    target.RingRadiusMax = target29;
    bool target30 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShakeEnabled, ref target30, hookCtx, false, context))
      target30 = this.ShakeEnabled;
    target.ShakeEnabled = target30;
    int target31 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShakeShakes, ref target31, hookCtx, false, context))
      target31 = this.ShakeShakes;
    target.ShakeShakes = target31;
    int target32 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShakeStrength, ref target32, hookCtx, false, context))
      target32 = this.ShakeStrength;
    target.ShakeStrength = target32;
    float target33 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShakeSpacingSeconds, ref target33, hookCtx, false, context))
      target33 = this.ShakeSpacingSeconds;
    target.ShakeSpacingSeconds = target33;
    float target34 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShakeRadiusMultiplier, ref target34, hookCtx, false, context))
      target34 = this.ShakeRadiusMultiplier;
    target.ShakeRadiusMultiplier = target34;
    float target35 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ProtectedRadiusTiles, ref target35, hookCtx, false, context))
      target35 = this.ProtectedRadiusTiles;
    target.ProtectedRadiusTiles = target35;
    bool target36 = false;
    if (!serialization.TryCustomCopy<bool>(this.RestrictToSafeZone, ref target36, hookCtx, false, context))
      target36 = this.RestrictToSafeZone;
    target.RestrictToSafeZone = target36;
    bool target37 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireZoneVisible, ref target37, hookCtx, false, context))
      target37 = this.RequireZoneVisible;
    target.RequireZoneVisible = target37;
    int target38 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinZonePhase, ref target38, hookCtx, false, context))
      target38 = this.MinZonePhase;
    target.MinZonePhase = target38;
    int target39 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxZonePhase, ref target39, hookCtx, false, context))
      target39 = this.MaxZonePhase;
    target.MaxZonePhase = target39;
    bool target40 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowDuringZoneShrink, ref target40, hookCtx, false, context))
      target40 = this.AllowDuringZoneShrink;
    target.AllowDuringZoneShrink = target40;
    bool target41 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseNextZoneOnShrink, ref target41, hookCtx, false, context))
      target41 = this.UseNextZoneOnShrink;
    target.UseNextZoneOnShrink = target41;
    float target42 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SafeZoneMarginTiles, ref target42, hookCtx, false, context))
      target42 = this.SafeZoneMarginTiles;
    target.SafeZoneMarginTiles = target42;
    float target43 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SafeZoneMarginPercent, ref target43, hookCtx, false, context))
      target43 = this.SafeZoneMarginPercent;
    target.SafeZoneMarginPercent = target43;
    bool target44 = false;
    if (!serialization.TryCustomCopy<bool>(this.FallbackToMapWhenNoZone, ref target44, hookCtx, false, context))
      target44 = this.FallbackToMapWhenNoZone;
    target.FallbackToMapWhenNoZone = target44;
    bool target45 = false;
    if (!serialization.TryCustomCopy<bool>(this.FallbackToMapWhenZoneActive, ref target45, hookCtx, false, context))
      target45 = this.FallbackToMapWhenZoneActive;
    target.FallbackToMapWhenZoneActive = target45;
    float target46 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StartDelaySeconds, ref target46, hookCtx, false, context))
      target46 = this.StartDelaySeconds;
    target.StartDelaySeconds = target46;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RedZoneSettings target,
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
    RedZoneSettings target1 = (RedZoneSettings) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RedZoneSettings Instantiate() => new RedZoneSettings();
}
