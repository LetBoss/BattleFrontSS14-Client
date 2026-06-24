// Decompiled with JetBrains decompiler
// Type: Content.Client.Shuttles.Commands.ShowEmergencyShuttleCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Shuttles.Systems;
using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Shuttles.Commands;

public sealed class ShowEmergencyShuttleCommand : LocalizedEntityCommands
{
  [Dependency]
  private ShuttleSystem _shuttle;

  public virtual string Command => "showemergencyshuttle";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    ShuttleSystem shuttle = this._shuttle;
    shuttle.EnableShuttlePosition = !shuttle.EnableShuttlePosition;
    shell.WriteLine(((LocalizedCommands) this).Loc.GetString("cmd-showemergencyshuttle-status", ("status", (object) this._shuttle.EnableShuttlePosition)));
  }
}
