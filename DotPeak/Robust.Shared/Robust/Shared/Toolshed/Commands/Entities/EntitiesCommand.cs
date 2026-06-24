// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.EntitiesCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class EntitiesCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public IEnumerable<EntityUid> Entities()
  {
    return (IEnumerable<EntityUid>) new EntitiesCommand.AllEntityEnumerator(this.EntityManager);
  }

  public sealed class AllEntityEnumerator(IEntityManager entMan) : 
    IEnumerable<EntityUid>,
    IEnumerable
  {
    public EntityUid[]? _arr;

    public IEntityManager EntMan { get; } = entMan;

    public IEnumerator<EntityUid> GetEnumerator()
    {
      if (this._arr == null)
        this._arr = this.EntMan.GetEntities().ToArray<EntityUid>();
      return ((IEnumerable<EntityUid>) this._arr).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      if (this._arr == null)
        this._arr = this.EntMan.GetEntities().ToArray<EntityUid>();
      return this._arr.GetEnumerator();
    }
  }
}
