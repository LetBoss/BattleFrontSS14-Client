// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Squads.CivSquadRadialMenuSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.GlobalMap;
using Content.Client._CIV14merka.Lobby;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.GlobalMap;
using Content.Shared._CIV14merka.Input;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Verbs;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.Squads;

public sealed class CivSquadRadialMenuSystem : EntitySystem
{
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private CivGlobalMapSystem _globalMap;
  [Dependency]
  private SharedVerbSystem _verbs;
  [Dependency]
  private CivRosterSystem _roster;
  private CivSquadRadialMenu? _menu;
  private MapCoordinates _pendingCoordinates = MapCoordinates.Nullspace;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(CivKeyFunctions.CivSquadRadial, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(OpenMenu)), true, false)).Register<CivSquadRadialMenuSystem>();
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<CivSquadRadialMenuSystem>();
    this.CloseMenu();
  }

  private bool OpenMenu(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State == null)
      return this.HandleReleaseSelection();
    if (args.State != 1 || !(this._ui.CurrentlyHovered is IViewportControl))
      return false;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent member;
    if (!localEntity.HasValue || !this.TryComp<CivTeamMemberComponent>(localEntity.Value, ref member) || member.TeamId <= 0 || !member.IsCommander && member.SquadId <= 0 || ((EntityUid) ref args.EntityUid).IsValid() && this._verbs.GetLocalVerbs(args.EntityUid, localEntity.Value, typeof (AlternativeVerb)).Count > 0)
      return false;
    MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
    if (MapId.op_Equality(map.MapId, MapId.Nullspace))
      return false;
    this._pendingCoordinates = map;
    this.CloseMenu();
    CivSquadRadialMenuState squadRadialMenuState = this.BuildMenuState(member);
    this._menu = new CivSquadRadialMenu();
    this._menu.SetOptions(squadRadialMenuState.TeamId, squadRadialMenuState.Title, squadRadialMenuState.Description, squadRadialMenuState.Options);
    this._menu.OnActionSelected += new Action<CivSquadRadialAction>(this.OnActionSelected);
    this._menu.OnClose += new Action(this.HandleMenuClosed);
    this._menu.OpenCenteredAt(this._input.MouseScreenPosition.Position / Vector2i.op_Implicit(this._clyde.ScreenSize));
    return true;
  }

  private bool HandleReleaseSelection()
  {
    if (this._menu == null)
      return false;
    CivSquadRadialAction action;
    if (this._menu.TryGetHoveredAction(this._ui.CurrentlyHovered, out action))
      this.OnActionSelected(action);
    else
      this.CloseMenu();
    return true;
  }

  private CivSquadRadialMenuState BuildMenuState(CivTeamMemberComponent member)
  {
    return !member.IsCommander ? this.BuildSquadMenuState(member.TeamId, member.SquadId, member.IsSquadLeader) : this.BuildCommanderMenuState(member.TeamId);
  }

  private CivSquadRadialMenuState BuildSquadMenuState(int teamId, int squadId, bool isSquadLeader)
  {
    List<CivSquadRadialOption> Options = new List<CivSquadRadialOption>(isSquadLeader ? 6 : 3);
    if (isSquadLeader)
    {
      Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Attack, this.Loc.GetString("civ-ui-squad-attack"), this.Loc.GetString("civ-ui-squad-sl-attack-desc"), this.Loc.GetString("civ-ui-squad-sl-pointer-hint"), Color.FromHex((ReadOnlySpan<char>) "#FF5B4D", new Color?())));
      Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Defense, this.Loc.GetString("civ-ui-squad-defense"), this.Loc.GetString("civ-ui-squad-sl-defense-desc"), this.Loc.GetString("civ-ui-squad-sl-pointer-hint"), Color.FromHex((ReadOnlySpan<char>) "#5B9DFF", new Color?())));
      Options.Add(new CivSquadRadialOption(CivSquadRadialAction.ManageRoster, this.Loc.GetString("civ-ui-squad-manage"), this.Loc.GetString("civ-ui-squad-manage-desc"), this.Loc.GetString("civ-ui-squad-manage-hint"), Color.FromHex((ReadOnlySpan<char>) "#C88EFF", new Color?())));
    }
    Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Enemy, this.Loc.GetString("civ-ui-squad-enemy"), this.Loc.GetString("civ-ui-squad-enemy-desc"), this.Loc.GetString("civ-ui-squad-enemy-hint"), Color.FromHex((ReadOnlySpan<char>) "#FF7A45", new Color?())));
    Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Help, this.Loc.GetString("civ-ui-squad-help"), this.Loc.GetString("civ-ui-squad-help-desc"), this.Loc.GetString("civ-ui-squad-help-hint"), Color.FromHex((ReadOnlySpan<char>) "#FFD454", new Color?())));
    Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Allies, this.Loc.GetString("civ-ui-squad-allies"), this.Loc.GetString("civ-ui-squad-allies-desc"), this.Loc.GetString("civ-ui-squad-allies-hint"), Color.FromHex((ReadOnlySpan<char>) "#6FE48D", new Color?())));
    string Title = isSquadLeader ? this.Loc.GetString("civ-ui-squad-title-squad", ("id", (object) squadId)) : this.Loc.GetString("civ-ui-squad-title-group", ("id", (object) squadId));
    string Description = isSquadLeader ? this.Loc.GetString("civ-ui-squad-desc-sl") : this.Loc.GetString("civ-ui-squad-desc-member");
    return new CivSquadRadialMenuState(teamId, Title, Description, (IReadOnlyList<CivSquadRadialOption>) Options);
  }

  private CivSquadRadialMenuState BuildCommanderMenuState(int teamId)
  {
    List<CivSquadRadialOption> Options = new List<CivSquadRadialOption>(5);
    int squadId;
    if (this._globalMap.TryGetCommanderSelectedSquadId(out squadId))
    {
      Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Attack, this.Loc.GetString("civ-ui-squad-attack"), this.Loc.GetString("civ-ui-squad-cmd-attack-desc", ("id", (object) squadId)), this.Loc.GetString("civ-ui-squad-cmd-attack-hint"), Color.FromHex((ReadOnlySpan<char>) "#FF5B4D", new Color?())));
      Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Defense, this.Loc.GetString("civ-ui-squad-defense"), this.Loc.GetString("civ-ui-squad-cmd-defense-desc", ("id", (object) squadId)), this.Loc.GetString("civ-ui-squad-cmd-defense-hint"), Color.FromHex((ReadOnlySpan<char>) "#5B9DFF", new Color?())));
    }
    Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Artillery, this.Loc.GetString("civ-ui-squad-artillery"), this.Loc.GetString("civ-ui-squad-artillery-desc"), this.Loc.GetString("civ-ui-squad-artillery-hint"), Color.FromHex((ReadOnlySpan<char>) "#FFD454", new Color?())));
    Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Enemy, this.Loc.GetString("civ-ui-squad-enemy"), this.Loc.GetString("civ-ui-squad-cmd-enemy-desc"), this.Loc.GetString("civ-ui-squad-cmd-enemy-hint"), Color.FromHex((ReadOnlySpan<char>) "#FF7A45", new Color?())));
    Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Recon, this.Loc.GetString("civ-ui-squad-recon"), this.Loc.GetString("civ-ui-squad-recon-desc"), this.Loc.GetString("civ-ui-squad-recon-hint"), Color.FromHex((ReadOnlySpan<char>) "#54C8C8", new Color?())));
    Options.Add(new CivSquadRadialOption(CivSquadRadialAction.Command, this.Loc.GetString("civ-ui-squad-command"), this.Loc.GetString("civ-ui-squad-command-desc"), this.Loc.GetString("civ-ui-squad-command-hint"), Color.FromHex((ReadOnlySpan<char>) "#C88EFF", new Color?())));
    string Title = this._globalMap.TryGetCommanderSelectedSquadId(out squadId) ? this.Loc.GetString("civ-ui-squad-title-command", ("id", (object) squadId)) : this.Loc.GetString("civ-ui-squad-title-commander");
    string Description = this._globalMap.TryGetCommanderSelectedSquadId(out int _) ? this.Loc.GetString("civ-ui-squad-desc-command-selected") : this.Loc.GetString("civ-ui-squad-desc-command-none");
    return new CivSquadRadialMenuState(teamId, Title, Description, (IReadOnlyList<CivSquadRadialOption>) Options);
  }

  private void OnActionSelected(CivSquadRadialAction action)
  {
    if (action == CivSquadRadialAction.Command)
    {
      this._globalMap.OpenCommanderWindow();
      this.CloseMenu();
    }
    else if (action == CivSquadRadialAction.ManageRoster)
    {
      this._roster.OpenWindow();
      this.CloseMenu();
    }
    else if (MapId.op_Equality(this._pendingCoordinates.MapId, MapId.Nullspace))
    {
      this.CloseMenu();
    }
    else
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      CivTeamMemberComponent teamMemberComponent;
      if (localEntity.HasValue && this.TryComp<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) && teamMemberComponent.IsCommander)
      {
        switch (action)
        {
          case CivSquadRadialAction.Artillery:
            this._globalMap.RequestCommanderCallArtillery(this._pendingCoordinates.MapId, this._pendingCoordinates.Position);
            this.CloseMenu();
            break;
          case CivSquadRadialAction.Enemy:
            this._globalMap.RequestPlaceMarker(CivGlobalMapMarkerType.Enemy, this._pendingCoordinates.MapId, this._pendingCoordinates.Position);
            this.CloseMenu();
            break;
          case CivSquadRadialAction.Recon:
            this._globalMap.RequestCommanderRecon(this._pendingCoordinates.Position);
            this.CloseMenu();
            break;
          default:
            CivCommanderOrderType orderType;
            int squadId;
            if (!CivSquadRadialMenuSystem.TryGetCommanderOrderType(action, out orderType) || !this._globalMap.TryGetCommanderSelectedSquadId(out squadId))
            {
              this._globalMap.OpenCommanderWindow();
              this.CloseMenu();
              break;
            }
            this._globalMap.RequestCommanderSetOrder(squadId, orderType, this._pendingCoordinates.MapId, this._pendingCoordinates.Position);
            this.CloseMenu();
            break;
        }
      }
      else
      {
        this._globalMap.RequestPlaceMarker(CivSquadRadialMenuSystem.GetMarkerType(action), this._pendingCoordinates.MapId, this._pendingCoordinates.Position);
        this.CloseMenu();
      }
    }
  }

  private static CivGlobalMapMarkerType GetMarkerType(CivSquadRadialAction action)
  {
    CivGlobalMapMarkerType markerType;
    switch (action)
    {
      case CivSquadRadialAction.Attack:
        markerType = CivGlobalMapMarkerType.Attack;
        break;
      case CivSquadRadialAction.Defense:
        markerType = CivGlobalMapMarkerType.Defense;
        break;
      case CivSquadRadialAction.Help:
        markerType = CivGlobalMapMarkerType.Help;
        break;
      case CivSquadRadialAction.Allies:
        markerType = CivGlobalMapMarkerType.Allies;
        break;
      default:
        markerType = CivGlobalMapMarkerType.Enemy;
        break;
    }
    return markerType;
  }

  private static bool TryGetCommanderOrderType(
    CivSquadRadialAction action,
    out CivCommanderOrderType orderType)
  {
    CivCommanderOrderType commanderOrderType;
    switch (action)
    {
      case CivSquadRadialAction.Attack:
        commanderOrderType = CivCommanderOrderType.Attack;
        break;
      case CivSquadRadialAction.Defense:
        commanderOrderType = CivCommanderOrderType.Defense;
        break;
      default:
        commanderOrderType = CivCommanderOrderType.None;
        break;
    }
    orderType = commanderOrderType;
    return orderType != 0;
  }

  private void HandleMenuClosed() => this.CloseMenu();

  private void CloseMenu()
  {
    if (this._menu == null)
      return;
    this._menu.OnActionSelected -= new Action<CivSquadRadialAction>(this.OnActionSelected);
    this._menu.OnClose -= new Action(this.HandleMenuClosed);
    ((Control) this._menu).Orphan();
    this._menu = (CivSquadRadialMenu) null;
    this._pendingCoordinates = MapCoordinates.Nullspace;
  }
}
