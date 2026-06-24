using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client._RMC14.UniformAccessories;
using Content.Client._RMC14.Webbing;
using Content.Client.Gameplay;
using Content.Client.Hands.Systems;
using Content.Client.Inventory;
using Content.Client.Storage.Systems;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Content.Client.UserInterface.Systems.Inventory.Widgets;
using Content.Client.UserInterface.Systems.Inventory.Windows;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Hands.Components;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Inventory;

public sealed class InventoryUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnSystemChanged<ClientInventorySystem>, IOnSystemLoaded<ClientInventorySystem>, IOnSystemUnloaded<ClientInventorySystem>, IOnSystemChanged<HandsSystem>, IOnSystemLoaded<HandsSystem>, IOnSystemUnloaded<HandsSystem>, IOnSystemChanged<WebbingSystem>, IOnSystemLoaded<WebbingSystem>, IOnSystemUnloaded<WebbingSystem>, IOnSystemChanged<UniformAccessorySystem>, IOnSystemLoaded<UniformAccessorySystem>, IOnSystemUnloaded<UniformAccessorySystem>
{
	[Dependency]
	private IEntityManager _entities;

	[UISystemDependency]
	private readonly ClientInventorySystem _inventorySystem;

	[UISystemDependency]
	private readonly HandsSystem _handsSystem;

	[UISystemDependency]
	private readonly ContainerSystem _container;

	[UISystemDependency]
	private readonly SpriteSystem _sprite;

	private EntityUid? _playerUid;

	private InventorySlotsComponent? _playerInventory;

	private readonly Dictionary<string, ItemSlotButtonContainer> _slotGroups = new Dictionary<string, ItemSlotButtonContainer>();

	private StrippingWindow? _strippingWindow;

	private ItemSlotButtonContainer? _inventoryHotbar;

	private SlotButton? _inventoryButton;

	private SlotControl? _lastHovered;

	private const string PubgInventoryTemplateId = "pubgHuman";

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, new Action(OnScreenLoad));
	}

	private void OnScreenLoad()
	{
		if (base.UIManager.ActiveScreen != null)
		{
			InventoryGui activeUIWidgetOrNull = base.UIManager.GetActiveUIWidgetOrNull<InventoryGui>();
			if (activeUIWidgetOrNull != null)
			{
				RegisterInventoryButton(activeUIWidgetOrNull.InventoryButton);
			}
		}
	}

	public void OnStateEntered(GameplayState state)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		_strippingWindow = base.UIManager.CreateWindow<StrippingWindow>();
		LayoutContainer.SetAnchorPreset((Control)(object)_strippingWindow, (LayoutPreset)8, false);
		CommandBinds.Builder.Bind(ContentKeyFunctions.OpenInventoryMenu, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			ToggleInventoryBar();
		}, (StateInputCmdDelegate)null, true, true)).Register<ClientInventorySystem>();
	}

	public void OnStateExited(GameplayState state)
	{
		if (_strippingWindow != null)
		{
			if (!((Control)_strippingWindow).Disposed)
			{
				((Control)_strippingWindow).Orphan();
			}
			_strippingWindow = null;
		}
		if (_inventoryHotbar != null)
		{
			((Control)_inventoryHotbar).Visible = false;
		}
		CommandBinds.Unregister<ClientInventorySystem>();
	}

	private SlotButton CreateSlotButton(ClientInventorySystem.SlotData data)
	{
		SlotButton slotButton = new SlotButton(data);
		slotButton.Pressed += ItemPressed;
		slotButton.StoragePressed += StoragePressed;
		slotButton.Hover += SlotButtonHovered;
		return slotButton;
	}

	public void RegisterInventoryBarContainer(ItemSlotButtonContainer inventoryHotbar)
	{
		_inventoryHotbar = inventoryHotbar;
	}

	public void RegisterInventoryButton(SlotButton? button)
	{
		if (_inventoryButton != null)
		{
			_inventoryButton.Pressed -= InventoryButtonPressed;
		}
		if (button != null)
		{
			_inventoryButton = button;
			_inventoryButton.Pressed += InventoryButtonPressed;
		}
	}

	private void InventoryButtonPressed(GUIBoundKeyEventArgs args, SlotControl control)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick))
		{
			ToggleInventoryBar();
		}
	}

	private void UpdateInventoryHotbar(InventorySlotsComponent? clientInv)
	{
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Expected O, but got Unknown
		if (clientInv == null)
		{
			_inventoryHotbar?.ClearButtons();
			if (_inventoryButton != null)
			{
				((Control)_inventoryButton).Visible = false;
			}
			return;
		}
		foreach (var (_, slotData2) in clientInv.SlotData)
		{
			if (!slotData2.ShowInWindow)
			{
				RemoveSlotFromOtherGroups(slotData2.SlotName);
				continue;
			}
			RemoveSlotFromOtherGroups(slotData2.SlotName, slotData2.SlotGroup);
			if (_slotGroups.TryGetValue(slotData2.SlotGroup, out ItemSlotButtonContainer value))
			{
				if (!value.TryGetButton(slotData2.SlotName, out SlotControl button))
				{
					button = CreateSlotButton(slotData2);
					value.AddButton(button);
				}
				bool showStorage = _entities.HasComponent<StorageComponent>(slotData2.HeldEntity);
				ClientInventorySystem.SlotSpriteUpdate update = new ClientInventorySystem.SlotSpriteUpdate(slotData2.HeldEntity, slotData2.SlotGroup, slotData2.SlotName, showStorage);
				SpriteUpdated(update);
			}
		}
		InventoryComponent inventoryComponent = default(InventoryComponent);
		if (_playerUid.HasValue && _entities.TryGetComponent<InventoryComponent>(_playerUid.Value, ref inventoryComponent) && inventoryComponent.TemplateId == "pubgHuman" && _slotGroups.TryGetValue("MainHotbar", out ItemSlotButtonContainer value2))
		{
			value2.ReorderPubgMainHotbar();
		}
		if (_inventoryHotbar == null)
		{
			return;
		}
		List<KeyValuePair<string, ClientInventorySystem.SlotData>> list = clientInv.SlotData.Where<KeyValuePair<string, ClientInventorySystem.SlotData>>((KeyValuePair<string, ClientInventorySystem.SlotData> p) => !p.Value.HasSlotGroup).ToList();
		if (_inventoryButton != null)
		{
			((Control)_inventoryButton).Visible = list.Count != 0;
		}
		if (list.Count == 0)
		{
			return;
		}
		foreach (Control item in new List<Control>((IEnumerable<Control>)((Control)_inventoryHotbar).Children))
		{
			if (!(item is SlotControl))
			{
				((Control)_inventoryHotbar).RemoveChild(item);
			}
		}
		int maxWidth = list.Max((KeyValuePair<string, ClientInventorySystem.SlotData> p) => p.Value.ButtonOffset.X) + 1;
		int num = list.Select((KeyValuePair<string, ClientInventorySystem.SlotData> p) => GetIndex(p.Value.ButtonOffset)).Max();
		_inventoryHotbar.MaxColumns = maxWidth;
		((GridContainer)_inventoryHotbar).Columns = maxWidth;
		for (int num2 = 0; num2 <= num; num2++)
		{
			int index = num2;
			KeyValuePair<string, ClientInventorySystem.SlotData>? keyValuePair2 = Extensions.FirstOrNull<KeyValuePair<string, ClientInventorySystem.SlotData>>((IEnumerable<KeyValuePair<string, ClientInventorySystem.SlotData>>)list, (Func<KeyValuePair<string, ClientInventorySystem.SlotData>, bool>)((KeyValuePair<string, ClientInventorySystem.SlotData> p) => GetIndex(p.Value.ButtonOffset) == index));
			if (keyValuePair2.HasValue)
			{
				KeyValuePair<string, ClientInventorySystem.SlotData> valueOrDefault = keyValuePair2.GetValueOrDefault();
				if (_inventoryHotbar.TryGetButton(valueOrDefault.Key, out SlotControl button2))
				{
					((Control)button2).SetPositionLast();
				}
			}
			else
			{
				((Control)_inventoryHotbar).AddChild(new Control
				{
					MinSize = new Vector2(64f, 64f)
				});
			}
		}
		int GetIndex(Vector2i position)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return position.Y * maxWidth + position.X;
		}
	}

	private void UpdateStrippingWindow(InventorySlotsComponent? clientInv)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (clientInv == null)
		{
			_strippingWindow.InventoryButtons.ClearButtons();
			return;
		}
		foreach (var (_, slotData2) in clientInv.SlotData)
		{
			if (slotData2.ShowInWindow)
			{
				if (!_strippingWindow.InventoryButtons.TryGetButton(slotData2.SlotName, out SlotControl button))
				{
					button = CreateSlotButton(slotData2);
					_strippingWindow.InventoryButtons.AddButton(button, slotData2.ButtonOffset);
				}
				bool showStorage = _entities.HasComponent<StorageComponent>(slotData2.HeldEntity);
				ClientInventorySystem.SlotSpriteUpdate update = new ClientInventorySystem.SlotSpriteUpdate(slotData2.HeldEntity, slotData2.SlotGroup, slotData2.SlotName, showStorage);
				SpriteUpdated(update);
			}
		}
	}

	public void ToggleStrippingMenu()
	{
		UpdateStrippingWindow(_playerInventory);
		if (((BaseWindow)_strippingWindow).IsOpen)
		{
			((BaseWindow)_strippingWindow).Close();
		}
		else
		{
			((BaseWindow)_strippingWindow).Open();
		}
	}

	public void ToggleInventoryBar()
	{
		if (_inventoryHotbar == null)
		{
			((UIController)this).Log.Warning("Tried to toggle inventory bar when none are assigned");
			return;
		}
		UpdateInventoryHotbar(_playerInventory);
		bool visible = !((Control)_inventoryHotbar).Visible;
		((Control)_inventoryHotbar).Visible = visible;
	}

	public void OnSystemLoaded(ClientInventorySystem system)
	{
		ClientInventorySystem inventorySystem = _inventorySystem;
		inventorySystem.OnSlotAdded = (Action<ClientInventorySystem.SlotData>)Delegate.Combine(inventorySystem.OnSlotAdded, new Action<ClientInventorySystem.SlotData>(AddSlot));
		ClientInventorySystem inventorySystem2 = _inventorySystem;
		inventorySystem2.OnSlotRemoved = (Action<ClientInventorySystem.SlotData>)Delegate.Combine(inventorySystem2.OnSlotRemoved, new Action<ClientInventorySystem.SlotData>(RemoveSlot));
		ClientInventorySystem inventorySystem3 = _inventorySystem;
		inventorySystem3.OnLinkInventorySlots = (Action<EntityUid, InventorySlotsComponent>)Delegate.Combine(inventorySystem3.OnLinkInventorySlots, new Action<EntityUid, InventorySlotsComponent>(LoadSlots));
		ClientInventorySystem inventorySystem4 = _inventorySystem;
		inventorySystem4.OnUnlinkInventory = (Action)Delegate.Combine(inventorySystem4.OnUnlinkInventory, new Action(UnloadSlots));
		ClientInventorySystem inventorySystem5 = _inventorySystem;
		inventorySystem5.OnSpriteUpdate = (Action<ClientInventorySystem.SlotSpriteUpdate>)Delegate.Combine(inventorySystem5.OnSpriteUpdate, new Action<ClientInventorySystem.SlotSpriteUpdate>(SpriteUpdated));
	}

	public void OnSystemUnloaded(ClientInventorySystem system)
	{
		ClientInventorySystem inventorySystem = _inventorySystem;
		inventorySystem.OnSlotAdded = (Action<ClientInventorySystem.SlotData>)Delegate.Remove(inventorySystem.OnSlotAdded, new Action<ClientInventorySystem.SlotData>(AddSlot));
		ClientInventorySystem inventorySystem2 = _inventorySystem;
		inventorySystem2.OnSlotRemoved = (Action<ClientInventorySystem.SlotData>)Delegate.Remove(inventorySystem2.OnSlotRemoved, new Action<ClientInventorySystem.SlotData>(RemoveSlot));
		ClientInventorySystem inventorySystem3 = _inventorySystem;
		inventorySystem3.OnLinkInventorySlots = (Action<EntityUid, InventorySlotsComponent>)Delegate.Remove(inventorySystem3.OnLinkInventorySlots, new Action<EntityUid, InventorySlotsComponent>(LoadSlots));
		ClientInventorySystem inventorySystem4 = _inventorySystem;
		inventorySystem4.OnUnlinkInventory = (Action)Delegate.Remove(inventorySystem4.OnUnlinkInventory, new Action(UnloadSlots));
		ClientInventorySystem inventorySystem5 = _inventorySystem;
		inventorySystem5.OnSpriteUpdate = (Action<ClientInventorySystem.SlotSpriteUpdate>)Delegate.Remove(inventorySystem5.OnSpriteUpdate, new Action<ClientInventorySystem.SlotSpriteUpdate>(SpriteUpdated));
	}

	private void ItemPressed(GUIBoundKeyEventArgs args, SlotControl control)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		string slotName = control.SlotName;
		if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIClick)
		{
			_inventorySystem.UIInventoryActivate(control.SlotName);
			((BoundKeyEventArgs)args).Handle();
		}
		else
		{
			if (_playerInventory == null || !_playerUid.HasValue)
			{
				return;
			}
			if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ExamineEntity)
			{
				_inventorySystem.UIInventoryExamine(slotName, _playerUid.Value);
			}
			else if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UseSecondary)
			{
				_inventorySystem.UIInventoryOpenContextMenu(slotName, _playerUid.Value);
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ActivateItemInWorld)
			{
				_inventorySystem.UIInventoryActivateItem(slotName, _playerUid.Value);
			}
			else
			{
				if (!(((BoundKeyEventArgs)args).Function == ContentKeyFunctions.AltActivateItemInWorld))
				{
					return;
				}
				_inventorySystem.UIInventoryAltActivateItem(slotName, _playerUid.Value);
			}
			((BoundKeyEventArgs)args).Handle();
		}
	}

	private void StoragePressed(GUIBoundKeyEventArgs args, SlotControl control)
	{
		_inventorySystem.UIInventoryStorageActivate(control.SlotName);
	}

	private void SlotButtonHovered(GUIMouseHoverEventArgs args, SlotControl control)
	{
		UpdateHover(control);
		_lastHovered = control;
	}

	public void UpdateHover(SlotControl control)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? playerUid = _playerUid;
		SpriteComponent item2 = default(SpriteComponent);
		if (!control.MouseIsHovering || !playerUid.HasValue || !_handsSystem.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(playerUid.Value), out var item) || !_entities.TryGetComponent<SpriteComponent>(item, ref item2) || !_inventorySystem.TryGetSlotContainer(playerUid.Value, control.SlotName, out ContainerSlot containerSlot, out SlotDefinition slotDefinition))
		{
			control.ClearHover();
			return;
		}
		EntityUid val = _entities.SpawnEntity("hoverentity", MapCoordinates.Nullspace, (ComponentRegistry)null);
		SpriteComponent component = _entities.GetComponent<SpriteComponent>(val);
		string reason;
		bool flag = _inventorySystem.CanEquip(playerUid.Value, item.Value, control.SlotName, out reason, slotDefinition) && ((SharedContainerSystem)_container).CanInsert(item.Value, (BaseContainer)(object)containerSlot, false, (TransformComponent)null);
		StorageComponent storageComp = default(StorageComponent);
		ItemSlotsComponent itemSlotsComponent = default(ItemSlotsComponent);
		if (!flag && _entities.TryGetComponent<StorageComponent>(containerSlot.ContainedEntity, ref storageComp))
		{
			flag = _entities.System<StorageSystem>().CanInsert(containerSlot.ContainedEntity.Value, item.Value, playerUid.Value, out reason, storageComp);
		}
		else if (!flag && _entities.TryGetComponent<ItemSlotsComponent>(containerSlot.ContainedEntity, ref itemSlotsComponent))
		{
			ItemSlotsSystem itemSlotsSystem = _entities.System<ItemSlotsSystem>();
			foreach (ItemSlot value in itemSlotsComponent.Slots.Values)
			{
				if (value.InsertOnInteract && itemSlotsSystem.CanInsert(containerSlot.ContainedEntity.Value, item.Value, null, value))
				{
					flag = true;
					break;
				}
			}
		}
		_sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((item.Value, item2)), Entity<SpriteComponent>.op_Implicit((val, component)));
		_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((val, component)), flag ? new Color((byte)0, byte.MaxValue, (byte)0, (byte)127) : new Color(byte.MaxValue, (byte)0, (byte)0, (byte)127));
		control.HoverSpriteView.SetEntity((EntityUid?)val);
	}

	private void AddSlot(ClientInventorySystem.SlotData data)
	{
		RemoveSlotFromOtherGroups(data.SlotName, data.SlotGroup);
		if (_slotGroups.TryGetValue(data.SlotGroup, out ItemSlotButtonContainer value) && !value.TryGetButton(data.SlotName, out SlotControl _))
		{
			SlotButton newButton = CreateSlotButton(data);
			value.AddButton(newButton);
		}
	}

	private void RemoveSlot(ClientInventorySystem.SlotData data)
	{
		foreach (ItemSlotButtonContainer value in _slotGroups.Values)
		{
			value.RemoveButton(data.SlotName);
		}
	}

	public void ReloadSlots()
	{
		_inventorySystem.ReloadInventory();
	}

	private void LoadSlots(EntityUid clientUid, InventorySlotsComponent clientInv)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UnloadSlots();
		_playerUid = clientUid;
		_playerInventory = clientInv;
		foreach (ClientInventorySystem.SlotData value in clientInv.SlotData.Values)
		{
			AddSlot(value);
			if (_inventoryButton != null)
			{
				((Control)_inventoryButton).Visible = true;
			}
		}
		UpdateInventoryHotbar(_playerInventory);
	}

	private void UnloadSlots()
	{
		if (_inventoryButton != null)
		{
			((Control)_inventoryButton).Visible = false;
		}
		_playerUid = null;
		_playerInventory = null;
		foreach (ItemSlotButtonContainer value in _slotGroups.Values)
		{
			value.ClearButtons();
		}
		UpdateInventoryHotbar(null);
	}

	private void SpriteUpdated(ClientInventorySystem.SlotSpriteUpdate update)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		ClientInventorySystem.SlotSpriteUpdate slotSpriteUpdate = update;
		slotSpriteUpdate.Deconstruct(out EntityUid? Entity, out string Group, out string Name, out bool ShowStorage);
		EntityUid? val = Entity;
		string key = Group;
		string slotName = Name;
		bool visible = ShowStorage;
		SlotControl slotControl = _strippingWindow?.InventoryButtons.GetButton(update.Name);
		if (slotControl != null)
		{
			slotControl.SetEntity(val);
			((Control)slotControl.StorageButton).Visible = visible;
		}
		SlotControl slotControl2 = _slotGroups.GetValueOrDefault(key)?.GetButton(slotName);
		if (slotControl2 != null)
		{
			VirtualItemComponent virtualItemComponent = default(VirtualItemComponent);
			if (_entities.TryGetComponent<VirtualItemComponent>(val, ref virtualItemComponent))
			{
				slotControl2.SetEntity(virtualItemComponent.BlockingEntity);
				slotControl2.Blocked = true;
			}
			else
			{
				slotControl2.SetEntity(val);
				slotControl2.Blocked = false;
				((Control)slotControl2.StorageButton).Visible = visible;
			}
		}
	}

	public bool RegisterSlotGroupContainer(ItemSlotButtonContainer slotContainer)
	{
		if (_slotGroups.TryAdd(slotContainer.SlotGroup, slotContainer))
		{
			return true;
		}
		return false;
	}

	public void RemoveSlotGroup(string slotGroupName)
	{
		_slotGroups.Remove(slotGroupName);
	}

	private void RemoveSlotFromOtherGroups(string slotName, string? keepGroup = null)
	{
		foreach (var (text2, itemSlotButtonContainer2) in _slotGroups)
		{
			if (!(text2 == keepGroup))
			{
				itemSlotButtonContainer2.RemoveButton(slotName);
			}
		}
	}

	public void OnSystemLoaded(HandsSystem system)
	{
		_handsSystem.OnPlayerItemAdded += OnItemAdded;
		_handsSystem.OnPlayerItemRemoved += OnItemRemoved;
		_handsSystem.OnPlayerSetActiveHand += SetActiveHand;
	}

	public void OnSystemUnloaded(HandsSystem system)
	{
		_handsSystem.OnPlayerItemAdded -= OnItemAdded;
		_handsSystem.OnPlayerItemRemoved -= OnItemRemoved;
		_handsSystem.OnPlayerSetActiveHand -= SetActiveHand;
	}

	private void OnItemAdded(string name, EntityUid entity)
	{
		if (_lastHovered != null)
		{
			UpdateHover(_lastHovered);
		}
	}

	private void OnItemRemoved(string name, EntityUid entity)
	{
		if (_lastHovered != null)
		{
			UpdateHover(_lastHovered);
		}
	}

	private void SetActiveHand(string? handName)
	{
		if (_lastHovered != null)
		{
			UpdateHover(_lastHovered);
		}
	}

	public void OnSystemLoaded(WebbingSystem system)
	{
		system.PlayerWebbingUpdated += InventoryUpdated;
	}

	public void OnSystemUnloaded(WebbingSystem system)
	{
		system.PlayerWebbingUpdated -= InventoryUpdated;
	}

	public void OnSystemLoaded(UniformAccessorySystem system)
	{
		system.PlayerMedalUpdated += InventoryUpdated;
	}

	public void OnSystemUnloaded(UniformAccessorySystem system)
	{
		system.PlayerMedalUpdated -= InventoryUpdated;
	}

	private void InventoryUpdated()
	{
		UpdateInventoryHotbar(_playerInventory);
	}
}
