// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Overwatch.OverwatchConsoleBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client._RMC14.Overwatch;

public sealed class OverwatchConsoleBui : RMCPopOutBui<OverwatchConsoleWindow>
{
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
  private readonly Dictionary<NetEntity, Dictionary<NetEntity, OverwatchConsoleBui.OverwatchRow>> _rows = new Dictionary<NetEntity, Dictionary<NetEntity, OverwatchConsoleBui.OverwatchRow>>();

  protected override OverwatchConsoleWindow? Window { get; set; }

  public OverwatchConsoleBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._overwatchConsole = this.EntMan.System<OverwatchConsoleSystem>();
    this._squad = this.EntMan.System<SquadSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    if (this.Window != null)
      return;
    this.Window = this.CreatePopOutableWindow<OverwatchConsoleWindow>();
    this.Window.OverwatchHeader.SetMarkupPermissive("[color=#88C7FA]OVERWATCH DISABLED - SELECT SQUAD[/color]");
    if (this.State is OverwatchConsoleBuiState state)
      this.RefreshState(state);
    this.UpdateView();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is OverwatchConsoleBuiState s))
      return;
    this.RefreshState(s);
  }

  private void RefreshState(OverwatchConsoleBuiState s)
  {
    OverwatchConsoleComponent console;
    if (this.Window == null || !this.EntMan.TryGetComponent<OverwatchConsoleComponent>(this.Owner, ref console))
      return;
    List<OverwatchSquad> list = s.Squads.ToList<OverwatchSquad>();
    list.Sort((Comparison<OverwatchSquad>) ((a, b) => string.CompareOrdinal(a.Name, b.Name)));
    foreach ((NetEntity key, PanelContainer panelContainer1) in this._squads)
    {
      NetEntity id = key;
      PanelContainer panelContainer2 = panelContainer1;
      if (list.All<OverwatchSquad>((Func<OverwatchSquad, bool>) (oldSquad => NetEntity.op_Inequality(oldSquad.Id, id))))
        ((Control) panelContainer2).Orphan();
    }
    foreach (OverwatchSquad overwatchSquad in list)
    {
      OverwatchSquad squad = overwatchSquad;
      if (!this._squads.ContainsKey(squad.Id))
      {
        Button button1 = new Button();
        button1.Text = squad.Name.ToUpper();
        ((Control) button1).ModulateSelfOverride = new Color?(squad.Color);
        ((Control) button1).StyleClasses.Add("OpenBoth");
        Button button2 = button1;
        ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleSelectSquadBuiMsg(squad.Id)));
        PanelContainer panel = this.CreatePanel();
        ((Control) panel).AddChild((Control) button2);
        ((Control) this.Window.SquadsContainer).AddChild((Control) panel);
        this._squads[squad.Id] = panel;
      }
    }
    Dictionary<ProtoId<JobPrototype>, int> roleSorting = new Dictionary<ProtoId<JobPrototype>, int>();
    NetEntity? activeSquad = this.GetActiveSquad();
    Thickness thickness;
    // ISSUE: explicit constructor call
    ((Thickness) ref thickness).\u002Ector(2f);
    foreach (OverwatchSquad squad1 in s.Squads)
    {
      OverwatchSquad squad = squad1;
      List<OverwatchMarine> marines;
      if (s.Marines.TryGetValue(squad.Id, out marines))
      {
        marines.Sort((Comparison<OverwatchMarine>) ((a, b) => Sorting(a).CompareTo(Sorting(b))));
        NetEntity? nullable1;
        OverwatchSquadView monitor;
        if (this._squadViews.TryGetValue(squad.Id, out monitor))
        {
          ((Control) monitor.RolesContainer).DisposeAllChildren();
        }
        else
        {
          monitor = new OverwatchSquadView();
          OverwatchSquadView overwatchSquadView = monitor;
          key = squad.Id;
          nullable1 = activeSquad;
          int num = nullable1.HasValue ? (NetEntity.op_Equality(key, nullable1.GetValueOrDefault()) ? 1 : 0) : 0;
          overwatchSquadView.Visible = num != 0;
          ((BaseButton) monitor.TacticalMapButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchViewTacticalMapBuiMsg()));
          ((BaseButton) monitor.OperatorButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleTakeOperatorBuiMsg()));
          monitor.SearchBar.OnTextChanged += (Action<LineEdit.LineEditEventArgs>) (_ => monitor.UpdateResults(console.Location, console.ShowDead, console.ShowHidden, marines, console));
          ((Control) monitor.ShowLocationButton.Label).ModulateSelfOverride = new Color?(Color.Black);
          ((BaseButton) monitor.ShowLocationButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
          {
            OverwatchLocation? nullable2;
            OverwatchLocation? nullable3;
            if (console.Location.HasValue)
            {
              nullable2 = console.Location;
              nullable3 = nullable2.HasValue ? new OverwatchLocation?(nullable2.GetValueOrDefault() + 1) : new OverwatchLocation?();
            }
            else
              nullable3 = new OverwatchLocation?(OverwatchLocation.Min);
            OverwatchLocation? location = nullable3;
            nullable2 = location;
            OverwatchLocation overwatchLocation = OverwatchLocation.Ship;
            if (nullable2.GetValueOrDefault() > overwatchLocation & nullable2.HasValue)
              location = new OverwatchLocation?();
            this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleSetLocationBuiMsg(location));
          });
          ((Control) monitor.ShowDeadButton.Label).ModulateSelfOverride = new Color?(Color.Black);
          ((BaseButton) monitor.ShowDeadButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleShowDeadBuiMsg(!console.ShowDead)));
          ((Control) monitor.ShowHiddenButton.Label).ModulateSelfOverride = new Color?(Color.Black);
          ((BaseButton) monitor.ShowHiddenButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleShowHiddenBuiMsg(!console.ShowHidden)));
          ((Control) monitor.TransferMarineButton.Label).ModulateSelfOverride = new Color?(Color.Black);
          ((BaseButton) monitor.TransferMarineButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleTransferMarineBuiMsg()));
          SupplyDropComputerComponent computerComponent;
          if (this.EntMan.TryGetComponent<SupplyDropComputerComponent>(this.Owner, ref computerComponent))
          {
            monitor.Longitude.Value = (float) computerComponent.Coordinates.X;
            monitor.Latitude.Value = (float) computerComponent.Coordinates.Y;
          }
          monitor.Longitude.OnValueChanged += (Action<FloatSpinBox.FloatSpinBoxEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleSupplyDropLongitudeBuiMsg((int) args.Value)));
          monitor.Latitude.OnValueChanged += (Action<FloatSpinBox.FloatSpinBoxEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleSupplyDropLatitudeBuiMsg((int) args.Value)));
          ((BaseButton) monitor.LaunchButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleSupplyDropLaunchBuiMsg()));
          ((BaseButton) monitor.SaveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleSupplyDropSaveBuiMsg((int) monitor.Longitude.Value, (int) monitor.Latitude.Value)));
          monitor.OrbitalLongitude.Value = (float) console.OrbitalCoordinates.X;
          monitor.OrbitalLatitude.Value = (float) console.OrbitalCoordinates.Y;
          monitor.OrbitalLongitude.OnValueChanged += (Action<FloatSpinBox.FloatSpinBoxEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleOrbitalLongitudeBuiMsg((int) args.Value)));
          monitor.OrbitalLatitude.OnValueChanged += (Action<FloatSpinBox.FloatSpinBoxEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleOrbitalLatitudeBuiMsg((int) args.Value)));
          ((BaseButton) monitor.OrbitalFireButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleOrbitalLaunchBuiMsg()));
          ((BaseButton) monitor.OrbitalSaveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleOrbitalSaveBuiMsg((int) monitor.Longitude.Value, (int) monitor.Latitude.Value)));
          ((BaseButton) monitor.MessageSquadButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_1 =>
          {
            OverwatchTextInputWindow window = new OverwatchTextInputWindow();
            window.MessageBox.OnTextEntered += (Action<LineEdit.LineEditEventArgs>) (_2 => SendSquadMessage());
            ((BaseButton) window.OkButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_3 => SendSquadMessage());
            ((BaseButton) window.CancelButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_4 => ((BaseWindow) window).Close());
            ((BaseWindow) window).OpenCentered();

            void SendSquadMessage()
            {
              this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleSendMessageBuiMsg(window.MessageBox.Text));
              ((BaseWindow) window).Close();
            }
          });
          bool flag = this.EntMan.HasComponent<SupplyDropComputerComponent>(this.Owner) && squad.CanSupplyDrop;
          TabContainer.SetTabVisible((Control) monitor.SupplyDrop, flag);
          OverwatchConsoleComponent consoleComponent;
          if (this.EntMan.TryGetComponent<OverwatchConsoleComponent>(this.Owner, ref consoleComponent))
          {
            TabContainer.SetTabVisible((Control) monitor.OrbitalBombardment, consoleComponent.CanOrbitalBombardment);
            ((Control) monitor.MessageSquadButton).Visible = consoleComponent.CanMessageSquad;
          }
          else
          {
            TabContainer.SetTabVisible((Control) monitor.OrbitalBombardment, false);
            ((Control) monitor.MessageSquadButton).Visible = false;
          }
          this._squadViews[squad.Id] = monitor;
          ((Control) this.Window.SquadViewContainer).AddChild((Control) monitor);
        }
        monitor.OverwatchLabel.Text = squad.Name + " Overwatch | Dashboard";
        monitor.OnStop += (Action) (() => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleStopOverwatchBuiMsg()));
        int num1 = 0;
        Dictionary<ProtoId<JobPrototype>, (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>)> dictionary1 = new Dictionary<ProtoId<JobPrototype>, (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>)>();
        foreach (JobPrototype squadRolePrototype in this._squad.SquadRolePrototypes)
          dictionary1[ProtoId<JobPrototype>.op_Implicit(squadRolePrototype.ID)] = (new HashSet<OverwatchMarine>(), new HashSet<OverwatchMarine>(), new HashSet<OverwatchMarine>());
        HashSet<NetEntity> hashSet = marines.Select<OverwatchMarine, NetEntity>((Func<OverwatchMarine, NetEntity>) (e => e.Id)).ToHashSet<NetEntity>();
        Dictionary<NetEntity, OverwatchConsoleBui.OverwatchRow> orNew1 = Extensions.GetOrNew<NetEntity, Dictionary<NetEntity, OverwatchConsoleBui.OverwatchRow>>(this._rows, squad.Id);
        KeyValuePair<NetEntity, OverwatchConsoleBui.OverwatchRow> keyValuePair1;
        OverwatchConsoleBui.OverwatchRow overwatchRow1;
        foreach (KeyValuePair<NetEntity, OverwatchConsoleBui.OverwatchRow> keyValuePair2 in Extensions.ToArray<NetEntity, OverwatchConsoleBui.OverwatchRow>(orNew1))
        {
          keyValuePair1 = keyValuePair2;
          (key, overwatchRow1) = keyValuePair1;
          NetEntity key = key;
          OverwatchConsoleBui.OverwatchRow overwatchRow2 = overwatchRow1;
          if (!hashSet.Contains(key))
          {
            ((Control) overwatchRow2.Name.Panel).Orphan();
            ((Control) overwatchRow2.Role.Panel).Orphan();
            ((Control) overwatchRow2.State.Panel).Orphan();
            ((Control) overwatchRow2.Location.Panel).Orphan();
            ((Control) overwatchRow2.Distance.Panel).Orphan();
            ((Control) overwatchRow2.Buttons.Container).Orphan();
            this._rows.Remove(key);
          }
        }
        foreach (OverwatchMarine overwatchMarine in marines)
        {
          OverwatchMarine marine = overwatchMarine;
          string str1 = "None";
          string str2 = (string) null;
          ProtoId<JobPrototype>? role1 = marine.Role;
          if (role1.HasValue)
          {
            LocId? roleOverride = marine.RoleOverride;
            string str3;
            if (roleOverride.HasValue && this._localization.TryGetString(LocId.op_Implicit(roleOverride.GetValueOrDefault()), ref str3))
            {
              str1 = str3;
            }
            else
            {
              JobPrototype jobPrototype;
              if (this._prototypes.TryIndex<JobPrototype>(marine.Role, ref jobPrototype))
                str1 = jobPrototype.LocalizedName;
            }
            Dictionary<ProtoId<JobPrototype>, (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>)> dictionary2 = dictionary1;
            role1 = marine.Role;
            ProtoId<JobPrototype> protoId = role1.Value;
            bool flag;
            ref bool local = ref flag;
            (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>) orNew2 = Extensions.GetOrNew<ProtoId<JobPrototype>, (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>)>(dictionary2, protoId, ref local);
            if (!flag)
            {
              orNew2.Item1 = new HashSet<OverwatchMarine>();
              orNew2.Item2 = new HashSet<OverwatchMarine>();
              orNew2.Item3 = new HashSet<OverwatchMarine>();
            }
            if (marine.State == MobState.Alive)
            {
              orNew2.Item2.Add(marine);
              ++num1;
            }
            if (marine.Deployed)
              orNew2.Item1.Add(marine);
            orNew2.Item3.Add(marine);
            Dictionary<ProtoId<JobPrototype>, (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>)> dictionary3 = dictionary1;
            role1 = marine.Role;
            ProtoId<JobPrototype> key = role1.Value;
            (_, _, _) = orNew2;
            (HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>) valueTuple;
            dictionary3[key] = valueTuple;
          }
          RankPrototype rankPrototype;
          if (marine.Rank.HasValue && this._prototypes.TryIndex<RankPrototype>(marine.Rank, ref rankPrototype))
            str2 = rankPrototype.Prefix;
          string str4 = str2 != null ? $"{str2} {marine.Name}" : marine.Name;
          OverwatchConsoleBui.OverwatchRow overwatchRow3;
          if (!orNew1.TryGetValue(marine.Id, out overwatchRow3))
          {
            Button button3 = new Button();
            ((Control) button3).StyleClasses.Add("OpenBoth");
            ((Control) button3).Margin = new Thickness(2f, 0.0f);
            Button button4 = button3;
            ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleWatchBuiMsg(marine.Id)));
            RichTextLabel richTextLabel1 = new RichTextLabel();
            ((Control) button4).AddChild((Control) richTextLabel1);
            PanelContainer panel1 = this.CreatePanel(50f);
            ((Control) button4).Margin = thickness;
            ((Control) panel1).AddChild((Control) button4);
            ((Control) monitor.Names).AddChild((Control) panel1);
            PanelContainer panel2 = this.CreatePanel(50f);
            Label label1 = new Label();
            label1.Text = str1;
            ((Control) label1).Margin = thickness;
            Label label2 = label1;
            ((Control) panel2).AddChild((Control) label2);
            ((Control) monitor.Roles).AddChild((Control) panel2);
            RichTextLabel richTextLabel2 = new RichTextLabel();
            ((Control) richTextLabel2).Margin = thickness;
            RichTextLabel richTextLabel3 = richTextLabel2;
            PanelContainer panel3 = this.CreatePanel(50f);
            ((Control) panel3).AddChild((Control) richTextLabel3);
            ((Control) monitor.States).AddChild((Control) panel3);
            PanelContainer panel4 = this.CreatePanel(50f);
            RichTextLabel richTextLabel4 = new RichTextLabel();
            ((Control) richTextLabel4).Margin = thickness;
            ((Control) richTextLabel4).MaxWidth = 250f;
            RichTextLabel richTextLabel5 = richTextLabel4;
            ((Control) panel4).AddChild((Control) richTextLabel5);
            ((Control) monitor.Locations).AddChild((Control) panel4);
            PanelContainer panel5 = this.CreatePanel(50f);
            Label label3 = new Label();
            ((Control) label3).Margin = thickness;
            Label label4 = label3;
            ((Control) panel5).AddChild((Control) label4);
            ((Control) monitor.Distances).AddChild((Control) panel5);
            Button button5 = new Button();
            ((Control) button5).MaxWidth = 25f;
            ((Control) button5).MaxHeight = 25f;
            ((Control) button5).VerticalAlignment = (Control.VAlignment) 1;
            ((Control) button5).StyleClasses.Add("OpenBoth");
            button5.Text = "-";
            ((Control) button5).ModulateSelfOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#BB1F1D", new Color?()));
            ((Control) button5).ToolTip = "Hide marine";
            Button button6 = button5;
            Button button7 = new Button();
            ((Control) button7).MaxWidth = 25f;
            ((Control) button7).MaxHeight = 25f;
            ((Control) button7).VerticalAlignment = (Control.VAlignment) 1;
            ((Control) button7).StyleClasses.Add("OpenBoth");
            button7.Text = "^";
            ((Control) button7).ModulateSelfOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#229132", new Color?()));
            ((Control) button7).ToolTip = "Promote marine to Squad Leader";
            Button button8 = button7;
            ((BaseButton) button6).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleHideBuiMsg(marine.Id, !this._overwatchConsole.IsHidden(Entity<OverwatchConsoleComponent>.op_Implicit((this.Owner, console)), marine.Id))));
            ((BaseButton) button8).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsolePromoteLeaderBuiMsg(marine.Id, squad.LeaderIcon)));
            PanelContainer panel6 = this.CreatePanel(50f);
            ((Control) button6).Margin = thickness;
            ((Control) panel6).AddChild((Control) button6);
            BoxContainer boxContainer = new BoxContainer()
            {
              Orientation = (BoxContainer.LayoutOrientation) 0
            };
            ((Control) boxContainer).AddChild((Control) panel6);
            PanelContainer panel7 = this.CreatePanel(50f);
            ((Control) button8).Margin = thickness;
            ((Control) panel7).AddChild((Control) button8);
            ((Control) boxContainer).AddChild((Control) panel7);
            ((Control) monitor.Buttons).AddChild((Control) boxContainer);
            overwatchRow3 = new OverwatchConsoleBui.OverwatchRow(marine.Role, (panel1, button4, richTextLabel1), (panel2, label2), (panel3, richTextLabel3), (panel4, richTextLabel5), (panel5, label4), (boxContainer, button6, button8));
            orNew1[marine.Id] = overwatchRow3;
            role1 = marine.Role;
            KeyValuePair<NetEntity, OverwatchConsoleBui.OverwatchRow>? nullable4;
            if (role1.HasValue && Extensions.TryFirstOrNull<KeyValuePair<NetEntity, OverwatchConsoleBui.OverwatchRow>>((IEnumerable<KeyValuePair<NetEntity, OverwatchConsoleBui.OverwatchRow>>) orNew1, (Func<KeyValuePair<NetEntity, OverwatchConsoleBui.OverwatchRow>, bool>) (r =>
            {
              if (!NetEntity.op_Inequality(r.Key, marine.Id))
                return false;
              ProtoId<JobPrototype>? roleId = r.Value.RoleId;
              ProtoId<JobPrototype>? role2 = marine.Role;
              if (roleId.HasValue != role2.HasValue)
                return false;
              return !roleId.HasValue || ProtoId<JobPrototype>.op_Equality(roleId.GetValueOrDefault(), role2.GetValueOrDefault());
            }), ref nullable4))
            {
              keyValuePair1 = nullable4.Value;
              overwatchRow1 = keyValuePair1.Value;
              int num2 = ((Control) overwatchRow1.Name.Panel).GetPositionInParent() + 1;
              ((Control) overwatchRow3.Name.Panel).SetPositionInParent(num2);
              ((Control) overwatchRow3.Role.Panel).SetPositionInParent(num2);
              ((Control) overwatchRow3.State.Panel).SetPositionInParent(num2);
              ((Control) overwatchRow3.Location.Panel).SetPositionInParent(num2);
              ((Control) overwatchRow3.Distance.Panel).SetPositionInParent(num2);
              ((Control) overwatchRow3.Buttons.Container).SetPositionInParent(num2);
            }
          }
          NetEntity camera = marine.Camera;
          key = new NetEntity();
          NetEntity netEntity = key;
          if (NetEntity.op_Equality(camera, netEntity))
          {
            overwatchRow3.Name.Label.SetMarkupPermissive($"[color={"#CED22B"}]{str4} (NO CAMERA)[/color]");
            overwatchRow3.Name.Button.Text = (string) null;
            ((BaseButton) overwatchRow3.Name.Button).Disabled = true;
          }
          else
          {
            overwatchRow3.Name.Label.Text = (string) null;
            overwatchRow3.Name.Button.Text = str4;
            ((BaseButton) overwatchRow3.Name.Button).Disabled = false;
          }
          overwatchRow3.Role.Label.Text = str1;
          (string, string) valueTuple1;
          switch (marine.State)
          {
            case MobState.Critical:
              valueTuple1 = ("Unconscious", "#CED22B");
              break;
            case MobState.Dead:
              valueTuple1 = ("Dead", "#A42625");
              break;
            default:
              valueTuple1 = ("Conscious", "#229132");
              break;
          }
          (string str5, string str6) = valueTuple1;
          if (marine.SSD && marine.State != MobState.Dead)
            str5 += " (SSD)";
          overwatchRow3.State.Label.SetMarkupPermissive($"[color={str6}]{str5}[/color]");
          overwatchRow3.Location.Label.Text = $"[color=white]{marine.AreaName}[/color]";
          string str7 = "N/A";
          Vector2? leaderDistance = marine.LeaderDistance;
          if (leaderDistance.HasValue && !Vector2Helpers.IsLengthZero(leaderDistance.GetValueOrDefault()))
          {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
            ref DefaultInterpolatedStringHandler local1 = ref interpolatedStringHandler;
            leaderDistance = marine.LeaderDistance;
            double num3 = (double) leaderDistance.Value.Length();
            local1.AppendFormatted<float>((float) num3, "F0");
            interpolatedStringHandler.AppendLiteral(" (");
            ref DefaultInterpolatedStringHandler local2 = ref interpolatedStringHandler;
            leaderDistance = marine.LeaderDistance;
            string shorthand = DirectionExtensions.GetDir(leaderDistance.Value).GetShorthand();
            local2.AppendFormatted(shorthand);
            interpolatedStringHandler.AppendLiteral(")");
            str7 = interpolatedStringHandler.ToStringAndClear();
          }
          overwatchRow3.Distance.Label.Text = str7;
          if (this._overwatchConsole.IsHidden(Entity<OverwatchConsoleComponent>.op_Implicit((this.Owner, console)), marine.Id))
          {
            key = marine.Id;
            nullable1 = squad.Leader;
            if ((nullable1.HasValue ? (NetEntity.op_Inequality(key, nullable1.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
            {
              overwatchRow3.Buttons.Hide.Text = "+";
              ((Control) overwatchRow3.Buttons.Hide).ModulateSelfOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#248E34", new Color?()));
              ((Control) overwatchRow3.Buttons.Hide).ToolTip = "Show marine";
              goto label_66;
            }
          }
          overwatchRow3.Buttons.Hide.Text = "-";
          ((Control) overwatchRow3.Buttons.Hide).ModulateSelfOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#BB1F1D", new Color?()));
          ((Control) overwatchRow3.Buttons.Hide).ToolTip = "Hide marine";
label_66:
          nullable1 = squad.Leader;
          key = marine.Id;
          if ((nullable1.HasValue ? (NetEntity.op_Equality(nullable1.GetValueOrDefault(), key) ? 1 : 0) : 0) != 0)
          {
            ((Control) overwatchRow3.Buttons.Hide).Visible = false;
            ((Control) overwatchRow3.Buttons.Promote).Visible = false;
          }
        }
        List<(string, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, bool, int)> valueTupleList = new List<(string, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, bool, int)>();
        foreach ((ProtoId<JobPrototype> key, (HashSet<OverwatchMarine> overwatchMarineSet1, HashSet<OverwatchMarine> overwatchMarineSet2, HashSet<OverwatchMarine> overwatchMarineSet3)) in dictionary1)
        {
          JobPrototype jobPrototype;
          if (this._prototypes.TryIndex<JobPrototype>(key, ref jobPrototype))
          {
            int? overwatchSortPriority = jobPrototype.OverwatchSortPriority;
            if (overwatchSortPriority.HasValue)
            {
              int valueOrDefault = overwatchSortPriority.GetValueOrDefault();
              valueTupleList.Add((jobPrototype.ID, overwatchMarineSet1, overwatchMarineSet2, overwatchMarineSet3, jobPrototype.OverwatchShowName, valueOrDefault));
            }
          }
        }
        valueTupleList.Sort((Comparison<(string, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, HashSet<OverwatchMarine>, bool, int)>) ((a, b) => a.Priority.CompareTo(b.Priority)));
        int num4 = 0;
        BoxContainer boxContainer1 = (BoxContainer) null;
        foreach ((string str, HashSet<OverwatchMarine> overwatchMarineSet4, HashSet<OverwatchMarine> overwatchMarineSet5, HashSet<OverwatchMarine> overwatchMarineSet6, bool flag, int _) in valueTupleList)
        {
          JobPrototype jobPrototype;
          if (this._prototypes.TryIndex<JobPrototype>(str, ref jobPrototype))
          {
            if (num4 % 2 == 0)
            {
              BoxContainer boxContainer2 = new BoxContainer();
              boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 0;
              ((Control) boxContainer2).HorizontalExpand = true;
              boxContainer2.SeparationOverride = new int?(5);
              boxContainer1 = boxContainer2;
              ((Control) monitor.RolesContainer).AddChild((Control) boxContainer1);
            }
            string markup1;
            string markup2;
            if (flag)
            {
              OverwatchMarine? nullable5;
              if (this._squad.IsSquadLeader(ProtoId<JobPrototype>.op_Implicit(str)) && squad.Leader.HasValue && Extensions.TryFirstOrNull<OverwatchMarine>((IEnumerable<OverwatchMarine>) marines, (Func<OverwatchMarine, bool>) (m => NetEntity.op_Equality(m.Id, squad.Leader.Value)), ref nullable5))
              {
                markup1 = nullable5.Value.Name;
                markup2 = nullable5.Value.State == MobState.Dead ? "[bold][color=#A42625]DEAD[/color][/bold]" : "[bold][color=#229132]ALIVE[/color][/bold]";
              }
              else
              {
                OverwatchMarine? nullable6;
                if (Extensions.TryFirstOrNull<OverwatchMarine>((IEnumerable<OverwatchMarine>) overwatchMarineSet6, ref nullable6))
                {
                  markup1 = nullable6.Value.Name;
                  markup2 = nullable6.Value.State == MobState.Dead ? "[bold][color=#A42625]DEAD[/color][/bold]" : "[bold][color=#229132]ALIVE[/color][/bold]";
                }
                else
                {
                  markup1 = "[bold][color=#A42625]NONE[/color][/bold]";
                  markup2 = "[bold][color=#A42625]N/A[/color][/bold]";
                }
              }
            }
            else
            {
              markup1 = $"[bold]{overwatchMarineSet4.Count} DEPLOYED[/bold]";
              markup2 = $"[bold][color={(overwatchMarineSet5.Count > 0 ? "#229132" : "#A42625")}]{overwatchMarineSet5.Count} ALIVE[/color][/bold]";
            }
            RichTextLabel label5 = new RichTextLabel();
            label5.SetMarkupPermissive(markup1);
            RichTextLabel label6 = new RichTextLabel();
            label6.SetMarkupPermissive(markup2);
            PanelContainer panel8 = this.CreatePanel(thickness: new Thickness?(new Thickness(0.0f, 0.0f, 0.0f, 1f)));
            RichTextLabel richTextLabel = new RichTextLabel();
            ((Control) richTextLabel).Margin = new Thickness(0.0f, 3f, 0.0f, 3f);
            RichTextLabel label7 = richTextLabel;
            label7.SetMarkupPermissive($"[bold]{jobPrototype.OverwatchRoleName}[/bold]");
            PanelContainer panelContainer3 = panel8;
            BoxContainer boxContainer3 = new BoxContainer();
            boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
            ((Control) boxContainer3).Children.Add(new Control()
            {
              HorizontalExpand = true
            });
            ((Control) boxContainer3).Children.Add((Control) label7);
            ((Control) boxContainer3).Children.Add(new Control()
            {
              HorizontalExpand = true
            });
            ((Control) boxContainer3).Margin = thickness;
            ((Control) panelContainer3).AddChild((Control) boxContainer3);
            PanelContainer panel9 = this.CreatePanel();
            ((Control) panel9).HorizontalExpand = true;
            PanelContainer panelContainer4 = panel9;
            BoxContainer boxContainer4 = new BoxContainer();
            boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 1;
            ((Control) boxContainer4).HorizontalExpand = true;
            ((Control) boxContainer4).Children.Add((Control) panel8);
            Control.OrderedChildCollection children1 = ((Control) boxContainer4).Children;
            BoxContainer boxContainer5 = new BoxContainer();
            boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 0;
            ((Control) boxContainer5).Children.Add(new Control()
            {
              HorizontalExpand = true
            });
            ((Control) boxContainer5).Children.Add((Control) label5);
            ((Control) boxContainer5).Children.Add(new Control()
            {
              HorizontalExpand = true
            });
            children1.Add((Control) boxContainer5);
            Control.OrderedChildCollection children2 = ((Control) boxContainer4).Children;
            BoxContainer boxContainer6 = new BoxContainer();
            boxContainer6.Orientation = (BoxContainer.LayoutOrientation) 0;
            ((Control) boxContainer6).Children.Add(new Control()
            {
              HorizontalExpand = true
            });
            ((Control) boxContainer6).Children.Add((Control) label6);
            ((Control) boxContainer6).Children.Add(new Control()
            {
              HorizontalExpand = true
            });
            children2.Add((Control) boxContainer6);
            ((Control) panelContainer4).AddChild((Control) boxContainer4);
            ++num4;
            ((Control) boxContainer1)?.AddChild((Control) panel9);
          }
        }
        string markup = $"[bold][color={(num1 > 0 ? "#229132" : "#A42625")}]{num1} ALIVE[/color][/bold]";
        RichTextLabel label8 = new RichTextLabel();
        label8.SetMarkupPermissive(markup);
        PanelContainer panel10 = this.CreatePanel();
        ((Control) panel10).HorizontalExpand = true;
        PanelContainer panel11 = this.CreatePanel(thickness: new Thickness?(new Thickness(0.0f, 0.0f, 0.0f, 1f)));
        RichTextLabel label9 = new RichTextLabel();
        label9.SetMarkupPermissive("[bold]Total/Living[/bold]");
        PanelContainer panelContainer5 = panel11;
        BoxContainer boxContainer7 = new BoxContainer();
        boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 0;
        ((Control) boxContainer7).Children.Add(new Control()
        {
          HorizontalExpand = true
        });
        ((Control) boxContainer7).Children.Add((Control) label9);
        ((Control) boxContainer7).Children.Add(new Control()
        {
          HorizontalExpand = true
        });
        ((Control) boxContainer7).Margin = thickness;
        ((Control) panelContainer5).AddChild((Control) boxContainer7);
        RichTextLabel label10 = new RichTextLabel();
        label10.SetMarkupPermissive($"[bold]{marines.Count} TOTAL[/bold]");
        PanelContainer panelContainer6 = panel10;
        BoxContainer boxContainer8 = new BoxContainer();
        boxContainer8.Orientation = (BoxContainer.LayoutOrientation) 1;
        ((Control) boxContainer8).HorizontalExpand = true;
        ((Control) boxContainer8).Children.Add((Control) panel11);
        Control.OrderedChildCollection children3 = ((Control) boxContainer8).Children;
        BoxContainer boxContainer9 = new BoxContainer();
        boxContainer9.Orientation = (BoxContainer.LayoutOrientation) 0;
        ((Control) boxContainer9).Children.Add(new Control()
        {
          HorizontalExpand = true
        });
        ((Control) boxContainer9).Children.Add((Control) label10);
        ((Control) boxContainer9).Children.Add(new Control()
        {
          HorizontalExpand = true
        });
        children3.Add((Control) boxContainer9);
        Control.OrderedChildCollection children4 = ((Control) boxContainer8).Children;
        BoxContainer boxContainer10 = new BoxContainer();
        boxContainer10.Orientation = (BoxContainer.LayoutOrientation) 0;
        ((Control) boxContainer10).Children.Add(new Control()
        {
          HorizontalExpand = true
        });
        ((Control) boxContainer10).Children.Add((Control) label8);
        ((Control) boxContainer10).Children.Add(new Control()
        {
          HorizontalExpand = true
        });
        children4.Add((Control) boxContainer10);
        ((Control) panelContainer6).AddChild((Control) boxContainer8);
        ((Control) monitor.RolesContainer).AddChild((Control) panel10);
        monitor.UpdateResults(console.Location, console.ShowDead, console.ShowHidden, marines, console);
      }

      int Sorting(OverwatchMarine marine)
      {
        NetEntity? leader = squad.Leader;
        NetEntity id = marine.Id;
        if ((leader.HasValue ? (NetEntity.op_Equality(leader.GetValueOrDefault(), id) ? 1 : 0) : 0) != 0)
          return 1000;
        ProtoId<JobPrototype>? role = marine.Role;
        if (!role.HasValue)
          return 0;
        ProtoId<JobPrototype> valueOrDefault1 = role.GetValueOrDefault();
        int num;
        if (roleSorting.TryGetValue(valueOrDefault1, out num))
          return num;
        JobPrototype jobPrototype;
        if (this._prototypes.TryIndex<JobPrototype>(valueOrDefault1, ref jobPrototype))
        {
          int? overwatchSortPriority = jobPrototype.OverwatchSortPriority;
          if (overwatchSortPriority.HasValue)
          {
            int valueOrDefault2 = overwatchSortPriority.GetValueOrDefault();
            roleSorting[valueOrDefault1] = valueOrDefault2;
            return valueOrDefault2;
          }
        }
        return 0;
      }
    }
    this.UpdateView();
  }

  private void UpdateView()
  {
    OverwatchConsoleComponent consoleComponent;
    if (this.Window == null || !this.EntMan.TryGetComponent<OverwatchConsoleComponent>(this.Owner, ref consoleComponent))
      return;
    SupplyDropComputerComponent componentOrNull = EntityManagerExt.GetComponentOrNull<SupplyDropComputerComponent>(this.EntMan, this.Owner);
    NetEntity? activeSquad = this.GetActiveSquad();
    if (!activeSquad.HasValue)
    {
      ((Control) this.Window.OverwatchViewContainer).Visible = true;
      ((Control) this.Window.SquadViewContainer).Visible = false;
      this.Window.Wrapper.VerticalAlignment = (Control.VAlignment) 1;
    }
    else
    {
      ((Control) this.Window.OverwatchViewContainer).Visible = false;
      ((Control) this.Window.SquadViewContainer).Visible = true;
      this.Window.Wrapper.VerticalAlignment = (Control.VAlignment) 0;
    }
    string str1 = this.GetOperator();
    foreach ((NetEntity key, OverwatchSquadView overwatchSquadView1) in this._squadViews)
    {
      NetEntity netEntity = key;
      OverwatchSquadView squad = overwatchSquadView1;
      OverwatchSquadView overwatchSquadView2 = squad;
      key = netEntity;
      NetEntity? nullable = activeSquad;
      int num = nullable.HasValue ? (NetEntity.op_Equality(key, nullable.GetValueOrDefault()) ? 1 : 0) : 0;
      overwatchSquadView2.Visible = num != 0;
      squad.OperatorButton.Text = str1 == null ? string.Empty : "Operator - " + str1;
      Button showLocationButton = squad.ShowLocationButton;
      OverwatchLocation? location1 = consoleComponent.Location;
      string str2;
      if (location1.HasValue)
      {
        switch (location1.GetValueOrDefault())
        {
          case OverwatchLocation.Min:
            str2 = "Shown: planetside";
            goto label_12;
          case OverwatchLocation.Ship:
            str2 = "Shown: shipside";
            goto label_12;
        }
      }
      str2 = "Shown: all";
label_12:
      showLocationButton.Text = str2;
      squad.ShowDeadButton.Text = consoleComponent.ShowDead ? "Hide dead" : "Show dead";
      squad.ShowHiddenButton.Text = consoleComponent.ShowHidden ? "Hide hidden" : "Show hidden";
      Thickness margin;
      // ISSUE: explicit constructor call
      ((Thickness) ref margin).\u002Ector(2f);
      if (componentOrNull != null)
      {
        squad.HasCrate = componentOrNull.HasCrate;
        squad.NextLaunchAt = componentOrNull.NextLaunchAt;
        this.AddSaving(squad.Longitudes, squad.Latitudes, squad.Comments, squad.Saves, margin);
        this.AddSavedLocation(consoleComponent.SavedLocations, margin, squad.Longitudes, squad.Latitudes, squad.Comments, squad.Saves, (Action<OverwatchSavedLocation>) (location =>
        {
          squad.Longitude.Value = (float) location.Longitude;
          squad.Latitude.Value = (float) location.Latitude;
          this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleSupplyDropLongitudeBuiMsg(location.Longitude));
          this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleSupplyDropLatitudeBuiMsg(location.Latitude));
        }));
      }
      this.AddSaving(squad.OrbitalLongitudes, squad.OrbitalLatitudes, squad.OrbitalComments, squad.OrbitalSaves, margin);
      this.AddSavedLocation(consoleComponent.SavedLocations, margin, squad.OrbitalLongitudes, squad.OrbitalLatitudes, squad.OrbitalComments, squad.OrbitalSaves, (Action<OverwatchSavedLocation>) (location =>
      {
        squad.Longitude.Value = (float) location.Longitude;
        squad.Latitude.Value = (float) location.Latitude;
        this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleOrbitalLongitudeBuiMsg(location.Longitude));
        this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleOrbitalLatitudeBuiMsg(location.Latitude));
      }));
      squad.HasOrbital = consoleComponent.HasOrbital;
      squad.NextOrbitalAt = consoleComponent.NextOrbitalLaunch;
    }
  }

  private void AddSaving(
    BoxContainer longitudes,
    BoxContainer latitudes,
    BoxContainer comments,
    BoxContainer saves,
    Thickness margin)
  {
    ((Control) longitudes).DisposeAllChildren();
    PanelContainer panel1 = this.CreatePanel(50f);
    PanelContainer panelContainer1 = panel1;
    Label label1 = new Label();
    label1.Text = "LONG.";
    ((Control) label1).Margin = margin;
    ((Control) panelContainer1).AddChild((Control) label1);
    ((Control) longitudes).AddChild((Control) panel1);
    ((Control) latitudes).DisposeAllChildren();
    PanelContainer panel2 = this.CreatePanel(50f);
    PanelContainer panelContainer2 = panel2;
    Label label2 = new Label();
    label2.Text = "LAT.";
    ((Control) label2).Margin = margin;
    ((Control) panelContainer2).AddChild((Control) label2);
    ((Control) latitudes).AddChild((Control) panel2);
    ((Control) comments).DisposeAllChildren();
    PanelContainer panel3 = this.CreatePanel(50f);
    PanelContainer panelContainer3 = panel3;
    Label label3 = new Label();
    label3.Text = "COMMENT";
    ((Control) label3).Margin = margin;
    ((Control) panelContainer3).AddChild((Control) label3);
    ((Control) comments).AddChild((Control) panel3);
    ((Control) saves).DisposeAllChildren();
    PanelContainer panel4 = this.CreatePanel(50f);
    PanelContainer panelContainer4 = panel4;
    Label label4 = new Label();
    label4.Text = " ";
    ((Control) label4).Margin = margin;
    ((Control) panelContainer4).AddChild((Control) label4);
    ((Control) saves).AddChild((Control) panel4);
  }

  private void AddSavedLocation(
    OverwatchSavedLocation?[] locations,
    Thickness margin,
    BoxContainer longitudes,
    BoxContainer latitudes,
    BoxContainer comments,
    BoxContainer saves,
    Action<OverwatchSavedLocation> onSave)
  {
    for (int index1 = 0; index1 < locations.Length; ++index1)
    {
      OverwatchSavedLocation? location1 = locations[index1];
      if (location1.HasValue)
      {
        OverwatchSavedLocation location = location1.GetValueOrDefault();
        PanelContainer panel1 = this.CreatePanel(50f);
        PanelContainer panelContainer1 = panel1;
        Label label1 = new Label();
        label1.Text = $"{location.Longitude}";
        ((Control) label1).Margin = margin;
        ((Control) panelContainer1).AddChild((Control) label1);
        ((Control) longitudes).AddChild((Control) panel1);
        PanelContainer panel2 = this.CreatePanel(50f);
        PanelContainer panelContainer2 = panel2;
        Label label2 = new Label();
        label2.Text = $"{location.Latitude}";
        ((Control) label2).Margin = margin;
        ((Control) panelContainer2).AddChild((Control) label2);
        ((Control) latitudes).AddChild((Control) panel2);
        LineEdit lineEdit = new LineEdit()
        {
          Text = location.Comment ?? ""
        };
        int index = index1;
        lineEdit.OnTextEntered += (Action<LineEdit.LineEditEventArgs>) (args => this.SaveComment(index, args.Text));
        PanelContainer panel3 = this.CreatePanel(50f);
        ((Control) panel3).AddChild((Control) lineEdit);
        ((Control) comments).AddChild((Control) panel3);
        PanelContainer panel4 = this.CreatePanel(50f);
        Button button1 = new Button();
        ((Control) button1).MaxWidth = 25f;
        ((Control) button1).MaxHeight = 25f;
        ((Control) button1).VerticalAlignment = (Control.VAlignment) 1;
        ((Control) button1).StyleClasses.Add("OpenBoth");
        button1.Text = "<";
        ((Control) button1).ModulateSelfOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#D3B400", new Color?()));
        ((Control) button1).ToolTip = "Save Comment";
        Button button2 = button1;
        ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => onSave(location));
        ((Control) panel4).AddChild((Control) button2);
        ((Control) saves).AddChild((Control) panel4);
      }
    }
  }

  private PanelContainer CreatePanel(float minHeight = 0.0f, Thickness? thickness = null)
  {
    Thickness valueOrDefault = thickness.GetValueOrDefault();
    if (!thickness.HasValue)
    {
      // ISSUE: explicit constructor call
      ((Thickness) ref valueOrDefault).\u002Ector(1f);
      thickness = new Thickness?(valueOrDefault);
    }
    PanelContainer panel = new PanelContainer()
    {
      PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BorderColor = Color.FromHex((ReadOnlySpan<char>) "#88C7FA", new Color?()),
        BorderThickness = thickness.Value
      }
    };
    if ((double) minHeight > 0.0)
      ((Control) panel).MinHeight = minHeight;
    return panel;
  }

  private NetEntity? GetActiveSquad()
  {
    OverwatchConsoleComponent consoleComponent;
    return !this.EntMan.TryGetComponent<OverwatchConsoleComponent>(this.Owner, ref consoleComponent) ? new NetEntity?() : consoleComponent.Squad;
  }

  private string? GetOperator()
  {
    OverwatchConsoleComponent consoleComponent;
    return !this.EntMan.TryGetComponent<OverwatchConsoleComponent>(this.Owner, ref consoleComponent) ? (string) null : consoleComponent.Operator;
  }

  private void SaveComment(int index, string text)
  {
    if (text.Length > 50)
      text = text.Substring(0, 50);
    this.SendPredictedMessage((BoundUserInterfaceMessage) new OverwatchConsoleLocationCommentBuiMsg(index, text));
  }

  public void Refresh()
  {
    if (!(this.State is OverwatchConsoleBuiState state))
      return;
    this.RefreshState(state);
  }

  private readonly record struct OverwatchRow(
    ProtoId<JobPrototype>? RoleId,
    (PanelContainer Panel, Button Button, RichTextLabel Label) Name,
    (PanelContainer Panel, Label Label) Role,
    (PanelContainer Panel, RichTextLabel Label) State,
    (PanelContainer Panel, RichTextLabel Label) Location,
    (PanelContainer Panel, Label Label) Distance,
    (BoxContainer Container, Button Hide, Button Promote) Buttons)
  ;
}
