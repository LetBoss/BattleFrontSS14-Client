// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Components.ProjectileStunAdjustComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ProjectileStunAdjustComponent : 
  Component,
  ISerializationGenerated<ProjectileStunAdjustComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StunDurationAdjustment = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DazeDurationAdjustment = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxRangeAdjustment = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ForceKnockBackAdjustment;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float KnockBackPowerMinAdjustment = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float KnockBackPowerMaxAdjustment = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool LosesEffectWithRangeAdjustment;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SlowsEffectBigXenosAdjustment;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SuperSlowTimeAdjustment = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SlowTimeAdjustment = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StunAreaAdjustment = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ProjectileStunAdjustComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ProjectileStunAdjustComponent) target1;
    if (serialization.TryCustomCopy<ProjectileStunAdjustComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunDurationAdjustment, ref target2, hookCtx, false, context))
      target2 = this.StunDurationAdjustment;
    target.StunDurationAdjustment = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DazeDurationAdjustment, ref target3, hookCtx, false, context))
      target3 = this.DazeDurationAdjustment;
    target.DazeDurationAdjustment = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxRangeAdjustment, ref target4, hookCtx, false, context))
      target4 = this.MaxRangeAdjustment;
    target.MaxRangeAdjustment = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ForceKnockBackAdjustment, ref target5, hookCtx, false, context))
      target5 = this.ForceKnockBackAdjustment;
    target.ForceKnockBackAdjustment = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.KnockBackPowerMinAdjustment, ref target6, hookCtx, false, context))
      target6 = this.KnockBackPowerMinAdjustment;
    target.KnockBackPowerMinAdjustment = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.KnockBackPowerMaxAdjustment, ref target7, hookCtx, false, context))
      target7 = this.KnockBackPowerMaxAdjustment;
    target.KnockBackPowerMaxAdjustment = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.LosesEffectWithRangeAdjustment, ref target8, hookCtx, false, context))
      target8 = this.LosesEffectWithRangeAdjustment;
    target.LosesEffectWithRangeAdjustment = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.SlowsEffectBigXenosAdjustment, ref target9, hookCtx, false, context))
      target9 = this.SlowsEffectBigXenosAdjustment;
    target.SlowsEffectBigXenosAdjustment = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SuperSlowTimeAdjustment, ref target10, hookCtx, false, context))
      target10 = this.SuperSlowTimeAdjustment;
    target.SuperSlowTimeAdjustment = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlowTimeAdjustment, ref target11, hookCtx, false, context))
      target11 = this.SlowTimeAdjustment;
    target.SlowTimeAdjustment = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunAreaAdjustment, ref target12, hookCtx, false, context))
      target12 = this.StunAreaAdjustment;
    target.StunAreaAdjustment = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ProjectileStunAdjustComponent target,
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
    ProjectileStunAdjustComponent target1 = (ProjectileStunAdjustComponent) target;
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
    ProjectileStunAdjustComponent target1 = (ProjectileStunAdjustComponent) target;
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
    ProjectileStunAdjustComponent target1 = (ProjectileStunAdjustComponent) target;
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
  virtual ProjectileStunAdjustComponent Component.Instantiate()
  {
    return new ProjectileStunAdjustComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ProjectileStunAdjustComponent_AutoState : IComponentState
  {
    public float StunDurationAdjustment;
    public float DazeDurationAdjustment;
    public float MaxRangeAdjustment;
    public bool ForceKnockBackAdjustment;
    public float KnockBackPowerMinAdjustment;
    public float KnockBackPowerMaxAdjustment;
    public bool LosesEffectWithRangeAdjustment;
    public bool SlowsEffectBigXenosAdjustment;
    public float SuperSlowTimeAdjustment;
    public float SlowTimeAdjustment;
    public float StunAreaAdjustment;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ProjectileStunAdjustComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ProjectileStunAdjustComponent, ComponentGetState>(new ComponentEventRefHandler<ProjectileStunAdjustComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ProjectileStunAdjustComponent, ComponentHandleState>(new ComponentEventRefHandler<ProjectileStunAdjustComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ProjectileStunAdjustComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ProjectileStunAdjustComponent.ProjectileStunAdjustComponent_AutoState()
      {
        StunDurationAdjustment = component.StunDurationAdjustment,
        DazeDurationAdjustment = component.DazeDurationAdjustment,
        MaxRangeAdjustment = component.MaxRangeAdjustment,
        ForceKnockBackAdjustment = component.ForceKnockBackAdjustment,
        KnockBackPowerMinAdjustment = component.KnockBackPowerMinAdjustment,
        KnockBackPowerMaxAdjustment = component.KnockBackPowerMaxAdjustment,
        LosesEffectWithRangeAdjustment = component.LosesEffectWithRangeAdjustment,
        SlowsEffectBigXenosAdjustment = component.SlowsEffectBigXenosAdjustment,
        SuperSlowTimeAdjustment = component.SuperSlowTimeAdjustment,
        SlowTimeAdjustment = component.SlowTimeAdjustment,
        StunAreaAdjustment = component.StunAreaAdjustment
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ProjectileStunAdjustComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ProjectileStunAdjustComponent.ProjectileStunAdjustComponent_AutoState current))
        return;
      component.StunDurationAdjustment = current.StunDurationAdjustment;
      component.DazeDurationAdjustment = current.DazeDurationAdjustment;
      component.MaxRangeAdjustment = current.MaxRangeAdjustment;
      component.ForceKnockBackAdjustment = current.ForceKnockBackAdjustment;
      component.KnockBackPowerMinAdjustment = current.KnockBackPowerMinAdjustment;
      component.KnockBackPowerMaxAdjustment = current.KnockBackPowerMaxAdjustment;
      component.LosesEffectWithRangeAdjustment = current.LosesEffectWithRangeAdjustment;
      component.SlowsEffectBigXenosAdjustment = current.SlowsEffectBigXenosAdjustment;
      component.SuperSlowTimeAdjustment = current.SuperSlowTimeAdjustment;
      component.SlowTimeAdjustment = current.SlowTimeAdjustment;
      component.StunAreaAdjustment = current.StunAreaAdjustment;
    }
  }
}
