using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client._CIV14merka.GlobalMap;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Input;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderBotControlSystem : EntitySystem
{
	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IOverlayManager _overlays;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private CivCommanderPurchasePlacementSystem _placement;

	[Dependency]
	private CivGlobalMapSystem _globalMap;

	private CivCommanderBotOverlay? _overlay;

	private CivBotOrderRadialMenu? _orderMenu;

	private readonly HashSet<EntityUid> _selectedBots = new HashSet<EntityUid>();

	private readonly List<EntityUid> _deselectScratch = new List<EntityUid>();

	private readonly List<CivBotStateInfo> _botStates = new List<CivBotStateInfo>();

	private bool _isBoxSelecting;

	private Vector2 _boxSelectStart;

	private Vector2 _boxSelectEnd;

	private CivBotOrderType _currentOrderMode = CivBotOrderType.Move;

	private bool _isPatrolMode;

	private readonly List<Vector2> _patrolPoints = new List<Vector2>();

	public IReadOnlySet<EntityUid> SelectedBots => _selectedBots;

	public IReadOnlyList<CivBotStateInfo> BotStates => _botStates;

	public bool IsBoxSelecting => _isBoxSelecting;

	public Vector2 BoxSelectStart => _boxSelectStart;

	public Vector2 BoxSelectEnd => _boxSelectEnd;

	public CivBotOrderType CurrentOrderMode => _currentOrderMode;

	public bool IsPatrolMode => _isPatrolMode;

	public IReadOnlyList<Vector2> PatrolPoints => _patrolPoints;

	public override void Initialize()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		CommandBinds.Builder.Bind(EngineKeyFunctions.Use, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleUse), true, true)).Bind(EngineKeyFunctions.UseSecondary, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleMove), true, true)).Bind(CivKeyFunctions.CivBotOrderRadial, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleOrderRadial), true, true))
			.Register<CivCommanderBotControlSystem>();
		((EntitySystem)this).SubscribeNetworkEvent<CivBotStateUpdateEvent>((EntitySessionEventHandler<CivBotStateUpdateEvent>)OnBotStateUpdate, (Type[])null, (Type[])null);
		_overlay = new CivCommanderBotOverlay((IEntityManager)(object)base.EntityManager, this);
		_overlays.AddOverlay((Overlay)(object)_overlay);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<CivCommanderBotControlSystem>();
		ClearSelection();
		if (_overlay != null)
		{
			_overlays.RemoveOverlay((Overlay)(object)_overlay);
			_overlay = null;
		}
		if (_orderMenu != null)
		{
			_orderMenu.OnOrderSelected -= OnRadialOrderSelected;
			((Control)_orderMenu).Dispose();
			_orderMenu = null;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!TryGetCommander(out var _, out var teamId))
		{
			ClearSelection();
			_botStates.Clear();
			return;
		}
		_deselectScratch.Clear();
		foreach (EntityUid selectedBot in _selectedBots)
		{
			if (!IsSelectable(selectedBot, teamId))
			{
				_deselectScratch.Add(selectedBot);
			}
		}
		foreach (EntityUid item in _deselectScratch)
		{
			_selectedBots.Remove(item);
		}
	}

	private void OnBotStateUpdate(CivBotStateUpdateEvent msg, EntitySessionEventArgs args)
	{
		_botStates.Clear();
		_botStates.AddRange(msg.Bots);
	}

	public void ClearSelection()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		CivCommanderBotComponent civCommanderBotComponent = default(CivCommanderBotComponent);
		foreach (EntityUid selectedBot in _selectedBots)
		{
			if (((EntitySystem)this).TryComp<CivCommanderBotComponent>(selectedBot, ref civCommanderBotComponent))
			{
				civCommanderBotComponent.IsSelected = false;
				((EntitySystem)this).Dirty(selectedBot, (IComponent)(object)civCommanderBotComponent, (MetaDataComponent)null);
			}
		}
		_selectedBots.Clear();
		_patrolPoints.Clear();
		_isPatrolMode = false;
	}

	public void SelectBot(EntityUid uid, bool addToSelection = false)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetCommander(out var _, out var teamId) && IsSelectable(uid, teamId))
		{
			if (!addToSelection)
			{
				ClearSelection();
			}
			_selectedBots.Add(uid);
			CivCommanderBotComponent civCommanderBotComponent = default(CivCommanderBotComponent);
			if (((EntitySystem)this).TryComp<CivCommanderBotComponent>(uid, ref civCommanderBotComponent))
			{
				civCommanderBotComponent.IsSelected = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)civCommanderBotComponent, (MetaDataComponent)null);
			}
			SyncSelectionToServer();
		}
	}

	public void DeselectBot(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (_selectedBots.Remove(uid))
		{
			CivCommanderBotComponent civCommanderBotComponent = default(CivCommanderBotComponent);
			if (((EntitySystem)this).TryComp<CivCommanderBotComponent>(uid, ref civCommanderBotComponent))
			{
				civCommanderBotComponent.IsSelected = false;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)civCommanderBotComponent, (MetaDataComponent)null);
			}
			SyncSelectionToServer();
		}
	}

	public void SelectBots(IEnumerable<EntityUid> uids, bool addToSelection = false)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetCommander(out var _, out var teamId))
		{
			return;
		}
		if (!addToSelection)
		{
			ClearSelection();
		}
		CivCommanderBotComponent civCommanderBotComponent = default(CivCommanderBotComponent);
		foreach (EntityUid uid2 in uids)
		{
			if (IsSelectable(uid2, teamId))
			{
				_selectedBots.Add(uid2);
				if (((EntitySystem)this).TryComp<CivCommanderBotComponent>(uid2, ref civCommanderBotComponent))
				{
					civCommanderBotComponent.IsSelected = true;
					((EntitySystem)this).Dirty(uid2, (IComponent)(object)civCommanderBotComponent, (MetaDataComponent)null);
				}
			}
		}
		SyncSelectionToServer();
	}

	public void SelectSquad(int squadId)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetCommander(out var _, out var teamId))
		{
			return;
		}
		ClearSelection();
		EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent> val = ((EntitySystem)this).EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent>();
		EntityUid val2 = default(EntityUid);
		CivCommanderBotComponent civCommanderBotComponent = default(CivCommanderBotComponent);
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		while (val.MoveNext(ref val2, ref civCommanderBotComponent, ref civTeamMemberComponent))
		{
			if (civTeamMemberComponent.TeamId == teamId && civCommanderBotComponent.SquadId == squadId && squadId != 0 && IsSelectable(val2, teamId))
			{
				_selectedBots.Add(val2);
				civCommanderBotComponent.IsSelected = true;
				((EntitySystem)this).Dirty(val2, (IComponent)(object)civCommanderBotComponent, (MetaDataComponent)null);
			}
		}
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivBotSelectSquadRequestEvent(squadId));
	}

	public void SetOrderMode(CivBotOrderType orderType)
	{
		_currentOrderMode = orderType;
		_isPatrolMode = orderType == CivBotOrderType.Patrol;
		_patrolPoints.Clear();
	}

	public void SendOrder(CivBotOrderType orderType, MapCoordinates target, EntityUid? followTarget = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (_selectedBots.Count != 0 && !(target.MapId == MapId.Nullspace))
		{
			List<NetEntity> bots = _selectedBots.Select((EntityUid uid) => ((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null)).ToList();
			NetEntity? followTarget2 = (followTarget.HasValue ? new NetEntity?(((EntitySystem)this).GetNetEntity(followTarget.Value, (MetaDataComponent)null)) : ((NetEntity?)null));
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivBotOrderRequestEvent(bots, orderType, target.MapId, target.Position, followTarget2));
		}
	}

	public void SendPatrol(List<Vector2> points, MapId mapId)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (_selectedBots.Count != 0 && points.Count >= 2 && !(mapId == MapId.Nullspace))
		{
			List<NetEntity> bots = _selectedBots.Select((EntityUid uid) => ((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null)).ToList();
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivBotPatrolPointsRequestEvent(bots, points, mapId));
		}
	}

	public void AddPatrolPoint(Vector2 point)
	{
		_patrolPoints.Add(point);
	}

	public void FinishPatrol(MapId mapId)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (_patrolPoints.Count >= 2)
		{
			SendPatrol(_patrolPoints.ToList(), mapId);
		}
		_patrolPoints.Clear();
		_isPatrolMode = false;
		_currentOrderMode = CivBotOrderType.Move;
	}

	public void CancelPatrol()
	{
		_patrolPoints.Clear();
		_isPatrolMode = false;
		_currentOrderMode = CivBotOrderType.Move;
	}

	public void StartBoxSelect(Vector2 screenPos)
	{
		_isBoxSelecting = true;
		_boxSelectStart = screenPos;
		_boxSelectEnd = screenPos;
	}

	public void UpdateBoxSelect(Vector2 screenPos)
	{
		if (_isBoxSelecting)
		{
			_boxSelectEnd = screenPos;
		}
	}

	public void EndBoxSelect(bool addToSelection = false)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		if (!_isBoxSelecting)
		{
			return;
		}
		_isBoxSelecting = false;
		if (!TryGetCommander(out var _, out var teamId))
		{
			return;
		}
		float num = MathF.Min(_boxSelectStart.X, _boxSelectEnd.X);
		float num2 = MathF.Max(_boxSelectStart.X, _boxSelectEnd.X);
		float num3 = MathF.Min(_boxSelectStart.Y, _boxSelectEnd.Y);
		float num4 = MathF.Max(_boxSelectStart.Y, _boxSelectEnd.Y);
		if (num2 - num < 5f && num4 - num3 < 5f)
		{
			ClearSelection();
			return;
		}
		List<EntityUid> list = new List<EntityUid>();
		EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		CivCommanderBotComponent civCommanderBotComponent = default(CivCommanderBotComponent);
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref civCommanderBotComponent, ref civTeamMemberComponent, ref val3))
		{
			if (civTeamMemberComponent.TeamId == teamId && IsSelectable(val2, teamId))
			{
				Vector2 worldPosition = _transform.GetWorldPosition(val3);
				Vector2 vector = _eye.WorldToScreen(worldPosition);
				if (vector.X >= num && vector.X <= num2 && vector.Y >= num3 && vector.Y <= num4)
				{
					list.Add(val2);
				}
			}
		}
		if (list.Count > 0)
		{
			SelectBots(list, addToSelection);
		}
	}

	private void SyncSelectionToServer()
	{
		List<NetEntity> bots = _selectedBots.Select((EntityUid uid) => ((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null)).ToList();
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivBotSelectRequestEvent(bots));
	}

	public bool TryGetSelected(out EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		uid = _selectedBots.FirstOrDefault();
		return uid != EntityUid.Invalid;
	}

	public int GetSelectedCount()
	{
		return _selectedBots.Count;
	}

	private bool HandleUse(in PointerInputCmdArgs args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (_placement.IsActive || !TryGetCommander(out var _, out var teamId) || !IsViewportHover())
		{
			return false;
		}
		if ((int)args.State != 1)
		{
			return false;
		}
		if (_isPatrolMode)
		{
			MapCoordinates val = _eye.PixelToMap(_input.MouseScreenPosition);
			if (val.MapId != MapId.Nullspace)
			{
				AddPatrolPoint(val.Position);
				return true;
			}
			return false;
		}
		if (IsSelectable(args.EntityUid, teamId))
		{
			if (_selectedBots.Contains(args.EntityUid))
			{
				DeselectBot(args.EntityUid);
				return true;
			}
			SelectBot(args.EntityUid, addToSelection: true);
			return true;
		}
		if (_selectedBots.Count > 0)
		{
			Vector2 position = _input.MouseScreenPosition.Position;
			EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent> val2 = ((EntitySystem)this).EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent>();
			EntityUid val3 = default(EntityUid);
			CivCommanderBotComponent civCommanderBotComponent = default(CivCommanderBotComponent);
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			TransformComponent val4 = default(TransformComponent);
			while (val2.MoveNext(ref val3, ref civCommanderBotComponent, ref civTeamMemberComponent, ref val4))
			{
				if (civTeamMemberComponent.TeamId == teamId)
				{
					Vector2 worldPosition = _transform.GetWorldPosition(val4);
					if ((_eye.WorldToScreen(worldPosition) - position).Length() < 64f)
					{
						return false;
					}
				}
			}
			ClearSelection();
			return true;
		}
		return false;
	}

	private bool HandleMove(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if ((int)args.State != 1 || _placement.IsActive || _selectedBots.Count == 0 || !TryGetCommander(out var _, out var _) || !IsViewportHover())
		{
			return false;
		}
		if (_isPatrolMode)
		{
			MapCoordinates val = _eye.PixelToMap(_input.MouseScreenPosition);
			if (val.MapId != MapId.Nullspace)
			{
				FinishPatrol(val.MapId);
				return true;
			}
			return false;
		}
		MapCoordinates val2 = _eye.PixelToMap(_input.MouseScreenPosition);
		if (val2.MapId == MapId.Nullspace)
		{
			return false;
		}
		if (_currentOrderMode == CivBotOrderType.Follow && args.EntityUid != EntityUid.Invalid)
		{
			SendOrder(CivBotOrderType.Follow, val2, args.EntityUid);
			return true;
		}
		SendOrder(_currentOrderMode, val2);
		return true;
	}

	private bool HandleOrderRadial(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		if ((int)args.State == 0)
		{
			return false;
		}
		if ((int)args.State != 1)
		{
			return false;
		}
		if (_selectedBots.Count == 0 || !TryGetCommander(out var _, out var _))
		{
			return false;
		}
		if (!IsViewportHover())
		{
			return false;
		}
		CivBotOrderRadialMenu? orderMenu = _orderMenu;
		if (orderMenu != null)
		{
			((Control)orderMenu).Dispose();
		}
		_orderMenu = CreateOrderMenu();
		((BaseWindow)_orderMenu).OpenCentered();
		return true;
	}

	private CivBotOrderRadialMenu CreateOrderMenu()
	{
		CivBotOrderRadialMenu civBotOrderRadialMenu = new CivBotOrderRadialMenu();
		civBotOrderRadialMenu.OnOrderSelected += OnRadialOrderSelected;
		((BaseWindow)civBotOrderRadialMenu).OnClose += delegate
		{
			Control keyboardFocused = _ui.KeyboardFocused;
			if (keyboardFocused != null)
			{
				keyboardFocused.ReleaseKeyboardFocus();
			}
		};
		return civBotOrderRadialMenu;
	}

	private void OnRadialOrderSelected(CivBotOrderType order)
	{
		SetOrderMode(order);
		CivBotOrderRadialMenu? orderMenu = _orderMenu;
		if (orderMenu != null)
		{
			((BaseWindow)orderMenu).Close();
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (_isBoxSelecting)
		{
			ScreenCoordinates mouseScreenPosition = _input.MouseScreenPosition;
			if (((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
			{
				UpdateBoxSelect(mouseScreenPosition.Position);
			}
		}
	}

	private bool TryGetCommander(out EntityUid uid, out int teamId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		uid = EntityUid.Invalid;
		teamId = 0;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent) && civTeamMemberComponent.IsCommander)
			{
				uid = valueOrDefault;
				teamId = civTeamMemberComponent.TeamId;
				return teamId != 0;
			}
		}
		return false;
	}

	private bool IsSelectable(EntityUid uid, int teamId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (((EntitySystem)this).Exists(uid) && ((EntitySystem)this).HasComp<CivCommanderBotComponent>(uid) && ((EntitySystem)this).TryComp<CivTeamMemberComponent>(uid, ref civTeamMemberComponent) && civTeamMemberComponent.TeamId == teamId)
		{
			MobStateComponent mobStateComponent = default(MobStateComponent);
			if (((EntitySystem)this).TryComp<MobStateComponent>(uid, ref mobStateComponent))
			{
				return mobStateComponent.CurrentState != MobState.Dead;
			}
			return true;
		}
		return false;
	}

	private bool IsViewportHover()
	{
		if (_ui.CurrentlyHovered != null)
		{
			return _ui.CurrentlyHovered is IViewportControl;
		}
		return true;
	}
}
