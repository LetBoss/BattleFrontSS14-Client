// Decompiled with JetBrains decompiler
// Type: Content.Client.MainMenu.MainScreen
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.MainMenu.UI;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared;
using Robust.Shared.AuthLib;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Utility;
using System;
using System.Text.RegularExpressions;

#nullable enable
namespace Content.Client.MainMenu;

public sealed class MainScreen : Robust.Client.State.State
{
  [Dependency]
  private IBaseClient _client;
  [Dependency]
  private IClientNetManager _netManager;
  [Dependency]
  private IConfigurationManager _configurationManager;
  [Dependency]
  private IGameController _controllerProxy;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;
  [Dependency]
  private ILogManager _logManager;
  private ISawmill _sawmill;
  private MainMenuControl _mainMenuControl;
  private bool _isConnecting;
  private static readonly Regex IPv6Regex = new Regex("\\[(.*:.*:.*)](?::(\\d+))?");

  protected virtual void Startup()
  {
    this._sawmill = this._logManager.GetSawmill("mainmenu");
    this._mainMenuControl = new MainMenuControl(this._resourceCache, this._configurationManager);
    ((Control) this._userInterfaceManager.StateRoot).AddChild((Control) this._mainMenuControl);
    ((BaseButton) this._mainMenuControl.QuitButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.QuitButtonPressed);
    ((BaseButton) this._mainMenuControl.OptionsButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OptionsButtonPressed);
    ((BaseButton) this._mainMenuControl.DirectConnectButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.DirectConnectButtonPressed);
    this._mainMenuControl.AddressBox.OnTextEntered += new Action<LineEdit.LineEditEventArgs>(this.AddressBoxEntered);
    ((BaseButton) this._mainMenuControl.ChangelogButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.ChangelogButtonPressed);
    this._client.RunLevelChanged += new EventHandler<RunLevelChangedEventArgs>(this.RunLevelChanged);
  }

  protected virtual void Shutdown()
  {
    this._client.RunLevelChanged -= new EventHandler<RunLevelChangedEventArgs>(this.RunLevelChanged);
    this._netManager.ConnectFailed -= new EventHandler<NetConnectFailArgs>(this._onConnectFailed);
    this._mainMenuControl.Orphan();
  }

  private void ChangelogButtonPressed(BaseButton.ButtonEventArgs args)
  {
    this._userInterfaceManager.GetUIController<ChangelogUIController>().ToggleWindow();
  }

  private void OptionsButtonPressed(BaseButton.ButtonEventArgs args)
  {
    this._userInterfaceManager.GetUIController<OptionsUIController>().ToggleWindow();
  }

  private void QuitButtonPressed(BaseButton.ButtonEventArgs args)
  {
    this._controllerProxy.Shutdown((string) null);
  }

  private void DirectConnectButtonPressed(BaseButton.ButtonEventArgs args)
  {
    this.TryConnect(this._mainMenuControl.AddressBox.Text);
  }

  private void AddressBoxEntered(LineEdit.LineEditEventArgs args)
  {
    if (this._isConnecting)
      return;
    this.TryConnect(args.Text);
  }

  private void TryConnect(string address)
  {
    string name = this._mainMenuControl.UsernameBox.Text.Trim();
    UsernameHelpers.UsernameInvalidReason reason;
    if (!UsernameHelpers.IsNameValid(name, out reason))
    {
      this._userInterfaceManager.Popup(Loc.GetString("main-menu-invalid-username-with-reason", new (string, object)[1]
      {
        ("invalidReason", (object) Loc.GetString(UsernameHelpersExt.ToText(reason)))
      }), Loc.GetString("main-menu-invalid-username"), true);
    }
    else
    {
      if (this._mainMenuControl.UsernameBox.Text != this._configurationManager.GetCVar<string>(CVars.PlayerName))
      {
        this._configurationManager.SetCVar<string>(CVars.PlayerName, name, false);
        this._configurationManager.SaveToFile();
      }
      this._setConnectingState(true);
      this._netManager.ConnectFailed += new EventHandler<NetConnectFailArgs>(this._onConnectFailed);
      try
      {
        string ip;
        ushort port;
        this.ParseAddress(address, out ip, out port);
        this._client.ConnectToServer(ip, port);
      }
      catch (ArgumentException ex)
      {
        this._userInterfaceManager.Popup("Unable to connect: " + ex.Message, "Connection error.", true);
        this._sawmill.Warning(ex.ToString());
        this._netManager.ConnectFailed -= new EventHandler<NetConnectFailArgs>(this._onConnectFailed);
        this._setConnectingState(false);
      }
    }
  }

  private void RunLevelChanged(object? obj, RunLevelChangedEventArgs args)
  {
    ClientRunLevel newLevel = args.NewLevel;
    if (newLevel != 1)
    {
      if (newLevel != 2)
        return;
      this._setConnectingState(true);
    }
    else
    {
      this._setConnectingState(false);
      this._netManager.ConnectFailed -= new EventHandler<NetConnectFailArgs>(this._onConnectFailed);
    }
  }

  private void ParseAddress(string address, out string ip, out ushort port)
  {
    Match match = MainScreen.IPv6Regex.Match(address);
    if (match != Match.Empty)
    {
      ip = match.Groups[1].Value;
      if (!match.Groups[2].Success)
        port = this._client.DefaultPort;
      else if (!ushort.TryParse(match.Groups[2].Value, out port))
        throw new ArgumentException("Not a valid port.");
    }
    else
    {
      string[] strArray = address.Split(':');
      ip = address;
      port = this._client.DefaultPort;
      if (strArray.Length > 2)
        throw new ArgumentException("Not a valid Address.");
      if (strArray.Length != 2)
        return;
      ip = strArray[0];
      if (!ushort.TryParse(strArray[1], out port))
        throw new ArgumentException("Not a valid port.");
    }
  }

  private void _onConnectFailed(object? _, NetConnectFailArgs args)
  {
    this._userInterfaceManager.Popup(Loc.GetString("main-menu-failed-to-connect", new (string, object)[1]
    {
      ("reason", (object) args.Reason)
    }), (string) null, true);
    this._netManager.ConnectFailed -= new EventHandler<NetConnectFailArgs>(this._onConnectFailed);
    this._setConnectingState(false);
  }

  private void _setConnectingState(bool state)
  {
    this._isConnecting = state;
    ((BaseButton) this._mainMenuControl.DirectConnectButton).Disabled = state;
  }
}
