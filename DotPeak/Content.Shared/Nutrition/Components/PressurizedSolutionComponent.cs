// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.PressurizedSolutionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[RegisterComponent]
[Access(new Type[] {typeof (PressurizedSolutionSystem)})]
public sealed class PressurizedSolutionComponent : 
  Component,
  ISerializationGenerated<PressurizedSolutionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Solution = "drink";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SpraySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/soda_spray.ogg");
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan FizzinessMaxDuration = TimeSpan.FromSeconds(120L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan FizzySettleTime;
  [DataField(null, false, 1, false, false, null)]
  public float FizzinessAddedOnShake = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float FizzinessAddedOnLand = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  public float SprayChanceModOnOpened = -0.01f;
  [DataField(null, false, 1, false, false, null)]
  public float SprayChanceModOnShake = -1f;
  [DataField(null, false, 1, false, false, null)]
  public float SprayChanceModOnLand = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SprayFizzinessThresholdRoll;
  [DataField(null, false, 1, false, false, null)]
  public LocId SprayHolderMessageSelf = (LocId) "pressurized-solution-spray-holder-self";
  [DataField(null, false, 1, false, false, null)]
  public LocId SprayHolderMessageOthers = (LocId) "pressurized-solution-spray-holder-others";
  [DataField(null, false, 1, false, false, null)]
  public LocId SprayGroundMessage = (LocId) "pressurized-solution-spray-ground";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PressurizedSolutionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PressurizedSolutionComponent) target1;
    if (serialization.TryCustomCopy<PressurizedSolutionComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target2, hookCtx, false, context))
      target2 = this.Solution;
    target.Solution = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.SpraySound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SpraySound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.SpraySound, hookCtx, context);
    target.SpraySound = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FizzinessMaxDuration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.FizzinessMaxDuration, hookCtx, context);
    target.FizzinessMaxDuration = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FizzySettleTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.FizzySettleTime, hookCtx, context);
    target.FizzySettleTime = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FizzinessAddedOnShake, ref target6, hookCtx, false, context))
      target6 = this.FizzinessAddedOnShake;
    target.FizzinessAddedOnShake = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FizzinessAddedOnLand, ref target7, hookCtx, false, context))
      target7 = this.FizzinessAddedOnLand;
    target.FizzinessAddedOnLand = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprayChanceModOnOpened, ref target8, hookCtx, false, context))
      target8 = this.SprayChanceModOnOpened;
    target.SprayChanceModOnOpened = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprayChanceModOnShake, ref target9, hookCtx, false, context))
      target9 = this.SprayChanceModOnShake;
    target.SprayChanceModOnShake = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprayChanceModOnLand, ref target10, hookCtx, false, context))
      target10 = this.SprayChanceModOnLand;
    target.SprayChanceModOnLand = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprayFizzinessThresholdRoll, ref target11, hookCtx, false, context))
      target11 = this.SprayFizzinessThresholdRoll;
    target.SprayFizzinessThresholdRoll = target11;
    LocId target12 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.SprayHolderMessageSelf, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<LocId>(this.SprayHolderMessageSelf, hookCtx, context);
    target.SprayHolderMessageSelf = target12;
    LocId target13 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.SprayHolderMessageOthers, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<LocId>(this.SprayHolderMessageOthers, hookCtx, context);
    target.SprayHolderMessageOthers = target13;
    LocId target14 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.SprayGroundMessage, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<LocId>(this.SprayGroundMessage, hookCtx, context);
    target.SprayGroundMessage = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PressurizedSolutionComponent target,
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
    PressurizedSolutionComponent target1 = (PressurizedSolutionComponent) target;
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
    PressurizedSolutionComponent target1 = (PressurizedSolutionComponent) target;
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
    PressurizedSolutionComponent target1 = (PressurizedSolutionComponent) target;
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
  virtual PressurizedSolutionComponent Component.Instantiate()
  {
    return new PressurizedSolutionComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PressurizedSolutionComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PressurizedSolutionComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<PressurizedSolutionComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      PressurizedSolutionComponent component,
      ref EntityUnpausedEvent args)
    {
      component.FizzySettleTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PressurizedSolutionComponent_AutoState : IComponentState
  {
    public TimeSpan FizzySettleTime;
    public float SprayFizzinessThresholdRoll;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PressurizedSolutionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PressurizedSolutionComponent, ComponentGetState>(new ComponentEventRefHandler<PressurizedSolutionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PressurizedSolutionComponent, ComponentHandleState>(new ComponentEventRefHandler<PressurizedSolutionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      PressurizedSolutionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PressurizedSolutionComponent.PressurizedSolutionComponent_AutoState()
      {
        FizzySettleTime = component.FizzySettleTime,
        SprayFizzinessThresholdRoll = component.SprayFizzinessThresholdRoll
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PressurizedSolutionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PressurizedSolutionComponent.PressurizedSolutionComponent_AutoState current))
        return;
      component.FizzySettleTime = current.FizzySettleTime;
      component.SprayFizzinessThresholdRoll = current.SprayFizzinessThresholdRoll;
    }
  }
}
