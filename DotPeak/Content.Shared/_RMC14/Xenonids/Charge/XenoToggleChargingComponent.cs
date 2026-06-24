// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Charge.XenoToggleChargingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Charge;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoChargeSystem)})]
public sealed class XenoToggleChargingComponent : 
  Component,
  ISerializationGenerated<XenoToggleChargingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MinimumSteps = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxStage = 8;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StepIncrement = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpeedPerStage = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaPerStep = (FixedPoint2) 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_footstep_charge1.ogg", new AudioParams?(AudioParams.Default.WithVolume(-4f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SoundEvery = 4;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxDeviation = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype>? Emote = (ProtoId<EmotePrototype>?) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? EmoteCooldown = new TimeSpan?(TimeSpan.FromSeconds(20L));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastMovedGrace = TimeSpan.FromSeconds(0.5);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoToggleChargingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoToggleChargingComponent) target1;
    if (serialization.TryCustomCopy<XenoToggleChargingComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinimumSteps, ref target2, hookCtx, false, context))
      target2 = this.MinimumSteps;
    target.MinimumSteps = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxStage, ref target3, hookCtx, false, context))
      target3 = this.MaxStage;
    target.MaxStage = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StepIncrement, ref target4, hookCtx, false, context))
      target4 = this.StepIncrement;
    target.StepIncrement = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedPerStage, ref target5, hookCtx, false, context))
      target5 = this.SpeedPerStage;
    target.SpeedPerStage = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaPerStep, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.PlasmaPerStep, hookCtx, context);
    target.PlasmaPerStep = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.SoundEvery, ref target8, hookCtx, false, context))
      target8 = this.SoundEvery;
    target.SoundEvery = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxDeviation, ref target9, hookCtx, false, context))
      target9 = this.MaxDeviation;
    target.MaxDeviation = target9;
    ProtoId<EmotePrototype>? target10 = new ProtoId<EmotePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>?>(this.Emote, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<EmotePrototype>?>(this.Emote, hookCtx, context);
    target.Emote = target10;
    TimeSpan? target11 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EmoteCooldown, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan?>(this.EmoteCooldown, hookCtx, context);
    target.EmoteCooldown = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastMovedGrace, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.LastMovedGrace, hookCtx, context);
    target.LastMovedGrace = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoToggleChargingComponent target,
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
    XenoToggleChargingComponent target1 = (XenoToggleChargingComponent) target;
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
    XenoToggleChargingComponent target1 = (XenoToggleChargingComponent) target;
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
    XenoToggleChargingComponent target1 = (XenoToggleChargingComponent) target;
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
  virtual XenoToggleChargingComponent Component.Instantiate() => new XenoToggleChargingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoToggleChargingComponent_AutoState : IComponentState
  {
    public float MinimumSteps;
    public int MaxStage;
    public float StepIncrement;
    public float SpeedPerStage;
    public FixedPoint2 PlasmaPerStep;
    public SoundSpecifier? Sound;
    public int SoundEvery;
    public float MaxDeviation;
    public ProtoId<EmotePrototype>? Emote;
    public TimeSpan? EmoteCooldown;
    public TimeSpan LastMovedGrace;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoToggleChargingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoToggleChargingComponent, ComponentGetState>(new ComponentEventRefHandler<XenoToggleChargingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoToggleChargingComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoToggleChargingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoToggleChargingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoToggleChargingComponent.XenoToggleChargingComponent_AutoState()
      {
        MinimumSteps = component.MinimumSteps,
        MaxStage = component.MaxStage,
        StepIncrement = component.StepIncrement,
        SpeedPerStage = component.SpeedPerStage,
        PlasmaPerStep = component.PlasmaPerStep,
        Sound = component.Sound,
        SoundEvery = component.SoundEvery,
        MaxDeviation = component.MaxDeviation,
        Emote = component.Emote,
        EmoteCooldown = component.EmoteCooldown,
        LastMovedGrace = component.LastMovedGrace
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoToggleChargingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoToggleChargingComponent.XenoToggleChargingComponent_AutoState current))
        return;
      component.MinimumSteps = current.MinimumSteps;
      component.MaxStage = current.MaxStage;
      component.StepIncrement = current.StepIncrement;
      component.SpeedPerStage = current.SpeedPerStage;
      component.PlasmaPerStep = current.PlasmaPerStep;
      component.Sound = current.Sound;
      component.SoundEvery = current.SoundEvery;
      component.MaxDeviation = current.MaxDeviation;
      component.Emote = current.Emote;
      component.EmoteCooldown = current.EmoteCooldown;
      component.LastMovedGrace = current.LastMovedGrace;
    }
  }
}
