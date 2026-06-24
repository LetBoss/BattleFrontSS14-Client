using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.UserInterface.Controls;
using Content.Shared._PUBG.Skin;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

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
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
	}

	public void LoadSubcategory(ProfileSubcategory subcategory)
	{
		_currentSubcategory = subcategory;
		((Control)this).RemoveAllChildren();
		switch (subcategory)
		{
		case ProfileSubcategory.Stats:
			LoadStatsView();
			break;
		case ProfileSubcategory.Sponsors:
			LoadSponsorsView();
			break;
		}
	}

	private void LoadStatsView()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Expected O, but got Unknown
		ScrollContainer val = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			Margin = new Thickness(20f)
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true
		};
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			Margin = new Thickness(0f, 0f, 10f, 0f)
		};
		Label val5 = new Label
		{
			Text = "СТАТИСТИКА ИГРОКА",
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 15f)
		};
		((Control)val5).SetOnlyStyleClass("LabelHeading");
		BoxContainer val6 = CreateVisualStatsSection();
		((Control)val4).AddChild((Control)(object)val5);
		((Control)val4).AddChild((Control)(object)val6);
		AlternatingBGContainer alternatingBGContainer = new AlternatingBGContainer();
		((BoxContainer)alternatingBGContainer).Orientation = (LayoutOrientation)1;
		alternatingBGContainer.HorizontalExpand = true;
		((Control)alternatingBGContainer).Margin = new Thickness(0f, 15f, 0f, 0f);
		AlternatingBGContainer alternatingBGContainer2 = alternatingBGContainer;
		int value = _avgSurvivalTime / 3600;
		int value2 = _avgSurvivalTime % 3600 / 60;
		string value3 = ((_playerRank > 0) ? $"#{_playerRank}" : "N/A");
		alternatingBGContainer2.AddControl((Control)(object)CreateStatRow("Ваш рейтинг:", $"{_playerRating} ({value3})"));
		alternatingBGContainer2.AddControl((Control)(object)CreateStatRow(Loc.GetString("pubg-reputation-mainmenu-label"), $"{_reputation}/100"));
		alternatingBGContainer2.AddControl((Control)(object)CreateStatRow("Всего игр:", _totalGames.ToString()));
		alternatingBGContainer2.AddControl((Control)(object)CreateStatRow("Убийств:", _totalKills.ToString()));
		alternatingBGContainer2.AddControl((Control)(object)CreateStatRow("Урона нанесено:", _totalDamage.ToString()));
		alternatingBGContainer2.AddControl((Control)(object)CreateStatRow("Время выживания:", $"{value}ч {value2}м"));
		alternatingBGContainer2.AddControl((Control)(object)CreateStatRow("Скинов:", $"{_unlockedCaseDropSkins}/{_totalCaseDropSkins}"));
		alternatingBGContainer2.AddControl((Control)(object)CreateStatRow("Эмоций:", $"{_availableEmotes}/{_totalEmotes}"));
		((Control)val4).AddChild((Control)(object)alternatingBGContainer2);
		BoxContainer val7 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			MinWidth = 320f,
			Margin = new Thickness(10f, 0f, 0f, 0f)
		};
		BoxContainer val8 = CreateLeaderboardSection();
		((Control)val7).AddChild((Control)(object)val8);
		BoxContainer val9 = CreateMatchHistorySection();
		((Control)val7).AddChild((Control)(object)val9);
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)(object)val7);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		((Control)this).AddChild((Control)(object)val);
	}

	private BoxContainer CreateVisualStatsSection()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Expected O, but got Unknown
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true
		};
		float num = ((_totalGames > 0) ? ((float)_wins / (float)_totalGames * 100f) : 0f);
		PubgStatCard obj = new PubgStatCard
		{
			Title = "WINRATE",
			Value = $"{num:F1}% ({_wins}/{_totalGames})",
			Progress = num / 100f,
			ShowProgress = true,
			ValueColor = Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null)
		};
		((Control)obj).HorizontalExpand = true;
		((Control)obj).Margin = new Thickness(0f, 0f, 0f, 10f);
		PubgStatCard pubgStatCard = obj;
		((Control)val).AddChild((Control)(object)pubgStatCard);
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			Margin = new Thickness(0f, 5f, 0f, 0f)
		};
		float num2 = ((_totalDeaths > 0) ? ((float)_totalKills / (float)_totalDeaths) : ((float)_totalKills));
		Color valueColor = ((num2 >= 2f) ? Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null) : ((num2 >= 1f) ? Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null) : ((num2 >= 0.5f) ? Color.FromHex((ReadOnlySpan<char>)"#FF9500", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#FF4444", (Color?)null))));
		PubgStatCard obj2 = new PubgStatCard
		{
			Title = "K/D RATIO",
			Value = $"{num2:F2}",
			ValueColor = valueColor
		};
		((Control)obj2).HorizontalExpand = true;
		((Control)obj2).Margin = new Thickness(0f, 0f, 10f, 0f);
		obj2.IsPulse = num2 >= 2f;
		PubgStatCard pubgStatCard2 = obj2;
		((Control)val2).AddChild((Control)(object)pubgStatCard2);
		int num3 = ((_totalGames > 0) ? (_totalDamage / _totalGames) : 0);
		Color valueColor2 = ((num3 >= 500) ? Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null) : ((num3 >= 200) ? Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#FF9500", (Color?)null)));
		PubgStatCard obj3 = new PubgStatCard
		{
			Title = "УРОН/ИГРА",
			Value = num3.ToString(),
			ValueColor = valueColor2
		};
		((Control)obj3).HorizontalExpand = true;
		obj3.IsPulse = num3 >= 500;
		PubgStatCard pubgStatCard3 = obj3;
		((Control)val2).AddChild((Control)(object)pubgStatCard3);
		float num4 = ((_totalGames > 0) ? ((float)_totalKills / (float)_totalGames) : 0f);
		Color valueColor3 = ((num4 >= 3f) ? Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null) : ((num4 >= 1f) ? Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#FF9500", (Color?)null)));
		PubgStatCard obj4 = new PubgStatCard
		{
			Title = "УБИЙСТВ/ИГРА",
			Value = $"{num4:F1}",
			ValueColor = valueColor3
		};
		((Control)obj4).HorizontalExpand = true;
		((Control)obj4).Margin = new Thickness(10f, 0f, 0f, 0f);
		obj4.IsPulse = num4 >= 3f;
		PubgStatCard pubgStatCard4 = obj4;
		((Control)val2).AddChild((Control)(object)pubgStatCard4);
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}

	private BoxContainer CreateLeaderboardSection()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true
		};
		Label val2 = new Label
		{
			Text = "ТОП ИГРОКОВ",
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 10f)
		};
		((Control)val2).SetOnlyStyleClass("LabelHeading");
		((Control)val).AddChild((Control)(object)val2);
		AlternatingBGContainer alternatingBGContainer = new AlternatingBGContainer();
		((BoxContainer)alternatingBGContainer).Orientation = (LayoutOrientation)1;
		alternatingBGContainer.HorizontalExpand = true;
		AlternatingBGContainer alternatingBGContainer2 = alternatingBGContainer;
		bool flag = false;
		if (_leaderboard.Count > 0)
		{
			foreach (LeaderboardEntryInfo item in _leaderboard)
			{
				bool flag2 = item.Rank == _playerRank && _playerRank > 0;
				if (flag2)
				{
					flag = true;
				}
				alternatingBGContainer2.AddControl((Control)(object)CreateLeaderboardRow(item, flag2));
			}
			if (!flag && _playerRank > 0)
			{
				Label control = new Label
				{
					Text = "...",
					FontColorOverride = Color.Gray,
					HorizontalAlignment = (HAlignment)2,
					Margin = new Thickness(0f, 5f, 0f, 5f)
				};
				alternatingBGContainer2.AddControl((Control)(object)control);
				LeaderboardEntryInfo entry = new LeaderboardEntryInfo
				{
					Rank = _playerRank,
					Username = "Вы",
					Rating = _playerRating,
					Games = _totalGames,
					Wins = _wins,
					Kills = _totalKills,
					DamageDealt = _totalDamage,
					SurvivalTime = _avgSurvivalTime
				};
				alternatingBGContainer2.AddControl((Control)(object)CreateLeaderboardRow(entry, isCurrentPlayer: true));
			}
		}
		else
		{
			Label control2 = new Label
			{
				Text = "Нет данных",
				FontColorOverride = Color.Gray,
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(0f, 10f, 0f, 10f)
			};
			alternatingBGContainer2.AddControl((Control)(object)control2);
		}
		((Control)val).AddChild((Control)(object)alternatingBGContainer2);
		return val;
	}

	private BoxContainer CreateMatchHistorySection()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			Margin = new Thickness(0f, 15f, 0f, 0f)
		};
		Label val2 = new Label
		{
			Text = "ПОСЛЕДНИЕ МАТЧИ",
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 10f)
		};
		((Control)val2).SetOnlyStyleClass("LabelHeading");
		((Control)val).AddChild((Control)(object)val2);
		AlternatingBGContainer alternatingBGContainer = new AlternatingBGContainer();
		((BoxContainer)alternatingBGContainer).Orientation = (LayoutOrientation)1;
		alternatingBGContainer.HorizontalExpand = true;
		AlternatingBGContainer alternatingBGContainer2 = alternatingBGContainer;
		if (_matchHistory.Count > 0)
		{
			foreach (MatchHistoryInfo item in _matchHistory.Take(5))
			{
				alternatingBGContainer2.AddControl((Control)(object)CreateMatchHistoryRow(item));
			}
		}
		else
		{
			Label control = new Label
			{
				Text = "Нет данных",
				FontColorOverride = Color.Gray,
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(0f, 10f, 0f, 10f)
			};
			alternatingBGContainer2.AddControl((Control)(object)control);
		}
		((Control)val).AddChild((Control)(object)alternatingBGContainer2);
		return val;
	}

	private BoxContainer CreateMatchHistoryRow(MatchHistoryInfo match)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Expected O, but got Unknown
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true
		};
		string text = (match.IsWin ? "W" : "L");
		Color value = (match.IsWin ? Color.Gold : Color.Red);
		Label val2 = new Label
		{
			Text = text,
			FontColorOverride = value,
			MinWidth = 25f,
			Margin = new Thickness(8f, 5f, 5f, 5f)
		};
		((Control)val).AddChild((Control)(object)val2);
		Color value2 = ((match.Placement == 1) ? Color.Gold : ((match.Placement <= 3) ? Color.LimeGreen : ((match.Placement <= 10) ? Color.Yellow : Color.White)));
		Label val3 = new Label
		{
			Text = $"#{match.Placement}",
			FontColorOverride = value2,
			MinWidth = 35f,
			Margin = new Thickness(5f, 5f, 5f, 5f)
		};
		((Control)val).AddChild((Control)(object)val3);
		Label val4 = new Label
		{
			Text = $"{match.Kills}K",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FF6B6B", (Color?)null),
			MinWidth = 35f,
			Margin = new Thickness(5f, 5f, 5f, 5f)
		};
		((Control)val).AddChild((Control)(object)val4);
		Label val5 = new Label
		{
			Text = $"{match.Damage}",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFAA55", (Color?)null),
			MinWidth = 50f,
			Margin = new Thickness(5f, 5f, 5f, 5f)
		};
		((Control)val).AddChild((Control)(object)val5);
		Color value3 = ((match.RatingChange >= 0) ? Color.LimeGreen : Color.Red);
		string value4 = ((match.RatingChange >= 0) ? "+" : "");
		Label val6 = new Label
		{
			Text = $"{value4}{match.RatingChange}",
			FontColorOverride = value3,
			HorizontalExpand = true,
			HorizontalAlignment = (HAlignment)3,
			Margin = new Thickness(5f, 5f, 8f, 5f)
		};
		((Control)val).AddChild((Control)(object)val6);
		return val;
	}

	private BoxContainer CreateLeaderboardRow(LeaderboardEntryInfo entry, bool isCurrentPlayer)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true
		};
		Color value = (Color)(entry.Rank switch
		{
			1 => Color.Gold, 
			2 => Color.Silver, 
			3 => Color.FromHex((ReadOnlySpan<char>)"#CD7F32", (Color?)null), 
			_ => isCurrentPlayer ? Color.LimeGreen : Color.White, 
		});
		Label val2 = new Label
		{
			Text = $"#{entry.Rank}",
			FontColorOverride = value,
			MinWidth = 35f,
			Margin = new Thickness(8f, 5f, 5f, 5f)
		};
		Label val3 = new Label
		{
			Text = entry.Username,
			FontColorOverride = (isCurrentPlayer ? Color.LimeGreen : Color.White),
			HorizontalExpand = true,
			Margin = new Thickness(5f, 5f, 5f, 5f)
		};
		Label val4 = new Label
		{
			Text = entry.Rating.ToString(),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#88AAFF", (Color?)null),
			Margin = new Thickness(5f, 5f, 8f, 5f)
		};
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val4);
		return val;
	}

	private void LoadSponsorsView()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		ScrollContainer val = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			Margin = new Thickness(20f)
		};
		Label val3 = new Label
		{
			Text = "✦ СПОНСОРКА ✦",
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 25f)
		};
		((Control)val3).SetOnlyStyleClass("LabelHeading");
		((Control)val2).AddChild((Control)(object)val3);
		PanelContainer val4 = CreateMainTierCard();
		((Control)val2).AddChild((Control)(object)val4);
		BoxContainer val5 = CreateDisplaySettingsLauncherSection();
		if (ShowDisplaySettings)
		{
			((Control)val2).AddChild((Control)(object)val5);
		}
		if (_sponsorActiveTiers.Count > 0)
		{
			BoxContainer val6 = CreateActiveTiersSection();
			((Control)val2).AddChild((Control)(object)val6);
		}
		BoxContainer val7 = CreatePermissionsSection();
		((Control)val2).AddChild((Control)(object)val7);
		BoxContainer val8 = CreateSponsorButton();
		((Control)val2).AddChild((Control)(object)val8);
		((Control)val).AddChild((Control)(object)val2);
		((Control)this).AddChild((Control)(object)val);
	}

	private PanelContainer CreateMainTierCard()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Expected O, but got Unknown
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Expected O, but got Unknown
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 20f),
			StyleClasses = { "AngleRect" }
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(50f, 20f, 50f, 20f)
		};
		if (_sponsorDisplayTier != null)
		{
			Color value = ((!string.IsNullOrEmpty(_sponsorDisplayTier.Color)) ? Color.FromHex((ReadOnlySpan<char>)_sponsorDisplayTier.Color, (Color?)null) : Color.Gold);
			Label val3 = new Label
			{
				Text = "ВАШ СТАТУС",
				FontColorOverride = Color.Gray,
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(0f, 0f, 0f, 10f)
			};
			((Control)val2).AddChild((Control)(object)val3);
			if (!string.IsNullOrEmpty(_sponsorDisplayTier.Badge))
			{
				Label val4 = new Label
				{
					Text = _sponsorDisplayTier.Badge,
					HorizontalAlignment = (HAlignment)2,
					Margin = new Thickness(0f, 0f, 0f, 5f)
				};
				((Control)val2).AddChild((Control)(object)val4);
			}
			Label val5 = new Label
			{
				Text = _sponsorDisplayTier.TierName,
				FontColorOverride = value,
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val5).SetOnlyStyleClass("LabelHeading");
			((Control)val2).AddChild((Control)(object)val5);
			Label val6 = new Label
			{
				Text = "Спасибо за поддержку! ♥",
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FF6B9D", (Color?)null),
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(0f, 10f, 0f, 0f)
			};
			((Control)val2).AddChild((Control)(object)val6);
		}
		else
		{
			Label val7 = new Label
			{
				Text = "У вас пока нет спонсорского статуса",
				FontColorOverride = Color.Gray,
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(0f, 10f, 0f, 10f)
			};
			((Control)val2).AddChild((Control)(object)val7);
			Label val8 = new Label
			{
				Text = "Поддержите проект и получите бонусы!",
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#88AAFF", (Color?)null),
				HorizontalAlignment = (HAlignment)2
			};
			((Control)val2).AddChild((Control)(object)val8);
		}
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}

	private BoxContainer CreateDisplaySettingsLauncherSection()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(0f, 0f, 0f, 20f)
		};
		Label val2 = new Label
		{
			Text = Loc.GetString("mainmenu-sponsor-display-title"),
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 4f)
		};
		((Control)val2).SetOnlyStyleClass("LabelHeading");
		((Control)val).AddChild((Control)(object)val2);
		Label val3 = new Label
		{
			Text = Loc.GetString("mainmenu-sponsor-display-subtitle"),
			FontColorOverride = Color.Gray,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 10f)
		};
		((Control)val).AddChild((Control)(object)val3);
		string item = Loc.GetString("mainmenu-sponsor-display-current-none");
		if (_sponsorDisplayMode == SponsorDisplayMode.Hidden)
		{
			item = Loc.GetString("mainmenu-sponsor-display-current-hidden");
		}
		else if (_sponsorDisplayTier != null)
		{
			item = _sponsorDisplayTier.TierName;
		}
		Label val4 = new Label();
		val4.Text = Loc.GetString("mainmenu-sponsor-display-current", new(string, object)[1] { ("value", item) });
		((Control)val4).HorizontalAlignment = (HAlignment)2;
		((Control)val4).Margin = new Thickness(0f, 0f, 0f, 8f);
		Label val5 = val4;
		((Control)val).AddChild((Control)(object)val5);
		if (_sponsorDisplayUpdating)
		{
			Label val6 = new Label
			{
				Text = Loc.GetString("mainmenu-sponsor-display-updating"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#88AAFF", (Color?)null),
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(0f, 0f, 0f, 8f)
			};
			((Control)val).AddChild((Control)(object)val6);
		}
		Button val7 = new Button
		{
			Text = Loc.GetString("mainmenu-sponsor-display-open-button"),
			HorizontalAlignment = (HAlignment)2,
			MinWidth = 260f,
			Disabled = _sponsorDisplayUpdating
		};
		((BaseButton)val7).OnPressed += delegate
		{
			RequestSponsorDisplaySettingsOpen();
		};
		((Control)val).AddChild((Control)(object)val7);
		return val;
	}

	private void RequestSponsorDisplaySettingsOpen()
	{
		if (!_sponsorDisplayUpdating)
		{
			_sponsorDisplayUpdating = true;
			if (_currentSubcategory == ProfileSubcategory.Sponsors)
			{
				LoadSubcategory(ProfileSubcategory.Sponsors);
			}
			this.OnOpenSponsorDisplaySettingsRequested?.Invoke();
		}
	}

	private BoxContainer CreateActiveTiersSection()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(0f, 0f, 0f, 20f)
		};
		Label val2 = new Label
		{
			Text = "АКТИВНЫЕ ПОДПИСКИ",
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 10f)
		};
		((Control)val2).SetOnlyStyleClass("LabelHeading");
		((Control)val).AddChild((Control)(object)val2);
		AlternatingBGContainer alternatingBGContainer = new AlternatingBGContainer();
		((BoxContainer)alternatingBGContainer).Orientation = (LayoutOrientation)1;
		alternatingBGContainer.HorizontalExpand = true;
		((Control)alternatingBGContainer).Margin = new Thickness(50f, 0f, 50f, 0f);
		AlternatingBGContainer alternatingBGContainer2 = alternatingBGContainer;
		foreach (SponsorActiveTierInfo sponsorActiveTier in _sponsorActiveTiers)
		{
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				HorizontalExpand = true
			};
			Label val4 = new Label
			{
				Text = "★ " + sponsorActiveTier.Name,
				FontColorOverride = Color.Gold,
				HorizontalExpand = true,
				Margin = new Thickness(10f, 8f, 10f, 8f)
			};
			((Control)val3).AddChild((Control)(object)val4);
			if (sponsorActiveTier.ExpiresAt.HasValue)
			{
				TimeSpan timeSpan = sponsorActiveTier.ExpiresAt.Value - DateTime.UtcNow;
				string text;
				Color value;
				if (timeSpan.TotalDays >= 30.0)
				{
					text = $"{(int)(timeSpan.TotalDays / 30.0)} мес.";
					value = Color.LimeGreen;
				}
				else if (timeSpan.TotalDays >= 1.0)
				{
					text = $"{(int)timeSpan.TotalDays} дн.";
					value = ((timeSpan.TotalDays < 7.0) ? Color.Orange : Color.LimeGreen);
				}
				else if (timeSpan.TotalHours >= 1.0)
				{
					text = $"{(int)timeSpan.TotalHours} ч.";
					value = Color.Red;
				}
				else
				{
					text = "< 1 ч.";
					value = Color.Red;
				}
				Label val5 = new Label
				{
					Text = "⏱ " + text,
					FontColorOverride = value,
					Margin = new Thickness(10f, 8f, 10f, 8f)
				};
				((Control)val3).AddChild((Control)(object)val5);
			}
			else
			{
				Label val6 = new Label
				{
					Text = "∞ Навсегда",
					FontColorOverride = Color.LimeGreen,
					Margin = new Thickness(10f, 8f, 10f, 8f)
				};
				((Control)val3).AddChild((Control)(object)val6);
			}
			alternatingBGContainer2.AddControl((Control)(object)val3);
		}
		((Control)val).AddChild((Control)(object)alternatingBGContainer2);
		return val;
	}

	private BoxContainer CreatePermissionsSection()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(0f, 0f, 0f, 20f)
		};
		Label val2 = new Label
		{
			Text = "ВАШИ ПРИВИЛЕГИИ",
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 10f)
		};
		((Control)val2).SetOnlyStyleClass("LabelHeading");
		((Control)val).AddChild((Control)(object)val2);
		if (_sponsorPermissionDetails.Count > 0)
		{
			foreach (IGrouping<string, SponsorPermissionDetailInfo> item in from p in _sponsorPermissionDetails
				group p by p.Source into g
				orderby (!g.Key.StartsWith("Тир")) ? 1 : 0
				select g)
			{
				Label val3 = new Label
				{
					Text = item.Key,
					FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#88AAFF", (Color?)null),
					Margin = new Thickness(50f, 10f, 0f, 5f)
				};
				((Control)val).AddChild((Control)(object)val3);
				AlternatingBGContainer alternatingBGContainer = new AlternatingBGContainer();
				((BoxContainer)alternatingBGContainer).Orientation = (LayoutOrientation)1;
				alternatingBGContainer.HorizontalExpand = true;
				((Control)alternatingBGContainer).Margin = new Thickness(50f, 0f, 50f, 0f);
				AlternatingBGContainer alternatingBGContainer2 = alternatingBGContainer;
				foreach (SponsorPermissionDetailInfo item2 in item)
				{
					alternatingBGContainer2.AddControl((Control)(object)CreatePermissionRow(item2, showSource: false));
				}
				((Control)val).AddChild((Control)(object)alternatingBGContainer2);
			}
		}
		else
		{
			Label val4 = new Label
			{
				Text = "Станьте спонсором, чтобы получить привилегии!",
				FontColorOverride = Color.Gray,
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(0f, 15f, 0f, 15f)
			};
			((Control)val).AddChild((Control)(object)val4);
		}
		return val;
	}

	private BoxContainer CreateSponsorButton()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 10f, 0f, 20f)
		};
		Button val2 = new Button
		{
			Text = "♥ СТАТЬ СПОНСОРОМ",
			MinWidth = 250f,
			MinHeight = 50f
		};
		((Control)val2).StyleClasses.Add("ButtonColorGreen");
		((BaseButton)val2).OnPressed += delegate
		{
			IoCManager.Resolve<IUriOpener>().OpenUri(new Uri("https://discord.gg/xdQ4vSKRB8"));
		};
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}

	private BoxContainer CreatePermissionRow(SponsorPermissionDetailInfo detail, bool showSource = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true
		};
		Label val2 = new Label
		{
			Text = detail.Name,
			HorizontalExpand = true,
			Margin = new Thickness(10f, 5f, 10f, 5f)
		};
		Label val3 = new Label
		{
			Text = ((detail.StackCount > 1) ? $"x{detail.StackCount}" : "✓"),
			FontColorOverride = Color.LimeGreen,
			Margin = new Thickness(10f, 5f, 10f, 5f),
			MinWidth = 40f
		};
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val3);
		if (showSource)
		{
			Label val4 = new Label
			{
				Text = detail.Source,
				FontColorOverride = Color.Gray,
				Margin = new Thickness(10f, 5f, 10f, 5f),
				MinWidth = 120f
			};
			((Control)val).AddChild((Control)(object)val4);
		}
		if (detail.ExpiresAt.HasValue)
		{
			TimeSpan timeSpan = detail.ExpiresAt.Value - DateTime.UtcNow;
			string text = ((timeSpan.TotalDays >= 1.0) ? $"{(int)timeSpan.TotalDays}д" : ((!(timeSpan.TotalHours >= 1.0)) ? "<1ч" : $"{(int)timeSpan.TotalHours}ч"));
			Label val5 = new Label
			{
				Text = "⏱ " + text,
				FontColorOverride = ((timeSpan.TotalDays < 3.0) ? Color.Orange : Color.Gray),
				Margin = new Thickness(10f, 5f, 10f, 5f),
				MinWidth = 60f
			};
			((Control)val).AddChild((Control)(object)val5);
		}
		return val;
	}

	public void UpdateSponsorData(Dictionary<string, int> permissions, List<SponsorPermissionDetailInfo> permissionDetails, SponsorTierInfo? displayTier, List<SponsorActiveTierInfo> activeTiers, SponsorDisplayMode displayMode, string? preferredTierKey)
	{
		_sponsorPermissions = permissions;
		_sponsorPermissionDetails = permissionDetails;
		_sponsorDisplayTier = displayTier;
		_sponsorActiveTiers = activeTiers;
		_sponsorDisplayMode = displayMode;
		_sponsorPreferredTierKey = preferredTierKey;
		_sponsorDisplayUpdating = false;
		if (_currentSubcategory == ProfileSubcategory.Sponsors)
		{
			LoadSubcategory(ProfileSubcategory.Sponsors);
		}
	}

	public void UpdateStatsData(int totalGames, int wins, int totalKills, int totalDamage, int totalSurvivalTime, int totalCaseDropSkins, int unlockedCaseDropSkins, int totalEmotes, int availableEmotes, int totalDeaths, int reputation)
	{
		_totalGames = totalGames;
		_wins = wins;
		_totalKills = totalKills;
		_totalDamage = totalDamage;
		_avgSurvivalTime = totalSurvivalTime;
		_totalCaseDropSkins = totalCaseDropSkins;
		_unlockedCaseDropSkins = unlockedCaseDropSkins;
		_totalEmotes = totalEmotes;
		_availableEmotes = availableEmotes;
		_totalDeaths = totalDeaths;
		_reputation = reputation;
		if (_currentSubcategory == ProfileSubcategory.Stats)
		{
			LoadSubcategory(ProfileSubcategory.Stats);
		}
	}

	public void UpdateMatchHistoryData(List<MatchHistoryInfo> matchHistory)
	{
		_matchHistory = matchHistory;
		if (_currentSubcategory == ProfileSubcategory.Stats)
		{
			LoadSubcategory(ProfileSubcategory.Stats);
		}
	}

	public void UpdateLeaderboardData(List<LeaderboardEntryInfo> leaderboard, int playerRank, int playerRating)
	{
		_leaderboard = leaderboard;
		_playerRank = playerRank;
		_playerRating = playerRating;
		if (_currentSubcategory == ProfileSubcategory.Stats)
		{
			LoadSubcategory(ProfileSubcategory.Stats);
		}
	}

	private BoxContainer CreateStatRow(string label, string value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true
		};
		Label val2 = new Label
		{
			Text = label,
			HorizontalExpand = true,
			Margin = new Thickness(10f, 5f, 10f, 5f)
		};
		Label val3 = new Label
		{
			Text = value,
			FontColorOverride = Color.LightGreen,
			HorizontalAlignment = (HAlignment)3,
			Margin = new Thickness(10f, 5f, 10f, 5f)
		};
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val3);
		return val;
	}
}
