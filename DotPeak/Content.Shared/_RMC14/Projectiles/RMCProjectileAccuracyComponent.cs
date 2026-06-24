// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.RMCProjectileAccuracyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Projectiles;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCProjectileSystem), typeof (CMGunSystem)})]
public sealed class RMCProjectileAccuracyComponent : 
  Component,
  ISerializationGenerated<RMCProjectileAccuracyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<AccuracyFalloffThreshold> Thresholds = new List<AccuracyFalloffThreshold>()
  {
    new AccuracyFalloffThreshold(5f, (FixedPoint2) 10, false)
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MinAccuracy = (FixedPoint2) 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Accuracy = (FixedPoint2) 90;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IgnoreFriendlyEvasion;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ForceHit = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates? ShotFrom;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public long GunSeed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public uint Tick;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCProjectileAccuracyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCProjectileAccuracyComponent) target1;
    if (serialization.TryCustomCopy<RMCProjectileAccuracyComponent>(this, ref target, hookCtx, false, context))
      return;
    List<AccuracyFalloffThreshold> target2 = (List<AccuracyFalloffThreshold>) null;
    if (this.Thresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<AccuracyFalloffThreshold>>(this.Thresholds, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<AccuracyFalloffThreshold>>(this.Thresholds, hookCtx, context);
    target.Thresholds = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MinAccuracy, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.MinAccuracy, hookCtx, context);
    target.MinAccuracy = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Accuracy, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.Accuracy, hookCtx, context);
    target.Accuracy = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreFriendlyEvasion, ref target5, hookCtx, false, context))
      target5 = this.IgnoreFriendlyEvasion;
    target.IgnoreFriendlyEvasion = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.ForceHit, ref target6, hookCtx, false, context))
      target6 = this.ForceHit;
    target.ForceHit = target6;
    EntityCoordinates? target7 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.ShotFrom, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityCoordinates?>(this.ShotFrom, hookCtx, context);
    target.ShotFrom = target7;
    long target8 = 0;
    if (!serialization.TryCustomCopy<long>(this.GunSeed, ref target8, hookCtx, false, context))
      target8 = this.GunSeed;
    target.GunSeed = target8;
    uint target9 = 0;
    if (!serialization.TryCustomCopy<uint>(this.Tick, ref target9, hookCtx, false, context))
      target9 = this.Tick;
    target.Tick = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCProjectileAccuracyComponent target,
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
    RMCProjectileAccuracyComponent target1 = (RMCProjectileAccuracyComponent) target;
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
    RMCProjectileAccuracyComponent target1 = (RMCProjectileAccuracyComponent) target;
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
    RMCProjectileAccuracyComponent target1 = (RMCProjectileAccuracyComponent) target;
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
  virtual RMCProjectileAccuracyComponent Component.Instantiate()
  {
    return new RMCProjectileAccuracyComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCProjectileAccuracyComponent_AutoState : IComponentState
  {
    public List<AccuracyFalloffThreshold> Thresholds;
    public FixedPoint2 MinAccuracy;
    public FixedPoint2 Accuracy;
    public bool IgnoreFriendlyEvasion;
    public bool ForceHit;
    public NetCoordinates? ShotFrom;
    public long GunSeed;
    public uint Tick;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCProjectileAccuracyComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCProjectileAccuracyComponent, ComponentGetState>(new ComponentEventRefHandler<RMCProjectileAccuracyComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCProjectileAccuracyComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCProjectileAccuracyComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCProjectileAccuracyComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCProjectileAccuracyComponent.RMCProjectileAccuracyComponent_AutoState()
      {
        Thresholds = component.Thresholds,
        MinAccuracy = component.MinAccuracy,
        Accuracy = component.Accuracy,
        IgnoreFriendlyEvasion = component.IgnoreFriendlyEvasion,
        ForceHit = component.ForceHit,
        ShotFrom = this.GetNetCoordinates(component.ShotFrom),
        GunSeed = component.GunSeed,
        Tick = component.Tick
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCProjectileAccuracyComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCProjectileAccuracyComponent.RMCProjectileAccuracyComponent_AutoState current))
        return;
      component.Thresholds = current.Thresholds == null ? (List<AccuracyFalloffThreshold>) null : new List<AccuracyFalloffThreshold>((IEnumerable<AccuracyFalloffThreshold>) current.Thresholds);
      component.MinAccuracy = current.MinAccuracy;
      component.Accuracy = current.Accuracy;
      component.IgnoreFriendlyEvasion = current.IgnoreFriendlyEvasion;
      component.ForceHit = current.ForceHit;
      component.ShotFrom = this.EnsureCoordinates<RMCProjectileAccuracyComponent>(current.ShotFrom, uid);
      component.GunSeed = current.GunSeed;
      component.Tick = current.Tick;
    }
  }
}
