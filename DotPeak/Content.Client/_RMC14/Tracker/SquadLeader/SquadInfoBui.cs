// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Tracker.SquadLeader.SquadInfoBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Tracker.SquadLeader;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Tracker.SquadLeader;

public sealed class SquadInfoBui : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _prototype;
  private SquadInfoWindow? _window;
  private readonly SpriteSystem _sprite;

  public SquadInfoBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<SquadInfoBui>(this);
    this._sprite = this.EntMan.System<SpriteSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<SquadInfoWindow>((BoundUserInterface) this);
    this.Refresh();
  }

  public void Refresh()
  {
    SquadInfoWindow window = this._window;
    SquadLeaderTrackerComponent trackerComponent;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<SquadLeaderTrackerComponent>(this.Owner, ref trackerComponent))
      return;
    Texture background = (Texture) null;
    Color backgroundColor = new Color();
    SquadMemberComponent squadMemberComponent;
    if (this.EntMan.TryGetComponent<SquadMemberComponent>(this.Owner, ref squadMemberComponent))
    {
      background = this._sprite.Frame0(squadMemberComponent.Background);
      backgroundColor = squadMemberComponent.BackgroundColor;
    }
    bool flag = this.EntMan.HasComponent<SquadLeaderComponent>(this.Owner);
    string str1;
    if (trackerComponent.Fireteams.SquadLeader != null)
      str1 = Loc.GetString("rmc-squad-info-squad-leader-name", new (string, object)[1]
      {
        ("leader", (object) trackerComponent.Fireteams.SquadLeader)
      });
    else
      str1 = Loc.GetString("rmc-squad-info-squad-leader-none");
    this._window.SquadLeaderLabel.Text = str1;
    ((BaseButton) this._window.ChangeTrackerButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new SquadLeaderTrackerChangeTrackedMsg()));
    ((Control) this._window.FireteamsContainer).DisposeAllChildren();
    SquadLeaderTrackerMarine valueOrDefault3;
    NetEntity id3;
    for (int index = 0; index < trackerComponent.Fireteams.Fireteams.Length; ++index)
    {
      SquadLeaderTrackerFireteam fireteam = trackerComponent.Fireteams.Fireteams[index];
      Dictionary<NetEntity, SquadLeaderTrackerMarine> members = fireteam?.Members;
      if (members != null && members.Count > 0)
      {
        SquadFireteamContainer fireteamContainer = new SquadFireteamContainer();
        string str2;
        if (fireteam.Leader.HasValue)
        {
          (string, object)[] valueTupleArray = new (string, object)[1];
          valueOrDefault3 = fireteam.Leader.Value;
          valueTupleArray[0] = ("leader", (object) valueOrDefault3.Name);
          str2 = Loc.GetString("rmc-squad-info-team-leader-name", valueTupleArray);
        }
        else
          str2 = Loc.GetString("rmc-squad-info-team-leader-none");
        string str3 = str2;
        fireteamContainer.LeaderLabel.Text = str3;
        int fireatemIndex = index;
        ((BaseButton) fireteamContainer.RemoveLeaderButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new SquadLeaderTrackerDemoteFireteamLeaderMsg(fireatemIndex)));
        ((Control) fireteamContainer.RemoveLeaderButton).Visible = fireteam.Leader.HasValue & flag;
        fireteamContainer.FireteamLabel.Text = Loc.GetString("rmc-squad-info-fireteam", new (string, object)[1]
        {
          ("fireteam", (object) (index + 1))
        });
        foreach ((id3, valueOrDefault3) in fireteam.Members)
        {
          SquadLeaderTrackerMarine member = valueOrDefault3;
          id3 = member.Id;
          ref SquadLeaderTrackerMarine? local = ref fireteam.Leader;
          NetEntity? nullable1;
          if (!local.HasValue)
          {
            nullable1 = new NetEntity?();
          }
          else
          {
            valueOrDefault3 = local.GetValueOrDefault();
            nullable1 = new NetEntity?(valueOrDefault3.Id);
          }
          NetEntity? nullable2 = nullable1;
          if ((nullable2.HasValue ? (NetEntity.op_Equality(id3, nullable2.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
          {
            SquadInfoRow row = this.CreateRow(member, background, backgroundColor);
            ((Control) fireteamContainer.MembersContainer).AddChild((Control) row);
            Button button1 = new Button();
            ((Control) button1).MaxWidth = 25f;
            ((Control) button1).MaxHeight = 25f;
            ((Control) button1).VerticalAlignment = (Control.VAlignment) 2;
            ((Control) button1).StyleClasses.Add("OpenBoth");
            button1.Text = "^";
            button1.TextAlign = (Label.AlignMode) 1;
            ((Control) button1).ToolTip = Loc.GetString("rmc-squad-info-promote-team-leader");
            ((Control) button1).Margin = new Thickness(0.0f, 0.0f, 2f, 0.0f);
            Button button2 = button1;
            ((Control) button2).Visible = flag;
            ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new SquadLeaderTrackerPromoteFireteamLeaderMsg(member.Id)));
            ((Control) row.ActionsContainer).AddChild((Control) button2);
            Button button3 = new Button();
            ((Control) button3).MaxWidth = 25f;
            ((Control) button3).MaxHeight = 25f;
            ((Control) button3).VerticalAlignment = (Control.VAlignment) 2;
            ((Control) button3).StyleClasses.Add("OpenBoth");
            button3.Text = "x";
            button3.TextAlign = (Label.AlignMode) 1;
            ((Control) button3).ToolTip = Loc.GetString("rmc-squad-info-unassign-fireteam");
            ((Control) button3).Margin = new Thickness(0.0f, 0.0f, 2f, 0.0f);
            Button button4 = button3;
            ((Control) button4).Visible = flag;
            ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new SquadLeaderTrackerUnassignFireteamMsg(member.Id)));
            ((Control) row.ActionsContainer).AddChild((Control) button4);
          }
        }
        ((Control) this._window.FireteamsContainer).AddChild((Control) fireteamContainer);
      }
    }
    SquadFireteamContainer fireteamContainer1 = new SquadFireteamContainer();
    ((Control) fireteamContainer1.LeaderContainer).Visible = false;
    ((Control) fireteamContainer1.RemoveLeaderButton).Visible = false;
    fireteamContainer1.FireteamLabel.Text = Loc.GetString("rmc-squad-info-unassigned");
    fireteamContainer1.ActionsLabel.Text = Loc.GetString("rmc-squad-info-actions");
    foreach ((id3, valueOrDefault3) in trackerComponent.Fireteams.Unassigned)
    {
      SquadLeaderTrackerMarine unassigned = valueOrDefault3;
      NetEntity? squadLeaderId = trackerComponent.Fireteams.SquadLeaderId;
      id3 = unassigned.Id;
      if ((squadLeaderId.HasValue ? (NetEntity.op_Equality(squadLeaderId.GetValueOrDefault(), id3) ? 1 : 0) : 0) == 0)
      {
        SquadInfoRow row = this.CreateRow(unassigned, background, backgroundColor);
        ((Control) fireteamContainer1.MembersContainer).AddChild((Control) row);
        for (int index = 0; index < trackerComponent.Fireteams.Fireteams.Length; ++index)
        {
          Button button5 = new Button();
          ((Control) button5).MaxWidth = 25f;
          ((Control) button5).MaxHeight = 25f;
          ((Control) button5).VerticalAlignment = (Control.VAlignment) 1;
          ((Control) button5).StyleClasses.Add("OpenBoth");
          button5.Text = $"{index + 1}";
          Button button6 = button5;
          int fireteamIndex = index;
          ((Control) button6).Visible = flag;
          ((BaseButton) button6).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new SquadLeaderTrackerAssignFireteamMsg(unassigned.Id, fireteamIndex)));
          ((Control) row.ActionsContainer).AddChild((Control) button6);
        }
      }
    }
    ((Control) this._window.FireteamsContainer).AddChild((Control) fireteamContainer1);
  }

  private SquadInfoRow CreateRow(
    SquadLeaderTrackerMarine member,
    Texture? background,
    Color backgroundColor)
  {
    SquadInfoRow row = new SquadInfoRow();
    ProtoId<JobPrototype>? role = member.Role;
    JobPrototype jobPrototype;
    if (role.HasValue && this._prototype.TryIndex<JobPrototype>(role.GetValueOrDefault(), ref jobPrototype))
    {
      if (member.IconOverride != null)
      {
        row.RoleIcon.Texture = this._sprite.Frame0((SpriteSpecifier) member.IconOverride);
      }
      else
      {
        JobIconPrototype jobIconPrototype;
        if (this._prototype.TryIndex<JobIconPrototype>(jobPrototype.Icon, ref jobIconPrototype))
          row.RoleIcon.Texture = this._sprite.Frame0(jobIconPrototype.Icon);
      }
      row.RoleBackground.Texture = background;
      ((Control) row.RoleBackground).ModulateSelfOverride = new Color?(backgroundColor);
    }
    row.NameLabel.Text = $"[bold]{FormattedMessage.EscapeText(member.Name)}[/bold]";
    return row;
  }
}
