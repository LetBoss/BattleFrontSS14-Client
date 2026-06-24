// Decompiled with JetBrains decompiler
// Type: Content.Shared.Flash.Components.FlashComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Flash.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedFlashSystem)})]
public sealed class FlashComponent : 
  Component,
  ISerializationGenerated<FlashComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FlashOnUse;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FlashOnMelee;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FlashingTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MeleeDuration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AoeFlashDuration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? MeleeStunDuration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SlowTo;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Probability;

  public FlashComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Weapons/flash.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithVolume(1f).WithMaxDistance(3f);
    this.Sound = (SoundSpecifier) soundPathSpecifier;
    this.Probability = 1f;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FlashComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FlashComponent) target1;
    if (serialization.TryCustomCopy<FlashComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.FlashOnUse, ref target2, hookCtx, false, context))
      target2 = this.FlashOnUse;
    target.FlashOnUse = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.FlashOnMelee, ref target3, hookCtx, false, context))
      target3 = this.FlashOnMelee;
    target.FlashOnMelee = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FlashingTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.FlashingTime, hookCtx, context);
    target.FlashingTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MeleeDuration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.MeleeDuration, hookCtx, context);
    target.MeleeDuration = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AoeFlashDuration, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.AoeFlashDuration, hookCtx, context);
    target.AoeFlashDuration = target6;
    TimeSpan? target7 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.MeleeStunDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan?>(this.MeleeStunDuration, hookCtx, context);
    target.MeleeStunDuration = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target8, hookCtx, false, context))
      target8 = this.Range;
    target.Range = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlowTo, ref target9, hookCtx, false, context))
      target9 = this.SlowTo;
    target.SlowTo = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Probability, ref target11, hookCtx, false, context))
      target11 = this.Probability;
    target.Probability = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FlashComponent target,
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
    FlashComponent target1 = (FlashComponent) target;
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
    FlashComponent target1 = (FlashComponent) target;
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
    FlashComponent target1 = (FlashComponent) target;
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
  virtual FlashComponent Component.Instantiate() => new FlashComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FlashComponent_AutoState : IComponentState
  {
    public bool FlashOnUse;
    public bool FlashOnMelee;
    public TimeSpan FlashingTime;
    public TimeSpan MeleeDuration;
    public TimeSpan AoeFlashDuration;
    public TimeSpan? MeleeStunDuration;
    public float Range;
    public float SlowTo;
    public SoundSpecifier Sound;
    public float Probability;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FlashComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FlashComponent, ComponentGetState>(new ComponentEventRefHandler<FlashComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FlashComponent, ComponentHandleState>(new ComponentEventRefHandler<FlashComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, FlashComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new FlashComponent.FlashComponent_AutoState()
      {
        FlashOnUse = component.FlashOnUse,
        FlashOnMelee = component.FlashOnMelee,
        FlashingTime = component.FlashingTime,
        MeleeDuration = component.MeleeDuration,
        AoeFlashDuration = component.AoeFlashDuration,
        MeleeStunDuration = component.MeleeStunDuration,
        Range = component.Range,
        SlowTo = component.SlowTo,
        Sound = component.Sound,
        Probability = component.Probability
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FlashComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FlashComponent.FlashComponent_AutoState current))
        return;
      component.FlashOnUse = current.FlashOnUse;
      component.FlashOnMelee = current.FlashOnMelee;
      component.FlashingTime = current.FlashingTime;
      component.MeleeDuration = current.MeleeDuration;
      component.AoeFlashDuration = current.AoeFlashDuration;
      component.MeleeStunDuration = current.MeleeStunDuration;
      component.Range = current.Range;
      component.SlowTo = current.SlowTo;
      component.Sound = current.Sound;
      component.Probability = current.Probability;
    }
  }
}
