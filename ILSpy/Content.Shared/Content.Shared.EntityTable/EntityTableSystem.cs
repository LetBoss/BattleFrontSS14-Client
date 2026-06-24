using System;
using System.Collections.Generic;
using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.EntityTable;

public sealed class EntityTableSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IRobustRandom _random;

	public IEnumerable<EntProtoId> GetSpawns(EntityTablePrototype entTableProto, System.Random? rand = null, EntityTableContext? ctx = null)
	{
		return GetSpawns(entTableProto.Table, rand, ctx);
	}

	public IEnumerable<EntProtoId> GetSpawns(EntityTableSelector? table, System.Random? rand = null, EntityTableContext? ctx = null)
	{
		if (table == null)
		{
			return new List<EntProtoId>();
		}
		if (rand == null)
		{
			rand = _random.GetRandom();
		}
		if (ctx == null)
		{
			ctx = new EntityTableContext();
		}
		return table.GetSpawns(rand, (IEntityManager)(object)base.EntityManager, _prototypeManager, ctx);
	}
}
