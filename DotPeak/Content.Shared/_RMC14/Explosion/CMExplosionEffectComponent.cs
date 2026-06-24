// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.CMExplosionEffectComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
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
namespace Content.Shared._RMC14.Explosion;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCExplosionSystem)})]
public sealed class CMExplosionEffectComponent : 
  Component,
  ISerializationGenerated<CMExplosionEffectComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? Explosion = (EntProtoId?) "CMExplosionEffectGrenade";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? ShockWave = (EntProtoId?) "RMCExplosionEffectGrenadeShockWave";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> ShrapnelEffects = new List<EntProtoId>()
  {
    (EntProtoId) "CMExplosionEffectShrapnel1",
    (EntProtoId) "CMExplosionEffectShrapnel2"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MinShrapnel = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxShrapnel = 9;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ShrapnelSpeed = 5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMExplosionEffectComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMExplosionEffectComponent) target1;
    if (serialization.TryCustomCopy<CMExplosionEffectComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Explosion, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.Explosion, hookCtx, context);
    target.Explosion = target2;
    EntProtoId? target3 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.ShockWave, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId?>(this.ShockWave, hookCtx, context);
    target.ShockWave = target3;
    List<EntProtoId> target4 = (List<EntProtoId>) null;
    if (this.ShrapnelEffects == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.ShrapnelEffects, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntProtoId>>(this.ShrapnelEffects, hookCtx, context);
    target.ShrapnelEffects = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinShrapnel, ref target5, hookCtx, false, context))
      target5 = this.MinShrapnel;
    target.MinShrapnel = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxShrapnel, ref target6, hookCtx, false, context))
      target6 = this.MaxShrapnel;
    target.MaxShrapnel = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShrapnelSpeed, ref target7, hookCtx, false, context))
      target7 = this.ShrapnelSpeed;
    target.ShrapnelSpeed = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMExplosionEffectComponent target,
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
    CMExplosionEffectComponent target1 = (CMExplosionEffectComponent) target;
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
    CMExplosionEffectComponent target1 = (CMExplosionEffectComponent) target;
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
    CMExplosionEffectComponent target1 = (CMExplosionEffectComponent) target;
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
  virtual CMExplosionEffectComponent Component.Instantiate() => new CMExplosionEffectComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMExplosionEffectComponent_AutoState : IComponentState
  {
    public EntProtoId? Explosion;
    public EntProtoId? ShockWave;
    public List<EntProtoId> ShrapnelEffects;
    public int MinShrapnel;
    public int MaxShrapnel;
    public float ShrapnelSpeed;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMExplosionEffectComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMExplosionEffectComponent, ComponentGetState>(new ComponentEventRefHandler<CMExplosionEffectComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMExplosionEffectComponent, ComponentHandleState>(new ComponentEventRefHandler<CMExplosionEffectComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMExplosionEffectComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMExplosionEffectComponent.CMExplosionEffectComponent_AutoState()
      {
        Explosion = component.Explosion,
        ShockWave = component.ShockWave,
        ShrapnelEffects = component.ShrapnelEffects,
        MinShrapnel = component.MinShrapnel,
        MaxShrapnel = component.MaxShrapnel,
        ShrapnelSpeed = component.ShrapnelSpeed
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMExplosionEffectComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMExplosionEffectComponent.CMExplosionEffectComponent_AutoState current))
        return;
      component.Explosion = current.Explosion;
      component.ShockWave = current.ShockWave;
      component.ShrapnelEffects = current.ShrapnelEffects == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.ShrapnelEffects);
      component.MinShrapnel = current.MinShrapnel;
      component.MaxShrapnel = current.MaxShrapnel;
      component.ShrapnelSpeed = current.ShrapnelSpeed;
    }
  }
}
