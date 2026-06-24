using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.Animations;
using Content.Shared.Hands;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.Storage.Systems;

public sealed class StorageSystem : SharedStorageSystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private EntityPickupAnimationSystem _entityPickupAnimation;

	private Dictionary<EntityUid, ItemStorageLocation> _oldStoredItems = new Dictionary<EntityUid, ItemStorageLocation>();

	private List<(StorageBoundUserInterface Bui, bool Value)> _queuedBuis = new List<(StorageBoundUserInterface, bool)>();

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StorageComponent, ComponentHandleState>((ComponentEventRefHandler<StorageComponent, ComponentHandleState>)OnStorageHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PickupAnimationEvent>((EntityEventHandler<PickupAnimationEvent>)HandlePickupAnimation, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<AnimateInsertingEntitiesEvent>((EntityEventHandler<AnimateInsertingEntitiesEvent>)HandleAnimatingInsertingEntities, (Type[])null, (Type[])null);
	}

	private void OnStorageHandleState(EntityUid uid, StorageComponent component, ref ComponentHandleState args)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is StorageComponentState storageComponentState))
		{
			return;
		}
		component.Grid.Clear();
		component.Grid.AddRange(storageComponentState.Grid);
		component.MaxItemSize = storageComponentState.MaxItemSize;
		component.Whitelist = storageComponentState.Whitelist;
		component.Blacklist = storageComponentState.Blacklist;
		_oldStoredItems.Clear();
		foreach (KeyValuePair<EntityUid, ItemStorageLocation> storedItem in component.StoredItems)
		{
			_oldStoredItems.Add(storedItem.Key, storedItem.Value);
		}
		component.StoredItems.Clear();
		foreach (KeyValuePair<NetEntity, ItemStorageLocation> storedItem2 in storageComponentState.StoredItems)
		{
			storedItem2.Deconstruct(out var key, out var value);
			NetEntity val = key;
			ItemStorageLocation value2 = value;
			EntityUid key2 = ((EntitySystem)this).EnsureEntity<StorageComponent>(val, uid);
			component.StoredItems[key2] = value2;
		}
		component.SavedLocations.Clear();
		foreach (KeyValuePair<string, List<ItemStorageLocation>> savedLocation in storageComponentState.SavedLocations)
		{
			component.SavedLocations[savedLocation.Key] = new List<ItemStorageLocation>(savedLocation.Value);
		}
		UpdateOccupied(Entity<StorageComponent>.op_Implicit((uid, component)));
		StorageBoundUserInterface storageBoundUserInterface = default(StorageBoundUserInterface);
		if (!component.StoredItems.SequenceEqual(_oldStoredItems) && UI.TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, ref storageBoundUserInterface))
		{
			storageBoundUserInterface.Refresh();
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			BaseContainer val2 = default(BaseContainer);
			StorageBoundUserInterface item = default(StorageBoundUserInterface);
			if (NestedStorage && localEntity.HasValue && ContainerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, null, null)), ref val2) && UI.TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(val2.Owner), (Enum)StorageComponent.StorageUiKey.Key, ref item))
			{
				_queuedBuis.Add((item, false));
			}
		}
	}

	public override void UpdateUI(Entity<StorageComponent?> entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		StorageBoundUserInterface storageBoundUserInterface = default(StorageBoundUserInterface);
		if (UI.TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum)StorageComponent.StorageUiKey.Key, ref storageBoundUserInterface))
		{
			storageBoundUserInterface.Refresh();
		}
	}

	protected override void HideStorageWindow(EntityUid uid, EntityUid actor)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		StorageBoundUserInterface item = default(StorageBoundUserInterface);
		if (UI.TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, ref item))
		{
			_queuedBuis.Add((item, false));
		}
	}

	protected override void ShowStorageWindow(EntityUid uid, EntityUid actor)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		StorageBoundUserInterface item = default(StorageBoundUserInterface);
		if (UI.TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)StorageComponent.StorageUiKey.Key, ref item))
		{
			_queuedBuis.Add((item, true));
		}
	}

	public override void PlayPickupAnimation(EntityUid uid, EntityCoordinates initialCoordinates, EntityCoordinates finalCoordinates, Angle initialRotation, EntityUid? user = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted)
		{
			PickupAnimation(uid, initialCoordinates, finalCoordinates, initialRotation);
		}
	}

	private void HandlePickupAnimation(PickupAnimationEvent msg)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		PickupAnimation(((EntitySystem)this).GetEntity(msg.ItemUid), ((EntitySystem)this).GetCoordinates(msg.InitialPosition), ((EntitySystem)this).GetCoordinates(msg.FinalPosition), msg.InitialAngle);
	}

	public void PickupAnimation(EntityUid item, EntityCoordinates initialCoords, EntityCoordinates finalCoords, Angle initialAngle)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted && !TransformSystem.InRange(finalCoords, initialCoords, 0.1f) && ((EntitySystem)this).Exists(initialCoords.EntityId) && ((EntitySystem)this).Exists(finalCoords.EntityId))
		{
			Vector2 final = Vector2.Transform(TransformSystem.ToMapCoordinates(finalCoords, true).Position, TransformSystem.GetInvWorldMatrix(initialCoords.EntityId));
			_entityPickupAnimation.AnimateEntityPickup(item, initialCoords, final, initialAngle);
		}
	}

	public void HandleAnimatingInsertingEntities(AnimateInsertingEntitiesEvent msg)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent val = default(TransformComponent);
		((EntitySystem)this).TryComp(((EntitySystem)this).GetEntity(msg.Storage), ref val);
		for (int i = 0; msg.StoredEntities.Count > i; i++)
		{
			EntityUid entity = ((EntitySystem)this).GetEntity(msg.StoredEntities[i]);
			NetCoordinates val2 = msg.EntityPositions[i];
			if (((EntitySystem)this).Exists(entity) && val != null)
			{
				_entityPickupAnimation.AnimateEntityPickup(entity, ((EntitySystem)this).GetCoordinates(val2), val.LocalPosition, msg.EntityAngles[i]);
			}
		}
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		foreach (var queuedBui in _queuedBuis)
		{
			var (storageBoundUserInterface, _) = queuedBui;
			if (queuedBui.Value)
			{
				storageBoundUserInterface.Show();
			}
			else
			{
				storageBoundUserInterface.Hide();
			}
		}
		_queuedBuis.Clear();
	}
}
