// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.OpenAHelpCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Bwoink;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Client.Commands;

[AnyCommand]
public sealed class OpenAHelpCommand : LocalizedCommands
{
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;

  public virtual string Command => "openahelp";

  public virtual string Help
  {
    get
    {
      return this.LocalizationManager.GetString($"cmd-{base.Command}-help", ("command", (object) base.Command));
    }
  }

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length >= 2)
      shell.WriteLine(base.Help);
    else if (args.Length == 0)
    {
      this._userInterfaceManager.GetUIController<AHelpUIController>().Open();
    }
    else
    {
      Guid result;
      if (Guid.TryParse(args[0], out result))
      {
        NetUserId userId;
        // ISSUE: explicit constructor call
        ((NetUserId) ref userId).\u002Ector(result);
        this._userInterfaceManager.GetUIController<AHelpUIController>().Open(userId);
      }
      else
        shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error"));
    }
  }
}
