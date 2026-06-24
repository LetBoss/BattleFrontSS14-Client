// Decompiled with JetBrains decompiler
// Type: Content.Client.Fullscreen.FullscreenHook
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Input;
using Robust.Client.Input;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Fullscreen;

public sealed class FullscreenHook
{
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private ILogManager _logManager;
  private ISawmill _sawmill;

  public void Initialize()
  {
    // ISSUE: method pointer
    this._inputManager.SetInputCommand(ContentKeyFunctions.ToggleFullscreen, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(ToggleFullscreen)), (StateInputCmdDelegate) null, true, true));
    this._sawmill = this._logManager.GetSawmill("fullscreen");
  }

  private void ToggleFullscreen(ICommonSession? session)
  {
    int cvar = this._cfg.GetCVar<int>(CVars.DisplayWindowMode);
    switch (cvar)
    {
      case 0:
        this._cfg.SetCVar<int>(CVars.DisplayWindowMode, 1, false);
        this._sawmill.Info("Switched to Fullscreen mode");
        break;
      case 1:
        this._cfg.SetCVar<int>(CVars.DisplayWindowMode, 0, false);
        this._sawmill.Info("Switched to Windowed mode");
        break;
      default:
        throw new InvalidOperationException($"Unexpected WindowMode value: {cvar}");
    }
  }
}
