using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Fob;
using Content.Shared._CIV14merka.Pvo;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._PUBG.Structures;
using Content.Shared._RMC14.Construction;
using Content.Shared._RMC14.Construction.Prototypes;
using Content.Shared._RMC14.Entrenching;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._PUBG.Construction;

public sealed class PubgBlueprintSystem : EntitySystem
{
	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly IPrototypeManager _prototype;

	[Dependency]
	private readonly IComponentFactory _compFactory;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	[Dependency]
	private readonly SharedStackSystem _stack;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	[Dependency]
	private readonly SharedDoAfterSystem _doAfter;

	[Dependency]
	private readonly RMCConstructionSystem _construction;

	[Dependency]
	private readonly SharedPhysicsSystem _physics;

	[Dependency]
	private readonly SharedHandsSystem _hands;

	[Dependency]
	private readonly SharedAudioSystem _audio;

	private static readonly SoundSpecifier BuildSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Weapons/rubberhammer.ogg", (AudioParams?)null);

	private const int MaxBlueprintsPerPlayer = 20;

	private string? _pvoCompName;

	private string? _bunkerCompName;

	private const float BunkerHalfExtent = 2.1f;

	private const float BuildClearanceTiles = 4f;

	public override void Initialize()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		((EntitySystem)this).SubscribeLocalEvent<PubgPlaceBlueprintEvent>((EntityEventRefHandler<PubgPlaceBlueprintEvent>)OnPlaceBlueprint, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgBlueprintComponent, InteractUsingEvent>((EntityEventRefHandler<PubgBlueprintComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgBlueprintComponent, PubgBlueprintBuildDoAfterEvent>((EntityEventRefHandler<PubgBlueprintComponent, PubgBlueprintBuildDoAfterEvent>)OnBuildDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgBlueprintComponent, DamageChangedEvent>((EntityEventRefHandler<PubgBlueprintComponent, DamageChangedEvent>)OnDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgBlueprintComponent, ExaminedEvent>((EntityEventRefHandler<PubgBlueprintComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(ContentKeyFunctions.MouseMiddle, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate(OnMiddleClick), true, false)).Register<PubgBlueprintSystem>();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<PubgBlueprintSystem>();
	}

	private void OnPlaceBlueprint(ref PubgPlaceBlueprintEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		RMCConstructionPrototype proto = default(RMCConstructionPrototype);
		if (_net.IsClient || !_prototype.TryIndex<RMCConstructionPrototype>(args.Recipe, ref proto))
		{
			return;
		}
		if (proto.RequiresFobZone && !IsInFriendlyFobZone(args.User, args.Coordinates))
		{
			_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-need-fob-zone"), args.User, args.User, PopupType.SmallCaution);
			return;
		}
		MapCoordinates placeMap = _transform.ToMapCoordinates(args.Coordinates, true);
		if (BuildsPvo(proto) && TooCloseToBunker(placeMap))
		{
			_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-pvo-no-bunker"), args.User, args.User, PopupType.SmallCaution);
			return;
		}
		if (BuildsBunker(proto) && TooCloseToPvo(placeMap))
		{
			_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-bunker-no-pvo"), args.User, args.User, PopupType.SmallCaution);
			return;
		}
		if (BuildsBunker(proto) && TooCloseToBunker(placeMap, 2.1f))
		{
			_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-bunker-no-bunker"), args.User, args.User, PopupType.SmallCaution);
			return;
		}
		int placed = 0;
		EntityQueryEnumerator<PubgBlueprintComponent> countQuery = ((EntitySystem)this).EntityQueryEnumerator<PubgBlueprintComponent>();
		EntityUid user = default(EntityUid);
		PubgBlueprintComponent existing = default(PubgBlueprintComponent);
		while (countQuery.MoveNext(ref user, ref existing))
		{
			EntityUid? placer = existing.Placer;
			user = args.User;
			if (placer.HasValue && placer.GetValueOrDefault() == user)
			{
				placed++;
			}
		}
		if (placed >= 20)
		{
			_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-limit", (ValueTuple<string, object>)("max", 20)), args.User, args.User, PopupType.SmallCaution);
			return;
		}
		EntityCoordinates coords = args.Coordinates.SnapToGrid((IEntityManager?)(object)base.EntityManager);
		EntityUid bp = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(proto.Prototype), coords, (ComponentRegistry)null);
		if (!proto.NoRotate)
		{
			_transform.SetLocalRotation(bp, DirectionExtensions.ToAngle(args.Direction), (TransformComponent)null);
		}
		PubgBlueprintComponent blueprint = ((EntitySystem)this).AddComp<PubgBlueprintComponent>(bp);
		blueprint.Recipe = args.Recipe;
		double seconds = Math.Max(proto.DoAfterTime.TotalSeconds, proto.DoAfterTimeMin.TotalSeconds);
		blueprint.PointsRequired = Math.Max(1, (int)Math.Ceiling(seconds));
		blueprint.FillMaterial = proto.FillMaterial;
		blueprint.MaterialCost = proto.MaterialCost.GetValueOrDefault();
		blueprint.Direction = args.Direction;
		blueprint.NoRotate = proto.NoRotate;
		blueprint.Placer = args.User;
		((EntitySystem)this).Dirty(bp, (IComponent)(object)blueprint, (MetaDataComponent)null);
		((EntitySystem)this).EnsureComp<RMCConstructionGhostComponent>(bp);
		PhysicsComponent phys = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(bp, ref phys))
		{
			_physics.SetCanCollide(bp, false, true, false, (FixturesComponent)null, phys);
		}
	}

	private void OnInteractUsing(Entity<PubgBlueprintComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid used = args.Used;
		bool isHammer = ((EntitySystem)this).HasComp<PubgBlueprintHammerComponent>(used);
		bool isFiller = !ent.Comp.Filled && ((EntitySystem)this).HasComp<PubgBlueprintFillerComponent>(used);
		if (!isHammer && !isFiller)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!_net.IsClient)
		{
			if (isHammer)
			{
				TryStartBuild(ent, args.User);
			}
			else
			{
				TryFill(ent, args.User, used);
			}
		}
	}

	private void TryFill(Entity<PubgBlueprintComponent> ent, EntityUid user, EntityUid used)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		StackComponent stack = default(StackComponent);
		if (ent.Comp.Filled || !((EntitySystem)this).TryComp<StackComponent>(used, ref stack))
		{
			return;
		}
		ProtoId<StackPrototype>? fillMaterial = ent.Comp.FillMaterial;
		if (fillMaterial.HasValue)
		{
			ProtoId<StackPrototype> need = fillMaterial.GetValueOrDefault();
			if (stack.StackTypeId != need.Id)
			{
				_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-wrong-material"), Entity<PubgBlueprintComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
				return;
			}
		}
		int cost = Math.Max(1, ent.Comp.MaterialCost);
		if (_stack.GetCount(used, stack) < cost)
		{
			_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-more-material", (ValueTuple<string, object>)("count", cost)), Entity<PubgBlueprintComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
		}
		else if (_stack.Use(used, cost, stack))
		{
			ent.Comp.Filled = true;
			((EntitySystem)this).Dirty<PubgBlueprintComponent>(ent, (MetaDataComponent)null);
			_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-filled"), Entity<PubgBlueprintComponent>.op_Implicit(ent), user, PopupType.Medium);
		}
	}

	private void TryStartBuild(Entity<PubgBlueprintComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Filled)
		{
			_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-needs-fill"), Entity<PubgBlueprintComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			return;
		}
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, TimeSpan.FromSeconds(1L), new PubgBlueprintBuildDoAfterEvent(), Entity<PubgBlueprintComponent>.op_Implicit(ent), Entity<PubgBlueprintComponent>.op_Implicit(ent))
		{
			NeedHand = true,
			BreakOnMove = true,
			MovementThreshold = 0.5f,
			BreakOnDamage = false,
			DuplicateCondition = (DuplicateConditions.SameTarget | DuplicateConditions.SameEvent)
		};
		_doAfter.TryStartDoAfter(doAfter);
	}

	private void OnBuildDoAfter(Entity<PubgBlueprintComponent> ent, ref PubgBlueprintBuildDoAfterEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !_net.IsClient && !((EntitySystem)this).Deleted(Entity<PubgBlueprintComponent>.op_Implicit(ent), (MetaDataComponent)null) && !ent.Comp.Completed)
		{
			ent.Comp.Points++;
			((EntitySystem)this).Dirty<PubgBlueprintComponent>(ent, (MetaDataComponent)null);
			_audio.PlayPvs(BuildSound, Entity<PubgBlueprintComponent>.op_Implicit(ent), (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.15f));
			if (ent.Comp.Points >= ent.Comp.PointsRequired)
			{
				FinishBlueprint(ent, args.User);
			}
			else
			{
				args.Repeat = true;
			}
		}
	}

	private void FinishBlueprint(Entity<PubgBlueprintComponent> ent, EntityUid user)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || ent.Comp.Completed)
		{
			return;
		}
		ent.Comp.Completed = true;
		EntityCoordinates coordinates = ((EntitySystem)this).Transform(Entity<PubgBlueprintComponent>.op_Implicit(ent)).Coordinates;
		Direction direction = ent.Comp.Direction;
		((EntitySystem)this).QueueDel((EntityUid?)Entity<PubgBlueprintComponent>.op_Implicit(ent));
		RMCConstructionPrototype proto = default(RMCConstructionPrototype);
		if (_prototype.TryIndex<RMCConstructionPrototype>(ent.Comp.Recipe, ref proto))
		{
			EntityUid built = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(proto.Prototype), coordinates, (ComponentRegistry)null);
			if (!proto.NoRotate)
			{
				_transform.SetLocalRotation(built, DirectionExtensions.ToAngle(direction), (TransformComponent)null);
			}
			if (!((EntitySystem)this).HasComp<BarricadeComponent>(built))
			{
				_construction.MakeConstructionImmuneToCollision(built, user);
			}
			RMCConstructionBuiltEvent ev = new RMCConstructionBuiltEvent(built, user);
			((EntitySystem)this).RaiseLocalEvent<RMCConstructionBuiltEvent>(ref ev);
		}
	}

	private void OnDamageChanged(Entity<PubgBlueprintComponent> ent, ref DamageChangedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && !ent.Comp.Completed && args.Damageable.TotalDamage.Float() >= ent.Comp.BreakDamage)
		{
			ent.Comp.Completed = true;
			_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-destroyed"), Entity<PubgBlueprintComponent>.op_Implicit(ent), PopupType.MediumCaution);
			((EntitySystem)this).QueueDel((EntityUid?)Entity<PubgBlueprintComponent>.op_Implicit(ent));
		}
	}

	private void OnExamine(Entity<PubgBlueprintComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Filled)
		{
			args.PushMarkup(base.Loc.GetString("pubg-blueprint-examine-filled"));
		}
		else
		{
			args.PushMarkup(base.Loc.GetString("pubg-blueprint-examine-need-material", (ValueTuple<string, object>)("count", Math.Max(1, ent.Comp.MaterialCost))));
		}
		args.PushMarkup(base.Loc.GetString("pubg-blueprint-examine-points", (ValueTuple<string, object>)("points", ent.Comp.Points), (ValueTuple<string, object>)("required", ent.Comp.PointsRequired)));
	}

	private bool OnMiddleClick(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
		if (val.HasValue)
		{
			EntityUid user = val.GetValueOrDefault();
			PubgBlueprintComponent bp = default(PubgBlueprintComponent);
			if (!((EntitySystem)this).TryComp<PubgBlueprintComponent>(uid, ref bp) || bp.Completed)
			{
				return false;
			}
			if (!HoldingHammer(user))
			{
				return false;
			}
			if (_net.IsServer)
			{
				bp.Completed = true;
				_popup.PopupEntity(base.Loc.GetString("pubg-blueprint-removed"), uid, user);
				((EntitySystem)this).QueueDel((EntityUid?)uid);
			}
			return true;
		}
		return false;
	}

	private bool IsInFriendlyFobZone(EntityUid user, EntityCoordinates coords)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates placeMap = _transform.ToMapCoordinates(coords, true);
		if (placeMap.MapId == MapId.Nullspace)
		{
			return false;
		}
		CivTeamMemberComponent member = default(CivTeamMemberComponent);
		string side = (((EntitySystem)this).TryComp<CivTeamMemberComponent>(user, ref member) ? member.SideId : null);
		EntityQueryEnumerator<CivFobComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<CivFobComponent, TransformComponent>();
		EntityUid fobUid = default(EntityUid);
		CivFobComponent fob = default(CivFobComponent);
		TransformComponent val = default(TransformComponent);
		while (query.MoveNext(ref fobUid, ref fob, ref val))
		{
			if (!((EntitySystem)this).HasComp<PubgBlueprintComponent>(fobUid) && (string.IsNullOrWhiteSpace(side) || string.IsNullOrWhiteSpace(fob.SideId) || string.Equals(fob.SideId, side, StringComparison.OrdinalIgnoreCase)))
			{
				MapCoordinates fobMap = _transform.GetMapCoordinates(fobUid, (TransformComponent)null);
				if (!(fobMap.MapId != placeMap.MapId) && (fobMap.Position - placeMap.Position).Length() <= fob.BuildZoneRange)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool BuildsPvo(RMCConstructionPrototype recipe)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (_pvoCompName == null)
		{
			_pvoCompName = _compFactory.GetComponentName(typeof(CivPvoComponent));
		}
		EntityPrototype built = default(EntityPrototype);
		if (_prototype.TryIndex(recipe.Prototype, ref built))
		{
			return ((Dictionary<string, ComponentRegistryEntry>)(object)built.Components).ContainsKey(_pvoCompName);
		}
		return false;
	}

	private bool BuildsBunker(RMCConstructionPrototype recipe)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (_bunkerCompName == null)
		{
			_bunkerCompName = _compFactory.GetComponentName(typeof(PubgBunkerComponent));
		}
		EntityPrototype built = default(EntityPrototype);
		if (_prototype.TryIndex(recipe.Prototype, ref built))
		{
			return ((Dictionary<string, ComponentRegistryEntry>)(object)built.Components).ContainsKey(_bunkerCompName);
		}
		return false;
	}

	private bool TooCloseToBunker(MapCoordinates placeMap, float placerHalfExtent = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (placeMap.MapId == MapId.Nullspace)
		{
			return false;
		}
		float threshold = 2.1f + placerHalfExtent + 4f;
		EntityQueryEnumerator<PubgBunkerComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<PubgBunkerComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		PubgBunkerComponent pubgBunkerComponent = default(PubgBunkerComponent);
		TransformComponent val = default(TransformComponent);
		while (query.MoveNext(ref uid, ref pubgBunkerComponent, ref val))
		{
			MapCoordinates bunkerMap = _transform.GetMapCoordinates(uid, (TransformComponent)null);
			if (!(bunkerMap.MapId != placeMap.MapId) && (bunkerMap.Position - placeMap.Position).Length() <= threshold)
			{
				return true;
			}
		}
		return false;
	}

	private bool TooCloseToPvo(MapCoordinates placeMap)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (placeMap.MapId == MapId.Nullspace)
		{
			return false;
		}
		float threshold = 6.1f;
		EntityQueryEnumerator<CivPvoComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<CivPvoComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		CivPvoComponent civPvoComponent = default(CivPvoComponent);
		TransformComponent val = default(TransformComponent);
		while (query.MoveNext(ref uid, ref civPvoComponent, ref val))
		{
			MapCoordinates pvoMap = _transform.GetMapCoordinates(uid, (TransformComponent)null);
			if (!(pvoMap.MapId != placeMap.MapId) && (pvoMap.Position - placeMap.Position).Length() <= threshold)
			{
				return true;
			}
		}
		return false;
	}

	private bool HoldingHammer(EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<HandsComponent>(user))
		{
			return false;
		}
		foreach (EntityUid held in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(user)))
		{
			if (((EntitySystem)this).HasComp<PubgBlueprintHammerComponent>(held))
			{
				return true;
			}
		}
		return false;
	}
}
