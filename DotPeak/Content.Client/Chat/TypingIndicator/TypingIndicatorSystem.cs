// Decompiled with JetBrains decompiler
// Type: Content.Client.Chat.TypingIndicator.TypingIndicatorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Content.Shared.Chat.TypingIndicator;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Chat.TypingIndicator;

public sealed class TypingIndicatorSystem : SharedTypingIndicatorSystem
{
  [Dependency]
  private IGameTiming _time;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IConfigurationManager _cfg;
  private readonly TimeSpan _typingTimeout = TimeSpan.FromSeconds(2L);
  private TimeSpan _lastTextChange;
  private bool _isClientTyping;
  private bool _isClientChatFocused;

  public override void Initialize()
  {
    base.Initialize();
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._cfg, CCVars.ChatShowTypingIndicator, new Action<bool>(this.OnShowTypingChanged), false);
  }

  public void ClientChangedChatText()
  {
    if (!this._cfg.GetCVar<bool>(CCVars.ChatShowTypingIndicator))
      return;
    this._isClientTyping = true;
    this.ClientUpdateTyping();
    this._lastTextChange = this._time.CurTime;
  }

  public void ClientSubmittedChatText()
  {
    if (!this._cfg.GetCVar<bool>(CCVars.ChatShowTypingIndicator))
      return;
    this._isClientTyping = false;
    this.ClientUpdateTyping();
  }

  public void ClientChangedChatFocus(bool isFocused)
  {
    if (!this._cfg.GetCVar<bool>(CCVars.ChatShowTypingIndicator))
      return;
    this._isClientChatFocused = isFocused;
    this.ClientUpdateTyping();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._time.IsFirstTimePredicted || !this._isClientTyping || !(this._time.CurTime - this._lastTextChange > this._typingTimeout))
      return;
    this._isClientTyping = false;
    this.ClientUpdateTyping();
  }

  private void ClientUpdateTyping()
  {
    if (!((ISharedPlayerManager) this._playerManager).LocalEntity.HasValue)
      return;
    TypingIndicatorState state = TypingIndicatorState.None;
    if (this._isClientChatFocused)
      state = this._isClientTyping ? TypingIndicatorState.Typing : TypingIndicatorState.Idle;
    this.RaisePredictiveEvent<TypingChangedEvent>(new TypingChangedEvent(state));
  }

  private void OnShowTypingChanged(bool showTyping)
  {
    if (showTyping)
      return;
    this._isClientTyping = false;
    this.ClientUpdateTyping();
  }
}
