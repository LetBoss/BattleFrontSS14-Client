// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.TeleportToCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Console.Commands;

public sealed class TeleportToCommand : LocalizedEntityCommands
{
  [Dependency]
  private readonly ISharedPlayerManager _players;
  [Dependency]
  private readonly IEntityManager _entities;
  [Dependency]
  private readonly SharedTransformSystem _transform;

  public override string Command => "tpto";

  public override bool RequireServerOrSingleplayer => true;

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    EntityUid? victimUid1;
    if (args.Length == 0 || !this.TryGetTransformFromUidOrUsername(args[0], shell, out victimUid1, out TransformComponent _))
      return;
    EntityCoordinates coordinates = new EntityCoordinates(victimUid1.Value, Vector2.Zero);
    PhysicsComponent component1;
    if (this._entities.TryGetComponent<PhysicsComponent>(victimUid1, out component1))
      coordinates = coordinates.Offset(component1.LocalCenter);
    List<(EntityUid, TransformComponent)> valueTupleList = new List<(EntityUid, TransformComponent)>();
    if (args.Length == 1)
    {
      EntityUid? attachedEntity = (EntityUid?) shell.Player?.AttachedEntity;
      TransformComponent component2;
      if (!this._entities.TryGetComponent<TransformComponent>(attachedEntity, out component2))
      {
        shell.WriteError(this.Loc.GetString("cmd-failure-no-attached-entity"));
        return;
      }
      valueTupleList.Add((attachedEntity.Value, component2));
    }
    else
    {
      foreach (string str in args)
      {
        EntityUid? victimUid2;
        TransformComponent transform;
        if (this.TryGetTransformFromUidOrUsername(str, shell, out victimUid2, out transform))
        {
          EntityUid? nullable1 = victimUid2;
          EntityUid? nullable2 = victimUid1;
          if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0)
            valueTupleList.Add((victimUid2.Value, transform));
        }
      }
    }
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(coordinates);
    foreach ((EntityUid, TransformComponent) valueTuple in valueTupleList)
    {
      this._transform.SetMapCoordinates(valueTuple.Item1, mapCoordinates);
      this._transform.AttachToGridOrMap(valueTuple.Item1, valueTuple.Item2);
    }
  }

  private bool TryGetTransformFromUidOrUsername(
    string str,
    IConsoleShell shell,
    [NotNullWhen(true)] out EntityUid? victimUid,
    [NotNullWhen(true)] out TransformComponent? transform)
  {
    NetEntity entity1;
    EntityUid? entity2;
    if (NetEntity.TryParse(str.AsSpan(), out entity1) && this._entities.TryGetEntity(entity1, out entity2) && this._entities.TryGetComponent<TransformComponent>(entity2, out transform) && !this._entities.HasComponent<MapComponent>(entity2))
    {
      victimUid = entity2;
      return true;
    }
    ICommonSession session;
    if (this._players.TryGetSessionByUsername(str, out session) && this._entities.TryGetComponent<TransformComponent>(session.AttachedEntity, out transform))
    {
      victimUid = session.AttachedEntity;
      return true;
    }
    shell.WriteError(this.Loc.GetString("cmd-tpto-parse-error", (nameof (str), (object) str)));
    transform = (TransformComponent) null;
    victimUid = new EntityUid?();
    return false;
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    if (args.Length == 0)
      return CompletionResult.Empty;
    string[] strArray = args;
    string last = strArray[strArray.Length - 1];
    return CompletionResult.FromHintOptions(((IEnumerable<ICommonSession>) this._players.Sessions).Select<ICommonSession, string>((Func<ICommonSession, string>) (x => x.Name ?? string.Empty)).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x) && x.StartsWith(last, StringComparison.CurrentCultureIgnoreCase))), this.Loc.GetString(args.Length == 1 ? "cmd-tpto-destination-hint" : "cmd-tpto-victim-hint"));
  }
}
