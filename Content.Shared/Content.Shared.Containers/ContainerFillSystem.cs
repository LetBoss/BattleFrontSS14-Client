using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Prototypes;
using Content.Shared._RMC14.Storage;
using Content.Shared.EntityTable;
using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Containers;

public sealed class ContainerFillSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private EntityTableSystem _entityTable;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ContainerFillComponent, MapInitEvent>((ComponentEventHandler<ContainerFillComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityTableContainerFillComponent, MapInitEvent>((EntityEventRefHandler<EntityTableContainerFillComponent, MapInitEvent>)OnTableMapInit, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, ContainerFillComponent component, MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		ContainerManagerComponent containerComp = default(ContainerManagerComponent);
		if (!((EntitySystem)this).TryComp<ContainerManagerComponent>(uid, ref containerComp))
		{
			return;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		EntityCoordinates coords = default(EntityCoordinates);
		((EntityCoordinates)(ref coords))._002Ector(uid, Vector2.Zero);
		BaseContainer container = default(BaseContainer);
		foreach (var (contaienrId, prototypes) in component.Containers)
		{
			if (!_containerSystem.TryGetContainer(uid, contaienrId, ref container, containerComp))
			{
				((EntitySystem)this).Log.Error($"Entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))} with a {"ContainerFillComponent"} is missing a container ({contaienrId}).");
				continue;
			}
			foreach (string proto in prototypes)
			{
				EntityUid ent = ((EntitySystem)this).Spawn(proto, coords);
				if (!_containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(ent), container, xform, false))
				{
					string alreadyContained = ((container.ContainedEntities.Count > 0) ? string.Join("\n", container.ContainedEntities.Select((EntityUid e) => $"\t - {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(e))}")) : "< empty >");
					if (CMPrototypeExtensions.FilterCM)
					{
						((EntitySystem)this).Log.Error($"Entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))} with a {"ContainerFillComponent"} failed to insert an entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent))}.\nCurrent contents:\n{alreadyContained}");
					}
					_transform.AttachToGridOrMap(ent, (TransformComponent)null);
					break;
				}
			}
		}
	}

	private void OnTableMapInit(Entity<EntityTableContainerFillComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		ContainerManagerComponent containerComp = default(ContainerManagerComponent);
		if (!((EntitySystem)this).TryComp<ContainerManagerComponent>(Entity<EntityTableContainerFillComponent>.op_Implicit(ent), ref containerComp) || ((EntitySystem)this).TerminatingOrDeleted(Entity<EntityTableContainerFillComponent>.op_Implicit(ent), (MetaDataComponent)null) || !((EntitySystem)this).Exists(Entity<EntityTableContainerFillComponent>.op_Implicit(ent)))
		{
			return;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(Entity<EntityTableContainerFillComponent>.op_Implicit(ent));
		EntityCoordinates coords = default(EntityCoordinates);
		((EntityCoordinates)(ref coords))._002Ector(Entity<EntityTableContainerFillComponent>.op_Implicit(ent), Vector2.Zero);
		StorageComponent storage = ((EntitySystem)this).CompOrNull<StorageComponent>(Entity<EntityTableContainerFillComponent>.op_Implicit(ent));
		BaseContainer container = default(BaseContainer);
		ItemComponent item = default(ItemComponent);
		foreach (var (containerId, table) in ent.Comp.Containers)
		{
			if (!_containerSystem.TryGetContainer(Entity<EntityTableContainerFillComponent>.op_Implicit(ent), containerId, ref container, containerComp))
			{
				((EntitySystem)this).Log.Error($"Entity {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<EntityTableContainerFillComponent>.op_Implicit(ent), (MetaDataComponent)null)} with a {"EntityTableContainerFillComponent"} is missing a container ({containerId}).");
				continue;
			}
			foreach (EntProtoId proto in _entityTable.GetSpawns(table))
			{
				EntityUid spawn;
				try
				{
					spawn = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(proto), coords);
				}
				catch (Exception value)
				{
					((EntitySystem)this).Log.Error($"Error spawning {proto} at {coords}:\n{value}");
					continue;
				}
				if (storage != null && ((EntitySystem)this).TryComp<ItemComponent>(spawn, ref item))
				{
					CMStorageItemFillEvent ev = new CMStorageItemFillEvent(Entity<ItemComponent>.op_Implicit((spawn, item)), storage);
					((EntitySystem)this).RaiseLocalEvent<CMStorageItemFillEvent>(Entity<EntityTableContainerFillComponent>.op_Implicit(ent), ref ev, false);
				}
				if (!_containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(spawn), container, xform, false))
				{
					string alreadyContained = ((container.ContainedEntities.Count > 0) ? string.Join("\n", container.ContainedEntities.Select((EntityUid e) => $"\t - {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(e))}")) : "< empty >");
					if (CMPrototypeExtensions.FilterCM)
					{
						((EntitySystem)this).Log.Error($"Entity {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<EntityTableContainerFillComponent>.op_Implicit(ent), (MetaDataComponent)null)} with a {"EntityTableContainerFillComponent"} failed to insert an entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(spawn))}.\nCurrent contents:\n{alreadyContained}");
					}
					_transform.AttachToGridOrMap(spawn, (TransformComponent)null);
					break;
				}
			}
		}
	}
}
