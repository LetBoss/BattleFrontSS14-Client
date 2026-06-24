using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.GlobalMap;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Client._CIV14merka.GlobalMap.UI;

public sealed class CivGlobalMapWindow : DefaultWindow
{
	private readonly CivGlobalMapSystem _system;

	private readonly CivGlobalMapCanvas _canvas;

	private readonly Label _roleLabel;

	private readonly Label _roundStatusLabel;

	private readonly Label _hintLabel;

	private readonly PanelContainer _commanderSidebar;

	private readonly Label _commanderSidebarSummaryLabel;

	private readonly BoxContainer _commanderSquadListContainer;

	private readonly Button _commanderSidebarAttackButton;

	private readonly Button _commanderSidebarDefenseButton;

	private readonly Button _commanderSidebarArtilleryOrderButton;

	private readonly Button _commanderSidebarClearOrderButton;

	private readonly Button _commanderSidebarRosterButton;

	private readonly PanelContainer _commanderPanel;

	private readonly Button _openCommanderButton;

	private readonly Label _commanderSummaryLabel;

	private readonly BoxContainer _commanderRosterContainer;

	private readonly OptionButton _commanderSquadSelector;

	private readonly Button _commanderAttackButton;

	private readonly Button _commanderDefenseButton;

	private readonly Button _commanderArtilleryOrderButton;

	private readonly Button _commanderClearOrderButton;

	private readonly OptionButton _commanderPlayerSelector;

	private readonly OptionButton _commanderDestinationSelector;

	private readonly Button _commanderMoveButton;

	private readonly Dictionary<CivGlobalMapMarkerType, Button> _markerButtons = new Dictionary<CivGlobalMapMarkerType, Button>();

	private readonly Button _removeModeButton;

	private CivGlobalMapMarkerType? _selectedMarkerType;

	private bool _isSquadLeader;

	private bool _isCommander;

	private CivCommanderState? _commanderState;

	private readonly Dictionary<int, int> _squadSelectorToSquadId = new Dictionary<int, int>();

	private readonly Dictionary<int, NetUserId> _playerSelectorToUserId = new Dictionary<int, NetUserId>();

	private readonly Dictionary<int, CommanderDestination> _destinationSelectorMap = new Dictionary<int, CommanderDestination>();

	private int? _pendingOrderSquadId;

	private CivCommanderOrderType? _pendingOrderType;

	public Control CommanderPanel => (Control)(object)_commanderPanel;

	public CivGlobalMapWindow(CivGlobalMapSystem system)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected O, but got Unknown
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Expected O, but got Unknown
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Expected O, but got Unknown
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Expected O, but got Unknown
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Expected O, but got Unknown
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Expected O, but got Unknown
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Expected O, but got Unknown
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Expected O, but got Unknown
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Expected O, but got Unknown
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Expected O, but got Unknown
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Expected O, but got Unknown
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Expected O, but got Unknown
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Expected O, but got Unknown
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Expected O, but got Unknown
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Expected O, but got Unknown
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Expected O, but got Unknown
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Expected O, but got Unknown
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Expected O, but got Unknown
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Expected O, but got Unknown
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Expected O, but got Unknown
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Expected O, but got Unknown
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Expected O, but got Unknown
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Expected O, but got Unknown
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Expected O, but got Unknown
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Expected O, but got Unknown
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Expected O, but got Unknown
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Expected O, but got Unknown
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Expected O, but got Unknown
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_0808: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Expected O, but got Unknown
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0840: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_0853: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Expected O, but got Unknown
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_0882: Expected O, but got Unknown
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Expected O, but got Unknown
		//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Expected O, but got Unknown
		//IL_0968: Unknown result type (might be due to invalid IL or missing references)
		//IL_096d: Unknown result type (might be due to invalid IL or missing references)
		//IL_097d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0984: Unknown result type (might be due to invalid IL or missing references)
		//IL_0990: Expected O, but got Unknown
		_system = system;
		((DefaultWindow)this).Title = Loc.GetString("civ-gmap-window-title");
		((Control)this).MinSize = new Vector2(900f, 700f);
		((Control)this).SetSize = new Vector2(1100f, 800f);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			Margin = new Thickness(8f),
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
		_roleLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-role-prefix"),
			HorizontalExpand = true
		};
		((Control)val).AddChild((Control)(object)_roleLabel);
		_roundStatusLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-round-status-prefix"),
			FontColorOverride = Color.White,
			HorizontalExpand = true
		};
		((Control)val).AddChild((Control)(object)_roundStatusLabel);
		_hintLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-hint-default"),
			FontColorOverride = Color.LightGray,
			HorizontalExpand = true
		};
		((Control)val).AddChild((Control)(object)_hintLabel);
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val).AddChild((Control)(object)val2);
		_commanderSidebar = new PanelContainer
		{
			MinWidth = 420f,
			HorizontalExpand = false,
			VerticalExpand = true,
			Visible = false
		};
		((Control)val2).AddChild((Control)(object)_commanderSidebar);
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			Margin = new Thickness(8f),
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)_commanderSidebar).AddChild((Control)(object)val3);
		((Control)val3).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-gmap-sidebar-squads"),
			StyleClasses = { "LabelHeading" }
		});
		_commanderSidebarSummaryLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-sidebar-no-data"),
			HorizontalExpand = true
		};
		((Control)val3).AddChild((Control)(object)_commanderSidebarSummaryLabel);
		((Control)_commanderSidebarSummaryLabel).Visible = false;
		GridContainer val4 = new GridContainer
		{
			Columns = 2,
			HorizontalExpand = true
		};
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val4).Visible = false;
		_commanderSidebarAttackButton = new Button
		{
			Text = Loc.GetString("civ-gmap-sidebar-order-attack"),
			HorizontalExpand = true
		};
		((BaseButton)_commanderSidebarAttackButton).OnPressed += delegate
		{
			BeginCommanderOrderPlacement(CivCommanderOrderType.Attack);
		};
		((Control)val4).AddChild((Control)(object)_commanderSidebarAttackButton);
		_commanderSidebarDefenseButton = new Button
		{
			Text = Loc.GetString("civ-gmap-sidebar-order-defense"),
			HorizontalExpand = true
		};
		((BaseButton)_commanderSidebarDefenseButton).OnPressed += delegate
		{
			BeginCommanderOrderPlacement(CivCommanderOrderType.Defense);
		};
		((Control)val4).AddChild((Control)(object)_commanderSidebarDefenseButton);
		_commanderSidebarArtilleryOrderButton = new Button
		{
			Text = Loc.GetString("civ-gmap-sidebar-order-artillery"),
			HorizontalExpand = true,
			Visible = false
		};
		((BaseButton)_commanderSidebarArtilleryOrderButton).OnPressed += delegate
		{
			BeginCommanderOrderPlacement(CivCommanderOrderType.Artillery);
		};
		((Control)val4).AddChild((Control)(object)_commanderSidebarArtilleryOrderButton);
		_commanderSidebarClearOrderButton = new Button
		{
			Text = Loc.GetString("civ-gmap-sidebar-order-clear"),
			HorizontalExpand = true
		};
		((BaseButton)_commanderSidebarClearOrderButton).OnPressed += delegate
		{
			ClearCommanderOrder();
		};
		((Control)val4).AddChild((Control)(object)_commanderSidebarClearOrderButton);
		_commanderSidebarRosterButton = new Button
		{
			Text = Loc.GetString("civ-gmap-sidebar-roster"),
			HorizontalExpand = true
		};
		((BaseButton)_commanderSidebarRosterButton).OnPressed += delegate
		{
			if (TryGetSelectedCommanderSquadId(out var squadId))
			{
				_system.OpenCommanderWindow(squadId);
			}
		};
		((Control)val3).AddChild((Control)(object)_commanderSidebarRosterButton);
		((Control)_commanderSidebarRosterButton).Visible = false;
		ScrollContainer val5 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val3).AddChild((Control)(object)val5);
		_commanderSquadListContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val5).AddChild((Control)(object)_commanderSquadListContainer);
		PanelContainer val6 = new PanelContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			SizeFlagsStretchRatio = 1.8f
		};
		((Control)val2).AddChild((Control)(object)val6);
		CivGlobalMapCanvas civGlobalMapCanvas = new CivGlobalMapCanvas(_system);
		((Control)civGlobalMapCanvas).HorizontalExpand = true;
		((Control)civGlobalMapCanvas).VerticalExpand = true;
		_canvas = civGlobalMapCanvas;
		((Control)val6).AddChild((Control)(object)_canvas);
		_canvas.CommanderOrderPlaced += OnCommanderOrderPlaced;
		_commanderPanel = new PanelContainer
		{
			MinWidth = 260f,
			HorizontalExpand = false,
			VerticalExpand = true,
			Visible = false
		};
		BoxContainer val7 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			Margin = new Thickness(8f),
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)_commanderPanel).AddChild((Control)(object)val7);
		((Control)val7).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-gmap-commander-title"),
			StyleClasses = { "LabelHeading" }
		});
		_commanderSummaryLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-commander-no-data"),
			HorizontalExpand = true
		};
		((Control)val7).AddChild((Control)(object)_commanderSummaryLabel);
		((Control)val7).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-gmap-commander-squad-control")
		});
		_commanderSquadSelector = new OptionButton
		{
			HorizontalExpand = true
		};
		_commanderSquadSelector.OnItemSelected += delegate(ItemSelectedEventArgs args)
		{
			_commanderSquadSelector.SelectId(args.Id);
			if (_squadSelectorToSquadId.TryGetValue(args.Id, out var value))
			{
				SelectCommanderSquad(value);
			}
		};
		((Control)val7).AddChild((Control)(object)_commanderSquadSelector);
		GridContainer val8 = new GridContainer
		{
			Columns = 2,
			HorizontalExpand = true
		};
		((Control)val7).AddChild((Control)(object)val8);
		_commanderAttackButton = new Button
		{
			Text = Loc.GetString("civ-gmap-commander-order-attack"),
			HorizontalExpand = true
		};
		((BaseButton)_commanderAttackButton).OnPressed += delegate
		{
			BeginCommanderOrderPlacement(CivCommanderOrderType.Attack);
		};
		((Control)val8).AddChild((Control)(object)_commanderAttackButton);
		_commanderDefenseButton = new Button
		{
			Text = Loc.GetString("civ-gmap-commander-order-defense"),
			HorizontalExpand = true
		};
		((BaseButton)_commanderDefenseButton).OnPressed += delegate
		{
			BeginCommanderOrderPlacement(CivCommanderOrderType.Defense);
		};
		((Control)val8).AddChild((Control)(object)_commanderDefenseButton);
		_commanderArtilleryOrderButton = new Button
		{
			Text = Loc.GetString("civ-gmap-commander-order-artillery"),
			HorizontalExpand = true,
			Visible = false
		};
		((BaseButton)_commanderArtilleryOrderButton).OnPressed += delegate
		{
			BeginCommanderOrderPlacement(CivCommanderOrderType.Artillery);
		};
		((Control)val8).AddChild((Control)(object)_commanderArtilleryOrderButton);
		_commanderClearOrderButton = new Button
		{
			Text = Loc.GetString("civ-gmap-commander-order-clear"),
			HorizontalExpand = true
		};
		((BaseButton)_commanderClearOrderButton).OnPressed += delegate
		{
			ClearCommanderOrder();
		};
		((Control)val8).AddChild((Control)(object)_commanderClearOrderButton);
		((Control)val7).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-gmap-commander-transfer-player")
		});
		_commanderPlayerSelector = new OptionButton
		{
			HorizontalExpand = true
		};
		_commanderPlayerSelector.OnItemSelected += delegate
		{
			RefreshCommanderControls();
		};
		((Control)val7).AddChild((Control)(object)_commanderPlayerSelector);
		_commanderDestinationSelector = new OptionButton
		{
			HorizontalExpand = true
		};
		_commanderDestinationSelector.OnItemSelected += delegate
		{
			RefreshCommanderControls();
		};
		((Control)val7).AddChild((Control)(object)_commanderDestinationSelector);
		_commanderMoveButton = new Button
		{
			Text = Loc.GetString("civ-gmap-commander-move"),
			HorizontalExpand = true
		};
		((BaseButton)_commanderMoveButton).OnPressed += delegate
		{
			ApplyCommanderMove();
		};
		((Control)val7).AddChild((Control)(object)_commanderMoveButton);
		ScrollContainer val9 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val7).AddChild((Control)(object)val9);
		_commanderRosterContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val9).AddChild((Control)(object)_commanderRosterContainer);
		BoxContainer val10 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		((Control)val).AddChild((Control)(object)val10);
		Button val11 = new Button
		{
			Text = Loc.GetString("civ-gmap-tool-center-self"),
			HorizontalExpand = true
		};
		((BaseButton)val11).OnPressed += delegate
		{
			_canvas.CenterOnSelf();
		};
		((Control)val10).AddChild((Control)(object)val11);
		_openCommanderButton = new Button
		{
			Text = Loc.GetString("civ-gmap-tool-commander"),
			HorizontalExpand = true,
			Visible = false
		};
		((BaseButton)_openCommanderButton).OnPressed += delegate
		{
			_system.OpenCommanderWindow();
		};
		((Control)val10).AddChild((Control)(object)_openCommanderButton);
		GridContainer val12 = new GridContainer
		{
			Columns = 3,
			HorizontalExpand = true
		};
		((Control)val).AddChild((Control)(object)val12);
		AddMarkerButton(val12, CivGlobalMapMarkerType.Attack, Loc.GetString("civ-gmap-marker-attack"));
		AddMarkerButton(val12, CivGlobalMapMarkerType.Defense, Loc.GetString("civ-gmap-marker-defense"));
		AddMarkerButton(val12, CivGlobalMapMarkerType.Enemy, Loc.GetString("civ-gmap-marker-enemy"));
		AddMarkerButton(val12, CivGlobalMapMarkerType.Help, Loc.GetString("civ-gmap-marker-help"));
		AddMarkerButton(val12, CivGlobalMapMarkerType.Allies, Loc.GetString("civ-gmap-marker-allies"));
		_removeModeButton = new Button
		{
			Text = Loc.GetString("civ-gmap-tool-remove-markers"),
			ToggleMode = true,
			HorizontalExpand = true
		};
		((BaseButton)_removeModeButton).OnToggled += OnRemoveModeToggled;
		((Control)val12).AddChild((Control)(object)_removeModeButton);
		UpdateCommanderPanel();
		UpdateCommanderSidebar();
	}

	public void UpdateState(MapId mapId, bool hasBounds, Vector2 boundsMin, Vector2 boundsMax, int teamId, int squadId, bool isSquadLeader, bool isCommander, string statusLabel, float roundTimeLeftSeconds, int team1AliveCount, int team2AliveCount, int team1Score, int team2Score, IReadOnlyList<CivGlobalMapMarkerState> markers, IReadOnlyList<CivGlobalMapPlayerState> players, IReadOnlyList<CivPointCapturePointState> points, IReadOnlyList<CivCommanderOrderState> orders, IReadOnlyList<CivGlobalMapDeathState> deaths, IReadOnlyList<CivFobMarkerState> fobs, CivCommanderState? commanderState)
	{
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		_isSquadLeader = isSquadLeader;
		_isCommander = isCommander;
		_commanderState = commanderState;
		string item = Loc.GetString((teamId == 2) ? "civ-team-short-rf" : "civ-team-short-usa");
		_roleLabel.Text = (isSquadLeader ? Loc.GetString("civ-gmap-role-squad-leader", new(string, object)[2]
		{
			("squad", squadId),
			("team", item)
		}) : Loc.GetString("civ-gmap-role-squad-member", new(string, object)[2]
		{
			("squad", squadId),
			("team", item)
		}));
		if (squadId == 0 && !isCommander)
		{
			_roleLabel.Text = Loc.GetString("civ-gmap-role-reserve", new(string, object)[1] { ("team", item) });
		}
		if (isCommander)
		{
			_roleLabel.Text = Loc.GetString("civ-gmap-role-commander", new(string, object)[1] { ("team", item) });
		}
		int num = ((teamId == 2) ? team2Score : team1Score);
		int num2 = ((teamId == 2) ? team1Score : team2Score);
		_roundStatusLabel.Text = Loc.GetString("civ-gmap-round-status", new(string, object)[6]
		{
			("status", statusLabel),
			("time", FormatTime(roundTimeLeftSeconds)),
			("usaAlive", team1AliveCount),
			("rfAlive", team2AliveCount),
			("ownScore", num),
			("enemyScore", num2)
		});
		_canvas.UpdateData(mapId, hasBounds, boundsMin, boundsMax, teamId, squadId, markers, players, points, orders, deaths, fobs, isCommander ? _system.GetCommanderSelectedSquadId() : ((int?)null));
		UpdateCommanderPanel();
		UpdateCommanderSidebar();
		UpdateButtonsAvailability();
		CivGlobalMapMarkerType? selectedMarkerType = _selectedMarkerType;
		if (selectedMarkerType.HasValue)
		{
			CivGlobalMapMarkerType valueOrDefault = selectedMarkerType.GetValueOrDefault();
			if (valueOrDefault.IsGlobal() && !_isSquadLeader && !_isCommander)
			{
				SelectMarkerType(null);
			}
		}
	}

	private void AddMarkerButton(GridContainer grid, CivGlobalMapMarkerType markerType, string text)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		Button val = new Button
		{
			Text = text,
			ToggleMode = true,
			HorizontalExpand = true
		};
		((BaseButton)val).OnToggled += delegate(ButtonToggledEventArgs args)
		{
			if (args.Pressed)
			{
				SelectMarkerType(markerType);
			}
			else if (_selectedMarkerType == markerType)
			{
				SelectMarkerType(null);
			}
		};
		_markerButtons[markerType] = val;
		((Control)grid).AddChild((Control)(object)val);
	}

	private void SelectMarkerType(CivGlobalMapMarkerType? markerType)
	{
		if (_isCommander && markerType.HasValue)
		{
			CivGlobalMapMarkerType valueOrDefault = markerType.GetValueOrDefault();
			if (TryGetSelectedCommanderSquadId(out var _) && TryGetCommanderOrderType(valueOrDefault, out var orderType))
			{
				BeginCommanderOrderPlacement(orderType);
				return;
			}
		}
		if (markerType.HasValue && _pendingOrderType.HasValue)
		{
			CancelCommanderOrderPlacement();
		}
		_selectedMarkerType = markerType;
		foreach (KeyValuePair<CivGlobalMapMarkerType, Button> markerButton in _markerButtons)
		{
			markerButton.Deconstruct(out var key, out var value);
			CivGlobalMapMarkerType civGlobalMapMarkerType = key;
			Button val = value;
			int num;
			if (markerType.HasValue)
			{
				CivGlobalMapMarkerType? civGlobalMapMarkerType2 = markerType;
				key = civGlobalMapMarkerType;
				num = ((civGlobalMapMarkerType2 == key) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			bool flag = (byte)num != 0;
			if (((BaseButton)val).Pressed != flag)
			{
				((BaseButton)val).Pressed = flag;
			}
		}
		if (markerType.HasValue && ((BaseButton)_removeModeButton).Pressed)
		{
			((BaseButton)_removeModeButton).Pressed = false;
		}
		_canvas.SelectedMarkerType = markerType;
		_canvas.RemoveMode = ((BaseButton)_removeModeButton).Pressed;
		UpdateHint();
	}

	private void OnRemoveModeToggled(ButtonToggledEventArgs args)
	{
		if (args.Pressed)
		{
			if (_pendingOrderType.HasValue)
			{
				CancelCommanderOrderPlacement();
			}
			SelectMarkerType(null);
		}
		_canvas.RemoveMode = args.Pressed;
		UpdateHint();
	}

	private void UpdateButtonsAvailability()
	{
		foreach (var (type, val2) in _markerButtons)
		{
			((BaseButton)val2).Disabled = type.IsGlobal() && !_isSquadLeader && !_isCommander;
		}
		((BaseButton)_removeModeButton).Disabled = !_isSquadLeader && !_isCommander;
		((Control)_openCommanderButton).Visible = false;
		((Control)_commanderSidebar).Visible = _isCommander;
	}

	private void UpdateHint()
	{
		if (_canvas.RemoveMode)
		{
			_hintLabel.Text = Loc.GetString("civ-gmap-hint-remove-mode");
			return;
		}
		int? pendingOrderSquadId = _pendingOrderSquadId;
		if (pendingOrderSquadId.HasValue)
		{
			int valueOrDefault = pendingOrderSquadId.GetValueOrDefault();
			CivCommanderOrderType? pendingOrderType = _pendingOrderType;
			if (pendingOrderType.HasValue)
			{
				CivCommanderOrderType valueOrDefault2 = pendingOrderType.GetValueOrDefault();
				_hintLabel.Text = Loc.GetString("civ-gmap-hint-pending-order", new(string, object)[2]
				{
					("order", GetCommanderOrderDisplayName(valueOrDefault2)),
					("squad", valueOrDefault)
				});
				return;
			}
		}
		if (!_selectedMarkerType.HasValue)
		{
			_hintLabel.Text = Loc.GetString("civ-gmap-hint-default");
			return;
		}
		_hintLabel.Text = Loc.GetString("civ-gmap-hint-place-marker", new(string, object)[1] { ("marker", _markerButtons[_selectedMarkerType.Value].Text ?? string.Empty) });
	}

	private void BeginCommanderOrderPlacement(CivCommanderOrderType orderType)
	{
		if (_isCommander && TryGetSelectedCommanderSquadId(out var squadId))
		{
			_pendingOrderSquadId = squadId;
			_pendingOrderType = orderType;
			_canvas.PendingCommanderOrderSquadId = squadId;
			_canvas.PendingCommanderOrderType = orderType;
			SelectMarkerType(null);
			if (((BaseButton)_removeModeButton).Pressed)
			{
				((BaseButton)_removeModeButton).Pressed = false;
			}
			UpdateCommanderPanel();
			UpdateCommanderSidebar();
			UpdateHint();
		}
	}

	private void ClearCommanderOrder()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_isCommander && TryGetSelectedCommanderSquadId(out var squadId))
		{
			_system.RequestCommanderSetOrder(squadId, CivCommanderOrderType.None, MapId.Nullspace, Vector2.Zero);
			CancelCommanderOrderPlacement();
		}
	}

	private void ApplyCommanderMove()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (_isCommander && TryGetSelectedCommanderPlayerId(out var userId) && _destinationSelectorMap.TryGetValue(_commanderDestinationSelector.SelectedId, out var value))
		{
			_system.RequestCommanderMovePlayer(userId, value.SquadId, value.CreateNewSquad);
		}
	}

	private void CancelCommanderOrderPlacement()
	{
		_pendingOrderSquadId = null;
		_pendingOrderType = null;
		_canvas.PendingCommanderOrderSquadId = null;
		_canvas.PendingCommanderOrderType = null;
		UpdateCommanderPanel();
		UpdateCommanderSidebar();
		UpdateHint();
	}

	private void OnCommanderOrderPlaced()
	{
		CancelCommanderOrderPlacement();
	}

	private void UpdateCommanderPanel()
	{
		((Control)_commanderPanel).Visible = _isCommander;
		if (!_isCommander)
		{
			_pendingOrderSquadId = null;
			_pendingOrderType = null;
			_canvas.PendingCommanderOrderSquadId = null;
			_canvas.PendingCommanderOrderType = null;
		}
		else
		{
			RebuildCommanderSelectors();
			RebuildCommanderRosterList();
			RefreshCommanderControls();
		}
	}

	private void UpdateCommanderSidebar()
	{
		((Control)_commanderSidebar).Visible = _isCommander;
		if (!_isCommander)
		{
			_commanderSidebarSummaryLabel.Text = Loc.GetString("civ-gmap-sidebar-no-data");
			((Control)_commanderSquadListContainer).DisposeAllChildren();
		}
		else
		{
			RebuildCommanderSquadList();
			RefreshCommanderSidebarControls();
		}
	}

	private void RefreshCommanderSidebarControls()
	{
		CivCommanderSquadState squad;
		bool flag = TryGetSelectedCommanderSquad(out squad);
		((BaseButton)_commanderSidebarAttackButton).Disabled = !flag;
		((BaseButton)_commanderSidebarDefenseButton).Disabled = !flag;
		((BaseButton)_commanderSidebarArtilleryOrderButton).Disabled = !flag;
		((BaseButton)_commanderSidebarClearOrderButton).Disabled = !flag;
		((BaseButton)_commanderSidebarRosterButton).Disabled = !flag;
		_commanderSidebarSummaryLabel.Text = BuildCommanderSummary();
	}

	private void RebuildCommanderSquadList()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Expected O, but got Unknown
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		((Control)_commanderSquadListContainer).DisposeAllChildren();
		if (_commanderState == null)
		{
			return;
		}
		int? commanderSelectedSquadId = _system.GetCommanderSelectedSquadId();
		Label val6;
		foreach (CivCommanderSquadState squad in _commanderState.Squads.OrderBy((CivCommanderSquadState entry) => entry.SquadId))
		{
			BoxContainer val = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 4,
				HorizontalExpand = true
			};
			BoxContainer val2 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 4,
				HorizontalExpand = false
			};
			Button val3 = new Button();
			val3.Text = Loc.GetString("civ-gmap-squad-button", new(string, object)[1] { ("squad", squad.SquadId) });
			((BaseButton)val3).ToggleMode = true;
			((BaseButton)val3).Pressed = commanderSelectedSquadId == squad.SquadId;
			((Control)val3).HorizontalExpand = true;
			Button val4 = val3;
			((BaseButton)val4).OnPressed += delegate
			{
				SelectCommanderSquad(squad.SquadId);
			};
			((Control)val2).AddChild((Control)(object)val4);
			Button val5 = new Button
			{
				Text = Loc.GetString("civ-gmap-sidebar-roster")
			};
			((BaseButton)val5).OnPressed += delegate
			{
				_system.OpenCommanderWindow(squad.SquadId);
			};
			((Control)val2).AddChild((Control)(object)val5);
			((Control)val).AddChild((Control)(object)val2);
			val6 = new Label();
			val6.Text = Loc.GetString("civ-gmap-squad-info", new(string, object)[3]
			{
				("leader", squad.LeaderName),
				("members", squad.Members.Count),
				("order", GetCommanderOrderDisplayName(squad.Order))
			});
			((Control)val6).HorizontalExpand = true;
			((Control)val).AddChild((Control)(object)val6);
			((Control)_commanderSquadListContainer).AddChild((Control)(object)val);
		}
		BoxContainer commanderSquadListContainer = _commanderSquadListContainer;
		val6 = new Label();
		val6.Text = Loc.GetString("civ-gmap-reserve-count", new(string, object)[1] { ("count", _commanderState.ReservePlayers.Count) });
		val6.FontColorOverride = Color.LightGray;
		((Control)val6).HorizontalExpand = true;
		((Control)commanderSquadListContainer).AddChild((Control)(object)val6);
	}

	private void SelectCommanderSquad(int squadId)
	{
		if (_isCommander)
		{
			_system.SetCommanderSelectedSquad(squadId);
			_canvas.CommanderSelectedSquadId = squadId;
			UpdateCommanderPanel();
			UpdateCommanderSidebar();
		}
	}

	private void SelectCommanderPlayer(NetUserId userId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		int key = _playerSelectorToUserId.FirstOrDefault((KeyValuePair<int, NetUserId> pair) => pair.Value == userId).Key;
		if (key != 0)
		{
			_commanderPlayerSelector.SelectId(key);
			RefreshCommanderControls();
		}
	}

	private string BuildCommanderSummary()
	{
		if (_commanderState == null)
		{
			return Loc.GetString("civ-gmap-sidebar-no-snapshot");
		}
		string text = Loc.GetString("civ-gmap-sidebar-summary", new(string, object)[3]
		{
			("team", _commanderState.TeamId),
			("squads", _commanderState.Squads.Count),
			("reserve", _commanderState.ReservePlayers.Count)
		});
		if (TryGetSelectedCommanderSquad(out CivCommanderSquadState squad))
		{
			text = text + "\n" + Loc.GetString("civ-gmap-sidebar-summary-squad", new(string, object)[2]
			{
				("squad", squad.SquadId),
				("order", GetCommanderOrderDisplayName(squad.Order))
			});
		}
		int? pendingOrderSquadId = _pendingOrderSquadId;
		if (pendingOrderSquadId.HasValue)
		{
			int valueOrDefault = pendingOrderSquadId.GetValueOrDefault();
			CivCommanderOrderType? pendingOrderType = _pendingOrderType;
			if (pendingOrderType.HasValue)
			{
				CivCommanderOrderType valueOrDefault2 = pendingOrderType.GetValueOrDefault();
				text = text + "\n" + Loc.GetString("civ-gmap-sidebar-summary-pending", new(string, object)[2]
				{
					("order", GetCommanderOrderDisplayName(valueOrDefault2)),
					("squad", valueOrDefault)
				});
			}
		}
		return text;
	}

	private bool TryGetSelectedCommanderSquad([NotNullWhen(true)] out CivCommanderSquadState? squad)
	{
		squad = null;
		if (_commanderState == null || !TryGetSelectedCommanderSquadId(out var squadId))
		{
			return false;
		}
		squad = _commanderState.Squads.FirstOrDefault((CivCommanderSquadState entry) => entry.SquadId == squadId);
		return squad != null;
	}

	private void RefreshCommanderControls()
	{
		int squadId;
		bool flag = TryGetSelectedCommanderSquadId(out squadId);
		NetUserId userId;
		bool flag2 = TryGetSelectedCommanderPlayerId(out userId);
		bool flag3 = _destinationSelectorMap.ContainsKey(_commanderDestinationSelector.SelectedId);
		((BaseButton)_commanderAttackButton).Disabled = !flag;
		((BaseButton)_commanderDefenseButton).Disabled = !flag;
		((BaseButton)_commanderArtilleryOrderButton).Disabled = !flag;
		((BaseButton)_commanderClearOrderButton).Disabled = !flag;
		((BaseButton)_commanderMoveButton).Disabled = !flag2 || !flag3;
		if (_commanderState == null)
		{
			_commanderSummaryLabel.Text = Loc.GetString("civ-gmap-sidebar-no-snapshot");
			return;
		}
		string text = Loc.GetString("civ-gmap-sidebar-summary", new(string, object)[3]
		{
			("team", _commanderState.TeamId),
			("squads", _commanderState.Squads.Count),
			("reserve", _commanderState.ReservePlayers.Count)
		});
		if (flag)
		{
			CivCommanderSquadState civCommanderSquadState = _commanderState.Squads.FirstOrDefault((CivCommanderSquadState entry) => entry.SquadId == squadId);
			if (civCommanderSquadState != null)
			{
				text = text + "\n" + Loc.GetString("civ-gmap-sidebar-summary-squad", new(string, object)[2]
				{
					("squad", civCommanderSquadState.SquadId),
					("order", GetCommanderOrderDisplayName(civCommanderSquadState.Order))
				});
			}
		}
		int? pendingOrderSquadId = _pendingOrderSquadId;
		if (pendingOrderSquadId.HasValue)
		{
			int valueOrDefault = pendingOrderSquadId.GetValueOrDefault();
			CivCommanderOrderType? pendingOrderType = _pendingOrderType;
			if (pendingOrderType.HasValue)
			{
				CivCommanderOrderType valueOrDefault2 = pendingOrderType.GetValueOrDefault();
				text = text + "\n" + Loc.GetString("civ-gmap-sidebar-summary-pending", new(string, object)[2]
				{
					("order", GetCommanderOrderDisplayName(valueOrDefault2)),
					("squad", valueOrDefault)
				});
			}
		}
		_commanderSummaryLabel.Text = text;
	}

	private void RebuildCommanderSelectors()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		int? num = _system.GetCommanderSelectedSquadId() ?? _pendingOrderSquadId;
		NetUserId value;
		NetUserId? selectedPlayerId = (_playerSelectorToUserId.TryGetValue(_commanderPlayerSelector.SelectedId, out value) ? new NetUserId?(value) : ((NetUserId?)null));
		int selectedId = _commanderDestinationSelector.SelectedId;
		_squadSelectorToSquadId.Clear();
		_commanderSquadSelector.Clear();
		_playerSelectorToUserId.Clear();
		_commanderPlayerSelector.Clear();
		_destinationSelectorMap.Clear();
		_commanderDestinationSelector.Clear();
		if (_commanderState == null)
		{
			return;
		}
		foreach (CivCommanderSquadState item2 in _commanderState.Squads.OrderBy((CivCommanderSquadState entry) => entry.SquadId))
		{
			_squadSelectorToSquadId[item2.SquadId] = item2.SquadId;
			_commanderSquadSelector.AddItem(Loc.GetString("civ-gmap-squad-button", new(string, object)[1] { ("squad", item2.SquadId) }), (int?)item2.SquadId);
			_destinationSelectorMap[item2.SquadId] = new CommanderDestination(item2.SquadId, CreateNewSquad: false);
			_commanderDestinationSelector.AddItem(Loc.GetString("civ-gmap-commander-destination-squad", new(string, object)[1] { ("squad", item2.SquadId) }), (int?)item2.SquadId);
		}
		int num2;
		if (_commanderState.Squads.Count > 0)
		{
			if (num.HasValue)
			{
				int valueOrDefault = num.GetValueOrDefault();
				if (_squadSelectorToSquadId.ContainsKey(valueOrDefault))
				{
					num2 = valueOrDefault;
					goto IL_021c;
				}
			}
			num2 = _commanderState.Squads[0].SquadId;
			goto IL_021c;
		}
		goto IL_024a;
		IL_021c:
		int num3 = num2;
		_canvas.CommanderSelectedSquadId = num3;
		_system.SetCommanderSelectedSquad(num3);
		_commanderSquadSelector.SelectId(num3);
		goto IL_024a;
		IL_024a:
		_destinationSelectorMap[-1] = new CommanderDestination(0, CreateNewSquad: false);
		_destinationSelectorMap[-2] = new CommanderDestination(0, CreateNewSquad: true);
		_commanderDestinationSelector.AddItem(Loc.GetString("civ-gmap-commander-destination-reserve"), (int?)(-1));
		_commanderDestinationSelector.AddItem(Loc.GetString("civ-gmap-commander-destination-new-squad"), (int?)(-2));
		List<CivCommanderPlayerState> list = (from player in _commanderState.Squads.SelectMany((CivCommanderSquadState entry) => entry.Members).Concat(_commanderState.ReservePlayers)
			where !player.IsCommander
			orderby player.Name
			select player).ToList();
		foreach (CivCommanderPlayerState item3 in list)
		{
			int num4 = _playerSelectorToUserId.Count + 1;
			_playerSelectorToUserId[num4] = item3.UserId;
			string item = ((item3.SquadId == 0) ? Loc.GetString("civ-gmap-commander-player-squad-reserve") : Loc.GetString("civ-gmap-commander-player-squad-num", new(string, object)[1] { ("squad", item3.SquadId) }));
			_commanderPlayerSelector.AddItem(Loc.GetString("civ-gmap-commander-player-option", new(string, object)[2]
			{
				("name", item3.Name),
				("squad", item)
			}), (int?)num4);
		}
		if (list.Count > 0)
		{
			int key = _playerSelectorToUserId.FirstOrDefault(delegate(KeyValuePair<int, NetUserId> pair)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				NetUserId value2 = pair.Value;
				NetUserId? val = selectedPlayerId;
				return val.HasValue && value2 == val.GetValueOrDefault();
			}).Key;
			_commanderPlayerSelector.SelectId((key != 0) ? key : _playerSelectorToUserId.Keys.Min());
		}
		if (_destinationSelectorMap.Count > 0)
		{
			int num5 = (_destinationSelectorMap.ContainsKey(selectedId) ? selectedId : (-1));
			_commanderDestinationSelector.SelectId(num5);
		}
	}

	private void RebuildCommanderRosterList()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected O, but got Unknown
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Expected O, but got Unknown
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Expected O, but got Unknown
		((Control)_commanderRosterContainer).DisposeAllChildren();
		if (_commanderState == null)
		{
			return;
		}
		foreach (CivCommanderSquadState item2 in _commanderState.Squads.OrderBy((CivCommanderSquadState entry) => entry.SquadId))
		{
			string commanderOrderDisplayName = GetCommanderOrderDisplayName(item2.Order);
			BoxContainer commanderRosterContainer = _commanderRosterContainer;
			Label val = new Label();
			val.Text = Loc.GetString("civ-gmap-roster-squad", new(string, object)[3]
			{
				("squad", item2.SquadId),
				("leader", item2.LeaderName),
				("order", commanderOrderDisplayName)
			});
			((Control)val).HorizontalExpand = true;
			((Control)commanderRosterContainer).AddChild((Control)(object)val);
			foreach (CivCommanderPlayerState item3 in from player in item2.Members
				orderby player.IsSquadLeader descending, player.Name
				select player)
			{
				string item = (item3.IsSquadLeader ? Loc.GetString("civ-gmap-roster-leader-mark") : string.Empty);
				BoxContainer commanderRosterContainer2 = _commanderRosterContainer;
				val = new Label();
				val.Text = Loc.GetString("civ-gmap-roster-member", new(string, object)[2]
				{
					("mark", item),
					("name", item3.Name)
				});
				((Control)val).HorizontalExpand = true;
				((Control)commanderRosterContainer2).AddChild((Control)(object)val);
			}
		}
		((Control)_commanderRosterContainer).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-gmap-roster-reserve"),
			StyleClasses = { "LabelHeading" }
		});
		if (_commanderState.ReservePlayers.Count == 0)
		{
			((Control)_commanderRosterContainer).AddChild((Control)new Label
			{
				Text = Loc.GetString("civ-gmap-roster-reserve-empty")
			});
			return;
		}
		foreach (CivCommanderPlayerState item4 in _commanderState.ReservePlayers.OrderBy((CivCommanderPlayerState player) => player.Name))
		{
			BoxContainer commanderRosterContainer3 = _commanderRosterContainer;
			Label val = new Label();
			val.Text = Loc.GetString("civ-gmap-roster-reserve-member", new(string, object)[1] { ("name", item4.Name) });
			((Control)val).HorizontalExpand = true;
			((Control)commanderRosterContainer3).AddChild((Control)(object)val);
		}
	}

	private bool TryGetSelectedCommanderSquadId(out int squadId)
	{
		return _squadSelectorToSquadId.TryGetValue(_commanderSquadSelector.SelectedId, out squadId);
	}

	private bool TryGetSelectedCommanderPlayerId(out NetUserId userId)
	{
		return _playerSelectorToUserId.TryGetValue(_commanderPlayerSelector.SelectedId, out userId);
	}

	private static string GetCommanderOrderDisplayName(CivCommanderOrderType orderType)
	{
		return orderType switch
		{
			CivCommanderOrderType.Attack => Loc.GetString("civ-gmap-order-attack"), 
			CivCommanderOrderType.Defense => Loc.GetString("civ-gmap-order-defense"), 
			CivCommanderOrderType.Artillery => Loc.GetString("civ-gmap-order-artillery"), 
			_ => Loc.GetString("civ-gmap-order-none"), 
		};
	}

	private static bool TryGetCommanderOrderType(CivGlobalMapMarkerType markerType, out CivCommanderOrderType orderType)
	{
		orderType = markerType switch
		{
			CivGlobalMapMarkerType.Attack => CivCommanderOrderType.Attack, 
			CivGlobalMapMarkerType.Defense => CivCommanderOrderType.Defense, 
			_ => CivCommanderOrderType.None, 
		};
		return orderType != CivCommanderOrderType.None;
	}

	private static string FormatTime(float totalSeconds)
	{
		if (!float.IsFinite(totalSeconds) || totalSeconds <= 0f)
		{
			return "00:00";
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
		if (!(timeSpan.TotalHours >= 1.0))
		{
			return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		}
		return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
	}
}
