// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.EscapeMenu.ChangelogUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Changelog;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;

#nullable enable
namespace Content.Client.UserInterface.Systems.EscapeMenu;

public sealed class ChangelogUIController : UIController
{
  private ChangelogWindow _changeLogWindow;

  public void OpenWindow()
  {
    this.EnsureWindow();
    this._changeLogWindow.OpenCentered();
    this._changeLogWindow.MoveToFront();
  }

  private void EnsureWindow()
  {
    ChangelogWindow changeLogWindow = this._changeLogWindow;
    if (changeLogWindow != null && !((Control) changeLogWindow).Disposed)
      return;
    this._changeLogWindow = this.UIManager.CreateWindow<ChangelogWindow>();
  }

  public void ToggleWindow()
  {
    this.EnsureWindow();
    if (this._changeLogWindow.IsOpen)
      this._changeLogWindow.Close();
    else
      this.OpenWindow();
  }
}
