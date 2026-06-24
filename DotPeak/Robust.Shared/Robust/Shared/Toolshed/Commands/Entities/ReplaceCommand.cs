// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.ReplaceCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class ReplaceCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public IEnumerable<EntityUid> Replace([PipedArgument] IEnumerable<EntityUid> input, EntProtoId prototype)
  {
    ReplaceCommand replaceCommand = this;
    foreach (EntityUid entityUid in input)
    {
      TransformComponent transformComponent = replaceCommand.Transform(entityUid);
      EntityCoordinates coordinates = transformComponent.Coordinates;
      Angle localRotation = transformComponent.LocalRotation;
      replaceCommand.QDel(entityUid);
      EntityUid entity = replaceCommand.Spawn((string) prototype, coordinates);
      replaceCommand.Transform(entity).LocalRotation = localRotation;
      yield return entity;
    }
  }
}
