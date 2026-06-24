// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.RunMapInitCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using System;
using System.Globalization;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class RunMapInitCommand : LocalizedEntityCommands
{
  [Dependency]
  private readonly SharedMapSystem _mapSystem;

  public override string Command => "mapinit";

  public override bool RequireServerOrSingleplayer => true;

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length != 1)
    {
      shell.WriteError("Wrong number of args.");
    }
    else
    {
      MapId mapId = new MapId(int.Parse(args[0], (IFormatProvider) CultureInfo.InvariantCulture));
      if (!this._mapSystem.MapExists(new MapId?(mapId)))
        shell.WriteError("Map does not exist!");
      else if (this._mapSystem.IsInitialized(mapId))
        shell.WriteError("Map is already initialized!");
      else
        this._mapSystem.InitializeMap(mapId);
    }
  }
}
