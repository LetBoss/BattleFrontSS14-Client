// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.LoadActionsCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Actions;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Commands;

[AnyCommand]
public sealed class LoadActionsCommand : LocalizedCommands
{
  [Dependency]
  private IEntitySystemManager _entitySystemManager;

  public virtual string Command => "loadacts";

  public virtual string Help
  {
    get
    {
      return this.LocalizationManager.GetString($"cmd-{base.Command}-help", ("command", (object) base.Command));
    }
  }

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length != 1)
    {
      shell.WriteLine(base.Help);
    }
    else
    {
      try
      {
        this._entitySystemManager.GetEntitySystem<ActionsSystem>().LoadActionAssignments(args[0], true);
      }
      catch
      {
        shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error"));
      }
    }
  }
}
