// Decompiled with JetBrains decompiler
// Type: Content.Client.Replay.UI.Loading.ReplayLoadingFailed
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Stylesheets;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Replay.UI.Loading;

public sealed class ReplayLoadingFailed : Robust.Client.State.State
{
  [Dependency]
  private IStylesheetManager _stylesheetManager;
  [Dependency]
  private IUserInterfaceManager _userInterface;
  private ReplayLoadingFailedControl? _control;

  public void SetData(Exception exception, Action? cancelPressed, Action? retryPressed)
  {
    this._control.SetData(exception, cancelPressed, retryPressed);
  }

  protected virtual void Startup()
  {
    this._control = new ReplayLoadingFailedControl(this._stylesheetManager);
    ((Control) this._userInterface.StateRoot).AddChild((Control) this._control);
  }

  protected virtual void Shutdown() => this._control?.Orphan();
}
