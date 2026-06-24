// Decompiled with JetBrains decompiler
// Type: Content.Client.GameTicking.Managers.TitleWindowManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.GameTicking.Managers;

public sealed class TitleWindowManager
{
  [Dependency]
  private IBaseClient _client;
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IGameController _gameController;

  public void Initialize()
  {
    this._cfg.OnValueChanged<string>(CVars.GameHostName, new Action<string>(this.OnHostnameChange), true);
    this._cfg.OnValueChanged<bool>(CCVars.GameHostnameInTitlebar, new Action<bool>(this.OnHostnameTitleChange), true);
    this._client.RunLevelChanged += new EventHandler<RunLevelChangedEventArgs>(this.OnRunLevelChangedChange);
  }

  public void Shutdown()
  {
    this._cfg.UnsubValueChanged<string>(CVars.GameHostName, new Action<string>(this.OnHostnameChange));
    this._cfg.UnsubValueChanged<bool>(CCVars.GameHostnameInTitlebar, new Action<bool>(this.OnHostnameTitleChange));
  }

  private void OnHostnameChange(string hostname)
  {
    string str = this._gameController.GameTitle();
    if (this._client.RunLevel == 1)
      this._clyde.SetWindowTitle(str);
    else if (this._cfg.GetCVar<bool>(CCVars.GameHostnameInTitlebar))
      this._clyde.SetWindowTitle($"{hostname} - {str}");
    else
      this._clyde.SetWindowTitle(str);
  }

  private void OnHostnameTitleChange(bool colonthree)
  {
    this.OnHostnameChange(this._cfg.GetCVar<string>(CVars.GameHostName));
  }

  private void OnRunLevelChangedChange(
    object? sender,
    RunLevelChangedEventArgs runLevelChangedEventArgs)
  {
    this.OnHostnameChange(this._cfg.GetCVar<string>(CVars.GameHostName));
  }
}
