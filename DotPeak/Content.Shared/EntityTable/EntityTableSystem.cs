// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.EntityTableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.EntityTable;

public sealed class EntityTableSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IRobustRandom _random;

  public IEnumerable<EntProtoId> GetSpawns(
    EntityTablePrototype entTableProto,
    System.Random? rand = null,
    EntityTableContext? ctx = null)
  {
    return this.GetSpawns(entTableProto.Table, rand, ctx);
  }

  public IEnumerable<EntProtoId> GetSpawns(
    EntityTableSelector? table,
    System.Random? rand = null,
    EntityTableContext? ctx = null)
  {
    if (table == null)
      return (IEnumerable<EntProtoId>) new List<EntProtoId>();
    if (rand == null)
      rand = this._random.GetRandom();
    if (ctx == null)
      ctx = new EntityTableContext();
    return table.GetSpawns(rand, (IEntityManager) this.EntityManager, this._prototypeManager, ctx);
  }
}
