using System;
using System.Linq;
using System.Numerics;
using Content.Client.Message;
using Content.Shared.GameTicking;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.RoundEnd;

public sealed class RoundEndSummaryWindow : DefaultWindow
{
	private readonly IEntityManager _entityManager;

	public int RoundId;

	public RoundEndSummaryWindow(string gm, string roundEnd, TimeSpan roundTimeSpan, int roundId, RoundEndMessageEvent.RoundEndPlayerInfo[] info, IEntityManager entityManager)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		_entityManager = entityManager;
		((Control)this).MinSize = new Vector2(520f, 580f);
		((DefaultWindow)this).Title = Loc.GetString("round-end-summary-window-title");
		RoundId = roundId;
		TabContainer val = new TabContainer();
		((Control)val).AddChild((Control)(object)MakeRoundEndSummaryTab(gm, roundEnd, roundTimeSpan, roundId));
		((Control)val).AddChild((Control)(object)MakePlayerManifestTab(info));
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
		((BaseWindow)this).OpenCenteredRight();
		((BaseWindow)this).MoveToFront();
	}

	private BoxContainer MakeRoundEndSummaryTab(string gamemode, string roundEnd, TimeSpan roundDuration, int roundId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Name = Loc.GetString("round-end-summary-window-round-end-summary-tab-title")
		};
		ScrollContainer val2 = new ScrollContainer
		{
			VerticalExpand = true,
			Margin = new Thickness(10f),
			HScrollEnabled = false
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		RichTextLabel val4 = new RichTextLabel();
		FormattedMessage val5 = new FormattedMessage();
		val5.AddMarkupOrThrow(Loc.GetString("round-end-summary-window-round-id-label", new(string, object)[1] { ("roundId", roundId) }));
		val5.AddText(" ");
		val5.AddMarkupOrThrow(Loc.GetString("round-end-summary-window-gamemode-name-label", new(string, object)[1] { ("gamemode", gamemode) }));
		val4.SetMessage(val5, (Color?)null);
		((Control)val3).AddChild((Control)(object)val4);
		RichTextLabel val6 = new RichTextLabel();
		val6.SetMarkup(Loc.GetString("round-end-summary-window-duration-label", new(string, object)[3]
		{
			("hours", roundDuration.Hours),
			("minutes", roundDuration.Minutes),
			("seconds", roundDuration.Seconds)
		}));
		((Control)val3).AddChild((Control)(object)val6);
		if (!string.IsNullOrEmpty(roundEnd))
		{
			RichTextLabel val7 = new RichTextLabel();
			val7.SetMarkup(roundEnd);
			((Control)val3).AddChild((Control)(object)val7);
		}
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}

	private BoxContainer MakePlayerManifestTab(RoundEndMessageEvent.RoundEndPlayerInfo[] playersInfo)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Name = Loc.GetString("round-end-summary-window-player-manifest-tab-title")
		};
		ScrollContainer val2 = new ScrollContainer
		{
			VerticalExpand = true,
			Margin = new Thickness(10f)
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		foreach (RoundEndMessageEvent.RoundEndPlayerInfo item2 in from p in playersInfo
			orderby p.Observer, !p.Antag
			select p)
		{
			BoxContainer val4 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			RichTextLabel val5 = new RichTextLabel
			{
				VerticalAlignment = (VAlignment)2,
				VerticalExpand = true
			};
			if (item2.PlayerNetEntity.HasValue)
			{
				((Control)val4).AddChild((Control)new SpriteView(item2.PlayerNetEntity.Value, _entityManager)
				{
					OverrideDirection = (Direction)0,
					VerticalAlignment = (VAlignment)2,
					SetSize = new Vector2(32f, 32f),
					VerticalExpand = true
				});
			}
			if (item2.PlayerICName != null)
			{
				if (item2.Observer)
				{
					val5.SetMarkup(Loc.GetString("round-end-summary-window-player-info-if-observer-text", new(string, object)[2]
					{
						("playerOOCName", item2.PlayerOOCName),
						("playerICName", item2.PlayerICName)
					}));
				}
				else
				{
					string item = (item2.Antag ? "red" : "white");
					val5.SetMarkup(Loc.GetString("round-end-summary-window-player-info-if-not-observer-text", new(string, object)[4]
					{
						("playerOOCName", item2.PlayerOOCName),
						("icNameColor", item),
						("playerICName", item2.PlayerICName),
						("playerRole", Loc.GetString(item2.Role))
					}));
				}
			}
			((Control)val4).AddChild((Control)(object)val5);
			((Control)val3).AddChild((Control)(object)val4);
		}
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}
}
