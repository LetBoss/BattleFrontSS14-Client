using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Storage;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class PilotedClothingSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedMoverController _moverController;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PilotedClothingComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<PilotedClothingComponent, EntInsertedIntoContainerMessage>)OnEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PilotedClothingComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<PilotedClothingComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PilotedClothingComponent, GotEquippedEvent>((EntityEventRefHandler<PilotedClothingComponent, GotEquippedEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PilotedClothingComponent, GotUnequippedEvent>((EntityEventRefHandler<PilotedClothingComponent, GotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
	}

	private void OnEntInserted(Entity<PilotedClothingComponent> entity, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		StorageComponent storage = default(StorageComponent);
		if (((EntitySystem)this).TryComp<StorageComponent>(Entity<PilotedClothingComponent>.op_Implicit(entity), ref storage) && (object)((ContainerModifiedMessage)args).Container == storage.Container && !_whitelist.IsWhitelistFail(entity.Comp.PilotWhitelist, ((ContainerModifiedMessage)args).Entity))
		{
			entity.Comp.Pilot = ((ContainerModifiedMessage)args).Entity;
			((EntitySystem)this).Dirty<PilotedClothingComponent>(entity, (MetaDataComponent)null);
			StartPiloting(entity);
		}
	}

	private void OnEntRemoved(Entity<PilotedClothingComponent> entity, ref EntRemovedFromContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity2 = ((ContainerModifiedMessage)args).Entity;
		EntityUid? pilot = entity.Comp.Pilot;
		if (pilot.HasValue && !(entity2 != pilot.GetValueOrDefault()))
		{
			StopPiloting(entity);
			entity.Comp.Pilot = null;
			((EntitySystem)this).Dirty<PilotedClothingComponent>(entity, (MetaDataComponent)null);
		}
	}

	private void OnEquipped(Entity<PilotedClothingComponent> entity, ref GotEquippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		ClothingComponent clothing = default(ClothingComponent);
		if (((EntitySystem)this).TryComp<ClothingComponent>(Entity<PilotedClothingComponent>.op_Implicit(entity), ref clothing) && (clothing.Slots & args.SlotFlags) != SlotFlags.NONE)
		{
			entity.Comp.Wearer = args.Equipee;
			((EntitySystem)this).Dirty<PilotedClothingComponent>(entity, (MetaDataComponent)null);
			StartPiloting(entity);
		}
	}

	private void OnUnequipped(Entity<PilotedClothingComponent> entity, ref GotUnequippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		StopPiloting(entity);
		entity.Comp.Wearer = null;
		((EntitySystem)this).Dirty<PilotedClothingComponent>(entity, (MetaDataComponent)null);
	}

	private bool StartPiloting(Entity<PilotedClothingComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!entity.Comp.Pilot.HasValue || !entity.Comp.Wearer.HasValue)
		{
			return false;
		}
		if (!_timing.IsFirstTimePredicted)
		{
			return false;
		}
		EntityUid pilotEnt = entity.Comp.Pilot.Value;
		EntityUid wearerEnt = entity.Comp.Wearer.Value;
		((EntitySystem)this).EnsureComp<PilotedByClothingComponent>(wearerEnt);
		if (entity.Comp.RelayMovement)
		{
			_moverController.SetRelay(pilotEnt, wearerEnt);
		}
		StartedPilotingClothingEvent pilotEv = new StartedPilotingClothingEvent(Entity<PilotedClothingComponent>.op_Implicit(entity), wearerEnt);
		((EntitySystem)this).RaiseLocalEvent<StartedPilotingClothingEvent>(pilotEnt, ref pilotEv, false);
		StartingBeingPilotedByClothing wearerEv = new StartingBeingPilotedByClothing(Entity<PilotedClothingComponent>.op_Implicit(entity), pilotEnt);
		((EntitySystem)this).RaiseLocalEvent<StartingBeingPilotedByClothing>(wearerEnt, ref wearerEv, false);
		return true;
	}

	private bool StopPiloting(Entity<PilotedClothingComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!entity.Comp.Pilot.HasValue || !entity.Comp.Wearer.HasValue)
		{
			return false;
		}
		EntityUid pilotEnt = entity.Comp.Pilot.Value;
		((EntitySystem)this).RemCompDeferred<RelayInputMoverComponent>(pilotEnt);
		EntityUid wearerEnt = entity.Comp.Wearer.Value;
		((EntitySystem)this).RemCompDeferred<MovementRelayTargetComponent>(wearerEnt);
		((EntitySystem)this).RemCompDeferred<PilotedByClothingComponent>(wearerEnt);
		StoppedPilotingClothingEvent pilotEv = new StoppedPilotingClothingEvent(Entity<PilotedClothingComponent>.op_Implicit(entity), wearerEnt);
		((EntitySystem)this).RaiseLocalEvent<StoppedPilotingClothingEvent>(pilotEnt, ref pilotEv, false);
		StoppedBeingPilotedByClothing wearerEv = new StoppedBeingPilotedByClothing(Entity<PilotedClothingComponent>.op_Implicit(entity), pilotEnt);
		((EntitySystem)this).RaiseLocalEvent<StoppedBeingPilotedByClothing>(wearerEnt, ref wearerEv, false);
		return true;
	}
}
