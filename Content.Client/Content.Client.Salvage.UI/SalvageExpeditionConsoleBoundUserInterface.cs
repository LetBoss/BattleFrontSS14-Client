using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Stylesheets;
using Content.Shared.CCVar;
using Content.Shared.Procedural;
using Content.Shared.Salvage.Expeditions;
using Content.Shared.Salvage.Expeditions.Modifiers;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Salvage.UI;

public sealed class SalvageExpeditionConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private OfferingWindow? _window;

	[Dependency]
	private IConfigurationManager _cfgManager;

	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private ILogManager _logManager;

	[Dependency]
	private IPrototypeManager _protoManager;

	private readonly ISawmill _sawmill;

	public SalvageExpeditionConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<SalvageExpeditionConsoleBoundUserInterface>(this);
		_sawmill = _logManager.GetSawmill("salvage.expedition.console");
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindowCenteredLeft<OfferingWindow>((BoundUserInterface)(object)this);
		_window.Title = Loc.GetString("salvage-expedition-window-title");
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Expected O, but got Unknown
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Expected O, but got Unknown
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Expected O, but got Unknown
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Expected O, but got Unknown
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Expected O, but got Unknown
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Expected O, but got Unknown
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Expected O, but got Unknown
		((BoundUserInterface)this).UpdateState(state);
		if (!(state is SalvageExpeditionConsoleState salvageExpeditionConsoleState) || _window == null)
		{
			return;
		}
		_window.Progression = null;
		_window.Cooldown = TimeSpan.FromSeconds(_cfgManager.GetCVar<float>(CCVars.SalvageExpeditionCooldown));
		_window.NextOffer = salvageExpeditionConsoleState.NextOffer;
		_window.Claimed = salvageExpeditionConsoleState.Claimed;
		_window.ClearOptions();
		SalvageSystem salvageSystem = _entManager.System<SalvageSystem>();
		for (int i = 0; i < salvageExpeditionConsoleState.Missions.Count; i++)
		{
			SalvageMissionParams missionParams = salvageExpeditionConsoleState.Missions[i];
			OfferingWindowOption offeringWindowOption = new OfferingWindowOption();
			offeringWindowOption.Title = Loc.GetString("salvage-expedition-type");
			string text = "Moderate";
			SalvageDifficultyPrototype salvageDifficultyPrototype = _protoManager.Index<SalvageDifficultyPrototype>(text);
			SalvageMission mission = salvageSystem.GetMission(salvageDifficultyPrototype, missionParams.Seed);
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = Loc.GetString("salvage-expedition-window-difficulty")
			});
			Color color = salvageDifficultyPrototype.Color;
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = Loc.GetString("salvage-expedition-difficulty-Moderate"),
				FontColorOverride = color,
				HorizontalAlignment = (HAlignment)1,
				Margin = new Thickness(0f, 0f, 0f, 5f)
			});
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = Loc.GetString("salvage-expedition-difficulty-players"),
				HorizontalAlignment = (HAlignment)1
			});
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = salvageDifficultyPrototype.RecommendedPlayers.ToString(),
				FontColorOverride = StyleNano.NanoGold,
				HorizontalAlignment = (HAlignment)1,
				Margin = new Thickness(0f, 0f, 0f, 5f)
			});
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = Loc.GetString("salvage-expedition-window-hostiles")
			});
			string faction = mission.Faction;
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = (string.IsNullOrWhiteSpace(Loc.GetString(LocId.op_Implicit(_protoManager.Index<SalvageFactionPrototype>(faction).Description))) ? LogAndReturnDefaultFactionDescription(faction) : Loc.GetString(LocId.op_Implicit(_protoManager.Index<SalvageFactionPrototype>(faction).Description))),
				FontColorOverride = StyleNano.NanoGold,
				HorizontalAlignment = (HAlignment)1,
				Margin = new Thickness(0f, 0f, 0f, 5f)
			});
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = Loc.GetString("salvage-expedition-window-duration")
			});
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = mission.Duration.ToString(),
				FontColorOverride = StyleNano.NanoGold,
				HorizontalAlignment = (HAlignment)1,
				Margin = new Thickness(0f, 0f, 0f, 5f)
			});
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = Loc.GetString("salvage-expedition-window-biome")
			});
			string biome = mission.Biome;
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = (string.IsNullOrWhiteSpace(Loc.GetString(LocId.op_Implicit(_protoManager.Index<SalvageBiomeModPrototype>(biome).Description))) ? LogAndReturnDefaultBiomDescription(biome) : Loc.GetString(LocId.op_Implicit(_protoManager.Index<SalvageBiomeModPrototype>(biome).Description))),
				FontColorOverride = StyleNano.NanoGold,
				HorizontalAlignment = (HAlignment)1,
				Margin = new Thickness(0f, 0f, 0f, 5f)
			});
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = Loc.GetString("salvage-expedition-window-modifiers")
			});
			List<string> modifiers = mission.Modifiers;
			offeringWindowOption.AddContent((Control)new Label
			{
				Text = string.Join("\n", modifiers.Select((string o) => "- " + o)).TrimEnd(),
				FontColorOverride = StyleNano.NanoGold,
				HorizontalAlignment = (HAlignment)1,
				Margin = new Thickness(0f, 0f, 0f, 5f)
			});
			offeringWindowOption.ClaimPressed += delegate
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ClaimSalvageMessage
				{
					Index = missionParams.Index
				});
			};
			offeringWindowOption.Claimed = salvageExpeditionConsoleState.ActiveMission == missionParams.Index;
			offeringWindowOption.Disabled = salvageExpeditionConsoleState.Claimed || salvageExpeditionConsoleState.Cooldown;
			_window.AddOption(offeringWindowOption);
		}
		string LogAndReturnDefaultBiomDescription(string text2)
		{
			_sawmill.Error("Description is null or white space for SalvageBiomeModPrototype: " + text2);
			return Loc.GetString(_protoManager.Index<SalvageBiomeModPrototype>(text2).ID);
		}
		string LogAndReturnDefaultFactionDescription(string text2)
		{
			_sawmill.Error("Description is null or white space for SalvageFactionPrototype: " + text2);
			return Loc.GetString(_protoManager.Index<SalvageFactionPrototype>(text2).ID);
		}
	}
}
