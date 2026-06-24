using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client._CIV14merka.Teams;
using Content.Client.UserInterface.Controls;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.GlobalMap.UI;

public sealed class CivCommanderWindow : FancyWindow
{
	[Dependency]
	private IEntityManager _entityManager;

	private readonly SpriteSystem _sprite;

	private readonly CivGlobalMapSystem _system;

	private readonly TextureRect _teamIcon;

	private readonly Label _titleLabel;

	private readonly Label _summaryLabel;

	private readonly Label _selectionLabel;

	private readonly BoxContainer _squadList;

	private readonly Label _squadHintLabel;

	private readonly Label _membersTitleLabel;

	private readonly Label _membersHintLabel;

	private readonly BoxContainer _memberList;

	private readonly Label _transferHintLabel;

	private readonly Label _selectedPlayerLabel;

	private readonly Button _moveToReserveButton;

	private readonly Button _moveToNewSquadButton;

	private readonly BoxContainer _destinationList;

	private CivCommanderState? _state;

	private int? _selectedSectionSquadId;

	private NetUserId? _selectedPlayerUserId;

	private NetUserId? _pendingFollowPlayerUserId;

	public CivCommanderWindow(CivGlobalMapSystem system)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Expected O, but got Unknown
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Expected O, but got Unknown
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Expected O, but got Unknown
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Expected O, but got Unknown
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Expected O, but got Unknown
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Expected O, but got Unknown
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Expected O, but got Unknown
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Expected O, but got Unknown
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Expected O, but got Unknown
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Expected O, but got Unknown
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Expected O, but got Unknown
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Expected O, but got Unknown
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Expected O, but got Unknown
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Expected O, but got Unknown
		IoCManager.InjectDependencies<CivCommanderWindow>(this);
		_sprite = _entityManager.System<SpriteSystem>();
		_system = system;
		base.Title = Loc.GetString("civ-gmap-hq-title");
		((Control)this).MinSize = new Vector2(1120f, 760f);
		((Control)this).SetSize = new Vector2(1220f, 820f);
		((BaseWindow)this).Resizable = true;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			HorizontalExpand = true,
			VerticalExpand = true,
			Margin = new Thickness(8f)
		};
		PanelContainer val2 = MakePanel(Color.FromHex((ReadOnlySpan<char>)"#1E232C", (Color?)null), Color.FromHex((ReadOnlySpan<char>)"#5B6D83", (Color?)null));
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 12,
			HorizontalExpand = true
		};
		_teamIcon = new TextureRect
		{
			MinSize = new Vector2(54f, 54f),
			MaxSize = new Vector2(54f, 54f),
			Stretch = (StretchMode)7,
			Visible = false
		};
		((Control)val3).AddChild(MakeIconPanel((Control)(object)_teamIcon, Color.FromHex((ReadOnlySpan<char>)"#4D87D9", (Color?)null)));
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 4,
			HorizontalExpand = true
		};
		_titleLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-hq-side-title"),
			StyleClasses = { "FancyWindowTitle" }
		};
		_summaryLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-hq-no-data"),
			FontColorOverride = Color.LightGray
		};
		_selectionLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-hq-select-hint"),
			FontColorOverride = Color.Gold
		};
		((Control)val4).AddChild((Control)(object)_titleLabel);
		((Control)val4).AddChild((Control)(object)_summaryLabel);
		((Control)val4).AddChild((Control)(object)_selectionLabel);
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 10,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		PanelContainer val6 = MakePanel(Color.FromHex((ReadOnlySpan<char>)"#252830", (Color?)null), Color.FromHex((ReadOnlySpan<char>)"#3E4653", (Color?)null));
		((Control)val6).SetWidth = 320f;
		((Control)val6).VerticalExpand = true;
		BoxContainer val7 = MakeVerticalBox(8);
		((Control)val7).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-gmap-hq-squads"),
			StyleClasses = { "FancyWindowTitle" }
		});
		_squadHintLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-hq-squads-hint"),
			FontColorOverride = Color.LightGray
		};
		((Control)val7).AddChild((Control)(object)_squadHintLabel);
		ScrollContainer val8 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			VScrollEnabled = true,
			HScrollEnabled = false
		};
		_squadList = MakeVerticalBox(8);
		((Control)val8).AddChild((Control)(object)_squadList);
		((Control)val7).AddChild((Control)(object)val8);
		((Control)val6).AddChild((Control)(object)val7);
		((Control)val5).AddChild((Control)(object)val6);
		PanelContainer val9 = MakePanel(Color.FromHex((ReadOnlySpan<char>)"#23272F", (Color?)null), Color.FromHex((ReadOnlySpan<char>)"#44505F", (Color?)null));
		((Control)val9).HorizontalExpand = true;
		((Control)val9).VerticalExpand = true;
		((Control)val9).SizeFlagsStretchRatio = 1.2f;
		BoxContainer val10 = MakeVerticalBox(8);
		_membersTitleLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-hq-members"),
			StyleClasses = { "FancyWindowTitle" }
		};
		_membersHintLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-hq-members-hint"),
			FontColorOverride = Color.LightGray
		};
		((Control)val10).AddChild((Control)(object)_membersTitleLabel);
		((Control)val10).AddChild((Control)(object)_membersHintLabel);
		ScrollContainer val11 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			VScrollEnabled = true,
			HScrollEnabled = false
		};
		_memberList = MakeVerticalBox(6);
		((Control)val11).AddChild((Control)(object)_memberList);
		((Control)val10).AddChild((Control)(object)val11);
		((Control)val9).AddChild((Control)(object)val10);
		((Control)val5).AddChild((Control)(object)val9);
		PanelContainer val12 = MakePanel(Color.FromHex((ReadOnlySpan<char>)"#252830", (Color?)null), Color.FromHex((ReadOnlySpan<char>)"#3E4653", (Color?)null));
		((Control)val12).SetWidth = 340f;
		((Control)val12).VerticalExpand = true;
		BoxContainer val13 = MakeVerticalBox(8);
		((Control)val13).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-gmap-hq-transfer"),
			StyleClasses = { "FancyWindowTitle" }
		});
		_selectedPlayerLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-hq-no-player"),
			FontColorOverride = Color.White
		};
		_transferHintLabel = new Label
		{
			Text = Loc.GetString("civ-gmap-hq-transfer-hint"),
			FontColorOverride = Color.LightGray
		};
		((Control)val13).AddChild((Control)(object)_selectedPlayerLabel);
		((Control)val13).AddChild((Control)(object)_transferHintLabel);
		GridContainer val14 = new GridContainer
		{
			Columns = 2,
			HorizontalExpand = true
		};
		_moveToReserveButton = new Button
		{
			Text = Loc.GetString("civ-gmap-hq-move-reserve"),
			HorizontalExpand = true
		};
		((BaseButton)_moveToReserveButton).OnPressed += delegate
		{
			MoveSelectedPlayerToReserve();
		};
		((Control)val14).AddChild((Control)(object)_moveToReserveButton);
		_moveToNewSquadButton = new Button
		{
			Text = Loc.GetString("civ-gmap-hq-move-new-squad"),
			HorizontalExpand = true
		};
		((BaseButton)_moveToNewSquadButton).OnPressed += delegate
		{
			MoveSelectedPlayerToNewSquad();
		};
		((Control)val14).AddChild((Control)(object)_moveToNewSquadButton);
		((Control)val13).AddChild((Control)(object)val14);
		ScrollContainer val15 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			VScrollEnabled = true,
			HScrollEnabled = false
		};
		_destinationList = MakeVerticalBox(6);
		((Control)val15).AddChild((Control)(object)_destinationList);
		((Control)val13).AddChild((Control)(object)val15);
		((Control)val12).AddChild((Control)(object)val13);
		((Control)val5).AddChild((Control)(object)val12);
		((Control)val).AddChild((Control)(object)val5);
		base.ContentsContainer.AddChild((Control)(object)val);
	}

	public void UpdateState(CivCommanderState? state, int? selectedSquadId)
	{
		_state = state;
		NormalizeSelection(selectedSquadId);
		Rebuild();
	}

	private void NormalizeSelection(int? preferredSquadId)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		if (_state == null)
		{
			_selectedSectionSquadId = null;
			_selectedPlayerUserId = null;
			_pendingFollowPlayerUserId = null;
			return;
		}
		NetUserId? pendingFollowPlayerUserId = _pendingFollowPlayerUserId;
		if (pendingFollowPlayerUserId.HasValue)
		{
			NetUserId valueOrDefault = pendingFollowPlayerUserId.GetValueOrDefault();
			if (TryFindPlayer(valueOrDefault, out CivCommanderPlayerState player))
			{
				_selectedSectionSquadId = player.SquadId;
				_selectedPlayerUserId = player.UserId;
			}
			_pendingFollowPlayerUserId = null;
		}
		int? selectedSectionSquadId = _selectedSectionSquadId;
		if (selectedSectionSquadId.HasValue)
		{
			int valueOrDefault2 = selectedSectionSquadId.GetValueOrDefault();
			if (HasSection(valueOrDefault2))
			{
				goto IL_0116;
			}
		}
		if (preferredSquadId.HasValue)
		{
			int valueOrDefault3 = preferredSquadId.GetValueOrDefault();
			if (HasSection(valueOrDefault3))
			{
				_selectedSectionSquadId = valueOrDefault3;
				goto IL_0116;
			}
		}
		if (_state.Squads.Count > 0)
		{
			_selectedSectionSquadId = _state.Squads[0].SquadId;
		}
		else
		{
			_selectedSectionSquadId = 0;
		}
		goto IL_0116;
		IL_0116:
		List<CivCommanderPlayerState> list = GetDisplayedPlayers().ToList();
		pendingFollowPlayerUserId = _selectedPlayerUserId;
		if (pendingFollowPlayerUserId.HasValue)
		{
			NetUserId selectedPlayerId = pendingFollowPlayerUserId.GetValueOrDefault();
			if (!list.All((CivCommanderPlayerState civCommanderPlayerState) => civCommanderPlayerState.UserId != selectedPlayerId))
			{
				return;
			}
		}
		_selectedPlayerUserId = ((list.Count > 0) ? new NetUserId?(list[0].UserId) : ((NetUserId?)null));
	}

	private void Rebuild()
	{
		UpdateHero();
		RebuildSquadList();
		RebuildMemberList();
		RebuildDestinationList();
	}

	private void UpdateHero()
	{
		if (_state == null)
		{
			((Control)_teamIcon).Visible = false;
			_titleLabel.Text = Loc.GetString("civ-gmap-hq-side-title");
			_summaryLabel.Text = Loc.GetString("civ-gmap-hq-no-data");
			_selectionLabel.Text = Loc.GetString("civ-gmap-hq-waiting-snapshot");
			return;
		}
		_teamIcon.Texture = _sprite.Frame0((SpriteSpecifier)(object)CivTeamIconResolver.GetTeamBadge(_state.TeamId));
		((Control)_teamIcon).Visible = true;
		string item = Loc.GetString((_state.TeamId == 2) ? "civ-team-short-rf" : "civ-team-short-usa");
		_titleLabel.Text = Loc.GetString("civ-gmap-hq-team-title", new(string, object)[1] { ("team", item) });
		_summaryLabel.Text = Loc.GetString("civ-gmap-hq-summary", new(string, object)[2]
		{
			("squads", _state.Squads.Count),
			("reserve", _state.ReservePlayers.Count)
		});
		if (TryGetDisplayedSquad(out CivCommanderSquadState squad))
		{
			_selectionLabel.Text = Loc.GetString("civ-gmap-hq-selected-squad", new(string, object)[2]
			{
				("squad", squad.SquadId),
				("order", GetOrderText(squad.Order))
			});
		}
		else
		{
			_selectionLabel.Text = Loc.GetString("civ-gmap-hq-selected-reserve", new(string, object)[1] { ("count", _state.ReservePlayers.Count) });
		}
	}

	private void RebuildSquadList()
	{
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Expected O, but got Unknown
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Expected O, but got Unknown
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Expected O, but got Unknown
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Expected O, but got Unknown
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		((Control)_squadList).DisposeAllChildren();
		if (_state == null)
		{
			((Control)_squadList).AddChild(MakeInfoCard(Loc.GetString("civ-gmap-hq-no-state")));
			return;
		}
		Label val6;
		foreach (CivCommanderSquadState squad in _state.Squads.OrderBy((CivCommanderSquadState entry) => entry.SquadId))
		{
			bool flag = _selectedSectionSquadId == squad.SquadId;
			Color border = (flag ? GetTeamAccent(_state.TeamId) : Color.FromHex((ReadOnlySpan<char>)"#37404B", (Color?)null));
			PanelContainer val = MakePanel(flag ? ((Color)(ref border)).WithAlpha(0.16f) : Color.FromHex((ReadOnlySpan<char>)"#20242C", (Color?)null), border, flag ? 2f : 1f);
			BoxContainer val2 = MakeVerticalBox(6);
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 6,
				HorizontalExpand = true
			};
			Button val4 = new Button();
			val4.Text = Loc.GetString("civ-gmap-hq-squad-button", new(string, object)[1] { ("squad", squad.SquadId) });
			((Control)val4).HorizontalExpand = true;
			((BaseButton)val4).ToggleMode = true;
			((BaseButton)val4).Pressed = flag;
			Button val5 = val4;
			((BaseButton)val5).OnPressed += delegate
			{
				SelectSection(squad.SquadId);
			};
			((Control)val3).AddChild((Control)(object)val5);
			((Control)val2).AddChild((Control)(object)val3);
			val6 = new Label();
			val6.Text = Loc.GetString("civ-gmap-hq-squad-leader", new(string, object)[1] { ("leader", squad.LeaderName) });
			val6.FontColorOverride = Color.White;
			((Control)val2).AddChild((Control)(object)val6);
			val6 = new Label();
			val6.Text = Loc.GetString("civ-gmap-hq-squad-members", new(string, object)[2]
			{
				("members", squad.Members.Count),
				("order", GetOrderText(squad.Order))
			});
			val6.FontColorOverride = Color.LightGray;
			((Control)val2).AddChild((Control)(object)val6);
			((Control)val).AddChild((Control)(object)val2);
			((Control)_squadList).AddChild((Control)(object)val);
		}
		bool flag2 = _selectedSectionSquadId == 0;
		Color border2 = (flag2 ? Color.FromHex((ReadOnlySpan<char>)"#7D8A97", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#37404B", (Color?)null));
		PanelContainer val7 = MakePanel(flag2 ? ((Color)(ref border2)).WithAlpha(0.16f) : Color.FromHex((ReadOnlySpan<char>)"#20242C", (Color?)null), border2, flag2 ? 2f : 1f);
		BoxContainer val8 = MakeVerticalBox(6);
		Button val9 = new Button
		{
			Text = Loc.GetString("civ-gmap-hq-reserve"),
			HorizontalExpand = true,
			ToggleMode = true,
			Pressed = flag2
		};
		((BaseButton)val9).OnPressed += delegate
		{
			SelectSection(0);
		};
		((Control)val8).AddChild((Control)(object)val9);
		val6 = new Label();
		val6.Text = Loc.GetString("civ-gmap-hq-reserve-members", new(string, object)[1] { ("count", _state.ReservePlayers.Count) });
		val6.FontColorOverride = Color.LightGray;
		((Control)val8).AddChild((Control)(object)val6);
		((Control)val7).AddChild((Control)(object)val8);
		((Control)_squadList).AddChild((Control)(object)val7);
	}

	private void RebuildMemberList()
	{
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Expected O, but got Unknown
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Expected O, but got Unknown
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Expected O, but got Unknown
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		((Control)_memberList).DisposeAllChildren();
		if (_state == null)
		{
			_membersTitleLabel.Text = Loc.GetString("civ-gmap-hq-members");
			_membersHintLabel.Text = Loc.GetString("civ-gmap-hq-members-no-data");
			((Control)_memberList).AddChild(MakeInfoCard(Loc.GetString("civ-gmap-hq-members-nothing")));
			return;
		}
		List<CivCommanderPlayerState> list = (from civCommanderPlayerState in GetDisplayedPlayers()
			orderby civCommanderPlayerState.IsSquadLeader descending, civCommanderPlayerState.Name
			select civCommanderPlayerState).ToList();
		int valueOrDefault = _selectedSectionSquadId.GetValueOrDefault();
		_membersTitleLabel.Text = ((valueOrDefault > 0) ? Loc.GetString("civ-gmap-hq-members-squad-title", new(string, object)[1] { ("squad", valueOrDefault) }) : Loc.GetString("civ-gmap-hq-members-reserve-title"));
		_membersHintLabel.Text = ((list.Count > 0) ? Loc.GetString("civ-gmap-hq-members-hint-select") : Loc.GetString("civ-gmap-hq-members-empty"));
		if (list.Count == 0)
		{
			((Control)_memberList).AddChild(MakeInfoCard(Loc.GetString("civ-gmap-hq-members-empty-list")));
			return;
		}
		foreach (CivCommanderPlayerState player in list)
		{
			NetUserId? selectedPlayerUserId = _selectedPlayerUserId;
			NetUserId userId = player.UserId;
			bool flag = selectedPlayerUserId.HasValue && selectedPlayerUserId.GetValueOrDefault() == userId;
			Color border = (flag ? Color.FromHex((ReadOnlySpan<char>)"#5A86C8", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#37404B", (Color?)null));
			PanelContainer val = MakePanel(flag ? ((Color)(ref border)).WithAlpha(0.16f) : Color.FromHex((ReadOnlySpan<char>)"#20242C", (Color?)null), border, flag ? 2f : 1f);
			BoxContainer val2 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 8,
				HorizontalExpand = true
			};
			Button val3 = new Button
			{
				Text = BuildPlayerButtonText(player),
				HorizontalExpand = true,
				ToggleMode = true,
				Pressed = flag
			};
			((BaseButton)val3).OnPressed += delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				_selectedPlayerUserId = player.UserId;
				Rebuild();
			};
			((Control)val2).AddChild((Control)(object)val3);
			Label val4 = new Label();
			val4.Text = ((player.SquadId == 0) ? Loc.GetString("civ-gmap-hq-member-reserve-mark") : Loc.GetString("civ-gmap-hq-member-squad-mark", new(string, object)[1] { ("squad", player.SquadId) }));
			val4.FontColorOverride = ((player.SquadId == 0) ? Color.LightGray : Color.White);
			((Control)val2).AddChild((Control)(object)val4);
			((Control)val).AddChild((Control)(object)val2);
			((Control)_memberList).AddChild((Control)(object)val);
		}
	}

	private void RebuildDestinationList()
	{
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		((Control)_destinationList).DisposeAllChildren();
		CivCommanderPlayerState player;
		bool flag = TryGetSelectedPlayer(out player);
		_selectedPlayerLabel.Text = (flag ? Loc.GetString("civ-gmap-hq-selected-player", new(string, object)[1] { ("name", player.Name) }) : Loc.GetString("civ-gmap-hq-no-player"));
		((BaseButton)_moveToReserveButton).Disabled = !flag || player.SquadId == 0;
		((BaseButton)_moveToNewSquadButton).Disabled = !flag;
		if (_state == null)
		{
			_transferHintLabel.Text = Loc.GetString("civ-gmap-hq-state-not-arrived");
			((Control)_destinationList).AddChild(MakeInfoCard(Loc.GetString("civ-gmap-hq-no-actions")));
			return;
		}
		_transferHintLabel.Text = (flag ? Loc.GetString("civ-gmap-hq-transfer-hint-instant") : Loc.GetString("civ-gmap-hq-transfer-hint-select-first"));
		if (!flag)
		{
			((Control)_destinationList).AddChild(MakeInfoCard(Loc.GetString("civ-gmap-hq-destinations-after-select")));
			return;
		}
		foreach (CivCommanderSquadState squad in _state.Squads.OrderBy((CivCommanderSquadState entry) => entry.SquadId))
		{
			bool disabled = player.SquadId == squad.SquadId;
			Button val = new Button();
			val.Text = Loc.GetString("civ-gmap-hq-destination-squad", new(string, object)[3]
			{
				("squad", squad.SquadId),
				("members", squad.Members.Count),
				("order", GetOrderText(squad.Order))
			});
			((Control)val).HorizontalExpand = true;
			((BaseButton)val).Disabled = disabled;
			Button val2 = val;
			((BaseButton)val2).OnPressed += delegate
			{
				MoveSelectedPlayerToSquad(squad.SquadId);
			};
			((Control)_destinationList).AddChild((Control)(object)val2);
		}
		if (_state.Squads.Count == 0)
		{
			((Control)_destinationList).AddChild(MakeInfoCard(Loc.GetString("civ-gmap-hq-no-squads")));
		}
	}

	private void SelectSection(int squadId)
	{
		_selectedSectionSquadId = squadId;
		_selectedPlayerUserId = null;
		if (squadId > 0)
		{
			_system.SetCommanderSelectedSquad(squadId);
		}
		NormalizeSelection(_system.GetCommanderSelectedSquadId());
		Rebuild();
	}

	private void MoveSelectedPlayerToSquad(int squadId)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSelectedPlayer(out CivCommanderPlayerState player))
		{
			_pendingFollowPlayerUserId = player.UserId;
			_selectedSectionSquadId = squadId;
			_selectedPlayerUserId = player.UserId;
			_system.SetCommanderSelectedSquad(squadId);
			_system.RequestCommanderMovePlayer(player.UserId, squadId);
			Rebuild();
		}
	}

	private void MoveSelectedPlayerToReserve()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSelectedPlayer(out CivCommanderPlayerState player))
		{
			_pendingFollowPlayerUserId = player.UserId;
			_selectedSectionSquadId = 0;
			_selectedPlayerUserId = player.UserId;
			_system.RequestCommanderMovePlayer(player.UserId, 0);
			Rebuild();
		}
	}

	private void MoveSelectedPlayerToNewSquad()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSelectedPlayer(out CivCommanderPlayerState player))
		{
			_pendingFollowPlayerUserId = player.UserId;
			_system.RequestCommanderMovePlayer(player.UserId, 0, createNewSquad: true);
		}
	}

	private bool TryGetDisplayedSquad(out CivCommanderSquadState squad)
	{
		squad = null;
		if (_state != null)
		{
			int? selectedSectionSquadId = _selectedSectionSquadId;
			if (selectedSectionSquadId.HasValue)
			{
				int squadId = selectedSectionSquadId.GetValueOrDefault();
				if (squadId > 0)
				{
					squad = _state.Squads.FirstOrDefault((CivCommanderSquadState entry) => entry.SquadId == squadId);
					return squad != null;
				}
			}
		}
		return false;
	}

	private IEnumerable<CivCommanderPlayerState> GetDisplayedPlayers()
	{
		if (_state == null)
		{
			return Enumerable.Empty<CivCommanderPlayerState>();
		}
		int? selectedSectionSquadId = _selectedSectionSquadId;
		if (selectedSectionSquadId.HasValue)
		{
			int squadId = selectedSectionSquadId.GetValueOrDefault();
			if (squadId > 0)
			{
				IEnumerable<CivCommanderPlayerState> enumerable = _state.Squads.FirstOrDefault((CivCommanderSquadState entry) => entry.SquadId == squadId)?.Members;
				return enumerable ?? Enumerable.Empty<CivCommanderPlayerState>();
			}
		}
		return _state.ReservePlayers;
	}

	private bool TryGetSelectedPlayer(out CivCommanderPlayerState player)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		player = null;
		if (_state != null)
		{
			NetUserId? selectedPlayerUserId = _selectedPlayerUserId;
			if (selectedPlayerUserId.HasValue)
			{
				NetUserId valueOrDefault = selectedPlayerUserId.GetValueOrDefault();
				return TryFindPlayer(valueOrDefault, out player);
			}
		}
		return false;
	}

	private bool TryFindPlayer(NetUserId userId, out CivCommanderPlayerState player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		player = null;
		if (_state == null)
		{
			return false;
		}
		player = _state.Squads.SelectMany((CivCommanderSquadState entry) => entry.Members).Concat(_state.ReservePlayers).FirstOrDefault((CivCommanderPlayerState candidate) => candidate.UserId == userId);
		return player != null;
	}

	private bool HasSection(int squadId)
	{
		if (_state == null)
		{
			return false;
		}
		if (squadId != 0)
		{
			return _state.Squads.Any((CivCommanderSquadState entry) => entry.SquadId == squadId);
		}
		return true;
	}

	private static PanelContainer MakePanel(Color background, Color border, float borderWidth = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0063: Expected O, but got Unknown
		return new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = background,
				BorderColor = border,
				BorderThickness = new Thickness(borderWidth),
				ContentMarginLeftOverride = 10f,
				ContentMarginTopOverride = 10f,
				ContentMarginRightOverride = 10f,
				ContentMarginBottomOverride = 10f
			}
		};
	}

	private static Control MakeIconPanel(Control icon, Color accent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		PanelContainer val = new PanelContainer
		{
			MinSize = new Vector2(72f, 72f),
			MaxSize = new Vector2(72f, 72f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = ((Color)(ref accent)).WithAlpha(0.14f),
				BorderColor = accent,
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 10f,
				ContentMarginTopOverride = 10f,
				ContentMarginRightOverride = 10f,
				ContentMarginBottomOverride = 10f
			}
		};
		((Control)val).AddChild(icon);
		return (Control)val;
	}

	private static BoxContainer MakeVerticalBox(int separation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		return new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = separation,
			HorizontalExpand = true,
			VerticalExpand = true
		};
	}

	private static Control MakeInfoCard(string text)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		PanelContainer obj = MakePanel(Color.FromHex((ReadOnlySpan<char>)"#20242C", (Color?)null), Color.FromHex((ReadOnlySpan<char>)"#37404B", (Color?)null));
		((Control)obj).AddChild((Control)new Label
		{
			Text = text,
			HorizontalExpand = true,
			FontColorOverride = Color.LightGray
		});
		return (Control)(object)obj;
	}

	private static Color GetTeamAccent(int teamId)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (teamId != 2)
		{
			return Color.FromHex((ReadOnlySpan<char>)"#4D87D9", (Color?)null);
		}
		return Color.FromHex((ReadOnlySpan<char>)"#C24E4E", (Color?)null);
	}

	private static string GetOrderText(CivCommanderOrderType order)
	{
		return order switch
		{
			CivCommanderOrderType.Attack => Loc.GetString("civ-gmap-hq-order-attack"), 
			CivCommanderOrderType.Defense => Loc.GetString("civ-gmap-hq-order-defense"), 
			CivCommanderOrderType.Artillery => Loc.GetString("civ-gmap-hq-order-artillery"), 
			_ => Loc.GetString("civ-gmap-hq-order-none"), 
		};
	}

	private static string BuildPlayerButtonText(CivCommanderPlayerState player)
	{
		if (player.IsSquadLeader)
		{
			return Loc.GetString("civ-gmap-hq-player-leader", new(string, object)[1] { ("name", player.Name) });
		}
		return player.Name;
	}
}
