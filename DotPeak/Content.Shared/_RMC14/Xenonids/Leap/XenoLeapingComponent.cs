// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Leap.XenoLeapingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Physics;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
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
[Access(new Type[] {typeof (XenoLeapSystem)})]
public sealed class XenoLeapingComponent : 
  Component,
  ISerializationGenerated<XenoLeapingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates Origin;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ParalyzeTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? LeapSound;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LeapEndTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MoveDelayTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool KnockedDown;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool PlayedSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool KnockdownRequiresInvisibility;
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
  public TimeSpan TargetJitterTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TargetCameraShakeStrength;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup IgnoredCollisionGroupLarge;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup IgnoredCollisionGroupSmall;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoLeapingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoLeapingComponent) target1;
    if (serialization.TryCustomCopy<XenoLeapingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityCoordinates target2 = new EntityCoordinates();
    if (!serialization.TryCustomCopy<EntityCoordinates>(this.Origin, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityCoordinates>(this.Origin, hookCtx, context);
    target.Origin = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ParalyzeTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.ParalyzeTime, hookCtx, context);
    target.ParalyzeTime = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LeapSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.LeapSound, hookCtx, context);
    target.LeapSound = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LeapEndTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.LeapEndTime, hookCtx, context);
    target.LeapEndTime = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MoveDelayTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.MoveDelayTime, hookCtx, context);
    target.MoveDelayTime = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.KnockedDown, ref target7, hookCtx, false, context))
      target7 = this.KnockedDown;
    target.KnockedDown = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.PlayedSound, ref target8, hookCtx, false, context))
      target8 = this.PlayedSound;
    target.PlayedSound = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.KnockdownRequiresInvisibility, ref target9, hookCtx, false, context))
      target9 = this.KnockdownRequiresInvisibility;
    target.KnockdownRequiresInvisibility = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.DestroyObjects, ref target10, hookCtx, false, context))
      target10 = this.DestroyObjects;
    target.DestroyObjects = target10;
    DamageSpecifier target11 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target11, hookCtx, false, context))
    {
      if (this.Damage == null)
        target11 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target11, hookCtx, context, true);
    }
    target.Damage = target11;
    EntProtoId? target12 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.HitEffect, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntProtoId?>(this.HitEffect, hookCtx, context);
    target.HitEffect = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TargetJitterTime, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.TargetJitterTime, hookCtx, context);
    target.TargetJitterTime = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.TargetCameraShakeStrength, ref target14, hookCtx, false, context))
      target14 = this.TargetCameraShakeStrength;
    target.TargetCameraShakeStrength = target14;
    CollisionGroup target15 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.IgnoredCollisionGroupLarge, ref target15, hookCtx, false, context))
      target15 = this.IgnoredCollisionGroupLarge;
    target.IgnoredCollisionGroupLarge = target15;
    CollisionGroup target16 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.IgnoredCollisionGroupSmall, ref target16, hookCtx, false, context))
      target16 = this.IgnoredCollisionGroupSmall;
    target.IgnoredCollisionGroupSmall = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoLeapingComponent target,
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
    XenoLeapingComponent target1 = (XenoLeapingComponent) target;
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
    XenoLeapingComponent target1 = (XenoLeapingComponent) target;
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
    XenoLeapingComponent target1 = (XenoLeapingComponent) target;
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
  virtual XenoLeapingComponent Component.Instantiate() => new XenoLeapingComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoLeapingComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoLeapingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoLeapingComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoLeapingComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LeapEndTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoLeapingComponent_AutoState : IComponentState
  {
    public NetCoordinates Origin;
    public TimeSpan ParalyzeTime;
    public 
    #nullable enable
    SoundSpecifier? LeapSound;
    public TimeSpan LeapEndTime;
    public TimeSpan MoveDelayTime;
    public bool KnockedDown;
    public bool PlayedSound;
    public bool KnockdownRequiresInvisibility;
    public bool DestroyObjects;
    public DamageSpecifier Damage;
    public EntProtoId? HitEffect;
    public TimeSpan TargetJitterTime;
    public int TargetCameraShakeStrength;
    public CollisionGroup IgnoredCollisionGroupLarge;
    public CollisionGroup IgnoredCollisionGroupSmall;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoLeapingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoLeapingComponent, ComponentGetState>(new ComponentEventRefHandler<XenoLeapingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoLeapingComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoLeapingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoLeapingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoLeapingComponent.XenoLeapingComponent_AutoState()
      {
        Origin = this.GetNetCoordinates(component.Origin),
        ParalyzeTime = component.ParalyzeTime,
        LeapSound = component.LeapSound,
        LeapEndTime = component.LeapEndTime,
        MoveDelayTime = component.MoveDelayTime,
        KnockedDown = component.KnockedDown,
        PlayedSound = component.PlayedSound,
        KnockdownRequiresInvisibility = component.KnockdownRequiresInvisibility,
        DestroyObjects = component.DestroyObjects,
        Damage = component.Damage,
        HitEffect = component.HitEffect,
        TargetJitterTime = component.TargetJitterTime,
        TargetCameraShakeStrength = component.TargetCameraShakeStrength,
        IgnoredCollisionGroupLarge = component.IgnoredCollisionGroupLarge,
        IgnoredCollisionGroupSmall = component.IgnoredCollisionGroupSmall
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoLeapingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoLeapingComponent.XenoLeapingComponent_AutoState current))
        return;
      component.Origin = this.EnsureCoordinates<XenoLeapingComponent>(current.Origin, uid);
      component.ParalyzeTime = current.ParalyzeTime;
      component.LeapSound = current.LeapSound;
      component.LeapEndTime = current.LeapEndTime;
      component.MoveDelayTime = current.MoveDelayTime;
      component.KnockedDown = current.KnockedDown;
      component.PlayedSound = current.PlayedSound;
      component.KnockdownRequiresInvisibility = current.KnockdownRequiresInvisibility;
      component.DestroyObjects = current.DestroyObjects;
      component.Damage = current.Damage;
      component.HitEffect = current.HitEffect;
      component.TargetJitterTime = current.TargetJitterTime;
      component.TargetCameraShakeStrength = current.TargetCameraShakeStrength;
      component.IgnoredCollisionGroupLarge = current.IgnoredCollisionGroupLarge;
      component.IgnoredCollisionGroupSmall = current.IgnoredCollisionGroupSmall;
    }
  }
}
