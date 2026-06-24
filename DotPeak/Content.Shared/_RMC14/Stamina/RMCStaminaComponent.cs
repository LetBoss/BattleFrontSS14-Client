// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stamina.RMCStaminaComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
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
namespace Content.Shared._RMC14.Stamina;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCStaminaComponent : 
  Component,
  ISerializationGenerated<RMCStaminaComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double Current = 100.0;
  [DataField(null, false, 1, false, false, null)]
  public double Max = 100.0;
  [DataField(null, false, 1, false, false, null)]
  public int RegenPerTick = 6;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Level;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TimeBetweenChecks = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextRegen;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextCheck;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> StaminaAlert = (ProtoId<AlertPrototype>) "RMCStamina";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowAlert;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RestPeriod = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int[] TierThresholds = new int[5]
  {
    100,
    70,
    30,
    10,
    0
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float[] SpeedModifiers = new float[5]
  {
    0.0f,
    1.5f,
    2.75f,
    3.75f,
    4.5f
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan EffectTime = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DazeTime = TimeSpan.FromSeconds(6L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCStaminaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCStaminaComponent) target1;
    if (serialization.TryCustomCopy<RMCStaminaComponent>(this, ref target, hookCtx, false, context))
      return;
    double target2 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Current, ref target2, hookCtx, false, context))
      target2 = this.Current;
    target.Current = target2;
    double target3 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Max, ref target3, hookCtx, false, context))
      target3 = this.Max;
    target.Max = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.RegenPerTick, ref target4, hookCtx, false, context))
      target4 = this.RegenPerTick;
    target.RegenPerTick = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.Level, ref target5, hookCtx, false, context))
      target5 = this.Level;
    target.Level = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeBetweenChecks, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.TimeBetweenChecks, hookCtx, context);
    target.TimeBetweenChecks = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextRegen, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.NextRegen, hookCtx, context);
    target.NextRegen = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextCheck, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.NextCheck, hookCtx, context);
    target.NextCheck = target8;
    ProtoId<AlertPrototype> target9 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.StaminaAlert, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.StaminaAlert, hookCtx, context);
    target.StaminaAlert = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowAlert, ref target10, hookCtx, false, context))
      target10 = this.ShowAlert;
    target.ShowAlert = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RestPeriod, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.RestPeriod, hookCtx, context);
    target.RestPeriod = target11;
    int[] target12 = (int[]) null;
    if (this.TierThresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<int[]>(this.TierThresholds, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<int[]>(this.TierThresholds, hookCtx, context);
    target.TierThresholds = target12;
    float[] target13 = (float[]) null;
    if (this.SpeedModifiers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.SpeedModifiers, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<float[]>(this.SpeedModifiers, hookCtx, context);
    target.SpeedModifiers = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EffectTime, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.EffectTime, hookCtx, context);
    target.EffectTime = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DazeTime, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.DazeTime, hookCtx, context);
    target.DazeTime = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCStaminaComponent target,
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
    RMCStaminaComponent target1 = (RMCStaminaComponent) target;
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
    RMCStaminaComponent target1 = (RMCStaminaComponent) target;
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
    RMCStaminaComponent target1 = (RMCStaminaComponent) target;
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
  virtual RMCStaminaComponent Component.Instantiate() => new RMCStaminaComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCStaminaComponent_AutoState : IComponentState
  {
    public double Current;
    public int Level;
    public TimeSpan NextRegen;
    public TimeSpan NextCheck;
    public bool ShowAlert;
    public int[] TierThresholds;
    public float[] SpeedModifiers;
    public TimeSpan EffectTime;
    public TimeSpan DazeTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCStaminaComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCStaminaComponent, ComponentGetState>(new ComponentEventRefHandler<RMCStaminaComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCStaminaComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCStaminaComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCStaminaComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCStaminaComponent.RMCStaminaComponent_AutoState()
      {
        Current = component.Current,
        Level = component.Level,
        NextRegen = component.NextRegen,
        NextCheck = component.NextCheck,
        ShowAlert = component.ShowAlert,
        TierThresholds = component.TierThresholds,
        SpeedModifiers = component.SpeedModifiers,
        EffectTime = component.EffectTime,
        DazeTime = component.DazeTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCStaminaComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCStaminaComponent.RMCStaminaComponent_AutoState current))
        return;
      component.Current = current.Current;
      component.Level = current.Level;
      component.NextRegen = current.NextRegen;
      component.NextCheck = current.NextCheck;
      component.ShowAlert = current.ShowAlert;
      component.TierThresholds = current.TierThresholds;
      component.SpeedModifiers = current.SpeedModifiers;
      component.EffectTime = current.EffectTime;
      component.DazeTime = current.DazeTime;
    }
  }
}
