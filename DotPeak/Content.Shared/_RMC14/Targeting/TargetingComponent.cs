// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Targeting.TargetingComponent
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
namespace Content.Shared._RMC14.Targeting;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCTargetingSystem)})]
public sealed class TargetingComponent : 
  Component,
  ISerializationGenerated<TargetingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Source;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> Targets = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid User;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates Origin;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntityUid, List<float>> LaserDurations = new Dictionary<EntityUid, List<float>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntityUid, List<float>> OriginalLaserDurations = new Dictionary<EntityUid, List<float>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TargetedEffects LaserType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DirectionTargetedEffects DirectionEffect;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TargetingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TargetingComponent) target1;
    if (serialization.TryCustomCopy<TargetingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Source, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.Source, hookCtx, context);
    target.Source = target2;
    List<EntityUid> target3 = (List<EntityUid>) null;
    if (this.Targets == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Targets, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<EntityUid>>(this.Targets, hookCtx, context);
    target.Targets = target3;
    EntityUid target4 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.User, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid>(this.User, hookCtx, context);
    target.User = target4;
    EntityCoordinates target5 = new EntityCoordinates();
    if (!serialization.TryCustomCopy<EntityCoordinates>(this.Origin, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityCoordinates>(this.Origin, hookCtx, context);
    target.Origin = target5;
    Dictionary<EntityUid, List<float>> target6 = (Dictionary<EntityUid, List<float>>) null;
    if (this.LaserDurations == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntityUid, List<float>>>(this.LaserDurations, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<EntityUid, List<float>>>(this.LaserDurations, hookCtx, context);
    target.LaserDurations = target6;
    Dictionary<EntityUid, List<float>> target7 = (Dictionary<EntityUid, List<float>>) null;
    if (this.OriginalLaserDurations == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntityUid, List<float>>>(this.OriginalLaserDurations, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<Dictionary<EntityUid, List<float>>>(this.OriginalLaserDurations, hookCtx, context);
    target.OriginalLaserDurations = target7;
    TargetedEffects target8 = TargetedEffects.None;
    if (!serialization.TryCustomCopy<TargetedEffects>(this.LaserType, ref target8, hookCtx, false, context))
      target8 = this.LaserType;
    target.LaserType = target8;
    DirectionTargetedEffects target9 = DirectionTargetedEffects.None;
    if (!serialization.TryCustomCopy<DirectionTargetedEffects>(this.DirectionEffect, ref target9, hookCtx, false, context))
      target9 = this.DirectionEffect;
    target.DirectionEffect = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TargetingComponent target,
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
    TargetingComponent target1 = (TargetingComponent) target;
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
    TargetingComponent target1 = (TargetingComponent) target;
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
    TargetingComponent target1 = (TargetingComponent) target;
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
  virtual TargetingComponent Component.Instantiate() => new TargetingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TargetingComponent_AutoState : IComponentState
  {
    public NetEntity Source;
    public List<NetEntity> Targets;
    public NetEntity User;
    public NetCoordinates Origin;
    public Dictionary<NetEntity, List<float>> LaserDurations;
    public Dictionary<NetEntity, List<float>> OriginalLaserDurations;
    public TargetedEffects LaserType;
    public DirectionTargetedEffects DirectionEffect;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TargetingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TargetingComponent, ComponentGetState>(new ComponentEventRefHandler<TargetingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TargetingComponent, ComponentHandleState>(new ComponentEventRefHandler<TargetingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TargetingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TargetingComponent.TargetingComponent_AutoState()
      {
        Source = this.GetNetEntity(component.Source),
        Targets = this.GetNetEntityList(component.Targets),
        User = this.GetNetEntity(component.User),
        Origin = this.GetNetCoordinates(component.Origin),
        LaserDurations = this.GetNetEntityDictionary<List<float>>(component.LaserDurations),
        OriginalLaserDurations = this.GetNetEntityDictionary<List<float>>(component.OriginalLaserDurations),
        LaserType = component.LaserType,
        DirectionEffect = component.DirectionEffect
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TargetingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TargetingComponent.TargetingComponent_AutoState current))
        return;
      component.Source = this.EnsureEntity<TargetingComponent>(current.Source, uid);
      this.EnsureEntityList<TargetingComponent>(current.Targets, uid, component.Targets);
      component.User = this.EnsureEntity<TargetingComponent>(current.User, uid);
      component.Origin = this.EnsureCoordinates<TargetingComponent>(current.Origin, uid);
      this.EnsureEntityDictionary<TargetingComponent, List<float>>(current.LaserDurations, uid, component.LaserDurations);
      this.EnsureEntityDictionary<TargetingComponent, List<float>>(current.OriginalLaserDurations, uid, component.OriginalLaserDurations);
      component.LaserType = current.LaserType;
      component.DirectionEffect = current.DirectionEffect;
    }
  }
}
