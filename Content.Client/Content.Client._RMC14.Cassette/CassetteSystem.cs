using System;
using System.Collections.Generic;
using System.IO;
using Content.Shared._RMC14.Cassette;
using Content.Shared._RMC14.CCVar;
using Content.Shared.GameTicking;
using Content.Shared.Inventory;
using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Cassette;

public sealed class CassetteSystem : SharedCassetteSystem
{
	[Dependency]
	private AudioSystem _audio;

	[Dependency]
	private IAudioManager _audioManager;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private IFileDialogManager _dialogs;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IGameTiming _timing;

	private float _gain;

	private readonly Dictionary<AudioStream, string> _names = new Dictionary<AudioStream, string>();

	public override void Initialize()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.VolumeGainCassettes, (Action<float>)SetGain, true);
		try
		{
			CassetteTapeComponent cassetteTapeComponent = default(CassetteTapeComponent);
			AudioResource val = default(AudioResource);
			foreach (EntityPrototype item in _prototype.EnumeratePrototypes<EntityPrototype>())
			{
				if (!item.TryGetComponent<CassetteTapeComponent>(ref cassetteTapeComponent, _compFactory))
				{
					continue;
				}
				foreach (SoundSpecifier song in cassetteTapeComponent.Songs)
				{
					string audioPath = ((SharedAudioSystem)_audio).GetAudioPath(((SharedAudioSystem)_audio).ResolveSound(song));
					_resourceCache.TryGetResource<AudioResource>(new ResPath(audioPath), ref val);
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error preloading cassette songs:\n{value}");
		}
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_names.Clear();
	}

	private void SetGain(float gain)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		_gain = gain;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		InventorySystem.InventorySlotEnumerator slotEnumerator = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(valueOrDefault));
		ContainerSlot container;
		CassettePlayerComponent cassettePlayerComponent = default(CassettePlayerComponent);
		while (slotEnumerator.MoveNext(out container))
		{
			localEntity = container.ContainedEntity;
			if (localEntity.HasValue)
			{
				EntityUid valueOrDefault2 = localEntity.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<CassettePlayerComponent>(valueOrDefault2, ref cassettePlayerComponent))
				{
					SetAudioGain(cassettePlayerComponent.AudioStream);
					SetAudioGain(cassettePlayerComponent.CustomAudioStream);
				}
			}
		}
	}

	protected override EntityUid? PlayCustomTrack(Entity<CassettePlayerComponent> player, Entity<CassetteTapeComponent> tape)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		base.PlayCustomTrack(player, tape);
		object? customTrack = tape.Comp.CustomTrack;
		AudioStream val = (AudioStream)((customTrack is AudioStream) ? customTrack : null);
		if (val == null)
		{
			return null;
		}
		if (!_timing.IsFirstTimePredicted)
		{
			return null;
		}
		if (!_names.TryGetValue(val, out string value))
		{
			return null;
		}
		AudioParams value2 = ((AudioParams)(ref player.Comp.AudioParams)).WithVolume(SharedAudioSystem.GainToVolume(_gain));
		return _audio.PlayGlobal(val, (ResolvedSoundSpecifier)new ResolvedPathSpecifier(value), (AudioParams?)value2)?.Item1;
	}

	protected override async void ChooseCustomTrack(Entity<CassetteTapeComponent> tape)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_ = 1;
		try
		{
			if (!_timing.IsFirstTimePredicted)
			{
				return;
			}
			FileDialogFilters val = new FileDialogFilters((Group[])(object)new Group[1]
			{
				new Group(new string[1] { "ogg" })
			});
			await using Stream stream = await _dialogs.OpenFile(val, FileAccess.ReadWrite, (FileShare?)null);
			if (stream != null)
			{
				AudioStream val2 = _audioManager.LoadAudioOggVorbis(stream, (string)null);
				tape.Comp.CustomTrack = val2;
				string text = $"/Audio/_RMC14/_CustomCassetteUploads/upload_{_names.Count}.ogg";
				_resourceCache.CacheResource<AudioResource>(text, new AudioResource(val2));
				_names[val2] = text;
				return;
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error choosing custom cassette track:\n{value}");
		}
	}

	private void SetAudioGain(EntityUid? audio)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		AudioComponent val = default(AudioComponent);
		if (((EntitySystem)this).TryComp<AudioComponent>(audio, ref val))
		{
			AudioComponent obj = val;
			AudioParams val2 = val.Params;
			((AudioParams)(ref val2)).Volume = SharedAudioSystem.GainToVolume(_gain);
			obj.Params = val2;
		}
	}
}
