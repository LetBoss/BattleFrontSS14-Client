using System;
using Content.Client.Administration.Managers;
using Content.Client.Ghost;
using Content.Shared.Administration;
using Content.Shared.Chat;
using Robust.Client.Console;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Content.Client.Chat.Managers;

internal sealed class ChatManager : IChatManager, ISharedChatManager
{
	[Dependency]
	private IClientConsoleHost _consoleHost;

	[Dependency]
	private IClientAdminManager _adminMgr;

	[Dependency]
	private IEntitySystemManager _systems;

	private ISawmill _sawmill;

	public void Initialize()
	{
		_sawmill = Logger.GetSawmill("chat");
		_sawmill.Level = (LogLevel)2;
	}

	public void SendAdminAlert(string message)
	{
	}

	public void SendAdminAlert(EntityUid player, string message)
	{
	}

	public void SendMessage(string text, ChatSelectChannel channel)
	{
		string text2 = text.ToString();
		if (channel <= ChatSelectChannel.Emotes)
		{
			if (channel <= ChatSelectChannel.Radio)
			{
				if (channel != ChatSelectChannel.Local)
				{
					if (channel == ChatSelectChannel.Whisper)
					{
						((IConsoleHost)_consoleHost).ExecuteCommand("whisper \"" + CommandParsing.Escape(text2) + "\"");
						return;
					}
					if (channel != ChatSelectChannel.Radio)
					{
						goto IL_0266;
					}
				}
				goto IL_0224;
			}
			switch (channel)
			{
			case ChatSelectChannel.LOOC:
				((IConsoleHost)_consoleHost).ExecuteCommand("looc \"" + CommandParsing.Escape(text2) + "\"");
				return;
			case ChatSelectChannel.OOC:
				((IConsoleHost)_consoleHost).ExecuteCommand("ooc \"" + CommandParsing.Escape(text2) + "\"");
				return;
			case ChatSelectChannel.Emotes:
				((IConsoleHost)_consoleHost).ExecuteCommand("me \"" + CommandParsing.Escape(text2) + "\"");
				return;
			}
		}
		else if (channel <= ChatSelectChannel.Console)
		{
			if (channel == ChatSelectChannel.Dead)
			{
				GhostSystem entitySystemOrNull = _systems.GetEntitySystemOrNull<GhostSystem>();
				if (entitySystemOrNull == null || !entitySystemOrNull.IsGhost)
				{
					if (_adminMgr.HasFlag(AdminFlags.Admin))
					{
						((IConsoleHost)_consoleHost).ExecuteCommand("dsay \"" + CommandParsing.Escape(text2) + "\"");
					}
					else
					{
						_sawmill.Warning("Tried to speak on deadchat without being ghost or admin.");
					}
					return;
				}
				goto IL_0224;
			}
			switch (channel)
			{
			case ChatSelectChannel.Console:
				((IConsoleHost)_consoleHost).ExecuteCommand(text);
				return;
			case ChatSelectChannel.Admin:
				((IConsoleHost)_consoleHost).ExecuteCommand("asay \"" + CommandParsing.Escape(text2) + "\"");
				return;
			}
		}
		else
		{
			switch (channel)
			{
			case ChatSelectChannel.Mentor:
				((IConsoleHost)_consoleHost).ExecuteCommand("msay \"" + CommandParsing.Escape(text2) + "\"");
				return;
			case ChatSelectChannel.Party:
				((IConsoleHost)_consoleHost).ExecuteCommand("party \"" + CommandParsing.Escape(text2) + "\"");
				return;
			case ChatSelectChannel.MiniGame:
				((IConsoleHost)_consoleHost).ExecuteCommand("minigame \"" + CommandParsing.Escape(text2) + "\"");
				return;
			case ChatSelectChannel.Lobby:
				((IConsoleHost)_consoleHost).ExecuteCommand("lobby \"" + CommandParsing.Escape(text2) + "\"");
				return;
			}
		}
		goto IL_0266;
		IL_0266:
		throw new ArgumentOutOfRangeException("channel", channel, null);
		IL_0224:
		((IConsoleHost)_consoleHost).ExecuteCommand("say \"" + CommandParsing.Escape(text2) + "\"");
	}
}
