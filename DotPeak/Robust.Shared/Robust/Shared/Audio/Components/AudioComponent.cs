// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Components.AudioComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Effects;
using Robust.Shared.Audio.Sources;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Audio.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, true)]
[Access(new Type[] {typeof (SharedAudioSystem)})]
public sealed class AudioComponent : 
  Robust.Shared.GameObjects.Component,
  IAudioSource,
  IDisposable,
  ISerializationGenerated<AudioComponent>,
  ISerializationGenerated,
  IComponentDelta,
  Robust.Shared.GameObjects.IComponent,
  ISerializationGenerated<Robust.Shared.GameObjects.IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  public AudioFlags Flags;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  public TimeSpan AudioStart;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? ExcludedEntity;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<EntityUid>? IncludedEntities;
  public bool Started;
  [AutoNetworkedField]
  [DataField(null, false, 1, true, false, null)]
  public string FileName = string.Empty;
  public bool Loaded;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public AudioParams Params = AudioParams.Default;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  internal IAudioSource Source = (IAudioSource) new DummyAudioSource();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Auxiliary;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AudioState State = AudioState.Playing;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? PauseTime;

  public override bool SessionSpecific => true;

  public void Pause() => this.Source.Pause();

  public void StartPlaying() => this.Source.StartPlaying();

  public void StopPlaying()
  {
    this.PlaybackPosition = 0.0f;
    this.Source.StopPlaying();
  }

  public void Restart() => this.Source.Restart();

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Playing
  {
    get => this.Source.Playing;
    set => this.Source.Playing = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Looping
  {
    get => this.Source.Looping;
    set => this.Source.Looping = value;
  }

  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedAudioSystem)})]
  public bool Global { get; set; }

  public float Pitch
  {
    get => this.Source.Pitch;
    set => this.Source.Pitch = value;
  }

  public float MaxDistance
  {
    get => this.Source.MaxDistance;
    set => this.Source.MaxDistance = value;
  }

  public float RolloffFactor
  {
    get => this.Source.RolloffFactor;
    set => this.Source.RolloffFactor = value;
  }

  public float ReferenceDistance
  {
    get => this.Source.ReferenceDistance;
    set => this.Source.ReferenceDistance = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 Position
  {
    get => this.Source.Position;
    set => this.Source.Position = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  public float Volume
  {
    get => this.Source.Volume;
    set => this.Source.Volume = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  public float Gain
  {
    get => this.Source.Gain;
    set => this.Source.Gain = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  public float Occlusion
  {
    get => this.Source.Occlusion;
    set => this.Source.Occlusion = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  public float PlaybackPosition
  {
    get => this.Source.PlaybackPosition;
    set => this.Source.PlaybackPosition = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 Velocity
  {
    get => this.Source.Velocity;
    set => this.Source.Velocity = value;
  }

  void IAudioSource.SetAuxiliary(IAuxiliaryAudio? audio) => this.Source.SetAuxiliary(audio);

  public void Dispose() => this.Source.Dispose();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AudioComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Robust.Shared.GameObjects.Component target1 = (Robust.Shared.GameObjects.Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AudioComponent) target1;
    if (serialization.TryCustomCopy<AudioComponent>(this, ref target, hookCtx, false, context))
      return;
    AudioFlags target2 = AudioFlags.None;
    if (!serialization.TryCustomCopy<AudioFlags>(this.Flags, ref target2, hookCtx, false, context))
      target2 = this.Flags;
    target.Flags = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AudioStart, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.AudioStart, hookCtx, context);
    target.AudioStart = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ExcludedEntity, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.ExcludedEntity, hookCtx, context);
    target.ExcludedEntity = target4;
    HashSet<EntityUid> target5 = (HashSet<EntityUid>) null;
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.IncludedEntities, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<EntityUid>>(this.IncludedEntities, hookCtx, context);
    target.IncludedEntities = target5;
    string target6 = (string) null;
    if (this.FileName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FileName, ref target6, hookCtx, false, context))
      target6 = this.FileName;
    target.FileName = target6;
    AudioParams target7 = new AudioParams();
    if (!serialization.TryCustomCopy<AudioParams>(this.Params, ref target7, hookCtx, false, context))
      serialization.CopyTo<AudioParams>(this.Params, ref target7, hookCtx, context);
    target.Params = target7;
    EntityUid? target8 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Auxiliary, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntityUid?>(this.Auxiliary, hookCtx, context);
    target.Auxiliary = target8;
    AudioState target9 = AudioState.Stopped;
    if (!serialization.TryCustomCopy<AudioState>(this.State, ref target9, hookCtx, false, context))
      target9 = this.State;
    target.State = target9;
    TimeSpan? target10 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.PauseTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan?>(this.PauseTime, hookCtx, context);
    target.PauseTime = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AudioComponent target,
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
    AudioComponent target1 = (AudioComponent) target;
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
    AudioComponent target1 = (AudioComponent) target;
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
    AudioComponent target1 = (AudioComponent) target;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AudioComponent target1 = (AudioComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AudioComponent Robust.Shared.GameObjects.Component.Instantiate() => new AudioComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AudioComponent_AutoState : IComponentState
  {
    public AudioFlags Flags;
    public TimeSpan AudioStart;
    public string FileName;
    public AudioParams Params;
    public NetEntity? Auxiliary;
    public AudioState State;
    public TimeSpan? PauseTime;
    public bool Global;

    public AudioComponent.AudioComponent_AutoState ShallowClone()
    {
      return new AudioComponent.AudioComponent_AutoState()
      {
        Flags = this.Flags,
        AudioStart = this.AudioStart,
        FileName = this.FileName,
        Params = this.Params,
        Auxiliary = this.Auxiliary,
        State = this.State,
        PauseTime = this.PauseTime,
        Global = this.Global
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AudioComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<AudioComponent>("Flags", "AudioStart", "FileName", "Params", "Auxiliary", "State", "PauseTime", "Global");
      this.SubscribeLocalEvent<AudioComponent, ComponentGetState>(new ComponentEventRefHandler<AudioComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AudioComponent, ComponentHandleState>(new ComponentEventRefHandler<AudioComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, AudioComponent component, ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new AudioComponent.Flags_FieldComponentState()
            {
              Flags = component.Flags
            };
            return;
          case 2:
            args.State = (IComponentState) new AudioComponent.AudioStart_FieldComponentState()
            {
              AudioStart = component.AudioStart
            };
            return;
          case 4:
            args.State = (IComponentState) new AudioComponent.FileName_FieldComponentState()
            {
              FileName = component.FileName
            };
            return;
          case 8:
            args.State = (IComponentState) new AudioComponent.Params_FieldComponentState()
            {
              Params = component.Params
            };
            return;
          case 16 /*0x10*/:
            args.State = (IComponentState) new AudioComponent.Auxiliary_FieldComponentState()
            {
              Auxiliary = this.GetNetEntity(component.Auxiliary)
            };
            return;
          case 32 /*0x20*/:
            args.State = (IComponentState) new AudioComponent.State_FieldComponentState()
            {
              State = component.State
            };
            return;
          case 64 /*0x40*/:
            args.State = (IComponentState) new AudioComponent.PauseTime_FieldComponentState()
            {
              PauseTime = component.PauseTime
            };
            return;
          case 128 /*0x80*/:
            args.State = (IComponentState) new AudioComponent.Global_FieldComponentState()
            {
              Global = component.Global
            };
            return;
        }
      }
      args.State = (IComponentState) new AudioComponent.AudioComponent_AutoState()
      {
        Flags = component.Flags,
        AudioStart = component.AudioStart,
        FileName = component.FileName,
        Params = component.Params,
        Auxiliary = this.GetNetEntity(component.Auxiliary),
        State = component.State,
        PauseTime = component.PauseTime,
        Global = component.Global
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AudioComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case AudioComponent.Flags_FieldComponentState fieldComponentState1:
          component.Flags = fieldComponentState1.Flags;
          break;
        case AudioComponent.AudioStart_FieldComponentState fieldComponentState2:
          component.AudioStart = fieldComponentState2.AudioStart;
          break;
        case AudioComponent.FileName_FieldComponentState fieldComponentState3:
          component.FileName = fieldComponentState3.FileName;
          break;
        case AudioComponent.Params_FieldComponentState fieldComponentState4:
          component.Params = fieldComponentState4.Params;
          break;
        case AudioComponent.Auxiliary_FieldComponentState fieldComponentState5:
          component.Auxiliary = this.EnsureEntity<AudioComponent>(fieldComponentState5.Auxiliary, uid);
          break;
        case AudioComponent.State_FieldComponentState fieldComponentState6:
          component.State = fieldComponentState6.State;
          break;
        case AudioComponent.PauseTime_FieldComponentState fieldComponentState7:
          component.PauseTime = fieldComponentState7.PauseTime;
          break;
        case AudioComponent.Global_FieldComponentState fieldComponentState8:
          component.Global = fieldComponentState8.Global;
          break;
        case AudioComponent.AudioComponent_AutoState componentAutoState:
          component.Flags = componentAutoState.Flags;
          component.AudioStart = componentAutoState.AudioStart;
          component.FileName = componentAutoState.FileName;
          component.Params = componentAutoState.Params;
          component.Auxiliary = this.EnsureEntity<AudioComponent>(componentAutoState.Auxiliary, uid);
          component.State = componentAutoState.State;
          component.PauseTime = componentAutoState.PauseTime;
          component.Global = componentAutoState.Global;
          break;
        default:
          return;
      }
      IComponentState current = args.Current;
      if (current == null)
        return;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, AudioComponent>(uid, component, ref args1);
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Flags_FieldComponentState : 
    IComponentDeltaState<AudioComponent.AudioComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public AudioFlags Flags;

    public void ApplyToFullState(AudioComponent.AudioComponent_AutoState fullState)
    {
      fullState.Flags = this.Flags;
    }

    public AudioComponent.AudioComponent_AutoState CreateNewFullState(
      AudioComponent.AudioComponent_AutoState fullState)
    {
      AudioComponent.AudioComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AudioStart_FieldComponentState : 
    IComponentDeltaState<AudioComponent.AudioComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan AudioStart;

    public void ApplyToFullState(AudioComponent.AudioComponent_AutoState fullState)
    {
      fullState.AudioStart = this.AudioStart;
    }

    public AudioComponent.AudioComponent_AutoState CreateNewFullState(
      AudioComponent.AudioComponent_AutoState fullState)
    {
      AudioComponent.AudioComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class FileName_FieldComponentState : 
    IComponentDeltaState<AudioComponent.AudioComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public string FileName;

    public void ApplyToFullState(AudioComponent.AudioComponent_AutoState fullState)
    {
      fullState.FileName = this.FileName;
    }

    public AudioComponent.AudioComponent_AutoState CreateNewFullState(
      AudioComponent.AudioComponent_AutoState fullState)
    {
      AudioComponent.AudioComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Params_FieldComponentState : 
    IComponentDeltaState<AudioComponent.AudioComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public AudioParams Params;

    public void ApplyToFullState(AudioComponent.AudioComponent_AutoState fullState)
    {
      fullState.Params = this.Params;
    }

    public AudioComponent.AudioComponent_AutoState CreateNewFullState(
      AudioComponent.AudioComponent_AutoState fullState)
    {
      AudioComponent.AudioComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Auxiliary_FieldComponentState : 
    IComponentDeltaState<AudioComponent.AudioComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? Auxiliary;

    public void ApplyToFullState(AudioComponent.AudioComponent_AutoState fullState)
    {
      fullState.Auxiliary = this.Auxiliary;
    }

    public AudioComponent.AudioComponent_AutoState CreateNewFullState(
      AudioComponent.AudioComponent_AutoState fullState)
    {
      AudioComponent.AudioComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class State_FieldComponentState : 
    IComponentDeltaState<AudioComponent.AudioComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public AudioState State;

    public void ApplyToFullState(AudioComponent.AudioComponent_AutoState fullState)
    {
      fullState.State = this.State;
    }

    public AudioComponent.AudioComponent_AutoState CreateNewFullState(
      AudioComponent.AudioComponent_AutoState fullState)
    {
      AudioComponent.AudioComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class PauseTime_FieldComponentState : 
    IComponentDeltaState<AudioComponent.AudioComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan? PauseTime;

    public void ApplyToFullState(AudioComponent.AudioComponent_AutoState fullState)
    {
      fullState.PauseTime = this.PauseTime;
    }

    public AudioComponent.AudioComponent_AutoState CreateNewFullState(
      AudioComponent.AudioComponent_AutoState fullState)
    {
      AudioComponent.AudioComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Global_FieldComponentState : 
    IComponentDeltaState<AudioComponent.AudioComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Global;

    public void ApplyToFullState(AudioComponent.AudioComponent_AutoState fullState)
    {
      fullState.Global = this.Global;
    }

    public AudioComponent.AudioComponent_AutoState CreateNewFullState(
      AudioComponent.AudioComponent_AutoState fullState)
    {
      AudioComponent.AudioComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
