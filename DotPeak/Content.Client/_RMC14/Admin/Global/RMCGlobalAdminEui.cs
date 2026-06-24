// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Admin.Global.RMCGlobalAdminEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.UI.CustomControls;
using Content.Client.Eui;
using Content.Shared._RMC14.Admin;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.TacticalMap;
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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
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
    this._window = new RMCGlobalAdminWindow();
    TabContainer.SetTabTitle((Control) this._window.CVarsTab, "CVars");
    TabContainer.SetTabTitle((Control) this._window.MarinesTab, "Marines");
    TabContainer.SetTabTitle((Control) this._window.XenosTab, "Xenos");
    TabContainer.SetTabTitle((Control) this._window.TacticalMapTab, "Tactical Map");
    TabContainer.SetTabTitle((Control) this._window.FactionsTab, "Factions");
    ((BaseButton) this._window.RefreshButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnRefresh);
    ((BaseWindow) this._window).OpenCentered();
  }

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is RMCAdminEuiState rmcAdminEuiState))
      return;
    ((Control) this._window.CVars).DisposeAllChildren();
    ((Control) this._window.Squads).DisposeAllChildren();
    ((Control) this._window.XenoTiers).DisposeAllChildren();
    ((Control) this._window.TacticalMapHistory).DisposeAllChildren();
    ((Control) this._window.Factions).DisposeAllChildren();
    foreach (string registeredCvar in this._config.GetRegisteredCVars())
    {
      if (registeredCvar.StartsWith("rmc.") && !registeredCvar.Contains("play_voicelines_"))
      {
        BoxContainer cvars = this._window.CVars;
        BoxContainer boxContainer = new BoxContainer();
        boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
        Control.OrderedChildCollection children1 = ((Control) boxContainer).Children;
        HSeparator hseparator = new HSeparator();
        hseparator.Color = Color.FromHex((ReadOnlySpan<char>) "#4972A1", new Color?());
        hseparator.Margin = new Thickness(0.0f, 10f);
        children1.Add((Control) hseparator);
        Control.OrderedChildCollection children2 = ((Control) boxContainer).Children;
        Label label = new Label();
        label.Text = registeredCvar;
        ((Control) label).MinWidth = 50f;
        children2.Add((Control) label);
        ((Control) boxContainer).Children.Add((Control) new Label()
        {
          Text = this._config.GetCVar(registeredCvar).ToString()
        });
        ((Control) cvars).AddChild((Control) boxContainer);
      }
    }
    foreach (Squad squad1 in rmcAdminEuiState.Squads)
    {
      Squad squad = squad1;
      RMCSquadRow rmcSquadRow1 = new RMCSquadRow();
      ((Control) rmcSquadRow1).HorizontalExpand = true;
      ((Control) rmcSquadRow1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
      RMCSquadRow rmcSquadRow2 = rmcSquadRow1;
      ((Control) rmcSquadRow2.AddToSquadButton).Visible = false;
      string name = string.Empty;
      Color color = Color.White;
      EntityPrototype entityPrototype;
      if (this._prototypes.TryIndex(squad.Id, ref entityPrototype))
      {
        name = entityPrototype.Name;
        SquadTeamComponent squadTeamComponent;
        if (entityPrototype.TryGetComponent<SquadTeamComponent>(ref squadTeamComponent, this._compFactory))
          color = squadTeamComponent.Color;
      }
      rmcSquadRow2.CreateSquadButton(squad.Exists, (Action) (() => this.SendMessage((EuiMessageBase) new RMCAdminCreateSquadMsg(squad.Id))), new int?(squad.Members), name, color);
      ((Control) this._window.Squads).AddChild((Control) rmcSquadRow2);
    }
    this._window.MarinesLabel.Text = $"Total marine players alive: {rmcAdminEuiState.Marines}";
    Dictionary<int, int> source = new Dictionary<int, int>();
    foreach (EntityPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<EntityPrototype>())
    {
      XenoComponent xenoComponent;
      if (!enumeratePrototype.Abstract && enumeratePrototype.TryGetComponent<XenoComponent>(ref xenoComponent, this._compFactory))
        Extensions.GetOrNew<int, int>(source, xenoComponent.Tier);
    }
    foreach (Xeno xeno in rmcAdminEuiState.Xenos)
    {
      EntityPrototype entityPrototype;
      XenoComponent xenoComponent;
      if (this._prototypes.TryIndex(xeno.Proto, ref entityPrototype) && entityPrototype.TryGetComponent<XenoComponent>(ref xenoComponent, this._compFactory))
        source[xenoComponent.Tier] = Extensions.GetOrNew<int, int>(source, xenoComponent.Tier) + 1;
    }
    foreach ((int key, int num) in (IEnumerable<KeyValuePair<int, int>>) source.OrderBy<KeyValuePair<int, int>, int>((Func<KeyValuePair<int, int>, int>) (x => x.Key)))
    {
      BoxContainer xenoTiers1 = this._window.XenoTiers;
      Label label = new Label();
      label.Text = $"Tier {key}: {num} xenos";
      ((Control) xenoTiers1).AddChild((Control) label);
      BoxContainer xenoTiers2 = this._window.XenoTiers;
      HSeparator hseparator = new HSeparator();
      hseparator.Color = Color.FromHex((ReadOnlySpan<char>) "#4972A1", new Color?());
      hseparator.Margin = new Thickness(0.0f, 10f);
      ((Control) xenoTiers2).AddChild((Control) hseparator);
    }
    this._window.XenosLabel.Text = $"Total xenonid players alive: {rmcAdminEuiState.Xenos.Count}";
    foreach ((Guid guid, string Actor, int Round) in rmcAdminEuiState.TacticalMapHistory)
    {
      Button button1 = new Button();
      button1.Text = $"Round {Round} by {Actor}";
      Button button2 = button1;
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((EuiMessageBase) new RMCAdminRequestTacticalMapHistory(guid)));
      ((Control) this._window.TacticalMapHistory).AddChild((Control) button2);
    }
    this._window.TacticalMap.Lines.Clear();
    (Guid Id, List<TacticalMapLine> Lines, string str2, int RoundId) = rmcAdminEuiState.TacticalMapLines;
    if (Id == new Guid() && Lines == null && str2 == (string) null && RoundId == 0)
    {
      this._window.TacticalMapLabel.Text = "Selected: None";
    }
    else
    {
      this._window.TacticalMapLabel.Text = $"Selected: Round {rmcAdminEuiState.TacticalMapLines.RoundId} by {rmcAdminEuiState.TacticalMapLines.Actor}";
      this._window.TacticalMap.Texture = Texture.Transparent;
      this._window.TacticalMap.Lines.AddRange((IEnumerable<TacticalMapLine>) rmcAdminEuiState.TacticalMapLines.Lines);
    }
    FactionData factionData2;
    foreach ((str2, factionData2) in rmcAdminEuiState.Factions)
    {
      string left = str2;
      FactionData factionData3 = factionData2;
      BoxContainer boxContainer1 = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 1
      };
      BoxContainer boxContainer2 = boxContainer1;
      Label label1 = new Label();
      label1.Text = left + ":";
      ((Control) label1).MinWidth = 50f;
      ((Control) boxContainer2).AddChild((Control) label1);
      foreach (string key in rmcAdminEuiState.Factions.Keys)
      {
        string right = key;
        if (!(left == right))
        {
          BoxContainer boxContainer3 = new BoxContainer()
          {
            Orientation = (BoxContainer.LayoutOrientation) 0
          };
          ButtonGroup buttonGroup = new ButtonGroup(true);
          Button button3 = new Button();
          button3.Text = "+";
          ((BaseButton) button3).ToggleMode = true;
          ((BaseButton) button3).Pressed = factionData3.Friendly.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(right));
          ((BaseButton) button3).Group = buttonGroup;
          Button button4 = button3;
          ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((EuiMessageBase) new RMCAdminFactionMsg(RMCAdminFactionMsgType.Friendly, left, right)));
          ((Control) boxContainer3).AddChild((Control) button4);
          Button button5 = new Button();
          button5.Text = "=";
          ((BaseButton) button5).ToggleMode = true;
          ((BaseButton) button5).Pressed = !factionData3.Friendly.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(right)) && !factionData3.Hostile.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(right));
          ((BaseButton) button5).Group = buttonGroup;
          Button button6 = button5;
          ((BaseButton) button6).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((EuiMessageBase) new RMCAdminFactionMsg(RMCAdminFactionMsgType.Neutral, left, right)));
          ((Control) boxContainer3).AddChild((Control) button6);
          Button button7 = new Button();
          button7.Text = "-";
          ((BaseButton) button7).ToggleMode = true;
          ((BaseButton) button7).Pressed = factionData3.Hostile.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(right));
          ((BaseButton) button7).Group = buttonGroup;
          Button button8 = button7;
          ((BaseButton) button8).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((EuiMessageBase) new RMCAdminFactionMsg(RMCAdminFactionMsgType.Hostile, left, right)));
          ((Control) boxContainer3).AddChild((Control) button8);
          BoxContainer boxContainer4 = boxContainer3;
          Label label2 = new Label();
          label2.Text = right ?? "";
          ((Control) label2).MinWidth = 50f;
          ((Control) boxContainer4).AddChild((Control) label2);
          ((Control) boxContainer1).AddChild((Control) boxContainer3);
        }
      }
      BoxContainer boxContainer5 = boxContainer1;
      HSeparator hseparator = new HSeparator();
      hseparator.Color = Color.FromHex((ReadOnlySpan<char>) "#4972A1", new Color?());
      hseparator.Margin = new Thickness(0.0f, 10f);
      ((Control) boxContainer5).AddChild((Control) hseparator);
      ((Control) this._window.Factions).AddChild((Control) boxContainer1);
    }
  }

  private void OnRefresh(BaseButton.ButtonEventArgs args)
  {
    this.SendMessage((EuiMessageBase) new RMCAdminRefresh());
  }
}
