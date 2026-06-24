using System;
using System.Collections.Generic;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Intel.Tech;

public sealed class RMCNukeAudioSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	private List<SoundPathSpecifier> _audios = new List<SoundPathSpecifier>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCNukeAudioEvent>((EntityEventHandler<RMCNukeAudioEvent>)OnNukeAudio, (Type[])null, (Type[])null);
	}

	private void OnNukeAudio(RMCNukeAudioEvent ev)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		int ranInt = new System.Random().Next(2);
		_audios.Add(new SoundPathSpecifier("/Audio/_RMC14/Machines/Nuke/nuke.ogg", (AudioParams?)null));
		_audios.Add(new SoundPathSpecifier("/Audio/_RMC14/Machines/Nuke/nuke2-1.ogg", (AudioParams?)null));
		_audio.PlayGlobal((SoundSpecifier)(object)_audios[ranInt], Filter.Broadcast(), true, (AudioParams?)null);
	}
}
