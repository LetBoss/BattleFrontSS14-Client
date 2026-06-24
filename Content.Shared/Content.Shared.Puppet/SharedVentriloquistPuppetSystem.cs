using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Puppet;

public abstract class SharedVentriloquistPuppetSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _blocker;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<VentriloquistPuppetComponent, UseAttemptEvent>((ComponentEventHandler<VentriloquistPuppetComponent, UseAttemptEvent>)Cancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentriloquistPuppetComponent, InteractionAttemptEvent>((EntityEventRefHandler<VentriloquistPuppetComponent, InteractionAttemptEvent>)CancelInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentriloquistPuppetComponent, DropAttemptEvent>((ComponentEventHandler<VentriloquistPuppetComponent, DropAttemptEvent>)Cancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentriloquistPuppetComponent, PickupAttemptEvent>((ComponentEventHandler<VentriloquistPuppetComponent, PickupAttemptEvent>)Cancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentriloquistPuppetComponent, UpdateCanMoveEvent>((ComponentEventHandler<VentriloquistPuppetComponent, UpdateCanMoveEvent>)Cancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentriloquistPuppetComponent, EmoteAttemptEvent>((ComponentEventHandler<VentriloquistPuppetComponent, EmoteAttemptEvent>)Cancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentriloquistPuppetComponent, ChangeDirectionAttemptEvent>((ComponentEventHandler<VentriloquistPuppetComponent, ChangeDirectionAttemptEvent>)Cancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentriloquistPuppetComponent, ComponentStartup>((ComponentEventHandler<VentriloquistPuppetComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
	}

	private void CancelInteract(Entity<VentriloquistPuppetComponent> ent, ref InteractionAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnStartup(EntityUid uid, VentriloquistPuppetComponent component, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_blocker.UpdateCanMove(uid);
	}

	private void Cancel<T>(EntityUid uid, VentriloquistPuppetComponent component, T args) where T : CancellableEntityEventArgs
	{
		((CancellableEntityEventArgs)args).Cancel();
	}
}
