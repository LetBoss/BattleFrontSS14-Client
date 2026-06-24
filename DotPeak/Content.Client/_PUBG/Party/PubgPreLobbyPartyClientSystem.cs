// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Party.PubgPreLobbyPartyClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.UserInterface.Systems.Party;
using Content.Shared._PUBG.Match;
using Content.Shared._PUBG.Party;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
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

  public event Action? PartyStateUpdated;

  public event Action? ModeOverviewUpdated;

  public IReadOnlyList<PubgPreLobbyPartyMemberState> Members
  {
    get => (IReadOnlyList<PubgPreLobbyPartyMemberState>) this._members;
  }

  public NetUserId? LeaderId => this._leaderId;

  public PubgMatchMode? SelectedMode => this._selectedMode;

  public bool PreferFullSquad => this._preferFullSquad;

  public bool AllReady
  {
    get
    {
      if (this._members.Count <= 1)
        return true;
      return this._members.All<PubgPreLobbyPartyMemberState>((Func<PubgPreLobbyPartyMemberState, bool>) (m => m.InPreLobby)) && this._members.All<PubgPreLobbyPartyMemberState>((Func<PubgPreLobbyPartyMemberState, bool>) (m =>
      {
        NetUserId userId = m.UserId;
        NetUserId? leaderId = this._leaderId;
        return (leaderId.HasValue ? (NetUserId.op_Equality(userId, leaderId.GetValueOrDefault()) ? 1 : 0) : 0) != 0 || m.IsReady;
      }));
    }
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgPreLobbyPartyStateEvent>(new EntitySessionEventHandler<PubgPreLobbyPartyStateEvent>(this.OnPartyState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgPreLobbyPartyInvitePromptEvent>(new EntitySessionEventHandler<PubgPreLobbyPartyInvitePromptEvent>(this.OnInvitePrompt), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgPreLobbyModeOverviewEvent>(new EntitySessionEventHandler<PubgPreLobbyModeOverviewEvent>(this.OnModeOverview), (Type[]) null, (Type[]) null);
  }

  private void OnPartyState(PubgPreLobbyPartyStateEvent ev, EntitySessionEventArgs args)
  {
    this._members.Clear();
    this._members.AddRange((IEnumerable<PubgPreLobbyPartyMemberState>) ev.Members);
    this._leaderId = ev.Members.Count == 0 ? new NetUserId?() : new NetUserId?(ev.LeaderId);
    this._selectedMode = ev.SelectedMode;
    this._preferFullSquad = ev.PreferFullSquad;
    Action partyStateUpdated = this.PartyStateUpdated;
    if (partyStateUpdated == null)
      return;
    partyStateUpdated();
  }

  private void OnModeOverview(PubgPreLobbyModeOverviewEvent ev, EntitySessionEventArgs args)
  {
    this._modeOverview.Clear();
    foreach (PubgPreLobbyModeOverviewEntry entry in ev.Entries)
      this._modeOverview[entry.Mode] = entry;
    Action modeOverviewUpdated = this.ModeOverviewUpdated;
    if (modeOverviewUpdated == null)
      return;
    modeOverviewUpdated();
  }

  public void SendModeSelection(PubgMatchMode? selectedMode, bool preferFullSquad)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPreLobbyPartyModeSelectEvent(selectedMode, preferFullSquad));
  }

  public void SendReadyToggle(bool isReady)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPreLobbyPartyReadyToggleEvent(isReady));
  }

  public void SendLeaveRequest()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPreLobbyPartyLeaveRequestEvent());
  }

  public void RequestState()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPreLobbyPartyStateRequestEvent());
  }

  public void RequestModeOverview()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPreLobbyModeOverviewRequestEvent());
  }

  public PubgPreLobbyModeOverviewEntry? GetModeOverview(PubgMatchMode mode)
  {
    return this._modeOverview.GetValueOrDefault<PubgMatchMode, PubgPreLobbyModeOverviewEntry>(mode);
  }

  private void OnInvitePrompt(PubgPreLobbyPartyInvitePromptEvent ev, EntitySessionEventArgs args)
  {
    if (this._inviteOpen)
    {
      this.RespondInvite(false);
    }
    else
    {
      if (this._inviteWindow == null)
        this._inviteWindow = new PubgPartyInviteWindow();
      this._inviteWindow.SetInviter(ev.InviterName);
      ((BaseButton) this._inviteWindow.AcceptButton).Disabled = false;
      ((BaseButton) this._inviteWindow.DeclineButton).Disabled = false;
      this._inviteHandled = false;
      this._inviteOpen = true;
      this._inviteExpired = false;
      this._inviteCloseAt = new TimeSpan?();
      this._inviteExpiresAt = new TimeSpan?(this._timing.CurTime + TimeSpan.FromSeconds((long) ev.TimeoutSeconds));
      this._lastTimerSeconds = ev.TimeoutSeconds;
      this._inviteWindow.SetTimerSeconds(this._lastTimerSeconds);
      ((BaseButton) this._inviteWindow.AcceptButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnInviteAcceptPressed);
      ((BaseButton) this._inviteWindow.DeclineButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnInviteDeclinePressed);
      ((BaseWindow) this._inviteWindow).OnClose += new Action(this.OnInviteClosed);
      ((BaseWindow) this._inviteWindow).OpenCentered();
    }
  }

  private void OnInviteAcceptPressed(BaseButton.ButtonEventArgs args)
  {
    this.SendInviteResponse(true);
  }

  private void OnInviteDeclinePressed(BaseButton.ButtonEventArgs args)
  {
    this.SendInviteResponse(false);
  }

  private void OnInviteClosed()
  {
    if (this._inviteHandled)
      return;
    this.SendInviteResponse(false);
  }

  private void SendInviteResponse(bool accepted)
  {
    if (this._inviteHandled)
      return;
    this._inviteHandled = true;
    this._inviteOpen = false;
    this.RespondInvite(accepted);
    if (this._inviteWindow != null)
    {
      ((BaseButton) this._inviteWindow.AcceptButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnInviteAcceptPressed);
      ((BaseButton) this._inviteWindow.DeclineButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnInviteDeclinePressed);
      ((BaseWindow) this._inviteWindow).OnClose -= new Action(this.OnInviteClosed);
      ((BaseWindow) this._inviteWindow).Close();
    }
    this._inviteExpiresAt = new TimeSpan?();
    this._inviteCloseAt = new TimeSpan?();
    this._inviteExpired = false;
  }

  private void CloseInviteWindowImmediate()
  {
    if (this._inviteWindow != null)
    {
      ((BaseButton) this._inviteWindow.AcceptButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnInviteAcceptPressed);
      ((BaseButton) this._inviteWindow.DeclineButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnInviteDeclinePressed);
      ((BaseWindow) this._inviteWindow).OnClose -= new Action(this.OnInviteClosed);
      ((BaseWindow) this._inviteWindow).Close();
    }
    this._inviteOpen = false;
    this._inviteHandled = true;
    this._inviteExpiresAt = new TimeSpan?();
    this._inviteCloseAt = new TimeSpan?();
    this._inviteExpired = false;
  }

  private void RespondInvite(bool accepted)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPreLobbyPartyInviteResponseEvent(accepted));
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._inviteCloseAt.HasValue && this._timing.CurTime >= this._inviteCloseAt.Value)
    {
      this.CloseInviteWindowImmediate();
    }
    else
    {
      if (!this._inviteOpen || this._inviteWindow == null || !this._inviteExpiresAt.HasValue)
        return;
      TimeSpan timeSpan = this._inviteExpiresAt.Value - this._timing.CurTime;
      if (timeSpan <= TimeSpan.Zero)
      {
        this.ExpireInvite();
      }
      else
      {
        int seconds = (int) Math.Ceiling(timeSpan.TotalSeconds);
        if (seconds == this._lastTimerSeconds)
          return;
        this._lastTimerSeconds = seconds;
        this._inviteWindow.SetTimerSeconds(seconds);
      }
    }
  }

  private void ExpireInvite()
  {
    if (this._inviteExpired)
      return;
    this._inviteExpired = true;
    this._inviteOpen = false;
    this._inviteHandled = true;
    this._inviteExpiresAt = new TimeSpan?();
    this.RespondInvite(false);
    if (this._inviteWindow != null)
    {
      ((BaseButton) this._inviteWindow.AcceptButton).Disabled = true;
      ((BaseButton) this._inviteWindow.DeclineButton).Disabled = true;
      this._inviteWindow.SetExpired();
    }
    this._inviteCloseAt = new TimeSpan?(this._timing.CurTime + TimeSpan.FromSeconds(1.5));
  }
}
