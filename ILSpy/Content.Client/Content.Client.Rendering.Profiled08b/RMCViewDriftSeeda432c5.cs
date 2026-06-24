using System;
using Content.Shared._RMC14.Weapons;
using Robust.Shared.GameObjects;

namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCViewDriftSeeda432c5 : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RMCWeaponProfileHelloEvent>((EntityEventHandler<RMCWeaponProfileHelloEvent>)_m3138436880cc, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RMCWeaponDrawSkewEvent>((EntityEventHandler<RMCWeaponDrawSkewEvent>)_m8e12e33972bb, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		base.EntityManager.System<RMCProfileCacheNodea4fdbc>()._m9814afeff83e(frameTime);
	}

	private void _m3138436880cc(RMCWeaponProfileHelloEvent ev)
	{
		base.EntityManager.System<RMCProfileCacheNodea4fdbc>()._mf1a84035361f(ev);
	}

	private void _m8e12e33972bb(RMCWeaponDrawSkewEvent ev)
	{
		base.EntityManager.System<RMCProfileCacheNodea4fdbc>()._m5e41c632b8aa(ev);
	}
}
