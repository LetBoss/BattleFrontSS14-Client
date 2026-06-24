using System;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Fluids;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class PressurizedSolutionSystem : EntitySystem
{
	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private OpenableSystem _openable;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedPuddleSystem _puddle;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private RMCReagentSystem _reagents;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PressurizedSolutionComponent, MapInitEvent>((EntityEventRefHandler<PressurizedSolutionComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PressurizedSolutionComponent, ShakeEvent>((EntityEventRefHandler<PressurizedSolutionComponent, ShakeEvent>)OnShake, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PressurizedSolutionComponent, OpenableOpenedEvent>((EntityEventRefHandler<PressurizedSolutionComponent, OpenableOpenedEvent>)OnOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PressurizedSolutionComponent, LandEvent>((EntityEventRefHandler<PressurizedSolutionComponent, LandEvent>)OnLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PressurizedSolutionComponent, SolutionContainerChangedEvent>((EntityEventRefHandler<PressurizedSolutionComponent, SolutionContainerChangedEvent>)OnSolutionUpdate, (Type[])null, (Type[])null);
	}

	private bool SprayCheck(Entity<PressurizedSolutionComponent> entity, float chanceMod = 0f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return Fizziness(Entity<PressurizedSolutionComponent>.op_Implicit((Entity<PressurizedSolutionComponent>.op_Implicit(entity), entity.Comp))) + (double)chanceMod > (double)entity.Comp.SprayFizzinessThresholdRoll;
	}

	private float SolutionFizzability(Entity<PressurizedSolutionComponent> entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.Solution, out Entity<SolutionComponent>? _, out Solution solution))
		{
			return 0f;
		}
		if (solution.Volume <= 0)
		{
			return 0f;
		}
		float totalFizzability = 0f;
		foreach (ReagentQuantity reagent in solution.Contents)
		{
			if (_reagents.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(reagent.Reagent.Prototype), out Reagent reagentProto) && reagentProto != null)
			{
				float proportion = (float)(reagent.Quantity / solution.Volume);
				totalFizzability += reagentProto.Fizziness * proportion;
			}
		}
		return totalFizzability;
	}

	private void AddFizziness(Entity<PressurizedSolutionComponent> entity, float amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		float fizzability = SolutionFizzability(entity);
		if (fizzability <= 0f)
		{
			return;
		}
		AttemptAddFizzinessEvent attemptEv = new AttemptAddFizzinessEvent(entity, amount);
		((EntitySystem)this).RaiseLocalEvent<AttemptAddFizzinessEvent>(Entity<PressurizedSolutionComponent>.op_Implicit(entity), ref attemptEv, false);
		if (!attemptEv.Cancelled)
		{
			amount *= fizzability;
			TimeSpan duration = amount * entity.Comp.FizzinessMaxDuration;
			TimeSpan newTime = ((entity.Comp.FizzySettleTime > _timing.CurTime) ? entity.Comp.FizzySettleTime : _timing.CurTime) + duration;
			TimeSpan maxEnd = _timing.CurTime + entity.Comp.FizzinessMaxDuration;
			if (newTime > maxEnd)
			{
				newTime = maxEnd;
			}
			entity.Comp.FizzySettleTime = newTime;
			RollSprayThreshold(entity);
		}
	}

	private void SprayOrAddFizziness(Entity<PressurizedSolutionComponent> entity, float chanceMod = 0f, float fizzinessToAdd = 0f, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (SprayCheck(entity, chanceMod))
		{
			TrySpray(Entity<PressurizedSolutionComponent>.op_Implicit((Entity<PressurizedSolutionComponent>.op_Implicit(entity), entity.Comp)), user);
		}
		else
		{
			AddFizziness(entity, fizzinessToAdd);
		}
	}

	private void RollSprayThreshold(Entity<PressurizedSolutionComponent> entity)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			entity.Comp.SprayFizzinessThresholdRoll = _random.NextFloat();
			((EntitySystem)this).Dirty(Entity<PressurizedSolutionComponent>.op_Implicit(entity), (IComponent)(object)entity.Comp, (MetaDataComponent)null);
		}
	}

	public bool CanSpray(Entity<PressurizedSolutionComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PressurizedSolutionComponent>(Entity<PressurizedSolutionComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return false;
		}
		return SolutionFizzability(Entity<PressurizedSolutionComponent>.op_Implicit((Entity<PressurizedSolutionComponent>.op_Implicit(entity), entity.Comp))) > 0f;
	}

	public bool TrySpray(Entity<PressurizedSolutionComponent?> entity, EntityUid? target = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PressurizedSolutionComponent>(Entity<PressurizedSolutionComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		if (!CanSpray(entity))
		{
			return false;
		}
		if (!_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.Solution, out Entity<SolutionComponent>? soln, out Solution interactions))
		{
			return false;
		}
		_openable.SetOpen(Entity<PressurizedSolutionComponent>.op_Implicit(entity));
		Solution solution = _solutionContainer.SplitSolution(soln.Value, interactions.Volume);
		TransformComponent transform = default(TransformComponent);
		if (((EntitySystem)this).TryComp(Entity<PressurizedSolutionComponent>.op_Implicit(entity), ref transform))
		{
			_puddle.TrySplashSpillAt(Entity<PressurizedSolutionComponent>.op_Implicit(entity), transform.Coordinates, solution, out var _, sound: false);
		}
		EntityUid drinkName = Identity.Entity(Entity<PressurizedSolutionComponent>.op_Implicit(entity), (IEntityManager)(object)base.EntityManager);
		if (target.HasValue)
		{
			EntityUid victimName = Identity.Entity(target.Value, (IEntityManager)(object)base.EntityManager);
			string selfMessage = base.Loc.GetString(LocId.op_Implicit(entity.Comp.SprayHolderMessageSelf), (ValueTuple<string, object>)("victim", victimName), (ValueTuple<string, object>)("drink", drinkName));
			string othersMessage = base.Loc.GetString(LocId.op_Implicit(entity.Comp.SprayHolderMessageOthers), (ValueTuple<string, object>)("victim", victimName), (ValueTuple<string, object>)("drink", drinkName));
			_popup.PopupPredicted(selfMessage, othersMessage, target.Value, target.Value);
		}
		else if (_timing.IsFirstTimePredicted)
		{
			_popup.PopupEntity(base.Loc.GetString(LocId.op_Implicit(entity.Comp.SprayGroundMessage), (ValueTuple<string, object>)("drink", drinkName)), Entity<PressurizedSolutionComponent>.op_Implicit(entity));
		}
		_audio.PlayPredicted(entity.Comp.SpraySound, Entity<PressurizedSolutionComponent>.op_Implicit(entity), target, (AudioParams?)null);
		TryClearFizziness(entity);
		return true;
	}

	public double Fizziness(Entity<PressurizedSolutionComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PressurizedSolutionComponent>(Entity<PressurizedSolutionComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return 0.0;
		}
		if (entity.Comp.FizzySettleTime <= _timing.CurTime)
		{
			return 0.0;
		}
		return Easings.InOutCubic((float)Math.Min((entity.Comp.FizzySettleTime - _timing.CurTime) / entity.Comp.FizzinessMaxDuration, 1.0));
	}

	public void TryClearFizziness(Entity<PressurizedSolutionComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PressurizedSolutionComponent>(Entity<PressurizedSolutionComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			entity.Comp.FizzySettleTime = TimeSpan.Zero;
			RollSprayThreshold(Entity<PressurizedSolutionComponent>.op_Implicit((Entity<PressurizedSolutionComponent>.op_Implicit(entity), entity.Comp)));
		}
	}

	private void OnMapInit(Entity<PressurizedSolutionComponent> entity, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RollSprayThreshold(entity);
	}

	private void OnOpened(Entity<PressurizedSolutionComponent> entity, ref OpenableOpenedEvent args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		string inHand;
		bool held = args.User.HasValue && _hands.IsHolding(Entity<HandsComponent>.op_Implicit(args.User.Value), Entity<PressurizedSolutionComponent>.op_Implicit(entity), out inHand);
		SprayOrAddFizziness(entity, entity.Comp.SprayChanceModOnOpened, -1f, held ? args.User : ((EntityUid?)null));
	}

	private void OnShake(Entity<PressurizedSolutionComponent> entity, ref ShakeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SprayOrAddFizziness(entity, entity.Comp.SprayChanceModOnShake, entity.Comp.FizzinessAddedOnShake, args.Shaker);
	}

	private void OnLand(Entity<PressurizedSolutionComponent> entity, ref LandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SprayOrAddFizziness(entity, entity.Comp.SprayChanceModOnLand, entity.Comp.FizzinessAddedOnLand);
	}

	private void OnSolutionUpdate(Entity<PressurizedSolutionComponent> entity, ref SolutionContainerChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.SolutionId != entity.Comp.Solution) && SolutionFizzability(entity) <= 0f)
		{
			TryClearFizziness(Entity<PressurizedSolutionComponent>.op_Implicit((Entity<PressurizedSolutionComponent>.op_Implicit(entity), entity.Comp)));
		}
	}
}
