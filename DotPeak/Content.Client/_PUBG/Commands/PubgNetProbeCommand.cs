// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Commands.PubgNetProbeCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.NetProbe;
using Content.Shared.Administration;
using Robust.Client.Player;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

#nullable enable
namespace Content.Client._PUBG.Commands;

[AnyCommand]
public sealed class PubgNetProbeCommand : LocalizedCommands
{
  [Dependency]
  private IEntitySystemManager _systems;
  [Dependency]
  private IPlayerManager _player;

  public virtual string Command => "pubgnetprobe";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length == 2)
    {
      SessionStatus? status = ((ISharedPlayerManager) this._player).LocalSession?.Status;
      if (!status.HasValue || status.GetValueOrDefault() - 2 > 1)
        shell.WriteError(this.Loc.GetString("cmd-pubgnetprobe-not-connected"));
      else
        shell.RemoteExecuteCommand(argStr);
    }
    else if (args.Length != 1)
    {
      shell.WriteError(this.Loc.GetString("cmd-pubgnetprobe-usage"));
    }
    else
    {
      int result;
      if (!int.TryParse(args[0], out result))
        shell.WriteError(this.Loc.GetString("cmd-pubgnetprobe-invalid-number", ("value", (object) args[0])));
      else if (result <= 0)
        shell.WriteError(this.Loc.GetString("cmd-pubgnetprobe-not-positive"));
      else if (result > 3072 /*0x0C00*/)
        shell.WriteError(this.Loc.GetString("cmd-pubgnetprobe-cap", ("limitKb", (object) 3072 /*0x0C00*/)));
      else
        this._systems.GetEntitySystem<PubgNetProbeSystem>().StartLocal(result);
    }
  }
}
