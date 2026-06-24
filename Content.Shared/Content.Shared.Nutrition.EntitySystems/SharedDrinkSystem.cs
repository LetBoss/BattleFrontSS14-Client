using System;
using System.Collections.Generic;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Shared.Nutrition.EntitySystems;

public abstract class SharedDrinkSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedBodySystem _body;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private FlavorProfileSystem _flavorProfile;

	[Dependency]
	private FoodSystem _food;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private OpenableSystem _openable;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DrinkComponent, AttemptShakeEvent>((EntityEventRefHandler<DrinkComponent, AttemptShakeEvent>)OnAttemptShake, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DrinkComponent, ExaminedEvent>((EntityEventRefHandler<DrinkComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DrinkComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<DrinkComponent, GetVerbsEvent<AlternativeVerb>>)AddDrinkVerb, (Type[])null, (Type[])null);
	}

	protected void OnAttemptShake(Entity<DrinkComponent> entity, ref AttemptShakeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (IsEmpty(Entity<DrinkComponent>.op_Implicit(entity), entity.Comp))
		{
			args.Cancelled = true;
		}
	}

	protected void OnExamined(Entity<DrinkComponent> entity, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		OpenableComponent openable = default(OpenableComponent);
		((EntitySystem)this).TryComp<OpenableComponent>(Entity<DrinkComponent>.op_Implicit(entity), ref openable);
		if (!_openable.IsClosed(entity.Owner, null, openable, predicted: true) && args.IsInDetailsRange && entity.Comp.Examinable)
		{
			if (IsEmpty(Entity<DrinkComponent>.op_Implicit(entity), entity.Comp))
			{
				args.PushMarkup(base.Loc.GetString("drink-component-on-examine-is-empty"));
				return;
			}
			if (((EntitySystem)this).HasComp<ExaminableSolutionComponent>(Entity<DrinkComponent>.op_Implicit(entity)))
			{
				args.PushText(base.Loc.GetString("drink-component-on-examine-exact-volume", (ValueTuple<string, object>)("amount", DrinkVolume(Entity<DrinkComponent>.op_Implicit(entity), entity.Comp))));
				return;
			}
			int num = (int)_solutionContainer.PercentFull(Entity<DrinkComponent>.op_Implicit(entity));
			string text = ((num > 66) ? ((num != 100) ? "drink-component-on-examine-is-mostly-full" : "drink-component-on-examine-is-full") : ((num <= 33) ? "drink-component-on-examine-is-mostly-empty" : HalfEmptyOrHalfFull(args)));
			string remainingString = text;
			args.PushMarkup(base.Loc.GetString(remainingString));
		}
	}

	private void AddDrinkVerb(Entity<DrinkComponent> entity, ref GetVerbsEvent<AlternativeVerb> ev)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		BodyComponent body = default(BodyComponent);
		if (!(entity.Owner == ev.User) && ev.CanInteract && ev.CanAccess && ((EntitySystem)this).TryComp<BodyComponent>(ev.User, ref body) && _body.TryGetBodyOrganEntityComps<StomachComponent>(Entity<BodyComponent>.op_Implicit((ev.User, body)), out List<Entity<StomachComponent, OrganComponent>> _) && _solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.Solution, out Entity<SolutionComponent>? _) && !_mobState.IsAlive(Entity<DrinkComponent>.op_Implicit(entity)))
		{
			EntityUid user = ev.User;
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0023: Unknown result type (might be due to invalid IL or missing references)
					TryDrink(user, user, entity.Comp, Entity<DrinkComponent>.op_Implicit(entity));
				},
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/drink.svg.192dpi.png")),
				Text = base.Loc.GetString("drink-system-verb-drink"),
				Priority = 2
			};
			ev.Verbs.Add(verb);
		}
	}

	protected FixedPoint2 DrinkVolume(EntityUid uid, DrinkComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DrinkComponent>(uid, ref component, true))
		{
			return FixedPoint2.Zero;
		}
		if (!_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), component.Solution, out Entity<SolutionComponent>? _, out Solution sol))
		{
			return FixedPoint2.Zero;
		}
		return sol.Volume;
	}

	protected bool IsEmpty(EntityUid uid, DrinkComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DrinkComponent>(uid, ref component, true))
		{
			return true;
		}
		return DrinkVolume(uid, component) <= 0;
	}

	private string HalfEmptyOrHalfFull(ExaminedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		string remainingString = "drink-component-on-examine-is-half-full";
		MetaDataComponent examiner = default(MetaDataComponent);
		if (((EntitySystem)this).TryComp(args.Examiner, ref examiner) && examiner.EntityName.Length > 0 && string.Compare(examiner.EntityName.Substring(0, 1), "m", StringComparison.InvariantCultureIgnoreCase) > 0)
		{
			remainingString = "drink-component-on-examine-is-half-empty";
		}
		return remainingString;
	}

	protected bool TryDrink(EntityUid user, EntityUid target, DrinkComponent drink, EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<BodyComponent>(target))
		{
			return false;
		}
		if (!_body.TryGetBodyOrganEntityComps<StomachComponent>(Entity<BodyComponent>.op_Implicit(target), out List<Entity<StomachComponent, OrganComponent>> _))
		{
			return false;
		}
		if (_openable.IsClosed(item, user, null, predicted: true))
		{
			return true;
		}
		if (!_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(item), drink.Solution, out Entity<SolutionComponent>? _, out Solution drinkSolution) || drinkSolution.Volume <= 0)
		{
			if (drink.IgnoreEmpty)
			{
				return false;
			}
			_popup.PopupClient(base.Loc.GetString("drink-component-try-use-drink-is-empty", (ValueTuple<string, object>)("entity", item)), item, user);
			return true;
		}
		if (_food.IsMouthBlocked(target, user))
		{
			return true;
		}
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(item), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: true))
		{
			return true;
		}
		bool forceDrink = user != target;
		if (forceDrink)
		{
			EntityUid userName = Identity.Entity(user, (IEntityManager)(object)base.EntityManager);
			_popup.PopupEntity(base.Loc.GetString("drink-component-force-feed", (ValueTuple<string, object>)("user", userName)), user, target);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(23, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
			handler.AppendLiteral(" is forcing ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
			handler.AppendLiteral(" to drink ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "drink", "ToPrettyString(item)");
			handler.AppendLiteral(" ");
			handler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(drinkSolution));
			adminLogger.Add(LogType.ForceFeed, LogImpact.High, ref handler);
		}
		else
		{
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(14, 3);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
			handler2.AppendLiteral(" is drinking ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "drink", "ToPrettyString(item)");
			handler2.AppendLiteral(" ");
			handler2.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(drinkSolution));
			adminLogger2.Add(LogType.Ingestion, LogImpact.Low, ref handler2);
		}
		string flavors = _flavorProfile.GetLocalizedFlavorsMessage(user, drinkSolution);
		DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, forceDrink ? drink.ForceFeedDelay : drink.Delay, new ConsumeDoAfterEvent(drink.Solution, flavors), item, target, item)
		{
			BreakOnHandChange = false,
			BreakOnMove = forceDrink,
			BreakOnDamage = true,
			MovementThreshold = 0.01f,
			DistanceThreshold = 1f,
			NeedHand = (forceDrink || _hands.IsHolding(Entity<HandsComponent>.op_Implicit(user), item))
		};
		_doAfter.TryStartDoAfter(doAfterEventArgs);
		return true;
	}
}
