using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.UniformAccessories;

public abstract class SharedUniformAccessorySystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<UniformAccessoryHolderComponent, MapInitEvent>((EntityEventRefHandler<UniformAccessoryHolderComponent, MapInitEvent>)OnHolderMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UniformAccessoryHolderComponent, InteractUsingEvent>((EntityEventRefHandler<UniformAccessoryHolderComponent, InteractUsingEvent>)OnHolderInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UniformAccessoryHolderComponent, GotEquippedEvent>((EntityEventRefHandler<UniformAccessoryHolderComponent, GotEquippedEvent>)OnHolderGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UniformAccessoryHolderComponent, GetVerbsEvent<EquipmentVerb>>((EntityEventRefHandler<UniformAccessoryHolderComponent, GetVerbsEvent<EquipmentVerb>>)OnHolderGetEquipmentVerbs, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<UniformAccessoryHolderComponent>(((EntitySystem)this).Subs, (object)UniformAccessoriesUi.Key, (BuiEventSubscriber<UniformAccessoryHolderComponent>)delegate(Subscriber<UniformAccessoryHolderComponent> subs)
		{
			subs.Event<UniformAccessoriesBuiMsg>((EntityEventRefHandler<UniformAccessoryHolderComponent, UniformAccessoriesBuiMsg>)OnAccessoriesBuiMsg);
		});
	}

	private void OnHolderMapInit(Entity<UniformAccessoryHolderComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		_container.EnsureContainer<Container>(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null);
		List<EntProtoId> startingAccessories = ent.Comp.StartingAccessories;
		if (startingAccessories == null || _net.IsClient)
		{
			return;
		}
		foreach (EntProtoId startingEntId in startingAccessories)
		{
			((EntitySystem)this).SpawnInContainerOrDrop(EntProtoId.op_Implicit(startingEntId), ent.Owner, ent.Comp.ContainerId, (TransformComponent)null, (ContainerManagerComponent)null, (ComponentRegistry)null);
		}
	}

	private void OnHolderInteractUsing(Entity<UniformAccessoryHolderComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		UniformAccessoryComponent accessory = default(UniformAccessoryComponent);
		if (!((EntitySystem)this).TryComp<UniformAccessoryComponent>(args.Used, ref accessory))
		{
			return;
		}
		Container container = _container.EnsureContainer<Container>(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null);
		((HandledEntityEventArgs)args).Handled = true;
		NetEntity? user = accessory.User;
		if (user.HasValue)
		{
			NetEntity accessoryUser = user.GetValueOrDefault();
			if (!BelongsToUser(accessoryUser, args.User))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-uniform-accessory-fail"), args.User, args.User, PopupType.SmallCaution);
				_hands.TryDrop(Entity<HandsComponent>.op_Implicit(args.User), Entity<UniformAccessoryHolderComponent>.op_Implicit(ent), null, checkActionBlocker: false);
				return;
			}
		}
		if (!ent.Comp.AllowedCategories.Contains(accessory.Category))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-uniform-accessory-fail-not-allowed"), args.User, args.User, PopupType.SmallCaution);
			return;
		}
		Dictionary<string, int> accessoryDictionary = new Dictionary<string, int>();
		UniformAccessoryComponent insertedComp = default(UniformAccessoryComponent);
		foreach (EntityUid inserted in ((BaseContainer)container).ContainedEntities)
		{
			if (((EntitySystem)this).TryComp<UniformAccessoryComponent>(inserted, ref insertedComp))
			{
				if (accessoryDictionary.TryGetValue(insertedComp.Category, out var count))
				{
					accessoryDictionary[insertedComp.Category] = count + 1;
				}
				else
				{
					accessoryDictionary[insertedComp.Category] = 1;
				}
			}
		}
		if (accessoryDictionary.TryGetValue(accessory.Category, out var amount) && accessory.Limit <= amount)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-uniform-accessory-fail-limit"), args.User, args.User, PopupType.SmallCaution);
			return;
		}
		_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used), (BaseContainer)(object)container, (TransformComponent)null, false);
		_item.VisualsChanged(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
	}

	private void OnHolderGotEquipped(Entity<UniformAccessoryHolderComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		UniformAccessoryComponent accessoryComp = default(UniformAccessoryComponent);
		foreach (EntityUid accessory in container.ContainedEntities)
		{
			if (!((EntitySystem)this).TryComp<UniformAccessoryComponent>(accessory, ref accessoryComp))
			{
				continue;
			}
			NetEntity? user = accessoryComp.User;
			if (user.HasValue)
			{
				NetEntity acccessoryUser = user.GetValueOrDefault();
				if (!BelongsToUser(acccessoryUser, args.Equipee))
				{
					_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(accessory), container, true, false, (EntityCoordinates?)null, (Angle?)null);
					return;
				}
			}
		}
		_item.VisualsChanged(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
	}

	private void OnHolderGetEquipmentVerbs(Entity<UniformAccessoryHolderComponent> ent, ref GetVerbsEvent<EquipmentVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		EntityUid? firstAccessory = default(EntityUid?);
		if (!args.CanAccess || !args.CanInteract || ((EntitySystem)this).HasComp<XenoComponent>(args.User) || !_container.TryGetContainer(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref firstAccessory))
		{
			return;
		}
		EntityUid user = args.User;
		args.Verbs.Add(new EquipmentVerb
		{
			Text = base.Loc.GetString("rmc-uniform-accessory-remove"),
			Act = delegate
			{
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				if (container.ContainedEntities.Count == 1 && firstAccessory.HasValue)
				{
					_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(firstAccessory.Value), container, true, false, (EntityCoordinates?)null, (Angle?)null);
					_hands.TryPickupAnyHand(user, firstAccessory.Value);
					_item.VisualsChanged(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
				}
				else
				{
					_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)UniformAccessoriesUi.Key, (EntityUid?)user, false);
				}
			},
			IconEntity = ((EntitySystem)this).GetNetEntity(firstAccessory, (MetaDataComponent)null)
		});
	}

	private void OnAccessoriesBuiMsg(Entity<UniformAccessoryHolderComponent> ent, ref UniformAccessoriesBuiMsg args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		EntityUid toRemove = ((EntitySystem)this).GetEntity(args.ToRemove);
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainer(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			if (_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(toRemove), container, true, false, (EntityCoordinates?)null, (Angle?)null))
			{
				_hands.TryPickupAnyHand(user, toRemove);
				_item.VisualsChanged(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
			}
			if (container.ContainedEntities.Count <= 1)
			{
				_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)UniformAccessoriesUi.Key);
				return;
			}
			UniformAccessoriesBuiState state = new UniformAccessoriesBuiState();
			_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)UniformAccessoriesUi.Key, (BoundUserInterfaceState)(object)state);
		}
	}

	public bool BelongsToUser(NetEntity user, EntityUid target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return user == ((EntitySystem)this).GetNetEntity(target, (MetaDataComponent)null);
	}

	public void SetAccessoriesHidden(EntityUid accessoryHolder, bool hideAccessories)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		UniformAccessoryHolderComponent comp = default(UniformAccessoryHolderComponent);
		if (((EntitySystem)this).TryComp<UniformAccessoryHolderComponent>(accessoryHolder, ref comp))
		{
			comp.HideAccessories = hideAccessories;
			((EntitySystem)this).Dirty(accessoryHolder, (IComponent)(object)comp, (MetaDataComponent)null);
			_item.VisualsChanged(accessoryHolder);
		}
	}
}
