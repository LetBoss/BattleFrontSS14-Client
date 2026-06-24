using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
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

namespace Content.Client._CIV14merka.RoundEnd;

public sealed class CivRoundEndWindow : DefaultWindow
{
	private readonly IPrototypeManager _proto;

	private readonly BoxContainer _summaryContainer;

	private readonly BoxContainer _myStatsContainer;

	private readonly BoxContainer _teamEntriesContainer;

	public CivRoundEndWindow()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected O, but got Unknown
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		_proto = IoCManager.Resolve<IPrototypeManager>();
		((DefaultWindow)this).Title = Loc.GetString("civ-ui-roundend-title");
		((BaseWindow)this).Resizable = false;
		((Control)this).MinSize = new Vector2(960f, 720f);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			Margin = new Thickness(10f),
			VerticalExpand = true,
			HorizontalExpand = true
		};
		TabContainer val2 = new TabContainer
		{
			VerticalExpand = true,
			HorizontalExpand = true
		};
		_summaryContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			VerticalExpand = true,
			HorizontalExpand = true
		};
		((Control)val2).AddChild(WrapScrollTab((Control)(object)_summaryContainer, Loc.GetString("civ-ui-roundend-tab-summary")));
		_myStatsContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			VerticalExpand = true,
			HorizontalExpand = true
		};
		((Control)val2).AddChild(WrapScrollTab((Control)(object)_myStatsContainer, Loc.GetString("civ-ui-roundend-tab-mystats")));
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Name = Loc.GetString("civ-ui-roundend-tab-manifest"),
			SeparationOverride = 6
		};
		((Control)val3).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-ui-roundend-manifest-subtitle"),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#B7C2D8", (Color?)null)
		});
		((Control)val3).AddChild(BuildHeaderRow());
		ScrollContainer val4 = new ScrollContainer
		{
			VerticalExpand = true,
			HorizontalExpand = true
		};
		_teamEntriesContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			VerticalExpand = true,
			HorizontalExpand = true
		};
		((Control)val4).AddChild((Control)(object)_teamEntriesContainer);
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		Button val5 = new Button
		{
			Text = Loc.GetString("civ-ui-roundend-close"),
			HorizontalAlignment = (HAlignment)2,
			MinSize = new Vector2(160f, 34f)
		};
		((BaseButton)val5).OnPressed += delegate
		{
			((BaseWindow)this).Close();
		};
		((Control)val).AddChild((Control)(object)val5);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}

	private static Control WrapScrollTab(Control content, string tabName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Name = tabName,
			VerticalExpand = true,
			HorizontalExpand = true
		};
		ScrollContainer val2 = new ScrollContainer
		{
			VerticalExpand = true,
			HorizontalExpand = true
		};
		((Control)val2).AddChild(content);
		((Control)val).AddChild((Control)(object)val2);
		return (Control)val;
	}

	public void SetTitleText(string title)
	{
		((DefaultWindow)this).Title = (string.IsNullOrWhiteSpace(title) ? Loc.GetString("civ-ui-roundend-title") : title);
	}

	public void SetSummary(CivRoundEndSummary summary)
	{
		((Control)_summaryContainer).RemoveAllChildren();
		((Control)_summaryContainer).AddChild(BuildSummaryHero(summary));
		if (summary.Sides.Count > 0)
		{
			((Control)_summaryContainer).AddChild(BuildSidesRow(summary.Sides));
		}
		BoxContainer val = BuildRow(BuildSummaryCard(Loc.GetString("civ-ui-roundend-card-mode"), summary.ModeText), BuildSummaryCard(Loc.GetString("civ-ui-roundend-card-map"), summary.MapText), BuildSummaryCard(Loc.GetString("civ-ui-roundend-card-duration"), summary.DurationText), BuildSummaryCard(Loc.GetString("civ-ui-roundend-card-lobby-return"), (summary.LobbyReturnSeconds > 0) ? Loc.GetString("civ-ui-roundend-lobby-return-seconds", new(string, object)[1] { ("seconds", summary.LobbyReturnSeconds) }) : Loc.GetString("civ-ui-roundend-lobby-return-instant")));
		((Control)_summaryContainer).AddChild((Control)(object)val);
		if (summary.TopAwards.Count > 0)
		{
			((Control)_summaryContainer).AddChild(BuildTopAwardsPanel(summary.TopAwards));
		}
	}

	private Control BuildSidesRow(List<CivRoundEndSideInfo> sides)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 12,
			HorizontalExpand = true
		};
		foreach (CivRoundEndSideInfo side in sides)
		{
			((Control)val).AddChild(BuildSideCard(side));
		}
		return (Control)(object)val;
	}

	private Control BuildSideCard(CivRoundEndSideInfo side)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		PanelContainer val = new PanelContainer
		{
			HorizontalExpand = true,
			PanelOverride = (StyleBox)(object)BuildPanelStyle(side.IsWinner ? "#1E2A1BF4" : "#141B2BF4", side.IsWinner ? "#9BDE7E" : "#5F7198")
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = side.TeamName,
			FontColorOverride = (side.IsWinner ? Color.FromHex((ReadOnlySpan<char>)"#BCF29A", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#E2E8F4", (Color?)null)),
			StyleClasses = { "LabelHeading" }
		});
		if (side.IsWinner)
		{
			((Control)val2).AddChild((Control)new Label
			{
				Text = Loc.GetString("civ-ui-roundend-side-winner"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#9BDE7E", (Color?)null)
			});
		}
		Label val3;
		if (side.HasScore)
		{
			val3 = new Label();
			val3.Text = Loc.GetString("civ-ui-roundend-side-score", new(string, object)[1] { ("score", side.Score) });
			val3.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFD05C", (Color?)null);
			((Control)val3).StyleClasses.Add("LabelHeading");
			((Control)val2).AddChild((Control)(object)val3);
		}
		val3 = new Label();
		val3.Text = (string.IsNullOrWhiteSpace(side.CommanderName) ? Loc.GetString("civ-ui-roundend-side-no-commander") : Loc.GetString("civ-ui-roundend-side-commander", new(string, object)[1] { ("name", side.CommanderName) }));
		val3.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFE39A", (Color?)null);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private Control BuildTopAwardsPanel(List<CivRoundTopAward> awards)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)BuildPanelStyle("#1B1F2CEE", "#5F7198")
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-ui-roundend-top-awards"),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFD05C", (Color?)null),
			StyleClasses = { "LabelHeading" }
		});
		foreach (CivRoundTopAward award in awards)
		{
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 10,
				HorizontalExpand = true
			};
			((Control)val3).AddChild((Control)new Label
			{
				Text = award.Title,
				MinWidth = 200f,
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFE39A", (Color?)null)
			});
			((Control)val3).AddChild((Control)new Label
			{
				Text = award.PlayerName,
				MinWidth = 200f,
				HorizontalExpand = true,
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#E2E8F4", (Color?)null)
			});
			((Control)val3).AddChild((Control)new Label
			{
				Text = award.ValueText,
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#8DC8FF", (Color?)null)
			});
			((Control)val2).AddChild((Control)(object)val3);
		}
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private Control BuildSummaryHero(CivRoundEndSummary summary)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)BuildPanelStyle("#141B2BF4", "#5F7198")
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = summary.WinnerText,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFD05C", (Color?)null),
			StyleClasses = { "LabelHeading" }
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = summary.ReasonText,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#E2E8F4", (Color?)null)
		});
		((Control)val).AddChild((Control)(object)val2);
		return (Control)val;
	}

	public void SetMyStats(NetUserId? localUser, List<CivRoundEndTeamEntry> entries)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		((Control)_myStatsContainer).RemoveAllChildren();
		CivRoundEndPlayerEntry civRoundEndPlayerEntry = FindLocalEntry(localUser, entries);
		CivPlayerRoundStats civPlayerRoundStats = civRoundEndPlayerEntry?.Stats;
		if (civPlayerRoundStats == null)
		{
			((Control)_myStatsContainer).AddChild((Control)new Label
			{
				Text = Loc.GetString("civ-ui-roundend-my-none"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A3AEBD", (Color?)null)
			});
			return;
		}
		((Control)_myStatsContainer).AddChild(BuildMyHero(civRoundEndPlayerEntry, civPlayerRoundStats));
		if (civRoundEndPlayerEntry.IsCommander)
		{
			BuildCommanderSections(civPlayerRoundStats);
		}
		else
		{
			BuildSoldierSections(civPlayerRoundStats);
		}
	}

	private static CivRoundEndPlayerEntry? FindLocalEntry(NetUserId? localUser, List<CivRoundEndTeamEntry> entries)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (localUser.HasValue)
		{
			NetUserId valueOrDefault = localUser.GetValueOrDefault();
			foreach (CivRoundEndTeamEntry entry in entries)
			{
				foreach (CivRoundEndPlayerEntry player in entry.Players)
				{
					CivPlayerRoundStats? stats = player.Stats;
					if (stats != null && stats.UserId == valueOrDefault)
					{
						return player;
					}
				}
			}
			return null;
		}
		return null;
	}

	private Control BuildMyHero(CivRoundEndPlayerEntry me, CivPlayerRoundStats stats)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)BuildPanelStyle("#141B2BF4", "#5F7198")
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = me.PlayerName,
			FontColorOverride = (me.IsCommander ? Color.FromHex((ReadOnlySpan<char>)"#FFE39A", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#F4F7FB", (Color?)null)),
			StyleClasses = { "LabelHeading" }
		});
		string text = (me.IsCommander ? Loc.GetString("civ-ui-roundend-my-role-commander") : me.RoleText);
		((Control)val2).AddChild((Control)new Label
		{
			Text = (string.IsNullOrWhiteSpace(me.SquadText) ? text : (text + " · " + me.SquadText)),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#B7C2D8", (Color?)null)
		});
		if (stats.Awards.Count > 0)
		{
			Label val3 = new Label();
			val3.Text = Loc.GetString("civ-ui-roundend-my-awards", new(string, object)[1] { ("awards", string.Join(", ", stats.Awards)) });
			val3.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFD05C", (Color?)null);
			((Control)val2).AddChild((Control)(object)val3);
		}
		((Control)val2).AddChild((Control)(object)(me.IsCommander ? BuildRow(BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-score"), stats.Score.ToString()), BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-approvals"), stats.CommanderPurchasesApproved.ToString()), BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-spent"), (stats.CommanderPointsSpent + stats.CommanderShopSpent).ToString()), BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-firekills"), (stats.AirstrikeKills + stats.ArtilleryKills).ToString())) : BuildRow(BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-score"), stats.Score.ToString()), BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-kd"), $"{stats.Kills}/{stats.Deaths}"), BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-kdr"), Kdr(stats)), BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-damage"), stats.DamageDealt.ToString()), BuildSummaryCard(Loc.GetString("civ-ui-roundend-my-m-survival"), FormatDuration(stats.SurvivalTime)))));
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private void BuildSoldierSections(CivPlayerRoundStats stats)
	{
		List<string> list = new List<string>();
		list.Add(Loc.GetString("civ-ui-roundend-my-combat", new(string, object)[3]
		{
			("kills", stats.Kills),
			("deaths", stats.Deaths),
			("kdr", Kdr(stats))
		}));
		list.Add(Loc.GetString("civ-ui-roundend-my-combat2", new(string, object)[3]
		{
			("streak", stats.BestKillstreak),
			("multi", stats.BestMultikill),
			("acc", Accuracy(stats))
		}));
		list.Add(Loc.GetString("civ-ui-roundend-my-combat3", new(string, object)[3]
		{
			("dealt", stats.DamageDealt),
			("taken", stats.DamageTaken),
			("tk", (stats.TeamKills > 0) ? Loc.GetString("civ-ui-roundend-my-combat-tk", new(string, object)[1] { ("count", stats.TeamKills) }) : "")
		}));
		List<string> lines = list;
		((Control)_myStatsContainer).AddChild(BuildSection("civ-ui-roundend-my-sec-combat", lines));
		((Control)_myStatsContainer).AddChild(BuildSection("civ-ui-roundend-my-sec-weapons", BuildWeaponLines(stats)));
		((Control)_myStatsContainer).AddChild(BuildSection("civ-ui-roundend-my-sec-killlog", BuildKillLogLines(stats)));
		if (stats.PointsCaptured > 0 || stats.PointsRecaptured > 0 || stats.PointHoldSeconds > 0 || stats.PointsDefendedContests > 0)
		{
			((Control)_myStatsContainer).AddChild(BuildSection("civ-ui-roundend-my-sec-objectives", new string[1] { Loc.GetString("civ-ui-roundend-my-obj", new(string, object)[4]
			{
				("captured", stats.PointsCaptured),
				("recaptured", stats.PointsRecaptured),
				("held", stats.PointHoldSeconds),
				("defended", stats.PointsDefendedContests)
			}) }));
		}
		if (stats.HealsApplied > 0 || stats.RevivesApplied > 0 || stats.MinesPlaced > 0 || stats.MineKillsConfirmed > 0 || stats.MortarHits > 0 || stats.MortarShellsFired > 0)
		{
			((Control)_myStatsContainer).AddChild(BuildSection("civ-ui-roundend-my-sec-support", new string[1] { Loc.GetString("civ-ui-roundend-my-support", new(string, object)[8]
			{
				("heals", stats.HealsApplied),
				("hp", stats.HealingDone),
				("revives", stats.RevivesApplied),
				("mines", stats.MinesPlaced),
				("minehits", stats.MineKillsEnemies),
				("minekills", stats.MineKillsConfirmed),
				("mortarhits", stats.MortarHits),
				("mortarfired", stats.MortarShellsFired)
			}) }));
		}
		if (stats.VehicleTimeSeconds > 0 || stats.VehicleKills > 0 || stats.VehiclesDestroyed > 0 || stats.Roadkills > 0)
		{
			((Control)_myStatsContainer).AddChild(BuildSection("civ-ui-roundend-my-sec-vehicle", new string[1] { Loc.GetString("civ-ui-roundend-my-vehicle", new(string, object)[4]
			{
				("time", FormatDuration(TimeSpan.FromSeconds(stats.VehicleTimeSeconds))),
				("kills", stats.VehicleKills),
				("destroyed", stats.VehiclesDestroyed),
				("roadkills", stats.Roadkills)
			}) }));
		}
	}

	private void BuildCommanderSections(CivPlayerRoundStats stats)
	{
		((Control)_myStatsContainer).AddChild(BuildSection("civ-ui-roundend-my-sec-command", new string[1] { Loc.GetString("civ-ui-roundend-my-command", new(string, object)[2]
		{
			("approved", stats.CommanderPurchasesApproved),
			("points", stats.CommanderPointsSpent)
		}) }));
		((Control)_myStatsContainer).AddChild(BuildSection("civ-ui-roundend-my-sec-economy", new string[1] { Loc.GetString("civ-ui-roundend-my-economy", new(string, object)[2]
		{
			("purchases", stats.CommanderShopPurchases),
			("spent", stats.CommanderShopSpent)
		}) }));
		((Control)_myStatsContainer).AddChild(BuildSection("civ-ui-roundend-my-sec-firesupport", new string[1] { Loc.GetString("civ-ui-roundend-my-firesupport", new(string, object)[2]
		{
			("airstrike", stats.AirstrikeKills),
			("artillery", stats.ArtilleryKills)
		}) }));
	}

	private List<string> BuildWeaponLines(CivPlayerRoundStats stats)
	{
		if (stats.WeaponKills.Count == 0 && stats.WeaponDamage.Count == 0)
		{
			return new List<string> { Loc.GetString("civ-ui-roundend-my-weapon-none") };
		}
		return (from w in (from w in stats.WeaponKills.Keys.Concat(stats.WeaponDamage.Keys).Distinct()
				orderby stats.WeaponKills.GetValueOrDefault(w) descending, stats.WeaponDamage.GetValueOrDefault(w) descending
				select w).Take(6)
			select Loc.GetString("civ-ui-roundend-my-weapon-line", new(string, object)[3]
			{
				("name", ResolveWeaponName(w)),
				("kills", stats.WeaponKills.GetValueOrDefault(w)),
				("damage", stats.WeaponDamage.GetValueOrDefault(w))
			})).ToList();
	}

	private List<string> BuildKillLogLines(CivPlayerRoundStats stats)
	{
		if (stats.KillDetails.Count == 0)
		{
			return new List<string> { Loc.GetString("civ-ui-roundend-my-kill-none") };
		}
		List<string> list = (from k in stats.KillDetails.OrderBy((CivKillDetail k) => k.AtSeconds).Take(14)
			select Loc.GetString("civ-ui-roundend-my-kill-line", new(string, object)[5]
			{
				("time", FormatSeconds(k.AtSeconds)),
				("victim", k.Victim),
				("weapon", ResolveWeaponName(k.Weapon)),
				("dist", k.Distance),
				("tk", k.Teamkill ? Loc.GetString("civ-ui-roundend-my-kill-tk") : "")
			})).ToList();
		if (stats.KillDetails.Count > 14)
		{
			list.Add(Loc.GetString("civ-ui-roundend-my-kill-more", new(string, object)[1] { ("count", stats.KillDetails.Count - 14) }));
		}
		return list;
	}

	private Control BuildSection(string headerKey, IEnumerable<string> lines)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)BuildPanelStyle("#1B1F2CEE", "#445574")
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString(headerKey),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#8DC8FF", (Color?)null),
			StyleClasses = { "LabelHeading" }
		});
		foreach (string line in lines)
		{
			((Control)val2).AddChild((Control)new Label
			{
				Text = line,
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#D3DCEF", (Color?)null)
			});
		}
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
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
		{
			EntityPrototype val = default(EntityPrototype);
			if (!_proto.TryIndex<EntityPrototype>(protoId, ref val))
			{
				return protoId;
			}
			return val.Name;
		}
		}
	}

	private static string Kdr(CivPlayerRoundStats stats)
	{
		return ((float)stats.Kills / (float)Math.Max(1, stats.Deaths)).ToString("F2");
	}

	private static int Accuracy(CivPlayerRoundStats stats)
	{
		int num = stats.ShotsFired.Values.Sum();
		if (num <= 0)
		{
			return 0;
		}
		return (int)Math.Round((double)stats.ShotsHit.Values.Sum() * 100.0 / (double)num);
	}

	private static string FormatDuration(TimeSpan span)
	{
		return $"{(int)span.TotalMinutes}:{span.Seconds:D2}";
	}

	private static string FormatSeconds(int seconds)
	{
		return $"{seconds / 60}:{seconds % 60:D2}";
	}

	public void SetTeamEntries(List<CivRoundEndTeamEntry> entries)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		((Control)_teamEntriesContainer).RemoveAllChildren();
		if (entries.Count == 0)
		{
			((Control)_teamEntriesContainer).AddChild((Control)new Label
			{
				Text = Loc.GetString("civ-ui-roundend-no-team-data"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A3AEBD", (Color?)null)
			});
			return;
		}
		foreach (CivRoundEndTeamEntry entry in entries)
		{
			((Control)_teamEntriesContainer).AddChild(BuildTeamGroup(entry));
		}
	}

	private Control BuildHeaderRow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#101521EE", (Color?)null),
				BorderColor = Color.FromHex((ReadOnlySpan<char>)"#4F5F82", (Color?)null),
				BorderThickness = new Thickness(1f)
			}
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Margin = new Thickness(8f, 6f),
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-ui-roundend-header-player"),
			HorizontalExpand = true,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A7B6CE", (Color?)null)
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-ui-roundend-header-role"),
			MinWidth = 180f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A7B6CE", (Color?)null)
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-ui-roundend-header-squad"),
			MinWidth = 110f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A7B6CE", (Color?)null)
		});
		((Control)val).AddChild((Control)(object)val2);
		return (Control)val;
	}

	private Control BuildTeamGroup(CivRoundEndTeamEntry entry)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)BuildPanelStyle("#1B1F2CEE", "#4F5F82")
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = (string.IsNullOrWhiteSpace(entry.RoleName) ? entry.TeamName : (entry.TeamName + " (" + entry.RoleName + ")")),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFD05C", (Color?)null),
			StyleClasses = { "LabelHeading" }
		});
		if (entry.Players.Count == 0)
		{
			((Control)val2).AddChild((Control)new Label
			{
				Text = Loc.GetString("civ-ui-roundend-no-players"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A3AEBD", (Color?)null)
			});
		}
		else
		{
			foreach (CivRoundEndPlayerEntry player in entry.Players)
			{
				((Control)val2).AddChild(BuildPlayerRow(player));
			}
		}
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private Control BuildPlayerRow(CivRoundEndPlayerEntry entry)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 2,
			HorizontalExpand = true
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = entry.PlayerName,
			HorizontalExpand = true,
			FontColorOverride = (entry.IsCommander ? Color.FromHex((ReadOnlySpan<char>)"#FFE39A", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#E2E8F4", (Color?)null))
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = entry.RoleText,
			MinWidth = 180f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#D3DCEF", (Color?)null)
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = entry.SquadText,
			MinWidth = 110f,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#8DC8FF", (Color?)null)
		});
		((Control)val).AddChild((Control)(object)val2);
		CivPlayerRoundStats stats = entry.Stats;
		if (stats != null)
		{
			string text = FormatPlayerStats(stats, entry.IsCommander);
			if (!string.IsNullOrEmpty(text))
			{
				((Control)val).AddChild((Control)new Label
				{
					Text = text,
					HorizontalExpand = true,
					FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A7B6CE", (Color?)null)
				});
			}
		}
		return (Control)(object)val;
	}

	private static string FormatPlayerStats(CivPlayerRoundStats stats, bool isCommander)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("    ");
		stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-score", new(string, object)[1] { ("score", stats.Score) }));
		if (stats.Awards.Count > 0)
		{
			stringBuilder.Append(" [");
			stringBuilder.Append(string.Join(", ", stats.Awards));
			stringBuilder.Append(']');
		}
		stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-kd", new(string, object)[2]
		{
			("kills", stats.Kills),
			("deaths", stats.Deaths)
		}));
		if (stats.TeamKills > 0)
		{
			stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-tk", new(string, object)[1] { ("count", stats.TeamKills) }));
		}
		if (stats.DamageDealt > 0)
		{
			stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-damage", new(string, object)[1] { ("damage", stats.DamageDealt) }));
		}
		if (isCommander && stats.CommanderPurchasesApproved > 0)
		{
			stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-commander", new(string, object)[2]
			{
				("approved", stats.CommanderPurchasesApproved),
				("points", stats.CommanderPointsSpent)
			}));
		}
		if (stats.SurvivalTime.TotalSeconds >= 1.0)
		{
			stringBuilder.Append(Loc.GetString("civ-ui-roundend-stat-survival", new(string, object)[2]
			{
				("minutes", (int)stats.SurvivalTime.TotalMinutes),
				("seconds", stats.SurvivalTime.Seconds)
			}));
		}
		return stringBuilder.ToString();
	}

	private BoxContainer BuildRow(params Control[] cards)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		foreach (Control val2 in cards)
		{
			((Control)val).AddChild(val2);
		}
		return val;
	}

	private Control BuildSummaryCard(string title, string value, string accentColor = "#B7C2D8")
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			HorizontalExpand = true,
			PanelOverride = (StyleBox)(object)BuildPanelStyle("#1B1F2CEE", "#445574")
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = title,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)accentColor, (Color?)null)
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = value,
			HorizontalExpand = true,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#F4F7FB", (Color?)null),
			StyleClasses = { "LabelHeading" }
		});
		((Control)val).AddChild((Control)(object)val2);
		return (Control)val;
	}

	private StyleBoxFlat BuildPanelStyle(string backgroundColor, string borderColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		StyleBoxFlat val = new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)backgroundColor, (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)borderColor, (Color?)null),
			BorderThickness = new Thickness(1f)
		};
		((StyleBox)val).SetContentMarginOverride((Margin)15, 8f);
		return val;
	}
}
