// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Leap.XenoLeapComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Physics;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Leap;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoLeapSystem), typeof (SharedXenoParasiteSystem)})]
public sealed class XenoLeapComponent : 
  Component,
  ISerializationGenerated<XenoLeapComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = FixedPoint2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Range = (FixedPoint2) 6;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan KnockdownTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? LeapSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Strength = 20;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool KnockdownRequiresInvisibility;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MoveDelayTime = TimeSpan.FromSeconds(0.7);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UnrootOnMelee;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DestroyObjects;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? HitEffect;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TargetJitterTime = TimeSpan.FromSeconds(0L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TargetCameraShakeStrength;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup IgnoredCollisionGroupLarge = CollisionGroup.MidImpassable | CollisionGroup.BarricadeImpassable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup IgnoredCollisionGroupSmall = CollisionGroup.BarricadeImpassable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? LastHit;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? LastHitAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float LastHitRange = 10f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoLeapComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoLeapComponent) target1;
    if (serialization.TryCustomCopy<XenoLeapComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Range, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.Range, hookCtx, context);
    target.Range = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.KnockdownTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.KnockdownTime, hookCtx, context);
    target.KnockdownTime = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LeapSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.LeapSound, hookCtx, context);
    target.LeapSound = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.Strength, ref target7, hookCtx, false, context))
      target7 = this.Strength;
    target.Strength = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.KnockdownRequiresInvisibility, ref target8, hookCtx, false, context))
      target8 = this.KnockdownRequiresInvisibility;
    target.KnockdownRequiresInvisibility = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MoveDelayTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.MoveDelayTime, hookCtx, context);
    target.MoveDelayTime = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.UnrootOnMelee, ref target10, hookCtx, false, context))
      target10 = this.UnrootOnMelee;
    target.UnrootOnMelee = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.DestroyObjects, ref target11, hookCtx, false, context))
      target11 = this.DestroyObjects;
    target.DestroyObjects = target11;
    DamageSpecifier target12 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target12, hookCtx, false, context))
    {
      if (this.Damage == null)
        target12 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target12, hookCtx, context, true);
    }
    target.Damage = target12;
    EntProtoId? target13 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.HitEffect, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId?>(this.HitEffect, hookCtx, context);
    target.HitEffect = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TargetJitterTime, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.TargetJitterTime, hookCtx, context);
    target.TargetJitterTime = target14;
    int target15 = 0;
    if (!serialization.TryCustomCopy<int>(this.TargetCameraShakeStrength, ref target15, hookCtx, false, context))
      target15 = this.TargetCameraShakeStrength;
    target.TargetCameraShakeStrength = target15;
    CollisionGroup target16 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.IgnoredCollisionGroupLarge, ref target16, hookCtx, false, context))
      target16 = this.IgnoredCollisionGroupLarge;
    target.IgnoredCollisionGroupLarge = target16;
    CollisionGroup target17 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.IgnoredCollisionGroupSmall, ref target17, hookCtx, false, context))
      target17 = this.IgnoredCollisionGroupSmall;
    target.IgnoredCollisionGroupSmall = target17;
    EntityUid? target18 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.LastHit, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<EntityUid?>(this.LastHit, hookCtx, context);
    target.LastHit = target18;
    TimeSpan? target19 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastHitAt, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<TimeSpan?>(this.LastHitAt, hookCtx, context);
    target.LastHitAt = target19;
    float target20 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LastHitRange, ref target20, hookCtx, false, context))
      target20 = this.LastHitRange;
    target.LastHitRange = target20;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoLeapComponent target,
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
    XenoLeapComponent target1 = (XenoLeapComponent) target;
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
    XenoLeapComponent target1 = (XenoLeapComponent) target;
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
    XenoLeapComponent target1 = (XenoLeapComponent) target;
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
  virtual XenoLeapComponent Component.Instantiate() => new XenoLeapComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoLeapComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoLeapComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoLeapComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoLeapComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.LastHitAt.HasValue)
        component.LastHitAt = new TimeSpan?(component.LastHitAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoLeapComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public TimeSpan Delay;
    public FixedPoint2 Range;
    public TimeSpan KnockdownTime;
    public 
    #nullable enable
    SoundSpecifier? LeapSound;
    public int Strength;
    public bool KnockdownRequiresInvisibility;
    public TimeSpan MoveDelayTime;
    public bool UnrootOnMelee;
    public bool DestroyObjects;
    public DamageSpecifier Damage;
    public EntProtoId? HitEffect;
    public TimeSpan TargetJitterTime;
    public int TargetCameraShakeStrength;
    public CollisionGroup IgnoredCollisionGroupLarge;
    public CollisionGroup IgnoredCollisionGroupSmall;
    public NetEntity? LastHit;
    public TimeSpan? LastHitAt;
    public float LastHitRange;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoLeapComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoLeapComponent, ComponentGetState>(new ComponentEventRefHandler<XenoLeapComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoLeapComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoLeapComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, XenoLeapComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoLeapComponent.XenoLeapComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        Delay = component.Delay,
        Range = component.Range,
        KnockdownTime = component.KnockdownTime,
        LeapSound = component.LeapSound,
        Strength = component.Strength,
        KnockdownRequiresInvisibility = component.KnockdownRequiresInvisibility,
        MoveDelayTime = component.MoveDelayTime,
        UnrootOnMelee = component.UnrootOnMelee,
        DestroyObjects = component.DestroyObjects,
        Damage = component.Damage,
        HitEffect = component.HitEffect,
        TargetJitterTime = component.TargetJitterTime,
        TargetCameraShakeStrength = component.TargetCameraShakeStrength,
        IgnoredCollisionGroupLarge = component.IgnoredCollisionGroupLarge,
        IgnoredCollisionGroupSmall = component.IgnoredCollisionGroupSmall,
        LastHit = this.GetNetEntity(component.LastHit),
        LastHitAt = component.LastHitAt,
        LastHitRange = component.LastHitRange
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoLeapComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoLeapComponent.XenoLeapComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.Delay = current.Delay;
      component.Range = current.Range;
      component.KnockdownTime = current.KnockdownTime;
      component.LeapSound = current.LeapSound;
      component.Strength = current.Strength;
      component.KnockdownRequiresInvisibility = current.KnockdownRequiresInvisibility;
      component.MoveDelayTime = current.MoveDelayTime;
      component.UnrootOnMelee = current.UnrootOnMelee;
      component.DestroyObjects = current.DestroyObjects;
      component.Damage = current.Damage;
      component.HitEffect = current.HitEffect;
      component.TargetJitterTime = current.TargetJitterTime;
      component.TargetCameraShakeStrength = current.TargetCameraShakeStrength;
      component.IgnoredCollisionGroupLarge = current.IgnoredCollisionGroupLarge;
      component.IgnoredCollisionGroupSmall = current.IgnoredCollisionGroupSmall;
      component.LastHit = this.EnsureEntity<XenoLeapComponent>(current.LastHit, uid);
      component.LastHitAt = current.LastHitAt;
      component.LastHitRange = current.LastHitRange;
    }
  }
}
