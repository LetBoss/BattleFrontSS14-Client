using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Actions.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Lock;
using Content.Shared.Magic.Components;
using Content.Shared.Magic.Events;
using Content.Shared.Maps;
using Content.Shared.Mind;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Speech.Muting;
using Content.Shared.Storage;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Spawners;

namespace Content.Shared.Magic;

public abstract class SharedMagicSystem : EntitySystem
{
	[Dependency]
	private ISerializationManager _seriMan;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedGunSystem _gunSystem;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedBodySystem _body;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedDoorSystem _door;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private LockSystem _lock;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private TurfSystem _turf;

	private static readonly ProtoId<TagPrototype> InvalidForGlobalSpawnSpellTag = ProtoId<TagPrototype>.op_Implicit("InvalidForGlobalSpawnSpell");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MagicComponent, BeforeCastSpellEvent>((EntityEventRefHandler<MagicComponent, BeforeCastSpellEvent>)OnBeforeCastSpell, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InstantSpawnSpellEvent>((EntityEventHandler<InstantSpawnSpellEvent>)OnInstantSpawn, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TeleportSpellEvent>((EntityEventHandler<TeleportSpellEvent>)OnTeleportSpell, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WorldSpawnSpellEvent>((EntityEventHandler<WorldSpawnSpellEvent>)OnWorldSpawn, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileSpellEvent>((EntityEventHandler<ProjectileSpellEvent>)OnProjectileSpell, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChangeComponentsSpellEvent>((EntityEventHandler<ChangeComponentsSpellEvent>)OnChangeComponentsSpell, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SmiteSpellEvent>((EntityEventHandler<SmiteSpellEvent>)OnSmiteSpell, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KnockSpellEvent>((EntityEventHandler<KnockSpellEvent>)OnKnockSpell, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChargeSpellEvent>((EntityEventHandler<ChargeSpellEvent>)OnChargeSpell, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RandomGlobalSpawnSpellEvent>((EntityEventHandler<RandomGlobalSpawnSpellEvent>)OnRandomGlobalSpawnSpell, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MindSwapSpellEvent>((EntityEventHandler<MindSwapSpellEvent>)OnMindSwapSpell, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VoidApplauseSpellEvent>((EntityEventHandler<VoidApplauseSpellEvent>)OnVoidApplause, (Type[])null, (Type[])null);
	}

	private void OnBeforeCastSpell(Entity<MagicComponent> ent, ref BeforeCastSpellEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		MagicComponent comp = ent.Comp;
		bool hasReqs = true;
		if (comp.RequiresClothes)
		{
			InventorySystem.InventorySlotEnumerator enumerator = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(args.Performer), SlotFlags.HEAD | SlotFlags.OUTERCLOTHING);
			ContainerSlot containerSlot;
			while (enumerator.MoveNext(out containerSlot))
			{
				EntityUid? containedEntity = containerSlot.ContainedEntity;
				if (containedEntity.HasValue)
				{
					EntityUid item = containedEntity.GetValueOrDefault();
					hasReqs = ((EntitySystem)this).HasComp<WizardClothesComponent>(item);
				}
				else
				{
					hasReqs = false;
				}
				if (!hasReqs)
				{
					break;
				}
			}
		}
		if (comp.RequiresSpeech && ((EntitySystem)this).HasComp<MutedComponent>(args.Performer))
		{
			hasReqs = false;
		}
		if (!hasReqs)
		{
			args.Cancelled = true;
			_popup.PopupClient(base.Loc.GetString("spell-requirements-failed"), args.Performer, args.Performer);
		}
	}

	private bool PassesSpellPrerequisites(EntityUid spell, EntityUid performer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		BeforeCastSpellEvent ev = new BeforeCastSpellEvent(performer);
		((EntitySystem)this).RaiseLocalEvent<BeforeCastSpellEvent>(spell, ref ev, false);
		return !ev.Cancelled;
	}

	private void OnInstantSpawn(InstantSpawnSpellEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(args.Action), args.Performer))
		{
			return;
		}
		TransformComponent transform = ((EntitySystem)this).Transform(args.Performer);
		foreach (EntityCoordinates position in GetInstantSpawnPositions(transform, args.PosData))
		{
			string proto = EntProtoId.op_Implicit(args.Prototype);
			EntityUid performer = args.Performer;
			bool preventCollideWithCaster = args.PreventCollideWithCaster;
			SpawnSpellHelper(proto, position, performer, null, preventCollideWithCaster);
		}
		((HandledEntityEventArgs)args).Handled = true;
	}

	private List<EntityCoordinates> GetInstantSpawnPositions(TransformComponent casterXform, MagicInstantSpawnData data)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected I4, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		if (!(data is TargetCasterPos))
		{
			EntityCoordinates coordinates;
			Angle localRotation;
			if (!(data is TargetInFrontSingle))
			{
				if (data is TargetInFront)
				{
					coordinates = casterXform.Coordinates;
					localRotation = casterXform.LocalRotation;
					EntityCoordinates directionPos = ((EntityCoordinates)(ref coordinates)).Offset(Vector2Helpers.Normalized(((Angle)(ref localRotation)).ToWorldVec()));
					MapGridComponent mapGrid = default(MapGridComponent);
					if (!((EntitySystem)this).TryComp<MapGridComponent>(casterXform.GridUid, ref mapGrid))
					{
						return new List<EntityCoordinates>();
					}
					if (!_turf.TryGetTileRef(directionPos, out var tileReference))
					{
						return new List<EntityCoordinates>();
					}
					Vector2i tileIndex = tileReference.Value.GridIndices;
					EntityCoordinates coords = _mapSystem.GridTileToLocal(casterXform.GridUid.Value, mapGrid, tileIndex);
					localRotation = casterXform.LocalRotation;
					Direction dir = ((Angle)(ref localRotation)).GetCardinalDir();
					switch ((int)dir)
					{
					case 0:
					case 4:
					{
						EntityCoordinates coordsPlus = _mapSystem.GridTileToLocal(casterXform.GridUid.Value, mapGrid, tileIndex + Vector2i.op_Implicit((1, 0)));
						EntityCoordinates coordsMinus = _mapSystem.GridTileToLocal(casterXform.GridUid.Value, mapGrid, tileIndex + Vector2i.op_Implicit((-1, 0)));
						return new List<EntityCoordinates>(3) { coords, coordsPlus, coordsMinus };
					}
					case 2:
					case 6:
					{
						EntityCoordinates coordsPlus = _mapSystem.GridTileToLocal(casterXform.GridUid.Value, mapGrid, tileIndex + Vector2i.op_Implicit((0, 1)));
						EntityCoordinates coordsMinus = _mapSystem.GridTileToLocal(casterXform.GridUid.Value, mapGrid, tileIndex + Vector2i.op_Implicit((0, -1)));
						return new List<EntityCoordinates>(3) { coords, coordsPlus, coordsMinus };
					}
					default:
						return new List<EntityCoordinates>();
					}
				}
				throw new ArgumentOutOfRangeException();
			}
			coordinates = casterXform.Coordinates;
			localRotation = casterXform.LocalRotation;
			EntityCoordinates directionPos2 = ((EntityCoordinates)(ref coordinates)).Offset(Vector2Helpers.Normalized(((Angle)(ref localRotation)).ToWorldVec()));
			MapGridComponent mapGrid2 = default(MapGridComponent);
			if (!((EntitySystem)this).TryComp<MapGridComponent>(casterXform.GridUid, ref mapGrid2))
			{
				return new List<EntityCoordinates>();
			}
			if (!_turf.TryGetTileRef(directionPos2, out var tileReference2))
			{
				return new List<EntityCoordinates>();
			}
			Vector2i tileIndex2 = tileReference2.Value.GridIndices;
			return new List<EntityCoordinates>(1) { _mapSystem.GridTileToLocal(casterXform.GridUid.Value, mapGrid2, tileIndex2) };
		}
		return new List<EntityCoordinates>(1) { casterXform.Coordinates };
	}

	private void OnWorldSpawn(WorldSpawnSpellEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(args.Action), args.Performer))
		{
			EntityCoordinates targetMapCoords = args.Target;
			WorldSpawnSpellHelper(args.Prototypes, targetMapCoords, args.Performer, args.Lifetime, args.Offset);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void WorldSpawnSpellHelper(List<EntitySpawnEntry> entityEntries, EntityCoordinates entityCoords, EntityUid performer, float? lifetime, Vector2 offsetVector2)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		List<string> spawns = EntitySpawnCollection.GetSpawns(entityEntries, _random);
		EntityCoordinates offsetCoords = entityCoords;
		foreach (string proto in spawns)
		{
			SpawnSpellHelper(proto, offsetCoords, performer, lifetime);
			offsetCoords = ((EntityCoordinates)(ref offsetCoords)).Offset(offsetVector2);
		}
	}

	private void OnProjectileSpell(ProjectileSpellEvent ev)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)ev).Handled && PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(ev.Action), ev.Performer) && _net.IsServer)
		{
			((HandledEntityEventArgs)ev).Handled = true;
			EntityCoordinates fromCoords = ((EntitySystem)this).Transform(ev.Performer).Coordinates;
			EntityCoordinates toCoords = ev.Target;
			Vector2 userVelocity = _physics.GetMapLinearVelocity(ev.Performer, (PhysicsComponent)null, (TransformComponent)null);
			MapCoordinates fromMap = _transform.ToMapCoordinates(fromCoords, true);
			EntityUid ent = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ev.Prototype), fromMap, (ComponentRegistry)null, default(Angle));
			Vector2 direction = _transform.ToMapCoordinates(toCoords, true).Position - fromMap.Position;
			_gunSystem.ShootProjectile(ent, direction, userVelocity, ev.Performer, ev.Performer);
		}
	}

	private void OnChangeComponentsSpell(ChangeComponentsSpellEvent ev)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)ev).Handled && PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(ev.Action), ev.Performer))
		{
			((HandledEntityEventArgs)ev).Handled = true;
			RemoveComponents(ev.Target, ev.ToRemove);
			AddComponents(ev.Target, ev.ToAdd);
		}
	}

	private void OnTeleportSpell(TeleportSpellEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(args.Action), args.Performer))
		{
			TransformComponent transform = ((EntitySystem)this).Transform(args.Performer);
			if (!(transform.MapID != _transform.GetMapId(args.Target)) && _interaction.InRangeUnobstructed(args.Performer, args.Target, 1000f, CollisionGroup.Opaque, null, popup: true))
			{
				_transform.SetCoordinates(args.Performer, args.Target);
				_transform.AttachToGridOrMap(args.Performer, transform);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	public virtual void OnVoidApplause(VoidApplauseSpellEvent ev)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)ev).Handled && PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(ev.Action), ev.Performer))
		{
			((HandledEntityEventArgs)ev).Handled = true;
			_transform.SwapPositions(Entity<TransformComponent>.op_Implicit(ev.Performer), Entity<TransformComponent>.op_Implicit(ev.Target));
		}
	}

	private void SpawnSpellHelper(string? proto, EntityCoordinates position, EntityUid performer, float? lifetime = null, bool preventCollide = false)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			EntityUid ent = ((EntitySystem)this).Spawn(proto, position.SnapToGrid((IEntityManager?)(object)base.EntityManager, _mapManager));
			if (lifetime.HasValue)
			{
				((EntitySystem)this).EnsureComp<TimedDespawnComponent>(ent).Lifetime = lifetime.Value;
			}
			if (preventCollide)
			{
				((EntitySystem)this).EnsureComp<PreventCollideComponent>(ent).Uid = performer;
			}
		}
	}

	private void AddComponents(EntityUid target, ComponentRegistry comps)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		foreach (var (name, data) in (Dictionary<string, ComponentRegistryEntry>)(object)comps)
		{
			if (!((EntitySystem)this).HasComp(target, ((object)data.Component).GetType()))
			{
				object temp = (object)(Component)((EntitySystem)this).Factory.GetComponent(name, false);
				_seriMan.CopyTo((object)data.Component, ref temp, (ISerializationContext)null, false, false);
				((EntitySystem)this).AddComp<Component>(target, (Component)temp, false);
			}
		}
	}

	private void RemoveComponents(EntityUid target, HashSet<string> comps)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		ComponentRegistration registration = default(ComponentRegistration);
		foreach (string toRemove in comps)
		{
			if (((EntitySystem)this).Factory.TryGetRegistration(toRemove, ref registration, false))
			{
				((EntitySystem)this).RemComp(target, registration.Type);
			}
		}
	}

	private void OnSmiteSpell(SmiteSpellEvent ev)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)ev).Handled && PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(ev.Action), ev.Performer))
		{
			((HandledEntityEventArgs)ev).Handled = true;
			Vector2 impulseVector = (_transform.GetMapCoordinates(ev.Target, ((EntitySystem)this).Transform(ev.Target)).Position - _transform.GetMapCoordinates(ev.Performer, ((EntitySystem)this).Transform(ev.Performer)).Position) * 10000f;
			_physics.ApplyLinearImpulse(ev.Target, impulseVector, (FixturesComponent)null, (PhysicsComponent)null);
			BodyComponent body = default(BodyComponent);
			if (((EntitySystem)this).TryComp<BodyComponent>(ev.Target, ref body))
			{
				_body.GibBody(ev.Target, gibOrgans: true, body);
			}
		}
	}

	private void OnKnockSpell(KnockSpellEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(args.Action), args.Performer))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		TransformComponent transform = ((EntitySystem)this).Transform(args.Performer);
		DoorBoltComponent doorBoltComp = default(DoorBoltComponent);
		DoorComponent doorComp = default(DoorComponent);
		LockComponent lockComp = default(LockComponent);
		foreach (EntityUid target in _lookup.GetEntitiesInRange(_transform.GetMapCoordinates(args.Performer, transform), args.Range, (LookupFlags)6))
		{
			if (_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.Performer), Entity<TransformComponent>.op_Implicit(target), 0f, CollisionGroup.Opaque))
			{
				if (((EntitySystem)this).TryComp<DoorBoltComponent>(target, ref doorBoltComp) && doorBoltComp.BoltsDown)
				{
					_door.SetBoltsDown(Entity<DoorBoltComponent>.op_Implicit((target, doorBoltComp)), value: false, null, predicted: true);
				}
				if (((EntitySystem)this).TryComp<DoorComponent>(target, ref doorComp) && doorComp.State != DoorState.Open)
				{
					_door.StartOpening(target);
				}
				if (((EntitySystem)this).TryComp<LockComponent>(target, ref lockComp) && lockComp.Locked)
				{
					_lock.Unlock(target, args.Performer, lockComp);
				}
			}
		}
	}

	private void OnChargeSpell(ChargeSpellEvent ev)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent handsComp = default(HandsComponent);
		if (((HandledEntityEventArgs)ev).Handled || !PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(ev.Action), ev.Performer) || !((EntitySystem)this).TryComp<HandsComponent>(ev.Performer, ref handsComp))
		{
			return;
		}
		EntityUid? wand = null;
		foreach (EntityUid item in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((ev.Performer, handsComp))))
		{
			if (_tag.HasTag(item, ProtoId<TagPrototype>.op_Implicit(ev.WandTag)))
			{
				wand = item;
			}
		}
		((HandledEntityEventArgs)ev).Handled = true;
		BasicEntityAmmoProviderComponent basicAmmoComp = default(BasicEntityAmmoProviderComponent);
		if (wand.HasValue && ((EntitySystem)this).TryComp<BasicEntityAmmoProviderComponent>(wand, ref basicAmmoComp) && basicAmmoComp.Count.HasValue)
		{
			_gunSystem.UpdateBasicEntityAmmoCount(wand.Value, basicAmmoComp.Count.Value + ev.Charge, basicAmmoComp);
		}
	}

	protected virtual void OnRandomGlobalSpawnSpell(RandomGlobalSpawnSpellEvent ev)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsServer || ((HandledEntityEventArgs)ev).Handled || !PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(ev.Action), ev.Performer))
		{
			return;
		}
		List<EntitySpawnEntry> spawns = ev.Spawns;
		if (spawns == null)
		{
			return;
		}
		((HandledEntityEventArgs)ev).Handled = true;
		foreach (Entity<MindComponent> human in _mind.GetAliveHumans())
		{
			if (!human.Comp.OwnedEntity.HasValue)
			{
				continue;
			}
			EntityUid ent = human.Comp.OwnedEntity.Value;
			if (_tag.HasTag(ent, InvalidForGlobalSpawnSpellTag))
			{
				continue;
			}
			MapCoordinates mapCoords = _transform.GetMapCoordinates(ent, (TransformComponent)null);
			foreach (string spawn in EntitySpawnCollection.GetSpawns(spawns, _random))
			{
				EntityUid spawned = ((EntitySystem)this).Spawn(spawn, mapCoords, (ComponentRegistry)null, default(Angle));
				_hands.PickupOrDrop(ent, spawned);
			}
		}
		_audio.PlayGlobal(ev.Sound, ev.Performer, (AudioParams?)null);
	}

	private void OnMindSwapSpell(MindSwapSpellEvent ev)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)ev).Handled || !PassesSpellPrerequisites(Entity<ActionComponent>.op_Implicit(ev.Action), ev.Performer))
		{
			return;
		}
		((HandledEntityEventArgs)ev).Handled = true;
		if (_mind.TryGetMind(ev.Performer, out EntityUid perMind, out MindComponent _))
		{
			EntityUid tarMind;
			MindComponent tarMindComp;
			bool num = _mind.TryGetMind(ev.Target, out tarMind, out tarMindComp);
			_mind.TransferTo(perMind, ev.Target);
			if (num)
			{
				_mind.TransferTo(tarMind, ev.Performer);
			}
			_stun.TryParalyze(ev.Target, ev.TargetStunDuration, refresh: true);
			_stun.TryParalyze(ev.Performer, ev.PerformerStunDuration, refresh: true);
		}
	}
}
