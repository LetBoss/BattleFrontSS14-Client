// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hedgehog.XenoSpikeShedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Hedgehog;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoShardSystem), typeof (XenoSpikeShieldSystem)})]
public sealed class XenoSpikeShedComponent : 
  Component,
  ISerializationGenerated<XenoSpikeShedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MinShards = 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CooldownExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ShardLockDuration = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ShardLockExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpeedBoost = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier ShardDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ShedRadius = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Projectile = (EntProtoId) "XenoHedgehogSpikeProjectileSpread";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ProjectileCount = 40;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? ProjectileHitLimit = new int?(6);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/spike_spray.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoSpikeShedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoSpikeShedComponent) target1;
    if (serialization.TryCustomCopy<XenoSpikeShedComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinShards, ref target2, hookCtx, false, context))
      target2 = this.MinShards;
    target.MinShards = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CooldownExpireAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.CooldownExpireAt, hookCtx, context);
    target.CooldownExpireAt = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShardLockDuration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ShardLockDuration, hookCtx, context);
    target.ShardLockDuration = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShardLockExpireAt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.ShardLockExpireAt, hookCtx, context);
    target.ShardLockExpireAt = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedBoost, ref target7, hookCtx, false, context))
      target7 = this.SpeedBoost;
    target.SpeedBoost = target7;
    DamageSpecifier target8 = (DamageSpecifier) null;
    if (this.ShardDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.ShardDamage, ref target8, hookCtx, false, context))
    {
      if (this.ShardDamage == null)
        target8 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.ShardDamage, ref target8, hookCtx, context, true);
    }
    target.ShardDamage = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShedRadius, ref target9, hookCtx, false, context))
      target9 = this.ShedRadius;
    target.ShedRadius = target9;
    EntProtoId target10 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Projectile, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntProtoId>(this.Projectile, hookCtx, context);
    target.Projectile = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.ProjectileCount, ref target11, hookCtx, false, context))
      target11 = this.ProjectileCount;
    target.ProjectileCount = target11;
    int? target12 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.ProjectileHitLimit, ref target12, hookCtx, false, context))
      target12 = this.ProjectileHitLimit;
    target.ProjectileHitLimit = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoSpikeShedComponent target,
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
    XenoSpikeShedComponent target1 = (XenoSpikeShedComponent) target;
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
    XenoSpikeShedComponent target1 = (XenoSpikeShedComponent) target;
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
    XenoSpikeShedComponent target1 = (XenoSpikeShedComponent) target;
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
  virtual XenoSpikeShedComponent Component.Instantiate() => new XenoSpikeShedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoSpikeShedComponent_AutoState : IComponentState
  {
    public int MinShards;
    public TimeSpan Cooldown;
    public TimeSpan CooldownExpireAt;
    public TimeSpan ShardLockDuration;
    public TimeSpan ShardLockExpireAt;
    public float SpeedBoost;
    public DamageSpecifier ShardDamage;
    public float ShedRadius;
    public EntProtoId Projectile;
    public int ProjectileCount;
    public int? ProjectileHitLimit;
    public SoundSpecifier Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoSpikeShedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoSpikeShedComponent, ComponentGetState>(new ComponentEventRefHandler<XenoSpikeShedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoSpikeShedComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoSpikeShedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoSpikeShedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoSpikeShedComponent.XenoSpikeShedComponent_AutoState()
      {
        MinShards = component.MinShards,
        Cooldown = component.Cooldown,
        CooldownExpireAt = component.CooldownExpireAt,
        ShardLockDuration = component.ShardLockDuration,
        ShardLockExpireAt = component.ShardLockExpireAt,
        SpeedBoost = component.SpeedBoost,
        ShardDamage = component.ShardDamage,
        ShedRadius = component.ShedRadius,
        Projectile = component.Projectile,
        ProjectileCount = component.ProjectileCount,
        ProjectileHitLimit = component.ProjectileHitLimit,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoSpikeShedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoSpikeShedComponent.XenoSpikeShedComponent_AutoState current))
        return;
      component.MinShards = current.MinShards;
      component.Cooldown = current.Cooldown;
      component.CooldownExpireAt = current.CooldownExpireAt;
      component.ShardLockDuration = current.ShardLockDuration;
      component.ShardLockExpireAt = current.ShardLockExpireAt;
      component.SpeedBoost = current.SpeedBoost;
      component.ShardDamage = current.ShardDamage;
      component.ShedRadius = current.ShedRadius;
      component.Projectile = current.Projectile;
      component.ProjectileCount = current.ProjectileCount;
      component.ProjectileHitLimit = current.ProjectileHitLimit;
      component.Sound = current.Sound;
    }
  }
}
