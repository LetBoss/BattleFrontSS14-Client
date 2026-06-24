// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.EscapeMenu.OptionsUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Options.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Client.UserInterface.Systems.EscapeMenu;

public sealed class OptionsUIController : UIController
{
  [Dependency]
  private IConsoleHost _con;
  private OptionsMenu _optionsWindow;

  public virtual void Initialize()
  {
    this._con.RegisterCommand("options", Loc.GetString("cmd-options-desc"), Loc.GetString("cmd-options-help"), new ConCommandCallback(this.OptionsCommand), false);
  }

  private void OptionsCommand(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length == 0)
    {
      this.ToggleWindow();
    }
    else
    {
      this.OpenWindow();
      int result;
      if (!int.TryParse(args[0], out result))
        shell.WriteError(Loc.GetString("cmd-parse-failure-integer", new (string, object)[1]
        {
          ("arg", (object) args[0])
        }));
      else
        this._optionsWindow.Tabs.CurrentTab = result;
    }
  }

  private void EnsureWindow()
  {
    OptionsMenu optionsWindow = this._optionsWindow;
    if (optionsWindow != null && !((Control) optionsWindow).Disposed)
      return;
    this._optionsWindow = this.UIManager.CreateWindow<OptionsMenu>();
  }

  public void OpenWindow()
  {
    this.EnsureWindow();
    this._optionsWindow.UpdateTabs();
    ((BaseWindow) this._optionsWindow).OpenCentered();
    ((BaseWindow) this._optionsWindow).MoveToFront();
  }

  public void ToggleWindow()
  {
    this.EnsureWindow();
    if (((BaseWindow) this._optionsWindow).IsOpen)
      ((BaseWindow) this._optionsWindow).Close();
    else
      this.OpenWindow();
  }
}
