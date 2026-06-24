using System;
using Content.Shared._RMC14.Xenonids.Evolution;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Xenonids.Evolution;

public sealed class XenoEvolutionUISystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoEvolutionComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<XenoEvolutionComponent, AfterAutoHandleStateEvent>)OnXenoEvolutionAfterState, (Type[])null, (Type[])null);
	}

	private void OnXenoEvolutionAfterState(Entity<XenoEvolutionComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UserInterfaceComponent val = default(UserInterfaceComponent);
		if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<XenoEvolutionComponent>.op_Implicit(ent), ref val))
		{
			return;
		}
		foreach (BoundUserInterface value in val.ClientOpenInterfaces.Values)
		{
			if (value is XenoEvolutionBui xenoEvolutionBui)
			{
				xenoEvolutionBui.Refresh();
			}
		}
	}
}
