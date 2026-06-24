// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.CreditsCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Credits;
using Content.Shared.Administration;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;

#nullable enable
namespace Content.Client.Commands;

[AnyCommand]
public sealed class CreditsCommand : LocalizedCommands
{
  public virtual string Command => "credits";

  public virtual string Help
  {
    get
    {
      return this.LocalizationManager.GetString($"cmd-{base.Command}-help", ("command", (object) base.Command));
    }
  }

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    ((BaseWindow) new CreditsWindow()).Open();
  }
}
