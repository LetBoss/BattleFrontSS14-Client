// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Collision.StunHostilesOnStepComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.StatusEffect;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Collision;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoCollisionSystem)})]
public sealed class StunHostilesOnStepComponent : 
  Component,
  ISerializationGenerated<StunHostilesOnStepComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Ratio = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<StatusEffectPrototype> DisableStatus = (ProtoId<StatusEffectPrototype>) "KnockedDown";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StunHostilesOnStepComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StunHostilesOnStepComponent) target1;
    if (serialization.TryCustomCopy<StunHostilesOnStepComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Ratio, ref target3, hookCtx, false, context))
      target3 = this.Ratio;
    target.Ratio = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target5;
    DamageSpecifier target6 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target6, hookCtx, false, context))
    {
      if (this.Damage == null)
        target6 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target6, hookCtx, context, true);
    }
    target.Damage = target6;
    ProtoId<StatusEffectPrototype> target7 = new ProtoId<StatusEffectPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<StatusEffectPrototype>>(this.DisableStatus, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<StatusEffectPrototype>>(this.DisableStatus, hookCtx, context);
    target.DisableStatus = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StunHostilesOnStepComponent target,
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
    StunHostilesOnStepComponent target1 = (StunHostilesOnStepComponent) target;
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
    StunHostilesOnStepComponent target1 = (StunHostilesOnStepComponent) target;
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
    StunHostilesOnStepComponent target1 = (StunHostilesOnStepComponent) target;
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
  virtual StunHostilesOnStepComponent Component.Instantiate() => new StunHostilesOnStepComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StunHostilesOnStepComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public float Ratio;
    public TimeSpan Cooldown;
    public TimeSpan Duration;
    public DamageSpecifier Damage;
    public ProtoId<StatusEffectPrototype> DisableStatus;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StunHostilesOnStepComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StunHostilesOnStepComponent, ComponentGetState>(new ComponentEventRefHandler<StunHostilesOnStepComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StunHostilesOnStepComponent, ComponentHandleState>(new ComponentEventRefHandler<StunHostilesOnStepComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StunHostilesOnStepComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StunHostilesOnStepComponent.StunHostilesOnStepComponent_AutoState()
      {
        Enabled = component.Enabled,
        Ratio = component.Ratio,
        Cooldown = component.Cooldown,
        Duration = component.Duration,
        Damage = component.Damage,
        DisableStatus = component.DisableStatus
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StunHostilesOnStepComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StunHostilesOnStepComponent.StunHostilesOnStepComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Ratio = current.Ratio;
      component.Cooldown = current.Cooldown;
      component.Duration = current.Duration;
      component.Damage = current.Damage;
      component.DisableStatus = current.DisableStatus;
    }
  }
}
