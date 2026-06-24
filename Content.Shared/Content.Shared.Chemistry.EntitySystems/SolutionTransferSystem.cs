using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Chemistry.EntitySystems;

public sealed class SolutionTransferSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	public static readonly FixedPoint2[] DefaultTransferAmounts = new FixedPoint2[9] { 1, 5, 10, 25, 50, 100, 250, 500, 1000 };

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<SolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>)AddSetTransferVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionTransferComponent, AfterInteractEvent>((EntityEventRefHandler<SolutionTransferComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionTransferComponent, TransferAmountSetValueMessage>((EntityEventRefHandler<SolutionTransferComponent, TransferAmountSetValueMessage>)OnTransferAmountSetValueMessage, (Type[])null, (Type[])null);
	}

	private void OnTransferAmountSetValueMessage(Entity<SolutionTransferComponent> ent, ref TransferAmountSetValueMessage message)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionTransferComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SolutionTransferComponent solutionTransferComponent = default(SolutionTransferComponent);
		val.Deconstruct(ref val2, ref solutionTransferComponent);
		EntityUid uid = val2;
		SolutionTransferComponent comp = solutionTransferComponent;
		FixedPoint2 newTransferAmount = (comp.TransferAmount = FixedPoint2.Clamp(message.Value, comp.MinimumTransferAmount, comp.MaximumTransferAmount));
		EntityUid user = ((BaseBoundUserInterfaceEvent)message).Actor;
		if (((EntityUid)(ref user)).Valid)
		{
			_popup.PopupEntity(base.Loc.GetString("comp-solution-transfer-set-amount", (ValueTuple<string, object>)("amount", newTransferAmount)), uid, user);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
	}

	private void AddSetTransferVerbs(Entity<SolutionTransferComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionTransferComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SolutionTransferComponent solutionTransferComponent = default(SolutionTransferComponent);
		val.Deconstruct(ref val2, ref solutionTransferComponent);
		EntityUid uid = val2;
		SolutionTransferComponent comp = solutionTransferComponent;
		if (!args.CanAccess || !args.CanInteract || !comp.CanChangeTransferAmount || args.Hands == null)
		{
			return;
		}
		GetVerbsEvent<AlternativeVerb> @event = args;
		args.Verbs.Add(new AlternativeVerb
		{
			Text = base.Loc.GetString("comp-solution-transfer-verb-custom-amount"),
			Category = VerbCategory.SetTransferAmount,
			Act = delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)TransferAmountUiKey.Key, (EntityUid?)@event.User, false);
			},
			Priority = 1
		});
		int priority = 0;
		EntityUid user = args.User;
		FixedPoint2[] array = ent.Comp.TransferAmounts ?? DefaultTransferAmounts;
		foreach (FixedPoint2 amount in array)
		{
			if (!(amount < comp.MinimumTransferAmount) && !(amount > comp.MaximumTransferAmount))
			{
				AlternativeVerb verb = new AlternativeVerb();
				verb.Text = base.Loc.GetString("comp-solution-transfer-verb-amount", (ValueTuple<string, object>)("amount", amount));
				verb.Category = VerbCategory.SetTransferAmount;
				verb.Act = delegate
				{
					//IL_005b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0066: Unknown result type (might be due to invalid IL or missing references)
					//IL_0087: Unknown result type (might be due to invalid IL or missing references)
					comp.TransferAmount = amount;
					_popup.PopupClient(base.Loc.GetString("comp-solution-transfer-set-amount", (ValueTuple<string, object>)("amount", amount)), uid, user);
					((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
				};
				verb.Priority = priority;
				priority--;
				args.Verbs.Add(verb);
			}
		}
	}

	private void OnAfterInteract(Entity<SolutionTransferComponent> ent, ref AfterInteractEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanReach)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		Entity<SolutionTransferComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SolutionTransferComponent solutionTransferComponent = default(SolutionTransferComponent);
		val.Deconstruct(ref val2, ref solutionTransferComponent);
		EntityUid uid = val2;
		SolutionTransferComponent comp = solutionTransferComponent;
		RefillableSolutionComponent refill = default(RefillableSolutionComponent);
		if (comp.CanReceive && !((EntitySystem)this).HasComp<RefillableSolutionComponent>(target2) && _solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target2), out Entity<SolutionComponent>? targetSoln, out Solution solution) && ((EntitySystem)this).TryComp<RefillableSolutionComponent>(uid, ref refill) && _solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit((ValueTuple<EntityUid, RefillableSolutionComponent, SolutionContainerManagerComponent>)(uid, refill, null)), out Entity<SolutionComponent>? ownerSoln, out Solution ownerRefill))
		{
			FixedPoint2 transferAmount = comp.TransferAmount;
			FixedPoint2? fixedPoint = refill?.MaxRefill;
			if (fixedPoint.HasValue)
			{
				FixedPoint2 maxRefill = fixedPoint.GetValueOrDefault();
				transferAmount = FixedPoint2.Min(transferAmount, maxRefill);
			}
			FixedPoint2 transferred = Transfer(args.User, target2, targetSoln.Value, uid, ownerSoln.Value, transferAmount);
			((HandledEntityEventArgs)args).Handled = true;
			if (transferred > 0)
			{
				string msg = ((ownerRefill.AvailableVolume == 0) ? "comp-solution-transfer-fill-fully" : "comp-solution-transfer-fill-normal");
				_popup.PopupClient(base.Loc.GetString(msg, new(string, object)[3]
				{
					("owner", args.Target),
					("amount", transferred),
					("target", uid)
				}), uid, args.User);
				return;
			}
		}
		RefillableSolutionComponent targetRefill = default(RefillableSolutionComponent);
		if (comp.CanSend && ((EntitySystem)this).TryComp<RefillableSolutionComponent>(target2, ref targetRefill) && _solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit((ValueTuple<EntityUid, RefillableSolutionComponent, SolutionContainerManagerComponent>)(target2, targetRefill, null)), out targetSoln, out solution) && _solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(uid), out ownerSoln, out solution))
		{
			FixedPoint2 transferAmount2 = comp.TransferAmount;
			FixedPoint2? fixedPoint = targetRefill?.MaxRefill;
			if (fixedPoint.HasValue)
			{
				FixedPoint2 maxRefill2 = fixedPoint.GetValueOrDefault();
				transferAmount2 = FixedPoint2.Min(transferAmount2, maxRefill2);
			}
			FixedPoint2 transferred2 = Transfer(args.User, uid, ownerSoln.Value, target2, targetSoln.Value, transferAmount2);
			((HandledEntityEventArgs)args).Handled = true;
			if (transferred2 > 0)
			{
				string message = base.Loc.GetString("comp-solution-transfer-transfer-solution", (ValueTuple<string, object>)("amount", transferred2), (ValueTuple<string, object>)("target", target2));
				_popup.PopupClient(message, uid, args.User);
			}
		}
	}

	public FixedPoint2 Transfer(EntityUid? user, EntityUid sourceEntity, Entity<SolutionComponent> source, EntityUid targetEntity, Entity<SolutionComponent> target, FixedPoint2 amount)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		SolutionTransferAttemptEvent transferAttempt = new SolutionTransferAttemptEvent(sourceEntity, source, targetEntity, target);
		((EntitySystem)this).RaiseLocalEvent<SolutionTransferAttemptEvent>(sourceEntity, ref transferAttempt, false);
		string reason = transferAttempt.CancelReason;
		if (reason != null)
		{
			_popup.PopupClient(reason, sourceEntity, user);
			return FixedPoint2.Zero;
		}
		Solution sourceSolution = source.Comp.Solution;
		if (sourceSolution.Volume == 0)
		{
			_popup.PopupClient(base.Loc.GetString("comp-solution-transfer-is-empty", (ValueTuple<string, object>)("target", sourceEntity)), sourceEntity, user);
			return FixedPoint2.Zero;
		}
		((EntitySystem)this).RaiseLocalEvent<SolutionTransferAttemptEvent>(targetEntity, ref transferAttempt, false);
		string targetReason = transferAttempt.CancelReason;
		if (targetReason != null)
		{
			_popup.PopupClient(targetReason, targetEntity, user);
			return FixedPoint2.Zero;
		}
		Solution targetSolution = target.Comp.Solution;
		if (targetSolution.AvailableVolume == 0)
		{
			_popup.PopupClient(base.Loc.GetString("comp-solution-transfer-is-full", (ValueTuple<string, object>)("target", targetEntity)), targetEntity, user);
			return FixedPoint2.Zero;
		}
		FixedPoint2 actualAmount = FixedPoint2.Min(amount, FixedPoint2.Min(sourceSolution.Volume, targetSolution.AvailableVolume));
		Solution solution = _solution.SplitSolution(source, actualAmount);
		_solution.AddSolution(target, solution);
		SolutionTransferredEvent ev = new SolutionTransferredEvent(sourceEntity, targetEntity, user, actualAmount);
		((EntitySystem)this).RaiseLocalEvent<SolutionTransferredEvent>(targetEntity, ref ev, false);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(38, 4);
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "player", "ToPrettyString(user)");
		handler.AppendLiteral(" transferred ");
		handler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(solution));
		handler.AppendLiteral(" to ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(targetEntity)), "target", "ToPrettyString(targetEntity)");
		handler.AppendLiteral(", which now contains ");
		handler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(targetSolution));
		adminLogger.Add(LogType.Action, LogImpact.Medium, ref handler);
		return actualAmount;
	}
}
