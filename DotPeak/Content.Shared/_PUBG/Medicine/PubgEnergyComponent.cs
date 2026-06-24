// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Medicine.PubgEnergyComponent
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
namespace Content.Shared._PUBG.Medicine;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class PubgEnergyComponent : 
  Component,
  ISerializationGenerated<PubgEnergyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxEnergy = 100f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Energy;
  [DataField(null, false, 1, false, false, null)]
  public float HealPerTick = 2f;
  [DataField(null, false, 1, false, false, null)]
  public float HealInterval = 5f;
  [DataField(null, false, 1, false, false, null)]
  public float EnergyDecayPerTick = 2f;
  [DataField(null, false, 1, false, false, null)]
  public float EnergyDecayInterval = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float SpeedBonusThresholdPercent = 60f;
  [DataField(null, false, 1, false, false, null)]
  public float SpeedBonusMultiplier = 1.1f;
  [DataField(null, false, 1, false, false, null)]
  public float SlowdownReductionThresholdPercent = 50f;
  [DataField(null, false, 1, false, false, null)]
  public float SlowdownReductionFactor = 0.5f;
  public float HealTimer;
  public float DecayTimer;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgEnergyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgEnergyComponent) target1;
    if (serialization.TryCustomCopy<PubgEnergyComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxEnergy, ref target2, hookCtx, false, context))
      target2 = this.MaxEnergy;
    target.MaxEnergy = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Energy, ref target3, hookCtx, false, context))
      target3 = this.Energy;
    target.Energy = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HealPerTick, ref target4, hookCtx, false, context))
      target4 = this.HealPerTick;
    target.HealPerTick = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HealInterval, ref target5, hookCtx, false, context))
      target5 = this.HealInterval;
    target.HealInterval = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnergyDecayPerTick, ref target6, hookCtx, false, context))
      target6 = this.EnergyDecayPerTick;
    target.EnergyDecayPerTick = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnergyDecayInterval, ref target7, hookCtx, false, context))
      target7 = this.EnergyDecayInterval;
    target.EnergyDecayInterval = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedBonusThresholdPercent, ref target8, hookCtx, false, context))
      target8 = this.SpeedBonusThresholdPercent;
    target.SpeedBonusThresholdPercent = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedBonusMultiplier, ref target9, hookCtx, false, context))
      target9 = this.SpeedBonusMultiplier;
    target.SpeedBonusMultiplier = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlowdownReductionThresholdPercent, ref target10, hookCtx, false, context))
      target10 = this.SlowdownReductionThresholdPercent;
    target.SlowdownReductionThresholdPercent = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlowdownReductionFactor, ref target11, hookCtx, false, context))
      target11 = this.SlowdownReductionFactor;
    target.SlowdownReductionFactor = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgEnergyComponent target,
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
    PubgEnergyComponent target1 = (PubgEnergyComponent) target;
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
    PubgEnergyComponent target1 = (PubgEnergyComponent) target;
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
    PubgEnergyComponent target1 = (PubgEnergyComponent) target;
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
  virtual PubgEnergyComponent Component.Instantiate() => new PubgEnergyComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgEnergyComponent_AutoState : IComponentState
  {
    public float MaxEnergy;
    public float Energy;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgEnergyComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgEnergyComponent, ComponentGetState>(new ComponentEventRefHandler<PubgEnergyComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgEnergyComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgEnergyComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgEnergyComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgEnergyComponent.PubgEnergyComponent_AutoState()
      {
        MaxEnergy = component.MaxEnergy,
        Energy = component.Energy
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgEnergyComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgEnergyComponent.PubgEnergyComponent_AutoState current))
        return;
      component.MaxEnergy = current.MaxEnergy;
      component.Energy = current.Energy;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, PubgEnergyComponent>(uid, component, ref args1);
    }
  }
}
