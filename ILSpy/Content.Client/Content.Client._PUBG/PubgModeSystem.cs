using System;
using Content.Shared._PUBG;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG;

public sealed class PubgModeSystem : EntitySystem
{
	public bool IsPubgModeActive { get; private set; }

	public int TeamSize { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgModeStatusEvent>((EntitySessionEventHandler<PubgModeStatusEvent>)OnPubgModeStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgTeamModeStatusEvent>((EntitySessionEventHandler<PubgTeamModeStatusEvent>)OnPubgTeamStatus, (Type[])null, (Type[])null);
	}

	private void OnPubgModeStatus(PubgModeStatusEvent ev, EntitySessionEventArgs args)
	{
		IsPubgModeActive = ev.Enabled;
		if (!ev.Enabled)
		{
			TeamSize = 0;
		}
	}

	private void OnPubgTeamStatus(PubgTeamModeStatusEvent ev, EntitySessionEventArgs args)
	{
		IsPubgModeActive = ev.Enabled;
		TeamSize = ev.TeamSize;
	}
}
