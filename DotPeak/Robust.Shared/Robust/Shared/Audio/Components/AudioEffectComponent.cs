// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Components.AudioEffectComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Effects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Audio.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedAudioSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class AudioEffectComponent : 
  Robust.Shared.GameObjects.Component,
  IAudioEffect,
  ISerializationGenerated<AudioEffectComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  internal IAudioEffect Effect = (IAudioEffect) new DummyAudioEffect();

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Density
  {
    get => this.Effect.Density;
    set => this.Effect.Density = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Diffusion
  {
    get => this.Effect.Diffusion;
    set => this.Effect.Diffusion = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Gain
  {
    get => this.Effect.Gain;
    set => this.Effect.Gain = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float GainHF
  {
    get => this.Effect.GainHF;
    set => this.Effect.GainHF = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float GainLF
  {
    get => this.Effect.GainLF;
    set => this.Effect.GainLF = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DecayTime
  {
    get => this.Effect.DecayTime;
    set => this.Effect.DecayTime = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DecayHFRatio
  {
    get => this.Effect.DecayHFRatio;
    set => this.Effect.DecayHFRatio = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DecayLFRatio
  {
    get => this.Effect.DecayLFRatio;
    set => this.Effect.DecayLFRatio = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ReflectionsGain
  {
    get => this.Effect.ReflectionsGain;
    set => this.Effect.ReflectionsGain = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ReflectionsDelay
  {
    get => this.Effect.ReflectionsDelay;
    set => this.Effect.ReflectionsDelay = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector3 ReflectionsPan
  {
    get => this.Effect.ReflectionsPan;
    set => this.Effect.ReflectionsPan = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float LateReverbGain
  {
    get => this.Effect.LateReverbGain;
    set => this.Effect.LateReverbGain = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float LateReverbDelay
  {
    get => this.Effect.LateReverbDelay;
    set => this.Effect.LateReverbDelay = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector3 LateReverbPan
  {
    get => this.Effect.LateReverbPan;
    set => this.Effect.LateReverbPan = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float EchoTime
  {
    get => this.Effect.EchoTime;
    set => this.Effect.EchoTime = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float EchoDepth
  {
    get => this.Effect.EchoDepth;
    set => this.Effect.EchoDepth = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ModulationTime
  {
    get => this.Effect.ModulationTime;
    set => this.Effect.ModulationTime = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ModulationDepth
  {
    get => this.Effect.ModulationDepth;
    set => this.Effect.ModulationDepth = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AirAbsorptionGainHF
  {
    get => this.Effect.AirAbsorptionGainHF;
    set => this.Effect.AirAbsorptionGainHF = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float HFReference
  {
    get => this.Effect.HFReference;
    set => this.Effect.HFReference = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float LFReference
  {
    get => this.Effect.LFReference;
    set => this.Effect.LFReference = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RoomRolloffFactor
  {
    get => this.Effect.RoomRolloffFactor;
    set => this.Effect.RoomRolloffFactor = value;
  }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int DecayHFLimit
  {
    get => this.Effect.DecayHFLimit;
    set => this.Effect.DecayHFLimit = value;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AudioEffectComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Robust.Shared.GameObjects.Component target1 = (Robust.Shared.GameObjects.Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AudioEffectComponent) target1;
    if (serialization.TryCustomCopy<AudioEffectComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Density, ref target2, hookCtx, false, context))
      target2 = this.Density;
    target.Density = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Diffusion, ref target3, hookCtx, false, context))
      target3 = this.Diffusion;
    target.Diffusion = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Gain, ref target4, hookCtx, false, context))
      target4 = this.Gain;
    target.Gain = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.GainHF, ref target5, hookCtx, false, context))
      target5 = this.GainHF;
    target.GainHF = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.GainLF, ref target6, hookCtx, false, context))
      target6 = this.GainLF;
    target.GainLF = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DecayTime, ref target7, hookCtx, false, context))
      target7 = this.DecayTime;
    target.DecayTime = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DecayHFRatio, ref target8, hookCtx, false, context))
      target8 = this.DecayHFRatio;
    target.DecayHFRatio = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DecayLFRatio, ref target9, hookCtx, false, context))
      target9 = this.DecayLFRatio;
    target.DecayLFRatio = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReflectionsGain, ref target10, hookCtx, false, context))
      target10 = this.ReflectionsGain;
    target.ReflectionsGain = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReflectionsDelay, ref target11, hookCtx, false, context))
      target11 = this.ReflectionsDelay;
    target.ReflectionsDelay = target11;
    Vector3 target12 = new Vector3();
    if (!serialization.TryCustomCopy<Vector3>(this.ReflectionsPan, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<Vector3>(this.ReflectionsPan, hookCtx, context);
    target.ReflectionsPan = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LateReverbGain, ref target13, hookCtx, false, context))
      target13 = this.LateReverbGain;
    target.LateReverbGain = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LateReverbDelay, ref target14, hookCtx, false, context))
      target14 = this.LateReverbDelay;
    target.LateReverbDelay = target14;
    Vector3 target15 = new Vector3();
    if (!serialization.TryCustomCopy<Vector3>(this.LateReverbPan, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<Vector3>(this.LateReverbPan, hookCtx, context);
    target.LateReverbPan = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EchoTime, ref target16, hookCtx, false, context))
      target16 = this.EchoTime;
    target.EchoTime = target16;
    float target17 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EchoDepth, ref target17, hookCtx, false, context))
      target17 = this.EchoDepth;
    target.EchoDepth = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ModulationTime, ref target18, hookCtx, false, context))
      target18 = this.ModulationTime;
    target.ModulationTime = target18;
    float target19 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ModulationDepth, ref target19, hookCtx, false, context))
      target19 = this.ModulationDepth;
    target.ModulationDepth = target19;
    float target20 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AirAbsorptionGainHF, ref target20, hookCtx, false, context))
      target20 = this.AirAbsorptionGainHF;
    target.AirAbsorptionGainHF = target20;
    float target21 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HFReference, ref target21, hookCtx, false, context))
      target21 = this.HFReference;
    target.HFReference = target21;
    float target22 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LFReference, ref target22, hookCtx, false, context))
      target22 = this.LFReference;
    target.LFReference = target22;
    float target23 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RoomRolloffFactor, ref target23, hookCtx, false, context))
      target23 = this.RoomRolloffFactor;
    target.RoomRolloffFactor = target23;
    int target24 = 0;
    if (!serialization.TryCustomCopy<int>(this.DecayHFLimit, ref target24, hookCtx, false, context))
      target24 = this.DecayHFLimit;
    target.DecayHFLimit = target24;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AudioEffectComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Robust.Shared.GameObjects.Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AudioEffectComponent target1 = (AudioEffectComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Robust.Shared.GameObjects.Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AudioEffectComponent target1 = (AudioEffectComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref Robust.Shared.GameObjects.IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AudioEffectComponent target1 = (AudioEffectComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Robust.Shared.GameObjects.IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Robust.Shared.GameObjects.IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AudioEffectComponent Robust.Shared.GameObjects.Component.Instantiate()
  {
    return new AudioEffectComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AudioEffectComponent_AutoState : IComponentState
  {
    public float Density;
    public float Diffusion;
    public float Gain;
    public float GainHF;
    public float GainLF;
    public float DecayTime;
    public float DecayHFRatio;
    public float DecayLFRatio;
    public float ReflectionsGain;
    public float ReflectionsDelay;
    public Vector3 ReflectionsPan;
    public float LateReverbGain;
    public float LateReverbDelay;
    public Vector3 LateReverbPan;
    public float EchoTime;
    public float EchoDepth;
    public float ModulationTime;
    public float ModulationDepth;
    public float AirAbsorptionGainHF;
    public float HFReference;
    public float LFReference;
    public float RoomRolloffFactor;
    public int DecayHFLimit;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AudioEffectComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AudioEffectComponent, ComponentGetState>(new ComponentEventRefHandler<AudioEffectComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AudioEffectComponent, ComponentHandleState>(new ComponentEventRefHandler<AudioEffectComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AudioEffectComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AudioEffectComponent.AudioEffectComponent_AutoState()
      {
        Density = component.Density,
        Diffusion = component.Diffusion,
        Gain = component.Gain,
        GainHF = component.GainHF,
        GainLF = component.GainLF,
        DecayTime = component.DecayTime,
        DecayHFRatio = component.DecayHFRatio,
        DecayLFRatio = component.DecayLFRatio,
        ReflectionsGain = component.ReflectionsGain,
        ReflectionsDelay = component.ReflectionsDelay,
        ReflectionsPan = component.ReflectionsPan,
        LateReverbGain = component.LateReverbGain,
        LateReverbDelay = component.LateReverbDelay,
        LateReverbPan = component.LateReverbPan,
        EchoTime = component.EchoTime,
        EchoDepth = component.EchoDepth,
        ModulationTime = component.ModulationTime,
        ModulationDepth = component.ModulationDepth,
        AirAbsorptionGainHF = component.AirAbsorptionGainHF,
        HFReference = component.HFReference,
        LFReference = component.LFReference,
        RoomRolloffFactor = component.RoomRolloffFactor,
        DecayHFLimit = component.DecayHFLimit
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AudioEffectComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AudioEffectComponent.AudioEffectComponent_AutoState current))
        return;
      component.Density = current.Density;
      component.Diffusion = current.Diffusion;
      component.Gain = current.Gain;
      component.GainHF = current.GainHF;
      component.GainLF = current.GainLF;
      component.DecayTime = current.DecayTime;
      component.DecayHFRatio = current.DecayHFRatio;
      component.DecayLFRatio = current.DecayLFRatio;
      component.ReflectionsGain = current.ReflectionsGain;
      component.ReflectionsDelay = current.ReflectionsDelay;
      component.ReflectionsPan = current.ReflectionsPan;
      component.LateReverbGain = current.LateReverbGain;
      component.LateReverbDelay = current.LateReverbDelay;
      component.LateReverbPan = current.LateReverbPan;
      component.EchoTime = current.EchoTime;
      component.EchoDepth = current.EchoDepth;
      component.ModulationTime = current.ModulationTime;
      component.ModulationDepth = current.ModulationDepth;
      component.AirAbsorptionGainHF = current.AirAbsorptionGainHF;
      component.HFReference = current.HFReference;
      component.LFReference = current.LFReference;
      component.RoomRolloffFactor = current.RoomRolloffFactor;
      component.DecayHFLimit = current.DecayHFLimit;
    }
  }
}
