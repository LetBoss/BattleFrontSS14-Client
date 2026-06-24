// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.RemoveMapCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class RemoveMapCommand : LocalizedEntityCommands
{
  [Dependency]
  private readonly IEntitySystemManager _systems;

  public override string Command => "rmmap";

  public override bool RequireServerOrSingleplayer => true;

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length != 1)
    {
      shell.WriteError("Wrong number of args.");
    }
    else
    {
      MapId mapId = new MapId(int.Parse(args[0]));
      SharedMapSystem entitySystem = this._systems.GetEntitySystem<SharedMapSystem>();
      if (!entitySystem.MapExists(new MapId?(mapId)))
      {
        shell.WriteError($"Map {mapId.Value} does not exist.");
      }
      else
      {
        entitySystem.DeleteMap(mapId);
        shell.WriteLine($"Map {mapId.Value} was removed.");
      }
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return args.Length != 1 ? CompletionResult.Empty : CompletionResult.FromHintOptions(CompletionHelper.MapIds(args[0], (IEntityManager) this.EntityManager), this.LocalizationManager.GetString("generic-map"));
  }
}
