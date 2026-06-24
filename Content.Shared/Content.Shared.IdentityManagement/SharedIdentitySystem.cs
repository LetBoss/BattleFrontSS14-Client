using System;
using Content.Shared.Clothing;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Inventory;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.IdentityManagement;

public abstract class SharedIdentitySystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	private static string SlotName = "identity";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<IdentityComponent, ComponentInit>((ComponentEventHandler<IdentityComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IdentityBlockerComponent, SeeIdentityAttemptEvent>((ComponentEventHandler<IdentityBlockerComponent, SeeIdentityAttemptEvent>)OnSeeIdentity, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IdentityBlockerComponent, InventoryRelayedEvent<SeeIdentityAttemptEvent>>((ComponentEventHandler<IdentityBlockerComponent, InventoryRelayedEvent<SeeIdentityAttemptEvent>>)delegate(EntityUid e, IdentityBlockerComponent c, InventoryRelayedEvent<SeeIdentityAttemptEvent> ev)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			OnSeeIdentity(e, c, ev.Args);
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IdentityBlockerComponent, ItemMaskToggledEvent>((EntityEventRefHandler<IdentityBlockerComponent, ItemMaskToggledEvent>)OnMaskToggled, (Type[])null, (Type[])null);
	}

	private void OnSeeIdentity(EntityUid uid, IdentityBlockerComponent component, SeeIdentityAttemptEvent args)
	{
		if (component.Enabled)
		{
			args.TotalCoverage |= component.Coverage;
			if (args.TotalCoverage == IdentityBlockerCoverage.FULL)
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	protected virtual void OnComponentInit(EntityUid uid, IdentityComponent component, ComponentInit args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.IdentityEntitySlot = _container.EnsureContainer<ContainerSlot>(uid, SlotName, (ContainerManagerComponent)null);
	}

	private void OnMaskToggled(Entity<IdentityBlockerComponent> ent, ref ItemMaskToggledEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Enabled = !args.Mask.Comp.IsToggled;
	}

	public virtual void QueueIdentityUpdate(EntityUid uid)
	{
	}
}
