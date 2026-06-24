using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Climbing.Components;
using Content.Shared.Climbing.Systems;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Jittering;
using Content.Shared.Mind;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Evolution;

public sealed class XenoEvolutionSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _action;

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private ClimbSystem _climb;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedGameTicker _gameTicker;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedXenoAnnounceSystem _xenoAnnounce;

	[Dependency]
	private SharedXenoHiveSystem _xenoHive;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedXenoWeedsSystem _xenoWeeds;

	[Dependency]
	private IMapManager _map;

	private TimeSpan _evolutionPointsRequireOvipositorAfter;

	private TimeSpan _evolutionAccumulatePointsBefore;

	private TimeSpan _evolveSameCasteCooldown;

	private TimeSpan _earlyEvoBoostBefore;

	private readonly HashSet<EntityUid> _climbable = new HashSet<EntityUid>();

	private readonly HashSet<EntityUid> _doors = new HashSet<EntityUid>();

	private readonly HashSet<EntityUid> _intersecting = new HashSet<EntityUid>();

	private EntityQuery<MobStateComponent> _mobStateQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_mobStateQuery = ((EntitySystem)this).GetEntityQuery<MobStateComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoDevolveComponent, XenoOpenDevolveActionEvent>((EntityEventRefHandler<XenoDevolveComponent, XenoOpenDevolveActionEvent>)OnXenoOpenDevolveAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEvolutionComponent, MapInitEvent>((EntityEventRefHandler<XenoEvolutionComponent, MapInitEvent>)OnXenoEvolveMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEvolutionComponent, XenoOpenEvolutionsActionEvent>((EntityEventRefHandler<XenoEvolutionComponent, XenoOpenEvolutionsActionEvent>)OnXenoEvolveAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEvolutionComponent, XenoEvolutionDoAfterEvent>((EntityEventRefHandler<XenoEvolutionComponent, XenoEvolutionDoAfterEvent>)OnXenoEvolveDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEvolutionComponent, NewXenoEvolvedEvent>((EntityEventRefHandler<XenoEvolutionComponent, NewXenoEvolvedEvent>)OnXenoEvolutionNewEvolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEvolutionComponent, XenoDevolvedEvent>((EntityEventRefHandler<XenoEvolutionComponent, XenoDevolvedEvent>)OnXenoEvolutionDevolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNewlyEvolvedComponent, PreventCollideEvent>((EntityEventRefHandler<XenoNewlyEvolvedComponent, PreventCollideEvent>)OnNewlyEvolvedPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEvolutionGranterComponent, NewXenoEvolvedEvent>((EntityEventRefHandler<XenoEvolutionGranterComponent, NewXenoEvolvedEvent>)OnGranterEvolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoOvipositorChangedEvent>((EntityEventRefHandler<XenoOvipositorChangedEvent>)OnOvipositorChanged, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<XenoEvolutionComponent>(((EntitySystem)this).Subs, (object)XenoEvolutionUIKey.Key, (BuiEventSubscriber<XenoEvolutionComponent>)delegate(Subscriber<XenoEvolutionComponent> subs)
		{
			subs.Event<XenoEvolveBuiMsg>((EntityEventRefHandler<XenoEvolutionComponent, XenoEvolveBuiMsg>)OnXenoEvolveBui);
			subs.Event<XenoStrainBuiMsg>((EntityEventRefHandler<XenoEvolutionComponent, XenoStrainBuiMsg>)OnXenoStrainBui);
		});
		BoundUserInterfaceRegisterExt.BuiEvents<XenoDevolveComponent>(((EntitySystem)this).Subs, (object)XenoDevolveUIKey.Key, (BuiEventSubscriber<XenoDevolveComponent>)delegate(Subscriber<XenoDevolveComponent> subs)
		{
			subs.Event<XenoDevolveBuiMsg>((EntityEventRefHandler<XenoDevolveComponent, XenoDevolveBuiMsg>)OnXenoDevolveBui);
		});
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCEvolutionPointsRequireOvipositorMinutes, (Action<int>)delegate(int v)
		{
			_evolutionPointsRequireOvipositorAfter = TimeSpan.FromMinutes(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCEvolutionPointsAccumulateBeforeMinutes, (Action<int>)delegate(int v)
		{
			_evolutionAccumulatePointsBefore = TimeSpan.FromMinutes(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCXenoEvolveSameCasteCooldownSeconds, (Action<int>)delegate(int v)
		{
			_evolveSameCasteCooldown = TimeSpan.FromSeconds(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCXenoEarlyEvoPointBoostBeforeMinutes, (Action<int>)delegate(int v)
		{
			_earlyEvoBoostBefore = TimeSpan.FromMinutes(v);
		}, true);
	}

	private void OnXenoOpenDevolveAction(Entity<XenoDevolveComponent> xeno, ref XenoOpenDevolveActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && DamagedCheckPopup(Entity<XenoDevolveComponent>.op_Implicit(xeno)))
		{
			((HandledEntityEventArgs)args).Handled = true;
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoDevolveUIKey.Key, (EntityUid?)Entity<XenoDevolveComponent>.op_Implicit(xeno), false);
		}
	}

	private void OnXenoEvolveMapInit(Entity<XenoEvolutionComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_action.AddAction(Entity<XenoEvolutionComponent>.op_Implicit(ent), ref ent.Comp.Action, EntProtoId<InstantActionComponent>.op_Implicit(ent.Comp.ActionId));
	}

	private void OnXenoEvolveAction(Entity<XenoEvolutionComponent> xeno, ref XenoOpenEvolutionsActionEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoEvolutionUIKey.Key, (EntityUid?)Entity<XenoEvolutionComponent>.op_Implicit(xeno), false);
			XenoEvolveBuiState state = new XenoEvolveBuiState(LackingOvipositor());
			_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoEvolutionUIKey.Key, (BoundUserInterfaceState)(object)state);
		}
	}

	private void OnXenoEvolveBui(Entity<XenoEvolutionComponent> xeno, ref XenoEvolveBuiMsg args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoEvolutionUIKey.Key, (EntityUid?)actor, false);
		if (_net.IsClient)
		{
			return;
		}
		if (!CanEvolvePopup(xeno, args.Choice))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} sent an invalid evolution choice: {args.Choice}.");
		}
		else
		{
			if (!DamagedCheckPopup(Entity<XenoEvolutionComponent>.op_Implicit(xeno), predicted: false))
			{
				return;
			}
			TimeSpan time = _timing.CurTime;
			EntityPrototype choice = default(EntityPrototype);
			if (_prototypes.TryIndex(args.Choice, ref choice) && choice.HasComponent<XenoEvolutionGranterComponent>(_compFactory))
			{
				Entity<HiveComponent>? hive = _xenoHive.GetHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner));
				if (hive.HasValue)
				{
					Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
					TimeSpan? lastQueenDeath = hive2.Comp.LastQueenDeath;
					if (lastQueenDeath.HasValue)
					{
						TimeSpan lastQueenDeath2 = lastQueenDeath.GetValueOrDefault();
						if (time < lastQueenDeath2 + hive2.Comp.NewQueenCooldown)
						{
							TimeSpan left = lastQueenDeath2 + hive2.Comp.NewQueenCooldown - time;
							string msg = base.Loc.GetString("rmc-xeno-evolution-cant-evolve-recent-queen-death-minutes", (ValueTuple<string, object>)("minutes", left.Minutes), (ValueTuple<string, object>)("seconds", left.Seconds));
							if (left.Minutes == 0)
							{
								msg = base.Loc.GetString("rmc-xeno-evolution-cant-evolve-recent-queen-death-seconds", (ValueTuple<string, object>)("seconds", left.Seconds));
							}
							_popup.PopupEntity(msg, Entity<XenoEvolutionComponent>.op_Implicit(xeno), Entity<XenoEvolutionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
							return;
						}
					}
				}
			}
			XenoEvolutionDoAfterEvent ev = new XenoEvolutionDoAfterEvent(args.Choice);
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoEvolutionComponent>.op_Implicit(xeno), xeno.Comp.EvolutionDelay, ev, Entity<XenoEvolutionComponent>.op_Implicit(xeno))
			{
				BreakOnRest = false
			};
			if (xeno.Comp.EvolutionDelay > TimeSpan.Zero)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-evolution-start"), Entity<XenoEvolutionComponent>.op_Implicit(xeno), Entity<XenoEvolutionComponent>.op_Implicit(xeno));
			}
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				_jitter.DoJitter(Entity<XenoEvolutionComponent>.op_Implicit(xeno), xeno.Comp.EvolutionDelay, refresh: true, 80f, 8f, forceValueChange: true);
				string popupOthers = base.Loc.GetString("rmc-xeno-evolution-start-others", (ValueTuple<string, object>)("xeno", xeno));
				_popup.PopupEntity(popupOthers, Entity<XenoEvolutionComponent>.op_Implicit(xeno), Filter.PvsExcept(Entity<XenoEvolutionComponent>.op_Implicit(xeno), 2f, (IEntityManager)null), recordReplay: true, PopupType.Medium);
				string popupSelf = base.Loc.GetString("rmc-xeno-evolution-start-self");
				_popup.PopupEntity(popupSelf, Entity<XenoEvolutionComponent>.op_Implicit(xeno), Entity<XenoEvolutionComponent>.op_Implicit(xeno), PopupType.Medium);
			}
		}
	}

	private void OnXenoStrainBui(Entity<XenoEvolutionComponent> xeno, ref XenoStrainBuiMsg args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoEvolutionUIKey.Key, (EntityUid?)actor, false);
		if (!_net.IsClient)
		{
			if (!xeno.Comp.Strains.Contains(args.Choice))
			{
				((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} sent an invalid strain choice: {args.Choice}.");
			}
			else if (ContainedCheckPopup(Entity<XenoEvolutionComponent>.op_Implicit(xeno)) && DamagedCheckPopup(Entity<XenoEvolutionComponent>.op_Implicit(xeno), predicted: false))
			{
				EntityUid newXeno = TransferXeno(Entity<XenoEvolutionComponent>.op_Implicit(xeno), args.Choice);
				NewXenoEvolvedEvent ev = new NewXenoEvolvedEvent(xeno, newXeno, SubtractPoints: false);
				((EntitySystem)this).RaiseLocalEvent<NewXenoEvolvedEvent>(newXeno, ref ev, true);
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(22, 2);
				handler.AppendLiteral("Xenonid ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoEvolutionComponent>.op_Implicit(xeno), (MetaDataComponent)null), "ToPrettyString(xeno)");
				handler.AppendLiteral(" chose strain ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(newXeno)), "ToPrettyString(newXeno)");
				adminLog.Add(LogType.RMCEvolve, ref handler);
				((EntitySystem)this).Del((EntityUid?)xeno.Owner);
				AfterNewXenoEvolvedEvent afterEv = default(AfterNewXenoEvolvedEvent);
				((EntitySystem)this).RaiseLocalEvent<AfterNewXenoEvolvedEvent>(newXeno, ref afterEv, false);
			}
		}
	}

	private void OnXenoDevolveBui(Entity<XenoDevolveComponent> xeno, ref XenoDevolveBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoEvolutionUIKey.Key, (EntityUid?)Entity<XenoDevolveComponent>.op_Implicit(xeno), false);
		TryDevolve(xeno, args.Choice);
	}

	private void OnXenoEvolveDoAfter(Entity<XenoEvolutionComponent> xeno, ref XenoEvolutionDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && !((HandledEntityEventArgs)args).Handled && !args.Cancelled && _mind.TryGetMind(Entity<XenoEvolutionComponent>.op_Implicit(xeno), out EntityUid _, out MindComponent _) && CanEvolvePopup(xeno, args.Choice))
		{
			((HandledEntityEventArgs)args).Handled = true;
			EntityUid newXeno = TransferXeno(Entity<XenoEvolutionComponent>.op_Implicit(xeno), args.Choice);
			NewXenoEvolvedEvent ev = new NewXenoEvolvedEvent(xeno, newXeno, SubtractPoints: true);
			((EntitySystem)this).RaiseLocalEvent<NewXenoEvolvedEvent>(newXeno, ref ev, true);
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(22, 2);
			handler.AppendLiteral("Xenonid ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoEvolutionComponent>.op_Implicit(xeno), (MetaDataComponent)null), "ToPrettyString(xeno)");
			handler.AppendLiteral(" evolved into ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(newXeno)), "ToPrettyString(newXeno)");
			adminLog.Add(LogType.RMCEvolve, ref handler);
			((EntitySystem)this).Del((EntityUid?)xeno.Owner);
			_popup.PopupEntity(base.Loc.GetString("cm-xeno-evolution-end"), newXeno, newXeno);
			AfterNewXenoEvolvedEvent afterEv = default(AfterNewXenoEvolvedEvent);
			((EntitySystem)this).RaiseLocalEvent<AfterNewXenoEvolvedEvent>(newXeno, ref afterEv, false);
		}
	}

	private void OnXenoEvolutionNewEvolved(Entity<XenoEvolutionComponent> xeno, ref NewXenoEvolvedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		TransferPoints(Entity<XenoEvolutionComponent>.op_Implicit((Entity<XenoEvolutionComponent>.op_Implicit(args.OldXeno), Entity<XenoEvolutionComponent>.op_Implicit(args.OldXeno))), xeno, args.SubtractPoints);
		_jitter.DoJitter(Entity<XenoEvolutionComponent>.op_Implicit(xeno), xeno.Comp.EvolutionJitterDuration, refresh: true, 80f, 8f, forceValueChange: true);
	}

	private void OnXenoEvolutionDevolved(Entity<XenoEvolutionComponent> xeno, ref XenoDevolvedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		TransferPoints(Entity<XenoEvolutionComponent>.op_Implicit(args.OldXeno), Entity<XenoEvolutionComponent>.op_Implicit((Entity<XenoEvolutionComponent>.op_Implicit(xeno), Entity<XenoEvolutionComponent>.op_Implicit(xeno))), subtract: false);
	}

	private void TransferPoints(Entity<XenoEvolutionComponent?> old, Entity<XenoEvolutionComponent> xeno, bool subtract)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<XenoEvolutionComponent>(Entity<XenoEvolutionComponent>.op_Implicit(old), ref old.Comp, false))
		{
			xeno.Comp.Points = (subtract ? FixedPoint2.Max(0, old.Comp.Points - old.Comp.Max) : old.Comp.Points);
			((EntitySystem)this).Dirty<XenoEvolutionComponent>(xeno, (MetaDataComponent)null);
		}
	}

	private void OnNewlyEvolvedPreventCollide(Entity<XenoNewlyEvolvedComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.StopCollide.Contains(args.OtherEntity))
		{
			args.Cancelled = true;
		}
	}

	private void OnGranterEvolved(Entity<XenoEvolutionGranterComponent> ent, ref NewXenoEvolvedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_xenoAnnounce.AnnounceSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), base.Loc.GetString("rmc-new-queen"));
	}

	private void OnOvipositorChanged(ref XenoOvipositorChangedEvent ev)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			EntityQueryEnumerator<ActorComponent, XenoEvolutionComponent> xenos = ((EntitySystem)this).EntityQueryEnumerator<ActorComponent, XenoEvolutionComponent>();
			XenoEvolveBuiState state = new XenoEvolveBuiState(LackingOvipositor());
			EntityUid uid = default(EntityUid);
			ActorComponent val = default(ActorComponent);
			XenoEvolutionComponent xenoEvolutionComponent = default(XenoEvolutionComponent);
			while (xenos.MoveNext(ref uid, ref val, ref xenoEvolutionComponent))
			{
				_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)XenoEvolutionUIKey.Key, (BoundUserInterfaceState)(object)state);
			}
		}
	}

	private bool ContainedCheckPopup(EntityUid xeno, bool doPopup = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!_container.IsEntityInContainer(xeno, (MetaDataComponent)null))
		{
			return true;
		}
		if (doPopup)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-evolution-failed-bad-location"), xeno, xeno, PopupType.MediumCaution);
		}
		return false;
	}

	private bool DamagedCheckPopup(EntityUid xeno, bool predicted = true, bool doPopup = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(xeno, ref damageable) || damageable.TotalDamage <= 1)
		{
			return true;
		}
		if (predicted)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-evolution-cant-evolve-damaged"), xeno, xeno, PopupType.MediumCaution);
		}
		else
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-evolution-cant-evolve-damaged"), xeno, xeno, PopupType.MediumCaution);
		}
		return false;
	}

	private bool CanEvolvePopup(Entity<XenoEvolutionComponent> xeno, EntProtoId newXeno, bool doPopup = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.EvolvesTo.Contains(newXeno) && !xeno.Comp.EvolvesToWithoutPoints.Contains(newXeno))
		{
			return false;
		}
		EntityPrototype prototype = default(EntityPrototype);
		if (!_prototypes.TryIndex(newXeno, ref prototype))
		{
			return true;
		}
		if (!ContainedCheckPopup(Entity<XenoEvolutionComponent>.op_Implicit(xeno), doPopup))
		{
			return false;
		}
		XenoEvolutionCappedComponent capped = default(XenoEvolutionCappedComponent);
		if (prototype.TryGetComponent<XenoEvolutionCappedComponent>(ref capped, _compFactory) && this.HasLiving<XenoEvolutionCappedComponent>(capped.Max, (Predicate<Entity<XenoEvolutionCappedComponent>>?)((Entity<XenoEvolutionCappedComponent> e) => e.Comp.Id == capped.Id)))
		{
			if (doPopup)
			{
				_popup.PopupEntity(base.Loc.GetString("cm-xeno-evolution-failed-already-have", (ValueTuple<string, object>)("prototype", prototype.Name)), Entity<XenoEvolutionComponent>.op_Implicit(xeno), Entity<XenoEvolutionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			}
			return false;
		}
		if (!xeno.Comp.CanEvolveWithoutGranter && !this.HasLiving<XenoEvolutionGranterComponent>(1, (Predicate<Entity<XenoEvolutionGranterComponent>>?)null))
		{
			if (doPopup)
			{
				_popup.PopupEntity(base.Loc.GetString("cm-xeno-evolution-failed-hive-shaken"), Entity<XenoEvolutionComponent>.op_Implicit(xeno), Entity<XenoEvolutionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			}
			return false;
		}
		RestrictEvolveOffWeedsComponent comp = default(RestrictEvolveOffWeedsComponent);
		if (((EntitySystem)this).TryComp<RestrictEvolveOffWeedsComponent>(xeno.Owner, ref comp))
		{
			EntityCoordinates coordinates = _transform.GetMoverCoordinates(Entity<XenoEvolutionComponent>.op_Implicit(xeno)).SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
			EntityUid? grid = _transform.GetGrid(coordinates);
			if (grid.HasValue)
			{
				EntityUid gridUid = grid.GetValueOrDefault();
				MapGridComponent grid2 = default(MapGridComponent);
				if (((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref grid2))
				{
					if (!_xenoWeeds.IsOnWeeds(Entity<MapGridComponent>.op_Implicit((gridUid, grid2)), coordinates) && comp.RestrictTime > _gameTicker.RoundDuration())
					{
						_popup.PopupEntity(base.Loc.GetString("rmc-xeno-evolution-failed-early-weeds"), Entity<XenoEvolutionComponent>.op_Implicit(xeno), Entity<XenoEvolutionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
						return false;
					}
					goto IL_01cb;
				}
			}
			return false;
		}
		goto IL_01cb;
		IL_01cb:
		XenoComponent newXenoComp = default(XenoComponent);
		prototype.TryGetComponent<XenoComponent>(ref newXenoComp, _compFactory);
		if (newXenoComp != null && newXenoComp.UnlockAt > _gameTicker.RoundDuration())
		{
			if (doPopup)
			{
				_popup.PopupEntity(base.Loc.GetString("cm-xeno-evolution-failed-cannot-support"), Entity<XenoEvolutionComponent>.op_Implicit(xeno), Entity<XenoEvolutionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			}
			return false;
		}
		if (newXenoComp != null && !newXenoComp.BypassTierCount)
		{
			Entity<HiveComponent>? hive = _xenoHive.GetHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner));
			if (hive.HasValue)
			{
				Entity<HiveComponent> oldHive = hive.GetValueOrDefault();
				if (_xenoHive.TryGetTierLimit(Entity<HiveComponent>.op_Implicit((Entity<HiveComponent>.op_Implicit(oldHive), oldHive.Comp)), newXenoComp.Tier, out var limit))
				{
					int existing = 0;
					double total = Math.Sqrt(oldHive.Comp.BurrowedLarva * oldHive.Comp.BurrowedLarvaSlotFactor);
					total = Math.Min(total, oldHive.Comp.BurrowedLarva);
					EntityQueryEnumerator<XenoComponent, HiveMemberComponent> current = ((EntitySystem)this).EntityQueryEnumerator<XenoComponent, HiveMemberComponent>();
					Dictionary<EntProtoId, int> slotCount = oldHive.Comp.FreeSlots.ToDictionary();
					EntityUid uid = default(EntityUid);
					XenoComponent existingComp = default(XenoComponent);
					HiveMemberComponent member = default(HiveMemberComponent);
					while (current.MoveNext(ref uid, ref existingComp, ref member))
					{
						if (_mobState.IsDead(uid))
						{
							continue;
						}
						EntityUid? grid = member.Hive;
						EntityUid owner = oldHive.Owner;
						if (!grid.HasValue || grid.GetValueOrDefault() != owner || !existingComp.CountedInSlots)
						{
							continue;
						}
						total += 1.0;
						if (existingComp.Tier >= newXenoComp.Tier)
						{
							if (slotCount.ContainsKey(EntProtoId.op_Implicit(existingComp.Role.Id)) && slotCount[EntProtoId.op_Implicit(existingComp.Role.Id)] > 0)
							{
								slotCount[EntProtoId.op_Implicit(existingComp.Role.Id)]--;
							}
							else
							{
								existing++;
							}
						}
					}
					if (total != 0.0 && (float)existing / (float)total >= limit && (!slotCount.ContainsKey(newXeno) || slotCount[newXeno] <= 0))
					{
						if (doPopup)
						{
							_popup.PopupEntity(base.Loc.GetString("cm-xeno-evolution-failed-hive-full", (ValueTuple<string, object>)("tier", newXenoComp.Tier)), Entity<XenoEvolutionComponent>.op_Implicit(xeno), Entity<XenoEvolutionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
						}
						return false;
					}
				}
			}
		}
		XenoRecentlyDevolvedComponent recently = default(XenoRecentlyDevolvedComponent);
		if (((EntitySystem)this).TryComp<XenoRecentlyDevolvedComponent>(Entity<XenoEvolutionComponent>.op_Implicit(xeno), ref recently) && recently.Recent.TryGetValue(newXeno, out var at) && at + _evolveSameCasteCooldown > _timing.CurTime)
		{
			TimeSpan timeRemaining = at + _evolveSameCasteCooldown - _timing.CurTime;
			string msg = base.Loc.GetString("rmc-xeno-evolution-cant-evolve-caste-cooldown", (ValueTuple<string, object>)("minutes", timeRemaining.Minutes), (ValueTuple<string, object>)("seconds", timeRemaining.Seconds));
			if (doPopup)
			{
				_popup.PopupEntity(msg, Entity<XenoEvolutionComponent>.op_Implicit(xeno), Entity<XenoEvolutionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			}
			return false;
		}
		return true;
	}

	private bool CanEvolveAny(Entity<XenoEvolutionComponent> xeno)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Points >= xeno.Comp.Max && xeno.Comp.EvolvesTo.Count > 0)
		{
			return true;
		}
		foreach (EntProtoId evolution in xeno.Comp.EvolvesToWithoutPoints)
		{
			if (CanEvolvePopup(xeno, evolution, doPopup: false))
			{
				return true;
			}
		}
		return false;
	}

	public int GetLiving<T>(Predicate<Entity<T>>? predicate = null) where T : IComponent
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		int total = 0;
		EntityQueryEnumerator<T> query = ((EntitySystem)this).EntityQueryEnumerator<T>();
		EntityUid uid = default(EntityUid);
		T comp = default(T);
		MobStateComponent mobState = default(MobStateComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if ((!_mobStateQuery.TryComp(uid, ref mobState) || !_mobState.IsDead(uid, mobState)) && (predicate == null || predicate(Entity<T>.op_Implicit((uid, comp)))))
			{
				total++;
			}
		}
		return total;
	}

	public bool HasLiving<T>(int count, Predicate<Entity<T>>? predicate = null) where T : IComponent
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (count <= 0)
		{
			return true;
		}
		int total = 0;
		EntityQueryEnumerator<T> query = ((EntitySystem)this).EntityQueryEnumerator<T>();
		EntityUid uid = default(EntityUid);
		T comp = default(T);
		MobStateComponent mobState = default(MobStateComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if ((!_mobStateQuery.TryComp(uid, ref mobState) || !_mobState.IsDead(uid, mobState)) && (predicate == null || predicate(Entity<T>.op_Implicit((uid, comp)))))
			{
				total++;
				if (total >= count)
				{
					return true;
				}
			}
		}
		return false;
	}

	public FixedPoint2 AddPointsCapped(Entity<XenoEvolutionComponent?> evolution, FixedPoint2 points)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoEvolutionComponent>(Entity<XenoEvolutionComponent>.op_Implicit(evolution), ref evolution.Comp, false))
		{
			return FixedPoint2.Zero;
		}
		FixedPoint2 oldPoints = evolution.Comp.Points;
		evolution.Comp.Points += FixedPoint2.Min(evolution.Comp.Max, points);
		((EntitySystem)this).Dirty<XenoEvolutionComponent>(evolution, (MetaDataComponent)null);
		return evolution.Comp.Points - oldPoints;
	}

	public void SetPoints(Entity<XenoEvolutionComponent> evolution, FixedPoint2 points)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		evolution.Comp.Points = points;
		((EntitySystem)this).Dirty<XenoEvolutionComponent>(evolution, (MetaDataComponent)null);
	}

	public bool NeedsOvipositor()
	{
		return _gameTicker.RoundDuration() > _evolutionPointsRequireOvipositorAfter;
	}

	public bool HasOvipositor()
	{
		return this.HasLiving<XenoEvolutionGranterComponent>(1, (Predicate<Entity<XenoEvolutionGranterComponent>>?)((Entity<XenoEvolutionGranterComponent> e) => ((EntitySystem)this).HasComp<XenoAttachedOvipositorComponent>(Entity<XenoEvolutionGranterComponent>.op_Implicit(e))));
	}

	public bool LackingOvipositor()
	{
		if (NeedsOvipositor())
		{
			return !HasOvipositor();
		}
		return false;
	}

	private EntityUid TransferXeno(EntityUid xeno, EntProtoId proto)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = _transform.GetMoverCoordinates(xeno);
		EntityUid newXeno = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(proto), coordinates);
		_xenoHive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno), Entity<HiveMemberComponent>.op_Implicit(newXeno));
		if (_mind.TryGetMind(xeno, out EntityUid mindId, out MindComponent _))
		{
			_mind.TransferTo(mindId, newXeno);
			_mind.UnVisit(mindId);
		}
		foreach (EntityUid held in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(xeno)))
		{
			_hands.TryDrop(Entity<HandsComponent>.op_Implicit(xeno), held);
		}
		XenoNewlyEvolvedComponent comp = ((EntitySystem)this).EnsureComp<XenoNewlyEvolvedComponent>(newXeno);
		_doors.Clear();
		_entityLookup.GetEntitiesIntersecting(xeno, _doors, (LookupFlags)110);
		foreach (EntityUid id in _doors)
		{
			if (((EntitySystem)this).HasComp<DoorComponent>(id) || ((EntitySystem)this).HasComp<AirlockComponent>(id))
			{
				comp.StopCollide.Add(id);
			}
		}
		XenoRecentlyDevolvedComponent newRecently = ((EntitySystem)this).EnsureComp<XenoRecentlyDevolvedComponent>(newXeno);
		XenoRecentlyDevolvedComponent oldRecently = default(XenoRecentlyDevolvedComponent);
		if (((EntitySystem)this).TryComp<XenoRecentlyDevolvedComponent>(xeno, ref oldRecently))
		{
			foreach (var (id2, time) in oldRecently.Recent)
			{
				newRecently.Recent[id2] = time;
			}
		}
		EntityPrototype obj = ((EntitySystem)this).Prototype(xeno, (MetaDataComponent)null);
		string oldId = ((obj != null) ? obj.ID : null);
		if (oldId != null)
		{
			newRecently.Recent[EntProtoId.op_Implicit(oldId)] = _timing.CurTime;
		}
		return newXeno;
	}

	private void TryDevolve(Entity<XenoDevolveComponent> xeno, EntProtoId to, bool damagedCheck = true)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (damagedCheck && !DamagedCheckPopup(Entity<XenoDevolveComponent>.op_Implicit(xeno)))
		{
			return;
		}
		EntityUid? val = Devolve(xeno, to);
		if (val.HasValue)
		{
			EntityUid newXeno = val.GetValueOrDefault();
			if (_net.IsServer)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-evolution-devolve", (ValueTuple<string, object>)("xeno", newXeno)), newXeno, newXeno, PopupType.LargeCaution);
			}
		}
	}

	public EntityUid? Devolve(Entity<XenoDevolveComponent> xeno, EntProtoId to)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !Enumerable.Contains(xeno.Comp.DevolvesTo, to))
		{
			return null;
		}
		EntityUid newXeno = TransferXeno(Entity<XenoDevolveComponent>.op_Implicit(xeno), to);
		XenoDevolvedEvent ev = new XenoDevolvedEvent(Entity<XenoDevolveComponent>.op_Implicit(xeno), newXeno);
		((EntitySystem)this).RaiseLocalEvent<XenoDevolvedEvent>(newXeno, ref ev, true);
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(23, 2);
		handler.AppendLiteral("Xenonid ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoDevolveComponent>.op_Implicit(xeno), (MetaDataComponent)null), "ToPrettyString(xeno)");
		handler.AppendLiteral(" devolved into ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(newXeno)), "ToPrettyString(newXeno)");
		adminLog.Add(LogType.RMCDevolve, ref handler);
		((EntitySystem)this).Del((EntityUid?)xeno.Owner);
		AfterNewXenoEvolvedEvent afterEv = default(AfterNewXenoEvolvedEvent);
		((EntitySystem)this).RaiseLocalEvent<AfterNewXenoEvolvedEvent>(newXeno, ref afterEv, false);
		return newXeno;
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoNewlyEvolvedComponent> newly = ((EntitySystem)this).EntityQueryEnumerator<XenoNewlyEvolvedComponent>();
		EntityUid uid = default(EntityUid);
		XenoNewlyEvolvedComponent comp = default(XenoNewlyEvolvedComponent);
		ClimbingComponent climbing = default(ClimbingComponent);
		while (newly.MoveNext(ref uid, ref comp))
		{
			if (comp.TriedClimb)
			{
				_intersecting.Clear();
				_entityLookup.GetEntitiesIntersecting(uid, _intersecting, (LookupFlags)110);
				for (int i = comp.StopCollide.Count - 1; i >= 0; i--)
				{
					EntityUid colliding = comp.StopCollide[i];
					if (!_intersecting.Contains(colliding))
					{
						comp.StopCollide.RemoveAt(i);
					}
				}
				if (comp.StopCollide.Count == 0)
				{
					((EntitySystem)this).RemCompDeferred<XenoNewlyEvolvedComponent>(uid);
				}
				continue;
			}
			comp.TriedClimb = true;
			if (!((EntitySystem)this).TryComp<ClimbingComponent>(uid, ref climbing))
			{
				continue;
			}
			_climbable.Clear();
			_entityLookup.GetEntitiesIntersecting(uid, _climbable, (LookupFlags)110);
			foreach (EntityUid intersecting in _climbable)
			{
				if (((EntitySystem)this).HasComp<ClimbableComponent>(intersecting))
				{
					_climb.ForciblySetClimbing(uid, intersecting);
					((EntitySystem)this).Dirty(uid, (IComponent)(object)climbing, (MetaDataComponent)null);
					break;
				}
			}
		}
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		TimeSpan roundDuration = _gameTicker.RoundDuration();
		bool needsOvipositor = NeedsOvipositor();
		bool hasGranter = (needsOvipositor ? HasOvipositor() : this.HasLiving<XenoEvolutionGranterComponent>(1, (Predicate<Entity<XenoEvolutionGranterComponent>>?)null));
		if (needsOvipositor)
		{
			EntityQueryEnumerator<XenoEvolutionGranterComponent> granters = ((EntitySystem)this).EntityQueryEnumerator<XenoEvolutionGranterComponent>();
			EntityUid uid2 = default(EntityUid);
			XenoEvolutionGranterComponent granter = default(XenoEvolutionGranterComponent);
			while (granters.MoveNext(ref uid2, ref granter))
			{
				if (!granter.GotOvipositorPopup)
				{
					granter.GotOvipositorPopup = true;
					((EntitySystem)this).Dirty(uid2, (IComponent)(object)granter, (MetaDataComponent)null);
					_popup.PopupEntity("It is time to settle down and let your children grow.", uid2, uid2, PopupType.LargeCaution);
					_xenoHive.AnnounceNeedsOvipositorToSameHive(Entity<HiveMemberComponent>.op_Implicit(uid2));
				}
			}
		}
		FixedPoint2 evoBonus = FixedPoint2.Zero;
		EntityQueryEnumerator<EvolutionBonusComponent> bonuses = ((EntitySystem)this).EntityQueryEnumerator<EvolutionBonusComponent>();
		EvolutionBonusComponent comp2 = default(EvolutionBonusComponent);
		while (bonuses.MoveNext(ref comp2))
		{
			evoBonus += comp2.Amount;
		}
		FixedPoint2? evoOverride = null;
		EntityQueryEnumerator<EvolutionOverrideComponent> overrides = ((EntitySystem)this).EntityQueryEnumerator<EvolutionOverrideComponent>();
		EvolutionOverrideComponent comp3 = default(EvolutionOverrideComponent);
		while (overrides.MoveNext(ref comp3))
		{
			evoOverride = comp3.Amount;
		}
		EntityQueryEnumerator<XenoEvolutionComponent> evolution = ((EntitySystem)this).EntityQueryEnumerator<XenoEvolutionComponent>();
		EntityUid uid3 = default(EntityUid);
		XenoEvolutionComponent comp4 = default(XenoEvolutionComponent);
		while (evolution.MoveNext(ref uid3, ref comp4))
		{
			if (comp4.Max == FixedPoint2.Zero || time < comp4.LastPointsAt + TimeSpan.FromSeconds(1L))
			{
				continue;
			}
			comp4.LastPointsAt = time;
			((EntitySystem)this).Dirty(uid3, (IComponent)(object)comp4, (MetaDataComponent)null);
			if (!comp4.GotPopup && CanEvolveAny(Entity<XenoEvolutionComponent>.op_Implicit((uid3, comp4))))
			{
				comp4.GotPopup = true;
				((EntitySystem)this).Dirty(uid3, (IComponent)(object)comp4, (MetaDataComponent)null);
				_popup.PopupEntity(base.Loc.GetString("cm-xeno-evolution-ready"), uid3, uid3, PopupType.Large);
				_audio.PlayEntity(comp4.EvolutionReadySound, uid3, uid3, (AudioParams?)null);
				continue;
			}
			FixedPoint2 points = ((_earlyEvoBoostBefore > _gameTicker.RoundDuration()) ? comp4.EarlyPointsPerSecond : comp4.PointsPerSecond);
			FixedPoint2 gain = evoOverride ?? (points + evoBonus);
			if (comp4.Points < comp4.Max || roundDuration < _evolutionAccumulatePointsBefore)
			{
				if (!needsOvipositor || !comp4.RequiresGranter || hasGranter)
				{
					SetPoints(Entity<XenoEvolutionComponent>.op_Implicit((uid3, comp4)), comp4.Points + gain);
				}
			}
			else if (comp4.Points > comp4.Max)
			{
				SetPoints(Entity<XenoEvolutionComponent>.op_Implicit((uid3, comp4)), FixedPoint2.Max(comp4.Points - gain, comp4.Max));
			}
		}
	}
}
