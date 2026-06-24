// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Connection.QueueState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._PUBG.Connection;

public sealed class QueueState : Robust.Client.State.State
{
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;
  private QueueGui? _gui;

  protected virtual Type? LinkedScreenType => typeof (QueueGui);

  protected virtual void Startup()
  {
    this._gui = (QueueGui) this._userInterfaceManager.ActiveScreen;
  }

  protected virtual void Shutdown() => this._gui = (QueueGui) null;

  public void UpdatePosition(int position, int total) => this._gui?.UpdatePosition(position, total);
}
