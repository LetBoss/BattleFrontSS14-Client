using System;
using Content.Shared._RMC14.Weapons;
using Robust.Shared.GameObjects;

namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCFrameCacheSeed1c235c : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RMCWeaponProfileCatalogRequestEvent>((EntityEventHandler<RMCWeaponProfileCatalogRequestEvent>)_m493e22efb540, (Type[])null, (Type[])null);
	}

	private void _m493e22efb540(RMCWeaponProfileCatalogRequestEvent ev)
	{
		base.EntityManager.System<RMCProfileCacheNodea4fdbc>()._me13d53b6c217(ev);
	}
}
