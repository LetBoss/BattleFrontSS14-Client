using System;
using System.Collections.Generic;
using System.Linq;
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

	private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null);

	private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null);

	private static readonly Color RedDanger = Color.FromHex((ReadOnlySpan<char>)"#FF4444", (Color?)null);

	private static readonly Color OrangeWarning = Color.FromHex((ReadOnlySpan<char>)"#FF9500", (Color?)null);

	private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>)"#0a0a0f", (Color?)null);

	private static readonly Color DisabledColor = Color.FromHex((ReadOnlySpan<char>)"#4a4a5a", (Color?)null);

	private static readonly Color EvenRowColor = Color.FromHex((ReadOnlySpan<char>)"#12121a", (Color?)null);

	private static readonly Color OddRowColor = Color.FromHex((ReadOnlySpan<char>)"#1a1a24", (Color?)null);

	private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>)"#2a2a3a", (Color?)null);

	private static readonly Color CardHoverColor = Color.FromHex((ReadOnlySpan<char>)"#252530", (Color?)null);

	private static readonly Color AccentGlow = Color.FromHex((ReadOnlySpan<char>)"#FFD700", (Color?)null);

	private static readonly Color PremiumGlow = Color.FromHex((ReadOnlySpan<char>)"#FF8C00", (Color?)null);

	private static readonly Color ProgressBg = Color.FromHex((ReadOnlySpan<char>)"#0d0d15", (Color?)null);

	private static readonly Color CompletedGlow = Color.FromHex((ReadOnlySpan<char>)"#00FFB3", (Color?)null);

	private SpriteSystem SpriteSystem => _spriteSystem ?? (_spriteSystem = _entityManager.System<SpriteSystem>());

	private static Color LerpColor(Color a, Color b, float t)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		t = Math.Clamp(t, 0f, 1f);
		return new Color(a.R + (b.R - a.R) * t, a.G + (b.G - a.G) * t, a.B + (b.B - a.B) * t, a.A + (b.A - a.A) * t);
	}

	public BattlePassTab()
	{
		IoCManager.InjectDependencies<BattlePassTab>(this);
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
	}

	public void UpdateState(BattlePassStateMessage state)
	{
		_cachedState = state;
		_tasksEndAt = state.TasksEndAt;
		_isLoading = false;
		BuildUI();
	}

	public void LoadSubcategory(BattlePassSubcategory subcategory)
	{
		((Control)this).RemoveAllChildren();
		if (_cachedState == null)
		{
			ShowLoading();
			RequestData();
		}
		else
		{
			BuildUI();
		}
	}

	private void RequestData()
	{
		if (!_isLoading)
		{
			_isLoading = true;
			_entityManager.System<BattlePassSystem>().RequestBattlePass();
		}
	}

	private void ShowLoading()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		PanelContainer val2 = new PanelContainer
		{
			MinWidth = 300f,
			MinHeight = 100f
		};
		((Control)val2).StyleClasses.Add("AngleRect");
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			Margin = new Thickness(20f)
		};
		Label val4 = new Label
		{
			Text = "◐",
			FontColorOverride = GoldAccent,
			HorizontalAlignment = (HAlignment)2
		};
		((Control)val4).SetOnlyStyleClass("LabelHeadingBigger");
		Label val5 = new Label
		{
			Text = Loc.GetString("pubg-bp-error-load-failed"),
			HorizontalAlignment = (HAlignment)2,
			FontColorOverride = Color.Gray,
			Margin = new Thickness(0f, 10f, 0f, 0f)
		};
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)(object)val5);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		((Control)this).AddChild((Control)(object)val);
	}

	private void BuildUI()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		((Control)this).RemoveAllChildren();
		BattlePassStateMessage cachedState = _cachedState;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val2 = CreateHeader(cachedState);
		((Control)val).AddChild((Control)(object)val2);
		Control val3 = CreateGoldDivider();
		((Control)val).AddChild(val3);
		Control val4 = CreateRewardsTimeline(cachedState);
		((Control)val).AddChild(val4);
		Control val5 = CreateTasksSection(cachedState);
		((Control)val).AddChild(val5);
		Control val6 = CreateFooter(cachedState);
		((Control)val).AddChild(val6);
		((Control)this).AddChild((Control)(object)val);
		ScrollToCurrentLevel(cachedState);
	}

	private Control CreateGoldDivider()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004d: Expected O, but got Unknown
		return (Control)new PanelContainer
		{
			MinHeight = 2f,
			HorizontalExpand = true,
			Margin = new Thickness(10f, 5f, 10f, 5f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = GoldAccent
			}
		};
	}

	private Control CreateDivider()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004d: Expected O, but got Unknown
		return (Control)new PanelContainer
		{
			MinHeight = 1f,
			HorizontalExpand = true,
			Margin = new Thickness(10f, 3f, 10f, 3f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = CardBorderColor
			}
		};
	}

	private BoxContainer CreateHeader(BattlePassStateMessage state)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_01d9: Expected O, but got Unknown
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Expected O, but got Unknown
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Expected O, but got Unknown
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Expected O, but got Unknown
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Expected O, but got Unknown
		//IL_03f5: Expected O, but got Unknown
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Expected O, but got Unknown
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Expected O, but got Unknown
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Expected O, but got Unknown
		//IL_04e8: Expected O, but got Unknown
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Expected O, but got Unknown
		//IL_0589: Expected O, but got Unknown
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Expected O, but got Unknown
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			Margin = new Thickness(10f, 10f, 10f, 0f)
		};
		PanelContainer val2 = new PanelContainer
		{
			HorizontalExpand = true,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = DarkPanel,
				BorderColor = GoldAccent,
				BorderThickness = new Thickness(0f, 0f, 0f, 2f)
			}
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			Margin = new Thickness(15f, 12f, 15f, 12f)
		};
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		Label val6 = new Label();
		val6.Text = Loc.GetString("pubg-bp-season", new(string, object)[1] { ("name", state.SeasonName) });
		val6.FontColorOverride = Color.White;
		Label val7 = val6;
		((Control)val7).SetOnlyStyleClass("LabelHeadingBigger");
		((Control)val5).AddChild((Control)(object)val7);
		if (state.HasPremium)
		{
			PanelContainer val8 = new PanelContainer
			{
				Margin = new Thickness(15f, 0f, 0f, 0f),
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1a1208", (Color?)null),
					BorderColor = AccentGlow,
					BorderThickness = new Thickness(2f),
					ContentMarginLeftOverride = 10f,
					ContentMarginRightOverride = 10f,
					ContentMarginTopOverride = 4f,
					ContentMarginBottomOverride = 4f
				}
			};
			BoxContainer val9 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			Label val10 = new Label
			{
				Text = "★ ",
				FontColorOverride = AccentGlow
			};
			((Control)val10).SetOnlyStyleClass("LabelHeading");
			((Control)val9).AddChild((Control)(object)val10);
			Label val11 = new Label
			{
				Text = Loc.GetString("pubg-bp-premium"),
				FontColorOverride = GoldAccent
			};
			((Control)val11).SetOnlyStyleClass("LabelHeading");
			((Control)val9).AddChild((Control)(object)val11);
			((Control)val8).AddChild((Control)(object)val9);
			((Control)val5).AddChild((Control)(object)val8);
		}
		((Control)val4).AddChild((Control)(object)val5);
		((Control)val3).AddChild((Control)(object)val4);
		Control val12 = new Control
		{
			HorizontalExpand = true
		};
		((Control)val3).AddChild(val12);
		int num = 0;
		int num2 = 0;
		foreach (BattlePassLevelInfo item in state.Levels.OrderBy((BattlePassLevelInfo l) => l.Level))
		{
			if (item.Level <= state.CurrentLevel)
			{
				num += item.XpRequired;
			}
			if (item.Level <= state.CurrentLevel + 1)
			{
				num2 += item.XpRequired;
			}
		}
		int num3 = state.CurrentXp - num;
		int num4 = num2 - num;
		if (num4 <= 0)
		{
			num4 = state.Levels.LastOrDefault()?.XpRequired ?? 100;
		}
		BoxContainer val13 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			VerticalAlignment = (VAlignment)2
		};
		PanelContainer val14 = new PanelContainer
		{
			Margin = new Thickness(0f, 0f, 15f, 0f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = GoldAccent,
				BorderColor = AccentGlow,
				BorderThickness = new Thickness(2f),
				ContentMarginLeftOverride = 14f,
				ContentMarginRightOverride = 14f,
				ContentMarginTopOverride = 8f,
				ContentMarginBottomOverride = 8f
			}
		};
		val6 = new Label();
		val6.Text = Loc.GetString("pubg-bp-level", new(string, object)[1] { ("level", state.CurrentLevel) });
		val6.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#0a0a0f", (Color?)null);
		Label val15 = val6;
		((Control)val15).SetOnlyStyleClass("LabelHeadingBigger");
		((Control)val14).AddChild((Control)(object)val15);
		((Control)val13).AddChild((Control)(object)val14);
		BoxContainer val16 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			MinWidth = 220f
		};
		PanelContainer val17 = new PanelContainer
		{
			MinHeight = 24f,
			HorizontalExpand = true,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = ProgressBg,
				BorderColor = CardBorderColor,
				BorderThickness = new Thickness(1f, 1f, 1f, 1f)
			}
		};
		int num5 = Math.Max(0, Math.Min(num3, num4));
		float num6 = ((num4 > 0) ? ((float)num5 / (float)num4) : 0f);
		Color backgroundColor = ((num6 >= 0.9f) ? CompletedGlow : ((num6 >= 0.6f) ? LerpColor(OrangeWarning, GreenSuccess, (num6 - 0.6f) / 0.3f) : OrangeWarning));
		PanelContainer val18 = new PanelContainer
		{
			HorizontalAlignment = (HAlignment)1,
			MinHeight = 22f,
			MinWidth = (int)(218f * num6),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = backgroundColor
			}
		};
		val6 = new Label();
		val6.Text = Loc.GetString("pubg-bp-xp", new(string, object)[2]
		{
			("current", Math.Max(0, num3)),
			("required", num4)
		});
		val6.FontColorOverride = Color.White;
		((Control)val6).HorizontalAlignment = (HAlignment)2;
		((Control)val6).VerticalAlignment = (VAlignment)2;
		((Control)val6).Margin = new Thickness(0f, 2f, 0f, 0f);
		Label val19 = val6;
		((Control)val17).AddChild((Control)(object)val18);
		((Control)val17).AddChild((Control)(object)val19);
		((Control)val16).AddChild((Control)(object)val17);
		((Control)val13).AddChild((Control)(object)val16);
		((Control)val3).AddChild((Control)(object)val13);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}

	private Control CreateRewardsTimeline(BattlePassStateMessage state)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			Margin = new Thickness(10f, 10f, 10f, 5f),
			MinHeight = 290f
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			MinWidth = 85f,
			Margin = new Thickness(0f, 0f, 10f, 0f)
		};
		Control val3 = CreateRowLabel(Loc.GetString("pubg-bp-free"), GreenSuccess, 90);
		((Control)val2).AddChild(val3);
		Control val4 = new Control
		{
			MinHeight = 55f
		};
		((Control)val2).AddChild(val4);
		Control val5 = CreateRowLabel("★ " + Loc.GetString("pubg-bp-premium"), GoldAccent, 90);
		((Control)val2).AddChild(val5);
		((Control)val).AddChild((Control)(object)val2);
		PanelContainer val6 = new PanelContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = ProgressBg,
				BorderColor = CardBorderColor,
				BorderThickness = new Thickness(2f)
			}
		};
		_rewardsScroll = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			HScrollEnabled = true,
			VScrollEnabled = false,
			Margin = new Thickness(5f)
		};
		BoxContainer val7 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Margin = new Thickness(10f, 5f, 20f, 5f)
		};
		_currentLevelIndex = 0;
		for (int i = 0; i < state.Levels.Count; i++)
		{
			BattlePassLevelInfo battlePassLevelInfo = state.Levels[i];
			if (battlePassLevelInfo.Level == state.CurrentLevel)
			{
				_currentLevelIndex = i;
			}
			Control val8 = CreateLevelNode(battlePassLevelInfo, state, i < state.Levels.Count - 1);
			((Control)val7).AddChild(val8);
		}
		((Control)_rewardsScroll).AddChild((Control)(object)val7);
		((Control)val6).AddChild((Control)(object)_rewardsScroll);
		((Control)val).AddChild((Control)(object)val6);
		return (Control)(object)val;
	}

	private Control CreateRowLabel(string text, Color color, int height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			MinHeight = height,
			VerticalAlignment = (VAlignment)2,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#15151a", (Color?)null),
				BorderColor = color,
				BorderThickness = new Thickness(0f, 0f, 2f, 0f),
				ContentMarginRightOverride = 8f
			}
		};
		Label val2 = new Label
		{
			Text = text,
			FontColorOverride = color,
			VerticalAlignment = (VAlignment)2,
			HorizontalAlignment = (HAlignment)3,
			Margin = new Thickness(5f)
		};
		((Control)val).AddChild((Control)(object)val2);
		return (Control)val;
	}

	private Control CreateLevelNode(BattlePassLevelInfo level, BattlePassStateMessage state, bool hasNext)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Expected O, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Expected O, but got Unknown
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Expected O, but got Unknown
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Expected O, but got Unknown
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Expected O, but got Unknown
		//IL_046f: Expected O, but got Unknown
		bool flag = state.CurrentLevel >= level.Level;
		bool flag2 = state.CurrentLevel == level.Level;
		List<BattlePassRewardInfo> list = level.Rewards.Where((BattlePassRewardInfo r) => !r.IsPremium).ToList();
		List<BattlePassRewardInfo> list2 = level.Rewards.Where((BattlePassRewardInfo r) => r.IsPremium).ToList();
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			MinWidth = 100 + (hasNext ? 45 : 0)
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			MinWidth = 100f,
			HorizontalAlignment = (HAlignment)2
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)2,
			MinHeight = 90f
		};
		if (list.Count > 0)
		{
			foreach (BattlePassRewardInfo item in list)
			{
				Control val4 = CreateTimelineRewardCard(item, level, state);
				((Control)val3).AddChild(val4);
			}
		}
		else
		{
			Control val5 = CreateEmptyRewardSlot();
			((Control)val3).AddChild(val5);
		}
		((Control)val2).AddChild((Control)(object)val3);
		BoxContainer val6 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 5f, 0f, 5f)
		};
		PanelContainer val7 = new PanelContainer
		{
			MinWidth = 36f,
			MinHeight = 36f
		};
		Color val8;
		Color backgroundColor;
		string text;
		if (flag2)
		{
			val8 = AccentGlow;
			backgroundColor = GoldAccent;
			text = "▶";
		}
		else if (flag)
		{
			val8 = CompletedGlow;
			backgroundColor = Color.FromHex((ReadOnlySpan<char>)"#14141f", (Color?)null);
			text = "✓";
		}
		else
		{
			val8 = DisabledColor;
			backgroundColor = Color.FromHex((ReadOnlySpan<char>)"#0a0a12", (Color?)null);
			text = "○";
		}
		val7.PanelOverride = (StyleBox)new StyleBoxFlat
		{
			BackgroundColor = backgroundColor,
			BorderColor = val8,
			BorderThickness = new Thickness((float)(flag2 ? 3 : 2))
		};
		Label val9 = new Label
		{
			Text = text,
			FontColorOverride = (flag2 ? Color.Black : val8),
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		((Control)val9).SetOnlyStyleClass(flag2 ? "LabelHeading" : "LabelSmall");
		((Control)val7).AddChild((Control)(object)val9);
		((Control)val6).AddChild((Control)(object)val7);
		((Control)val2).AddChild((Control)(object)val6);
		BoxContainer val10 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)2
		};
		Label val11 = new Label
		{
			Text = level.Level.ToString(),
			FontColorOverride = (flag2 ? GoldAccent : (flag ? GreenSuccess : DisabledColor)),
			HorizontalAlignment = (HAlignment)2
		};
		((Control)val11).SetOnlyStyleClass("LabelHeading");
		((Control)val10).AddChild((Control)(object)val11);
		((Control)val2).AddChild((Control)(object)val10);
		BoxContainer val12 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)2,
			MinHeight = 90f,
			Margin = new Thickness(0f, 10f, 0f, 0f)
		};
		if (list2.Count > 0)
		{
			foreach (BattlePassRewardInfo item2 in list2)
			{
				Control val13 = CreateTimelineRewardCard(item2, level, state);
				((Control)val12).AddChild(val13);
			}
		}
		else
		{
			Control val14 = CreateEmptyRewardSlot();
			((Control)val12).AddChild(val14);
		}
		((Control)val2).AddChild((Control)(object)val12);
		((Control)val).AddChild((Control)(object)val2);
		if (hasNext)
		{
			Color backgroundColor2 = (flag ? GreenSuccess : Color.FromHex((ReadOnlySpan<char>)"#1a1a24", (Color?)null));
			Color borderColor = (flag ? CompletedGlow : Color.FromHex((ReadOnlySpan<char>)"#2a2a3a", (Color?)null));
			PanelContainer val15 = new PanelContainer
			{
				MinWidth = 45f,
				MinHeight = 4f,
				VerticalAlignment = (VAlignment)2,
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = backgroundColor2,
					BorderColor = borderColor,
					BorderThickness = new Thickness(0f, 1f, 0f, 1f)
				}
			};
			((Control)val).AddChild((Control)(object)val15);
		}
		return (Control)(object)val;
	}

	private Control CreateEmptyRewardSlot()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			MinWidth = 55f,
			MinHeight = 70f,
			Margin = new Thickness(2f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#18181c", (Color?)null),
				BorderColor = Color.FromHex((ReadOnlySpan<char>)"#2a2a2a", (Color?)null),
				BorderThickness = new Thickness(1f, 0f, 1f, 0f)
			}
		};
		Label val2 = new Label
		{
			Text = "—",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#3a3a3a", (Color?)null),
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		((Control)val).AddChild((Control)(object)val2);
		return (Control)val;
	}

	private Control CreateTimelineRewardCard(BattlePassRewardInfo reward, BattlePassLevelInfo level, BattlePassStateMessage state)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_018b: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Expected O, but got Unknown
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected O, but got Unknown
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Expected O, but got Unknown
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Expected O, but got Unknown
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Expected O, but got Unknown
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Expected O, but got Unknown
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Expected O, but got Unknown
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Expected O, but got Unknown
		bool flag = state.CurrentLevel >= level.Level;
		bool flag2 = state.ClaimedRewardIds.Contains(reward.Id);
		bool flag3 = !reward.IsPremium || state.HasPremium;
		Color borderColor;
		Color backgroundColor;
		if (flag2)
		{
			borderColor = Color.FromHex((ReadOnlySpan<char>)"#1a2a2a", (Color?)null);
			backgroundColor = Color.FromHex((ReadOnlySpan<char>)"#0f0f18", (Color?)null);
		}
		else if (flag && flag3)
		{
			borderColor = (reward.IsPremium ? GoldAccent : GreenSuccess);
			backgroundColor = Color.FromHex((ReadOnlySpan<char>)"#14141f", (Color?)null);
		}
		else
		{
			borderColor = CardBorderColor;
			backgroundColor = Color.FromHex((ReadOnlySpan<char>)"#0a0a12", (Color?)null);
		}
		PubgAnimatedRewardCard pubgAnimatedRewardCard = new PubgAnimatedRewardCard();
		pubgAnimatedRewardCard.CanClaim = flag;
		pubgAnimatedRewardCard.IsClaimed = flag2;
		pubgAnimatedRewardCard.IsPremium = reward.IsPremium;
		pubgAnimatedRewardCard.CanClaimPremium = flag3;
		pubgAnimatedRewardCard.OnClaimPressed += delegate
		{
			ClaimReward(reward.Id);
		};
		PanelContainer val = new PanelContainer
		{
			MinWidth = 60f,
			MinHeight = 80f,
			Margin = new Thickness(2f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = backgroundColor,
				BorderColor = borderColor,
				BorderThickness = new Thickness((float)((!(flag && flag3) || flag2) ? 1 : 2))
			}
		};
		if (flag2)
		{
			((Control)val).Modulate = new Color(0.6f, 0.6f, 0.6f, 1f);
		}
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			Margin = new Thickness(4f)
		};
		if (reward.IsPremium)
		{
			Label val3 = new Label
			{
				Text = "★",
				FontColorOverride = GoldAccent,
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val2).AddChild((Control)(object)val3);
		}
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			MinHeight = 32f
		};
		Control val5 = CreateRewardIcon(reward);
		if (val5 != null)
		{
			((Control)val4).AddChild(val5);
		}
		else
		{
			Label val6 = new Label
			{
				Text = GetRewardEmoji(reward.RewardType),
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val4).AddChild((Control)(object)val6);
		}
		((Control)val2).AddChild((Control)(object)val4);
		string shortRewardText = GetShortRewardText(reward);
		if (!string.IsNullOrEmpty(shortRewardText))
		{
			Label val7 = new Label
			{
				Text = shortRewardText,
				FontColorOverride = Color.White,
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val2).AddChild((Control)(object)val7);
		}
		if (reward.Duration.HasValue)
		{
			Label val8 = new Label
			{
				Text = $"{reward.Duration}d",
				FontColorOverride = OrangeWarning,
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val2).AddChild((Control)(object)val8);
		}
		if (flag2)
		{
			Label val9 = new Label
			{
				Text = "✓",
				FontColorOverride = GreenSuccess,
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val2).AddChild((Control)(object)val9);
		}
		else if (flag && flag3)
		{
			Button val10 = new Button
			{
				Text = "▼",
				MinWidth = 35f,
				MinHeight = 20f,
				Modulate = (reward.IsPremium ? GoldAccent : GreenSuccess)
			};
			((Control)val10).StyleClasses.Add("ButtonColorGreen");
			((BaseButton)val10).OnPressed += delegate
			{
				ClaimReward(reward.Id);
			};
			((Control)val2).AddChild((Control)(object)val10);
		}
		else if (!flag3)
		{
			Label val11 = new Label
			{
				Text = "\ud83d\udd12",
				FontColorOverride = GoldAccent,
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val2).AddChild((Control)(object)val11);
		}
		((Control)val).AddChild((Control)(object)val2);
		pubgAnimatedRewardCard.SetContent((Control)(object)val);
		return (Control)(object)pubgAnimatedRewardCard;
	}

	private string GetRewardEmoji(string rewardType)
	{
		return rewardType switch
		{
			"coins" => "\ud83d\udcb0", 
			"scrap" => "⚙", 
			"premiumCoins" => "\ud83d\udc8e", 
			"item" => "\ud83d\udce6", 
			"emote" => "\ud83d\ude00", 
			"permission" => "\ud83d\udd11", 
			"tier" => "\ud83c\udfc6", 
			_ => "\ud83c\udf81", 
		};
	}

	private string GetShortRewardText(BattlePassRewardInfo reward)
	{
		switch (reward.RewardType)
		{
		case "coins":
		case "scrap":
		case "premiumCoins":
			return reward.RewardValue;
		default:
			return "";
		}
	}

	private Control CreateTasksSection(BattlePassStateMessage state)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_007f: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0104: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Expected O, but got Unknown
		//IL_01e0: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Expected O, but got Unknown
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected O, but got Unknown
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Expected O, but got Unknown
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Expected O, but got Unknown
		//IL_0427: Expected O, but got Unknown
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Expected O, but got Unknown
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Expected O, but got Unknown
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Expected O, but got Unknown
		//IL_0312: Expected O, but got Unknown
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Expected O, but got Unknown
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true,
			Margin = new Thickness(10f, 5f, 10f, 0f)
		};
		PanelContainer val2 = new PanelContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = DarkPanel,
				BorderColor = CardBorderColor,
				BorderThickness = new Thickness(1f)
			}
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			Margin = new Thickness(0f)
		};
		PanelContainer val4 = new PanelContainer
		{
			HorizontalExpand = true,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#15151a", (Color?)null),
				BorderColor = GoldAccent,
				BorderThickness = new Thickness(0f, 0f, 0f, 1f)
			}
		};
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			Margin = new Thickness(12f, 8f, 12f, 8f)
		};
		Label val6 = new Label
		{
			Text = Loc.GetString("pubg-bp-tasks-title"),
			HorizontalExpand = true,
			FontColorOverride = Color.White
		};
		((Control)val6).SetOnlyStyleClass("LabelHeading");
		((Control)val5).AddChild((Control)(object)val6);
		PanelContainer val7 = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1a1a20", (Color?)null),
				ContentMarginLeftOverride = 8f,
				ContentMarginRightOverride = 8f,
				ContentMarginTopOverride = 2f,
				ContentMarginBottomOverride = 2f
			}
		};
		BoxContainer val8 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		Label val9 = new Label
		{
			Text = "⏱ ",
			FontColorOverride = GoldAccent
		};
		((Control)val8).AddChild((Control)(object)val9);
		_timerLabel = new Label
		{
			Text = GetTimeRemaining(),
			FontColorOverride = GoldAccent
		};
		((Control)val8).AddChild((Control)(object)_timerLabel);
		((Control)val7).AddChild((Control)(object)val8);
		((Control)val5).AddChild((Control)(object)val7);
		if (state.HasPremium)
		{
			PanelContainer val10 = new PanelContainer
			{
				Margin = new Thickness(10f, 0f, 0f, 0f),
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = ((state.SkipsRemaining > 0) ? Color.FromHex((ReadOnlySpan<char>)"#1a2a1a", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#1a1a1a", (Color?)null)),
					ContentMarginLeftOverride = 8f,
					ContentMarginRightOverride = 8f,
					ContentMarginTopOverride = 2f,
					ContentMarginBottomOverride = 2f
				}
			};
			Label val11 = new Label();
			val11.Text = Loc.GetString("pubg-bp-tasks-skips", new(string, object)[1] { ("remaining", state.SkipsRemaining) });
			val11.FontColorOverride = ((state.SkipsRemaining > 0) ? GreenSuccess : DisabledColor);
			Label val12 = val11;
			((Control)val10).AddChild((Control)(object)val12);
			((Control)val5).AddChild((Control)(object)val10);
		}
		else
		{
			PanelContainer val13 = new PanelContainer
			{
				Margin = new Thickness(10f, 0f, 0f, 0f),
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#2a2520", (Color?)null),
					BorderColor = GoldAccent,
					BorderThickness = new Thickness(1f),
					ContentMarginLeftOverride = 8f,
					ContentMarginRightOverride = 8f,
					ContentMarginTopOverride = 2f,
					ContentMarginBottomOverride = 2f
				}
			};
			Label val14 = new Label
			{
				Text = "★ " + Loc.GetString("pubg-bp-tasks-skips-locked"),
				FontColorOverride = GoldAccent
			};
			((Control)val13).AddChild((Control)(object)val14);
			((Control)val5).AddChild((Control)(object)val13);
		}
		((Control)val4).AddChild((Control)(object)val5);
		((Control)val3).AddChild((Control)(object)val4);
		ScrollContainer val15 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			VScrollEnabled = true,
			HScrollEnabled = false
		};
		BoxContainer val16 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true
		};
		List<BattlePassTaskInfo> list = state.Tasks.Where((BattlePassTaskInfo t) => !t.IsSkipped).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			BattlePassTaskInfo task = list[num];
			Control val17 = CreateCompactTaskRow(task, state, num % 2 == 0);
			((Control)val16).AddChild(val17);
		}
		((Control)val15).AddChild((Control)(object)val16);
		((Control)val3).AddChild((Control)(object)val15);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private Control CreateCompactTaskRow(BattlePassTaskInfo task, BattlePassStateMessage state, bool isEven)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		//IL_01cb: Expected O, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Expected O, but got Unknown
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Expected O, but got Unknown
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Expected O, but got Unknown
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Expected O, but got Unknown
		//IL_033c: Expected O, but got Unknown
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Expected O, but got Unknown
		//IL_03eb: Expected O, but got Unknown
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Expected O, but got Unknown
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Expected O, but got Unknown
		//IL_053b: Expected O, but got Unknown
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Expected O, but got Unknown
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Expected O, but got Unknown
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Expected O, but got Unknown
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Expected O, but got Unknown
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Expected O, but got Unknown
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Expected O, but got Unknown
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Expected O, but got Unknown
		bool flag = task.RequiredPerm != null;
		bool flag2 = !flag || state.HasPremium;
		bool flag3 = task.Progress >= task.TargetValue;
		bool xpClaimed = task.XpClaimed;
		PubgTaskRowPanel pubgTaskRowPanel = new PubgTaskRowPanel();
		((Control)pubgTaskRowPanel).HorizontalExpand = true;
		pubgTaskRowPanel.IsHoverable = !xpClaimed && flag2;
		pubgTaskRowPanel.IsCompleted = xpClaimed;
		((PanelContainer)pubgTaskRowPanel).PanelOverride = (StyleBox)new StyleBoxFlat
		{
			BackgroundColor = (isEven ? EvenRowColor : OddRowColor),
			BorderColor = (xpClaimed ? CompletedGlow : ((flag3 && flag2) ? GoldAccent : Color.Transparent)),
			BorderThickness = new Thickness((float)((xpClaimed || (flag3 && flag2)) ? 2 : 0), 0f, 0f, 0f)
		};
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			Margin = new Thickness(12f, 8f, 12f, 8f)
		};
		PanelContainer val2 = new PanelContainer
		{
			MinWidth = 45f,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = (flag ? Color.FromHex((ReadOnlySpan<char>)"#2a2520", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#1a1a22", (Color?)null)),
				BorderColor = (flag ? GoldAccent : CardBorderColor),
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 6f,
				ContentMarginRightOverride = 6f,
				ContentMarginTopOverride = 2f,
				ContentMarginBottomOverride = 2f
			}
		};
		Label val3 = new Label
		{
			Text = (flag ? $"★{task.Slot}" : $"#{task.Slot}"),
			FontColorOverride = (flag ? GoldAccent : Color.White),
			HorizontalAlignment = (HAlignment)2
		};
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		Label val4 = new Label
		{
			Text = GetTaskDisplayText(task),
			HorizontalExpand = true,
			Margin = new Thickness(12f, 0f, 12f, 0f),
			FontColorOverride = (flag2 ? Color.White : DisabledColor)
		};
		((Control)val).AddChild((Control)(object)val4);
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			MinWidth = 170f
		};
		PanelContainer val6 = new PanelContainer
		{
			MinWidth = 100f,
			MinHeight = 18f,
			VerticalAlignment = (VAlignment)2,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = ProgressBg,
				BorderColor = CardBorderColor,
				BorderThickness = new Thickness(1f)
			}
		};
		float num = ((task.TargetValue > 0) ? Math.Min((float)task.Progress / (float)task.TargetValue, 1f) : 0f);
		Color backgroundColor = (flag3 ? CompletedGlow : ((num >= 0.75f) ? LerpColor(OrangeWarning, GreenSuccess, (num - 0.75f) / 0.25f) : OrangeWarning));
		PanelContainer val7 = new PanelContainer
		{
			HorizontalAlignment = (HAlignment)1,
			MinHeight = 16f,
			MinWidth = (int)(98f * num),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = backgroundColor
			}
		};
		((Control)val6).AddChild((Control)(object)val7);
		((Control)val5).AddChild((Control)(object)val6);
		Label val8 = new Label
		{
			Text = $" {task.Progress}/{task.TargetValue}",
			FontColorOverride = (flag3 ? GreenSuccess : Color.LightGray),
			MinWidth = 60f,
			Margin = new Thickness(5f, 0f, 0f, 0f)
		};
		((Control)val5).AddChild((Control)(object)val8);
		((Control)val).AddChild((Control)(object)val5);
		PanelContainer val9 = new PanelContainer
		{
			MinWidth = 60f,
			Margin = new Thickness(10f, 0f, 10f, 0f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1a2a1a", (Color?)null),
				ContentMarginLeftOverride = 6f,
				ContentMarginRightOverride = 6f,
				ContentMarginTopOverride = 2f,
				ContentMarginBottomOverride = 2f
			}
		};
		Label val10 = new Label
		{
			Text = $"+{task.XpReward}",
			FontColorOverride = GoldAccent,
			HorizontalAlignment = (HAlignment)2
		};
		((Control)val9).AddChild((Control)(object)val10);
		((Control)val).AddChild((Control)(object)val9);
		BoxContainer val11 = new BoxContainer
		{
			MinWidth = 85f,
			HorizontalAlignment = (HAlignment)3
		};
		if (!flag2)
		{
			Label val12 = new Label
			{
				Text = "\ud83d\udd12",
				FontColorOverride = GoldAccent,
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val11).AddChild((Control)(object)val12);
		}
		else if (xpClaimed)
		{
			Label val13 = new Label
			{
				Text = "✓ " + Loc.GetString("pubg-bp-task-completed"),
				FontColorOverride = GreenSuccess,
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val11).AddChild((Control)(object)val13);
		}
		else if (task.IsSkipped)
		{
			Label val14 = new Label
			{
				Text = Loc.GetString("pubg-bp-task-skipped"),
				FontColorOverride = DisabledColor,
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val11).AddChild((Control)(object)val14);
		}
		else if (flag3)
		{
			Button val15 = new Button
			{
				Text = Loc.GetString("pubg-bp-reward-claim"),
				MinWidth = 80f,
				MinHeight = 24f
			};
			((Control)val15).StyleClasses.Add("ButtonColorGreen");
			((BaseButton)val15).OnPressed += delegate
			{
				ClaimTaskXp(task.Id);
			};
			((Control)val11).AddChild((Control)(object)val15);
		}
		else if (state.HasPremium && state.SkipsRemaining > 0)
		{
			Button val16 = new Button
			{
				Text = Loc.GetString("pubg-bp-task-skip"),
				MinWidth = 70f,
				MinHeight = 22f
			};
			((BaseButton)val16).OnPressed += delegate
			{
				SkipTask(task.Id);
			};
			((Control)val11).AddChild((Control)(object)val16);
		}
		((Control)val).AddChild((Control)(object)val11);
		((Control)pubgTaskRowPanel).AddChild((Control)(object)val);
		return (Control)(object)pubgTaskRowPanel;
	}

	private Control CreateFooter(BattlePassStateMessage state)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0060: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			Margin = new Thickness(10f, 5f, 10f, 10f)
		};
		PanelContainer val2 = new PanelContainer
		{
			MinHeight = 1f,
			HorizontalExpand = true,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = CardBorderColor
			}
		};
		((Control)val).AddChild((Control)(object)val2);
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			Margin = new Thickness(5f, 5f, 5f, 0f)
		};
		Label val4 = new Label
		{
			Text = state.SeasonName,
			FontColorOverride = DisabledColor,
			HorizontalExpand = true
		};
		((Control)val3).AddChild((Control)(object)val4);
		Label val5 = new Label
		{
			Text = (state.HasPremium ? ("★ " + Loc.GetString("pubg-bp-premium")) : Loc.GetString("pubg-bp-free")),
			FontColorOverride = (state.HasPremium ? GoldAccent : GreenSuccess),
			HorizontalAlignment = (HAlignment)3
		};
		((Control)val3).AddChild((Control)(object)val5);
		((Control)val).AddChild((Control)(object)val3);
		return (Control)val;
	}

	private void ScrollToCurrentLevel(BattlePassStateMessage state)
	{
		if (_rewardsScroll != null)
		{
			int num = Math.Max(0, (_currentLevelIndex - 2) * 145);
			_rewardsScroll.HScroll = num;
		}
	}

	private Control? CreateRewardIcon(BattlePassRewardInfo reward)
	{
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		string text = reward.RewardType switch
		{
			"coins" => "SpaceCash", 
			"scrap" => "SheetSteel1", 
			"premiumCoins" => "MaterialDiamond1", 
			"item" => reward.RewardValue, 
			"emote" => reward.RewardValue, 
			_ => null, 
		};
		if (text == null)
		{
			return null;
		}
		EntityPrototype val = default(EntityPrototype);
		if (!_prototypeManager.TryIndex<EntityPrototype>(text, ref val))
		{
			return null;
		}
		ActionComponent actionComponent = default(ActionComponent);
		if (reward.RewardType == "emote" && val.TryGetComponent<ActionComponent>("Action", ref actionComponent) && actionComponent.Icon != null)
		{
			Texture texture = SpriteSystem.Frame0(actionComponent.Icon);
			return (Control?)new TextureRect
			{
				Texture = texture,
				SetWidth = 28f,
				SetHeight = 28f,
				Stretch = (StretchMode)7
			};
		}
		IRsiStateLike prototypeIcon = SpriteSystem.GetPrototypeIcon(val);
		return (Control?)new TextureRect
		{
			Texture = ((IDirectionalTextureProvider)prototypeIcon).Default,
			SetWidth = 28f,
			SetHeight = 28f,
			Stretch = (StretchMode)7
		};
	}

	private string GetTaskDisplayText(BattlePassTaskInfo task)
	{
		return PubgBattlePassTaskFormatter.GetTaskDisplayText(task);
	}

	private string GetTimeRemaining()
	{
		TimeSpan timeSpan = _tasksEndAt - DateTime.UtcNow;
		if (timeSpan.TotalSeconds <= 0.0)
		{
			return "00:00:00";
		}
		return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
	}

	private void ClaimReward(string rewardId)
	{
		_entityManager.System<BattlePassSystem>().ClaimReward(rewardId);
	}

	private void SkipTask(string taskId)
	{
		_entityManager.System<BattlePassSystem>().SkipTask(taskId);
	}

	private void ClaimTaskXp(string taskId)
	{
		_entityManager.System<BattlePassSystem>().ClaimTaskXp(taskId);
	}

	private void BuyPremium()
	{
		_entityManager.System<BattlePassSystem>().BuyPremium();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_timerLabel != null)
		{
			_timerLabel.Text = GetTimeRemaining();
		}
	}
}
