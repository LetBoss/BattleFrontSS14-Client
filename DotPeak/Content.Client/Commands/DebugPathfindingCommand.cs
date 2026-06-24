// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.DebugPathfindingCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.NPC;
using Content.Shared.NPC;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Commands;

public sealed class DebugPathfindingCommand : LocalizedCommands
{
  [Dependency]
  private IEntitySystemManager _entitySystemManager;

  public virtual string Command => "pathfinder";

  public virtual string Help
  {
    get
    {
      return this.LocalizationManager.GetString($"cmd-{base.Command}-help", ("command", (object) base.Command));
    }
  }

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    PathfindingSystem entitySystem = this._entitySystemManager.GetEntitySystem<PathfindingSystem>();
    if (args.Length == 0)
    {
      entitySystem.Modes = PathfindingDebugMode.None;
    }
    else
    {
      foreach (string str in args)
      {
        PathfindingDebugMode result;
        if (!Enum.TryParse<PathfindingDebugMode>(str, out result))
        {
          shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error", ("arg", (object) str)));
        }
        else
        {
          entitySystem.Modes ^= result;
          shell.WriteLine(this.LocalizationManager.GetString($"cmd-{base.Command}-notify", ("arg", (object) str), ("newMode", (object) ((entitySystem.Modes & result) != 0))));
        }
      }
    }
  }

  public virtual CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    if (args.Length > 1)
      return CompletionResult.Empty;
    List<PathfindingDebugMode> list = ((IEnumerable<PathfindingDebugMode>) Enum.GetValues<PathfindingDebugMode>()).ToList<PathfindingDebugMode>();
    List<CompletionOption> completionOptionList = new List<CompletionOption>();
    foreach (PathfindingDebugMode pathfindingDebugMode in list)
    {
      if (pathfindingDebugMode != PathfindingDebugMode.None)
        completionOptionList.Add(new CompletionOption(pathfindingDebugMode.ToString(), (string) null, (CompletionOptionFlags) 0));
    }
    return CompletionResult.FromOptions((IEnumerable<CompletionOption>) completionOptionList);
  }
}
