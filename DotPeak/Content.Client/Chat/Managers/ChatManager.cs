// Decompiled with JetBrains decompiler
// Type: Content.Client.Chat.Managers.ChatManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;

#nullable enable
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
    this._sawmill = Logger.GetSawmill("chat");
    this._sawmill.Level = new LogLevel?((LogLevel) 2);
  }

  public void SendAdminAlert(string message)
  {
  }

  public void SendAdminAlert(EntityUid player, string message)
  {
  }

  public void SendMessage(string text, ChatSelectChannel channel)
  {
    string str = text.ToString();
    switch (channel)
    {
      case ChatSelectChannel.Local:
      case ChatSelectChannel.Radio:
        ((IConsoleHost) this._consoleHost).ExecuteCommand($"say \"{CommandParsing.Escape(str)}\"");
        break;
      case ChatSelectChannel.Whisper:
        ((IConsoleHost) this._consoleHost).ExecuteCommand($"whisper \"{CommandParsing.Escape(str)}\"");
        break;
      case ChatSelectChannel.LOOC:
        ((IConsoleHost) this._consoleHost).ExecuteCommand($"looc \"{CommandParsing.Escape(str)}\"");
        break;
      case ChatSelectChannel.OOC:
        ((IConsoleHost) this._consoleHost).ExecuteCommand($"ooc \"{CommandParsing.Escape(str)}\"");
        break;
      case ChatSelectChannel.Emotes:
        ((IConsoleHost) this._consoleHost).ExecuteCommand($"me \"{CommandParsing.Escape(str)}\"");
        break;
      case ChatSelectChannel.Dead:
        GhostSystem entitySystemOrNull = this._systems.GetEntitySystemOrNull<GhostSystem>();
        if (entitySystemOrNull == null || !entitySystemOrNull.IsGhost)
        {
          if (this._adminMgr.HasFlag(AdminFlags.Admin))
          {
            ((IConsoleHost) this._consoleHost).ExecuteCommand($"dsay \"{CommandParsing.Escape(str)}\"");
            break;
          }
          this._sawmill.Warning("Tried to speak on deadchat without being ghost or admin.");
          break;
        }
        goto case ChatSelectChannel.Local;
      case ChatSelectChannel.Admin:
        ((IConsoleHost) this._consoleHost).ExecuteCommand($"asay \"{CommandParsing.Escape(str)}\"");
        break;
      case ChatSelectChannel.Console:
        ((IConsoleHost) this._consoleHost).ExecuteCommand(text);
        break;
      case ChatSelectChannel.Mentor:
        ((IConsoleHost) this._consoleHost).ExecuteCommand($"msay \"{CommandParsing.Escape(str)}\"");
        break;
      case ChatSelectChannel.Party:
        ((IConsoleHost) this._consoleHost).ExecuteCommand($"party \"{CommandParsing.Escape(str)}\"");
        break;
      case ChatSelectChannel.MiniGame:
        ((IConsoleHost) this._consoleHost).ExecuteCommand($"minigame \"{CommandParsing.Escape(str)}\"");
        break;
      case ChatSelectChannel.Lobby:
        ((IConsoleHost) this._consoleHost).ExecuteCommand($"lobby \"{CommandParsing.Escape(str)}\"");
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof (channel), (object) channel, (string) null);
    }
  }
}
