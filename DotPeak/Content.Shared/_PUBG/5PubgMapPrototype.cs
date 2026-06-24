// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.AirdropSettings
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
public sealed class AirdropSettings : 
  ISerializationGenerated<AirdropSettings>,
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
  public bool UseNextZoneIfDropAfterStateChange = true;
  [DataField(null, false, 1, false, false, null)]
  public float SafeZoneMarginTiles = 6f;
  [DataField(null, false, 1, false, false, null)]
  public float SafeZoneMarginPercent = 0.08f;
  [DataField(null, false, 1, false, false, null)]
  public bool FallbackToMapWhenNoZone = true;
  [DataField(null, false, 1, false, false, null)]
  public bool FallbackToMapWhenZoneActive;
  [DataField(null, false, 1, false, false, null)]
  public float DropDelayMinSeconds = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float DropDelayMaxSeconds = 50f;
  [DataField(null, false, 1, false, false, null)]
  public float StartDelaySeconds = 30f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AirdropSettings target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<AirdropSettings>(this, ref target, hookCtx, false, context))
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
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireZoneVisible, ref target6, hookCtx, false, context))
      target6 = this.RequireZoneVisible;
    target.RequireZoneVisible = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinZonePhase, ref target7, hookCtx, false, context))
      target7 = this.MinZonePhase;
    target.MinZonePhase = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxZonePhase, ref target8, hookCtx, false, context))
      target8 = this.MaxZonePhase;
    target.MaxZonePhase = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowDuringZoneShrink, ref target9, hookCtx, false, context))
      target9 = this.AllowDuringZoneShrink;
    target.AllowDuringZoneShrink = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseNextZoneOnShrink, ref target10, hookCtx, false, context))
      target10 = this.UseNextZoneOnShrink;
    target.UseNextZoneOnShrink = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseNextZoneIfDropAfterStateChange, ref target11, hookCtx, false, context))
      target11 = this.UseNextZoneIfDropAfterStateChange;
    target.UseNextZoneIfDropAfterStateChange = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SafeZoneMarginTiles, ref target12, hookCtx, false, context))
      target12 = this.SafeZoneMarginTiles;
    target.SafeZoneMarginTiles = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SafeZoneMarginPercent, ref target13, hookCtx, false, context))
      target13 = this.SafeZoneMarginPercent;
    target.SafeZoneMarginPercent = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.FallbackToMapWhenNoZone, ref target14, hookCtx, false, context))
      target14 = this.FallbackToMapWhenNoZone;
    target.FallbackToMapWhenNoZone = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.FallbackToMapWhenZoneActive, ref target15, hookCtx, false, context))
      target15 = this.FallbackToMapWhenZoneActive;
    target.FallbackToMapWhenZoneActive = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DropDelayMinSeconds, ref target16, hookCtx, false, context))
      target16 = this.DropDelayMinSeconds;
    target.DropDelayMinSeconds = target16;
    float target17 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DropDelayMaxSeconds, ref target17, hookCtx, false, context))
      target17 = this.DropDelayMaxSeconds;
    target.DropDelayMaxSeconds = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StartDelaySeconds, ref target18, hookCtx, false, context))
      target18 = this.StartDelaySeconds;
    target.StartDelaySeconds = target18;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AirdropSettings target,
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
    AirdropSettings target1 = (AirdropSettings) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public AirdropSettings Instantiate() => new AirdropSettings();
}
