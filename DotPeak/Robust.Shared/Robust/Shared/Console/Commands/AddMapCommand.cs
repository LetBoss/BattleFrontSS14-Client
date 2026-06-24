// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.AddMapCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class AddMapCommand : LocalizedEntityCommands
{
  [Dependency]
  private readonly SharedMapSystem _mapSystem;

  public override string Command => "addmap";

  public override bool RequireServerOrSingleplayer => true;

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length < 1)
      return;
    MapId mapId = new MapId(int.Parse(args[0]));
    if (!this._mapSystem.MapExists(new MapId?(mapId)))
    {
      bool runMapInit = args.Length < 2 || !bool.Parse(args[1]);
      this.EntityManager.System<SharedMapSystem>().CreateMap(mapId, runMapInit);
      shell.WriteLine($"Map with ID {mapId} created.");
    }
    else
      shell.WriteError($"Map with ID {mapId} already exists!");
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    switch (args.Length)
    {
      case 1:
        // ISSUE: object of a compiler-generated type is created
        return CompletionResult.FromHintOptions((IEnumerable<CompletionOption>) new \u003C\u003Ez__ReadOnlySingleElementList<CompletionOption>(new CompletionOption($"{this._mapSystem.GetNextMapId()}")), this.LocalizationManager.GetString("generic-mapid"));
      case 2:
        return CompletionResult.FromHint(this.LocalizationManager.GetString("cmd-addmap-hint-2"));
      default:
        return CompletionResult.Empty;
    }
  }
}
