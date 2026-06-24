// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Commands.LeaderboardCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.Leaderboard;
using Content.Shared.Administration;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using System;

#nullable enable
namespace Content.Client._PUBG.Commands;

[AnyCommand]
public sealed class LeaderboardCommand : IConsoleCommand
{
  private LeaderboardWindow? _window;

  public string Command => "leaderboard";

  public string Description => "дуфвукищфкв";

  public string Help => "Usage: leaderboard";

  public void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (this._window == null)
    {
      this._window = new LeaderboardWindow();
      ((BaseWindow) this._window).OnClose += (Action) (() => this._window = (LeaderboardWindow) null);
    }
    if (((BaseWindow) this._window).IsOpen)
      ((BaseWindow) this._window).Close();
    else
      ((BaseWindow) this._window).OpenCentered();
  }
}
