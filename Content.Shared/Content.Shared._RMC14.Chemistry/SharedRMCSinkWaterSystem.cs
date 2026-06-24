using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Chemistry;

public sealed class SharedRMCSinkWaterSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCSinkWaterComponent, InteractUsingEvent>((EntityEventRefHandler<RMCSinkWaterComponent, InteractUsingEvent>)OnSinkInteractUsing, (Type[])null, (Type[])null);
	}

	private void OnSinkInteractUsing(Entity<RMCSinkWaterComponent> sink, ref InteractUsingEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		RefillableSolutionComponent refillable = default(RefillableSolutionComponent);
		if (((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp<RefillableSolutionComponent>(args.Used, ref refillable) || !_solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit((ValueTuple<EntityUid, RefillableSolutionComponent, SolutionContainerManagerComponent>)(args.Used, refillable, null)), out Entity<SolutionComponent>? targetSolution, out Solution solution))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		FixedPoint2 availableSpace = solution.AvailableVolume;
		if (availableSpace <= FixedPoint2.Zero)
		{
			string fullMessage = base.Loc.GetString("rmc-sink-container-full", (ValueTuple<string, object>)("container", args.Used));
			_popup.PopupClient(fullMessage, Entity<RMCSinkWaterComponent>.op_Implicit(sink), args.User);
			return;
		}
		FixedPoint2 transferAmount = availableSpace;
		SolutionTransferComponent transfer = default(SolutionTransferComponent);
		if (((EntitySystem)this).TryComp<SolutionTransferComponent>(args.Used, ref transfer))
		{
			transferAmount = FixedPoint2.Min(transfer.TransferAmount, availableSpace);
		}
		Solution waterSolution = new Solution();
		waterSolution.AddReagent(ProtoId<ReagentPrototype>.op_Implicit(sink.Comp.Reagent), transferAmount);
		_solution.TryAddSolution(targetSolution.Value, waterSolution);
		string message = base.Loc.GetString("rmc-sink-fill-container", new(string, object)[3]
		{
			("user", args.User),
			("container", args.Used),
			("sink", sink)
		});
		_popup.PopupPredicted(message, args.User, args.User);
	}
}
