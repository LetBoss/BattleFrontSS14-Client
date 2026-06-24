using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client._RMC14.Actions;
using Content.Client.Actions;
using Content.Client.Construction;
using Content.Client.Gameplay;
using Content.Client.Hands;
using Content.Client.Interaction;
using Content.Client.Outline;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Actions.Controls;
using Content.Client.UserInterface.Systems.Actions.Widgets;
using Content.Client.UserInterface.Systems.Actions.Windows;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Actions.Components;
using Content.Shared.CombatMode;
using Content.Shared.Input;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Actions;

public sealed class ActionUIController : UIController, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnSystemChanged<ActionsSystem>, IOnSystemLoaded<ActionsSystem>, IOnSystemUnloaded<ActionsSystem>
{
	[Dependency]
	private IOverlayManager _overlays;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IInputManager _input;

	[UISystemDependency]
	private readonly ActionsSystem? _actionsSystem;

	[UISystemDependency]
	private readonly InteractionOutlineSystem? _interactionOutline;

	[UISystemDependency]
	private readonly TargetOutlineSystem? _targetOutline;

	[UISystemDependency]
	private readonly SpriteSystem _spriteSystem;

	private ActionButtonContainer? _container;

	private readonly List<EntityUid?> _actions = new List<EntityUid?>();

	private readonly DragDropHelper<ActionButton> _menuDragHelper;

	private readonly TextureRect _dragShadow;

	private ActionsWindow? _window;

	private readonly List<EntityUid?> _vehicleActions = new List<EntityUid?>();

	private bool _vehicleHotbarOverride;

	private ActionsBar? ActionsBar => base.UIManager.GetActiveUIWidgetOrNull<ActionsBar>();

	private MenuButton? ActionButton => base.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.ActionButton;

	public bool IsDragging => _menuDragHelper.IsDragging;

	public EntityUid? SelectingTargetFor { get; private set; }

	public ActionUIController()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		_menuDragHelper = new DragDropHelper<ActionButton>(OnMenuBeginDrag, OnMenuContinueDrag, OnMenuEndDrag);
		_dragShadow = new TextureRect
		{
			MinSize = new Vector2(64f, 64f),
			Stretch = (StretchMode)1,
			Visible = false,
			SetSize = new Vector2(64f, 64f),
			MouseFilter = (MouseFilterMode)2
		};
	}

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, new Action(OnScreenLoad));
		uIController.OnScreenUnload = (Action)Delegate.Combine(uIController.OnScreenUnload, new Action(OnScreenUnload));
	}

	private void OnScreenLoad()
	{
		LoadGui();
	}

	private void OnScreenUnload()
	{
		UnloadGui();
	}

	public void OnStateEntered(GameplayState state)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		if (_actionsSystem != null)
		{
			_actionsSystem.OnActionAdded += OnActionAdded;
			_actionsSystem.OnActionRemoved += OnActionRemoved;
			_actionsSystem.ActionsUpdated += OnActionsUpdated;
		}
		UpdateFilterLabel();
		QueueWindowUpdate();
		((Control)_dragShadow).Orphan();
		((Control)base.UIManager.PopupRoot).AddChild((Control)(object)_dragShadow);
		BindingsBuilder val = CommandBinds.Builder;
		BoundKeyFunction[] hotbarBoundKeys = ContentKeyFunctions.GetHotbarBoundKeys();
		for (int i = 0; i < hotbarBoundKeys.Length; i++)
		{
			int boundId = i;
			BoundKeyFunction val2 = hotbarBoundKeys[i];
			val = val.Bind(val2, (InputCmdHandler)new PointerInputCmdHandler((PointerInputCmdDelegate2)delegate(in PointerInputCmdArgs args)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				if ((int)args.State != 1)
				{
					return false;
				}
				TriggerAction(boundId);
				return true;
			}, false, true));
		}
		val.Bind(ContentKeyFunctions.OpenActionsMenu, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			ToggleWindow();
		}, (StateInputCmdDelegate)null, true, true)).BindBefore(EngineKeyFunctions.Use, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(TargetingOnUse), true, true), new Type[2]
		{
			typeof(ConstructionSystem),
			typeof(DragDropSystem)
		}).BindBefore(EngineKeyFunctions.UIRightClick, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(TargetingCancel), true, true), Array.Empty<Type>())
			.Register<ActionUIController>();
	}

	private bool TargetingCancel(in PointerInputCmdArgs args)
	{
		if (!_timing.IsFirstTimePredicted)
		{
			return false;
		}
		if (!SelectingTargetFor.HasValue)
		{
			return false;
		}
		StopTargeting();
		return true;
	}

	private bool TargetingOnUse(in PointerInputCmdArgs args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted && _actionsSystem != null)
		{
			EntityUid? selectingTargetFor = SelectingTargetFor;
			if (selectingTargetFor.HasValue)
			{
				EntityUid valueOrDefault = selectingTargetFor.GetValueOrDefault();
				selectingTargetFor = ((ISharedPlayerManager)_playerManager).LocalEntity;
				if (selectingTargetFor.HasValue)
				{
					EntityUid valueOrDefault2 = selectingTargetFor.GetValueOrDefault();
					ActionsComponent item = default(ActionsComponent);
					if (!base.EntityManager.TryGetComponent<ActionsComponent>(valueOrDefault2, ref item))
					{
						return false;
					}
					Entity<ActionComponent>? action = _actionsSystem.GetAction(Entity<ActionComponent>.op_Implicit(valueOrDefault));
					if (action.HasValue)
					{
						Entity<ActionComponent> valueOrDefault3 = action.GetValueOrDefault();
						TargetActionComponent targetActionComponent = default(TargetActionComponent);
						if (base.EntityManager.TryGetComponent<TargetActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault3), ref targetActionComponent))
						{
							if (!_actionsSystem.ValidAction(valueOrDefault3))
							{
								return !targetActionComponent.InteractOnMiss;
							}
							ActionTargetAttemptEvent actionTargetAttemptEvent = new ActionTargetAttemptEvent(args, Entity<ActionsComponent>.op_Implicit((valueOrDefault2, item)), Entity<ActionComponent>.op_Implicit(valueOrDefault3));
							((IDirectedEventBus)base.EntityManager.EventBus).RaiseLocalEvent<ActionTargetAttemptEvent>(Entity<ActionComponent>.op_Implicit(valueOrDefault3), ref actionTargetAttemptEvent, false);
							if (!actionTargetAttemptEvent.Handled)
							{
								((UIController)this).Log.Error($"Action {base.EntityManager.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(valueOrDefault))} did not handle ActionTargetAttemptEvent!");
								return false;
							}
							if (actionTargetAttemptEvent.FoundTarget ? (!targetActionComponent.Repeat) : targetActionComponent.DeselectOnMiss)
							{
								StopTargeting();
							}
							return true;
						}
					}
					return false;
				}
				return false;
			}
		}
		return false;
	}

	public void UnloadButton()
	{
		if (ActionButton != null)
		{
			((BaseButton)ActionButton).OnPressed -= ActionButtonPressed;
		}
	}

	public void LoadButton()
	{
		if (ActionButton != null)
		{
			((BaseButton)ActionButton).OnPressed += ActionButtonPressed;
		}
	}

	private void OnWindowOpened()
	{
		MenuButton? actionButton = ActionButton;
		if (actionButton != null)
		{
			((BaseButton)actionButton).SetClickPressed(true);
		}
		SearchAndDisplay();
	}

	private void OnWindowClosed()
	{
		MenuButton? actionButton = ActionButton;
		if (actionButton != null)
		{
			((BaseButton)actionButton).SetClickPressed(false);
		}
	}

	public void OnStateExited(GameplayState state)
	{
		if (_actionsSystem != null)
		{
			_actionsSystem.OnActionAdded -= OnActionAdded;
			_actionsSystem.OnActionRemoved -= OnActionRemoved;
			_actionsSystem.ActionsUpdated -= OnActionsUpdated;
		}
		CommandBinds.Unregister<ActionUIController>();
	}

	private void TriggerAction(int index)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<EntityUid?> activeHotbarActions = GetActiveHotbarActions();
		if (index < 0 || index >= activeHotbarActions.Count)
		{
			return;
		}
		EntityUid? val = activeHotbarActions[index];
		if (!val.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = val.GetValueOrDefault();
		Entity<ActionComponent>? val2 = _actionsSystem?.GetAction(Entity<ActionComponent>.op_Implicit(valueOrDefault));
		if (val2.HasValue)
		{
			Entity<ActionComponent> valueOrDefault2 = val2.GetValueOrDefault();
			TargetActionComponent item = default(TargetActionComponent);
			if (base.EntityManager.TryGetComponent<TargetActionComponent>(valueOrDefault, ref item))
			{
				ToggleTargeting(Entity<ActionComponent, TargetActionComponent>.op_Implicit((valueOrDefault, valueOrDefault2.Comp, item)));
			}
			else
			{
				_actionsSystem?.TriggerAction(valueOrDefault2);
			}
		}
	}

	private void OnActionAdded(EntityUid actionId)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? val = _actionsSystem?.GetAction(Entity<ActionComponent>.op_Implicit(actionId));
		if (val.HasValue)
		{
			Entity<ActionComponent> valueOrDefault = val.GetValueOrDefault();
			TargetActionComponent item = default(TargetActionComponent);
			if (valueOrDefault.Comp.Toggled && base.EntityManager.TryGetComponent<TargetActionComponent>(actionId, ref item))
			{
				StartTargeting(Entity<ActionComponent, TargetActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault), item)));
			}
			if (base.EntityManager.HasComponent<VehicleHardpointActionComponent>(actionId))
			{
				RefreshVehicleHotbarOverride(forceUpdate: true);
			}
			else if (!_actions.Contains(Entity<ActionComponent>.op_Implicit(valueOrDefault)))
			{
				_actions.Add(Entity<ActionComponent>.op_Implicit(valueOrDefault));
				RefreshVehicleHotbarOverride(forceUpdate: true);
			}
		}
	}

	private void OnActionRemoved(EntityUid actionId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = actionId;
		EntityUid? selectingTargetFor = SelectingTargetFor;
		if (selectingTargetFor.HasValue && val == selectingTargetFor.GetValueOrDefault())
		{
			StopTargeting();
		}
		if (base.EntityManager.HasComponent<VehicleHardpointActionComponent>(actionId) || _vehicleActions.Contains(actionId))
		{
			_vehicleActions.RemoveAll(delegate(EntityUid? x)
			{
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				EntityUid? val2 = x;
				EntityUid val3 = actionId;
				return val2.HasValue && val2.GetValueOrDefault() == val3;
			});
			RefreshVehicleHotbarOverride();
		}
		else
		{
			_actions.RemoveAll(delegate(EntityUid? x)
			{
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				EntityUid? val2 = x;
				EntityUid val3 = actionId;
				return val2.HasValue && val2.GetValueOrDefault() == val3;
			});
			RefreshVehicleHotbarOverride(forceUpdate: true);
		}
	}

	private void OnActionsUpdated()
	{
		QueueWindowUpdate();
		RefreshVehicleHotbarOverride(forceUpdate: true);
	}

	private void ActionButtonPressed(ButtonEventArgs args)
	{
		ToggleWindow();
	}

	private void ToggleWindow()
	{
		if (_window != null)
		{
			if (((BaseWindow)_window).IsOpen)
			{
				((BaseWindow)_window).Close();
			}
			else
			{
				((BaseWindow)_window).Open();
			}
		}
	}

	private void UpdateFilterLabel()
	{
		if (_window != null)
		{
			if (_window.FilterButton.SelectedKeys.Count == 0)
			{
				((Control)_window.FilterLabel).Visible = false;
				return;
			}
			((Control)_window.FilterLabel).Visible = true;
			_window.FilterLabel.Text = Loc.GetString("ui-actionmenu-filter-label", new(string, object)[1] { ("selectedLabels", string.Join(", ", _window.FilterButton.SelectedLabels)) });
		}
	}

	private bool MatchesFilter(Entity<ActionComponent> ent, ActionsWindow.Filters filter)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		val.Deconstruct(ref val2, ref actionComponent);
		EntityUid val3 = val2;
		ActionComponent actionComponent2 = actionComponent;
		switch (filter)
		{
		case ActionsWindow.Filters.Enabled:
			return actionComponent2.Enabled;
		case ActionsWindow.Filters.Item:
		{
			int result2;
			if (actionComponent2.Container.HasValue)
			{
				EntityUid? localEntity = actionComponent2.Container;
				EntityUid? container = ((ISharedPlayerManager)_playerManager).LocalEntity;
				result2 = ((localEntity.HasValue != container.HasValue || (localEntity.HasValue && localEntity.GetValueOrDefault() != container.GetValueOrDefault())) ? 1 : 0);
			}
			else
			{
				result2 = 0;
			}
			return (byte)result2 != 0;
		}
		case ActionsWindow.Filters.Innate:
		{
			int result;
			if (actionComponent2.Container.HasValue)
			{
				EntityUid? container = actionComponent2.Container;
				EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
				result = ((container.HasValue == localEntity.HasValue && (!container.HasValue || container.GetValueOrDefault() == localEntity.GetValueOrDefault())) ? 1 : 0);
			}
			else
			{
				result = 1;
			}
			return (byte)result != 0;
		}
		case ActionsWindow.Filters.Instant:
			return base.EntityManager.HasComponent<InstantActionComponent>(val3);
		case ActionsWindow.Filters.Targeted:
			return base.EntityManager.HasComponent<TargetActionComponent>(val3);
		default:
			throw new ArgumentOutOfRangeException("filter", filter, null);
		}
	}

	private void ClearList()
	{
		ActionsWindow? window = _window;
		if (window != null && !((Control)window).Disposed)
		{
			((Control)_window.ResultsGrid).RemoveAllChildren();
		}
	}

	private unsafe void PopulateActions(IEnumerable<Entity<ActionComponent>> actions)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		ActionsWindow window = _window;
		if (window == null || ((Control)window).Disposed || !((BaseWindow)window).IsOpen || _actionsSystem == null)
		{
			return;
		}
		_window.UpdateNeeded = false;
		List<ActionButton> list = new List<ActionButton>(((Control)_window.ResultsGrid).ChildCount);
		Enumerator enumerator = ((Control)_window.ResultsGrid).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (((Enumerator)(ref enumerator)).Current is ActionButton item)
				{
					list.Add(item);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		int i = 0;
		foreach (Entity<ActionComponent> action in actions)
		{
			if (i < list.Count)
			{
				list[i++].UpdateData(Entity<ActionComponent>.op_Implicit(action), _actionsSystem);
				continue;
			}
			ActionButton actionButton = new ActionButton(base.EntityManager, _spriteSystem, this)
			{
				Locked = true
			};
			actionButton.ActionPressed += OnWindowActionPressed;
			actionButton.ActionUnpressed += OnWindowActionUnPressed;
			actionButton.ActionFocusExited += OnWindowActionFocusExisted;
			actionButton.UpdateData(Entity<ActionComponent>.op_Implicit(action), _actionsSystem);
			((Control)_window.ResultsGrid).AddChild((Control)(object)actionButton);
		}
		for (; i < list.Count; i++)
		{
			((Control)list[i]).Orphan();
		}
	}

	public void QueueWindowUpdate()
	{
		if (_window != null)
		{
			_window.UpdateNeeded = true;
		}
	}

	private void SearchAndDisplay()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ActionsWindow window = _window;
		if (window == null || ((Control)window).Disposed || !((BaseWindow)window).IsOpen || _actionsSystem == null)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid player = localEntity.GetValueOrDefault();
		string search = _window.SearchBar.Text;
		IReadOnlyList<ActionsWindow.Filters> filters = _window.FilterButton.SelectedKeys;
		IEnumerable<Entity<ActionComponent>> clientActions = _actionsSystem.GetClientActions();
		if (filters.Count == 0 && string.IsNullOrWhiteSpace(search))
		{
			PopulateActions(clientActions);
			return;
		}
		clientActions = clientActions.Where(delegate(Entity<ActionComponent> action)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			if (filters.Count > 0 && filters.Any((ActionsWindow.Filters filter) => !MatchesFilter(action, filter)))
			{
				return false;
			}
			if (action.Comp.Keywords.Any((string keyword) => search.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
			{
				return true;
			}
			if (base.EntityManager.GetComponent<MetaDataComponent>(Entity<ActionComponent>.op_Implicit(action)).EntityName.Contains(search, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (action.Comp.Container.HasValue)
			{
				EntityUid? container = action.Comp.Container;
				EntityUid val = player;
				if (!container.HasValue || !(container.GetValueOrDefault() == val))
				{
					return base.EntityManager.GetComponent<MetaDataComponent>(action.Comp.Container.Value).EntityName.Contains(search, StringComparison.OrdinalIgnoreCase);
				}
			}
			return false;
		});
		PopulateActions(clientActions);
	}

	private void SetAction(ActionButton button, EntityUid? actionId, bool updateSlots = true, bool allowOverrideClear = false)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (_actionsSystem == null || (_vehicleHotbarOverride && !actionId.HasValue && !allowOverrideClear))
		{
			return;
		}
		List<EntityUid?> editableHotbarActions = GetEditableHotbarActions();
		int position;
		if (!actionId.HasValue)
		{
			button.ClearData();
			ActionButtonContainer? container = _container;
			if (container != null && container.TryGetButtonIndex(button, out position) && editableHotbarActions.Count > position && position >= 0)
			{
				editableHotbarActions.RemoveAt(position);
			}
		}
		else if (button.TryReplaceWith(actionId.Value, _actionsSystem) && _container != null && _container.TryGetButtonIndex(button, out position))
		{
			if (position >= editableHotbarActions.Count)
			{
				editableHotbarActions.Add(actionId);
			}
			else
			{
				editableHotbarActions[position] = actionId;
			}
		}
		if (updateSlots)
		{
			_container?.SetActionData(_actionsSystem, GetActiveHotbarActions().ToArray());
		}
		if (!_vehicleHotbarOverride)
		{
			base.EntityManager.SystemOrNull<RMCActionsSystem>()?.ActionsChanged(_actions);
		}
	}

	private void DragAction()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		ActionButton dragged = _menuDragHelper.Dragged;
		if (dragged != null)
		{
			Entity<ActionComponent>? action = dragged.Action;
			if (action.HasValue)
			{
				Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
				EntityUid? actionId = null;
				if (base.UIManager.MouseGetControl(_input.MouseScreenPosition) is ActionButton actionButton)
				{
					action = actionButton.Action;
					actionId = (action.HasValue ? new EntityUid?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((EntityUid?)null));
					SetAction(actionButton, Entity<ActionComponent>.op_Implicit(valueOrDefault), updateSlots: false);
				}
				if (((Control)dragged).Parent is ActionButtonContainer)
				{
					SetAction(dragged, actionId, updateSlots: false, allowOverrideClear: true);
				}
				if (_actionsSystem != null)
				{
					_container?.SetActionData(_actionsSystem, GetActiveHotbarActions().ToArray());
				}
				_menuDragHelper.EndDrag();
				return;
			}
		}
		_menuDragHelper.EndDrag();
	}

	private void OnClearPressed(ButtonEventArgs args)
	{
		if (_window != null)
		{
			_window.SearchBar.Clear();
			_window.FilterButton.DeselectAll();
			UpdateFilterLabel();
			QueueWindowUpdate();
		}
	}

	private void OnSearchChanged(LineEditEventArgs args)
	{
		QueueWindowUpdate();
	}

	private void OnFilterSelected(ItemPressedEventArgs<ActionsWindow.Filters> args)
	{
		UpdateFilterLabel();
		QueueWindowUpdate();
	}

	private void OnWindowActionPressed(GUIBoundKeyEventArgs args, ActionButton action)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick) || !(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.Use))
		{
			HandleActionPressed(args, action);
		}
	}

	private void OnWindowActionUnPressed(GUIBoundKeyEventArgs args, ActionButton dragged)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick) || !(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.Use))
		{
			HandleActionUnpressed(args, dragged);
		}
	}

	private void OnWindowActionFocusExisted(ActionButton button)
	{
		_menuDragHelper.EndDrag();
	}

	private void OnActionPressed(GUIBoundKeyEventArgs args, ActionButton button)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIRightClick)
		{
			if (_vehicleHotbarOverride)
			{
				((BoundKeyEventArgs)args).Handle();
				return;
			}
			SetAction(button, null);
			((BoundKeyEventArgs)args).Handle();
		}
		else if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick))
		{
			HandleActionPressed(args, button);
		}
	}

	private void HandleActionPressed(GUIBoundKeyEventArgs args, ActionButton button)
	{
		((BoundKeyEventArgs)args).Handle();
		if (button.Action.HasValue)
		{
			_menuDragHelper.MouseDown(button);
		}
	}

	private void OnActionUnpressed(GUIBoundKeyEventArgs args, ActionButton button)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick))
		{
			HandleActionUnpressed(args, button);
		}
	}

	private void HandleActionUnpressed(GUIBoundKeyEventArgs args, ActionButton button)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (_actionsSystem == null)
		{
			return;
		}
		((BoundKeyEventArgs)args).Handle();
		if (_menuDragHelper.IsDragging)
		{
			DragAction();
			return;
		}
		_menuDragHelper.EndDrag();
		Entity<ActionComponent>? action = button.Action;
		if (action.HasValue)
		{
			Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
			TargetActionComponent item = default(TargetActionComponent);
			if (!base.EntityManager.TryGetComponent<TargetActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), ref item))
			{
				_actionsSystem?.TriggerAction(valueOrDefault);
			}
			else
			{
				ToggleTargeting(Entity<ActionComponent, TargetActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, item)));
			}
		}
	}

	private bool OnMenuBeginDrag()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? val = _menuDragHelper.Dragged?.Action;
		if (val.HasValue)
		{
			Entity<ActionComponent> valueOrDefault = val.GetValueOrDefault();
			SpriteComponent val2 = default(SpriteComponent);
			if (base.EntityManager.TryGetComponent<SpriteComponent>(valueOrDefault.Comp.EntityIcon, ref val2))
			{
				IRsiStateLike icon = val2.Icon;
				Texture val3 = ((icon != null) ? icon.GetFrame((RsiDirection)0, 0) : null);
				if (val3 != null)
				{
					_dragShadow.Texture = val3;
					goto IL_00ae;
				}
			}
			SpriteSpecifier icon2 = valueOrDefault.Comp.Icon;
			if (icon2 != null)
			{
				_dragShadow.Texture = _spriteSystem.Frame0(icon2);
			}
			else
			{
				_dragShadow.Texture = null;
			}
		}
		goto IL_00ae;
		IL_00ae:
		LayoutContainer.SetPosition((Control)(object)_dragShadow, base.UIManager.MousePositionScaled.Position - new Vector2(32f, 32f));
		return true;
	}

	private bool OnMenuContinueDrag(float frameTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		LayoutContainer.SetPosition((Control)(object)_dragShadow, base.UIManager.MousePositionScaled.Position - new Vector2(32f, 32f));
		((Control)_dragShadow).Visible = true;
		return true;
	}

	private void OnMenuEndDrag()
	{
		_dragShadow.Texture = null;
		((Control)_dragShadow).Visible = false;
	}

	private void UnloadGui()
	{
		_actionsSystem?.UnlinkAllActions();
		if (ActionsBar != null && _window != null)
		{
			((BaseWindow)_window).OnOpen -= OnWindowOpened;
			((BaseWindow)_window).OnClose -= OnWindowClosed;
			((BaseButton)_window.ClearButton).OnPressed -= OnClearPressed;
			_window.SearchBar.OnTextChanged -= OnSearchChanged;
			_window.FilterButton.OnItemSelected -= OnFilterSelected;
			if (!((Control)_window).Disposed)
			{
				((Control)_window).Orphan();
			}
			_window = null;
		}
	}

	private void LoadGui()
	{
		UnloadGui();
		_window = base.UIManager.CreateWindow<ActionsWindow>();
		LayoutContainer.SetAnchorPreset((Control)(object)_window, (LayoutPreset)5, false);
		((BaseWindow)_window).OnOpen += OnWindowOpened;
		((BaseWindow)_window).OnClose += OnWindowClosed;
		((BaseButton)_window.ClearButton).OnPressed += OnClearPressed;
		_window.SearchBar.OnTextChanged += OnSearchChanged;
		_window.FilterButton.OnItemSelected += OnFilterSelected;
		if (ActionsBar != null)
		{
			RegisterActionContainer(ActionsBar.ActionsContainer);
			_actionsSystem?.LinkAllActions();
		}
	}

	public void RegisterActionContainer(ActionButtonContainer container)
	{
		if (_container != null)
		{
			_container.ActionPressed -= OnActionPressed;
			_container.ActionUnpressed -= OnActionUnpressed;
		}
		_container = container;
		_container.ActionPressed += OnActionPressed;
		_container.ActionUnpressed += OnActionUnpressed;
	}

	private void ClearActions()
	{
		_actions.Clear();
		_vehicleActions.Clear();
		_vehicleHotbarOverride = false;
		_container?.ClearActionData();
	}

	private void AssignSlots(List<ActionsSystem.SlotAssignment> assignments)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (_actionsSystem == null)
		{
			return;
		}
		_actions.Clear();
		foreach (ActionsSystem.SlotAssignment assignment in assignments)
		{
			if (!base.EntityManager.HasComponent<VehicleHardpointActionComponent>(assignment.ActionId))
			{
				_actions.Add(assignment.ActionId);
			}
		}
		RefreshVehicleHotbarOverride(forceUpdate: true);
	}

	public void RemoveActionContainer()
	{
		_container = null;
	}

	public void OnSystemLoaded(ActionsSystem system)
	{
		system.LinkActions += OnComponentLinked;
		system.UnlinkActions += OnComponentUnlinked;
		system.ClearAssignments += ClearActions;
		system.AssignSlot += AssignSlots;
	}

	public void OnSystemUnloaded(ActionsSystem system)
	{
		system.LinkActions -= OnComponentLinked;
		system.UnlinkActions -= OnComponentUnlinked;
		system.ClearAssignments -= ClearActions;
		system.AssignSlot -= AssignSlots;
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		_menuDragHelper.Update(((FrameEventArgs)(ref args)).DeltaSeconds);
		RefreshVehicleHotbarOverride();
		ActionsWindow window = _window;
		if (window != null && window.UpdateNeeded)
		{
			SearchAndDisplay();
		}
	}

	private void OnComponentLinked(ActionsComponent component)
	{
		if (_actionsSystem != null)
		{
			LoadDefaultActions();
			RefreshVehicleHotbarOverride(forceUpdate: true);
			QueueWindowUpdate();
		}
	}

	private void OnComponentUnlinked()
	{
		_vehicleHotbarOverride = false;
		_vehicleActions.Clear();
		_container?.ClearActionData();
		QueueWindowUpdate();
		StopTargeting();
	}

	private void LoadDefaultActions()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (_actionsSystem == null)
		{
			return;
		}
		List<Entity<ActionComponent>> list = (from action in _actionsSystem.GetClientActions()
			where action.Comp.AutoPopulate
			select action).ToList();
		list.Sort(ActionsSystem.ActionComparer);
		_actions.Clear();
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		foreach (Entity<ActionComponent> item in list)
		{
			item.Deconstruct(ref val, ref actionComponent);
			EntityUid val2 = val;
			if (!base.EntityManager.HasComponent<VehicleHardpointActionComponent>(val2) && !_actions.Contains(val2))
			{
				_actions.Add(val2);
			}
		}
	}

	private void ToggleTargeting(Entity<ActionComponent, TargetActionComponent> ent)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? selectingTargetFor = SelectingTargetFor;
		EntityUid val = Entity<ActionComponent, TargetActionComponent>.op_Implicit(ent);
		if (selectingTargetFor.HasValue && selectingTargetFor.GetValueOrDefault() == val)
		{
			StopTargeting();
		}
		else
		{
			StartTargeting(ent);
		}
	}

	private void StartTargeting(Entity<ActionComponent, TargetActionComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent, TargetActionComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		TargetActionComponent targetActionComponent = default(TargetActionComponent);
		val.Deconstruct(ref val2, ref actionComponent, ref targetActionComponent);
		EntityUid val3 = val2;
		ActionComponent actionComponent2 = actionComponent;
		TargetActionComponent targetActionComponent2 = targetActionComponent;
		StopTargeting();
		SelectingTargetFor = val3;
		_actionsSystem?.SetToggled(Entity<ActionComponent>.op_Implicit(val3), toggled: true);
		EntityUid? container = actionComponent2.Container;
		ShowHandItemOverlay showHandItemOverlay = default(ShowHandItemOverlay);
		if (targetActionComponent2.TargetingIndicator && _overlays.TryGetOverlay<ShowHandItemOverlay>(ref showHandItemOverlay))
		{
			if (actionComponent2.ItemIconStyle == ItemActionIconStyle.BigItem && actionComponent2.Container.HasValue)
			{
				showHandItemOverlay.EntityOverride = container;
			}
			else if (actionComponent2.Toggled && actionComponent2.IconOn != null)
			{
				showHandItemOverlay.IconOverride = _spriteSystem.Frame0(actionComponent2.IconOn);
			}
			else if (actionComponent2.Icon != null)
			{
				showHandItemOverlay.IconOverride = _spriteSystem.Frame0(actionComponent2.Icon);
			}
		}
		if (_container != null)
		{
			foreach (ActionButton button in _container.GetButtons())
			{
				Entity<ActionComponent>? action = button.Action;
				if (action.HasValue && action.GetValueOrDefault().Owner == val3)
				{
					button.UpdateIcons();
				}
			}
		}
		EntityTargetActionComponent entityTargetActionComponent = default(EntityTargetActionComponent);
		if (!base.EntityManager.TryGetComponent<EntityTargetActionComponent>(val3, ref entityTargetActionComponent) || !entityTargetActionComponent.ToggleOutline)
		{
			return;
		}
		Func<EntityUid, bool> predicate = null;
		EntityUid? attachedEnt = actionComponent2.AttachedEntity;
		if (!entityTargetActionComponent.CanTargetSelf)
		{
			predicate = delegate(EntityUid e)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				EntityUid? val4 = attachedEnt;
				return !val4.HasValue || e != val4.GetValueOrDefault();
			};
		}
		float range = (targetActionComponent2.CheckCanAccess ? targetActionComponent2.Range : (-1f));
		_interactionOutline?.SetEnabled(enabled: false);
		_targetOutline?.Enable(range, targetActionComponent2.CheckCanAccess, predicate, entityTargetActionComponent.Whitelist, entityTargetActionComponent.Blacklist, null);
	}

	private void StopTargeting()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (!SelectingTargetFor.HasValue)
		{
			return;
		}
		EntityUid? selectingTargetFor = SelectingTargetFor;
		ActionsSystem? actionsSystem = _actionsSystem;
		if (actionsSystem != null)
		{
			EntityUid? val = selectingTargetFor;
			actionsSystem.SetToggled(val.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(val.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: false);
		}
		SelectingTargetFor = null;
		_targetOutline?.Disable();
		_interactionOutline?.SetEnabled(enabled: true);
		if (_container != null)
		{
			foreach (ActionButton button in _container.GetButtons())
			{
				Entity<ActionComponent>? action = button.Action;
				bool num;
				if (!action.HasValue)
				{
					num = !selectingTargetFor.HasValue;
				}
				else
				{
					EntityUid owner = action.GetValueOrDefault().Owner;
					EntityUid? val = selectingTargetFor;
					if (!val.HasValue)
					{
						continue;
					}
					num = owner == val.GetValueOrDefault();
				}
				if (num)
				{
					button.UpdateIcons();
				}
			}
		}
		ShowHandItemOverlay showHandItemOverlay = default(ShowHandItemOverlay);
		if (_overlays.TryGetOverlay<ShowHandItemOverlay>(ref showHandItemOverlay))
		{
			showHandItemOverlay.IconOverride = null;
			showHandItemOverlay.EntityOverride = null;
		}
	}

	private IReadOnlyList<EntityUid?> GetActiveHotbarActions()
	{
		if (!_vehicleHotbarOverride)
		{
			return _actions;
		}
		return _vehicleActions;
	}

	private List<EntityUid?> GetEditableHotbarActions()
	{
		if (!_vehicleHotbarOverride)
		{
			return _actions;
		}
		return _vehicleActions;
	}

	private void RefreshVehicleHotbarOverride(bool forceUpdate = false)
	{
		if (_actionsSystem == null)
		{
			return;
		}
		bool flag = HasVehicleHotbarContext();
		bool flag2 = ShouldUseVehicleHotbarOverride();
		bool flag3 = false;
		if (!flag2 && !_vehicleHotbarOverride && !forceUpdate)
		{
			return;
		}
		if (flag2)
		{
			if (!_vehicleHotbarOverride)
			{
				_vehicleHotbarOverride = true;
				flag3 = true;
			}
			if (RebuildVehicleActionList())
			{
				flag3 = true;
			}
		}
		else if (_vehicleHotbarOverride)
		{
			_vehicleHotbarOverride = false;
			if (!flag)
			{
				_vehicleActions.Clear();
			}
			flag3 = true;
		}
		else if (!flag && _vehicleActions.Count > 0)
		{
			_vehicleActions.Clear();
		}
		if (flag3 || forceUpdate)
		{
			_container?.SetActionData(_actionsSystem, GetActiveHotbarActions().ToArray());
		}
	}

	private bool ShouldUseVehicleHotbarOverride()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (!base.EntityManager.HasComponent<VehicleWeaponsOperatorComponent>(valueOrDefault))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private bool HasVehicleHotbarContext()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			return base.EntityManager.HasComponent<VehicleWeaponsOperatorComponent>(valueOrDefault);
		}
		return false;
	}

	private bool RebuildVehicleActionList()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		if (_actionsSystem == null)
		{
			return false;
		}
		List<EntityUid?> list = _vehicleActions.ToList();
		List<EntityUid?> list2 = new List<EntityUid?>();
		bool flag = true;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			VehicleViewToggleComponent vehicleViewToggleComponent = default(VehicleViewToggleComponent);
			if (base.EntityManager.TryGetComponent<VehicleViewToggleComponent>(valueOrDefault, ref vehicleViewToggleComponent))
			{
				flag = vehicleViewToggleComponent.IsOutside;
			}
		}
		if (!flag)
		{
			foreach (EntityUid? action in _actions)
			{
				if (action.HasValue)
				{
					EntityUid valueOrDefault2 = action.GetValueOrDefault();
					if (!base.EntityManager.HasComponent<VehicleHardpointActionComponent>(valueOrDefault2))
					{
						list2.Add(valueOrDefault2);
					}
				}
			}
		}
		else
		{
			localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
			if (localEntity.HasValue)
			{
				EntityUid valueOrDefault3 = localEntity.GetValueOrDefault();
				CombatModeComponent combatModeComponent = default(CombatModeComponent);
				if (base.EntityManager.TryGetComponent<CombatModeComponent>(valueOrDefault3, ref combatModeComponent))
				{
					localEntity = combatModeComponent.CombatToggleActionEntity;
					if (localEntity.HasValue)
					{
						EntityUid valueOrDefault4 = localEntity.GetValueOrDefault();
						if (!base.EntityManager.HasComponent<VehicleHardpointActionComponent>(valueOrDefault4))
						{
							list2.Add(valueOrDefault4);
						}
					}
				}
			}
		}
		if (flag)
		{
			EntityUid val = default(EntityUid);
			ActionComponent actionComponent = default(ActionComponent);
			foreach (Entity<ActionComponent> item in _actionsSystem.GetClientActions().Where<Entity<ActionComponent>>((Entity<ActionComponent> action) =>
			{
				VehicleHardpointActionComponent vehicleHardpointActionComponent = default(VehicleHardpointActionComponent);
				return base.EntityManager.TryGetComponent<VehicleHardpointActionComponent>(Entity<ActionComponent>.op_Implicit(action), ref vehicleHardpointActionComponent);
			}).OrderBy(delegate(Entity<ActionComponent> action)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				VehicleHardpointActionComponent vehicleHardpointActionComponent = default(VehicleHardpointActionComponent);
				base.EntityManager.TryGetComponent<VehicleHardpointActionComponent>(Entity<ActionComponent>.op_Implicit(action), ref vehicleHardpointActionComponent);
				return vehicleHardpointActionComponent?.SortOrder ?? 0;
			}))
			{
				item.Deconstruct(ref val, ref actionComponent);
				EntityUid value = val;
				list2.Add(value);
			}
		}
		EntityUid? val2 = null;
		localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault5 = localEntity.GetValueOrDefault();
			VehicleViewToggleComponent vehicleViewToggleComponent2 = default(VehicleViewToggleComponent);
			if (base.EntityManager.TryGetComponent<VehicleViewToggleComponent>(valueOrDefault5, ref vehicleViewToggleComponent2))
			{
				localEntity = vehicleViewToggleComponent2.Action;
				if (localEntity.HasValue)
				{
					EntityUid valueOrDefault6 = localEntity.GetValueOrDefault();
					if (!base.EntityManager.HasComponent<VehicleHardpointActionComponent>(valueOrDefault6))
					{
						val2 = valueOrDefault6;
					}
				}
			}
		}
		if (!val2.HasValue)
		{
			MetaDataComponent val3 = default(MetaDataComponent);
			foreach (Entity<ActionComponent> clientAction in _actionsSystem.GetClientActions())
			{
				EntityUid owner = clientAction.Owner;
				if (!base.EntityManager.HasComponent<VehicleHardpointActionComponent>(owner) && base.EntityManager.TryGetComponent<MetaDataComponent>(owner, ref val3))
				{
					EntityPrototype entityPrototype = val3.EntityPrototype;
					if (!(((entityPrototype != null) ? entityPrototype.ID : null) != "ActionVehicleToggleView"))
					{
						val2 = owner;
						break;
					}
				}
			}
		}
		if (val2.HasValue)
		{
			EntityUid valueOrDefault7 = val2.GetValueOrDefault();
			list2.Add(valueOrDefault7);
		}
		EntityUid? val4 = null;
		localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault8 = localEntity.GetValueOrDefault();
			VehicleLockActionComponent vehicleLockActionComponent = default(VehicleLockActionComponent);
			if (base.EntityManager.TryGetComponent<VehicleLockActionComponent>(valueOrDefault8, ref vehicleLockActionComponent))
			{
				localEntity = vehicleLockActionComponent.Action;
				if (localEntity.HasValue)
				{
					EntityUid valueOrDefault9 = localEntity.GetValueOrDefault();
					if (!base.EntityManager.HasComponent<VehicleHardpointActionComponent>(valueOrDefault9))
					{
						val4 = valueOrDefault9;
					}
				}
			}
		}
		if (!val4.HasValue)
		{
			MetaDataComponent val5 = default(MetaDataComponent);
			foreach (Entity<ActionComponent> clientAction2 in _actionsSystem.GetClientActions())
			{
				EntityUid owner2 = clientAction2.Owner;
				if (!base.EntityManager.HasComponent<VehicleHardpointActionComponent>(owner2) && base.EntityManager.TryGetComponent<MetaDataComponent>(owner2, ref val5))
				{
					EntityPrototype entityPrototype2 = val5.EntityPrototype;
					if (!(((entityPrototype2 != null) ? entityPrototype2.ID : null) != "ActionVehicleLock"))
					{
						val4 = owner2;
						break;
					}
				}
			}
		}
		if (val4.HasValue)
		{
			EntityUid valueOrDefault10 = val4.GetValueOrDefault();
			list2.Add(valueOrDefault10);
		}
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		foreach (EntityUid? item2 in list2)
		{
			if (item2.HasValue)
			{
				EntityUid valueOrDefault11 = item2.GetValueOrDefault();
				hashSet.Add(valueOrDefault11);
			}
		}
		List<EntityUid?> list3 = new List<EntityUid?>();
		foreach (EntityUid? vehicleAction in _vehicleActions)
		{
			if (vehicleAction.HasValue)
			{
				EntityUid valueOrDefault12 = vehicleAction.GetValueOrDefault();
				if (hashSet.Remove(valueOrDefault12))
				{
					list3.Add(valueOrDefault12);
				}
			}
		}
		foreach (EntityUid? item3 in list2)
		{
			if (item3.HasValue)
			{
				EntityUid valueOrDefault13 = item3.GetValueOrDefault();
				if (hashSet.Remove(valueOrDefault13))
				{
					list3.Add(valueOrDefault13);
				}
			}
		}
		_vehicleActions.Clear();
		_vehicleActions.AddRange(list3);
		if (list.Count != _vehicleActions.Count)
		{
			return true;
		}
		for (int num = 0; num < list.Count; num++)
		{
			localEntity = list[num];
			EntityUid? val6 = _vehicleActions[num];
			if (localEntity.HasValue != val6.HasValue || (localEntity.HasValue && localEntity.GetValueOrDefault() != val6.GetValueOrDefault()))
			{
				return true;
			}
		}
		return false;
	}
}
