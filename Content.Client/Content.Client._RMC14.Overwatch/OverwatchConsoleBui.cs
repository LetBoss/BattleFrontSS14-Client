using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Content.Client._RMC14.UserInterface;
using Content.Client.Message;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Maths;
using Content.Shared._RMC14.Overwatch;
using Content.Shared._RMC14.SupplyDrop;
using Content.Shared.Mobs;
using Content.Shared.Roles;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Overwatch;

public sealed class OverwatchConsoleBui : RMCPopOutBui<OverwatchConsoleWindow>
{
	private readonly record struct OverwatchRow(ProtoId<JobPrototype>? RoleId, (PanelContainer Panel, Button Button, RichTextLabel Label) Name, (PanelContainer Panel, Label Label) Role, (PanelContainer Panel, RichTextLabel Label) State, (PanelContainer Panel, RichTextLabel Label) Location, (PanelContainer Panel, Label Label) Distance, (BoxContainer Container, Button Hide, Button Promote) Buttons);

	[Dependency]
	private ILocalizationManager _localization;

	[Dependency]
	private IPrototypeManager _prototypes;

	private const string GreenColor = "#229132";

	private const string RedColor = "#A42625";

	private const string YellowColor = "#CED22B";

	private readonly OverwatchConsoleSystem _overwatchConsole;

	private readonly SquadSystem _squad;

	private readonly Dictionary<NetEntity, OverwatchSquadView> _squadViews = new Dictionary<NetEntity, OverwatchSquadView>();

	private readonly Dictionary<NetEntity, PanelContainer> _squads = new Dictionary<NetEntity, PanelContainer>();

	private readonly Dictionary<NetEntity, Dictionary<NetEntity, OverwatchRow>> _rows = new Dictionary<NetEntity, Dictionary<NetEntity, OverwatchRow>>();

	protected override OverwatchConsoleWindow? Window { get; set; }

	public OverwatchConsoleBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		_overwatchConsole = ((BoundUserInterface)this).EntMan.System<OverwatchConsoleSystem>();
		_squad = ((BoundUserInterface)this).EntMan.System<SquadSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		if (Window == null)
		{
			Window = ((BoundUserInterface)(object)this).CreatePopOutableWindow<OverwatchConsoleWindow>();
			Window.OverwatchHeader.SetMarkupPermissive("[color=#88C7FA]OVERWATCH DISABLED - SELECT SQUAD[/color]");
			if (((BoundUserInterface)this).State is OverwatchConsoleBuiState s)
			{
				RefreshState(s);
			}
			UpdateView();
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is OverwatchConsoleBuiState s)
		{
			RefreshState(s);
		}
	}

	private void RefreshState(OverwatchConsoleBuiState s)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
		//IL_168f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1691: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da0: Expected O, but got Unknown
		//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbb: Expected O, but got Unknown
		//IL_0ddd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e31: Expected O, but got Unknown
		//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e60: Expected O, but got Unknown
		//IL_0eaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec8: Expected O, but got Unknown
		//IL_0f00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0e: Expected O, but got Unknown
		//IL_0f2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9d: Expected O, but got Unknown
		//IL_0f9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
		//IL_100b: Expected O, but got Unknown
		//IL_104c: Unknown result type (might be due to invalid IL or missing references)
		//IL_105b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1060: Unknown result type (might be due to invalid IL or missing references)
		//IL_1069: Expected O, but got Unknown
		//IL_108b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1111: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_096d: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1796: Unknown result type (might be due to invalid IL or missing references)
		//IL_179b: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b7: Expected O, but got Unknown
		//IL_15c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1472: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd6: Expected O, but got Unknown
		//IL_1c19: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c31: Expected O, but got Unknown
		//IL_1c40: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c52: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c63: Expected O, but got Unknown
		//IL_1c63: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c70: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c76: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c87: Expected O, but got Unknown
		//IL_1c87: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c88: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c93: Expected O, but got Unknown
		//IL_1c93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c9a: Expected O, but got Unknown
		//IL_1ce0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cec: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d06: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d18: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d29: Expected O, but got Unknown
		//IL_1d29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d36: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d41: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d4d: Expected O, but got Unknown
		//IL_1d52: Expected O, but got Unknown
		//IL_1d52: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d58: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d7b: Expected O, but got Unknown
		//IL_1d7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d88: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d9f: Expected O, but got Unknown
		//IL_1da4: Expected O, but got Unknown
		//IL_1da9: Expected O, but got Unknown
		//IL_1604: Unknown result type (might be due to invalid IL or missing references)
		//IL_1609: Unknown result type (might be due to invalid IL or missing references)
		//IL_151d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1521: Unknown result type (might be due to invalid IL or missing references)
		//IL_1619: Unknown result type (might be due to invalid IL or missing references)
		//IL_161e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1563: Unknown result type (might be due to invalid IL or missing references)
		//IL_1974: Unknown result type (might be due to invalid IL or missing references)
		//IL_197b: Expected O, but got Unknown
		//IL_1985: Unknown result type (might be due to invalid IL or missing references)
		//IL_198c: Expected O, but got Unknown
		//IL_19b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_19c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_19c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_19db: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e7: Expected O, but got Unknown
		//IL_1a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a19: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2a: Expected O, but got Unknown
		//IL_1a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a42: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4e: Expected O, but got Unknown
		//IL_1a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5a: Expected O, but got Unknown
		//IL_1a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a80: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a87: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aad: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac4: Expected O, but got Unknown
		//IL_1ac4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae8: Expected O, but got Unknown
		//IL_1aed: Expected O, but got Unknown
		//IL_1aed: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b05: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b16: Expected O, but got Unknown
		//IL_1b16: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b23: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3a: Expected O, but got Unknown
		//IL_1b3f: Expected O, but got Unknown
		//IL_1b44: Expected O, but got Unknown
		OverwatchConsoleComponent console = default(OverwatchConsoleComponent);
		if (Window == null || !((BoundUserInterface)this).EntMan.TryGetComponent<OverwatchConsoleComponent>(((BoundUserInterface)this).Owner, ref console))
		{
			return;
		}
		List<OverwatchSquad> list = s.Squads.ToList();
		list.Sort((OverwatchSquad a, OverwatchSquad b) => string.CompareOrdinal(a.Name, b.Name));
		NetEntity key;
		foreach (KeyValuePair<NetEntity, PanelContainer> squad3 in _squads)
		{
			squad3.Deconstruct(out key, out var value);
			NetEntity id = key;
			PanelContainer val = value;
			if (list.All((OverwatchSquad oldSquad) => oldSquad.Id != id))
			{
				((Control)val).Orphan();
			}
		}
		foreach (OverwatchSquad squad in list)
		{
			if (!_squads.ContainsKey(squad.Id))
			{
				Button val2 = new Button
				{
					Text = squad.Name.ToUpper(),
					ModulateSelfOverride = squad.Color,
					StyleClasses = { "OpenBoth" }
				};
				((BaseButton)val2).OnPressed += delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleSelectSquadBuiMsg(squad.Id));
				};
				PanelContainer val3 = CreatePanel();
				((Control)val3).AddChild((Control)(object)val2);
				((Control)Window.SquadsContainer).AddChild((Control)(object)val3);
				_squads[squad.Id] = val3;
			}
		}
		Dictionary<ProtoId<JobPrototype>, int> roleSorting = new Dictionary<ProtoId<JobPrototype>, int>();
		NetEntity? activeSquad = GetActiveSquad();
		Thickness margin = default(Thickness);
		((Thickness)(ref margin))._002Ector(2f);
		SupplyDropComputerComponent supplyDropComputerComponent = default(SupplyDropComputerComponent);
		OverwatchConsoleComponent overwatchConsoleComponent = default(OverwatchConsoleComponent);
		string text3 = default(string);
		JobPrototype jobPrototype = default(JobPrototype);
		RankPrototype rankPrototype = default(RankPrototype);
		KeyValuePair<NetEntity, OverwatchRow>? keyValuePair2 = default(KeyValuePair<NetEntity, OverwatchRow>?);
		bool flag2 = default(bool);
		JobPrototype jobPrototype2 = default(JobPrototype);
		JobPrototype jobPrototype3 = default(JobPrototype);
		OverwatchMarine? overwatchMarine = default(OverwatchMarine?);
		OverwatchMarine? overwatchMarine2 = default(OverwatchMarine?);
		foreach (OverwatchSquad squad4 in s.Squads)
		{
			OverwatchSquad squad2 = squad4;
			if (!s.Marines.TryGetValue(squad2.Id, out List<OverwatchMarine> marines))
			{
				continue;
			}
			marines.Sort((OverwatchMarine a, OverwatchMarine b) => Sorting(a).CompareTo(Sorting(b)));
			if (_squadViews.TryGetValue(squad2.Id, out OverwatchSquadView monitor))
			{
				((Control)monitor.RolesContainer).DisposeAllChildren();
			}
			else
			{
				monitor = new OverwatchSquadView();
				OverwatchSquadView overwatchSquadView = monitor;
				key = squad2.Id;
				NetEntity? val4 = activeSquad;
				((Control)overwatchSquadView).Visible = val4.HasValue && key == val4.GetValueOrDefault();
				((BaseButton)monitor.TacticalMapButton).OnPressed += delegate
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchViewTacticalMapBuiMsg());
				};
				((BaseButton)monitor.OperatorButton).OnPressed += delegate
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleTakeOperatorBuiMsg());
				};
				monitor.SearchBar.OnTextChanged += delegate
				{
					monitor.UpdateResults(console.Location, console.ShowDead, console.ShowHidden, marines, console);
				};
				((Control)monitor.ShowLocationButton.Label).ModulateSelfOverride = Color.Black;
				((BaseButton)monitor.ShowLocationButton).OnPressed += delegate
				{
					OverwatchLocation? overwatchLocation = ((!console.Location.HasValue) ? new OverwatchLocation?(OverwatchLocation.Min) : (console.Location + 1));
					if (overwatchLocation > OverwatchLocation.Ship)
					{
						overwatchLocation = null;
					}
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleSetLocationBuiMsg(overwatchLocation));
				};
				((Control)monitor.ShowDeadButton.Label).ModulateSelfOverride = Color.Black;
				((BaseButton)monitor.ShowDeadButton).OnPressed += delegate
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleShowDeadBuiMsg(!console.ShowDead));
				};
				((Control)monitor.ShowHiddenButton.Label).ModulateSelfOverride = Color.Black;
				((BaseButton)monitor.ShowHiddenButton).OnPressed += delegate
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleShowHiddenBuiMsg(!console.ShowHidden));
				};
				((Control)monitor.TransferMarineButton.Label).ModulateSelfOverride = Color.Black;
				((BaseButton)monitor.TransferMarineButton).OnPressed += delegate
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleTransferMarineBuiMsg());
				};
				if (((BoundUserInterface)this).EntMan.TryGetComponent<SupplyDropComputerComponent>(((BoundUserInterface)this).Owner, ref supplyDropComputerComponent))
				{
					monitor.Longitude.Value = supplyDropComputerComponent.Coordinates.X;
					monitor.Latitude.Value = supplyDropComputerComponent.Coordinates.Y;
				}
				monitor.Longitude.OnValueChanged += delegate(FloatSpinBoxEventArgs args)
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleSupplyDropLongitudeBuiMsg((int)args.Value));
				};
				monitor.Latitude.OnValueChanged += delegate(FloatSpinBoxEventArgs args)
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleSupplyDropLatitudeBuiMsg((int)args.Value));
				};
				((BaseButton)monitor.LaunchButton).OnPressed += delegate
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleSupplyDropLaunchBuiMsg());
				};
				((BaseButton)monitor.SaveButton).OnPressed += delegate
				{
					int longitude = (int)monitor.Longitude.Value;
					int latitude = (int)monitor.Latitude.Value;
					OverwatchConsoleSupplyDropSaveBuiMsg overwatchConsoleSupplyDropSaveBuiMsg = new OverwatchConsoleSupplyDropSaveBuiMsg(longitude, latitude);
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)overwatchConsoleSupplyDropSaveBuiMsg);
				};
				monitor.OrbitalLongitude.Value = console.OrbitalCoordinates.X;
				monitor.OrbitalLatitude.Value = console.OrbitalCoordinates.Y;
				monitor.OrbitalLongitude.OnValueChanged += delegate(FloatSpinBoxEventArgs args)
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleOrbitalLongitudeBuiMsg((int)args.Value));
				};
				monitor.OrbitalLatitude.OnValueChanged += delegate(FloatSpinBoxEventArgs args)
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleOrbitalLatitudeBuiMsg((int)args.Value));
				};
				((BaseButton)monitor.OrbitalFireButton).OnPressed += delegate
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleOrbitalLaunchBuiMsg());
				};
				((BaseButton)monitor.OrbitalSaveButton).OnPressed += delegate
				{
					int longitude = (int)monitor.Longitude.Value;
					int latitude = (int)monitor.Latitude.Value;
					OverwatchConsoleOrbitalSaveBuiMsg overwatchConsoleOrbitalSaveBuiMsg = new OverwatchConsoleOrbitalSaveBuiMsg(longitude, latitude);
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)overwatchConsoleOrbitalSaveBuiMsg);
				};
				((BaseButton)monitor.MessageSquadButton).OnPressed += delegate
				{
					OverwatchTextInputWindow window = new OverwatchTextInputWindow();
					window.MessageBox.OnTextEntered += delegate
					{
						SendSquadMessage();
					};
					((BaseButton)window.OkButton).OnPressed += delegate
					{
						SendSquadMessage();
					};
					((BaseButton)window.CancelButton).OnPressed += delegate
					{
						((BaseWindow)window).Close();
					};
					((BaseWindow)window).OpenCentered();
					void SendSquadMessage()
					{
						((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleSendMessageBuiMsg(window.MessageBox.Text));
						((BaseWindow)window).Close();
					}
				};
				bool flag = ((BoundUserInterface)this).EntMan.HasComponent<SupplyDropComputerComponent>(((BoundUserInterface)this).Owner) && squad2.CanSupplyDrop;
				TabContainer.SetTabVisible((Control)(object)monitor.SupplyDrop, flag);
				if (((BoundUserInterface)this).EntMan.TryGetComponent<OverwatchConsoleComponent>(((BoundUserInterface)this).Owner, ref overwatchConsoleComponent))
				{
					TabContainer.SetTabVisible((Control)(object)monitor.OrbitalBombardment, overwatchConsoleComponent.CanOrbitalBombardment);
					((Control)monitor.MessageSquadButton).Visible = overwatchConsoleComponent.CanMessageSquad;
				}
				else
				{
					TabContainer.SetTabVisible((Control)(object)monitor.OrbitalBombardment, false);
					((Control)monitor.MessageSquadButton).Visible = false;
				}
				_squadViews[squad2.Id] = monitor;
				((Control)Window.SquadViewContainer).AddChild((Control)(object)monitor);
			}
			monitor.OverwatchLabel.Text = squad2.Name + " Overwatch | Dashboard";
			monitor.OnStop += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleStopOverwatchBuiMsg());
			};
			int num = 0;
			Dictionary<ProtoId<JobPrototype>, (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>)> dictionary = new Dictionary<ProtoId<JobPrototype>, (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>)>();
			ImmutableArray<JobPrototype>.Enumerator enumerator3 = _squad.SquadRolePrototypes.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				JobPrototype current = enumerator3.Current;
				dictionary[ProtoId<JobPrototype>.op_Implicit(current.ID)] = (new HashSet<OverwatchMarine>(), new HashSet<OverwatchMarine>(), new HashSet<OverwatchMarine>());
			}
			HashSet<NetEntity> hashSet = marines.Select((OverwatchMarine e) => e.Id).ToHashSet();
			Dictionary<NetEntity, OverwatchRow> orNew = Extensions.GetOrNew<NetEntity, Dictionary<NetEntity, OverwatchRow>>(_rows, squad2.Id);
			KeyValuePair<NetEntity, OverwatchRow>[] array = Extensions.ToArray<NetEntity, OverwatchRow>(orNew);
			OverwatchRow value2;
			foreach (KeyValuePair<NetEntity, OverwatchRow> keyValuePair in array)
			{
				keyValuePair.Deconstruct(out key, out value2);
				NetEntity val5 = key;
				OverwatchRow overwatchRow = value2;
				if (!hashSet.Contains(val5))
				{
					((Control)overwatchRow.Name.Panel).Orphan();
					((Control)overwatchRow.Role.Panel).Orphan();
					((Control)overwatchRow.State.Panel).Orphan();
					((Control)overwatchRow.Location.Panel).Orphan();
					((Control)overwatchRow.Distance.Panel).Orphan();
					((Control)overwatchRow.Buttons.Container).Orphan();
					_rows.Remove(val5);
				}
			}
			foreach (OverwatchMarine marine in marines)
			{
				string text = "None";
				string text2 = null;
				if (marine.Role.HasValue)
				{
					LocId? roleOverride = marine.RoleOverride;
					if (roleOverride.HasValue)
					{
						LocId valueOrDefault = roleOverride.GetValueOrDefault();
						if (_localization.TryGetString(LocId.op_Implicit(valueOrDefault), ref text3))
						{
							text = text3;
							goto IL_0c2e;
						}
					}
					if (_prototypes.TryIndex<JobPrototype>(marine.Role, ref jobPrototype))
					{
						text = jobPrototype.LocalizedName;
					}
					goto IL_0c2e;
				}
				goto IL_0cf3;
				IL_15e5:
				NetEntity? val4 = squad2.Leader;
				key = marine.Id;
				OverwatchRow value3;
				if (val4.HasValue && val4.GetValueOrDefault() == key)
				{
					((Control)value3.Buttons.Hide).Visible = false;
					((Control)value3.Buttons.Promote).Visible = false;
				}
				continue;
				IL_0cf3:
				if (marine.Rank.HasValue && _prototypes.TryIndex<RankPrototype>(marine.Rank, ref rankPrototype))
				{
					text2 = rankPrototype.Prefix;
				}
				string text4 = ((text2 != null) ? (text2 + " " + marine.Name) : marine.Name);
				if (!orNew.TryGetValue(marine.Id, out value3))
				{
					Button val6 = new Button
					{
						StyleClasses = { "OpenBoth" },
						Margin = new Thickness(2f, 0f)
					};
					((BaseButton)val6).OnPressed += delegate
					{
						//IL_001b: Unknown result type (might be due to invalid IL or missing references)
						((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleWatchBuiMsg(marine.Id));
					};
					RichTextLabel val7 = new RichTextLabel();
					((Control)val6).AddChild((Control)(object)val7);
					PanelContainer val8 = CreatePanel(50f);
					((Control)val6).Margin = margin;
					((Control)val8).AddChild((Control)(object)val6);
					((Control)monitor.Names).AddChild((Control)(object)val8);
					PanelContainer val9 = CreatePanel(50f);
					Label val10 = new Label
					{
						Text = text,
						Margin = margin
					};
					((Control)val9).AddChild((Control)(object)val10);
					((Control)monitor.Roles).AddChild((Control)(object)val9);
					RichTextLabel val11 = new RichTextLabel
					{
						Margin = margin
					};
					PanelContainer val12 = CreatePanel(50f);
					((Control)val12).AddChild((Control)(object)val11);
					((Control)monitor.States).AddChild((Control)(object)val12);
					PanelContainer val13 = CreatePanel(50f);
					RichTextLabel val14 = new RichTextLabel
					{
						Margin = margin,
						MaxWidth = 250f
					};
					((Control)val13).AddChild((Control)(object)val14);
					((Control)monitor.Locations).AddChild((Control)(object)val13);
					PanelContainer val15 = CreatePanel(50f);
					Label val16 = new Label
					{
						Margin = margin
					};
					((Control)val15).AddChild((Control)(object)val16);
					((Control)monitor.Distances).AddChild((Control)(object)val15);
					Button val17 = new Button
					{
						MaxWidth = 25f,
						MaxHeight = 25f,
						VerticalAlignment = (VAlignment)1,
						StyleClasses = { "OpenBoth" },
						Text = "-",
						ModulateSelfOverride = Color.FromHex((ReadOnlySpan<char>)"#BB1F1D", (Color?)null),
						ToolTip = "Hide marine"
					};
					Button val18 = new Button
					{
						MaxWidth = 25f,
						MaxHeight = 25f,
						VerticalAlignment = (VAlignment)1,
						StyleClasses = { "OpenBoth" },
						Text = "^",
						ModulateSelfOverride = Color.FromHex((ReadOnlySpan<char>)"#229132", (Color?)null),
						ToolTip = "Promote marine to Squad Leader"
					};
					((BaseButton)val17).OnPressed += delegate
					{
						//IL_002f: Unknown result type (might be due to invalid IL or missing references)
						//IL_004e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0059: Unknown result type (might be due to invalid IL or missing references)
						//IL_0082: Unknown result type (might be due to invalid IL or missing references)
						bool hide = !_overwatchConsole.IsHidden(Entity<OverwatchConsoleComponent>.op_Implicit((((BoundUserInterface)this).Owner, console)), marine.Id);
						((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleHideBuiMsg(marine.Id, hide));
					};
					((BaseButton)val18).OnPressed += delegate
					{
						//IL_001b: Unknown result type (might be due to invalid IL or missing references)
						((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsolePromoteLeaderBuiMsg(marine.Id, squad2.LeaderIcon));
					};
					PanelContainer val19 = CreatePanel(50f);
					((Control)val17).Margin = margin;
					((Control)val19).AddChild((Control)(object)val17);
					BoxContainer val20 = new BoxContainer
					{
						Orientation = (LayoutOrientation)0
					};
					((Control)val20).AddChild((Control)(object)val19);
					PanelContainer val21 = CreatePanel(50f);
					((Control)val18).Margin = margin;
					((Control)val21).AddChild((Control)(object)val18);
					((Control)val20).AddChild((Control)(object)val21);
					((Control)monitor.Buttons).AddChild((Control)(object)val20);
					value3 = new OverwatchRow(marine.Role, (Panel: val8, Button: val6, Label: val7), (Panel: val9, Label: val10), (Panel: val12, Label: val11), (Panel: val13, Label: val14), (Panel: val15, Label: val16), (Container: val20, Hide: val17, Promote: val18));
					orNew[marine.Id] = value3;
					if (marine.Role.HasValue && Extensions.TryFirstOrNull<KeyValuePair<NetEntity, OverwatchRow>>((IEnumerable<KeyValuePair<NetEntity, OverwatchRow>>)orNew, (Func<KeyValuePair<NetEntity, OverwatchRow>, bool>)delegate(KeyValuePair<NetEntity, OverwatchRow> r)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_000d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0054: Unknown result type (might be due to invalid IL or missing references)
						//IL_005b: Unknown result type (might be due to invalid IL or missing references)
						if (r.Key != marine.Id)
						{
							ProtoId<JobPrototype>? roleId = r.Value.RoleId;
							ProtoId<JobPrototype>? role = marine.Role;
							if (roleId.HasValue != role.HasValue)
							{
								return false;
							}
							if (!roleId.HasValue)
							{
								return true;
							}
							return roleId.GetValueOrDefault() == role.GetValueOrDefault();
						}
						return false;
					}, ref keyValuePair2))
					{
						value2 = keyValuePair2.Value.Value;
						int positionInParent = ((Control)value2.Name.Panel).GetPositionInParent() + 1;
						((Control)value3.Name.Panel).SetPositionInParent(positionInParent);
						((Control)value3.Role.Panel).SetPositionInParent(positionInParent);
						((Control)value3.State.Panel).SetPositionInParent(positionInParent);
						((Control)value3.Location.Panel).SetPositionInParent(positionInParent);
						((Control)value3.Distance.Panel).SetPositionInParent(positionInParent);
						((Control)value3.Buttons.Container).SetPositionInParent(positionInParent);
					}
				}
				NetEntity camera = marine.Camera;
				key = default(NetEntity);
				if (camera == key)
				{
					value3.Name.Label.SetMarkupPermissive($"[color={"#CED22B"}]{text4} (NO CAMERA)[/color]");
					value3.Name.Button.Text = null;
					((BaseButton)value3.Name.Button).Disabled = true;
				}
				else
				{
					value3.Name.Label.Text = null;
					value3.Name.Button.Text = text4;
					((BaseButton)value3.Name.Button).Disabled = false;
				}
				value3.Role.Label.Text = text;
				var (text5, value4) = marine.State switch
				{
					MobState.Critical => ("Unconscious", "#CED22B"), 
					MobState.Dead => ("Dead", "#A42625"), 
					_ => ("Conscious", "#229132"), 
				};
				if (marine.SSD && marine.State != MobState.Dead)
				{
					text5 += " (SSD)";
				}
				value3.State.Label.SetMarkupPermissive($"[color={value4}]{text5}[/color]");
				value3.Location.Label.Text = "[color=white]" + marine.AreaName + "[/color]";
				string text6 = "N/A";
				Vector2? leaderDistance = marine.LeaderDistance;
				if (leaderDistance.HasValue)
				{
					Vector2 valueOrDefault2 = leaderDistance.GetValueOrDefault();
					if (!Vector2Helpers.IsLengthZero(valueOrDefault2))
					{
						text6 = $"{marine.LeaderDistance.Value.Length():F0} ({DirectionExtensions.GetDir(marine.LeaderDistance.Value).GetShorthand()})";
					}
				}
				value3.Distance.Label.Text = text6;
				if (_overwatchConsole.IsHidden(Entity<OverwatchConsoleComponent>.op_Implicit((((BoundUserInterface)this).Owner, console)), marine.Id))
				{
					key = marine.Id;
					val4 = squad2.Leader;
					if (!val4.HasValue || key != val4.GetValueOrDefault())
					{
						value3.Buttons.Hide.Text = "+";
						((Control)value3.Buttons.Hide).ModulateSelfOverride = Color.FromHex((ReadOnlySpan<char>)"#248E34", (Color?)null);
						((Control)value3.Buttons.Hide).ToolTip = "Show marine";
						goto IL_15e5;
					}
				}
				value3.Buttons.Hide.Text = "-";
				((Control)value3.Buttons.Hide).ModulateSelfOverride = Color.FromHex((ReadOnlySpan<char>)"#BB1F1D", (Color?)null);
				((Control)value3.Buttons.Hide).ToolTip = "Hide marine";
				goto IL_15e5;
				IL_0c2e:
				(HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>) orNew2 = Extensions.GetOrNew<ProtoId<JobPrototype>, (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>)>(dictionary, marine.Role.Value, ref flag2);
				if (!flag2)
				{
					orNew2.Item1 = new HashSet<OverwatchMarine>();
					orNew2.Item2 = new HashSet<OverwatchMarine>();
					orNew2.Item3 = new HashSet<OverwatchMarine>();
				}
				if (marine.State == MobState.Alive)
				{
					orNew2.Item2.Add(marine);
					num++;
				}
				if (marine.Deployed)
				{
					orNew2.Item1.Add(marine);
				}
				orNew2.Item3.Add(marine);
				dictionary[marine.Role.Value] = orNew2;
				goto IL_0cf3;
			}
			List<(string, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, bool, int)> list2 = new List<(string, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, bool, int)>();
			foreach (KeyValuePair<ProtoId<JobPrototype>, (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>)> item4 in dictionary)
			{
				item4.Deconstruct(out var key2, out var value5);
				(HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>) tuple2 = value5;
				ProtoId<JobPrototype> val22 = key2;
				var (item, item2, item3) = tuple2;
				if (_prototypes.TryIndex<JobPrototype>(val22, ref jobPrototype2))
				{
					int? overwatchSortPriority = jobPrototype2.OverwatchSortPriority;
					if (overwatchSortPriority.HasValue)
					{
						int valueOrDefault3 = overwatchSortPriority.GetValueOrDefault();
						list2.Add((jobPrototype2.ID, item, item2, item3, jobPrototype2.OverwatchShowName, valueOrDefault3));
					}
				}
			}
			list2.Sort(((string Role, HashSet<OverwatchMarine> Deployed, HashSet<OverwatchMarine> Alive, HashSet<OverwatchMarine> All, bool DisplayName, int Priority) a, (string Role, HashSet<OverwatchMarine> Deployed, HashSet<OverwatchMarine> Alive, HashSet<OverwatchMarine> All, bool DisplayName, int Priority) b) => a.Priority.CompareTo(b.Priority));
			int num3 = 0;
			BoxContainer val23 = null;
			foreach (var (text7, hashSet2, hashSet3, hashSet4, flag3, _) in list2)
			{
				if (!_prototypes.TryIndex<JobPrototype>(text7, ref jobPrototype3))
				{
					continue;
				}
				if (num3 % 2 == 0)
				{
					val23 = new BoxContainer
					{
						Orientation = (LayoutOrientation)0,
						HorizontalExpand = true,
						SeparationOverride = 5
					};
					((Control)monitor.RolesContainer).AddChild((Control)(object)val23);
				}
				string markup;
				string markup2;
				if (flag3)
				{
					if (_squad.IsSquadLeader(ProtoId<JobPrototype>.op_Implicit(text7)) && squad2.Leader.HasValue && Extensions.TryFirstOrNull<OverwatchMarine>((IEnumerable<OverwatchMarine>)marines, (Func<OverwatchMarine, bool>)((OverwatchMarine m) => m.Id == squad2.Leader.Value), ref overwatchMarine))
					{
						markup = overwatchMarine.Value.Name;
						markup2 = ((overwatchMarine.Value.State == MobState.Dead) ? "[bold][color=#A42625]DEAD[/color][/bold]" : "[bold][color=#229132]ALIVE[/color][/bold]");
					}
					else if (Extensions.TryFirstOrNull<OverwatchMarine>((IEnumerable<OverwatchMarine>)hashSet4, ref overwatchMarine2))
					{
						markup = overwatchMarine2.Value.Name;
						markup2 = ((overwatchMarine2.Value.State == MobState.Dead) ? "[bold][color=#A42625]DEAD[/color][/bold]" : "[bold][color=#229132]ALIVE[/color][/bold]");
					}
					else
					{
						markup = "[bold][color=#A42625]NONE[/color][/bold]";
						markup2 = "[bold][color=#A42625]N/A[/color][/bold]";
					}
				}
				else
				{
					markup = $"[bold]{hashSet2.Count} DEPLOYED[/bold]";
					string value6 = ((hashSet3.Count > 0) ? "#229132" : "#A42625");
					markup2 = $"[bold][color={value6}]{hashSet3.Count} ALIVE[/color][/bold]";
				}
				RichTextLabel val24 = new RichTextLabel();
				val24.SetMarkupPermissive(markup);
				RichTextLabel val25 = new RichTextLabel();
				val25.SetMarkupPermissive(markup2);
				PanelContainer val26 = CreatePanel(0f, (Thickness?)new Thickness(0f, 0f, 0f, 1f));
				RichTextLabel val27 = new RichTextLabel
				{
					Margin = new Thickness(0f, 3f, 0f, 3f)
				};
				val27.SetMarkupPermissive("[bold]" + jobPrototype3.OverwatchRoleName + "[/bold]");
				BoxContainer val28 = new BoxContainer
				{
					Orientation = (LayoutOrientation)0
				};
				((Control)val28).Children.Add(new Control
				{
					HorizontalExpand = true
				});
				((Control)val28).Children.Add((Control)(object)val27);
				((Control)val28).Children.Add(new Control
				{
					HorizontalExpand = true
				});
				((Control)val28).Margin = margin;
				((Control)val26).AddChild((Control)val28);
				PanelContainer val29 = CreatePanel();
				((Control)val29).HorizontalExpand = true;
				BoxContainer val30 = new BoxContainer
				{
					Orientation = (LayoutOrientation)1,
					HorizontalExpand = true
				};
				((Control)val30).Children.Add((Control)(object)val26);
				OrderedChildCollection children = ((Control)val30).Children;
				BoxContainer val31 = new BoxContainer
				{
					Orientation = (LayoutOrientation)0
				};
				((Control)val31).Children.Add(new Control
				{
					HorizontalExpand = true
				});
				((Control)val31).Children.Add((Control)(object)val24);
				((Control)val31).Children.Add(new Control
				{
					HorizontalExpand = true
				});
				children.Add((Control)val31);
				OrderedChildCollection children2 = ((Control)val30).Children;
				BoxContainer val32 = new BoxContainer
				{
					Orientation = (LayoutOrientation)0
				};
				((Control)val32).Children.Add(new Control
				{
					HorizontalExpand = true
				});
				((Control)val32).Children.Add((Control)(object)val25);
				((Control)val32).Children.Add(new Control
				{
					HorizontalExpand = true
				});
				children2.Add((Control)val32);
				((Control)val29).AddChild((Control)val30);
				num3++;
				if (val23 != null)
				{
					((Control)val23).AddChild((Control)(object)val29);
				}
			}
			string value7 = ((num > 0) ? "#229132" : "#A42625");
			string markup3 = $"[bold][color={value7}]{num} ALIVE[/color][/bold]";
			RichTextLabel val33 = new RichTextLabel();
			val33.SetMarkupPermissive(markup3);
			PanelContainer val34 = CreatePanel();
			((Control)val34).HorizontalExpand = true;
			PanelContainer val35 = CreatePanel(0f, (Thickness?)new Thickness(0f, 0f, 0f, 1f));
			RichTextLabel val36 = new RichTextLabel();
			val36.SetMarkupPermissive("[bold]Total/Living[/bold]");
			BoxContainer val37 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			((Control)val37).Children.Add(new Control
			{
				HorizontalExpand = true
			});
			((Control)val37).Children.Add((Control)(object)val36);
			((Control)val37).Children.Add(new Control
			{
				HorizontalExpand = true
			});
			((Control)val37).Margin = margin;
			((Control)val35).AddChild((Control)val37);
			RichTextLabel val38 = new RichTextLabel();
			val38.SetMarkupPermissive($"[bold]{marines.Count} TOTAL[/bold]");
			BoxContainer val39 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				HorizontalExpand = true
			};
			((Control)val39).Children.Add((Control)(object)val35);
			OrderedChildCollection children3 = ((Control)val39).Children;
			BoxContainer val40 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			((Control)val40).Children.Add(new Control
			{
				HorizontalExpand = true
			});
			((Control)val40).Children.Add((Control)(object)val38);
			((Control)val40).Children.Add(new Control
			{
				HorizontalExpand = true
			});
			children3.Add((Control)val40);
			OrderedChildCollection children4 = ((Control)val39).Children;
			BoxContainer val41 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			((Control)val41).Children.Add(new Control
			{
				HorizontalExpand = true
			});
			((Control)val41).Children.Add((Control)(object)val33);
			((Control)val41).Children.Add(new Control
			{
				HorizontalExpand = true
			});
			children4.Add((Control)val41);
			((Control)val34).AddChild((Control)val39);
			((Control)monitor.RolesContainer).AddChild((Control)(object)val34);
			monitor.UpdateResults(console.Location, console.ShowDead, console.ShowHidden, marines, console);
			int Sorting(OverwatchMarine overwatchMarine3)
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				NetEntity? leader = squad2.Leader;
				NetEntity id2 = overwatchMarine3.Id;
				if (leader.HasValue && leader.GetValueOrDefault() == id2)
				{
					return 1000;
				}
				ProtoId<JobPrototype>? role = overwatchMarine3.Role;
				if (!role.HasValue)
				{
					return 0;
				}
				ProtoId<JobPrototype> valueOrDefault4 = role.GetValueOrDefault();
				if (roleSorting.TryGetValue(valueOrDefault4, out var value8))
				{
					return value8;
				}
				JobPrototype jobPrototype4 = default(JobPrototype);
				if (_prototypes.TryIndex<JobPrototype>(valueOrDefault4, ref jobPrototype4))
				{
					int? overwatchSortPriority2 = jobPrototype4.OverwatchSortPriority;
					if (overwatchSortPriority2.HasValue)
					{
						int valueOrDefault5 = overwatchSortPriority2.GetValueOrDefault();
						roleSorting[valueOrDefault4] = valueOrDefault5;
						return valueOrDefault5;
					}
				}
				return 0;
			}
		}
		UpdateView();
	}

	private void UpdateView()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		OverwatchConsoleComponent overwatchConsoleComponent = default(OverwatchConsoleComponent);
		if (Window == null || !((BoundUserInterface)this).EntMan.TryGetComponent<OverwatchConsoleComponent>(((BoundUserInterface)this).Owner, ref overwatchConsoleComponent))
		{
			return;
		}
		SupplyDropComputerComponent componentOrNull = EntityManagerExt.GetComponentOrNull<SupplyDropComputerComponent>(((BoundUserInterface)this).EntMan, ((BoundUserInterface)this).Owner);
		NetEntity? activeSquad = GetActiveSquad();
		if (!activeSquad.HasValue)
		{
			((Control)Window.OverwatchViewContainer).Visible = true;
			((Control)Window.SquadViewContainer).Visible = false;
			Window.Wrapper.VerticalAlignment = (VAlignment)1;
		}
		else
		{
			((Control)Window.OverwatchViewContainer).Visible = false;
			((Control)Window.SquadViewContainer).Visible = true;
			Window.Wrapper.VerticalAlignment = (VAlignment)0;
		}
		string text = GetOperator();
		Thickness margin = default(Thickness);
		foreach (KeyValuePair<NetEntity, OverwatchSquadView> squadView in _squadViews)
		{
			squadView.Deconstruct(out var key, out var value);
			NetEntity val = key;
			OverwatchSquadView squad = value;
			OverwatchSquadView overwatchSquadView = squad;
			key = val;
			NetEntity? val2 = activeSquad;
			((Control)overwatchSquadView).Visible = val2.HasValue && key == val2.GetValueOrDefault();
			squad.OperatorButton.Text = ((text == null) ? string.Empty : ("Operator - " + text));
			Button showLocationButton = squad.ShowLocationButton;
			showLocationButton.Text = overwatchConsoleComponent.Location switch
			{
				OverwatchLocation.Min => "Shown: planetside", 
				OverwatchLocation.Ship => "Shown: shipside", 
				_ => "Shown: all", 
			};
			squad.ShowDeadButton.Text = (overwatchConsoleComponent.ShowDead ? "Hide dead" : "Show dead");
			squad.ShowHiddenButton.Text = (overwatchConsoleComponent.ShowHidden ? "Hide hidden" : "Show hidden");
			((Thickness)(ref margin))._002Ector(2f);
			if (componentOrNull != null)
			{
				squad.HasCrate = componentOrNull.HasCrate;
				squad.NextLaunchAt = componentOrNull.NextLaunchAt;
				AddSaving(squad.Longitudes, squad.Latitudes, squad.Comments, squad.Saves, margin);
				AddSavedLocation(overwatchConsoleComponent.SavedLocations, margin, squad.Longitudes, squad.Latitudes, squad.Comments, squad.Saves, delegate(OverwatchSavedLocation location)
				{
					squad.Longitude.Value = location.Longitude;
					squad.Latitude.Value = location.Latitude;
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleSupplyDropLongitudeBuiMsg(location.Longitude));
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleSupplyDropLatitudeBuiMsg(location.Latitude));
				});
			}
			AddSaving(squad.OrbitalLongitudes, squad.OrbitalLatitudes, squad.OrbitalComments, squad.OrbitalSaves, margin);
			AddSavedLocation(overwatchConsoleComponent.SavedLocations, margin, squad.OrbitalLongitudes, squad.OrbitalLatitudes, squad.OrbitalComments, squad.OrbitalSaves, delegate(OverwatchSavedLocation location)
			{
				squad.Longitude.Value = location.Longitude;
				squad.Latitude.Value = location.Latitude;
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleOrbitalLongitudeBuiMsg(location.Longitude));
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleOrbitalLatitudeBuiMsg(location.Latitude));
			});
			squad.HasOrbital = overwatchConsoleComponent.HasOrbital;
			squad.NextOrbitalAt = overwatchConsoleComponent.NextOrbitalLaunch;
		}
	}

	private void AddSaving(BoxContainer longitudes, BoxContainer latitudes, BoxContainer comments, BoxContainer saves, Thickness margin)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		((Control)longitudes).DisposeAllChildren();
		PanelContainer val = CreatePanel(50f);
		((Control)val).AddChild((Control)new Label
		{
			Text = "LONG.",
			Margin = margin
		});
		((Control)longitudes).AddChild((Control)(object)val);
		((Control)latitudes).DisposeAllChildren();
		val = CreatePanel(50f);
		((Control)val).AddChild((Control)new Label
		{
			Text = "LAT.",
			Margin = margin
		});
		((Control)latitudes).AddChild((Control)(object)val);
		((Control)comments).DisposeAllChildren();
		val = CreatePanel(50f);
		((Control)val).AddChild((Control)new Label
		{
			Text = "COMMENT",
			Margin = margin
		});
		((Control)comments).AddChild((Control)(object)val);
		((Control)saves).DisposeAllChildren();
		val = CreatePanel(50f);
		((Control)val).AddChild((Control)new Label
		{
			Text = " ",
			Margin = margin
		});
		((Control)saves).AddChild((Control)(object)val);
	}

	private void AddSavedLocation(OverwatchSavedLocation?[] locations, Thickness margin, BoxContainer longitudes, BoxContainer latitudes, BoxContainer comments, BoxContainer saves, Action<OverwatchSavedLocation> onSave)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected O, but got Unknown
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		for (int i = 0; i < locations.Length; i++)
		{
			OverwatchSavedLocation? overwatchSavedLocation = locations[i];
			if (overwatchSavedLocation.HasValue)
			{
				OverwatchSavedLocation location = overwatchSavedLocation.GetValueOrDefault();
				PanelContainer val = CreatePanel(50f);
				((Control)val).AddChild((Control)new Label
				{
					Text = $"{location.Longitude}",
					Margin = margin
				});
				((Control)longitudes).AddChild((Control)(object)val);
				val = CreatePanel(50f);
				((Control)val).AddChild((Control)new Label
				{
					Text = $"{location.Latitude}",
					Margin = margin
				});
				((Control)latitudes).AddChild((Control)(object)val);
				LineEdit val2 = new LineEdit
				{
					Text = (location.Comment ?? "")
				};
				int index = i;
				val2.OnTextEntered += delegate(LineEditEventArgs args)
				{
					SaveComment(index, args.Text);
				};
				val = CreatePanel(50f);
				((Control)val).AddChild((Control)(object)val2);
				((Control)comments).AddChild((Control)(object)val);
				val = CreatePanel(50f);
				Button val3 = new Button
				{
					MaxWidth = 25f,
					MaxHeight = 25f,
					VerticalAlignment = (VAlignment)1,
					StyleClasses = { "OpenBoth" },
					Text = "<",
					ModulateSelfOverride = Color.FromHex((ReadOnlySpan<char>)"#D3B400", (Color?)null),
					ToolTip = "Save Comment"
				};
				((BaseButton)val3).OnPressed += delegate
				{
					onSave(location);
				};
				((Control)val).AddChild((Control)(object)val3);
				((Control)saves).AddChild((Control)(object)val);
			}
		}
	}

	private PanelContainer CreatePanel(float minHeight = 0f, Thickness? thickness = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_0061: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Thickness valueOrDefault = thickness.GetValueOrDefault();
		if (!thickness.HasValue)
		{
			((Thickness)(ref valueOrDefault))._002Ector(1f);
			thickness = valueOrDefault;
		}
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BorderColor = Color.FromHex((ReadOnlySpan<char>)"#88C7FA", (Color?)null),
				BorderThickness = thickness.Value
			}
		};
		if (minHeight > 0f)
		{
			((Control)val).MinHeight = minHeight;
		}
		return val;
	}

	private NetEntity? GetActiveSquad()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		OverwatchConsoleComponent overwatchConsoleComponent = default(OverwatchConsoleComponent);
		if (!((BoundUserInterface)this).EntMan.TryGetComponent<OverwatchConsoleComponent>(((BoundUserInterface)this).Owner, ref overwatchConsoleComponent))
		{
			return null;
		}
		return overwatchConsoleComponent.Squad;
	}

	private string? GetOperator()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		OverwatchConsoleComponent overwatchConsoleComponent = default(OverwatchConsoleComponent);
		if (!((BoundUserInterface)this).EntMan.TryGetComponent<OverwatchConsoleComponent>(((BoundUserInterface)this).Owner, ref overwatchConsoleComponent))
		{
			return null;
		}
		return overwatchConsoleComponent.Operator;
	}

	private void SaveComment(int index, string text)
	{
		if (text.Length > 50)
		{
			text = text.Substring(0, 50);
		}
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OverwatchConsoleLocationCommentBuiMsg(index, text));
	}

	public void Refresh()
	{
		if (((BoundUserInterface)this).State is OverwatchConsoleBuiState s)
		{
			RefreshState(s);
		}
	}
}
