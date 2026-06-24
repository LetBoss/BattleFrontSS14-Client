using System;
using Content.Shared._RMC14.Xenonids.Construction;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class XenoChooseStructureSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<XenoConstructionComponent, AfterAutoHandleStateEvent>)OnXenoConstructionAfterState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenBuildingBoostComponent, ComponentStartup>((EntityEventRefHandler<QueenBuildingBoostComponent, ComponentStartup>)OnBoostAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenBuildingBoostComponent, ComponentRemove>((EntityEventRefHandler<QueenBuildingBoostComponent, ComponentRemove>)OnBoostRemoved, (Type[])null, (Type[])null);
	}

	private void OnBoostAdded(Entity<QueenBuildingBoostComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		RefreshUI(ent.Owner);
	}

	private void OnBoostRemoved(Entity<QueenBuildingBoostComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		RefreshUI(ent.Owner);
	}

	private void RefreshUI(EntityUid entity)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		UserInterfaceComponent val = default(UserInterfaceComponent);
		if (!_timing.IsFirstTimePredicted || !((EntitySystem)this).TryComp<UserInterfaceComponent>(entity, ref val))
		{
			return;
		}
		foreach (BoundUserInterface value in val.ClientOpenInterfaces.Values)
		{
			if (value is XenoChooseStructureBui xenoChooseStructureBui)
			{
				((BoundUserInterface)xenoChooseStructureBui).Close();
			}
		}
	}

	private void OnXenoConstructionAfterState(Entity<XenoConstructionComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		UserInterfaceComponent val = default(UserInterfaceComponent);
		if (!_timing.IsFirstTimePredicted || !((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<XenoConstructionComponent>.op_Implicit(ent), ref val))
		{
			return;
		}
		foreach (BoundUserInterface value in val.ClientOpenInterfaces.Values)
		{
			if (value is XenoChooseStructureBui xenoChooseStructureBui)
			{
				xenoChooseStructureBui.Refresh();
			}
		}
	}
}
