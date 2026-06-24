using System;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Audio;

public sealed class RMCAudioSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeNetworkEvent<RMCAudioPlayGlobalEvent>((EntityEventHandler<RMCAudioPlayGlobalEvent>)OnPlayGlobal, (Type[])null, (Type[])null);
	}

	private void OnPlayGlobal(RMCAudioPlayGlobalEvent msg)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsServer)
		{
			(EntityUid, AudioComponent)? audio = _audio.PlayGlobal(msg.Sound, Filter.Local(), true, (AudioParams?)msg.AudioParams);
			if (audio.HasValue && !base.EntityManager.HasComponent(audio.Value.Item1, msg.Component, (MetaDataComponent)null))
			{
				base.EntityManager.AddComponent(audio.Value.Item1, msg.Component, (MetaDataComponent)null);
			}
		}
	}

	public void PlayGlobal<T>(SoundSpecifier sound, AudioParams audioParams) where T : IComponent, new()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		ushort? netID = _compFactory.GetRegistration<T>().NetID;
		if (netID.HasValue)
		{
			ushort netId = netID.GetValueOrDefault();
			RMCAudioPlayGlobalEvent ev = new RMCAudioPlayGlobalEvent(sound, audioParams, netId);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev);
		}
		if (!_net.IsClient)
		{
			(EntityUid, AudioComponent)? audio = _audio.PlayGlobal(sound, Filter.Empty(), true, (AudioParams?)audioParams);
			if (audio.HasValue)
			{
				((EntitySystem)this).EnsureComp<T>(audio.Value.Item1);
			}
		}
	}
}
