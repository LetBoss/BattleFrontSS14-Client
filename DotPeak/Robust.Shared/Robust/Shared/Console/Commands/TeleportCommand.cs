// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.TeleportCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class TeleportCommand : LocalizedEntityCommands
{
  [Dependency]
  private readonly IMapManager _map;
  [Dependency]
  private readonly IEntityManager _entityManager;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  [Dependency]
  private readonly SharedMapSystem _mapSystem;

  public override string Command => "tp";

  public override bool RequireServerOrSingleplayer => true;

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    ICommonSession player = shell.Player;
    if (player == null)
      return;
    EntityUid? attachedEntity = player.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    float result1;
    float result2;
    if (args.Length < 2 || !float.TryParse(args[0], out result1) || !float.TryParse(args[1], out result2))
    {
      shell.WriteError(this.Help);
    }
    else
    {
      TransformComponent component = this._entityManager.GetComponent<TransformComponent>(valueOrDefault);
      Vector2 vector2 = new Vector2(result1, result2);
      this._transform.AttachToGridOrMap(valueOrDefault, component);
      int result3;
      MapId mapId = args.Length != 3 || !int.TryParse(args[2], out result3) ? component.MapID : new MapId(result3);
      if (!this._mapSystem.MapExists(new MapId?(mapId)))
      {
        shell.WriteError($"Map {mapId} doesn't exist!");
      }
      else
      {
        EntityUid uid1;
        if (this._map.TryFindGridAt(mapId, vector2, out uid1, out MapGridComponent _))
        {
          Vector2 position = Vector2.Transform(vector2, this._transform.GetInvWorldMatrix(uid1));
          this._transform.SetCoordinates(valueOrDefault, component, new EntityCoordinates(uid1, position));
        }
        else
        {
          EntityUid? uid2;
          if (this._mapSystem.TryGetMap(new MapId?(mapId), out uid2))
          {
            this._transform.SetWorldPosition((Entity<TransformComponent>) (valueOrDefault, component), vector2);
            this._transform.SetParent(valueOrDefault, component, uid2.Value);
          }
        }
        shell.WriteLine($"Teleported {shell.Player} to {mapId}:{result1},{result2}.");
      }
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    CompletionResult completion;
    switch (args.Length)
    {
      case 1:
        completion = CompletionResult.FromHint("<x>");
        break;
      case 2:
        completion = CompletionResult.FromHint("<y>");
        break;
      case 3:
        completion = CompletionResult.FromHintOptions(CompletionHelper.MapIds(this._entityManager), "[MapId]");
        break;
      default:
        completion = CompletionResult.Empty;
        break;
    }
    return completion;
  }
}
