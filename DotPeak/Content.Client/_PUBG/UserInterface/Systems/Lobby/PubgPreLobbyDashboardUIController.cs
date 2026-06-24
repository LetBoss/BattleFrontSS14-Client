// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Lobby.PubgPreLobbyDashboardUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.Friends;
using Content.Client.Lobby;
using Content.Client.Lobby.UI;
using Content.Shared._PUBG.Friends;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Lobby;

public sealed class PubgPreLobbyDashboardUIController : 
  UIController,
  IOnStateEntered<LobbyState>,
  IOnStateExited<LobbyState>
{
  [Dependency]
  private IGameTiming _timing;
  private static readonly TimeSpan ToastLifetime = TimeSpan.FromSeconds(8L);
  private static readonly Color OnlineFriendColor = Color.FromHex((ReadOnlySpan<char>) "#65E477", new Color?());
  private static readonly Color PlaceholderColor = Color.FromHex((ReadOnlySpan<char>) "#7A7A7A", new Color?());
  private const int ToastWidth = 300;
  private const int MaxLobbyNameLength = 22;
  private const int MaxVisibleToasts = 1;
  private LobbyState? _lobbyState;
  private bool _friendsSubscribed;
  private readonly List<PubgPreLobbyDashboardUIController.DashboardToastEntry> _toastEntries = new List<PubgPreLobbyDashboardUIController.DashboardToastEntry>();
  private readonly Queue<PubgFriendRequestEntry> _queuedRequestToasts = new Queue<PubgFriendRequestEntry>();

  public void OnStateEntered(LobbyState state)
  {
    this._lobbyState = state;
    this.EnsureFriendsSubscribed();
    this.InitializeOnlineFriendsPanel();
    PubgFriendsClientSystem friendsClientSystem = this.EntityManager.System<PubgFriendsClientSystem>();
    friendsClientSystem.ResetIncomingRequestBaseline();
    friendsClientSystem.RequestState();
    this.FlushQueuedToasts();
  }

  public void OnStateExited(LobbyState state)
  {
    this.ClearToasts();
    this._lobbyState = (LobbyState) null;
    this.UnsubscribeFriends();
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (this._lobbyState == null)
      return;
    this.FlushQueuedToasts();
    this.CleanupExpiredToasts();
  }

  private void EnsureFriendsSubscribed()
  {
    if (this._friendsSubscribed)
      return;
    PubgFriendsClientSystem friendsClientSystem = this.EntityManager.System<PubgFriendsClientSystem>();
    friendsClientSystem.StateUpdated += new Action<IReadOnlyList<PubgFriendEntry>, IReadOnlyList<PubgFriendRequestEntry>, IReadOnlyList<PubgFriendOutgoingEntry>>(this.OnFriendsStateUpdated);
    friendsClientSystem.IncomingRequestAdded += new Action<PubgFriendRequestEntry>(this.OnIncomingRequestAdded);
    this._friendsSubscribed = true;
  }

  private void UnsubscribeFriends()
  {
    if (!this._friendsSubscribed)
      return;
    PubgFriendsClientSystem friendsClientSystem = this.EntityManager.SystemOrNull<PubgFriendsClientSystem>();
    if (friendsClientSystem != null)
    {
      friendsClientSystem.StateUpdated -= new Action<IReadOnlyList<PubgFriendEntry>, IReadOnlyList<PubgFriendRequestEntry>, IReadOnlyList<PubgFriendOutgoingEntry>>(this.OnFriendsStateUpdated);
      friendsClientSystem.IncomingRequestAdded -= new Action<PubgFriendRequestEntry>(this.OnIncomingRequestAdded);
    }
    this._friendsSubscribed = false;
  }

  private LobbyGui? GetLobbyGui() => this._lobbyState?.Lobby;

  private void OnFriendsStateUpdated(
    IReadOnlyList<PubgFriendEntry> friends,
    IReadOnlyList<PubgFriendRequestEntry> requests,
    IReadOnlyList<PubgFriendOutgoingEntry> outgoing)
  {
    this.UpdateOnlineFriends(friends);
  }

  private void OnIncomingRequestAdded(PubgFriendRequestEntry request)
  {
    this.ShowRequestToast(request);
  }

  private void InitializeOnlineFriendsPanel()
  {
    LobbyGui lobbyGui = this.GetLobbyGui();
    if (lobbyGui == null)
      return;
    lobbyGui.LobbyOnlineFriendsTitleLabel.Text = Loc.GetString("pubg-lobby-online-friends-title-count", new (string, object)[1]
    {
      ("count", (object) 0)
    });
    ((Control) lobbyGui.LobbyOnlineFriendsList).RemoveAllChildren();
    ((Control) lobbyGui.LobbyOnlineFriendsList).AddChild((Control) new Label()
    {
      Text = Loc.GetString("pubg-lobby-online-friends-empty"),
      FontColorOverride = new Color?(PubgPreLobbyDashboardUIController.PlaceholderColor),
      ClipText = true
    });
  }

  private void UpdateOnlineFriends(IReadOnlyList<PubgFriendEntry> friends)
  {
    LobbyGui lobbyGui = this.GetLobbyGui();
    if (lobbyGui == null)
      return;
    List<PubgFriendEntry> list = friends.Where<PubgFriendEntry>((Func<PubgFriendEntry, bool>) (friend => friend.Status != 0)).OrderBy<PubgFriendEntry, int>((Func<PubgFriendEntry, int>) (friend => friend.Status == PubgFriendStatus.InGame ? 1 : 0)).ThenBy<PubgFriendEntry, string>((Func<PubgFriendEntry, string>) (friend => friend.Ckey)).ToList<PubgFriendEntry>();
    lobbyGui.LobbyOnlineFriendsTitleLabel.Text = Loc.GetString("pubg-lobby-online-friends-title-count", new (string, object)[1]
    {
      ("count", (object) list.Count)
    });
    ((Control) lobbyGui.LobbyOnlineFriendsList).RemoveAllChildren();
    if (list.Count == 0)
    {
      ((Control) lobbyGui.LobbyOnlineFriendsList).AddChild((Control) new Label()
      {
        Text = Loc.GetString("pubg-lobby-online-friends-empty"),
        FontColorOverride = new Color?(PubgPreLobbyDashboardUIController.PlaceholderColor),
        ClipText = true
      });
    }
    else
    {
      foreach (PubgFriendEntry pubgFriendEntry in list)
      {
        string str1 = PubgPreLobbyDashboardUIController.TruncateName(pubgFriendEntry.Ckey);
        string str2;
        switch (pubgFriendEntry.Status)
        {
          case PubgFriendStatus.Lobby:
            str2 = Loc.GetString("pubg-friends-status-lobby");
            break;
          case PubgFriendStatus.InGame:
            str2 = Loc.GetString("pubg-friends-status-game");
            break;
          default:
            str2 = Loc.GetString("pubg-friends-status-offline");
            break;
        }
        string str3 = str2;
        ((Control) lobbyGui.LobbyOnlineFriendsList).AddChild((Control) new Label()
        {
          Text = Loc.GetString("pubg-lobby-online-friends-row", new (string, object)[2]
          {
            ("name", (object) str1),
            ("status", (object) str3)
          }),
          FontColorOverride = new Color?(PubgPreLobbyDashboardUIController.OnlineFriendColor),
          ClipText = true
        });
      }
    }
  }

  private void ShowRequestToast(PubgFriendRequestEntry request)
  {
    LobbyGui lobbyGui = this.GetLobbyGui();
    if (lobbyGui == null)
    {
      this._queuedRequestToasts.Enqueue(request);
    }
    else
    {
      while (this._toastEntries.Count >= 1)
        this.RemoveToast(this._toastEntries[0]);
      PanelContainer panelContainer1 = new PanelContainer();
      ((Control) panelContainer1).HorizontalExpand = true;
      ((Control) panelContainer1).MinWidth = 300f;
      ((Control) panelContainer1).MaxWidth = 300f;
      PanelContainer Panel = panelContainer1;
      PanelContainer panelContainer2 = Panel;
      StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
      styleBoxFlat.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#172033EE", new Color?());
      styleBoxFlat.BorderColor = Color.FromHex((ReadOnlySpan<char>) "#4AA3FF", new Color?());
      styleBoxFlat.BorderThickness = new Thickness(1f, 1f, 1f, 1f);
      ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(8f);
      ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(8f);
      ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(8f);
      ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(8f);
      panelContainer2.PanelOverride = (StyleBox) styleBoxFlat;
      BoxContainer boxContainer1 = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 1,
        SeparationOverride = new int?(6)
      };
      ((Control) boxContainer1).AddChild((Control) new Label()
      {
        Text = Loc.GetString("pubg-lobby-dashboard-toast-title"),
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#D6E7FF", new Color?()))
      });
      BoxContainer boxContainer2 = boxContainer1;
      Label label1 = new Label();
      label1.Text = Loc.GetString("pubg-lobby-dashboard-toast-body", new (string, object)[1]
      {
        ("name", (object) PubgPreLobbyDashboardUIController.TruncateName(request.Ckey))
      });
      label1.FontColorOverride = new Color?(Color.White);
      label1.ClipText = true;
      ((Control) label1).MaxWidth = 284f;
      Label label2 = label1;
      ((Control) boxContainer2).AddChild((Control) label2);
      BoxContainer boxContainer3 = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 0,
        SeparationOverride = new int?(6)
      };
      Button button1 = new Button();
      button1.Text = Loc.GetString("pubg-lobby-dashboard-toast-open");
      ((Control) button1).MinWidth = 130f;
      ((Control) button1).HorizontalExpand = true;
      Button button2 = button1;
      Button button3 = new Button();
      button3.Text = Loc.GetString("pubg-lobby-dashboard-toast-close");
      ((Control) button3).MinWidth = 90f;
      Button button4 = button3;
      ((Control) boxContainer3).AddChild((Control) button2);
      ((Control) boxContainer3).AddChild((Control) button4);
      ((Control) boxContainer1).AddChild((Control) boxContainer3);
      ((Control) Panel).AddChild((Control) boxContainer1);
      ((Control) lobbyGui.LobbyToastContainer).AddChild((Control) Panel);
      PubgPreLobbyDashboardUIController.DashboardToastEntry toast = new PubgPreLobbyDashboardUIController.DashboardToastEntry(Panel, this._timing.CurTime + PubgPreLobbyDashboardUIController.ToastLifetime);
      this._toastEntries.Add(toast);
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this.EntityManager.System<PubgFriendsClientSystem>().ToggleWindow();
        this.RemoveToast(toast);
      });
      ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.RemoveToast(toast));
    }
  }

  private void FlushQueuedToasts()
  {
    LobbyGui lobbyGui = this.GetLobbyGui();
    if (this._queuedRequestToasts.Count == 0 || lobbyGui == null)
      return;
    while (this._queuedRequestToasts.Count > 0)
      this.ShowRequestToast(this._queuedRequestToasts.Dequeue());
  }

  private void CleanupExpiredToasts()
  {
    if (this._toastEntries.Count == 0)
      return;
    for (int index = this._toastEntries.Count - 1; index >= 0; --index)
    {
      if (this._timing.CurTime >= this._toastEntries[index].ExpiresAt)
        this.RemoveToast(this._toastEntries[index]);
    }
  }

  private void RemoveToast(
    PubgPreLobbyDashboardUIController.DashboardToastEntry toast)
  {
    ((Control) toast.Panel).Orphan();
    this._toastEntries.Remove(toast);
  }

  private void ClearToasts()
  {
    foreach (PubgPreLobbyDashboardUIController.DashboardToastEntry toastEntry in this._toastEntries)
      ((Control) toastEntry.Panel).Orphan();
    this._toastEntries.Clear();
    this._queuedRequestToasts.Clear();
    this.InitializeOnlineFriendsPanel();
  }

  private static string TruncateName(string name)
  {
    return name.Length <= 22 ? name : name.Substring(0, 22) + "...";
  }

  private sealed record DashboardToastEntry(PanelContainer Panel, TimeSpan ExpiresAt);
}
