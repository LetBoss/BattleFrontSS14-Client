// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.RedZoneComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG;

[RegisterComponent]
[NetworkedComponent]
public sealed class RedZoneComponent : 
  Component,
  ISerializationGenerated<RedZoneComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Active;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 OverallCenter;
  [DataField(null, false, 1, false, false, null)]
  public float OverallRadius;
  [DataField(null, false, 1, false, false, null)]
  public MapId MapId;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 CurrentBombCenter;
  [DataField(null, false, 1, false, false, null)]
  public float CurrentBombRadius;
  [DataField(null, false, 1, false, false, null)]
  public float BombInitialRadius = 1.2f;
  [DataField(null, false, 1, false, false, null)]
  public float BombFinalRadius = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  public float BombShrinkDuration = 1f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? ZoneEndTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? NextBombTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? CurrentBombStartTime;
  [DataField(null, false, 1, false, false, null)]
  public float DelayBetweenBombs = 1f;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RedZoneComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RedZoneComponent) target1;
    if (serialization.TryCustomCopy<RedZoneComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target2, hookCtx, false, context))
      target2 = this.Active;
    target.Active = target2;
    Vector2 target3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OverallCenter, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2>(this.OverallCenter, hookCtx, context);
    target.OverallCenter = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OverallRadius, ref target4, hookCtx, false, context))
      target4 = this.OverallRadius;
    target.OverallRadius = target4;
    MapId target5 = new MapId();
    if (!serialization.TryCustomCopy<MapId>(this.MapId, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<MapId>(this.MapId, hookCtx, context);
    target.MapId = target5;
    Vector2 target6 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.CurrentBombCenter, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2>(this.CurrentBombCenter, hookCtx, context);
    target.CurrentBombCenter = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CurrentBombRadius, ref target7, hookCtx, false, context))
      target7 = this.CurrentBombRadius;
    target.CurrentBombRadius = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BombInitialRadius, ref target8, hookCtx, false, context))
      target8 = this.BombInitialRadius;
    target.BombInitialRadius = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BombFinalRadius, ref target9, hookCtx, false, context))
      target9 = this.BombFinalRadius;
    target.BombFinalRadius = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BombShrinkDuration, ref target10, hookCtx, false, context))
      target10 = this.BombShrinkDuration;
    target.BombShrinkDuration = target10;
    TimeSpan? target11 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ZoneEndTime, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan?>(this.ZoneEndTime, hookCtx, context);
    target.ZoneEndTime = target11;
    TimeSpan? target12 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextBombTime, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan?>(this.NextBombTime, hookCtx, context);
    target.NextBombTime = target12;
    TimeSpan? target13 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.CurrentBombStartTime, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan?>(this.CurrentBombStartTime, hookCtx, context);
    target.CurrentBombStartTime = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DelayBetweenBombs, ref target14, hookCtx, false, context))
      target14 = this.DelayBetweenBombs;
    target.DelayBetweenBombs = target14;
    int target15 = 0;
    if (!serialization.TryCustomCopy<int>(this.PatternRandomWeight, ref target15, hookCtx, false, context))
      target15 = this.PatternRandomWeight;
    target.PatternRandomWeight = target15;
    int target16 = 0;
    if (!serialization.TryCustomCopy<int>(this.PatternClusterWeight, ref target16, hookCtx, false, context))
      target16 = this.PatternClusterWeight;
    target.PatternClusterWeight = target16;
    int target17 = 0;
    if (!serialization.TryCustomCopy<int>(this.PatternLineWeight, ref target17, hookCtx, false, context))
      target17 = this.PatternLineWeight;
    target.PatternLineWeight = target17;
    int target18 = 0;
    if (!serialization.TryCustomCopy<int>(this.PatternRingWeight, ref target18, hookCtx, false, context))
      target18 = this.PatternRingWeight;
    target.PatternRingWeight = target18;
    int target19 = 0;
    if (!serialization.TryCustomCopy<int>(this.ClusterCountMin, ref target19, hookCtx, false, context))
      target19 = this.ClusterCountMin;
    target.ClusterCountMin = target19;
    int target20 = 0;
    if (!serialization.TryCustomCopy<int>(this.ClusterCountMax, ref target20, hookCtx, false, context))
      target20 = this.ClusterCountMax;
    target.ClusterCountMax = target20;
    float target21 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ClusterRadius, ref target21, hookCtx, false, context))
      target21 = this.ClusterRadius;
    target.ClusterRadius = target21;
    int target22 = 0;
    if (!serialization.TryCustomCopy<int>(this.LineCountMin, ref target22, hookCtx, false, context))
      target22 = this.LineCountMin;
    target.LineCountMin = target22;
    int target23 = 0;
    if (!serialization.TryCustomCopy<int>(this.LineCountMax, ref target23, hookCtx, false, context))
      target23 = this.LineCountMax;
    target.LineCountMax = target23;
    float target24 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LineSpacing, ref target24, hookCtx, false, context))
      target24 = this.LineSpacing;
    target.LineSpacing = target24;
    int target25 = 0;
    if (!serialization.TryCustomCopy<int>(this.RingCount, ref target25, hookCtx, false, context))
      target25 = this.RingCount;
    target.RingCount = target25;
    float target26 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RingRadiusMin, ref target26, hookCtx, false, context))
      target26 = this.RingRadiusMin;
    target.RingRadiusMin = target26;
    float target27 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RingRadiusMax, ref target27, hookCtx, false, context))
      target27 = this.RingRadiusMax;
    target.RingRadiusMax = target27;
    bool target28 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShakeEnabled, ref target28, hookCtx, false, context))
      target28 = this.ShakeEnabled;
    target.ShakeEnabled = target28;
    int target29 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShakeShakes, ref target29, hookCtx, false, context))
      target29 = this.ShakeShakes;
    target.ShakeShakes = target29;
    int target30 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShakeStrength, ref target30, hookCtx, false, context))
      target30 = this.ShakeStrength;
    target.ShakeStrength = target30;
    float target31 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShakeSpacingSeconds, ref target31, hookCtx, false, context))
      target31 = this.ShakeSpacingSeconds;
    target.ShakeSpacingSeconds = target31;
    float target32 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShakeRadiusMultiplier, ref target32, hookCtx, false, context))
      target32 = this.ShakeRadiusMultiplier;
    target.ShakeRadiusMultiplier = target32;
    float target33 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ProtectedRadiusTiles, ref target33, hookCtx, false, context))
      target33 = this.ProtectedRadiusTiles;
    target.ProtectedRadiusTiles = target33;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RedZoneComponent target,
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
    RedZoneComponent target1 = (RedZoneComponent) target;
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
    RedZoneComponent target1 = (RedZoneComponent) target;
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
    RedZoneComponent target1 = (RedZoneComponent) target;
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
  virtual RedZoneComponent Component.Instantiate() => new RedZoneComponent();
}
