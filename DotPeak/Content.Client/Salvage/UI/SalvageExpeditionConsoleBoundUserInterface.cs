// Decompiled with JetBrains decompiler
// Type: Content.Client.Salvage.UI.SalvageExpeditionConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Salvage.UI;

public sealed class SalvageExpeditionConsoleBoundUserInterface : BoundUserInterface
{
  [Robust.Shared.ViewVariables.ViewVariables]
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
    IoCManager.InjectDependencies<SalvageExpeditionConsoleBoundUserInterface>(this);
    this._sawmill = this._logManager.GetSawmill("salvage.expedition.console");
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindowCenteredLeft<OfferingWindow>((BoundUserInterface) this);
    this._window.Title = Loc.GetString("salvage-expedition-window-title");
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is SalvageExpeditionConsoleState expeditionConsoleState) || this._window == null)
      return;
    this._window.Progression = new TimeSpan?();
    this._window.Cooldown = TimeSpan.FromSeconds((double) this._cfgManager.GetCVar<float>(CCVars.SalvageExpeditionCooldown));
    this._window.NextOffer = expeditionConsoleState.NextOffer;
    this._window.Claimed = expeditionConsoleState.Claimed;
    this._window.ClearOptions();
    SalvageSystem salvageSystem = this._entManager.System<SalvageSystem>();
    for (int index = 0; index < expeditionConsoleState.Missions.Count; ++index)
    {
      SalvageMissionParams missionParams = expeditionConsoleState.Missions[index];
      OfferingWindowOption option = new OfferingWindowOption();
      option.Title = Loc.GetString("salvage-expedition-type");
      SalvageDifficultyPrototype difficulty = this._protoManager.Index<SalvageDifficultyPrototype>("Moderate");
      SalvageMission mission = salvageSystem.GetMission(difficulty, missionParams.Seed);
      option.AddContent((Control) new Label()
      {
        Text = Loc.GetString("salvage-expedition-window-difficulty")
      });
      Color color = difficulty.Color;
      OfferingWindowOption offeringWindowOption1 = option;
      Label label1 = new Label();
      label1.Text = Loc.GetString("salvage-expedition-difficulty-Moderate");
      label1.FontColorOverride = new Color?(color);
      ((Control) label1).HorizontalAlignment = (Control.HAlignment) 1;
      ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 5f);
      offeringWindowOption1.AddContent((Control) label1);
      OfferingWindowOption offeringWindowOption2 = option;
      Label label2 = new Label();
      label2.Text = Loc.GetString("salvage-expedition-difficulty-players");
      ((Control) label2).HorizontalAlignment = (Control.HAlignment) 1;
      offeringWindowOption2.AddContent((Control) label2);
      OfferingWindowOption offeringWindowOption3 = option;
      Label label3 = new Label();
      label3.Text = difficulty.RecommendedPlayers.ToString();
      label3.FontColorOverride = new Color?(StyleNano.NanoGold);
      ((Control) label3).HorizontalAlignment = (Control.HAlignment) 1;
      ((Control) label3).Margin = new Thickness(0.0f, 0.0f, 0.0f, 5f);
      offeringWindowOption3.AddContent((Control) label3);
      option.AddContent((Control) new Label()
      {
        Text = Loc.GetString("salvage-expedition-window-hostiles")
      });
      string faction = mission.Faction;
      OfferingWindowOption offeringWindowOption4 = option;
      Label label4 = new Label();
      label4.Text = string.IsNullOrWhiteSpace(Loc.GetString(LocId.op_Implicit(this._protoManager.Index<SalvageFactionPrototype>(faction).Description))) ? LogAndReturnDefaultFactionDescription(faction) : Loc.GetString(LocId.op_Implicit(this._protoManager.Index<SalvageFactionPrototype>(faction).Description));
      label4.FontColorOverride = new Color?(StyleNano.NanoGold);
      ((Control) label4).HorizontalAlignment = (Control.HAlignment) 1;
      ((Control) label4).Margin = new Thickness(0.0f, 0.0f, 0.0f, 5f);
      offeringWindowOption4.AddContent((Control) label4);
      option.AddContent((Control) new Label()
      {
        Text = Loc.GetString("salvage-expedition-window-duration")
      });
      OfferingWindowOption offeringWindowOption5 = option;
      Label label5 = new Label();
      label5.Text = mission.Duration.ToString();
      label5.FontColorOverride = new Color?(StyleNano.NanoGold);
      ((Control) label5).HorizontalAlignment = (Control.HAlignment) 1;
      ((Control) label5).Margin = new Thickness(0.0f, 0.0f, 0.0f, 5f);
      offeringWindowOption5.AddContent((Control) label5);
      option.AddContent((Control) new Label()
      {
        Text = Loc.GetString("salvage-expedition-window-biome")
      });
      string biome = mission.Biome;
      OfferingWindowOption offeringWindowOption6 = option;
      Label label6 = new Label();
      label6.Text = string.IsNullOrWhiteSpace(Loc.GetString(LocId.op_Implicit(this._protoManager.Index<SalvageBiomeModPrototype>(biome).Description))) ? LogAndReturnDefaultBiomDescription(biome) : Loc.GetString(LocId.op_Implicit(this._protoManager.Index<SalvageBiomeModPrototype>(biome).Description));
      label6.FontColorOverride = new Color?(StyleNano.NanoGold);
      ((Control) label6).HorizontalAlignment = (Control.HAlignment) 1;
      ((Control) label6).Margin = new Thickness(0.0f, 0.0f, 0.0f, 5f);
      offeringWindowOption6.AddContent((Control) label6);
      option.AddContent((Control) new Label()
      {
        Text = Loc.GetString("salvage-expedition-window-modifiers")
      });
      List<string> modifiers = mission.Modifiers;
      OfferingWindowOption offeringWindowOption7 = option;
      Label label7 = new Label();
      label7.Text = string.Join("\n", modifiers.Select<string, string>((Func<string, string>) (o => "- " + o))).TrimEnd();
      label7.FontColorOverride = new Color?(StyleNano.NanoGold);
      ((Control) label7).HorizontalAlignment = (Control.HAlignment) 1;
      ((Control) label7).Margin = new Thickness(0.0f, 0.0f, 0.0f, 5f);
      offeringWindowOption7.AddContent((Control) label7);
      option.ClaimPressed += (Action<BaseButton.ButtonEventArgs>) (args => this.SendMessage((BoundUserInterfaceMessage) new ClaimSalvageMessage()
      {
        Index = missionParams.Index
      }));
      option.Claimed = (int) expeditionConsoleState.ActiveMission == (int) missionParams.Index;
      option.Disabled = expeditionConsoleState.Claimed || expeditionConsoleState.Cooldown;
      this._window.AddOption(option);
    }

    string LogAndReturnDefaultFactionDescription(string faction)
    {
      this._sawmill.Error("Description is null or white space for SalvageFactionPrototype: " + faction);
      return Loc.GetString(this._protoManager.Index<SalvageFactionPrototype>(faction).ID);
    }

    string LogAndReturnDefaultBiomDescription(string biome)
    {
      this._sawmill.Error("Description is null or white space for SalvageBiomeModPrototype: " + biome);
      return Loc.GetString(this._protoManager.Index<SalvageBiomeModPrototype>(biome).ID);
    }
  }
}
