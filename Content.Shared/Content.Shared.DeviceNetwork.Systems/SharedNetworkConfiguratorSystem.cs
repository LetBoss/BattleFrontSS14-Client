using System;
using Content.Shared.DeviceNetwork.Components;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceNetwork.Systems;

public abstract class SharedNetworkConfiguratorSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NetworkConfiguratorComponent, ActivatableUIOpenAttemptEvent>((ComponentEventHandler<NetworkConfiguratorComponent, ActivatableUIOpenAttemptEvent>)OnUiOpenAttempt, (Type[])null, (Type[])null);
	}

	private void OnUiOpenAttempt(EntityUid uid, NetworkConfiguratorComponent configurator, ActivatableUIOpenAttemptEvent args)
	{
		if (configurator.LinkModeActive)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
