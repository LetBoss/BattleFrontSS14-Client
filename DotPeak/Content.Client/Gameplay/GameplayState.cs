// Decompiled with JetBrains decompiler
// Type: Content.Client.Gameplay.GameplayState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Changelog;
using Content.Client.Hands;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.Viewport;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Gameplay;

[Virtual]
public class GameplayState : GameplayStateBase, IMainViewportState
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private IUserInterfaceManager _uiManager;
  [Dependency]
  private ChangelogManager _changelog;
  [Dependency]
  private IConfigurationManager _configurationManager;
  private FpsCounter _fpsCounter;
  private Label _version;
  private readonly GameplayStateLoadController _loadController;

  public MainViewport Viewport => this._uiManager.ActiveScreen.GetWidget<MainViewport>();

  public GameplayState()
  {
    IoCManager.InjectDependencies<GameplayState>(this);
    this._loadController = this._uiManager.GetUIController<GameplayStateLoadController>();
  }

  protected override void Startup()
  {
    base.Startup();
    this.LoadMainScreen();
    this._configurationManager.OnValueChanged<string>(CCVars.UILayout, new Action<string>(this.ReloadMainScreenValueChange), false);
    this._overlayManager.AddOverlay((Overlay) new ShowHandItemOverlay());
    this._fpsCounter = new FpsCounter(this._gameTiming);
    ((Control) this.UserInterfaceManager.PopupRoot).AddChild((Control) this._fpsCounter);
    ((Control) this._fpsCounter).Visible = this._configurationManager.GetCVar<bool>(CCVars.HudFpsCounterVisible);
    this._configurationManager.OnValueChanged<bool>(CCVars.HudFpsCounterVisible, (Action<bool>) (show => ((Control) this._fpsCounter).Visible = show), false);
    this._version = new Label();
    this._version.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFFFFF20", new Color?()));
    this._version.Text = this._changelog.GetClientVersion();
    ((Control) this.UserInterfaceManager.PopupRoot).AddChild((Control) this._version);
    this._configurationManager.OnValueChanged<bool>(CCVars.HudVersionWatermark, (Action<bool>) (show => ((Control) this._version).Visible = this.VersionVisible()), true);
    this._configurationManager.OnValueChanged<bool>(CCVars.ForceClientHudVersionWatermark, (Action<bool>) (show => ((Control) this._version).Visible = this.VersionVisible()), true);
    LayoutContainer.SetPosition((Control) this._version, new Vector2(70f, 0.0f));
  }

  private bool VersionVisible()
  {
    return this._configurationManager.GetCVar<bool>(CCVars.HudVersionWatermark) | this._configurationManager.GetCVar<bool>(CCVars.ForceClientHudVersionWatermark);
  }

  protected override void Shutdown()
  {
    this._overlayManager.RemoveOverlay<ShowHandItemOverlay>();
    base.Shutdown();
    this._eyeManager.MainViewport = (IViewportControl) this.UserInterfaceManager.MainViewport;
    ((Control) this._fpsCounter).Orphan();
    ((Control) this._version).Orphan();
    this._uiManager.ClearWindows();
    this._configurationManager.UnsubValueChanged<string>(CCVars.UILayout, new Action<string>(this.ReloadMainScreenValueChange));
    this.UnloadMainScreen();
  }

  private void ReloadMainScreenValueChange(string _) => this.ReloadMainScreen();

  public void ReloadMainScreen()
  {
    if (this._uiManager.ActiveScreen?.GetWidget<MainViewport>() == null)
      return;
    this.UnloadMainScreen();
    this.LoadMainScreen();
  }

  private void UnloadMainScreen()
  {
    this._loadController.UnloadScreen();
    this._uiManager.UnloadScreen();
  }

  private void LoadMainScreen()
  {
    ScreenType result;
    if (!Enum.TryParse<ScreenType>(this._configurationManager.GetCVar<string>(CCVars.UILayout), out result))
      result = ScreenType.Default;
    switch (result)
    {
      case ScreenType.Default:
        this._uiManager.LoadScreen<DefaultGameScreen>();
        break;
      case ScreenType.Separated:
        this._uiManager.LoadScreen<SeparatedChatGameScreen>();
        break;
      case ScreenType.Battlefront:
        this._uiManager.LoadScreen<BattlefrontGameScreen>();
        break;
    }
    this._loadController.LoadScreen();
  }

  protected override void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
  {
    if (args.Viewport == null)
      base.OnKeyBindStateChanged(new ViewportBoundKeyEventArgs(args.KeyEventArgs, (Control) this.Viewport.Viewport));
    else
      base.OnKeyBindStateChanged(args);
  }
}
