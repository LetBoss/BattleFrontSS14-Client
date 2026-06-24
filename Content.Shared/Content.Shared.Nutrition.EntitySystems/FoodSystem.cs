using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class FoodSystem : EntitySystem
{
	[Dependency]
	private SharedBodySystem _body;

	[Dependency]
	private FlavorProfileSystem _flavorProfile;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private OpenableSystem _openable;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private ReactiveSystem _reaction;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	private StomachSystem _stomach;

	[Dependency]
	private UtensilSystem _utensil;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public const float MaxFeedDistance = 1.5f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FoodComponent, UseInHandEvent>((EntityEventRefHandler<FoodComponent, UseInHandEvent>)OnUseFoodInHand, (Type[])null, new Type[2]
		{
			typeof(OpenableSystem),
			typeof(InventorySystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<FoodComponent, AfterInteractEvent>((EntityEventRefHandler<FoodComponent, AfterInteractEvent>)OnFeedFood, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoodComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<FoodComponent, GetVerbsEvent<AlternativeVerb>>)AddEatVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoodComponent, ConsumeDoAfterEvent>((EntityEventRefHandler<FoodComponent, ConsumeDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, IngestionAttemptEvent>((EntityEventRefHandler<InventoryComponent, IngestionAttemptEvent>)OnInventoryIngestAttempt, (Type[])null, (Type[])null);
	}

	private void OnUseFoodInHand(Entity<FoodComponent> entity, ref UseInHandEvent ev)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)ev).Handled)
		{
			(bool, bool) result = TryFeed(ev.User, ev.User, Entity<FoodComponent>.op_Implicit(entity), entity.Comp);
			((HandledEntityEventArgs)ev).Handled = result.Item2;
		}
	}

	private void OnFeedFood(Entity<FoodComponent> entity, ref AfterInteractEvent args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Target.HasValue && args.CanReach)
		{
			(bool, bool) result = TryFeed(args.User, args.Target.Value, Entity<FoodComponent>.op_Implicit(entity), entity.Comp);
			((HandledEntityEventArgs)args).Handled = result.Item2;
		}
	}

	public (bool Success, bool Handled) TryFeed(EntityUid user, EntityUid target, EntityUid food, FoodComponent foodComp)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		if (food == user || (_mobState.IsAlive(food) && foodComp.RequireDead))
		{
			return (Success: false, Handled: false);
		}
		BodyComponent body = default(BodyComponent);
		if (!((EntitySystem)this).TryComp<BodyComponent>(target, ref body))
		{
			return (Success: false, Handled: false);
		}
		if (((EntitySystem)this).HasComp<UnremoveableComponent>(food))
		{
			return (Success: false, Handled: false);
		}
		if (_openable.IsClosed(food, user, null, predicted: true))
		{
			return (Success: false, Handled: true);
		}
		if (!_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(food), foodComp.Solution, out Entity<SolutionComponent>? _, out Solution foodSolution))
		{
			return (Success: false, Handled: false);
		}
		if (!_body.TryGetBodyOrganEntityComps<StomachComponent>(Entity<BodyComponent>.op_Implicit((target, body)), out List<Entity<StomachComponent, OrganComponent>> stomachs))
		{
			return (Success: false, Handled: false);
		}
		if (!IsDigestibleBy(food, foodComp, stomachs))
		{
			return (Success: false, Handled: false);
		}
		if (!TryGetRequiredUtensils(user, foodComp, out List<EntityUid> _))
		{
			return (Success: false, Handled: false);
		}
		StorageComponent storageState = default(StorageComponent);
		if (((EntitySystem)this).TryComp<StorageComponent>(food, ref storageState) && ((BaseContainer)storageState.Container).ContainedEntities.Any())
		{
			_popup.PopupClient(base.Loc.GetString("food-has-used-storage", (ValueTuple<string, object>)("food", food)), user, user);
			return (Success: false, Handled: true);
		}
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (((EntitySystem)this).TryComp<ItemSlotsComponent>(food, ref itemSlots) && itemSlots.Slots.Any<KeyValuePair<string, ItemSlot>>((KeyValuePair<string, ItemSlot> slot) => slot.Value.HasItem))
		{
			_popup.PopupClient(base.Loc.GetString("food-has-used-storage", (ValueTuple<string, object>)("food", food)), user, user);
			return (Success: false, Handled: true);
		}
		string flavors = _flavorProfile.GetLocalizedFlavorsMessage(food, user, foodSolution);
		if (GetUsesRemaining(food, foodComp) <= 0)
		{
			_popup.PopupClient(base.Loc.GetString("food-system-try-use-food-is-empty", (ValueTuple<string, object>)("entity", food)), user, user);
			DeleteAndSpawnTrash(foodComp, food, user);
			return (Success: false, Handled: true);
		}
		if (IsMouthBlocked(target, user))
		{
			return (Success: false, Handled: true);
		}
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(food), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: true))
		{
			return (Success: false, Handled: true);
		}
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: true))
		{
			return (Success: false, Handled: true);
		}
		MapCoordinates mapCoordinates = _transform.GetMapCoordinates(user, (TransformComponent)null);
		if (!((MapCoordinates)(ref mapCoordinates)).InRange(_transform.GetMapCoordinates(target, (TransformComponent)null), 1.5f))
		{
			string message = base.Loc.GetString("interaction-system-user-interaction-cannot-reach");
			_popup.PopupClient(message, user, user);
			return (Success: false, Handled: true);
		}
		bool forceFeed = user != target;
		if (forceFeed)
		{
			EntityUid userName = Identity.Entity(user, (IEntityManager)(object)base.EntityManager);
			_popup.PopupEntity(base.Loc.GetString("food-system-force-feed", (ValueTuple<string, object>)("user", userName)), user, target);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(21, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
			handler.AppendLiteral(" is forcing ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
			handler.AppendLiteral(" to eat ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(food)), "food", "ToPrettyString(food)");
			handler.AppendLiteral(" ");
			handler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(foodSolution));
			adminLogger.Add(LogType.ForceFeed, LogImpact.Medium, ref handler);
		}
		else
		{
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(12, 3);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
			handler2.AppendLiteral(" is eating ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(food)), "food", "ToPrettyString(food)");
			handler2.AppendLiteral(" ");
			handler2.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(foodSolution));
			adminLogger2.Add(LogType.Ingestion, LogImpact.Low, ref handler2);
		}
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, forceFeed ? foodComp.ForceFeedDelay : foodComp.Delay, new ConsumeDoAfterEvent(foodComp.Solution, flavors), food, target, food)
		{
			BreakOnHandChange = false,
			BreakOnMove = forceFeed,
			BreakOnDamage = true,
			MovementThreshold = 0.3f,
			DistanceThreshold = 1.5f,
			NeedHand = (forceFeed || _hands.IsHolding(Entity<HandsComponent>.op_Implicit(user), food))
		};
		_doAfter.TryStartDoAfter(doAfterArgs);
		return (Success: true, Handled: true);
	}

	private void OnDoAfter(Entity<FoodComponent> entity, ref ConsumeDoAfterEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		BodyComponent body = default(BodyComponent);
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled || ((Component)entity.Comp).Deleted || !args.Target.HasValue || !((EntitySystem)this).TryComp<BodyComponent>(args.Target.Value, ref body) || !_body.TryGetBodyOrganEntityComps<StomachComponent>(Entity<BodyComponent>.op_Implicit((args.Target.Value, body)), out List<Entity<StomachComponent, OrganComponent>> stomachs) || !args.Used.HasValue || !_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(args.Used.Value), args.Solution, out Entity<SolutionComponent>? soln, out Solution solution) || !TryGetRequiredUtensils(args.User, entity.Comp, out List<EntityUid> utensils) || IsMouthBlocked(args.Target.Value) || !_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(args.Target.Value)))
		{
			return;
		}
		EntityUid user = args.User;
		EntityUid? target = args.Target;
		bool forceFeed = !target.HasValue || user != target.GetValueOrDefault();
		((HandledEntityEventArgs)args).Handled = true;
		FixedPoint2 transferAmount = (entity.Comp.TransferAmount.HasValue ? FixedPoint2.Min(entity.Comp.TransferAmount.Value, solution.Volume) : solution.Volume);
		Solution split = _solutionContainer.SplitSolution(soln.Value, transferAmount);
		FixedPoint2 highestAvailable = FixedPoint2.Zero;
		Entity<StomachComponent>? stomachToUse = null;
		foreach (Entity<StomachComponent, OrganComponent> ent in stomachs)
		{
			EntityUid owner = ent.Owner;
			if (_stomach.CanTransferSolution(owner, split, ent.Comp1) && _solutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(owner), "stomach", ref ent.Comp1.Solution, out Solution stomachSol) && !(stomachSol.AvailableVolume <= highestAvailable))
			{
				stomachToUse = Entity<StomachComponent, OrganComponent>.op_Implicit(ent);
				highestAvailable = stomachSol.AvailableVolume;
			}
		}
		if (!stomachToUse.HasValue)
		{
			_solutionContainer.TryAddSolution(soln.Value, split);
			_popup.PopupClient(forceFeed ? base.Loc.GetString("food-system-you-cannot-eat-any-more-other", (ValueTuple<string, object>)("target", args.Target.Value)) : base.Loc.GetString("food-system-you-cannot-eat-any-more"), args.Target.Value, args.User);
			return;
		}
		_reaction.DoEntityReaction(args.Target.Value, solution, ReactionMethod.Ingestion);
		StomachSystem stomach = _stomach;
		EntityUid owner2 = stomachToUse.Value.Owner;
		Entity<StomachComponent>? val = stomachToUse;
		stomach.TryTransferSolution(owner2, split, val.HasValue ? Entity<StomachComponent>.op_Implicit(val.GetValueOrDefault()) : null);
		string flavors = args.FlavorMessage;
		if (forceFeed)
		{
			EntityUid targetName = Identity.Entity(args.Target.Value, (IEntityManager)(object)base.EntityManager);
			EntityUid userName = Identity.Entity(args.User, (IEntityManager)(object)base.EntityManager);
			_popup.PopupEntity(base.Loc.GetString("food-system-force-feed-success", (ValueTuple<string, object>)("user", userName), (ValueTuple<string, object>)("flavors", flavors)), entity.Owner, entity.Owner);
			_popup.PopupClient(base.Loc.GetString("food-system-force-feed-success-user", (ValueTuple<string, object>)("target", targetName)), args.User, args.User);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(16, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity.Owner)), "user", "ToPrettyString(entity.Owner)");
			handler.AppendLiteral(" forced ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "target", "ToPrettyString(args.User)");
			handler.AppendLiteral(" to eat ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity.Owner)), "food", "ToPrettyString(entity.Owner)");
			adminLogger.Add(LogType.ForceFeed, LogImpact.Medium, ref handler);
		}
		else
		{
			_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(entity.Comp.EatMessage), (ValueTuple<string, object>)("food", entity.Owner), (ValueTuple<string, object>)("flavors", flavors)), args.User, args.User);
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(5, 2);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "target", "ToPrettyString(args.User)");
			handler2.AppendLiteral(" ate ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity.Owner)), "food", "ToPrettyString(entity.Owner)");
			adminLogger2.Add(LogType.Ingestion, LogImpact.Low, ref handler2);
		}
		SharedAudioSystem audio = _audio;
		SoundSpecifier useSound = entity.Comp.UseSound;
		EntityUid value = args.Target.Value;
		EntityUid? val2 = args.User;
		AudioParams val3 = ((AudioParams)(ref AudioParams.Default)).WithVolume(-1f);
		audio.PlayPredicted(useSound, value, val2, (AudioParams?)((AudioParams)(ref val3)).WithVariation((float?)0.2f));
		foreach (EntityUid utensil in utensils)
		{
			_utensil.TryBreak(utensil, args.User);
		}
		args.Repeat = !forceFeed;
		StackComponent stack = default(StackComponent);
		if (((EntitySystem)this).TryComp<StackComponent>(Entity<FoodComponent>.op_Implicit(entity), ref stack))
		{
			if (stack.Count > 1)
			{
				_stack.SetCount(entity.Owner, stack.Count - 1);
				_solutionContainer.TryAddSolution(soln.Value, split);
				return;
			}
		}
		else if (GetUsesRemaining(entity.Owner, entity.Comp) > 0)
		{
			return;
		}
		args.Repeat = false;
		DeleteAndSpawnTrash(entity.Comp, entity.Owner, args.User);
	}

	public void DeleteAndSpawnTrash(FoodComponent component, EntityUid food, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		BeforeFullyEatenEvent ev = new BeforeFullyEatenEvent
		{
			User = user
		};
		((EntitySystem)this).RaiseLocalEvent<BeforeFullyEatenEvent>(food, ev, false);
		if (((CancellableEntityEventArgs)ev).Cancelled)
		{
			return;
		}
		DestructionAttemptEvent attemptEv = new DestructionAttemptEvent();
		((EntitySystem)this).RaiseLocalEvent<DestructionAttemptEvent>(food, attemptEv, false);
		if (((CancellableEntityEventArgs)attemptEv).Cancelled)
		{
			return;
		}
		AfterFullyEatenEvent afterEvent = new AfterFullyEatenEvent(user);
		((EntitySystem)this).RaiseLocalEvent<AfterFullyEatenEvent>(food, ref afterEvent, false);
		DestructionEventArgs dev = new DestructionEventArgs();
		((EntitySystem)this).RaiseLocalEvent<DestructionEventArgs>(food, dev, false);
		if (component.Trash.Count == 0)
		{
			((EntitySystem)this).PredictedQueueDel(food);
			return;
		}
		MapCoordinates position = _transform.GetMapCoordinates(food, (TransformComponent)null);
		List<EntProtoId> trash = component.Trash;
		string inHand;
		bool tryPickup = _hands.IsHolding(Entity<HandsComponent>.op_Implicit(user), food, out inHand);
		((EntitySystem)this).PredictedDel(Entity<MetaDataComponent, TransformComponent>.op_Implicit(food));
		foreach (EntProtoId trash2 in trash)
		{
			EntityUid spawnedTrash = base.EntityManager.PredictedSpawn(EntProtoId.op_Implicit(trash2), position, (ComponentRegistry)null, default(Angle));
			if (tryPickup)
			{
				_hands.TryPickupAnyHand(user, spawnedTrash);
			}
		}
	}

	private void AddEatVerb(Entity<FoodComponent> entity, ref GetVerbsEvent<AlternativeVerb> ev)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		BodyComponent body = default(BodyComponent);
		if (!(entity.Owner == ev.User) && ev.CanInteract && ev.CanAccess && ((EntitySystem)this).TryComp<BodyComponent>(ev.User, ref body) && _body.TryGetBodyOrganEntityComps<StomachComponent>(Entity<BodyComponent>.op_Implicit((ev.User, body)), out List<Entity<StomachComponent, OrganComponent>> stomachs) && (!_mobState.IsAlive(Entity<FoodComponent>.op_Implicit(entity)) || !entity.Comp.RequireDead) && IsDigestibleBy(Entity<FoodComponent>.op_Implicit(entity), entity.Comp, stomachs))
		{
			EntityUid user = ev.User;
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					TryFeed(user, user, Entity<FoodComponent>.op_Implicit(entity), entity.Comp);
				},
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/cutlery.svg.192dpi.png")),
				Text = base.Loc.GetString("food-system-verb-eat"),
				Priority = -1
			};
			ev.Verbs.Add(verb);
		}
	}

	public bool IsDigestibleBy(EntityUid uid, EntityUid food, FoodComponent? foodComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FoodComponent>(food, ref foodComp, false))
		{
			return false;
		}
		if (!_body.TryGetBodyOrganEntityComps<StomachComponent>(Entity<BodyComponent>.op_Implicit(uid), out List<Entity<StomachComponent, OrganComponent>> stomachs))
		{
			return false;
		}
		return IsDigestibleBy(food, foodComp, stomachs);
	}

	private bool IsDigestibleBy(EntityUid food, FoodComponent component, List<Entity<StomachComponent, OrganComponent>> stomachs)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		bool digestible = true;
		if (stomachs.Count < component.RequiredStomachs)
		{
			return false;
		}
		foreach (Entity<StomachComponent, OrganComponent> ent in stomachs)
		{
			if (ent.Comp1.SpecialDigestible != null)
			{
				if (_whitelistSystem.IsWhitelistPass(ent.Comp1.SpecialDigestible, food))
				{
					return true;
				}
				if (ent.Comp1.IsSpecialDigestibleExclusive)
				{
					return false;
				}
			}
		}
		if (component.RequiresSpecialDigestion)
		{
			return false;
		}
		return digestible;
	}

	private bool TryGetRequiredUtensils(EntityUid user, FoodComponent component, out List<EntityUid> utensils, HandsComponent? hands = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		utensils = new List<EntityUid>();
		if (component.Utensil == UtensilType.None)
		{
			return true;
		}
		if (!((EntitySystem)this).Resolve<HandsComponent>(user, ref hands, false))
		{
			return true;
		}
		UtensilType usedTypes = UtensilType.None;
		UtensilComponent utensil = default(UtensilComponent);
		foreach (EntityUid item in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((user, hands))))
		{
			if (((EntitySystem)this).TryComp<UtensilComponent>(item, ref utensil) && (utensil.Types & component.Utensil) != UtensilType.None && (usedTypes & utensil.Types) != utensil.Types)
			{
				usedTypes |= utensil.Types;
				utensils.Add(item);
			}
		}
		if (component.UtensilRequired && (usedTypes & component.Utensil) != component.Utensil)
		{
			_popup.PopupClient(base.Loc.GetString("food-you-need-to-hold-utensil", (ValueTuple<string, object>)("utensil", component.Utensil ^ usedTypes)), user, user);
			return false;
		}
		return true;
	}

	private void OnInventoryIngestAttempt(Entity<InventoryComponent> entity, ref IngestionAttemptEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled)
		{
			IngestionBlockerComponent blocker = default(IngestionBlockerComponent);
			EntityUid? headUid;
			if (_inventory.TryGetSlotEntity(entity.Owner, "mask", out var maskUid) && ((EntitySystem)this).TryComp<IngestionBlockerComponent>(maskUid, ref blocker) && blocker.Enabled)
			{
				args.Blocker = maskUid;
				((CancellableEntityEventArgs)args).Cancel();
			}
			else if (_inventory.TryGetSlotEntity(entity.Owner, "head", out headUid) && ((EntitySystem)this).TryComp<IngestionBlockerComponent>(headUid, ref blocker) && blocker.Enabled)
			{
				args.Blocker = headUid;
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	public bool IsMouthBlocked(EntityUid uid, EntityUid? popupUid = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		IngestionAttemptEvent attempt = new IngestionAttemptEvent();
		((EntitySystem)this).RaiseLocalEvent<IngestionAttemptEvent>(uid, attempt, false);
		if (((CancellableEntityEventArgs)attempt).Cancelled && attempt.Blocker.HasValue && popupUid.HasValue)
		{
			_popup.PopupClient(base.Loc.GetString("food-system-remove-mask", (ValueTuple<string, object>)("entity", attempt.Blocker.Value)), uid, popupUid.Value);
		}
		return ((CancellableEntityEventArgs)attempt).Cancelled;
	}

	public int GetUsesRemaining(EntityUid uid, FoodComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FoodComponent>(uid, ref comp, true))
		{
			return 0;
		}
		if (!_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), comp.Solution, out Entity<SolutionComponent>? _, out Solution solution) || solution.Volume == 0)
		{
			return 0;
		}
		if (!comp.TransferAmount.HasValue)
		{
			return 1;
		}
		return Math.Max(1, (int)Math.Ceiling((solution.Volume / comp.TransferAmount.Value).Float()));
	}
}
