// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.SpawnCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Placement;
using System;
using System.Globalization;

#nullable enable
namespace Robust.Shared.Console.Commands;

public sealed class SpawnCommand : LocalizedCommands
{
  [Dependency]
  private readonly IEntityManager _entityManager;

  public override string Command => "spawn";

  public override bool RequireServerOrSingleplayer => true;

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    int length = args.Length;
    if (length < 1 || length > 3)
      shell.WriteError("Incorrect number of arguments. " + this.Help);
    EntityUid uid = (EntityUid?) shell.Player?.AttachedEntity ?? EntityUid.Invalid;
    PlacementEntityEvent? nullable = new PlacementEntityEvent?();
    if (args.Length == 1 && uid != EntityUid.Invalid)
    {
      EntityCoordinates coordinates = this._entityManager.GetComponent<TransformComponent>(uid).Coordinates;
      nullable = new PlacementEntityEvent?(new PlacementEntityEvent(this._entityManager.SpawnEntity(args[0], coordinates), coordinates, PlacementEventAction.Create, shell.Player?.UserId));
    }
    else if (args.Length == 2)
    {
      EntityCoordinates coordinates = this._entityManager.GetComponent<TransformComponent>(this._entityManager.GetEntity(NetEntity.Parse(args[1].AsSpan()))).Coordinates;
      nullable = new PlacementEntityEvent?(new PlacementEntityEvent(this._entityManager.SpawnEntity(args[0], coordinates), coordinates, PlacementEventAction.Create, shell.Player?.UserId));
    }
    else if (uid != EntityUid.Invalid)
    {
      MapCoordinates coordinates = new MapCoordinates(float.Parse(args[1], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(args[2], (IFormatProvider) CultureInfo.InvariantCulture), this._entityManager.GetComponent<TransformComponent>(uid).MapID);
      EntityUid entityUid = this._entityManager.SpawnEntity(args[0], coordinates);
      nullable = new PlacementEntityEvent?(new PlacementEntityEvent(entityUid, this._entityManager.GetComponent<TransformComponent>(entityUid).Coordinates, PlacementEventAction.Create, shell.Player?.UserId));
    }
    if (!nullable.HasValue)
      return;
    this._entityManager.EventBus.RaiseEvent<PlacementEntityEvent>(EventSource.Local, nullable.Value);
  }
}
