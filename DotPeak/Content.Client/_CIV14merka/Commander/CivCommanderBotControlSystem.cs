// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderBotControlSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
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

  public IReadOnlySet<EntityUid> SelectedBots => (IReadOnlySet<EntityUid>) this._selectedBots;

  public IReadOnlyList<CivBotStateInfo> BotStates
  {
    get => (IReadOnlyList<CivBotStateInfo>) this._botStates;
  }

  public bool IsBoxSelecting => this._isBoxSelecting;

  public Vector2 BoxSelectStart => this._boxSelectStart;

  public Vector2 BoxSelectEnd => this._boxSelectEnd;

  public CivBotOrderType CurrentOrderMode => this._currentOrderMode;

  public bool IsPatrolMode => this._isPatrolMode;

  public IReadOnlyList<Vector2> PatrolPoints => (IReadOnlyList<Vector2>) this._patrolPoints;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(EngineKeyFunctions.Use, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleUse)), true, true)).Bind(EngineKeyFunctions.UseSecondary, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleMove)), true, true)).Bind(CivKeyFunctions.CivBotOrderRadial, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleOrderRadial)), true, true)).Register<CivCommanderBotControlSystem>();
    this.SubscribeNetworkEvent<CivBotStateUpdateEvent>(new EntitySessionEventHandler<CivBotStateUpdateEvent>(this.OnBotStateUpdate), (Type[]) null, (Type[]) null);
    this._overlay = new CivCommanderBotOverlay((IEntityManager) this.EntityManager, this);
    this._overlays.AddOverlay((Overlay) this._overlay);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<CivCommanderBotControlSystem>();
    this.ClearSelection();
    if (this._overlay != null)
    {
      this._overlays.RemoveOverlay((Overlay) this._overlay);
      this._overlay = (CivCommanderBotOverlay) null;
    }
    if (this._orderMenu == null)
      return;
    this._orderMenu.OnOrderSelected -= new Action<CivBotOrderType>(this.OnRadialOrderSelected);
    ((Control) this._orderMenu).Dispose();
    this._orderMenu = (CivBotOrderRadialMenu) null;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    int teamId;
    if (!this.TryGetCommander(out EntityUid _, out teamId))
    {
      this.ClearSelection();
      this._botStates.Clear();
    }
    else
    {
      this._deselectScratch.Clear();
      foreach (EntityUid selectedBot in this._selectedBots)
      {
        if (!this.IsSelectable(selectedBot, teamId))
          this._deselectScratch.Add(selectedBot);
      }
      foreach (EntityUid entityUid in this._deselectScratch)
        this._selectedBots.Remove(entityUid);
    }
  }

  private void OnBotStateUpdate(CivBotStateUpdateEvent msg, EntitySessionEventArgs args)
  {
    this._botStates.Clear();
    this._botStates.AddRange((IEnumerable<CivBotStateInfo>) msg.Bots);
  }

  public void ClearSelection()
  {
    foreach (EntityUid selectedBot in this._selectedBots)
    {
      CivCommanderBotComponent commanderBotComponent;
      if (this.TryComp<CivCommanderBotComponent>(selectedBot, ref commanderBotComponent))
      {
        commanderBotComponent.IsSelected = false;
        this.Dirty(selectedBot, (IComponent) commanderBotComponent, (MetaDataComponent) null);
      }
    }
    this._selectedBots.Clear();
    this._patrolPoints.Clear();
    this._isPatrolMode = false;
  }

  public void SelectBot(EntityUid uid, bool addToSelection = false)
  {
    int teamId;
    if (!this.TryGetCommander(out EntityUid _, out teamId) || !this.IsSelectable(uid, teamId))
      return;
    if (!addToSelection)
      this.ClearSelection();
    this._selectedBots.Add(uid);
    CivCommanderBotComponent commanderBotComponent;
    if (this.TryComp<CivCommanderBotComponent>(uid, ref commanderBotComponent))
    {
      commanderBotComponent.IsSelected = true;
      this.Dirty(uid, (IComponent) commanderBotComponent, (MetaDataComponent) null);
    }
    this.SyncSelectionToServer();
  }

  public void DeselectBot(EntityUid uid)
  {
    if (!this._selectedBots.Remove(uid))
      return;
    CivCommanderBotComponent commanderBotComponent;
    if (this.TryComp<CivCommanderBotComponent>(uid, ref commanderBotComponent))
    {
      commanderBotComponent.IsSelected = false;
      this.Dirty(uid, (IComponent) commanderBotComponent, (MetaDataComponent) null);
    }
    this.SyncSelectionToServer();
  }

  public void SelectBots(IEnumerable<EntityUid> uids, bool addToSelection = false)
  {
    int teamId;
    if (!this.TryGetCommander(out EntityUid _, out teamId))
      return;
    if (!addToSelection)
      this.ClearSelection();
    foreach (EntityUid uid in uids)
    {
      if (this.IsSelectable(uid, teamId))
      {
        this._selectedBots.Add(uid);
        CivCommanderBotComponent commanderBotComponent;
        if (this.TryComp<CivCommanderBotComponent>(uid, ref commanderBotComponent))
        {
          commanderBotComponent.IsSelected = true;
          this.Dirty(uid, (IComponent) commanderBotComponent, (MetaDataComponent) null);
        }
      }
    }
    this.SyncSelectionToServer();
  }

  public void SelectSquad(int squadId)
  {
    int teamId;
    if (!this.TryGetCommander(out EntityUid _, out teamId))
      return;
    this.ClearSelection();
    EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent>();
    EntityUid uid;
    CivCommanderBotComponent commanderBotComponent;
    CivTeamMemberComponent teamMemberComponent;
    while (entityQueryEnumerator.MoveNext(ref uid, ref commanderBotComponent, ref teamMemberComponent))
    {
      if (teamMemberComponent.TeamId == teamId && commanderBotComponent.SquadId == squadId && squadId != 0 && this.IsSelectable(uid, teamId))
      {
        this._selectedBots.Add(uid);
        commanderBotComponent.IsSelected = true;
        this.Dirty(uid, (IComponent) commanderBotComponent, (MetaDataComponent) null);
      }
    }
    this.RaiseNetworkEvent((EntityEventArgs) new CivBotSelectSquadRequestEvent(squadId));
  }

  public void SetOrderMode(CivBotOrderType orderType)
  {
    this._currentOrderMode = orderType;
    this._isPatrolMode = orderType == CivBotOrderType.Patrol;
    this._patrolPoints.Clear();
  }

  public void SendOrder(CivBotOrderType orderType, MapCoordinates target, EntityUid? followTarget = null)
  {
    if (this._selectedBots.Count == 0 || MapId.op_Equality(target.MapId, MapId.Nullspace))
      return;
    List<NetEntity> list = this._selectedBots.Select<EntityUid, NetEntity>((Func<EntityUid, NetEntity>) (uid => this.GetNetEntity(uid, (MetaDataComponent) null))).ToList<NetEntity>();
    NetEntity? followTarget1 = followTarget.HasValue ? new NetEntity?(this.GetNetEntity(followTarget.Value, (MetaDataComponent) null)) : new NetEntity?();
    this.RaiseNetworkEvent((EntityEventArgs) new CivBotOrderRequestEvent((IEnumerable<NetEntity>) list, orderType, target.MapId, target.Position, followTarget1));
  }

  public void SendPatrol(List<Vector2> points, MapId mapId)
  {
    if (this._selectedBots.Count == 0 || points.Count < 2 || MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new CivBotPatrolPointsRequestEvent((IEnumerable<NetEntity>) this._selectedBots.Select<EntityUid, NetEntity>((Func<EntityUid, NetEntity>) (uid => this.GetNetEntity(uid, (MetaDataComponent) null))).ToList<NetEntity>(), (IEnumerable<Vector2>) points, mapId));
  }

  public void AddPatrolPoint(Vector2 point) => this._patrolPoints.Add(point);

  public void FinishPatrol(MapId mapId)
  {
    if (this._patrolPoints.Count >= 2)
      this.SendPatrol(this._patrolPoints.ToList<Vector2>(), mapId);
    this._patrolPoints.Clear();
    this._isPatrolMode = false;
    this._currentOrderMode = CivBotOrderType.Move;
  }

  public void CancelPatrol()
  {
    this._patrolPoints.Clear();
    this._isPatrolMode = false;
    this._currentOrderMode = CivBotOrderType.Move;
  }

  public void StartBoxSelect(Vector2 screenPos)
  {
    this._isBoxSelecting = true;
    this._boxSelectStart = screenPos;
    this._boxSelectEnd = screenPos;
  }

  public void UpdateBoxSelect(Vector2 screenPos)
  {
    if (!this._isBoxSelecting)
      return;
    this._boxSelectEnd = screenPos;
  }

  public void EndBoxSelect(bool addToSelection = false)
  {
    if (!this._isBoxSelecting)
      return;
    this._isBoxSelecting = false;
    int teamId;
    if (!this.TryGetCommander(out EntityUid _, out teamId))
      return;
    float num1 = MathF.Min(this._boxSelectStart.X, this._boxSelectEnd.X);
    float num2 = MathF.Max(this._boxSelectStart.X, this._boxSelectEnd.X);
    float num3 = MathF.Min(this._boxSelectStart.Y, this._boxSelectEnd.Y);
    float num4 = MathF.Max(this._boxSelectStart.Y, this._boxSelectEnd.Y);
    if ((double) num2 - (double) num1 < 5.0 && (double) num4 - (double) num3 < 5.0)
    {
      this.ClearSelection();
    }
    else
    {
      List<EntityUid> uids = new List<EntityUid>();
      EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent>();
      EntityUid uid;
      CivCommanderBotComponent commanderBotComponent;
      CivTeamMemberComponent teamMemberComponent;
      TransformComponent transformComponent;
      while (entityQueryEnumerator.MoveNext(ref uid, ref commanderBotComponent, ref teamMemberComponent, ref transformComponent))
      {
        if (teamMemberComponent.TeamId == teamId && this.IsSelectable(uid, teamId))
        {
          Vector2 screen = this._eye.WorldToScreen(this._transform.GetWorldPosition(transformComponent));
          if ((double) screen.X >= (double) num1 && (double) screen.X <= (double) num2 && (double) screen.Y >= (double) num3 && (double) screen.Y <= (double) num4)
            uids.Add(uid);
        }
      }
      if (uids.Count <= 0)
        return;
      this.SelectBots((IEnumerable<EntityUid>) uids, addToSelection);
    }
  }

  private void SyncSelectionToServer()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivBotSelectRequestEvent((IEnumerable<NetEntity>) this._selectedBots.Select<EntityUid, NetEntity>((Func<EntityUid, NetEntity>) (uid => this.GetNetEntity(uid, (MetaDataComponent) null))).ToList<NetEntity>()));
  }

  public bool TryGetSelected(out EntityUid uid)
  {
    uid = this._selectedBots.FirstOrDefault<EntityUid>();
    return EntityUid.op_Inequality(uid, EntityUid.Invalid);
  }

  public int GetSelectedCount() => this._selectedBots.Count;

  private bool HandleUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    int teamId;
    if (this._placement.IsActive || !this.TryGetCommander(out EntityUid _, out teamId) || !this.IsViewportHover() || args.State != 1)
      return false;
    if (this._isPatrolMode)
    {
      MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
      if (!MapId.op_Inequality(map.MapId, MapId.Nullspace))
        return false;
      this.AddPatrolPoint(map.Position);
      return true;
    }
    if (this.IsSelectable(args.EntityUid, teamId))
    {
      if (this._selectedBots.Contains(args.EntityUid))
      {
        this.DeselectBot(args.EntityUid);
        return true;
      }
      this.SelectBot(args.EntityUid, true);
      return true;
    }
    if (this._selectedBots.Count <= 0)
      return false;
    Vector2 position = this._input.MouseScreenPosition.Position;
    EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent>();
    EntityUid entityUid;
    CivCommanderBotComponent commanderBotComponent;
    CivTeamMemberComponent teamMemberComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref commanderBotComponent, ref teamMemberComponent, ref transformComponent))
    {
      if (teamMemberComponent.TeamId == teamId && (double) (this._eye.WorldToScreen(this._transform.GetWorldPosition(transformComponent)) - position).Length() < 64.0)
        return false;
    }
    this.ClearSelection();
    return true;
  }

  private bool HandleMove(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State != 1 || this._placement.IsActive || this._selectedBots.Count == 0 || !this.TryGetCommander(out EntityUid _, out int _) || !this.IsViewportHover())
      return false;
    if (this._isPatrolMode)
    {
      MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
      if (!MapId.op_Inequality(map.MapId, MapId.Nullspace))
        return false;
      this.FinishPatrol(map.MapId);
      return true;
    }
    MapCoordinates map1 = this._eye.PixelToMap(this._input.MouseScreenPosition);
    if (MapId.op_Equality(map1.MapId, MapId.Nullspace))
      return false;
    if (this._currentOrderMode == CivBotOrderType.Follow && EntityUid.op_Inequality(args.EntityUid, EntityUid.Invalid))
    {
      this.SendOrder(CivBotOrderType.Follow, map1, new EntityUid?(args.EntityUid));
      return true;
    }
    this.SendOrder(this._currentOrderMode, map1);
    return true;
  }

  private bool HandleOrderRadial(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State == null || args.State != 1 || this._selectedBots.Count == 0 || !this.TryGetCommander(out EntityUid _, out int _) || !this.IsViewportHover())
      return false;
    ((Control) this._orderMenu)?.Dispose();
    this._orderMenu = this.CreateOrderMenu();
    this._orderMenu.OpenCentered();
    return true;
  }

  private CivBotOrderRadialMenu CreateOrderMenu()
  {
    CivBotOrderRadialMenu orderMenu = new CivBotOrderRadialMenu();
    orderMenu.OnOrderSelected += new Action<CivBotOrderType>(this.OnRadialOrderSelected);
    orderMenu.OnClose += (Action) (() => this._ui.KeyboardFocused?.ReleaseKeyboardFocus());
    return orderMenu;
  }

  private void OnRadialOrderSelected(CivBotOrderType order)
  {
    this.SetOrderMode(order);
    this._orderMenu?.Close();
  }

  public virtual void FrameUpdate(float frameTime)
  {
    if (!this._isBoxSelecting)
      return;
    ScreenCoordinates mouseScreenPosition = this._input.MouseScreenPosition;
    if (!((ScreenCoordinates) ref mouseScreenPosition).IsValid)
      return;
    this.UpdateBoxSelect(mouseScreenPosition.Position);
  }

  private bool TryGetCommander(out EntityUid uid, out int teamId)
  {
    uid = EntityUid.Invalid;
    teamId = 0;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (localEntity.HasValue)
    {
      EntityUid valueOrDefault = localEntity.GetValueOrDefault();
      CivTeamMemberComponent teamMemberComponent;
      if (this.TryComp<CivTeamMemberComponent>(valueOrDefault, ref teamMemberComponent) && teamMemberComponent.IsCommander)
      {
        uid = valueOrDefault;
        teamId = teamMemberComponent.TeamId;
        return teamId != 0;
      }
    }
    return false;
  }

  private bool IsSelectable(EntityUid uid, int teamId)
  {
    CivTeamMemberComponent teamMemberComponent;
    if (!this.Exists(uid) || !this.HasComp<CivCommanderBotComponent>(uid) || !this.TryComp<CivTeamMemberComponent>(uid, ref teamMemberComponent) || teamMemberComponent.TeamId != teamId)
      return false;
    MobStateComponent mobStateComponent;
    return !this.TryComp<MobStateComponent>(uid, ref mobStateComponent) || mobStateComponent.CurrentState != MobState.Dead;
  }

  private bool IsViewportHover()
  {
    return this._ui.CurrentlyHovered == null || this._ui.CurrentlyHovered is IViewportControl;
  }
}
