using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared.Administration.Logs;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Construction;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.RCD.Components;
using Content.Shared.Tag;
using Content.Shared.Tiles;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Prototypes;

namespace Content.Shared.RCD.Systems;

public sealed class RCDSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private ITileDefinitionManager _tileDefMan;

	[Dependency]
	private FloorTileSystem _floors;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedChargesSystem _sharedCharges;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TagSystem _tags;

	private readonly int _instantConstructionDelay;

	private readonly EntProtoId _instantConstructionFx = EntProtoId.op_Implicit("EffectRCDConstruct0");

	private readonly ProtoId<RCDPrototype> _deconstructTileProto = ProtoId<RCDPrototype>.op_Implicit("DeconstructTile");

	private readonly ProtoId<RCDPrototype> _deconstructLatticeProto = ProtoId<RCDPrototype>.op_Implicit("DeconstructLattice");

	private static readonly ProtoId<TagPrototype> CatwalkTag = ProtoId<TagPrototype>.op_Implicit("Catwalk");

	private HashSet<EntityUid> _intersectingEntities = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RCDComponent, MapInitEvent>((ComponentEventHandler<RCDComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RCDComponent, ExaminedEvent>((ComponentEventHandler<RCDComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RCDComponent, AfterInteractEvent>((ComponentEventHandler<RCDComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RCDComponent, RCDDoAfterEvent>((ComponentEventHandler<RCDComponent, RCDDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RCDComponent, DoAfterAttemptEvent<RCDDoAfterEvent>>((ComponentEventHandler<RCDComponent, DoAfterAttemptEvent<RCDDoAfterEvent>>)OnDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RCDComponent, RCDSystemMessage>((ComponentEventHandler<RCDComponent, RCDSystemMessage>)OnRCDSystemMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RCDConstructionGhostRotationEvent>((EntitySessionEventHandler<RCDConstructionGhostRotationEvent>)OnRCDconstructionGhostRotationEvent, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, RCDComponent component, MapInitEvent args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (component.AvailablePrototypes.Count > 0)
		{
			component.ProtoId = component.AvailablePrototypes.ElementAt(0);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
		else
		{
			((EntitySystem)this).QueueDel((EntityUid?)uid);
		}
	}

	private void OnRCDSystemMessage(EntityUid uid, RCDComponent component, RCDSystemMessage args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (component.AvailablePrototypes.Contains(args.ProtoId) && _protoManager.HasIndex<RCDPrototype>(args.ProtoId))
		{
			component.ProtoId = args.ProtoId;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void OnExamine(EntityUid uid, RCDComponent component, ExaminedEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsInDetailsRange)
		{
			return;
		}
		RCDPrototype prototype = _protoManager.Index<RCDPrototype>(component.ProtoId);
		string msg = base.Loc.GetString("rcd-component-examine-mode-details", (ValueTuple<string, object>)("mode", base.Loc.GetString(prototype.SetName)));
		if (prototype.Mode == RcdMode.ConstructTile || prototype.Mode == RcdMode.ConstructObject)
		{
			string name = base.Loc.GetString(prototype.SetName);
			EntityPrototype proto = default(EntityPrototype);
			if (prototype.Prototype != null && _protoManager.TryIndex(EntProtoId.op_Implicit(prototype.Prototype), ref proto))
			{
				name = proto.Name;
			}
			msg = base.Loc.GetString("rcd-component-examine-build-details", (ValueTuple<string, object>)("name", name));
		}
		args.PushMarkup(msg);
	}

	private void OnAfterInteract(EntityUid uid, RCDComponent component, AfterInteractEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach)
		{
			return;
		}
		EntityUid user = args.User;
		EntityCoordinates location = args.ClickLocation;
		RCDPrototype prototype = _protoManager.Index<RCDPrototype>(component.ProtoId);
		if (!((EntityCoordinates)(ref location)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			return;
		}
		EntityUid? gridUid = _transform.GetGrid(location);
		MapGridComponent mapGrid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref mapGrid))
		{
			_popup.PopupClient(base.Loc.GetString("rcd-component-no-valid-grid"), uid, user);
			return;
		}
		TileRef tile = _mapSystem.GetTileRef(gridUid.Value, mapGrid, location);
		Vector2i position = _mapSystem.TileIndicesFor(gridUid.Value, mapGrid, location);
		if (!IsRCDOperationStillValid(uid, component, gridUid.Value, mapGrid, tile, position, args.Target, args.User) || !_net.IsServer)
		{
			return;
		}
		int cost = prototype.Cost;
		float delay = prototype.Delay;
		EntProtoId? effectPrototype = prototype.Effect;
		switch (prototype.Mode)
		{
		case RcdMode.Deconstruct:
		{
			if (args.Target.HasValue)
			{
				RCDDeconstructableComponent destructible = default(RCDDeconstructableComponent);
				if (((EntitySystem)this).TryComp<RCDDeconstructableComponent>(args.Target, ref destructible))
				{
					cost = destructible.Cost;
					delay = destructible.Delay;
					effectPrototype = destructible.Effect;
				}
				break;
			}
			TileRef deconstructedTile = _mapSystem.GetTileRef(gridUid.Value, mapGrid, location);
			ProtoId<RCDPrototype> protoName = ((!_turf.IsSpace(deconstructedTile)) ? _deconstructTileProto : _deconstructLatticeProto);
			RCDPrototype deconProto = default(RCDPrototype);
			if (_protoManager.TryIndex<RCDPrototype>(protoName, ref deconProto))
			{
				cost = deconProto.Cost;
				delay = deconProto.Delay;
				effectPrototype = deconProto.Effect;
			}
			break;
		}
		case RcdMode.ConstructTile:
		{
			TileRef contructedTile = _mapSystem.GetTileRef(gridUid.Value, mapGrid, location);
			if (!((Tile)(ref contructedTile.Tile)).IsEmpty)
			{
				delay = _instantConstructionDelay;
				effectPrototype = _instantConstructionFx;
			}
			break;
		}
		}
		EntProtoId? val = effectPrototype;
		EntityUid effect = ((EntitySystem)this).Spawn(val.HasValue ? EntProtoId.op_Implicit(val.GetValueOrDefault()) : null, location);
		RCDDoAfterEvent ev = new RCDDoAfterEvent(((EntitySystem)this).GetNetCoordinates(location, (MetaDataComponent)null), component.ConstructionDirection, component.ProtoId, cost, ((EntitySystem)this).GetNetEntity(effect, (MetaDataComponent)null));
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, uid, args.Target, uid)
		{
			BreakOnDamage = true,
			BreakOnHandChange = true,
			BreakOnMove = true,
			AttemptFrequency = AttemptFrequency.EveryTick,
			CancelDuplicate = false,
			BlockDuplicate = false
		};
		((HandledEntityEventArgs)args).Handled = true;
		if (!_doAfter.TryStartDoAfter(doAfterArgs))
		{
			((EntitySystem)this).QueueDel((EntityUid?)effect);
		}
	}

	private void OnDoAfterAttempt(EntityUid uid, RCDComponent component, DoAfterAttemptEvent<RCDDoAfterEvent> args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (args.Event?.DoAfter?.Args == null)
		{
			return;
		}
		if (component.ProtoId != args.Event.StartingProtoId)
		{
			((CancellableEntityEventArgs)args).Cancel();
			return;
		}
		EntityCoordinates location = ((EntitySystem)this).GetCoordinates(args.Event.Location);
		EntityUid? gridUid = _transform.GetGrid(location);
		MapGridComponent mapGrid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref mapGrid))
		{
			((CancellableEntityEventArgs)args).Cancel();
			return;
		}
		TileRef tile = _mapSystem.GetTileRef(gridUid.Value, mapGrid, location);
		Vector2i position = _mapSystem.TileIndicesFor(gridUid.Value, mapGrid, location);
		if (!IsRCDOperationStillValid(uid, component, gridUid.Value, mapGrid, tile, position, args.Event.Target, args.Event.User))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnDoAfter(EntityUid uid, RCDComponent component, RCDDoAfterEvent args)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			if (_net.IsServer)
			{
				((EntitySystem)this).QueueDel(((EntitySystem)this).GetEntity(args.Effect));
			}
		}
		else
		{
			if (((HandledEntityEventArgs)args).Handled)
			{
				return;
			}
			((HandledEntityEventArgs)args).Handled = true;
			EntityCoordinates location = ((EntitySystem)this).GetCoordinates(args.Location);
			EntityUid? gridUid = _transform.GetGrid(location);
			MapGridComponent mapGrid = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref mapGrid))
			{
				TileRef tile = _mapSystem.GetTileRef(gridUid.Value, mapGrid, location);
				Vector2i position = _mapSystem.TileIndicesFor(gridUid.Value, mapGrid, location);
				if (IsRCDOperationStillValid(uid, component, gridUid.Value, mapGrid, tile, position, args.Target, args.User))
				{
					FinalizeRCDOperation(uid, component, gridUid.Value, mapGrid, tile, position, args.Direction, args.Target, args.User);
					_audio.PlayPredicted(component.SuccessSound, uid, (EntityUid?)args.User, (AudioParams?)null);
					_sharedCharges.AddCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(uid), -args.Cost);
				}
			}
		}
	}

	private void OnRCDconstructionGhostRotationEvent(RCDConstructionGhostRotationEvent ev, EntitySessionEventArgs session)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = ((EntitySystem)this).GetEntity(ev.NetEntity);
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref session)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid player = attachedEntity.GetValueOrDefault();
			attachedEntity = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(player));
			EntityUid val = uid;
			RCDComponent rcd = default(RCDComponent);
			if (attachedEntity.HasValue && !(attachedEntity.GetValueOrDefault() != val) && ((EntitySystem)this).TryComp<RCDComponent>(uid, ref rcd))
			{
				rcd.ConstructionDirection = ev.Direction;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)rcd, (MetaDataComponent)null);
			}
		}
	}

	public bool IsRCDOperationStillValid(EntityUid uid, RCDComponent component, EntityUid gridUid, MapGridComponent mapGrid, TileRef tile, Vector2i position, EntityUid? target, EntityUid user, bool popMsgs = true)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		RCDPrototype prototype = _protoManager.Index<RCDPrototype>(component.ProtoId);
		int charges = _sharedCharges.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(uid));
		if (charges == 0)
		{
			if (popMsgs)
			{
				_popup.PopupClient(base.Loc.GetString("rcd-component-no-ammo-message"), uid, user);
			}
			return false;
		}
		if (prototype.Cost > charges)
		{
			if (popMsgs)
			{
				_popup.PopupClient(base.Loc.GetString("rcd-component-insufficient-ammo-message"), uid, user);
			}
			return false;
		}
		if (!((!target.HasValue) ? _interaction.InRangeUnobstructed(user, _mapSystem.GridTileToWorld(gridUid, mapGrid, position), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popMsgs) : _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target.Value), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popMsgs)))
		{
			return false;
		}
		switch (prototype.Mode)
		{
		case RcdMode.ConstructTile:
		case RcdMode.ConstructObject:
			return IsConstructionLocationValid(uid, component, gridUid, mapGrid, tile, position, user, popMsgs);
		case RcdMode.Deconstruct:
			return IsDeconstructionStillValid(uid, tile, target, user, popMsgs);
		default:
			return false;
		}
	}

	private bool IsConstructionLocationValid(EntityUid uid, RCDComponent component, EntityUid gridUid, MapGridComponent mapGrid, TileRef tile, Vector2i position, EntityUid user, bool popMsgs = true)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		RCDPrototype prototype = _protoManager.Index<RCDPrototype>(component.ProtoId);
		if (prototype.ConstructionRules.Contains(RcdConstructionRule.MustBuildOnEmptyTile) && !((Tile)(ref tile.Tile)).IsEmpty)
		{
			if (popMsgs)
			{
				_popup.PopupClient(base.Loc.GetString("rcd-component-must-build-on-empty-tile-message"), uid, user);
			}
			return false;
		}
		if (!prototype.ConstructionRules.Contains(RcdConstructionRule.CanBuildOnEmptyTile) && ((Tile)(ref tile.Tile)).IsEmpty)
		{
			if (popMsgs)
			{
				_popup.PopupClient(base.Loc.GetString("rcd-component-cannot-build-on-empty-tile-message"), uid, user);
			}
			return false;
		}
		if (prototype.ConstructionRules.Contains(RcdConstructionRule.MustBuildOnSubfloor) && !_turf.GetContentTileDefinition(tile).IsSubFloor)
		{
			if (popMsgs)
			{
				_popup.PopupClient(base.Loc.GetString("rcd-component-must-build-on-subfloor-message"), uid, user);
			}
			return false;
		}
		if (prototype.Mode == RcdMode.ConstructTile)
		{
			if (!_floors.CanPlaceTile(gridUid, mapGrid, tile.GridIndices, out string reason))
			{
				if (popMsgs)
				{
					_popup.PopupClient(reason, uid, user);
				}
				return false;
			}
			if (_turf.GetContentTileDefinition(tile).ID == prototype.Prototype)
			{
				if (popMsgs)
				{
					_popup.PopupClient(base.Loc.GetString("rcd-component-cannot-build-identical-tile"), uid, user);
				}
				return false;
			}
			return true;
		}
		bool isWindow = prototype.ConstructionRules.Contains(RcdConstructionRule.IsWindow);
		bool isCatwalk = prototype.ConstructionRules.Contains(RcdConstructionRule.IsCatwalk);
		_intersectingEntities.Clear();
		_lookup.GetLocalEntitiesIntersecting(gridUid, position, _intersectingEntities, -0.05f, (LookupFlags)78, (MapGridComponent)null);
		FixturesComponent fixtures = default(FixturesComponent);
		foreach (EntityUid ent in _intersectingEntities)
		{
			if (isWindow && ((EntitySystem)this).HasComp<SharedCanBuildWindowOnTopComponent>(ent))
			{
				continue;
			}
			if (isCatwalk && _tags.HasTag(ent, CatwalkTag))
			{
				if (popMsgs)
				{
					_popup.PopupClient(base.Loc.GetString("rcd-component-cannot-build-on-occupied-tile-message"), uid, user);
				}
				return false;
			}
			if (prototype.CollisionMask == CollisionGroup.None || !((EntitySystem)this).TryComp<FixturesComponent>(ent, ref fixtures))
			{
				continue;
			}
			foreach (Fixture fixture in fixtures.Fixtures.Values)
			{
				if (fixture.Hard && fixture.CollisionLayer > 0 && ((uint)fixture.CollisionLayer & (uint)prototype.CollisionMask) != 0 && (prototype.CollisionPolygon == null || DoesCustomBoundsIntersectWithFixture(prototype.CollisionPolygon, component.ConstructionTransform, ent, fixture)))
				{
					if (popMsgs)
					{
						_popup.PopupClient(base.Loc.GetString("rcd-component-cannot-build-on-occupied-tile-message"), uid, user);
					}
					return false;
				}
			}
		}
		return true;
	}

	private bool IsDeconstructionStillValid(EntityUid uid, TileRef tile, EntityUid? target, EntityUid user, bool popMsgs = true)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		RCDDeconstructableComponent deconstructible = default(RCDDeconstructableComponent);
		if (!target.HasValue)
		{
			if (((Tile)(ref tile.Tile)).IsEmpty)
			{
				if (popMsgs)
				{
					_popup.PopupClient(base.Loc.GetString("rcd-component-nothing-to-deconstruct-message"), uid, user);
				}
				return false;
			}
			if (_turf.IsTileBlocked(tile, CollisionGroup.MobMask))
			{
				if (popMsgs)
				{
					_popup.PopupClient(base.Loc.GetString("rcd-component-tile-obstructed-message"), uid, user);
				}
				return false;
			}
			if (_turf.GetContentTileDefinition(tile).Indestructible)
			{
				if (popMsgs)
				{
					_popup.PopupClient(base.Loc.GetString("rcd-component-tile-indestructible-message"), uid, user);
				}
				return false;
			}
		}
		else if (!((EntitySystem)this).TryComp<RCDDeconstructableComponent>(target, ref deconstructible) || !deconstructible.Deconstructable)
		{
			if (popMsgs)
			{
				_popup.PopupClient(base.Loc.GetString("rcd-component-deconstruct-target-not-on-whitelist-message"), uid, user);
			}
			return false;
		}
		return true;
	}

	private void FinalizeRCDOperation(EntityUid uid, RCDComponent component, EntityUid gridUid, MapGridComponent mapGrid, TileRef tile, Vector2i position, Direction direction, EntityUid? target, EntityUid user)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsServer)
		{
			return;
		}
		RCDPrototype prototype = _protoManager.Index<RCDPrototype>(component.ProtoId);
		if (prototype.Prototype == null)
		{
			return;
		}
		switch (prototype.Mode)
		{
		case RcdMode.ConstructTile:
		{
			_mapSystem.SetTile(gridUid, mapGrid, position, new Tile((int)_tileDefMan[prototype.Prototype].TileId, (byte)0, (byte)0, (byte)0));
			ISharedAdminLogManager adminLogger3 = _adminLogger;
			LogStringHandler handler3 = new LogStringHandler(28, 4);
			handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
			handler3.AppendLiteral(" used RCD to set grid: ");
			handler3.AppendFormatted<EntityUid>(gridUid, "gridUid");
			handler3.AppendLiteral(" ");
			handler3.AppendFormatted<Vector2i>(position, "position");
			handler3.AppendLiteral(" to ");
			handler3.AppendFormatted(prototype.Prototype);
			adminLogger3.Add(LogType.RCD, LogImpact.High, ref handler3);
			break;
		}
		case RcdMode.ConstructObject:
		{
			EntityUid ent = ((EntitySystem)this).Spawn(prototype.Prototype, _mapSystem.GridTileToLocal(gridUid, mapGrid, position));
			switch (prototype.Rotation)
			{
			case RcdRotation.Fixed:
				((EntitySystem)this).Transform(ent).LocalRotation = Angle.Zero;
				break;
			case RcdRotation.Camera:
				((EntitySystem)this).Transform(ent).LocalRotation = ((EntitySystem)this).Transform(uid).LocalRotation;
				break;
			case RcdRotation.User:
				((EntitySystem)this).Transform(ent).LocalRotation = DirectionExtensions.ToAngle(direction);
				break;
			}
			ISharedAdminLogManager adminLogger4 = _adminLogger;
			LogStringHandler handler4 = new LogStringHandler(32, 4);
			handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
			handler4.AppendLiteral(" used RCD to spawn ");
			handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent)), "ToPrettyString(ent)");
			handler4.AppendLiteral(" at ");
			handler4.AppendFormatted<Vector2i>(position, "position");
			handler4.AppendLiteral(" on grid ");
			handler4.AppendFormatted<EntityUid>(gridUid, "gridUid");
			adminLogger4.Add(LogType.RCD, LogImpact.High, ref handler4);
			break;
		}
		case RcdMode.Deconstruct:
			if (!target.HasValue)
			{
				Tile tileDef = (Tile)((_turf.GetContentTileDefinition(tile).ID != "Lattice") ? new Tile((int)_tileDefMan["Lattice"].TileId, (byte)0, (byte)0, (byte)0) : Tile.Empty);
				_mapSystem.SetTile(gridUid, mapGrid, position, tileDef);
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(44, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
				handler.AppendLiteral(" used RCD to set grid: ");
				handler.AppendFormatted<EntityUid>(gridUid, "gridUid");
				handler.AppendLiteral(" tile: ");
				handler.AppendFormatted<Vector2i>(position, "position");
				handler.AppendLiteral(" open to space");
				adminLogger.Add(LogType.RCD, LogImpact.High, ref handler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = _adminLogger;
				LogStringHandler handler2 = new LogStringHandler(20, 2);
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
				handler2.AppendLiteral(" used RCD to delete ");
				handler2.AppendFormatted(((EntitySystem)this).ToPrettyString(target, (MetaDataComponent)null), "target", "ToPrettyString(target)");
				adminLogger2.Add(LogType.RCD, LogImpact.High, ref handler2);
				((EntitySystem)this).QueueDel(target);
			}
			break;
		}
	}

	private bool DoesCustomBoundsIntersectWithFixture(PolygonShape boundingPolygon, Transform boundingTransform, EntityUid fixtureOwner, Fixture fixture)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent entXformComp = ((EntitySystem)this).Transform(fixtureOwner);
		Transform entXform = default(Transform);
		((Transform)(ref entXform))._002Ector(default(Vector2), entXformComp.LocalRotation);
		Box2 val = boundingPolygon.ComputeAABB(boundingTransform, 0);
		Box2 val2 = fixture.Shape.ComputeAABB(entXform, 0);
		return ((Box2)(ref val)).Intersects(ref val2);
	}
}
