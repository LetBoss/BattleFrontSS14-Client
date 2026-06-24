using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Vendors;

public sealed class RMCVendorUserRechargeSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _gameTiming;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCVendorUserRechargeComponent, ComponentStartup>((EntityEventRefHandler<RMCVendorUserRechargeComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<RMCVendorUserRechargeComponent> ent, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.LastUpdate == TimeSpan.Zero)
		{
			ent.Comp.LastUpdate = _gameTiming.CurTime;
		}
	}
}
