using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Chemistry.EntitySystems;

public sealed class SolutionSpikerSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RefillableSolutionComponent, InteractUsingEvent>((EntityEventRefHandler<RefillableSolutionComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
	}

	private void OnInteractUsing(Entity<RefillableSolutionComponent> entity, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (TrySpike(args.Used, args.Target, args.User, entity.Comp))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private bool TrySpike(EntityUid source, EntityUid target, EntityUid user, RefillableSolutionComponent? spikableTarget = null, SolutionSpikerComponent? spikableSource = null, SolutionContainerManagerComponent? managerSource = null, SolutionContainerManagerComponent? managerTarget = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SolutionSpikerComponent, SolutionContainerManagerComponent>(source, ref spikableSource, ref managerSource, false) || !((EntitySystem)this).Resolve<RefillableSolutionComponent, SolutionContainerManagerComponent>(target, ref spikableTarget, ref managerTarget, false) || !_solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit((target, spikableTarget, managerTarget)), out Entity<SolutionComponent>? targetSoln, out Solution targetSolution) || !_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((source, managerSource)), spikableSource.SourceSolution, out Entity<SolutionComponent>? _, out Solution sourceSolution))
		{
			return false;
		}
		if (targetSolution.Volume == 0 && !spikableSource.IgnoreEmpty)
		{
			_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(spikableSource.PopupEmpty), (ValueTuple<string, object>)("spiked-entity", target), (ValueTuple<string, object>)("spike-entity", source)), user, user);
			return false;
		}
		if (!_solution.ForceAddSolution(targetSoln.Value, sourceSolution))
		{
			return false;
		}
		_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(spikableSource.Popup), (ValueTuple<string, object>)("spiked-entity", target), (ValueTuple<string, object>)("spike-entity", source)), user, user);
		sourceSolution.RemoveAllSolution();
		if (spikableSource.Delete)
		{
			((EntitySystem)this).QueueDel((EntityUid?)source);
		}
		return true;
	}
}
