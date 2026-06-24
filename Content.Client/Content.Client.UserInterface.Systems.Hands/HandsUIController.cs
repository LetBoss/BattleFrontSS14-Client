using System;
using System.Collections.Generic;
using Content.Client.Gameplay;
using Content.Client.Hands.Systems;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Hands.Controls;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Shared.Hands.Components;
using Content.Shared.Input;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Timing;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Hands;

public sealed class HandsUIController : UIController, IOnStateEntered<GameplayState>, IOnSystemChanged<HandsSystem>, IOnSystemLoaded<HandsSystem>, IOnSystemUnloaded<HandsSystem>
{
	[Dependency]
	private IEntityManager _entities;

	[Dependency]
	private IPlayerManager _player;

	[UISystemDependency]
	private readonly HandsSystem _handsSystem;

	[UISystemDependency]
	private readonly UseDelaySystem _useDelay;

	private readonly List<HandsContainer> _handsContainers = new List<HandsContainer>();

	private readonly Dictionary<string, int> _handContainerIndices = new Dictionary<string, int>();

	private readonly Dictionary<string, HandButton> _handLookup = new Dictionary<string, HandButton>();

	private HandsComponent? _playerHandsComponent;

	private HandButton? _activeHand;

	private HandButton? _statusHandLeft;

	private HandButton? _statusHandRight;

	private int _backupSuffix;

	private HotbarGui? HandsGui => base.UIManager.GetActiveUIWidgetOrNull<HotbarGui>();

	public void OnSystemLoaded(HandsSystem system)
	{
		_handsSystem.OnPlayerAddHand += OnAddHand;
		_handsSystem.OnPlayerItemAdded += OnItemAdded;
		_handsSystem.OnPlayerItemRemoved += OnItemRemoved;
		_handsSystem.OnPlayerSetActiveHand += SetActiveHand;
		_handsSystem.OnPlayerRemoveHand += OnRemoveHand;
		_handsSystem.OnPlayerHandsAdded += LoadPlayerHands;
		_handsSystem.OnPlayerHandsRemoved += UnloadPlayerHands;
		_handsSystem.OnPlayerHandBlocked += HandBlocked;
		_handsSystem.OnPlayerHandUnblocked += HandUnblocked;
	}

	public void OnSystemUnloaded(HandsSystem system)
	{
		_handsSystem.OnPlayerAddHand -= OnAddHand;
		_handsSystem.OnPlayerItemAdded -= OnItemAdded;
		_handsSystem.OnPlayerItemRemoved -= OnItemRemoved;
		_handsSystem.OnPlayerSetActiveHand -= SetActiveHand;
		_handsSystem.OnPlayerRemoveHand -= OnRemoveHand;
		_handsSystem.OnPlayerHandsAdded -= LoadPlayerHands;
		_handsSystem.OnPlayerHandsRemoved -= UnloadPlayerHands;
		_handsSystem.OnPlayerHandBlocked -= HandBlocked;
		_handsSystem.OnPlayerHandUnblocked -= HandUnblocked;
	}

	private void OnAddHand(Entity<HandsComponent> entity, string name, HandLocation location)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityUid owner = entity.Owner;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && !(owner != localEntity.GetValueOrDefault()))
		{
			AddHand(name, location);
		}
	}

	private void OnRemoveHand(Entity<HandsComponent> entity, string name)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityUid owner = entity.Owner;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && !(owner != localEntity.GetValueOrDefault()))
		{
			RemoveHand(name);
		}
	}

	private void HandPressed(GUIBoundKeyEventArgs args, SlotControl hand)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (_handsSystem.TryGetPlayerHands(out Entity<HandsComponent>? hands))
		{
			if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIClick)
			{
				_handsSystem.UIHandClick(hands.Value, hand.SlotName);
				((BoundKeyEventArgs)args).Handle();
			}
			else if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UseSecondary)
			{
				_handsSystem.UIHandOpenContextMenu(hand.SlotName);
				((BoundKeyEventArgs)args).Handle();
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ActivateItemInWorld)
			{
				_handsSystem.UIHandActivate(hand.SlotName);
				((BoundKeyEventArgs)args).Handle();
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.AltActivateItemInWorld)
			{
				_handsSystem.UIHandAltActivateItem(hand.SlotName);
				((BoundKeyEventArgs)args).Handle();
			}
			else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ExamineEntity)
			{
				_handsSystem.UIInventoryExamine(hand.SlotName);
				((BoundKeyEventArgs)args).Handle();
			}
		}
	}

	private void UnloadPlayerHands()
	{
		if (HandsGui != null)
		{
			((Control)HandsGui).Visible = false;
		}
		_handContainerIndices.Clear();
		_handLookup.Clear();
		_playerHandsComponent = null;
		foreach (HandsContainer handsContainer in _handsContainers)
		{
			handsContainer.Clear();
		}
	}

	private void LoadPlayerHands(Entity<HandsComponent> handsComp)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (HandsGui != null)
		{
			((Control)HandsGui).Visible = true;
		}
		_playerHandsComponent = Entity<HandsComponent>.op_Implicit(handsComp);
		VirtualItemComponent virtualItemComponent = default(VirtualItemComponent);
		foreach (KeyValuePair<string, Hand> hand2 in handsComp.Comp.Hands)
		{
			hand2.Deconstruct(out var key, out var value);
			string text = key;
			Hand hand = value;
			HandButton handButton = AddHand(text, hand.Location);
			if (_handsSystem.TryGetHeldItem(handsComp.AsNullable(), text, out var held) && _entities.TryGetComponent<VirtualItemComponent>(held, ref virtualItemComponent))
			{
				handButton.SetEntity(virtualItemComponent.BlockingEntity);
				handButton.Blocked = true;
			}
			else
			{
				handButton.SetEntity(held);
				handButton.Blocked = false;
			}
		}
		if (handsComp.Comp.ActiveHandId != null)
		{
			SetActiveHand(handsComp.Comp.ActiveHandId);
		}
	}

	private void HandBlocked(string handName)
	{
		if (_handLookup.TryGetValue(handName, out HandButton value))
		{
			value.Blocked = true;
		}
	}

	private void HandUnblocked(string handName)
	{
		if (_handLookup.TryGetValue(handName, out HandButton value))
		{
			value.Blocked = false;
		}
	}

	private int GetHandContainerIndex(string containerName)
	{
		if (!_handContainerIndices.TryGetValue(containerName, out var value))
		{
			return -1;
		}
		return value;
	}

	private void OnItemAdded(string name, EntityUid entity)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		HandButton hand = GetHand(name);
		if (hand != null)
		{
			VirtualItemComponent virtualItemComponent = default(VirtualItemComponent);
			if (_entities.TryGetComponent<VirtualItemComponent>(entity, ref virtualItemComponent))
			{
				hand.SetEntity(virtualItemComponent.BlockingEntity);
				hand.Blocked = true;
			}
			else
			{
				hand.SetEntity(entity);
				hand.Blocked = false;
			}
			UpdateHandStatus(hand, entity);
		}
	}

	private void OnItemRemoved(string name, EntityUid entity)
	{
		HandButton hand = GetHand(name);
		if (hand != null)
		{
			hand.SetEntity(null);
			UpdateHandStatus(hand, null);
		}
	}

	private HandsContainer GetFirstAvailableContainer()
	{
		if (_handsContainers.Count == 0)
		{
			throw new Exception("Could not find an attached hand hud container");
		}
		foreach (HandsContainer handsContainer in _handsContainers)
		{
			if (!handsContainer.IsFull)
			{
				return handsContainer;
			}
		}
		throw new Exception("All attached hand hud containers were full!");
	}

	public bool TryGetHandContainer(string containerName, out HandsContainer? container)
	{
		container = null;
		int handContainerIndex = GetHandContainerIndex(containerName);
		if (handContainerIndex == -1)
		{
			return false;
		}
		container = _handsContainers[handContainerIndex];
		return true;
	}

	private void StorageActivate(GUIBoundKeyEventArgs args, SlotControl handControl)
	{
		_handsSystem.UIHandActivate(handControl.SlotName);
	}

	private void SetActiveHand(string? handName)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (handName == null)
		{
			if (_activeHand != null)
			{
				_activeHand.Highlight = false;
			}
		}
		else
		{
			if (!_handLookup.TryGetValue(handName, out HandButton value) || value == _activeHand)
			{
				return;
			}
			if (_activeHand != null)
			{
				_activeHand.Highlight = false;
			}
			value.Highlight = true;
			_activeHand = value;
			if (HandsGui == null || _playerHandsComponent == null)
			{
				return;
			}
			ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
			EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
			if (!val.HasValue)
			{
				return;
			}
			EntityUid valueOrDefault = val.GetValueOrDefault();
			if (_handsSystem.TryGetHand(Entity<HandsComponent>.op_Implicit((valueOrDefault, _playerHandsComponent)), handName, out var hand))
			{
				EntityUid? heldItem = _handsSystem.GetHeldItem(Entity<HandsComponent>.op_Implicit((valueOrDefault, _playerHandsComponent)), handName);
				HandUILocation uILocation = hand.Value.Location.GetUILocation();
				if (uILocation == HandUILocation.Left)
				{
					_statusHandLeft = value;
					HandsGui.UpdatePanelEntityLeft(heldItem);
				}
				else
				{
					_statusHandRight = value;
					HandsGui.UpdatePanelEntityRight(heldItem);
				}
				HandsGui.SetHighlightHand(uILocation);
			}
		}
	}

	private HandButton? GetHand(string handName)
	{
		_handLookup.TryGetValue(handName, out HandButton value);
		return value;
	}

	private HandButton AddHand(string handName, HandLocation location)
	{
		HandButton handButton = new HandButton(handName, location);
		handButton.StoragePressed += StorageActivate;
		handButton.Pressed += HandPressed;
		if (!_handLookup.TryAdd(handName, handButton))
		{
			return _handLookup[handName];
		}
		if (HandsGui != null)
		{
			HandsGui.HandContainer.AddButton(handButton);
		}
		else
		{
			GetFirstAvailableContainer().AddButton(handButton);
		}
		if (location.GetUILocation() == HandUILocation.Left)
		{
			if (_statusHandLeft == null)
			{
				_statusHandLeft = handButton;
			}
		}
		else if (_statusHandRight == null)
		{
			_statusHandRight = handButton;
		}
		UpdateVisibleStatusPanels();
		return handButton;
	}

	public void ReloadHands()
	{
		UnloadPlayerHands();
		_handsSystem.ReloadHandButtons();
	}

	public unsafe void SwapHands(HandsContainer other, HandsContainer? source = null)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (HandsGui == null && source == null)
		{
			throw new ArgumentException("Cannot swap hands if no source hand container exists!");
		}
		if (source == null)
		{
			source = HandsGui.HandContainer;
		}
		List<Control> list = new List<Control>();
		Enumerator enumerator = ((Control)source).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				if (current is HandButton)
				{
					list.Add(current);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		foreach (Control item in list)
		{
			((Control)source).RemoveChild(item);
			((Control)other).AddChild(item);
		}
	}

	private void RemoveHand(string handName)
	{
		RemoveHand(handName, out HandButton _);
	}

	private bool RemoveHand(string handName, out HandButton? handButton)
	{
		if (!_handLookup.TryGetValue(handName, out handButton))
		{
			return false;
		}
		if (((Control)handButton).Parent is HandsContainer handsContainer)
		{
			handsContainer.RemoveButton(handButton);
		}
		if (_statusHandLeft == handButton)
		{
			_statusHandLeft = null;
		}
		if (_statusHandRight == handButton)
		{
			_statusHandRight = null;
		}
		_handLookup.Remove(handName);
		((Control)handButton).Orphan();
		UpdateVisibleStatusPanels();
		return true;
	}

	private void UpdateVisibleStatusPanels()
	{
		bool left = false;
		bool right = false;
		foreach (HandButton value in _handLookup.Values)
		{
			if (value.HandLocation.GetUILocation() == HandUILocation.Left)
			{
				left = true;
			}
			else
			{
				right = true;
			}
		}
		HandsGui?.UpdateStatusVisibility(left, right);
	}

	public string RegisterHandContainer(HandsContainer handContainer)
	{
		string text = "HandContainer_" + _backupSuffix;
		if (handContainer.Indexer == null)
		{
			handContainer.Indexer = text;
			_backupSuffix++;
		}
		else
		{
			text = handContainer.Indexer;
		}
		_handContainerIndices.Add(text, _handsContainers.Count);
		_handsContainers.Add(handContainer);
		return text;
	}

	public bool RemoveHandContainer(string handContainerName)
	{
		int handContainerIndex = GetHandContainerIndex(handContainerName);
		if (handContainerIndex == -1)
		{
			return false;
		}
		_handContainerIndices.Remove(handContainerName);
		_handsContainers.RemoveAt(handContainerIndex);
		return true;
	}

	public bool RemoveHandContainer(string handContainerName, out HandsContainer? container)
	{
		int value;
		bool result = _handContainerIndices.TryGetValue(handContainerName, out value);
		container = _handsContainers[value];
		_handContainerIndices.Remove(handContainerName);
		_handsContainers.RemoveAt(value);
		return result;
	}

	public void OnStateEntered(GameplayState state)
	{
		if (HandsGui != null)
		{
			((Control)HandsGui).Visible = _playerHandsComponent != null;
		}
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		UseDelayComponent item = default(UseDelayComponent);
		foreach (HandsContainer handsContainer in _handsContainers)
		{
			foreach (HandButton button in handsContainer.GetButtons())
			{
				if (!_entities.TryGetComponent<UseDelayComponent>(button.Entity, ref item))
				{
					((Control)button.CooldownDisplay).Visible = false;
					continue;
				}
				UseDelayInfo lastEndingDelay = _useDelay.GetLastEndingDelay(Entity<UseDelayComponent>.op_Implicit((button.Entity.Value, item)));
				((Control)button.CooldownDisplay).Visible = true;
				button.CooldownDisplay.FromTime(lastEndingDelay.StartTime, lastEndingDelay.EndTime);
			}
		}
	}

	private void UpdateHandStatus(HandButton hand, EntityUid? entity)
	{
		if (hand == _statusHandLeft)
		{
			HandsGui?.UpdatePanelEntityLeft(entity);
		}
		if (hand == _statusHandRight)
		{
			HandsGui?.UpdatePanelEntityRight(entity);
		}
	}
}
