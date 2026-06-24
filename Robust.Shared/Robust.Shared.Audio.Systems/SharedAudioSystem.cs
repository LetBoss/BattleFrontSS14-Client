using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
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

namespace Robust.Shared.Audio.Systems;

public abstract class SharedAudioSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected abstract class AudioMessage : EntityEventArgs
	{
		public ResolvedSoundSpecifier Specifier = new ResolvedPathSpecifier(string.Empty);

		public AudioParams AudioParams;
	}

	[Serializable]
	[NetSerializable]
	protected sealed class PlayAudioGlobalMessage : AudioMessage
	{
	}

	[Serializable]
	[NetSerializable]
	protected sealed class PlayAudioPositionalMessage : AudioMessage
	{
		public NetCoordinates Coordinates;
	}

	[Serializable]
	[NetSerializable]
	protected sealed class PlayAudioEntityMessage : AudioMessage
	{
		public NetEntity NetEntity;
	}

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

	public IReadOnlyDictionary<string, EntityUid> Auxiliaries => _auxiliaries;

	public override void Initialize()
	{
		base.Initialize();
		InitializeEffect();
		ZOffset = CfgManager.GetCVar(CVars.AudioZOffset);
		base.Subs.CVar(CfgManager, CVars.AudioZOffset, SetZOffset);
		SubscribeLocalEvent<AudioComponent, ComponentGetStateAttemptEvent>(OnAudioGetStateAttempt);
		SubscribeLocalEvent<AudioComponent, EntityUnpausedEvent>(OnAudioUnpaused);
	}

	public void SetPlaybackPosition(Entity<AudioComponent?>? nullEntity, float position)
	{
		if (!nullEntity.HasValue)
		{
			return;
		}
		Entity<AudioComponent> value = nullEntity.Value;
		if (!Resolve(value.Owner, ref value.Comp, logMissing: false))
		{
			return;
		}
		TimeSpan audioLength = GetAudioLength(value.Comp.FileName);
		position = CalculateAudioPosition(value, (float)audioLength.TotalSeconds, position);
		if (audioLength.TotalSeconds < (double)position)
		{
			if (!_netManager.IsClient)
			{
				QueueDel(nullEntity.Value);
			}
			value.Comp.StopPlaying();
			return;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)position - ((value.Comp.PauseTime ?? Timing.CurTime) - value.Comp.AudioStart).TotalSeconds);
		if (Math.Abs(timeSpan.TotalSeconds) <= 0.01)
		{
			return;
		}
		if (value.Comp.PauseTime.HasValue)
		{
			value.Comp.PauseTime = value.Comp.PauseTime.Value + timeSpan;
			DirtyField(value, "PauseTime");
		}
		else
		{
			value.Comp.AudioStart -= timeSpan;
			DirtyField(value, "AudioStart");
			if (TryComp(value.Owner, out TimedDespawnComponent comp))
			{
				comp.Lifetime -= (float)timeSpan.TotalSeconds;
			}
		}
		value.Comp.PlaybackPosition = position;
	}

	private float GetPlaybackPosition(AudioComponent component)
	{
		return (float)(Timing.CurTime - (component.PauseTime ?? TimeSpan.Zero) - component.AudioStart).TotalSeconds;
	}

	public virtual void SetMapAudio(Entity<AudioComponent>? audio)
	{
		if (audio.HasValue)
		{
			audio.Value.Comp.Global = true;
			MetadataSys.AddFlag(audio.Value.Owner, MetaDataFlags.Undetachable);
		}
	}

	public virtual void SetGridAudio(Entity<AudioComponent>? entity)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (entity.HasValue)
		{
			entity.Value.Comp.Flags |= AudioFlags.GridAudio;
			EntityUid? gridUid = Transform(entity.Value).GridUid;
			if (TryComp(gridUid, out PhysicsComponent comp))
			{
				XformSystem.SetLocalPosition(entity.Value.Owner, comp.LocalCenter);
			}
			if (TryComp(gridUid, out MapGridComponent comp2))
			{
				Box2 localAABB = comp2.LocalAABB;
				Vector2 extents = ((Box2)(ref localAABB)).Extents;
				float num = MathF.Max(extents.X, extents.Y);
				entity.Value.Comp.Params = entity.Value.Comp.Params.WithMaxDistance(num + 15f).WithReferenceDistance(num);
			}
			entity.Value.Comp.Flags |= AudioFlags.NoOcclusion;
			Dirty(entity.Value);
		}
	}

	public void SetState(EntityUid? entity, AudioState state, bool force = false, AudioComponent? component = null)
	{
		if (!entity.HasValue || !Resolve(entity.Value, ref component, logMissing: false) || (component.State == state && !force))
		{
			return;
		}
		if (component.State == AudioState.Paused && state == AudioState.Playing)
		{
			TimeSpan curTime = Timing.CurTime;
			TimeSpan? pauseTime = component.PauseTime;
			TimeSpan? timeSpan = curTime - pauseTime;
			component.AudioStart += timeSpan ?? TimeSpan.Zero;
			component.PlaybackPosition = (float)(Timing.CurTime - component.AudioStart).TotalSeconds;
			DirtyField(entity.Value, component, "AudioStart");
		}
		if (component.State == AudioState.Stopped && state == AudioState.Playing)
		{
			component.AudioStart = Timing.CurTime;
			component.PauseTime = null;
			DirtyField(entity.Value, component, "AudioStart");
			DirtyField(entity.Value, component, "PauseTime");
		}
		switch (state)
		{
		case AudioState.Stopped:
			component.AudioStart = Timing.CurTime;
			component.PauseTime = null;
			DirtyField(entity.Value, component, "AudioStart");
			DirtyField(entity.Value, component, "PauseTime");
			component.StopPlaying();
			RemComp<TimedDespawnComponent>(entity.Value);
			break;
		case AudioState.Paused:
			component.PauseTime = Timing.CurTime;
			DirtyField(entity.Value, component, "PauseTime");
			component.Pause();
			RemComp<TimedDespawnComponent>(entity.Value);
			break;
		case AudioState.Playing:
			component.PauseTime = null;
			DirtyField(entity.Value, component, "PauseTime");
			component.StartPlaying();
			if (!component.Looping)
			{
				EnsureComp<TimedDespawnComponent>(entity.Value).Lifetime = (float)GetAudioLength(component.FileName).TotalSeconds + 1f;
			}
			break;
		}
		component.State = state;
		DirtyField(entity.Value, component, "State");
	}

	protected void SetZOffset(float value)
	{
		ZOffset = value;
	}

	protected virtual void OnAudioUnpaused(EntityUid uid, AudioComponent component, ref EntityUnpausedEvent args)
	{
		component.AudioStart += args.PausedTime;
	}

	private void OnAudioGetStateAttempt(EntityUid uid, AudioComponent component, ref ComponentGetStateAttemptEvent args)
	{
		EntityUid? entityUid = args.Player?.AttachedEntity;
		if (component.ExcludedEntity.HasValue && entityUid == component.ExcludedEntity)
		{
			args.Cancelled = true;
		}
		else if (entityUid.HasValue && component.IncludedEntities != null && !component.IncludedEntities.Contains(entityUid.Value))
		{
			args.Cancelled = true;
		}
	}

	public float GetAudioDistance(float length)
	{
		return MathF.Sqrt(MathF.Pow(length, 2f) + MathF.Pow(ZOffset, 2f));
	}

	public ResolvedSoundSpecifier ResolveSound(SoundSpecifier specifier)
	{
		if (!(specifier is SoundPathSpecifier soundPathSpecifier))
		{
			if (specifier is SoundCollectionSpecifier soundCollectionSpecifier)
			{
				if (soundCollectionSpecifier.Collection == null)
				{
					return new ResolvedPathSpecifier(string.Empty);
				}
				SoundCollectionPrototype soundCollectionPrototype = ProtoMan.Index<SoundCollectionPrototype>(soundCollectionSpecifier.Collection);
				int index = RandMan.Next(soundCollectionPrototype.PickFiles.Count);
				return new ResolvedCollectionSpecifier(soundCollectionSpecifier.Collection, index);
			}
			return new ResolvedPathSpecifier(string.Empty);
		}
		return new ResolvedPathSpecifier((soundPathSpecifier.Path == default(ResPath)) ? string.Empty : soundPathSpecifier.Path.ToString());
	}

	[Obsolete("Use ResolveSound() and pass around resolved sound specifiers instead.")]
	public string GetSound(SoundSpecifier specifier)
	{
		ResolvedSoundSpecifier specifier2 = ResolveSound(specifier);
		return GetAudioPath(specifier2);
	}

	protected float CalculateAudioPosition(Entity<AudioComponent> ent, float? length = null, float? position = null)
	{
		float valueOrDefault = position.GetValueOrDefault();
		if (!position.HasValue)
		{
			valueOrDefault = (float)((ent.Comp.PauseTime ?? Timing.CurTime) - ent.Comp.AudioStart).TotalSeconds;
			position = valueOrDefault;
		}
		valueOrDefault = length.GetValueOrDefault();
		if (!length.HasValue)
		{
			valueOrDefault = (float)GetAudioLength(ent.Comp.FileName).TotalSeconds;
			length = valueOrDefault;
		}
		if (ent.Comp.Params.Loop)
		{
			position %= length;
		}
		float max = Math.Max(length.Value - 0.01f, 0f);
		position = Math.Clamp(position.Value, 0f, max);
		return position.Value;
	}

	[return: NotNullIfNotNull("specifier")]
	public string? GetAudioPath(ResolvedSoundSpecifier? specifier)
	{
		if (!(specifier is ResolvedPathSpecifier { Path: var path }))
		{
			if (!(specifier is ResolvedCollectionSpecifier resolvedCollectionSpecifier))
			{
				if (specifier == null)
				{
					return null;
				}
				throw new ArgumentOutOfRangeException("specifier", specifier, "argument is not a ResolvedPathSpecifier or a ResolvedCollectionSpecifier");
			}
			string result;
			if (resolvedCollectionSpecifier.Collection.HasValue)
			{
				IPrototypeManager protoMan = ProtoMan;
				ProtoId<SoundCollectionPrototype>? collection = resolvedCollectionSpecifier.Collection;
				result = protoMan.Index<SoundCollectionPrototype>(collection.HasValue ? ((string)collection.GetValueOrDefault()) : null).PickFiles[resolvedCollectionSpecifier.Index].ToString();
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}
		return path.ToString();
	}

	protected Entity<AudioComponent> SetupAudio(ResolvedSoundSpecifier? specifier, AudioParams? audioParams, bool initialize = true, TimeSpan? length = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entityUid = EntityManager.CreateEntityUninitialized("Audio", MapCoordinates.Nullspace);
		string audioPath = GetAudioPath(specifier);
		MetadataSys.SetEntityName(entityUid, "Audio (" + audioPath + ")", null, raiseEvents: false);
		AudioParams valueOrDefault = audioParams.GetValueOrDefault();
		if (!audioParams.HasValue)
		{
			valueOrDefault = AudioParams.Default;
			audioParams = valueOrDefault;
		}
		AudioComponent audioComponent = AddComp<AudioComponent>(entityUid);
		audioComponent.FileName = audioPath ?? string.Empty;
		audioComponent.Params = audioParams.Value;
		audioComponent.AudioStart = Timing.CurTime;
		if (!audioParams.Value.Loop)
		{
			TimeSpan valueOrDefault2 = length.GetValueOrDefault();
			if (!length.HasValue)
			{
				valueOrDefault2 = GetAudioLength(audioPath);
				length = valueOrDefault2;
			}
			AddComp<TimedDespawnComponent>(entityUid).Lifetime = (float)length.Value.TotalSeconds + 1f;
		}
		if (audioComponent.Params.Variation.HasValue && audioComponent.Params.Variation.Value != 0f)
		{
			audioComponent.Params.Pitch *= (float)RandMan.NextGaussian(1.0, audioComponent.Params.Variation.Value);
		}
		if (initialize)
		{
			EntityManager.InitializeAndStartEntity(entityUid);
		}
		return new Entity<AudioComponent>(entityUid, audioComponent);
	}

	public static float GainToVolume(float value)
	{
		if (value < 0f)
		{
			value = 0f;
		}
		return 10f * MathF.Log10(value);
	}

	public static float VolumeToGain(float value)
	{
		float num = MathF.Pow(10f, value / 10f);
		if (num < 0f)
		{
			throw new InvalidOperationException($"Tried to get gain calculation that resulted in invalid value of {num}");
		}
		return num;
	}

	public void SetGain(EntityUid? entity, float value, AudioComponent? component = null)
	{
		if (entity.HasValue && Resolve(entity.Value, ref component))
		{
			float value2 = GainToVolume(value);
			SetVolume(entity, value2, component);
		}
	}

	public void SetVolume(EntityUid? entity, float value, AudioComponent? component = null)
	{
		if (entity.HasValue && Resolve(entity.Value, ref component) && !component.Params.Volume.Equals(value))
		{
			if (float.IsNaN(value))
			{
				value = float.NegativeInfinity;
			}
			component.Params.Volume = value;
			component.Volume = value;
			DirtyField(entity.Value, component, "Params");
		}
	}

	public TimeSpan GetAudioLength(ResolvedSoundSpecifier specifier)
	{
		return GetAudioLength(GetAudioPath(specifier));
	}

	protected TimeSpan GetAudioLength(string filename)
	{
		if (!filename.StartsWith('/'))
		{
			throw new ArgumentException("Path must be rooted. Path: " + filename);
		}
		return GetAudioLengthImpl(filename);
	}

	protected abstract TimeSpan GetAudioLengthImpl(string filename);

	public EntityUid? Stop(EntityUid? uid, AudioComponent? component = null)
	{
		if (!uid.HasValue || !Resolve(uid.Value, ref component, logMissing: false))
		{
			return null;
		}
		if (!Timing.IsFirstTimePredicted || (_netManager.IsClient && !IsClientSide(uid.Value)))
		{
			return null;
		}
		QueueDel(uid);
		return null;
	}

	public abstract (EntityUid Entity, AudioComponent Component)? PlayGlobal(ResolvedSoundSpecifier? filename, Filter playerFilter, bool recordReplay, AudioParams? audioParams = null);

	public (EntityUid Entity, AudioComponent Component)? PlayGlobal(SoundSpecifier? sound, Filter playerFilter, bool recordReplay, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayGlobal(ResolveSound(sound), playerFilter, recordReplay, audioParams ?? sound.Params);
		}
		return null;
	}

	public abstract (EntityUid Entity, AudioComponent Component)? PlayGlobal(ResolvedSoundSpecifier? filename, ICommonSession recipient, AudioParams? audioParams = null);

	public (EntityUid Entity, AudioComponent Component)? PlayGlobal(SoundSpecifier? sound, ICommonSession recipient, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayGlobal(ResolveSound(sound), recipient, audioParams ?? sound.Params);
		}
		return null;
	}

	public abstract void LoadStream<T>(Entity<AudioComponent> entity, T stream);

	public abstract (EntityUid Entity, AudioComponent Component)? PlayGlobal(ResolvedSoundSpecifier? filename, EntityUid recipient, AudioParams? audioParams = null);

	public (EntityUid Entity, AudioComponent Component)? PlayGlobal(SoundSpecifier? sound, EntityUid recipient, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayGlobal(ResolveSound(sound), recipient, audioParams ?? sound.Params);
		}
		return null;
	}

	public abstract (EntityUid Entity, AudioComponent Component)? PlayEntity(ResolvedSoundSpecifier? filename, Filter playerFilter, EntityUid uid, bool recordReplay, AudioParams? audioParams = null);

	public abstract (EntityUid Entity, AudioComponent Component)? PlayEntity(ResolvedSoundSpecifier? filename, ICommonSession recipient, EntityUid uid, AudioParams? audioParams = null);

	public abstract (EntityUid Entity, AudioComponent Component)? PlayEntity(ResolvedSoundSpecifier? filename, EntityUid recipient, EntityUid uid, AudioParams? audioParams = null);

	public (EntityUid Entity, AudioComponent Component)? PlayEntity(SoundSpecifier? sound, Filter playerFilter, EntityUid uid, bool recordReplay, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayEntity(ResolveSound(sound), playerFilter, uid, recordReplay, audioParams ?? sound.Params);
		}
		return null;
	}

	public (EntityUid Entity, AudioComponent Component)? PlayEntity(SoundSpecifier? sound, ICommonSession recipient, EntityUid uid, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayEntity(ResolveSound(sound), recipient, uid, audioParams ?? sound.Params);
		}
		return null;
	}

	public (EntityUid Entity, AudioComponent Component)? PlayEntity(SoundSpecifier? sound, EntityUid recipient, EntityUid uid, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayEntity(ResolveSound(sound), recipient, uid, audioParams ?? sound.Params);
		}
		return null;
	}

	public (EntityUid Entity, AudioComponent Component)? PlayPvs(SoundSpecifier? sound, EntityUid uid, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayPvs(ResolveSound(sound), uid, audioParams ?? sound.Params);
		}
		return null;
	}

	public (EntityUid Entity, AudioComponent Component)? PlayPvs(SoundSpecifier? sound, EntityCoordinates coordinates, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayPvs(ResolveSound(sound), coordinates, audioParams ?? sound.Params);
		}
		return null;
	}

	public abstract (EntityUid Entity, AudioComponent Component)? PlayPvs(ResolvedSoundSpecifier? filename, EntityCoordinates coordinates, AudioParams? audioParams = null);

	public abstract (EntityUid Entity, AudioComponent Component)? PlayPvs(ResolvedSoundSpecifier? filename, EntityUid uid, AudioParams? audioParams = null);

	public abstract (EntityUid Entity, AudioComponent Component)? PlayLocal(SoundSpecifier? sound, EntityUid source, EntityUid? soundInitiator, AudioParams? audioParams = null);

	public abstract (EntityUid Entity, AudioComponent Component)? PlayPredicted(SoundSpecifier? sound, EntityUid source, EntityUid? user, AudioParams? audioParams = null);

	public abstract (EntityUid Entity, AudioComponent Component)? PlayPredicted(SoundSpecifier? sound, EntityCoordinates coordinates, EntityUid? user, AudioParams? audioParams = null);

	public abstract (EntityUid Entity, AudioComponent Component)? PlayStatic(ResolvedSoundSpecifier? filename, Filter playerFilter, EntityCoordinates coordinates, bool recordReplay, AudioParams? audioParams = null);

	public abstract (EntityUid Entity, AudioComponent Component)? PlayStatic(ResolvedSoundSpecifier? filename, ICommonSession recipient, EntityCoordinates coordinates, AudioParams? audioParams = null);

	public abstract (EntityUid Entity, AudioComponent Component)? PlayStatic(ResolvedSoundSpecifier? filename, EntityUid recipient, EntityCoordinates coordinates, AudioParams? audioParams = null);

	public (EntityUid Entity, AudioComponent Component)? PlayStatic(SoundSpecifier? sound, Filter playerFilter, EntityCoordinates coordinates, bool recordReplay, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayStatic(ResolveSound(sound), playerFilter, coordinates, recordReplay, audioParams ?? sound.Params);
		}
		return null;
	}

	public (EntityUid Entity, AudioComponent Component)? PlayStatic(SoundSpecifier? sound, ICommonSession recipient, EntityCoordinates coordinates, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayStatic(ResolveSound(sound), recipient, coordinates, audioParams ?? sound.Params);
		}
		return null;
	}

	public (EntityUid Entity, AudioComponent Component)? PlayStatic(SoundSpecifier? sound, EntityUid recipient, EntityCoordinates coordinates, AudioParams? audioParams = null)
	{
		if (sound != null)
		{
			return PlayStatic(ResolveSound(sound), recipient, coordinates, audioParams ?? sound.Params);
		}
		return null;
	}

	public bool IsPlaying(EntityUid? stream, AudioComponent? component = null)
	{
		if (!stream.HasValue || !Resolve(stream.Value, ref component, logMissing: false))
		{
			return false;
		}
		return component.State == AudioState.Playing;
	}

	protected virtual void InitializeEffect()
	{
		SubscribeLocalEvent<AudioPresetComponent, ComponentStartup>(OnPresetStartup);
		SubscribeLocalEvent<AudioPresetComponent, ComponentShutdown>(OnPresetShutdown);
	}

	private void OnPresetStartup(EntityUid uid, AudioPresetComponent component, ComponentStartup args)
	{
		_auxiliaries[component.Preset] = uid;
	}

	private void OnPresetShutdown(EntityUid uid, AudioPresetComponent component, ComponentShutdown args)
	{
		_auxiliaries.Remove(component.Preset);
	}

	public virtual (EntityUid Entity, AudioAuxiliaryComponent Component) CreateAuxiliary()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entityUid = Spawn(null, MapCoordinates.Nullspace);
		AudioAuxiliaryComponent item = AddComp<AudioAuxiliaryComponent>(entityUid);
		return (Entity: entityUid, Component: item);
	}

	public virtual (EntityUid Entity, AudioEffectComponent Component) CreateEffect()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entityUid = Spawn(null, MapCoordinates.Nullspace);
		AudioEffectComponent item = AddComp<AudioEffectComponent>(entityUid);
		return (Entity: entityUid, Component: item);
	}

	public virtual void SetAuxiliary(EntityUid uid, AudioComponent audio, EntityUid? auxUid)
	{
		audio.Auxiliary = auxUid;
		Dirty(uid, audio);
	}

	public virtual void SetEffect(EntityUid auxUid, AudioAuxiliaryComponent aux, EntityUid? effectUid)
	{
		aux.Effect = effectUid;
		Dirty(auxUid, aux);
	}

	public void SetEffect(EntityUid? audioUid, AudioComponent? component, string effectProto)
	{
		if (audioUid.HasValue && component != null)
		{
			SetAuxiliary(audioUid.Value, component, _auxiliaries[effectProto]);
		}
	}

	public void SetEffectPreset(EntityUid effectUid, AudioEffectComponent effectComp, AudioPresetPrototype preset)
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
		Dirty(effectUid, effectComp);
	}

	public void SetEffectPreset(EntityUid effectUid, AudioEffectComponent effectComp, ReverbProperties preset)
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
		Dirty(effectUid, effectComp);
	}
}
