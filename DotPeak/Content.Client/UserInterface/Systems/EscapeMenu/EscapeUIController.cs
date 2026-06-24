// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.EscapeMenu.EscapeUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.LinkAccount;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Guidebook;
using Content.Client.UserInterface.Systems.Info;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.CCVar;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.EscapeMenu;

public sealed class EscapeUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  [Dependency]
  private IClientConsoleHost _console;
  [Dependency]
  private IUriOpener _uri;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private ChangelogUIController _changelog;
  [Dependency]
  private InfoUIController _info;
  [Dependency]
  private OptionsUIController _options;
  [Dependency]
  private GuidebookUIController _guidebook;
  [Dependency]
  private LinkAccountManager _linkAccount;
  private Content.Client.Options.UI.EscapeMenu? _escapeWindow;

  private MenuButton? EscapeButton
  {
    get => this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.EscapeButton;
  }

  public virtual void Initialize()
  {
    this._linkAccount.Updated += (Action) (() =>
    {
      if (this._escapeWindow == null)
        return;
      ((Control) this._escapeWindow.PatronPerksButton).Visible = this._linkAccount.CanViewPatronPerks();
    });
  }

  public void UnloadButton()
  {
    if (this.EscapeButton == null)
      return;
    ((BaseButton) this.EscapeButton).Pressed = false;
    ((BaseButton) this.EscapeButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.EscapeButtonOnOnPressed);
  }

  public void LoadButton()
  {
    if (this.EscapeButton == null)
      return;
    ((BaseButton) this.EscapeButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.EscapeButtonOnOnPressed);
  }

  private void ActivateButton() => ((BaseButton) this.EscapeButton).SetClickPressed(true);

  private void DeactivateButton() => ((BaseButton) this.EscapeButton).SetClickPressed(false);

  public void OnStateEntered(GameplayState state)
  {
    this._escapeWindow = this.UIManager.CreateWindow<Content.Client.Options.UI.EscapeMenu>();
    ((BaseWindow) this._escapeWindow).OnClose += new Action(this.DeactivateButton);
    ((BaseWindow) this._escapeWindow).OnOpen += new Action(this.ActivateButton);
    ((BaseButton) this._escapeWindow.ChangelogButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.CloseEscapeWindow();
      this._changelog.ToggleWindow();
    });
    ((BaseButton) this._escapeWindow.DiscordButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._uri.OpenUri("https://discord.gg/xdQ4vSKRB8"));
    ((Control) this._escapeWindow.PatronPerksButton).Visible = this._linkAccount.CanViewPatronPerks();
    ((BaseButton) this._escapeWindow.PatronPerksButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.CloseEscapeWindow();
      this.UIManager.GetUIController<LinkAccountUIController>().TogglePatronPerksWindow();
    });
    ((BaseButton) this._escapeWindow.RulesButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.CloseEscapeWindow();
      this._info.OpenWindow();
    });
    ((BaseButton) this._escapeWindow.DisconnectButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.CloseEscapeWindow();
      ((IConsoleHost) this._console).ExecuteCommand("disconnect");
    });
    ((BaseButton) this._escapeWindow.OptionsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.CloseEscapeWindow();
      this._options.OpenWindow();
    });
    ((BaseButton) this._escapeWindow.QuitButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.CloseEscapeWindow();
      ((IConsoleHost) this._console).ExecuteCommand("quit");
    });
    ((BaseButton) this._escapeWindow.WikiButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._uri.OpenUri(this._cfg.GetCVar<string>(CCVars.InfoLinksWiki)));
    ((BaseButton) this._escapeWindow.GuidebookButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._guidebook.ToggleGuidebook());
    ((Control) this._escapeWindow.WikiButton).Visible = this._cfg.GetCVar<string>(CCVars.InfoLinksWiki) != "";
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(EngineKeyFunctions.EscapeMenu, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnStateEntered\u003Eb__16_9)), (StateInputCmdDelegate) null, true, true)).Register<EscapeUIController>();
  }

  public void OnStateExited(GameplayState state)
  {
    if (this._escapeWindow != null)
    {
      if (!((Control) this._escapeWindow).Disposed)
        ((Control) this._escapeWindow).Orphan();
      this._escapeWindow = (Content.Client.Options.UI.EscapeMenu) null;
    }
    CommandBinds.Unregister<EscapeUIController>();
  }

  private void EscapeButtonOnOnPressed(BaseButton.ButtonEventArgs obj) => this.ToggleWindow();

  private void CloseEscapeWindow() => ((BaseWindow) this._escapeWindow)?.Close();

  public void ToggleWindow()
  {
    if (this._escapeWindow == null)
      return;
    if (((BaseWindow) this._escapeWindow).IsOpen)
    {
      this.CloseEscapeWindow();
      ((BaseButton) this.EscapeButton).Pressed = false;
    }
    else
    {
      ((BaseWindow) this._escapeWindow).OpenCentered();
      ((BaseButton) this.EscapeButton).Pressed = true;
    }
  }
}
