using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Administration.UI.CustomControls;
using Content.Client.Eui;
using Content.Shared._RMC14.Admin;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Eui;
using Content.Shared.NPC.Prototypes;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Admin.Global;

public sealed class RMCGlobalAdminEui : BaseEui
{
	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private IPrototypeManager _prototypes;

	private RMCGlobalAdminWindow _window;

	public override void Opened()
	{
		_window = new RMCGlobalAdminWindow();
		TabContainer.SetTabTitle((Control)(object)_window.CVarsTab, "CVars");
		TabContainer.SetTabTitle((Control)(object)_window.MarinesTab, "Marines");
		TabContainer.SetTabTitle((Control)(object)_window.XenosTab, "Xenos");
		TabContainer.SetTabTitle((Control)(object)_window.TacticalMapTab, "Tactical Map");
		TabContainer.SetTabTitle((Control)(object)_window.FactionsTab, "Factions");
		((BaseButton)_window.RefreshButton).OnPressed += OnRefresh;
		((BaseWindow)_window).OpenCentered();
	}

	public override void HandleState(EuiStateBase state)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_0141: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Expected O, but got Unknown
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Expected O, but got Unknown
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Expected O, but got Unknown
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Expected O, but got Unknown
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Expected O, but got Unknown
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Expected O, but got Unknown
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Expected O, but got Unknown
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_0938: Unknown result type (might be due to invalid IL or missing references)
		//IL_094d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Expected O, but got Unknown
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_0894: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Expected O, but got Unknown
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f7: Expected O, but got Unknown
		if (!(state is RMCAdminEuiState rMCAdminEuiState))
		{
			return;
		}
		((Control)_window.CVars).DisposeAllChildren();
		((Control)_window.Squads).DisposeAllChildren();
		((Control)_window.XenoTiers).DisposeAllChildren();
		((Control)_window.TacticalMapHistory).DisposeAllChildren();
		((Control)_window.Factions).DisposeAllChildren();
		foreach (string registeredCVar in _config.GetRegisteredCVars())
		{
			if (registeredCVar.StartsWith("rmc.") && !registeredCVar.Contains("play_voicelines_"))
			{
				BoxContainer cVars = _window.CVars;
				BoxContainer val = new BoxContainer
				{
					Orientation = (LayoutOrientation)1
				};
				OrderedChildCollection children = ((Control)val).Children;
				HSeparator obj = new HSeparator
				{
					Color = Color.FromHex((ReadOnlySpan<char>)"#4972A1", (Color?)null)
				};
				((Control)obj).Margin = new Thickness(0f, 10f);
				children.Add((Control)(object)obj);
				((Control)val).Children.Add((Control)new Label
				{
					Text = registeredCVar,
					MinWidth = 50f
				});
				((Control)val).Children.Add((Control)new Label
				{
					Text = _config.GetCVar(registeredCVar).ToString()
				});
				((Control)cVars).AddChild((Control)val);
			}
		}
		EntityPrototype val2 = default(EntityPrototype);
		SquadTeamComponent squadTeamComponent = default(SquadTeamComponent);
		foreach (Squad squad in rMCAdminEuiState.Squads)
		{
			RMCSquadRow rMCSquadRow = new RMCSquadRow();
			((Control)rMCSquadRow).HorizontalExpand = true;
			((Control)rMCSquadRow).Margin = new Thickness(0f, 0f, 0f, 10f);
			RMCSquadRow rMCSquadRow2 = rMCSquadRow;
			((Control)rMCSquadRow2.AddToSquadButton).Visible = false;
			string name = string.Empty;
			Color color = Color.White;
			if (_prototypes.TryIndex(squad.Id, ref val2))
			{
				name = val2.Name;
				if (val2.TryGetComponent<SquadTeamComponent>(ref squadTeamComponent, _compFactory))
				{
					color = squadTeamComponent.Color;
				}
			}
			rMCSquadRow2.CreateSquadButton(squad.Exists, delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				SendMessage(new RMCAdminCreateSquadMsg(squad.Id));
			}, squad.Members, name, color);
			((Control)_window.Squads).AddChild((Control)(object)rMCSquadRow2);
		}
		_window.MarinesLabel.Text = $"Total marine players alive: {rMCAdminEuiState.Marines}";
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		XenoComponent xenoComponent = default(XenoComponent);
		foreach (EntityPrototype item3 in _prototypes.EnumeratePrototypes<EntityPrototype>())
		{
			if (!item3.Abstract && item3.TryGetComponent<XenoComponent>(ref xenoComponent, _compFactory))
			{
				Extensions.GetOrNew<int, int>(dictionary, xenoComponent.Tier);
			}
		}
		EntityPrototype val3 = default(EntityPrototype);
		XenoComponent xenoComponent2 = default(XenoComponent);
		foreach (Xeno xeno in rMCAdminEuiState.Xenos)
		{
			if (_prototypes.TryIndex(xeno.Proto, ref val3) && val3.TryGetComponent<XenoComponent>(ref xenoComponent2, _compFactory))
			{
				dictionary[xenoComponent2.Tier] = Extensions.GetOrNew<int, int>(dictionary, xenoComponent2.Tier) + 1;
			}
		}
		foreach (var (value, value2) in dictionary.OrderBy((KeyValuePair<int, int> x) => x.Key))
		{
			((Control)_window.XenoTiers).AddChild((Control)new Label
			{
				Text = $"Tier {value}: {value2} xenos"
			});
			BoxContainer xenoTiers = _window.XenoTiers;
			HSeparator obj2 = new HSeparator
			{
				Color = Color.FromHex((ReadOnlySpan<char>)"#4972A1", (Color?)null)
			};
			((Control)obj2).Margin = new Thickness(0f, 10f);
			((Control)xenoTiers).AddChild((Control)(object)obj2);
		}
		_window.XenosLabel.Text = $"Total xenonid players alive: {rMCAdminEuiState.Xenos.Count}";
		foreach (var item4 in rMCAdminEuiState.TacticalMapHistory)
		{
			Guid guid = item4.Id;
			string item = item4.Actor;
			int item2 = item4.Round;
			Button val4 = new Button
			{
				Text = $"Round {item2} by {item}"
			};
			((BaseButton)val4).OnPressed += delegate
			{
				SendMessage(new RMCAdminRequestTacticalMapHistory(guid));
			};
			((Control)_window.TacticalMapHistory).AddChild((Control)(object)val4);
		}
		_window.TacticalMap.Lines.Clear();
		var (guid2, list, key, num3) = rMCAdminEuiState.TacticalMapLines;
		if (guid2 == default(Guid) && list == null && key == null && num3 == 0)
		{
			_window.TacticalMapLabel.Text = "Selected: None";
		}
		else
		{
			_window.TacticalMapLabel.Text = $"Selected: Round {rMCAdminEuiState.TacticalMapLines.RoundId} by {rMCAdminEuiState.TacticalMapLines.Actor}";
			((TextureRect)_window.TacticalMap).Texture = Texture.Transparent;
			_window.TacticalMap.Lines.AddRange(rMCAdminEuiState.TacticalMapLines.Lines);
		}
		foreach (KeyValuePair<string, FactionData> faction in rMCAdminEuiState.Factions)
		{
			faction.Deconstruct(out key, out var value3);
			string left = key;
			FactionData factionData = value3;
			BoxContainer val5 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1
			};
			((Control)val5).AddChild((Control)new Label
			{
				Text = left + ":",
				MinWidth = 50f
			});
			foreach (string right in rMCAdminEuiState.Factions.Keys)
			{
				if (!(left == right))
				{
					BoxContainer val6 = new BoxContainer
					{
						Orientation = (LayoutOrientation)0
					};
					ButtonGroup val7 = new ButtonGroup(true);
					Button val8 = new Button
					{
						Text = "+",
						ToggleMode = true,
						Pressed = factionData.Friendly.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(right)),
						Group = val7
					};
					((BaseButton)val8).OnPressed += delegate
					{
						SendMessage(new RMCAdminFactionMsg(RMCAdminFactionMsgType.Friendly, left, right));
					};
					((Control)val6).AddChild((Control)(object)val8);
					Button val9 = new Button
					{
						Text = "=",
						ToggleMode = true,
						Pressed = (!factionData.Friendly.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(right)) && !factionData.Hostile.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(right))),
						Group = val7
					};
					((BaseButton)val9).OnPressed += delegate
					{
						SendMessage(new RMCAdminFactionMsg(RMCAdminFactionMsgType.Neutral, left, right));
					};
					((Control)val6).AddChild((Control)(object)val9);
					Button val10 = new Button
					{
						Text = "-",
						ToggleMode = true,
						Pressed = factionData.Hostile.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(right)),
						Group = val7
					};
					((BaseButton)val10).OnPressed += delegate
					{
						SendMessage(new RMCAdminFactionMsg(RMCAdminFactionMsgType.Hostile, left, right));
					};
					((Control)val6).AddChild((Control)(object)val10);
					((Control)val6).AddChild((Control)new Label
					{
						Text = (right ?? ""),
						MinWidth = 50f
					});
					((Control)val5).AddChild((Control)(object)val6);
				}
			}
			HSeparator obj3 = new HSeparator
			{
				Color = Color.FromHex((ReadOnlySpan<char>)"#4972A1", (Color?)null)
			};
			((Control)obj3).Margin = new Thickness(0f, 10f);
			((Control)val5).AddChild((Control)(object)obj3);
			((Control)_window.Factions).AddChild((Control)(object)val5);
		}
	}

	private void OnRefresh(ButtonEventArgs args)
	{
		SendMessage(new RMCAdminRefresh());
	}
}
