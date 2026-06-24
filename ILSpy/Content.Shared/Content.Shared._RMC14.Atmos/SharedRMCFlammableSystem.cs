using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.OnCollide;
using Content.Shared._RMC14.Water;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Alert;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Directions;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Maps;
using Content.Shared.Paper;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Atmos;

public abstract class SharedRMCFlammableSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private IMapManager _map;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedOnCollideSystem _onCollide;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private RMCReagentSystem _reagents;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private CMArmorSystem _armor;

	[Dependency]
	private XenoPlasmaSystem _plasma;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	private static readonly ProtoId<AlertPrototype> FireAlert = ProtoId<AlertPrototype>.op_Implicit("Fire");

	private static readonly ProtoId<ReagentPrototype> WaterReagent = ProtoId<ReagentPrototype>.op_Implicit("Water");

	private static readonly ProtoId<TagPrototype> StructureTag = ProtoId<TagPrototype>.op_Implicit("Structure");

	private static readonly ProtoId<TagPrototype> WallTag = ProtoId<TagPrototype>.op_Implicit("Wall");

	private static readonly ProtoId<DamageTypePrototype> HeatDamage = ProtoId<DamageTypePrototype>.op_Implicit("Heat");

	private EntityQuery<BlockTileFireComponent> _blockTileFireQuery;

	private EntityQuery<DoorComponent> _doorQuery;

	private EntityQuery<FlammableComponent> _flammableQuery;

	private EntityQuery<RMCIgniteOnCollideComponent> _igniteOnCollideQuery;

	private EntityQuery<ProjectileComponent> _projectileQuery;

	private EntityQuery<TileFireComponent> _tileFireQuery;

	private EntityQuery<InventoryComponent> _inventoryQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		_blockTileFireQuery = ((EntitySystem)this).GetEntityQuery<BlockTileFireComponent>();
		_doorQuery = ((EntitySystem)this).GetEntityQuery<DoorComponent>();
		_flammableQuery = ((EntitySystem)this).GetEntityQuery<FlammableComponent>();
		_igniteOnCollideQuery = ((EntitySystem)this).GetEntityQuery<RMCIgniteOnCollideComponent>();
		_projectileQuery = ((EntitySystem)this).GetEntityQuery<ProjectileComponent>();
		_tileFireQuery = ((EntitySystem)this).GetEntityQuery<TileFireComponent>();
		_inventoryQuery = ((EntitySystem)this).GetEntityQuery<InventoryComponent>();
		((EntitySystem)this).SubscribeLocalEvent<IgniteOnProjectileHitComponent, ProjectileHitEvent>((EntityEventRefHandler<IgniteOnProjectileHitComponent, ProjectileHitEvent>)OnIgniteOnProjectileHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TileFireComponent, MapInitEvent>((EntityEventRefHandler<TileFireComponent, MapInitEvent>)OnTileFireMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TileFireComponent, VaporHitEvent>((EntityEventRefHandler<TileFireComponent, VaporHitEvent>)OnTileFireVaporHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TileFireComponent, InteractHandEvent>((EntityEventRefHandler<TileFireComponent, InteractHandEvent>)OnTileFireInteractHand, new Type[1] { typeof(InteractionPopupSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TileFireComponent, PreventCollideEvent>((EntityEventRefHandler<TileFireComponent, PreventCollideEvent>)OnTileFirePreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CraftsIntoMolotovComponent, ExaminedEvent>((EntityEventRefHandler<CraftsIntoMolotovComponent, ExaminedEvent>)OnCraftsIntoMolotovExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CraftsIntoMolotovComponent, InteractUsingEvent>((EntityEventRefHandler<CraftsIntoMolotovComponent, InteractUsingEvent>)OnCraftsIntoMolotovInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CraftsIntoMolotovComponent, CraftMolotovDoAfterEvent>((EntityEventRefHandler<CraftsIntoMolotovComponent, CraftMolotovDoAfterEvent>)OnCraftsIntoMolotovDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TileFireOnTriggerComponent, RMCTriggerEvent>((EntityEventRefHandler<TileFireOnTriggerComponent, RMCTriggerEvent>)OnTileFireTriggered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TileFireOnTriggerComponent, CMExplosiveTriggeredEvent>((EntityEventRefHandler<TileFireOnTriggerComponent, CMExplosiveTriggeredEvent>)OnTileFireOnTriggerExplosive, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DirectionalTileFireOnTriggerComponent, RMCTriggerEvent>((EntityEventRefHandler<DirectionalTileFireOnTriggerComponent, RMCTriggerEvent>)OnDirectionTileFireTriggered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCIgniteOnCollideComponent, StartCollideEvent>((EntityEventRefHandler<RMCIgniteOnCollideComponent, StartCollideEvent>)OnIgniteCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCIgniteOnCollideComponent, DamageCollideEvent>((EntityEventRefHandler<RMCIgniteOnCollideComponent, DamageCollideEvent>)OnIgniteDamageCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SteppingOnFireComponent, CMGetArmorEvent>((EntityEventRefHandler<SteppingOnFireComponent, CMGetArmorEvent>)OnSteppingOnFireGetArmor, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SteppingOnFireComponent, ComponentRemove>((EntityEventRefHandler<SteppingOnFireComponent, ComponentRemove>)OnSteppingOnFireRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CanBeFirePattedComponent, InteractHandEvent>((EntityEventRefHandler<CanBeFirePattedComponent, InteractHandEvent>)OnCanBeFirePattedInteractHand, new Type[1] { typeof(InteractionPopupSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlammableComponent, IgnitedEvent>((EntityEventRefHandler<FlammableComponent, IgnitedEvent>)OnFlammableIgnite, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlammableComponent, RMCExtinguishedEvent>((EntityEventRefHandler<FlammableComponent, RMCExtinguishedEvent>)OnFlammableExtinguished, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PlasmaFrenzyComponent, IgnitedEvent>((EntityEventRefHandler<PlasmaFrenzyComponent, IgnitedEvent>)OnPlasmaFrenzyIgnite, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCImmuneToIgnitionComponent, GetIgnitionImmunityEvent>((EntityEventRefHandler<RMCImmuneToIgnitionComponent, GetIgnitionImmunityEvent>)OnGetIgnitionImmunity, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCImmuneToIgnitionComponent, InventoryRelayedEvent<GetIgnitionImmunityEvent>>((EntityEventRefHandler<RMCImmuneToIgnitionComponent, InventoryRelayedEvent<GetIgnitionImmunityEvent>>)OnGetIgnitionEquipmentImmunity, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCImmuneToIgnitionComponent, ExaminedEvent>((EntityEventRefHandler<RMCImmuneToIgnitionComponent, ExaminedEvent>)OnIgnitionImmunityExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCImmuneToFireTileDamageComponent, RMCGetFireImmunityEvent>((EntityEventRefHandler<RMCImmuneToFireTileDamageComponent, RMCGetFireImmunityEvent>)OnImmuneToTileFireGet, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCImmuneToFireTileDamageComponent, ExaminedEvent>((EntityEventRefHandler<RMCImmuneToFireTileDamageComponent, ExaminedEvent>)OnImmuneToTileFireExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFireArmorDebuffModifierComponent, ExaminedEvent>((EntityEventRefHandler<RMCFireArmorDebuffModifierComponent, ExaminedEvent>)OnFireArmorDebuffModifierExamined, (Type[])null, (Type[])null);
	}

	private void OnIgniteOnProjectileHit(Entity<IgniteOnProjectileHitComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (CanBeIgnited(args.Target, Entity<IgniteOnProjectileHitComponent>.op_Implicit(ent), ent.Comp.Intensity))
		{
			ChangeBurnColor(args.Target, ent.Comp.BurnColor);
			Ignite(Entity<FlammableComponent>.op_Implicit(args.Target), ent.Comp.Intensity, ent.Comp.Duration, ent.Comp.Duration, igniteDamage: false);
		}
	}

	private void OnTileFireMapInit(Entity<TileFireComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.SpawnedAt = _timing.CurTime;
		((EntitySystem)this).Dirty<TileFireComponent>(ent, (MetaDataComponent)null);
	}

	private void OnTileFireVaporHit(Entity<TileFireComponent> ent, ref VaporHitEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		bool water = false;
		foreach (string container in args.Solution.Comp.Containers)
		{
			if (_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(args.Solution.Owner), container, out Entity<SolutionComponent>? _, out Solution solution) && solution.ContainsPrototype(ProtoId<ReagentPrototype>.op_Implicit(WaterReagent)))
			{
				water = true;
				break;
			}
		}
		if (water)
		{
			if (ent.Comp.ExtinguishInstantly)
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<TileFireComponent>.op_Implicit(ent));
				return;
			}
			ent.Comp.Duration -= TimeSpan.FromSeconds(args.Power);
			((EntitySystem)this).Dirty<TileFireComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnTileFireInteractHand(Entity<TileFireComponent> ent, ref InteractHandEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		TileFirePatterComponent patter = default(TileFirePatterComponent);
		if (((EntitySystem)this).TryComp<TileFirePatterComponent>(user, ref patter))
		{
			TimeSpan time = _timing.CurTime;
			if (!(time < patter.Last + patter.Cooldown))
			{
				patter.Last = time;
				((EntitySystem)this).Dirty(user, (IComponent)(object)patter, (MetaDataComponent)null);
				ent.Comp.Duration -= patter.RemoveDuration * ent.Comp.PatExtinguishMultiplier;
				((EntitySystem)this).Dirty<TileFireComponent>(ent, (MetaDataComponent)null);
				_rmcMelee.DoLunge(user, Entity<TileFireComponent>.op_Implicit(ent));
				SharedAudioSystem audio = _audio;
				SoundSpecifier? sound = patter.Sound;
				EntityUid? val = user;
				AudioParams val2 = ((AudioParams)(ref AudioParams.Default)).WithVolume(-8f);
				audio.PlayPredicted(sound, user, val, (AudioParams?)((AudioParams)(ref val2)).WithVariation((float?)0.05f));
			}
		}
	}

	private void OnTileFirePreventCollide(Entity<TileFireComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && (_projectileQuery.HasComp(args.OtherEntity) || _tileFireQuery.HasComp(args.OtherEntity)))
		{
			args.Cancelled = true;
		}
	}

	private void OnCraftsIntoMolotovExamined(Entity<CraftsIntoMolotovComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		if (!CanCraftMolotovPopup(ent, args.Examiner, popup: false, out var _))
		{
			return;
		}
		using (args.PushGroup("CraftsIntoMolotovComponent"))
		{
			args.PushMarkup("[color=cyan]You can turn this into a molotov with a piece of paper![/color]");
		}
	}

	private void OnCraftsIntoMolotovInteractUsing(Entity<CraftsIntoMolotovComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<PaperComponent>(args.Used) && CanCraftMolotovPopup(ent, args.User, popup: true, out var _))
		{
			CraftMolotovDoAfterEvent ev = new CraftMolotovDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, ent.Comp.Delay, ev, Entity<CraftsIntoMolotovComponent>.op_Implicit(ent), Entity<CraftsIntoMolotovComponent>.op_Implicit(ent), args.Used)
			{
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnCraftsIntoMolotovDoAfter(Entity<CraftsIntoMolotovComponent> ent, ref CraftMolotovDoAfterEvent args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (((EntitySystem)this).HasComp<PaperComponent>(args.Used) && CanCraftMolotovPopup(ent, args.User, popup: true, out var intensity) && !_net.IsClient)
			{
				EntityCoordinates coords = _transform.GetMoverCoordinates(Entity<CraftsIntoMolotovComponent>.op_Implicit(ent));
				EntityUid molotov = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ent.Comp.Spawns), coords);
				TileFireOnTriggerComponent tileFire = ((EntitySystem)this).EnsureComp<TileFireOnTriggerComponent>(molotov);
				tileFire.Duration = intensity.Int();
				((EntitySystem)this).Dirty(molotov, (IComponent)(object)tileFire, (MetaDataComponent)null);
				((EntitySystem)this).Del((EntityUid?)Entity<CraftsIntoMolotovComponent>.op_Implicit(ent));
				((EntitySystem)this).Del(args.Used);
				_hands.TryPickupAnyHand(args.User, molotov);
			}
		}
	}

	private void OnTileFireTriggered(Entity<TileFireOnTriggerComponent> ent, ref RMCTriggerEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coords = _transform.GetMoverCoordinates(Entity<TileFireOnTriggerComponent>.op_Implicit(ent));
		_audio.PlayPvs(ent.Comp.Sound, coords, (AudioParams?)null);
		EntityCoordinates tile = coords.SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
		SpawnFireDiamond(ent.Comp.Spawn, tile, ent.Comp.Range, ent.Comp.Intensity, ent.Comp.Duration);
		((EntitySystem)this).QueueDel((EntityUid?)Entity<TileFireOnTriggerComponent>.op_Implicit(ent));
	}

	private void OnDirectionTileFireTriggered(Entity<DirectionalTileFireOnTriggerComponent> ent, ref RMCTriggerEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		(EntityCoordinates, Angle) moverCoordinates = _transform.GetMoverCoordinateRotation(Entity<DirectionalTileFireOnTriggerComponent>.op_Implicit(ent), ((EntitySystem)this).Transform(Entity<DirectionalTileFireOnTriggerComponent>.op_Implicit(ent)));
		EntityCoordinates tile = moverCoordinates.Item1.SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
		DirectionalTileFireOnTriggerComponent comp = ent.Comp;
		Angle val = DirectionExtensions.ToAngle(ent.Comp.Direction);
		val = Angle.FromDegrees(((Angle)(ref val)).Degrees + ((Angle)(ref moverCoordinates.Item2)).Degrees);
		comp.Direction = ((Angle)(ref val)).GetDir();
		((EntitySystem)this).Dirty<DirectionalTileFireOnTriggerComponent>(ent, (MetaDataComponent)null);
		if (ent.Comp.Rebounded)
		{
			tile = tile.Offset(ent.Comp.Direction);
		}
		_audio.PlayPvs(ent.Comp.Sound, moverCoordinates.Item1, (AudioParams?)null);
		SpawnFireCone(ent, tile, ent.Comp.Intensity, ent.Comp.Duration);
		((EntitySystem)this).QueueDel((EntityUid?)Entity<DirectionalTileFireOnTriggerComponent>.op_Implicit(ent));
	}

	private void OnTileFireOnTriggerExplosive(Entity<TileFireOnTriggerComponent> ent, ref CMExplosiveTriggeredEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coords = _transform.GetMoverCoordinates(Entity<TileFireOnTriggerComponent>.op_Implicit(ent)).SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
		SpawnFireDiamond(ent.Comp.Spawn, coords, ent.Comp.Range, ent.Comp.Intensity, ent.Comp.Duration);
	}

	private void OnIgniteCollide(Entity<RMCIgniteOnCollideComponent> ent, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		TryIgnite(ent, args.OtherEntity, checkIgnited: false);
	}

	private void OnIgniteDamageCollide(Entity<RMCIgniteOnCollideComponent> ent, ref DamageCollideEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (CanBeIgnited(args.Target, Entity<RMCIgniteOnCollideComponent>.op_Implicit(ent), ent.Comp.Intensity, directHit: true))
		{
			Ignite(Entity<FlammableComponent>.op_Implicit(args.Target), ent.Comp.Intensity, ent.Comp.Duration, ent.Comp.MaxStacks);
		}
	}

	private void OnSteppingOnFireRemoved(Entity<SteppingOnFireComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit((ValueTuple<EntityUid, CMArmorComponent>)(Entity<SteppingOnFireComponent>.op_Implicit(ent), null)));
	}

	private void OnSteppingOnFireGetArmor(Entity<SteppingOnFireComponent> ent, ref CMGetArmorEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.ArmorModifier *= ent.Comp.ArmorMultiplier;
	}

	private void OnCanBeFirePattedInteractHand(Entity<CanBeFirePattedComponent> ent, ref InteractHandEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		FirePatterComponent patter = default(FirePatterComponent);
		FlammableComponent flammable = default(FlammableComponent);
		if (args.Target != ent.Owner || user == args.Target || !((EntitySystem)this).TryComp<FirePatterComponent>(user, ref patter) || _entityWhitelist.IsBlacklistPass(patter.Blacklist, Entity<CanBeFirePattedComponent>.op_Implicit(ent)) || !((EntitySystem)this).TryComp<FlammableComponent>(Entity<CanBeFirePattedComponent>.op_Implicit(ent), ref flammable) || !flammable.OnFire)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		TimeSpan time = _timing.CurTime;
		if (!(time < patter.LastPat + patter.Cooldown))
		{
			patter.LastPat = time;
			((EntitySystem)this).Dirty(user, (IComponent)(object)patter, (MetaDataComponent)null);
			Pat(Entity<FlammableComponent>.op_Implicit(ent.Owner), patter.Stacks);
			_audio.PlayPredicted(patter.Sound, user, (EntityUid?)user, (AudioParams?)null);
			_popup.PopupClient("You try to put out the fire on " + ((EntitySystem)this).Name(Entity<CanBeFirePattedComponent>.op_Implicit(ent), (MetaDataComponent)null) + "!", Entity<CanBeFirePattedComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			_popup.PopupEntity(((EntitySystem)this).Name(user, (MetaDataComponent)null) + " tries to put out the fire on you!", Entity<CanBeFirePattedComponent>.op_Implicit(ent), Entity<CanBeFirePattedComponent>.op_Implicit(ent), PopupType.SmallCaution);
			Filter others = Filter.PvsExcept(Entity<CanBeFirePattedComponent>.op_Implicit(ent), 2f, (IEntityManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid e) => e == user || e == ent.Owner));
			_popup.PopupEntity(((EntitySystem)this).Name(user, (MetaDataComponent)null) + " tries to put out the fire on " + ((EntitySystem)this).Name(Entity<CanBeFirePattedComponent>.op_Implicit(ent), (MetaDataComponent)null) + "!", Entity<CanBeFirePattedComponent>.op_Implicit(ent), others, recordReplay: true);
		}
	}

	private void OnFlammableIgnite(Entity<FlammableComponent> ent, ref IgnitedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<OnFireComponent>(Entity<FlammableComponent>.op_Implicit(ent));
	}

	private void OnFlammableExtinguished(Entity<FlammableComponent> ent, ref RMCExtinguishedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<OnFireComponent>(Entity<FlammableComponent>.op_Implicit(ent));
		((EntitySystem)this).RemCompDeferred<RMCFireBypassActiveComponent>(Entity<FlammableComponent>.op_Implicit(ent));
	}

	public void UpdateFireAlert(EntityUid ent)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ShowFireAlertEvent ev = default(ShowFireAlertEvent);
		((EntitySystem)this).RaiseLocalEvent<ShowFireAlertEvent>(ent, ref ev, false);
		if (ev.Show)
		{
			_alerts.ShowAlert(ent, FireAlert);
		}
		else
		{
			_alerts.ClearAlert(ent, FireAlert);
		}
	}

	public bool IsOnFire(Entity<FlammableComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<FlammableComponent>(Entity<FlammableComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return ent.Comp.OnFire;
		}
		return false;
	}

	public virtual bool Ignite(Entity<FlammableComponent?> flammable, int intensity, int duration, int? maxStacks, bool igniteDamage = true)
	{
		return false;
	}

	public virtual void Extinguish(Entity<FlammableComponent?> flammable)
	{
	}

	public virtual void Pat(Entity<FlammableComponent?> flammable, int stacks)
	{
	}

	public virtual void AdjustStacks(Entity<FlammableComponent?> flammable, int stacks)
	{
	}

	public virtual void DoStopDropRollAnimation(EntityUid uid)
	{
	}

	private void SpawnFireChain(EntProtoId spawn, EntityUid chain, EntityCoordinates coordinates, int? intensity, int? duration)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityUid spawned = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(spawn), coordinates);
		if (intensity.HasValue || duration.HasValue)
		{
			RMCIgniteOnCollideComponent ignite = ((EntitySystem)this).EnsureComp<RMCIgniteOnCollideComponent>(spawned);
			if (intensity.HasValue)
			{
				ignite.Intensity = intensity.Value;
			}
			if (duration.HasValue)
			{
				ignite.Duration = duration.Value;
			}
			((EntitySystem)this).Dirty(spawned, (IComponent)(object)ignite, (MetaDataComponent)null);
		}
		DamageOnCollideComponent onCollide = ((EntitySystem)this).EnsureComp<DamageOnCollideComponent>(spawned);
		_onCollide.SetChain(Entity<DamageOnCollideComponent>.op_Implicit((spawned, onCollide)), chain);
	}

	private void SpawnFires(EntProtoId spawn, EntityCoordinates coordinates, int range, EntityUid chain, int? intensity, int? duration, HashSet<EntityCoordinates>? spawned = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		if (spawned == null)
		{
			spawned = new HashSet<EntityCoordinates>();
		}
		ImmutableArray<Direction>.Enumerator enumerator = _rmcMap.CardinalDirections.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Direction cardinal = enumerator.Current;
			EntityCoordinates target = coordinates.Offset(cardinal);
			if (!spawned.Add(target))
			{
				continue;
			}
			bool cont;
			int nextRange = SpawnFire(target, spawn, chain, range, intensity, duration, out cont);
			if (nextRange == 0 || cont)
			{
				continue;
			}
			Timer.Spawn(TimeSpan.FromMilliseconds(50L), (Action)delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				try
				{
					SpawnFires(spawn, target, nextRange, chain, intensity, duration, spawned);
				}
				catch (Exception value)
				{
					((EntitySystem)this).Log.Error($"Error occurred spawning fires:\n{value}");
				}
			}, default(CancellationToken));
		}
	}

	public void SpawnFireDiamond(EntProtoId spawn, EntityCoordinates center, int range, int? intensity = null, int? duration = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Entity<CollideChainComponent> chain = _onCollide.SpawnChain();
		SpawnFire(center, spawn, Entity<CollideChainComponent>.op_Implicit(chain), range, intensity, duration, out var _);
		SpawnFires(spawn, center, range, Entity<CollideChainComponent>.op_Implicit(chain), intensity, duration);
	}

	public void SpawnFireLines(EntProtoId spawn, EntityCoordinates center, int cardinalRange, int ordinalRange, int? intensity = null, int? duration = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Entity<CollideChainComponent> chain = _onCollide.SpawnChain();
		HashSet<EntityCoordinates> spawned = new HashSet<EntityCoordinates>();
		ImmutableArray<Direction>.Enumerator enumerator = DirectionExtensions.AllDirections.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Direction direction = enumerator.Current;
			int nextRange = (_rmcMap.CardinalDirections.Contains(direction) ? cardinalRange : ordinalRange);
			EntityCoordinates target = center.Offset(direction);
			while (nextRange > 0)
			{
				if (spawned.Add(target))
				{
					nextRange = SpawnFire(target, spawn, Entity<CollideChainComponent>.op_Implicit(chain), nextRange, intensity, duration, out var cont);
					target = target.Offset(direction);
					if (cont)
					{
						break;
					}
				}
			}
		}
	}

	public int SpawnFire(EntityCoordinates target, EntProtoId spawn, EntityUid chain, int range, int? intensity, int? duration, out bool cont)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		cont = false;
		if (!_rmcMap.TryGetTileDef(target, out ContentTileDefinition tile) || tile.ID == "Space")
		{
			cont = true;
			return range;
		}
		if (_rmcMap.HasAnchoredEntityEnumerator<TileFireComponent>(target, out Entity<TileFireComponent> oldTileFire, (Direction?)null, (DirectionFlag)0))
		{
			EntProtoId<TileFireComponent>? id = oldTileFire.Comp.Id;
			EntProtoId? val = (id.HasValue ? new EntProtoId?(EntProtoId<TileFireComponent>.op_Implicit(id.GetValueOrDefault())) : ((EntProtoId?)null));
			if (val.HasValue && spawn == val.GetValueOrDefault())
			{
				cont = true;
				return range;
			}
			((EntitySystem)this).QueueDel((EntityUid?)Entity<TileFireComponent>.op_Implicit(oldTileFire));
		}
		int nextRange = range - 1;
		RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(target, null, (DirectionFlag)0);
		EntityUid uid;
		while (anchored.MoveNext(out uid))
		{
			if (_blockTileFireQuery.HasComp(uid))
			{
				nextRange = 0;
				break;
			}
			if (_tag.HasAnyTag(uid, StructureTag, WallTag) && !_doorQuery.HasComp(uid))
			{
				nextRange = 0;
				break;
			}
		}
		SpawnFireChain(spawn, chain, target, intensity, duration);
		return nextRange;
	}

	private void SpawnFireCone(Entity<DirectionalTileFireOnTriggerComponent> ent, EntityCoordinates center, int? intensity = null, int? duration = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		Entity<CollideChainComponent> chain = _onCollide.SpawnChain();
		if (_net.IsClient)
		{
			return;
		}
		ent.Comp.DiagonalRange = (int)Math.Floor((double)ent.Comp.Range / 2.0);
		((EntitySystem)this).Dirty<DirectionalTileFireOnTriggerComponent>(ent, (MetaDataComponent)null);
		bool initialShot = !ent.Comp.InitialSpread;
		EntityCoordinates target = center;
		HashSet<EntityCoordinates> targets = new HashSet<EntityCoordinates>();
		while (ent.Comp.Range > 0)
		{
			foreach (EntityCoordinates coordinate in AddTarget(ent, target, initialShot))
			{
				targets.Add(coordinate);
			}
			initialShot = false;
			RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(target, null, (DirectionFlag)0);
			EntityUid uid;
			while (anchored.MoveNext(out uid))
			{
				if (_tag.HasAnyTag(uid, WallTag) && !_doorQuery.HasComp(uid))
				{
					ent.Comp.Range = 0;
					break;
				}
			}
			target = ChangeTarget(target, ent.Comp.Direction);
			ent.Comp.Range--;
			ent.Comp.DiagonalRange--;
		}
		foreach (EntityCoordinates ignitionTarget in targets)
		{
			if (CheckViableTile(ent, ignitionTarget))
			{
				SpawnFireChain(ent.Comp.Spawn, Entity<CollideChainComponent>.op_Implicit(chain), ignitionTarget, intensity, duration);
			}
		}
	}

	private EntityCoordinates ChangeTarget(EntityCoordinates target, Direction direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return target.Offset(direction);
	}

	private HashSet<EntityCoordinates> AddTarget(Entity<DirectionalTileFireOnTriggerComponent> ent, EntityCoordinates target, bool initialShot)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityCoordinates> targets = new HashSet<EntityCoordinates> { target };
		int width = ent.Comp.Width;
		int widthExtension = ent.Comp.Width + 1;
		Angle val = DirectionExtensions.ToAngle(ent.Comp.Direction);
		double degrees = ((Angle)(ref val)).Degrees;
		EntityCoordinates centerTarget = target;
		EntityCoordinates leftTarget = target;
		EntityCoordinates rightTarget = target;
		while (width > 0)
		{
			if ((int)degrees % 90 != 0)
			{
				while (widthExtension > 0 && ent.Comp.DiagonalRange > 0)
				{
					centerTarget = ChangeTarget(centerTarget, ent.Comp.Direction);
					EntityCoordinates target2 = leftTarget;
					val = Angle.FromDegrees(degrees - degrees % 90.0);
					leftTarget = ChangeTarget(target2, ((Angle)(ref val)).GetDir());
					EntityCoordinates target3 = rightTarget;
					val = Angle.FromDegrees(degrees + degrees % 90.0);
					rightTarget = ChangeTarget(target3, ((Angle)(ref val)).GetDir());
					targets.Add(leftTarget);
					targets.Add(rightTarget);
					targets.Add(centerTarget);
					widthExtension--;
				}
			}
			else if (!initialShot)
			{
				EntityCoordinates target4 = leftTarget;
				val = Angle.FromDegrees(degrees - 90.0);
				leftTarget = ChangeTarget(target4, ((Angle)(ref val)).GetDir());
				EntityCoordinates target5 = rightTarget;
				val = Angle.FromDegrees(degrees + 90.0);
				rightTarget = ChangeTarget(target5, ((Angle)(ref val)).GetDir());
				targets.Add(leftTarget);
				targets.Add(rightTarget);
			}
			width--;
		}
		return targets;
	}

	private bool CheckViableTile(Entity<DirectionalTileFireOnTriggerComponent> ent, EntityCoordinates target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!_rmcMap.TryGetTileDef(target, out ContentTileDefinition tile) || tile.ID == "Space")
		{
			return false;
		}
		if (_rmcMap.HasAnchoredEntityEnumerator<TileFireComponent>(target, out Entity<TileFireComponent> oldTileFire, (Direction?)null, (DirectionFlag)0))
		{
			((EntitySystem)this).QueueDel((EntityUid?)Entity<TileFireComponent>.op_Implicit(oldTileFire));
		}
		return true;
	}

	private bool CanCraftMolotovPopup(Entity<CraftsIntoMolotovComponent> ent, EntityUid user, bool popup, out FixedPoint2 intensity)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		intensity = default(FixedPoint2);
		if (!_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.SolutionId, out Entity<SolutionComponent>? _, out Solution solution) || solution.Volume <= FixedPoint2.Zero)
		{
			if (popup)
			{
				_popup.PopupClient("The " + ((EntitySystem)this).Name(Entity<CraftsIntoMolotovComponent>.op_Implicit(ent), (MetaDataComponent)null) + " is empty...", Entity<CraftsIntoMolotovComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			}
			return false;
		}
		intensity = FixedPoint2.Zero;
		foreach (ReagentQuantity solutionReagent in solution)
		{
			if (_reagents.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(solutionReagent.Reagent.Prototype), out Reagent reagent))
			{
				intensity += reagent.IntensityMod * solutionReagent.Quantity;
			}
		}
		if (intensity < ent.Comp.MinIntensity)
		{
			if (popup)
			{
				string msg = "There's not enough flammable liquid in the " + ((EntitySystem)this).Name(Entity<CraftsIntoMolotovComponent>.op_Implicit(ent), (MetaDataComponent)null) + "!";
				_popup.PopupClient(msg, Entity<CraftsIntoMolotovComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			}
			return false;
		}
		intensity = FixedPoint2.Min(intensity, ent.Comp.MaxIntensity);
		return true;
	}

	private void OnPlasmaFrenzyIgnite(Entity<PlasmaFrenzyComponent> ent, ref IgnitedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		XenoPlasmaComponent plasma = default(XenoPlasmaComponent);
		if (((EntitySystem)this).TryComp<XenoPlasmaComponent>(Entity<PlasmaFrenzyComponent>.op_Implicit(ent), ref plasma))
		{
			if (plasma.Plasma < plasma.MaxPlasma && _net.IsServer)
			{
				_emote.TryEmoteWithChat(Entity<PlasmaFrenzyComponent>.op_Implicit(ent), ent.Comp.RoarEmote);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-plasma-frenzy-fire"), Entity<PlasmaFrenzyComponent>.op_Implicit(ent), Entity<PlasmaFrenzyComponent>.op_Implicit(ent), PopupType.SmallCaution);
			}
			_plasma.SetPlasma(Entity<XenoPlasmaComponent>.op_Implicit((Entity<PlasmaFrenzyComponent>.op_Implicit(ent), plasma)), plasma.MaxPlasma);
		}
	}

	private void OnGetIgnitionEquipmentImmunity(Entity<RMCImmuneToIgnitionComponent> ent, ref InventoryRelayedEvent<GetIgnitionImmunityEvent> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnGetIgnitionImmunity(ent, ref args.Args);
	}

	private void OnGetIgnitionImmunity(Entity<RMCImmuneToIgnitionComponent> ent, ref GetIgnitionImmunityEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.IntensityResistance >= args.Intensity && (ent.Comp.ImmuneToDirectHits || !args.DirectHit))
		{
			args.Ignite = false;
		}
	}

	private void OnIgnitionImmunityExamined(Entity<RMCImmuneToIgnitionComponent> ent, ref ExaminedEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("RMCImmuneToIgnitionComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-immune-to-ignition-examine", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("direct", ent.Comp.ImmuneToDirectHits)));
		}
	}

	private void OnImmuneToTileFireGet(Entity<RMCImmuneToFireTileDamageComponent> ent, ref RMCGetFireImmunityEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Fire.HasValue)
		{
			args.Immune = true;
		}
		else if (!_entityWhitelist.IsWhitelistPass(ent.Comp.BypassWhitelist, args.Fire.Value))
		{
			args.Immune = true;
		}
	}

	private void OnImmuneToTileFireExamined(Entity<RMCImmuneToFireTileDamageComponent> ent, ref ExaminedEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("RMCImmuneToFireTileDamageComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-immune-to-fire-tile-damage-examine", (ValueTuple<string, object>)("ent", ent)));
		}
	}

	private void OnFireArmorDebuffModifierExamined(Entity<RMCFireArmorDebuffModifierComponent> ent, ref ExaminedEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("RMCFireArmorDebuffModifierComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-fire-armor-debuff-modifier-examine", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("percentage", $"{(ent.Comp.DebuffModifier - 1f) * 100f:F0}")));
		}
	}

	public void SetIntensityDuration(Entity<RMCIgniteOnCollideComponent?, DamageOnCollideComponent?> ent, int? intensity, int? duration)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Resolve<RMCIgniteOnCollideComponent, DamageOnCollideComponent>(Entity<RMCIgniteOnCollideComponent, DamageOnCollideComponent>.op_Implicit(ent), ref ent.Comp1, ref ent.Comp2, false);
		if (ent.Comp1 != null)
		{
			if (intensity.HasValue)
			{
				ent.Comp1.Intensity = intensity.Value;
			}
			if (duration.HasValue)
			{
				ent.Comp1.Duration = duration.Value;
			}
			((EntitySystem)this).Dirty(Entity<RMCIgniteOnCollideComponent, DamageOnCollideComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp1, (MetaDataComponent)null);
		}
		if (ent.Comp2 != null)
		{
			if (duration.HasValue)
			{
				ent.Comp2.Damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(HeatDamage)] = duration.Value;
			}
			((EntitySystem)this).Dirty(Entity<RMCIgniteOnCollideComponent, DamageOnCollideComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp2, (MetaDataComponent)null);
		}
	}

	private void TryIgnite(Entity<RMCIgniteOnCollideComponent> ent, EntityUid other, bool checkIgnited)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<SteppingOnFireComponent>(other);
		Entity<FlammableComponent> flammableEnt = default(Entity<FlammableComponent>);
		flammableEnt._002Ector(other, (FlammableComponent)null);
		if (!((EntitySystem)this).Resolve<FlammableComponent>(Entity<FlammableComponent>.op_Implicit(flammableEnt), ref flammableEnt.Comp, false))
		{
			return;
		}
		bool wasOnFire = IsOnFire(flammableEnt);
		if ((checkIgnited && wasOnFire) || !CanBeIgnited(other, Entity<RMCIgniteOnCollideComponent>.op_Implicit(ent), ent.Comp.Intensity))
		{
			return;
		}
		RMCGetFireImmunityEvent tileEv = new RMCGetFireImmunityEvent(Entity<RMCIgniteOnCollideComponent>.op_Implicit(ent));
		((EntitySystem)this).RaiseLocalEvent<RMCGetFireImmunityEvent>(other, ref tileEv, false);
		if (tileEv.Ignite && Ignite(flammableEnt, ent.Comp.Intensity, ent.Comp.Duration, ent.Comp.MaxStacks))
		{
			ChangeBurnColor(Entity<FlammableComponent>.op_Implicit(flammableEnt), ent.Comp.BurnColor);
			if (!CanFireBypassImmunity(Entity<RMCIgniteOnCollideComponent>.op_Implicit(ent), other))
			{
				((EntitySystem)this).RemCompDeferred<RMCFireBypassActiveComponent>(other);
			}
			else
			{
				((EntitySystem)this).EnsureComp<RMCFireBypassActiveComponent>(other);
			}
			if (!wasOnFire && IsOnFire(flammableEnt) && CanFireBypassImmunity(Entity<RMCIgniteOnCollideComponent>.op_Implicit(ent), other))
			{
				_damageable.TryChangeDamage(Entity<FlammableComponent>.op_Implicit(flammableEnt), flammableEnt.Comp.Damage * ent.Comp.Intensity, ignoreResistances: true);
			}
		}
	}

	private void ApplyTileEffect(Entity<SteppingOnFireComponent> ent, RMCIgniteOnCollideComponent ignite, EntityUid fireEntity)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan timing = _timing.CurTime;
		DamageSpecifier tile = ignite.TileDamage;
		if (tile == null)
		{
			return;
		}
		SteppingOnFireComponent stepping = ent.Comp;
		EntityUid uid = ent.Owner;
		if (ignite.ArmorMultiplier < stepping.ArmorMultiplier && _entityWhitelist.IsWhitelistPassOrNull(ignite.ArmorWhitelist, uid))
		{
			stepping.ArmorMultiplier = ignite.ArmorMultiplier;
			RMCFireArmorDebuffModifierComponent mod = default(RMCFireArmorDebuffModifierComponent);
			if (((EntitySystem)this).TryComp<RMCFireArmorDebuffModifierComponent>(uid, ref mod))
			{
				stepping.ArmorMultiplier *= mod.DebuffModifier;
			}
			_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit((ValueTuple<EntityUid, CMArmorComponent>)(uid, null)));
		}
		EntityCoordinates coords = _transform.GetMoverCoordinates(uid);
		EntityCoordinates? lastPosition = stepping.LastPosition;
		if (lastPosition.HasValue)
		{
			EntityCoordinates last = lastPosition.GetValueOrDefault();
			float distance = default(float);
			if (((EntityCoordinates)(ref last)).TryDistance((IEntityManager)(object)base.EntityManager, _transform, coords, ref distance))
			{
				stepping.Distance += distance;
				if (stepping.Distance >= 1f)
				{
					stepping.Distance = 0f;
					if (CanFireBypassImmunity(fireEntity, uid))
					{
						_damageable.TryChangeDamage(uid, tile * ignite.Intensity, ignoreResistances: true);
					}
				}
			}
		}
		FlammableComponent flammable = default(FlammableComponent);
		if (!_flammableQuery.TryComp(ent.Owner, ref flammable))
		{
			return;
		}
		if (CanBeIgnited(uid, fireEntity, ignite.Intensity))
		{
			Ignite(Entity<FlammableComponent>.op_Implicit((uid, flammable)), ignite.Intensity, ignite.Duration, ignite.MaxStacks);
			if (!CanFireBypassImmunity(fireEntity, uid))
			{
				((EntitySystem)this).RemCompDeferred<RMCFireBypassActiveComponent>(uid);
			}
			else
			{
				((EntitySystem)this).EnsureComp<RMCFireBypassActiveComponent>(uid);
			}
		}
		else if (CanFireBypassImmunity(fireEntity, uid))
		{
			GetFireProtectionEvent ev = new GetFireProtectionEvent();
			((EntitySystem)this).RaiseLocalEvent<GetFireProtectionEvent>(uid, ref ev, false);
			InventoryComponent inv = default(InventoryComponent);
			if (_inventoryQuery.TryComp(uid, ref inv))
			{
				_inventory.RelayEvent(Entity<InventoryComponent>.op_Implicit((uid, inv)), ref ev);
			}
			if (ent.Comp.UpdateAt <= timing)
			{
				_damageable.TryChangeDamage(uid, (float)ignite.Intensity / 5f * flammable.Damage * ev.Multiplier, ignoreResistances: true, interruptsDoAfters: false);
				ent.Comp.UpdateAt = timing + ent.Comp.UpdateTime;
			}
		}
		stepping.LastPosition = coords;
		((EntitySystem)this).Dirty<SteppingOnFireComponent>(ent, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<TileFireComponent> tileFireQuery = ((EntitySystem)this).EntityQueryEnumerator<TileFireComponent>();
		EntityUid uid = default(EntityUid);
		TileFireComponent fire = default(TileFireComponent);
		while (tileFireQuery.MoveNext(ref uid, ref fire))
		{
			TimeSpan timeLeft = fire.SpawnedAt + fire.Duration - time;
			if (timeLeft <= TimeSpan.Zero)
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid);
			}
			else if (time < fire.SpawnedAt + fire.BigFireDuration)
			{
				_appearance.SetData(uid, (Enum)TileFireLayers.Base, (object)TileFireVisuals.Four, (AppearanceComponent)null);
			}
			else if (timeLeft < TimeSpan.FromSeconds(9L))
			{
				_appearance.SetData(uid, (Enum)TileFireLayers.Base, (object)TileFireVisuals.One, (AppearanceComponent)null);
			}
			else if (timeLeft < TimeSpan.FromSeconds(25L))
			{
				_appearance.SetData(uid, (Enum)TileFireLayers.Base, (object)TileFireVisuals.Two, (AppearanceComponent)null);
			}
			else
			{
				_appearance.SetData(uid, (Enum)TileFireLayers.Base, (object)TileFireVisuals.Three, (AppearanceComponent)null);
			}
		}
		EntityQueryEnumerator<RMCIgniteOnCollideComponent> applyQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCIgniteOnCollideComponent>();
		EntityUid uid2 = default(EntityUid);
		RMCIgniteOnCollideComponent apply = default(RMCIgniteOnCollideComponent);
		while (applyQuery.MoveNext(ref uid2, ref apply))
		{
			foreach (EntityUid contact in _physics.GetEntitiesIntersectingBody(uid2, (int)apply.Collision, true, (PhysicsComponent)null, (FixturesComponent)null, (TransformComponent)null))
			{
				TryIgnite(Entity<RMCIgniteOnCollideComponent>.op_Implicit((uid2, apply)), contact, checkIgnited: true);
			}
			RMCAnchoredEntitiesEnumerator enumerator2 = _rmcMap.GetAnchoredEntitiesEnumerator(uid2, null, (DirectionFlag)0);
			EntityUid contact2;
			while (enumerator2.MoveNext(out contact2))
			{
				TryIgnite(Entity<RMCIgniteOnCollideComponent>.op_Implicit((uid2, apply)), contact2, checkIgnited: true);
			}
			if (!apply.InitDamaged)
			{
				apply.InitDamaged = true;
				((EntitySystem)this).Dirty(uid2, (IComponent)(object)apply, (MetaDataComponent)null);
				((EntitySystem)this).RemCompDeferred<DamageOnCollideComponent>(uid2);
			}
		}
		EntityQueryEnumerator<ExtinguishFireComponent> extinguishQuery = ((EntitySystem)this).EntityQueryEnumerator<ExtinguishFireComponent>();
		EntityUid uid3 = default(EntityUid);
		ExtinguishFireComponent extinguish = default(ExtinguishFireComponent);
		FlammableComponent flammable = default(FlammableComponent);
		while (extinguishQuery.MoveNext(ref uid3, ref extinguish))
		{
			if (extinguish.Extinguished)
			{
				continue;
			}
			extinguish.Extinguished = true;
			((EntitySystem)this).Dirty(uid3, (IComponent)(object)extinguish, (MetaDataComponent)null);
			foreach (EntityUid entIntersecting in _physics.GetEntitiesIntersectingBody(uid3, (int)extinguish.Collision, true, (PhysicsComponent)null, (FixturesComponent)null, (TransformComponent)null))
			{
				if (_flammableQuery.TryComp(entIntersecting, ref flammable))
				{
					ExtinguishFireAttemptEvent ev = new ExtinguishFireAttemptEvent(uid3, entIntersecting);
					((EntitySystem)this).RaiseLocalEvent<ExtinguishFireAttemptEvent>(uid3, ref ev, false);
					if (!ev.Cancelled)
					{
						Extinguish(Entity<FlammableComponent>.op_Implicit((entIntersecting, flammable)));
					}
				}
			}
		}
		EntityQueryEnumerator<SprayExtinguishTileFireComponent> tileExtinguishQuery = ((EntitySystem)this).EntityQueryEnumerator<SprayExtinguishTileFireComponent>();
		EntityUid uid4 = default(EntityUid);
		SprayExtinguishTileFireComponent extinguishTile = default(SprayExtinguishTileFireComponent);
		TileFireComponent tileFire = default(TileFireComponent);
		while (tileExtinguishQuery.MoveNext(ref uid4, ref extinguishTile))
		{
			if (extinguishTile.Extinguished)
			{
				continue;
			}
			extinguishTile.Extinguished = true;
			((EntitySystem)this).Dirty(uid4, (IComponent)(object)extinguishTile, (MetaDataComponent)null);
			RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(uid4, null, (DirectionFlag)0);
			EntityUid anchorUid;
			while (anchored.MoveNext(out anchorUid))
			{
				if (_tileFireQuery.TryComp(anchorUid, ref tileFire))
				{
					tileFire.Duration -= extinguishTile.ExtinguishAmount * tileFire.SprayExtinguishMultiplier;
					((EntitySystem)this).Dirty(anchorUid, (IComponent)(object)tileFire, (MetaDataComponent)null);
				}
			}
		}
		EntityQueryEnumerator<SteppingOnFireComponent, PhysicsComponent> steppingQuery = ((EntitySystem)this).EntityQueryEnumerator<SteppingOnFireComponent, PhysicsComponent>();
		EntityUid uid5 = default(EntityUid);
		SteppingOnFireComponent stepping = default(SteppingOnFireComponent);
		PhysicsComponent body = default(PhysicsComponent);
		RMCIgniteOnCollideComponent ignite = default(RMCIgniteOnCollideComponent);
		while (steppingQuery.MoveNext(ref uid5, ref stepping, ref body))
		{
			stepping.ArmorMultiplier = 1.0;
			((EntitySystem)this).Dirty(uid5, (IComponent)(object)stepping, (MetaDataComponent)null);
			bool isStepping = false;
			foreach (EntityUid contact3 in _physics.GetContactingEntities(uid5, body, true))
			{
				if (_igniteOnCollideQuery.TryComp(contact3, ref ignite))
				{
					ApplyTileEffect(Entity<SteppingOnFireComponent>.op_Implicit((uid5, stepping)), ignite, contact3);
					isStepping = true;
					break;
				}
			}
			if (!isStepping)
			{
				HashSet<Entity<RMCIgniteOnCollideComponent>> nearbyEntities = _entityLookup.GetEntitiesInRange<RMCIgniteOnCollideComponent>(((EntitySystem)this).Transform(uid5).Coordinates, 0.35f, (LookupFlags)110);
				if (nearbyEntities.Count != 0)
				{
					Entity<RMCIgniteOnCollideComponent> nearbyEntity = nearbyEntities.First();
					ApplyTileEffect(Entity<SteppingOnFireComponent>.op_Implicit((uid5, stepping)), nearbyEntity.Comp, nearbyEntity.Owner);
					isStepping = true;
				}
			}
			if (!isStepping)
			{
				((EntitySystem)this).RemCompDeferred<SteppingOnFireComponent>(uid5);
			}
		}
		EntityQueryEnumerator<OnFireComponent, FlammableComponent> burningQuery = ((EntitySystem)this).EntityQueryEnumerator<OnFireComponent, FlammableComponent>();
		EntityUid uid6 = default(EntityUid);
		OnFireComponent onFireComponent = default(OnFireComponent);
		FlammableComponent flammable2 = default(FlammableComponent);
		while (burningQuery.MoveNext(ref uid6, ref onFireComponent, ref flammable2))
		{
			EntityCoordinates coords = _transform.GetMoverCoordinates(uid6);
			if (_rmcMap.HasAnchoredEntityEnumerator<RMCWaterComponent>(coords, (Direction?)null, (DirectionFlag)0))
			{
				Extinguish(Entity<FlammableComponent>.op_Implicit((uid6, flammable2)));
			}
		}
	}

	private bool CanFireBypassImmunity(EntityUid fireEntity, EntityUid targetEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<RMCFireImmunityBypassComponent>(fireEntity))
		{
			return true;
		}
		RMCGetFireImmunityEvent tileEv = new RMCGetFireImmunityEvent(fireEntity);
		((EntitySystem)this).RaiseLocalEvent<RMCGetFireImmunityEvent>(targetEntity, ref tileEv, false);
		return !tileEv.Immune;
	}

	public bool CanBeIgnited(EntityUid target, EntityUid fireSource, int intensity, bool directHit = false)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		GetIgnitionImmunityEvent ev = new GetIgnitionImmunityEvent(intensity, directHit);
		((EntitySystem)this).RaiseLocalEvent<GetIgnitionImmunityEvent>(target, ref ev, false);
		((EntitySystem)this).RaiseLocalEvent<GetIgnitionImmunityEvent>(ref ev);
		InventoryComponent inv = default(InventoryComponent);
		if (_inventoryQuery.TryComp(target, ref inv))
		{
			_inventory.RelayEvent(Entity<InventoryComponent>.op_Implicit((target, inv)), ref ev);
		}
		return ev.Ignite;
	}

	public void ChangeBurnColor(EntityUid target, Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		RMCFireColorComponent fireColorComp = default(RMCFireColorComponent);
		if (((EntitySystem)this).TryComp<RMCFireColorComponent>(target, ref fireColorComp))
		{
			fireColorComp.Color = color;
			((EntitySystem)this).Dirty(target, (IComponent)(object)fireColorComp, (MetaDataComponent)null);
		}
	}
}
