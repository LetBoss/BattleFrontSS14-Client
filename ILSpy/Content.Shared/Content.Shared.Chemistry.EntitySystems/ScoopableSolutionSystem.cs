using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;

namespace Content.Shared.Chemistry.EntitySystems;

public sealed class ScoopableSolutionSystem : EntitySystem
{
	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	[Dependency]
	private SolutionTransferSystem _solutionTransfer;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ScoopableSolutionComponent, InteractUsingEvent>((EntityEventRefHandler<ScoopableSolutionComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
	}

	private void OnInteractUsing(Entity<ScoopableSolutionComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = TryScoop(ent, args.Used, args.User);
		}
	}

	public bool TryScoop(Entity<ScoopableSolutionComponent> ent, EntityUid beaker, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.Solution, out Entity<SolutionComponent>? src, out Solution srcSolution) || !_solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(beaker), out Entity<SolutionComponent>? target, out Solution _))
		{
			return false;
		}
		if (_solutionTransfer.Transfer(user, Entity<ScoopableSolutionComponent>.op_Implicit(ent), src.Value, beaker, target.Value, srcSolution.Volume) == 0)
		{
			return false;
		}
		_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Popup), (ValueTuple<string, object>)("scooped", ent.Owner), (ValueTuple<string, object>)("beaker", beaker)), user, user);
		if (srcSolution.Volume == 0 && ent.Comp.Delete)
		{
			((EntitySystem)this).RemCompDeferred<ScoopableSolutionComponent>(Entity<ScoopableSolutionComponent>.op_Implicit(ent));
			if (!_netManager.IsClient)
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<ScoopableSolutionComponent>.op_Implicit(ent));
			}
		}
		return true;
	}
}
