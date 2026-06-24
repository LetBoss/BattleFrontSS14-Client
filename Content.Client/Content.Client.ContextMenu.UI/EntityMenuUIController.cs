using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.CombatMode;
using Content.Client.Examine;
using Content.Client.Gameplay;
using Content.Client.Verbs;
using Content.Client.Verbs.UI;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared.CCVar;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.ContextMenu.UI;

public sealed class EntityMenuUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private sealed class PrototypeAndStatesContextMenuComparer : IEqualityComparer<EntityUid>
	{
		private static readonly List<Func<EntityUid, EntityUid, IEntityManager, bool>> EqualsList = new List<Func<EntityUid, EntityUid, IEntityManager, bool>>
		{
			(EntityUid a, EntityUid b, IEntityManager entMan) => entMan.GetComponent<MetaDataComponent>(a).EntityPrototype.ID == entMan.GetComponent<MetaDataComponent>(b).EntityPrototype.ID,
			delegate(EntityUid a, EntityUid b, IEntityManager entMan)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				SpriteComponent val = default(SpriteComponent);
				entMan.TryGetComponent<SpriteComponent>(a, ref val);
				SpriteComponent val2 = default(SpriteComponent);
				entMan.TryGetComponent<SpriteComponent>(b, ref val2);
				if (val != null && val2 != null)
				{
					IEnumerable<string> source = from s in val.AllLayers
						where s.Visible
						select s.RsiState.Name;
					return Enumerable.SequenceEqual(second: from s in val2.AllLayers
						where s.Visible
						select s.RsiState.Name into t
						orderby t
						select t, first: source.OrderBy((string t) => t));
				}
				return val == val2;
			}
		};

		private static readonly List<Func<EntityUid, IEntityManager, int>> GetHashCodeList = new List<Func<EntityUid, IEntityManager, int>>
		{
			(EntityUid e, IEntityManager entMan) => EqualityComparer<string>.Default.GetHashCode(entMan.GetComponent<MetaDataComponent>(e).EntityPrototype.ID),
			delegate(EntityUid e, IEntityManager entMan)
			{
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				int num = 0;
				foreach (string item in from s in entMan.GetComponent<SpriteComponent>(e).AllLayers
					where s.Visible
					select s.RsiState.Name)
				{
					num ^= EqualityComparer<string>.Default.GetHashCode(item);
				}
				return num;
			}
		};

		private readonly int _depth;

		private readonly IEntityManager _entMan;

		private static int Count => EqualsList.Count - 1;

		public PrototypeAndStatesContextMenuComparer(int step = 0, IEntityManager? entMan = null)
		{
			IoCManager.Resolve<IEntityManager>(ref entMan);
			_depth = ((step > Count) ? Count : step);
			_entMan = entMan;
		}

		public bool Equals(EntityUid x, EntityUid y)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (x == default(EntityUid))
			{
				return y == default(EntityUid);
			}
			if (y != default(EntityUid))
			{
				return EqualsList[_depth](x, y, _entMan);
			}
			return false;
		}

		public int GetHashCode(EntityUid e)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return GetHashCodeList[_depth](e, _entMan);
		}
	}

	public const int GroupingTypesCount = 2;

	[Dependency]
	private IEntitySystemManager _systemManager;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IStateManager _stateManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private ContextMenuUIController _context;

	[Dependency]
	private VerbMenuUIController _verb;

	[UISystemDependency]
	private readonly VerbSystem _verbSystem;

	[UISystemDependency]
	private readonly ExamineSystem _examineSystem;

	[UISystemDependency]
	private readonly TransformSystem _xform;

	[UISystemDependency]
	private readonly CombatModeSystem _combatMode;

	private bool _updating;

	public Dictionary<EntityUid, EntityMenuElement> Elements = new Dictionary<EntityUid, EntityMenuElement>();

	private int GroupingContextMenuType { get; set; }

	public void OnGroupingChanged(int obj)
	{
		_context.Close();
		GroupingContextMenuType = obj;
	}

	private List<List<EntityUid>> GroupEntities(IEnumerable<EntityUid> entities, int depth = 0)
	{
		if (GroupingContextMenuType == 0)
		{
			return (from grp in (from e in entities
					group e by Identity.Name(e, _entityManager, ((ISharedPlayerManager)_playerManager).LocalEntity)).ToList()
				select grp.ToList()).ToList();
		}
		return (from grp in entities.GroupBy((EntityUid e) => e, new PrototypeAndStatesContextMenuComparer(depth, _entityManager)).ToList()
			select grp.ToList()).ToList();
	}

	public void OnStateEntered(GameplayState state)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		_updating = true;
		_cfg.OnValueChanged<int>(CCVars.EntityMenuGroupingType, (Action<int>)OnGroupingChanged, true);
		ContextMenuUIController context = _context;
		context.OnContextKeyEvent = (Action<ContextMenuElement, GUIBoundKeyEventArgs>)Delegate.Combine(context.OnContextKeyEvent, new Action<ContextMenuElement, GUIBoundKeyEventArgs>(OnKeyBindDown));
		CommandBinds.Builder.Bind(EngineKeyFunctions.UseSecondary, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleOpenEntityMenu), true, true)).Register<EntityMenuUIController>();
	}

	public void OnStateExited(GameplayState state)
	{
		_updating = false;
		Elements.Clear();
		_cfg.UnsubValueChanged<int>(CCVars.EntityMenuGroupingType, (Action<int>)OnGroupingChanged);
		ContextMenuUIController context = _context;
		context.OnContextKeyEvent = (Action<ContextMenuElement, GUIBoundKeyEventArgs>)Delegate.Remove(context.OnContextKeyEvent, new Action<ContextMenuElement, GUIBoundKeyEventArgs>(OnKeyBindDown));
		CommandBinds.Unregister<EntityMenuUIController>();
	}

	public void OpenRootMenu(List<EntityUid> entities)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (((Control)_context.RootMenu).Visible)
		{
			_context.Close();
		}
		List<List<EntityUid>> list = GroupEntities(entities).ToList();
		list.Sort((List<EntityUid> x, List<EntityUid> y) => string.Compare(Identity.Name(x.First(), _entityManager, ((ISharedPlayerManager)_playerManager).LocalEntity), Identity.Name(y.First(), _entityManager, ((ISharedPlayerManager)_playerManager).LocalEntity), StringComparison.CurrentCulture));
		Elements.Clear();
		AddToUI(list);
		UIBox2 value = UIBox2.FromDimensions(_userInterfaceManager.MousePositionScaled.Position, new Vector2(1f, 1f));
		((Popup)_context.RootMenu).Open((UIBox2?)value, (Vector2?)null, (Vector2?)null);
	}

	public void OnKeyBindDown(ContextMenuElement element, GUIBoundKeyEventArgs args)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (!(element is EntityMenuElement { Entity: var val }))
		{
			return;
		}
		EntityUid? val2 = val;
		if (!val2.HasValue)
		{
			val = GetFirstEntityOrNull(element.SubMenu);
		}
		if (_entityManager.Deleted(val))
		{
			return;
		}
		if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ExamineEntity)
		{
			_systemManager.GetEntitySystem<ExamineSystem>().DoExamine(val.Value);
			((BoundKeyEventArgs)args).Handle();
		}
		else if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.Use || ((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ActivateItemInWorld || ((BoundKeyEventArgs)args).Function == ContentKeyFunctions.AltActivateItemInWorld || ((BoundKeyEventArgs)args).Function == ContentKeyFunctions.Point || ((BoundKeyEventArgs)args).Function == ContentKeyFunctions.TryPullObject || ((BoundKeyEventArgs)args).Function == ContentKeyFunctions.MovePulledObject)
		{
			InputSystem entitySystem = _systemManager.GetEntitySystem<InputSystem>();
			BoundKeyFunction function = ((BoundKeyEventArgs)args).Function;
			KeyFunctionId val3 = _inputManager.NetworkBindMap.KeyFunctionID(function);
			ClientFullInputCmdMessage val4 = new ClientFullInputCmdMessage(_gameTiming.CurTick, _gameTiming.TickFraction, val3);
			val4.set_State((BoundKeyState)1);
			val4.set_Coordinates(_entityManager.GetComponent<TransformComponent>(val.Value).Coordinates);
			val4.set_ScreenCoordinates(((BoundKeyEventArgs)args).PointerLocation);
			val4.set_Uid(val.Value);
			ClientFullInputCmdMessage val5 = val4;
			ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
			if (localSession != null)
			{
				entitySystem.HandleInputCommand(localSession, function, (IFullInputCmdMessage)(object)val5, false);
			}
			_context.Close();
			((BoundKeyEventArgs)args).Handle();
		}
	}

	private bool HandleOpenEntityMenu(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if ((int)args.State != 1)
		{
			return false;
		}
		if (!(_stateManager.CurrentState is GameplayStateBase))
		{
			return false;
		}
		CombatModeSystem combatMode = _combatMode;
		ICommonSession session = args.Session;
		if (combatMode.IsInCombatMode((session != null) ? session.AttachedEntity : ((EntityUid?)null)))
		{
			return false;
		}
		MapCoordinates targetPos = ((SharedTransformSystem)_xform).ToMapCoordinates(args.Coordinates, true);
		if (_verbSystem.TryGetEntityMenuEntities(targetPos, out List<EntityUid> entities))
		{
			OpenRootMenu(entities);
		}
		return true;
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		if (!_updating || _context.RootMenu == null || !((Control)_context.RootMenu).Visible)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid player = localEntity.GetValueOrDefault();
		if (!((EntityUid)(ref player)).IsValid())
		{
			return;
		}
		MenuVisibility menuVisibility = _verbSystem.Visibility;
		if (!_eyeManager.CurrentEye.DrawFov)
		{
			menuVisibility &= ~MenuVisibility.NoFov;
		}
		MenuVisibilityEvent menuVisibilityEvent = new MenuVisibilityEvent
		{
			Visibility = menuVisibility
		};
		((IDirectedEventBus)_entityManager.EventBus).RaiseLocalEvent<MenuVisibilityEvent>(player, ref menuVisibilityEvent, false);
		menuVisibility = menuVisibilityEvent.Visibility;
		ExaminerComponent examinerComp = default(ExaminerComponent);
		_entityManager.TryGetComponent<ExaminerComponent>(player, ref examinerComp);
		EntityQuery<TransformComponent> entityQuery = _entityManager.GetEntityQuery<TransformComponent>();
		TransformComponent val = default(TransformComponent);
		MapCoordinates target = default(MapCoordinates);
		foreach (EntityUid entity in Elements.Keys.ToList())
		{
			if (!entityQuery.TryGetComponent(entity, ref val))
			{
				RemoveEntity(entity);
			}
			else if ((menuVisibility & MenuVisibility.NoFov) != MenuVisibility.NoFov)
			{
				((MapCoordinates)(ref target))._002Ector(((SharedTransformSystem)_xform).GetWorldPosition(val, entityQuery), val.MapID);
				if (!_examineSystem.CanExamine(player, target, (EntityUid e) => e == player || e == entity, entity, examinerComp))
				{
					RemoveEntity(entity);
				}
			}
		}
	}

	private void AddToUI(List<List<EntityUid>> entityGroups)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (entityGroups.Count == 1)
		{
			AddGroupToMenu(entityGroups[0], _context.RootMenu);
			return;
		}
		foreach (List<EntityUid> entityGroup in entityGroups)
		{
			if (entityGroup.Count > 1)
			{
				AddGroupToUI(entityGroup);
			}
			else
			{
				AddEntityToMenu(entityGroup[0], _context.RootMenu);
			}
		}
	}

	private void AddGroupToUI(List<EntityUid> group)
	{
		EntityMenuElement entityMenuElement = new EntityMenuElement();
		ContextMenuPopup menu = new ContextMenuPopup(_context, entityMenuElement);
		AddGroupToMenu(group, menu);
		UpdateElement(entityMenuElement);
		_context.AddElement(_context.RootMenu, entityMenuElement);
	}

	private void AddGroupToMenu(List<EntityUid> group, ContextMenuPopup menu)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid item in group)
		{
			AddEntityToMenu(item, menu);
		}
	}

	private void AddEntityToMenu(EntityUid entity, ContextMenuPopup menu)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		EntityMenuElement element = new EntityMenuElement(entity);
		element.SubMenu = new ContextMenuPopup(_context, element);
		((Popup)element.SubMenu).OnPopupOpen += delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			_verb.OpenVerbMenu(entity, force: false, element.SubMenu);
		};
		((Popup)element.SubMenu).OnPopupHide += ((Control)element.SubMenu.MenuBody).DisposeAllChildren;
		_context.AddElement(menu, element);
		Elements.TryAdd(entity, element);
	}

	private void RemoveEntity(EntityUid entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!Elements.TryGetValue(entity, out EntityMenuElement value))
		{
			((UIController)this).Log.Error($"Attempted to remove unknown entity from the entity menu: {_entityManager.GetComponent<MetaDataComponent>(entity).EntityName} ({entity})");
			return;
		}
		ContextMenuElement? obj = value.ParentMenu?.ParentElement;
		((Control)value).Orphan();
		Elements.Remove(entity);
		if (obj is EntityMenuElement element)
		{
			UpdateElement(element);
		}
		if (((Control)_context.RootMenu.MenuBody).ChildCount == 0)
		{
			_context.Close();
		}
	}

	private void UpdateElement(EntityMenuElement element)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (element.SubMenu == null)
		{
			return;
		}
		EntityUid? firstEntityOrNull = GetFirstEntityOrNull(element.SubMenu);
		if (!firstEntityOrNull.HasValue)
		{
			((Control)element).Orphan();
			return;
		}
		element.UpdateEntity(firstEntityOrNull);
		element.UpdateCount();
		if (element.Count == 1)
		{
			element.Entity = firstEntityOrNull;
			((Control)element.SubMenu).Orphan();
			element.SubMenu = null;
			Elements[firstEntityOrNull.Value] = element;
		}
		if (element.ParentMenu?.ParentElement is EntityMenuElement element2)
		{
			UpdateElement(element2);
		}
	}

	private unsafe EntityUid? GetFirstEntityOrNull(ContextMenuPopup? menu)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (menu == null)
		{
			return null;
		}
		Enumerator enumerator = ((Control)menu.MenuBody).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (!(((Enumerator)(ref enumerator)).Current is EntityMenuElement entityMenuElement))
				{
					continue;
				}
				if (entityMenuElement.Entity.HasValue)
				{
					if (!_entityManager.Deleted(entityMenuElement.Entity))
					{
						return entityMenuElement.Entity;
					}
					continue;
				}
				EntityUid? firstEntityOrNull = GetFirstEntityOrNull(entityMenuElement.SubMenu);
				if (firstEntityOrNull.HasValue)
				{
					return firstEntityOrNull;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		return null;
	}
}
