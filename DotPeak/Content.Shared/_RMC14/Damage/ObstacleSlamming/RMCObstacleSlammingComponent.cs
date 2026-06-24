// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.ObstacleSlamming.RMCObstacleSlammingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Damage.ObstacleSlamming;

[Access(new Type[] {typeof (RMCObstacleSlammingSystem)})]
[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCObstacleSlammingComponent : 
  Component,
  ISerializationGenerated<RMCObstacleSlammingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MobSizeCoefficient = 20f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ThrowSpeedCoefficient = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float KnockbackPower = 0.3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float KnockBackSpeed = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundHit = (SoundSpecifier) new SoundCollectionSpecifier("MetalSlam");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? HitEffect;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DamageCooldown = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  public TimeSpan? LastHit;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCObstacleSlammingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCObstacleSlammingComponent) target1;
    if (serialization.TryCustomCopy<RMCObstacleSlammingComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MobSizeCoefficient, ref target2, hookCtx, false, context))
      target2 = this.MobSizeCoefficient;
    target.MobSizeCoefficient = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThrowSpeedCoefficient, ref target3, hookCtx, false, context))
      target3 = this.ThrowSpeedCoefficient;
    target.ThrowSpeedCoefficient = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.KnockbackPower, ref target4, hookCtx, false, context))
      target4 = this.KnockbackPower;
    target.KnockbackPower = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.KnockBackSpeed, ref target5, hookCtx, false, context))
      target5 = this.KnockBackSpeed;
    target.KnockBackSpeed = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundHit, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.SoundHit, hookCtx, context);
    target.SoundHit = target6;
    EntProtoId? target7 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.HitEffect, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId?>(this.HitEffect, hookCtx, context);
    target.HitEffect = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DamageCooldown, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.DamageCooldown, hookCtx, context);
    target.DamageCooldown = target8;
    TimeSpan? target9 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastHit, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan?>(this.LastHit, hookCtx, context);
    target.LastHit = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCObstacleSlammingComponent target,
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
    RMCObstacleSlammingComponent target1 = (RMCObstacleSlammingComponent) target;
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
    RMCObstacleSlammingComponent target1 = (RMCObstacleSlammingComponent) target;
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
    RMCObstacleSlammingComponent target1 = (RMCObstacleSlammingComponent) target;
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
  virtual RMCObstacleSlammingComponent Component.Instantiate()
  {
    return new RMCObstacleSlammingComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCObstacleSlammingComponent_AutoState : IComponentState
  {
    public float MobSizeCoefficient;
    public float ThrowSpeedCoefficient;
    public float KnockbackPower;
    public float KnockBackSpeed;
    public SoundSpecifier? SoundHit;
    public EntProtoId? HitEffect;
    public TimeSpan DamageCooldown;
    public TimeSpan? LastHit;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCObstacleSlammingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCObstacleSlammingComponent, ComponentGetState>(new ComponentEventRefHandler<RMCObstacleSlammingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCObstacleSlammingComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCObstacleSlammingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCObstacleSlammingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCObstacleSlammingComponent.RMCObstacleSlammingComponent_AutoState()
      {
        MobSizeCoefficient = component.MobSizeCoefficient,
        ThrowSpeedCoefficient = component.ThrowSpeedCoefficient,
        KnockbackPower = component.KnockbackPower,
        KnockBackSpeed = component.KnockBackSpeed,
        SoundHit = component.SoundHit,
        HitEffect = component.HitEffect,
        DamageCooldown = component.DamageCooldown,
        LastHit = component.LastHit
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCObstacleSlammingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCObstacleSlammingComponent.RMCObstacleSlammingComponent_AutoState current))
        return;
      component.MobSizeCoefficient = current.MobSizeCoefficient;
      component.ThrowSpeedCoefficient = current.ThrowSpeedCoefficient;
      component.KnockbackPower = current.KnockbackPower;
      component.KnockBackSpeed = current.KnockBackSpeed;
      component.SoundHit = current.SoundHit;
      component.HitEffect = current.HitEffect;
      component.DamageCooldown = current.DamageCooldown;
      component.LastHit = current.LastHit;
    }
  }
}
