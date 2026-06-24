using System;
using Content.Shared.CCVar;
using Content.Shared.Chat.TypingIndicator;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;

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
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _cfg, CCVars.ChatShowTypingIndicator, (Action<bool>)OnShowTypingChanged, false);
	}

	public void ClientChangedChatText()
	{
		if (_cfg.GetCVar<bool>(CCVars.ChatShowTypingIndicator))
		{
			_isClientTyping = true;
			ClientUpdateTyping();
			_lastTextChange = _time.CurTime;
		}
	}

	public void ClientSubmittedChatText()
	{
		if (_cfg.GetCVar<bool>(CCVars.ChatShowTypingIndicator))
		{
			_isClientTyping = false;
			ClientUpdateTyping();
		}
	}

	public void ClientChangedChatFocus(bool isFocused)
	{
		if (_cfg.GetCVar<bool>(CCVars.ChatShowTypingIndicator))
		{
			_isClientChatFocused = isFocused;
			ClientUpdateTyping();
		}
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (_time.IsFirstTimePredicted && _isClientTyping && _time.CurTime - _lastTextChange > _typingTimeout)
		{
			_isClientTyping = false;
			ClientUpdateTyping();
		}
	}

	private void ClientUpdateTyping()
	{
		if (((ISharedPlayerManager)_playerManager).LocalEntity.HasValue)
		{
			TypingIndicatorState state = TypingIndicatorState.None;
			if (_isClientChatFocused)
			{
				state = ((!_isClientTyping) ? TypingIndicatorState.Idle : TypingIndicatorState.Typing);
			}
			((EntitySystem)this).RaisePredictiveEvent<TypingChangedEvent>(new TypingChangedEvent(state));
		}
	}

	private void OnShowTypingChanged(bool showTyping)
	{
		if (!showTyping)
		{
			_isClientTyping = false;
			ClientUpdateTyping();
		}
	}
}
