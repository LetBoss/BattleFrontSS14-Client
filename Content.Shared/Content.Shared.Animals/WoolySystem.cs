using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Animals;

public sealed class WoolySystem : EntitySystem
{
	[Dependency]
	private HungerSystem _hunger;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<WoolyComponent, BeforeFullyEatenEvent>((EntityEventRefHandler<WoolyComponent, BeforeFullyEatenEvent>)OnBeforeFullyEaten, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WoolyComponent, MapInitEvent>((ComponentEventHandler<WoolyComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WoolyComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<WoolyComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, WoolyComponent component, MapInitEvent args)
	{
		component.NextGrowth = _timing.CurTime + component.GrowthDelay;
	}

	private void OnEntRemoved(Entity<WoolyComponent> entity, ref EntRemovedFromContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Solution.HasValue && !(((ContainerModifiedMessage)args).Entity != entity.Comp.Solution.Value.Owner))
		{
			entity.Comp.Solution = null;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<WoolyComponent> query = ((EntitySystem)this).EntityQueryEnumerator<WoolyComponent>();
		EntityUid uid = default(EntityUid);
		WoolyComponent wooly = default(WoolyComponent);
		HungerComponent hunger = default(HungerComponent);
		while (query.MoveNext(ref uid, ref wooly))
		{
			if (_timing.CurTime < wooly.NextGrowth)
			{
				continue;
			}
			wooly.NextGrowth += wooly.GrowthDelay;
			if (_mobState.IsDead(uid) || !_solutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), wooly.SolutionName, ref wooly.Solution, out Solution solution) || solution.AvailableVolume == 0)
			{
				continue;
			}
			if (((EntitySystem)this).TryComp<HungerComponent>(uid, ref hunger))
			{
				if ((int)_hunger.GetHungerThreshold(hunger) < 4)
				{
					continue;
				}
				_hunger.ModifyHunger(uid, 0f - wooly.HungerUsage, hunger);
			}
			_solutionContainer.TryAddReagent(wooly.Solution.Value, ProtoId<ReagentPrototype>.op_Implicit(wooly.ReagentId), wooly.Quantity, out var _);
		}
	}

	private void OnBeforeFullyEaten(Entity<WoolyComponent> ent, ref BeforeFullyEatenEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}
}
