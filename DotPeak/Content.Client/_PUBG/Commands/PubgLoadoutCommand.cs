// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Commands.PubgLoadoutCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.UserInterface.Systems.Loadout;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._PUBG.Commands;

[AnyCommand]
public sealed class PubgLoadoutCommand : IConsoleCommand
{
  public string Command => "pubg_loadout";

  public string Description => "Open PUBG loadout UI";

  public string Help => "Usage: pubg_loadout";

  public void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    IoCManager.Resolve<IUserInterfaceManager>().GetUIController<PubgLoadoutUIController>().ToggleForTest();
  }
}
