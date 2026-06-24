using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Chemistry;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Hypospray.Events;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Forensics;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Chemistry.EntitySystems;

public sealed class HypospraySystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private ReactiveSystem _reactiveSystem;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainers;

	[Dependency]
	private UseDelaySystem _useDelay;

	[Dependency]
	private RMCSharedHypospraySystem _rmcHypospray;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HyposprayComponent, AfterInteractEvent>((EntityEventRefHandler<HyposprayComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HyposprayComponent, MeleeHitEvent>((EntityEventRefHandler<HyposprayComponent, MeleeHitEvent>)OnAttack, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HyposprayComponent, UseInHandEvent>((EntityEventRefHandler<HyposprayComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HyposprayComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<HyposprayComponent, GetVerbsEvent<AlternativeVerb>>)AddToggleModeVerb, (Type[])null, (Type[])null);
	}

	private void OnUseInHand(Entity<HyposprayComponent> entity, ref UseInHandEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = TryDoInject(entity, args.User, args.User);
		}
	}

	private void OnAfterInteract(Entity<HyposprayComponent> entity, ref AfterInteractEvent args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach && args.Target.HasValue)
		{
			((HandledEntityEventArgs)args).Handled = TryUseHypospray(entity, args.Target.Value, args.User);
		}
	}

	private void OnAttack(Entity<HyposprayComponent> entity, ref MeleeHitEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<EntityUid> hitEntities = args.HitEntities;
		if (hitEntities == null || hitEntities.Count != 0)
		{
			TryDoInject(entity, args.HitEntities[0], args.User);
		}
	}

	private bool TryUseHypospray(Entity<HyposprayComponent> entity, EntityUid target, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.CanContainerDraw && !EligibleEntity(target, Entity<HyposprayComponent>.op_Implicit(entity)) && _solutionContainers.TryGetDrawableSolution(Entity<DrawableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target), out Entity<SolutionComponent>? drawableSolution, out Solution _))
		{
			return TryDraw(entity, target, drawableSolution.Value, user);
		}
		return TryDoInject(entity, target, user);
	}

	public bool TryDoInject(Entity<HyposprayComponent> entity, EntityUid target, EntityUid user, bool doAfter = true)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		if (doAfter)
		{
			return _rmcHypospray.DoAfter(entity, target, user);
		}
		Entity<HyposprayComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		HyposprayComponent hyposprayComponent = default(HyposprayComponent);
		val.Deconstruct(ref val2, ref hyposprayComponent);
		EntityUid uid = val2;
		HyposprayComponent component = hyposprayComponent;
		if (!EligibleEntity(target, component))
		{
			return false;
		}
		UseDelayComponent delayComp = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(uid, ref delayComp) && _useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((uid, delayComp))))
		{
			return false;
		}
		string msgFormat = null;
		SelfBeforeHyposprayInjectsEvent selfEvent = new SelfBeforeHyposprayInjectsEvent(user, entity.Owner, target);
		((EntitySystem)this).RaiseLocalEvent<SelfBeforeHyposprayInjectsEvent>(user, selfEvent, false);
		if (((CancellableEntityEventArgs)selfEvent).Cancelled)
		{
			_popup.PopupClient(base.Loc.GetString(selfEvent.InjectMessageOverride ?? "hypospray-cant-inject", (ValueTuple<string, object>)("owner", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))), target, user);
			return false;
		}
		target = selfEvent.TargetGettingInjected;
		if (!EligibleEntity(target, component))
		{
			return false;
		}
		TargetBeforeHyposprayInjectsEvent targetEvent = new TargetBeforeHyposprayInjectsEvent(user, entity.Owner, target);
		((EntitySystem)this).RaiseLocalEvent<TargetBeforeHyposprayInjectsEvent>(target, targetEvent, false);
		if (((CancellableEntityEventArgs)targetEvent).Cancelled)
		{
			_popup.PopupClient(base.Loc.GetString(targetEvent.InjectMessageOverride ?? "hypospray-cant-inject", (ValueTuple<string, object>)("owner", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))), target, user);
			return false;
		}
		target = targetEvent.TargetGettingInjected;
		if (!EligibleEntity(target, component))
		{
			return false;
		}
		if (targetEvent.InjectMessageOverride != null)
		{
			msgFormat = targetEvent.InjectMessageOverride;
		}
		else if (selfEvent.InjectMessageOverride != null)
		{
			msgFormat = selfEvent.InjectMessageOverride;
		}
		else if (target == user)
		{
			msgFormat = "hypospray-component-inject-self-message";
		}
		if (!_solutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), component.SolutionName, out Entity<SolutionComponent>? hypoSpraySoln, out Solution hypoSpraySolution) || hypoSpraySolution.Volume == 0)
		{
			_popup.PopupClient(base.Loc.GetString("hypospray-component-empty-message"), target, user);
			return true;
		}
		if (!_solutionContainers.TryGetInjectableSolution(Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target), out Entity<SolutionComponent>? targetSoln, out Solution targetSolution))
		{
			_popup.PopupClient(base.Loc.GetString("hypospray-cant-inject", (ValueTuple<string, object>)("target", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))), target, user);
			return false;
		}
		_popup.PopupClient(base.Loc.GetString(msgFormat ?? "hypospray-component-inject-other-message", (ValueTuple<string, object>)("other", target)), target, user);
		if (target != user)
		{
			_popup.PopupEntity(base.Loc.GetString("hypospray-component-feel-prick-message"), target, target);
		}
		_audio.PlayPredicted(component.InjectSound, target, (EntityUid?)user, (AudioParams?)null);
		if (delayComp != null)
		{
			_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((uid, delayComp)));
		}
		FixedPoint2 realTransferAmount = FixedPoint2.Min(component.TransferAmount, targetSolution.AvailableVolume);
		if (realTransferAmount <= 0)
		{
			_popup.PopupClient(base.Loc.GetString("hypospray-component-transfer-already-full-message", (ValueTuple<string, object>)("owner", target)), target, user);
			return true;
		}
		Solution removedSolution = _solutionContainers.SplitSolution(hypoSpraySoln.Value, realTransferAmount);
		if (!targetSolution.CanAddSolution(removedSolution))
		{
			return true;
		}
		_reactiveSystem.DoEntityReaction(target, removedSolution, ReactionMethod.Injection);
		_solutionContainers.TryAddSolution(targetSoln.Value, removedSolution);
		TransferDnaEvent transferDnaEvent = new TransferDnaEvent();
		transferDnaEvent.Donor = target;
		transferDnaEvent.Recipient = uid;
		TransferDnaEvent ev = transferDnaEvent;
		((EntitySystem)this).RaiseLocalEvent<TransferDnaEvent>(target, ref ev, false);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(36, 4);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
		handler.AppendLiteral(" injected ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
		handler.AppendLiteral(" with a solution ");
		handler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(removedSolution), 0, "removedSolution");
		handler.AppendLiteral(" using a ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "using", "ToPrettyString(uid)");
		adminLogger.Add(LogType.ForceFeed, ref handler);
		return true;
	}

	private bool TryDraw(Entity<HyposprayComponent> entity, EntityUid target, Entity<SolutionComponent> targetSolution, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		if (!_solutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.SolutionName, out Entity<SolutionComponent>? soln, out Solution solution) || solution.AvailableVolume == 0)
		{
			return false;
		}
		FixedPoint2 realTransferAmount = FixedPoint2.Min(entity.Comp.TransferAmount, targetSolution.Comp.Solution.Volume, solution.AvailableVolume);
		if (realTransferAmount <= 0)
		{
			_popup.PopupClient(base.Loc.GetString("injector-component-target-is-empty-message", (ValueTuple<string, object>)("target", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))), entity.Owner, user);
			return false;
		}
		Solution removedSolution = _solutionContainers.Draw(Entity<DrawableSolutionComponent>.op_Implicit(target), targetSolution, realTransferAmount);
		if (!_solutionContainers.TryAddSolution(soln.Value, removedSolution))
		{
			return false;
		}
		_popup.PopupClient(base.Loc.GetString("injector-component-draw-success-message", (ValueTuple<string, object>)("amount", removedSolution.Volume), (ValueTuple<string, object>)("target", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))), entity.Owner, user);
		return true;
	}

	public bool EligibleEntity(EntityUid entity, HyposprayComponent component)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!component.OnlyAffectsMobs)
		{
			return ((EntitySystem)this).HasComp<SolutionContainerManagerComponent>(entity);
		}
		if (((EntitySystem)this).HasComp<SolutionContainerManagerComponent>(entity))
		{
			return ((EntitySystem)this).HasComp<MobStateComponent>(entity);
		}
		return false;
	}

	private void AddToggleModeVerb(Entity<HyposprayComponent> entity, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.Hands != null && !entity.Comp.InjectOnly)
		{
			EntityUid user = args.User;
			AlternativeVerb verb = new AlternativeVerb
			{
				Text = base.Loc.GetString("hypospray-verb-mode-label"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					ToggleMode(entity, user);
				}
			};
			args.Verbs.Add(verb);
		}
	}

	private void ToggleMode(Entity<HyposprayComponent> entity, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		SetMode(entity, !entity.Comp.OnlyAffectsMobs);
		string msg = ((entity.Comp.OnlyAffectsMobs && entity.Comp.CanContainerDraw) ? "hypospray-verb-mode-inject-mobs-only" : "hypospray-verb-mode-inject-all");
		_popup.PopupClient(base.Loc.GetString(msg), Entity<HyposprayComponent>.op_Implicit(entity), user);
	}

	public void SetMode(Entity<HyposprayComponent> entity, bool onlyAffectsMobs)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.OnlyAffectsMobs != onlyAffectsMobs)
		{
			entity.Comp.OnlyAffectsMobs = onlyAffectsMobs;
			((EntitySystem)this).Dirty<HyposprayComponent>(entity, (MetaDataComponent)null);
		}
	}
}
