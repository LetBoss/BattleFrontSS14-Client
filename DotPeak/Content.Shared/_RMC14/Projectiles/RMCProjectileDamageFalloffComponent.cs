// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.RMCProjectileDamageFalloffComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Weapons.Ranged;
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
[Access(new Type[] {typeof (RMCProjectileSystem), typeof (PubgGunRangeSystem)})]
public sealed class RMCProjectileDamageFalloffComponent : 
  Component,
  ISerializationGenerated<RMCProjectileDamageFalloffComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<DamageFalloffThreshold> Thresholds = new List<DamageFalloffThreshold>()
  {
    new DamageFalloffThreshold(0.0f, (FixedPoint2) 1, false),
    new DamageFalloffThreshold(22f, (FixedPoint2) 9999, true)
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MinRemainingDamageMult = (FixedPoint2) 0.05f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 WeaponMult = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates? ShotFrom;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCProjectileDamageFalloffComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCProjectileDamageFalloffComponent) target1;
    if (serialization.TryCustomCopy<RMCProjectileDamageFalloffComponent>(this, ref target, hookCtx, false, context))
      return;
    List<DamageFalloffThreshold> target2 = (List<DamageFalloffThreshold>) null;
    if (this.Thresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<DamageFalloffThreshold>>(this.Thresholds, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<DamageFalloffThreshold>>(this.Thresholds, hookCtx, context);
    target.Thresholds = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MinRemainingDamageMult, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.MinRemainingDamageMult, hookCtx, context);
    target.MinRemainingDamageMult = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.WeaponMult, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.WeaponMult, hookCtx, context);
    target.WeaponMult = target4;
    EntityCoordinates? target5 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.ShotFrom, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityCoordinates?>(this.ShotFrom, hookCtx, context);
    target.ShotFrom = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCProjectileDamageFalloffComponent target,
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
    RMCProjectileDamageFalloffComponent target1 = (RMCProjectileDamageFalloffComponent) target;
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
    RMCProjectileDamageFalloffComponent target1 = (RMCProjectileDamageFalloffComponent) target;
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
    RMCProjectileDamageFalloffComponent target1 = (RMCProjectileDamageFalloffComponent) target;
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
  virtual RMCProjectileDamageFalloffComponent Component.Instantiate()
  {
    return new RMCProjectileDamageFalloffComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCProjectileDamageFalloffComponent_AutoState : IComponentState
  {
    public List<DamageFalloffThreshold> Thresholds;
    public FixedPoint2 MinRemainingDamageMult;
    public FixedPoint2 WeaponMult;
    public NetCoordinates? ShotFrom;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCProjectileDamageFalloffComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCProjectileDamageFalloffComponent, ComponentGetState>(new ComponentEventRefHandler<RMCProjectileDamageFalloffComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCProjectileDamageFalloffComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCProjectileDamageFalloffComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCProjectileDamageFalloffComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCProjectileDamageFalloffComponent.RMCProjectileDamageFalloffComponent_AutoState()
      {
        Thresholds = component.Thresholds,
        MinRemainingDamageMult = component.MinRemainingDamageMult,
        WeaponMult = component.WeaponMult,
        ShotFrom = this.GetNetCoordinates(component.ShotFrom)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCProjectileDamageFalloffComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCProjectileDamageFalloffComponent.RMCProjectileDamageFalloffComponent_AutoState current))
        return;
      component.Thresholds = current.Thresholds == null ? (List<DamageFalloffThreshold>) null : new List<DamageFalloffThreshold>((IEnumerable<DamageFalloffThreshold>) current.Thresholds);
      component.MinRemainingDamageMult = current.MinRemainingDamageMult;
      component.WeaponMult = current.WeaponMult;
      component.ShotFrom = this.EnsureCoordinates<RMCProjectileDamageFalloffComponent>(current.ShotFrom, uid);
    }
  }
}
