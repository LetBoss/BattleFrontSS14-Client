// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Party.PubgPartyClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;

#nullable enable
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

  public event Action? PartyStateUpdated;

  public IReadOnlyList<PubgPartyMemberState> Members
  {
    get => (IReadOnlyList<PubgPartyMemberState>) this._members;
  }

  public int TeamSize => this._teamSize;

  public bool IsFiftyFiftyMode => this._teamSize == 50;

  public string? LocalTeamTag => this._localTeamTag;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgPartyStateEvent>(new EntitySessionEventHandler<PubgPartyStateEvent>(this.OnPartyState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgPartyInvitePromptEvent>(new EntitySessionEventHandler<PubgPartyInvitePromptEvent>(this.OnInvitePrompt), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgModeStatusEvent>(new EntitySessionEventHandler<PubgModeStatusEvent>(this.OnPubgModeStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgTeamModeStatusEvent>(new EntitySessionEventHandler<PubgTeamModeStatusEvent>(this.OnPubgTeamModeStatus), (Type[]) null, (Type[]) null);
  }

  private void OnPartyState(PubgPartyStateEvent ev, EntitySessionEventArgs args)
  {
    this._members.Clear();
    this._members.AddRange((IEnumerable<PubgPartyMemberState>) ev.Members);
    this._localTeamTag = ev.TeamTag;
    Action partyStateUpdated = this.PartyStateUpdated;
    if (partyStateUpdated == null)
      return;
    partyStateUpdated();
  }

  private void OnPubgModeStatus(PubgModeStatusEvent ev, EntitySessionEventArgs args)
  {
    if (ev.Enabled)
      return;
    this._members.Clear();
    this._teamSize = 1;
    this._localTeamTag = (string) null;
    Action partyStateUpdated = this.PartyStateUpdated;
    if (partyStateUpdated != null)
      partyStateUpdated();
    this.CloseInviteWindow();
  }

  private void OnPubgTeamModeStatus(PubgTeamModeStatusEvent ev, EntitySessionEventArgs args)
  {
    this._teamSize = ev.Enabled ? Math.Max(1, ev.TeamSize) : 1;
    if (this._teamSize != 50)
      this._localTeamTag = (string) null;
    Action partyStateUpdated = this.PartyStateUpdated;
    if (partyStateUpdated == null)
      return;
    partyStateUpdated();
  }

  private void OnInvitePrompt(PubgPartyInvitePromptEvent ev, EntitySessionEventArgs args)
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

  public void RequestInvite(NetEntity target)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPartyInviteRequestEvent(target));
  }

  public void RespondInvite(bool accepted)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPartyInviteResponseEvent(accepted));
  }

  public void RequestLeave()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPartyLeaveRequestEvent());
  }

  public void RequestVoice()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPartyVoiceRequestEvent());
  }

  public PubgPartyMemberState? GetLocalMember()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return (PubgPartyMemberState) null;
    NetEntity netEntity = this.GetNetEntity(localEntity.Value, (MetaDataComponent) null);
    foreach (PubgPartyMemberState member in this._members)
    {
      if (NetEntity.op_Equality(member.Entity, netEntity))
        return member;
    }
    return (PubgPartyMemberState) null;
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

  private void CloseInviteWindow()
  {
    if (this._inviteOpen && !this._inviteHandled)
    {
      this.SendInviteResponse(false);
    }
    else
    {
      if (this._inviteWindow != null)
      {
        ((BaseButton) this._inviteWindow.AcceptButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnInviteAcceptPressed);
        ((BaseButton) this._inviteWindow.DeclineButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnInviteDeclinePressed);
        ((BaseWindow) this._inviteWindow).OnClose -= new Action(this.OnInviteClosed);
        ((BaseWindow) this._inviteWindow).Close();
      }
      this._inviteOpen = false;
      this._inviteHandled = false;
      this._inviteExpiresAt = new TimeSpan?();
      this._inviteCloseAt = new TimeSpan?();
      this._inviteExpired = false;
    }
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
