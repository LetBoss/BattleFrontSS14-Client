using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client._PUBG.Friends.UI;
using Content.Shared._PUBG.Friends;
using Content.Shared._PUBG.Party;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

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

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgFriendsStateEvent>((EntitySessionEventHandler<PubgFriendsStateEvent>)OnState, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_window == null || !((BaseWindow)_window).IsOpen || _timing.CurTime < _nextCooldownCheck)
		{
			return;
		}
		_nextCooldownCheck = _timing.CurTime + TimeSpan.FromSeconds(0.5);
		if (!_inviteCooldowns.Any((KeyValuePair<NetUserId, TimeSpan> p) => p.Value > _timing.CurTime))
		{
			return;
		}
		foreach (var (userId, until) in _inviteCooldowns)
		{
			_window.SetInviteCooldown(userId, until);
		}
		_window.UpdateFriends(_friends, _outgoing);
	}

	public void ToggleWindow()
	{
		EnsureWindow();
		if (((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).Close();
			return;
		}
		((BaseWindow)_window).OpenCentered();
		RequestState();
	}

	public void RequestState()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgFriendsRequestStateEvent());
	}

	public void ResetIncomingRequestBaseline()
	{
		_receivedInitialState = false;
	}

	private void EnsureWindow()
	{
		if (_window == null)
		{
			_window = new PubgFriendsWindow();
			((BaseWindow)_window).OnClose += OnWindowClosed;
			_window.AddFriendRequested += OnAddFriendRequested;
			_window.InviteRequested += OnInviteRequested;
			_window.RemoveRequested += OnRemoveFriendRequested;
			_window.RequestResponded += OnRequestResponded;
			_window.CancelRequested += OnCancelRequested;
			_window.UpdateFriends(_friends, _outgoing);
			_window.UpdateRequests(_requests);
		}
	}

	private void OnWindowClosed()
	{
		if (_window != null)
		{
			_window.AddFriendRequested -= OnAddFriendRequested;
			_window.InviteRequested -= OnInviteRequested;
			_window.RemoveRequested -= OnRemoveFriendRequested;
			_window.RequestResponded -= OnRequestResponded;
			_window.CancelRequested -= OnCancelRequested;
			((BaseWindow)_window).OnClose -= OnWindowClosed;
			_window = null;
		}
	}

	private void OnState(PubgFriendsStateEvent ev, EntitySessionEventArgs args)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		HashSet<NetUserId> hashSet = new HashSet<NetUserId>(_requests.Select((PubgFriendRequestEntry request) => request.UserId));
		_friends.Clear();
		_friends.AddRange(ev.Friends);
		_requests.Clear();
		_requests.AddRange(ev.Requests);
		_outgoing.Clear();
		_outgoing.AddRange(ev.Outgoing);
		if (_receivedInitialState)
		{
			foreach (PubgFriendRequestEntry request in _requests)
			{
				if (!hashSet.Contains(request.UserId))
				{
					this.IncomingRequestAdded?.Invoke(request);
				}
			}
		}
		else
		{
			_receivedInitialState = true;
		}
		this.PendingRequestsUpdated?.Invoke(_requests.Count);
		this.StateUpdated?.Invoke(_friends, _requests, _outgoing);
		if (_window == null)
		{
			return;
		}
		foreach (var (userId, until) in _inviteCooldowns)
		{
			_window.SetInviteCooldown(userId, until);
		}
		_window.UpdateFriends(_friends, _outgoing);
		_window.UpdateRequests(_requests);
	}

	private void OnAddFriendRequested(string ckey)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgFriendAddRequestEvent(ckey));
	}

	private void OnInviteRequested(NetUserId targetId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPreLobbyPartyInviteRequestEvent(targetId));
		TimeSpan timeSpan = _timing.CurTime + TimeSpan.FromSeconds(15L);
		_inviteCooldowns[targetId] = timeSpan;
		_window?.SetInviteCooldown(targetId, timeSpan);
		_window?.UpdateFriends(_friends, _outgoing);
	}

	private void OnRemoveFriendRequested(NetUserId targetId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgFriendRemoveEvent(targetId));
	}

	private void OnRequestResponded(NetUserId requesterId, bool accepted)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgFriendRequestResponseEvent(requesterId, accepted));
	}

	private void OnCancelRequested(NetUserId targetId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgFriendCancelRequestEvent(targetId));
	}
}
