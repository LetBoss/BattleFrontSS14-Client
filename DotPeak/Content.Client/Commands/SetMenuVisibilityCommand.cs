// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.SetMenuVisibilityCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Verbs;
using Content.Shared.Verbs;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Commands;

internal sealed class SetMenuVisibilityCommand : LocalizedCommands
{
  [Dependency]
  private IEntitySystemManager _entitySystemManager;

  public virtual string Command => "menuvis";

  public virtual string Help
  {
    get
    {
      return this.LocalizationManager.GetString($"cmd-{base.Command}-help", ("command", (object) base.Command));
    }
  }

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    MenuVisibility visibility;
    if (!this.TryParseArguments(shell, args, out visibility))
      return;
    this._entitySystemManager.GetEntitySystem<VerbSystem>().Visibility = visibility;
  }

  private bool TryParseArguments(IConsoleShell shell, string[] args, out MenuVisibility visibility)
  {
    visibility = MenuVisibility.Default;
    foreach (string str in args)
    {
      switch (str.ToLower())
      {
        case "nofov":
          visibility |= MenuVisibility.NoFov;
          break;
        case "incontainer":
          visibility |= MenuVisibility.InContainer;
          break;
        case "invisible":
          visibility |= MenuVisibility.Invisible;
          break;
        case "all":
          visibility |= MenuVisibility.All;
          break;
        default:
          shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error", ("arg", (object) str)));
          return false;
      }
    }
    return true;
  }
}
