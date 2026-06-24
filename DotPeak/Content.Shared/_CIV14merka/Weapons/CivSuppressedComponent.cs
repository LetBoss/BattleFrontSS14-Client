// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Weapons.CivSuppressedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Weapons;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedCivSuppressionSystem)})]
public sealed class CivSuppressedComponent : 
  Component,
  ISerializationGenerated<CivSuppressedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Intensity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CivSuppressionVisualProfile VisualProfile;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastAppliedAt = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DecayTime = TimeSpan.FromSeconds(2.7999999523162842);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ShotPenaltyDegrees = 4f;
  [DataField(null, false, 1, false, false, null)]
  public float FreshTargetSuppressionMultiplier = 0.6f;
  [DataField(null, false, 1, false, false, null)]
  public float HighStressThreshold = 0.65f;
  [DataField(null, false, 1, false, false, null)]
  public float HighStressShotPenaltyMultiplier = 1.75f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float VisualShockMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float VisualSwayMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan VisualRecoveryDelay = TimeSpan.FromSeconds(0.44999998807907104);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float VisualRingThreshold = 0.72f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float VisualRingVolume = -11f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan VisualRingCooldown = TimeSpan.FromSeconds(1.3500000238418579);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NextNearMissAt = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivSuppressedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivSuppressedComponent) target1;
    if (serialization.TryCustomCopy<CivSuppressedComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Intensity, ref target2, hookCtx, false, context))
      target2 = this.Intensity;
    target.Intensity = target2;
    CivSuppressionVisualProfile target3 = CivSuppressionVisualProfile.IncomingFire;
    if (!serialization.TryCustomCopy<CivSuppressionVisualProfile>(this.VisualProfile, ref target3, hookCtx, false, context))
      target3 = this.VisualProfile;
    target.VisualProfile = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastAppliedAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.LastAppliedAt, hookCtx, context);
    target.LastAppliedAt = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DecayTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.DecayTime, hookCtx, context);
    target.DecayTime = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShotPenaltyDegrees, ref target6, hookCtx, false, context))
      target6 = this.ShotPenaltyDegrees;
    target.ShotPenaltyDegrees = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FreshTargetSuppressionMultiplier, ref target7, hookCtx, false, context))
      target7 = this.FreshTargetSuppressionMultiplier;
    target.FreshTargetSuppressionMultiplier = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HighStressThreshold, ref target8, hookCtx, false, context))
      target8 = this.HighStressThreshold;
    target.HighStressThreshold = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HighStressShotPenaltyMultiplier, ref target9, hookCtx, false, context))
      target9 = this.HighStressShotPenaltyMultiplier;
    target.HighStressShotPenaltyMultiplier = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VisualShockMultiplier, ref target10, hookCtx, false, context))
      target10 = this.VisualShockMultiplier;
    target.VisualShockMultiplier = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VisualSwayMultiplier, ref target11, hookCtx, false, context))
      target11 = this.VisualSwayMultiplier;
    target.VisualSwayMultiplier = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.VisualRecoveryDelay, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.VisualRecoveryDelay, hookCtx, context);
    target.VisualRecoveryDelay = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VisualRingThreshold, ref target13, hookCtx, false, context))
      target13 = this.VisualRingThreshold;
    target.VisualRingThreshold = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VisualRingVolume, ref target14, hookCtx, false, context))
      target14 = this.VisualRingVolume;
    target.VisualRingVolume = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.VisualRingCooldown, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.VisualRingCooldown, hookCtx, context);
    target.VisualRingCooldown = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextNearMissAt, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.NextNearMissAt, hookCtx, context);
    target.NextNearMissAt = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivSuppressedComponent target,
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
    CivSuppressedComponent target1 = (CivSuppressedComponent) target;
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
    CivSuppressedComponent target1 = (CivSuppressedComponent) target;
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
    CivSuppressedComponent target1 = (CivSuppressedComponent) target;
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
  virtual CivSuppressedComponent Component.Instantiate() => new CivSuppressedComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivSuppressedComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivSuppressedComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<CivSuppressedComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      CivSuppressedComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastAppliedAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CivSuppressedComponent_AutoState : IComponentState
  {
    public float Intensity;
    public CivSuppressionVisualProfile VisualProfile;
    public TimeSpan LastAppliedAt;
    public TimeSpan DecayTime;
    public float ShotPenaltyDegrees;
    public float VisualShockMultiplier;
    public float VisualSwayMultiplier;
    public TimeSpan VisualRecoveryDelay;
    public float VisualRingThreshold;
    public float VisualRingVolume;
    public TimeSpan VisualRingCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivSuppressedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivSuppressedComponent, ComponentGetState>(new ComponentEventRefHandler<CivSuppressedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CivSuppressedComponent, ComponentHandleState>(new ComponentEventRefHandler<CivSuppressedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      CivSuppressedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CivSuppressedComponent.CivSuppressedComponent_AutoState()
      {
        Intensity = component.Intensity,
        VisualProfile = component.VisualProfile,
        LastAppliedAt = component.LastAppliedAt,
        DecayTime = component.DecayTime,
        ShotPenaltyDegrees = component.ShotPenaltyDegrees,
        VisualShockMultiplier = component.VisualShockMultiplier,
        VisualSwayMultiplier = component.VisualSwayMultiplier,
        VisualRecoveryDelay = component.VisualRecoveryDelay,
        VisualRingThreshold = component.VisualRingThreshold,
        VisualRingVolume = component.VisualRingVolume,
        VisualRingCooldown = component.VisualRingCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CivSuppressedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CivSuppressedComponent.CivSuppressedComponent_AutoState current))
        return;
      component.Intensity = current.Intensity;
      component.VisualProfile = current.VisualProfile;
      component.LastAppliedAt = current.LastAppliedAt;
      component.DecayTime = current.DecayTime;
      component.ShotPenaltyDegrees = current.ShotPenaltyDegrees;
      component.VisualShockMultiplier = current.VisualShockMultiplier;
      component.VisualSwayMultiplier = current.VisualSwayMultiplier;
      component.VisualRecoveryDelay = current.VisualRecoveryDelay;
      component.VisualRingThreshold = current.VisualRingThreshold;
      component.VisualRingVolume = current.VisualRingVolume;
      component.VisualRingCooldown = current.VisualRingCooldown;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CivSuppressedComponent>(uid, component, ref args1);
    }
  }
}
