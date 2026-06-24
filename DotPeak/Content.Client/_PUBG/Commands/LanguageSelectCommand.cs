// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Commands.LanguageSelectCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.UserInterface.LanguageSelect;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._PUBG.Commands;

[AnyCommand]
public sealed class LanguageSelectCommand : IConsoleCommand
{
  public string Command => "languageselect";

  public string Description => "language";

  public string Help => "Usage: languageselect";

  public void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    IoCManager.Resolve<LanguageSelectManager>().ShowLanguageSelect();
  }
}
