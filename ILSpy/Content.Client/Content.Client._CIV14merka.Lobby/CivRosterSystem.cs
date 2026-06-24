using System;
using System.Collections.Generic;
using Content.Client._CIV14merka.Lobby.UI;
using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;

namespace Content.Client._CIV14merka.Lobby;

public sealed class CivRosterSystem : EntitySystem
{
	private CivRosterStateEvent _state = new CivRosterStateEvent(enabled: false, roundInProgress: false, lateJoinActive: false, canEnterRound: false, null, isJoinedRound: false, hasParticipatedInCurrentRound: false, rejoinBlockedForCurrentRound: false, null, null, new List<CivRosterTeamEntry>(), new List<CivRosterPlayerEntry>());

	private CivRosterWindow? _window;

	private CivRosterControl? _inlineControl;

	private CivRosterInviteWindow? _inviteWindow;

	private readonly Queue<CivRosterInvitePromptEvent> _pendingInvites = new Queue<CivRosterInvitePromptEvent>();

	private readonly HashSet<int> _pendingInviteIds = new HashSet<int>();

	private CivRosterInvitePromptEvent? _activeInvite;

	private bool _closingInviteWindow;

	public event Action<CivRosterStateEvent>? StateUpdated;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivRosterStateEvent>((EntitySessionEventHandler<CivRosterStateEvent>)OnState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivRosterInvitePromptEvent>((EntitySessionEventHandler<CivRosterInvitePromptEvent>)OnInvitePrompt, (Type[])null, (Type[])null);
	}

	public CivRosterStateEvent GetState()
	{
		return _state;
	}

	public void RequestState()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterRequestStateEvent());
	}

	public void SelectTeam(int teamId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterSelectTeamRequestEvent(teamId));
	}

	public void JoinSquad(int teamId, int squadId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterJoinSquadRequestEvent(teamId, squadId));
	}

	public void CreateSquad(int teamId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterCreateSquadRequestEvent(teamId));
	}

	public void EnterRound()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterEnterRoundRequestEvent());
	}

	public void LeaveSquad()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterLeaveSquadRequestEvent());
	}

	public void SetSquadOpen(int teamId, int squadId, bool isOpen)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterSetSquadOpenRequestEvent(teamId, squadId, isOpen));
	}

	public void InvitePlayer(int teamId, int squadId, NetUserId targetUserId)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterInviteRequestEvent(teamId, squadId, targetUserId));
	}

	public void KickMember(int teamId, int squadId, NetUserId targetUserId)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterKickMemberRequestEvent(teamId, squadId, targetUserId));
	}

	public void RenameSquad(int teamId, int squadId, string name)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterRenameSquadRequestEvent(teamId, squadId, name));
	}

	public void SelectClass(CivTdmClass selectedClass)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterSelectClassRequestEvent(selectedClass));
	}

	public void SetAllowAutoLeader(bool allow)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterSetAllowAutoLeaderRequestEvent(allow));
	}

	public void SelectPreferredClass(CivTdmClass selectedClass)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterSelectClassRequestEvent(selectedClass));
	}

	public void NominateCommander(int teamId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterNominateCommanderRequestEvent(teamId));
	}

	public void WithdrawCommander(int teamId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterWithdrawCommanderRequestEvent(teamId));
	}

	public void OpenWindow()
	{
		EnsureWindow();
		UpdateWindow();
		CivRosterWindow window = _window;
		if (window != null && !((BaseWindow)window).IsOpen)
		{
			((BaseWindow)_window).OpenCentered();
		}
		RequestState();
	}

	public void CloseWindow()
	{
		if (_window != null)
		{
			((BaseWindow)_window).OnClose -= OnWindowClosed;
			((BaseWindow)_window).Close();
			_window = null;
		}
	}

	public void AttachInlineControl(CivRosterControl control)
	{
		if (_inlineControl != control)
		{
			DetachInlineControl();
			_inlineControl = control;
			WireControl(_inlineControl);
			_inlineControl.UpdateState(_state);
			RequestState();
		}
	}

	public void DetachInlineControl()
	{
		if (_inlineControl != null)
		{
			UnwireControl(_inlineControl);
			_inlineControl = null;
		}
	}

	private void OnState(CivRosterStateEvent msg, EntitySessionEventArgs args)
	{
		_state = msg;
		this.StateUpdated?.Invoke(msg);
		UpdateWindow();
	}

	private void OnInvitePrompt(CivRosterInvitePromptEvent msg, EntitySessionEventArgs args)
	{
		EnsureInviteWindow();
		if (_pendingInviteIds.Add(msg.InviteId))
		{
			_pendingInvites.Enqueue(msg);
			ShowNextInvite();
		}
	}

	private void EnsureWindow()
	{
		if (_window == null)
		{
			_window = new CivRosterWindow();
			WireControl(_window.RosterControl);
			((BaseWindow)_window).OnClose += OnWindowClosed;
		}
	}

	private void EnsureInviteWindow()
	{
		if (_inviteWindow == null)
		{
			_inviteWindow = new CivRosterInviteWindow();
			_inviteWindow.AcceptPressed += OnInviteAccepted;
			_inviteWindow.DeclinePressed += OnInviteDeclined;
			((BaseWindow)_inviteWindow).OnClose += OnInviteWindowClosed;
		}
	}

	private void UpdateWindow()
	{
		_window?.RosterControl.UpdateState(_state);
		_inlineControl?.UpdateState(_state);
	}

	private void OnWindowClosed()
	{
		if (_window != null)
		{
			((BaseWindow)_window).OnClose -= OnWindowClosed;
			UnwireControl(_window.RosterControl);
			_window = null;
		}
	}

	private void OnInviteAccepted()
	{
		RespondToInvite(accept: true);
	}

	private void OnInviteDeclined()
	{
		RespondToInvite(accept: false);
	}

	private void OnInviteWindowClosed()
	{
		if (_closingInviteWindow)
		{
			_closingInviteWindow = false;
			DisposeInviteWindow();
		}
		else
		{
			RespondToAllPendingInvites(accept: false);
			DisposeInviteWindow();
		}
	}

	private void DisposeInviteWindow()
	{
		if (_inviteWindow != null)
		{
			_inviteWindow.AcceptPressed -= OnInviteAccepted;
			_inviteWindow.DeclinePressed -= OnInviteDeclined;
			((BaseWindow)_inviteWindow).OnClose -= OnInviteWindowClosed;
			_inviteWindow = null;
		}
	}

	private void RespondToInvite(bool accept)
	{
		if (_activeInvite != null)
		{
			int inviteId = _activeInvite.InviteId;
			_activeInvite = null;
			_pendingInviteIds.Remove(inviteId);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterInviteResponseEvent(inviteId, accept));
			ShowNextInvite();
		}
	}

	private void RespondToAllPendingInvites(bool accept)
	{
		if (_activeInvite != null)
		{
			int inviteId = _activeInvite.InviteId;
			_activeInvite = null;
			_pendingInviteIds.Remove(inviteId);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterInviteResponseEvent(inviteId, accept));
		}
		CivRosterInvitePromptEvent result;
		while (_pendingInvites.TryDequeue(out result))
		{
			_pendingInviteIds.Remove(result.InviteId);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRosterInviteResponseEvent(result.InviteId, accept));
		}
	}

	private void ShowNextInvite()
	{
		EnsureInviteWindow();
		if (_activeInvite == null && _pendingInvites.TryDequeue(out CivRosterInvitePromptEvent result))
		{
			_activeInvite = result;
		}
		if (_activeInvite == null)
		{
			CivRosterInviteWindow inviteWindow = _inviteWindow;
			if (inviteWindow != null && ((BaseWindow)inviteWindow).IsOpen)
			{
				_closingInviteWindow = true;
				((BaseWindow)_inviteWindow).Close();
			}
		}
		else
		{
			_inviteWindow.UpdateInvite(_activeInvite);
			if (!((BaseWindow)_inviteWindow).IsOpen)
			{
				((BaseWindow)_inviteWindow).OpenCentered();
			}
		}
	}

	private void WireControl(CivRosterControl control)
	{
		control.TeamSelected += SelectTeam;
		control.JoinSquadRequested += JoinSquad;
		control.LeaveSquadRequested += LeaveSquad;
		control.CreateSquadRequested += CreateSquad;
		control.EnterRoundRequested += EnterRound;
		control.KickRequested += KickMember;
		control.RenameSquadRequested += RenameSquad;
		control.NominateCommanderRequested += NominateCommander;
		control.WithdrawCommanderRequested += WithdrawCommander;
		control.ClassSelected += SelectClass;
	}

	private void UnwireControl(CivRosterControl control)
	{
		control.TeamSelected -= SelectTeam;
		control.JoinSquadRequested -= JoinSquad;
		control.LeaveSquadRequested -= LeaveSquad;
		control.CreateSquadRequested -= CreateSquad;
		control.EnterRoundRequested -= EnterRound;
		control.KickRequested -= KickMember;
		control.RenameSquadRequested -= RenameSquad;
		control.NominateCommanderRequested -= NominateCommander;
		control.WithdrawCommanderRequested -= WithdrawCommander;
		control.ClassSelected -= SelectClass;
	}
}
