// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.MobMoverComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Movement.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class MobMoverComponent : 
  Component,
  ISerializationGenerated<MobMoverComponent>,
  ISerializationGenerated
{
  private float _stepSoundDistance;
  [DataField(null, false, 1, false, false, null)]
  public float GrabRange = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float PushStrength = 600f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StepSoundMoveDistanceRunning = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StepSoundMoveDistanceWalking = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FootstepVariation;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntityCoordinates LastPosition { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float StepSoundDistance
  {
    get => this._stepSoundDistance;
    set
    {
      if (MathHelper.CloseToPercent(this._stepSoundDistance, value, 1E-05))
        return;
      this._stepSoundDistance = value;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float GrabRangeVV
  {
    get => this.GrabRange;
    set
    {
      if (MathHelper.CloseToPercent(this.GrabRange, value, 1E-05))
        return;
      this.GrabRange = value;
      IoCManager.Resolve<IEntityManager>().Dirty(this.Owner, (IComponent) this);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float PushStrengthVV
  {
    get => this.PushStrength;
    set
    {
      if (MathHelper.CloseToPercent(this.PushStrength, value, 1E-05))
        return;
      this.PushStrength = value;
      IoCManager.Resolve<IEntityManager>().Dirty(this.Owner, (IComponent) this);
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MobMoverComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MobMoverComponent) target1;
    if (serialization.TryCustomCopy<MobMoverComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.GrabRange, ref target2, hookCtx, false, context))
      target2 = this.GrabRange;
    target.GrabRange = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PushStrength, ref target3, hookCtx, false, context))
      target3 = this.PushStrength;
    target.PushStrength = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StepSoundMoveDistanceRunning, ref target4, hookCtx, false, context))
      target4 = this.StepSoundMoveDistanceRunning;
    target.StepSoundMoveDistanceRunning = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StepSoundMoveDistanceWalking, ref target5, hookCtx, false, context))
      target5 = this.StepSoundMoveDistanceWalking;
    target.StepSoundMoveDistanceWalking = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FootstepVariation, ref target6, hookCtx, false, context))
      target6 = this.FootstepVariation;
    target.FootstepVariation = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MobMoverComponent target,
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
    MobMoverComponent target1 = (MobMoverComponent) target;
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
    MobMoverComponent target1 = (MobMoverComponent) target;
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
    MobMoverComponent target1 = (MobMoverComponent) target;
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
  virtual MobMoverComponent Component.Instantiate() => new MobMoverComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MobMoverComponent_AutoState : IComponentState
  {
    public float StepSoundMoveDistanceRunning;
    public float StepSoundMoveDistanceWalking;
    public float FootstepVariation;
    public float GrabRangeVV;
    public float PushStrengthVV;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MobMoverComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MobMoverComponent, ComponentGetState>(new ComponentEventRefHandler<MobMoverComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MobMoverComponent, ComponentHandleState>(new ComponentEventRefHandler<MobMoverComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, MobMoverComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new MobMoverComponent.MobMoverComponent_AutoState()
      {
        StepSoundMoveDistanceRunning = component.StepSoundMoveDistanceRunning,
        StepSoundMoveDistanceWalking = component.StepSoundMoveDistanceWalking,
        FootstepVariation = component.FootstepVariation,
        GrabRangeVV = component.GrabRangeVV,
        PushStrengthVV = component.PushStrengthVV
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MobMoverComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MobMoverComponent.MobMoverComponent_AutoState current))
        return;
      component.StepSoundMoveDistanceRunning = current.StepSoundMoveDistanceRunning;
      component.StepSoundMoveDistanceWalking = current.StepSoundMoveDistanceWalking;
      component.FootstepVariation = current.FootstepVariation;
      component.GrabRangeVV = current.GrabRangeVV;
      component.PushStrengthVV = current.PushStrengthVV;
    }
  }
}
