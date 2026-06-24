// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.DeleteCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class DeleteCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public void Delete([PipedArgument] IEnumerable<EntityUid> entities)
  {
    foreach (EntityUid entity in entities)
      this.Del(entity);
  }

  [CommandImplementation(null)]
  public void Delete(EntityUid entity) => this.Del(entity);
}
