using System;
using Content.Shared.EntityTable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.ComponentTable;

public sealed class SharedComponentTableSystem : EntitySystem
{
	[Dependency]
	private EntityTableSystem _entTable;

	[Dependency]
	private IPrototypeManager _proto;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ComponentTableComponent, MapInitEvent>((EntityEventRefHandler<ComponentTableComponent, MapInitEvent>)OnTableInit, (Type[])null, (Type[])null);
	}

	private void OnTableInit(Entity<ComponentTableComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype entProto = default(EntityPrototype);
		foreach (EntProtoId entity in _entTable.GetSpawns(ent.Comp.Table))
		{
			if (_proto.TryIndex(entity, ref entProto))
			{
				base.EntityManager.AddComponents(Entity<ComponentTableComponent>.op_Implicit(ent), entProto.Components, true);
			}
		}
	}
}
