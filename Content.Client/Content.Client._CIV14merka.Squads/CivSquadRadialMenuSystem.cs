using System;
using System.Collections.Generic;
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

	public override void Initialize()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		CommandBinds.Builder.Bind(CivKeyFunctions.CivSquadRadial, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(OpenMenu), true, false)).Register<CivSquadRadialMenuSystem>();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<CivSquadRadialMenuSystem>();
		CloseMenu();
	}

	private bool OpenMenu(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		if ((int)args.State == 0)
		{
			return HandleReleaseSelection();
		}
		if ((int)args.State != 1)
		{
			return false;
		}
		if (!(_ui.CurrentlyHovered is IViewportControl))
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (!localEntity.HasValue || !((EntitySystem)this).TryComp<CivTeamMemberComponent>(localEntity.Value, ref civTeamMemberComponent))
		{
			return false;
		}
		if (civTeamMemberComponent.TeamId <= 0 || (!civTeamMemberComponent.IsCommander && civTeamMemberComponent.SquadId <= 0))
		{
			return false;
		}
		if (((EntityUid)(ref args.EntityUid)).IsValid() && _verbs.GetLocalVerbs(args.EntityUid, localEntity.Value, typeof(AlternativeVerb)).Count > 0)
		{
			return false;
		}
		MapCoordinates val = _eye.PixelToMap(_input.MouseScreenPosition);
		if (val.MapId == MapId.Nullspace)
		{
			return false;
		}
		_pendingCoordinates = val;
		CloseMenu();
		CivSquadRadialMenuState civSquadRadialMenuState = BuildMenuState(civTeamMemberComponent);
		_menu = new CivSquadRadialMenu();
		_menu.SetOptions(civSquadRadialMenuState.TeamId, civSquadRadialMenuState.Title, civSquadRadialMenuState.Description, civSquadRadialMenuState.Options);
		_menu.OnActionSelected += OnActionSelected;
		((BaseWindow)_menu).OnClose += HandleMenuClosed;
		((BaseWindow)_menu).OpenCenteredAt(_input.MouseScreenPosition.Position / Vector2i.op_Implicit(_clyde.ScreenSize));
		return true;
	}

	private bool HandleReleaseSelection()
	{
		if (_menu == null)
		{
			return false;
		}
		if (_menu.TryGetHoveredAction(_ui.CurrentlyHovered, out var action))
		{
			OnActionSelected(action);
		}
		else
		{
			CloseMenu();
		}
		return true;
	}

	private CivSquadRadialMenuState BuildMenuState(CivTeamMemberComponent member)
	{
		if (!member.IsCommander)
		{
			return BuildSquadMenuState(member.TeamId, member.SquadId, member.IsSquadLeader);
		}
		return BuildCommanderMenuState(member.TeamId);
	}

	private CivSquadRadialMenuState BuildSquadMenuState(int teamId, int squadId, bool isSquadLeader)
	{
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		List<CivSquadRadialOption> list = new List<CivSquadRadialOption>(isSquadLeader ? 6 : 3);
		if (isSquadLeader)
		{
			list.Add(new CivSquadRadialOption(CivSquadRadialAction.Attack, base.Loc.GetString("civ-ui-squad-attack"), base.Loc.GetString("civ-ui-squad-sl-attack-desc"), base.Loc.GetString("civ-ui-squad-sl-pointer-hint"), Color.FromHex((ReadOnlySpan<char>)"#FF5B4D", (Color?)null)));
			list.Add(new CivSquadRadialOption(CivSquadRadialAction.Defense, base.Loc.GetString("civ-ui-squad-defense"), base.Loc.GetString("civ-ui-squad-sl-defense-desc"), base.Loc.GetString("civ-ui-squad-sl-pointer-hint"), Color.FromHex((ReadOnlySpan<char>)"#5B9DFF", (Color?)null)));
			list.Add(new CivSquadRadialOption(CivSquadRadialAction.ManageRoster, base.Loc.GetString("civ-ui-squad-manage"), base.Loc.GetString("civ-ui-squad-manage-desc"), base.Loc.GetString("civ-ui-squad-manage-hint"), Color.FromHex((ReadOnlySpan<char>)"#C88EFF", (Color?)null)));
		}
		list.Add(new CivSquadRadialOption(CivSquadRadialAction.Enemy, base.Loc.GetString("civ-ui-squad-enemy"), base.Loc.GetString("civ-ui-squad-enemy-desc"), base.Loc.GetString("civ-ui-squad-enemy-hint"), Color.FromHex((ReadOnlySpan<char>)"#FF7A45", (Color?)null)));
		list.Add(new CivSquadRadialOption(CivSquadRadialAction.Help, base.Loc.GetString("civ-ui-squad-help"), base.Loc.GetString("civ-ui-squad-help-desc"), base.Loc.GetString("civ-ui-squad-help-hint"), Color.FromHex((ReadOnlySpan<char>)"#FFD454", (Color?)null)));
		list.Add(new CivSquadRadialOption(CivSquadRadialAction.Allies, base.Loc.GetString("civ-ui-squad-allies"), base.Loc.GetString("civ-ui-squad-allies-desc"), base.Loc.GetString("civ-ui-squad-allies-hint"), Color.FromHex((ReadOnlySpan<char>)"#6FE48D", (Color?)null)));
		string title = (isSquadLeader ? base.Loc.GetString("civ-ui-squad-title-squad", (ValueTuple<string, object>)("id", squadId)) : base.Loc.GetString("civ-ui-squad-title-group", (ValueTuple<string, object>)("id", squadId)));
		string description = (isSquadLeader ? base.Loc.GetString("civ-ui-squad-desc-sl") : base.Loc.GetString("civ-ui-squad-desc-member"));
		return new CivSquadRadialMenuState(teamId, title, description, list);
	}

	private CivSquadRadialMenuState BuildCommanderMenuState(int teamId)
	{
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		List<CivSquadRadialOption> list = new List<CivSquadRadialOption>(5);
		if (_globalMap.TryGetCommanderSelectedSquadId(out var squadId))
		{
			list.Add(new CivSquadRadialOption(CivSquadRadialAction.Attack, base.Loc.GetString("civ-ui-squad-attack"), base.Loc.GetString("civ-ui-squad-cmd-attack-desc", (ValueTuple<string, object>)("id", squadId)), base.Loc.GetString("civ-ui-squad-cmd-attack-hint"), Color.FromHex((ReadOnlySpan<char>)"#FF5B4D", (Color?)null)));
			list.Add(new CivSquadRadialOption(CivSquadRadialAction.Defense, base.Loc.GetString("civ-ui-squad-defense"), base.Loc.GetString("civ-ui-squad-cmd-defense-desc", (ValueTuple<string, object>)("id", squadId)), base.Loc.GetString("civ-ui-squad-cmd-defense-hint"), Color.FromHex((ReadOnlySpan<char>)"#5B9DFF", (Color?)null)));
		}
		list.Add(new CivSquadRadialOption(CivSquadRadialAction.Artillery, base.Loc.GetString("civ-ui-squad-artillery"), base.Loc.GetString("civ-ui-squad-artillery-desc"), base.Loc.GetString("civ-ui-squad-artillery-hint"), Color.FromHex((ReadOnlySpan<char>)"#FFD454", (Color?)null)));
		list.Add(new CivSquadRadialOption(CivSquadRadialAction.Enemy, base.Loc.GetString("civ-ui-squad-enemy"), base.Loc.GetString("civ-ui-squad-cmd-enemy-desc"), base.Loc.GetString("civ-ui-squad-cmd-enemy-hint"), Color.FromHex((ReadOnlySpan<char>)"#FF7A45", (Color?)null)));
		list.Add(new CivSquadRadialOption(CivSquadRadialAction.Recon, base.Loc.GetString("civ-ui-squad-recon"), base.Loc.GetString("civ-ui-squad-recon-desc"), base.Loc.GetString("civ-ui-squad-recon-hint"), Color.FromHex((ReadOnlySpan<char>)"#54C8C8", (Color?)null)));
		list.Add(new CivSquadRadialOption(CivSquadRadialAction.Command, base.Loc.GetString("civ-ui-squad-command"), base.Loc.GetString("civ-ui-squad-command-desc"), base.Loc.GetString("civ-ui-squad-command-hint"), Color.FromHex((ReadOnlySpan<char>)"#C88EFF", (Color?)null)));
		string title = (_globalMap.TryGetCommanderSelectedSquadId(out squadId) ? base.Loc.GetString("civ-ui-squad-title-command", (ValueTuple<string, object>)("id", squadId)) : base.Loc.GetString("civ-ui-squad-title-commander"));
		int squadId2;
		string description = (_globalMap.TryGetCommanderSelectedSquadId(out squadId2) ? base.Loc.GetString("civ-ui-squad-desc-command-selected") : base.Loc.GetString("civ-ui-squad-desc-command-none"));
		return new CivSquadRadialMenuState(teamId, title, description, list);
	}

	private void OnActionSelected(CivSquadRadialAction action)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		switch (action)
		{
		case CivSquadRadialAction.Command:
			_globalMap.OpenCommanderWindow();
			CloseMenu();
			return;
		case CivSquadRadialAction.ManageRoster:
			_roster.OpenWindow();
			CloseMenu();
			return;
		}
		if (_pendingCoordinates.MapId == MapId.Nullspace)
		{
			CloseMenu();
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent) && civTeamMemberComponent.IsCommander)
			{
				switch (action)
				{
				case CivSquadRadialAction.Enemy:
					_globalMap.RequestPlaceMarker(CivGlobalMapMarkerType.Enemy, _pendingCoordinates.MapId, _pendingCoordinates.Position);
					CloseMenu();
					return;
				case CivSquadRadialAction.Artillery:
					_globalMap.RequestCommanderCallArtillery(_pendingCoordinates.MapId, _pendingCoordinates.Position);
					CloseMenu();
					return;
				case CivSquadRadialAction.Recon:
					_globalMap.RequestCommanderRecon(_pendingCoordinates.Position);
					CloseMenu();
					return;
				}
				if (!TryGetCommanderOrderType(action, out var orderType) || !_globalMap.TryGetCommanderSelectedSquadId(out var squadId))
				{
					_globalMap.OpenCommanderWindow();
					CloseMenu();
				}
				else
				{
					_globalMap.RequestCommanderSetOrder(squadId, orderType, _pendingCoordinates.MapId, _pendingCoordinates.Position);
					CloseMenu();
				}
				return;
			}
		}
		_globalMap.RequestPlaceMarker(GetMarkerType(action), _pendingCoordinates.MapId, _pendingCoordinates.Position);
		CloseMenu();
	}

	private static CivGlobalMapMarkerType GetMarkerType(CivSquadRadialAction action)
	{
		return action switch
		{
			CivSquadRadialAction.Attack => CivGlobalMapMarkerType.Attack, 
			CivSquadRadialAction.Defense => CivGlobalMapMarkerType.Defense, 
			CivSquadRadialAction.Help => CivGlobalMapMarkerType.Help, 
			CivSquadRadialAction.Allies => CivGlobalMapMarkerType.Allies, 
			_ => CivGlobalMapMarkerType.Enemy, 
		};
	}

	private static bool TryGetCommanderOrderType(CivSquadRadialAction action, out CivCommanderOrderType orderType)
	{
		orderType = action switch
		{
			CivSquadRadialAction.Attack => CivCommanderOrderType.Attack, 
			CivSquadRadialAction.Defense => CivCommanderOrderType.Defense, 
			_ => CivCommanderOrderType.None, 
		};
		return orderType != CivCommanderOrderType.None;
	}

	private void HandleMenuClosed()
	{
		CloseMenu();
	}

	private void CloseMenu()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (_menu != null)
		{
			_menu.OnActionSelected -= OnActionSelected;
			((BaseWindow)_menu).OnClose -= HandleMenuClosed;
			((Control)_menu).Orphan();
			_menu = null;
			_pendingCoordinates = MapCoordinates.Nullspace;
		}
	}
}
