using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Clothing;
using Content.Shared.Fluids.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Fluids.EntitySystems;

public sealed class SpillWhenWornSystem : EntitySystem
{
	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private SharedPuddleSystem _puddle;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SpillWhenWornComponent, ClothingGotEquippedEvent>((EntityEventRefHandler<SpillWhenWornComponent, ClothingGotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpillWhenWornComponent, ClothingGotUnequippedEvent>((EntityEventRefHandler<SpillWhenWornComponent, ClothingGotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpillWhenWornComponent, SolutionAccessAttemptEvent>((EntityEventRefHandler<SpillWhenWornComponent, SolutionAccessAttemptEvent>)OnSolutionAccessAttempt, (Type[])null, (Type[])null);
	}

	private void OnGotEquipped(Entity<SpillWhenWornComponent> ent, ref ClothingGotEquippedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.Solution, out Entity<SolutionComponent>? soln, out Solution solution) && solution.Volume > 0)
		{
			Solution drainedSolution = _solutionContainer.Drain(Entity<DrainableSolutionComponent>.op_Implicit(ent.Owner), soln.Value, solution.Volume);
			_puddle.TrySplashSpillAt(ent.Owner, ((EntitySystem)this).Transform(args.Wearer).Coordinates, drainedSolution, out var _);
		}
		ent.Comp.IsWorn = true;
		((EntitySystem)this).Dirty<SpillWhenWornComponent>(ent, (MetaDataComponent)null);
	}

	private void OnGotUnequipped(Entity<SpillWhenWornComponent> ent, ref ClothingGotUnequippedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.IsWorn = false;
		((EntitySystem)this).Dirty<SpillWhenWornComponent>(ent, (MetaDataComponent)null);
	}

	private void OnSolutionAccessAttempt(Entity<SpillWhenWornComponent> ent, ref SolutionAccessAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.IsWorn && !(ent.Comp.Solution != args.SolutionName))
		{
			args.Cancelled = true;
		}
	}
}
