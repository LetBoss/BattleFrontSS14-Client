using System;
using System.Collections.Generic;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Chemistry.SmartFridge;

public abstract class SharedRMCSmartFridgeSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	private readonly HashSet<Entity<RMCSmartFridgeComponent>> _smartFridges = new HashSet<Entity<RMCSmartFridgeComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCSmartFridgeComponent, InteractUsingEvent>((EntityEventRefHandler<RMCSmartFridgeComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<RMCSmartFridgeComponent>(((EntitySystem)this).Subs, (object)RMCSmartFridgeUI.Key, (BuiEventSubscriber<RMCSmartFridgeComponent>)delegate(Subscriber<RMCSmartFridgeComponent> subs)
		{
			subs.Event<RMCSmartFridgeVendMsg>((EntityEventRefHandler<RMCSmartFridgeComponent, RMCSmartFridgeVendMsg>)OnVend);
		});
	}

	private void OnInteractUsing(Entity<RMCSmartFridgeComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<RMCSmartFridgeInsertableComponent>(args.Used))
		{
			Container container = _container.EnsureContainer<Container>(Entity<RMCSmartFridgeComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null);
			_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used), (BaseContainer)(object)container, (TransformComponent)null, false);
			((EntitySystem)this).Dirty<RMCSmartFridgeComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void TransferToNearby(EntityCoordinates coords, float range, EntityUid transfer)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		_smartFridges.Clear();
		_entityLookup.GetEntitiesInRange<RMCSmartFridgeComponent>(coords, range, _smartFridges, (LookupFlags)110);
		Entity<RMCSmartFridgeComponent>? fridge = default(Entity<RMCSmartFridgeComponent>?);
		if (Extensions.TryFirstOrNull<Entity<RMCSmartFridgeComponent>>((IEnumerable<Entity<RMCSmartFridgeComponent>>)_smartFridges, ref fridge))
		{
			Container container = _container.EnsureContainer<Container>(Entity<RMCSmartFridgeComponent>.op_Implicit(fridge.Value), fridge.Value.Comp.ContainerId, (ContainerManagerComponent)null);
			_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(transfer), (BaseContainer)(object)container, (TransformComponent)null, false);
			((EntitySystem)this).Dirty<RMCSmartFridgeComponent>(fridge.Value, (MetaDataComponent)null);
		}
	}

	private void OnVend(Entity<RMCSmartFridgeComponent> ent, ref RMCSmartFridgeVendMsg args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? vend = default(EntityUid?);
		BaseContainer container = default(BaseContainer);
		if (((EntitySystem)this).TryGetEntity(args.Vend, ref vend) && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(vend.Value, null)), ref container) && !(container.Owner != ent.Owner) && !(container.ID != ent.Comp.ContainerId) && _container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(vend.Value), container, true, false, (EntityCoordinates?)null, (Angle?)null))
		{
			_hands.TryPickupAnyHand(((BaseBoundUserInterfaceEvent)args).Actor, vend.Value);
		}
	}
}
