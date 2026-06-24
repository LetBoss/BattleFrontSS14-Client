// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.ToggleOutlineCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Commands;

[AnyCommand]
public sealed class ToggleOutlineCommand : LocalizedCommands
{
  [Dependency]
  private IConfigurationManager _configurationManager;

  public virtual string Command => "toggleoutline";

  public virtual string Help
  {
    get
    {
      return this.LocalizationManager.GetString($"cmd-{base.Command}-help", ("command", (object) base.Command));
    }
  }

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    CVarDef<bool> outlineEnabled = CCVars.OutlineEnabled;
    bool cvar = this._configurationManager.GetCVar<bool>(outlineEnabled);
    this._configurationManager.SetCVar<bool>(outlineEnabled, !cvar, false);
    shell.WriteLine(this.LocalizationManager.GetString($"cmd-{base.Command}-notify", ("state", (object) this._configurationManager.GetCVar<bool>(outlineEnabled))));
  }
}
