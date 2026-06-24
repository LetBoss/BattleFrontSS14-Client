// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hedgehog.XenoSpikeShieldComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
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
public sealed class XenoSpikeShieldComponent : 
  Component,
  ISerializationGenerated<XenoSpikeShieldComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ShieldDuration = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? ShieldExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShardCost = 150;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(11L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CooldownExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier SpikeDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpikeRadius = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastProcTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AccumulatedDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Projectile = (EntProtoId) "XenoHedgehogSpikeProjectileSpreadShort";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ProjectileCount = 7;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? ProjectileHitLimit = new int?(6);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ShieldAmount = (FixedPoint2) 500;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId EffectId = (EntProtoId) "RMCEffectShieldBlue";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoSpikeShieldComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoSpikeShieldComponent) target1;
    if (serialization.TryCustomCopy<XenoSpikeShieldComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShieldDuration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ShieldDuration, hookCtx, context);
    target.ShieldDuration = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ShieldExpireAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.ShieldExpireAt, hookCtx, context);
    target.ShieldExpireAt = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShardCost, ref target4, hookCtx, false, context))
      target4 = this.ShardCost;
    target.ShardCost = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CooldownExpireAt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.CooldownExpireAt, hookCtx, context);
    target.CooldownExpireAt = target6;
    DamageSpecifier target7 = (DamageSpecifier) null;
    if (this.SpikeDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.SpikeDamage, ref target7, hookCtx, false, context))
    {
      if (this.SpikeDamage == null)
        target7 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.SpikeDamage, ref target7, hookCtx, context, true);
    }
    target.SpikeDamage = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpikeRadius, ref target8, hookCtx, false, context))
      target8 = this.SpikeRadius;
    target.SpikeRadius = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastProcTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.LastProcTime, hookCtx, context);
    target.LastProcTime = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AccumulatedDamage, ref target10, hookCtx, false, context))
      target10 = this.AccumulatedDamage;
    target.AccumulatedDamage = target10;
    EntProtoId target11 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Projectile, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId>(this.Projectile, hookCtx, context);
    target.Projectile = target11;
    int target12 = 0;
    if (!serialization.TryCustomCopy<int>(this.ProjectileCount, ref target12, hookCtx, false, context))
      target12 = this.ProjectileCount;
    target.ProjectileCount = target12;
    int? target13 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.ProjectileHitLimit, ref target13, hookCtx, false, context))
      target13 = this.ProjectileHitLimit;
    target.ProjectileHitLimit = target13;
    FixedPoint2 target14 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ShieldAmount, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<FixedPoint2>(this.ShieldAmount, hookCtx, context);
    target.ShieldAmount = target14;
    EntProtoId target15 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EffectId, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntProtoId>(this.EffectId, hookCtx, context);
    target.EffectId = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoSpikeShieldComponent target,
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
    XenoSpikeShieldComponent target1 = (XenoSpikeShieldComponent) target;
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
    XenoSpikeShieldComponent target1 = (XenoSpikeShieldComponent) target;
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
    XenoSpikeShieldComponent target1 = (XenoSpikeShieldComponent) target;
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
  virtual XenoSpikeShieldComponent Component.Instantiate() => new XenoSpikeShieldComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoSpikeShieldComponent_AutoState : IComponentState
  {
    public TimeSpan ShieldDuration;
    public TimeSpan? ShieldExpireAt;
    public int ShardCost;
    public TimeSpan Cooldown;
    public TimeSpan CooldownExpireAt;
    public DamageSpecifier SpikeDamage;
    public float SpikeRadius;
    public TimeSpan LastProcTime;
    public float AccumulatedDamage;
    public EntProtoId Projectile;
    public int ProjectileCount;
    public int? ProjectileHitLimit;
    public FixedPoint2 ShieldAmount;
    public EntProtoId EffectId;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoSpikeShieldComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoSpikeShieldComponent, ComponentGetState>(new ComponentEventRefHandler<XenoSpikeShieldComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoSpikeShieldComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoSpikeShieldComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoSpikeShieldComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoSpikeShieldComponent.XenoSpikeShieldComponent_AutoState()
      {
        ShieldDuration = component.ShieldDuration,
        ShieldExpireAt = component.ShieldExpireAt,
        ShardCost = component.ShardCost,
        Cooldown = component.Cooldown,
        CooldownExpireAt = component.CooldownExpireAt,
        SpikeDamage = component.SpikeDamage,
        SpikeRadius = component.SpikeRadius,
        LastProcTime = component.LastProcTime,
        AccumulatedDamage = component.AccumulatedDamage,
        Projectile = component.Projectile,
        ProjectileCount = component.ProjectileCount,
        ProjectileHitLimit = component.ProjectileHitLimit,
        ShieldAmount = component.ShieldAmount,
        EffectId = component.EffectId
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoSpikeShieldComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoSpikeShieldComponent.XenoSpikeShieldComponent_AutoState current))
        return;
      component.ShieldDuration = current.ShieldDuration;
      component.ShieldExpireAt = current.ShieldExpireAt;
      component.ShardCost = current.ShardCost;
      component.Cooldown = current.Cooldown;
      component.CooldownExpireAt = current.CooldownExpireAt;
      component.SpikeDamage = current.SpikeDamage;
      component.SpikeRadius = current.SpikeRadius;
      component.LastProcTime = current.LastProcTime;
      component.AccumulatedDamage = current.AccumulatedDamage;
      component.Projectile = current.Projectile;
      component.ProjectileCount = current.ProjectileCount;
      component.ProjectileHitLimit = current.ProjectileHitLimit;
      component.ShieldAmount = current.ShieldAmount;
      component.EffectId = current.EffectId;
    }
  }
}
