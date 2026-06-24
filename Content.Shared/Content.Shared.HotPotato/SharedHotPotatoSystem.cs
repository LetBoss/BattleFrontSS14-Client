using System;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Shared.HotPotato;

public abstract class SharedHotPotatoSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HotPotatoComponent, ContainerGettingRemovedAttemptEvent>((ComponentEventHandler<HotPotatoComponent, ContainerGettingRemovedAttemptEvent>)OnRemoveAttempt, (Type[])null, (Type[])null);
	}

	private void OnRemoveAttempt(EntityUid uid, HotPotatoComponent comp, ContainerGettingRemovedAttemptEvent args)
	{
		if (!comp.CanTransfer)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
