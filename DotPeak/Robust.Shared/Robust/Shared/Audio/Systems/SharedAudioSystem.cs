// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Systems.SharedAudioSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Effects;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Audio.Systems;

public abstract class SharedAudioSystem : EntitySystem
{
  [Dependency]
  protected readonly IConfigurationManager CfgManager;
  [Dependency]
  protected readonly IGameTiming Timing;
  [Dependency]
  private readonly INetManager _netManager;
  [Dependency]
  protected readonly IPrototypeManager ProtoMan;
  [Dependency]
  protected readonly IRobustRandom RandMan;
  [Dependency]
  protected readonly MetaDataSystem MetadataSys;
  [Dependency]
  protected readonly SharedTransformSystem XformSystem;
  public const float AudioDespawnBuffer = 1f;
  public const float DefaultSoundRange = 15f;
  protected readonly Dictionary<string, EntityUid> _auxiliaries = new Dictionary<string, EntityUid>();

  public int OcclusionCollisionMask { get; set; }

  public virtual float ZOffset { get; protected set; }

  public override void Initialize()
  {
    base.Initialize();
    this.InitializeEffect();
    this.ZOffset = this.CfgManager.GetCVar<float>(CVars.AudioZOffset);
    this.Subs.CVar<float>(this.CfgManager, CVars.AudioZOffset, new Action<float>(this.SetZOffset));
    this.SubscribeLocalEvent<AudioComponent, ComponentGetStateAttemptEvent>(new ComponentEventRefHandler<AudioComponent, ComponentGetStateAttemptEvent>(this.OnAudioGetStateAttempt));
    this.SubscribeLocalEvent<AudioComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<AudioComponent, EntityUnpausedEvent>(this.OnAudioUnpaused));
  }

  public void SetPlaybackPosition(Entity<AudioComponent?>? nullEntity, float position)
  {
    if (!nullEntity.HasValue)
      return;
    Entity<AudioComponent> entity = nullEntity.Value;
    if (!this.Resolve<AudioComponent>(entity.Owner, ref entity.Comp, false))
      return;
    TimeSpan audioLength = this.GetAudioLength(entity.Comp.FileName);
    position = this.CalculateAudioPosition(entity, new float?((float) audioLength.TotalSeconds), new float?(position));
    if (audioLength.TotalSeconds < (double) position)
    {
      if (!this._netManager.IsClient)
        this.QueueDel(new EntityUid?((EntityUid) nullEntity.Value));
      entity.Comp.StopPlaying();
    }
    else
    {
      TimeSpan timeSpan1 = (entity.Comp.PauseTime ?? this.Timing.CurTime) - entity.Comp.AudioStart;
      TimeSpan timeSpan2 = TimeSpan.FromSeconds((double) position - timeSpan1.TotalSeconds);
      if (Math.Abs(timeSpan2.TotalSeconds) <= 0.01)
        return;
      if (entity.Comp.PauseTime.HasValue)
      {
        entity.Comp.PauseTime = new TimeSpan?(entity.Comp.PauseTime.Value + timeSpan2);
        this.DirtyField<AudioComponent>(entity, "PauseTime");
      }
      else
      {
        entity.Comp.AudioStart -= timeSpan2;
        this.DirtyField<AudioComponent>(entity, "AudioStart");
        TimedDespawnComponent comp;
        if (this.TryComp<TimedDespawnComponent>(entity.Owner, out comp))
          comp.Lifetime -= (float) timeSpan2.TotalSeconds;
      }
      entity.Comp.PlaybackPosition = position;
    }
  }

  private float GetPlaybackPosition(AudioComponent component)
  {
    return (float) (this.Timing.CurTime - (component.PauseTime ?? TimeSpan.Zero) - component.AudioStart).TotalSeconds;
  }

  public virtual void SetMapAudio(Entity<AudioComponent>? audio)
  {
    if (!audio.HasValue)
      return;
    audio.Value.Comp.Global = true;
    this.MetadataSys.AddFlag(audio.Value.Owner, MetaDataFlags.Undetachable);
  }

  public virtual void SetGridAudio(Entity<AudioComponent>? entity)
  {
    if (!entity.HasValue)
      return;
    entity.Value.Comp.Flags |= AudioFlags.GridAudio;
    EntityUid? gridUid = this.Transform((EntityUid) entity.Value).GridUid;
    PhysicsComponent comp1;
    if (this.TryComp<PhysicsComponent>(gridUid, out comp1))
      this.XformSystem.SetLocalPosition(entity.Value.Owner, comp1.LocalCenter);
    MapGridComponent comp2;
    if (this.TryComp<MapGridComponent>(gridUid, out comp2))
    {
      Box2 localAabb = comp2.LocalAABB;
      Vector2 extents = ((Box2) ref localAabb).Extents;
      float refDistance = MathF.Max(extents.X, extents.Y);
      entity.Value.Comp.Params = entity.Value.Comp.Params.WithMaxDistance(refDistance + 15f).WithReferenceDistance(refDistance);
    }
    entity.Value.Comp.Flags |= AudioFlags.NoOcclusion;
    this.Dirty<AudioComponent>(entity.Value);
  }

  public void SetState(EntityUid? entity, AudioState state, bool force = false, AudioComponent? component = null)
  {
    if (!entity.HasValue || !this.Resolve<AudioComponent>(entity.Value, ref component, false) || component.State == state && !force)
      return;
    if (component.State == AudioState.Paused && state == AudioState.Playing)
    {
      TimeSpan curTime = this.Timing.CurTime;
      TimeSpan? pauseTime = component.PauseTime;
      TimeSpan? nullable = pauseTime.HasValue ? new TimeSpan?(curTime - pauseTime.GetValueOrDefault()) : new TimeSpan?();
      component.AudioStart += nullable ?? TimeSpan.Zero;
      component.PlaybackPosition = (float) (this.Timing.CurTime - component.AudioStart).TotalSeconds;
      this.DirtyField<AudioComponent>(entity.Value, component, "AudioStart");
    }
    if (component.State == AudioState.Stopped && state == AudioState.Playing)
    {
      component.AudioStart = this.Timing.CurTime;
      component.PauseTime = new TimeSpan?();
      this.DirtyField<AudioComponent>(entity.Value, component, "AudioStart");
      this.DirtyField<AudioComponent>(entity.Value, component, "PauseTime");
    }
    switch (state)
    {
      case AudioState.Stopped:
        component.AudioStart = this.Timing.CurTime;
        component.PauseTime = new TimeSpan?();
        this.DirtyField<AudioComponent>(entity.Value, component, "AudioStart");
        this.DirtyField<AudioComponent>(entity.Value, component, "PauseTime");
        component.StopPlaying();
        this.RemComp<TimedDespawnComponent>(entity.Value);
        break;
      case AudioState.Playing:
        component.PauseTime = new TimeSpan?();
        this.DirtyField<AudioComponent>(entity.Value, component, "PauseTime");
        component.StartPlaying();
        if (!component.Looping)
        {
          this.EnsureComp<TimedDespawnComponent>(entity.Value).Lifetime = (float) (this.GetAudioLength(component.FileName).TotalSeconds + 1.0);
          break;
        }
        break;
      case AudioState.Paused:
        component.PauseTime = new TimeSpan?(this.Timing.CurTime);
        this.DirtyField<AudioComponent>(entity.Value, component, "PauseTime");
        component.Pause();
        this.RemComp<TimedDespawnComponent>(entity.Value);
        break;
    }
    component.State = state;
    this.DirtyField<AudioComponent>(entity.Value, component, "State");
  }

  protected void SetZOffset(float value) => this.ZOffset = value;

  protected virtual void OnAudioUnpaused(
    EntityUid uid,
    AudioComponent component,
    ref EntityUnpausedEvent args)
  {
    component.AudioStart += args.PausedTime;
  }

  private void OnAudioGetStateAttempt(
    EntityUid uid,
    AudioComponent component,
    ref ComponentGetStateAttemptEvent args)
  {
    EntityUid? attachedEntity = (EntityUid?) args.Player?.AttachedEntity;
    if (component.ExcludedEntity.HasValue)
    {
      EntityUid? nullable = attachedEntity;
      EntityUid? excludedEntity = component.ExcludedEntity;
      if ((nullable.HasValue == excludedEntity.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == excludedEntity.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      {
        args.Cancelled = true;
        return;
      }
    }
    if (!attachedEntity.HasValue || component.IncludedEntities == null || component.IncludedEntities.Contains(attachedEntity.Value))
      return;
    args.Cancelled = true;
  }

  public float GetAudioDistance(float length)
  {
    return MathF.Sqrt(MathF.Pow(length, 2f) + MathF.Pow(this.ZOffset, 2f));
  }

  public ResolvedSoundSpecifier ResolveSound(SoundSpecifier specifier)
  {
    switch (specifier)
    {
      case SoundPathSpecifier soundPathSpecifier:
        return (ResolvedSoundSpecifier) new ResolvedPathSpecifier(soundPathSpecifier.Path == new ResPath() ? string.Empty : soundPathSpecifier.Path.ToString());
      case SoundCollectionSpecifier collectionSpecifier:
        if (collectionSpecifier.Collection == null)
          return (ResolvedSoundSpecifier) new ResolvedPathSpecifier(string.Empty);
        int index = this.RandMan.Next(this.ProtoMan.Index<SoundCollectionPrototype>(collectionSpecifier.Collection).PickFiles.Count);
        return (ResolvedSoundSpecifier) new ResolvedCollectionSpecifier(collectionSpecifier.Collection, index);
      default:
        return (ResolvedSoundSpecifier) new ResolvedPathSpecifier(string.Empty);
    }
  }

  [Obsolete("Use ResolveSound() and pass around resolved sound specifiers instead.")]
  public string GetSound(SoundSpecifier specifier)
  {
    return this.GetAudioPath(this.ResolveSound(specifier));
  }

  protected float CalculateAudioPosition(
    Entity<AudioComponent> ent,
    float? length = null,
    float? position = null)
  {
    float valueOrDefault = position.GetValueOrDefault();
    TimeSpan timeSpan;
    if (!position.HasValue)
    {
      timeSpan = (ent.Comp.PauseTime ?? this.Timing.CurTime) - ent.Comp.AudioStart;
      position = new float?((float) timeSpan.TotalSeconds);
    }
    valueOrDefault = length.GetValueOrDefault();
    if (!length.HasValue)
    {
      timeSpan = this.GetAudioLength(ent.Comp.FileName);
      length = new float?((float) timeSpan.TotalSeconds);
    }
    if (ent.Comp.Params.Loop)
    {
      float? nullable1 = position;
      float? nullable2 = length;
      position = nullable1.HasValue & nullable2.HasValue ? new float?(nullable1.GetValueOrDefault() % nullable2.GetValueOrDefault()) : new float?();
    }
    float max = Math.Max(length.Value - 0.01f, 0.0f);
    position = new float?(Math.Clamp(position.Value, 0.0f, max));
    return position.Value;
  }

  [return: NotNullIfNotNull("specifier")]
  public string? GetAudioPath(ResolvedSoundSpecifier? specifier)
  {
    switch (specifier)
    {
      case ResolvedPathSpecifier resolvedPathSpecifier:
        return resolvedPathSpecifier.Path.ToString();
      case ResolvedCollectionSpecifier collectionSpecifier:
        string empty;
        if (collectionSpecifier.Collection.HasValue)
        {
          IPrototypeManager protoMan = this.ProtoMan;
          ProtoId<SoundCollectionPrototype>? collection = collectionSpecifier.Collection;
          string valueOrDefault = collection.HasValue ? (string) collection.GetValueOrDefault() : (string) null;
          empty = protoMan.Index<SoundCollectionPrototype>(valueOrDefault).PickFiles[collectionSpecifier.Index].ToString();
        }
        else
          empty = string.Empty;
        return empty;
      case null:
        return (string) null;
      default:
        throw new ArgumentOutOfRangeException(nameof (specifier), (object) specifier, "argument is not a ResolvedPathSpecifier or a ResolvedCollectionSpecifier");
    }
  }

  protected Entity<AudioComponent> SetupAudio(
    ResolvedSoundSpecifier? specifier,
    AudioParams? audioParams,
    bool initialize = true,
    TimeSpan? length = null)
  {
    EntityUid entityUninitialized = this.EntityManager.CreateEntityUninitialized("Audio", MapCoordinates.Nullspace, (ComponentRegistry) null, new Angle());
    string audioPath = this.GetAudioPath(specifier);
    this.MetadataSys.SetEntityName(entityUninitialized, $"Audio ({audioPath})", raiseEvents: false);
    audioParams.GetValueOrDefault();
    if (!audioParams.HasValue)
      audioParams = new AudioParams?(AudioParams.Default);
    AudioComponent comp = this.AddComp<AudioComponent>(entityUninitialized);
    comp.FileName = audioPath ?? string.Empty;
    comp.Params = audioParams.Value;
    comp.AudioStart = this.Timing.CurTime;
    if (!audioParams.Value.Loop)
    {
      length.GetValueOrDefault();
      if (!length.HasValue)
        length = new TimeSpan?(this.GetAudioLength(audioPath));
      this.AddComp<TimedDespawnComponent>(entityUninitialized).Lifetime = (float) (length.Value.TotalSeconds + 1.0);
    }
    float? variation = comp.Params.Variation;
    if (variation.HasValue)
    {
      variation = comp.Params.Variation;
      if ((double) variation.Value != 0.0)
      {
        ref AudioParams local = ref comp.Params;
        double pitch = (double) local.Pitch;
        IRobustRandom randMan = this.RandMan;
        variation = comp.Params.Variation;
        double σ = (double) variation.Value;
        double num = randMan.NextGaussian(1.0, σ);
        local.Pitch = (float) (pitch * num);
      }
    }
    if (initialize)
      this.EntityManager.InitializeAndStartEntity(entityUninitialized, new MapId?());
    return new Entity<AudioComponent>(entityUninitialized, comp);
  }

  public static float GainToVolume(float value)
  {
    if ((double) value < 0.0)
      value = 0.0f;
    return 10f * MathF.Log10(value);
  }

  public static float VolumeToGain(float value)
  {
    float num = MathF.Pow(10f, value / 10f);
    return (double) num >= 0.0 ? num : throw new InvalidOperationException($"Tried to get gain calculation that resulted in invalid value of {num}");
  }

  public void SetGain(EntityUid? entity, float value, AudioComponent? component = null)
  {
    if (!entity.HasValue || !this.Resolve<AudioComponent>(entity.Value, ref component))
      return;
    float volume = SharedAudioSystem.GainToVolume(value);
    this.SetVolume(entity, volume, component);
  }

  public void SetVolume(EntityUid? entity, float value, AudioComponent? component = null)
  {
    if (!entity.HasValue || !this.Resolve<AudioComponent>(entity.Value, ref component) || component.Params.Volume.Equals(value))
      return;
    if (float.IsNaN(value))
      value = float.NegativeInfinity;
    component.Params.Volume = value;
    component.Volume = value;
    this.DirtyField<AudioComponent>(entity.Value, component, "Params");
  }

  public TimeSpan GetAudioLength(ResolvedSoundSpecifier specifier)
  {
    return this.GetAudioLength(this.GetAudioPath(specifier));
  }

  protected TimeSpan GetAudioLength(string filename)
  {
    return filename.StartsWith('/') ? this.GetAudioLengthImpl(filename) : throw new ArgumentException("Path must be rooted. Path: " + filename);
  }

  protected abstract TimeSpan GetAudioLengthImpl(string filename);

  public EntityUid? Stop(EntityUid? uid, AudioComponent? component = null)
  {
    if (!uid.HasValue || !this.Resolve<AudioComponent>(uid.Value, ref component, false))
      return new EntityUid?();
    if (!this.Timing.IsFirstTimePredicted || this._netManager.IsClient && !this.IsClientSide(uid.Value))
      return new EntityUid?();
    this.QueueDel(uid);
    return new EntityUid?();
  }

  public abstract (EntityUid Entity, AudioComponent Component)? PlayGlobal(
    ResolvedSoundSpecifier? filename,
    Filter playerFilter,
    bool recordReplay,
    AudioParams? audioParams = null);

  public (EntityUid Entity, AudioComponent Component)? PlayGlobal(
    SoundSpecifier? sound,
    Filter playerFilter,
    bool recordReplay,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayGlobal(this.ResolveSound(sound), playerFilter, recordReplay, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public abstract (EntityUid Entity, AudioComponent Component)? PlayGlobal(
    ResolvedSoundSpecifier? filename,
    ICommonSession recipient,
    AudioParams? audioParams = null);

  public (EntityUid Entity, AudioComponent Component)? PlayGlobal(
    SoundSpecifier? sound,
    ICommonSession recipient,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayGlobal(this.ResolveSound(sound), recipient, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public abstract void LoadStream<T>(Entity<AudioComponent> entity, T stream);

  public abstract (EntityUid Entity, AudioComponent Component)? PlayGlobal(
    ResolvedSoundSpecifier? filename,
    EntityUid recipient,
    AudioParams? audioParams = null);

  public (EntityUid Entity, AudioComponent Component)? PlayGlobal(
    SoundSpecifier? sound,
    EntityUid recipient,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayGlobal(this.ResolveSound(sound), recipient, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public abstract (EntityUid Entity, AudioComponent Component)? PlayEntity(
    ResolvedSoundSpecifier? filename,
    Filter playerFilter,
    EntityUid uid,
    bool recordReplay,
    AudioParams? audioParams = null);

  public abstract (EntityUid Entity, AudioComponent Component)? PlayEntity(
    ResolvedSoundSpecifier? filename,
    ICommonSession recipient,
    EntityUid uid,
    AudioParams? audioParams = null);

  public abstract (EntityUid Entity, AudioComponent Component)? PlayEntity(
    ResolvedSoundSpecifier? filename,
    EntityUid recipient,
    EntityUid uid,
    AudioParams? audioParams = null);

  public (EntityUid Entity, AudioComponent Component)? PlayEntity(
    SoundSpecifier? sound,
    Filter playerFilter,
    EntityUid uid,
    bool recordReplay,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayEntity(this.ResolveSound(sound), playerFilter, uid, recordReplay, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public (EntityUid Entity, AudioComponent Component)? PlayEntity(
    SoundSpecifier? sound,
    ICommonSession recipient,
    EntityUid uid,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayEntity(this.ResolveSound(sound), recipient, uid, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public (EntityUid Entity, AudioComponent Component)? PlayEntity(
    SoundSpecifier? sound,
    EntityUid recipient,
    EntityUid uid,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayEntity(this.ResolveSound(sound), recipient, uid, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public (EntityUid Entity, AudioComponent Component)? PlayPvs(
    SoundSpecifier? sound,
    EntityUid uid,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayPvs(this.ResolveSound(sound), uid, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public (EntityUid Entity, AudioComponent Component)? PlayPvs(
    SoundSpecifier? sound,
    EntityCoordinates coordinates,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayPvs(this.ResolveSound(sound), coordinates, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public abstract (EntityUid Entity, AudioComponent Component)? PlayPvs(
    ResolvedSoundSpecifier? filename,
    EntityCoordinates coordinates,
    AudioParams? audioParams = null);

  public abstract (EntityUid Entity, AudioComponent Component)? PlayPvs(
    ResolvedSoundSpecifier? filename,
    EntityUid uid,
    AudioParams? audioParams = null);

  public abstract (EntityUid Entity, AudioComponent Component)? PlayLocal(
    SoundSpecifier? sound,
    EntityUid source,
    EntityUid? soundInitiator,
    AudioParams? audioParams = null);

  public abstract (EntityUid Entity, AudioComponent Component)? PlayPredicted(
    SoundSpecifier? sound,
    EntityUid source,
    EntityUid? user,
    AudioParams? audioParams = null);

  public abstract (EntityUid Entity, AudioComponent Component)? PlayPredicted(
    SoundSpecifier? sound,
    EntityCoordinates coordinates,
    EntityUid? user,
    AudioParams? audioParams = null);

  public abstract (EntityUid Entity, AudioComponent Component)? PlayStatic(
    ResolvedSoundSpecifier? filename,
    Filter playerFilter,
    EntityCoordinates coordinates,
    bool recordReplay,
    AudioParams? audioParams = null);

  public abstract (EntityUid Entity, AudioComponent Component)? PlayStatic(
    ResolvedSoundSpecifier? filename,
    ICommonSession recipient,
    EntityCoordinates coordinates,
    AudioParams? audioParams = null);

  public abstract (EntityUid Entity, AudioComponent Component)? PlayStatic(
    ResolvedSoundSpecifier? filename,
    EntityUid recipient,
    EntityCoordinates coordinates,
    AudioParams? audioParams = null);

  public (EntityUid Entity, AudioComponent Component)? PlayStatic(
    SoundSpecifier? sound,
    Filter playerFilter,
    EntityCoordinates coordinates,
    bool recordReplay,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayStatic(this.ResolveSound(sound), playerFilter, coordinates, recordReplay, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public (EntityUid Entity, AudioComponent Component)? PlayStatic(
    SoundSpecifier? sound,
    ICommonSession recipient,
    EntityCoordinates coordinates,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayStatic(this.ResolveSound(sound), recipient, coordinates, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public (EntityUid Entity, AudioComponent Component)? PlayStatic(
    SoundSpecifier? sound,
    EntityUid recipient,
    EntityCoordinates coordinates,
    AudioParams? audioParams = null)
  {
    return sound != null ? this.PlayStatic(this.ResolveSound(sound), recipient, coordinates, new AudioParams?(audioParams ?? sound.Params)) : new (EntityUid, AudioComponent)?();
  }

  public bool IsPlaying(EntityUid? stream, AudioComponent? component = null)
  {
    return stream.HasValue && this.Resolve<AudioComponent>(stream.Value, ref component, false) && component.State == AudioState.Playing;
  }

  public IReadOnlyDictionary<string, EntityUid> Auxiliaries
  {
    get => (IReadOnlyDictionary<string, EntityUid>) this._auxiliaries;
  }

  protected virtual void InitializeEffect()
  {
    this.SubscribeLocalEvent<AudioPresetComponent, ComponentStartup>(new ComponentEventHandler<AudioPresetComponent, ComponentStartup>(this.OnPresetStartup));
    this.SubscribeLocalEvent<AudioPresetComponent, ComponentShutdown>(new ComponentEventHandler<AudioPresetComponent, ComponentShutdown>(this.OnPresetShutdown));
  }

  private void OnPresetStartup(
    EntityUid uid,
    AudioPresetComponent component,
    ComponentStartup args)
  {
    this._auxiliaries[component.Preset] = uid;
  }

  private void OnPresetShutdown(
    EntityUid uid,
    AudioPresetComponent component,
    ComponentShutdown args)
  {
    this._auxiliaries.Remove(component.Preset);
  }

  public virtual (EntityUid Entity, AudioAuxiliaryComponent Component) CreateAuxiliary()
  {
    EntityUid uid = this.Spawn((string) null, MapCoordinates.Nullspace, rotation: new Angle());
    AudioAuxiliaryComponent auxiliaryComponent = this.AddComp<AudioAuxiliaryComponent>(uid);
    return (uid, auxiliaryComponent);
  }

  public virtual (EntityUid Entity, AudioEffectComponent Component) CreateEffect()
  {
    EntityUid uid = this.Spawn((string) null, MapCoordinates.Nullspace, rotation: new Angle());
    AudioEffectComponent audioEffectComponent = this.AddComp<AudioEffectComponent>(uid);
    return (uid, audioEffectComponent);
  }

  public virtual void SetAuxiliary(EntityUid uid, AudioComponent audio, EntityUid? auxUid)
  {
    audio.Auxiliary = auxUid;
    this.Dirty(uid, (IComponent) audio);
  }

  public virtual void SetEffect(
    EntityUid auxUid,
    AudioAuxiliaryComponent aux,
    EntityUid? effectUid)
  {
    aux.Effect = effectUid;
    this.Dirty(auxUid, (IComponent) aux);
  }

  public void SetEffect(EntityUid? audioUid, AudioComponent? component, string effectProto)
  {
    if (!audioUid.HasValue || component == null)
      return;
    this.SetAuxiliary(audioUid.Value, component, new EntityUid?(this._auxiliaries[effectProto]));
  }

  public void SetEffectPreset(
    EntityUid effectUid,
    AudioEffectComponent effectComp,
    AudioPresetPrototype preset)
  {
    effectComp.Density = preset.Density;
    effectComp.Diffusion = preset.Diffusion;
    effectComp.Gain = preset.Gain;
    effectComp.GainHF = preset.GainHF;
    effectComp.GainLF = preset.GainLF;
    effectComp.DecayTime = preset.DecayTime;
    effectComp.DecayHFRatio = preset.DecayHFRatio;
    effectComp.DecayLFRatio = preset.DecayLFRatio;
    effectComp.ReflectionsGain = preset.ReflectionsGain;
    effectComp.ReflectionsDelay = preset.ReflectionsDelay;
    effectComp.ReflectionsPan = preset.ReflectionsPan;
    effectComp.LateReverbGain = preset.LateReverbGain;
    effectComp.LateReverbDelay = preset.LateReverbDelay;
    effectComp.LateReverbPan = preset.LateReverbPan;
    effectComp.EchoTime = preset.EchoTime;
    effectComp.EchoDepth = preset.EchoDepth;
    effectComp.ModulationTime = preset.ModulationTime;
    effectComp.ModulationDepth = preset.ModulationDepth;
    effectComp.AirAbsorptionGainHF = preset.AirAbsorptionGainHF;
    effectComp.HFReference = preset.HFReference;
    effectComp.LFReference = preset.LFReference;
    effectComp.RoomRolloffFactor = preset.RoomRolloffFactor;
    effectComp.DecayHFLimit = preset.DecayHFLimit;
    this.Dirty(effectUid, (IComponent) effectComp);
  }

  public void SetEffectPreset(
    EntityUid effectUid,
    AudioEffectComponent effectComp,
    ReverbProperties preset)
  {
    effectComp.Density = preset.Density;
    effectComp.Diffusion = preset.Diffusion;
    effectComp.Gain = preset.Gain;
    effectComp.GainHF = preset.GainHF;
    effectComp.GainLF = preset.GainLF;
    effectComp.DecayTime = preset.DecayTime;
    effectComp.DecayHFRatio = preset.DecayHFRatio;
    effectComp.DecayLFRatio = preset.DecayLFRatio;
    effectComp.ReflectionsGain = preset.ReflectionsGain;
    effectComp.ReflectionsDelay = preset.ReflectionsDelay;
    effectComp.ReflectionsPan = preset.ReflectionsPan;
    effectComp.LateReverbGain = preset.LateReverbGain;
    effectComp.LateReverbDelay = preset.LateReverbDelay;
    effectComp.LateReverbPan = preset.LateReverbPan;
    effectComp.EchoTime = preset.EchoTime;
    effectComp.EchoDepth = preset.EchoDepth;
    effectComp.ModulationTime = preset.ModulationTime;
    effectComp.ModulationDepth = preset.ModulationDepth;
    effectComp.AirAbsorptionGainHF = preset.AirAbsorptionGainHF;
    effectComp.HFReference = preset.HFReference;
    effectComp.LFReference = preset.LFReference;
    effectComp.RoomRolloffFactor = preset.RoomRolloffFactor;
    effectComp.DecayHFLimit = preset.DecayHFLimit;
    this.Dirty(effectUid, (IComponent) effectComp);
  }

  [NetSerializable]
  [Serializable]
  protected abstract class AudioMessage : EntityEventArgs
  {
    public ResolvedSoundSpecifier Specifier = (ResolvedSoundSpecifier) new ResolvedPathSpecifier(string.Empty);
    public AudioParams AudioParams;
  }

  [NetSerializable]
  [Serializable]
  protected sealed class PlayAudioGlobalMessage : SharedAudioSystem.AudioMessage
  {
  }

  [NetSerializable]
  [Serializable]
  protected sealed class PlayAudioPositionalMessage : SharedAudioSystem.AudioMessage
  {
    public NetCoordinates Coordinates;
  }

  [NetSerializable]
  [Serializable]
  protected sealed class PlayAudioEntityMessage : SharedAudioSystem.AudioMessage
  {
    public NetEntity NetEntity;
  }
}
