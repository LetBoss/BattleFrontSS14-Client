// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.CivRosterSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Lobby.UI;
using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.Lobby;

public sealed class CivRosterSystem : EntitySystem
{
  private CivRosterStateEvent _state = new CivRosterStateEvent(false, false, false, false, (string) null, false, false, false, new int?(), new int?(), new List<CivRosterTeamEntry>(), new List<CivRosterPlayerEntry>());
  private CivRosterWindow? _window;
  private CivRosterControl? _inlineControl;
  private CivRosterInviteWindow? _inviteWindow;
  private readonly Queue<CivRosterInvitePromptEvent> _pendingInvites = new Queue<CivRosterInvitePromptEvent>();
  private readonly HashSet<int> _pendingInviteIds = new HashSet<int>();
  private CivRosterInvitePromptEvent? _activeInvite;
  private bool _closingInviteWindow;

  public event Action<CivRosterStateEvent>? StateUpdated;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivRosterStateEvent>(new EntitySessionEventHandler<CivRosterStateEvent>(this.OnState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivRosterInvitePromptEvent>(new EntitySessionEventHandler<CivRosterInvitePromptEvent>(this.OnInvitePrompt), (Type[]) null, (Type[]) null);
  }

  public CivRosterStateEvent GetState() => this._state;

  public void RequestState()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterRequestStateEvent());
  }

  public void SelectTeam(int teamId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterSelectTeamRequestEvent(teamId));
  }

  public void JoinSquad(int teamId, int squadId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterJoinSquadRequestEvent(teamId, squadId));
  }

  public void CreateSquad(int teamId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterCreateSquadRequestEvent(teamId));
  }

  public void EnterRound()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterEnterRoundRequestEvent());
  }

  public void LeaveSquad()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterLeaveSquadRequestEvent());
  }

  public void SetSquadOpen(int teamId, int squadId, bool isOpen)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterSetSquadOpenRequestEvent(teamId, squadId, isOpen));
  }

  public void InvitePlayer(int teamId, int squadId, NetUserId targetUserId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterInviteRequestEvent(teamId, squadId, targetUserId));
  }

  public void KickMember(int teamId, int squadId, NetUserId targetUserId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterKickMemberRequestEvent(teamId, squadId, targetUserId));
  }

  public void RenameSquad(int teamId, int squadId, string name)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterRenameSquadRequestEvent(teamId, squadId, name));
  }

  public void SelectClass(CivTdmClass selectedClass)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterSelectClassRequestEvent(selectedClass));
  }

  public void SetAllowAutoLeader(bool allow)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterSetAllowAutoLeaderRequestEvent(allow));
  }

  public void SelectPreferredClass(CivTdmClass selectedClass)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterSelectClassRequestEvent(selectedClass));
  }

  public void NominateCommander(int teamId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterNominateCommanderRequestEvent(teamId));
  }

  public void WithdrawCommander(int teamId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterWithdrawCommanderRequestEvent(teamId));
  }

  public void OpenWindow()
  {
    this.EnsureWindow();
    this.UpdateWindow();
    CivRosterWindow window = this._window;
    if (window != null && !window.IsOpen)
      this._window.OpenCentered();
    this.RequestState();
  }

  public void CloseWindow()
  {
    if (this._window == null)
      return;
    this._window.OnClose -= new Action(this.OnWindowClosed);
    this._window.Close();
    this._window = (CivRosterWindow) null;
  }

  public void AttachInlineControl(CivRosterControl control)
  {
    if (this._inlineControl == control)
      return;
    this.DetachInlineControl();
    this._inlineControl = control;
    this.WireControl(this._inlineControl);
    this._inlineControl.UpdateState(this._state);
    this.RequestState();
  }

  public void DetachInlineControl()
  {
    if (this._inlineControl == null)
      return;
    this.UnwireControl(this._inlineControl);
    this._inlineControl = (CivRosterControl) null;
  }

  private void OnState(CivRosterStateEvent msg, EntitySessionEventArgs args)
  {
    this._state = msg;
    Action<CivRosterStateEvent> stateUpdated = this.StateUpdated;
    if (stateUpdated != null)
      stateUpdated(msg);
    this.UpdateWindow();
  }

  private void OnInvitePrompt(CivRosterInvitePromptEvent msg, EntitySessionEventArgs args)
  {
    this.EnsureInviteWindow();
    if (!this._pendingInviteIds.Add(msg.InviteId))
      return;
    this._pendingInvites.Enqueue(msg);
    this.ShowNextInvite();
  }

  private void EnsureWindow()
  {
    if (this._window != null)
      return;
    this._window = new CivRosterWindow();
    this.WireControl(this._window.RosterControl);
    this._window.OnClose += new Action(this.OnWindowClosed);
  }

  private void EnsureInviteWindow()
  {
    if (this._inviteWindow != null)
      return;
    this._inviteWindow = new CivRosterInviteWindow();
    this._inviteWindow.AcceptPressed += new Action(this.OnInviteAccepted);
    this._inviteWindow.DeclinePressed += new Action(this.OnInviteDeclined);
    ((BaseWindow) this._inviteWindow).OnClose += new Action(this.OnInviteWindowClosed);
  }

  private void UpdateWindow()
  {
    this._window?.RosterControl.UpdateState(this._state);
    this._inlineControl?.UpdateState(this._state);
  }

  private void OnWindowClosed()
  {
    if (this._window == null)
      return;
    this._window.OnClose -= new Action(this.OnWindowClosed);
    this.UnwireControl(this._window.RosterControl);
    this._window = (CivRosterWindow) null;
  }

  private void OnInviteAccepted() => this.RespondToInvite(true);

  private void OnInviteDeclined() => this.RespondToInvite(false);

  private void OnInviteWindowClosed()
  {
    if (this._closingInviteWindow)
    {
      this._closingInviteWindow = false;
      this.DisposeInviteWindow();
    }
    else
    {
      this.RespondToAllPendingInvites(false);
      this.DisposeInviteWindow();
    }
  }

  private void DisposeInviteWindow()
  {
    if (this._inviteWindow == null)
      return;
    this._inviteWindow.AcceptPressed -= new Action(this.OnInviteAccepted);
    this._inviteWindow.DeclinePressed -= new Action(this.OnInviteDeclined);
    ((BaseWindow) this._inviteWindow).OnClose -= new Action(this.OnInviteWindowClosed);
    this._inviteWindow = (CivRosterInviteWindow) null;
  }

  private void RespondToInvite(bool accept)
  {
    if (this._activeInvite == null)
      return;
    int inviteId = this._activeInvite.InviteId;
    this._activeInvite = (CivRosterInvitePromptEvent) null;
    this._pendingInviteIds.Remove(inviteId);
    this.RaiseNetworkEvent((EntityEventArgs) new CivRosterInviteResponseEvent(inviteId, accept));
    this.ShowNextInvite();
  }

  private void RespondToAllPendingInvites(bool accept)
  {
    if (this._activeInvite != null)
    {
      int inviteId = this._activeInvite.InviteId;
      this._activeInvite = (CivRosterInvitePromptEvent) null;
      this._pendingInviteIds.Remove(inviteId);
      this.RaiseNetworkEvent((EntityEventArgs) new CivRosterInviteResponseEvent(inviteId, accept));
    }
    CivRosterInvitePromptEvent result;
    while (this._pendingInvites.TryDequeue(out result))
    {
      this._pendingInviteIds.Remove(result.InviteId);
      this.RaiseNetworkEvent((EntityEventArgs) new CivRosterInviteResponseEvent(result.InviteId, accept));
    }
  }

  private void ShowNextInvite()
  {
    this.EnsureInviteWindow();
    CivRosterInvitePromptEvent result;
    if (this._activeInvite == null && this._pendingInvites.TryDequeue(out result))
      this._activeInvite = result;
    if (this._activeInvite == null)
    {
      CivRosterInviteWindow inviteWindow = this._inviteWindow;
      if (inviteWindow == null || !((BaseWindow) inviteWindow).IsOpen)
        return;
      this._closingInviteWindow = true;
      ((BaseWindow) this._inviteWindow).Close();
    }
    else
    {
      this._inviteWindow.UpdateInvite(this._activeInvite);
      if (((BaseWindow) this._inviteWindow).IsOpen)
        return;
      ((BaseWindow) this._inviteWindow).OpenCentered();
    }
  }

  private void WireControl(CivRosterControl control)
  {
    control.TeamSelected += new Action<int>(this.SelectTeam);
    control.JoinSquadRequested += new Action<int, int>(this.JoinSquad);
    control.LeaveSquadRequested += new Action(this.LeaveSquad);
    control.CreateSquadRequested += new Action<int>(this.CreateSquad);
    control.EnterRoundRequested += new Action(this.EnterRound);
    control.KickRequested += new Action<int, int, NetUserId>(this.KickMember);
    control.RenameSquadRequested += new Action<int, int, string>(this.RenameSquad);
    control.NominateCommanderRequested += new Action<int>(this.NominateCommander);
    control.WithdrawCommanderRequested += new Action<int>(this.WithdrawCommander);
    control.ClassSelected += new Action<CivTdmClass>(this.SelectClass);
  }

  private void UnwireControl(CivRosterControl control)
  {
    control.TeamSelected -= new Action<int>(this.SelectTeam);
    control.JoinSquadRequested -= new Action<int, int>(this.JoinSquad);
    control.LeaveSquadRequested -= new Action(this.LeaveSquad);
    control.CreateSquadRequested -= new Action<int>(this.CreateSquad);
    control.EnterRoundRequested -= new Action(this.EnterRound);
    control.KickRequested -= new Action<int, int, NetUserId>(this.KickMember);
    control.RenameSquadRequested -= new Action<int, int, string>(this.RenameSquad);
    control.NominateCommanderRequested -= new Action<int>(this.NominateCommander);
    control.WithdrawCommanderRequested -= new Action<int>(this.WithdrawCommander);
    control.ClassSelected -= new Action<CivTdmClass>(this.SelectClass);
  }
}
