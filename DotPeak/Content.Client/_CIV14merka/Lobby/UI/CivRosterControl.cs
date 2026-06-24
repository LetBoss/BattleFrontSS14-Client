// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.UI.CivRosterControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivRosterControl : BoxContainer
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IPrototypeManager _prototype;
  private static readonly Color PanelBg = Color.FromHex((ReadOnlySpan<char>) "#1A1E24", new Color?());
  private static readonly Color PanelBgLight = Color.FromHex((ReadOnlySpan<char>) "#22272E", new Color?());
  private static readonly Color BorderDark = Color.FromHex((ReadOnlySpan<char>) "#2A3140", new Color?());
  private static readonly Color BorderLight = Color.FromHex((ReadOnlySpan<char>) "#3A4553", new Color?());
  private static readonly Color AccentGreen = Color.FromHex((ReadOnlySpan<char>) "#4CAF50", new Color?());
  private static readonly Color AccentAmber = Color.FromHex((ReadOnlySpan<char>) "#D9A441", new Color?());
  private static readonly Color AccentRust = Color.FromHex((ReadOnlySpan<char>) "#C24E4E", new Color?());
  private static readonly Color TextPrimary = Color.FromHex((ReadOnlySpan<char>) "#EAEAEA", new Color?());
  private static readonly Color TextSecondary = Color.FromHex((ReadOnlySpan<char>) "#A0A5B2", new Color?());
  private static readonly Color TextMuted = Color.FromHex((ReadOnlySpan<char>) "#6B7280", new Color?());
  private static readonly Color Team1Color = Color.FromHex((ReadOnlySpan<char>) "#4D87D9", new Color?());
  private static readonly Color Team2Color = Color.FromHex((ReadOnlySpan<char>) "#C24E4E", new Color?());
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
  private CivRosterStateEvent _state = new CivRosterStateEvent(false, false, false, false, (string) null, false, false, false, new int?(), new int?(), new List<CivRosterTeamEntry>(), new List<CivRosterPlayerEntry>());
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
    IoCManager.InjectDependencies<CivRosterControl>(this);
    this._sprite = this._entityManager.System<SpriteSystem>();
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    this.SeparationOverride = new int?(8);
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    this._lockTexture = this._resourceCache.GetResource<TextureResource>("/Textures/Interface/VerbIcons/lock.svg.192dpi.png", true).Texture;
    this._unlockTexture = this._resourceCache.GetResource<TextureResource>("/Textures/Interface/VerbIcons/unlock.svg.192dpi.png", true).Texture;
    this._squadTexture = this._sprite.Frame0((SpriteSpecifier) CivTeamIconResolver.GetGenericSquadBadge());
    PanelContainer panelContainer = CivRosterControl.MakePanel(CivRosterControl.PanelBgLight, CivRosterControl.BorderLight);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(12);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = CivRosterControl.MakeVerticalBox(2);
    ((Control) boxContainer3).HorizontalExpand = true;
    Label label = new Label();
    label.Text = Loc.GetString("civ-lobby-roster-title");
    ((Control) label).StyleClasses.Add("LabelHeadingBigger");
    label.FontColorOverride = new Color?(CivRosterControl.TextPrimary);
    this._titleLabel = label;
    this._statusLabel = new Label()
    {
      FontColorOverride = new Color?(CivRosterControl.TextSecondary)
    };
    ((Control) boxContainer3).AddChild((Control) this._titleLabel);
    ((Control) boxContainer3).AddChild((Control) this._statusLabel);
    ((Control) boxContainer2).AddChild((Control) boxContainer3);
    Button button = new Button();
    button.Text = Loc.GetString("civ-lobby-roster-enter-round");
    ((Control) button).MinWidth = 180f;
    ((Control) button).MinHeight = 40f;
    ((Control) button).Visible = false;
    ((Control) button).StyleClasses.Add("ButtonColorGreen");
    ((Control) button).StyleClasses.Add("ButtonBig");
    this._enterRoundButton = button;
    ((BaseButton) this._enterRoundButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action enterRoundRequested = this.EnterRoundRequested;
      if (enterRoundRequested == null)
        return;
      enterRoundRequested();
    });
    ((Control) boxContainer2).AddChild((Control) this._enterRoundButton);
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    ((Control) this).AddChild((Control) panelContainer);
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer4.SeparationOverride = new int?(8);
    ((Control) boxContainer4).HorizontalExpand = true;
    this._tabsRow = boxContainer4;
    ((Control) this).AddChild((Control) this._tabsRow);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    scrollContainer1.VScrollEnabled = true;
    scrollContainer1.HScrollEnabled = false;
    ScrollContainer scrollContainer2 = scrollContainer1;
    this._body = CivRosterControl.MakeVerticalBox(8);
    ((Control) scrollContainer2).AddChild((Control) this._body);
    ((Control) this).AddChild((Control) scrollContainer2);
  }

  public void UpdateState(CivRosterStateEvent state)
  {
    this._state = state;
    this.NormalizeSelection();
    this.Rebuild();
  }

  private void NormalizeSelection()
  {
    if (!this._state.Enabled)
    {
      this._inspectedTeamId = new int?();
      this._expandedSquadId = new int?();
    }
    else
    {
      CivRosterTeamEntry team = this._state.Teams.FirstOrDefault<CivRosterTeamEntry>((Func<CivRosterTeamEntry, bool>) (t =>
      {
        int teamId = t.TeamId;
        int? inspectedTeamId = this._inspectedTeamId;
        int valueOrDefault = inspectedTeamId.GetValueOrDefault();
        return teamId == valueOrDefault & inspectedTeamId.HasValue;
      })) ?? this._state.Teams.FirstOrDefault<CivRosterTeamEntry>((Func<CivRosterTeamEntry, bool>) (t =>
      {
        int teamId = t.TeamId;
        int? selectedTeamId = this._state.SelectedTeamId;
        int valueOrDefault = selectedTeamId.GetValueOrDefault();
        return teamId == valueOrDefault & selectedTeamId.HasValue;
      })) ?? this._state.Teams.OrderBy<CivRosterTeamEntry, int>((Func<CivRosterTeamEntry, int>) (t => t.TeamId)).FirstOrDefault<CivRosterTeamEntry>();
      this._inspectedTeamId = team?.TeamId;
      if (team == null)
      {
        this._expandedSquadId = new int?();
      }
      else
      {
        if (team.Squads.Any<CivRosterSquadEntry>((Func<CivRosterSquadEntry, bool>) (s =>
        {
          int squadId = s.SquadId;
          int? expandedSquadId = this._expandedSquadId;
          int valueOrDefault = expandedSquadId.GetValueOrDefault();
          return squadId == valueOrDefault & expandedSquadId.HasValue;
        })))
          return;
        this._expandedSquadId = team.SelectedSquadIfPresent(this._state.SelectedSquadId)?.SquadId;
      }
    }
  }

  private void Rebuild()
  {
    this.RebuildHeader();
    this.RebuildTabs();
    this.RebuildBody();
  }

  private void RebuildHeader()
  {
    if (!this._state.Enabled)
    {
      this._titleLabel.Text = Loc.GetString("civ-lobby-roster-title");
      this._statusLabel.Text = Loc.GetString("civ-lobby-roster-status-pick-mode");
      ((Control) this._enterRoundButton).Visible = false;
    }
    else if (this._state.LateJoinActive)
    {
      this._titleLabel.Text = Loc.GetString("civ-lobby-roster-latejoin-title");
      this._statusLabel.Text = this._state.CanEnterRound ? Loc.GetString("civ-lobby-roster-latejoin-pick") : this._state.EnterRoundUnavailableReason ?? Loc.GetString("civ-lobby-roster-enter-unavailable");
      ((Control) this._enterRoundButton).Visible = !this._state.IsJoinedRound;
      ((BaseButton) this._enterRoundButton).Disabled = !this._state.CanEnterRound;
    }
    else
    {
      this._titleLabel.Text = Loc.GetString("civ-lobby-roster-title");
      this._statusLabel.Text = this.BuildStatusText();
      ((Control) this._enterRoundButton).Visible = false;
    }
  }

  private string BuildStatusText()
  {
    CivRosterPlayerEntry self = this._state.Players.FirstOrDefault<CivRosterPlayerEntry>((Func<CivRosterPlayerEntry, bool>) (p => p.IsSelected));
    if (self == null)
      return Loc.GetString("civ-lobby-roster-status-pick-before-start");
    CivRosterTeamEntry civRosterTeamEntry = this._state.Teams.FirstOrDefault<CivRosterTeamEntry>((Func<CivRosterTeamEntry, bool>) (t =>
    {
      int teamId1 = t.TeamId;
      int? teamId2 = self.TeamId;
      int valueOrDefault = teamId2.GetValueOrDefault();
      return teamId1 == valueOrDefault & teamId2.HasValue;
    }));
    if (civRosterTeamEntry == null || !self.SquadId.HasValue)
    {
      if (self.State == CivRosterPlayerState.Ready)
        return Loc.GetString("civ-lobby-roster-status-ready-auto");
      if (civRosterTeamEntry == null)
        return Loc.GetString("civ-lobby-roster-status-pick");
      return Loc.GetString("civ-lobby-roster-status-class-pref", new (string, object)[1]
      {
        ("class", (object) CivTdmClassHelper.GetDisplayName(self.Class))
      });
    }
    string displayName = CivTdmClassHelper.GetDisplayName(self.Class);
    return Loc.GetString("civ-lobby-roster-status-assigned", new (string, object)[3]
    {
      ("team", (object) civRosterTeamEntry.TeamName),
      ("squad", (object) self.SquadId),
      ("class", (object) displayName)
    });
  }

  private void RebuildTabs()
  {
    ((Control) this._tabsRow).RemoveAllChildren();
    if (!this._state.Enabled || this._state.Teams.Count == 0)
      return;
    foreach (CivRosterTeamEntry team in (IEnumerable<CivRosterTeamEntry>) this._state.Teams.OrderBy<CivRosterTeamEntry, int>((Func<CivRosterTeamEntry, int>) (t => t.TeamId)))
      ((Control) this._tabsRow).AddChild(this.BuildTeamTab(team));
  }

  private Control BuildTeamTab(CivRosterTeamEntry team)
  {
    Color teamAccent = CivRosterControl.GetTeamAccent(team.TeamId);
    int teamId = team.TeamId;
    int? inspectedTeamId = this._inspectedTeamId;
    int valueOrDefault = inspectedTeamId.GetValueOrDefault();
    bool flag = teamId == valueOrDefault & inspectedTeamId.HasValue;
    bool isSelected = team.IsSelected;
    PanelContainer panelContainer = CivRosterControl.MakePanel(flag ? ((Color) ref teamAccent).WithAlpha(0.18f) : CivRosterControl.PanelBg, flag ? teamAccent : (isSelected ? ((Color) ref CivRosterControl.AccentGreen).WithAlpha(0.6f) : CivRosterControl.BorderDark), flag ? 2f : 1f);
    ((Control) panelContainer).HorizontalExpand = true;
    ((Control) panelContainer).VerticalExpand = true;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(6);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer3.SeparationOverride = new int?(10);
    ((Control) boxContainer3).HorizontalExpand = true;
    BoxContainer boxContainer4 = boxContainer3;
    BoxContainer boxContainer5 = boxContainer4;
    TextureRect textureRect = new TextureRect();
    textureRect.Texture = this.GetTeamTexture(team, team.TeamId);
    ((Control) textureRect).MinSize = new Vector2(44f, 44f);
    ((Control) textureRect).MaxSize = new Vector2(44f, 44f);
    textureRect.Stretch = (TextureRect.StretchMode) 7;
    ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer5).AddChild((Control) textureRect);
    BoxContainer boxContainer6 = CivRosterControl.MakeVerticalBox(2);
    ((Control) boxContainer6).HorizontalExpand = true;
    BoxContainer boxContainer7 = boxContainer6;
    Label label = new Label();
    label.Text = team.TeamName.ToUpperInvariant();
    ((Control) label).StyleClasses.Add("LabelHeadingBigger");
    label.FontColorOverride = new Color?(teamAccent);
    ((Control) boxContainer7).AddChild((Control) label);
    string str = Loc.GetString("civ-lobby-roster-team-subtitle", new (string, object)[2]
    {
      ("players", (object) team.PlayerCount),
      ("squads", (object) team.Squads.Count)
    });
    if (isSelected)
      str = Loc.GetString("civ-lobby-roster-team-yours", new (string, object)[1]
      {
        ("subtitle", (object) str)
      });
    ((Control) boxContainer6).AddChild((Control) new Label()
    {
      Text = str,
      FontColorOverride = new Color?(isSelected ? CivRosterControl.AccentGreen : CivRosterControl.TextSecondary)
    });
    ((Control) boxContainer4).AddChild((Control) boxContainer6);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    (string Text, Color Color) = this.BuildTeamCardCommanderLine(team);
    ((Control) boxContainer2).AddChild((Control) new Label()
    {
      Text = Text,
      FontColorOverride = new Color?(Color)
    });
    if (!isSelected && team.CanSelect)
    {
      Button button1 = new Button();
      button1.Text = Loc.GetString("civ-lobby-roster-select-side");
      ((Control) button1).StyleClasses.Add("ButtonColorGreen");
      ((Control) button1).HorizontalExpand = true;
      ((Control) button1).MinHeight = 30f;
      Button button2 = button1;
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this._inspectedTeamId = new int?(team.TeamId);
        this._expandedSquadId = new int?();
        Action<int> teamSelected = this.TeamSelected;
        if (teamSelected == null)
          return;
        teamSelected(team.TeamId);
      });
      ((Control) boxContainer2).AddChild((Control) button2);
    }
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    ContainerButton containerButton = new ContainerButton();
    ((Control) containerButton).HorizontalExpand = true;
    ((Control) containerButton).VerticalExpand = true;
    ((Control) containerButton).AddChild((Control) panelContainer);
    ((BaseButton) containerButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this._inspectedTeamId = new int?(team.TeamId);
      this.Rebuild();
    });
    return (Control) containerButton;
  }

  private (string Text, Color Color) BuildTeamCardCommanderLine(CivRosterTeamEntry team)
  {
    if (!string.IsNullOrWhiteSpace(team.CommanderName))
      return (Loc.GetString("civ-lobby-roster-team-commander", new (string, object)[1]
      {
        ("name", (object) team.CommanderName)
      }), CivRosterControl.AccentGreen);
    if (team.CommanderCandidates.Count <= 0)
      return (Loc.GetString("civ-lobby-roster-team-no-commander"), CivRosterControl.TextMuted);
    List<CivCommanderCandidateEntry> commanderCandidates = team.CommanderCandidates;
    int totalBase = commanderCandidates.Sum<CivCommanderCandidateEntry>((Func<CivCommanderCandidateEntry, int>) (c => Math.Max(c.PlaytimeMinutes, 1)));
    Dictionary<NetUserId, double> weights = commanderCandidates.ToDictionary<CivCommanderCandidateEntry, NetUserId, double>((Func<CivCommanderCandidateEntry, NetUserId>) (c => c.UserId), (Func<CivCommanderCandidateEntry, double>) (c => (double) Math.Max(c.PlaytimeMinutes, 1) + (double) totalBase * (double) c.Priority * 0.1));
    double totalWeight = weights.Values.Sum();
    List<CivCommanderCandidateEntry> list1 = commanderCandidates.OrderByDescending<CivCommanderCandidateEntry, double>((Func<CivCommanderCandidateEntry, double>) (c => weights[c.UserId])).ToList<CivCommanderCandidateEntry>();
    IEnumerable<CivCommanderCandidateEntry> source = list1.Take<CivCommanderCandidateEntry>(4);
    int num1 = list1.Count - 4;
    List<string> list2 = source.Select<CivCommanderCandidateEntry, string>((Func<CivCommanderCandidateEntry, string>) (c =>
    {
      double num2 = totalWeight > 0.0 ? weights[c.UserId] / totalWeight * 100.0 : 0.0;
      string str1;
      if (c.Priority <= 0)
        str1 = "";
      else
        str1 = $" [★{c.Priority}]";
      string str2 = str1;
      string str3 = c.IsSelf ? " (вы)" : "";
      return $"{c.Name}{str2}{str3} {num2:F0}%";
    })).ToList<string>();
    if (num1 > 0)
      list2.Add($"+{num1}");
    return (string.Join(", ", (IEnumerable<string>) list2), CivRosterControl.AccentAmber);
  }

  private void RebuildBody()
  {
    ((Control) this._body).RemoveAllChildren();
    if (!this._state.Enabled)
    {
      ((Control) this._body).AddChild(CivRosterControl.MakeInfoCard(Loc.GetString("civ-lobby-roster-status-pick-mode")));
    }
    else
    {
      CivRosterTeamEntry team = this._state.Teams.FirstOrDefault<CivRosterTeamEntry>((Func<CivRosterTeamEntry, bool>) (t =>
      {
        int teamId = t.TeamId;
        int? inspectedTeamId = this._inspectedTeamId;
        int valueOrDefault = inspectedTeamId.GetValueOrDefault();
        return teamId == valueOrDefault & inspectedTeamId.HasValue;
      }));
      if (team == null)
        ((Control) this._body).AddChild(CivRosterControl.MakeInfoCard(Loc.GetString("civ-lobby-roster-teams-not-loaded")));
      else if (this._state.RejoinBlockedForCurrentRound)
      {
        ((Control) this._body).AddChild(CivRosterControl.MakeInfoCard(Loc.GetString("civ-lobby-roster-rejoin-blocked")));
      }
      else
      {
        ((Control) this._body).AddChild(this.BuildCommanderRow(team));
        CivRosterPlayerEntry self = this._state.Players.FirstOrDefault<CivRosterPlayerEntry>((Func<CivRosterPlayerEntry, bool>) (p => p.IsSelected));
        if (!this._state.IsJoinedRound && self != null)
        {
          int? nullable = self.SquadId;
          if (!nullable.HasValue)
          {
            nullable = self.TeamId;
            int teamId = team.TeamId;
            if (nullable.GetValueOrDefault() == teamId & nullable.HasValue)
            {
              NetUserId? commanderUserId = team.CommanderUserId;
              NetUserId userId = self.UserId;
              if ((commanderUserId.HasValue ? (NetUserId.op_Inequality(commanderUserId.GetValueOrDefault(), userId) ? 1 : 0) : 1) != 0)
                ((Control) this._body).AddChild(this.BuildPreferredClassCard(self));
            }
          }
        }
        ((Control) this._body).AddChild(this.BuildCreateSquadRow(team));
        if (team.Squads.Count == 0)
        {
          ((Control) this._body).AddChild(CivRosterControl.MakeInfoCard(Loc.GetString("civ-lobby-roster-no-squads")));
        }
        else
        {
          foreach (CivRosterSquadEntry squad in (IEnumerable<CivRosterSquadEntry>) team.Squads.OrderBy<CivRosterSquadEntry, int>((Func<CivRosterSquadEntry, int>) (s => s.SquadId)))
            ((Control) this._body).AddChild(this.BuildSquadAccordion(team, squad));
        }
      }
    }
  }

  private Control BuildCommanderRow(CivRosterTeamEntry team)
  {
    PanelContainer panelContainer = CivRosterControl.MakePanel(CivRosterControl.PanelBgLight, CivRosterControl.BorderLight);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = CivRosterControl.MakeVerticalBox(2);
    ((Control) boxContainer3).HorizontalExpand = true;
    ((Control) boxContainer3).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-lobby-roster-commander-header"),
      FontColorOverride = new Color?(CivRosterControl.TextMuted)
    });
    Control control = this.BuildCommanderLine(team);
    ((Control) boxContainer3).AddChild(control);
    ((Control) boxContainer2).AddChild((Control) boxContainer3);
    if (!this._state.LateJoinActive && team.IsSelected && string.IsNullOrEmpty(team.CommanderName))
    {
      if (team.CommanderCandidates.FirstOrDefault<CivCommanderCandidateEntry>((Func<CivCommanderCandidateEntry, bool>) (c => c.IsSelf)) != null)
      {
        Button button1 = new Button();
        button1.Text = Loc.GetString("civ-lobby-roster-withdraw");
        ((Control) button1).ToolTip = Loc.GetString("civ-lobby-roster-withdraw-tooltip");
        Button button2 = button1;
        ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
        {
          Action<int> commanderRequested = this.WithdrawCommanderRequested;
          if (commanderRequested == null)
            return;
          commanderRequested(team.TeamId);
        });
        ((Control) boxContainer2).AddChild((Control) button2);
      }
      else if (team.CanNominate)
      {
        Button button3 = new Button();
        button3.Text = Loc.GetString("civ-lobby-roster-nominate");
        ((Control) button3).StyleClasses.Add("ButtonColorGreen");
        Button button4 = button3;
        ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnNominatePressed(team.TeamId));
        ((Control) boxContainer2).AddChild((Control) button4);
      }
      else if (!string.IsNullOrWhiteSpace(team.NominateUnavailableReason))
      {
        BoxContainer boxContainer4 = boxContainer2;
        Label label = new Label();
        label.Text = team.NominateUnavailableReason;
        label.FontColorOverride = new Color?(CivRosterControl.TextMuted);
        ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
        ((Control) boxContainer4).AddChild((Control) label);
      }
    }
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  private void OnNominatePressed(int teamId)
  {
    if (this._state.SelfPlaytimeMinutes >= 600)
    {
      Action<int> commanderRequested = this.NominateCommanderRequested;
      if (commanderRequested == null)
        return;
      commanderRequested(teamId);
    }
    else
    {
      CivCommanderWarningWindow commanderWarningWindow = new CivCommanderWarningWindow();
      commanderWarningWindow.ConfirmPressed += (Action) (() =>
      {
        Action<int> commanderRequested = this.NominateCommanderRequested;
        if (commanderRequested == null)
          return;
        commanderRequested(teamId);
      });
      ((BaseWindow) commanderWarningWindow).OpenCentered();
    }
  }

  private Control BuildCommanderLine(CivRosterTeamEntry team)
  {
    if (!string.IsNullOrEmpty(team.CommanderName))
      return (Control) new Label()
      {
        Text = team.CommanderName,
        FontColorOverride = new Color?(CivRosterControl.AccentGreen)
      };
    if (team.CommanderCandidates.Count == 0)
      return (Control) new Label()
      {
        Text = Loc.GetString("civ-lobby-roster-commander-none"),
        FontColorOverride = new Color?(CivRosterControl.TextMuted)
      };
    List<CivCommanderCandidateEntry> commanderCandidates = team.CommanderCandidates;
    int totalBase = commanderCandidates.Sum<CivCommanderCandidateEntry>((Func<CivCommanderCandidateEntry, int>) (c => Math.Max(c.PlaytimeMinutes, 1)));
    Dictionary<NetUserId, double> weights = commanderCandidates.ToDictionary<CivCommanderCandidateEntry, NetUserId, double>((Func<CivCommanderCandidateEntry, NetUserId>) (c => c.UserId), (Func<CivCommanderCandidateEntry, double>) (c => (double) Math.Max(c.PlaytimeMinutes, 1) + (double) totalBase * (double) c.Priority * 0.1));
    double num1 = weights.Values.Sum();
    List<CivCommanderCandidateEntry> list = commanderCandidates.OrderByDescending<CivCommanderCandidateEntry, double>((Func<CivCommanderCandidateEntry, double>) (x => weights[x.UserId])).ToList<CivCommanderCandidateEntry>();
    BoxContainer boxContainer1 = CivRosterControl.MakeVerticalBox(1);
    foreach (CivCommanderCandidateEntry commanderCandidateEntry in list.Take<CivCommanderCandidateEntry>(4))
    {
      double num2 = num1 > 0.0 ? weights[commanderCandidateEntry.UserId] / num1 * 100.0 : 0.0;
      string str1;
      if (commanderCandidateEntry.Priority <= 0)
        str1 = "";
      else
        str1 = $" [★{commanderCandidateEntry.Priority}]";
      string str2 = str1;
      string str3 = commanderCandidateEntry.IsSelf ? " (вы)" : "";
      float num3 = (float) commanderCandidateEntry.PlaytimeMinutes / 60f;
      BoxContainer boxContainer2 = boxContainer1;
      Label label = new Label();
      label.Text = $"{commanderCandidateEntry.Name}{str2}{str3}  {num2:F0}% ({num3:F0}ч)";
      label.FontColorOverride = new Color?(CivRosterControl.AccentAmber);
      ((Control) boxContainer2).AddChild((Control) label);
    }
    int num4 = list.Count - 4;
    if (num4 > 0)
    {
      BoxContainer boxContainer3 = boxContainer1;
      Label label = new Label();
      label.Text = $"+{num4}";
      label.FontColorOverride = new Color?(CivRosterControl.TextMuted);
      ((Control) boxContainer3).AddChild((Control) label);
    }
    return (Control) boxContainer1;
  }

  private Control BuildCreateSquadRow(CivRosterTeamEntry team)
  {
    Button button = new Button();
    button.Text = this._state.LateJoinActive ? Loc.GetString("civ-lobby-roster-join-free-squad") : Loc.GetString("civ-lobby-roster-create-squad");
    ((Control) button).HorizontalExpand = true;
    ((Control) button).MinHeight = 36f;
    ((BaseButton) button).Disabled = !team.CanCreateSquad;
    Button squadRow = button;
    if (!team.CanCreateSquad && !string.IsNullOrWhiteSpace(team.CreateSquadUnavailableReason))
      ((Control) squadRow).ToolTip = team.CreateSquadUnavailableReason;
    ((BaseButton) squadRow).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<int> createSquadRequested = this.CreateSquadRequested;
      if (createSquadRequested == null)
        return;
      createSquadRequested(team.TeamId);
    });
    return (Control) squadRow;
  }

  private Control BuildPreferredClassCard(CivRosterPlayerEntry self)
  {
    PanelContainer panelContainer = CivRosterControl.MakePanel(CivRosterControl.PanelBgLight, CivRosterControl.BorderLight);
    BoxContainer boxContainer1 = CivRosterControl.MakeVerticalBox(8);
    ((Control) boxContainer1).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-lobby-roster-preferred-class-header"),
      FontColorOverride = new Color?(CivRosterControl.TextMuted)
    });
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer2.SeparationOverride = new int?(6);
    ((Control) boxContainer2).HorizontalExpand = true;
    BoxContainer boxContainer3 = boxContainer2;
    foreach (CivTdmClass preferredClassOption in CivRosterControl.PreferredClassOptions)
      ((Control) boxContainer3).AddChild(this.BuildPreferredClassChip(preferredClassOption, self.Class));
    ((Control) boxContainer1).AddChild((Control) boxContainer3);
    ((Control) boxContainer1).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-lobby-roster-preferred-class-hint"),
      FontColorOverride = new Color?(CivRosterControl.TextMuted)
    });
    ((Control) panelContainer).AddChild((Control) boxContainer1);
    return (Control) panelContainer;
  }

  private Control BuildPreferredClassChip(CivTdmClass cls, CivTdmClass currentClass)
  {
    bool flag = cls == currentClass;
    Color color1 = flag ? ((Color) ref CivRosterControl.AccentGreen).WithAlpha(0.2f) : CivRosterControl.PanelBg;
    Color color2 = flag ? CivRosterControl.AccentGreen : CivRosterControl.BorderDark;
    Color color3 = flag ? CivRosterControl.AccentGreen : CivRosterControl.TextPrimary;
    PanelContainer panelContainer1 = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = color1;
    styleBoxFlat.BorderColor = color2;
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(6f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(6f);
    panelContainer1.PanelOverride = (StyleBox) styleBoxFlat;
    PanelContainer panelContainer2 = panelContainer1;
    ((Control) panelContainer2).AddChild((Control) new Label()
    {
      Text = CivTdmClassHelper.GetDisplayName(cls),
      FontColorOverride = new Color?(color3)
    });
    ContainerButton containerButton = new ContainerButton();
    ((Control) containerButton).HorizontalExpand = false;
    ((BaseButton) containerButton).Disabled = flag;
    ((Control) containerButton).AddChild((Control) panelContainer2);
    ((BaseButton) containerButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<CivTdmClass> classSelected = this.ClassSelected;
      if (classSelected == null)
        return;
      classSelected(cls);
    });
    return (Control) containerButton;
  }

  private Control BuildSquadAccordion(CivRosterTeamEntry team, CivRosterSquadEntry squad)
  {
    Color teamAccent = CivRosterControl.GetTeamAccent(team.TeamId);
    int squadId = squad.SquadId;
    int? expandedSquadId = this._expandedSquadId;
    int valueOrDefault = expandedSquadId.GetValueOrDefault();
    bool isExpanded = (squadId == valueOrDefault & expandedSquadId.HasValue ? 1 : 0) != 0;
    bool isMember = squad.IsMember;
    PanelContainer panelContainer = CivRosterControl.MakePanel(isExpanded ? CivRosterControl.PanelBgLight : CivRosterControl.PanelBg, isExpanded ? teamAccent : (isMember ? ((Color) ref CivRosterControl.AccentGreen).WithAlpha(0.6f) : CivRosterControl.BorderDark), isExpanded ? 2f : 1f);
    BoxContainer boxContainer1 = CivRosterControl.MakeVerticalBox(6);
    ContainerButton containerButton1 = new ContainerButton();
    ((Control) containerButton1).HorizontalExpand = true;
    ContainerButton containerButton2 = containerButton1;
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer2.SeparationOverride = new int?(10);
    ((Control) boxContainer2).HorizontalExpand = true;
    BoxContainer boxContainer3 = boxContainer2;
    BoxContainer boxContainer4 = boxContainer3;
    Label label1 = new Label();
    label1.Text = isExpanded ? "[-]" : "[+]";
    label1.FontColorOverride = new Color?(CivRosterControl.TextSecondary);
    ((Control) label1).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) label1).MinWidth = 24f;
    ((Control) boxContainer4).AddChild((Control) label1);
    BoxContainer boxContainer5 = boxContainer3;
    TextureRect textureRect = new TextureRect();
    textureRect.Texture = squad.IsOpen ? this._unlockTexture : this._lockTexture;
    ((Control) textureRect).MinSize = new Vector2(18f, 18f);
    ((Control) textureRect).MaxSize = new Vector2(18f, 18f);
    textureRect.Stretch = (TextureRect.StretchMode) 7;
    ((Control) textureRect).ModulateSelfOverride = new Color?(squad.IsOpen ? CivRosterControl.AccentGreen : CivRosterControl.TextMuted);
    ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer5).AddChild((Control) textureRect);
    BoxContainer boxContainer6 = CivRosterControl.MakeVerticalBox(2);
    ((Control) boxContainer6).HorizontalExpand = true;
    Label label2 = new Label()
    {
      Text = CivRosterControl.GetSquadTitle(squad),
      FontColorOverride = new Color?(isMember ? CivRosterControl.AccentGreen : CivRosterControl.TextPrimary)
    };
    ((Control) boxContainer6).AddChild((Control) label2);
    ((Control) boxContainer6).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-lobby-roster-squad-meta", new (string, object)[4]
      {
        ("leader", (object) squad.LeaderName),
        ("count", (object) squad.MemberCount),
        ("max", (object) squad.MaxMembers),
        ("recruit", (object) Loc.GetString(squad.IsOpen ? "civ-lobby-roster-squad-recruit-open" : "civ-lobby-roster-squad-recruit-closed"))
      }),
      FontColorOverride = new Color?(CivRosterControl.TextSecondary)
    });
    ((Control) boxContainer3).AddChild((Control) boxContainer6);
    if (isMember)
    {
      BoxContainer boxContainer7 = boxContainer3;
      Label label3 = new Label();
      label3.Text = Loc.GetString("civ-lobby-roster-you-here");
      label3.FontColorOverride = new Color?(CivRosterControl.AccentGreen);
      ((Control) label3).VerticalAlignment = (Control.VAlignment) 2;
      ((Control) boxContainer7).AddChild((Control) label3);
    }
    ((Control) containerButton2).AddChild((Control) boxContainer3);
    ((BaseButton) containerButton2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this._expandedSquadId = isExpanded ? new int?() : new int?(squad.SquadId);
      this.Rebuild();
    });
    ((Control) boxContainer1).AddChild((Control) containerButton2);
    if (isExpanded)
      ((Control) boxContainer1).AddChild(this.BuildSquadDetails(team, squad));
    ((Control) panelContainer).AddChild((Control) boxContainer1);
    return (Control) panelContainer;
  }

  private Control BuildSquadDetails(CivRosterTeamEntry team, CivRosterSquadEntry squad)
  {
    BoxContainer boxContainer1 = CivRosterControl.MakeVerticalBox(8);
    ((Control) boxContainer1).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-lobby-roster-members-header"),
      FontColorOverride = new Color?(CivRosterControl.TextMuted)
    });
    ((Control) boxContainer1).AddChild(this.BuildMembersList(squad));
    if (squad.CanRename)
    {
      BoxContainer boxContainer2 = new BoxContainer();
      boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 0;
      boxContainer2.SeparationOverride = new int?(6);
      ((Control) boxContainer2).HorizontalExpand = true;
      BoxContainer boxContainer3 = boxContainer2;
      int teamId = squad.TeamId;
      int squadId = squad.SquadId;
      int? renameSquadId = this._renameSquadId;
      int num = squadId;
      bool flag = renameSquadId.GetValueOrDefault() == num & renameSquadId.HasValue;
      LineEdit lineEdit = new LineEdit();
      ((Control) lineEdit).HorizontalExpand = true;
      lineEdit.PlaceHolder = Loc.GetString("civ-lobby-roster-squad-rename-placeholder");
      lineEdit.Text = flag ? this._renameDraft : squad.SquadName ?? string.Empty;
      LineEdit nameEdit = lineEdit;
      Button button = new Button()
      {
        Text = Loc.GetString("civ-lobby-roster-squad-rename-button")
      };
      nameEdit.OnTextChanged += (Action<LineEdit.LineEditEventArgs>) (args =>
      {
        this._renameSquadId = new int?(squadId);
        this._renameDraft = args.Text;
      });
      ((BaseButton) button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => DoRename());
      nameEdit.OnTextEntered += (Action<LineEdit.LineEditEventArgs>) (_ => DoRename());
      ((Control) boxContainer3).AddChild((Control) nameEdit);
      ((Control) boxContainer3).AddChild((Control) button);
      ((Control) boxContainer1).AddChild((Control) boxContainer3);
      if (flag)
      {
        ((Control) nameEdit).GrabKeyboardFocus();
        nameEdit.CursorPosition = nameEdit.Text.Length;
      }

      void DoRename()
      {
        Action<int, int, string> renameSquadRequested = this.RenameSquadRequested;
        if (renameSquadRequested != null)
          renameSquadRequested(teamId, squadId, nameEdit.Text);
        this._renameSquadId = new int?();
        this._renameDraft = string.Empty;
      }
    }
    if (squad.RoleTickets.Count > 0)
    {
      CivRosterPlayerEntry self = squad.Members.FirstOrDefault<CivRosterPlayerEntry>((Func<CivRosterPlayerEntry, bool>) (m => m.IsSelected));
      bool canPick = squad.IsMember && self != null && !self.IsLeader;
      ((Control) boxContainer1).AddChild((Control) new Label()
      {
        Text = Loc.GetString("civ-lobby-roster-class-header"),
        FontColorOverride = new Color?(CivRosterControl.TextMuted)
      });
      ((Control) boxContainer1).AddChild(this.BuildClassChips(squad, self, canPick));
    }
    ((Control) boxContainer1).AddChild(this.BuildSquadActions(team, squad));
    return (Control) boxContainer1;
  }

  private Control BuildMembersList(CivRosterSquadEntry squad)
  {
    BoxContainer boxContainer = CivRosterControl.MakeVerticalBox(4);
    List<CivRosterPlayerEntry> list = this.OrderMembers(squad).ToList<CivRosterPlayerEntry>();
    if (list.Count == 0)
    {
      ((Control) boxContainer).AddChild(CivRosterControl.MakeInfoCard(Loc.GetString("civ-lobby-roster-squad-empty")));
      return (Control) boxContainer;
    }
    foreach (CivRosterPlayerEntry member in list)
      ((Control) boxContainer).AddChild(this.BuildMemberRow(squad, member));
    return (Control) boxContainer;
  }

  private Control BuildMemberRow(CivRosterSquadEntry squad, CivRosterPlayerEntry member)
  {
    PanelContainer panelContainer = CivRosterControl.MakePanel(CivRosterControl.PanelBg, CivRosterControl.BorderDark);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = boxContainer2;
    TextureRect textureRect = new TextureRect();
    textureRect.Texture = member.IsLeader ? this.GetTeamTexture(squad.TeamId) : this._squadTexture;
    ((Control) textureRect).MinSize = new Vector2(20f, 20f);
    ((Control) textureRect).MaxSize = new Vector2(20f, 20f);
    textureRect.Stretch = (TextureRect.StretchMode) 7;
    ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer3).AddChild((Control) textureRect);
    BoxContainer boxContainer4 = CivRosterControl.MakeVerticalBox(2);
    ((Control) boxContainer4).HorizontalExpand = true;
    string str1 = member.IsLeader ? Loc.GetString("civ-lobby-roster-member-leader") : CivTdmClassHelper.GetDisplayName(member.Class);
    string str2 = member.IsSelected ? Loc.GetString("civ-lobby-roster-member-self") : "";
    ((Control) boxContainer4).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-lobby-roster-member-name", new (string, object)[2]
      {
        ("name", (object) member.Name),
        ("self", (object) str2)
      }),
      FontColorOverride = new Color?(member.IsSelected ? CivRosterControl.AccentGreen : CivRosterControl.TextPrimary)
    });
    ((Control) boxContainer4).AddChild((Control) new Label()
    {
      Text = str1,
      FontColorOverride = new Color?(CivRosterControl.TextSecondary)
    });
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    string str3;
    switch (member.State)
    {
      case CivRosterPlayerState.Ready:
        str3 = Loc.GetString("civ-lobby-roster-state-ready");
        break;
      case CivRosterPlayerState.Joined:
        str3 = Loc.GetString("civ-lobby-roster-state-joined");
        break;
      case CivRosterPlayerState.Disconnected:
        str3 = Loc.GetString("civ-lobby-roster-state-disconnected");
        break;
      default:
        str3 = Loc.GetString("civ-lobby-roster-state-lobby");
        break;
    }
    string str4 = str3;
    Color color1;
    switch (member.State)
    {
      case CivRosterPlayerState.Ready:
        color1 = CivRosterControl.AccentGreen;
        break;
      case CivRosterPlayerState.Joined:
        color1 = CivRosterControl.AccentAmber;
        break;
      case CivRosterPlayerState.Disconnected:
        color1 = CivRosterControl.TextMuted;
        break;
      default:
        color1 = CivRosterControl.TextSecondary;
        break;
    }
    Color color2 = color1;
    BoxContainer boxContainer5 = boxContainer2;
    Label label = new Label();
    label.Text = str4;
    label.FontColorOverride = new Color?(color2);
    ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer5).AddChild((Control) label);
    if (squad.CanManage && !member.IsSelected && member.State != CivRosterPlayerState.Joined)
    {
      Button button1 = new Button();
      button1.Text = Loc.GetString("civ-lobby-roster-kick");
      ((Control) button1).MinWidth = 50f;
      Button button2 = button1;
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        Action<int, int, NetUserId> kickRequested = this.KickRequested;
        if (kickRequested == null)
          return;
        kickRequested(squad.TeamId, squad.SquadId, member.UserId);
      });
      ((Control) boxContainer2).AddChild((Control) button2);
    }
    ((Control) panelContainer).AddChild((Control) boxContainer2);
    return (Control) panelContainer;
  }

  private Control BuildClassChips(
    CivRosterSquadEntry squad,
    CivRosterPlayerEntry? self,
    bool canPick)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(6);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    foreach ((CivTdmClass cls, int available, int total) in squad.RoleTickets.Where<KeyValuePair<CivTdmClass, (int, int)>>((Func<KeyValuePair<CivTdmClass, (int, int)>, bool>) (kvp => !CivRosterControl.IsLeaderOnlyClass(kvp.Key))).OrderBy<KeyValuePair<CivTdmClass, (int, int)>, int>((Func<KeyValuePair<CivTdmClass, (int, int)>, int>) (kvp => (int) kvp.Key)).Select<KeyValuePair<CivTdmClass, (int, int)>, (CivTdmClass, int, int)>((Func<KeyValuePair<CivTdmClass, (int, int)>, (CivTdmClass, int, int)>) (kvp => (kvp.Key, kvp.Value.Available, kvp.Value.Total))).ToList<(CivTdmClass, int, int)>())
      ((Control) boxContainer2).AddChild(this.BuildClassChip(cls, available, total, self?.Class, canPick));
    return (Control) boxContainer2;
  }

  private Control BuildClassChip(
    CivTdmClass cls,
    int available,
    int total,
    CivTdmClass? currentClass,
    bool canPick)
  {
    CivTdmClass? nullable = currentClass;
    CivTdmClass civTdmClass = cls;
    bool flag1 = nullable.GetValueOrDefault() == civTdmClass & nullable.HasValue;
    bool flag2 = total > 0 && available <= 0 && !flag1;
    Color color1 = flag1 ? ((Color) ref CivRosterControl.AccentGreen).WithAlpha(0.2f) : CivRosterControl.PanelBg;
    Color color2 = flag1 ? CivRosterControl.AccentGreen : (flag2 ? ((Color) ref CivRosterControl.AccentRust).WithAlpha(0.5f) : CivRosterControl.BorderDark);
    Color color3 = flag1 ? CivRosterControl.AccentGreen : (flag2 ? CivRosterControl.TextMuted : CivRosterControl.TextPrimary);
    PanelContainer panelContainer1 = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = color1;
    styleBoxFlat.BorderColor = color2;
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(6f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(6f);
    panelContainer1.PanelOverride = (StyleBox) styleBoxFlat;
    PanelContainer panelContainer2 = panelContainer1;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(6);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    ((Control) boxContainer2).AddChild((Control) new Label()
    {
      Text = CivTdmClassHelper.GetDisplayName(cls),
      FontColorOverride = new Color?(color3)
    });
    if (total > 0)
    {
      string str1;
      if (total < 999)
        str1 = $"{Math.Max(0, total - available)}/{total}";
      else
        str1 = "∞";
      string str2 = str1;
      ((Control) boxContainer2).AddChild((Control) new Label()
      {
        Text = str2,
        FontColorOverride = new Color?(flag2 ? CivRosterControl.AccentRust : CivRosterControl.TextSecondary)
      });
    }
    ((Control) panelContainer2).AddChild((Control) boxContainer2);
    ContainerButton containerButton1 = new ContainerButton();
    ((Control) containerButton1).HorizontalExpand = false;
    ((BaseButton) containerButton1).Disabled = !canPick | flag1 | flag2;
    ContainerButton containerButton2 = containerButton1;
    ((Control) containerButton2).AddChild((Control) panelContainer2);
    if (canPick)
      ((BaseButton) containerButton2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        Action<CivTdmClass> classSelected = this.ClassSelected;
        if (classSelected == null)
          return;
        classSelected(cls);
      });
    return (Control) containerButton2;
  }

  private Control BuildSquadActions(CivRosterTeamEntry team, CivRosterSquadEntry squad)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer1.SeparationOverride = new int?(8);
    ((Control) boxContainer1).HorizontalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    if (!squad.IsMember && squad.CanJoin)
    {
      Button button1 = new Button();
      button1.Text = this._state.LateJoinActive ? Loc.GetString("civ-lobby-roster-join-and-enter") : Loc.GetString("civ-lobby-roster-join");
      ((Control) button1).HorizontalExpand = true;
      ((Control) button1).StyleClasses.Add("ButtonColorGreen");
      Button button2 = button1;
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        Action<int, int> joinSquadRequested = this.JoinSquadRequested;
        if (joinSquadRequested == null)
          return;
        joinSquadRequested(squad.TeamId, squad.SquadId);
      });
      ((Control) boxContainer2).AddChild((Control) button2);
    }
    else if (!squad.IsMember && !string.IsNullOrWhiteSpace(squad.JoinUnavailableReason))
    {
      BoxContainer boxContainer3 = boxContainer2;
      Label label = new Label();
      label.Text = squad.JoinUnavailableReason;
      label.FontColorOverride = new Color?(CivRosterControl.AccentRust);
      ((Control) label).HorizontalExpand = true;
      ((Control) boxContainer3).AddChild((Control) label);
    }
    if (squad.IsMember && squad.CanLeave)
    {
      Button button3 = new Button();
      button3.Text = Loc.GetString("civ-lobby-roster-leave");
      ((Control) button3).HorizontalExpand = true;
      Button button4 = button3;
      ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        Action leaveSquadRequested = this.LeaveSquadRequested;
        if (leaveSquadRequested == null)
          return;
        leaveSquadRequested();
      });
      ((Control) boxContainer2).AddChild((Control) button4);
    }
    else if (squad.IsMember && !string.IsNullOrWhiteSpace(squad.LeaveUnavailableReason))
    {
      BoxContainer boxContainer4 = boxContainer2;
      Label label = new Label();
      label.Text = squad.LeaveUnavailableReason;
      label.FontColorOverride = new Color?(CivRosterControl.TextMuted);
      ((Control) label).HorizontalExpand = true;
      ((Control) boxContainer4).AddChild((Control) label);
    }
    if (((Control) boxContainer2).ChildCount == 0)
    {
      BoxContainer boxContainer5 = boxContainer2;
      Label label = new Label();
      label.Text = Loc.GetString("civ-lobby-roster-no-actions");
      label.FontColorOverride = new Color?(CivRosterControl.TextMuted);
      ((Control) label).HorizontalExpand = true;
      ((Control) boxContainer5).AddChild((Control) label);
    }
    return (Control) boxContainer2;
  }

  private IEnumerable<CivRosterPlayerEntry> OrderMembers(CivRosterSquadEntry squad)
  {
    NetUserId? leaderId = squad.LeaderId;
    CivRosterPlayerEntry rosterPlayerEntry = !leaderId.HasValue ? (CivRosterPlayerEntry) null : squad.Members.FirstOrDefault<CivRosterPlayerEntry>((Func<CivRosterPlayerEntry, bool>) (m => NetUserId.op_Equality(m.UserId, leaderId.Value))) ?? this._state.Players.FirstOrDefault<CivRosterPlayerEntry>((Func<CivRosterPlayerEntry, bool>) (p => NetUserId.op_Equality(p.UserId, leaderId.Value)));
    if (rosterPlayerEntry != null)
      yield return rosterPlayerEntry;
    foreach (CivRosterPlayerEntry member in squad.Members)
    {
      if (!leaderId.HasValue || !NetUserId.op_Equality(member.UserId, leaderId.Value))
        yield return member;
    }
  }

  private static bool IsLeaderOnlyClass(CivTdmClass cls)
  {
    bool flag;
    switch (cls)
    {
      case CivTdmClass.SquadLeader:
      case CivTdmClass.EngineerLeader:
      case CivTdmClass.MedicLeader:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    return flag;
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
          return squad.SquadName;
        return Loc.GetString("civ-lobby-roster-squad-title", new (string, object)[1]
        {
          ("id", (object) squad.SquadId)
        });
    }
  }

  private Texture GetTeamTexture(int teamId)
  {
    return this.GetTeamTexture(this._state.Teams.FirstOrDefault<CivRosterTeamEntry>((Func<CivRosterTeamEntry, bool>) (t => t.TeamId == teamId)), teamId);
  }

  private Texture GetTeamTexture(CivRosterTeamEntry? team, int fallbackTeamId)
  {
    string key = team?.SideId ?? string.Empty;
    Texture teamTexture1;
    if (!string.IsNullOrWhiteSpace(key) && this._factionIconCache.TryGetValue(key, out teamTexture1))
      return teamTexture1;
    SpriteSpecifier spriteSpecifier = (SpriteSpecifier) null;
    CivFactionPrototype factionPrototype;
    if (!string.IsNullOrWhiteSpace(key) && this._prototype.TryIndex<CivFactionPrototype>(key, ref factionPrototype))
      spriteSpecifier = factionPrototype.Icon;
    if (spriteSpecifier == null)
      spriteSpecifier = (SpriteSpecifier) (CivTeamIconResolver.GetTeamFlag(fallbackTeamId) ?? CivTeamIconResolver.GetTeamBadge(fallbackTeamId));
    Texture teamTexture2 = this._sprite.Frame0(spriteSpecifier);
    if (!string.IsNullOrWhiteSpace(key))
      this._factionIconCache[key] = teamTexture2;
    return teamTexture2;
  }

  private static Color GetTeamAccent(int teamId)
  {
    return teamId != 1 ? CivRosterControl.Team2Color : CivRosterControl.Team1Color;
  }

  private static PanelContainer MakePanel(Color background, Color border, float thickness = 1f)
  {
    PanelContainer panelContainer = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = background;
    styleBoxFlat.BorderColor = border;
    styleBoxFlat.BorderThickness = new Thickness(thickness);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(12f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(12f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(10f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    return panelContainer;
  }

  private static BoxContainer MakeVerticalBox(int separation)
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer.SeparationOverride = new int?(separation);
    ((Control) boxContainer).HorizontalExpand = true;
    return boxContainer;
  }

  private static Control MakeInfoCard(string text)
  {
    PanelContainer panelContainer = CivRosterControl.MakePanel(CivRosterControl.PanelBgLight, CivRosterControl.BorderDark);
    Label label = new Label();
    label.Text = text;
    label.FontColorOverride = new Color?(CivRosterControl.TextSecondary);
    ((Control) label).HorizontalExpand = true;
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) panelContainer).AddChild((Control) label);
    return (Control) panelContainer;
  }
}
