using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Spillable;
using Content.Shared.StepTrigger.Components;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared.Fluids;

public abstract class SharedPuddleSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainerSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private RMCReagentSystem _rmcReagents;

	private static readonly ProtoId<ReagentPrototype> Blood = ProtoId<ReagentPrototype>.op_Implicit("Blood");

	private static readonly ProtoId<ReagentPrototype> Slime = ProtoId<ReagentPrototype>.op_Implicit("Slime");

	private static readonly ProtoId<ReagentPrototype> CopperBlood = ProtoId<ReagentPrototype>.op_Implicit("CopperBlood");

	private static readonly string[] StandoutReagents = new string[3]
	{
		ProtoId<ReagentPrototype>.op_Implicit(Blood),
		ProtoId<ReagentPrototype>.op_Implicit(Slime),
		ProtoId<ReagentPrototype>.op_Implicit(CopperBlood)
	};

	public const float LowThreshold = 0.3f;

	public const float MediumThreshold = 0.6f;

	[Dependency]
	protected OpenableSystem Openable;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RefillableSolutionComponent, CanDragEvent>((EntityEventRefHandler<RefillableSolutionComponent, CanDragEvent>)OnRefillableCanDrag, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DumpableSolutionComponent, CanDropTargetEvent>((EntityEventRefHandler<DumpableSolutionComponent, CanDropTargetEvent>)OnDumpCanDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DrainableSolutionComponent, CanDropTargetEvent>((EntityEventRefHandler<DrainableSolutionComponent, CanDropTargetEvent>)OnDrainCanDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RefillableSolutionComponent, CanDropDraggedEvent>((EntityEventRefHandler<RefillableSolutionComponent, CanDropDraggedEvent>)OnRefillableCanDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PuddleComponent, SolutionContainerChangedEvent>((EntityEventRefHandler<PuddleComponent, SolutionContainerChangedEvent>)OnSolutionUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PuddleComponent, GetFootstepSoundEvent>((EntityEventRefHandler<PuddleComponent, GetFootstepSoundEvent>)OnGetFootstepSound, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PuddleComponent, ExaminedEvent>((EntityEventRefHandler<PuddleComponent, ExaminedEvent>)HandlePuddleExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PuddleComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<PuddleComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
		InitializeSpillable();
	}

	protected virtual void OnSolutionUpdate(Entity<PuddleComponent> entity, ref SolutionContainerChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.SolutionId != entity.Comp.SolutionName))
		{
			UpdateAppearance(Entity<PuddleComponent, AppearanceComponent>.op_Implicit((Entity<PuddleComponent>.op_Implicit(entity), entity.Comp)));
		}
	}

	private void OnRefillableCanDrag(Entity<RefillableSolutionComponent> entity, ref CanDragEvent args)
	{
		args.Handled = true;
	}

	private void OnDumpCanDropTarget(Entity<DumpableSolutionComponent> entity, ref CanDropTargetEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<DrainableSolutionComponent>(args.Dragged))
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnDrainCanDropTarget(Entity<DrainableSolutionComponent> entity, ref CanDropTargetEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<RefillableSolutionComponent>(args.Dragged))
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnRefillableCanDropDragged(Entity<RefillableSolutionComponent> entity, ref CanDropDraggedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<DrainableSolutionComponent>(args.Target) || ((EntitySystem)this).HasComp<DumpableSolutionComponent>(args.Target))
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnGetFootstepSound(Entity<PuddleComponent> entity, ref GetFootstepSoundEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (_solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.SolutionName, ref entity.Comp.Solution, out Solution solution))
		{
			ReagentId? reagentId = solution.GetPrimaryReagentId();
			if (!string.IsNullOrWhiteSpace(reagentId?.Prototype) && _rmcReagents.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(reagentId.Value.Prototype), out Reagent proto))
			{
				args.Sound = proto.FootstepSound;
			}
		}
	}

	private void HandlePuddleExamined(Entity<PuddleComponent> entity, ref ExaminedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("PuddleComponent"))
		{
			StepTriggerComponent slippery = default(StepTriggerComponent);
			if (((EntitySystem)this).TryComp<StepTriggerComponent>(Entity<PuddleComponent>.op_Implicit(entity), ref slippery) && slippery.Active)
			{
				args.PushMarkup(base.Loc.GetString("puddle-component-examine-is-slippery-text"));
			}
			if (((EntitySystem)this).HasComp<EvaporationComponent>(Entity<PuddleComponent>.op_Implicit(entity)) && _solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.SolutionName, ref entity.Comp.Solution, out Solution solution))
			{
				if (CanFullyEvaporate(solution))
				{
					args.PushMarkup(base.Loc.GetString("puddle-component-examine-evaporating"));
				}
				else if (solution.GetTotalPrototypeQuantity(GetEvaporatingReagents(solution)) > FixedPoint2.Zero)
				{
					args.PushMarkup(base.Loc.GetString("puddle-component-examine-evaporating-partial"));
				}
				else
				{
					args.PushMarkup(base.Loc.GetString("puddle-component-examine-evaporating-no"));
				}
			}
			else
			{
				args.PushMarkup(base.Loc.GetString("puddle-component-examine-evaporating-no"));
			}
		}
	}

	private void OnEntRemoved(Entity<PuddleComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((ContainerModifiedMessage)args).Entity;
		EntityUid? val = ent.Comp.Solution?.Owner;
		if (val.HasValue && entity == val.GetValueOrDefault())
		{
			ent.Comp.Solution = null;
		}
	}

	private void UpdateAppearance(Entity<PuddleComponent?, AppearanceComponent?> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		Entity<PuddleComponent, AppearanceComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		PuddleComponent puddleComponent = default(PuddleComponent);
		AppearanceComponent val3 = default(AppearanceComponent);
		val.Deconstruct(ref val2, ref puddleComponent, ref val3);
		EntityUid uid = val2;
		PuddleComponent puddle = puddleComponent;
		AppearanceComponent appearance = val3;
		if (!((EntitySystem)this).Resolve<PuddleComponent, AppearanceComponent>(Entity<PuddleComponent, AppearanceComponent>.op_Implicit(ent), ref puddle, ref appearance, true))
		{
			return;
		}
		FixedPoint2 volume = FixedPoint2.Zero;
		Color color = Color.White;
		if (_solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), puddle.SolutionName, ref puddle.Solution, out Solution solution))
		{
			volume = solution.Volume / puddle.OverflowVolume;
			color = solution.GetColorWithout(_prototypeManager, StandoutReagents);
			color = ((Color)(ref color)).WithAlpha(0.7f);
			string[] standoutReagents = StandoutReagents;
			foreach (string standout in standoutReagents)
			{
				FixedPoint2 quantity = solution.GetTotalPrototypeQuantity(standout);
				if (!(quantity <= FixedPoint2.Zero))
				{
					float interpolateValue = quantity.Float() / solution.Volume.Float();
					color = Color.InterpolateBetween(color, _rmcReagents.Index(ProtoId<ReagentPrototype>.op_Implicit(standout)).SubstanceColor, interpolateValue);
				}
			}
		}
		_appearance.SetData(Entity<PuddleComponent, AppearanceComponent>.op_Implicit(ent), (Enum)PuddleVisuals.CurrentVolume, (object)volume.Float(), appearance);
		_appearance.SetData(Entity<PuddleComponent, AppearanceComponent>.op_Implicit(ent), (Enum)PuddleVisuals.SolutionColor, (object)color, appearance);
	}

	public void DoTileReactions(TileRef tileRef, Solution solution)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = solution.Contents.Count - 1; i >= 0; i--)
		{
			solution.Contents[i].Deconstruct(out var id, out var quantity);
			ReagentId reagent = id;
			FixedPoint2 quantity2 = quantity;
			FixedPoint2 removed = _rmcReagents.Index(ProtoId<ReagentPrototype>.op_Implicit(reagent.Prototype)).ReactionTile(tileRef, quantity2, (IEntityManager)(object)base.EntityManager, reagent.Data);
			if (!(removed <= FixedPoint2.Zero))
			{
				solution.RemoveReagent(reagent, removed);
			}
		}
	}

	public abstract bool TrySplashSpillAt(EntityUid uid, EntityCoordinates coordinates, Solution solution, out EntityUid puddleUid, bool sound = true, EntityUid? user = null);

	public abstract bool TrySpillAt(EntityCoordinates coordinates, Solution solution, out EntityUid puddleUid, bool sound = true);

	public abstract bool TrySpillAt(EntityUid uid, Solution solution, out EntityUid puddleUid, bool sound = true, TransformComponent? transformComponent = null);

	public abstract bool TrySpillAt(TileRef tileRef, Solution solution, out EntityUid puddleUid, bool sound = true, bool tileReact = true);

	public string[] GetEvaporatingReagents(Solution solution)
	{
		List<string> evaporatingReagents = new List<string>();
		foreach (ReagentPrototype solProto in solution.GetReagentPrototypes(_prototypeManager).Keys)
		{
			if (solProto.EvaporationSpeed > FixedPoint2.Zero)
			{
				evaporatingReagents.Add(solProto.ID);
			}
		}
		return evaporatingReagents.ToArray();
	}

	public string[] GetAbsorbentReagents(Solution solution)
	{
		List<string> absorbentReagents = new List<string>();
		foreach (ReagentPrototype solProto in solution.GetReagentPrototypes(_prototypeManager).Keys)
		{
			if (solProto.Absorbent)
			{
				absorbentReagents.Add(solProto.ID);
			}
		}
		return absorbentReagents.ToArray();
	}

	public bool CanFullyEvaporate(Solution solution)
	{
		return solution.GetTotalPrototypeQuantity(GetEvaporatingReagents(solution)) == solution.Volume;
	}

	public Dictionary<string, FixedPoint2> GetEvaporationSpeeds(Solution solution)
	{
		Dictionary<string, FixedPoint2> evaporatingSpeeds = new Dictionary<string, FixedPoint2>();
		foreach (ReagentPrototype solProto in solution.GetReagentPrototypes(_prototypeManager).Keys)
		{
			if (solProto.EvaporationSpeed > FixedPoint2.Zero)
			{
				evaporatingSpeeds.Add(solProto.ID, solProto.EvaporationSpeed);
			}
		}
		return evaporatingSpeeds;
	}

	protected virtual void InitializeSpillable()
	{
		((EntitySystem)this).SubscribeLocalEvent<SpillableComponent, ExaminedEvent>((EntityEventRefHandler<SpillableComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpillableComponent, GetVerbsEvent<Verb>>((EntityEventRefHandler<SpillableComponent, GetVerbsEvent<Verb>>)AddSpillVerb, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<SpillableComponent> entity, ref ExaminedEvent args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("SpillableComponent"))
		{
			args.PushMarkup(base.Loc.GetString("spill-examine-is-spillable"));
			if (((EntitySystem)this).HasComp<MeleeWeaponComponent>(Entity<SpillableComponent>.op_Implicit(entity)))
			{
				args.PushMarkup(base.Loc.GetString("spill-examine-spillable-weapon"));
			}
		}
	}

	private void AddSpillVerb(Entity<SpillableComponent> entity, ref GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || args.Hands == null || !_solutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(args.Target), entity.Comp.SolutionName, out Entity<SolutionComponent>? soln, out Solution solution) || Openable.IsClosed(args.Target) || solution.Volume == FixedPoint2.Zero)
		{
			return;
		}
		Verb verb = new Verb
		{
			Text = base.Loc.GetString("spill-target-verb-get-data-text")
		};
		if (!entity.Comp.SpillDelay.HasValue)
		{
			EntityUid target = args.Target;
			verb.Act = delegate
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				Solution solution2 = _solutionContainerSystem.SplitSolution(soln.Value, solution.Volume);
				TrySpillAt(((EntitySystem)this).Transform(target).Coordinates, solution2, out var _);
				InjectorComponent injectorComponent = default(InjectorComponent);
				if (((EntitySystem)this).TryComp<InjectorComponent>(Entity<SpillableComponent>.op_Implicit(entity), ref injectorComponent))
				{
					injectorComponent.ToggleState = InjectorToggleMode.Draw;
					((EntitySystem)this).Dirty(Entity<SpillableComponent>.op_Implicit(entity), (IComponent)(object)injectorComponent, (MetaDataComponent)null);
				}
			};
		}
		else
		{
			EntityUid user = args.User;
			verb.Act = delegate
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				_doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, entity.Comp.SpillDelay.GetValueOrDefault(), new SpillDoAfterEvent(), entity.Owner, entity.Owner)
				{
					BreakOnDamage = true,
					BreakOnMove = true,
					NeedHand = true
				});
			};
		}
		verb.Impact = LogImpact.Medium;
		verb.DoContactInteraction = true;
		args.Verbs.Add(verb);
	}
}
