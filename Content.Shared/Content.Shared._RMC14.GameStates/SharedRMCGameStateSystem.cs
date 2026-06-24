using System;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.GameStates;

public sealed class SharedRMCGameStateSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeNetworkEvent<RMCSetPredictionEvent>((EntityEventHandler<RMCSetPredictionEvent>)OnDisablePrediction, (Type[])null, (Type[])null);
	}

	private void OnDisablePrediction(RMCSetPredictionEvent ev)
	{
		if (!_net.IsServer)
		{
			_config.SetCVar<bool>(CVars.NetPredict, ev.Enable, false);
		}
	}
}
