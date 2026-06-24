using System;
using System.Collections.Generic;
using Content.Client._PUBG.UserInterface.Systems.Party;
using Content.Shared._PUBG;
using Content.Shared._PUBG.Party;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.Party;

public sealed class PubgPartyClientSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IGameTiming _timing;

	private readonly List<PubgPartyMemberState> _members = new List<PubgPartyMemberState>();

	private int _teamSize = 1;

	private string? _localTeamTag;

	private PubgPartyInviteWindow? _inviteWindow;

	private bool _inviteHandled;

	private bool _inviteOpen;

	private bool _inviteExpired;

	private int _lastTimerSeconds;

	private TimeSpan? _inviteExpiresAt;

	private TimeSpan? _inviteCloseAt;

	public IReadOnlyList<PubgPartyMemberState> Members => _members;

	public int TeamSize => _teamSize;

	public bool IsFiftyFiftyMode => _teamSize == 50;

	public string? LocalTeamTag => _localTeamTag;

	public event Action? PartyStateUpdated;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgPartyStateEvent>((EntitySessionEventHandler<PubgPartyStateEvent>)OnPartyState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgPartyInvitePromptEvent>((EntitySessionEventHandler<PubgPartyInvitePromptEvent>)OnInvitePrompt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgModeStatusEvent>((EntitySessionEventHandler<PubgModeStatusEvent>)OnPubgModeStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgTeamModeStatusEvent>((EntitySessionEventHandler<PubgTeamModeStatusEvent>)OnPubgTeamModeStatus, (Type[])null, (Type[])null);
	}

	private void OnPartyState(PubgPartyStateEvent ev, EntitySessionEventArgs args)
	{
		_members.Clear();
		_members.AddRange(ev.Members);
		_localTeamTag = ev.TeamTag;
		this.PartyStateUpdated?.Invoke();
	}

	private void OnPubgModeStatus(PubgModeStatusEvent ev, EntitySessionEventArgs args)
	{
		if (!ev.Enabled)
		{
			_members.Clear();
			_teamSize = 1;
			_localTeamTag = null;
			this.PartyStateUpdated?.Invoke();
			CloseInviteWindow();
		}
	}

	private void OnPubgTeamModeStatus(PubgTeamModeStatusEvent ev, EntitySessionEventArgs args)
	{
		_teamSize = ((!ev.Enabled) ? 1 : Math.Max(1, ev.TeamSize));
		if (_teamSize != 50)
		{
			_localTeamTag = null;
		}
		this.PartyStateUpdated?.Invoke();
	}

	private void OnInvitePrompt(PubgPartyInvitePromptEvent ev, EntitySessionEventArgs args)
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

	public void RequestInvite(NetEntity target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPartyInviteRequestEvent(target));
	}

	public void RespondInvite(bool accepted)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPartyInviteResponseEvent(accepted));
	}

	public void RequestLeave()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPartyLeaveRequestEvent());
	}

	public void RequestVoice()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPartyVoiceRequestEvent());
	}

	public PubgPartyMemberState? GetLocalMember()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return null;
		}
		NetEntity netEntity = ((EntitySystem)this).GetNetEntity(localEntity.Value, (MetaDataComponent)null);
		foreach (PubgPartyMemberState member in _members)
		{
			if (member.Entity == netEntity)
			{
				return member;
			}
		}
		return null;
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

	private void CloseInviteWindow()
	{
		if (_inviteOpen && !_inviteHandled)
		{
			SendInviteResponse(accepted: false);
			return;
		}
		if (_inviteWindow != null)
		{
			((BaseButton)_inviteWindow.AcceptButton).OnPressed -= OnInviteAcceptPressed;
			((BaseButton)_inviteWindow.DeclineButton).OnPressed -= OnInviteDeclinePressed;
			((BaseWindow)_inviteWindow).OnClose -= OnInviteClosed;
			((BaseWindow)_inviteWindow).Close();
		}
		_inviteOpen = false;
		_inviteHandled = false;
		_inviteExpiresAt = null;
		_inviteCloseAt = null;
		_inviteExpired = false;
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
