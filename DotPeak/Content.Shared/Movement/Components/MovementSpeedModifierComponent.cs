// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.MovementSpeedModifierComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Systems;
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
namespace Content.Shared.Movement.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (MovementSpeedModifierSystem)})]
public sealed class MovementSpeedModifierComponent : 
  Component,
  ISerializationGenerated<MovementSpeedModifierComponent>,
  ISerializationGenerated
{
  public const float DefaultWeightlessFriction = 1f;
  public const float DefaultWeightlessModifier = 0.7f;
  public const float DefaultWeightlessAcceleration = 1f;
  public const float DefaultAcceleration = 20f;
  public const float DefaultFriction = 2.5f;
  public const float DefaultFrictionNoInput = 2.5f;
  public const float DefaultMinimumFrictionSpeed = 0.005f;
  public const float DefaultBaseWalkSpeed = 2.5f;
  public const float DefaultBaseSprintSpeed = 4.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BaseWalkSpeed = 2.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BaseSprintSpeed = 4.5f;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float BaseAcceleration = 20f;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float BaseFriction = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float MinimumFrictionSpeed = 0.005f;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float Acceleration;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float Friction;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float FrictionNoInput;
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float WalkSpeedModifier = 1f;
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float SprintSpeedModifier = 1f;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float BaseWeightlessFriction = 1f;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float BaseWeightlessModifier = 0.7f;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float BaseWeightlessAcceleration = 1f;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float WeightlessAcceleration;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float WeightlessModifier;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float WeightlessFriction;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float WeightlessFrictionNoInput;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public float? OffGridFriction;

  [Robust.Shared.ViewVariables.ViewVariables]
  public float CurrentWalkSpeed => this.WalkSpeedModifier * this.BaseWalkSpeed;

  [Robust.Shared.ViewVariables.ViewVariables]
  public float CurrentSprintSpeed => this.SprintSpeedModifier * this.BaseSprintSpeed;

  [Robust.Shared.ViewVariables.ViewVariables]
  public float WeightlessWalkSpeed => this.WeightlessModifier * this.BaseWalkSpeed;

  [Robust.Shared.ViewVariables.ViewVariables]
  public float WeightlessSprintSpeed => this.WeightlessModifier * this.BaseSprintSpeed;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MovementSpeedModifierComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MovementSpeedModifierComponent) target1;
    if (serialization.TryCustomCopy<MovementSpeedModifierComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseWalkSpeed, ref target2, hookCtx, false, context))
      target2 = this.BaseWalkSpeed;
    target.BaseWalkSpeed = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseSprintSpeed, ref target3, hookCtx, false, context))
      target3 = this.BaseSprintSpeed;
    target.BaseSprintSpeed = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseAcceleration, ref target4, hookCtx, false, context))
      target4 = this.BaseAcceleration;
    target.BaseAcceleration = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseFriction, ref target5, hookCtx, false, context))
      target5 = this.BaseFriction;
    target.BaseFriction = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinimumFrictionSpeed, ref target6, hookCtx, false, context))
      target6 = this.MinimumFrictionSpeed;
    target.MinimumFrictionSpeed = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Acceleration, ref target7, hookCtx, false, context))
      target7 = this.Acceleration;
    target.Acceleration = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Friction, ref target8, hookCtx, false, context))
      target8 = this.Friction;
    target.Friction = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FrictionNoInput, ref target9, hookCtx, false, context))
      target9 = this.FrictionNoInput;
    target.FrictionNoInput = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseWeightlessFriction, ref target10, hookCtx, false, context))
      target10 = this.BaseWeightlessFriction;
    target.BaseWeightlessFriction = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseWeightlessModifier, ref target11, hookCtx, false, context))
      target11 = this.BaseWeightlessModifier;
    target.BaseWeightlessModifier = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseWeightlessAcceleration, ref target12, hookCtx, false, context))
      target12 = this.BaseWeightlessAcceleration;
    target.BaseWeightlessAcceleration = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeightlessAcceleration, ref target13, hookCtx, false, context))
      target13 = this.WeightlessAcceleration;
    target.WeightlessAcceleration = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeightlessModifier, ref target14, hookCtx, false, context))
      target14 = this.WeightlessModifier;
    target.WeightlessModifier = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeightlessFriction, ref target15, hookCtx, false, context))
      target15 = this.WeightlessFriction;
    target.WeightlessFriction = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeightlessFrictionNoInput, ref target16, hookCtx, false, context))
      target16 = this.WeightlessFrictionNoInput;
    target.WeightlessFrictionNoInput = target16;
    float? target17 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.OffGridFriction, ref target17, hookCtx, false, context))
      target17 = this.OffGridFriction;
    target.OffGridFriction = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MovementSpeedModifierComponent target,
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
    MovementSpeedModifierComponent target1 = (MovementSpeedModifierComponent) target;
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
    MovementSpeedModifierComponent target1 = (MovementSpeedModifierComponent) target;
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
    MovementSpeedModifierComponent target1 = (MovementSpeedModifierComponent) target;
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
  virtual MovementSpeedModifierComponent Component.Instantiate()
  {
    return new MovementSpeedModifierComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MovementSpeedModifierComponent_AutoState : IComponentState
  {
    public float BaseWalkSpeed;
    public float BaseSprintSpeed;
    public float BaseAcceleration;
    public float BaseFriction;
    public float Acceleration;
    public float Friction;
    public float FrictionNoInput;
    public float WalkSpeedModifier;
    public float SprintSpeedModifier;
    public float BaseWeightlessFriction;
    public float BaseWeightlessModifier;
    public float BaseWeightlessAcceleration;
    public float WeightlessAcceleration;
    public float WeightlessModifier;
    public float WeightlessFriction;
    public float WeightlessFrictionNoInput;
    public float? OffGridFriction;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MovementSpeedModifierComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MovementSpeedModifierComponent, ComponentGetState>(new ComponentEventRefHandler<MovementSpeedModifierComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MovementSpeedModifierComponent, ComponentHandleState>(new ComponentEventRefHandler<MovementSpeedModifierComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MovementSpeedModifierComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MovementSpeedModifierComponent.MovementSpeedModifierComponent_AutoState()
      {
        BaseWalkSpeed = component.BaseWalkSpeed,
        BaseSprintSpeed = component.BaseSprintSpeed,
        BaseAcceleration = component.BaseAcceleration,
        BaseFriction = component.BaseFriction,
        Acceleration = component.Acceleration,
        Friction = component.Friction,
        FrictionNoInput = component.FrictionNoInput,
        WalkSpeedModifier = component.WalkSpeedModifier,
        SprintSpeedModifier = component.SprintSpeedModifier,
        BaseWeightlessFriction = component.BaseWeightlessFriction,
        BaseWeightlessModifier = component.BaseWeightlessModifier,
        BaseWeightlessAcceleration = component.BaseWeightlessAcceleration,
        WeightlessAcceleration = component.WeightlessAcceleration,
        WeightlessModifier = component.WeightlessModifier,
        WeightlessFriction = component.WeightlessFriction,
        WeightlessFrictionNoInput = component.WeightlessFrictionNoInput,
        OffGridFriction = component.OffGridFriction
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MovementSpeedModifierComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MovementSpeedModifierComponent.MovementSpeedModifierComponent_AutoState current))
        return;
      component.BaseWalkSpeed = current.BaseWalkSpeed;
      component.BaseSprintSpeed = current.BaseSprintSpeed;
      component.BaseAcceleration = current.BaseAcceleration;
      component.BaseFriction = current.BaseFriction;
      component.Acceleration = current.Acceleration;
      component.Friction = current.Friction;
      component.FrictionNoInput = current.FrictionNoInput;
      component.WalkSpeedModifier = current.WalkSpeedModifier;
      component.SprintSpeedModifier = current.SprintSpeedModifier;
      component.BaseWeightlessFriction = current.BaseWeightlessFriction;
      component.BaseWeightlessModifier = current.BaseWeightlessModifier;
      component.BaseWeightlessAcceleration = current.BaseWeightlessAcceleration;
      component.WeightlessAcceleration = current.WeightlessAcceleration;
      component.WeightlessModifier = current.WeightlessModifier;
      component.WeightlessFriction = current.WeightlessFriction;
      component.WeightlessFrictionNoInput = current.WeightlessFrictionNoInput;
      component.OffGridFriction = current.OffGridFriction;
    }
  }
}
