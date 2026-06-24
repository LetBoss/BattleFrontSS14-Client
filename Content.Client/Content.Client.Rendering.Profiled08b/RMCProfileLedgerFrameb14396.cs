using System;
using Content.Shared._RMC14.Weapons;
using Robust.Shared.GameObjects;

namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCProfileLedgerFrameb14396 : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RMCWeaponProfilePingEvent>((EntityEventHandler<RMCWeaponProfilePingEvent>)_m0298b3d9346b, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RMCWeaponProfilePulseRequestEvent>((EntityEventHandler<RMCWeaponProfilePulseRequestEvent>)_ma706658768c1, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RMCWeaponProfileFrameRequestEvent>((EntityEventHandler<RMCWeaponProfileFrameRequestEvent>)_ma54add8e8957, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RMCWeaponProfileIntegrityRequestEvent>((EntityEventHandler<RMCWeaponProfileIntegrityRequestEvent>)_m5a00f42b3368, (Type[])null, (Type[])null);
	}

	private void _m0298b3d9346b(RMCWeaponProfilePingEvent ev)
	{
		base.EntityManager.System<RMCProfileCacheNodea4fdbc>()._m09870773dc85(ev);
	}

	private void _ma706658768c1(RMCWeaponProfilePulseRequestEvent ev)
	{
		base.EntityManager.System<RMCProfileCacheNodea4fdbc>()._m2670d1b35bd4(ev);
	}

	private void _ma54add8e8957(RMCWeaponProfileFrameRequestEvent ev)
	{
		base.EntityManager.System<RMCProfileCacheNodea4fdbc>()._mfb1ca1c81369(ev);
	}

	private void _m5a00f42b3368(RMCWeaponProfileIntegrityRequestEvent ev)
	{
		base.EntityManager.System<RMCProfileCacheNodea4fdbc>()._m4ec44a315a26(ev);
	}
}
