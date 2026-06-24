// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.Penetration.RMCPenetratingProjectileComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Projectiles.Penetration;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCPenetratingProjectileComponent : 
  Component,
  ISerializationGenerated<RMCPenetratingProjectileComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 32f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates? ShotFrom;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> HitTargets = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RangeLossPerHit = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DamageMultiplierLossPerHit = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WallMultiplier = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BigXenoMultiplier = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ThickMembraneMultiplier = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MembraneMultiplier = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCPenetratingProjectileComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCPenetratingProjectileComponent) target1;
    if (serialization.TryCustomCopy<RMCPenetratingProjectileComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    EntityCoordinates? target3 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.ShotFrom, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityCoordinates?>(this.ShotFrom, hookCtx, context);
    target.ShotFrom = target3;
    List<EntityUid> target4 = (List<EntityUid>) null;
    if (this.HitTargets == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.HitTargets, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntityUid>>(this.HitTargets, hookCtx, context);
    target.HitTargets = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RangeLossPerHit, ref target5, hookCtx, false, context))
      target5 = this.RangeLossPerHit;
    target.RangeLossPerHit = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageMultiplierLossPerHit, ref target6, hookCtx, false, context))
      target6 = this.DamageMultiplierLossPerHit;
    target.DamageMultiplierLossPerHit = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WallMultiplier, ref target7, hookCtx, false, context))
      target7 = this.WallMultiplier;
    target.WallMultiplier = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BigXenoMultiplier, ref target8, hookCtx, false, context))
      target8 = this.BigXenoMultiplier;
    target.BigXenoMultiplier = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThickMembraneMultiplier, ref target9, hookCtx, false, context))
      target9 = this.ThickMembraneMultiplier;
    target.ThickMembraneMultiplier = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MembraneMultiplier, ref target10, hookCtx, false, context))
      target10 = this.MembraneMultiplier;
    target.MembraneMultiplier = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCPenetratingProjectileComponent target,
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
    RMCPenetratingProjectileComponent target1 = (RMCPenetratingProjectileComponent) target;
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
    RMCPenetratingProjectileComponent target1 = (RMCPenetratingProjectileComponent) target;
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
    RMCPenetratingProjectileComponent target1 = (RMCPenetratingProjectileComponent) target;
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
  virtual RMCPenetratingProjectileComponent Component.Instantiate()
  {
    return new RMCPenetratingProjectileComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCPenetratingProjectileComponent_AutoState : IComponentState
  {
    public float Range;
    public NetCoordinates? ShotFrom;
    public List<NetEntity> HitTargets;
    public float RangeLossPerHit;
    public float DamageMultiplierLossPerHit;
    public float WallMultiplier;
    public float BigXenoMultiplier;
    public float ThickMembraneMultiplier;
    public float MembraneMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCPenetratingProjectileComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCPenetratingProjectileComponent, ComponentGetState>(new ComponentEventRefHandler<RMCPenetratingProjectileComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCPenetratingProjectileComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCPenetratingProjectileComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCPenetratingProjectileComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCPenetratingProjectileComponent.RMCPenetratingProjectileComponent_AutoState()
      {
        Range = component.Range,
        ShotFrom = this.GetNetCoordinates(component.ShotFrom),
        HitTargets = this.GetNetEntityList(component.HitTargets),
        RangeLossPerHit = component.RangeLossPerHit,
        DamageMultiplierLossPerHit = component.DamageMultiplierLossPerHit,
        WallMultiplier = component.WallMultiplier,
        BigXenoMultiplier = component.BigXenoMultiplier,
        ThickMembraneMultiplier = component.ThickMembraneMultiplier,
        MembraneMultiplier = component.MembraneMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCPenetratingProjectileComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCPenetratingProjectileComponent.RMCPenetratingProjectileComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.ShotFrom = this.EnsureCoordinates<RMCPenetratingProjectileComponent>(current.ShotFrom, uid);
      this.EnsureEntityList<RMCPenetratingProjectileComponent>(current.HitTargets, uid, component.HitTargets);
      component.RangeLossPerHit = current.RangeLossPerHit;
      component.DamageMultiplierLossPerHit = current.DamageMultiplierLossPerHit;
      component.WallMultiplier = current.WallMultiplier;
      component.BigXenoMultiplier = current.BigXenoMultiplier;
      component.ThickMembraneMultiplier = current.ThickMembraneMultiplier;
      component.MembraneMultiplier = current.MembraneMultiplier;
    }
  }
}
