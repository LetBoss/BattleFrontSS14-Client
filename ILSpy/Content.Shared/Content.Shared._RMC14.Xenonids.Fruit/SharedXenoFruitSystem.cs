using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.ResinHole;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Content.Shared._RMC14.Xenonids.Fruit.Events;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Xenonids.Fruit;

public sealed class SharedXenoFruitSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IMapManager _map;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private ISharedAdminLogManager _adminLogs;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MobThresholdSystem _mobThreshold;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private TagSystem _tags;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private XenoShieldSystem _xenoShield;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedXenoPheromonesSystem _xenoPhero;

	[Dependency]
	private SharedXenoWeedsSystem _xenoWeeds;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private SharedAuraSystem _aura;

	[Dependency]
	private IComponentFactory _componentFactory;

	private static readonly ProtoId<DamageTypePrototype> FruitPlantDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	private static readonly ProtoId<TagPrototype> AirlockTag = ProtoId<TagPrototype>.op_Implicit("Airlock");

	private static readonly ProtoId<TagPrototype> StructureTag = ProtoId<TagPrototype>.op_Implicit("Structure");

	private EntityQuery<MobStateComponent> _mobStateQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_mobStateQuery = ((EntitySystem)this).GetEntityQuery<MobStateComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitPlanterComponent, XenoFruitChooseActionEvent>((EntityEventRefHandler<XenoFruitPlanterComponent, XenoFruitChooseActionEvent>)OnXenoFruitChooseAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitChooseActionComponent, XenoFruitChosenEvent>((EntityEventRefHandler<XenoFruitChooseActionComponent, XenoFruitChosenEvent>)OnActionFruitChosen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, ExaminedEvent>((ComponentEventHandler<XenoFruitComponent, ExaminedEvent>)OnXenoFruitExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, ActivateInWorldEvent>((EntityEventRefHandler<XenoFruitComponent, ActivateInWorldEvent>)OnXenoFruitActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, AfterInteractEvent>((EntityEventRefHandler<XenoFruitComponent, AfterInteractEvent>)OnXenoFruitAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, GetVerbsEvent<ActivationVerb>>((ComponentEventHandler<XenoFruitComponent, GetVerbsEvent<ActivationVerb>>)OnXenoFruitGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<XenoFruitComponent, GetVerbsEvent<AlternativeVerb>>)OnXenoFruitGetAltVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitPlanterComponent, XenoFruitPlantActionEvent>((EntityEventRefHandler<XenoFruitPlanterComponent, XenoFruitPlantActionEvent>)OnXenoFruitPlantAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, XenoFruitHarvestDoAfterEvent>((EntityEventRefHandler<XenoFruitComponent, XenoFruitHarvestDoAfterEvent>)OnXenoFruitHarvestDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, XenoFruitConsumeDoAfterEvent>((EntityEventRefHandler<XenoFruitComponent, XenoFruitConsumeDoAfterEvent>)OnXenoFruitConsumeDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitHealComponent, ExaminedEvent>((ComponentEventHandler<XenoFruitHealComponent, ExaminedEvent>)OnXenoHealFruitExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitRegenComponent, ExaminedEvent>((ComponentEventHandler<XenoFruitRegenComponent, ExaminedEvent>)OnXenoRegenFruitExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitShieldComponent, ExaminedEvent>((ComponentEventHandler<XenoFruitShieldComponent, ExaminedEvent>)OnXenoShieldFruitExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitHasteComponent, ExaminedEvent>((ComponentEventHandler<XenoFruitHasteComponent, ExaminedEvent>)OnXenoHasteFruitExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitSpeedComponent, ExaminedEvent>((ComponentEventHandler<XenoFruitSpeedComponent, ExaminedEvent>)OnXenoSpeedFruitExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitPlasmaComponent, ExaminedEvent>((ComponentEventHandler<XenoFruitPlasmaComponent, ExaminedEvent>)OnXenoPlasmaFruitExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GardenerShieldComponent, RemovedShieldEvent>((EntityEventRefHandler<GardenerShieldComponent, RemovedShieldEvent>)OnShieldRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitEffectRegenComponent, XenoFruitEffectRegenEvent>((EntityEventRefHandler<XenoFruitEffectRegenComponent, XenoFruitEffectRegenEvent>)OnXenoFruitEffectRegen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitEffectPlasmaComponent, XenoFruitEffectPlasmaEvent>((EntityEventRefHandler<XenoFruitEffectPlasmaComponent, XenoFruitEffectPlasmaEvent>)OnXenoFruitEffectPlasma, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitEffectSpeedComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoFruitEffectSpeedComponent, RefreshMovementSpeedModifiersEvent>)OnXenoFruitSpeedRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitEffectSpeedComponent, ComponentShutdown>((EntityEventRefHandler<XenoFruitEffectSpeedComponent, ComponentShutdown>)OnXenoFruitEffectSpeedShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitEffectHasteComponent, MeleeHitEvent>((EntityEventRefHandler<XenoFruitEffectHasteComponent, MeleeHitEvent>)OnXenoFruitEffectHasteHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitEffectHasteComponent, ComponentShutdown>((EntityEventRefHandler<XenoFruitEffectHasteComponent, ComponentShutdown>)OnXenoFruitEffectHasteShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<XenoFruitComponent, AfterAutoHandleStateEvent>)OnXenoFruitAfterState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, DestructionEventArgs>((EntityEventRefHandler<XenoFruitComponent, DestructionEventArgs>)OnXenoFruitDestruction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, ComponentShutdown>((EntityEventRefHandler<XenoFruitComponent, ComponentShutdown>)OnXenoFruitShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoFruitComponent, EntityTerminatingEvent>)OnXenoFruitTerminating, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<XenoFruitPlanterComponent>(((EntitySystem)this).Subs, (object)XenoFruitChooseUI.Key, (BuiEventSubscriber<XenoFruitPlanterComponent>)delegate(Subscriber<XenoFruitPlanterComponent> subs)
		{
			subs.Event<XenoFruitChooseBuiMsg>((EntityEventRefHandler<XenoFruitPlanterComponent, XenoFruitChooseBuiMsg>)OnXenoFruitChooseBui);
		});
	}

	private void OnXenoFruitChooseAction(Entity<XenoFruitPlanterComponent> xeno, ref XenoFruitChooseActionEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoFruitChooseUI.Key, Entity<XenoFruitPlanterComponent>.op_Implicit(xeno), false);
	}

	private void OnXenoFruitChooseBui(Entity<XenoFruitPlanterComponent> xeno, ref XenoFruitChooseBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.CanPlant.Contains(args.FruitId))
		{
			return;
		}
		xeno.Comp.FruitChoice = args.FruitId;
		((EntitySystem)this).Dirty<XenoFruitPlanterComponent>(xeno, (MetaDataComponent)null);
		XenoFruitChosenEvent evAction = new XenoFruitChosenEvent(args.FruitId);
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoFruitPlanterComponent>.op_Implicit(xeno)))
		{
			action.Deconstruct(ref val, ref actionComponent);
			EntityUid id = val;
			((EntitySystem)this).RaiseLocalEvent<XenoFruitChosenEvent>(id, ref evAction, false);
		}
		EntProtoId<XenoFruitComponent> fruitProto = default(EntProtoId<XenoFruitComponent>);
		fruitProto._002Ector(EntProtoId.op_Implicit(args.FruitId));
		XenoFruitPlanterVisualsChangedEvent evXeno = new XenoFruitPlanterVisualsChangedEvent(fruitProto);
		((EntitySystem)this).RaiseLocalEvent<XenoFruitPlanterVisualsChangedEvent>(Entity<XenoFruitPlanterComponent>.op_Implicit(xeno), ref evXeno, false);
	}

	private void OnActionFruitChosen(Entity<XenoFruitChooseActionComponent> xeno, ref XenoFruitChosenEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		Entity<ActionComponent>? action = _actions.GetAction(Entity<ActionComponent>.op_Implicit(xeno.Owner));
		if (action.HasValue)
		{
			Entity<ActionComponent> action2 = action.GetValueOrDefault();
			EntityPrototype fruit = default(EntityPrototype);
			if (_prototype.TryIndex(args.Choice, ref fruit))
			{
				_actions.SetIcon(action2.AsNullable(), (SpriteSpecifier?)new Rsi(new ResPath("_RMC14/Structures/Xenos/xeno_fruit.rsi"), GetFruitSprite(fruit)));
			}
		}
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-choose", (ValueTuple<string, object>)("fruit", args.Choice)), Entity<XenoFruitChooseActionComponent>.op_Implicit(xeno), Entity<XenoFruitChooseActionComponent>.op_Implicit(xeno));
	}

	private void OnXenoFruitExamined(EntityUid uid, XenoFruitComponent fruit, ExaminedEvent args)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		string state = fruit.State switch
		{
			XenoFruitState.Item => "rmc-xeno-fruit-examine-grown", 
			XenoFruitState.Grown => "rmc-xeno-fruit-examine-grown", 
			XenoFruitState.Growing => "rmc-xeno-fruit-examine-growing", 
			XenoFruitState.Eaten => "rmc-xeno-fruit-examine-spent", 
			_ => "rmc-xeno-fruit-examine-grown", 
		};
		args.PushMarkup(base.Loc.GetString("rmc-xeno-fruit-examine-base", (ValueTuple<string, object>)("growthStatus", base.Loc.GetString(state))));
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-fruit-consume-examine"), -10);
		}
	}

	private void OnXenoHealFruitExamined(EntityUid uid, XenoFruitHealComponent fruit, ExaminedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-fruit-instant-heal", (ValueTuple<string, object>)("amount", fruit.HealAmount)), -12);
		}
	}

	private void OnXenoRegenFruitExamined(EntityUid uid, XenoFruitRegenComponent fruit, ExaminedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-fruit-regen-heal", (ValueTuple<string, object>)("amount", fruit.RegenPerTick), (ValueTuple<string, object>)("time", (fruit.TickCount * fruit.TickPeriod).TotalSeconds)), -12);
		}
	}

	private void OnXenoShieldFruitExamined(EntityUid uid, XenoFruitShieldComponent fruit, ExaminedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-fruit-shield", new(string, object)[4]
			{
				("percent", fruit.ShieldRatio * 100),
				("max", fruit.ShieldAmount),
				("duration", fruit.Duration.TotalSeconds),
				("decay", fruit.ShieldDecay)
			}), -12);
		}
	}

	private void OnXenoHasteFruitExamined(EntityUid uid, XenoFruitHasteComponent fruit, ExaminedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-fruit-cooldown", new(string, object)[3]
			{
				("amount", fruit.ReductionPerSlash * 100),
				("max", fruit.ReductionMax * 100),
				("time", fruit.Duration.TotalSeconds)
			}), -12);
		}
	}

	private void OnXenoSpeedFruitExamined(EntityUid uid, XenoFruitSpeedComponent fruit, ExaminedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-fruit-speed", (ValueTuple<string, object>)("amount", fruit.SpeedModifier), (ValueTuple<string, object>)("time", fruit.Duration.TotalSeconds)), -12);
		}
	}

	private void OnXenoPlasmaFruitExamined(EntityUid uid, XenoFruitPlasmaComponent fruit, ExaminedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-fruit-regen-plasma", (ValueTuple<string, object>)("amount", fruit.RegenPerTick), (ValueTuple<string, object>)("time", (fruit.TickCount * fruit.TickPeriod).TotalSeconds)), -12);
		}
	}

	private void OnXenoFruitActivateInWorld(Entity<XenoFruitComponent> fruit, ref ActivateInWorldEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			EntityUid user = args.User;
			if (fruit.Comp.State != XenoFruitState.Item)
			{
				TryHarvest(fruit, user);
			}
			else
			{
				TryConsume(fruit, user);
			}
		}
	}

	private void OnXenoFruitAfterInteract(Entity<XenoFruitComponent> fruit, ref AfterInteractEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
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
		if (((EntitySystem)this).HasComp<MobStateComponent>(target2))
		{
			if (args.User == target2)
			{
				TryConsume(fruit, args.User);
			}
			else
			{
				TryFeed(fruit, args.User, target2);
			}
		}
	}

	private void OnXenoFruitGetVerbs(EntityUid fruit, XenoFruitComponent comp, GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		if (args.CanAccess && args.CanInteract && comp.State != XenoFruitState.Item)
		{
			ActivationVerb harvestVerb = new ActivationVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					TryHarvest(Entity<XenoFruitComponent>.op_Implicit((fruit, comp)), args.User);
				},
				Text = base.Loc.GetString("rmc-xeno-fruit-verb-harvest"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/pickup.svg.192dpi.png"))
			};
			args.Verbs.Add(harvestVerb);
		}
	}

	private void OnXenoFruitGetAltVerbs(EntityUid fruit, XenoFruitComponent comp, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		if (args.CanAccess && args.CanInteract)
		{
			AlternativeVerb consumeVerb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					TryConsume(Entity<XenoFruitComponent>.op_Implicit((fruit, comp)), args.User);
				},
				Text = base.Loc.GetString("rmc-xeno-fruit-verb-consume"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/cutlery.svg.192dpi.png"))
			};
			args.Verbs.Add(consumeVerb);
		}
	}

	private bool CanPlantOnTilePopup(Entity<XenoFruitPlanterComponent> xeno, EntityCoordinates target, bool checkWeeds, out string popup)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		popup = base.Loc.GetString("rmc-xeno-fruit-plant-failed");
		EntityUid? grid = _transform.GetGrid(target);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				target = target.SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
				if (checkWeeds && !_xenoWeeds.IsOnWeeds(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), target))
				{
					popup = base.Loc.GetString("rmc-xeno-fruit-plant-failed-weeds");
					return false;
				}
				if (_xenoWeeds.IsOnWeeds(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), target, sourceOnly: true))
				{
					popup = base.Loc.GetString("rmc-xeno-fruit-plant-failed-node");
					return false;
				}
				Entity<XenoWeedsComponent>? weed = _xenoWeeds.GetWeedsOnFloor(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), target);
				if (checkWeeds && weed.HasValue && !_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(weed.Value.Owner)))
				{
					popup = base.Loc.GetString("rmc-xeno-fruit-wrong-hive");
					return false;
				}
				Vector2i tile = _mapSystem.CoordinatesToTile(gridId, grid2, target);
				AnchoredEntitiesEnumerator anchored = _mapSystem.GetAnchoredEntitiesEnumerator(gridId, grid2, tile);
				EntityUid? uid = default(EntityUid?);
				while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref uid))
				{
					if (((EntitySystem)this).HasComp<XenoFruitComponent>(uid))
					{
						popup = base.Loc.GetString("rmc-xeno-fruit-plant-failed-fruit");
						return false;
					}
					if (((EntitySystem)this).HasComp<XenoResinHoleComponent>(uid))
					{
						popup = base.Loc.GetString("rmc-xeno-fruit-plant-failed-resin-hole");
						return false;
					}
					if (((EntitySystem)this).HasComp<StrapComponent>(uid) || ((EntitySystem)this).HasComp<XenoEggComponent>(uid) || ((EntitySystem)this).HasComp<XenoConstructComponent>(uid) || _tags.HasAnyTag(uid.Value, StructureTag, AirlockTag))
					{
						popup = base.Loc.GetString("rmc-xeno-fruit-plant-failed");
						return false;
					}
				}
				if (_turf.IsTileBlocked(gridId, tile, CollisionGroup.FlyingMobMask | CollisionGroup.MidImpassable, grid2))
				{
					popup = base.Loc.GetString("rmc-xeno-fruit-plant-failed");
					return false;
				}
				return true;
			}
		}
		popup = base.Loc.GetString("rmc-xeno-fruit-plant-failed");
		return false;
	}

	private void OnXenoFruitPlantAction(Entity<XenoFruitPlanterComponent> xeno, ref XenoFruitPlantActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntProtoId? fruitChoice = xeno.Comp.FruitChoice;
		if (!fruitChoice.HasValue)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-plant-failed-select"), xeno.Owner, xeno.Owner, PopupType.SmallCaution);
			return;
		}
		EntityCoordinates coordinates = _transform.GetMoverCoordinates(Entity<XenoFruitPlanterComponent>.op_Implicit(xeno)).SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
		if (!((EntityCoordinates)(ref coordinates)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			return;
		}
		if (!CanPlantOnTilePopup(xeno, coordinates, args.CheckWeeds, out string popup))
		{
			_popup.PopupClient(popup, coordinates, xeno.Owner, PopupType.SmallCaution);
		}
		else
		{
			if (!_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), args.PlasmaCost))
			{
				return;
			}
			DamageSpecifier fruitDamage = new DamageSpecifier
			{
				DamageDict = { [ProtoId<DamageTypePrototype>.op_Implicit(FruitPlantDamageType)] = args.HealthCost }
			};
			DamageableComponent damage = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<DamageableComponent>(Entity<XenoFruitPlanterComponent>.op_Implicit(xeno), ref damage))
			{
				_damageable.AddDamage(xeno.Owner, damage, fruitDamage);
			}
			((HandledEntityEventArgs)args).Handled = true;
			_audio.PlayPredicted(xeno.Comp.PlantSound, coordinates, (EntityUid?)Entity<XenoFruitPlanterComponent>.op_Implicit(xeno), (AudioParams?)null);
			bool fruitOverflow = false;
			if (_net.IsServer)
			{
				fruitChoice = xeno.Comp.FruitChoice;
				EntityUid entity = ((EntitySystem)this).Spawn(fruitChoice.HasValue ? EntProtoId.op_Implicit(fruitChoice.GetValueOrDefault()) : null, coordinates);
				XenoFruitComponent fruit = ((EntitySystem)this).EnsureComp<XenoFruitComponent>(entity);
				TransformComponent xform = ((EntitySystem)this).Transform(entity);
				_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(entity));
				_transform.SetCoordinates(entity, coordinates);
				_transform.SetLocalRotation(entity, Angle.op_Implicit(0f), (TransformComponent)null);
				SetFruitState(Entity<XenoFruitComponent>.op_Implicit((entity, fruit)), XenoFruitState.Growing);
				_transform.AnchorEntity(entity, xform);
				if (!xeno.Comp.PlantedFruit.Contains(entity))
				{
					xeno.Comp.PlantedFruit.Add(entity);
				}
				EntityUid? weeds = _xenoWeeds.GetWeedsOnFloor(coordinates);
				XenoWeedsComponent weedsComp = default(XenoWeedsComponent);
				if (((EntitySystem)this).TryComp<XenoWeedsComponent>(weeds, ref weedsComp))
				{
					fruit.GrowTime *= (double)weedsComp.FruitGrowthMultiplier;
				}
				fruit.Planter = xeno.Owner;
				if (xeno.Comp.PlantedFruit.Count > xeno.Comp.MaxFruitAllowed)
				{
					fruitOverflow = true;
					EntityUid removedFruit = xeno.Comp.PlantedFruit[0];
					xeno.Comp.PlantedFruit.Remove(removedFruit);
					((EntitySystem)this).QueueDel((EntityUid?)removedFruit);
				}
				ISharedAdminLogManager adminLogs = _adminLogs;
				LogStringHandler handler = new LogStringHandler(18, 3);
				handler.AppendLiteral("Xeno ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(xeno.Owner)), "xeno", "ToPrettyString(xeno.Owner)");
				handler.AppendLiteral(" planted ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity)), "entity", "ToPrettyString(entity)");
				handler.AppendLiteral(" at ");
				handler.AppendFormatted<EntityCoordinates>(coordinates, "coordinates");
				adminLogs.Add(LogType.RMCXenoFruitPlant, ref handler);
			}
			string popupSelf = (fruitOverflow ? base.Loc.GetString("rmc-xeno-fruit-plant-limit-exceeded") : base.Loc.GetString("rmc-xeno-fruit-plant-success-self"));
			string popupOthers = base.Loc.GetString("rmc-xeno-fruit-plant-success-others", (ValueTuple<string, object>)("xeno", xeno));
			_popup.PopupPredicted(popupSelf, popupOthers, xeno.Owner, xeno.Owner);
			UpdateFruitCount(xeno);
		}
	}

	public void GardenerFruitActionMessage(Entity<XenoFruitComponent> fruit, LocId message)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (fruit.Comp.Planter.HasValue && !_net.IsClient)
		{
			_popup.PopupEntity(base.Loc.GetString(LocId.op_Implicit(message)), fruit.Comp.Planter.Value, fruit.Comp.Planter.Value, PopupType.SmallCaution);
		}
	}

	private bool TryHarvest(Entity<XenoFruitComponent> fruit, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		if (fruit.Comp.State == XenoFruitState.Item)
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<HandsComponent>(user))
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<MarineComponent>(user) && !_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(fruit.Owner), Entity<HiveMemberComponent>.op_Implicit(user)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-wrong-hive"), user, user, PopupType.SmallCaution);
			return false;
		}
		if (((EntitySystem)this).HasComp<XenoComponent>(user) && !((EntitySystem)this).HasComp<XenoFruitPlanterComponent>(user) && fruit.Comp.State == XenoFruitState.Growing)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-pick-failed-not-mature", (ValueTuple<string, object>)("fruit", fruit)), user, user, PopupType.SmallCaution);
			return false;
		}
		XenoFruitPlanterComponent planter = default(XenoFruitPlanterComponent);
		float pickMult = (((EntitySystem)this).TryComp<XenoFruitPlanterComponent>(user, ref planter) ? planter.FruitPickingMultiplier : 1f);
		XenoFruitHarvestDoAfterEvent ev = new XenoFruitHarvestDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, fruit.Comp.HarvestDelay * pickMult, ev, Entity<XenoFruitComponent>.op_Implicit(fruit), fruit.Owner)
		{
			NeedHand = true,
			BreakOnMove = true,
			RequireCanInteract = true,
			BlockDuplicate = true,
			DuplicateCondition = DuplicateConditions.SameEvent
		};
		if (!_doAfter.TryStartDoAfter(doAfter))
		{
			if (((EntitySystem)this).HasComp<XenoComponent>(user))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-harvest-failed-xeno"), user, user, PopupType.SmallCaution);
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-harvest-failed-marine"), user, user, PopupType.SmallCaution);
			}
			return false;
		}
		if (((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-harvest-start-xeno", (ValueTuple<string, object>)("fruit", fruit)), user, user);
		}
		else
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-harvest-start-marine", (ValueTuple<string, object>)("fruit", fruit)), user, user);
		}
		return true;
	}

	private void OnXenoFruitHarvestDoAfter(Entity<XenoFruitComponent> fruit, ref XenoFruitHarvestDoAfterEvent args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_audio.PlayPredicted(fruit.Comp.HarvestSound, Entity<XenoFruitComponent>.op_Implicit(fruit), (EntityUid?)args.User, (AudioParams?)null);
		if (fruit.Comp.State == XenoFruitState.Growing)
		{
			if (((EntitySystem)this).HasComp<XenoComponent>(args.User))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-harvest-failed-not-mature-xeno", (ValueTuple<string, object>)("fruit", fruit)), args.User, args.User, PopupType.MediumCaution);
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-harvest-failed-not-mature-marine", (ValueTuple<string, object>)("fruit", fruit)), args.User, args.User, PopupType.MediumCaution);
			}
			if (!_net.IsClient && !((EntitySystem)this).TerminatingOrDeleted(Entity<XenoFruitComponent>.op_Implicit(fruit), (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(Entity<XenoFruitComponent>.op_Implicit(fruit)))
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<XenoFruitComponent>.op_Implicit(fruit));
			}
			return;
		}
		if (((EntitySystem)this).HasComp<XenoComponent>(args.User))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-harvest-success-xeno", (ValueTuple<string, object>)("fruit", fruit)), args.User, args.User);
		}
		else
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-harvest-success-marine", (ValueTuple<string, object>)("fruit", fruit)), args.User, args.User);
		}
		TransformComponent xform = ((EntitySystem)this).Transform(Entity<XenoFruitComponent>.op_Implicit(fruit));
		_transform.Unanchor(Entity<XenoFruitComponent>.op_Implicit(fruit), xform, true);
		SetFruitState(fruit, XenoFruitState.Item);
		_hands.TryPickup(args.User, Entity<XenoFruitComponent>.op_Implicit(fruit));
		((EntitySystem)this).RemCompDeferred<AuraComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit));
		EntityUid user = args.User;
		EntityUid? planter = fruit.Comp.Planter;
		if (!planter.HasValue || user != planter.GetValueOrDefault())
		{
			GardenerFruitActionMessage(fruit, LocId.op_Implicit("rmc-xeno-fruit-picked"));
		}
	}

	private bool TryConsume(Entity<XenoFruitComponent> fruit, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(user) || fruit.Comp.State == XenoFruitState.Eaten)
		{
			return false;
		}
		if (fruit.Comp.State == XenoFruitState.Growing)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-pick-failed-not-mature", (ValueTuple<string, object>)("fruit", fruit)), user, user, PopupType.SmallCaution);
			return false;
		}
		if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(fruit.Owner), Entity<HiveMemberComponent>.op_Implicit(user)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-wrong-hive"), user, user, PopupType.SmallCaution);
			return false;
		}
		if (((EntitySystem)this).HasComp<XenoFruitSpeedComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit)) && ((EntitySystem)this).HasComp<XenoFruitEffectSpeedComponent>(user))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-effect-already", (ValueTuple<string, object>)("fruit", fruit)), user, user, PopupType.SmallCaution);
			return false;
		}
		DamageableComponent damage = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(user, ref damage))
		{
			return false;
		}
		if (!fruit.Comp.CanConsumeAtFull && damage.TotalDamage == 0)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-pick-failed-health-full"), user, user);
			return false;
		}
		XenoFruitConsumeDoAfterEvent ev = new XenoFruitConsumeDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, fruit.Comp.ConsumeDelay, ev, Entity<XenoFruitComponent>.op_Implicit(fruit), user, Entity<XenoFruitComponent>.op_Implicit(fruit))
		{
			NeedHand = true,
			BreakOnMove = true,
			RequireCanInteract = true
		};
		string popupSelf = base.Loc.GetString("rmc-xeno-fruit-eat-fail-self", (ValueTuple<string, object>)("fruit", fruit));
		string popupOthers = base.Loc.GetString("rmc-xeno-fruit-eat-fail-others", (ValueTuple<string, object>)("fruit", fruit), (ValueTuple<string, object>)("xeno", user));
		if (!_doAfter.TryStartDoAfter(doAfter))
		{
			_popup.PopupPredicted(popupSelf, popupOthers, user, user);
			return false;
		}
		popupSelf = base.Loc.GetString("rmc-xeno-fruit-eat-start-self", (ValueTuple<string, object>)("fruit", fruit));
		popupOthers = base.Loc.GetString("rmc-xeno-fruit-eat-start-others", (ValueTuple<string, object>)("fruit", fruit), (ValueTuple<string, object>)("xeno", user));
		_popup.PopupPredicted(popupSelf, popupOthers, user, user);
		return true;
	}

	private bool TryFeed(Entity<XenoFruitComponent> fruit, EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(user) || fruit.Comp.State == XenoFruitState.Eaten)
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<XenoComponent>(target))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-feed-refuse", (ValueTuple<string, object>)("target", target), (ValueTuple<string, object>)("fruit", fruit)), user, user, PopupType.SmallCaution);
			return false;
		}
		if (_mobState.IsDead(target))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-feed-dead", (ValueTuple<string, object>)("target", target), (ValueTuple<string, object>)("fruit", fruit)), user, user);
			return false;
		}
		if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(fruit.Owner), Entity<HiveMemberComponent>.op_Implicit(user)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-wrong-hive"), user, user, PopupType.SmallCaution);
			return false;
		}
		if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(user), Entity<HiveMemberComponent>.op_Implicit(target)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-feed-wrong-hive", (ValueTuple<string, object>)("target", target)), user, user, PopupType.SmallCaution);
			return false;
		}
		if (((EntitySystem)this).HasComp<XenoFruitSpeedComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit)) && ((EntitySystem)this).HasComp<XenoFruitEffectSpeedComponent>(target))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-effect-already-feed", (ValueTuple<string, object>)("xeno", target), (ValueTuple<string, object>)("fruit", fruit)), user, user, PopupType.SmallCaution);
			return false;
		}
		DamageableComponent damage = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(target, ref damage))
		{
			return false;
		}
		if (!fruit.Comp.CanConsumeAtFull && damage.TotalDamage == 0)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-pick-failed-health-full-target"), user, user);
			return false;
		}
		XenoFruitPlanterComponent planter = default(XenoFruitPlanterComponent);
		float fruitFeedSpeed = (((EntitySystem)this).TryComp<XenoFruitPlanterComponent>(user, ref planter) ? planter.FruitFeedingMultiplier : 1f);
		XenoFruitConsumeDoAfterEvent ev = new XenoFruitConsumeDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, fruit.Comp.ConsumeDelay * fruitFeedSpeed, ev, Entity<XenoFruitComponent>.op_Implicit(fruit), target, Entity<XenoFruitComponent>.op_Implicit(fruit))
		{
			NeedHand = true,
			BreakOnMove = true,
			BreakOnHandChange = true,
			RequireCanInteract = true,
			TargetEffect = EntProtoId.op_Implicit("RMCEffectHealBusy")
		};
		string popupSelf = base.Loc.GetString("rmc-xeno-fruit-feed-fail-self", (ValueTuple<string, object>)("target", target), (ValueTuple<string, object>)("fruit", fruit));
		string popupTarget = base.Loc.GetString("rmc-xeno-fruit-feed-fail-target", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("fruit", fruit));
		string popupOthers = base.Loc.GetString("rmc-xeno-fruit-feed-fail-others", new(string, object)[3]
		{
			("user", user),
			("target", target),
			("fruit", fruit)
		});
		if (!_doAfter.TryStartDoAfter(doAfter))
		{
			_popup.PopupClient(popupTarget, target, target, PopupType.MediumCaution);
			_popup.PopupPredicted(popupSelf, popupOthers, user, user);
			return false;
		}
		popupSelf = base.Loc.GetString("rmc-xeno-fruit-feed-start-self", (ValueTuple<string, object>)("target", target), (ValueTuple<string, object>)("fruit", fruit));
		popupTarget = base.Loc.GetString("rmc-xeno-fruit-feed-start-target", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("fruit", fruit));
		popupOthers = base.Loc.GetString("rmc-xeno-fruit-feed-start-others", new(string, object)[3]
		{
			("user", user),
			("target", target),
			("fruit", fruit)
		});
		_popup.PopupClient(popupTarget, target, target, PopupType.MediumCaution);
		_popup.PopupPredicted(popupSelf, popupOthers, user, user);
		return true;
	}

	private void OnXenoFruitConsumeDoAfter(Entity<XenoFruitComponent> fruit, ref XenoFruitConsumeDoAfterEvent args)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		EntityUid user = args.User;
		if (((EntitySystem)this).TerminatingOrDeleted(Entity<XenoFruitComponent>.op_Implicit(fruit), (MetaDataComponent)null) || base.EntityManager.IsQueuedForDeletion(Entity<XenoFruitComponent>.op_Implicit(fruit)) || fruit.Comp.State == XenoFruitState.Eaten)
		{
			if (user == target2)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-pick-failed-no-longer", (ValueTuple<string, object>)("fruit", fruit)), user, user, PopupType.SmallCaution);
			}
			return;
		}
		ApplyFruitEffects(fruit, target2);
		_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(fruit.Comp.Popup)), target2, target2, PopupType.Medium);
		EntityUid user2 = args.User;
		target = fruit.Comp.Planter;
		if (!target.HasValue || user2 != target.GetValueOrDefault())
		{
			target = args.Target;
			EntityUid? planter = fruit.Comp.Planter;
			if (target.HasValue != planter.HasValue || (target.HasValue && target.GetValueOrDefault() != planter.GetValueOrDefault()))
			{
				GardenerFruitActionMessage(fruit, LocId.op_Implicit("rmc-xeno-fruit-consumed"));
			}
		}
		SetFruitState(fruit, XenoFruitState.Eaten);
		((EntitySystem)this).RemCompDeferred<AuraComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit));
		if (_net.IsServer)
		{
			((EntitySystem)this).EnsureComp<TimedDespawnComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit)).Lifetime = fruit.Comp.SpentDespawnTime;
		}
	}

	private void ApplyFruitEffects(Entity<XenoFruitComponent> fruit, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitHealComponent healComp = default(XenoFruitHealComponent);
		if (((EntitySystem)this).TryComp<XenoFruitHealComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit), ref healComp))
		{
			_xeno.HealDamage(Entity<DamageableComponent>.op_Implicit((ValueTuple<EntityUid, DamageableComponent>)(target, null)), healComp.HealAmount);
		}
		XenoFruitRegenComponent regenComp = default(XenoFruitRegenComponent);
		if (((EntitySystem)this).TryComp<XenoFruitRegenComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit), ref regenComp))
		{
			ApplyFruitRegen(Entity<XenoFruitRegenComponent>.op_Implicit((fruit.Owner, regenComp)), target);
		}
		XenoFruitPlasmaComponent plasmaComp = default(XenoFruitPlasmaComponent);
		if (((EntitySystem)this).TryComp<XenoFruitPlasmaComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit), ref plasmaComp))
		{
			ApplyFruitPlasma(Entity<XenoFruitPlasmaComponent>.op_Implicit((fruit.Owner, plasmaComp)), target);
		}
		XenoFruitShieldComponent shieldComp = default(XenoFruitShieldComponent);
		if (((EntitySystem)this).TryComp<XenoFruitShieldComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit), ref shieldComp))
		{
			ApplyFruitShield(Entity<XenoFruitShieldComponent>.op_Implicit((fruit.Owner, shieldComp)), target);
		}
		XenoFruitSpeedComponent speedComp = default(XenoFruitSpeedComponent);
		if (((EntitySystem)this).TryComp<XenoFruitSpeedComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit), ref speedComp))
		{
			ApplyFruitSpeed(Entity<XenoFruitSpeedComponent>.op_Implicit((fruit.Owner, speedComp)), target);
		}
		XenoFruitHasteComponent hasteComp = default(XenoFruitHasteComponent);
		if (((EntitySystem)this).TryComp<XenoFruitHasteComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit), ref hasteComp))
		{
			ApplyFruitHaste(Entity<XenoFruitHasteComponent>.op_Implicit((fruit.Owner, hasteComp)), target);
		}
	}

	private void ApplyFruitRegen(Entity<XenoFruitRegenComponent> fruit, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitEffectRegenComponent xenoFruitEffectRegenComponent = ((EntitySystem)this).EnsureComp<XenoFruitEffectRegenComponent>(target);
		xenoFruitEffectRegenComponent.TickPeriod = fruit.Comp.TickPeriod;
		xenoFruitEffectRegenComponent.TicksLeft = fruit.Comp.TickCount;
		xenoFruitEffectRegenComponent.RegenPerTick = fruit.Comp.RegenPerTick;
	}

	private void OnXenoFruitEffectRegen(Entity<XenoFruitEffectRegenComponent> xeno, ref XenoFruitEffectRegenEvent ev)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_xeno.HealDamage(Entity<DamageableComponent>.op_Implicit((ValueTuple<EntityUid, DamageableComponent>)(xeno.Owner, null)), xeno.Comp.RegenPerTick);
	}

	private void ApplyFruitPlasma(Entity<XenoFruitPlasmaComponent> fruit, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitEffectPlasmaComponent xenoFruitEffectPlasmaComponent = ((EntitySystem)this).EnsureComp<XenoFruitEffectPlasmaComponent>(target);
		xenoFruitEffectPlasmaComponent.TickPeriod = fruit.Comp.TickPeriod;
		xenoFruitEffectPlasmaComponent.TicksLeft = fruit.Comp.TickCount;
		xenoFruitEffectPlasmaComponent.RegenPerTick = fruit.Comp.RegenPerTick;
	}

	private void OnXenoFruitEffectPlasma(Entity<XenoFruitEffectPlasmaComponent> xeno, ref XenoFruitEffectPlasmaEvent ev)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_xenoPlasma.RegenPlasma(Entity<XenoPlasmaComponent>.op_Implicit((ValueTuple<EntityUid, XenoPlasmaComponent>)(xeno.Owner, null)), xeno.Comp.RegenPerTick);
	}

	private void ApplyFruitSpeed(Entity<XenoFruitSpeedComponent> fruit, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitEffectSpeedComponent xenoFruitEffectSpeedComponent = ((EntitySystem)this).EnsureComp<XenoFruitEffectSpeedComponent>(target);
		xenoFruitEffectSpeedComponent.Duration = fruit.Comp.Duration;
		xenoFruitEffectSpeedComponent.SpeedModifier = fruit.Comp.SpeedModifier;
		_movementSpeed.RefreshMovementSpeedModifiers(target);
	}

	private void OnXenoFruitEffectSpeedShutdown(Entity<XenoFruitEffectSpeedComponent> xeno, ref ComponentShutdown ev)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-effect-end"), xeno.Owner, xeno.Owner, PopupType.MediumCaution);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoFruitEffectSpeedComponent>.op_Implicit(xeno));
	}

	private void OnXenoFruitSpeedRefresh(Entity<XenoFruitEffectSpeedComponent> xeno, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitEffectSpeedComponent comp = xeno.Comp;
		MovementSpeedModifierComponent baseSpeed = default(MovementSpeedModifierComponent);
		if (((EntitySystem)this).TryComp<MovementSpeedModifierComponent>(Entity<XenoFruitEffectSpeedComponent>.op_Implicit(xeno), ref baseSpeed))
		{
			float modSpeedWalk = baseSpeed.BaseWalkSpeed + comp.SpeedModifier.Float();
			float modSpeedSprint = baseSpeed.BaseSprintSpeed + comp.SpeedModifier.Float();
			args.ModifySpeed(modSpeedWalk / baseSpeed.BaseWalkSpeed, modSpeedSprint / baseSpeed.BaseSprintSpeed);
		}
	}

	private void ApplyFruitShield(Entity<XenoFruitShieldComponent> fruit, EntityUid target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitShieldComponent comp = fruit.Comp;
		FixedPoint2 maxShield = _mobThreshold.GetThresholdForState(target, MobState.Dead) * comp.ShieldRatio;
		FixedPoint2 shieldAmount = ((maxShield < comp.ShieldAmount) ? maxShield : comp.ShieldAmount);
		_xenoShield.ApplyShield(target, XenoShieldSystem.ShieldType.Gardener, shieldAmount, comp.Duration, comp.ShieldDecay.Double(), addShield: true, shieldAmount.Double());
		((EntitySystem)this).EnsureComp<GardenerShieldComponent>(target);
	}

	public void OnShieldRemove(Entity<GardenerShieldComponent> ent, ref RemovedShieldEvent args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (args.Type == XenoShieldSystem.ShieldType.Gardener)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-defensive-shield-end"), Entity<GardenerShieldComponent>.op_Implicit(ent), Entity<GardenerShieldComponent>.op_Implicit(ent), PopupType.MediumCaution);
		}
		((EntitySystem)this).RemCompDeferred<GardenerShieldComponent>(Entity<GardenerShieldComponent>.op_Implicit(ent));
	}

	public void ApplyFruitHaste(Entity<XenoFruitHasteComponent> fruit, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitEffectHasteComponent comp = default(XenoFruitEffectHasteComponent);
		bool result = ((EntitySystem)this).EnsureComp<XenoFruitEffectHasteComponent>(target, ref comp);
		comp.Duration = fruit.Comp.Duration;
		comp.ReductionMax = fruit.Comp.ReductionMax;
		comp.ReductionPerSlash = fruit.Comp.ReductionPerSlash;
		comp.ReductionCurrent = (result ? comp.ReductionCurrent : ((FixedPoint2)0));
		comp.EndAt = null;
	}

	private void RefreshUseDelays(EntityUid user, FixedPoint2 amount)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		ActionReducedUseDelayComponent comp = default(ActionReducedUseDelayComponent);
		foreach (Entity<ActionComponent> action in _actions.GetActions(user))
		{
			action.Deconstruct(ref val, ref actionComponent);
			EntityUid actionId = val;
			if (((EntitySystem)this).TryComp<ActionReducedUseDelayComponent>(actionId, ref comp))
			{
				ActionReducedUseDelayEvent ev = new ActionReducedUseDelayEvent(amount);
				((EntitySystem)this).RaiseLocalEvent<ActionReducedUseDelayEvent>(actionId, ev, false);
				((EntitySystem)this).Dirty(actionId, (IComponent)(object)comp, (MetaDataComponent)null);
			}
		}
	}

	private void OnXenoFruitEffectHasteHit(Entity<XenoFruitEffectHasteComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !args.IsHit || args.HitEntities.Count == 0)
		{
			return;
		}
		bool found = false;
		MobStateComponent mobState = default(MobStateComponent);
		foreach (EntityUid entity in args.HitEntities)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoFruitEffectHasteComponent>.op_Implicit(xeno), entity) && _mobStateQuery.TryComp(entity, ref mobState) && mobState.CurrentState != MobState.Dead)
			{
				found = true;
				break;
			}
		}
		if (found && !(xeno.Comp.ReductionCurrent >= xeno.Comp.ReductionMax))
		{
			xeno.Comp.ReductionCurrent += xeno.Comp.ReductionPerSlash;
			RefreshUseDelays(xeno.Owner, xeno.Comp.ReductionCurrent);
		}
	}

	private void OnXenoFruitEffectHasteShutdown(Entity<XenoFruitEffectHasteComponent> xeno, ref ComponentShutdown ev)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-fruit-effect-end"), xeno.Owner, xeno.Owner, PopupType.MediumCaution);
		RefreshUseDelays(xeno.Owner, 0);
	}

	public bool TrySpeedupGrowth(Entity<XenoFruitComponent> fruit, TimeSpan amount)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitComponent comp = fruit.Comp;
		if (!comp.GrowAt.HasValue || comp.State != XenoFruitState.Growing)
		{
			return false;
		}
		comp.GrowAt -= amount;
		return true;
	}

	private void XenoFruitRemoved(Entity<XenoFruitComponent> fruit)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityUid? planter = fruit.Comp.Planter;
		if (planter.HasValue)
		{
			EntityUid planter2 = planter.GetValueOrDefault();
			XenoFruitPlanterComponent planterComp = default(XenoFruitPlanterComponent);
			if (((EntitySystem)this).TryComp<XenoFruitPlanterComponent>(planter2, ref planterComp) && planterComp.PlantedFruit.Contains(fruit.Owner))
			{
				planterComp.PlantedFruit.Remove(fruit.Owner);
				UpdateFruitCount(Entity<XenoFruitPlanterComponent>.op_Implicit((planter2, planterComp)));
			}
		}
	}

	private void OnXenoFruitDestruction(Entity<XenoFruitComponent> fruit, ref DestructionEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		GardenerFruitActionMessage(fruit, LocId.op_Implicit("rmc-xeno-fruit-destroyed"));
		XenoFruitRemoved(fruit);
	}

	private void OnXenoFruitShutdown(Entity<XenoFruitComponent> fruit, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitRemoved(fruit);
	}

	private void OnXenoFruitTerminating(Entity<XenoFruitComponent> fruit, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitRemoved(fruit);
	}

	private void OnXenoFruitAfterState(Entity<XenoFruitComponent> fruit, ref AfterAutoHandleStateEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitStateChangedEvent ev = default(XenoFruitStateChangedEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoFruitStateChangedEvent>(Entity<XenoFruitComponent>.op_Implicit(fruit), ref ev, false);
	}

	private void SetFruitState(Entity<XenoFruitComponent> fruit, XenoFruitState state)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		fruit.Comp.State = state;
		((EntitySystem)this).Dirty<XenoFruitComponent>(fruit, (MetaDataComponent)null);
		if (((EntitySystem)this).HasComp<XenoPheromonesObjectComponent>(Entity<XenoFruitComponent>.op_Implicit(fruit)))
		{
			switch (state)
			{
			case XenoFruitState.Grown:
				_xenoPhero.TryActivatePheromonesObject(Entity<XenoPheromonesObjectComponent>.op_Implicit(fruit.Owner));
				break;
			case XenoFruitState.Item:
				_xenoPhero.DeactivatePheromones(Entity<XenoPheromonesComponent>.op_Implicit(fruit.Owner));
				break;
			}
		}
		XenoFruitStateChangedEvent ev = default(XenoFruitStateChangedEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoFruitStateChangedEvent>(Entity<XenoFruitComponent>.op_Implicit(fruit), ref ev, false);
	}

	private void UpdateFruitCount(Entity<XenoFruitPlanterComponent> xeno)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitChooseBuiState state = new XenoFruitChooseBuiState(xeno.Comp.PlantedFruit.Count, xeno.Comp.MaxFruitAllowed);
		_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoFruitChooseUI.Key, (BoundUserInterfaceState)(object)state);
	}

	public string GetFruitSprite(EntityPrototype ent)
	{
		XenoFruitComponent fruit = default(XenoFruitComponent);
		if (!ent.TryGetComponent<XenoFruitComponent>(ref fruit, _componentFactory))
		{
			return "fruit_lesser_spent";
		}
		return fruit.GrownState;
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoFruitComponent> fruitQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoFruitComponent>();
		EntityUid uid = default(EntityUid);
		XenoFruitComponent fruit = default(XenoFruitComponent);
		while (fruitQuery.MoveNext(ref uid, ref fruit))
		{
			if (fruit.State == XenoFruitState.Growing)
			{
				XenoFruitComponent xenoFruitComponent = fruit;
				TimeSpan valueOrDefault = xenoFruitComponent.GrowAt.GetValueOrDefault();
				if (!xenoFruitComponent.GrowAt.HasValue)
				{
					valueOrDefault = time + fruit.GrowTime;
					xenoFruitComponent.GrowAt = valueOrDefault;
				}
				valueOrDefault = time;
				TimeSpan? growAt = fruit.GrowAt;
				if (!(valueOrDefault < growAt))
				{
					SetFruitState(Entity<XenoFruitComponent>.op_Implicit((uid, fruit)), XenoFruitState.Grown);
					_aura.GiveAura(uid, fruit.OutlineColor, null);
				}
			}
		}
		EntityQueryEnumerator<XenoFruitEffectSpeedComponent> speedQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoFruitEffectSpeedComponent>();
		EntityUid uid2 = default(EntityUid);
		XenoFruitEffectSpeedComponent effect = default(XenoFruitEffectSpeedComponent);
		while (speedQuery.MoveNext(ref uid2, ref effect))
		{
			XenoFruitEffectSpeedComponent xenoFruitEffectSpeedComponent = effect;
			TimeSpan valueOrDefault = xenoFruitEffectSpeedComponent.EndAt.GetValueOrDefault();
			if (!xenoFruitEffectSpeedComponent.EndAt.HasValue)
			{
				valueOrDefault = time + effect.Duration;
				xenoFruitEffectSpeedComponent.EndAt = valueOrDefault;
			}
			valueOrDefault = time;
			TimeSpan? growAt = effect.EndAt;
			if (!(valueOrDefault < growAt))
			{
				((EntitySystem)this).RemCompDeferred<XenoFruitEffectSpeedComponent>(uid2);
			}
		}
		EntityQueryEnumerator<XenoFruitEffectHasteComponent> hasteQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoFruitEffectHasteComponent>();
		EntityUid uid3 = default(EntityUid);
		XenoFruitEffectHasteComponent effect2 = default(XenoFruitEffectHasteComponent);
		while (hasteQuery.MoveNext(ref uid3, ref effect2))
		{
			XenoFruitEffectHasteComponent xenoFruitEffectHasteComponent = effect2;
			TimeSpan valueOrDefault = xenoFruitEffectHasteComponent.EndAt.GetValueOrDefault();
			if (!xenoFruitEffectHasteComponent.EndAt.HasValue)
			{
				valueOrDefault = time + effect2.Duration;
				xenoFruitEffectHasteComponent.EndAt = valueOrDefault;
			}
			valueOrDefault = time;
			TimeSpan? growAt = effect2.EndAt;
			if (!(valueOrDefault < growAt))
			{
				((EntitySystem)this).RemCompDeferred<XenoFruitEffectHasteComponent>(uid3);
			}
		}
		EntityQueryEnumerator<XenoFruitEffectRegenComponent> regenQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoFruitEffectRegenComponent>();
		EntityUid uid4 = default(EntityUid);
		XenoFruitEffectRegenComponent effect3 = default(XenoFruitEffectRegenComponent);
		while (regenQuery.MoveNext(ref uid4, ref effect3))
		{
			XenoFruitEffectRegenComponent xenoFruitEffectRegenComponent = effect3;
			TimeSpan valueOrDefault = xenoFruitEffectRegenComponent.NextTickAt.GetValueOrDefault();
			if (!xenoFruitEffectRegenComponent.NextTickAt.HasValue)
			{
				valueOrDefault = time + effect3.TickPeriod;
				xenoFruitEffectRegenComponent.NextTickAt = valueOrDefault;
			}
			valueOrDefault = time;
			TimeSpan? growAt = effect3.NextTickAt;
			if (!(valueOrDefault < growAt))
			{
				if (effect3.TicksLeft <= 0)
				{
					((EntitySystem)this).RemCompDeferred<XenoFruitEffectRegenComponent>(uid4);
					continue;
				}
				XenoFruitEffectRegenEvent ev = new XenoFruitEffectRegenEvent();
				((EntitySystem)this).RaiseLocalEvent<XenoFruitEffectRegenEvent>(uid4, ev, false);
				effect3.TicksLeft--;
				effect3.NextTickAt = time + effect3.TickPeriod;
			}
		}
		EntityQueryEnumerator<XenoFruitEffectPlasmaComponent> plasmaQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoFruitEffectPlasmaComponent>();
		EntityUid uid5 = default(EntityUid);
		XenoFruitEffectPlasmaComponent effect4 = default(XenoFruitEffectPlasmaComponent);
		while (plasmaQuery.MoveNext(ref uid5, ref effect4))
		{
			XenoFruitEffectPlasmaComponent xenoFruitEffectPlasmaComponent = effect4;
			TimeSpan valueOrDefault = xenoFruitEffectPlasmaComponent.NextTickAt.GetValueOrDefault();
			if (!xenoFruitEffectPlasmaComponent.NextTickAt.HasValue)
			{
				valueOrDefault = time + effect4.TickPeriod;
				xenoFruitEffectPlasmaComponent.NextTickAt = valueOrDefault;
			}
			valueOrDefault = time;
			TimeSpan? growAt = effect4.NextTickAt;
			if (!(valueOrDefault < growAt))
			{
				if (effect4.TicksLeft <= 0)
				{
					((EntitySystem)this).RemCompDeferred<XenoFruitEffectPlasmaComponent>(uid5);
					continue;
				}
				XenoFruitEffectPlasmaEvent ev2 = new XenoFruitEffectPlasmaEvent();
				((EntitySystem)this).RaiseLocalEvent<XenoFruitEffectPlasmaEvent>(uid5, ev2, false);
				effect4.TicksLeft--;
				effect4.NextTickAt = time + effect4.TickPeriod;
			}
		}
	}
}
