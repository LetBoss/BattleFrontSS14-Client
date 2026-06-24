// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.LocationCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class LocationCommand : LocalizedEntityCommands
{
  [Dependency]
  private readonly IEntityManager _ent;
  [Dependency]
  private readonly SharedTransformSystem _transform;

  public override string Command => "loc";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    ICommonSession player = shell.Player;
    if (player == null)
      return;
    EntityUid? attachedEntity = player.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityCoordinates coordinates = this._ent.GetComponent<TransformComponent>(attachedEntity.GetValueOrDefault()).Coordinates;
    MapId mapId = this._transform.GetMapId(coordinates);
    EntityUid? grid = this._transform.GetGrid(coordinates);
    shell.WriteLine($"MapID:{mapId} GridUid:{grid} X:{coordinates.X:N2} Y:{coordinates.Y:N2}");
  }
}
