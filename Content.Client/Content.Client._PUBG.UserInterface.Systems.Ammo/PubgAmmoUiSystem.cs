using System;
using Content.Shared._PUBG;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.UserInterface.Systems.Ammo;

public sealed class PubgAmmoUiSystem : EntitySystem
{
	public event Action<PubgAmmoUpdateEvent>? AmmoUpdated;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgAmmoUpdateEvent>((EntityEventHandler<PubgAmmoUpdateEvent>)OnAmmoUpdate, (Type[])null, (Type[])null);
	}

	private void OnAmmoUpdate(PubgAmmoUpdateEvent msg)
	{
		this.AmmoUpdated?.Invoke(msg);
	}

	public void RequestRefresh()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgAmmoRefreshRequestEvent());
	}
}
