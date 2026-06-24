using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.Examine;
using Content.Client.Hands.Systems;
using Content.Client.Strip;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Hands.Controls;
using Content.Client.Verbs.UI;
using Content.Shared.Cuffs;
using Content.Shared.Cuffs.Components;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Strip.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Inventory;

public sealed class StrippableBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IUserInterfaceManager _ui;

	private readonly ExamineSystem _examine;

	private readonly HandsSystem _hands;

	private readonly InventorySystem _inv;

	private readonly SharedCuffableSystem _cuffable;

	private readonly StrippableSystem _strippable;

	[ViewVariables]
	private const int ButtonSeparation = 4;

	[ViewVariables]
	public const string HiddenPocketEntityId = "StrippingHiddenEntity";

	[ViewVariables]
	private StrippingMenu? _strippingMenu;

	[ViewVariables]
	private readonly EntityUid _virtualHiddenEntity;

	[ViewVariables]
	private int _handCount;

	[ViewVariables]
	private Vector2i _inventoryDimensions;

	public StrippableBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		_examine = base.EntMan.System<ExamineSystem>();
		_hands = base.EntMan.System<HandsSystem>();
		_inv = base.EntMan.System<InventorySystem>();
		_cuffable = base.EntMan.System<SharedCuffableSystem>();
		_strippable = base.EntMan.System<StrippableSystem>();
		_virtualHiddenEntity = base.EntMan.SpawnEntity("StrippingHiddenEntity", MapCoordinates.Nullspace, (ComponentRegistry)null);
	}

	protected override void Open()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_strippingMenu = BoundUserInterfaceExt.CreateWindowCenteredLeft<StrippingMenu>((BoundUserInterface)(object)this);
		_strippingMenu.OnDirty += UpdateMenu;
		((DefaultWindow)_strippingMenu).Title = Loc.GetString("strippable-bound-user-interface-stripping-menu-title", new(string, object)[1] { ("ownerName", Identity.Name(((BoundUserInterface)this).Owner, base.EntMan)) });
	}

	protected override void Dispose(bool disposing)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (disposing)
		{
			if (_strippingMenu != null)
			{
				_strippingMenu.OnDirty -= UpdateMenu;
			}
			base.EntMan.DeleteEntity((EntityUid?)_virtualHiddenEntity);
			((BoundUserInterface)this).Dispose(disposing);
		}
	}

	public void DirtyMenu()
	{
		if (_strippingMenu != null)
		{
			_strippingMenu.Dirty = true;
		}
	}

	public void UpdateMenu()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		if (_strippingMenu == null)
		{
			return;
		}
		_strippingMenu.ClearButtons();
		_handCount = 0;
		_inventoryDimensions = Vector2i.Zero;
		InventoryComponent inventoryComponent = default(InventoryComponent);
		if (base.EntMan.TryGetComponent<InventoryComponent>(((BoundUserInterface)this).Owner, ref inventoryComponent))
		{
			SlotDefinition[] slots = inventoryComponent.Slots;
			foreach (SlotDefinition slotDefinition in slots)
			{
				AddInventoryButton(((BoundUserInterface)this).Owner, slotDefinition.Name, inventoryComponent);
			}
		}
		HandsComponent handsComponent = default(HandsComponent);
		if (base.EntMan.TryGetComponent<HandsComponent>(((BoundUserInterface)this).Owner, ref handsComponent) && handsComponent.CanBeStripped)
		{
			string key;
			Hand value;
			foreach (KeyValuePair<string, Hand> hand4 in handsComponent.Hands)
			{
				hand4.Deconstruct(out key, out value);
				string handId = key;
				Hand hand = value;
				if (hand.Location == HandLocation.Right)
				{
					AddHandButton(Entity<HandsComponent>.op_Implicit((((BoundUserInterface)this).Owner, handsComponent)), handId, hand);
				}
			}
			foreach (KeyValuePair<string, Hand> hand5 in handsComponent.Hands)
			{
				hand5.Deconstruct(out key, out value);
				string handId2 = key;
				Hand hand2 = value;
				if (hand2.Location == HandLocation.Middle)
				{
					AddHandButton(Entity<HandsComponent>.op_Implicit((((BoundUserInterface)this).Owner, handsComponent)), handId2, hand2);
				}
			}
			foreach (KeyValuePair<string, Hand> hand6 in handsComponent.Hands)
			{
				hand6.Deconstruct(out key, out value);
				string handId3 = key;
				Hand hand3 = value;
				if (hand3.Location == HandLocation.Left)
				{
					AddHandButton(Entity<HandsComponent>.op_Implicit((((BoundUserInterface)this).Owner, handsComponent)), handId3, hand3);
				}
			}
		}
		EnsnareableComponent ensnareableComponent = default(EnsnareableComponent);
		if (base.EntMan.TryGetComponent<EnsnareableComponent>(((BoundUserInterface)this).Owner, ref ensnareableComponent) && ensnareableComponent.IsEnsnared)
		{
			Button val = new Button
			{
				Text = Loc.GetString("strippable-bound-user-interface-stripping-menu-ensnare-button"),
				StyleClasses = { "OpenRight" }
			};
			((BaseButton)val).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new StrippingEnsnareButtonPressed());
			};
			((Control)_strippingMenu.SnareContainer).AddChild((Control)(object)val);
		}
		int num = Math.Max(200, Math.Max(_handCount, _inventoryDimensions.X + 1) * (SlotControl.DefaultButtonSize + 4) + 20);
		int num2 = Math.Max(200, (_inventoryDimensions.Y + ((_handCount <= 0) ? 1 : 2)) * (SlotControl.DefaultButtonSize + 4) + 53);
		if (ensnareableComponent != null && ensnareableComponent.IsEnsnared)
		{
			num2 += 20;
		}
		((Control)_strippingMenu).SetSize = new Vector2(num, num2);
	}

	private void AddHandButton(Entity<HandsComponent> ent, string handId, Hand hand)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		HandButton handButton = new HandButton(handId, hand.Location);
		handButton.Pressed += SlotPressed;
		EntityUid? heldItem = _hands.GetHeldItem(ent.AsNullable(), handId);
		VirtualItemComponent virtualItemComponent = default(VirtualItemComponent);
		if (base.EntMan.TryGetComponent<VirtualItemComponent>(heldItem, ref virtualItemComponent))
		{
			handButton.Blocked = true;
			CuffableComponent component = default(CuffableComponent);
			if (base.EntMan.TryGetComponent<CuffableComponent>(((BoundUserInterface)this).Owner, ref component) && _cuffable.GetAllCuffs(component).Contains(virtualItemComponent.BlockingEntity))
			{
				((Control)handButton.BlockedRect).MouseFilter = (MouseFilterMode)2;
			}
		}
		UpdateEntityIcon(handButton, heldItem);
		((Control)_strippingMenu.HandsContainer).AddChild((Control)(object)handButton);
		LayoutContainer.SetPosition((Control)(object)handButton, Vector2i.op_Implicit(new Vector2i(_handCount, 0) * (SlotControl.DefaultButtonSize + 4)));
		_handCount++;
	}

	private void SlotPressed(GUIBoundKeyEventArgs ev, SlotControl slot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (((BoundKeyEventArgs)ev).Function == EngineKeyFunctions.Use)
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new StrippingSlotButtonPressed(slot.SlotName, slot is HandButton));
		}
		else if (slot.Entity.HasValue)
		{
			if (((BoundKeyEventArgs)ev).Function == ContentKeyFunctions.ExamineEntity)
			{
				_examine.DoExamine(slot.Entity.Value);
				((BoundKeyEventArgs)ev).Handle();
			}
			else if (((BoundKeyEventArgs)ev).Function == EngineKeyFunctions.UseSecondary)
			{
				_ui.GetUIController<VerbMenuUIController>().OpenVerbMenu(slot.Entity.Value);
				((BoundKeyEventArgs)ev).Handle();
			}
		}
	}

	private void AddInventoryButton(EntityUid invUid, string slotId, InventoryComponent inv)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (_inv.TryGetSlotContainer(invUid, slotId, out ContainerSlot containerSlot, out SlotDefinition slotDefinition, inv))
		{
			EntityUid? entity = containerSlot.ContainedEntity;
			if (entity.HasValue && _strippable.IsStripHidden(slotDefinition, ((ISharedPlayerManager)_player).LocalEntity))
			{
				entity = _virtualHiddenEntity;
			}
			SlotButton slotButton = new SlotButton(new ClientInventorySystem.SlotData(slotDefinition, containerSlot));
			slotButton.Pressed += SlotPressed;
			((Control)_strippingMenu.InventoryContainer).AddChild((Control)(object)slotButton);
			UpdateEntityIcon(slotButton, entity);
			LayoutContainer.SetPosition((Control)(object)slotButton, Vector2i.op_Implicit(slotDefinition.StrippingWindowPos * (SlotControl.DefaultButtonSize + 4)));
			if (slotDefinition.StrippingWindowPos.X > _inventoryDimensions.X)
			{
				_inventoryDimensions = new Vector2i(slotDefinition.StrippingWindowPos.X, _inventoryDimensions.Y);
			}
			if (slotDefinition.StrippingWindowPos.Y > _inventoryDimensions.Y)
			{
				_inventoryDimensions = new Vector2i(_inventoryDimensions.X, slotDefinition.StrippingWindowPos.Y);
			}
		}
	}

	private void UpdateEntityIcon(SlotControl button, EntityUid? entity)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		button.ClearHover();
		((Control)button.StorageButton).Visible = false;
		if (!entity.HasValue)
		{
			button.SetEntity(null);
			return;
		}
		VirtualItemComponent virtualItemComponent = default(VirtualItemComponent);
		EntityUid? entity2;
		if (base.EntMan.TryGetComponent<VirtualItemComponent>(entity, ref virtualItemComponent))
		{
			entity2 = (base.EntMan.HasComponent<SpriteComponent>(virtualItemComponent.BlockingEntity) ? new EntityUid?(virtualItemComponent.BlockingEntity) : ((EntityUid?)null));
		}
		else
		{
			if (!base.EntMan.HasComponent<SpriteComponent>(entity))
			{
				return;
			}
			entity2 = entity;
		}
		button.SetEntity(entity2);
	}
}
