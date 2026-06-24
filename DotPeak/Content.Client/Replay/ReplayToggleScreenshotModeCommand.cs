// Decompiled with JetBrains decompiler
// Type: Content.Client.Replay.ReplayToggleScreenshotModeCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Chat;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Shared.Chat;
using Robust.Client.Replays.Commands;
using Robust.Client.Replays.UI;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Replay;

public sealed class ReplayToggleScreenshotModeCommand : BaseReplayCommand
{
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;
  [Dependency]
  private ContentReplayPlaybackManager _replayManager;

  public virtual string Command => "replay_toggle_screenshot_mode";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    UIScreen activeScreen = this._userInterfaceManager.ActiveScreen;
    if (activeScreen == null)
      return;
    this._replayManager.IsScreenshotMode = !this._replayManager.IsScreenshotMode;
    bool isScreenshotMode = this._replayManager.IsScreenshotMode;
    activeScreen.ShowWidget<ReplayControlWidget>(isScreenshotMode);
    foreach (ChatBox chat in (IEnumerable<ChatBox>) this._userInterfaceManager.GetUIController<ChatUIController>().Chats)
    {
      ((Control) chat.ChatInput).Visible = !isScreenshotMode;
      if (!isScreenshotMode)
        chat.ChatInput.ChannelSelector.Select(ChatSelectChannel.Local);
    }
  }
}
