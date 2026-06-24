using System;
using Content.Shared._RMC14.Medical.Refill;
using Content.Shared._RMC14.Vendors;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Vendors;

public sealed class CMAutomatedVendorSystem : SharedCMAutomatedVendorSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CMAutomatedVendorComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<CMAutomatedVendorComponent, AfterAutoHandleStateEvent>)OnRefresh<CMAutomatedVendorComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMSolutionRefillerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<CMSolutionRefillerComponent, AfterAutoHandleStateEvent>)OnRefresh<CMSolutionRefillerComponent>, (Type[])null, (Type[])null);
	}

	private void OnRefresh<T>(Entity<T> ent, ref AfterAutoHandleStateEvent args) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UserInterfaceComponent val = default(UserInterfaceComponent);
		if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<T>.op_Implicit(ent), ref val))
		{
			return;
		}
		foreach (BoundUserInterface value in val.ClientOpenInterfaces.Values)
		{
			if (value is ICMAutomatedVendorRefreshable iCMAutomatedVendorRefreshable)
			{
				iCMAutomatedVendorRefreshable.Refresh();
			}
		}
	}
}
