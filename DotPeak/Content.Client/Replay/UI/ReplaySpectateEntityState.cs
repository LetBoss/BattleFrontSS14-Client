// Decompiled with JetBrains decompiler
// Type: Content.Client.Replay.UI.ReplaySpectateEntityState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Chat;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Robust.Client.Replays.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Replay.UI;

[Virtual]
public class ReplaySpectateEntityState : GameplayState
{
  [Dependency]
  private ContentReplayPlaybackManager _replayManager;

  protected override void Startup()
  {
    base.Startup();
    UIScreen activeScreen = this.UserInterfaceManager.ActiveScreen;
    if (activeScreen == null)
      return;
    activeScreen.ShowWidget<GameTopMenuBar>(false);
    ReplayControlWidget orAddWidget = activeScreen.GetOrAddWidget<ReplayControlWidget>();
    LayoutContainer.SetAnchorAndMarginPreset((Control) orAddWidget, (LayoutContainer.LayoutPreset) 0, (LayoutContainer.LayoutPresetMode) 0, 10);
    ((Control) orAddWidget).Visible = !this._replayManager.IsScreenshotMode;
    foreach (ChatBox chat in (IEnumerable<ChatBox>) this.UserInterfaceManager.GetUIController<ChatUIController>().Chats)
      ((Control) chat.ChatInput).Visible = this._replayManager.IsScreenshotMode;
  }

  protected override void Shutdown()
  {
    UIScreen activeScreen = this.UserInterfaceManager.ActiveScreen;
    if (activeScreen != null)
    {
      activeScreen.RemoveWidget<ReplayControlWidget>();
      activeScreen.ShowWidget<GameTopMenuBar>(true);
    }
    foreach (ChatBox chat in (IEnumerable<ChatBox>) this.UserInterfaceManager.GetUIController<ChatUIController>().Chats)
      ((Control) chat.ChatInput).Visible = true;
    base.Shutdown();
  }
}
