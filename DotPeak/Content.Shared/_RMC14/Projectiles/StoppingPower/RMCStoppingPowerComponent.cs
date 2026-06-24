// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.StoppingPower.RMCStoppingPowerComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Projectiles.StoppingPower;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCStoppingPowerComponent : 
  Component,
  ISerializationGenerated<RMCStoppingPowerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CurrentStoppingPower;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxStoppingPower = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int StoppingThreshold = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BigXenoScreenShakeThreshold = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BigXenoInterruptThreshold = 4;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double StoppingPowerDivider = 30.0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float XenoStunMultiplier = 0.3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BigXenoStunTime = TimeSpan.FromSeconds(0.7);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public MapCoordinates? ShotFrom;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RequiresAimedShot = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FocusedCounter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? FocusedCounterThreshold = new int?(2);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCStoppingPowerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCStoppingPowerComponent) target1;
    if (serialization.TryCustomCopy<RMCStoppingPowerComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CurrentStoppingPower, ref target2, hookCtx, false, context))
      target2 = this.CurrentStoppingPower;
    target.CurrentStoppingPower = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxStoppingPower, ref target3, hookCtx, false, context))
      target3 = this.MaxStoppingPower;
    target.MaxStoppingPower = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.StoppingThreshold, ref target4, hookCtx, false, context))
      target4 = this.StoppingThreshold;
    target.StoppingThreshold = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.BigXenoScreenShakeThreshold, ref target5, hookCtx, false, context))
      target5 = this.BigXenoScreenShakeThreshold;
    target.BigXenoScreenShakeThreshold = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.BigXenoInterruptThreshold, ref target6, hookCtx, false, context))
      target6 = this.BigXenoInterruptThreshold;
    target.BigXenoInterruptThreshold = target6;
    double target7 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.StoppingPowerDivider, ref target7, hookCtx, false, context))
      target7 = this.StoppingPowerDivider;
    target.StoppingPowerDivider = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.XenoStunMultiplier, ref target8, hookCtx, false, context))
      target8 = this.XenoStunMultiplier;
    target.XenoStunMultiplier = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BigXenoStunTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.BigXenoStunTime, hookCtx, context);
    target.BigXenoStunTime = target9;
    MapCoordinates? target10 = new MapCoordinates?();
    if (!serialization.TryCustomCopy<MapCoordinates?>(this.ShotFrom, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<MapCoordinates?>(this.ShotFrom, hookCtx, context);
    target.ShotFrom = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequiresAimedShot, ref target11, hookCtx, false, context))
      target11 = this.RequiresAimedShot;
    target.RequiresAimedShot = target11;
    int target12 = 0;
    if (!serialization.TryCustomCopy<int>(this.FocusedCounter, ref target12, hookCtx, false, context))
      target12 = this.FocusedCounter;
    target.FocusedCounter = target12;
    int? target13 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.FocusedCounterThreshold, ref target13, hookCtx, false, context))
      target13 = this.FocusedCounterThreshold;
    target.FocusedCounterThreshold = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCStoppingPowerComponent target,
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
    RMCStoppingPowerComponent target1 = (RMCStoppingPowerComponent) target;
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
    RMCStoppingPowerComponent target1 = (RMCStoppingPowerComponent) target;
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
    RMCStoppingPowerComponent target1 = (RMCStoppingPowerComponent) target;
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
  virtual RMCStoppingPowerComponent Component.Instantiate() => new RMCStoppingPowerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCStoppingPowerComponent_AutoState : IComponentState
  {
    public float CurrentStoppingPower;
    public int MaxStoppingPower;
    public int StoppingThreshold;
    public int BigXenoScreenShakeThreshold;
    public int BigXenoInterruptThreshold;
    public double StoppingPowerDivider;
    public float XenoStunMultiplier;
    public TimeSpan BigXenoStunTime;
    public MapCoordinates? ShotFrom;
    public bool RequiresAimedShot;
    public int FocusedCounter;
    public int? FocusedCounterThreshold;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCStoppingPowerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCStoppingPowerComponent, ComponentGetState>(new ComponentEventRefHandler<RMCStoppingPowerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCStoppingPowerComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCStoppingPowerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCStoppingPowerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCStoppingPowerComponent.RMCStoppingPowerComponent_AutoState()
      {
        CurrentStoppingPower = component.CurrentStoppingPower,
        MaxStoppingPower = component.MaxStoppingPower,
        StoppingThreshold = component.StoppingThreshold,
        BigXenoScreenShakeThreshold = component.BigXenoScreenShakeThreshold,
        BigXenoInterruptThreshold = component.BigXenoInterruptThreshold,
        StoppingPowerDivider = component.StoppingPowerDivider,
        XenoStunMultiplier = component.XenoStunMultiplier,
        BigXenoStunTime = component.BigXenoStunTime,
        ShotFrom = component.ShotFrom,
        RequiresAimedShot = component.RequiresAimedShot,
        FocusedCounter = component.FocusedCounter,
        FocusedCounterThreshold = component.FocusedCounterThreshold
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCStoppingPowerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCStoppingPowerComponent.RMCStoppingPowerComponent_AutoState current))
        return;
      component.CurrentStoppingPower = current.CurrentStoppingPower;
      component.MaxStoppingPower = current.MaxStoppingPower;
      component.StoppingThreshold = current.StoppingThreshold;
      component.BigXenoScreenShakeThreshold = current.BigXenoScreenShakeThreshold;
      component.BigXenoInterruptThreshold = current.BigXenoInterruptThreshold;
      component.StoppingPowerDivider = current.StoppingPowerDivider;
      component.XenoStunMultiplier = current.XenoStunMultiplier;
      component.BigXenoStunTime = current.BigXenoStunTime;
      component.ShotFrom = current.ShotFrom;
      component.RequiresAimedShot = current.RequiresAimedShot;
      component.FocusedCounter = current.FocusedCounter;
      component.FocusedCounterThreshold = current.FocusedCounterThreshold;
    }
  }
}
