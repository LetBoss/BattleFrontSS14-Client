using System;
using System.Diagnostics.CodeAnalysis;
using Content.Client.Storage.Components;
using Content.Shared.Destructible;
using Content.Shared.Foldable;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Movement.Events;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.Storage.Systems;

public sealed class EntityStorageSystem : SharedEntityStorageSystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, EntityUnpausedEvent>((ComponentEventHandler<EntityStorageComponent, EntityUnpausedEvent>)base.OnEntityUnpausedEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, ComponentInit>((ComponentEventHandler<EntityStorageComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, ComponentStartup>((ComponentEventHandler<EntityStorageComponent, ComponentStartup>)OnComponentStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, ActivateInWorldEvent>((ComponentEventHandler<EntityStorageComponent, ActivateInWorldEvent>)base.OnInteract, (Type[])null, new Type[1] { typeof(LockSystem) });
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, LockToggleAttemptEvent>((ComponentEventRefHandler<EntityStorageComponent, LockToggleAttemptEvent>)base.OnLockToggleAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, DestructionEventArgs>((ComponentEventHandler<EntityStorageComponent, DestructionEventArgs>)base.OnDestruction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<EntityStorageComponent, GetVerbsEvent<InteractionVerb>>)base.AddToggleOpenVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, ContainerRelayMovementEntityEvent>((ComponentEventRefHandler<EntityStorageComponent, ContainerRelayMovementEntityEvent>)base.OnRelayMovement, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, FoldAttemptEvent>((ComponentEventRefHandler<EntityStorageComponent, FoldAttemptEvent>)base.OnFoldAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, ComponentGetState>((ComponentEventRefHandler<EntityStorageComponent, ComponentGetState>)base.OnGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageComponent, ComponentHandleState>((ComponentEventRefHandler<EntityStorageComponent, ComponentHandleState>)base.OnHandleState, (Type[])null, (Type[])null);
	}

	public override bool ResolveStorage(EntityUid uid, [NotNullWhen(true)] ref SharedEntityStorageComponent? component)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (component != null)
		{
			return true;
		}
		EntityStorageComponent entityStorageComponent = default(EntityStorageComponent);
		((EntitySystem)this).TryComp<EntityStorageComponent>(uid, ref entityStorageComponent);
		component = entityStorageComponent;
		return component != null;
	}
}
