// Decompiled with JetBrains decompiler
// Type: Content.Client.Changelog.ChangelogCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.EscapeMenu;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Changelog;

[AnyCommand]
public sealed class ChangelogCommand : LocalizedCommands
{
  [Dependency]
  private IUserInterfaceManager _uiManager;

  public virtual string Command => "changelog";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    this._uiManager.GetUIController<ChangelogUIController>().OpenWindow();
  }
}
