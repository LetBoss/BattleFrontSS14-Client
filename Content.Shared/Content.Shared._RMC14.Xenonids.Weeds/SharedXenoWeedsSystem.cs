using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Communications;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Xenonids.Construction.FloorResin;
using Content.Shared._RMC14.Xenonids.Construction.ResinHole;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Climbing.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Maps;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Weeds;

public abstract class SharedXenoWeedsSystem : EntitySystem
{
	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private IMapManager _map;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private ITileDefinitionManager _tile;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private readonly HashSet<EntityUid> _toUpdate = new HashSet<EntityUid>();

	private readonly HashSet<EntityUid> _intersecting = new HashSet<EntityUid>();

	private EntityQuery<AffectableByWeedsComponent> _affectedQuery;

	private EntityQuery<XenoWeedsComponent> _weedsQuery;

	private EntityQuery<ResinSlowdownModifierComponent> _slowResinQuery;

	private EntityQuery<ResinSpeedupModifierComponent> _fastResinQuery;

	private EntityQuery<XenoComponent> _xenoQuery;

	private EntityQuery<BlockWeedsComponent> _blockWeedsQuery;

	private EntityQuery<HiveMemberComponent> _hiveMemberQuery;

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
		_affectedQuery = ((EntitySystem)this).GetEntityQuery<AffectableByWeedsComponent>();
		_weedsQuery = ((EntitySystem)this).GetEntityQuery<XenoWeedsComponent>();
		_slowResinQuery = ((EntitySystem)this).GetEntityQuery<ResinSlowdownModifierComponent>();
		_fastResinQuery = ((EntitySystem)this).GetEntityQuery<ResinSpeedupModifierComponent>();
		_xenoQuery = ((EntitySystem)this).GetEntityQuery<XenoComponent>();
		_blockWeedsQuery = ((EntitySystem)this).GetEntityQuery<BlockWeedsComponent>();
		_hiveMemberQuery = ((EntitySystem)this).GetEntityQuery<HiveMemberComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedsComponent, AnchorStateChangedEvent>((EntityEventRefHandler<XenoWeedsComponent, AnchorStateChangedEvent>)OnWeedsAnchorChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedsComponent, ComponentShutdown>((EntityEventRefHandler<XenoWeedsComponent, ComponentShutdown>)OnModifierShutdown<XenoWeedsComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedsComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoWeedsComponent, EntityTerminatingEvent>)OnWeedsTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedsComponent, MapInitEvent>((EntityEventRefHandler<XenoWeedsComponent, MapInitEvent>)OnWeedsMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedsComponent, StartCollideEvent>((EntityEventRefHandler<XenoWeedsComponent, StartCollideEvent>)OnWeedsStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedsComponent, EndCollideEvent>((EntityEventRefHandler<XenoWeedsComponent, EndCollideEvent>)OnWeedsEndCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedsComponent, ExaminedEvent>((EntityEventRefHandler<XenoWeedsComponent, ExaminedEvent>)OnWeedsExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWallWeedsComponent, ComponentRemove>((EntityEventRefHandler<XenoWallWeedsComponent, ComponentRemove>)OnWallWeedsRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWallWeedsComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoWallWeedsComponent, EntityTerminatingEvent>)OnWallWeedsRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedableComponent, AnchorStateChangedEvent>((EntityEventRefHandler<XenoWeedableComponent, AnchorStateChangedEvent>)OnWeedableAnchorStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedableComponent, ComponentRemove>((EntityEventRefHandler<XenoWeedableComponent, ComponentRemove>)OnWeedableRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedableComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoWeedableComponent, EntityTerminatingEvent>)OnWeedableRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageOffWeedsComponent, MapInitEvent>((EntityEventRefHandler<DamageOffWeedsComponent, MapInitEvent>)OnDamageOffWeedsMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AffectableByWeedsComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<AffectableByWeedsComponent, RefreshMovementSpeedModifiersEvent>)WeedsRefreshPassiveSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AffectableByWeedsComponent, XenoOvipositorChangedEvent>((EntityEventRefHandler<AffectableByWeedsComponent, XenoOvipositorChangedEvent>)WeedsOvipositorChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedsSpreadingComponent, MapInitEvent>((EntityEventRefHandler<XenoWeedsSpreadingComponent, MapInitEvent>)OnSpreadingMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ResinSlowdownModifierComponent, ComponentShutdown>((EntityEventRefHandler<ResinSlowdownModifierComponent, ComponentShutdown>)OnModifierShutdown<ResinSlowdownModifierComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ResinSlowdownModifierComponent, StartCollideEvent>((EntityEventRefHandler<ResinSlowdownModifierComponent, StartCollideEvent>)OnResinSlowdownStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ResinSlowdownModifierComponent, EndCollideEvent>((EntityEventRefHandler<ResinSlowdownModifierComponent, EndCollideEvent>)OnResinSlowdownEndCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ResinSpeedupModifierComponent, ComponentShutdown>((EntityEventRefHandler<ResinSpeedupModifierComponent, ComponentShutdown>)OnModifierShutdown<ResinSpeedupModifierComponent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ResinSpeedupModifierComponent, StartCollideEvent>((EntityEventRefHandler<ResinSpeedupModifierComponent, StartCollideEvent>)OnResinSpeedupStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ResinSpeedupModifierComponent, EndCollideEvent>((EntityEventRefHandler<ResinSpeedupModifierComponent, EndCollideEvent>)OnResinSpeedupEndCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedPhysicsSystem));
	}

	private void OnWeedsExamined(Entity<XenoWeedsComponent> weeds, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.Examiner) || weeds.Comp.FruitGrowthMultiplier == 1f)
		{
			return;
		}
		using (args.PushGroup("XenoWeedsComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-fruit-weed-boost", (ValueTuple<string, object>)("percent", (int)(weeds.Comp.FruitGrowthMultiplier * 100f))));
		}
	}

	private void OnWeedsAnchorChanged(Entity<XenoWeedsComponent> weeds, ref AnchorStateChangedEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer && !((AnchorStateChangedEvent)(ref args)).Anchored)
		{
			((EntitySystem)this).QueueDel((EntityUid?)Entity<XenoWeedsComponent>.op_Implicit(weeds));
		}
	}

	private void OnModifierShutdown<T>(Entity<T> ent, ref ComponentShutdown args) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent phys = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(Entity<T>.op_Implicit(ent), ref phys))
		{
			_toUpdate.UnionWith(_physics.GetContactingEntities(Entity<T>.op_Implicit(ent), phys, false));
		}
	}

	private void OnWeedsTerminating(Entity<XenoWeedsComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.IsSource)
		{
			XenoWeedsComponent weeds = default(XenoWeedsComponent);
			if (_weedsQuery.TryComp(ent.Comp.Source, ref weeds))
			{
				weeds.Spread.Remove(Entity<XenoWeedsComponent>.op_Implicit(ent));
				((EntitySystem)this).Dirty(ent.Comp.Source.Value, (IComponent)(object)weeds, (MetaDataComponent)null);
			}
			{
				foreach (EntityUid weededEntity in ent.Comp.LocalWeeded)
				{
					if (!((EntitySystem)this).HasComp<CommunicationsTowerComponent>(weededEntity))
					{
						_appearance.SetData(weededEntity, (Enum)WeededEntityLayers.Layer, (object)false, (AppearanceComponent)null);
					}
				}
				return;
			}
		}
		XenoWeedsComponent weeds2 = default(XenoWeedsComponent);
		foreach (EntityUid spread in ent.Comp.Spread)
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(spread, (MetaDataComponent)null))
			{
				if (_weedsQuery.TryComp(spread, ref weeds2))
				{
					weeds2.Source = null;
					((EntitySystem)this).Dirty(spread, (IComponent)(object)weeds2, (MetaDataComponent)null);
				}
				((EntitySystem)this).EnsureComp<TimedDespawnComponent>(spread).Lifetime = (float)_random.Next(ent.Comp.MinRandomDelete, ent.Comp.MaxRandomDelete).TotalSeconds;
			}
		}
		ent.Comp.Spread.Clear();
		((EntitySystem)this).Dirty<XenoWeedsComponent>(ent, (MetaDataComponent)null);
	}

	private void OnWeedsMapInit(Entity<XenoWeedsComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		AffectableByWeedsComponent affected = default(AffectableByWeedsComponent);
		foreach (EntityUid intersecting in _physics.GetEntitiesIntersectingBody(Entity<XenoWeedsComponent>.op_Implicit(ent), 65, true, (PhysicsComponent)null, (FixturesComponent)null, (TransformComponent)null))
		{
			if (_affectedQuery.TryComp(intersecting, ref affected) && !affected.OnXenoWeeds)
			{
				_toUpdate.Add(intersecting);
			}
		}
	}

	private void OnWeedsStartCollide(Entity<XenoWeedsComponent> ent, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EntityUid other = args.OtherEntity;
		AffectableByWeedsComponent affected = default(AffectableByWeedsComponent);
		if (_affectedQuery.TryComp(other, ref affected) && !affected.OnXenoWeeds)
		{
			_toUpdate.Add(other);
		}
	}

	private void OnWeedsEndCollide(Entity<XenoWeedsComponent> ent, ref EndCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EntityUid other = args.OtherEntity;
		AffectableByWeedsComponent affected = default(AffectableByWeedsComponent);
		if (_affectedQuery.TryComp(other, ref affected) && affected.OnXenoWeeds)
		{
			_toUpdate.Add(other);
		}
	}

	private void OnWallWeedsRemove<T>(Entity<XenoWallWeedsComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		XenoWeedsComponent weeds = default(XenoWeedsComponent);
		if (((EntitySystem)this).TryComp<XenoWeedsComponent>(ent.Comp.Weeds, ref weeds))
		{
			weeds.Spread.Remove(Entity<XenoWallWeedsComponent>.op_Implicit(ent));
			((EntitySystem)this).Dirty(ent.Comp.Weeds.Value, (IComponent)(object)weeds, (MetaDataComponent)null);
		}
	}

	private void OnWeedableAnchorStateChanged(Entity<XenoWeedableComponent> weedable, ref AnchorStateChangedEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer && !((AnchorStateChangedEvent)(ref args)).Anchored)
		{
			((EntitySystem)this).QueueDel(weedable.Comp.Entity);
		}
	}

	private void OnWeedableRemove<T>(Entity<XenoWeedableComponent> weedable, ref T args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer && weedable.Comp.Entity.HasValue)
		{
			((EntitySystem)this).QueueDel(weedable.Comp.Entity);
		}
	}

	private void OnDamageOffWeedsMapInit(Entity<DamageOffWeedsComponent> damage, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		damage.Comp.DamageAt = _timing.CurTime + damage.Comp.Every;
	}

	private void WeedsRefreshPassiveSpeed(Entity<AffectableByWeedsComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physicsComponent = default(PhysicsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(Entity<AffectableByWeedsComponent>.op_Implicit(ent), ref physicsComponent))
		{
			return;
		}
		float speedWeeds = 0f;
		float speedResin = 0f;
		bool isXeno = _xenoQuery.HasComp(Entity<AffectableByWeedsComponent>.op_Implicit(ent));
		HiveMemberComponent hive = default(HiveMemberComponent);
		_hiveMemberQuery.TryComp(Entity<AffectableByWeedsComponent>.op_Implicit(ent), ref hive);
		bool anyWeeds = false;
		bool anySlowResin = false;
		bool anyFastResin = false;
		bool friendlyWeeds = false;
		int entriesResin = 0;
		int entriesWeeds = 0;
		_intersecting.Clear();
		_physics.GetContactingEntities(Entity<PhysicsComponent>.op_Implicit((Entity<AffectableByWeedsComponent>.op_Implicit(ent), physicsComponent)), _intersecting, false);
		TransformComponent transform = default(TransformComponent);
		if (((EntitySystem)this).TryComp(Entity<AffectableByWeedsComponent>.op_Implicit(ent), ref transform) && transform.Anchored)
		{
			RMCAnchoredEntitiesEnumerator anchoredQuery = _rmcMap.GetAnchoredEntitiesEnumerator(Entity<AffectableByWeedsComponent>.op_Implicit(ent), null, (DirectionFlag)0);
			EntityUid anchored;
			while (anchoredQuery.MoveNext(out anchored))
			{
				_intersecting.Add(anchored);
			}
		}
		ResinSlowdownModifierComponent slowResin = default(ResinSlowdownModifierComponent);
		ResinSpeedupModifierComponent fastResin = default(ResinSpeedupModifierComponent);
		XenoWeedsComponent weeds = default(XenoWeedsComponent);
		foreach (EntityUid contacting in _intersecting)
		{
			if (_slowResinQuery.TryComp(contacting, ref slowResin))
			{
				if (hive == null || !_hive.IsMember(Entity<HiveMemberComponent>.op_Implicit(contacting), hive.Hive))
				{
					speedResin = ((!((EntitySystem)this).HasComp<RMCArmorSpeedTierUserComponent>(contacting)) ? (speedResin + slowResin.OutsiderSpeedModifier) : (speedResin + slowResin.OutsiderSpeedModifierArmor));
					entriesResin++;
				}
				anySlowResin = true;
			}
			else if (_fastResinQuery.TryComp(contacting, ref fastResin))
			{
				if (isXeno && hive != null && _hive.IsMember(Entity<HiveMemberComponent>.op_Implicit(contacting), hive.Hive))
				{
					speedResin += fastResin.HiveSpeedModifier;
					entriesResin++;
				}
				anyFastResin = true;
			}
			else if (_weedsQuery.TryComp(contacting, ref weeds))
			{
				anyWeeds = true;
				if (isXeno && hive != null && _hive.IsMember(Entity<HiveMemberComponent>.op_Implicit(contacting), hive.Hive))
				{
					speedWeeds += weeds.SpeedMultiplierXeno;
					friendlyWeeds = true;
					entriesWeeds++;
				}
				else if (hive == null || !_hive.IsMember(Entity<HiveMemberComponent>.op_Implicit(contacting), hive.Hive))
				{
					speedWeeds = ((!((EntitySystem)this).HasComp<RMCArmorSpeedTierUserComponent>(contacting)) ? (speedWeeds + weeds.SpeedMultiplierOutsider) : (speedWeeds + weeds.SpeedMultiplierOutsiderArmor));
					entriesWeeds++;
				}
			}
		}
		if (!anyWeeds && ((EntitySystem)this).Transform(Entity<AffectableByWeedsComponent>.op_Implicit(ent)).Anchored && _rmcMap.HasAnchoredEntityEnumerator<XenoWeedsComponent>(ent.Owner.ToCoordinates(), (Direction?)null, (DirectionFlag)0))
		{
			anyWeeds = true;
		}
		float finalSpeed = 1f;
		if (entriesWeeds > 0)
		{
			speedWeeds /= (float)entriesWeeds;
		}
		if (entriesResin > 0)
		{
			speedResin /= (float)entriesResin;
		}
		if ((speedWeeds > 1f || speedResin > 1f) && entriesResin > 0 && entriesWeeds > 0)
		{
			finalSpeed = speedWeeds * speedResin;
		}
		else if (entriesResin > 0)
		{
			finalSpeed = speedResin;
		}
		else if (entriesWeeds > 0)
		{
			finalSpeed = speedWeeds;
		}
		args.ModifySpeed(finalSpeed, finalSpeed);
		ent.Comp.OnXenoWeeds = anyWeeds;
		ent.Comp.OnFriendlyWeeds = friendlyWeeds;
		ent.Comp.OnXenoSlowResin = anySlowResin;
		ent.Comp.OnXenoFastResin = anyFastResin;
		((EntitySystem)this).Dirty<AffectableByWeedsComponent>(ent, (MetaDataComponent)null);
	}

	private void WeedsOvipositorChanged(Entity<AffectableByWeedsComponent> ent, ref XenoOvipositorChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		AffectableByWeedsComponent affected = default(AffectableByWeedsComponent);
		if (_affectedQuery.TryComp(Entity<AffectableByWeedsComponent>.op_Implicit(ent), ref affected) && !affected.OnXenoSlowResin)
		{
			_toUpdate.Add(Entity<AffectableByWeedsComponent>.op_Implicit(ent));
		}
	}

	public bool HasWeedsNearby(Entity<MapGridComponent> grid, EntityCoordinates coordinates, int range = 5)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Vector2i position = _mapSystem.LocalToTile(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), coordinates);
		Box2 checkArea = default(Box2);
		((Box2)(ref checkArea))._002Ector((float)(position.X - range + 1), (float)(position.Y - range + 1), (float)(position.X + range), (float)(position.Y + range));
		XenoWeedsComponent weeds = default(XenoWeedsComponent);
		foreach (EntityUid anchored in _mapSystem.GetLocalAnchoredEntities(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), checkArea))
		{
			if (((EntitySystem)this).TryComp<XenoWeedsComponent>(anchored, ref weeds) && weeds.IsSource)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsOnHiveWeeds(Entity<MapGridComponent> grid, EntityCoordinates coordinates, bool sourceOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Entity<XenoWeedsComponent>? weed = GetWeedsOnFloor(grid, coordinates, sourceOnly);
		Entity<XenoWeedsComponent>? val = weed;
		XenoWeedsComponent weedComp = default(XenoWeedsComponent);
		if (!((EntitySystem)this).TryComp<XenoWeedsComponent>(val.HasValue ? new EntityUid?(Entity<XenoWeedsComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null), ref weedComp))
		{
			return false;
		}
		EntityPrototype spawns = default(EntityPrototype);
		if (_prototype.TryIndex(weedComp.Spawns, ref spawns))
		{
			return spawns.HasComponent<HiveWeedsComponent>((IComponentFactory?)null);
		}
		return false;
	}

	public bool IsOnWeeds(Entity<MapGridComponent> grid, EntityCoordinates coordinates, bool sourceOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetWeedsOnFloor(grid, coordinates, sourceOnly).HasValue;
	}

	public Entity<XenoWeedsComponent>? GetWeedsOnFloor(Entity<MapGridComponent> grid, EntityCoordinates coordinates, bool sourceOnly = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Vector2i position = _mapSystem.LocalToTile(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), coordinates);
		AnchoredEntitiesEnumerator enumerator = _mapSystem.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), position);
		EntityUid? anchored = default(EntityUid?);
		XenoWeedsComponent weeds = default(XenoWeedsComponent);
		while (((AnchoredEntitiesEnumerator)(ref enumerator)).MoveNext(ref anchored))
		{
			if (_weedsQuery.TryComp(anchored, ref weeds) && (!sourceOnly || weeds.IsSource))
			{
				return Entity<XenoWeedsComponent>.op_Implicit((anchored.Value, weeds));
			}
		}
		return null;
	}

	public EntityUid? GetWeedsOnFloor(EntityCoordinates coordinates, bool sourceOnly = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				Entity<XenoWeedsComponent>? weedsOnFloor = GetWeedsOnFloor(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), coordinates, sourceOnly);
				if (!weedsOnFloor.HasValue)
				{
					return null;
				}
				return Entity<XenoWeedsComponent>.op_Implicit(weedsOnFloor.GetValueOrDefault());
			}
		}
		return null;
	}

	public bool IsOnWeeds(Entity<TransformComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(Entity<TransformComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		EntityCoordinates coordinates = _rmcMap.SnapToGrid(_transform.GetMoverCoordinates(Entity<TransformComponent>.op_Implicit(entity), entity.Comp));
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (grid.HasValue)
		{
			EntityUid gridUid = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref grid2))
			{
				return IsOnWeeds(Entity<MapGridComponent>.op_Implicit((gridUid, grid2)), coordinates);
			}
		}
		return false;
	}

	public bool IsOnFriendlyWeeds(Entity<TransformComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(Entity<TransformComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		EntityCoordinates coordinates = _rmcMap.SnapToGrid(_transform.GetMoverCoordinates(Entity<TransformComponent>.op_Implicit(entity), entity.Comp));
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (grid.HasValue)
		{
			EntityUid gridUid = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref grid2))
			{
				Entity<XenoWeedsComponent>? weeds = GetWeedsOnFloor(Entity<MapGridComponent>.op_Implicit((gridUid, grid2)), coordinates);
				if (!weeds.HasValue)
				{
					return false;
				}
				if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(entity.Owner), Entity<HiveMemberComponent>.op_Implicit(weeds.Value.Owner)))
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}

	private void OnResinSlowdownStartCollide(Entity<ResinSlowdownModifierComponent> ent, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EntityUid other = args.OtherEntity;
		AffectableByWeedsComponent affected = default(AffectableByWeedsComponent);
		if (_affectedQuery.TryComp(other, ref affected) && !affected.OnXenoSlowResin)
		{
			_toUpdate.Add(other);
		}
	}

	private void OnResinSlowdownEndCollide(Entity<ResinSlowdownModifierComponent> ent, ref EndCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EntityUid other = args.OtherEntity;
		AffectableByWeedsComponent affected = default(AffectableByWeedsComponent);
		if (_affectedQuery.TryComp(other, ref affected) && affected.OnXenoSlowResin)
		{
			_toUpdate.Add(other);
		}
	}

	private void OnResinSpeedupStartCollide(Entity<ResinSpeedupModifierComponent> ent, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EntityUid other = args.OtherEntity;
		AffectableByWeedsComponent affected = default(AffectableByWeedsComponent);
		if (_affectedQuery.TryComp(other, ref affected) && !affected.OnXenoFastResin)
		{
			_toUpdate.Add(other);
		}
	}

	private void OnResinSpeedupEndCollide(Entity<ResinSpeedupModifierComponent> ent, ref EndCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EntityUid other = args.OtherEntity;
		AffectableByWeedsComponent affected = default(AffectableByWeedsComponent);
		if (_affectedQuery.TryComp(other, ref affected) && affected.OnXenoFastResin)
		{
			_toUpdate.Add(other);
		}
	}

	private void OnSpreadingMapInit(Entity<XenoWeedsSpreadingComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.SpreadAt = _timing.CurTime + ent.Comp.SpreadDelay;
		((EntitySystem)this).Dirty<XenoWeedsSpreadingComponent>(ent, (MetaDataComponent)null);
	}

	public bool CanSpreadWeedsPopup(Entity<MapGridComponent> grid, Vector2i tile, EntityUid? user, bool semiWeedable = false, bool source = false)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		TileRef tileRef = default(TileRef);
		ITileDefinition tileDef = default(ITileDefinition);
		if (!_mapSystem.TryGetTileRef(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), tile, ref tileRef) || !_tile.TryGetDefinition(tileRef.Tile.TypeId, ref tileDef) || ((IPrototype)tileDef).ID == "Space" || (tileDef is ContentTileDefinition { WeedsSpreadable: false } && !(tileDef is ContentTileDefinition contentTileDefinition2 && contentTileDefinition2.SemiWeedable && semiWeedable)))
		{
			GenericPopup();
			return false;
		}
		if (!_area.CanResinPopup(Entity<MapGridComponent, AreaGridComponent>.op_Implicit((ValueTuple<EntityUid, MapGridComponent, AreaGridComponent>)(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), null)), tile, user))
		{
			return false;
		}
		AnchoredEntitiesEnumerator targetTileAnchored = _mapSystem.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), tile);
		EntityUid? uid = default(EntityUid?);
		while (((AnchoredEntitiesEnumerator)(ref targetTileAnchored)).MoveNext(ref uid))
		{
			if (_blockWeedsQuery.HasComp(uid))
			{
				return false;
			}
			if (source && ((EntitySystem)this).HasComp<XenoResinHoleComponent>(uid))
			{
				return false;
			}
		}
		return true;
		void GenericPopup()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (user.HasValue)
			{
				string msg = base.Loc.GetString("cm-xeno-construction-failed-weeds");
				_popup.PopupClient(msg, user.Value, user.Value, PopupType.SmallCaution);
			}
		}
	}

	public bool CanPlaceWeedsPopup(EntityUid xeno, Entity<MapGridComponent> grid, EntityCoordinates coordinates, bool limitDistance)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if (_rmcMap.HasAnchoredEntityEnumerator<XenoWeedsComponent>(coordinates, out Entity<XenoWeedsComponent> oldWeeds, (Direction?)null, (DirectionFlag)0))
		{
			if (oldWeeds.Comp.IsSource)
			{
				_popup.PopupClient("There's a pod here already!", Entity<XenoWeedsComponent>.op_Implicit(oldWeeds), xeno, PopupType.SmallCaution);
				return false;
			}
			if (oldWeeds.Comp.BlockOtherWeeds)
			{
				_popup.PopupClient("These weeds are too strong to plant a node on!", Entity<XenoWeedsComponent>.op_Implicit(oldWeeds), xeno, PopupType.SmallCaution);
				return false;
			}
		}
		if (limitDistance && !HasWeedsNearby(grid, coordinates))
		{
			_popup.PopupClient("We can only plant weed nodes near other weed nodes our hive owns!", xeno, xeno, PopupType.SmallCaution);
			return false;
		}
		foreach (EntityUid entity in _mapSystem.GetAnchoredEntities(grid, ((EntityCoordinates)(ref coordinates)).ToVector2i((IEntityManager)(object)base.EntityManager, _map, _transform)))
		{
			if ((((EntitySystem)this).HasComp<ClimbableComponent>(entity) || ((EntitySystem)this).HasComp<RMCReactorPoweredLightComponent>(entity)) && !((EntitySystem)this).HasComp<BarricadeComponent>(entity))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-weeds-blocked"), xeno, xeno, PopupType.SmallCaution);
				return false;
			}
		}
		return true;
	}

	public void UpdateQueued(EntityUid update)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(update);
	}

	public override void Update(float frameTime)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (EntityUid mobId in _toUpdate)
			{
				UpdateQueued(mobId);
			}
		}
		finally
		{
			_toUpdate.Clear();
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<DamageOffWeedsComponent, DamageableComponent> query = ((EntitySystem)this).EntityQueryEnumerator<DamageOffWeedsComponent, DamageableComponent>();
		EntityUid uid = default(EntityUid);
		DamageOffWeedsComponent damage = default(DamageOffWeedsComponent);
		DamageableComponent damageable = default(DamageableComponent);
		AffectableByWeedsComponent affected = default(AffectableByWeedsComponent);
		BaseContainer container = default(BaseContainer);
		while (query.MoveNext(ref uid, ref damage, ref damageable))
		{
			if ((((EntitySystem)this).TryComp<AffectableByWeedsComponent>(uid, ref affected) && affected.OnXenoWeeds) || ((EntitySystem)this).HasComp<InXenoTunnelComponent>(uid))
			{
				if (damage.DamageAt.HasValue)
				{
					damage.DamageAt = null;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)damage, (MetaDataComponent)null);
				}
				continue;
			}
			if (!damage.DamageAt.HasValue)
			{
				damage.DamageAt = time + damage.Every;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)damage, (MetaDataComponent)null);
			}
			TimeSpan value = time;
			TimeSpan? damageAt = damage.DamageAt;
			if (!(value < damageAt))
			{
				damage.DamageAt = time + damage.Every;
				if ((!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(uid, null)), ref container) || !_xenoQuery.HasComp(container.Owner)) && (!damage.RestingStopsDamage || !((EntitySystem)this).HasComp<XenoRestingComponent>(uid)))
				{
					_damageable.TryChangeDamage(uid, damage.Damage, ignoreResistances: false, interruptsDoAfters: true, damageable);
				}
			}
		}
	}
}
