using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Audio;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Audio;

public sealed class AmbientSoundSystem : SharedAmbientSoundSystem
{
	private readonly struct QueryState(Vector2 mapPos, TransformComponent player, SharedTransformSystem transformSystem)
	{
		public readonly Dictionary<string, List<(float Importance, Entity<AmbientSoundComponent>)>> SourceDict = new Dictionary<string, List<(float, Entity<AmbientSoundComponent>)>>();

		public readonly Vector2 MapPos = mapPos;

		public readonly TransformComponent Player = player;

		public readonly SharedTransformSystem TransformSystem = transformSystem;
	}

	[Dependency]
	private AmbientSoundTreeSystem _treeSys;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private ContentAudioSystem _contentAudio;

	[Dependency]
	private SharedTransformSystem _xformSystem;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IRobustRandom _random;

	private AmbientSoundOverlay? _overlay;

	private int _maxAmbientCount;

	private bool _overlayEnabled;

	private float _maxAmbientRange;

	private float _cooldown;

	private TimeSpan _targetTime = TimeSpan.Zero;

	private float _ambienceVolume;

	private static AudioParams _params;

	private readonly Dictionary<Entity<AmbientSoundComponent>, (EntityUid? Stream, SoundSpecifier Sound, string Path)> _playingSounds = new Dictionary<Entity<AmbientSoundComponent>, (EntityUid?, SoundSpecifier, string)>();

	private readonly Dictionary<string, int> _playingCount = new Dictionary<string, int>();

	private Vector2 MaxAmbientVector => new Vector2(_maxAmbientRange, _maxAmbientRange);

	private int MaxSingleSound => (int)((float)_maxAmbientCount / 2.6666667f);

	public bool OverlayEnabled
	{
		get
		{
			return _overlayEnabled;
		}
		set
		{
			if (_overlayEnabled != value)
			{
				_overlayEnabled = value;
				IOverlayManager val = IoCManager.Resolve<IOverlayManager>();
				if (_overlayEnabled)
				{
					_overlay = new AmbientSoundOverlay((IEntityManager)(object)((EntitySystem)this).EntityManager, this, ((EntitySystem)this).EntityManager.System<EntityLookupSystem>());
					val.AddOverlay((Overlay)(object)_overlay);
				}
				else
				{
					val.RemoveOverlay((Overlay)(object)_overlay);
					_overlay = null;
				}
			}
		}
	}

	protected override void QueueUpdate(EntityUid uid, AmbientSoundComponent ambience)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ComponentTreeSystem<AmbientSoundTreeComponent, AmbientSoundComponent>)(object)_treeSys).QueueTreeUpdate(uid, ambience, (TransformComponent)null);
	}

	public bool IsActive(Entity<AmbientSoundComponent> component)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _playingSounds.ContainsKey(component);
	}

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		((EntitySystem)this).UpdatesAfter.Add(typeof(AmbientSoundTreeSystem));
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _cfg, CCVars.AmbientCooldown, (Action<float>)SetCooldown, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _cfg, CCVars.MaxAmbientSources, (Action<int>)SetAmbientCount, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _cfg, CCVars.AmbientRange, (Action<float>)SetAmbientRange, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _cfg, CCVars.AmbienceVolume, (Action<float>)SetAmbienceGain, true);
		((EntitySystem)this).SubscribeLocalEvent<AmbientSoundComponent, ComponentShutdown>((ComponentEventHandler<AmbientSoundComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnShutdown(EntityUid uid, AmbientSoundComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (_playingSounds.Remove(Entity<AmbientSoundComponent>.op_Implicit((uid, component)), out (EntityUid?, SoundSpecifier, string) value))
		{
			_audio.Stop(value.Item1, (AudioComponent)null);
			_playingCount[value.Item3]--;
			if (_playingCount[value.Item3] == 0)
			{
				_playingCount.Remove(value.Item3);
			}
		}
	}

	private void SetAmbienceGain(float value)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		_ambienceVolume = SharedAudioSystem.GainToVolume(value);
		foreach (var (val2, tuple2) in _playingSounds)
		{
			if (tuple2.Item1.HasValue)
			{
				var (val3, _, _) = tuple2;
				_audio.SetVolume(val3, ((AudioParams)(ref _params)).Volume + val2.Comp.Volume + _ambienceVolume, (AudioComponent)null);
			}
		}
	}

	private void SetCooldown(float value)
	{
		_cooldown = value;
	}

	private void SetAmbientCount(int value)
	{
		_maxAmbientCount = value;
	}

	private void SetAmbientRange(float value)
	{
		_maxAmbientRange = value;
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		ClearSounds();
	}

	private int PlayingCount(string countSound)
	{
		int num = 0;
		foreach (KeyValuePair<Entity<AmbientSoundComponent>, (EntityUid?, SoundSpecifier, string)> playingSound in _playingSounds)
		{
			playingSound.Deconstruct(out var _, out var value);
			if (value.Item3.Equals(countSound))
			{
				num++;
			}
		}
		return num;
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (_gameTiming.IsFirstTimePredicted && !(_cooldown <= 0f) && !(_gameTiming.CurTime < _targetTime))
		{
			_targetTime = _gameTiming.CurTime + TimeSpan.FromSeconds(_cooldown);
			EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
			TransformComponent playerXform = default(TransformComponent);
			if (!((EntitySystem)this).TryComp(localEntity, ref playerXform))
			{
				ClearSounds();
			}
			else
			{
				ProcessNearbyAmbience(playerXform);
			}
		}
	}

	private void ClearSounds()
	{
		foreach (var value in _playingSounds.Values)
		{
			EntityUid? item = value.Stream;
			_audio.Stop(item, (AudioComponent)null);
		}
		_playingSounds.Clear();
		_playingCount.Clear();
	}

	private static bool Callback(ref QueryState state, in ComponentTreeEntry<AmbientSoundComponent> value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		AmbientSoundComponent ambientSoundComponent = default(AmbientSoundComponent);
		TransformComponent val = default(TransformComponent);
		value.Deconstruct(ref ambientSoundComponent, ref val);
		AmbientSoundComponent ambientSoundComponent2 = ambientSoundComponent;
		TransformComponent val2 = val;
		float num = ((val2.ParentUid == state.Player.ParentUid) ? (val2.LocalPosition - state.Player.LocalPosition) : (state.TransformSystem.GetWorldPosition(val2) - state.MapPos)).Length();
		if (num >= ambientSoundComponent2.Range)
		{
			return true;
		}
		SoundSpecifier sound = ambientSoundComponent2.Sound;
		SoundPathSpecifier val3 = (SoundPathSpecifier)(object)((sound is SoundPathSpecifier) ? sound : null);
		string text = ((val3 == null) ? (((SoundCollectionSpecifier)ambientSoundComponent2.Sound).Collection ?? string.Empty) : ((object)val3.Path/*cast due to constrained. prefix*/).ToString());
		float item = num * (ambientSoundComponent2.Volume + 32f);
		Extensions.GetOrNew<string, List<(float, Entity<AmbientSoundComponent>)>>(state.SourceDict, text).Add((item, Entity<AmbientSoundComponent>.op_Implicit((value.Uid, ambientSoundComponent2))));
		return true;
	}

	private void ProcessNearbyAmbience(TransformComponent playerXform)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery<TransformComponent> entityQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		EntityQuery<MetaDataComponent> entityQuery2 = ((EntitySystem)this).GetEntityQuery<MetaDataComponent>();
		MapCoordinates mapCoordinates = _xformSystem.GetMapCoordinates(playerXform);
		TransformComponent val = default(TransformComponent);
		string key3;
		foreach (KeyValuePair<Entity<AmbientSoundComponent>, (EntityUid?, SoundSpecifier, string)> playingSound in _playingSounds)
		{
			playingSound.Deconstruct(out var key, out var value);
			Entity<AmbientSoundComponent> key2 = key;
			(EntityUid?, SoundSpecifier, string) tuple = value;
			EntityUid owner = key2.Owner;
			AmbientSoundComponent comp = key2.Comp;
			if (!comp.Enabled || tuple.Item2 != comp.Sound || !entityQuery.TryGetComponent(owner, ref val) || !(val.MapID == playerXform.MapID) || entityQuery2.GetComponent(owner).EntityPaused || !(((val.ParentUid == playerXform.ParentUid) ? (val.LocalPosition - playerXform.LocalPosition) : (_xformSystem.GetWorldPosition(val) - mapCoordinates.Position)).LengthSquared() < comp.Range * comp.Range))
			{
				_contentAudio.FadeOut(tuple.Item1);
				_playingSounds.Remove(key2);
				Dictionary<string, int> playingCount = _playingCount;
				key3 = tuple.Item3;
				playingCount[key3]--;
				if (_playingCount[tuple.Item3] == 0)
				{
					_playingCount.Remove(tuple.Item3);
				}
			}
		}
		if (_playingSounds.Count >= _maxAmbientCount)
		{
			return;
		}
		Vector2 position = mapCoordinates.Position;
		QueryState queryState = new QueryState(position, playerXform, _xformSystem);
		Box2 val2 = default(Box2);
		((Box2)(ref val2))._002Ector(position - MaxAmbientVector, position + MaxAmbientVector);
		((ComponentTreeSystem<AmbientSoundTreeComponent, AmbientSoundComponent>)(object)_treeSys).QueryAabb<QueryState>(ref queryState, (QueryCallbackDelegate<ComponentTreeEntry<AmbientSoundComponent>, QueryState>)Callback, mapCoordinates.MapId, val2, true);
		foreach (KeyValuePair<string, List<(float, Entity<AmbientSoundComponent>)>> item2 in queryState.SourceDict)
		{
			item2.Deconstruct(out key3, out var value2);
			string text = key3;
			List<(float, Entity<AmbientSoundComponent>)> list = value2;
			if (_playingSounds.Count >= _maxAmbientCount)
			{
				break;
			}
			if (_playingCount.TryGetValue(text, out var value3) && value3 >= MaxSingleSound)
			{
				continue;
			}
			list.Sort(((float Importance, Entity<AmbientSoundComponent>) a, (float Importance, Entity<AmbientSoundComponent>) b) => b.Importance.CompareTo(a.Importance));
			foreach (var item3 in list)
			{
				Entity<AmbientSoundComponent> item = item3.Item2;
				EntityUid owner2 = item.Owner;
				AmbientSoundComponent comp2 = item.Comp;
				if (_playingSounds.ContainsKey(item) || entityQuery2.GetComponent(owner2).EntityPaused)
				{
					continue;
				}
				AudioParams val3 = ((AudioParams)(ref _params)).AddVolume(comp2.Volume + _ambienceVolume);
				val3 = ((AudioParams)(ref val3)).WithPlayOffset(_random.NextFloat(0f, 100f));
				AudioParams value4 = ((AudioParams)(ref val3)).WithMaxDistance(comp2.Range);
				(EntityUid, AudioComponent)? tuple2 = _audio.PlayEntity(comp2.Sound, Filter.Local(), owner2, false, (AudioParams?)value4);
				if (tuple2.HasValue)
				{
					_playingSounds[item] = (tuple2.Value.Item1, comp2.Sound, text);
					value3++;
					if (_playingSounds.Count >= _maxAmbientCount)
					{
						break;
					}
				}
			}
			if (value3 != 0)
			{
				_playingCount[text] = value3;
			}
		}
	}

	static AmbientSoundSystem()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		AudioParams val = ((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.01f);
		val = ((AudioParams)(ref val)).WithLoop(true);
		_params = ((AudioParams)(ref val)).WithMaxDistance(7f);
	}
}
