// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.SpawnCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class SpawnCommand : ToolshedCommand
{
  private SharedContainerSystem? sharedContainerSystem;

  [CommandImplementation("at")]
  public EntityUid SpawnAt([PipedArgument] EntityCoordinates target, EntProtoId proto)
  {
    return this.Spawn((string) proto, target);
  }

  [CommandImplementation("at")]
  public IEnumerable<EntityUid> SpawnAt([PipedArgument] IEnumerable<EntityCoordinates> target, EntProtoId proto)
  {
    return target.Select<EntityCoordinates, EntityUid>((Func<EntityCoordinates, EntityUid>) (x => this.SpawnAt(x, proto)));
  }

  [CommandImplementation("on")]
  public EntityUid SpawnOn([PipedArgument] EntityUid target, EntProtoId proto)
  {
    return this.Spawn((string) proto, this.Transform(target).Coordinates);
  }

  [CommandImplementation("on")]
  public IEnumerable<EntityUid> SpawnOn([PipedArgument] IEnumerable<EntityUid> target, EntProtoId proto)
  {
    return target.Select<EntityUid, EntityUid>((Func<EntityUid, EntityUid>) (x => this.SpawnOn(x, proto)));
  }

  [CommandImplementation("in")]
  public EntityUid SpawnIn([PipedArgument] EntityUid target, string containerId, EntProtoId proto)
  {
    EntityUid entity = this.SpawnOn(target, proto);
    TransformComponent component1;
    MetaDataComponent component2;
    if (!this.TryComp<TransformComponent>(entity, out component1) || !this.TryComp<MetaDataComponent>(entity, out component2))
      return entity;
    PhysicsComponent component3;
    this.TryComp<PhysicsComponent>(entity, out component3);
    if (this.sharedContainerSystem == null)
      this.sharedContainerSystem = this.EntityManager.System<SharedContainerSystem>();
    BaseContainer container = this.sharedContainerSystem.GetContainer(target, containerId);
    this.sharedContainerSystem.InsertOrDrop((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) (entity, component1, component2, component3), container);
    return entity;
  }

  [CommandImplementation("in")]
  public IEnumerable<EntityUid> SpawnIn(
    [PipedArgument] IEnumerable<EntityUid> target,
    string containerId,
    EntProtoId proto)
  {
    return target.Select<EntityUid, EntityUid>((Func<EntityUid, EntityUid>) (x => this.SpawnIn(x, containerId, proto)));
  }

  [CommandImplementation("attached")]
  public EntityUid SpawnIn([PipedArgument] EntityUid target, EntProtoId proto)
  {
    return this.Spawn((string) proto, new EntityCoordinates(target, Vector2.Zero));
  }

  [CommandImplementation("attached")]
  public IEnumerable<EntityUid> SpawnIn([PipedArgument] IEnumerable<EntityUid> target, EntProtoId proto)
  {
    return target.Select<EntityUid, EntityUid>((Func<EntityUid, EntityUid>) (x => this.SpawnIn(x, proto)));
  }
}
