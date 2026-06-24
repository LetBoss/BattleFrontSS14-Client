using System;
using System.Collections.Generic;
using Content.Client.Clothing;
using Content.Client.Examine;
using Content.Client.Verbs.UI;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Storage;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.ViewVariables;

namespace Content.Client.Inventory;

public sealed class ClientInventorySystem : InventorySystem
{
	public sealed class SlotData
	{
		public SlotDefinition SlotDef;

		public bool Blocked;

		public bool Highlighted;

		[ViewVariables]
		public ContainerSlot? Container;

		public EntityUid? HeldEntity
		{
			get
			{
				ContainerSlot? container = Container;
				if (container == null)
				{
					return null;
				}
				return container.ContainedEntity;
			}
		}

		public bool HasSlotGroup => SlotDef.SlotGroup != "Default";

		public Vector2i ButtonOffset => SlotDef.UIWindowPosition;

		public string SlotName => SlotDef.Name;

		public bool ShowInWindow => SlotDef.ShowInWindow;

		public string SlotGroup => SlotDef.SlotGroup;

		public string SlotDisplayName => SlotDef.DisplayName;

		public string TextureName => "Slots/" + SlotDef.TextureName;

		public string FullTextureName => SlotDef.FullTextureName;

		public SlotData(SlotDefinition slotDef, ContainerSlot? container = null, bool highlighted = false, bool blocked = false)
		{
			SlotDef = slotDef;
			Highlighted = highlighted;
			Blocked = blocked;
			Container = container;
		}

		public SlotData(SlotData oldData, bool highlighted = false, bool blocked = false)
		{
			SlotDef = oldData.SlotDef;
			Highlighted = highlighted;
			Container = oldData.Container;
			Blocked = blocked;
		}

		public static implicit operator SlotData(SlotDefinition s)
		{
			return new SlotData(s);
		}

		public static implicit operator SlotDefinition(SlotData s)
		{
			return s.SlotDef;
		}
	}

	public readonly record struct SlotSpriteUpdate(EntityUid? Entity, string Group, string Name, bool ShowStorage);

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private ClientClothingSystem _clothingVisualsSystem;

	[Dependency]
	private ExamineSystem _examine;

	public Action<SlotData>? EntitySlotUpdate;

	public Action<SlotData>? OnSlotAdded;

	public Action<SlotData>? OnSlotRemoved;

	public Action<EntityUid, InventorySlotsComponent>? OnLinkInventorySlots;

	public Action? OnUnlinkInventory;

	public Action<SlotSpriteUpdate>? OnSpriteUpdate;

	private readonly Queue<(InventorySlotsComponent comp, EntityEventArgs args)> _equipEventsQueue = new Queue<(InventorySlotsComponent, EntityEventArgs)>();

	public override void Initialize()
	{
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<InventorySlotsComponent, LocalPlayerAttachedEvent>((ComponentEventHandler<InventorySlotsComponent, LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventorySlotsComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<InventorySlotsComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, ComponentShutdown>((ComponentEventHandler<InventoryComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventorySlotsComponent, DidEquipEvent>((ComponentEventHandler<InventorySlotsComponent, DidEquipEvent>)delegate(EntityUid _, InventorySlotsComponent comp, DidEquipEvent args)
		{
			_equipEventsQueue.Enqueue((comp, (EntityEventArgs)(object)args));
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventorySlotsComponent, DidUnequipEvent>((ComponentEventHandler<InventorySlotsComponent, DidUnequipEvent>)delegate(EntityUid _, InventorySlotsComponent comp, DidUnequipEvent args)
		{
			_equipEventsQueue.Enqueue((comp, (EntityEventArgs)(object)args));
		}, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		(InventorySlotsComponent, EntityEventArgs) result;
		while (_equipEventsQueue.TryDequeue(out result))
		{
			var (component, e) = result;
			if (!(e is DidEquipEvent args))
			{
				if (!(e is DidUnequipEvent args2))
				{
					throw new InvalidOperationException($"Received queued event of unknown type: {((object)e).GetType()}");
				}
				OnDidUnequip(component, args2);
			}
			else
			{
				OnDidEquip(component, args);
			}
		}
	}

	private void OnDidUnequip(InventorySlotsComponent component, DidUnequipEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		UpdateSlot(args.Equipee, component, args.Slot);
		EntityUid equipee = args.Equipee;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(equipee != localEntity.GetValueOrDefault()))
		{
			SlotSpriteUpdate obj = new SlotSpriteUpdate(null, args.SlotGroup, args.Slot, ShowStorage: false);
			OnSpriteUpdate?.Invoke(obj);
		}
	}

	private void OnDidEquip(InventorySlotsComponent component, DidEquipEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		UpdateSlot(args.Equipee, component, args.Slot);
		EntityUid equipee = args.Equipee;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(equipee != localEntity.GetValueOrDefault()))
		{
			SlotSpriteUpdate obj = new SlotSpriteUpdate(args.Equipment, args.SlotGroup, args.Slot, ((EntitySystem)this).HasComp<StorageComponent>(args.Equipment));
			OnSpriteUpdate?.Invoke(obj);
		}
	}

	private void OnShutdown(EntityUid uid, InventoryComponent component, ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && uid == localEntity.GetValueOrDefault())
		{
			OnUnlinkInventory?.Invoke();
		}
	}

	private void OnPlayerDetached(EntityUid uid, InventorySlotsComponent component, LocalPlayerDetachedEvent args)
	{
		OnUnlinkInventory?.Invoke();
	}

	private void OnPlayerAttached(EntityUid uid, InventorySlotsComponent component, LocalPlayerAttachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSlots(uid, out SlotDefinition[] slotDefinitions))
		{
			SlotDefinition[] array = slotDefinitions;
			foreach (SlotDefinition slotDefinition in array)
			{
				if (TryGetSlotContainer(uid, slotDefinition.Name, out ContainerSlot containerSlot, out SlotDefinition _))
				{
					if (!component.SlotData.TryGetValue(slotDefinition.Name, out SlotData value))
					{
						value = new SlotData(slotDefinition);
						component.SlotData[slotDefinition.Name] = value;
					}
					value.Container = containerSlot;
				}
			}
		}
		OnLinkInventorySlots?.Invoke(uid, component);
	}

	public override void Shutdown()
	{
		CommandBinds.Unregister<ClientInventorySystem>();
		base.Shutdown();
	}

	protected override void OnInit(EntityUid uid, InventoryComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		base.OnInit(uid, component, args);
		_clothingVisualsSystem.InitClothing(uid, component);
		InventorySlotsComponent component2 = default(InventorySlotsComponent);
		if (((EntitySystem)this).TryComp<InventorySlotsComponent>(uid, ref component2))
		{
			SlotDefinition[] slots = component.Slots;
			foreach (SlotDefinition newSlotDef in slots)
			{
				TryAddSlotDef(uid, component2, newSlotDef);
			}
		}
	}

	public void ReloadInventory(InventorySlotsComponent? component = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && ((EntitySystem)this).Resolve<InventorySlotsComponent>(localEntity.Value, ref component, false))
		{
			OnUnlinkInventory?.Invoke();
			OnLinkInventorySlots?.Invoke(localEntity.Value, component);
		}
	}

	public void SetSlotHighlight(EntityUid owner, InventorySlotsComponent component, string slotName, bool state)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		SlotData oldData = component.SlotData[slotName];
		SlotData slotData = (component.SlotData[slotName] = new SlotData(oldData, state));
		SlotData obj = slotData;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && owner == localEntity.GetValueOrDefault())
		{
			EntitySlotUpdate?.Invoke(obj);
		}
	}

	public void UpdateSlot(EntityUid owner, InventorySlotsComponent component, string slotName, bool? blocked = null, bool? highlight = null)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		SlotData slotData = component.SlotData[slotName];
		bool highlighted = slotData.Highlighted;
		bool blocked2 = slotData.Blocked;
		if (blocked.HasValue)
		{
			blocked2 = blocked.Value;
		}
		if (highlight.HasValue)
		{
			highlighted = highlight.Value;
		}
		SlotData slotData2 = (component.SlotData[slotName] = new SlotData(component.SlotData[slotName], highlighted, blocked2));
		SlotData obj = slotData2;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && owner == localEntity.GetValueOrDefault())
		{
			EntitySlotUpdate?.Invoke(obj);
		}
	}

	public bool TryAddSlotDef(EntityUid owner, InventorySlotsComponent component, SlotDefinition newSlotDef)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		TryGetSlotContainer(owner, newSlotDef.Name, out ContainerSlot containerSlot, out SlotDefinition _);
		SlotData slotData = new SlotData(newSlotDef, containerSlot);
		if (!component.SlotData.TryAdd(newSlotDef.Name, slotData))
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && owner == localEntity.GetValueOrDefault())
		{
			OnSlotAdded?.Invoke(slotData);
		}
		return true;
	}

	public void UIInventoryActivate(string slot)
	{
		((EntitySystem)this).RaisePredictiveEvent<UseSlotNetworkMessage>(new UseSlotNetworkMessage(slot));
	}

	public void UIInventoryStorageActivate(string slot)
	{
		((EntitySystem)this).RaisePredictiveEvent<OpenSlotStorageNetworkMessage>(new OpenSlotStorageNetworkMessage(slot));
	}

	public void UIInventoryExamine(string slot, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSlotEntity(uid, slot, out var entityUid))
		{
			_examine.DoExamine(entityUid.Value);
		}
	}

	public void UIInventoryOpenContextMenu(string slot, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSlotEntity(uid, slot, out var entityUid))
		{
			_ui.GetUIController<VerbMenuUIController>().OpenVerbMenu(entityUid.Value);
		}
	}

	public void UIInventoryActivateItem(string slot, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSlotEntity(uid, slot, out var entityUid))
		{
			((EntitySystem)this).RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(((EntitySystem)this).GetNetEntity(entityUid.Value, (MetaDataComponent)null)));
		}
	}

	public void UIInventoryAltActivateItem(string slot, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSlotEntity(uid, slot, out var entityUid))
		{
			((EntitySystem)this).RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(((EntitySystem)this).GetNetEntity(entityUid.Value, (MetaDataComponent)null), altInteract: true));
		}
	}

	protected override void UpdateInventoryTemplate(Entity<InventoryComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateInventoryTemplate(ent);
		InventorySlotsComponent inventorySlotsComponent = default(InventorySlotsComponent);
		if (!((EntitySystem)this).TryComp<InventorySlotsComponent>(Entity<InventoryComponent>.op_Implicit(ent), ref inventorySlotsComponent))
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>(ent.Comp.Slots.Length);
		SlotDefinition[] slots = ent.Comp.Slots;
		foreach (SlotDefinition slotDefinition in slots)
		{
			hashSet.Add(slotDefinition.Name);
			TryGetSlotContainer(ent.Owner, slotDefinition.Name, out ContainerSlot containerSlot, out SlotDefinition _);
			if (inventorySlotsComponent.SlotData.TryGetValue(slotDefinition.Name, out SlotData value))
			{
				value.SlotDef = slotDefinition;
				value.Container = containerSlot;
			}
			else
			{
				TryAddSlotDef(ent.Owner, inventorySlotsComponent, slotDefinition);
			}
		}
		List<string> list = new List<string>();
		foreach (var (item, _) in inventorySlotsComponent.SlotData)
		{
			if (!hashSet.Contains(item))
			{
				list.Add(item);
			}
		}
		EntityUid owner;
		EntityUid? localEntity;
		foreach (string item2 in list)
		{
			if (inventorySlotsComponent.SlotData.TryGetValue(item2, out SlotData value2) && inventorySlotsComponent.SlotData.Remove(item2))
			{
				owner = ent.Owner;
				localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
				if (localEntity.HasValue && owner == localEntity.GetValueOrDefault())
				{
					OnSlotRemoved?.Invoke(value2);
				}
			}
		}
		owner = ent.Owner;
		localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && owner == localEntity.GetValueOrDefault())
		{
			OnLinkInventorySlots?.Invoke(ent.Owner, inventorySlotsComponent);
		}
	}
}
