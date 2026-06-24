// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.BattlePassTab
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.BattlePass;
using Content.Shared._PUBG.BattlePass;
using Content.Shared.Actions.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class BattlePassTab : BoxContainer
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private SpriteSystem? _spriteSystem;
  private BattlePassStateMessage? _cachedState;
  private bool _isLoading;
  private DateTime _tasksEndAt;
  private Label? _timerLabel;
  private ScrollContainer? _rewardsScroll;
  private int _currentLevelIndex;
  private const string CoinsProtoId = "SpaceCash";
  private const string DiamondsProtoId = "MaterialDiamond1";
  private const string ScrapProtoId = "SheetSteel1";
  private const int LevelNodeWidth = 100;
  private const int LevelNodeSpacing = 15;
  private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?());
  private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?());
  private static readonly Color RedDanger = Color.FromHex((ReadOnlySpan<char>) "#FF4444", new Color?());
  private static readonly Color OrangeWarning = Color.FromHex((ReadOnlySpan<char>) "#FF9500", new Color?());
  private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>) "#0a0a0f", new Color?());
  private static readonly Color DisabledColor = Color.FromHex((ReadOnlySpan<char>) "#4a4a5a", new Color?());
  private static readonly Color EvenRowColor = Color.FromHex((ReadOnlySpan<char>) "#12121a", new Color?());
  private static readonly Color OddRowColor = Color.FromHex((ReadOnlySpan<char>) "#1a1a24", new Color?());
  private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>) "#2a2a3a", new Color?());
  private static readonly Color CardHoverColor = Color.FromHex((ReadOnlySpan<char>) "#252530", new Color?());
  private static readonly Color AccentGlow = Color.FromHex((ReadOnlySpan<char>) "#FFD700", new Color?());
  private static readonly Color PremiumGlow = Color.FromHex((ReadOnlySpan<char>) "#FF8C00", new Color?());
  private static readonly Color ProgressBg = Color.FromHex((ReadOnlySpan<char>) "#0d0d15", new Color?());
  private static readonly Color CompletedGlow = Color.FromHex((ReadOnlySpan<char>) "#00FFB3", new Color?());

  private static Color LerpColor(Color a, Color b, float t)
  {
    t = Math.Clamp(t, 0.0f, 1f);
    return new Color(a.R + (b.R - a.R) * t, a.G + (b.G - a.G) * t, a.B + (b.B - a.B) * t, a.A + (b.A - a.A) * t);
  }

  public BattlePassTab()
  {
    IoCManager.InjectDependencies<BattlePassTab>(this);
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
  }

  private SpriteSystem SpriteSystem
  {
    get => this._spriteSystem ?? (this._spriteSystem = this._entityManager.System<SpriteSystem>());
  }

  public void UpdateState(BattlePassStateMessage state)
  {
    this._cachedState = state;
    this._tasksEndAt = state.TasksEndAt;
    this._isLoading = false;
    this.BuildUI();
  }

  public void LoadSubcategory(BattlePassSubcategory subcategory)
  {
    ((Control) this).RemoveAllChildren();
    if (this._cachedState == null)
    {
      this.ShowLoading();
      this.RequestData();
    }
    else
      this.BuildUI();
  }

  private void RequestData()
  {
    if (this._isLoading)
      return;
    this._isLoading = true;
    this._entityManager.System<BattlePassSystem>().RequestBattlePass();
  }

  private void ShowLoading()
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer1).VerticalAlignment = (Control.VAlignment) 2;
    BoxContainer boxContainer2 = boxContainer1;
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).MinWidth = 300f;
    ((Control) panelContainer1).MinHeight = 100f;
    PanelContainer panelContainer2 = panelContainer1;
    ((Control) panelContainer2).StyleClasses.Add("AngleRect");
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer3).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer3).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer3).Margin = new Thickness(20f);
    BoxContainer boxContainer4 = boxContainer3;
    Label label1 = new Label();
    label1.Text = "◐";
    label1.FontColorOverride = new Color?(BattlePassTab.GoldAccent);
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeadingBigger");
    Label label3 = new Label();
    label3.Text = Loc.GetString("pubg-bp-error-load-failed");
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    label3.FontColorOverride = new Color?(Color.Gray);
    ((Control) label3).Margin = new Thickness(0.0f, 10f, 0.0f, 0.0f);
    Label label4 = label3;
    ((Control) boxContainer4).AddChild((Control) label2);
    ((Control) boxContainer4).AddChild((Control) label4);
    ((Control) panelContainer2).AddChild((Control) boxContainer4);
    ((Control) boxContainer2).AddChild((Control) panelContainer2);
    ((Control) this).AddChild((Control) boxContainer2);
  }

  private void BuildUI()
  {
    ((Control) this).RemoveAllChildren();
    BattlePassStateMessage cachedState = this._cachedState;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer header = this.CreateHeader(cachedState);
    ((Control) boxContainer2).AddChild((Control) header);
    Control goldDivider = this.CreateGoldDivider();
    ((Control) boxContainer2).AddChild(goldDivider);
    Control rewardsTimeline = this.CreateRewardsTimeline(cachedState);
    ((Control) boxContainer2).AddChild(rewardsTimeline);
    Control tasksSection = this.CreateTasksSection(cachedState);
    ((Control) boxContainer2).AddChild(tasksSection);
    Control footer = this.CreateFooter(cachedState);
    ((Control) boxContainer2).AddChild(footer);
    ((Control) this).AddChild((Control) boxContainer2);
    this.ScrollToCurrentLevel(cachedState);
  }

  private Control CreateGoldDivider()
  {
    PanelContainer goldDivider = new PanelContainer();
    ((Control) goldDivider).MinHeight = 2f;
    ((Control) goldDivider).HorizontalExpand = true;
    ((Control) goldDivider).Margin = new Thickness(10f, 5f, 10f, 5f);
    goldDivider.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = BattlePassTab.GoldAccent
    };
    return (Control) goldDivider;
  }

  private Control CreateDivider()
  {
    PanelContainer divider = new PanelContainer();
    ((Control) divider).MinHeight = 1f;
    ((Control) divider).HorizontalExpand = true;
    ((Control) divider).Margin = new Thickness(10f, 3f, 10f, 3f);
    divider.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = BattlePassTab.CardBorderColor
    };
    return (Control) divider;
  }

  private BoxContainer CreateHeader(BattlePassStateMessage state)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(10f, 10f, 10f, 0.0f);
    BoxContainer header = boxContainer1;
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).HorizontalExpand = true;
    panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = BattlePassTab.DarkPanel,
      BorderColor = BattlePassTab.GoldAccent,
      BorderThickness = new Thickness(0.0f, 0.0f, 0.0f, 2f)
    };
    PanelContainer panelContainer2 = panelContainer1;
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer2).HorizontalExpand = true;
    ((Control) boxContainer2).Margin = new Thickness(15f, 12f, 15f, 12f);
    BoxContainer boxContainer3 = boxContainer2;
    BoxContainer boxContainer4 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    BoxContainer boxContainer5 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0
    };
    Label label1 = new Label()
    {
      Text = Loc.GetString("pubg-bp-season", new (string, object)[1]
      {
        ("name", (object) state.SeasonName)
      }),
      FontColorOverride = new Color?(Color.White)
    };
    ((Control) label1).SetOnlyStyleClass("LabelHeadingBigger");
    ((Control) boxContainer5).AddChild((Control) label1);
    if (state.HasPremium)
    {
      PanelContainer panelContainer3 = new PanelContainer();
      ((Control) panelContainer3).Margin = new Thickness(15f, 0.0f, 0.0f, 0.0f);
      StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
      styleBoxFlat.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1a1208", new Color?());
      styleBoxFlat.BorderColor = BattlePassTab.AccentGlow;
      styleBoxFlat.BorderThickness = new Thickness(2f);
      ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(10f);
      ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(10f);
      ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(4f);
      ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(4f);
      panelContainer3.PanelOverride = (StyleBox) styleBoxFlat;
      PanelContainer panelContainer4 = panelContainer3;
      BoxContainer boxContainer6 = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 0
      };
      Label label2 = new Label()
      {
        Text = "★ ",
        FontColorOverride = new Color?(BattlePassTab.AccentGlow)
      };
      ((Control) label2).SetOnlyStyleClass("LabelHeading");
      ((Control) boxContainer6).AddChild((Control) label2);
      Label label3 = new Label()
      {
        Text = Loc.GetString("pubg-bp-premium"),
        FontColorOverride = new Color?(BattlePassTab.GoldAccent)
      };
      ((Control) label3).SetOnlyStyleClass("LabelHeading");
      ((Control) boxContainer6).AddChild((Control) label3);
      ((Control) panelContainer4).AddChild((Control) boxContainer6);
      ((Control) boxContainer5).AddChild((Control) panelContainer4);
    }
    ((Control) boxContainer4).AddChild((Control) boxContainer5);
    ((Control) boxContainer3).AddChild((Control) boxContainer4);
    Control control = new Control()
    {
      HorizontalExpand = true
    };
    ((Control) boxContainer3).AddChild(control);
    int num1 = 0;
    int num2 = 0;
    foreach (BattlePassLevelInfo battlePassLevelInfo in (IEnumerable<BattlePassLevelInfo>) state.Levels.OrderBy<BattlePassLevelInfo, int>((Func<BattlePassLevelInfo, int>) (l => l.Level)))
    {
      if (battlePassLevelInfo.Level <= state.CurrentLevel)
        num1 += battlePassLevelInfo.XpRequired;
      if (battlePassLevelInfo.Level <= state.CurrentLevel + 1)
        num2 += battlePassLevelInfo.XpRequired;
    }
    int num3 = state.CurrentXp - num1;
    int val2 = num2 - num1;
    if (val2 <= 0)
    {
      BattlePassLevelInfo battlePassLevelInfo = state.Levels.LastOrDefault<BattlePassLevelInfo>();
      val2 = battlePassLevelInfo != null ? battlePassLevelInfo.XpRequired : 100;
    }
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer7).VerticalAlignment = (Control.VAlignment) 2;
    BoxContainer boxContainer8 = boxContainer7;
    PanelContainer panelContainer5 = new PanelContainer();
    ((Control) panelContainer5).Margin = new Thickness(0.0f, 0.0f, 15f, 0.0f);
    StyleBoxFlat styleBoxFlat1 = new StyleBoxFlat();
    styleBoxFlat1.BackgroundColor = BattlePassTab.GoldAccent;
    styleBoxFlat1.BorderColor = BattlePassTab.AccentGlow;
    styleBoxFlat1.BorderThickness = new Thickness(2f);
    ((StyleBox) styleBoxFlat1).ContentMarginLeftOverride = new float?(14f);
    ((StyleBox) styleBoxFlat1).ContentMarginRightOverride = new float?(14f);
    ((StyleBox) styleBoxFlat1).ContentMarginTopOverride = new float?(8f);
    ((StyleBox) styleBoxFlat1).ContentMarginBottomOverride = new float?(8f);
    panelContainer5.PanelOverride = (StyleBox) styleBoxFlat1;
    PanelContainer panelContainer6 = panelContainer5;
    Label label4 = new Label()
    {
      Text = Loc.GetString("pubg-bp-level", new (string, object)[1]
      {
        ("level", (object) state.CurrentLevel)
      }),
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#0a0a0f", new Color?()))
    };
    ((Control) label4).SetOnlyStyleClass("LabelHeadingBigger");
    ((Control) panelContainer6).AddChild((Control) label4);
    ((Control) boxContainer8).AddChild((Control) panelContainer6);
    BoxContainer boxContainer9 = new BoxContainer();
    boxContainer9.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer9).MinWidth = 220f;
    BoxContainer boxContainer10 = boxContainer9;
    PanelContainer panelContainer7 = new PanelContainer();
    ((Control) panelContainer7).MinHeight = 24f;
    ((Control) panelContainer7).HorizontalExpand = true;
    panelContainer7.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = BattlePassTab.ProgressBg,
      BorderColor = BattlePassTab.CardBorderColor,
      BorderThickness = new Thickness(1f, 1f, 1f, 1f)
    };
    PanelContainer panelContainer8 = panelContainer7;
    int num4 = Math.Max(0, Math.Min(num3, val2));
    float num5 = val2 > 0 ? (float) num4 / (float) val2 : 0.0f;
    Color color = (double) num5 >= 0.89999997615814209 ? BattlePassTab.CompletedGlow : ((double) num5 >= 0.60000002384185791 ? BattlePassTab.LerpColor(BattlePassTab.OrangeWarning, BattlePassTab.GreenSuccess, (float) (((double) num5 - 0.60000002384185791) / 0.30000001192092896)) : BattlePassTab.OrangeWarning);
    PanelContainer panelContainer9 = new PanelContainer();
    ((Control) panelContainer9).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) panelContainer9).MinHeight = 22f;
    ((Control) panelContainer9).MinWidth = (float) (int) (218.0 * (double) num5);
    panelContainer9.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = color
    };
    PanelContainer panelContainer10 = panelContainer9;
    Label label5 = new Label();
    label5.Text = Loc.GetString("pubg-bp-xp", new (string, object)[2]
    {
      ("current", (object) Math.Max(0, num3)),
      ("required", (object) val2)
    });
    label5.FontColorOverride = new Color?(Color.White);
    ((Control) label5).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label5).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) label5).Margin = new Thickness(0.0f, 2f, 0.0f, 0.0f);
    Label label6 = label5;
    ((Control) panelContainer8).AddChild((Control) panelContainer10);
    ((Control) panelContainer8).AddChild((Control) label6);
    ((Control) boxContainer10).AddChild((Control) panelContainer8);
    ((Control) boxContainer8).AddChild((Control) boxContainer10);
    ((Control) boxContainer3).AddChild((Control) boxContainer8);
    ((Control) panelContainer2).AddChild((Control) boxContainer3);
    ((Control) header).AddChild((Control) panelContainer2);
    return header;
  }

  private Control CreateRewardsTimeline(BattlePassStateMessage state)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(10f, 10f, 10f, 5f);
    ((Control) boxContainer1).MinHeight = 290f;
    BoxContainer rewardsTimeline = boxContainer1;
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer2).MinWidth = 85f;
    ((Control) boxContainer2).Margin = new Thickness(0.0f, 0.0f, 10f, 0.0f);
    BoxContainer boxContainer3 = boxContainer2;
    Control rowLabel1 = this.CreateRowLabel(Loc.GetString("pubg-bp-free"), BattlePassTab.GreenSuccess, 90);
    ((Control) boxContainer3).AddChild(rowLabel1);
    Control control = new Control() { MinHeight = 55f };
    ((Control) boxContainer3).AddChild(control);
    Control rowLabel2 = this.CreateRowLabel("★ " + Loc.GetString("pubg-bp-premium"), BattlePassTab.GoldAccent, 90);
    ((Control) boxContainer3).AddChild(rowLabel2);
    ((Control) rewardsTimeline).AddChild((Control) boxContainer3);
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).VerticalExpand = true;
    panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = BattlePassTab.ProgressBg,
      BorderColor = BattlePassTab.CardBorderColor,
      BorderThickness = new Thickness(2f)
    };
    PanelContainer panelContainer2 = panelContainer1;
    ScrollContainer scrollContainer = new ScrollContainer();
    ((Control) scrollContainer).HorizontalExpand = true;
    ((Control) scrollContainer).VerticalExpand = true;
    scrollContainer.HScrollEnabled = true;
    scrollContainer.VScrollEnabled = false;
    ((Control) scrollContainer).Margin = new Thickness(5f);
    this._rewardsScroll = scrollContainer;
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer4).Margin = new Thickness(10f, 5f, 20f, 5f);
    BoxContainer boxContainer5 = boxContainer4;
    this._currentLevelIndex = 0;
    for (int index = 0; index < state.Levels.Count; ++index)
    {
      BattlePassLevelInfo level = state.Levels[index];
      if (level.Level == state.CurrentLevel)
        this._currentLevelIndex = index;
      Control levelNode = this.CreateLevelNode(level, state, index < state.Levels.Count - 1);
      ((Control) boxContainer5).AddChild(levelNode);
    }
    ((Control) this._rewardsScroll).AddChild((Control) boxContainer5);
    ((Control) panelContainer2).AddChild((Control) this._rewardsScroll);
    ((Control) rewardsTimeline).AddChild((Control) panelContainer2);
    return (Control) rewardsTimeline;
  }

  private Control CreateRowLabel(string text, Color color, int height)
  {
    PanelContainer rowLabel = new PanelContainer();
    ((Control) rowLabel).MinHeight = (float) height;
    ((Control) rowLabel).VerticalAlignment = (Control.VAlignment) 2;
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#15151a", new Color?());
    styleBoxFlat.BorderColor = color;
    styleBoxFlat.BorderThickness = new Thickness(0.0f, 0.0f, 2f, 0.0f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(8f);
    rowLabel.PanelOverride = (StyleBox) styleBoxFlat;
    Label label = new Label();
    label.Text = text;
    label.FontColorOverride = new Color?(color);
    ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 3;
    ((Control) label).Margin = new Thickness(5f);
    ((Control) rowLabel).AddChild((Control) label);
    return (Control) rowLabel;
  }

  private Control CreateLevelNode(
    BattlePassLevelInfo level,
    BattlePassStateMessage state,
    bool hasNext)
  {
    bool flag1 = state.CurrentLevel >= level.Level;
    bool flag2 = state.CurrentLevel == level.Level;
    List<BattlePassRewardInfo> list1 = level.Rewards.Where<BattlePassRewardInfo>((Func<BattlePassRewardInfo, bool>) (r => !r.IsPremium)).ToList<BattlePassRewardInfo>();
    List<BattlePassRewardInfo> list2 = level.Rewards.Where<BattlePassRewardInfo>((Func<BattlePassRewardInfo, bool>) (r => r.IsPremium)).ToList<BattlePassRewardInfo>();
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).MinWidth = (float) (100 + (hasNext ? 45 : 0));
    BoxContainer levelNode = boxContainer1;
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer2).MinWidth = 100f;
    ((Control) boxContainer2).HorizontalAlignment = (Control.HAlignment) 2;
    BoxContainer boxContainer3 = boxContainer2;
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer4).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer4).MinHeight = 90f;
    BoxContainer boxContainer5 = boxContainer4;
    if (list1.Count > 0)
    {
      foreach (BattlePassRewardInfo reward in list1)
      {
        Control timelineRewardCard = this.CreateTimelineRewardCard(reward, level, state);
        ((Control) boxContainer5).AddChild(timelineRewardCard);
      }
    }
    else
    {
      Control emptyRewardSlot = this.CreateEmptyRewardSlot();
      ((Control) boxContainer5).AddChild(emptyRewardSlot);
    }
    ((Control) boxContainer3).AddChild((Control) boxContainer5);
    BoxContainer boxContainer6 = new BoxContainer();
    boxContainer6.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer6).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer6).Margin = new Thickness(0.0f, 5f, 0.0f, 5f);
    BoxContainer boxContainer7 = boxContainer6;
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).MinWidth = 36f;
    ((Control) panelContainer1).MinHeight = 36f;
    PanelContainer panelContainer2 = panelContainer1;
    Color color1;
    Color color2;
    string str;
    if (flag2)
    {
      color1 = BattlePassTab.AccentGlow;
      color2 = BattlePassTab.GoldAccent;
      str = "▶";
    }
    else if (flag1)
    {
      color1 = BattlePassTab.CompletedGlow;
      color2 = Color.FromHex((ReadOnlySpan<char>) "#14141f", new Color?());
      str = "✓";
    }
    else
    {
      color1 = BattlePassTab.DisabledColor;
      color2 = Color.FromHex((ReadOnlySpan<char>) "#0a0a12", new Color?());
      str = "○";
    }
    panelContainer2.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = color2,
      BorderColor = color1,
      BorderThickness = new Thickness(flag2 ? 3f : 2f)
    };
    Label label1 = new Label();
    label1.Text = str;
    label1.FontColorOverride = new Color?(flag2 ? Color.Black : color1);
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).VerticalAlignment = (Control.VAlignment) 2;
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass(flag2 ? "LabelHeading" : "LabelSmall");
    ((Control) panelContainer2).AddChild((Control) label2);
    ((Control) boxContainer7).AddChild((Control) panelContainer2);
    ((Control) boxContainer3).AddChild((Control) boxContainer7);
    BoxContainer boxContainer8 = new BoxContainer();
    boxContainer8.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer8).HorizontalAlignment = (Control.HAlignment) 2;
    BoxContainer boxContainer9 = boxContainer8;
    Label label3 = new Label();
    label3.Text = level.Level.ToString();
    label3.FontColorOverride = new Color?(flag2 ? BattlePassTab.GoldAccent : (flag1 ? BattlePassTab.GreenSuccess : BattlePassTab.DisabledColor));
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    Label label4 = label3;
    ((Control) label4).SetOnlyStyleClass("LabelHeading");
    ((Control) boxContainer9).AddChild((Control) label4);
    ((Control) boxContainer3).AddChild((Control) boxContainer9);
    BoxContainer boxContainer10 = new BoxContainer();
    boxContainer10.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer10).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer10).MinHeight = 90f;
    ((Control) boxContainer10).Margin = new Thickness(0.0f, 10f, 0.0f, 0.0f);
    BoxContainer boxContainer11 = boxContainer10;
    if (list2.Count > 0)
    {
      foreach (BattlePassRewardInfo reward in list2)
      {
        Control timelineRewardCard = this.CreateTimelineRewardCard(reward, level, state);
        ((Control) boxContainer11).AddChild(timelineRewardCard);
      }
    }
    else
    {
      Control emptyRewardSlot = this.CreateEmptyRewardSlot();
      ((Control) boxContainer11).AddChild(emptyRewardSlot);
    }
    ((Control) boxContainer3).AddChild((Control) boxContainer11);
    ((Control) levelNode).AddChild((Control) boxContainer3);
    if (hasNext)
    {
      Color color3 = flag1 ? BattlePassTab.GreenSuccess : Color.FromHex((ReadOnlySpan<char>) "#1a1a24", new Color?());
      Color color4 = flag1 ? BattlePassTab.CompletedGlow : Color.FromHex((ReadOnlySpan<char>) "#2a2a3a", new Color?());
      PanelContainer panelContainer3 = new PanelContainer();
      ((Control) panelContainer3).MinWidth = 45f;
      ((Control) panelContainer3).MinHeight = 4f;
      ((Control) panelContainer3).VerticalAlignment = (Control.VAlignment) 2;
      panelContainer3.PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = color3,
        BorderColor = color4,
        BorderThickness = new Thickness(0.0f, 1f, 0.0f, 1f)
      };
      PanelContainer panelContainer4 = panelContainer3;
      ((Control) levelNode).AddChild((Control) panelContainer4);
    }
    return (Control) levelNode;
  }

  private Control CreateEmptyRewardSlot()
  {
    PanelContainer emptyRewardSlot = new PanelContainer();
    ((Control) emptyRewardSlot).MinWidth = 55f;
    ((Control) emptyRewardSlot).MinHeight = 70f;
    ((Control) emptyRewardSlot).Margin = new Thickness(2f);
    emptyRewardSlot.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#18181c", new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) "#2a2a2a", new Color?()),
      BorderThickness = new Thickness(1f, 0.0f, 1f, 0.0f)
    };
    Label label = new Label();
    label.Text = "—";
    label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#3a3a3a", new Color?()));
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) emptyRewardSlot).AddChild((Control) label);
    return (Control) emptyRewardSlot;
  }

  private Control CreateTimelineRewardCard(
    BattlePassRewardInfo reward,
    BattlePassLevelInfo level,
    BattlePassStateMessage state)
  {
    bool flag1 = state.CurrentLevel >= level.Level;
    bool flag2 = state.ClaimedRewardIds.Contains(reward.Id);
    bool flag3 = !reward.IsPremium || state.HasPremium;
    Color color1;
    Color color2;
    if (flag2)
    {
      color1 = Color.FromHex((ReadOnlySpan<char>) "#1a2a2a", new Color?());
      color2 = Color.FromHex((ReadOnlySpan<char>) "#0f0f18", new Color?());
    }
    else if (flag1 & flag3)
    {
      color1 = reward.IsPremium ? BattlePassTab.GoldAccent : BattlePassTab.GreenSuccess;
      color2 = Color.FromHex((ReadOnlySpan<char>) "#14141f", new Color?());
    }
    else
    {
      color1 = BattlePassTab.CardBorderColor;
      color2 = Color.FromHex((ReadOnlySpan<char>) "#0a0a12", new Color?());
    }
    PubgAnimatedRewardCard timelineRewardCard = new PubgAnimatedRewardCard();
    timelineRewardCard.CanClaim = flag1;
    timelineRewardCard.IsClaimed = flag2;
    timelineRewardCard.IsPremium = reward.IsPremium;
    timelineRewardCard.CanClaimPremium = flag3;
    timelineRewardCard.OnClaimPressed += (Action) (() => this.ClaimReward(reward.Id));
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).MinWidth = 60f;
    ((Control) panelContainer).MinHeight = 80f;
    ((Control) panelContainer).Margin = new Thickness(2f);
    panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = color2,
      BorderColor = color1,
      BorderThickness = new Thickness(!(flag1 & flag3) || flag2 ? 1f : 2f)
    };
    PanelContainer content = panelContainer;
    if (flag2)
      ((Control) content).Modulate = new Color(0.6f, 0.6f, 0.6f, 1f);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer1).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer1).Margin = new Thickness(4f);
    BoxContainer boxContainer2 = boxContainer1;
    if (reward.IsPremium)
    {
      Label label1 = new Label();
      label1.Text = "★";
      label1.FontColorOverride = new Color?(BattlePassTab.GoldAccent);
      ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
      Label label2 = label1;
      ((Control) boxContainer2).AddChild((Control) label2);
    }
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer3).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer3).MinHeight = 32f;
    BoxContainer boxContainer4 = boxContainer3;
    Control rewardIcon = this.CreateRewardIcon(reward);
    if (rewardIcon != null)
    {
      ((Control) boxContainer4).AddChild(rewardIcon);
    }
    else
    {
      Label label3 = new Label();
      label3.Text = this.GetRewardEmoji(reward.RewardType);
      ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
      Label label4 = label3;
      ((Control) boxContainer4).AddChild((Control) label4);
    }
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    string shortRewardText = this.GetShortRewardText(reward);
    if (!string.IsNullOrEmpty(shortRewardText))
    {
      Label label5 = new Label();
      label5.Text = shortRewardText;
      label5.FontColorOverride = new Color?(Color.White);
      ((Control) label5).HorizontalAlignment = (Control.HAlignment) 2;
      Label label6 = label5;
      ((Control) boxContainer2).AddChild((Control) label6);
    }
    if (reward.Duration.HasValue)
    {
      Label label7 = new Label();
      label7.Text = $"{reward.Duration}d";
      label7.FontColorOverride = new Color?(BattlePassTab.OrangeWarning);
      ((Control) label7).HorizontalAlignment = (Control.HAlignment) 2;
      Label label8 = label7;
      ((Control) boxContainer2).AddChild((Control) label8);
    }
    if (flag2)
    {
      Label label9 = new Label();
      label9.Text = "✓";
      label9.FontColorOverride = new Color?(BattlePassTab.GreenSuccess);
      ((Control) label9).HorizontalAlignment = (Control.HAlignment) 2;
      Label label10 = label9;
      ((Control) boxContainer2).AddChild((Control) label10);
    }
    else if (flag1 & flag3)
    {
      Button button1 = new Button();
      button1.Text = "▼";
      ((Control) button1).MinWidth = 35f;
      ((Control) button1).MinHeight = 20f;
      ((Control) button1).Modulate = reward.IsPremium ? BattlePassTab.GoldAccent : BattlePassTab.GreenSuccess;
      Button button2 = button1;
      ((Control) button2).StyleClasses.Add("ButtonColorGreen");
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ClaimReward(reward.Id));
      ((Control) boxContainer2).AddChild((Control) button2);
    }
    else if (!flag3)
    {
      Label label11 = new Label();
      label11.Text = "\uD83D\uDD12";
      label11.FontColorOverride = new Color?(BattlePassTab.GoldAccent);
      ((Control) label11).HorizontalAlignment = (Control.HAlignment) 2;
      Label label12 = label11;
      ((Control) boxContainer2).AddChild((Control) label12);
    }
    ((Control) content).AddChild((Control) boxContainer2);
    timelineRewardCard.SetContent((Control) content);
    return (Control) timelineRewardCard;
  }

  private string GetRewardEmoji(string rewardType)
  {
    string rewardEmoji;
    if (rewardType != null)
    {
      switch (rewardType.Length)
      {
        case 4:
          switch (rewardType[0])
          {
            case 'i':
              if (rewardType == "item")
              {
                rewardEmoji = "\uD83D\uDCE6";
                goto label_19;
              }
              break;
            case 't':
              if (rewardType == "tier")
              {
                rewardEmoji = "\uD83C\uDFC6";
                goto label_19;
              }
              break;
          }
          break;
        case 5:
          switch (rewardType[0])
          {
            case 'c':
              if (rewardType == "coins")
              {
                rewardEmoji = "\uD83D\uDCB0";
                goto label_19;
              }
              break;
            case 'e':
              if (rewardType == "emote")
              {
                rewardEmoji = "\uD83D\uDE00";
                goto label_19;
              }
              break;
            case 's':
              if (rewardType == "scrap")
              {
                rewardEmoji = "⚙";
                goto label_19;
              }
              break;
          }
          break;
        case 10:
          if (rewardType == "permission")
          {
            rewardEmoji = "\uD83D\uDD11";
            goto label_19;
          }
          break;
        case 12:
          if (rewardType == "premiumCoins")
          {
            rewardEmoji = "\uD83D\uDC8E";
            goto label_19;
          }
          break;
      }
    }
    rewardEmoji = "\uD83C\uDF81";
label_19:
    return rewardEmoji;
  }

  private string GetShortRewardText(BattlePassRewardInfo reward)
  {
    string rewardType = reward.RewardType;
    return rewardType == "coins" || rewardType == "scrap" || rewardType == "premiumCoins" ? reward.RewardValue : "";
  }

  private Control CreateTasksSection(BattlePassStateMessage state)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(10f, 5f, 10f, 0.0f);
    BoxContainer tasksSection = boxContainer1;
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).VerticalExpand = true;
    panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = BattlePassTab.DarkPanel,
      BorderColor = BattlePassTab.CardBorderColor,
      BorderThickness = new Thickness(1f)
    };
    PanelContainer panelContainer2 = panelContainer1;
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer2).HorizontalExpand = true;
    ((Control) boxContainer2).Margin = new Thickness(0.0f);
    BoxContainer boxContainer3 = boxContainer2;
    PanelContainer panelContainer3 = new PanelContainer();
    ((Control) panelContainer3).HorizontalExpand = true;
    panelContainer3.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#15151a", new Color?()),
      BorderColor = BattlePassTab.GoldAccent,
      BorderThickness = new Thickness(0.0f, 0.0f, 0.0f, 1f)
    };
    PanelContainer panelContainer4 = panelContainer3;
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer4).HorizontalExpand = true;
    ((Control) boxContainer4).Margin = new Thickness(12f, 8f, 12f, 8f);
    BoxContainer boxContainer5 = boxContainer4;
    Label label1 = new Label();
    label1.Text = Loc.GetString("pubg-bp-tasks-title");
    ((Control) label1).HorizontalExpand = true;
    label1.FontColorOverride = new Color?(Color.White);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    ((Control) boxContainer5).AddChild((Control) label2);
    PanelContainer panelContainer5 = new PanelContainer();
    StyleBoxFlat styleBoxFlat1 = new StyleBoxFlat();
    styleBoxFlat1.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1a1a20", new Color?());
    ((StyleBox) styleBoxFlat1).ContentMarginLeftOverride = new float?(8f);
    ((StyleBox) styleBoxFlat1).ContentMarginRightOverride = new float?(8f);
    ((StyleBox) styleBoxFlat1).ContentMarginTopOverride = new float?(2f);
    ((StyleBox) styleBoxFlat1).ContentMarginBottomOverride = new float?(2f);
    panelContainer5.PanelOverride = (StyleBox) styleBoxFlat1;
    PanelContainer panelContainer6 = panelContainer5;
    BoxContainer boxContainer6 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0
    };
    Label label3 = new Label()
    {
      Text = "⏱ ",
      FontColorOverride = new Color?(BattlePassTab.GoldAccent)
    };
    ((Control) boxContainer6).AddChild((Control) label3);
    this._timerLabel = new Label()
    {
      Text = this.GetTimeRemaining(),
      FontColorOverride = new Color?(BattlePassTab.GoldAccent)
    };
    ((Control) boxContainer6).AddChild((Control) this._timerLabel);
    ((Control) panelContainer6).AddChild((Control) boxContainer6);
    ((Control) boxContainer5).AddChild((Control) panelContainer6);
    if (state.HasPremium)
    {
      PanelContainer panelContainer7 = new PanelContainer();
      ((Control) panelContainer7).Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
      StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat();
      styleBoxFlat2.BackgroundColor = state.SkipsRemaining > 0 ? Color.FromHex((ReadOnlySpan<char>) "#1a2a1a", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#1a1a1a", new Color?());
      ((StyleBox) styleBoxFlat2).ContentMarginLeftOverride = new float?(8f);
      ((StyleBox) styleBoxFlat2).ContentMarginRightOverride = new float?(8f);
      ((StyleBox) styleBoxFlat2).ContentMarginTopOverride = new float?(2f);
      ((StyleBox) styleBoxFlat2).ContentMarginBottomOverride = new float?(2f);
      panelContainer7.PanelOverride = (StyleBox) styleBoxFlat2;
      PanelContainer panelContainer8 = panelContainer7;
      Label label4 = new Label()
      {
        Text = Loc.GetString("pubg-bp-tasks-skips", new (string, object)[1]
        {
          ("remaining", (object) state.SkipsRemaining)
        }),
        FontColorOverride = new Color?(state.SkipsRemaining > 0 ? BattlePassTab.GreenSuccess : BattlePassTab.DisabledColor)
      };
      ((Control) panelContainer8).AddChild((Control) label4);
      ((Control) boxContainer5).AddChild((Control) panelContainer8);
    }
    else
    {
      PanelContainer panelContainer9 = new PanelContainer();
      ((Control) panelContainer9).Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
      StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat();
      styleBoxFlat3.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#2a2520", new Color?());
      styleBoxFlat3.BorderColor = BattlePassTab.GoldAccent;
      styleBoxFlat3.BorderThickness = new Thickness(1f);
      ((StyleBox) styleBoxFlat3).ContentMarginLeftOverride = new float?(8f);
      ((StyleBox) styleBoxFlat3).ContentMarginRightOverride = new float?(8f);
      ((StyleBox) styleBoxFlat3).ContentMarginTopOverride = new float?(2f);
      ((StyleBox) styleBoxFlat3).ContentMarginBottomOverride = new float?(2f);
      panelContainer9.PanelOverride = (StyleBox) styleBoxFlat3;
      PanelContainer panelContainer10 = panelContainer9;
      Label label5 = new Label()
      {
        Text = "★ " + Loc.GetString("pubg-bp-tasks-skips-locked"),
        FontColorOverride = new Color?(BattlePassTab.GoldAccent)
      };
      ((Control) panelContainer10).AddChild((Control) label5);
      ((Control) boxContainer5).AddChild((Control) panelContainer10);
    }
    ((Control) panelContainer4).AddChild((Control) boxContainer5);
    ((Control) boxContainer3).AddChild((Control) panelContainer4);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    scrollContainer1.VScrollEnabled = true;
    scrollContainer1.HScrollEnabled = false;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer7).HorizontalExpand = true;
    BoxContainer boxContainer8 = boxContainer7;
    List<BattlePassTaskInfo> list = state.Tasks.Where<BattlePassTaskInfo>((Func<BattlePassTaskInfo, bool>) (t => !t.IsSkipped)).ToList<BattlePassTaskInfo>();
    for (int index = 0; index < list.Count; ++index)
    {
      Control compactTaskRow = this.CreateCompactTaskRow(list[index], state, index % 2 == 0);
      ((Control) boxContainer8).AddChild(compactTaskRow);
    }
    ((Control) scrollContainer2).AddChild((Control) boxContainer8);
    ((Control) boxContainer3).AddChild((Control) scrollContainer2);
    ((Control) panelContainer2).AddChild((Control) boxContainer3);
    ((Control) tasksSection).AddChild((Control) panelContainer2);
    return (Control) tasksSection;
  }

  private Control CreateCompactTaskRow(
    BattlePassTaskInfo task,
    BattlePassStateMessage state,
    bool isEven)
  {
    bool flag1 = task.RequiredPerm != null;
    bool flag2 = !flag1 || state.HasPremium;
    bool flag3 = task.Progress >= task.TargetValue;
    bool xpClaimed = task.XpClaimed;
    PubgTaskRowPanel compactTaskRow = new PubgTaskRowPanel();
    ((Control) compactTaskRow).HorizontalExpand = true;
    compactTaskRow.IsHoverable = !xpClaimed & flag2;
    compactTaskRow.IsCompleted = xpClaimed;
    compactTaskRow.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = (isEven ? BattlePassTab.EvenRowColor : BattlePassTab.OddRowColor),
      BorderColor = (xpClaimed ? BattlePassTab.CompletedGlow : (flag3 & flag2 ? BattlePassTab.GoldAccent : Color.Transparent)),
      BorderThickness = new Thickness(xpClaimed || flag3 & flag2 ? 2f : 0.0f, 0.0f, 0.0f, 0.0f)
    };
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(12f, 8f, 12f, 8f);
    BoxContainer boxContainer2 = boxContainer1;
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).MinWidth = 45f;
    StyleBoxFlat styleBoxFlat1 = new StyleBoxFlat();
    styleBoxFlat1.BackgroundColor = flag1 ? Color.FromHex((ReadOnlySpan<char>) "#2a2520", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#1a1a22", new Color?());
    styleBoxFlat1.BorderColor = flag1 ? BattlePassTab.GoldAccent : BattlePassTab.CardBorderColor;
    styleBoxFlat1.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat1).ContentMarginLeftOverride = new float?(6f);
    ((StyleBox) styleBoxFlat1).ContentMarginRightOverride = new float?(6f);
    ((StyleBox) styleBoxFlat1).ContentMarginTopOverride = new float?(2f);
    ((StyleBox) styleBoxFlat1).ContentMarginBottomOverride = new float?(2f);
    panelContainer1.PanelOverride = (StyleBox) styleBoxFlat1;
    PanelContainer panelContainer2 = panelContainer1;
    Label label1 = new Label();
    string str;
    if (!flag1)
      str = $"#{task.Slot}";
    else
      str = $"★{task.Slot}";
    label1.Text = str;
    label1.FontColorOverride = new Color?(flag1 ? BattlePassTab.GoldAccent : Color.White);
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    Label label2 = label1;
    ((Control) panelContainer2).AddChild((Control) label2);
    ((Control) boxContainer2).AddChild((Control) panelContainer2);
    Label label3 = new Label();
    label3.Text = this.GetTaskDisplayText(task);
    ((Control) label3).HorizontalExpand = true;
    ((Control) label3).Margin = new Thickness(12f, 0.0f, 12f, 0.0f);
    label3.FontColorOverride = new Color?(flag2 ? Color.White : BattlePassTab.DisabledColor);
    Label label4 = label3;
    ((Control) boxContainer2).AddChild((Control) label4);
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer3).MinWidth = 170f;
    BoxContainer boxContainer4 = boxContainer3;
    PanelContainer panelContainer3 = new PanelContainer();
    ((Control) panelContainer3).MinWidth = 100f;
    ((Control) panelContainer3).MinHeight = 18f;
    ((Control) panelContainer3).VerticalAlignment = (Control.VAlignment) 2;
    panelContainer3.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = BattlePassTab.ProgressBg,
      BorderColor = BattlePassTab.CardBorderColor,
      BorderThickness = new Thickness(1f)
    };
    PanelContainer panelContainer4 = panelContainer3;
    float num = task.TargetValue > 0 ? Math.Min((float) task.Progress / (float) task.TargetValue, 1f) : 0.0f;
    Color color = flag3 ? BattlePassTab.CompletedGlow : ((double) num >= 0.75 ? BattlePassTab.LerpColor(BattlePassTab.OrangeWarning, BattlePassTab.GreenSuccess, (float) (((double) num - 0.75) / 0.25)) : BattlePassTab.OrangeWarning);
    PanelContainer panelContainer5 = new PanelContainer();
    ((Control) panelContainer5).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) panelContainer5).MinHeight = 16f;
    ((Control) panelContainer5).MinWidth = (float) (int) (98.0 * (double) num);
    panelContainer5.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = color
    };
    PanelContainer panelContainer6 = panelContainer5;
    ((Control) panelContainer4).AddChild((Control) panelContainer6);
    ((Control) boxContainer4).AddChild((Control) panelContainer4);
    Label label5 = new Label();
    label5.Text = $" {task.Progress}/{task.TargetValue}";
    label5.FontColorOverride = new Color?(flag3 ? BattlePassTab.GreenSuccess : Color.LightGray);
    ((Control) label5).MinWidth = 60f;
    ((Control) label5).Margin = new Thickness(5f, 0.0f, 0.0f, 0.0f);
    Label label6 = label5;
    ((Control) boxContainer4).AddChild((Control) label6);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    PanelContainer panelContainer7 = new PanelContainer();
    ((Control) panelContainer7).MinWidth = 60f;
    ((Control) panelContainer7).Margin = new Thickness(10f, 0.0f, 10f, 0.0f);
    StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat();
    styleBoxFlat2.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1a2a1a", new Color?());
    ((StyleBox) styleBoxFlat2).ContentMarginLeftOverride = new float?(6f);
    ((StyleBox) styleBoxFlat2).ContentMarginRightOverride = new float?(6f);
    ((StyleBox) styleBoxFlat2).ContentMarginTopOverride = new float?(2f);
    ((StyleBox) styleBoxFlat2).ContentMarginBottomOverride = new float?(2f);
    panelContainer7.PanelOverride = (StyleBox) styleBoxFlat2;
    PanelContainer panelContainer8 = panelContainer7;
    Label label7 = new Label();
    label7.Text = $"+{task.XpReward}";
    label7.FontColorOverride = new Color?(BattlePassTab.GoldAccent);
    ((Control) label7).HorizontalAlignment = (Control.HAlignment) 2;
    Label label8 = label7;
    ((Control) panelContainer8).AddChild((Control) label8);
    ((Control) boxContainer2).AddChild((Control) panelContainer8);
    BoxContainer boxContainer5 = new BoxContainer();
    ((Control) boxContainer5).MinWidth = 85f;
    ((Control) boxContainer5).HorizontalAlignment = (Control.HAlignment) 3;
    BoxContainer boxContainer6 = boxContainer5;
    if (!flag2)
    {
      Label label9 = new Label();
      label9.Text = "\uD83D\uDD12";
      label9.FontColorOverride = new Color?(BattlePassTab.GoldAccent);
      ((Control) label9).HorizontalAlignment = (Control.HAlignment) 2;
      Label label10 = label9;
      ((Control) boxContainer6).AddChild((Control) label10);
    }
    else if (xpClaimed)
    {
      Label label11 = new Label();
      label11.Text = "✓ " + Loc.GetString("pubg-bp-task-completed");
      label11.FontColorOverride = new Color?(BattlePassTab.GreenSuccess);
      ((Control) label11).HorizontalAlignment = (Control.HAlignment) 2;
      Label label12 = label11;
      ((Control) boxContainer6).AddChild((Control) label12);
    }
    else if (task.IsSkipped)
    {
      Label label13 = new Label();
      label13.Text = Loc.GetString("pubg-bp-task-skipped");
      label13.FontColorOverride = new Color?(BattlePassTab.DisabledColor);
      ((Control) label13).HorizontalAlignment = (Control.HAlignment) 2;
      Label label14 = label13;
      ((Control) boxContainer6).AddChild((Control) label14);
    }
    else if (flag3)
    {
      Button button1 = new Button();
      button1.Text = Loc.GetString("pubg-bp-reward-claim");
      ((Control) button1).MinWidth = 80f;
      ((Control) button1).MinHeight = 24f;
      Button button2 = button1;
      ((Control) button2).StyleClasses.Add("ButtonColorGreen");
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ClaimTaskXp(task.Id));
      ((Control) boxContainer6).AddChild((Control) button2);
    }
    else if (state.HasPremium && state.SkipsRemaining > 0)
    {
      Button button3 = new Button();
      button3.Text = Loc.GetString("pubg-bp-task-skip");
      ((Control) button3).MinWidth = 70f;
      ((Control) button3).MinHeight = 22f;
      Button button4 = button3;
      ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SkipTask(task.Id));
      ((Control) boxContainer6).AddChild((Control) button4);
    }
    ((Control) boxContainer2).AddChild((Control) boxContainer6);
    ((Control) compactTaskRow).AddChild((Control) boxContainer2);
    return (Control) compactTaskRow;
  }

  private Control CreateFooter(BattlePassStateMessage state)
  {
    BoxContainer footer = new BoxContainer();
    footer.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) footer).HorizontalExpand = true;
    ((Control) footer).Margin = new Thickness(10f, 5f, 10f, 10f);
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).MinHeight = 1f;
    ((Control) panelContainer).HorizontalExpand = true;
    panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = BattlePassTab.CardBorderColor
    };
    ((Control) footer).AddChild((Control) panelContainer);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(5f, 5f, 5f, 0.0f);
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    label1.Text = state.SeasonName;
    label1.FontColorOverride = new Color?(BattlePassTab.DisabledColor);
    ((Control) label1).HorizontalExpand = true;
    Label label2 = label1;
    ((Control) boxContainer2).AddChild((Control) label2);
    Label label3 = new Label();
    label3.Text = state.HasPremium ? "★ " + Loc.GetString("pubg-bp-premium") : Loc.GetString("pubg-bp-free");
    label3.FontColorOverride = new Color?(state.HasPremium ? BattlePassTab.GoldAccent : BattlePassTab.GreenSuccess);
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 3;
    Label label4 = label3;
    ((Control) boxContainer2).AddChild((Control) label4);
    ((Control) footer).AddChild((Control) boxContainer2);
    return (Control) footer;
  }

  private void ScrollToCurrentLevel(BattlePassStateMessage state)
  {
    if (this._rewardsScroll == null)
      return;
    this._rewardsScroll.HScroll = (float) Math.Max(0, (this._currentLevelIndex - 2) * 145);
  }

  private Control? CreateRewardIcon(BattlePassRewardInfo reward)
  {
    string str1;
    switch (reward.RewardType)
    {
      case "coins":
        str1 = "SpaceCash";
        break;
      case "scrap":
        str1 = "SheetSteel1";
        break;
      case "premiumCoins":
        str1 = "MaterialDiamond1";
        break;
      case "item":
        str1 = reward.RewardValue;
        break;
      case "emote":
        str1 = reward.RewardValue;
        break;
      default:
        str1 = (string) null;
        break;
    }
    string str2 = str1;
    if (str2 == null)
      return (Control) null;
    EntityPrototype entityPrototype;
    if (!this._prototypeManager.TryIndex<EntityPrototype>(str2, ref entityPrototype))
      return (Control) null;
    ActionComponent actionComponent;
    if (reward.RewardType == "emote" && entityPrototype.TryGetComponent<ActionComponent>("Action", ref actionComponent) && actionComponent.Icon != null)
    {
      Texture texture = this.SpriteSystem.Frame0(actionComponent.Icon);
      TextureRect rewardIcon = new TextureRect();
      rewardIcon.Texture = texture;
      ((Control) rewardIcon).SetWidth = 28f;
      ((Control) rewardIcon).SetHeight = 28f;
      rewardIcon.Stretch = (TextureRect.StretchMode) 7;
      return (Control) rewardIcon;
    }
    IRsiStateLike prototypeIcon = this.SpriteSystem.GetPrototypeIcon(entityPrototype);
    TextureRect rewardIcon1 = new TextureRect();
    rewardIcon1.Texture = ((IDirectionalTextureProvider) prototypeIcon).Default;
    ((Control) rewardIcon1).SetWidth = 28f;
    ((Control) rewardIcon1).SetHeight = 28f;
    rewardIcon1.Stretch = (TextureRect.StretchMode) 7;
    return (Control) rewardIcon1;
  }

  private string GetTaskDisplayText(BattlePassTaskInfo task)
  {
    return PubgBattlePassTaskFormatter.GetTaskDisplayText(task);
  }

  private string GetTimeRemaining()
  {
    TimeSpan timeSpan = this._tasksEndAt - DateTime.UtcNow;
    if (timeSpan.TotalSeconds <= 0.0)
      return "00:00:00";
    return $"{(int) timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
  }

  private void ClaimReward(string rewardId)
  {
    this._entityManager.System<BattlePassSystem>().ClaimReward(rewardId);
  }

  private void SkipTask(string taskId)
  {
    this._entityManager.System<BattlePassSystem>().SkipTask(taskId);
  }

  private void ClaimTaskXp(string taskId)
  {
    this._entityManager.System<BattlePassSystem>().ClaimTaskXp(taskId);
  }

  private void BuyPremium() => this._entityManager.System<BattlePassSystem>().BuyPremium();

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    ((Control) this).FrameUpdate(args);
    if (this._timerLabel == null)
      return;
    this._timerLabel.Text = this.GetTimeRemaining();
  }
}
