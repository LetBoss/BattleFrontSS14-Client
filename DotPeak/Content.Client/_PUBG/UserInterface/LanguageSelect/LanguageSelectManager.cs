// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.LanguageSelect.LanguageSelectManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.Settings;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.LanguageSelect;

public sealed class LanguageSelectManager
{
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IClyde _clyde;
  private LanguageSelectWindow? _window;
  private ISawmill _sawmill;

  public void Initialize()
  {
    this._sawmill = Logger.GetSawmill("language-select");
    this.TryApplyGeneralAutoSettings();
    this.TryShowLanguageSelectOnFirstRun();
  }

  public void ShowLanguageSelect(bool forceEnglishPrompt = false)
  {
    if (this._window != null)
    {
      this._sawmill.Warning("Language select window already open");
    }
    else
    {
      this._window = new LanguageSelectWindow(forceEnglishPrompt);
      ((BaseWindow) this._window).OnClose += new Action(this.OnWindowClosed);
      ((Control) this._ui.WindowRoot).AddChild((Control) this._window);
      LayoutContainer.SetAnchorPreset((Control) this._window, (LayoutContainer.LayoutPreset) 8, false);
      ((BaseWindow) this._window).Open();
    }
  }

  private void OnWindowClosed() => this._window = (LanguageSelectWindow) null;

  private void TryApplyGeneralAutoSettings()
  {
    int cvar = this._cfg.GetCVar<int>(CCVars.PubgGeneralAutoSettingsVersion);
    if (cvar >= 3)
      return;
    Vector2i screenSize = this._clyde.ScreenSize;
    if (screenSize.X <= 0 || screenSize.Y <= 0)
      return;
    PubgGeneralAutoSettings.Apply(this._cfg, screenSize, cvar);
    this._cfg.SetCVar<int>(CCVars.PubgGeneralAutoSettingsVersion, 3, false);
    this._cfg.SaveToFile();
  }

  private void TryShowLanguageSelectOnFirstRun()
  {
    if (!string.Equals(this._cfg.GetCVar<string>(CCVars.Language), "auto", StringComparison.OrdinalIgnoreCase))
      return;
    this.ShowLanguageSelect(true);
  }
}
