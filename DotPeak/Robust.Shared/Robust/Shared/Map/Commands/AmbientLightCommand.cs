// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Commands.AmbientLightCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

#nullable enable
namespace Robust.Shared.Map.Commands;

public sealed class AmbientLightCommand : IConsoleCommand
{
  [Dependency]
  private readonly IEntitySystemManager _systems;

  public string Command => "setambientlight";

  public string Description => Loc.GetString("cmd-set-ambient-light-desc");

  public string Help => Loc.GetString("cmd-set-ambient-light-help");

  public void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length != 5)
    {
      shell.WriteError(Loc.GetString("cmd-invalid-arg-number-error"));
    }
    else
    {
      int result1;
      if (!int.TryParse(args[0], out result1))
      {
        shell.WriteError(Loc.GetString("cmd-parse-failure-integer"));
      }
      else
      {
        MapId mapId = new MapId(result1);
        SharedMapSystem entitySystem = this._systems.GetEntitySystem<SharedMapSystem>();
        if (!entitySystem.MapExists(new MapId?(mapId)))
        {
          shell.WriteError(Loc.GetString("cmd-parse-failure-mapid", ("arg", (object) mapId.Value)));
        }
        else
        {
          byte result2;
          byte result3;
          byte result4;
          byte result5;
          if (!byte.TryParse(args[1], out result2) || !byte.TryParse(args[2], out result3) || !byte.TryParse(args[3], out result4) || !byte.TryParse(args[4], out result5))
          {
            shell.WriteError(Loc.GetString("cmd-set-ambient-light-parse"));
          }
          else
          {
            Color color = Color.FromSrgb(new Color(result2, result3, result4, result5));
            entitySystem.SetAmbientLight(mapId, color);
          }
        }
      }
    }
  }
}
