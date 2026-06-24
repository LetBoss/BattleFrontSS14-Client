using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client._PUBG.UserInterface.Systems.Party;
using Content.Shared._PUBG.Match;
using Content.Shared._PUBG.Party;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.Party;

public sealed class PubgPreLobbyPartyClientSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	private readonly List<PubgPreLobbyPartyMemberState> _members = new List<PubgPreLobbyPartyMemberState>();

	private readonly Dictionary<PubgMatchMode, PubgPreLobbyModeOverviewEntry> _modeOverview = new Dictionary<PubgMatchMode, PubgPreLobbyModeOverviewEntry>();

	private NetUserId? _leaderId;

	private PubgMatchMode? _selectedMode;

	private bool _preferFullSquad;

	private PubgPartyInviteWindow? _inviteWindow;

	private bool _inviteHandled;

	private bool _inviteOpen;

	private bool _inviteExpired;

	private int _lastTimerSeconds;

	private TimeSpan? _inviteExpiresAt;

	private TimeSpan? _inviteCloseAt;

	public IReadOnlyList<PubgPreLobbyPartyMemberState> Members => _members;

	public NetUserId? LeaderId => _leaderId;

	public PubgMatchMode? SelectedMode => _selectedMode;

	public bool PreferFullSquad => _preferFullSquad;

	public bool AllReady
	{
		get
		{
			if (_members.Count > 1)
			{
				if (_members.All((PubgPreLobbyPartyMemberState m) => m.InPreLobby))
				{
					return _members.All(delegate(PubgPreLobbyPartyMemberState m)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						//IL_001a: Unknown result type (might be due to invalid IL or missing references)
						//IL_001d: Unknown result type (might be due to invalid IL or missing references)
						NetUserId userId = m.UserId;
						NetUserId? leaderId = _leaderId;
						return (leaderId.HasValue && userId == leaderId.GetValueOrDefault()) || m.IsReady;
					});
				}
				return false;
			}
			return true;
		}
	}

	public event Action? PartyStateUpdated;

	public event Action? ModeOverviewUpdated;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgPreLobbyPartyStateEvent>((EntitySessionEventHandler<PubgPreLobbyPartyStateEvent>)OnPartyState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgPreLobbyPartyInvitePromptEvent>((EntitySessionEventHandler<PubgPreLobbyPartyInvitePromptEvent>)OnInvitePrompt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgPreLobbyModeOverviewEvent>((EntitySessionEventHandler<PubgPreLobbyModeOverviewEvent>)OnModeOverview, (Type[])null, (Type[])null);
	}

	private void OnPartyState(PubgPreLobbyPartyStateEvent ev, EntitySessionEventArgs args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		_members.Clear();
		_members.AddRange(ev.Members);
		_leaderId = ((ev.Members.Count == 0) ? ((NetUserId?)null) : new NetUserId?(ev.LeaderId));
		_selectedMode = ev.SelectedMode;
		_preferFullSquad = ev.PreferFullSquad;
		this.PartyStateUpdated?.Invoke();
	}

	private void OnModeOverview(PubgPreLobbyModeOverviewEvent ev, EntitySessionEventArgs args)
	{
		_modeOverview.Clear();
		foreach (PubgPreLobbyModeOverviewEntry entry in ev.Entries)
		{
			_modeOverview[entry.Mode] = entry;
		}
		this.ModeOverviewUpdated?.Invoke();
	}

	public void SendModeSelection(PubgMatchMode? selectedMode, bool preferFullSquad)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPreLobbyPartyModeSelectEvent(selectedMode, preferFullSquad));
	}

	public void SendReadyToggle(bool isReady)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPreLobbyPartyReadyToggleEvent(isReady));
	}

	public void SendLeaveRequest()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPreLobbyPartyLeaveRequestEvent());
	}

	public void RequestState()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPreLobbyPartyStateRequestEvent());
	}

	public void RequestModeOverview()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPreLobbyModeOverviewRequestEvent());
	}

	public PubgPreLobbyModeOverviewEntry? GetModeOverview(PubgMatchMode mode)
	{
		return _modeOverview.GetValueOrDefault(mode);
	}

	private void OnInvitePrompt(PubgPreLobbyPartyInvitePromptEvent ev, EntitySessionEventArgs args)
	{
		if (_inviteOpen)
		{
			RespondInvite(accepted: false);
			return;
		}
		if (_inviteWindow == null)
		{
			_inviteWindow = new PubgPartyInviteWindow();
		}
		_inviteWindow.SetInviter(ev.InviterName);
		((BaseButton)_inviteWindow.AcceptButton).Disabled = false;
		((BaseButton)_inviteWindow.DeclineButton).Disabled = false;
		_inviteHandled = false;
		_inviteOpen = true;
		_inviteExpired = false;
		_inviteCloseAt = null;
		_inviteExpiresAt = _timing.CurTime + TimeSpan.FromSeconds(ev.TimeoutSeconds);
		_lastTimerSeconds = ev.TimeoutSeconds;
		_inviteWindow.SetTimerSeconds(_lastTimerSeconds);
		((BaseButton)_inviteWindow.AcceptButton).OnPressed += OnInviteAcceptPressed;
		((BaseButton)_inviteWindow.DeclineButton).OnPressed += OnInviteDeclinePressed;
		((BaseWindow)_inviteWindow).OnClose += OnInviteClosed;
		((BaseWindow)_inviteWindow).OpenCentered();
	}

	private void OnInviteAcceptPressed(ButtonEventArgs args)
	{
		SendInviteResponse(accepted: true);
	}

	private void OnInviteDeclinePressed(ButtonEventArgs args)
	{
		SendInviteResponse(accepted: false);
	}

	private void OnInviteClosed()
	{
		if (!_inviteHandled)
		{
			SendInviteResponse(accepted: false);
		}
	}

	private void SendInviteResponse(bool accepted)
	{
		if (!_inviteHandled)
		{
			_inviteHandled = true;
			_inviteOpen = false;
			RespondInvite(accepted);
			if (_inviteWindow != null)
			{
				((BaseButton)_inviteWindow.AcceptButton).OnPressed -= OnInviteAcceptPressed;
				((BaseButton)_inviteWindow.DeclineButton).OnPressed -= OnInviteDeclinePressed;
				((BaseWindow)_inviteWindow).OnClose -= OnInviteClosed;
				((BaseWindow)_inviteWindow).Close();
			}
			_inviteExpiresAt = null;
			_inviteCloseAt = null;
			_inviteExpired = false;
		}
	}

	private void CloseInviteWindowImmediate()
	{
		if (_inviteWindow != null)
		{
			((BaseButton)_inviteWindow.AcceptButton).OnPressed -= OnInviteAcceptPressed;
			((BaseButton)_inviteWindow.DeclineButton).OnPressed -= OnInviteDeclinePressed;
			((BaseWindow)_inviteWindow).OnClose -= OnInviteClosed;
			((BaseWindow)_inviteWindow).Close();
		}
		_inviteOpen = false;
		_inviteHandled = true;
		_inviteExpiresAt = null;
		_inviteCloseAt = null;
		_inviteExpired = false;
	}

	private void RespondInvite(bool accepted)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPreLobbyPartyInviteResponseEvent(accepted));
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (_inviteCloseAt.HasValue && _timing.CurTime >= _inviteCloseAt.Value)
		{
			CloseInviteWindowImmediate();
		}
		else
		{
			if (!_inviteOpen || _inviteWindow == null || !_inviteExpiresAt.HasValue)
			{
				return;
			}
			TimeSpan timeSpan = _inviteExpiresAt.Value - _timing.CurTime;
			if (timeSpan <= TimeSpan.Zero)
			{
				ExpireInvite();
				return;
			}
			int num = (int)Math.Ceiling(timeSpan.TotalSeconds);
			if (num != _lastTimerSeconds)
			{
				_lastTimerSeconds = num;
				_inviteWindow.SetTimerSeconds(num);
			}
		}
	}

	private void ExpireInvite()
	{
		if (!_inviteExpired)
		{
			_inviteExpired = true;
			_inviteOpen = false;
			_inviteHandled = true;
			_inviteExpiresAt = null;
			RespondInvite(accepted: false);
			if (_inviteWindow != null)
			{
				((BaseButton)_inviteWindow.AcceptButton).Disabled = true;
				((BaseButton)_inviteWindow.DeclineButton).Disabled = true;
				_inviteWindow.SetExpired();
			}
			_inviteCloseAt = _timing.CurTime + TimeSpan.FromSeconds(1.5);
		}
	}
}
