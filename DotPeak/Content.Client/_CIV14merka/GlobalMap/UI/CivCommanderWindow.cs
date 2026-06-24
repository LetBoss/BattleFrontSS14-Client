// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.GlobalMap.UI.CivCommanderWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Teams;
using Content.Client.UserInterface.Controls;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
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
    IoCManager.InjectDependencies<CivCommanderWindow>(this);
    this._sprite = this._entityManager.System<SpriteSystem>();
    this._system = system;
    this.Title = Loc.GetString("civ-gmap-hq-title");
    ((Control) this).MinSize = new Vector2(1120f, 760f);
    ((Control) this).SetSize = new Vector2(1220f, 820f);
    this.Resizable = true;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(10);
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(8f);
    BoxContainer boxContainer2 = boxContainer1;
    PanelContainer panelContainer1 = CivCommanderWindow.MakePanel(Color.FromHex((ReadOnlySpan<char>) "#1E232C", new Color?()), Color.FromHex((ReadOnlySpan<char>) "#5B6D83", new Color?()));
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer3.SeparationOverride = new int?(12);
    ((Control) boxContainer3).HorizontalExpand = true;
    BoxContainer boxContainer4 = boxContainer3;
    TextureRect textureRect = new TextureRect();
    ((Control) textureRect).MinSize = new Vector2(54f, 54f);
    ((Control) textureRect).MaxSize = new Vector2(54f, 54f);
    textureRect.Stretch = (TextureRect.StretchMode) 7;
    ((Control) textureRect).Visible = false;
    this._teamIcon = textureRect;
    ((Control) boxContainer4).AddChild(CivCommanderWindow.MakeIconPanel((Control) this._teamIcon, Color.FromHex((ReadOnlySpan<char>) "#4D87D9", new Color?())));
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer5.SeparationOverride = new int?(4);
    ((Control) boxContainer5).HorizontalExpand = true;
    BoxContainer boxContainer6 = boxContainer5;
    Label label1 = new Label();
    label1.Text = Loc.GetString("civ-gmap-hq-side-title");
    ((Control) label1).StyleClasses.Add("FancyWindowTitle");
    this._titleLabel = label1;
    this._summaryLabel = new Label()
    {
      Text = Loc.GetString("civ-gmap-hq-no-data"),
      FontColorOverride = new Color?(Color.LightGray)
    };
    this._selectionLabel = new Label()
    {
      Text = Loc.GetString("civ-gmap-hq-select-hint"),
      FontColorOverride = new Color?(Color.Gold)
    };
    ((Control) boxContainer6).AddChild((Control) this._titleLabel);
    ((Control) boxContainer6).AddChild((Control) this._summaryLabel);
    ((Control) boxContainer6).AddChild((Control) this._selectionLabel);
    ((Control) boxContainer4).AddChild((Control) boxContainer6);
    ((Control) panelContainer1).AddChild((Control) boxContainer4);
    ((Control) boxContainer2).AddChild((Control) panelContainer1);
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer7.SeparationOverride = new int?(10);
    ((Control) boxContainer7).HorizontalExpand = true;
    ((Control) boxContainer7).VerticalExpand = true;
    BoxContainer boxContainer8 = boxContainer7;
    PanelContainer panelContainer2 = CivCommanderWindow.MakePanel(Color.FromHex((ReadOnlySpan<char>) "#252830", new Color?()), Color.FromHex((ReadOnlySpan<char>) "#3E4653", new Color?()));
    ((Control) panelContainer2).SetWidth = 320f;
    ((Control) panelContainer2).VerticalExpand = true;
    BoxContainer boxContainer9 = CivCommanderWindow.MakeVerticalBox(8);
    BoxContainer boxContainer10 = boxContainer9;
    Label label2 = new Label();
    label2.Text = Loc.GetString("civ-gmap-hq-squads");
    ((Control) label2).StyleClasses.Add("FancyWindowTitle");
    ((Control) boxContainer10).AddChild((Control) label2);
    this._squadHintLabel = new Label()
    {
      Text = Loc.GetString("civ-gmap-hq-squads-hint"),
      FontColorOverride = new Color?(Color.LightGray)
    };
    ((Control) boxContainer9).AddChild((Control) this._squadHintLabel);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    scrollContainer1.VScrollEnabled = true;
    scrollContainer1.HScrollEnabled = false;
    ScrollContainer scrollContainer2 = scrollContainer1;
    this._squadList = CivCommanderWindow.MakeVerticalBox(8);
    ((Control) scrollContainer2).AddChild((Control) this._squadList);
    ((Control) boxContainer9).AddChild((Control) scrollContainer2);
    ((Control) panelContainer2).AddChild((Control) boxContainer9);
    ((Control) boxContainer8).AddChild((Control) panelContainer2);
    PanelContainer panelContainer3 = CivCommanderWindow.MakePanel(Color.FromHex((ReadOnlySpan<char>) "#23272F", new Color?()), Color.FromHex((ReadOnlySpan<char>) "#44505F", new Color?()));
    ((Control) panelContainer3).HorizontalExpand = true;
    ((Control) panelContainer3).VerticalExpand = true;
    ((Control) panelContainer3).SizeFlagsStretchRatio = 1.2f;
    BoxContainer boxContainer11 = CivCommanderWindow.MakeVerticalBox(8);
    Label label3 = new Label();
    label3.Text = Loc.GetString("civ-gmap-hq-members");
    ((Control) label3).StyleClasses.Add("FancyWindowTitle");
    this._membersTitleLabel = label3;
    this._membersHintLabel = new Label()
    {
      Text = Loc.GetString("civ-gmap-hq-members-hint"),
      FontColorOverride = new Color?(Color.LightGray)
    };
    ((Control) boxContainer11).AddChild((Control) this._membersTitleLabel);
    ((Control) boxContainer11).AddChild((Control) this._membersHintLabel);
    ScrollContainer scrollContainer3 = new ScrollContainer();
    ((Control) scrollContainer3).HorizontalExpand = true;
    ((Control) scrollContainer3).VerticalExpand = true;
    scrollContainer3.VScrollEnabled = true;
    scrollContainer3.HScrollEnabled = false;
    ScrollContainer scrollContainer4 = scrollContainer3;
    this._memberList = CivCommanderWindow.MakeVerticalBox(6);
    ((Control) scrollContainer4).AddChild((Control) this._memberList);
    ((Control) boxContainer11).AddChild((Control) scrollContainer4);
    ((Control) panelContainer3).AddChild((Control) boxContainer11);
    ((Control) boxContainer8).AddChild((Control) panelContainer3);
    PanelContainer panelContainer4 = CivCommanderWindow.MakePanel(Color.FromHex((ReadOnlySpan<char>) "#252830", new Color?()), Color.FromHex((ReadOnlySpan<char>) "#3E4653", new Color?()));
    ((Control) panelContainer4).SetWidth = 340f;
    ((Control) panelContainer4).VerticalExpand = true;
    BoxContainer boxContainer12 = CivCommanderWindow.MakeVerticalBox(8);
    BoxContainer boxContainer13 = boxContainer12;
    Label label4 = new Label();
    label4.Text = Loc.GetString("civ-gmap-hq-transfer");
    ((Control) label4).StyleClasses.Add("FancyWindowTitle");
    ((Control) boxContainer13).AddChild((Control) label4);
    this._selectedPlayerLabel = new Label()
    {
      Text = Loc.GetString("civ-gmap-hq-no-player"),
      FontColorOverride = new Color?(Color.White)
    };
    this._transferHintLabel = new Label()
    {
      Text = Loc.GetString("civ-gmap-hq-transfer-hint"),
      FontColorOverride = new Color?(Color.LightGray)
    };
    ((Control) boxContainer12).AddChild((Control) this._selectedPlayerLabel);
    ((Control) boxContainer12).AddChild((Control) this._transferHintLabel);
    GridContainer gridContainer1 = new GridContainer();
    gridContainer1.Columns = 2;
    ((Control) gridContainer1).HorizontalExpand = true;
    GridContainer gridContainer2 = gridContainer1;
    Button button1 = new Button();
    button1.Text = Loc.GetString("civ-gmap-hq-move-reserve");
    ((Control) button1).HorizontalExpand = true;
    this._moveToReserveButton = button1;
    ((BaseButton) this._moveToReserveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.MoveSelectedPlayerToReserve());
    ((Control) gridContainer2).AddChild((Control) this._moveToReserveButton);
    Button button2 = new Button();
    button2.Text = Loc.GetString("civ-gmap-hq-move-new-squad");
    ((Control) button2).HorizontalExpand = true;
    this._moveToNewSquadButton = button2;
    ((BaseButton) this._moveToNewSquadButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.MoveSelectedPlayerToNewSquad());
    ((Control) gridContainer2).AddChild((Control) this._moveToNewSquadButton);
    ((Control) boxContainer12).AddChild((Control) gridContainer2);
    ScrollContainer scrollContainer5 = new ScrollContainer();
    ((Control) scrollContainer5).HorizontalExpand = true;
    ((Control) scrollContainer5).VerticalExpand = true;
    scrollContainer5.VScrollEnabled = true;
    scrollContainer5.HScrollEnabled = false;
    ScrollContainer scrollContainer6 = scrollContainer5;
    this._destinationList = CivCommanderWindow.MakeVerticalBox(6);
    ((Control) scrollContainer6).AddChild((Control) this._destinationList);
    ((Control) boxContainer12).AddChild((Control) scrollContainer6);
    ((Control) panelContainer4).AddChild((Control) boxContainer12);
    ((Control) boxContainer8).AddChild((Control) panelContainer4);
    ((Control) boxContainer2).AddChild((Control) boxContainer8);
    this.ContentsContainer.AddChild((Control) boxContainer2);
  }

  public void UpdateState(CivCommanderState? state, int? selectedSquadId)
  {
    this._state = state;
    this.NormalizeSelection(selectedSquadId);
    this.Rebuild();
  }

  private void NormalizeSelection(int? preferredSquadId)
  {
    if (this._state == null)
    {
      this._selectedSectionSquadId = new int?();
      this._selectedPlayerUserId = new NetUserId?();
      this._pendingFollowPlayerUserId = new NetUserId?();
    }
    else
    {
      NetUserId? followPlayerUserId = this._pendingFollowPlayerUserId;
      if (followPlayerUserId.HasValue)
      {
        CivCommanderPlayerState player;
        if (this.TryFindPlayer(followPlayerUserId.GetValueOrDefault(), out player))
        {
          this._selectedSectionSquadId = new int?(player.SquadId);
          this._selectedPlayerUserId = new NetUserId?(player.UserId);
        }
        this._pendingFollowPlayerUserId = new NetUserId?();
      }
      int? selectedSectionSquadId = this._selectedSectionSquadId;
      if (!selectedSectionSquadId.HasValue || !this.HasSection(selectedSectionSquadId.GetValueOrDefault()))
      {
        if (preferredSquadId.HasValue)
        {
          int valueOrDefault = preferredSquadId.GetValueOrDefault();
          if (this.HasSection(valueOrDefault))
          {
            this._selectedSectionSquadId = new int?(valueOrDefault);
            goto label_11;
          }
        }
        this._selectedSectionSquadId = this._state.Squads.Count <= 0 ? new int?(0) : new int?(this._state.Squads[0].SquadId);
      }
label_11:
      List<CivCommanderPlayerState> list = this.GetDisplayedPlayers().ToList<CivCommanderPlayerState>();
      NetUserId? nullable1 = this._selectedPlayerUserId;
      if (nullable1.HasValue)
      {
        NetUserId selectedPlayerId = nullable1.GetValueOrDefault();
        if (!list.All<CivCommanderPlayerState>((Func<CivCommanderPlayerState, bool>) (player => NetUserId.op_Inequality(player.UserId, selectedPlayerId))))
          return;
      }
      NetUserId? nullable2;
      if (list.Count <= 0)
      {
        nullable1 = new NetUserId?();
        nullable2 = nullable1;
      }
      else
        nullable2 = new NetUserId?(list[0].UserId);
      this._selectedPlayerUserId = nullable2;
    }
  }

  private void Rebuild()
  {
    this.UpdateHero();
    this.RebuildSquadList();
    this.RebuildMemberList();
    this.RebuildDestinationList();
  }

  private void UpdateHero()
  {
    if (this._state == null)
    {
      ((Control) this._teamIcon).Visible = false;
      this._titleLabel.Text = Loc.GetString("civ-gmap-hq-side-title");
      this._summaryLabel.Text = Loc.GetString("civ-gmap-hq-no-data");
      this._selectionLabel.Text = Loc.GetString("civ-gmap-hq-waiting-snapshot");
    }
    else
    {
      this._teamIcon.Texture = this._sprite.Frame0((SpriteSpecifier) CivTeamIconResolver.GetTeamBadge(this._state.TeamId));
      ((Control) this._teamIcon).Visible = true;
      this._titleLabel.Text = Loc.GetString("civ-gmap-hq-team-title", new (string, object)[1]
      {
        ("team", (object) Loc.GetString(this._state.TeamId == 2 ? "civ-team-short-rf" : "civ-team-short-usa"))
      });
      this._summaryLabel.Text = Loc.GetString("civ-gmap-hq-summary", new (string, object)[2]
      {
        ("squads", (object) this._state.Squads.Count),
        ("reserve", (object) this._state.ReservePlayers.Count)
      });
      CivCommanderSquadState squad;
      if (this.TryGetDisplayedSquad(out squad))
        this._selectionLabel.Text = Loc.GetString("civ-gmap-hq-selected-squad", new (string, object)[2]
        {
          ("squad", (object) squad.SquadId),
          ("order", (object) CivCommanderWindow.GetOrderText(squad.Order))
        });
      else
        this._selectionLabel.Text = Loc.GetString("civ-gmap-hq-selected-reserve", new (string, object)[1]
        {
          ("count", (object) this._state.ReservePlayers.Count)
        });
    }
  }

  private void RebuildSquadList()
  {
    ((Control) this._squadList).DisposeAllChildren();
    if (this._state == null)
    {
      ((Control) this._squadList).AddChild(CivCommanderWindow.MakeInfoCard(Loc.GetString("civ-gmap-hq-no-state")));
    }
    else
    {
      foreach (CivCommanderSquadState commanderSquadState in (IEnumerable<CivCommanderSquadState>) this._state.Squads.OrderBy<CivCommanderSquadState, int>((Func<CivCommanderSquadState, int>) (entry => entry.SquadId)))
      {
        CivCommanderSquadState squad = commanderSquadState;
        int? selectedSectionSquadId = this._selectedSectionSquadId;
        int squadId = squad.SquadId;
        bool flag = selectedSectionSquadId.GetValueOrDefault() == squadId & selectedSectionSquadId.HasValue;
        Color border = flag ? CivCommanderWindow.GetTeamAccent(this._state.TeamId) : Color.FromHex((ReadOnlySpan<char>) "#37404B", new Color?());
        PanelContainer panelContainer = CivCommanderWindow.MakePanel(flag ? ((Color) ref border).WithAlpha(0.16f) : Color.FromHex((ReadOnlySpan<char>) "#20242C", new Color?()), border, flag ? 2f : 1f);
        BoxContainer boxContainer1 = CivCommanderWindow.MakeVerticalBox(6);
        BoxContainer boxContainer2 = new BoxContainer();
        boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 0;
        boxContainer2.SeparationOverride = new int?(6);
        ((Control) boxContainer2).HorizontalExpand = true;
        BoxContainer boxContainer3 = boxContainer2;
        Button button1 = new Button();
        button1.Text = Loc.GetString("civ-gmap-hq-squad-button", new (string, object)[1]
        {
          ("squad", (object) squad.SquadId)
        });
        ((Control) button1).HorizontalExpand = true;
        ((BaseButton) button1).ToggleMode = true;
        ((BaseButton) button1).Pressed = flag;
        Button button2 = button1;
        ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SelectSection(squad.SquadId));
        ((Control) boxContainer3).AddChild((Control) button2);
        ((Control) boxContainer1).AddChild((Control) boxContainer3);
        ((Control) boxContainer1).AddChild((Control) new Label()
        {
          Text = Loc.GetString("civ-gmap-hq-squad-leader", new (string, object)[1]
          {
            ("leader", (object) squad.LeaderName)
          }),
          FontColorOverride = new Color?(Color.White)
        });
        ((Control) boxContainer1).AddChild((Control) new Label()
        {
          Text = Loc.GetString("civ-gmap-hq-squad-members", new (string, object)[2]
          {
            ("members", (object) squad.Members.Count),
            ("order", (object) CivCommanderWindow.GetOrderText(squad.Order))
          }),
          FontColorOverride = new Color?(Color.LightGray)
        });
        ((Control) panelContainer).AddChild((Control) boxContainer1);
        ((Control) this._squadList).AddChild((Control) panelContainer);
      }
      int? selectedSectionSquadId1 = this._selectedSectionSquadId;
      int num = 0;
      bool flag1 = selectedSectionSquadId1.GetValueOrDefault() == num & selectedSectionSquadId1.HasValue;
      Color border1 = flag1 ? Color.FromHex((ReadOnlySpan<char>) "#7D8A97", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#37404B", new Color?());
      PanelContainer panelContainer1 = CivCommanderWindow.MakePanel(flag1 ? ((Color) ref border1).WithAlpha(0.16f) : Color.FromHex((ReadOnlySpan<char>) "#20242C", new Color?()), border1, flag1 ? 2f : 1f);
      BoxContainer boxContainer = CivCommanderWindow.MakeVerticalBox(6);
      Button button3 = new Button();
      button3.Text = Loc.GetString("civ-gmap-hq-reserve");
      ((Control) button3).HorizontalExpand = true;
      ((BaseButton) button3).ToggleMode = true;
      ((BaseButton) button3).Pressed = flag1;
      Button button4 = button3;
      ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SelectSection(0));
      ((Control) boxContainer).AddChild((Control) button4);
      ((Control) boxContainer).AddChild((Control) new Label()
      {
        Text = Loc.GetString("civ-gmap-hq-reserve-members", new (string, object)[1]
        {
          ("count", (object) this._state.ReservePlayers.Count)
        }),
        FontColorOverride = new Color?(Color.LightGray)
      });
      ((Control) panelContainer1).AddChild((Control) boxContainer);
      ((Control) this._squadList).AddChild((Control) panelContainer1);
    }
  }

  private void RebuildMemberList()
  {
    ((Control) this._memberList).DisposeAllChildren();
    if (this._state == null)
    {
      this._membersTitleLabel.Text = Loc.GetString("civ-gmap-hq-members");
      this._membersHintLabel.Text = Loc.GetString("civ-gmap-hq-members-no-data");
      ((Control) this._memberList).AddChild(CivCommanderWindow.MakeInfoCard(Loc.GetString("civ-gmap-hq-members-nothing")));
    }
    else
    {
      List<CivCommanderPlayerState> list = this.GetDisplayedPlayers().OrderByDescending<CivCommanderPlayerState, bool>((Func<CivCommanderPlayerState, bool>) (player => player.IsSquadLeader)).ThenBy<CivCommanderPlayerState, string>((Func<CivCommanderPlayerState, string>) (player => player.Name)).ToList<CivCommanderPlayerState>();
      int valueOrDefault = this._selectedSectionSquadId.GetValueOrDefault();
      Label membersTitleLabel = this._membersTitleLabel;
      string str1;
      if (valueOrDefault <= 0)
        str1 = Loc.GetString("civ-gmap-hq-members-reserve-title");
      else
        str1 = Loc.GetString("civ-gmap-hq-members-squad-title", new (string, object)[1]
        {
          ("squad", (object) valueOrDefault)
        });
      membersTitleLabel.Text = str1;
      this._membersHintLabel.Text = list.Count > 0 ? Loc.GetString("civ-gmap-hq-members-hint-select") : Loc.GetString("civ-gmap-hq-members-empty");
      if (list.Count == 0)
      {
        ((Control) this._memberList).AddChild(CivCommanderWindow.MakeInfoCard(Loc.GetString("civ-gmap-hq-members-empty-list")));
      }
      else
      {
        foreach (CivCommanderPlayerState commanderPlayerState in list)
        {
          CivCommanderPlayerState player = commanderPlayerState;
          NetUserId? selectedPlayerUserId = this._selectedPlayerUserId;
          NetUserId userId = player.UserId;
          bool flag = selectedPlayerUserId.HasValue && NetUserId.op_Equality(selectedPlayerUserId.GetValueOrDefault(), userId);
          Color border = flag ? Color.FromHex((ReadOnlySpan<char>) "#5A86C8", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#37404B", new Color?());
          PanelContainer panelContainer = CivCommanderWindow.MakePanel(flag ? ((Color) ref border).WithAlpha(0.16f) : Color.FromHex((ReadOnlySpan<char>) "#20242C", new Color?()), border, flag ? 2f : 1f);
          BoxContainer boxContainer1 = new BoxContainer();
          boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
          boxContainer1.SeparationOverride = new int?(8);
          ((Control) boxContainer1).HorizontalExpand = true;
          BoxContainer boxContainer2 = boxContainer1;
          Button button1 = new Button();
          button1.Text = CivCommanderWindow.BuildPlayerButtonText(player);
          ((Control) button1).HorizontalExpand = true;
          ((BaseButton) button1).ToggleMode = true;
          ((BaseButton) button1).Pressed = flag;
          Button button2 = button1;
          ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
          {
            this._selectedPlayerUserId = new NetUserId?(player.UserId);
            this.Rebuild();
          });
          ((Control) boxContainer2).AddChild((Control) button2);
          BoxContainer boxContainer3 = boxContainer2;
          Label label1 = new Label();
          Label label2 = label1;
          string str2;
          if (player.SquadId != 0)
            str2 = Loc.GetString("civ-gmap-hq-member-squad-mark", new (string, object)[1]
            {
              ("squad", (object) player.SquadId)
            });
          else
            str2 = Loc.GetString("civ-gmap-hq-member-reserve-mark");
          label2.Text = str2;
          label1.FontColorOverride = new Color?(player.SquadId == 0 ? Color.LightGray : Color.White);
          Label label3 = label1;
          ((Control) boxContainer3).AddChild((Control) label3);
          ((Control) panelContainer).AddChild((Control) boxContainer2);
          ((Control) this._memberList).AddChild((Control) panelContainer);
        }
      }
    }
  }

  private void RebuildDestinationList()
  {
    ((Control) this._destinationList).DisposeAllChildren();
    CivCommanderPlayerState player;
    bool selectedPlayer = this.TryGetSelectedPlayer(out player);
    Label selectedPlayerLabel = this._selectedPlayerLabel;
    string str;
    if (!selectedPlayer)
      str = Loc.GetString("civ-gmap-hq-no-player");
    else
      str = Loc.GetString("civ-gmap-hq-selected-player", new (string, object)[1]
      {
        ("name", (object) player.Name)
      });
    selectedPlayerLabel.Text = str;
    ((BaseButton) this._moveToReserveButton).Disabled = !selectedPlayer || player.SquadId == 0;
    ((BaseButton) this._moveToNewSquadButton).Disabled = !selectedPlayer;
    if (this._state == null)
    {
      this._transferHintLabel.Text = Loc.GetString("civ-gmap-hq-state-not-arrived");
      ((Control) this._destinationList).AddChild(CivCommanderWindow.MakeInfoCard(Loc.GetString("civ-gmap-hq-no-actions")));
    }
    else
    {
      this._transferHintLabel.Text = selectedPlayer ? Loc.GetString("civ-gmap-hq-transfer-hint-instant") : Loc.GetString("civ-gmap-hq-transfer-hint-select-first");
      if (!selectedPlayer)
      {
        ((Control) this._destinationList).AddChild(CivCommanderWindow.MakeInfoCard(Loc.GetString("civ-gmap-hq-destinations-after-select")));
      }
      else
      {
        foreach (CivCommanderSquadState commanderSquadState in (IEnumerable<CivCommanderSquadState>) this._state.Squads.OrderBy<CivCommanderSquadState, int>((Func<CivCommanderSquadState, int>) (entry => entry.SquadId)))
        {
          CivCommanderSquadState squad = commanderSquadState;
          bool flag = player.SquadId == squad.SquadId;
          Button button1 = new Button();
          button1.Text = Loc.GetString("civ-gmap-hq-destination-squad", new (string, object)[3]
          {
            ("squad", (object) squad.SquadId),
            ("members", (object) squad.Members.Count),
            ("order", (object) CivCommanderWindow.GetOrderText(squad.Order))
          });
          ((Control) button1).HorizontalExpand = true;
          ((BaseButton) button1).Disabled = flag;
          Button button2 = button1;
          ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.MoveSelectedPlayerToSquad(squad.SquadId));
          ((Control) this._destinationList).AddChild((Control) button2);
        }
        if (this._state.Squads.Count != 0)
          return;
        ((Control) this._destinationList).AddChild(CivCommanderWindow.MakeInfoCard(Loc.GetString("civ-gmap-hq-no-squads")));
      }
    }
  }

  private void SelectSection(int squadId)
  {
    this._selectedSectionSquadId = new int?(squadId);
    this._selectedPlayerUserId = new NetUserId?();
    if (squadId > 0)
      this._system.SetCommanderSelectedSquad(squadId);
    this.NormalizeSelection(this._system.GetCommanderSelectedSquadId());
    this.Rebuild();
  }

  private void MoveSelectedPlayerToSquad(int squadId)
  {
    CivCommanderPlayerState player;
    if (!this.TryGetSelectedPlayer(out player))
      return;
    this._pendingFollowPlayerUserId = new NetUserId?(player.UserId);
    this._selectedSectionSquadId = new int?(squadId);
    this._selectedPlayerUserId = new NetUserId?(player.UserId);
    this._system.SetCommanderSelectedSquad(squadId);
    this._system.RequestCommanderMovePlayer(player.UserId, squadId);
    this.Rebuild();
  }

  private void MoveSelectedPlayerToReserve()
  {
    CivCommanderPlayerState player;
    if (!this.TryGetSelectedPlayer(out player))
      return;
    this._pendingFollowPlayerUserId = new NetUserId?(player.UserId);
    this._selectedSectionSquadId = new int?(0);
    this._selectedPlayerUserId = new NetUserId?(player.UserId);
    this._system.RequestCommanderMovePlayer(player.UserId, 0);
    this.Rebuild();
  }

  private void MoveSelectedPlayerToNewSquad()
  {
    CivCommanderPlayerState player;
    if (!this.TryGetSelectedPlayer(out player))
      return;
    this._pendingFollowPlayerUserId = new NetUserId?(player.UserId);
    this._system.RequestCommanderMovePlayer(player.UserId, 0, true);
  }

  private bool TryGetDisplayedSquad(out CivCommanderSquadState squad)
  {
    squad = (CivCommanderSquadState) null;
    if (this._state != null)
    {
      int? selectedSectionSquadId = this._selectedSectionSquadId;
      if (selectedSectionSquadId.HasValue)
      {
        int squadId = selectedSectionSquadId.GetValueOrDefault();
        if (squadId > 0)
        {
          squad = this._state.Squads.FirstOrDefault<CivCommanderSquadState>((Func<CivCommanderSquadState, bool>) (entry => entry.SquadId == squadId));
          return squad != null;
        }
      }
    }
    return false;
  }

  private IEnumerable<CivCommanderPlayerState> GetDisplayedPlayers()
  {
    if (this._state == null)
      return Enumerable.Empty<CivCommanderPlayerState>();
    int? selectedSectionSquadId = this._selectedSectionSquadId;
    if (selectedSectionSquadId.HasValue)
    {
      int squadId = selectedSectionSquadId.GetValueOrDefault();
      if (squadId > 0)
        return (IEnumerable<CivCommanderPlayerState>) this._state.Squads.FirstOrDefault<CivCommanderSquadState>((Func<CivCommanderSquadState, bool>) (entry => entry.SquadId == squadId))?.Members ?? Enumerable.Empty<CivCommanderPlayerState>();
    }
    return (IEnumerable<CivCommanderPlayerState>) this._state.ReservePlayers;
  }

  private bool TryGetSelectedPlayer(out CivCommanderPlayerState player)
  {
    player = (CivCommanderPlayerState) null;
    if (this._state != null)
    {
      NetUserId? selectedPlayerUserId = this._selectedPlayerUserId;
      if (selectedPlayerUserId.HasValue)
        return this.TryFindPlayer(selectedPlayerUserId.GetValueOrDefault(), out player);
    }
    return false;
  }

  private bool TryFindPlayer(NetUserId userId, out CivCommanderPlayerState player)
  {
    player = (CivCommanderPlayerState) null;
    if (this._state == null)
      return false;
    player = this._state.Squads.SelectMany<CivCommanderSquadState, CivCommanderPlayerState>((Func<CivCommanderSquadState, IEnumerable<CivCommanderPlayerState>>) (entry => (IEnumerable<CivCommanderPlayerState>) entry.Members)).Concat<CivCommanderPlayerState>((IEnumerable<CivCommanderPlayerState>) this._state.ReservePlayers).FirstOrDefault<CivCommanderPlayerState>((Func<CivCommanderPlayerState, bool>) (candidate => NetUserId.op_Equality(candidate.UserId, userId)));
    return player != null;
  }

  private bool HasSection(int squadId)
  {
    if (this._state == null)
      return false;
    return squadId == 0 || this._state.Squads.Any<CivCommanderSquadState>((Func<CivCommanderSquadState, bool>) (entry => entry.SquadId == squadId));
  }

  private static PanelContainer MakePanel(Color background, Color border, float borderWidth = 1f)
  {
    PanelContainer panelContainer = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = background;
    styleBoxFlat.BorderColor = border;
    styleBoxFlat.BorderThickness = new Thickness(borderWidth);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(10f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    return panelContainer;
  }

  private static Control MakeIconPanel(Control icon, Color accent)
  {
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).MinSize = new Vector2(72f, 72f);
    ((Control) panelContainer).MaxSize = new Vector2(72f, 72f);
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = ((Color) ref accent).WithAlpha(0.14f);
    styleBoxFlat.BorderColor = accent;
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(10f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(10f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) panelContainer).AddChild(icon);
    return (Control) panelContainer;
  }

  private static BoxContainer MakeVerticalBox(int separation)
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer.SeparationOverride = new int?(separation);
    ((Control) boxContainer).HorizontalExpand = true;
    ((Control) boxContainer).VerticalExpand = true;
    return boxContainer;
  }

  private static Control MakeInfoCard(string text)
  {
    PanelContainer panelContainer = CivCommanderWindow.MakePanel(Color.FromHex((ReadOnlySpan<char>) "#20242C", new Color?()), Color.FromHex((ReadOnlySpan<char>) "#37404B", new Color?()));
    Label label = new Label();
    label.Text = text;
    ((Control) label).HorizontalExpand = true;
    label.FontColorOverride = new Color?(Color.LightGray);
    ((Control) panelContainer).AddChild((Control) label);
    return (Control) panelContainer;
  }

  private static Color GetTeamAccent(int teamId)
  {
    return teamId != 2 ? Color.FromHex((ReadOnlySpan<char>) "#4D87D9", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#C24E4E", new Color?());
  }

  private static string GetOrderText(CivCommanderOrderType order)
  {
    string orderText;
    switch (order)
    {
      case CivCommanderOrderType.Attack:
        orderText = Loc.GetString("civ-gmap-hq-order-attack");
        break;
      case CivCommanderOrderType.Defense:
        orderText = Loc.GetString("civ-gmap-hq-order-defense");
        break;
      case CivCommanderOrderType.Artillery:
        orderText = Loc.GetString("civ-gmap-hq-order-artillery");
        break;
      default:
        orderText = Loc.GetString("civ-gmap-hq-order-none");
        break;
    }
    return orderText;
  }

  private static string BuildPlayerButtonText(CivCommanderPlayerState player)
  {
    if (!player.IsSquadLeader)
      return player.Name;
    return Loc.GetString("civ-gmap-hq-player-leader", new (string, object)[1]
    {
      ("name", (object) player.Name)
    });
  }
}
