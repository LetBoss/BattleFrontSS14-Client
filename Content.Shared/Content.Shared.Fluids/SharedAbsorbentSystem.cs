using System;
using System.Numerics;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids.Components;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared.Fluids;

public abstract class SharedAbsorbentSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popups;

	[Dependency]
	protected SharedPuddleSystem Puddle;

	[Dependency]
	private SharedMeleeWeaponSystem _melee;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	protected SharedSolutionContainerSystem SolutionContainer;

	[Dependency]
	private UseDelaySystem _useDelay;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private SharedItemSystem _item;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AbsorbentComponent, AfterInteractEvent>((EntityEventRefHandler<AbsorbentComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AbsorbentComponent, UserActivateInWorldEvent>((EntityEventRefHandler<AbsorbentComponent, UserActivateInWorldEvent>)OnActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AbsorbentComponent, SolutionContainerChangedEvent>((EntityEventRefHandler<AbsorbentComponent, SolutionContainerChangedEvent>)OnAbsorbentSolutionChange, (Type[])null, (Type[])null);
	}

	private void OnActivateInWorld(Entity<AbsorbentComponent> ent, ref UserActivateInWorldEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			Mop(ent, args.User, args.Target);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnAfterInteract(Entity<AbsorbentComponent> ent, ref AfterInteractEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanReach && !((HandledEntityEventArgs)args).Handled)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				Mop(ent, args.User, target2);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnAbsorbentSolutionChange(Entity<AbsorbentComponent> ent, ref SolutionContainerChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (SolutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.SolutionName, out Entity<SolutionComponent>? _, out Solution solution))
		{
			ent.Comp.Progress.Clear();
			string[] absorbentReagents = Puddle.GetAbsorbentReagents(solution);
			FixedPoint2 mopReagent = solution.GetTotalPrototypeQuantity(absorbentReagents);
			if (mopReagent > FixedPoint2.Zero)
			{
				ent.Comp.Progress[solution.GetColorWithOnly(_proto, absorbentReagents)] = mopReagent.Float();
			}
			Color otherColor = solution.GetColorWithout(_proto, absorbentReagents);
			FixedPoint2 other = solution.Volume - mopReagent;
			if (other > FixedPoint2.Zero)
			{
				ent.Comp.Progress[otherColor] = other.Float();
			}
			if (solution.AvailableVolume > FixedPoint2.Zero)
			{
				ent.Comp.Progress[Color.DarkGray] = solution.AvailableVolume.Float();
			}
			((EntitySystem)this).Dirty<AbsorbentComponent>(ent, (MetaDataComponent)null);
			_item.VisualsChanged(Entity<AbsorbentComponent>.op_Implicit(ent));
		}
	}

	[Obsolete("Use Entity<T> variant")]
	public void Mop(EntityUid user, EntityUid target, EntityUid used, AbsorbentComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Mop(Entity<AbsorbentComponent>.op_Implicit((used, component)), user, target);
	}

	public void Mop(Entity<AbsorbentComponent> absorbEnt, EntityUid user, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (SolutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(absorbEnt.Owner), absorbEnt.Comp.SolutionName, out Entity<SolutionComponent>? absorberSoln) && (!((EntitySystem)this).TryComp<UseDelayComponent>(Entity<AbsorbentComponent>.op_Implicit(absorbEnt), ref useDelay) || !_useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((absorbEnt.Owner, useDelay)))) && !TryPuddleInteract(Entity<AbsorbentComponent, UseDelayComponent>.op_Implicit((absorbEnt.Owner, absorbEnt.Comp, useDelay)), absorberSoln.Value, user, target) && absorbEnt.Comp.UseAbsorberSolution)
		{
			TryRefillableInteract(Entity<AbsorbentComponent, UseDelayComponent>.op_Implicit((absorbEnt.Owner, absorbEnt.Comp, useDelay)), absorberSoln.Value, user, target);
		}
	}

	private bool TryRefillableInteract(Entity<AbsorbentComponent, UseDelayComponent?> absorbEnt, Entity<SolutionComponent> absorbentSoln, EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		RefillableSolutionComponent refillable = default(RefillableSolutionComponent);
		if (!((EntitySystem)this).TryComp<RefillableSolutionComponent>(target, ref refillable))
		{
			return false;
		}
		if (!SolutionContainer.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit((ValueTuple<EntityUid, RefillableSolutionComponent, SolutionContainerManagerComponent>)(target, refillable, null)), out Entity<SolutionComponent>? refillableSoln, out Solution refillableSolution))
		{
			return false;
		}
		if (refillableSolution.Volume <= 0)
		{
			if (!TryTransferFromAbsorbentToRefillable(Entity<AbsorbentComponent, UseDelayComponent>.op_Implicit(absorbEnt), absorbentSoln, refillableSoln.Value, user, target))
			{
				return false;
			}
		}
		else if (!TryTwoWayAbsorbentRefillableTransfer(Entity<AbsorbentComponent, UseDelayComponent>.op_Implicit(absorbEnt), absorbentSoln, refillableSoln.Value, user, target))
		{
			return false;
		}
		Entity<AbsorbentComponent, UseDelayComponent> val = absorbEnt;
		EntityUid val2 = default(EntityUid);
		AbsorbentComponent absorbentComponent = default(AbsorbentComponent);
		UseDelayComponent useDelayComponent = default(UseDelayComponent);
		val.Deconstruct(ref val2, ref absorbentComponent, ref useDelayComponent);
		EntityUid used = val2;
		AbsorbentComponent absorber = absorbentComponent;
		UseDelayComponent useDelay = useDelayComponent;
		_audio.PlayPredicted(absorber.TransferSound, target, (EntityUid?)user, (AudioParams?)null);
		if (useDelay != null)
		{
			_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((used, useDelay)));
		}
		return true;
	}

	private bool TryTransferFromAbsorbentToRefillable(Entity<AbsorbentComponent> absorbEnt, Entity<SolutionComponent> absorbentSoln, Entity<SolutionComponent> refillableSoln, EntityUid user, EntityUid target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		if (absorbentSoln.Comp.Solution.Volume <= 0)
		{
			_popups.PopupClient(base.Loc.GetString("mopping-system-target-container-empty", (ValueTuple<string, object>)("target", target)), user, user);
			return false;
		}
		Solution refillableSolution = refillableSoln.Comp.Solution;
		FixedPoint2 transferAmount = ((absorbEnt.Comp.PickupAmount < refillableSolution.AvailableVolume) ? absorbEnt.Comp.PickupAmount : refillableSolution.AvailableVolume);
		if (transferAmount <= 0)
		{
			_popups.PopupClient(base.Loc.GetString("mopping-system-full", (ValueTuple<string, object>)("used", absorbEnt)), Entity<AbsorbentComponent>.op_Implicit(absorbEnt), user);
			return false;
		}
		Solution contaminants = SolutionContainer.SplitSolutionWithout(absorbentSoln, transferAmount, Puddle.GetAbsorbentReagents(absorbentSoln.Comp.Solution));
		SolutionContainer.TryAddSolution(refillableSoln, (contaminants.Volume > 0) ? contaminants : SolutionContainer.SplitSolution(absorbentSoln, transferAmount));
		return true;
	}

	private bool TryTwoWayAbsorbentRefillableTransfer(Entity<AbsorbentComponent> absorbEnt, Entity<SolutionComponent> absorbentSoln, Entity<SolutionComponent> refillableSoln, EntityUid user, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		Solution contaminantsFromAbsorbent = SolutionContainer.SplitSolutionWithout(absorbentSoln, absorbEnt.Comp.PickupAmount, Puddle.GetAbsorbentReagents(absorbentSoln.Comp.Solution));
		Solution absorbentSolution = absorbentSoln.Comp.Solution;
		if (contaminantsFromAbsorbent.Volume == FixedPoint2.Zero && absorbentSolution.AvailableVolume == FixedPoint2.Zero)
		{
			_popups.PopupClient(base.Loc.GetString("mopping-system-puddle-space", (ValueTuple<string, object>)("used", absorbEnt)), user, user);
			return false;
		}
		FixedPoint2 waterPulled = ((absorbEnt.Comp.PickupAmount < absorbentSolution.AvailableVolume) ? absorbEnt.Comp.PickupAmount : absorbentSolution.AvailableVolume);
		Solution refillableSolution = refillableSoln.Comp.Solution;
		Solution waterFromRefillable = refillableSolution.SplitSolutionWithOnly(waterPulled, Puddle.GetAbsorbentReagents(refillableSoln.Comp.Solution));
		SolutionContainer.UpdateChemicals(refillableSoln);
		if (waterFromRefillable.Volume == FixedPoint2.Zero && contaminantsFromAbsorbent.Volume == FixedPoint2.Zero)
		{
			_popups.PopupClient(base.Loc.GetString("mopping-system-target-container-empty-water", (ValueTuple<string, object>)("target", target)), user, user);
			return false;
		}
		bool anyTransferOccurred = false;
		if (waterFromRefillable.Volume > FixedPoint2.Zero)
		{
			SolutionContainer.TryAddSolution(absorbentSoln, waterFromRefillable);
			anyTransferOccurred = true;
		}
		if (contaminantsFromAbsorbent.Volume <= 0)
		{
			return anyTransferOccurred;
		}
		if (refillableSolution.AvailableVolume <= 0)
		{
			_popups.PopupClient(base.Loc.GetString("mopping-system-full", (ValueTuple<string, object>)("used", target)), user, user);
		}
		else
		{
			Solution contaminantsForRefillable = contaminantsFromAbsorbent.SplitSolution(refillableSolution.AvailableVolume);
			SolutionContainer.TryAddSolution(refillableSoln, contaminantsForRefillable);
			anyTransferOccurred = true;
		}
		SolutionContainer.TryAddSolution(absorbentSoln, contaminantsFromAbsorbent);
		return anyTransferOccurred;
	}

	private bool TryPuddleInteract(Entity<AbsorbentComponent, UseDelayComponent?> absorbEnt, Entity<SolutionComponent> absorberSoln, EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		PuddleComponent puddle = default(PuddleComponent);
		if (!((EntitySystem)this).TryComp<PuddleComponent>(target, ref puddle))
		{
			return false;
		}
		if (!SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(target), puddle.SolutionName, ref puddle.Solution, out Solution puddleSolution) || puddleSolution.Volume <= 0)
		{
			return false;
		}
		Entity<AbsorbentComponent, UseDelayComponent> val = absorbEnt;
		EntityUid val2 = default(EntityUid);
		AbsorbentComponent absorbentComponent = default(AbsorbentComponent);
		UseDelayComponent useDelayComponent = default(UseDelayComponent);
		val.Deconstruct(ref val2, ref absorbentComponent, ref useDelayComponent);
		AbsorbentComponent absorber = absorbentComponent;
		UseDelayComponent useDelay = useDelayComponent;
		bool isRemoved = false;
		Solution puddleSplit;
		Angle val3;
		if (absorber.UseAbsorberSolution)
		{
			if (puddleSolution.GetTotalPrototypeQuantity(Puddle.GetAbsorbentReagents(puddleSolution)) == puddleSolution.Volume)
			{
				_popups.PopupClient(base.Loc.GetString("mopping-system-puddle-already-mopped", (ValueTuple<string, object>)("target", target)), target, user);
				return true;
			}
			Solution absorberSolution = absorberSoln.Comp.Solution;
			FixedPoint2 available = absorberSolution.GetTotalPrototypeQuantity(Puddle.GetAbsorbentReagents(absorberSolution));
			if (available == FixedPoint2.Zero)
			{
				_popups.PopupClient(base.Loc.GetString("mopping-system-no-water", (ValueTuple<string, object>)("used", absorbEnt)), Entity<AbsorbentComponent, UseDelayComponent>.op_Implicit(absorbEnt), user);
				return true;
			}
			FixedPoint2 transferMax = absorber.PickupAmount;
			FixedPoint2 transferAmount = ((available > transferMax) ? transferMax : available);
			puddleSplit = puddleSolution.SplitSolutionWithout(transferAmount, Puddle.GetAbsorbentReagents(puddleSolution));
			Solution absorberSplit = absorberSolution.SplitSolutionWithOnly(puddleSplit.Volume, Puddle.GetAbsorbentReagents(absorberSolution));
			TransformComponent targetXform = ((EntitySystem)this).Transform(target);
			EntityUid? gridUid = targetXform.GridUid;
			MapGridComponent mapGrid = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref mapGrid))
			{
				TileRef tileRef = _mapSystem.GetTileRef(gridUid.Value, mapGrid, targetXform.Coordinates);
				Puddle.DoTileReactions(tileRef, absorberSplit);
			}
			SolutionContainer.AddSolution(puddle.Solution.Value, absorberSplit);
		}
		else
		{
			puddleSplit = puddleSolution.SplitSolutionWithout(absorber.PickupAmount, Puddle.GetAbsorbentReagents(puddleSolution));
			if (puddleSolution.Volume == FixedPoint2.Zero)
			{
				string text = EntProtoId.op_Implicit(absorber.MoppedEffect);
				EntityCoordinates coordinates = ((EntitySystem)this).Transform(target).Coordinates;
				val3 = default(Angle);
				((EntitySystem)this).PredictedSpawnAttachedTo(text, coordinates, (ComponentRegistry)null, val3);
				((EntitySystem)this).PredictedQueueDel(target);
				isRemoved = true;
			}
		}
		SolutionContainer.AddSolution(absorberSoln, puddleSplit);
		_audio.PlayPredicted(absorber.PickupSound, isRemoved ? Entity<AbsorbentComponent, UseDelayComponent>.op_Implicit(absorbEnt) : target, (EntityUid?)user, (AudioParams?)null);
		if (useDelay != null)
		{
			_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((Entity<AbsorbentComponent, UseDelayComponent>.op_Implicit(absorbEnt), useDelay)));
		}
		TransformComponent userXform = ((EntitySystem)this).Transform(user);
		Vector2 localPos = Vector2.Transform(_transform.GetWorldPosition(target), _transform.GetInvWorldMatrix(userXform));
		val3 = userXform.LocalRotation;
		localPos = ((Angle)(ref val3)).RotateVec(ref localPos);
		_melee.DoLunge(user, Entity<AbsorbentComponent, UseDelayComponent>.op_Implicit(absorbEnt), Angle.Zero, localPos, null);
		return true;
	}
}
