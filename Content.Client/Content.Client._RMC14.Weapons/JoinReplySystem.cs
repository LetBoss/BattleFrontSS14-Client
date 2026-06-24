using System;
using Content.Shared._RMC14.Weapons;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Weapons;

public sealed class JoinReplySystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RMCWeaponProfileInitEvent>((EntityEventHandler<RMCWeaponProfileInitEvent>)OnJoinRequest, (Type[])null, (Type[])null);
	}

	private void OnJoinRequest(RMCWeaponProfileInitEvent ev)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponProfileReadyEvent(ev.Nonce, ev.Token));
	}
}
