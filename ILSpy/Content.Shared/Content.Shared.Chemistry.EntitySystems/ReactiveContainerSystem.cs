using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Chemistry.EntitySystems;

public sealed class ReactiveContainerSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private ReactiveSystem _reactiveSystem;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainerSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ReactiveContainerComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<ReactiveContainerComponent, EntInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReactiveContainerComponent, SolutionContainerChangedEvent>((ComponentEventHandler<ReactiveContainerComponent, SolutionContainerChangedEvent>)OnSolutionChange, (Type[])null, (Type[])null);
	}

	private void OnInserted(EntityUid uid, ReactiveContainerComponent comp, EntInsertedIntoContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<ReactiveComponent>(((ContainerModifiedMessage)args).Entity) && _solutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), comp.Solution, out Entity<SolutionComponent>? _, out Solution solution) && !(solution.Volume == 0))
		{
			_reactiveSystem.DoEntityReaction(((ContainerModifiedMessage)args).Entity, solution, ReactionMethod.Touch);
		}
	}

	private void OnSolutionChange(EntityUid uid, ReactiveContainerComponent comp, SolutionContainerChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		ContainerManagerComponent manager = default(ContainerManagerComponent);
		BaseContainer container = default(BaseContainer);
		if (!_solutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), comp.Solution, out Entity<SolutionComponent>? _, out Solution solution) || solution.Volume == 0 || !((EntitySystem)this).TryComp<ContainerManagerComponent>(uid, ref manager) || !_containerSystem.TryGetContainer(uid, comp.Container, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		foreach (EntityUid entity2 in container.ContainedEntities)
		{
			if (((EntitySystem)this).HasComp<ReactiveComponent>(entity2))
			{
				_reactiveSystem.DoEntityReaction(entity2, solution, ReactionMethod.Touch);
			}
		}
	}
}
