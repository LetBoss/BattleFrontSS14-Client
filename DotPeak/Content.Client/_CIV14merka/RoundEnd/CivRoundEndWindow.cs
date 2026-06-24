// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.RoundEnd.CivRoundEndWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Stats;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

#nullable enable
namespace Content.Client._CIV14merka.RoundEnd;

public sealed class CivRoundEndWindow : DefaultWindow
{
  private readonly IPrototypeManager _proto;
  private readonly BoxContainer _summaryContainer;
  private readonly BoxContainer _myStatsContainer;
  private readonly BoxContainer _teamEntriesContainer;

  public CivRoundEndWindow()
  {
    this._proto = IoCManager.Resolve<IPrototypeManager>();
    this.Title = Loc.GetString("civ-ui-roundend-title");
    ((BaseWindow) this).Resizable = false;
    ((Control) this).MinSize = new Vector2(960f, 720f);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).Margin = new Thickness(10f);
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    TabContainer tabContainer1 = new TabContainer();
    ((Control) tabContainer1).VerticalExpand = true;
    ((Control) tabContainer1).HorizontalExpand = true;
    TabContainer tabContainer2 = tabContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer3.SeparationOverride = new int?(10);
    ((Control) boxContainer3).VerticalExpand = true;
    ((Control) boxContainer3).HorizontalExpand = true;
    this._summaryContainer = boxContainer3;
    ((Control) tabContainer2).AddChild(CivRoundEndWindow.WrapScrollTab((Control) this._summaryContainer, Loc.GetString("civ-ui-roundend-tab-summary")));
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer4.SeparationOverride = new int?(10);
    ((Control) boxContainer4).VerticalExpand = true;
    ((Control) boxContainer4).HorizontalExpand = true;
    this._myStatsContainer = boxContainer4;
    ((Control) tabContainer2).AddChild(CivRoundEndWindow.WrapScrollTab((Control) this._myStatsContainer, Loc.GetString("civ-ui-roundend-tab-mystats")));
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer5).Name = Loc.GetString("civ-ui-roundend-tab-manifest");
    boxContainer5.SeparationOverride = new int?(6);
    BoxContainer boxContainer6 = boxContainer5;
    ((Control) boxContainer6).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-ui-roundend-manifest-subtitle"),
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#B7C2D8", new Color?()))
    });
    ((Control) boxContainer6).AddChild(this.BuildHeaderRow());
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).VerticalExpand = true;
    ((Control) scrollContainer1).HorizontalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer7.SeparationOverride = new int?(8);
    ((Control) boxContainer7).VerticalExpand = true;
    ((Control) boxContainer7).HorizontalExpand = true;
    this._teamEntriesContainer = boxContainer7;
    ((Control) scrollContainer2).AddChild((Control) this._teamEntriesContainer);
    ((Control) boxContainer6).AddChild((Control) scrollContainer2);
    ((Control) tabContainer2).AddChild((Control) boxContainer6);
    ((Control) boxContainer2).AddChild((Control) tabContainer2);
    Button button1 = new Button();
    button1.Text = Loc.GetString("civ-ui-roundend-close");
    ((Control) button1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) button1).MinSize = new Vector2(160f, 34f);
    Button button2 = button1;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => ((BaseWindow) this).Close());
    ((Control) boxContainer2).AddChild((Control) button2);
    this.Contents.AddChild((Control) boxContainer2);
  }

  private static Control WrapScrollTab(Control content, string tabName)
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer).Name = tabName;
    ((Control) boxContainer).VerticalExpand = true;
    ((Control) boxContainer).HorizontalExpand = true;
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).VerticalExpand = true;
    ((Control) scrollContainer1).HorizontalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    ((Control) scrollContainer2).AddChild(content);
    ((Control) boxContainer).AddChild((Control) scrollContainer2);
    return (Control) boxContainer;
  }

  public void SetTitleText(string title)
  {
    this.Title = string.IsNullOrWhiteSpace(title) ? Loc.GetString("civ-ui-roundend-title") : title;
  }

  public void SetSummary(CivRoundEndSummary summary)
  {
    ((Control) this._summaryContainer).RemoveAllChildren();
    ((Control) this._summaryContainer).AddChild(this.BuildSummaryHero(summary));
    if (summary.Sides.Count > 0)
      ((Control) this._summaryContainer).AddChild(this.BuildSidesRow(summary.Sides));
    Control[] controlArray = new Control[4]
    {
      this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-card-mode"), summary.ModeText),
      this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-card-map"), summary.MapText),
      this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-card-duration"), summary.DurationText),
      null
    };
    string title = Loc.GetString("civ-ui-roundend-card-lobby-return");
    string str;
    if (summary.LobbyReturnSeconds <= 0)
      str = Loc.GetString("civ-ui-roundend-lobby-return-instant");
    else
      str = Loc.GetString("civ-ui-roundend-lobby-return-seconds", new (string, object)[1]
      {
        ("seconds", (object) summary.LobbyReturnSeconds)
      });
    controlArray[3] = this.BuildSummaryCard(title, str);
    ((Control) this._summaryContainer).AddChild((Control) this.BuildRow(controlArray));
    if (summary.TopAwards.Count <= 0)
      return;
    ((Control) this._summaryContainer).AddChild(this.BuildTopAwardsPanel(summary.TopAwards));
  }

  private Control BuildSidesRow(List<CivRoundEndSideInfo> sides)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(12);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    foreach (CivRoundEndSideInfo side in sides)
      ((Control) boxContainer2).AddChild(this.BuildSideCard(side));
    return (Control) boxContainer2;
  }

  private Control BuildSideCard(CivRoundEndSideInfo side)
  {
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).HorizontalExpand = true;
    panelContainer1.PanelOverride = (StyleBox) this.BuildPanelStyle(side.IsWinner ? "#1E2A1BF4" : "#141B2BF4", side.IsWinner ? "#9BDE7E" : "#5F7198");
    PanelContainer panelContainer2 = panelContainer1;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(6);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label1 = new Label();
    label1.Text = side.TeamName;
    label1.FontColorOverride = new Color?(side.IsWinner ? Color.FromHex((ReadOnlySpan<char>) "#BCF29A", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#E2E8F4", new Color?()));
    ((Control) label1).StyleClasses.Add("LabelHeading");
    ((Control) boxContainer3).AddChild((Control) label1);
    if (side.IsWinner)
      ((Control) boxContainer2).AddChild((Control) new Label()
      {
        Text = Loc.GetString("civ-ui-roundend-side-winner"),
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#9BDE7E", new Color?()))
      });
    if (side.HasScore)
    {
      BoxContainer boxContainer4 = boxContainer2;
      Label label2 = new Label();
      label2.Text = Loc.GetString("civ-ui-roundend-side-score", new (string, object)[1]
      {
        ("score", (object) side.Score)
      });
      label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFD05C", new Color?()));
      ((Control) label2).StyleClasses.Add("LabelHeading");
      Label label3 = label2;
      ((Control) boxContainer4).AddChild((Control) label3);
    }
    BoxContainer boxContainer5 = boxContainer2;
    Label label4 = new Label();
    Label label5 = label4;
    string str;
    if (!string.IsNullOrWhiteSpace(side.CommanderName))
      str = Loc.GetString("civ-ui-roundend-side-commander", new (string, object)[1]
      {
        ("name", (object) side.CommanderName)
      });
    else
      str = Loc.GetString("civ-ui-roundend-side-no-commander");
    label5.Text = str;
    label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFE39A", new Color?()));
    Label label6 = label4;
    ((Control) boxContainer5).AddChild((Control) label6);
    ((Control) panelContainer2).AddChild((Control) boxContainer2);
    return (Control) panelContainer2;
  }

  private Control BuildTopAwardsPanel(List<CivRoundTopAward> awards)
  {
    PanelContainer panelContainer = new PanelContainer()
    {
      PanelOverride = (StyleBox) this.BuildPanelStyle("#1B1F2CEE", "#5F7198")
    };
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(6);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label1 = new Label();
    label1.Text = Loc.GetString("civ-ui-roundend-top-awards");
    label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFD05C", new Color?()));
    ((Control) label1).StyleClasses.Add("LabelHeading");
    ((Control) boxContainer3).AddChild((Control) label1);
    foreach (CivRoundTopAward award in awards)
    {
      BoxContainer boxContainer4 = new BoxContainer();
      boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 0;
      boxContainer4.SeparationOverride = new int?(10);
      ((Control) boxContainer4).HorizontalExpand = true;
      BoxContainer boxContainer5 = boxContainer4;
      BoxContainer boxContainer6 = boxContainer5;
      Label label2 = new Label();
      label2.Text = award.Title;
      ((Control) label2).MinWidth = 200f;
      label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFE39A", new Color?()));
      ((Control) boxContainer6).AddChild((Control) label2);
      BoxContainer boxContainer7 = boxContainer5;
      Label label3 = new Label();
      label3.Text = award.PlayerName;
      ((Control) label3).MinWidth = 200f;
      ((Control) label3).HorizontalExpand = true;
      label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#E2E8F4", new Color?()));
      ((Control) boxContainer7).AddChild((Control) label3);
      ((Control) boxContainer5).AddChild((Control) new Label()
      {
        Text = award.ValueText,
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#8DC8FF", new Color?()))
      });
      ((Control) boxContainer2).AddChild((Control) boxContainer5);
    }
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  private Control BuildSummaryHero(CivRoundEndSummary summary)
  {
    PanelContainer panelContainer = new PanelContainer();
    panelContainer.PanelOverride = (StyleBox) this.BuildPanelStyle("#141B2BF4", "#5F7198");
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(8);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label = new Label();
    label.Text = summary.WinnerText;
    label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFD05C", new Color?()));
    ((Control) label).StyleClasses.Add("LabelHeading");
    ((Control) boxContainer3).AddChild((Control) label);
    ((Control) boxContainer2).AddChild((Control) new Label()
    {
      Text = summary.ReasonText,
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#E2E8F4", new Color?()))
    });
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  public void SetMyStats(NetUserId? localUser, List<CivRoundEndTeamEntry> entries)
  {
    ((Control) this._myStatsContainer).RemoveAllChildren();
    CivRoundEndPlayerEntry localEntry = CivRoundEndWindow.FindLocalEntry(localUser, entries);
    CivPlayerRoundStats stats = localEntry?.Stats;
    if (stats == null)
    {
      ((Control) this._myStatsContainer).AddChild((Control) new Label()
      {
        Text = Loc.GetString("civ-ui-roundend-my-none"),
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A3AEBD", new Color?()))
      });
    }
    else
    {
      ((Control) this._myStatsContainer).AddChild(this.BuildMyHero(localEntry, stats));
      if (localEntry.IsCommander)
        this.BuildCommanderSections(stats);
      else
        this.BuildSoldierSections(stats);
    }
  }

  private static CivRoundEndPlayerEntry? FindLocalEntry(
    NetUserId? localUser,
    List<CivRoundEndTeamEntry> entries)
  {
    if (!localUser.HasValue)
      return (CivRoundEndPlayerEntry) null;
    NetUserId valueOrDefault = localUser.GetValueOrDefault();
    foreach (CivRoundEndTeamEntry entry in entries)
    {
      foreach (CivRoundEndPlayerEntry player in entry.Players)
      {
        CivPlayerRoundStats stats = player.Stats;
        if ((stats != null ? (NetUserId.op_Equality(stats.UserId, valueOrDefault) ? 1 : 0) : 0) != 0)
          return player;
      }
    }
    return (CivRoundEndPlayerEntry) null;
  }

  private Control BuildMyHero(CivRoundEndPlayerEntry me, CivPlayerRoundStats stats)
  {
    PanelContainer panelContainer = new PanelContainer()
    {
      PanelOverride = (StyleBox) this.BuildPanelStyle("#141B2BF4", "#5F7198")
    };
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(6);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label = new Label();
    label.Text = me.PlayerName;
    label.FontColorOverride = new Color?(me.IsCommander ? Color.FromHex((ReadOnlySpan<char>) "#FFE39A", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#F4F7FB", new Color?()));
    ((Control) label).StyleClasses.Add("LabelHeading");
    ((Control) boxContainer3).AddChild((Control) label);
    string str = me.IsCommander ? Loc.GetString("civ-ui-roundend-my-role-commander") : me.RoleText;
    ((Control) boxContainer2).AddChild((Control) new Label()
    {
      Text = (string.IsNullOrWhiteSpace(me.SquadText) ? str : $"{str} · {me.SquadText}"),
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#B7C2D8", new Color?()))
    });
    if (stats.Awards.Count > 0)
      ((Control) boxContainer2).AddChild((Control) new Label()
      {
        Text = Loc.GetString("civ-ui-roundend-my-awards", new (string, object)[1]
        {
          ("awards", (object) string.Join(", ", (IEnumerable<string>) stats.Awards))
        }),
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFD05C", new Color?()))
      });
    BoxContainer boxContainer4 = boxContainer2;
    BoxContainer boxContainer5;
    if (!me.IsCommander)
      boxContainer5 = this.BuildRow(this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-score"), stats.Score.ToString()), this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-kd"), $"{stats.Kills}/{stats.Deaths}"), this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-kdr"), CivRoundEndWindow.Kdr(stats)), this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-damage"), stats.DamageDealt.ToString()), this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-survival"), CivRoundEndWindow.FormatDuration(stats.SurvivalTime)));
    else
      boxContainer5 = this.BuildRow(this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-score"), stats.Score.ToString()), this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-approvals"), stats.CommanderPurchasesApproved.ToString()), this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-spent"), (stats.CommanderPointsSpent + stats.CommanderShopSpent).ToString()), this.BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-firekills"), (stats.AirstrikeKills + stats.ArtilleryKills).ToString()));
    ((Control) boxContainer4).AddChild((Control) boxContainer5);
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  private void BuildSoldierSections(CivPlayerRoundStats stats)
  {
    List<string> lines = new List<string>();
    lines.Add(Loc.GetString("civ-ui-roundend-my-combat", new (string, object)[3]
    {
      ("kills", (object) stats.Kills),
      ("deaths", (object) stats.Deaths),
      ("kdr", (object) CivRoundEndWindow.Kdr(stats))
    }));
    lines.Add(Loc.GetString("civ-ui-roundend-my-combat2", new (string, object)[3]
    {
      ("streak", (object) stats.BestKillstreak),
      ("multi", (object) stats.BestMultikill),
      ("acc", (object) CivRoundEndWindow.Accuracy(stats))
    }));
    List<string> stringList = lines;
    (string, object)[] valueTupleArray = new (string, object)[3];
    valueTupleArray[0] = ("dealt", (object) stats.DamageDealt);
    valueTupleArray[1] = ("taken", (object) stats.DamageTaken);
    string str1;
    if (stats.TeamKills <= 0)
      str1 = "";
    else
      str1 = Loc.GetString("civ-ui-roundend-my-combat-tk", new (string, object)[1]
      {
        ("count", (object) stats.TeamKills)
      });
    valueTupleArray[2] = ("tk", (object) str1);
    string str2 = Loc.GetString("civ-ui-roundend-my-combat3", valueTupleArray);
    stringList.Add(str2);
    ((Control) this._myStatsContainer).AddChild(this.BuildSection("civ-ui-roundend-my-sec-combat", (IEnumerable<string>) lines));
    ((Control) this._myStatsContainer).AddChild(this.BuildSection("civ-ui-roundend-my-sec-weapons", (IEnumerable<string>) this.BuildWeaponLines(stats)));
    ((Control) this._myStatsContainer).AddChild(this.BuildSection("civ-ui-roundend-my-sec-killlog", (IEnumerable<string>) this.BuildKillLogLines(stats)));
    if (stats.PointsCaptured > 0 || stats.PointsRecaptured > 0 || stats.PointHoldSeconds > 0 || stats.PointsDefendedContests > 0)
      ((Control) this._myStatsContainer).AddChild(this.BuildSection("civ-ui-roundend-my-sec-objectives", (IEnumerable<string>) new string[1]
      {
        Loc.GetString("civ-ui-roundend-my-obj", new (string, object)[4]
        {
          ("captured", (object) stats.PointsCaptured),
          ("recaptured", (object) stats.PointsRecaptured),
          ("held", (object) stats.PointHoldSeconds),
          ("defended", (object) stats.PointsDefendedContests)
        })
      }));
    if (stats.HealsApplied > 0 || stats.RevivesApplied > 0 || stats.MinesPlaced > 0 || stats.MineKillsConfirmed > 0 || stats.MortarHits > 0 || stats.MortarShellsFired > 0)
      ((Control) this._myStatsContainer).AddChild(this.BuildSection("civ-ui-roundend-my-sec-support", (IEnumerable<string>) new string[1]
      {
        Loc.GetString("civ-ui-roundend-my-support", new (string, object)[8]
        {
          ("heals", (object) stats.HealsApplied),
          ("hp", (object) stats.HealingDone),
          ("revives", (object) stats.RevivesApplied),
          ("mines", (object) stats.MinesPlaced),
          ("minehits", (object) stats.MineKillsEnemies),
          ("minekills", (object) stats.MineKillsConfirmed),
          ("mortarhits", (object) stats.MortarHits),
          ("mortarfired", (object) stats.MortarShellsFired)
        })
      }));
    if (stats.VehicleTimeSeconds <= 0 && stats.VehicleKills <= 0 && stats.VehiclesDestroyed <= 0 && stats.Roadkills <= 0)
      return;
    ((Control) this._myStatsContainer).AddChild(this.BuildSection("civ-ui-roundend-my-sec-vehicle", (IEnumerable<string>) new string[1]
    {
      Loc.GetString("civ-ui-roundend-my-vehicle", new (string, object)[4]
      {
        ("time", (object) CivRoundEndWindow.FormatDuration(TimeSpan.FromSeconds((long) stats.VehicleTimeSeconds))),
        ("kills", (object) stats.VehicleKills),
        ("destroyed", (object) stats.VehiclesDestroyed),
        ("roadkills", (object) stats.Roadkills)
      })
    }));
  }

  private void BuildCommanderSections(CivPlayerRoundStats stats)
  {
    ((Control) this._myStatsContainer).AddChild(this.BuildSection("civ-ui-roundend-my-sec-command", (IEnumerable<string>) new string[1]
    {
      Loc.GetString("civ-ui-roundend-my-command", new (string, object)[2]
      {
        ("approved", (object) stats.CommanderPurchasesApproved),
        ("points", (object) stats.CommanderPointsSpent)
      })
    }));
    ((Control) this._myStatsContainer).AddChild(this.BuildSection("civ-ui-roundend-my-sec-economy", (IEnumerable<string>) new string[1]
    {
      Loc.GetString("civ-ui-roundend-my-economy", new (string, object)[2]
      {
        ("purchases", (object) stats.CommanderShopPurchases),
        ("spent", (object) stats.CommanderShopSpent)
      })
    }));
    ((Control) this._myStatsContainer).AddChild(this.BuildSection("civ-ui-roundend-my-sec-firesupport", (IEnumerable<string>) new string[1]
    {
      Loc.GetString("civ-ui-roundend-my-firesupport", new (string, object)[2]
      {
        ("airstrike", (object) stats.AirstrikeKills),
        ("artillery", (object) stats.ArtilleryKills)
      })
    }));
  }

  private List<string> BuildWeaponLines(CivPlayerRoundStats stats)
  {
    if (stats.WeaponKills.Count != 0 || stats.WeaponDamage.Count != 0)
      return stats.WeaponKills.Keys.Concat<string>((IEnumerable<string>) stats.WeaponDamage.Keys).Distinct<string>().OrderByDescending<string, int>((Func<string, int>) (w => stats.WeaponKills.GetValueOrDefault<string, int>(w))).ThenByDescending<string, int>((Func<string, int>) (w => stats.WeaponDamage.GetValueOrDefault<string, int>(w))).Take<string>(6).Select<string, string>((Func<string, string>) (w => Loc.GetString("civ-ui-roundend-my-weapon-line", new (string, object)[3]
      {
        ("name", (object) this.ResolveWeaponName(w)),
        ("kills", (object) stats.WeaponKills.GetValueOrDefault<string, int>(w)),
        ("damage", (object) stats.WeaponDamage.GetValueOrDefault<string, int>(w))
      }))).ToList<string>();
    return new List<string>()
    {
      Loc.GetString("civ-ui-roundend-my-weapon-none")
    };
  }

  private List<string> BuildKillLogLines(CivPlayerRoundStats stats)
  {
    if (stats.KillDetails.Count == 0)
      return new List<string>()
      {
        Loc.GetString("civ-ui-roundend-my-kill-none")
      };
    List<string> list = stats.KillDetails.OrderBy<CivKillDetail, int>((Func<CivKillDetail, int>) (k => k.AtSeconds)).Take<CivKillDetail>(14).Select<CivKillDetail, string>((Func<CivKillDetail, string>) (k => Loc.GetString("civ-ui-roundend-my-kill-line", new (string, object)[5]
    {
      ("time", (object) CivRoundEndWindow.FormatSeconds(k.AtSeconds)),
      ("victim", (object) k.Victim),
      ("weapon", (object) this.ResolveWeaponName(k.Weapon)),
      ("dist", (object) k.Distance),
      ("tk", k.Teamkill ? (object) Loc.GetString("civ-ui-roundend-my-kill-tk") : (object) "")
    }))).ToList<string>();
    if (stats.KillDetails.Count > 14)
      list.Add(Loc.GetString("civ-ui-roundend-my-kill-more", new (string, object)[1]
      {
        ("count", (object) (stats.KillDetails.Count - 14))
      }));
    return list;
  }

  private Control BuildSection(string headerKey, IEnumerable<string> lines)
  {
    PanelContainer panelContainer = new PanelContainer()
    {
      PanelOverride = (StyleBox) this.BuildPanelStyle("#1B1F2CEE", "#445574")
    };
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(4);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label = new Label();
    label.Text = Loc.GetString(headerKey);
    label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#8DC8FF", new Color?()));
    ((Control) label).StyleClasses.Add("LabelHeading");
    ((Control) boxContainer3).AddChild((Control) label);
    foreach (string line in lines)
      ((Control) boxContainer2).AddChild((Control) new Label()
      {
        Text = line,
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#D3DCEF", new Color?()))
      });
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  private string ResolveWeaponName(string protoId)
  {
    switch (protoId)
    {
      case "Unarmed":
        return Loc.GetString("civ-ui-roundend-weapon-unarmed");
      case "Airstrike":
        return Loc.GetString("civ-ui-roundend-weapon-airstrike");
      case "Artillery":
        return Loc.GetString("civ-ui-roundend-weapon-artillery");
      case "Mortar":
        return Loc.GetString("civ-ui-roundend-weapon-mortar");
      case "Mine":
        return Loc.GetString("civ-ui-roundend-weapon-mine");
      default:
        EntityPrototype entityPrototype;
        return !this._proto.TryIndex<EntityPrototype>(protoId, ref entityPrototype) ? protoId : entityPrototype.Name;
    }
  }

  private static string Kdr(CivPlayerRoundStats stats)
  {
    return ((float) stats.Kills / (float) Math.Max(1, stats.Deaths)).ToString("F2");
  }

  private static int Accuracy(CivPlayerRoundStats stats)
  {
    int num = stats.ShotsFired.Values.Sum();
    return num <= 0 ? 0 : (int) Math.Round((double) stats.ShotsHit.Values.Sum() * 100.0 / (double) num);
  }

  private static string FormatDuration(TimeSpan span)
  {
    return $"{(int) span.TotalMinutes}:{span.Seconds:D2}";
  }

  private static string FormatSeconds(int seconds) => $"{seconds / 60}:{seconds % 60:D2}";

  public void SetTeamEntries(List<CivRoundEndTeamEntry> entries)
  {
    ((Control) this._teamEntriesContainer).RemoveAllChildren();
    if (entries.Count == 0)
    {
      ((Control) this._teamEntriesContainer).AddChild((Control) new Label()
      {
        Text = Loc.GetString("civ-ui-roundend-no-team-data"),
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A3AEBD", new Color?()))
      });
    }
    else
    {
      foreach (CivRoundEndTeamEntry entry in entries)
        ((Control) this._teamEntriesContainer).AddChild(this.BuildTeamGroup(entry));
    }
  }

  private Control BuildHeaderRow()
  {
    PanelContainer panelContainer = new PanelContainer();
    panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#101521EE", new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) "#4F5F82", new Color?()),
      BorderThickness = new Thickness(1f)
    };
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).Margin = new Thickness(8f, 6f);
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label1 = new Label();
    label1.Text = Loc.GetString("civ-ui-roundend-header-player");
    ((Control) label1).HorizontalExpand = true;
    label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A7B6CE", new Color?()));
    ((Control) boxContainer3).AddChild((Control) label1);
    BoxContainer boxContainer4 = boxContainer2;
    Label label2 = new Label();
    label2.Text = Loc.GetString("civ-ui-roundend-header-role");
    ((Control) label2).MinWidth = 180f;
    label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A7B6CE", new Color?()));
    ((Control) boxContainer4).AddChild((Control) label2);
    BoxContainer boxContainer5 = boxContainer2;
    Label label3 = new Label();
    label3.Text = Loc.GetString("civ-ui-roundend-header-squad");
    ((Control) label3).MinWidth = 110f;
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A7B6CE", new Color?()));
    ((Control) boxContainer5).AddChild((Control) label3);
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  private Control BuildTeamGroup(CivRoundEndTeamEntry entry)
  {
    PanelContainer panelContainer = new PanelContainer()
    {
      PanelOverride = (StyleBox) this.BuildPanelStyle("#1B1F2CEE", "#4F5F82")
    };
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(6);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    Label label = new Label();
    label.Text = string.IsNullOrWhiteSpace(entry.RoleName) ? entry.TeamName : $"{entry.TeamName} ({entry.RoleName})";
    label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFD05C", new Color?()));
    ((Control) label).StyleClasses.Add("LabelHeading");
    ((Control) boxContainer3).AddChild((Control) label);
    if (entry.Players.Count == 0)
    {
      ((Control) boxContainer2).AddChild((Control) new Label()
      {
        Text = Loc.GetString("civ-ui-roundend-no-players"),
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A3AEBD", new Color?()))
      });
    }
    else
    {
      foreach (CivRoundEndPlayerEntry player in entry.Players)
        ((Control) boxContainer2).AddChild(this.BuildPlayerRow(player));
    }
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  private Control BuildPlayerRow(CivRoundEndPlayerEntry entry)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(2);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer3.SeparationOverride = new int?(10);
    ((Control) boxContainer3).HorizontalExpand = true;
    BoxContainer boxContainer4 = boxContainer3;
    BoxContainer boxContainer5 = boxContainer4;
    Label label1 = new Label();
    label1.Text = entry.PlayerName;
    ((Control) label1).HorizontalExpand = true;
    label1.FontColorOverride = new Color?(entry.IsCommander ? Color.FromHex((ReadOnlySpan<char>) "#FFE39A", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#E2E8F4", new Color?()));
    ((Control) boxContainer5).AddChild((Control) label1);
    BoxContainer boxContainer6 = boxContainer4;
    Label label2 = new Label();
    label2.Text = entry.RoleText;
    ((Control) label2).MinWidth = 180f;
    label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#D3DCEF", new Color?()));
    ((Control) boxContainer6).AddChild((Control) label2);
    BoxContainer boxContainer7 = boxContainer4;
    Label label3 = new Label();
    label3.Text = entry.SquadText;
    ((Control) label3).MinWidth = 110f;
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#8DC8FF", new Color?()));
    ((Control) boxContainer7).AddChild((Control) label3);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    CivPlayerRoundStats stats = entry.Stats;
    if (stats != null)
    {
      string str = CivRoundEndWindow.FormatPlayerStats(stats, entry.IsCommander);
      if (!string.IsNullOrEmpty(str))
      {
        BoxContainer boxContainer8 = boxContainer2;
        Label label4 = new Label();
        label4.Text = str;
        ((Control) label4).HorizontalExpand = true;
        label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A7B6CE", new Color?()));
        ((Control) boxContainer8).AddChild((Control) label4);
      }
    }
    return (Control) boxContainer2;
  }

  private static string FormatPlayerStats(CivPlayerRoundStats stats, bool isCommander)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("    ");
    stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-score", new (string, object)[1]
    {
      ("score", (object) stats.Score)
    }));
    if (stats.Awards.Count > 0)
    {
      stringBuilder.Append(" [");
      stringBuilder.Append(string.Join(", ", (IEnumerable<string>) stats.Awards));
      stringBuilder.Append(']');
    }
    stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-kd", new (string, object)[2]
    {
      ("kills", (object) stats.Kills),
      ("deaths", (object) stats.Deaths)
    }));
    if (stats.TeamKills > 0)
      stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-tk", new (string, object)[1]
      {
        ("count", (object) stats.TeamKills)
      }));
    if (stats.DamageDealt > 0)
      stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-damage", new (string, object)[1]
      {
        ("damage", (object) stats.DamageDealt)
      }));
    if (isCommander && stats.CommanderPurchasesApproved > 0)
      stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-commander", new (string, object)[2]
      {
        ("approved", (object) stats.CommanderPurchasesApproved),
        ("points", (object) stats.CommanderPointsSpent)
      }));
    if (stats.SurvivalTime.TotalSeconds >= 1.0)
      stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-survival", new (string, object)[2]
      {
        ("minutes", (object) (int) stats.SurvivalTime.TotalMinutes),
        ("seconds", (object) stats.SurvivalTime.Seconds)
      }));
    return stringBuilder.ToString();
  }

  private BoxContainer BuildRow(params Control[] cards)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    foreach (Control card in cards)
      ((Control) boxContainer2).AddChild(card);
    return boxContainer2;
  }

  private Control BuildSummaryCard(string title, string value, string accentColor = "#B7C2D8")
  {
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).HorizontalExpand = true;
    panelContainer.PanelOverride = (StyleBox) this.BuildPanelStyle("#1B1F2CEE", "#445574");
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(4);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    ((Control) boxContainer2).AddChild((Control) new Label()
    {
      Text = title,
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) accentColor, new Color?()))
    });
    BoxContainer boxContainer3 = boxContainer2;
    Label label = new Label();
    label.Text = value;
    ((Control) label).HorizontalExpand = true;
    label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#F4F7FB", new Color?()));
    ((Control) label).StyleClasses.Add("LabelHeading");
    ((Control) boxContainer3).AddChild((Control) label);
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  private StyleBoxFlat BuildPanelStyle(string backgroundColor, string borderColor)
  {
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) backgroundColor, new Color?());
    styleBoxFlat.BorderColor = Color.FromHex((ReadOnlySpan<char>) borderColor, new Color?());
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 8f);
    return styleBoxFlat;
  }
}
