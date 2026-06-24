// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Areas.ShowAreasCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._RMC14.Areas;

public sealed class ShowAreasCommand : IConsoleCommand
{
  [Dependency]
  private IEntityManager _entities;

  public string Command => "showareas";

  public string Description => "Shows areas depending on their properties.";

  public string Help => $"Usage: {this.Command} disable | {this.Command} cas";

  public void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length == 0 || args.Length > 1)
    {
      shell.WriteLine(this.Help);
    }
    else
    {
      AreasCommandSystem areasCommandSystem = this._entities.System<AreasCommandSystem>();
      switch (args[0].ToLowerInvariant())
      {
        case "cas":
          areasCommandSystem.ShowCAS = !areasCommandSystem.ShowCAS;
          shell.WriteLine($"Showing areas with {"ShowCAS"}: {areasCommandSystem.ShowCAS}");
          areasCommandSystem.Enabled = true;
          break;
        case "disable":
          areasCommandSystem.Enabled = false;
          shell.WriteLine("Disabled area visualizer");
          break;
        default:
          shell.WriteLine(this.Help);
          break;
      }
    }
  }
}
