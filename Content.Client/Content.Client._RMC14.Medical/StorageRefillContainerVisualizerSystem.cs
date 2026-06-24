using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Medical.Refill;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Medical;

public sealed class StorageRefillContainerVisualizerSystem : VisualizerSystem<RMCRefillSolutionFromContainerOnStoreComponent>
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	[Dependency]
	private CMRefillableSolutionSystem _refillable;

	protected override void OnAppearanceChange(EntityUid uid, RMCRefillSolutionFromContainerOnStoreComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		Color val = default(Color);
		int num = default(int);
		if (sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)SolutionContainerStoreVisuals.Color, ref val, (AppearanceComponent)null) && base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)SolutionContainerStoreVisuals.Base, ref num, false))
		{
			BaseContainer val2 = default(BaseContainer);
			EntityUid? val3 = default(EntityUid?);
			if (!_container.TryGetContainer(uid, component.ContainerId, ref val2, (ContainerManagerComponent)null) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)val2.ContainedEntities, ref val3) || (!_solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(val3.Value), out Entity<SolutionComponent>? soln, out Solution solution) && !_refillable.TryGetPressurizedSolution(Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(val3.Value), out soln, out solution)) || solution.Volume == 0)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
				return;
			}
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
			base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, ((Color)(ref val)).WithAlpha(component.LayerOpacity));
		}
	}
}
