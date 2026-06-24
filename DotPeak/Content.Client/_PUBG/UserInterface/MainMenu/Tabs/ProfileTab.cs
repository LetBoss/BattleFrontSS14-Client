// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.ProfileTab
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared._PUBG.Skin;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class ProfileTab : BoxContainer
{
  private ProfileSubcategory _currentSubcategory;
  private Dictionary<string, int> _sponsorPermissions = new Dictionary<string, int>();
  private List<SponsorPermissionDetailInfo> _sponsorPermissionDetails = new List<SponsorPermissionDetailInfo>();
  private SponsorTierInfo? _sponsorDisplayTier;
  private List<SponsorActiveTierInfo> _sponsorActiveTiers = new List<SponsorActiveTierInfo>();
  private SponsorDisplayMode _sponsorDisplayMode;
  private string? _sponsorPreferredTierKey;
  private bool _sponsorDisplayUpdating;
  private int _totalGames;
  private int _wins;
  private int _totalKills;
  private int _totalDamage;
  private int _avgSurvivalTime;
  private int _totalCaseDropSkins;
  private int _unlockedCaseDropSkins;
  private int _totalEmotes;
  private int _availableEmotes;
  private int _totalDeaths;
  private List<LeaderboardEntryInfo> _leaderboard = new List<LeaderboardEntryInfo>();
  private int _playerRank;
  private int _playerRating;
  private int _reputation = 100;
  private List<MatchHistoryInfo> _matchHistory = new List<MatchHistoryInfo>();

  public bool ShowDisplaySettings { get; set; } = true;

  public event Action? OnOpenSponsorDisplaySettingsRequested;

  public ProfileTab()
  {
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
  }

  public void LoadSubcategory(ProfileSubcategory subcategory)
  {
    this._currentSubcategory = subcategory;
    ((Control) this).RemoveAllChildren();
    if (subcategory != ProfileSubcategory.Stats)
    {
      if (subcategory != ProfileSubcategory.Sponsors)
        return;
      this.LoadSponsorsView();
    }
    else
      this.LoadStatsView();
  }

  private void LoadStatsView()
  {
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(20f);
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer3).HorizontalExpand = true;
    BoxContainer boxContainer4 = boxContainer3;
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer5).HorizontalExpand = true;
    ((Control) boxContainer5).Margin = new Thickness(0.0f, 0.0f, 10f, 0.0f);
    BoxContainer boxContainer6 = boxContainer5;
    Label label1 = new Label();
    label1.Text = "СТАТИСТИКА ИГРОКА";
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    BoxContainer visualStatsSection = this.CreateVisualStatsSection();
    ((Control) boxContainer6).AddChild((Control) label2);
    ((Control) boxContainer6).AddChild((Control) visualStatsSection);
    AlternatingBGContainer alternatingBgContainer1 = new AlternatingBGContainer();
    alternatingBgContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    alternatingBgContainer1.HorizontalExpand = true;
    ((Control) alternatingBgContainer1).Margin = new Thickness(0.0f, 15f, 0.0f, 0.0f);
    AlternatingBGContainer alternatingBgContainer2 = alternatingBgContainer1;
    int num1 = this._avgSurvivalTime / 3600;
    int num2 = this._avgSurvivalTime % 3600 / 60;
    string str1;
    if (this._playerRank <= 0)
      str1 = "N/A";
    else
      str1 = $"#{this._playerRank}";
    string str2 = str1;
    alternatingBgContainer2.AddControl((Control) this.CreateStatRow("Ваш рейтинг:", $"{this._playerRating} ({str2})"));
    alternatingBgContainer2.AddControl((Control) this.CreateStatRow(Loc.GetString("pubg-reputation-mainmenu-label"), $"{this._reputation}/100"));
    alternatingBgContainer2.AddControl((Control) this.CreateStatRow("Всего игр:", this._totalGames.ToString()));
    alternatingBgContainer2.AddControl((Control) this.CreateStatRow("Убийств:", this._totalKills.ToString()));
    alternatingBgContainer2.AddControl((Control) this.CreateStatRow("Урона нанесено:", this._totalDamage.ToString()));
    alternatingBgContainer2.AddControl((Control) this.CreateStatRow("Время выживания:", $"{num1}ч {num2}м"));
    alternatingBgContainer2.AddControl((Control) this.CreateStatRow("Скинов:", $"{this._unlockedCaseDropSkins}/{this._totalCaseDropSkins}"));
    alternatingBgContainer2.AddControl((Control) this.CreateStatRow("Эмоций:", $"{this._availableEmotes}/{this._totalEmotes}"));
    ((Control) boxContainer6).AddChild((Control) alternatingBgContainer2);
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer7).MinWidth = 320f;
    ((Control) boxContainer7).Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
    BoxContainer boxContainer8 = boxContainer7;
    BoxContainer leaderboardSection = this.CreateLeaderboardSection();
    ((Control) boxContainer8).AddChild((Control) leaderboardSection);
    BoxContainer matchHistorySection = this.CreateMatchHistorySection();
    ((Control) boxContainer8).AddChild((Control) matchHistorySection);
    ((Control) boxContainer4).AddChild((Control) boxContainer6);
    ((Control) boxContainer4).AddChild((Control) boxContainer8);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ((Control) scrollContainer2).AddChild((Control) boxContainer2);
    ((Control) this).AddChild((Control) scrollContainer2);
  }

  private BoxContainer CreateVisualStatsSection()
  {
    BoxContainer visualStatsSection = new BoxContainer();
    visualStatsSection.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) visualStatsSection).HorizontalExpand = true;
    float num1 = this._totalGames > 0 ? (float) ((double) this._wins / (double) this._totalGames * 100.0) : 0.0f;
    PubgStatCard pubgStatCard1 = new PubgStatCard();
    pubgStatCard1.Title = "WINRATE";
    pubgStatCard1.Value = $"{num1:F1}% ({this._wins}/{this._totalGames})";
    pubgStatCard1.Progress = num1 / 100f;
    pubgStatCard1.ShowProgress = true;
    pubgStatCard1.ValueColor = Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?());
    pubgStatCard1.HorizontalExpand = true;
    pubgStatCard1.Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    ((Control) visualStatsSection).AddChild((Control) pubgStatCard1);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(0.0f, 5f, 0.0f, 0.0f);
    BoxContainer boxContainer2 = boxContainer1;
    float num2 = this._totalDeaths > 0 ? (float) this._totalKills / (float) this._totalDeaths : (float) this._totalKills;
    Color color1 = (double) num2 >= 2.0 ? Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?()) : ((double) num2 >= 1.0 ? Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?()) : ((double) num2 >= 0.5 ? Color.FromHex((ReadOnlySpan<char>) "#FF9500", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#FF4444", new Color?())));
    PubgStatCard pubgStatCard2 = new PubgStatCard();
    pubgStatCard2.Title = "K/D RATIO";
    pubgStatCard2.Value = $"{num2:F2}";
    pubgStatCard2.ValueColor = color1;
    pubgStatCard2.HorizontalExpand = true;
    pubgStatCard2.Margin = new Thickness(0.0f, 0.0f, 10f, 0.0f);
    pubgStatCard2.IsPulse = (double) num2 >= 2.0;
    PubgStatCard pubgStatCard3 = pubgStatCard2;
    ((Control) boxContainer2).AddChild((Control) pubgStatCard3);
    int num3 = this._totalGames > 0 ? this._totalDamage / this._totalGames : 0;
    Color color2 = num3 >= 500 ? Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?()) : (num3 >= 200 ? Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#FF9500", new Color?()));
    PubgStatCard pubgStatCard4 = new PubgStatCard();
    pubgStatCard4.Title = "УРОН/ИГРА";
    pubgStatCard4.Value = num3.ToString();
    pubgStatCard4.ValueColor = color2;
    pubgStatCard4.HorizontalExpand = true;
    pubgStatCard4.IsPulse = num3 >= 500;
    PubgStatCard pubgStatCard5 = pubgStatCard4;
    ((Control) boxContainer2).AddChild((Control) pubgStatCard5);
    float num4 = this._totalGames > 0 ? (float) this._totalKills / (float) this._totalGames : 0.0f;
    Color color3 = (double) num4 >= 3.0 ? Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?()) : ((double) num4 >= 1.0 ? Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#FF9500", new Color?()));
    PubgStatCard pubgStatCard6 = new PubgStatCard();
    pubgStatCard6.Title = "УБИЙСТВ/ИГРА";
    pubgStatCard6.Value = $"{num4:F1}";
    pubgStatCard6.ValueColor = color3;
    pubgStatCard6.HorizontalExpand = true;
    pubgStatCard6.Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
    pubgStatCard6.IsPulse = (double) num4 >= 3.0;
    PubgStatCard pubgStatCard7 = pubgStatCard6;
    ((Control) boxContainer2).AddChild((Control) pubgStatCard7);
    ((Control) visualStatsSection).AddChild((Control) boxContainer2);
    return visualStatsSection;
  }

  private BoxContainer CreateLeaderboardSection()
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer).HorizontalExpand = true;
    BoxContainer leaderboardSection = boxContainer;
    Label label1 = new Label();
    label1.Text = "ТОП ИГРОКОВ";
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    ((Control) leaderboardSection).AddChild((Control) label2);
    AlternatingBGContainer alternatingBgContainer1 = new AlternatingBGContainer();
    alternatingBgContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    alternatingBgContainer1.HorizontalExpand = true;
    AlternatingBGContainer alternatingBgContainer2 = alternatingBgContainer1;
    bool flag = false;
    if (this._leaderboard.Count > 0)
    {
      foreach (LeaderboardEntryInfo entry in this._leaderboard)
      {
        bool isCurrentPlayer = entry.Rank == this._playerRank && this._playerRank > 0;
        if (isCurrentPlayer)
          flag = true;
        alternatingBgContainer2.AddControl((Control) this.CreateLeaderboardRow(entry, isCurrentPlayer));
      }
      if (!flag && this._playerRank > 0)
      {
        Label label3 = new Label();
        label3.Text = "...";
        label3.FontColorOverride = new Color?(Color.Gray);
        ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
        ((Control) label3).Margin = new Thickness(0.0f, 5f, 0.0f, 5f);
        Label label4 = label3;
        alternatingBgContainer2.AddControl((Control) label4);
        LeaderboardEntryInfo entry = new LeaderboardEntryInfo()
        {
          Rank = this._playerRank,
          Username = "Вы",
          Rating = this._playerRating,
          Games = this._totalGames,
          Wins = this._wins,
          Kills = this._totalKills,
          DamageDealt = this._totalDamage,
          SurvivalTime = this._avgSurvivalTime
        };
        alternatingBgContainer2.AddControl((Control) this.CreateLeaderboardRow(entry, true));
      }
    }
    else
    {
      Label label5 = new Label();
      label5.Text = "Нет данных";
      label5.FontColorOverride = new Color?(Color.Gray);
      ((Control) label5).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label5).Margin = new Thickness(0.0f, 10f, 0.0f, 10f);
      Label label6 = label5;
      alternatingBgContainer2.AddControl((Control) label6);
    }
    ((Control) leaderboardSection).AddChild((Control) alternatingBgContainer2);
    return leaderboardSection;
  }

  private BoxContainer CreateMatchHistorySection()
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer).HorizontalExpand = true;
    ((Control) boxContainer).Margin = new Thickness(0.0f, 15f, 0.0f, 0.0f);
    BoxContainer matchHistorySection = boxContainer;
    Label label1 = new Label();
    label1.Text = "ПОСЛЕДНИЕ МАТЧИ";
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    ((Control) matchHistorySection).AddChild((Control) label2);
    AlternatingBGContainer alternatingBgContainer1 = new AlternatingBGContainer();
    alternatingBgContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    alternatingBgContainer1.HorizontalExpand = true;
    AlternatingBGContainer alternatingBgContainer2 = alternatingBgContainer1;
    if (this._matchHistory.Count > 0)
    {
      foreach (MatchHistoryInfo match in this._matchHistory.Take<MatchHistoryInfo>(5))
        alternatingBgContainer2.AddControl((Control) this.CreateMatchHistoryRow(match));
    }
    else
    {
      Label label3 = new Label();
      label3.Text = "Нет данных";
      label3.FontColorOverride = new Color?(Color.Gray);
      ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label3).Margin = new Thickness(0.0f, 10f, 0.0f, 10f);
      Label label4 = label3;
      alternatingBgContainer2.AddControl((Control) label4);
    }
    ((Control) matchHistorySection).AddChild((Control) alternatingBgContainer2);
    return matchHistorySection;
  }

  private BoxContainer CreateMatchHistoryRow(MatchHistoryInfo match)
  {
    BoxContainer matchHistoryRow = new BoxContainer();
    matchHistoryRow.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) matchHistoryRow).HorizontalExpand = true;
    string str1 = match.IsWin ? "W" : "L";
    Color color1 = match.IsWin ? Color.Gold : Color.Red;
    Label label1 = new Label();
    label1.Text = str1;
    label1.FontColorOverride = new Color?(color1);
    ((Control) label1).MinWidth = 25f;
    ((Control) label1).Margin = new Thickness(8f, 5f, 5f, 5f);
    ((Control) matchHistoryRow).AddChild((Control) label1);
    Color color2 = match.Placement == 1 ? Color.Gold : (match.Placement <= 3 ? Color.LimeGreen : (match.Placement <= 10 ? Color.Yellow : Color.White));
    Label label2 = new Label();
    label2.Text = $"#{match.Placement}";
    label2.FontColorOverride = new Color?(color2);
    ((Control) label2).MinWidth = 35f;
    ((Control) label2).Margin = new Thickness(5f, 5f, 5f, 5f);
    ((Control) matchHistoryRow).AddChild((Control) label2);
    Label label3 = new Label();
    label3.Text = $"{match.Kills}K";
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FF6B6B", new Color?()));
    ((Control) label3).MinWidth = 35f;
    ((Control) label3).Margin = new Thickness(5f, 5f, 5f, 5f);
    ((Control) matchHistoryRow).AddChild((Control) label3);
    Label label4 = new Label();
    label4.Text = $"{match.Damage}";
    label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFAA55", new Color?()));
    ((Control) label4).MinWidth = 50f;
    ((Control) label4).Margin = new Thickness(5f, 5f, 5f, 5f);
    ((Control) matchHistoryRow).AddChild((Control) label4);
    Color color3 = match.RatingChange >= 0 ? Color.LimeGreen : Color.Red;
    string str2 = match.RatingChange >= 0 ? "+" : "";
    Label label5 = new Label();
    label5.Text = $"{str2}{match.RatingChange}";
    label5.FontColorOverride = new Color?(color3);
    ((Control) label5).HorizontalExpand = true;
    ((Control) label5).HorizontalAlignment = (Control.HAlignment) 3;
    ((Control) label5).Margin = new Thickness(5f, 5f, 8f, 5f);
    ((Control) matchHistoryRow).AddChild((Control) label5);
    return matchHistoryRow;
  }

  private BoxContainer CreateLeaderboardRow(LeaderboardEntryInfo entry, bool isCurrentPlayer)
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer).HorizontalExpand = true;
    BoxContainer leaderboardRow = boxContainer;
    Color color1;
    switch (entry.Rank)
    {
      case 1:
        color1 = Color.Gold;
        break;
      case 2:
        color1 = Color.Silver;
        break;
      case 3:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#CD7F32", new Color?());
        break;
      default:
        color1 = isCurrentPlayer ? Color.LimeGreen : Color.White;
        break;
    }
    Color color2 = color1;
    Label label1 = new Label();
    label1.Text = $"#{entry.Rank}";
    label1.FontColorOverride = new Color?(color2);
    ((Control) label1).MinWidth = 35f;
    ((Control) label1).Margin = new Thickness(8f, 5f, 5f, 5f);
    Label label2 = label1;
    Label label3 = new Label();
    label3.Text = entry.Username;
    label3.FontColorOverride = new Color?(isCurrentPlayer ? Color.LimeGreen : Color.White);
    ((Control) label3).HorizontalExpand = true;
    ((Control) label3).Margin = new Thickness(5f, 5f, 5f, 5f);
    Label label4 = label3;
    Label label5 = new Label();
    label5.Text = entry.Rating.ToString();
    label5.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#88AAFF", new Color?()));
    ((Control) label5).Margin = new Thickness(5f, 5f, 8f, 5f);
    Label label6 = label5;
    ((Control) leaderboardRow).AddChild((Control) label2);
    ((Control) leaderboardRow).AddChild((Control) label4);
    ((Control) leaderboardRow).AddChild((Control) label6);
    return leaderboardRow;
  }

  private void LoadSponsorsView()
  {
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(20f);
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    label1.Text = "✦ СПОНСОРКА ✦";
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 25f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    ((Control) boxContainer2).AddChild((Control) label2);
    PanelContainer mainTierCard = this.CreateMainTierCard();
    ((Control) boxContainer2).AddChild((Control) mainTierCard);
    BoxContainer settingsLauncherSection = this.CreateDisplaySettingsLauncherSection();
    if (this.ShowDisplaySettings)
      ((Control) boxContainer2).AddChild((Control) settingsLauncherSection);
    if (this._sponsorActiveTiers.Count > 0)
    {
      BoxContainer activeTiersSection = this.CreateActiveTiersSection();
      ((Control) boxContainer2).AddChild((Control) activeTiersSection);
    }
    BoxContainer permissionsSection = this.CreatePermissionsSection();
    ((Control) boxContainer2).AddChild((Control) permissionsSection);
    BoxContainer sponsorButton = this.CreateSponsorButton();
    ((Control) boxContainer2).AddChild((Control) sponsorButton);
    ((Control) scrollContainer2).AddChild((Control) boxContainer2);
    ((Control) this).AddChild((Control) scrollContainer2);
  }

  private PanelContainer CreateMainTierCard()
  {
    PanelContainer mainTierCard = new PanelContainer();
    ((Control) mainTierCard).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) mainTierCard).Margin = new Thickness(0.0f, 0.0f, 0.0f, 20f);
    ((Control) mainTierCard).StyleClasses.Add("AngleRect");
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer1).Margin = new Thickness(50f, 20f, 50f, 20f);
    BoxContainer boxContainer2 = boxContainer1;
    if (this._sponsorDisplayTier != null)
    {
      Color color = !string.IsNullOrEmpty(this._sponsorDisplayTier.Color) ? Color.FromHex((ReadOnlySpan<char>) this._sponsorDisplayTier.Color, new Color?()) : Color.Gold;
      Label label1 = new Label();
      label1.Text = "ВАШ СТАТУС";
      label1.FontColorOverride = new Color?(Color.Gray);
      ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
      Label label2 = label1;
      ((Control) boxContainer2).AddChild((Control) label2);
      if (!string.IsNullOrEmpty(this._sponsorDisplayTier.Badge))
      {
        Label label3 = new Label();
        label3.Text = this._sponsorDisplayTier.Badge;
        ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
        ((Control) label3).Margin = new Thickness(0.0f, 0.0f, 0.0f, 5f);
        Label label4 = label3;
        ((Control) boxContainer2).AddChild((Control) label4);
      }
      Label label5 = new Label();
      label5.Text = this._sponsorDisplayTier.TierName;
      label5.FontColorOverride = new Color?(color);
      ((Control) label5).HorizontalAlignment = (Control.HAlignment) 2;
      Label label6 = label5;
      ((Control) label6).SetOnlyStyleClass("LabelHeading");
      ((Control) boxContainer2).AddChild((Control) label6);
      Label label7 = new Label();
      label7.Text = "Спасибо за поддержку! ♥";
      label7.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FF6B9D", new Color?()));
      ((Control) label7).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label7).Margin = new Thickness(0.0f, 10f, 0.0f, 0.0f);
      Label label8 = label7;
      ((Control) boxContainer2).AddChild((Control) label8);
    }
    else
    {
      Label label9 = new Label();
      label9.Text = "У вас пока нет спонсорского статуса";
      label9.FontColorOverride = new Color?(Color.Gray);
      ((Control) label9).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label9).Margin = new Thickness(0.0f, 10f, 0.0f, 10f);
      Label label10 = label9;
      ((Control) boxContainer2).AddChild((Control) label10);
      Label label11 = new Label();
      label11.Text = "Поддержите проект и получите бонусы!";
      label11.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#88AAFF", new Color?()));
      ((Control) label11).HorizontalAlignment = (Control.HAlignment) 2;
      Label label12 = label11;
      ((Control) boxContainer2).AddChild((Control) label12);
    }
    ((Control) mainTierCard).AddChild((Control) boxContainer2);
    return mainTierCard;
  }

  private BoxContainer CreateDisplaySettingsLauncherSection()
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer).Margin = new Thickness(0.0f, 0.0f, 0.0f, 20f);
    BoxContainer settingsLauncherSection = boxContainer;
    Label label1 = new Label();
    label1.Text = Loc.GetString("mainmenu-sponsor-display-title");
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 4f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    ((Control) settingsLauncherSection).AddChild((Control) label2);
    Label label3 = new Label();
    label3.Text = Loc.GetString("mainmenu-sponsor-display-subtitle");
    label3.FontColorOverride = new Color?(Color.Gray);
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label3).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    Label label4 = label3;
    ((Control) settingsLauncherSection).AddChild((Control) label4);
    string tierName = Loc.GetString("mainmenu-sponsor-display-current-none");
    if (this._sponsorDisplayMode == SponsorDisplayMode.Hidden)
      tierName = Loc.GetString("mainmenu-sponsor-display-current-hidden");
    else if (this._sponsorDisplayTier != null)
      tierName = this._sponsorDisplayTier.TierName;
    Label label5 = new Label();
    label5.Text = Loc.GetString("mainmenu-sponsor-display-current", new (string, object)[1]
    {
      ("value", (object) tierName)
    });
    ((Control) label5).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label5).Margin = new Thickness(0.0f, 0.0f, 0.0f, 8f);
    Label label6 = label5;
    ((Control) settingsLauncherSection).AddChild((Control) label6);
    if (this._sponsorDisplayUpdating)
    {
      Label label7 = new Label();
      label7.Text = Loc.GetString("mainmenu-sponsor-display-updating");
      label7.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#88AAFF", new Color?()));
      ((Control) label7).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label7).Margin = new Thickness(0.0f, 0.0f, 0.0f, 8f);
      Label label8 = label7;
      ((Control) settingsLauncherSection).AddChild((Control) label8);
    }
    Button button1 = new Button();
    button1.Text = Loc.GetString("mainmenu-sponsor-display-open-button");
    ((Control) button1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) button1).MinWidth = 260f;
    ((BaseButton) button1).Disabled = this._sponsorDisplayUpdating;
    Button button2 = button1;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.RequestSponsorDisplaySettingsOpen());
    ((Control) settingsLauncherSection).AddChild((Control) button2);
    return settingsLauncherSection;
  }

  private void RequestSponsorDisplaySettingsOpen()
  {
    if (this._sponsorDisplayUpdating)
      return;
    this._sponsorDisplayUpdating = true;
    if (this._currentSubcategory == ProfileSubcategory.Sponsors)
      this.LoadSubcategory(ProfileSubcategory.Sponsors);
    Action settingsRequested = this.OnOpenSponsorDisplaySettingsRequested;
    if (settingsRequested == null)
      return;
    settingsRequested();
  }

  private BoxContainer CreateActiveTiersSection()
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 20f);
    BoxContainer activeTiersSection = boxContainer1;
    Label label1 = new Label();
    label1.Text = "АКТИВНЫЕ ПОДПИСКИ";
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    ((Control) activeTiersSection).AddChild((Control) label2);
    AlternatingBGContainer alternatingBgContainer1 = new AlternatingBGContainer();
    alternatingBgContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    alternatingBgContainer1.HorizontalExpand = true;
    ((Control) alternatingBgContainer1).Margin = new Thickness(50f, 0.0f, 50f, 0.0f);
    AlternatingBGContainer alternatingBgContainer2 = alternatingBgContainer1;
    foreach (SponsorActiveTierInfo sponsorActiveTier in this._sponsorActiveTiers)
    {
      BoxContainer boxContainer2 = new BoxContainer();
      boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 0;
      ((Control) boxContainer2).HorizontalExpand = true;
      BoxContainer boxContainer3 = boxContainer2;
      Label label3 = new Label();
      label3.Text = "★ " + sponsorActiveTier.Name;
      label3.FontColorOverride = new Color?(Color.Gold);
      ((Control) label3).HorizontalExpand = true;
      ((Control) label3).Margin = new Thickness(10f, 8f, 10f, 8f);
      Label label4 = label3;
      ((Control) boxContainer3).AddChild((Control) label4);
      DateTime? expiresAt = sponsorActiveTier.ExpiresAt;
      if (expiresAt.HasValue)
      {
        expiresAt = sponsorActiveTier.ExpiresAt;
        TimeSpan timeSpan = expiresAt.Value - DateTime.UtcNow;
        string str;
        Color color;
        if (timeSpan.TotalDays >= 30.0)
        {
          str = $"{(int) (timeSpan.TotalDays / 30.0)} мес.";
          color = Color.LimeGreen;
        }
        else if (timeSpan.TotalDays >= 1.0)
        {
          str = $"{(int) timeSpan.TotalDays} дн.";
          color = timeSpan.TotalDays < 7.0 ? Color.Orange : Color.LimeGreen;
        }
        else if (timeSpan.TotalHours >= 1.0)
        {
          str = $"{(int) timeSpan.TotalHours} ч.";
          color = Color.Red;
        }
        else
        {
          str = "< 1 ч.";
          color = Color.Red;
        }
        Label label5 = new Label();
        label5.Text = "⏱ " + str;
        label5.FontColorOverride = new Color?(color);
        ((Control) label5).Margin = new Thickness(10f, 8f, 10f, 8f);
        Label label6 = label5;
        ((Control) boxContainer3).AddChild((Control) label6);
      }
      else
      {
        Label label7 = new Label();
        label7.Text = "∞ Навсегда";
        label7.FontColorOverride = new Color?(Color.LimeGreen);
        ((Control) label7).Margin = new Thickness(10f, 8f, 10f, 8f);
        Label label8 = label7;
        ((Control) boxContainer3).AddChild((Control) label8);
      }
      alternatingBgContainer2.AddControl((Control) boxContainer3);
    }
    ((Control) activeTiersSection).AddChild((Control) alternatingBgContainer2);
    return activeTiersSection;
  }

  private BoxContainer CreatePermissionsSection()
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer).Margin = new Thickness(0.0f, 0.0f, 0.0f, 20f);
    BoxContainer permissionsSection = boxContainer;
    Label label1 = new Label();
    label1.Text = "ВАШИ ПРИВИЛЕГИИ";
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    ((Control) permissionsSection).AddChild((Control) label2);
    if (this._sponsorPermissionDetails.Count > 0)
    {
      foreach (IGrouping<string, SponsorPermissionDetailInfo> grouping in (IEnumerable<IGrouping<string, SponsorPermissionDetailInfo>>) this._sponsorPermissionDetails.GroupBy<SponsorPermissionDetailInfo, string>((Func<SponsorPermissionDetailInfo, string>) (p => p.Source)).OrderBy<IGrouping<string, SponsorPermissionDetailInfo>, int>((Func<IGrouping<string, SponsorPermissionDetailInfo>, int>) (g => !g.Key.StartsWith("Тир") ? 1 : 0)))
      {
        Label label3 = new Label();
        label3.Text = grouping.Key;
        label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#88AAFF", new Color?()));
        ((Control) label3).Margin = new Thickness(50f, 10f, 0.0f, 5f);
        Label label4 = label3;
        ((Control) permissionsSection).AddChild((Control) label4);
        AlternatingBGContainer alternatingBgContainer1 = new AlternatingBGContainer();
        alternatingBgContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
        alternatingBgContainer1.HorizontalExpand = true;
        ((Control) alternatingBgContainer1).Margin = new Thickness(50f, 0.0f, 50f, 0.0f);
        AlternatingBGContainer alternatingBgContainer2 = alternatingBgContainer1;
        foreach (SponsorPermissionDetailInfo detail in (IEnumerable<SponsorPermissionDetailInfo>) grouping)
          alternatingBgContainer2.AddControl((Control) this.CreatePermissionRow(detail, false));
        ((Control) permissionsSection).AddChild((Control) alternatingBgContainer2);
      }
    }
    else
    {
      Label label5 = new Label();
      label5.Text = "Станьте спонсором, чтобы получить привилегии!";
      label5.FontColorOverride = new Color?(Color.Gray);
      ((Control) label5).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label5).Margin = new Thickness(0.0f, 15f, 0.0f, 15f);
      Label label6 = label5;
      ((Control) permissionsSection).AddChild((Control) label6);
    }
    return permissionsSection;
  }

  private BoxContainer CreateSponsorButton()
  {
    BoxContainer sponsorButton = new BoxContainer();
    sponsorButton.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) sponsorButton).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) sponsorButton).Margin = new Thickness(0.0f, 10f, 0.0f, 20f);
    Button button1 = new Button();
    button1.Text = "♥ СТАТЬ СПОНСОРОМ";
    ((Control) button1).MinWidth = 250f;
    ((Control) button1).MinHeight = 50f;
    Button button2 = button1;
    ((Control) button2).StyleClasses.Add("ButtonColorGreen");
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => IoCManager.Resolve<IUriOpener>().OpenUri(new Uri("https://discord.gg/xdQ4vSKRB8")));
    ((Control) sponsorButton).AddChild((Control) button2);
    return sponsorButton;
  }

  private BoxContainer CreatePermissionRow(SponsorPermissionDetailInfo detail, bool showSource = true)
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer).HorizontalExpand = true;
    BoxContainer permissionRow = boxContainer;
    Label label1 = new Label();
    label1.Text = detail.Name;
    ((Control) label1).HorizontalExpand = true;
    ((Control) label1).Margin = new Thickness(10f, 5f, 10f, 5f);
    Label label2 = label1;
    Label label3 = new Label();
    string str1;
    if (detail.StackCount <= 1)
      str1 = "✓";
    else
      str1 = $"x{detail.StackCount}";
    label3.Text = str1;
    label3.FontColorOverride = new Color?(Color.LimeGreen);
    ((Control) label3).Margin = new Thickness(10f, 5f, 10f, 5f);
    ((Control) label3).MinWidth = 40f;
    Label label4 = label3;
    ((Control) permissionRow).AddChild((Control) label2);
    ((Control) permissionRow).AddChild((Control) label4);
    if (showSource)
    {
      Label label5 = new Label();
      label5.Text = detail.Source;
      label5.FontColorOverride = new Color?(Color.Gray);
      ((Control) label5).Margin = new Thickness(10f, 5f, 10f, 5f);
      ((Control) label5).MinWidth = 120f;
      Label label6 = label5;
      ((Control) permissionRow).AddChild((Control) label6);
    }
    DateTime? expiresAt = detail.ExpiresAt;
    if (expiresAt.HasValue)
    {
      expiresAt = detail.ExpiresAt;
      TimeSpan timeSpan = expiresAt.Value - DateTime.UtcNow;
      string str2;
      if (timeSpan.TotalDays >= 1.0)
        str2 = $"{(int) timeSpan.TotalDays}д";
      else if (timeSpan.TotalHours >= 1.0)
        str2 = $"{(int) timeSpan.TotalHours}ч";
      else
        str2 = "<1ч";
      Label label7 = new Label();
      label7.Text = "⏱ " + str2;
      label7.FontColorOverride = new Color?(timeSpan.TotalDays < 3.0 ? Color.Orange : Color.Gray);
      ((Control) label7).Margin = new Thickness(10f, 5f, 10f, 5f);
      ((Control) label7).MinWidth = 60f;
      Label label8 = label7;
      ((Control) permissionRow).AddChild((Control) label8);
    }
    return permissionRow;
  }

  public void UpdateSponsorData(
    Dictionary<string, int> permissions,
    List<SponsorPermissionDetailInfo> permissionDetails,
    SponsorTierInfo? displayTier,
    List<SponsorActiveTierInfo> activeTiers,
    SponsorDisplayMode displayMode,
    string? preferredTierKey)
  {
    this._sponsorPermissions = permissions;
    this._sponsorPermissionDetails = permissionDetails;
    this._sponsorDisplayTier = displayTier;
    this._sponsorActiveTiers = activeTiers;
    this._sponsorDisplayMode = displayMode;
    this._sponsorPreferredTierKey = preferredTierKey;
    this._sponsorDisplayUpdating = false;
    if (this._currentSubcategory != ProfileSubcategory.Sponsors)
      return;
    this.LoadSubcategory(ProfileSubcategory.Sponsors);
  }

  public void UpdateStatsData(
    int totalGames,
    int wins,
    int totalKills,
    int totalDamage,
    int totalSurvivalTime,
    int totalCaseDropSkins,
    int unlockedCaseDropSkins,
    int totalEmotes,
    int availableEmotes,
    int totalDeaths,
    int reputation)
  {
    this._totalGames = totalGames;
    this._wins = wins;
    this._totalKills = totalKills;
    this._totalDamage = totalDamage;
    this._avgSurvivalTime = totalSurvivalTime;
    this._totalCaseDropSkins = totalCaseDropSkins;
    this._unlockedCaseDropSkins = unlockedCaseDropSkins;
    this._totalEmotes = totalEmotes;
    this._availableEmotes = availableEmotes;
    this._totalDeaths = totalDeaths;
    this._reputation = reputation;
    if (this._currentSubcategory != ProfileSubcategory.Stats)
      return;
    this.LoadSubcategory(ProfileSubcategory.Stats);
  }

  public void UpdateMatchHistoryData(List<MatchHistoryInfo> matchHistory)
  {
    this._matchHistory = matchHistory;
    if (this._currentSubcategory != ProfileSubcategory.Stats)
      return;
    this.LoadSubcategory(ProfileSubcategory.Stats);
  }

  public void UpdateLeaderboardData(
    List<LeaderboardEntryInfo> leaderboard,
    int playerRank,
    int playerRating)
  {
    this._leaderboard = leaderboard;
    this._playerRank = playerRank;
    this._playerRating = playerRating;
    if (this._currentSubcategory != ProfileSubcategory.Stats)
      return;
    this.LoadSubcategory(ProfileSubcategory.Stats);
  }

  private BoxContainer CreateStatRow(string label, string value)
  {
    BoxContainer statRow = new BoxContainer();
    statRow.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) statRow).HorizontalExpand = true;
    Label label1 = new Label();
    label1.Text = label;
    ((Control) label1).HorizontalExpand = true;
    ((Control) label1).Margin = new Thickness(10f, 5f, 10f, 5f);
    Label label2 = label1;
    Label label3 = new Label();
    label3.Text = value;
    label3.FontColorOverride = new Color?(Color.LightGreen);
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 3;
    ((Control) label3).Margin = new Thickness(10f, 5f, 10f, 5f);
    Label label4 = label3;
    ((Control) statRow).AddChild((Control) label2);
    ((Control) statRow).AddChild((Control) label4);
    return statRow;
  }
}
