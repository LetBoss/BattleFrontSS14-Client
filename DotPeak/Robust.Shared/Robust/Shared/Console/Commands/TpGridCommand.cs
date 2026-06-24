// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.TpGridCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using System;
using System.Globalization;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class TpGridCommand : LocalizedEntityCommands
{
  [Dependency]
  private readonly IEntityManager _ent;
  [Dependency]
  private readonly SharedMapSystem _map;

  public override string Command => "tpgrid";

  public override bool RequireServerOrSingleplayer => true;

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    int length = args.Length;
    if (length < 3 || length > 4)
    {
      shell.WriteError(this.Loc.GetString("cmd-invalid-arg-number-error"));
    }
    else
    {
      NetEntity entity1;
      if (!NetEntity.TryParse(args[0].AsSpan(), out entity1))
      {
        shell.WriteError(this.Loc.GetString("cmd-parse-failure-uid", ("arg", (object) args[0])));
      }
      else
      {
        EntityUid? entity2;
        if (!this._ent.TryGetEntity(entity1, out entity2) || !this._ent.HasComponent<MapGridComponent>(entity2) || this._ent.HasComponent<MapComponent>(entity2))
        {
          shell.WriteError(this.Loc.GetString("cmd-parse-failure-grid", ("arg", (object) args[0])));
        }
        else
        {
          float x = float.Parse(args[1], (IFormatProvider) CultureInfo.InvariantCulture);
          float y = float.Parse(args[2], (IFormatProvider) CultureInfo.InvariantCulture);
          MapId mapId = this._ent.GetComponent<TransformComponent>(entity2.Value).MapID;
          if (args.Length > 3)
          {
            int result;
            if (!int.TryParse(args[3], out result))
            {
              shell.WriteError(this.Loc.GetString("cmd-parse-failure-mapid", ("arg", (object) args[3])));
              return;
            }
            mapId = new MapId(result);
          }
          EntityUid map = this._map.GetMap(mapId);
          if (map == EntityUid.Invalid)
          {
            shell.WriteError(this.Loc.GetString("cmd-parse-failure-mapid", ("arg", (object) mapId.Value)));
          }
          else
          {
            EntityCoordinates entityCoordinates = new EntityCoordinates(map, new Vector2(x, y));
            this._ent.System<SharedTransformSystem>().SetCoordinates(entity2.Value, entityCoordinates);
          }
        }
      }
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    CompletionResult completion;
    switch (args.Length)
    {
      case 1:
        string[] strArray = args;
        completion = CompletionResult.FromHintOptions(CompletionHelper.Components<MapGridComponent>(strArray[strArray.Length - 1], this._ent), "<GridUid>");
        break;
      case 2:
        completion = CompletionResult.FromHint("<x>");
        break;
      case 3:
        completion = CompletionResult.FromHint("<y>");
        break;
      case 4:
        completion = CompletionResult.FromHintOptions(CompletionHelper.MapIds(this._ent), "[MapId]");
        break;
      default:
        completion = CompletionResult.Empty;
        break;
    }
    return completion;
  }
}
