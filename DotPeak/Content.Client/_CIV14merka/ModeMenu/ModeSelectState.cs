// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.ModeMenu.ModeSelectState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.ModeMenu.UI;
using Content.Client.GameTicking.Managers;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client._CIV14merka.ModeMenu;

public sealed class ModeSelectState : Robust.Client.State.State
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;
  private ClientGameTicker _gameTicker;

  protected virtual Type? LinkedScreenType => typeof (ModeSelectGui);

  public ModeSelectGui? Screen { get; private set; }

  protected virtual void Startup()
  {
    if (this._userInterfaceManager.ActiveScreen == null)
      return;
    this.Screen = (ModeSelectGui) this._userInterfaceManager.ActiveScreen;
    this._gameTicker = this._entityManager.System<ClientGameTicker>();
    ((BaseButton) this.Screen.PubgButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnPubgPressed);
    ((BaseButton) this.Screen.Civ14Button).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnCivPressed);
    this._gameTicker.ModeMenuStatusUpdated += new Action(this.UpdateUi);
    this.UpdateUi();
  }

  protected virtual void Shutdown()
  {
    if (this.Screen != null)
    {
      ((BaseButton) this.Screen.PubgButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnPubgPressed);
      ((BaseButton) this.Screen.Civ14Button).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnCivPressed);
    }
    if (this._gameTicker != null)
      this._gameTicker.ModeMenuStatusUpdated -= new Action(this.UpdateUi);
    this.Screen = (ModeSelectGui) null;
  }

  private void OnPubgPressed(BaseButton.ButtonEventArgs args) => this._gameTicker.SelectPubgMode();

  private void OnCivPressed(BaseButton.ButtonEventArgs args) => this._gameTicker.SelectCiv14Mode();

  private void UpdateUi()
  {
    if (this.Screen == null)
      return;
    this.Screen.ServerOnlineLabel.Text = Loc.GetString("civ-ui-mode-online", new (string, object)[1]
    {
      ("count", (object) this._gameTicker.ServerOnlineCount)
    });
    ModeSelectState.UpdateModeButton(this.Screen.PubgButton, "PUBG", this._gameTicker.PubgModeOnlineCount, this._gameTicker.IsPubgModeSelectable);
    ModeSelectState.UpdateModeButton(this.Screen.Civ14Button, "TDM", this._gameTicker.Civ14ModeOnlineCount, this._gameTicker.IsCiv14ModeSelectable);
  }

  private static void UpdateModeButton(
    Button button,
    string modeName,
    int onlineCount,
    bool enabled)
  {
    ((BaseButton) button).Disabled = !enabled;
    Button button1 = button;
    string str;
    if (!enabled)
      str = Loc.GetString("civ-ui-mode-button-disabled", new (string, object)[1]
      {
        ("mode", (object) modeName)
      });
    else
      str = Loc.GetString("civ-ui-mode-button-online", new (string, object)[2]
      {
        ("mode", (object) modeName),
        ("count", (object) onlineCount)
      });
    button1.Text = str;
  }
}
