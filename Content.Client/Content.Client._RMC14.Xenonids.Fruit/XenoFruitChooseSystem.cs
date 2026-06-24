using System;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Xenonids.Fruit;

public sealed class XenoFruitChooseSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitPlanterComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<XenoFruitPlanterComponent, AfterAutoHandleStateEvent>)OnXenoFruitAfterState, (Type[])null, (Type[])null);
	}

	private void OnXenoFruitAfterState(Entity<XenoFruitPlanterComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!_timing.IsFirstTimePredicted || !((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<XenoFruitPlanterComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is XenoFruitChooseBui xenoFruitChooseBui)
				{
					xenoFruitChooseBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"XenoFruitChooseBui"}\n{value}");
		}
	}
}
