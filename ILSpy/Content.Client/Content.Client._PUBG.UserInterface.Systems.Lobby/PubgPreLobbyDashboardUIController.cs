using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Content.Client._PUBG.UserInterface.Systems.Lobby;

public sealed class PubgPreLobbyDashboardUIController : UIController, IOnStateEntered<LobbyState>, IOnStateExited<LobbyState>
{
	private sealed record DashboardToastEntry(PanelContainer Panel, TimeSpan ExpiresAt);

	[Dependency]
	private IGameTiming _timing;

	private static readonly TimeSpan ToastLifetime = TimeSpan.FromSeconds(8L);

	private static readonly Color OnlineFriendColor = Color.FromHex((ReadOnlySpan<char>)"#65E477", (Color?)null);

	private static readonly Color PlaceholderColor = Color.FromHex((ReadOnlySpan<char>)"#7A7A7A", (Color?)null);

	private const int ToastWidth = 300;

	private const int MaxLobbyNameLength = 22;

	private const int MaxVisibleToasts = 1;

	private LobbyState? _lobbyState;

	private bool _friendsSubscribed;

	private readonly List<DashboardToastEntry> _toastEntries = new List<DashboardToastEntry>();

	private readonly Queue<PubgFriendRequestEntry> _queuedRequestToasts = new Queue<PubgFriendRequestEntry>();

	public void OnStateEntered(LobbyState state)
	{
		_lobbyState = state;
		EnsureFriendsSubscribed();
		InitializeOnlineFriendsPanel();
		PubgFriendsClientSystem pubgFriendsClientSystem = base.EntityManager.System<PubgFriendsClientSystem>();
		pubgFriendsClientSystem.ResetIncomingRequestBaseline();
		pubgFriendsClientSystem.RequestState();
		FlushQueuedToasts();
	}

	public void OnStateExited(LobbyState state)
	{
		ClearToasts();
		_lobbyState = null;
		UnsubscribeFriends();
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		if (_lobbyState != null)
		{
			FlushQueuedToasts();
			CleanupExpiredToasts();
		}
	}

	private void EnsureFriendsSubscribed()
	{
		if (!_friendsSubscribed)
		{
			PubgFriendsClientSystem pubgFriendsClientSystem = base.EntityManager.System<PubgFriendsClientSystem>();
			pubgFriendsClientSystem.StateUpdated += OnFriendsStateUpdated;
			pubgFriendsClientSystem.IncomingRequestAdded += OnIncomingRequestAdded;
			_friendsSubscribed = true;
		}
	}

	private void UnsubscribeFriends()
	{
		if (_friendsSubscribed)
		{
			PubgFriendsClientSystem pubgFriendsClientSystem = base.EntityManager.SystemOrNull<PubgFriendsClientSystem>();
			if (pubgFriendsClientSystem != null)
			{
				pubgFriendsClientSystem.StateUpdated -= OnFriendsStateUpdated;
				pubgFriendsClientSystem.IncomingRequestAdded -= OnIncomingRequestAdded;
			}
			_friendsSubscribed = false;
		}
	}

	private LobbyGui? GetLobbyGui()
	{
		return _lobbyState?.Lobby;
	}

	private void OnFriendsStateUpdated(IReadOnlyList<PubgFriendEntry> friends, IReadOnlyList<PubgFriendRequestEntry> requests, IReadOnlyList<PubgFriendOutgoingEntry> outgoing)
	{
		UpdateOnlineFriends(friends);
	}

	private void OnIncomingRequestAdded(PubgFriendRequestEntry request)
	{
		ShowRequestToast(request);
	}

	private void InitializeOnlineFriendsPanel()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		LobbyGui lobbyGui = GetLobbyGui();
		if (lobbyGui != null)
		{
			lobbyGui.LobbyOnlineFriendsTitleLabel.Text = Loc.GetString("pubg-lobby-online-friends-title-count", new(string, object)[1] { ("count", 0) });
			((Control)lobbyGui.LobbyOnlineFriendsList).RemoveAllChildren();
			((Control)lobbyGui.LobbyOnlineFriendsList).AddChild((Control)new Label
			{
				Text = Loc.GetString("pubg-lobby-online-friends-empty"),
				FontColorOverride = PlaceholderColor,
				ClipText = true
			});
		}
	}

	private void UpdateOnlineFriends(IReadOnlyList<PubgFriendEntry> friends)
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		LobbyGui lobbyGui = GetLobbyGui();
		if (lobbyGui == null)
		{
			return;
		}
		List<PubgFriendEntry> list = (from friend in friends
			where friend.Status != PubgFriendStatus.Offline
			orderby (friend.Status == PubgFriendStatus.InGame) ? 1 : 0, friend.Ckey
			select friend).ToList();
		lobbyGui.LobbyOnlineFriendsTitleLabel.Text = Loc.GetString("pubg-lobby-online-friends-title-count", new(string, object)[1] { ("count", list.Count) });
		((Control)lobbyGui.LobbyOnlineFriendsList).RemoveAllChildren();
		if (list.Count == 0)
		{
			((Control)lobbyGui.LobbyOnlineFriendsList).AddChild((Control)new Label
			{
				Text = Loc.GetString("pubg-lobby-online-friends-empty"),
				FontColorOverride = PlaceholderColor,
				ClipText = true
			});
			return;
		}
		foreach (PubgFriendEntry item3 in list)
		{
			string item = TruncateName(item3.Ckey);
			string item2 = item3.Status switch
			{
				PubgFriendStatus.Lobby => Loc.GetString("pubg-friends-status-lobby"), 
				PubgFriendStatus.InGame => Loc.GetString("pubg-friends-status-game"), 
				_ => Loc.GetString("pubg-friends-status-offline"), 
			};
			BoxContainer lobbyOnlineFriendsList = lobbyGui.LobbyOnlineFriendsList;
			Label val = new Label();
			val.Text = Loc.GetString("pubg-lobby-online-friends-row", new(string, object)[2]
			{
				("name", item),
				("status", item2)
			});
			val.FontColorOverride = OnlineFriendColor;
			val.ClipText = true;
			((Control)lobbyOnlineFriendsList).AddChild((Control)(object)val);
		}
	}

	private void ShowRequestToast(PubgFriendRequestEntry request)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Expected O, but got Unknown
		LobbyGui lobbyGui = GetLobbyGui();
		if (lobbyGui == null)
		{
			_queuedRequestToasts.Enqueue(request);
			return;
		}
		while (_toastEntries.Count >= 1)
		{
			RemoveToast(_toastEntries[0]);
		}
		PanelContainer val = new PanelContainer
		{
			HorizontalExpand = true,
			MinWidth = 300f,
			MaxWidth = 300f
		};
		val.PanelOverride = (StyleBox)new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#172033EE", (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)"#4AA3FF", (Color?)null),
			BorderThickness = new Thickness(1f, 1f, 1f, 1f),
			ContentMarginLeftOverride = 8f,
			ContentMarginRightOverride = 8f,
			ContentMarginTopOverride = 8f,
			ContentMarginBottomOverride = 8f
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 6
		};
		((Control)val2).AddChild((Control)new Label
		{
			Text = Loc.GetString("pubg-lobby-dashboard-toast-title"),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#D6E7FF", (Color?)null)
		});
		Label val3 = new Label();
		val3.Text = Loc.GetString("pubg-lobby-dashboard-toast-body", new(string, object)[1] { ("name", TruncateName(request.Ckey)) });
		val3.FontColorOverride = Color.White;
		val3.ClipText = true;
		((Control)val3).MaxWidth = 284f;
		((Control)val2).AddChild((Control)(object)val3);
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 6
		};
		Button val5 = new Button
		{
			Text = Loc.GetString("pubg-lobby-dashboard-toast-open"),
			MinWidth = 130f,
			HorizontalExpand = true
		};
		Button val6 = new Button
		{
			Text = Loc.GetString("pubg-lobby-dashboard-toast-close"),
			MinWidth = 90f
		};
		((Control)val4).AddChild((Control)(object)val5);
		((Control)val4).AddChild((Control)(object)val6);
		((Control)val2).AddChild((Control)(object)val4);
		((Control)val).AddChild((Control)(object)val2);
		((Control)lobbyGui.LobbyToastContainer).AddChild((Control)(object)val);
		DashboardToastEntry toast = new DashboardToastEntry(val, _timing.CurTime + ToastLifetime);
		_toastEntries.Add(toast);
		((BaseButton)val5).OnPressed += delegate
		{
			base.EntityManager.System<PubgFriendsClientSystem>().ToggleWindow();
			RemoveToast(toast);
		};
		((BaseButton)val6).OnPressed += delegate
		{
			RemoveToast(toast);
		};
	}

	private void FlushQueuedToasts()
	{
		LobbyGui lobbyGui = GetLobbyGui();
		if (_queuedRequestToasts.Count != 0 && lobbyGui != null)
		{
			while (_queuedRequestToasts.Count > 0)
			{
				ShowRequestToast(_queuedRequestToasts.Dequeue());
			}
		}
	}

	private void CleanupExpiredToasts()
	{
		if (_toastEntries.Count == 0)
		{
			return;
		}
		for (int num = _toastEntries.Count - 1; num >= 0; num--)
		{
			if (_timing.CurTime >= _toastEntries[num].ExpiresAt)
			{
				RemoveToast(_toastEntries[num]);
			}
		}
	}

	private void RemoveToast(DashboardToastEntry toast)
	{
		((Control)toast.Panel).Orphan();
		_toastEntries.Remove(toast);
	}

	private void ClearToasts()
	{
		foreach (DashboardToastEntry toastEntry in _toastEntries)
		{
			((Control)toastEntry.Panel).Orphan();
		}
		_toastEntries.Clear();
		_queuedRequestToasts.Clear();
		InitializeOnlineFriendsPanel();
	}

	private static string TruncateName(string name)
	{
		if (name.Length <= 22)
		{
			return name;
		}
		return name.Substring(0, 22) + "...";
	}
}
