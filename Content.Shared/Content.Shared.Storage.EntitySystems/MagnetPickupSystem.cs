using System;
using Content.Shared.Inventory;
using Content.Shared.Storage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Shared.Storage.EntitySystems;

public sealed class MagnetPickupSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedStorageSystem _storage;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	private static readonly TimeSpan ScanDelay = TimeSpan.FromSeconds(1L);

	private EntityQuery<PhysicsComponent> _physicsQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		((EntitySystem)this).SubscribeLocalEvent<MagnetPickupComponent, MapInitEvent>((ComponentEventHandler<MagnetPickupComponent, MapInitEvent>)OnMagnetMapInit, (Type[])null, (Type[])null);
	}

	private void OnMagnetMapInit(EntityUid uid, MagnetPickupComponent component, MapInitEvent args)
	{
		component.NextScan = _timing.CurTime;
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<MagnetPickupComponent, StorageComponent, TransformComponent, MetaDataComponent> query = ((EntitySystem)this).EntityQueryEnumerator<MagnetPickupComponent, StorageComponent, TransformComponent, MetaDataComponent>();
		TimeSpan currentTime = _timing.CurTime;
		EntityUid uid = default(EntityUid);
		MagnetPickupComponent comp = default(MagnetPickupComponent);
		StorageComponent storage = default(StorageComponent);
		TransformComponent xform = default(TransformComponent);
		MetaDataComponent meta = default(MetaDataComponent);
		PhysicsComponent physics = default(PhysicsComponent);
		while (query.MoveNext(ref uid, ref comp, ref storage, ref xform, ref meta))
		{
			if (comp.NextScan > currentTime)
			{
				continue;
			}
			comp.NextScan += ScanDelay;
			if (!_inventory.TryGetContainingSlot(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, xform, meta)), out SlotDefinition slotDef) || (slotDef.SlotFlags & comp.SlotFlags) == 0 || !_storage.HasSpace(Entity<StorageComponent>.op_Implicit((uid, storage))))
			{
				continue;
			}
			EntityUid parentUid = xform.ParentUid;
			bool playedSound = false;
			EntityCoordinates finalCoords = xform.Coordinates;
			EntityCoordinates moverCoords = _transform.GetMoverCoordinates(uid, xform);
			foreach (EntityUid near in _lookup.GetEntitiesInRange(uid, comp.Range, (LookupFlags)10))
			{
				if (_whitelistSystem.IsWhitelistFail(storage.Whitelist, near) || !_physicsQuery.TryGetComponent(near, ref physics) || (int)physics.BodyStatus != 0 || near == parentUid)
				{
					continue;
				}
				TransformComponent nearXform = ((EntitySystem)this).Transform(near);
				MapCoordinates nearMap = _transform.GetMapCoordinates(near, nearXform);
				EntityCoordinates nearCoords = _transform.ToCoordinates(Entity<TransformComponent>.op_Implicit(moverCoords.EntityId), nearMap);
				SharedStorageSystem storage2 = _storage;
				EntityUid uid2 = uid;
				StorageComponent storageComp = storage;
				bool playSound = !playedSound;
				if (storage2.Insert(uid2, near, out var stacked, null, storageComp, playSound))
				{
					if (stacked.HasValue)
					{
						_storage.PlayPickupAnimation(stacked.Value, nearCoords, finalCoords, nearXform.LocalRotation);
					}
					else
					{
						_storage.PlayPickupAnimation(near, nearCoords, finalCoords, nearXform.LocalRotation);
					}
					playedSound = true;
				}
			}
		}
	}
}
