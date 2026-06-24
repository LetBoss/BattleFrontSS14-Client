// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Commands.LanguageAutoCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._PUBG.Commands;

[AnyCommand]
public sealed class LanguageAutoCommand : IConsoleCommand
{
  public string Command => "languageauto";

  public string Description => "Reset language setting to auto.";

  public string Help => "Usage: languageauto";

  public void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    IConfigurationManager iconfigurationManager = IoCManager.Resolve<IConfigurationManager>();
    iconfigurationManager.SetCVar<string>(CCVars.Language, "auto", false);
    iconfigurationManager.SaveToFile();
    shell.WriteLine("locale.language set to auto");
  }
}
