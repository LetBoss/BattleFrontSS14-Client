// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.OrbitalCannon.OrbitalCannonExplosion
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Explosion;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared._RMC14.OrbitalCannon;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class OrbitalCannonExplosion : 
  ISerializationGenerated<OrbitalCannonExplosion>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ExplosionPrototype>? Type;
  [DataField(null, false, 1, false, false, null)]
  public float Total;
  [DataField(null, false, 1, false, false, null)]
  public float Slope;
  [DataField(null, false, 1, false, false, null)]
  public float Max;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Delay;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? Fire;
  [DataField(null, false, 1, false, false, null)]
  public int FireRange = 18;
  [DataField(null, false, 1, false, false, null)]
  public int Intensity = 80 /*0x50*/;
  [DataField(null, false, 1, false, false, null)]
  public int Duration = 70;
  [DataField(null, false, 1, false, false, null)]
  public int Times = 1;
  [DataField(null, false, 1, false, false, null)]
  public int TimesPer = 1;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DelayPer;
  [DataField(null, false, 1, false, false, null)]
  public int Spread;
  [DataField(null, false, 1, false, false, null)]
  public bool CheckProtectionPer;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? ExplosionEffect;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OrbitalCannonExplosion target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<OrbitalCannonExplosion>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ExplosionPrototype>? target1 = new ProtoId<ExplosionPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<ExplosionPrototype>?>(this.Type, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<ExplosionPrototype>?>(this.Type, hookCtx, context);
    target.Type = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Total, ref target2, hookCtx, false, context))
      target2 = this.Total;
    target.Total = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Slope, ref target3, hookCtx, false, context))
      target3 = this.Slope;
    target.Slope = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Max, ref target4, hookCtx, false, context))
      target4 = this.Max;
    target.Max = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target5;
    EntProtoId? target6 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Fire, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId?>(this.Fire, hookCtx, context);
    target.Fire = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.FireRange, ref target7, hookCtx, false, context))
      target7 = this.FireRange;
    target.FireRange = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.Intensity, ref target8, hookCtx, false, context))
      target8 = this.Intensity;
    target.Intensity = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.Duration, ref target9, hookCtx, false, context))
      target9 = this.Duration;
    target.Duration = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.Times, ref target10, hookCtx, false, context))
      target10 = this.Times;
    target.Times = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.TimesPer, ref target11, hookCtx, false, context))
      target11 = this.TimesPer;
    target.TimesPer = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DelayPer, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.DelayPer, hookCtx, context);
    target.DelayPer = target12;
    int target13 = 0;
    if (!serialization.TryCustomCopy<int>(this.Spread, ref target13, hookCtx, false, context))
      target13 = this.Spread;
    target.Spread = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.CheckProtectionPer, ref target14, hookCtx, false, context))
      target14 = this.CheckProtectionPer;
    target.CheckProtectionPer = target14;
    EntProtoId? target15 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.ExplosionEffect, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntProtoId?>(this.ExplosionEffect, hookCtx, context);
    target.ExplosionEffect = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OrbitalCannonExplosion target,
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
    OrbitalCannonExplosion target1 = (OrbitalCannonExplosion) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public OrbitalCannonExplosion Instantiate() => new OrbitalCannonExplosion();
}
