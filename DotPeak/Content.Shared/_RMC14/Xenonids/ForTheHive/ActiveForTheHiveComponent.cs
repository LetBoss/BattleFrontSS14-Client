// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ForTheHive.ActiveForTheHiveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ForTheHive;

[RegisterComponent]
[NetworkedComponent]
public sealed class ActiveForTheHiveComponent : 
  Component,
  ISerializationGenerated<ActiveForTheHiveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Duration;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TimeLeft;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier WindingUpSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/runner_charging_1.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier WindingDownSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/runner_charging_2.ogg");
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NextUpdate;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UpdateEvery = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public bool UseWindUpSound = true;
  [DataField(null, false, 1, false, false, null)]
  public float InitialVolume = -3f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxVolume = 23f;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier BaseDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  public float AcidRangeRatio = 200f;
  [DataField(null, false, 1, false, false, null)]
  public float BurnRangeRatio = 100f;
  [DataField(null, false, 1, false, false, null)]
  public float BurnDamageRatio = 5f;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Acid = (EntProtoId) "XenoAcidNormal";
  [DataField(null, false, 1, false, false, null)]
  public XenoAcidStrength AcidStrength = XenoAcidStrength.Normal;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AcidTime = TimeSpan.FromSeconds((long) byte.MaxValue);
  [DataField(null, false, 1, false, false, null)]
  public float AcidDps = 8f;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId AcidSmoke = (EntProtoId) "RMCSmokeRunner";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier KaboomSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/blobattack.ogg");
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan CoreSpawnTime = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan CorpseSpawnTime = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 SlowDown = FixedPoint2.New(0.45);
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry? MobAcid;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActiveForTheHiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActiveForTheHiveComponent) target1;
    if (serialization.TryCustomCopy<ActiveForTheHiveComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeLeft, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.TimeLeft, hookCtx, context);
    target.TimeLeft = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.WindingUpSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WindingUpSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.WindingUpSound, hookCtx, context);
    target.WindingUpSound = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.WindingDownSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WindingDownSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.WindingDownSound, hookCtx, context);
    target.WindingDownSound = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdate, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NextUpdate, hookCtx, context);
    target.NextUpdate = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateEvery, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.UpdateEvery, hookCtx, context);
    target.UpdateEvery = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseWindUpSound, ref target8, hookCtx, false, context))
      target8 = this.UseWindUpSound;
    target.UseWindUpSound = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InitialVolume, ref target9, hookCtx, false, context))
      target9 = this.InitialVolume;
    target.InitialVolume = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxVolume, ref target10, hookCtx, false, context))
      target10 = this.MaxVolume;
    target.MaxVolume = target10;
    DamageSpecifier target11 = (DamageSpecifier) null;
    if (this.BaseDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.BaseDamage, ref target11, hookCtx, false, context))
    {
      if (this.BaseDamage == null)
        target11 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.BaseDamage, ref target11, hookCtx, context, true);
    }
    target.BaseDamage = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AcidRangeRatio, ref target12, hookCtx, false, context))
      target12 = this.AcidRangeRatio;
    target.AcidRangeRatio = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BurnRangeRatio, ref target13, hookCtx, false, context))
      target13 = this.BurnRangeRatio;
    target.BurnRangeRatio = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BurnDamageRatio, ref target14, hookCtx, false, context))
      target14 = this.BurnDamageRatio;
    target.BurnDamageRatio = target14;
    EntProtoId target15 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Acid, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntProtoId>(this.Acid, hookCtx, context);
    target.Acid = target15;
    XenoAcidStrength target16 = (XenoAcidStrength) 0;
    if (!serialization.TryCustomCopy<XenoAcidStrength>(this.AcidStrength, ref target16, hookCtx, false, context))
      target16 = this.AcidStrength;
    target.AcidStrength = target16;
    TimeSpan target17 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AcidTime, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<TimeSpan>(this.AcidTime, hookCtx, context);
    target.AcidTime = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AcidDps, ref target18, hookCtx, false, context))
      target18 = this.AcidDps;
    target.AcidDps = target18;
    EntProtoId target19 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.AcidSmoke, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<EntProtoId>(this.AcidSmoke, hookCtx, context);
    target.AcidSmoke = target19;
    SoundSpecifier target20 = (SoundSpecifier) null;
    if (this.KaboomSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.KaboomSound, ref target20, hookCtx, true, context))
      target20 = serialization.CreateCopy<SoundSpecifier>(this.KaboomSound, hookCtx, context);
    target.KaboomSound = target20;
    TimeSpan target21 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CoreSpawnTime, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<TimeSpan>(this.CoreSpawnTime, hookCtx, context);
    target.CoreSpawnTime = target21;
    TimeSpan target22 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CorpseSpawnTime, ref target22, hookCtx, false, context))
      target22 = serialization.CreateCopy<TimeSpan>(this.CorpseSpawnTime, hookCtx, context);
    target.CorpseSpawnTime = target22;
    FixedPoint2 target23 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.SlowDown, ref target23, hookCtx, false, context))
      target23 = serialization.CreateCopy<FixedPoint2>(this.SlowDown, hookCtx, context);
    target.SlowDown = target23;
    ComponentRegistry target24 = (ComponentRegistry) null;
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.MobAcid, ref target24, hookCtx, false, context))
      target24 = serialization.CreateCopy<ComponentRegistry>(this.MobAcid, hookCtx, context);
    target.MobAcid = target24;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActiveForTheHiveComponent target,
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
    ActiveForTheHiveComponent target1 = (ActiveForTheHiveComponent) target;
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
    ActiveForTheHiveComponent target1 = (ActiveForTheHiveComponent) target;
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
    ActiveForTheHiveComponent target1 = (ActiveForTheHiveComponent) target;
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
  virtual ActiveForTheHiveComponent Component.Instantiate() => new ActiveForTheHiveComponent();
}
