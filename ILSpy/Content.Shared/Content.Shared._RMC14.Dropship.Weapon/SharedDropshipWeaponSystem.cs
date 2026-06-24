using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.ElectronicSystem;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Utility.Systems;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Explosion.Implosion;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Medical.MedevacStretcher;
using Content.Shared._RMC14.OnCollide;
using Content.Shared._RMC14.PowerLoader;
using Content.Shared._RMC14.Rangefinder;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Administration.Logs;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Examine;
using Content.Shared.Explosion;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.GameTicking;
using Content.Shared.IgnitionSource;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Light.Components;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.ParaDrop;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Dropship.Weapon;

public abstract class SharedDropshipWeaponSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoorSystem _door;

	[Dependency]
	private SharedDropshipSystem _dropship;

	[Dependency]
	private SharedRMCEquipmentDeployerSystem _equipmentDeployer;

	[Dependency]
	private DropshipUtilitySystem _dropshipUtility;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private NameModifierSystem _name;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedOnCollideSystem _onCollide;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPointLightSystem _pointLight;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private PowerLoaderSystem _powerloader;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedRMCFlammableSystem _rmcFlammable;

	[Dependency]
	private SharedRMCExplosionSystem _rmcExplosion;

	[Dependency]
	private RMCImplosionSystem _rmcImplosion;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SquadSystem _squad;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	private static readonly EntProtoId DropshipTargetMarker = EntProtoId.op_Implicit("RMCLaserDropshipTarget");

	private const string SpotlightState = "spotlights_";

	private readonly HashSet<Entity<DamageableComponent>> _damageables = new HashSet<Entity<DamageableComponent>>();

	private readonly List<EntityUid> _targetsToRemove = new List<EntityUid>();

	private int _nextId = 1;

	public bool CasDebug { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlareSignalComponent, IgnitionEvent>((EntityEventRefHandler<FlareSignalComponent, IgnitionEvent>)OnFlareSignalIgnition, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlareSignalComponent, GettingPickedUpAttemptEvent>((EntityEventRefHandler<FlareSignalComponent, GettingPickedUpAttemptEvent>)OnFlareSignalPickupAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlareSignalComponent, ExaminedEvent>((EntityEventRefHandler<FlareSignalComponent, ExaminedEvent>)OnFlareSignalExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlareSignalComponent, DroppedEvent>((EntityEventRefHandler<FlareSignalComponent, DroppedEvent>)OnFlareSignalDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlareSignalComponent, ThrownEvent>((EntityEventRefHandler<FlareSignalComponent, ThrownEvent>)OnFlareSignalThrown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlareSignalComponent, GrenadeContentThrownEvent>((EntityEventRefHandler<FlareSignalComponent, GrenadeContentThrownEvent>)OnFlareSignalGrenadeContentThrown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlareSignalComponent, StopThrowEvent>((EntityEventRefHandler<FlareSignalComponent, StopThrowEvent>)OnFlareSignalStopThrow, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlareSignalComponent, ContainerGettingInsertedAttemptEvent>((EntityEventRefHandler<FlareSignalComponent, ContainerGettingInsertedAttemptEvent>)OnFlareSignalContainerGettingInsertedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipTerminalWeaponsComponent, MapInitEvent>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, MapInitEvent>)OnTerminalMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipTerminalWeaponsComponent, BoundUIOpenedEvent>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, BoundUIOpenedEvent>)OnTerminalBUIOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipTerminalWeaponsComponent, BoundUIClosedEvent>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, BoundUIClosedEvent>)OnTerminalBUIClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipTargetComponent, MapInitEvent>((EntityEventRefHandler<DropshipTargetComponent, MapInitEvent>)OnDropshipTargetMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipTargetComponent, ComponentRemove>((EntityEventRefHandler<DropshipTargetComponent, ComponentRemove>)OnDropshipTargetRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipTargetComponent, EntityTerminatingEvent>((EntityEventRefHandler<DropshipTargetComponent, EntityTerminatingEvent>)OnDropshipTargetRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipTargetComponent, ExaminedEvent>((EntityEventRefHandler<DropshipTargetComponent, ExaminedEvent>)OnActiveFlareExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveFlareSignalComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<ActiveFlareSignalComponent, RefreshNameModifiersEvent>)OnRefreshNameModifier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipTargetEyeComponent, ComponentRemove>((EntityEventRefHandler<DropshipTargetEyeComponent, ComponentRemove>)OnDropshipTargetEyeRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipTargetEyeComponent, EntityTerminatingEvent>((EntityEventRefHandler<DropshipTargetEyeComponent, EntityTerminatingEvent>)OnDropshipTargetEyeRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipAmmoComponent, ExaminedEvent>((EntityEventRefHandler<DropshipAmmoComponent, ExaminedEvent>)OnAmmoExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipAmmoComponent, PowerLoaderInteractEvent>((EntityEventRefHandler<DropshipAmmoComponent, PowerLoaderInteractEvent>)OnAmmoInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivateDropshipWeaponOnSpawnComponent, MapInitEvent>((EntityEventRefHandler<ActivateDropshipWeaponOnSpawnComponent, MapInitEvent>)OnDropshipWeaponOnSpawnFire, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<DropshipTerminalWeaponsComponent>(((EntitySystem)this).Subs, (object)DropshipTerminalWeaponsUi.Key, (BuiEventSubscriber<DropshipTerminalWeaponsComponent>)delegate(Subscriber<DropshipTerminalWeaponsComponent> subs)
		{
			subs.Event<DropshipTerminalWeaponsChangeScreenMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChangeScreenMsg>)OnWeaponsChangeScreenMsg);
			subs.Event<DropshipTerminalWeaponsChooseWeaponMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseWeaponMsg>)OnWeaponsChooseWeaponMsg);
			subs.Event<DropshipTerminalWeaponsChooseMedevacMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseMedevacMsg>)OnWeaponsChooseMedevacMsg);
			subs.Event<DropshipTerminalWeaponsChooseFultonMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseFultonMsg>)OnWeaponsChooseFultonMsg);
			subs.Event<DropshipTerminalWeaponsChooseParaDropMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseParaDropMsg>)OnWeaponsChooseParaDropMsg);
			subs.Event<DropshipTerminalWeaponsChooseSpotlightMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseSpotlightMsg>)OnWeaponsChooseSpotlightMsg);
			subs.Event<DropshipTerminalWeaponsChooseEquipmentDeployerMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseEquipmentDeployerMsg>)OnWeaponsChooseEquipmentDeployerMsg);
			subs.Event<DropshipTerminalWeaponsFireMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsFireMsg>)OnWeaponsFireMsg);
			subs.Event<DropshipTerminalWeaponsNightVisionMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsNightVisionMsg>)OnWeaponsNightVisionMsg);
			subs.Event<DropshipTerminalWeaponsExitMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsExitMsg>)OnWeaponsExitMsg);
			subs.Event<DropshipTerminalWeaponsCancelMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsCancelMsg>)OnWeaponsCancelMsg);
			subs.Event<DropshipTerminalWeaponsAdjustOffsetMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsAdjustOffsetMsg>)OnWeaponsAdjustOffset);
			subs.Event<DropshipTerminalWeaponsResetOffsetMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsResetOffsetMsg>)OnWeaponsResetOffset);
			subs.Event<DropshipTerminalWeaponsTargetsPreviousMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsTargetsPreviousMsg>)OnWeaponsTargetsPrevious);
			subs.Event<DropshipTerminalWeaponsTargetsNextMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsTargetsNextMsg>)OnWeaponsTargetsNext);
			subs.Event<DropshipTerminalWeaponsTargetsSelectMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsTargetsSelectMsg>)OnWeaponsTargetsSelect);
			subs.Event<DropshipTerminalWeaponsMedevacPreviousMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsMedevacPreviousMsg>)OnWeaponsMedevacPrevious);
			subs.Event<DropshipTerminalWeaponsMedevacNextMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsMedevacNextMsg>)OnWeaponsMedevacNext);
			subs.Event<DropshipTerminalWeaponsMedevacSelectMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsMedevacSelectMsg>)OnWeaponsMedevacSelect);
			subs.Event<DropshipTerminalWeaponsFultonPreviousMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsFultonPreviousMsg>)OnWeaponsFultonPrevious);
			subs.Event<DropshipTerminalWeaponsFultonNextMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsFultonNextMsg>)OnWeaponsFultonNext);
			subs.Event<DropshipTerminalWeaponsFultonSelectMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsFultonSelectMsg>)OnWeaponsFultonSelect);
			subs.Event<DropShipTerminalWeaponsParaDropTargetSelectMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropShipTerminalWeaponsParaDropTargetSelectMsg>)OnWeaponsParaDropSelect);
			subs.Event<DropShipTerminalWeaponsSpotlightToggleMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropShipTerminalWeaponsSpotlightToggleMsg>)OnWeaponsSpotlightSelect);
			subs.Event<DropShipTerminalWeaponsEquipmentDeployToggleMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropShipTerminalWeaponsEquipmentDeployToggleMsg>)OnEquipmentDeploy);
			subs.Event<DropShipTerminalWeaponsEquipmentAutoDeployToggleMsg>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropShipTerminalWeaponsEquipmentAutoDeployToggleMsg>)OnEquipmentToggleAutoDeploy);
		});
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, RMCCVars.RMCDropshipCASDebug, (Action<bool>)delegate(bool v)
		{
			CasDebug = v;
		}, true);
	}

	private void SetTarget(Entity<DropshipTerminalWeaponsComponent> dropshipWeaponsTerminal, EntityUid? newTarget)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = newTarget;
		EntityUid? target = dropshipWeaponsTerminal.Comp.Target;
		if ((val.HasValue == target.HasValue && (!val.HasValue || val.GetValueOrDefault() == target.GetValueOrDefault())) || !_dropship.TryGetGridDropship(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(dropshipWeaponsTerminal), out Entity<DropshipComponent> dropship))
		{
			return;
		}
		dropshipWeaponsTerminal.Comp.Target = newTarget;
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(dropshipWeaponsTerminal, (MetaDataComponent)null);
		DropshipTargetChangedEvent ev = new DropshipTargetChangedEvent(((EntitySystem)this).GetNetEntity(newTarget, (MetaDataComponent)null));
		foreach (EntityUid attachmentPoint in dropship.Comp.AttachmentPoints)
		{
			((EntitySystem)this).RaiseLocalEvent<DropshipTargetChangedEvent>(attachmentPoint, ev, false);
		}
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_nextId = 1;
	}

	private void OnFlareSignalIgnition(Entity<FlareSignalComponent> ent, ref IgnitionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Ignite)
		{
			if (((EntitySystem)this).HasComp<PhysicsComponent>(Entity<FlareSignalComponent>.op_Implicit(ent)))
			{
				_physics.SetBodyType(Entity<FlareSignalComponent>.op_Implicit(ent), (BodyType)8, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
			}
			((EntitySystem)this).RemCompDeferred<DropshipTargetComponent>(Entity<FlareSignalComponent>.op_Implicit(ent));
		}
	}

	private void OnFlareSignalPickupAttempt(Entity<FlareSignalComponent> ent, ref GettingPickedUpAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && IsFlareLit(Entity<FlareSignalComponent>.op_Implicit(ent)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnFlareSignalExamined(Entity<FlareSignalComponent> ent, ref ExaminedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("FlareSignalComponent"))
		{
			ExpendableLightComponent expendable = default(ExpendableLightComponent);
			if (((EntitySystem)this).TryComp<ExpendableLightComponent>(Entity<FlareSignalComponent>.op_Implicit(ent), ref expendable) && expendable.CurrentState != ExpendableLightState.Dead)
			{
				args.PushMarkup(base.Loc.GetString("rmc-laser-designator-signal-flare-examine"));
			}
		}
	}

	private void OnFlareSignalDropped(Entity<FlareSignalComponent> ent, ref DroppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (IsFlareLit(Entity<FlareSignalComponent>.op_Implicit(ent)))
		{
			StartTrackingActiveFlare(ent, args.User);
		}
	}

	private void OnFlareSignalThrown(Entity<FlareSignalComponent> ent, ref ThrownEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (IsFlareLit(Entity<FlareSignalComponent>.op_Implicit(ent)))
		{
			StartTrackingActiveFlare(ent, args.User);
		}
	}

	private void OnFlareSignalStopThrow(Entity<FlareSignalComponent> ent, ref StopThrowEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<DropshipTargetComponent>(Entity<FlareSignalComponent>.op_Implicit(ent)))
		{
			_physics.SetBodyType(Entity<FlareSignalComponent>.op_Implicit(ent), (BodyType)4, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
		}
	}

	private void OnFlareSignalContainerGettingInsertedAttempt(Entity<FlareSignalComponent> ent, ref ContainerGettingInsertedAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && IsFlareLit(Entity<FlareSignalComponent>.op_Implicit(ent)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnFlareSignalGrenadeContentThrown(Entity<FlareSignalComponent> ent, ref GrenadeContentThrownEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent projectile = default(ProjectileComponent);
		if (((EntitySystem)this).TryComp<ProjectileComponent>(args.Source, ref projectile))
		{
			int id = ComputeNextId();
			string abbreviation = base.Loc.GetString("rmc-laser-designator-target-abbreviation", (ValueTuple<string, object>)("id", id));
			if (projectile.Shooter.HasValue)
			{
				abbreviation = GetUserAbbreviation(projectile.Shooter.Value, id);
			}
			RMCAirShotComponent airShot = default(RMCAirShotComponent);
			if (projectile.Weapon.HasValue && ((EntitySystem)this).TryComp<RMCAirShotComponent>(projectile.Weapon, ref airShot))
			{
				airShot.LastFlareId = abbreviation;
				((EntitySystem)this).Dirty(projectile.Weapon.Value, (IComponent)(object)airShot, (MetaDataComponent)null);
			}
			MakeDropshipTarget(Entity<FlareSignalComponent>.op_Implicit(ent), abbreviation);
			_physics.SetBodyType(Entity<FlareSignalComponent>.op_Implicit(ent), (BodyType)4, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
		}
	}

	private void OnActiveFlareExamined(Entity<DropshipTargetComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		string id = ent.Comp.Abbreviation;
		if (id != null)
		{
			args.PushMarkup(base.Loc.GetString("rmc-laser-designator-signal-flare-examine-id", (ValueTuple<string, object>)("id", id)));
		}
	}

	private void OnTerminalMapInit(Entity<DropshipTerminalWeaponsComponent> ent, ref MapInitEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		List<DropshipTerminalWeaponsComponent.TargetEnt> targets = new List<DropshipTerminalWeaponsComponent.TargetEnt>();
		EntityQueryEnumerator<DropshipTargetComponent> targetsQuery = ((EntitySystem)this).EntityQueryEnumerator<DropshipTargetComponent>();
		EntityUid uid = default(EntityUid);
		DropshipTargetComponent target = default(DropshipTargetComponent);
		while (targetsQuery.MoveNext(ref uid, ref target))
		{
			targets.Add(new DropshipTerminalWeaponsComponent.TargetEnt(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null), target.Abbreviation));
		}
		targets.Sort((DropshipTerminalWeaponsComponent.TargetEnt a, DropshipTerminalWeaponsComponent.TargetEnt b) => string.CompareOrdinal(a.Name, b.Name));
		ent.Comp.Targets = targets;
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
	}

	private void OnTerminalBUIOpened(Entity<DropshipTerminalWeaponsComponent> ent, ref BoundUIOpenedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		AddPvs(ent, Entity<ActorComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor));
	}

	private void OnTerminalBUIClosed(Entity<DropshipTerminalWeaponsComponent> ent, ref BoundUIClosedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		RemovePvs(ent, Entity<ActorComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor));
	}

	private void OnDropshipTargetMapInit(Entity<DropshipTargetComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		NetEntity netEnt = ((EntitySystem)this).GetNetEntity(Entity<DropshipTargetComponent>.op_Implicit(ent), (MetaDataComponent)null);
		EntityQueryEnumerator<DropshipTerminalWeaponsComponent> terminals = ((EntitySystem)this).EntityQueryEnumerator<DropshipTerminalWeaponsComponent>();
		EntityUid uid = default(EntityUid);
		DropshipTerminalWeaponsComponent terminal = default(DropshipTerminalWeaponsComponent);
		while (terminals.MoveNext(ref uid, ref terminal))
		{
			List<DropshipTerminalWeaponsComponent.TargetEnt> targets = terminal.Targets;
			if (((EntitySystem)this).HasComp<MedevacStretcherComponent>(Entity<DropshipTargetComponent>.op_Implicit(ent)))
			{
				targets = terminal.Medevacs;
			}
			else if (((EntitySystem)this).HasComp<RMCActiveFultonComponent>(Entity<DropshipTargetComponent>.op_Implicit(ent)))
			{
				targets = terminal.Fultons;
			}
			targets.Add(new DropshipTerminalWeaponsComponent.TargetEnt(netEnt, ent.Comp.Abbreviation));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)terminal, (MetaDataComponent)null);
		}
	}

	private void OnDropshipTargetRemove<T>(Entity<DropshipTargetComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		NetEntity netUid = ((EntitySystem)this).GetNetEntity(Entity<DropshipTargetComponent>.op_Implicit(ent), (MetaDataComponent)null);
		EntityQueryEnumerator<DropshipTerminalWeaponsComponent> terminals = ((EntitySystem)this).EntityQueryEnumerator<DropshipTerminalWeaponsComponent>();
		EntityUid uid = default(EntityUid);
		DropshipTerminalWeaponsComponent terminal = default(DropshipTerminalWeaponsComponent);
		EntityUid key;
		while (terminals.MoveNext(ref uid, ref terminal))
		{
			EntityUid? target = terminal.Target;
			key = Entity<DropshipTargetComponent>.op_Implicit(ent);
			if (target.HasValue && target.GetValueOrDefault() == key)
			{
				RemovePvsActors(Entity<DropshipTerminalWeaponsComponent>.op_Implicit((uid, terminal)));
				SetTarget(Entity<DropshipTerminalWeaponsComponent>.op_Implicit((uid, terminal)), null);
			}
			List<DropshipTerminalWeaponsComponent.TargetEnt> targets = terminal.Targets;
			if (((EntitySystem)this).HasComp<MedevacStretcherComponent>(Entity<DropshipTargetComponent>.op_Implicit(ent)))
			{
				targets = terminal.Medevacs;
			}
			else if (((EntitySystem)this).HasComp<RMCActiveFultonComponent>(Entity<DropshipTargetComponent>.op_Implicit(ent)))
			{
				targets = terminal.Fultons;
			}
			Span<DropshipTerminalWeaponsComponent.TargetEnt> span = CollectionsMarshal.AsSpan(targets);
			for (int i = 0; i < span.Length; i++)
			{
				if (!(span[i].Id != netUid))
				{
					targets.RemoveAt(i);
					break;
				}
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)terminal, (MetaDataComponent)null);
		}
		if (_net.IsClient)
		{
			return;
		}
		foreach (KeyValuePair<EntityUid, EntityUid> eye2 in ent.Comp.Eyes)
		{
			eye2.Deconstruct(out key, out var value);
			EntityUid eye = value;
			((EntitySystem)this).QueueDel((EntityUid?)eye);
		}
	}

	private void OnDropshipTargetEyeRemove<T>(Entity<DropshipTargetEyeComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		DropshipTargetComponent target = default(DropshipTargetComponent);
		if (((EntitySystem)this).TerminatingOrDeleted(ent.Comp.Target, (MetaDataComponent)null) || !((EntitySystem)this).TryComp<DropshipTargetComponent>(ent.Comp.Target, ref target))
		{
			return;
		}
		_targetsToRemove.Clear();
		foreach (var (terminal, val3) in target.Eyes)
		{
			if (val3 == ent.Owner)
			{
				_targetsToRemove.Add(terminal);
			}
		}
		foreach (EntityUid remove in _targetsToRemove)
		{
			target.Eyes.Remove(remove);
		}
	}

	private void OnAmmoExamined(Entity<DropshipAmmoComponent> ent, ref ExaminedEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("DropshipAmmoComponent"))
		{
			args.PushText(base.Loc.GetString("rmc-dropship-ammo-examine", (ValueTuple<string, object>)("rounds", ent.Comp.Rounds)));
		}
	}

	private void OnAmmoInteract(Entity<DropshipAmmoComponent> ent, ref PowerLoaderInteractEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (args.Handled)
		{
			return;
		}
		DropshipWeaponPointComponent point = default(DropshipWeaponPointComponent);
		BaseContainer container = default(BaseContainer);
		EntityUid? weapon = default(EntityUid?);
		if (((EntitySystem)this).TryComp<DropshipWeaponPointComponent>(args.Target, ref point) && _container.TryGetContainer(args.Target, point.WeaponContainerSlotId, ref container, (ContainerManagerComponent)null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref weapon))
		{
			string id = ent.Comp.Weapon.Id;
			EntityPrototype obj = ((EntitySystem)this).Prototype(weapon.Value, (MetaDataComponent)null);
			if (id != ((obj != null) ? obj.ID : null))
			{
				args.Handled = true;
				{
					foreach (EntityUid buckled in args.Buckled)
					{
						_popup.PopupClient(base.Loc.GetString("rmc-power-loader-wrong-weapon"), args.Target, buckled, PopupType.SmallCaution);
					}
					return;
				}
			}
		}
		DropshipAmmoComponent otherAmmo = default(DropshipAmmoComponent);
		if (!((EntitySystem)this).TryComp<DropshipAmmoComponent>(args.Target, ref otherAmmo))
		{
			return;
		}
		args.Handled = true;
		if (ent.Comp.AmmoType != otherAmmo.AmmoType)
		{
			foreach (EntityUid buckled2 in args.Buckled)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-power-loader-wrong-ammo"), args.Target, buckled2, PopupType.SmallCaution);
			}
			return;
		}
		if (otherAmmo.Rounds == otherAmmo.MaxRounds)
		{
			foreach (EntityUid buckled3 in args.Buckled)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-power-loader-full-ammo", (ValueTuple<string, object>)("ammo", args.Target)), args.Target, buckled3, PopupType.SmallCaution);
			}
			return;
		}
		int roundsToFill = Math.Min(ent.Comp.Rounds, otherAmmo.MaxRounds - otherAmmo.Rounds);
		ent.Comp.Rounds -= roundsToFill;
		otherAmmo.Rounds += roundsToFill;
		_appearance.SetData(Entity<DropshipAmmoComponent>.op_Implicit(ent), (Enum)DropshipAmmoVisuals.Fill, (object)ent.Comp.Rounds, (AppearanceComponent)null);
		_appearance.SetData(args.Target, (Enum)DropshipAmmoVisuals.Fill, (object)otherAmmo.Rounds, (AppearanceComponent)null);
		((EntitySystem)this).Dirty<DropshipAmmoComponent>(ent, (MetaDataComponent)null);
		((EntitySystem)this).Dirty(args.Target, (IComponent)(object)otherAmmo, (MetaDataComponent)null);
		if (ent.Comp.Rounds <= 0)
		{
			if (_net.IsServer)
			{
				((EntitySystem)this).QueueDel((EntityUid?)args.Used);
			}
			_container.TryRemoveFromContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(args.Used), true);
			_powerloader.TrySyncHands(Entity<PowerLoaderComponent>.op_Implicit(args.PowerLoader));
		}
		foreach (EntityUid buckled4 in args.Buckled)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-power-loader-transfer-ammo", (ValueTuple<string, object>)("rounds", roundsToFill), (ValueTuple<string, object>)("ammo", args.Target)), args.Target, buckled4);
		}
	}

	private void OnWeaponsChangeScreenMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsChangeScreenMsg args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (Enum.IsDefined(args.Screen))
		{
			ref DropshipTerminalWeaponsComponent.Screen screen = ref args.First ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo;
			screen.State = args.Screen;
			if (args.Screen == DropshipTerminalWeaponsScreen.StrikeWeapon)
			{
				screen.Weapon = null;
			}
			((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
			RefreshWeaponsUI(ent);
		}
	}

	private void OnWeaponsChooseWeaponMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsChooseWeaponMsg args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? weapon = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(args.Weapon, ref weapon) || !_dropship.IsWeaponAttached(Entity<DropshipWeaponComponent>.op_Implicit(weapon.Value)))
		{
			if (((EntitySystem)this).HasComp<RMCEquipmentDeployerComponent>(weapon))
			{
				EntityUid point = ((EntitySystem)this).Transform(((EntitySystem)this).GetEntity(args.Weapon)).ParentUid;
				SetScreenUtility<RMCEquipmentDeployerComponent>(ent, args.First, DropshipTerminalWeaponsScreen.EquipmentDeployer, ((EntitySystem)this).GetNetEntity(point, (MetaDataComponent)null));
			}
			return;
		}
		ref DropshipTerminalWeaponsComponent.Screen screen = ref args.First ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo;
		screen.Weapon = args.Weapon;
		if (screen.State == DropshipTerminalWeaponsScreen.Equip)
		{
			screen.State = DropshipTerminalWeaponsScreen.SelectingWeapon;
		}
		else if (screen.State == DropshipTerminalWeaponsScreen.StrikeWeapon)
		{
			screen.State = DropshipTerminalWeaponsScreen.Target;
		}
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
		RefreshWeaponsUI(ent);
	}

	private void OnWeaponsChooseMedevacMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsChooseMedevacMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetScreenUtility<MedevacComponent>(ent, args.First, DropshipTerminalWeaponsScreen.Medevac, (NetEntity?)null);
	}

	private void OnWeaponsChooseFultonMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsChooseFultonMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetScreenUtility<RMCFultonComponent>(ent, args.First, DropshipTerminalWeaponsScreen.Fulton, (NetEntity?)null);
	}

	private void OnWeaponsChooseParaDropMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsChooseParaDropMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetScreenUtility<RMCParaDropComponent>(ent, args.First, DropshipTerminalWeaponsScreen.Paradrop, (NetEntity?)null);
	}

	private void OnWeaponsChooseSpotlightMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsChooseSpotlightMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SetScreenUtility<DropshipSpotlightComponent>(ent, args.First, DropshipTerminalWeaponsScreen.Spotlight, (NetEntity?)args.Slot);
	}

	private void OnWeaponsChooseEquipmentDeployerMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsChooseEquipmentDeployerMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SetScreenUtility<RMCEquipmentDeployerComponent>(ent, args.First, DropshipTerminalWeaponsScreen.EquipmentDeployer, args.Slot);
	}

	private void OnWeaponsFireMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsFireMsg args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
		ref DropshipTerminalWeaponsComponent.Screen screen = ref args.First ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo;
		NetEntity? weapon = screen.Weapon;
		if (weapon.HasValue)
		{
			NetEntity netWeapon = weapon.GetValueOrDefault();
			EntityUid? weapon2 = default(EntityUid?);
			DropshipWeaponComponent weaponComp = default(DropshipWeaponComponent);
			if (!((EntitySystem)this).TryGetEntity(netWeapon, ref weapon2) || !((EntitySystem)this).TryComp<DropshipWeaponComponent>(weapon2, ref weaponComp))
			{
				screen.Weapon = null;
				((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
				return;
			}
			Entity<DropshipComponent> dropship = default(Entity<DropshipComponent>);
			if (!CasDebug)
			{
				if (!_dropship.TryGetGridDropship(weapon2.Value, out dropship))
				{
					return;
				}
				FTLComponent ftl = default(FTLComponent);
				if (!((EntitySystem)this).TryComp<FTLComponent>(Entity<DropshipComponent>.op_Implicit(dropship), ref ftl) || (ftl.State != FTLState.Travelling && ftl.State != FTLState.Arriving))
				{
					string msg = base.Loc.GetString("rmc-dropship-weapons-fire-not-flying");
					_popup.PopupCursor(msg, actor, PopupType.SmallCaution);
					return;
				}
			}
			EntityUid? target = ent.Comp.Target;
			if (!target.HasValue)
			{
				return;
			}
			EntityUid target2 = target.GetValueOrDefault();
			if (!IsValidTarget(Entity<DropshipTargetComponent>.op_Implicit(target2)))
			{
				RemovePvsActors(ent);
				SetTarget(ent, null);
				((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
				return;
			}
			EntityCoordinates coordinates = _transform.GetMoverCoordinates(target2).SnapToGrid((IEntityManager?)(object)base.EntityManager, _mapManager);
			if (!CasDebug && !_area.CanCAS(coordinates))
			{
				string msg2 = base.Loc.GetString("rmc-laser-designator-not-cas");
				_popup.PopupCursor(msg2, actor);
			}
			else if (!CasDebug && weaponComp.Skills != null && !_skills.HasSkills(Entity<SkillsComponent>.op_Implicit(actor), weaponComp.Skills))
			{
				string msg3 = base.Loc.GetString("rmc-laser-designator-not-skilled");
				_popup.PopupCursor(msg3, actor);
			}
			else
			{
				if (!CasDebug && !weaponComp.FireInTransport && !((EntitySystem)this).HasComp<DropshipInFlyByComponent>(Entity<DropshipComponent>.op_Implicit(dropship)))
				{
					return;
				}
				TimeSpan time = _timing.CurTime;
				TimeSpan value = time;
				TimeSpan? nextFireAt = weaponComp.NextFireAt;
				Entity<DropshipAmmoComponent> ammo;
				if (value < nextFireAt)
				{
					string msg4 = base.Loc.GetString("rmc-dropship-weapons-fire-cooldown", (ValueTuple<string, object>)("weapon", weapon2));
					_popup.PopupCursor(msg4, actor);
				}
				else if (!TryGetWeaponAmmo(Entity<DropshipWeaponComponent>.op_Implicit((weapon2.Value, weaponComp)), out ammo) || ammo.Comp.Rounds < ammo.Comp.RoundsPerShot)
				{
					string msg5 = base.Loc.GetString("rmc-dropship-weapons-fire-no-ammo", (ValueTuple<string, object>)("weapon", weapon2));
					_popup.PopupCursor(msg5, actor);
				}
				else if (ammo.Comp.Rounds >= ammo.Comp.RoundsPerShot)
				{
					DropshipWeaponShotEvent ev = new DropshipWeaponShotEvent(ammo.Comp.TargetSpread, ammo.Comp.BulletSpread, ammo.Comp.TravelTime, ammo.Comp.RoundsPerShot, ammo.Comp.ShotsPerVolley, ammo.Comp.Damage, ammo.Comp.ArmorPiercing, ammo.Comp.SoundTravelTime, ammo.Comp.SoundCockpit, ammo.Comp.SoundMarker, ammo.Comp.SoundGround, ammo.Comp.SoundImpact, ammo.Comp.SoundWarning, ammo.Comp.MarkerWarning, ammo.Comp.ImpactEffects, ammo.Comp.Explosion, ammo.Comp.Implosion, ammo.Comp.Fire, ammo.Comp.SoundEveryShots);
					((EntitySystem)this).RaiseLocalEvent<DropshipWeaponShotEvent>(Entity<DropshipComponent>.op_Implicit(dropship), ref ev, false);
					ammo.Comp.Rounds -= ammo.Comp.RoundsPerShot;
					_appearance.SetData(Entity<DropshipAmmoComponent>.op_Implicit(ammo), (Enum)DropshipAmmoVisuals.Fill, (object)ammo.Comp.Rounds, (AppearanceComponent)null);
					_powerloader.SyncAppearance(Entity<DropshipWeaponPointComponent>.op_Implicit(((EntitySystem)this).Transform(weapon2.Value).ParentUid));
					((EntitySystem)this).Dirty<DropshipAmmoComponent>(ammo, (MetaDataComponent)null);
					_audio.PlayPvs(ev.SoundCockpit, weapon2.Value, (AudioParams?)null);
					weaponComp.NextFireAt = time + weaponComp.FireDelay;
					((EntitySystem)this).Dirty(weapon2.Value, (IComponent)(object)weaponComp, (MetaDataComponent)null);
					float spread = ev.Spread;
					EntityCoordinates targetCoords = coordinates;
					if (spread != 0f)
					{
						targetCoords = ((EntityCoordinates)(ref targetCoords)).Offset(_random.NextVector2(0f - spread, spread + 1f));
					}
					EntityUid inFlight = ((EntitySystem)this).Spawn((string)null, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
					AmmoInFlightComponent inFlightComp = new AmmoInFlightComponent
					{
						Target = targetCoords,
						MarkerAt = time + ev.TravelTime,
						ShotsLeft = ev.RoundsPerShot,
						ShotsPerVolley = ev.ShotsPerVolley,
						Damage = ev.Damage,
						ArmorPiercing = ev.ArmorPiercing,
						BulletSpread = ev.BulletSpread,
						SoundTravelTime = ev.SoundTravelTime,
						SoundMarker = ev.SoundMarker,
						SoundGround = ev.SoundGround,
						SoundImpact = ev.SoundImpact,
						SoundWarning = ev.SoundWarning,
						MarkerWarning = ev.MarkerWarning,
						WarningMarkerAt = time + TimeSpan.FromSeconds(1L),
						ImpactEffects = ev.ImpactEffect,
						Explosion = ev.Explosion,
						Implosion = ev.Implosion,
						Fire = ev.Fire,
						SoundEveryShots = ev.SoundEveryShots
					};
					((EntitySystem)this).AddComp<AmmoInFlightComponent>(inFlight, inFlightComp, true);
					if (ammo.Comp.DeleteOnEmpty && ammo.Comp.Rounds <= 0)
					{
						((EntitySystem)this).QueueDel((EntityUid?)Entity<DropshipAmmoComponent>.op_Implicit(ammo));
					}
					ISharedAdminLogManager adminLog = _adminLog;
					LogStringHandler handler = new LogStringHandler(11, 3);
					handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "ToPrettyString(args.Actor)");
					handler.AppendLiteral(" fired ");
					handler.AppendFormatted(((EntitySystem)this).ToPrettyString(weapon2, (MetaDataComponent)null), "ToPrettyString(weapon)");
					handler.AppendLiteral(" at ");
					handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target2)), "ToPrettyString(target)");
					adminLog.Add(LogType.RMCDropshipWeapon, ref handler);
				}
			}
		}
		else
		{
			string msg6 = base.Loc.GetString("rmc-dropship-weapons-fire-no-weapon");
			_popup.PopupCursor(msg6, actor, PopupType.SmallCaution);
		}
	}

	private void OnDropshipWeaponOnSpawnFire(Entity<ActivateDropshipWeaponOnSpawnComponent> active, ref MapInitEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		DropshipAmmoComponent ammo = default(DropshipAmmoComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryComp<DropshipAmmoComponent>(Entity<ActivateDropshipWeaponOnSpawnComponent>.op_Implicit(active), ref ammo))
		{
			TimeSpan time = _timing.CurTime;
			EntityUid inFlight = ((EntitySystem)this).Spawn((string)null, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
			AmmoInFlightComponent inFlightComp = new AmmoInFlightComponent
			{
				Target = _transform.GetMoverCoordinates(Entity<ActivateDropshipWeaponOnSpawnComponent>.op_Implicit(active)).SnapToGrid((IEntityManager?)(object)base.EntityManager, _mapManager),
				MarkerAt = time + ammo.TravelTime,
				ShotsLeft = ammo.RoundsPerShot,
				ShotsPerVolley = ammo.ShotsPerVolley,
				Damage = ammo.Damage,
				ArmorPiercing = ammo.ArmorPiercing,
				BulletSpread = ammo.BulletSpread,
				SoundTravelTime = ammo.SoundTravelTime,
				SoundMarker = ammo.SoundMarker,
				SoundGround = ammo.SoundGround,
				SoundImpact = ammo.SoundImpact,
				SoundWarning = ammo.SoundWarning,
				MarkerWarning = ammo.MarkerWarning,
				WarningMarkerAt = time + TimeSpan.FromSeconds(1L),
				ImpactEffects = ammo.ImpactEffects,
				Explosion = ammo.Explosion,
				Implosion = ammo.Implosion,
				Fire = ammo.Fire,
				SoundEveryShots = ammo.SoundEveryShots
			};
			((EntitySystem)this).AddComp<AmmoInFlightComponent>(inFlight, inFlightComp, true);
			((EntitySystem)this).QueueDel((EntityUid?)Entity<ActivateDropshipWeaponOnSpawnComponent>.op_Implicit(active));
		}
	}

	private void OnWeaponsNightVisionMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsNightVisionMsg args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			ent.Comp.NightVision = args.On;
			Entity<DropshipTerminalWeaponsComponent> terminal = ent;
			EntityUid? target = ent.Comp.Target;
			EntityUid? val = EnsureTargetEye(terminal, target.HasValue ? new Entity<DropshipTargetComponent>?(Entity<DropshipTargetComponent>.op_Implicit(target.GetValueOrDefault())) : ((Entity<DropshipTargetComponent>?)null));
			if (val.HasValue)
			{
				EntityUid target2 = val.GetValueOrDefault();
				_eye.SetDrawLight(Entity<EyeComponent>.op_Implicit(target2), !ent.Comp.NightVision);
			}
		}
	}

	private void OnWeaponsExitMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsExitMsg args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		(args.First ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo).State = DropshipTerminalWeaponsScreen.Main;
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
		RefreshWeaponsUI(ent);
	}

	private void OnWeaponsCancelMsg(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsCancelMsg args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		ref DropshipTerminalWeaponsComponent.Screen screen = ref args.First ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo;
		DropshipTerminalWeaponsScreen state = screen.State;
		DropshipTerminalWeaponsScreen state2 = (((uint)(state - 3) > 1u) ? screen.State : DropshipTerminalWeaponsScreen.Target);
		screen.State = state2;
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
		RefreshWeaponsUI(ent);
	}

	private void OnWeaponsAdjustOffset(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsAdjustOffsetMsg args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (args.Direction.IsCardinal())
		{
			Vector2i adjust = DirectionExtensions.ToIntVec(args.Direction);
			Vector2i newOffset = ent.Comp.Offset + adjust;
			Vector2i limit = ent.Comp.OffsetLimit;
			((Vector2i)(ref newOffset))._002Ector(Math.Clamp(newOffset.X, -limit.X, limit.X), Math.Clamp(newOffset.Y, -limit.Y, limit.Y));
			ent.Comp.Offset = newOffset;
			Entity<DropshipTerminalWeaponsComponent> terminal = ent;
			EntityUid? target = ent.Comp.Target;
			EntityUid? val = EnsureTargetEye(terminal, target.HasValue ? new Entity<DropshipTargetComponent>?(Entity<DropshipTargetComponent>.op_Implicit(target.GetValueOrDefault())) : ((Entity<DropshipTargetComponent>?)null));
			if (val.HasValue)
			{
				EntityUid target2 = val.GetValueOrDefault();
				_eye.SetOffset(target2, Vector2i.op_Implicit(ent.Comp.Offset), (EyeComponent)null);
			}
			((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
			RefreshWeaponsUI(ent);
		}
	}

	private void OnWeaponsResetOffset(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsResetOffsetMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Offset = Vector2i.Zero;
		Entity<DropshipTerminalWeaponsComponent> terminal = ent;
		EntityUid? target = ent.Comp.Target;
		EntityUid? val = EnsureTargetEye(terminal, target.HasValue ? new Entity<DropshipTargetComponent>?(Entity<DropshipTargetComponent>.op_Implicit(target.GetValueOrDefault())) : ((Entity<DropshipTargetComponent>?)null));
		if (val.HasValue)
		{
			EntityUid target2 = val.GetValueOrDefault();
			_eye.SetOffset(target2, Vector2i.op_Implicit(ent.Comp.Offset), (EyeComponent)null);
		}
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
		RefreshWeaponsUI(ent);
	}

	private void OnWeaponsTargetsPrevious(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsTargetsPreviousMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.TargetsPage = Math.Max(0, ent.Comp.TargetsPage - 1);
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
		RefreshWeaponsUI(ent);
	}

	private void OnWeaponsTargetsNext(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsTargetsNextMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.TargetsPage = Math.Min(ent.Comp.Targets.Count / 5, ent.Comp.TargetsPage + 1);
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
		RefreshWeaponsUI(ent);
	}

	private void OnWeaponsTargetsSelect(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsTargetsSelectMsg args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(args.Target, ref target) || !((EntitySystem)this).HasComp<DropshipTargetComponent>(target))
		{
			RefreshWeaponsUI(ent);
		}
		else
		{
			UpdateTarget(ent, target.Value);
		}
	}

	private void OnWeaponsMedevacPrevious(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsMedevacPreviousMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.MedevacsPage = Math.Max(0, ent.Comp.MedevacsPage - 1);
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
		RefreshWeaponsUI(ent);
	}

	private void OnWeaponsMedevacNext(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsMedevacNextMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.MedevacsPage = Math.Min(ent.Comp.Medevacs.Count % 5, ent.Comp.MedevacsPage + 1);
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
		RefreshWeaponsUI(ent);
	}

	private void OnWeaponsMedevacSelect(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsMedevacSelectMsg args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(args.Target, ref target) || !((EntitySystem)this).HasComp<MedevacStretcherComponent>(target))
		{
			RefreshWeaponsUI(ent);
			return;
		}
		UpdateTarget(ent, target.Value);
		_popup.PopupClient("You move your dropship above the selected stretcher's beacon. You can now manually activate the medevac system to hoist the patient up.", ((BaseBoundUserInterfaceEvent)args).Actor);
	}

	private void OnWeaponsFultonPrevious(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsFultonPreviousMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.FultonsPage = Math.Max(0, ent.Comp.FultonsPage - 1);
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
		RefreshWeaponsUI(ent);
	}

	private void OnWeaponsFultonNext(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsFultonNextMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.FultonsPage = Math.Min(ent.Comp.Fultons.Count % 5, ent.Comp.FultonsPage + 1);
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
		RefreshWeaponsUI(ent);
	}

	private void OnWeaponsFultonSelect(Entity<DropshipTerminalWeaponsComponent> ent, ref DropshipTerminalWeaponsFultonSelectMsg args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid? target = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(args.Target, ref target) || !((EntitySystem)this).HasComp<RMCActiveFultonComponent>(target))
		{
			RefreshWeaponsUI(ent);
		}
		else
		{
			if (!_dropship.TryGetGridDropship(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropship) || dropship.Comp.AttachmentPoints.Count == 0)
			{
				return;
			}
			DropshipUtilityPointComponent utilityComp = default(DropshipUtilityPointComponent);
			BaseContainer container = default(BaseContainer);
			DropshipUtilityComponent utility = default(DropshipUtilityComponent);
			foreach (EntityUid point in dropship.Comp.AttachmentPoints)
			{
				if (!((EntitySystem)this).TryComp<DropshipUtilityPointComponent>(point, ref utilityComp) || !_container.TryGetContainer(point, utilityComp.UtilitySlotId, ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count == 0)
				{
					continue;
				}
				EntityUid contained = container.ContainedEntities[0];
				if (((EntitySystem)this).HasComp<RMCFultonComponent>(contained))
				{
					if (!((EntitySystem)this).TryComp<DropshipUtilityComponent>(contained, ref utility) || _dropshipUtility.IsActivatable(Entity<DropshipUtilityComponent>.op_Implicit((contained, utility)), ((BaseBoundUserInterfaceEvent)args).Actor, out string popup))
					{
						((EntitySystem)this).RemComp<DropshipTargetComponent>(target.Value);
						((EntitySystem)this).RemCompDeferred<RMCActiveFultonComponent>(target.Value);
						_transform.PlaceNextTo(Entity<TransformComponent>.op_Implicit(target.Value), Entity<TransformComponent>.op_Implicit(point));
						RefreshWeaponsUI(ent);
						return;
					}
					_popup.PopupCursor(popup, ((BaseBoundUserInterfaceEvent)args).Actor);
				}
			}
			RefreshWeaponsUI(ent);
		}
	}

	private void OnWeaponsParaDropSelect(Entity<DropshipTerminalWeaponsComponent> ent, ref DropShipTerminalWeaponsParaDropTargetSelectMsg args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		if (!ent.Comp.Target.HasValue)
		{
			if (_net.IsClient)
			{
				string msg = base.Loc.GetString("rmc-dropship-paradrop-lock-no-target");
				_popup.PopupCursor(msg, ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.SmallCaution);
			}
			RefreshWeaponsUI(ent);
		}
		else
		{
			if (!_dropship.TryGetGridDropship(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropship) || dropship.Comp.AttachmentPoints.Count == 0)
			{
				return;
			}
			FTLComponent ftl = default(FTLComponent);
			if (!CasDebug && (!((EntitySystem)this).TryComp<FTLComponent>(Entity<DropshipComponent>.op_Implicit(dropship), ref ftl) || ftl.State != FTLState.Travelling))
			{
				if (_net.IsClient)
				{
					string msg2 = base.Loc.GetString("rmc-dropship-paradrop-lock-target-not-flying");
					_popup.PopupCursor(msg2, ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.SmallCaution);
				}
				return;
			}
			if (!IsValidTarget(Entity<DropshipTargetComponent>.op_Implicit(ent.Comp.Target.Value)))
			{
				RemovePvsActors(ent);
				SetTarget(ent, null);
				((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
				return;
			}
			EntityCoordinates coordinates = _transform.GetMoverCoordinates(ent.Comp.Target.Value);
			if (!CasDebug && !_area.CanCAS(coordinates))
			{
				if (_net.IsClient)
				{
					string msg3 = base.Loc.GetString("rmc-laser-designator-not-cas");
					_popup.PopupCursor(msg3, ((BaseBoundUserInterfaceEvent)args).Actor);
				}
				return;
			}
			if (args.On)
			{
				TransformChildrenEnumerator enumerator = ((EntitySystem)this).Transform(Entity<DropshipComponent>.op_Implicit(dropship)).ChildEnumerator;
				EntityUid child = default(EntityUid);
				DoorComponent door = default(DoorComponent);
				DoorBoltComponent doorBolt = default(DoorBoltComponent);
				while (((TransformChildrenEnumerator)(ref enumerator)).MoveNext(ref child))
				{
					if (((EntitySystem)this).TryComp<DoorComponent>(child, ref door) && door.Location == DoorLocation.Aft && ((EntitySystem)this).TryComp<DoorBoltComponent>(child, ref doorBolt) && door.State != DoorState.Open)
					{
						_door.StartOpening(child);
						_door.TrySetBoltDown(Entity<DoorBoltComponent>.op_Implicit((child, doorBolt)), value: true);
					}
				}
				ActiveParaDropComponent paraDrop = ((EntitySystem)this).EnsureComp<ActiveParaDropComponent>(Entity<DropshipComponent>.op_Implicit(dropship));
				paraDrop.DropTarget = ent.Comp.Target;
				((EntitySystem)this).Dirty(Entity<DropshipComponent>.op_Implicit(dropship), (IComponent)(object)paraDrop, (MetaDataComponent)null);
			}
			else
			{
				((EntitySystem)this).RemComp<ActiveParaDropComponent>(Entity<DropshipComponent>.op_Implicit(dropship));
			}
			RefreshWeaponsUI(ent);
		}
	}

	private void OnWeaponsSpotlightSelect(Entity<DropshipTerminalWeaponsComponent> ent, ref DropShipTerminalWeaponsSpotlightToggleMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? selectedSystem = ((EntitySystem)this).GetEntity(ent.Comp.SelectedSystem);
		DropshipSpotlightComponent spotlight = default(DropshipSpotlightComponent);
		if (_dropship.TryGetGridDropship(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropship) && dropship.Comp.AttachmentPoints.Count != 0 && ((EntitySystem)this).TryComp<DropshipSpotlightComponent>(selectedSystem, ref spotlight))
		{
			EntityUid systemPoint = ((EntitySystem)this).Transform(selectedSystem.Value).ParentUid;
			_pointLight.SetEnabled(systemPoint, args.On, (SharedPointLightComponent)null, (MetaDataComponent)null);
			_appearance.SetData(systemPoint, (Enum)DropshipUtilityVisuals.State, (object)(args.On ? "spotlights_on" : "spotlights_off"), (AppearanceComponent)null);
			spotlight.Enabled = args.On;
			((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
			RefreshWeaponsUI(ent);
		}
	}

	private void OnEquipmentDeploy(Entity<DropshipTerminalWeaponsComponent> ent, ref DropShipTerminalWeaponsEquipmentDeployToggleMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? selectedSystem = ((EntitySystem)this).GetEntity(ent.Comp.SelectedSystem);
		if (_dropship.TryGetGridDropship(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropship) && dropship.Comp.AttachmentPoints.Count != 0 && selectedSystem.HasValue && _equipmentDeployer.TryGetContainer(selectedSystem.Value, out BaseContainer container))
		{
			Vector2 deployOffset = default(Vector2);
			float rotationOffset = 0f;
			DropshipWeaponPointComponent weaponPoint = default(DropshipWeaponPointComponent);
			if (((EntitySystem)this).TryComp<DropshipWeaponPointComponent>(selectedSystem, ref weaponPoint))
			{
				_equipmentDeployer.TryGetOffset(container.ContainedEntities[0], out deployOffset, out rotationOffset, weaponPoint.Location);
			}
			_equipmentDeployer.TryDeploy(container.ContainedEntities[0], args.Deploy, deployOffset, rotationOffset, null, ((BaseBoundUserInterfaceEvent)args).Actor);
			RefreshWeaponsUI(ent);
		}
	}

	private void OnEquipmentToggleAutoDeploy(Entity<DropshipTerminalWeaponsComponent> ent, ref DropShipTerminalWeaponsEquipmentAutoDeployToggleMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? selectedSystem = ((EntitySystem)this).GetEntity(ent.Comp.SelectedSystem);
		if (_dropship.TryGetGridDropship(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropship) && dropship.Comp.AttachmentPoints.Count != 0 && selectedSystem.HasValue && _equipmentDeployer.TryGetContainer(selectedSystem.Value, out BaseContainer container))
		{
			_equipmentDeployer.SetAutoDeploy(container.ContainedEntities[0], args.AutoDeploy);
			RefreshWeaponsUI(ent);
		}
	}

	private void OnRefreshNameModifier(Entity<ActiveFlareSignalComponent> ent, ref RefreshNameModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Abbreviation != null)
		{
			args.AddModifier(LocId.op_Implicit(ent.Comp.Abbreviation), 0);
		}
	}

	private void UpdateTarget(Entity<DropshipTerminalWeaponsComponent> ent, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		RemovePvsActors(ent);
		SetTarget(ent, target);
		Entity<DropshipTerminalWeaponsComponent> terminal = ent;
		EntityUid? target2 = ent.Comp.Target;
		EntityUid? val = EnsureTargetEye(terminal, target2.HasValue ? new Entity<DropshipTargetComponent>?(Entity<DropshipTargetComponent>.op_Implicit(target2.GetValueOrDefault())) : ((Entity<DropshipTargetComponent>?)null));
		if (val.HasValue)
		{
			EntityUid targetEye = val.GetValueOrDefault();
			_eye.SetOffset(targetEye, Vector2i.op_Implicit(ent.Comp.Offset), (EyeComponent)null);
			_eye.SetDrawLight(Entity<EyeComponent>.op_Implicit(targetEye), !ent.Comp.NightVision);
		}
		AddPvsActors(ent);
		RefreshWeaponsUI(ent);
		((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
	}

	protected virtual void RefreshWeaponsUI(Entity<DropshipTerminalWeaponsComponent> terminal)
	{
	}

	public bool TryGetWeaponAmmo(Entity<DropshipWeaponComponent?> weapon, out Entity<DropshipAmmoComponent> ammo)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		ammo = default(Entity<DropshipAmmoComponent>);
		if (!((EntitySystem)this).Resolve<DropshipWeaponComponent>(Entity<DropshipWeaponComponent>.op_Implicit(weapon), ref weapon.Comp, false))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		DropshipWeaponPointComponent point = default(DropshipWeaponPointComponent);
		BaseContainer ammoContainer = default(BaseContainer);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<DropshipWeaponComponent>.op_Implicit(weapon), null)), ref container) || !((EntitySystem)this).TryComp<DropshipWeaponPointComponent>(container.Owner, ref point) || !_container.TryGetContainer(container.Owner, point.AmmoContainerSlotId, ref ammoContainer, (ContainerManagerComponent)null))
		{
			return false;
		}
		DropshipAmmoComponent ammoComp = default(DropshipAmmoComponent);
		foreach (EntityUid contained in ammoContainer.ContainedEntities)
		{
			if (((EntitySystem)this).TryComp<DropshipAmmoComponent>(contained, ref ammoComp))
			{
				ammo = Entity<DropshipAmmoComponent>.op_Implicit((contained, ammoComp));
				return true;
			}
		}
		return false;
	}

	public int GetWeaponRounds(Entity<DropshipWeaponComponent?> weapon)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetWeaponAmmo(weapon, out Entity<DropshipAmmoComponent> ammo))
		{
			return ammo.Comp.Rounds;
		}
		return 0;
	}

	private bool IsValidTarget(Entity<DropshipTargetComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DropshipTargetComponent>(Entity<DropshipTargetComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(Entity<DropshipTargetComponent>.op_Implicit(ent));
		if (!CasDebug && !((EntitySystem)this).HasComp<RMCPlanetComponent>(xform.GridUid))
		{
			return false;
		}
		if (!ent.Comp.IsTargetableByWeapons)
		{
			return false;
		}
		return true;
	}

	public string GetUserAbbreviation(EntityUid user, int id)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		string abbreviation = base.Loc.GetString("rmc-laser-designator-target-abbreviation", (ValueTuple<string, object>)("id", id));
		if (_squad.TryGetMemberSquad(Entity<SquadMemberComponent>.op_Implicit(user), out Entity<SquadTeamComponent> squad))
		{
			string squadName = ((EntitySystem)this).Name(Entity<SquadTeamComponent>.op_Implicit(squad), (MetaDataComponent)null);
			if (!string.IsNullOrWhiteSpace(squadName) && squadName.Length > 0)
			{
				squadName = $"{squadName[0]}";
			}
			abbreviation = base.Loc.GetString("rmc-laser-designator-target-abbreviation-squad", (ValueTuple<string, object>)("letter", squadName), (ValueTuple<string, object>)("id", id));
		}
		return abbreviation;
	}

	protected virtual void AddPvs(Entity<DropshipTerminalWeaponsComponent> terminal, Entity<ActorComponent?> actor)
	{
	}

	protected virtual void RemovePvs(Entity<DropshipTerminalWeaponsComponent> terminal, Entity<ActorComponent?> actor)
	{
	}

	private void AddPvsActors(Entity<DropshipTerminalWeaponsComponent> terminal)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid actor in _ui.GetActors(Entity<UserInterfaceComponent>.op_Implicit(terminal.Owner), (Enum)DropshipTerminalWeaponsUi.Key))
		{
			AddPvs(terminal, Entity<ActorComponent>.op_Implicit(actor));
		}
	}

	private void RemovePvsActors(Entity<DropshipTerminalWeaponsComponent> terminal)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid actor in _ui.GetActors(Entity<UserInterfaceComponent>.op_Implicit(terminal.Owner), (Enum)DropshipTerminalWeaponsUi.Key))
		{
			RemovePvs(terminal, Entity<ActorComponent>.op_Implicit(actor));
		}
	}

	private void StartTrackingActiveFlare(Entity<FlareSignalComponent> ent, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		ActiveFlareSignalComponent active = default(ActiveFlareSignalComponent);
		if (!((EntitySystem)this).EnsureComp<ActiveFlareSignalComponent>(Entity<FlareSignalComponent>.op_Implicit(ent), ref active) && !_net.IsClient)
		{
			int id = ComputeNextId();
			active.Abbreviation = base.Loc.GetString("rmc-laser-designator-target-abbreviation", (ValueTuple<string, object>)("id", id));
			if (user.HasValue)
			{
				active.Abbreviation = GetUserAbbreviation(user.Value, id);
			}
		}
	}

	private bool IsFlareLit(EntityUid flare)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ExpendableLightComponent expendable = default(ExpendableLightComponent);
		if (((EntitySystem)this).TryComp<ExpendableLightComponent>(flare, ref expendable))
		{
			return expendable.Activated;
		}
		return false;
	}

	private bool TryActivateSignalFlareTarget(Entity<ActiveFlareSignalComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<DropshipTargetComponent>(Entity<ActiveFlareSignalComponent>.op_Implicit(ent)))
		{
			return true;
		}
		if (!IsFlareLit(Entity<ActiveFlareSignalComponent>.op_Implicit(ent)))
		{
			return false;
		}
		if (ent.Comp.Abbreviation == null)
		{
			return false;
		}
		DropshipTargetComponent target = new DropshipTargetComponent
		{
			Abbreviation = ent.Comp.Abbreviation
		};
		((EntitySystem)this).AddComp<DropshipTargetComponent>(Entity<ActiveFlareSignalComponent>.op_Implicit(ent), target, true);
		((EntitySystem)this).Dirty(Entity<ActiveFlareSignalComponent>.op_Implicit(ent), (IComponent)(object)target, (MetaDataComponent)null);
		_name.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
		_physics.SetBodyType(Entity<ActiveFlareSignalComponent>.op_Implicit(ent), (BodyType)4, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
		return true;
	}

	public int ComputeNextId()
	{
		return _nextId++;
	}

	public void MakeDropshipTarget(EntityUid ent, string abbreviation)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		DropshipTargetComponent dropshipTarget = new DropshipTargetComponent
		{
			Abbreviation = abbreviation
		};
		((EntitySystem)this).AddComp<DropshipTargetComponent>(ent, dropshipTarget, true);
		((EntitySystem)this).Dirty(ent, (IComponent)(object)dropshipTarget, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_086a: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<ActiveFlareSignalComponent, TransformComponent> activeFlares = ((EntitySystem)this).EntityQueryEnumerator<ActiveFlareSignalComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		ActiveFlareSignalComponent active = default(ActiveFlareSignalComponent);
		TransformComponent xform = default(TransformComponent);
		while (activeFlares.MoveNext(ref uid, ref active, ref xform))
		{
			active.LastCoordinates.Enqueue(((EntitySystem)this).GetNetCoordinates(xform.Coordinates, (MetaDataComponent)null));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)active, (MetaDataComponent)null);
			if (active.LastCoordinates.Count < 10)
			{
				continue;
			}
			active.LastCoordinates.Dequeue();
			bool all = true;
			foreach (NetCoordinates last in active.LastCoordinates)
			{
				if (!_transform.InRange(((EntitySystem)this).GetCoordinates(last), xform.Coordinates, 0.01f))
				{
					all = false;
					break;
				}
			}
			if (all && TryActivateSignalFlareTarget(Entity<ActiveFlareSignalComponent>.op_Implicit((uid, active))))
			{
				((EntitySystem)this).RemCompDeferred<ActiveFlareSignalComponent>(uid);
			}
		}
		EntityQueryEnumerator<AmmoInFlightComponent> inFlight = ((EntitySystem)this).EntityQueryEnumerator<AmmoInFlightComponent>();
		EntityUid uid2 = default(EntityUid);
		AmmoInFlightComponent flight = default(AmmoInFlightComponent);
		while (inFlight.MoveNext(ref uid2, ref flight))
		{
			if (_net.IsClient)
			{
				continue;
			}
			if (!flight.WarnedSound)
			{
				flight.WarnedSound = true;
				((EntitySystem)this).Dirty(uid2, (IComponent)(object)flight, (MetaDataComponent)null);
				if (flight.SoundWarning != null)
				{
					_audio.PlayPvs(flight.SoundWarning, flight.Target, (AudioParams?)null);
				}
			}
			if (time >= flight.WarningMarkerAt && !flight.WarnedMarker)
			{
				flight.WarnedMarker = true;
				((EntitySystem)this).Dirty(uid2, (IComponent)(object)flight, (MetaDataComponent)null);
				if (flight.MarkerWarning)
				{
					flight.WarningMarker = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(DropshipTargetMarker), flight.Target);
					((EntitySystem)this).EnsureComp<TimedDespawnComponent>(flight.WarningMarker.Value).Lifetime = (float)(flight.MarkerAt - _timing.CurTime).TotalSeconds;
				}
			}
			if (flight.MarkerAt > time)
			{
				continue;
			}
			if (!flight.SpawnedMarker)
			{
				flight.SpawnedMarker = true;
				flight.Marker = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(DropshipTargetMarker), flight.Target);
				((EntitySystem)this).Dirty(uid2, (IComponent)(object)flight, (MetaDataComponent)null);
				_audio.PlayPvs(flight.SoundMarker, flight.Marker.Value, (AudioParams?)null);
				if (_net.IsServer)
				{
					((EntitySystem)this).QueueDel(flight.WarningMarker);
				}
				flight.WarningMarker = null;
			}
			if (flight.MarkerAt.Add(TimeSpan.FromSeconds(1L)) > time)
			{
				continue;
			}
			if (flight.Marker.HasValue)
			{
				if (_net.IsServer)
				{
					((EntitySystem)this).QueueDel(flight.Marker);
				}
				flight.Marker = null;
				((EntitySystem)this).Dirty(uid2, (IComponent)(object)flight, (MetaDataComponent)null);
			}
			if (flight.NextShot > time)
			{
				continue;
			}
			if (flight.ShotsLeft > 0)
			{
				flight.ShotsLeft -= flight.ShotsPerVolley;
				flight.NextShot = time + flight.ShotDelay;
				flight.SoundShotsLeft--;
				((EntitySystem)this).Dirty(uid2, (IComponent)(object)flight, (MetaDataComponent)null);
				Vector2 spread = Vector2.Zero;
				if (flight.BulletSpread > 0)
				{
					spread = _random.NextVector2((float)(-flight.BulletSpread), (float)(flight.BulletSpread + 1));
				}
				MapCoordinates val = _transform.ToMapCoordinates(flight.Target, true);
				MapCoordinates target = ((MapCoordinates)(ref val)).Offset(spread);
				foreach (EntProtoId effect in flight.ImpactEffects)
				{
					((EntitySystem)this).Spawn(EntProtoId.op_Implicit(effect), target, (ComponentRegistry)null, _random.NextAngle());
				}
				if (flight.Damage != null)
				{
					_damageables.Clear();
					_entityLookup.GetEntitiesInRange<DamageableComponent>(target, 0.49f, _damageables, (LookupFlags)78);
					foreach (Entity<DamageableComponent> damageable in _damageables)
					{
						DamageableSystem damageable2 = _damageable;
						EntityUid? uid3 = Entity<DamageableComponent>.op_Implicit(damageable);
						DamageSpecifier? damage = flight.Damage;
						DamageableComponent damageable3 = Entity<DamageableComponent>.op_Implicit(damageable);
						int armorPiercing = flight.ArmorPiercing;
						damageable2.TryChangeDamage(uid3, damage, ignoreResistances: false, interruptsDoAfters: true, damageable3, null, null, armorPiercing);
					}
				}
				if (flight.Implosion != null)
				{
					_rmcImplosion.Implode(flight.Implosion, target);
				}
				if (flight.Fire != null)
				{
					Entity<CollideChainComponent> chain = _onCollide.SpawnChain();
					int? total = flight.Fire.Total;
					bool cont;
					if (total.HasValue)
					{
						int total2 = total.GetValueOrDefault();
						List<Vector2i> tiles = new List<Vector2i>();
						for (int x = -flight.Fire.Range; x <= flight.Fire.Range; x++)
						{
							for (int y = -flight.Fire.Range; y <= flight.Fire.Range; y++)
							{
								tiles.Add(Vector2i.op_Implicit((x, y)));
							}
						}
						for (int i = 0; i < total2; i++)
						{
							if (tiles.Count == 0)
							{
								break;
							}
							Vector2i tile = RandomExtensions.PickAndTake<Vector2i>(_random, (IList<Vector2i>)tiles);
							EntityCoordinates coords = ((EntityCoordinates)(ref flight.Target)).Offset(Vector2i.op_Implicit(tile));
							_rmcFlammable.SpawnFire(coords, flight.Fire.Type, Entity<CollideChainComponent>.op_Implicit(chain), flight.Fire.Range, flight.Fire.Intensity, flight.Fire.Duration, out cont);
						}
					}
					else
					{
						_rmcFlammable.SpawnFireLines(flight.Fire.Type, flight.Target, flight.Fire.CardinalRange, flight.Fire.OrdinalRange, flight.Fire.Intensity, flight.Fire.Duration);
						for (int j = -flight.Fire.Range; j <= flight.Fire.Range; j++)
						{
							for (int k = -flight.Fire.Range; k <= flight.Fire.Range; k++)
							{
								EntityCoordinates coords2 = ((EntityCoordinates)(ref flight.Target)).Offset(new Vector2(j, k));
								_rmcFlammable.SpawnFire(coords2, flight.Fire.Type, Entity<CollideChainComponent>.op_Implicit(chain), flight.Fire.Range, flight.Fire.Intensity, flight.Fire.Duration, out cont);
							}
						}
					}
				}
				if (flight.Explosion != null)
				{
					_rmcExplosion.QueueExplosion(target, ProtoId<ExplosionPrototype>.op_Implicit(flight.Explosion.Type), flight.Explosion.Total, flight.Explosion.Slope, flight.Explosion.Max, uid2, 1f, int.MaxValue, canCreateVacuum: false);
				}
				if (flight.SoundShotsLeft <= 0)
				{
					flight.SoundShotsLeft = flight.SoundEveryShots;
					_audio.PlayPvs(flight.SoundImpact, flight.Target, (AudioParams?)null);
				}
				continue;
			}
			AmmoInFlightComponent ammoInFlightComponent = flight;
			TimeSpan valueOrDefault = ammoInFlightComponent.PlayGroundSoundAt.GetValueOrDefault();
			if (!ammoInFlightComponent.PlayGroundSoundAt.HasValue)
			{
				valueOrDefault = _timing.CurTime + flight.SoundTravelTime;
				ammoInFlightComponent.PlayGroundSoundAt = valueOrDefault;
			}
			((EntitySystem)this).Dirty(uid2, (IComponent)(object)flight, (MetaDataComponent)null);
			valueOrDefault = time;
			TimeSpan? playGroundSoundAt = flight.PlayGroundSoundAt;
			if (valueOrDefault >= playGroundSoundAt)
			{
				_audio.PlayPvs(flight.SoundGround, flight.Target, (AudioParams?)null);
				if (_net.IsServer)
				{
					((EntitySystem)this).QueueDel((EntityUid?)uid2);
				}
			}
		}
		EntityQueryEnumerator<ActiveLaserDesignatorComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActiveLaserDesignatorComponent, TransformComponent>();
		EntityUid uid4 = default(EntityUid);
		ActiveLaserDesignatorComponent active2 = default(ActiveLaserDesignatorComponent);
		TransformComponent xform2 = default(TransformComponent);
		while (query.MoveNext(ref uid4, ref active2, ref xform2))
		{
			if (!_transform.InRange(xform2.Coordinates, active2.Origin, active2.BreakRange))
			{
				((EntitySystem)this).RemCompDeferred<ActiveLaserDesignatorComponent>(uid4);
			}
		}
	}

	public void TargetUpdated(Entity<DropshipTargetComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<DropshipTerminalWeaponsComponent> terminals = ((EntitySystem)this).EntityQueryEnumerator<DropshipTerminalWeaponsComponent>();
		EntityUid uid = default(EntityUid);
		DropshipTerminalWeaponsComponent terminal = default(DropshipTerminalWeaponsComponent);
		while (terminals.MoveNext(ref uid, ref terminal))
		{
			Span<DropshipTerminalWeaponsComponent.TargetEnt> span = CollectionsMarshal.AsSpan(terminal.Medevacs);
			for (int i = 0; i < span.Length; i++)
			{
				ref DropshipTerminalWeaponsComponent.TargetEnt target = ref span[i];
				if (!(target.Id != ((EntitySystem)this).GetNetEntity(ent.Owner, (MetaDataComponent)null)))
				{
					target = target with
					{
						Name = ent.Comp.Abbreviation
					};
					break;
				}
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)terminal, (MetaDataComponent)null);
		}
	}

	private EntityUid? EnsureTargetEye(Entity<DropshipTerminalWeaponsComponent> terminal, Entity<DropshipTargetComponent?>? targetNullable)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (!targetNullable.HasValue)
		{
			return null;
		}
		Entity<DropshipTargetComponent> target = targetNullable.Value;
		if (!((EntitySystem)this).Resolve<DropshipTargetComponent>(Entity<DropshipTargetComponent>.op_Implicit(target), ref target.Comp, false))
		{
			return null;
		}
		if (!target.Comp.Eyes.TryGetValue(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(terminal), out var eye))
		{
			if (_net.IsClient)
			{
				return null;
			}
			eye = ((EntitySystem)this).Spawn((string)null, target.Owner.ToCoordinates());
			target.Comp.Eyes[Entity<DropshipTerminalWeaponsComponent>.op_Implicit(terminal)] = eye;
			((EntitySystem)this).Dirty<DropshipTargetComponent>(target, (MetaDataComponent)null);
			EyeComponent eyeComp = ((EntitySystem)this).EnsureComp<EyeComponent>(eye);
			_eye.SetDrawFov(eye, false, eyeComp);
			DropshipTargetEyeComponent targetEyeComp = ((EntitySystem)this).EnsureComp<DropshipTargetEyeComponent>(eye);
			targetEyeComp.Target = Entity<DropshipTargetComponent>.op_Implicit(target);
			((EntitySystem)this).Dirty(eye, (IComponent)(object)targetEyeComp, (MetaDataComponent)null);
		}
		return eye;
	}

	public bool TryGetTargetEye(Entity<DropshipTerminalWeaponsComponent> terminal, Entity<DropshipTargetComponent?> target, out EntityUid eye)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DropshipTargetComponent>(Entity<DropshipTargetComponent>.op_Implicit(target), ref target.Comp, false) && target.Comp.Eyes.TryGetValue(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(terminal), out eye))
		{
			return true;
		}
		eye = default(EntityUid);
		return false;
	}

	public void MakeTarget(EntityUid target, string abbreviation, bool targetableByWeapons)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		DropshipTargetComponent targetComp = new DropshipTargetComponent
		{
			Abbreviation = abbreviation,
			IsTargetableByWeapons = targetableByWeapons
		};
		((EntitySystem)this).AddComp<DropshipTargetComponent>(target, targetComp, true);
	}

	private void SetScreenUtility<T>(Entity<DropshipTerminalWeaponsComponent> ent, bool first, DropshipTerminalWeaponsScreen state, NetEntity? selected = null) where T : IComponent
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if (!_dropship.TryGetGridDropship(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropship) || dropship.Comp.AttachmentPoints.Count == 0)
		{
			return;
		}
		bool hasUtility = false;
		bool hasElectronicSystem = false;
		bool hasWeaponSystem = false;
		DropshipElectronicSystemPointComponent electronicSystemComp = default(DropshipElectronicSystemPointComponent);
		BaseContainer electronicSystemContainer = default(BaseContainer);
		DropshipWeaponPointComponent weaponPointComponent = default(DropshipWeaponPointComponent);
		BaseContainer weaponContainer = default(BaseContainer);
		DropshipUtilityPointComponent utilityComp = default(DropshipUtilityPointComponent);
		BaseContainer container = default(BaseContainer);
		foreach (EntityUid point in dropship.Comp.AttachmentPoints)
		{
			if (((EntitySystem)this).TryComp<DropshipElectronicSystemPointComponent>(point, ref electronicSystemComp) && _container.TryGetContainer(point, electronicSystemComp.ContainerId, ref electronicSystemContainer, (ContainerManagerComponent)null) && electronicSystemContainer.ContainedEntities.Count > 0)
			{
				foreach (EntityUid contained in electronicSystemContainer.ContainedEntities)
				{
					if (((EntitySystem)this).HasComp<T>(contained))
					{
						hasElectronicSystem = true;
						break;
					}
				}
			}
			if (((EntitySystem)this).TryComp<DropshipWeaponPointComponent>(point, ref weaponPointComponent) && _container.TryGetContainer(point, weaponPointComponent.WeaponContainerSlotId, ref weaponContainer, (ContainerManagerComponent)null) && weaponContainer.ContainedEntities.Count > 0)
			{
				foreach (EntityUid contained2 in weaponContainer.ContainedEntities)
				{
					if (((EntitySystem)this).HasComp<T>(contained2))
					{
						hasWeaponSystem = true;
						break;
					}
				}
			}
			if (!((EntitySystem)this).TryComp<DropshipUtilityPointComponent>(point, ref utilityComp) || !_container.TryGetContainer(point, utilityComp.UtilitySlotId, ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count == 0)
			{
				continue;
			}
			foreach (EntityUid contained3 in container.ContainedEntities)
			{
				if (((EntitySystem)this).HasComp<T>(contained3))
				{
					hasUtility = true;
					break;
				}
			}
		}
		if (hasUtility || hasElectronicSystem || hasWeaponSystem)
		{
			(first ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo).State = state;
			ent.Comp.SelectedSystem = selected;
			((EntitySystem)this).Dirty<DropshipTerminalWeaponsComponent>(ent, (MetaDataComponent)null);
			RefreshWeaponsUI(ent);
		}
	}
}
