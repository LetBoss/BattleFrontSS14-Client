// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Friends.PubgFriendsClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.Friends.UI;
using Content.Shared._PUBG.Friends;
using Content.Shared._PUBG.Party;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.Friends;

public sealed class PubgFriendsClientSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  private readonly List<PubgFriendEntry> _friends = new List<PubgFriendEntry>();
  private readonly List<PubgFriendRequestEntry> _requests = new List<PubgFriendRequestEntry>();
  private readonly List<PubgFriendOutgoingEntry> _outgoing = new List<PubgFriendOutgoingEntry>();
  private PubgFriendsWindow? _window;
  private readonly Dictionary<NetUserId, TimeSpan> _inviteCooldowns = new Dictionary<NetUserId, TimeSpan>();
  private TimeSpan _nextCooldownCheck;
  private bool _receivedInitialState;

  public event Action<int>? PendingRequestsUpdated;

  public event Action<IReadOnlyList<PubgFriendEntry>, IReadOnlyList<PubgFriendRequestEntry>, IReadOnlyList<PubgFriendOutgoingEntry>>? StateUpdated;

  public event Action<PubgFriendRequestEntry>? IncomingRequestAdded;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgFriendsStateEvent>(new EntitySessionEventHandler<PubgFriendsStateEvent>(this.OnState), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._window == null || !((BaseWindow) this._window).IsOpen || this._timing.CurTime < this._nextCooldownCheck)
      return;
    this._nextCooldownCheck = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
    if (!this._inviteCooldowns.Any<KeyValuePair<NetUserId, TimeSpan>>((Func<KeyValuePair<NetUserId, TimeSpan>, bool>) (p => p.Value > this._timing.CurTime)))
      return;
    foreach ((NetUserId netUserId, TimeSpan until) in this._inviteCooldowns)
      this._window.SetInviteCooldown(netUserId, until);
    this._window.UpdateFriends((IReadOnlyList<PubgFriendEntry>) this._friends, (IReadOnlyList<PubgFriendOutgoingEntry>) this._outgoing);
  }

  public void ToggleWindow()
  {
    this.EnsureWindow();
    if (((BaseWindow) this._window).IsOpen)
    {
      ((BaseWindow) this._window).Close();
    }
    else
    {
      ((BaseWindow) this._window).OpenCentered();
      this.RequestState();
    }
  }

  public void RequestState()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgFriendsRequestStateEvent());
  }

  public void ResetIncomingRequestBaseline() => this._receivedInitialState = false;

  private void EnsureWindow()
  {
    if (this._window != null)
      return;
    this._window = new PubgFriendsWindow();
    ((BaseWindow) this._window).OnClose += new Action(this.OnWindowClosed);
    this._window.AddFriendRequested += new Action<string>(this.OnAddFriendRequested);
    this._window.InviteRequested += new Action<NetUserId>(this.OnInviteRequested);
    this._window.RemoveRequested += new Action<NetUserId>(this.OnRemoveFriendRequested);
    this._window.RequestResponded += new Action<NetUserId, bool>(this.OnRequestResponded);
    this._window.CancelRequested += new Action<NetUserId>(this.OnCancelRequested);
    this._window.UpdateFriends((IReadOnlyList<PubgFriendEntry>) this._friends, (IReadOnlyList<PubgFriendOutgoingEntry>) this._outgoing);
    this._window.UpdateRequests((IReadOnlyList<PubgFriendRequestEntry>) this._requests);
  }

  private void OnWindowClosed()
  {
    if (this._window == null)
      return;
    this._window.AddFriendRequested -= new Action<string>(this.OnAddFriendRequested);
    this._window.InviteRequested -= new Action<NetUserId>(this.OnInviteRequested);
    this._window.RemoveRequested -= new Action<NetUserId>(this.OnRemoveFriendRequested);
    this._window.RequestResponded -= new Action<NetUserId, bool>(this.OnRequestResponded);
    this._window.CancelRequested -= new Action<NetUserId>(this.OnCancelRequested);
    ((BaseWindow) this._window).OnClose -= new Action(this.OnWindowClosed);
    this._window = (PubgFriendsWindow) null;
  }

  private void OnState(PubgFriendsStateEvent ev, EntitySessionEventArgs args)
  {
    HashSet<NetUserId> netUserIdSet = new HashSet<NetUserId>(this._requests.Select<PubgFriendRequestEntry, NetUserId>((Func<PubgFriendRequestEntry, NetUserId>) (request => request.UserId)));
    this._friends.Clear();
    this._friends.AddRange((IEnumerable<PubgFriendEntry>) ev.Friends);
    this._requests.Clear();
    this._requests.AddRange((IEnumerable<PubgFriendRequestEntry>) ev.Requests);
    this._outgoing.Clear();
    this._outgoing.AddRange((IEnumerable<PubgFriendOutgoingEntry>) ev.Outgoing);
    if (this._receivedInitialState)
    {
      foreach (PubgFriendRequestEntry request in this._requests)
      {
        if (!netUserIdSet.Contains(request.UserId))
        {
          Action<PubgFriendRequestEntry> incomingRequestAdded = this.IncomingRequestAdded;
          if (incomingRequestAdded != null)
            incomingRequestAdded(request);
        }
      }
    }
    else
      this._receivedInitialState = true;
    Action<int> pendingRequestsUpdated = this.PendingRequestsUpdated;
    if (pendingRequestsUpdated != null)
      pendingRequestsUpdated(this._requests.Count);
    Action<IReadOnlyList<PubgFriendEntry>, IReadOnlyList<PubgFriendRequestEntry>, IReadOnlyList<PubgFriendOutgoingEntry>> stateUpdated = this.StateUpdated;
    if (stateUpdated != null)
      stateUpdated((IReadOnlyList<PubgFriendEntry>) this._friends, (IReadOnlyList<PubgFriendRequestEntry>) this._requests, (IReadOnlyList<PubgFriendOutgoingEntry>) this._outgoing);
    if (this._window == null)
      return;
    foreach ((NetUserId netUserId, TimeSpan until) in this._inviteCooldowns)
      this._window.SetInviteCooldown(netUserId, until);
    this._window.UpdateFriends((IReadOnlyList<PubgFriendEntry>) this._friends, (IReadOnlyList<PubgFriendOutgoingEntry>) this._outgoing);
    this._window.UpdateRequests((IReadOnlyList<PubgFriendRequestEntry>) this._requests);
  }

  private void OnAddFriendRequested(string ckey)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgFriendAddRequestEvent(ckey));
  }

  private void OnInviteRequested(NetUserId targetId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPreLobbyPartyInviteRequestEvent(targetId));
    TimeSpan until = this._timing.CurTime + TimeSpan.FromSeconds(15L);
    this._inviteCooldowns[targetId] = until;
    this._window?.SetInviteCooldown(targetId, until);
    this._window?.UpdateFriends((IReadOnlyList<PubgFriendEntry>) this._friends, (IReadOnlyList<PubgFriendOutgoingEntry>) this._outgoing);
  }

  private void OnRemoveFriendRequested(NetUserId targetId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgFriendRemoveEvent(targetId));
  }

  private void OnRequestResponded(NetUserId requesterId, bool accepted)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgFriendRequestResponseEvent(requesterId, accepted));
  }

  private void OnCancelRequested(NetUserId targetId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgFriendCancelRequestEvent(targetId));
  }
}
