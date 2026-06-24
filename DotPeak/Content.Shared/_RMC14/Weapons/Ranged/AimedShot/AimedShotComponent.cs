// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.AimedShot.AimedShotComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Targeting;
using Content.Shared.Whitelist;
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.AimedShot;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCAimedShotSystem)})]
public sealed class AimedShotComponent : 
  Component,
  ISerializationGenerated<AimedShotComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionAimedShot";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Activated;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier AimingSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/Handling/target_on.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextAimedShot = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AimedShotCooldown = TimeSpan.FromSeconds(2.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Range = 32 /*0x20*/;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MinRange = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> Targets = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? CurrentTarget;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AimDuration = 1.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double AimDistanceDifficulty = 0.05;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist Whitelist = new EntityWhitelist();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ProjectileSpeed = 62;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TargetedEffects TargetEffect = TargetedEffects.Targeted;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DirectionTargetedEffects DirectionTargetEffect = DirectionTargetedEffects.DirectionTargeted;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AimedShotComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AimedShotComponent) target1;
    if (serialization.TryCustomCopy<AimedShotComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Activated, ref target4, hookCtx, false, context))
      target4 = this.Activated;
    target.Activated = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.AimingSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AimingSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.AimingSound, hookCtx, context);
    target.AimingSound = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextAimedShot, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NextAimedShot, hookCtx, context);
    target.NextAimedShot = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AimedShotCooldown, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.AimedShotCooldown, hookCtx, context);
    target.AimedShotCooldown = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target8, hookCtx, false, context))
      target8 = this.Range;
    target.Range = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinRange, ref target9, hookCtx, false, context))
      target9 = this.MinRange;
    target.MinRange = target9;
    List<EntityUid> target10 = (List<EntityUid>) null;
    if (this.Targets == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Targets, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<List<EntityUid>>(this.Targets, hookCtx, context);
    target.Targets = target10;
    EntityUid? target11 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CurrentTarget, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntityUid?>(this.CurrentTarget, hookCtx, context);
    target.CurrentTarget = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AimDuration, ref target12, hookCtx, false, context))
      target12 = this.AimDuration;
    target.AimDuration = target12;
    double target13 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.AimDistanceDifficulty, ref target13, hookCtx, false, context))
      target13 = this.AimDistanceDifficulty;
    target.AimDistanceDifficulty = target13;
    EntityWhitelist target14 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target14, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target14 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target14, hookCtx, context, true);
    }
    target.Whitelist = target14;
    int target15 = 0;
    if (!serialization.TryCustomCopy<int>(this.ProjectileSpeed, ref target15, hookCtx, false, context))
      target15 = this.ProjectileSpeed;
    target.ProjectileSpeed = target15;
    TargetedEffects target16 = TargetedEffects.None;
    if (!serialization.TryCustomCopy<TargetedEffects>(this.TargetEffect, ref target16, hookCtx, false, context))
      target16 = this.TargetEffect;
    target.TargetEffect = target16;
    DirectionTargetedEffects target17 = DirectionTargetedEffects.None;
    if (!serialization.TryCustomCopy<DirectionTargetedEffects>(this.DirectionTargetEffect, ref target17, hookCtx, false, context))
      target17 = this.DirectionTargetEffect;
    target.DirectionTargetEffect = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AimedShotComponent target,
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
    AimedShotComponent target1 = (AimedShotComponent) target;
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
    AimedShotComponent target1 = (AimedShotComponent) target;
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
    AimedShotComponent target1 = (AimedShotComponent) target;
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
  virtual AimedShotComponent Component.Instantiate() => new AimedShotComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AimedShotComponent_AutoState : IComponentState
  {
    public EntProtoId ActionId;
    public NetEntity? Action;
    public bool Activated;
    public SoundSpecifier AimingSound;
    public TimeSpan NextAimedShot;
    public TimeSpan AimedShotCooldown;
    public int Range;
    public int MinRange;
    public List<NetEntity> Targets;
    public NetEntity? CurrentTarget;
    public float AimDuration;
    public double AimDistanceDifficulty;
    public EntityWhitelist Whitelist;
    public int ProjectileSpeed;
    public TargetedEffects TargetEffect;
    public DirectionTargetedEffects DirectionTargetEffect;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AimedShotComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AimedShotComponent, ComponentGetState>(new ComponentEventRefHandler<AimedShotComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AimedShotComponent, ComponentHandleState>(new ComponentEventRefHandler<AimedShotComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AimedShotComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AimedShotComponent.AimedShotComponent_AutoState()
      {
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        Activated = component.Activated,
        AimingSound = component.AimingSound,
        NextAimedShot = component.NextAimedShot,
        AimedShotCooldown = component.AimedShotCooldown,
        Range = component.Range,
        MinRange = component.MinRange,
        Targets = this.GetNetEntityList(component.Targets),
        CurrentTarget = this.GetNetEntity(component.CurrentTarget),
        AimDuration = component.AimDuration,
        AimDistanceDifficulty = component.AimDistanceDifficulty,
        Whitelist = component.Whitelist,
        ProjectileSpeed = component.ProjectileSpeed,
        TargetEffect = component.TargetEffect,
        DirectionTargetEffect = component.DirectionTargetEffect
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AimedShotComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AimedShotComponent.AimedShotComponent_AutoState current))
        return;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<AimedShotComponent>(current.Action, uid);
      component.Activated = current.Activated;
      component.AimingSound = current.AimingSound;
      component.NextAimedShot = current.NextAimedShot;
      component.AimedShotCooldown = current.AimedShotCooldown;
      component.Range = current.Range;
      component.MinRange = current.MinRange;
      this.EnsureEntityList<AimedShotComponent>(current.Targets, uid, component.Targets);
      component.CurrentTarget = this.EnsureEntity<AimedShotComponent>(current.CurrentTarget, uid);
      component.AimDuration = current.AimDuration;
      component.AimDistanceDifficulty = current.AimDistanceDifficulty;
      component.Whitelist = current.Whitelist;
      component.ProjectileSpeed = current.ProjectileSpeed;
      component.TargetEffect = current.TargetEffect;
      component.DirectionTargetEffect = current.DirectionTargetEffect;
    }
  }
}
