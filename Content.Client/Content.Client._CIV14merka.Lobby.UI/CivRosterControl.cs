using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client._CIV14merka.Teams;
using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Factions;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivRosterControl : BoxContainer
{
	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IPrototypeManager _prototype;

	private static readonly Color PanelBg = Color.FromHex((ReadOnlySpan<char>)"#1A1E24", (Color?)null);

	private static readonly Color PanelBgLight = Color.FromHex((ReadOnlySpan<char>)"#22272E", (Color?)null);

	private static readonly Color BorderDark = Color.FromHex((ReadOnlySpan<char>)"#2A3140", (Color?)null);

	private static readonly Color BorderLight = Color.FromHex((ReadOnlySpan<char>)"#3A4553", (Color?)null);

	private static readonly Color AccentGreen = Color.FromHex((ReadOnlySpan<char>)"#4CAF50", (Color?)null);

	private static readonly Color AccentAmber = Color.FromHex((ReadOnlySpan<char>)"#D9A441", (Color?)null);

	private static readonly Color AccentRust = Color.FromHex((ReadOnlySpan<char>)"#C24E4E", (Color?)null);

	private static readonly Color TextPrimary = Color.FromHex((ReadOnlySpan<char>)"#EAEAEA", (Color?)null);

	private static readonly Color TextSecondary = Color.FromHex((ReadOnlySpan<char>)"#A0A5B2", (Color?)null);

	private static readonly Color TextMuted = Color.FromHex((ReadOnlySpan<char>)"#6B7280", (Color?)null);

	private static readonly Color Team1Color = Color.FromHex((ReadOnlySpan<char>)"#4D87D9", (Color?)null);

	private static readonly Color Team2Color = Color.FromHex((ReadOnlySpan<char>)"#C24E4E", (Color?)null);

	private static readonly CivTdmClass[] PreferredClassOptions = new CivTdmClass[4]
	{
		CivTdmClass.Rifleman,
		CivTdmClass.MachineGunner,
		CivTdmClass.Specialist,
		CivTdmClass.Medic
	};

	private readonly SpriteSystem _sprite;

	private readonly Texture _lockTexture;

	private readonly Texture _unlockTexture;

	private readonly Texture _squadTexture;

	private readonly Dictionary<string, Texture> _factionIconCache = new Dictionary<string, Texture>();

	private readonly Label _titleLabel;

	private readonly Label _statusLabel;

	private readonly Button _enterRoundButton;

	private readonly BoxContainer _tabsRow;

	private readonly BoxContainer _body;

	private CivRosterStateEvent _state = new CivRosterStateEvent(enabled: false, roundInProgress: false, lateJoinActive: false, canEnterRound: false, null, isJoinedRound: false, hasParticipatedInCurrentRound: false, rejoinBlockedForCurrentRound: false, null, null, new List<CivRosterTeamEntry>(), new List<CivRosterPlayerEntry>());

	private int? _inspectedTeamId;

	private int? _expandedSquadId;

	private int? _renameSquadId;

	private string _renameDraft = string.Empty;

	private const int CommanderWarningPlaytimeMinutes = 600;

	public event Action<int>? TeamSelected;

	public event Action<int, int>? JoinSquadRequested;

	public event Action? LeaveSquadRequested;

	public event Action<int>? CreateSquadRequested;

	public event Action? EnterRoundRequested;

	public event Action<int, int, NetUserId>? KickRequested;

	public event Action<int, int, string>? RenameSquadRequested;

	public event Action<int>? NominateCommanderRequested;

	public event Action<int>? WithdrawCommanderRequested;

	public event Action<CivTdmClass>? ClassSelected;

	public CivRosterControl()
	{
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Expected O, but got Unknown
		IoCManager.InjectDependencies<CivRosterControl>(this);
		_sprite = _entityManager.System<SpriteSystem>();
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		((BoxContainer)this).SeparationOverride = 8;
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		_lockTexture = _resourceCache.GetResource<TextureResource>("/Textures/Interface/VerbIcons/lock.svg.192dpi.png", true).Texture;
		_unlockTexture = _resourceCache.GetResource<TextureResource>("/Textures/Interface/VerbIcons/unlock.svg.192dpi.png", true).Texture;
		_squadTexture = _sprite.Frame0((SpriteSpecifier)(object)CivTeamIconResolver.GetGenericSquadBadge());
		PanelContainer val = MakePanel(PanelBgLight, BorderLight);
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 12,
			HorizontalExpand = true
		};
		BoxContainer val3 = MakeVerticalBox(2);
		((Control)val3).HorizontalExpand = true;
		_titleLabel = new Label
		{
			Text = Loc.GetString("civ-lobby-roster-title"),
			StyleClasses = { "LabelHeadingBigger" },
			FontColorOverride = TextPrimary
		};
		_statusLabel = new Label
		{
			FontColorOverride = TextSecondary
		};
		((Control)val3).AddChild((Control)(object)_titleLabel);
		((Control)val3).AddChild((Control)(object)_statusLabel);
		((Control)val2).AddChild((Control)(object)val3);
		_enterRoundButton = new Button
		{
			Text = Loc.GetString("civ-lobby-roster-enter-round"),
			MinWidth = 180f,
			MinHeight = 40f,
			Visible = false,
			StyleClasses = { "ButtonColorGreen" },
			StyleClasses = { "ButtonBig" }
		};
		((BaseButton)_enterRoundButton).OnPressed += delegate
		{
			this.EnterRoundRequested?.Invoke();
		};
		((Control)val2).AddChild((Control)(object)_enterRoundButton);
		((Control)val).AddChild((Control)(object)val2);
		((Control)this).AddChild((Control)(object)val);
		_tabsRow = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8,
			HorizontalExpand = true
		};
		((Control)this).AddChild((Control)(object)_tabsRow);
		ScrollContainer val4 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			VScrollEnabled = true,
			HScrollEnabled = false
		};
		_body = MakeVerticalBox(8);
		((Control)val4).AddChild((Control)(object)_body);
		((Control)this).AddChild((Control)(object)val4);
	}

	public void UpdateState(CivRosterStateEvent state)
	{
		_state = state;
		NormalizeSelection();
		Rebuild();
	}

	private void NormalizeSelection()
	{
		if (!_state.Enabled)
		{
			_inspectedTeamId = null;
			_expandedSquadId = null;
			return;
		}
		CivRosterTeamEntry civRosterTeamEntry = _state.Teams.FirstOrDefault((CivRosterTeamEntry t) => t.TeamId == _inspectedTeamId) ?? _state.Teams.FirstOrDefault((CivRosterTeamEntry t) => t.TeamId == _state.SelectedTeamId) ?? _state.Teams.OrderBy((CivRosterTeamEntry t) => t.TeamId).FirstOrDefault();
		_inspectedTeamId = civRosterTeamEntry?.TeamId;
		if (civRosterTeamEntry == null)
		{
			_expandedSquadId = null;
		}
		else if (!civRosterTeamEntry.Squads.Any((CivRosterSquadEntry s) => s.SquadId == _expandedSquadId))
		{
			_expandedSquadId = civRosterTeamEntry.SelectedSquadIfPresent(_state.SelectedSquadId)?.SquadId;
		}
	}

	private void Rebuild()
	{
		RebuildHeader();
		RebuildTabs();
		RebuildBody();
	}

	private void RebuildHeader()
	{
		if (!_state.Enabled)
		{
			_titleLabel.Text = Loc.GetString("civ-lobby-roster-title");
			_statusLabel.Text = Loc.GetString("civ-lobby-roster-status-pick-mode");
			((Control)_enterRoundButton).Visible = false;
		}
		else if (_state.LateJoinActive)
		{
			_titleLabel.Text = Loc.GetString("civ-lobby-roster-latejoin-title");
			_statusLabel.Text = (_state.CanEnterRound ? Loc.GetString("civ-lobby-roster-latejoin-pick") : (_state.EnterRoundUnavailableReason ?? Loc.GetString("civ-lobby-roster-enter-unavailable")));
			((Control)_enterRoundButton).Visible = !_state.IsJoinedRound;
			((BaseButton)_enterRoundButton).Disabled = !_state.CanEnterRound;
		}
		else
		{
			_titleLabel.Text = Loc.GetString("civ-lobby-roster-title");
			_statusLabel.Text = BuildStatusText();
			((Control)_enterRoundButton).Visible = false;
		}
	}

	private string BuildStatusText()
	{
		CivRosterPlayerEntry self = _state.Players.FirstOrDefault((CivRosterPlayerEntry p) => p.IsSelected);
		if (self == null)
		{
			return Loc.GetString("civ-lobby-roster-status-pick-before-start");
		}
		CivRosterTeamEntry civRosterTeamEntry = _state.Teams.FirstOrDefault((CivRosterTeamEntry t) => t.TeamId == self.TeamId);
		if (civRosterTeamEntry == null || !self.SquadId.HasValue)
		{
			if (self.State == CivRosterPlayerState.Ready)
			{
				return Loc.GetString("civ-lobby-roster-status-ready-auto");
			}
			if (civRosterTeamEntry == null)
			{
				return Loc.GetString("civ-lobby-roster-status-pick");
			}
			return Loc.GetString("civ-lobby-roster-status-class-pref", new(string, object)[1] { ("class", CivTdmClassHelper.GetDisplayName(self.Class)) });
		}
		string displayName = CivTdmClassHelper.GetDisplayName(self.Class);
		return Loc.GetString("civ-lobby-roster-status-assigned", new(string, object)[3]
		{
			("team", civRosterTeamEntry.TeamName),
			("squad", self.SquadId),
			("class", displayName)
		});
	}

	private void RebuildTabs()
	{
		((Control)_tabsRow).RemoveAllChildren();
		if (!_state.Enabled || _state.Teams.Count == 0)
		{
			return;
		}
		foreach (CivRosterTeamEntry item in _state.Teams.OrderBy((CivRosterTeamEntry t) => t.TeamId))
		{
			((Control)_tabsRow).AddChild(BuildTeamTab(item));
		}
	}

	private Control BuildTeamTab(CivRosterTeamEntry team)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Expected O, but got Unknown
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Expected O, but got Unknown
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Expected O, but got Unknown
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Expected O, but got Unknown
		Color teamAccent = GetTeamAccent(team.TeamId);
		bool flag = team.TeamId == _inspectedTeamId;
		bool isSelected = team.IsSelected;
		Color background = (flag ? ((Color)(ref teamAccent)).WithAlpha(0.18f) : PanelBg);
		Color border = (flag ? teamAccent : (isSelected ? ((Color)(ref AccentGreen)).WithAlpha(0.6f) : BorderDark));
		PanelContainer val = MakePanel(background, border, flag ? 2f : 1f);
		((Control)val).HorizontalExpand = true;
		((Control)val).VerticalExpand = true;
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		((Control)val3).AddChild((Control)new TextureRect
		{
			Texture = GetTeamTexture(team, team.TeamId),
			MinSize = new Vector2(44f, 44f),
			MaxSize = new Vector2(44f, 44f),
			Stretch = (StretchMode)7,
			VerticalAlignment = (VAlignment)2
		});
		BoxContainer val4 = MakeVerticalBox(2);
		((Control)val4).HorizontalExpand = true;
		((Control)val4).AddChild((Control)new Label
		{
			Text = team.TeamName.ToUpperInvariant(),
			StyleClasses = { "LabelHeadingBigger" },
			FontColorOverride = teamAccent
		});
		string text = Loc.GetString("civ-lobby-roster-team-subtitle", new(string, object)[2]
		{
			("players", team.PlayerCount),
			("squads", team.Squads.Count)
		});
		if (isSelected)
		{
			text = Loc.GetString("civ-lobby-roster-team-yours", new(string, object)[1] { ("subtitle", text) });
		}
		((Control)val4).AddChild((Control)new Label
		{
			Text = text,
			FontColorOverride = (isSelected ? AccentGreen : TextSecondary)
		});
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val2).AddChild((Control)(object)val3);
		var (text2, value) = BuildTeamCardCommanderLine(team);
		((Control)val2).AddChild((Control)new Label
		{
			Text = text2,
			FontColorOverride = value
		});
		if (!isSelected && team.CanSelect)
		{
			Button val5 = new Button
			{
				Text = Loc.GetString("civ-lobby-roster-select-side"),
				StyleClasses = { "ButtonColorGreen" },
				HorizontalExpand = true,
				MinHeight = 30f
			};
			((BaseButton)val5).OnPressed += delegate
			{
				_inspectedTeamId = team.TeamId;
				_expandedSquadId = null;
				this.TeamSelected?.Invoke(team.TeamId);
			};
			((Control)val2).AddChild((Control)(object)val5);
		}
		((Control)val).AddChild((Control)(object)val2);
		ContainerButton val6 = new ContainerButton
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val6).AddChild((Control)(object)val);
		((BaseButton)val6).OnPressed += delegate
		{
			_inspectedTeamId = team.TeamId;
			Rebuild();
		};
		return (Control)val6;
	}

	private (string Text, Color Color) BuildTeamCardCommanderLine(CivRosterTeamEntry team)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrWhiteSpace(team.CommanderName))
		{
			return (Text: Loc.GetString("civ-lobby-roster-team-commander", new(string, object)[1] { ("name", team.CommanderName) }), Color: AccentGreen);
		}
		if (team.CommanderCandidates.Count > 0)
		{
			List<CivCommanderCandidateEntry> commanderCandidates = team.CommanderCandidates;
			int totalBase = commanderCandidates.Sum((CivCommanderCandidateEntry c) => Math.Max(c.PlaytimeMinutes, 1));
			Dictionary<NetUserId, double> weights = commanderCandidates.ToDictionary((CivCommanderCandidateEntry c) => c.UserId, (CivCommanderCandidateEntry c) => (double)Math.Max(c.PlaytimeMinutes, 1) + (double)totalBase * (double)c.Priority * 0.1);
			double totalWeight = weights.Values.Sum();
			List<CivCommanderCandidateEntry> list = commanderCandidates.OrderByDescending((CivCommanderCandidateEntry c) => weights[c.UserId]).ToList();
			IEnumerable<CivCommanderCandidateEntry> source = list.Take(4);
			int num = list.Count - 4;
			List<string> list2 = source.Select(delegate(CivCommanderCandidateEntry c)
			{
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				double value = ((totalWeight > 0.0) ? (weights[c.UserId] / totalWeight * 100.0) : 0.0);
				string value2 = ((c.Priority > 0) ? $" [★{c.Priority}]" : "");
				string value3 = (c.IsSelf ? " (вы)" : "");
				return $"{c.Name}{value2}{value3} {value:F0}%";
			}).ToList();
			if (num > 0)
			{
				list2.Add($"+{num}");
			}
			return (Text: string.Join(", ", list2), Color: AccentAmber);
		}
		return (Text: Loc.GetString("civ-lobby-roster-team-no-commander"), Color: TextMuted);
	}

	private void RebuildBody()
	{
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		((Control)_body).RemoveAllChildren();
		if (!_state.Enabled)
		{
			((Control)_body).AddChild(MakeInfoCard(Loc.GetString("civ-lobby-roster-status-pick-mode")));
			return;
		}
		CivRosterTeamEntry civRosterTeamEntry = _state.Teams.FirstOrDefault((CivRosterTeamEntry t) => t.TeamId == _inspectedTeamId);
		if (civRosterTeamEntry == null)
		{
			((Control)_body).AddChild(MakeInfoCard(Loc.GetString("civ-lobby-roster-teams-not-loaded")));
			return;
		}
		if (_state.RejoinBlockedForCurrentRound)
		{
			((Control)_body).AddChild(MakeInfoCard(Loc.GetString("civ-lobby-roster-rejoin-blocked")));
			return;
		}
		((Control)_body).AddChild(BuildCommanderRow(civRosterTeamEntry));
		CivRosterPlayerEntry civRosterPlayerEntry = _state.Players.FirstOrDefault((CivRosterPlayerEntry p) => p.IsSelected);
		if (!_state.IsJoinedRound && civRosterPlayerEntry != null && !civRosterPlayerEntry.SquadId.HasValue && civRosterPlayerEntry.TeamId == civRosterTeamEntry.TeamId)
		{
			NetUserId? commanderUserId = civRosterTeamEntry.CommanderUserId;
			NetUserId userId = civRosterPlayerEntry.UserId;
			if (!commanderUserId.HasValue || commanderUserId.GetValueOrDefault() != userId)
			{
				((Control)_body).AddChild(BuildPreferredClassCard(civRosterPlayerEntry));
			}
		}
		((Control)_body).AddChild(BuildCreateSquadRow(civRosterTeamEntry));
		if (civRosterTeamEntry.Squads.Count == 0)
		{
			((Control)_body).AddChild(MakeInfoCard(Loc.GetString("civ-lobby-roster-no-squads")));
			return;
		}
		foreach (CivRosterSquadEntry item in civRosterTeamEntry.Squads.OrderBy((CivRosterSquadEntry s) => s.SquadId))
		{
			((Control)_body).AddChild(BuildSquadAccordion(civRosterTeamEntry, item));
		}
	}

	private Control BuildCommanderRow(CivRosterTeamEntry team)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Expected O, but got Unknown
		PanelContainer obj = MakePanel(PanelBgLight, BorderLight);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		BoxContainer val2 = MakeVerticalBox(2);
		((Control)val2).HorizontalExpand = true;
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-lobby-roster-commander-header"),
			FontColorOverride = TextMuted
		});
		Control val3 = BuildCommanderLine(team);
		((Control)val2).AddChild(val3);
		((Control)val).AddChild((Control)(object)val2);
		if (!_state.LateJoinActive && team.IsSelected && string.IsNullOrEmpty(team.CommanderName))
		{
			if (team.CommanderCandidates.FirstOrDefault((CivCommanderCandidateEntry c) => c.IsSelf) != null)
			{
				Button val4 = new Button
				{
					Text = Loc.GetString("civ-lobby-roster-withdraw"),
					ToolTip = Loc.GetString("civ-lobby-roster-withdraw-tooltip")
				};
				((BaseButton)val4).OnPressed += delegate
				{
					this.WithdrawCommanderRequested?.Invoke(team.TeamId);
				};
				((Control)val).AddChild((Control)(object)val4);
			}
			else if (team.CanNominate)
			{
				Button val5 = new Button
				{
					Text = Loc.GetString("civ-lobby-roster-nominate"),
					StyleClasses = { "ButtonColorGreen" }
				};
				((BaseButton)val5).OnPressed += delegate
				{
					OnNominatePressed(team.TeamId);
				};
				((Control)val).AddChild((Control)(object)val5);
			}
			else if (!string.IsNullOrWhiteSpace(team.NominateUnavailableReason))
			{
				((Control)val).AddChild((Control)new Label
				{
					Text = team.NominateUnavailableReason,
					FontColorOverride = TextMuted,
					VerticalAlignment = (VAlignment)2
				});
			}
		}
		((Control)obj).AddChild((Control)(object)val);
		return (Control)(object)obj;
	}

	private void OnNominatePressed(int teamId)
	{
		if (_state.SelfPlaytimeMinutes >= 600)
		{
			this.NominateCommanderRequested?.Invoke(teamId);
			return;
		}
		CivCommanderWarningWindow civCommanderWarningWindow = new CivCommanderWarningWindow();
		civCommanderWarningWindow.ConfirmPressed += delegate
		{
			this.NominateCommanderRequested?.Invoke(teamId);
		};
		((BaseWindow)civCommanderWarningWindow).OpenCentered();
	}

	private Control BuildCommanderLine(CivRosterTeamEntry team)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Expected O, but got Unknown
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Expected O, but got Unknown
		if (!string.IsNullOrEmpty(team.CommanderName))
		{
			return (Control)new Label
			{
				Text = team.CommanderName,
				FontColorOverride = AccentGreen
			};
		}
		if (team.CommanderCandidates.Count == 0)
		{
			return (Control)new Label
			{
				Text = Loc.GetString("civ-lobby-roster-commander-none"),
				FontColorOverride = TextMuted
			};
		}
		List<CivCommanderCandidateEntry> commanderCandidates = team.CommanderCandidates;
		int totalBase = commanderCandidates.Sum((CivCommanderCandidateEntry c) => Math.Max(c.PlaytimeMinutes, 1));
		Dictionary<NetUserId, double> weights = commanderCandidates.ToDictionary((CivCommanderCandidateEntry c) => c.UserId, (CivCommanderCandidateEntry c) => (double)Math.Max(c.PlaytimeMinutes, 1) + (double)totalBase * (double)c.Priority * 0.1);
		double num = weights.Values.Sum();
		List<CivCommanderCandidateEntry> list = commanderCandidates.OrderByDescending((CivCommanderCandidateEntry x) => weights[x.UserId]).ToList();
		BoxContainer val = MakeVerticalBox(1);
		foreach (CivCommanderCandidateEntry item in list.Take(4))
		{
			double value = ((num > 0.0) ? (weights[item.UserId] / num * 100.0) : 0.0);
			string value2 = ((item.Priority > 0) ? $" [★{item.Priority}]" : "");
			string value3 = (item.IsSelf ? " (вы)" : "");
			float value4 = (float)item.PlaytimeMinutes / 60f;
			((Control)val).AddChild((Control)new Label
			{
				Text = $"{item.Name}{value2}{value3}  {value:F0}% ({value4:F0}ч)",
				FontColorOverride = AccentAmber
			});
		}
		int num2 = list.Count - 4;
		if (num2 > 0)
		{
			((Control)val).AddChild((Control)new Label
			{
				Text = $"+{num2}",
				FontColorOverride = TextMuted
			});
		}
		return (Control)(object)val;
	}

	private Control BuildCreateSquadRow(CivRosterTeamEntry team)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		Button val = new Button
		{
			Text = (_state.LateJoinActive ? Loc.GetString("civ-lobby-roster-join-free-squad") : Loc.GetString("civ-lobby-roster-create-squad")),
			HorizontalExpand = true,
			MinHeight = 36f,
			Disabled = !team.CanCreateSquad
		};
		if (!team.CanCreateSquad && !string.IsNullOrWhiteSpace(team.CreateSquadUnavailableReason))
		{
			((Control)val).ToolTip = team.CreateSquadUnavailableReason;
		}
		((BaseButton)val).OnPressed += delegate
		{
			this.CreateSquadRequested?.Invoke(team.TeamId);
		};
		return (Control)(object)val;
	}

	private Control BuildPreferredClassCard(CivRosterPlayerEntry self)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		PanelContainer val = MakePanel(PanelBgLight, BorderLight);
		BoxContainer val2 = MakeVerticalBox(8);
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-lobby-roster-preferred-class-header"),
			FontColorOverride = TextMuted
		});
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		CivTdmClass[] preferredClassOptions = PreferredClassOptions;
		foreach (CivTdmClass cls in preferredClassOptions)
		{
			((Control)val3).AddChild(BuildPreferredClassChip(cls, self.Class));
		}
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-lobby-roster-preferred-class-hint"),
			FontColorOverride = TextMuted
		});
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private Control BuildPreferredClassChip(CivTdmClass cls, CivTdmClass currentClass)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00bf: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		bool flag = cls == currentClass;
		Color backgroundColor = (flag ? ((Color)(ref AccentGreen)).WithAlpha(0.2f) : PanelBg);
		Color borderColor = (flag ? AccentGreen : BorderDark);
		Color value = (flag ? AccentGreen : TextPrimary);
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = backgroundColor,
				BorderColor = borderColor,
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 10f,
				ContentMarginRightOverride = 10f,
				ContentMarginTopOverride = 6f,
				ContentMarginBottomOverride = 6f
			}
		};
		((Control)val).AddChild((Control)new Label
		{
			Text = CivTdmClassHelper.GetDisplayName(cls),
			FontColorOverride = value
		});
		ContainerButton val2 = new ContainerButton
		{
			HorizontalExpand = false,
			Disabled = flag
		};
		((Control)val2).AddChild((Control)(object)val);
		((BaseButton)val2).OnPressed += delegate
		{
			this.ClassSelected?.Invoke(cls);
		};
		return (Control)val2;
	}

	private Control BuildSquadAccordion(CivRosterTeamEntry team, CivRosterSquadEntry squad)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Expected O, but got Unknown
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Expected O, but got Unknown
		Color teamAccent = GetTeamAccent(team.TeamId);
		bool isExpanded = squad.SquadId == _expandedSquadId;
		bool isMember = squad.IsMember;
		Color background = (isExpanded ? PanelBgLight : PanelBg);
		Color border = (isExpanded ? teamAccent : (isMember ? ((Color)(ref AccentGreen)).WithAlpha(0.6f) : BorderDark));
		PanelContainer val = MakePanel(background, border, isExpanded ? 2f : 1f);
		BoxContainer val2 = MakeVerticalBox(6);
		ContainerButton val3 = new ContainerButton
		{
			HorizontalExpand = true
		};
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		((Control)val4).AddChild((Control)new Label
		{
			Text = (isExpanded ? "[-]" : "[+]"),
			FontColorOverride = TextSecondary,
			VerticalAlignment = (VAlignment)2,
			MinWidth = 24f
		});
		((Control)val4).AddChild((Control)new TextureRect
		{
			Texture = (squad.IsOpen ? _unlockTexture : _lockTexture),
			MinSize = new Vector2(18f, 18f),
			MaxSize = new Vector2(18f, 18f),
			Stretch = (StretchMode)7,
			ModulateSelfOverride = (squad.IsOpen ? AccentGreen : TextMuted),
			VerticalAlignment = (VAlignment)2
		});
		BoxContainer val5 = MakeVerticalBox(2);
		((Control)val5).HorizontalExpand = true;
		Label val6 = new Label
		{
			Text = GetSquadTitle(squad),
			FontColorOverride = (isMember ? AccentGreen : TextPrimary)
		};
		((Control)val5).AddChild((Control)(object)val6);
		Label val7 = new Label();
		val7.Text = Loc.GetString("civ-lobby-roster-squad-meta", new(string, object)[4]
		{
			("leader", squad.LeaderName),
			("count", squad.MemberCount),
			("max", squad.MaxMembers),
			("recruit", Loc.GetString(squad.IsOpen ? "civ-lobby-roster-squad-recruit-open" : "civ-lobby-roster-squad-recruit-closed"))
		});
		val7.FontColorOverride = TextSecondary;
		((Control)val5).AddChild((Control)(object)val7);
		((Control)val4).AddChild((Control)(object)val5);
		if (isMember)
		{
			((Control)val4).AddChild((Control)new Label
			{
				Text = Loc.GetString("civ-lobby-roster-you-here"),
				FontColorOverride = AccentGreen,
				VerticalAlignment = (VAlignment)2
			});
		}
		((Control)val3).AddChild((Control)(object)val4);
		((BaseButton)val3).OnPressed += delegate
		{
			_expandedSquadId = (isExpanded ? ((int?)null) : new int?(squad.SquadId));
			Rebuild();
		};
		((Control)val2).AddChild((Control)(object)val3);
		if (isExpanded)
		{
			((Control)val2).AddChild(BuildSquadDetails(team, squad));
		}
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private Control BuildSquadDetails(CivRosterTeamEntry team, CivRosterSquadEntry squad)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Expected O, but got Unknown
		BoxContainer val = MakeVerticalBox(8);
		((Control)val).AddChild((Control)new Label
		{
			Text = Loc.GetString("civ-lobby-roster-members-header"),
			FontColorOverride = TextMuted
		});
		((Control)val).AddChild(BuildMembersList(squad));
		int teamId;
		int squadId;
		LineEdit nameEdit;
		if (squad.CanRename)
		{
			BoxContainer val2 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 6,
				HorizontalExpand = true
			};
			teamId = squad.TeamId;
			squadId = squad.SquadId;
			bool flag = _renameSquadId == squadId;
			nameEdit = new LineEdit
			{
				HorizontalExpand = true,
				PlaceHolder = Loc.GetString("civ-lobby-roster-squad-rename-placeholder"),
				Text = (flag ? _renameDraft : (squad.SquadName ?? string.Empty))
			};
			Button val3 = new Button
			{
				Text = Loc.GetString("civ-lobby-roster-squad-rename-button")
			};
			nameEdit.OnTextChanged += delegate(LineEditEventArgs args)
			{
				_renameSquadId = squadId;
				_renameDraft = args.Text;
			};
			((BaseButton)val3).OnPressed += delegate
			{
				DoRename();
			};
			nameEdit.OnTextEntered += delegate
			{
				DoRename();
			};
			((Control)val2).AddChild((Control)(object)nameEdit);
			((Control)val2).AddChild((Control)(object)val3);
			((Control)val).AddChild((Control)(object)val2);
			if (flag)
			{
				((Control)nameEdit).GrabKeyboardFocus();
				nameEdit.CursorPosition = nameEdit.Text.Length;
			}
		}
		if (squad.RoleTickets.Count > 0)
		{
			CivRosterPlayerEntry civRosterPlayerEntry = squad.Members.FirstOrDefault((CivRosterPlayerEntry m) => m.IsSelected);
			bool canPick = squad.IsMember && civRosterPlayerEntry != null && !civRosterPlayerEntry.IsLeader;
			((Control)val).AddChild((Control)new Label
			{
				Text = Loc.GetString("civ-lobby-roster-class-header"),
				FontColorOverride = TextMuted
			});
			((Control)val).AddChild(BuildClassChips(squad, civRosterPlayerEntry, canPick));
		}
		((Control)val).AddChild(BuildSquadActions(team, squad));
		return (Control)(object)val;
		void DoRename()
		{
			this.RenameSquadRequested?.Invoke(teamId, squadId, nameEdit.Text);
			_renameSquadId = null;
			_renameDraft = string.Empty;
		}
	}

	private Control BuildMembersList(CivRosterSquadEntry squad)
	{
		BoxContainer val = MakeVerticalBox(4);
		List<CivRosterPlayerEntry> list = OrderMembers(squad).ToList();
		if (list.Count == 0)
		{
			((Control)val).AddChild(MakeInfoCard(Loc.GetString("civ-lobby-roster-squad-empty")));
			return (Control)(object)val;
		}
		foreach (CivRosterPlayerEntry item in list)
		{
			((Control)val).AddChild(BuildMemberRow(squad, item));
		}
		return (Control)(object)val;
	}

	private Control BuildMemberRow(CivRosterSquadEntry squad, CivRosterPlayerEntry member)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Expected O, but got Unknown
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Expected O, but got Unknown
		PanelContainer val = MakePanel(PanelBg, BorderDark);
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 10,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new TextureRect
		{
			Texture = (member.IsLeader ? GetTeamTexture(squad.TeamId) : _squadTexture),
			MinSize = new Vector2(20f, 20f),
			MaxSize = new Vector2(20f, 20f),
			Stretch = (StretchMode)7,
			VerticalAlignment = (VAlignment)2
		});
		BoxContainer val3 = MakeVerticalBox(2);
		((Control)val3).HorizontalExpand = true;
		string text = (member.IsLeader ? Loc.GetString("civ-lobby-roster-member-leader") : CivTdmClassHelper.GetDisplayName(member.Class));
		string item = (member.IsSelected ? Loc.GetString("civ-lobby-roster-member-self") : "");
		Label val4 = new Label();
		val4.Text = Loc.GetString("civ-lobby-roster-member-name", new(string, object)[2]
		{
			("name", member.Name),
			("self", item)
		});
		val4.FontColorOverride = (member.IsSelected ? AccentGreen : TextPrimary);
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)new Label
		{
			Text = text,
			FontColorOverride = TextSecondary
		});
		((Control)val2).AddChild((Control)(object)val3);
		string text2 = member.State switch
		{
			CivRosterPlayerState.Ready => Loc.GetString("civ-lobby-roster-state-ready"), 
			CivRosterPlayerState.Joined => Loc.GetString("civ-lobby-roster-state-joined"), 
			CivRosterPlayerState.Disconnected => Loc.GetString("civ-lobby-roster-state-disconnected"), 
			_ => Loc.GetString("civ-lobby-roster-state-lobby"), 
		};
		Color value = (Color)(member.State switch
		{
			CivRosterPlayerState.Ready => AccentGreen, 
			CivRosterPlayerState.Joined => AccentAmber, 
			CivRosterPlayerState.Disconnected => TextMuted, 
			_ => TextSecondary, 
		});
		((Control)val2).AddChild((Control)new Label
		{
			Text = text2,
			FontColorOverride = value,
			VerticalAlignment = (VAlignment)2
		});
		if (squad.CanManage && !member.IsSelected && member.State != CivRosterPlayerState.Joined)
		{
			Button val5 = new Button
			{
				Text = Loc.GetString("civ-lobby-roster-kick"),
				MinWidth = 50f
			};
			((BaseButton)val5).OnPressed += delegate
			{
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				this.KickRequested?.Invoke(squad.TeamId, squad.SquadId, member.UserId);
			};
			((Control)val2).AddChild((Control)(object)val5);
		}
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private Control BuildClassChips(CivRosterSquadEntry squad, CivRosterPlayerEntry? self, bool canPick)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		foreach (var (cls, available, total) in (from kvp in squad.RoleTickets
			where !IsLeaderOnlyClass(kvp.Key)
			orderby (int)kvp.Key
			select (Key: kvp.Key, Available: kvp.Value.Available, Total: kvp.Value.Total)).ToList())
		{
			((Control)val).AddChild(BuildClassChip(cls, available, total, self?.Class, canPick));
		}
		return (Control)(object)val;
	}

	private Control BuildClassChip(CivTdmClass cls, int available, int total, CivTdmClass? currentClass, bool canPick)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0105: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		bool flag = currentClass == cls;
		bool flag2 = total > 0 && available <= 0 && !flag;
		Color backgroundColor = (flag ? ((Color)(ref AccentGreen)).WithAlpha(0.2f) : PanelBg);
		Color borderColor = (flag ? AccentGreen : (flag2 ? ((Color)(ref AccentRust)).WithAlpha(0.5f) : BorderDark));
		Color value = (flag ? AccentGreen : (flag2 ? TextMuted : TextPrimary));
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = backgroundColor,
				BorderColor = borderColor,
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 10f,
				ContentMarginRightOverride = 10f,
				ContentMarginTopOverride = 6f,
				ContentMarginBottomOverride = 6f
			}
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 6,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = CivTdmClassHelper.GetDisplayName(cls),
			FontColorOverride = value
		});
		if (total > 0)
		{
			string text = ((total >= 999) ? "∞" : $"{Math.Max(0, total - available)}/{total}");
			((Control)val2).AddChild((Control)new Label
			{
				Text = text,
				FontColorOverride = (flag2 ? AccentRust : TextSecondary)
			});
		}
		((Control)val).AddChild((Control)(object)val2);
		ContainerButton val3 = new ContainerButton
		{
			HorizontalExpand = false,
			Disabled = (!canPick || flag || flag2)
		};
		((Control)val3).AddChild((Control)(object)val);
		if (canPick)
		{
			((BaseButton)val3).OnPressed += delegate
			{
				this.ClassSelected?.Invoke(cls);
			};
		}
		return (Control)(object)val3;
	}

	private Control BuildSquadActions(CivRosterTeamEntry team, CivRosterSquadEntry squad)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8,
			HorizontalExpand = true
		};
		if (!squad.IsMember && squad.CanJoin)
		{
			Button val2 = new Button
			{
				Text = (_state.LateJoinActive ? Loc.GetString("civ-lobby-roster-join-and-enter") : Loc.GetString("civ-lobby-roster-join")),
				HorizontalExpand = true,
				StyleClasses = { "ButtonColorGreen" }
			};
			((BaseButton)val2).OnPressed += delegate
			{
				this.JoinSquadRequested?.Invoke(squad.TeamId, squad.SquadId);
			};
			((Control)val).AddChild((Control)(object)val2);
		}
		else if (!squad.IsMember && !string.IsNullOrWhiteSpace(squad.JoinUnavailableReason))
		{
			((Control)val).AddChild((Control)new Label
			{
				Text = squad.JoinUnavailableReason,
				FontColorOverride = AccentRust,
				HorizontalExpand = true
			});
		}
		if (squad.IsMember && squad.CanLeave)
		{
			Button val3 = new Button
			{
				Text = Loc.GetString("civ-lobby-roster-leave"),
				HorizontalExpand = true
			};
			((BaseButton)val3).OnPressed += delegate
			{
				this.LeaveSquadRequested?.Invoke();
			};
			((Control)val).AddChild((Control)(object)val3);
		}
		else if (squad.IsMember && !string.IsNullOrWhiteSpace(squad.LeaveUnavailableReason))
		{
			((Control)val).AddChild((Control)new Label
			{
				Text = squad.LeaveUnavailableReason,
				FontColorOverride = TextMuted,
				HorizontalExpand = true
			});
		}
		if (((Control)val).ChildCount == 0)
		{
			((Control)val).AddChild((Control)new Label
			{
				Text = Loc.GetString("civ-lobby-roster-no-actions"),
				FontColorOverride = TextMuted,
				HorizontalExpand = true
			});
		}
		return (Control)(object)val;
	}

	private IEnumerable<CivRosterPlayerEntry> OrderMembers(CivRosterSquadEntry squad)
	{
		NetUserId? leaderId = squad.LeaderId;
		CivRosterPlayerEntry civRosterPlayerEntry = ((!leaderId.HasValue) ? null : (squad.Members.FirstOrDefault((CivRosterPlayerEntry m) => m.UserId == leaderId.Value) ?? _state.Players.FirstOrDefault((CivRosterPlayerEntry p) => p.UserId == leaderId.Value)));
		if (civRosterPlayerEntry != null)
		{
			yield return civRosterPlayerEntry;
		}
		foreach (CivRosterPlayerEntry member in squad.Members)
		{
			if (!leaderId.HasValue || !(member.UserId == leaderId.Value))
			{
				yield return member;
			}
		}
	}

	private static bool IsLeaderOnlyClass(CivTdmClass cls)
	{
		if (cls == CivTdmClass.SquadLeader || cls - 6 <= CivTdmClass.MachineGunner)
		{
			return true;
		}
		return false;
	}

	private static string GetSquadTitle(CivRosterSquadEntry squad)
	{
		switch (squad.Type)
		{
		case CivSquadType.Engineer:
			return Loc.GetString("civ-lobby-roster-squad-type-engineer");
		case CivSquadType.Medic:
			return Loc.GetString("civ-lobby-roster-squad-type-medic");
		case CivSquadType.Support:
			return Loc.GetString("civ-lobby-roster-squad-type-support");
		default:
			if (!string.IsNullOrWhiteSpace(squad.SquadName))
			{
				return squad.SquadName;
			}
			return Loc.GetString("civ-lobby-roster-squad-title", new(string, object)[1] { ("id", squad.SquadId) });
		}
	}

	private Texture GetTeamTexture(int teamId)
	{
		CivRosterTeamEntry team = _state.Teams.FirstOrDefault((CivRosterTeamEntry t) => t.TeamId == teamId);
		return GetTeamTexture(team, teamId);
	}

	private Texture GetTeamTexture(CivRosterTeamEntry? team, int fallbackTeamId)
	{
		string text = team?.SideId ?? string.Empty;
		if (!string.IsNullOrWhiteSpace(text) && _factionIconCache.TryGetValue(text, out Texture value))
		{
			return value;
		}
		SpriteSpecifier val = null;
		CivFactionPrototype civFactionPrototype = default(CivFactionPrototype);
		if (!string.IsNullOrWhiteSpace(text) && _prototype.TryIndex<CivFactionPrototype>(text, ref civFactionPrototype))
		{
			val = civFactionPrototype.Icon;
		}
		if (val == null)
		{
			val = (SpriteSpecifier)(object)(CivTeamIconResolver.GetTeamFlag(fallbackTeamId) ?? CivTeamIconResolver.GetTeamBadge(fallbackTeamId));
		}
		Texture val2 = _sprite.Frame0(val);
		if (!string.IsNullOrWhiteSpace(text))
		{
			_factionIconCache[text] = val2;
		}
		return val2;
	}

	private static Color GetTeamAccent(int teamId)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		if (teamId != 1)
		{
			return Team2Color;
		}
		return Team1Color;
	}

	private static PanelContainer MakePanel(Color background, Color border, float thickness = 1f)
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
				BorderThickness = new Thickness(thickness),
				ContentMarginLeftOverride = 12f,
				ContentMarginTopOverride = 10f,
				ContentMarginRightOverride = 12f,
				ContentMarginBottomOverride = 10f
			}
		};
	}

	private static BoxContainer MakeVerticalBox(int separation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		return new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = separation,
			HorizontalExpand = true
		};
	}

	private static Control MakeInfoCard(string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		PanelContainer obj = MakePanel(PanelBgLight, BorderDark);
		((Control)obj).AddChild((Control)new Label
		{
			Text = text,
			FontColorOverride = TextSecondary,
			HorizontalExpand = true,
			HorizontalAlignment = (HAlignment)2
		});
		return (Control)(object)obj;
	}
}
