// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stun.RMCStunOnHit
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Stun;

[DataDefinition]
[NetSerializable]
[Serializable]
public struct RMCStunOnHit : ISerializationGenerated<RMCStunOnHit>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public float MaxRange;
  [DataField(null, false, 1, false, false, null)]
  public float KnockBackPowerMin;
  [DataField(null, false, 1, false, false, null)]
  public float KnockBackPowerMax;
  [DataField(null, false, 1, false, false, null)]
  public float KnockBackSpeed;
  [DataField(null, false, 1, false, false, null)]
  public bool ForceKnockBack;
  [DataField(null, false, 1, false, false, null)]
  public bool LosesEffectWithRange;
  [DataField(null, false, 1, false, false, null)]
  public bool SlowsEffectBigXenos;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan StunTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan SuperSlowTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan SlowTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DazeTime;
  [DataField(null, false, 1, false, false, null)]
  public float StunArea;

  public RMCStunOnHit()
  {
    this.Whitelist = (EntityWhitelist) null;
    this.ForceKnockBack = false;
    this.LosesEffectWithRange = false;
    this.SlowsEffectBigXenos = false;
    this.DazeTime = new TimeSpan();
    this.MaxRange = 2.5f;
    this.KnockBackPowerMin = 1f;
    this.KnockBackPowerMax = 1f;
    this.KnockBackSpeed = 5f;
    this.StunTime = TimeSpan.FromSeconds(1.4);
    this.SuperSlowTime = TimeSpan.FromSeconds(1L);
    this.SlowTime = TimeSpan.FromSeconds(2L);
    this.StunArea = 0.5f;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCStunOnHit target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RMCStunOnHit>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target1 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target1, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target1 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target1, hookCtx, context);
    }
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxRange, ref target2, hookCtx, false, context))
      target2 = this.MaxRange;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.KnockBackPowerMin, ref target3, hookCtx, false, context))
      target3 = this.KnockBackPowerMin;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.KnockBackPowerMax, ref target4, hookCtx, false, context))
      target4 = this.KnockBackPowerMax;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.KnockBackSpeed, ref target5, hookCtx, false, context))
      target5 = this.KnockBackSpeed;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.ForceKnockBack, ref target6, hookCtx, false, context))
      target6 = this.ForceKnockBack;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.LosesEffectWithRange, ref target7, hookCtx, false, context))
      target7 = this.LosesEffectWithRange;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.SlowsEffectBigXenos, ref target8, hookCtx, false, context))
      target8 = this.SlowsEffectBigXenos;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context);
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SuperSlowTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.SuperSlowTime, hookCtx, context);
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowTime, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.SlowTime, hookCtx, context);
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DazeTime, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.DazeTime, hookCtx, context);
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunArea, ref target13, hookCtx, false, context))
      target13 = this.StunArea;
    target = target with
    {
      Whitelist = target1,
      MaxRange = target2,
      KnockBackPowerMin = target3,
      KnockBackPowerMax = target4,
      KnockBackSpeed = target5,
      ForceKnockBack = target6,
      LosesEffectWithRange = target7,
      SlowsEffectBigXenos = target8,
      StunTime = target9,
      SuperSlowTime = target10,
      SlowTime = target11,
      DazeTime = target12,
      StunArea = target13
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCStunOnHit target,
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
    RMCStunOnHit target1 = (RMCStunOnHit) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RMCStunOnHit Instantiate() => new RMCStunOnHit();
}
