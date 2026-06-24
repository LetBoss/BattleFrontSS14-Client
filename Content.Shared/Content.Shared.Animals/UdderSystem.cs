using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Udder;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Animals;

public sealed class UdderSystem : EntitySystem
{
	[Dependency]
	private HungerSystem _hunger;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainerSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<UdderComponent, MapInitEvent>((ComponentEventHandler<UdderComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UdderComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<UdderComponent, GetVerbsEvent<AlternativeVerb>>)AddMilkVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UdderComponent, MilkingDoAfterEvent>((EntityEventRefHandler<UdderComponent, MilkingDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UdderComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<UdderComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, UdderComponent component, MapInitEvent args)
	{
		component.NextGrowth = _timing.CurTime + component.GrowthDelay;
	}

	private void OnEntRemoved(Entity<UdderComponent> entity, ref EntRemovedFromContainerMessage args)
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
		EntityQueryEnumerator<UdderComponent> query = ((EntitySystem)this).EntityQueryEnumerator<UdderComponent>();
		EntityUid uid = default(EntityUid);
		UdderComponent udder = default(UdderComponent);
		HungerComponent hunger = default(HungerComponent);
		while (query.MoveNext(ref uid, ref udder))
		{
			if (_timing.CurTime < udder.NextGrowth)
			{
				continue;
			}
			udder.NextGrowth += udder.GrowthDelay;
			if (_mobState.IsDead(uid) || !_solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), udder.SolutionName, ref udder.Solution, out Solution solution) || solution.AvailableVolume == 0)
			{
				continue;
			}
			if (((EntitySystem)this).TryComp<HungerComponent>(uid, ref hunger))
			{
				if ((int)_hunger.GetHungerThreshold(hunger) < 4)
				{
					continue;
				}
				_hunger.ModifyHunger(uid, 0f - udder.HungerUsage, hunger);
			}
			_solutionContainerSystem.TryAddReagent(udder.Solution.Value, ProtoId<ReagentPrototype>.op_Implicit(udder.ReagentId), udder.QuantityPerUpdate, out var _);
		}
	}

	private void AttemptMilk(Entity<UdderComponent?> udder, EntityUid userUid, EntityUid containerUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<UdderComponent>(Entity<UdderComponent>.op_Implicit(udder), ref udder.Comp, true))
		{
			DoAfterArgs doargs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, userUid, 5f, new MilkingDoAfterEvent(), Entity<UdderComponent>.op_Implicit(udder), Entity<UdderComponent>.op_Implicit(udder), containerUid)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				MovementThreshold = 1f
			};
			_doAfterSystem.TryStartDoAfter(doargs);
		}
	}

	private void OnDoAfter(Entity<UdderComponent> entity, ref MilkingDoAfterEvent args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled || !args.Args.Used.HasValue || !_solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.SolutionName, ref entity.Comp.Solution, out Solution solution) || !_solutionContainerSystem.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(args.Args.Used.Value), out Entity<SolutionComponent>? targetSoln, out Solution targetSolution))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		FixedPoint2 quantity = solution.Volume;
		if (quantity == 0)
		{
			_popupSystem.PopupClient(base.Loc.GetString("udder-system-dry"), entity.Owner, args.Args.User);
			return;
		}
		if (quantity > targetSolution.AvailableVolume)
		{
			quantity = targetSolution.AvailableVolume;
		}
		Solution split = _solutionContainerSystem.SplitSolution(entity.Comp.Solution.Value, quantity);
		_solutionContainerSystem.TryAddSolution(targetSoln.Value, split);
		_popupSystem.PopupClient(base.Loc.GetString("udder-system-success", (ValueTuple<string, object>)("amount", quantity), (ValueTuple<string, object>)("target", Identity.Entity(args.Args.Used.Value, (IEntityManager)(object)base.EntityManager))), entity.Owner, args.Args.User, PopupType.Medium);
	}

	private void AddMilkVerb(Entity<UdderComponent> entity, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Using.HasValue && args.CanInteract && ((EntitySystem)this).HasComp<RefillableSolutionComponent>(args.Using.Value))
		{
			EntityUid uid = entity.Owner;
			EntityUid user = args.User;
			EntityUid used = args.Using.Value;
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					AttemptMilk(Entity<UdderComponent>.op_Implicit(uid), user, used);
				},
				Text = base.Loc.GetString("udder-system-verb-milk"),
				Priority = 2
			};
			args.Verbs.Add(verb);
		}
	}
}
