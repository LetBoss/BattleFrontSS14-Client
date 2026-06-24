using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Movement.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Marines.HyperSleep;

public abstract class SharedHyperSleepChamberSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private IGameTiming _timing;

	private EntityQuery<HyperSleepChamberComponent> _hyperSleepQuery;

	private readonly HashSet<EntityUid> _intersecting = new HashSet<EntityUid>();

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_hyperSleepQuery = ((EntitySystem)this).GetEntityQuery<HyperSleepChamberComponent>();
		((EntitySystem)this).SubscribeLocalEvent<HyperSleepChamberComponent, MapInitEvent>((EntityEventRefHandler<HyperSleepChamberComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HyperSleepChamberComponent, ContainerIsInsertingAttemptEvent>((EntityEventRefHandler<HyperSleepChamberComponent, ContainerIsInsertingAttemptEvent>)OnInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HyperSleepChamberComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<HyperSleepChamberComponent, EntInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InsideHyperSleepChamberComponent, MoveInputEvent>((EntityEventRefHandler<InsideHyperSleepChamberComponent, MoveInputEvent>)OnMoveInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OutsideHyperSleepChamberComponent, PreventCollideEvent>((EntityEventRefHandler<OutsideHyperSleepChamberComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<HyperSleepChamberComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_containers.EnsureContainer<Container>(Entity<HyperSleepChamberComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null);
	}

	private void OnInsertAttempt(Entity<HyperSleepChamberComponent> ent, ref ContainerIsInsertingAttemptEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(((ContainerAttemptEventBase)args).EntityUid))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnInserted(Entity<HyperSleepChamberComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			((EntitySystem)this).EnsureComp<InsideHyperSleepChamberComponent>(((ContainerModifiedMessage)args).Entity).Chamber = Entity<HyperSleepChamberComponent>.op_Implicit(ent);
		}
	}

	private void OnMoveInput(Entity<InsideHyperSleepChamberComponent> ent, ref MoveInputEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (args.HasDirectionalMovement && !_timing.ApplyingState)
		{
			EntityUid? chamber = ent.Comp.Chamber;
			if (chamber.HasValue)
			{
				EntityUid chamber2 = chamber.GetValueOrDefault();
				((EntitySystem)this).RemCompDeferred<InsideHyperSleepChamberComponent>(Entity<InsideHyperSleepChamberComponent>.op_Implicit(ent));
				OutsideHyperSleepChamberComponent outside = ((EntitySystem)this).EnsureComp<OutsideHyperSleepChamberComponent>(Entity<InsideHyperSleepChamberComponent>.op_Implicit(ent));
				outside.Chamber = chamber2;
				((EntitySystem)this).Dirty(Entity<InsideHyperSleepChamberComponent>.op_Implicit(ent), (IComponent)(object)outside, (MetaDataComponent)null);
			}
		}
	}

	private void OnPreventCollide(Entity<OutsideHyperSleepChamberComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? chamber = ent.Comp.Chamber;
		EntityUid otherEntity = args.OtherEntity;
		if (chamber.HasValue && chamber.GetValueOrDefault() == otherEntity)
		{
			args.Cancelled = true;
		}
	}

	public void EjectChamber(Entity<HyperSleepChamberComponent?> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (_hyperSleepQuery.Resolve(Entity<HyperSleepChamberComponent>.op_Implicit(ent), ref ent.Comp, false) && _containers.TryGetContainer(Entity<HyperSleepChamberComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			_containers.EmptyContainer(container, false, (EntityCoordinates?)null, true);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<OutsideHyperSleepChamberComponent> query = ((EntitySystem)this).EntityQueryEnumerator<OutsideHyperSleepChamberComponent>();
		EntityUid uid = default(EntityUid);
		OutsideHyperSleepChamberComponent comp = default(OutsideHyperSleepChamberComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			EntityUid? chamber = comp.Chamber;
			if (chamber.HasValue)
			{
				EntityUid chamber2 = chamber.GetValueOrDefault();
				_intersecting.Clear();
				_entityLookup.GetEntitiesIntersecting(uid, _intersecting, (LookupFlags)110);
				if (!_intersecting.Contains(chamber2))
				{
					((EntitySystem)this).RemCompDeferred<OutsideHyperSleepChamberComponent>(uid);
				}
			}
			else
			{
				((EntitySystem)this).RemCompDeferred<OutsideHyperSleepChamberComponent>(uid);
			}
		}
	}
}
