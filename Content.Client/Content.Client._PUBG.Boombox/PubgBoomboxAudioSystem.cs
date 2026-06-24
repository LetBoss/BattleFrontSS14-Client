using System;
using System.Collections.Generic;
using Content.Shared._PUBG.Boombox;
using Content.Shared.CCVar;
using Robust.Client.Audio;
using Robust.Client.ResourceManagement;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._PUBG.Boombox;

public sealed class PubgBoomboxAudioSystem : EntitySystem
{
	private sealed class ActiveAudio
	{
		public string TrackId = string.Empty;

		public EntityUid AudioEntity;

		public AudioComponent? AudioComp;

		public TimeSpan PlaybackStart;

		public float Volume;

		public float MaxDistance;
	}

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IResourceManager _res;

	[Dependency]
	private AudioSystem _audio;

	[Dependency]
	private PubgBoomboxCacheSystem _cache;

	private static readonly ResPath Prefix = ResPath.Root / "PubgBoombox";

	private const int MaxLoadedResources = 3;

	private const float ResyncToleranceSeconds = 0.5f;

	private static readonly MemoryContentRoot ContentRoot = new MemoryContentRoot();

	private static bool _rootRegistered;

	private readonly Dictionary<string, AudioResource> _resources = new Dictionary<string, AudioResource>();

	private readonly List<string> _resourceOrder = new List<string>();

	private readonly Dictionary<EntityUid, ActiveAudio> _active = new Dictionary<EntityUid, ActiveAudio>();

	private bool _enabled = true;

	private float _gain = 1f;

	private float _pollTimer;

	public override void Initialize()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		if (!_rootRegistered)
		{
			_res.AddRoot(Prefix, (IContentRoot)(object)ContentRoot);
			_rootRegistered = true;
		}
		_cfg.OnValueChanged<bool>(CCVars.PubgBoomboxSoundEnabled, (Action<bool>)OnEnabledChanged, true);
		_cfg.OnValueChanged<float>(CCVars.PubgBoomboxVolume, (Action<float>)OnGainChanged, true);
		((EntitySystem)this).SubscribeLocalEvent<PubgBoomboxComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<PubgBoomboxComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgBoomboxComponent, ComponentShutdown>((EntityEventRefHandler<PubgBoomboxComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		_cache.TrackReady += OnTrackReady;
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_cfg.UnsubValueChanged<bool>(CCVars.PubgBoomboxSoundEnabled, (Action<bool>)OnEnabledChanged);
		_cfg.UnsubValueChanged<float>(CCVars.PubgBoomboxVolume, (Action<float>)OnGainChanged);
		_cache.TrackReady -= OnTrackReady;
	}

	private void OnEnabledChanged(bool enabled)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		_enabled = enabled;
		if (enabled)
		{
			return;
		}
		foreach (EntityUid item in new List<EntityUid>(_active.Keys))
		{
			StopActive(item);
		}
	}

	private void OnGainChanged(float gain)
	{
		_gain = gain;
		foreach (ActiveAudio value in _active.Values)
		{
			ApplyLiveVolume(value);
		}
	}

	private void ApplyLiveVolume(ActiveAudio active)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		AudioComponent audioComp = active.AudioComp;
		if (audioComp != null && ((EntitySystem)this).Exists(active.AudioEntity) && !((EntitySystem)this).TerminatingOrDeleted(active.AudioEntity, (MetaDataComponent)null))
		{
			((SharedAudioSystem)_audio).SetVolume((EntityUid?)active.AudioEntity, SharedAudioSystem.GainToVolume(active.Volume * _gain), audioComp);
		}
	}

	private void OnHandleState(Entity<PubgBoomboxComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Reconcile(ent);
	}

	private void OnShutdown(Entity<PubgBoomboxComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		StopActive(Entity<PubgBoomboxComponent>.op_Implicit(ent));
	}

	private void OnTrackReady(string trackId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<PubgBoomboxComponent> val = ((EntitySystem)this).EntityQueryEnumerator<PubgBoomboxComponent>();
		EntityUid item = default(EntityUid);
		PubgBoomboxComponent pubgBoomboxComponent = default(PubgBoomboxComponent);
		while (val.MoveNext(ref item, ref pubgBoomboxComponent))
		{
			if (pubgBoomboxComponent.Playing && pubgBoomboxComponent.TrackId == trackId)
			{
				Reconcile(Entity<PubgBoomboxComponent>.op_Implicit((item, pubgBoomboxComponent)));
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		_pollTimer += frameTime;
		if (_pollTimer < 1f)
		{
			return;
		}
		_pollTimer = 0f;
		EntityQueryEnumerator<PubgBoomboxComponent> val = ((EntitySystem)this).EntityQueryEnumerator<PubgBoomboxComponent>();
		EntityUid val2 = default(EntityUid);
		PubgBoomboxComponent pubgBoomboxComponent = default(PubgBoomboxComponent);
		while (val.MoveNext(ref val2, ref pubgBoomboxComponent))
		{
			if (pubgBoomboxComponent.Playing || _active.ContainsKey(val2))
			{
				Reconcile(Entity<PubgBoomboxComponent>.op_Implicit((val2, pubgBoomboxComponent)));
			}
		}
	}

	private void Reconcile(Entity<PubgBoomboxComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		Entity<PubgBoomboxComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		PubgBoomboxComponent pubgBoomboxComponent = default(PubgBoomboxComponent);
		val.Deconstruct(ref val2, ref pubgBoomboxComponent);
		EntityUid val3 = val2;
		PubgBoomboxComponent pubgBoomboxComponent2 = pubgBoomboxComponent;
		float num = (float)(_timing.CurTime - pubgBoomboxComponent2.PlaybackStart).TotalSeconds;
		if (!_enabled || !pubgBoomboxComponent2.Playing || pubgBoomboxComponent2.TrackId == null || !(num >= -1f) || !(num < pubgBoomboxComponent2.TrackDuration))
		{
			StopActive(val3);
			return;
		}
		string trackId = pubgBoomboxComponent2.TrackId;
		if (!_cache.HasTrack(trackId))
		{
			StopActive(val3);
			_cache.EnsureTrack(trackId);
			return;
		}
		if (_active.TryGetValue(val3, out ActiveAudio value) && value.TrackId == trackId && Math.Abs((value.PlaybackStart - pubgBoomboxComponent2.PlaybackStart).TotalSeconds) < 0.5 && !((EntitySystem)this).TerminatingOrDeleted(value.AudioEntity, (MetaDataComponent)null) && ((EntitySystem)this).Exists(value.AudioEntity) && MathHelper.CloseTo(value.MaxDistance, pubgBoomboxComponent2.MaxDistance, 1E-07f))
		{
			if (!MathHelper.CloseTo(value.Volume, pubgBoomboxComponent2.Volume, 1E-07f))
			{
				value.Volume = pubgBoomboxComponent2.Volume;
				ApplyLiveVolume(value);
			}
			return;
		}
		StopActive(val3);
		AudioResource val4 = LoadResource(trackId);
		if (val4 == null)
		{
			return;
		}
		AudioParams val5 = ((AudioParams)(ref AudioParams.Default)).WithVolume(SharedAudioSystem.GainToVolume(pubgBoomboxComponent2.Volume * _gain));
		AudioParams value2 = ((AudioParams)(ref val5)).WithMaxDistance(pubgBoomboxComponent2.MaxDistance);
		ResolvedPathSpecifier val6 = new ResolvedPathSpecifier(Prefix / (trackId + ".ogg"));
		(EntityUid, AudioComponent)? tuple = _audio.PlayEntity(val4.AudioStream, val3, (ResolvedSoundSpecifier)(object)val6, (AudioParams?)value2);
		if (tuple.HasValue)
		{
			EntityUid item = tuple.Value.Item1;
			if (num > 0.25f)
			{
				((SharedAudioSystem)_audio).SetPlaybackPosition((Entity<AudioComponent>?)Entity<AudioComponent>.op_Implicit(item), num);
			}
			_active[val3] = new ActiveAudio
			{
				TrackId = trackId,
				AudioEntity = item,
				AudioComp = tuple.Value.Item2,
				PlaybackStart = pubgBoomboxComponent2.PlaybackStart,
				Volume = pubgBoomboxComponent2.Volume,
				MaxDistance = pubgBoomboxComponent2.MaxDistance
			};
		}
	}

	private void StopActive(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (_active.Remove(uid, out ActiveAudio value) && ((EntitySystem)this).Exists(value.AudioEntity) && !((EntitySystem)this).TerminatingOrDeleted(value.AudioEntity, (MetaDataComponent)null))
		{
			((EntitySystem)this).QueueDel((EntityUid?)value.AudioEntity);
		}
	}

	private AudioResource? LoadResource(string trackId)
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (_resources.TryGetValue(trackId, out AudioResource value))
		{
			return value;
		}
		if (!_cache.TryGetTrack(trackId, out byte[] data))
		{
			return null;
		}
		ResPath val = default(ResPath);
		((ResPath)(ref val))._002Ector(trackId + ".ogg");
		try
		{
			ContentRoot.AddOrUpdateFile(val, data);
			AudioResource val2 = new AudioResource();
			((BaseResource)val2).Load(IoCManager.Instance, Prefix / val);
			_resources[trackId] = val2;
			_resourceOrder.Add(trackId);
			while (_resourceOrder.Count > 3)
			{
				string text = _resourceOrder[0];
				_resourceOrder.RemoveAt(0);
				_resources.Remove(text);
				ContentRoot.RemoveFile(new ResPath(text + ".ogg"));
			}
			return val2;
		}
		catch (Exception ex)
		{
			((EntitySystem)this).Log.Warning("[Boombox] Failed to load track " + trackId + ": " + ex.Message);
			ContentRoot.RemoveFile(val);
			return null;
		}
	}
}
