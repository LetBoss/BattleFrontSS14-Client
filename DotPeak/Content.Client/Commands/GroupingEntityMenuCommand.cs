// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.GroupingEntityMenuCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Commands;

public sealed class GroupingEntityMenuCommand : LocalizedCommands
{
  [Dependency]
  private IConfigurationManager _configurationManager;

  public virtual string Command => "entitymenug";

  public virtual string Help
  {
    get
    {
      return this.LocalizationManager.GetString($"cmd-{base.Command}-help", ("command", (object) base.Command), ("groupingTypesCount", (object) 2));
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
      int result;
      if (!int.TryParse(args[0], out result))
        shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error", ("arg", (object) args[0])));
      else if (result < 0 || result > 1)
      {
        shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error", ("arg", (object) args[0])));
      }
      else
      {
        CVarDef<int> menuGroupingType = CCVars.EntityMenuGroupingType;
        this._configurationManager.SetCVar<int>(menuGroupingType, result, false);
        shell.WriteLine(this.LocalizationManager.GetString($"cmd-{base.Command}-notify", ("cvar", (object) this._configurationManager.GetCVar<int>(menuGroupingType))));
      }
    }
  }
}
