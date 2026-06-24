// Decompiled with JetBrains decompiler
// Type: Content.Client.Replay.UI.ReplayGhostState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Actions.Widgets;
using Content.Client.UserInterface.Systems.Alerts.Widgets;
using Content.Client.UserInterface.Systems.Ghost.Widgets;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Robust.Client.UserInterface;

#nullable disable
namespace Content.Client.Replay.UI;

public sealed class ReplayGhostState : ReplaySpectateEntityState
{
  protected override void Startup()
  {
    base.Startup();
    UIScreen activeScreen = this.UserInterfaceManager.ActiveScreen;
    if (activeScreen == null)
      return;
    activeScreen.ShowWidget<GhostGui>(false);
    activeScreen.ShowWidget<ActionsBar>(false);
    activeScreen.ShowWidget<AlertsUI>(false);
    activeScreen.ShowWidget<HotbarGui>(false);
  }

  protected override void Shutdown()
  {
    UIScreen activeScreen = this.UserInterfaceManager.ActiveScreen;
    if (activeScreen != null)
    {
      activeScreen.ShowWidget<GhostGui>(true);
      activeScreen.ShowWidget<ActionsBar>(true);
      activeScreen.ShowWidget<AlertsUI>(true);
      activeScreen.ShowWidget<HotbarGui>(true);
    }
    base.Shutdown();
  }
}
