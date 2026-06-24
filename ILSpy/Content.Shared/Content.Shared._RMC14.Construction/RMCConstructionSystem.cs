using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._PUBG.Construction;
using Content.Shared._RMC14.Construction.Prototypes;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Ladder;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Construction.Components;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Construction;

public sealed class RMCConstructionSystem : EntitySystem
{
	[Dependency]
	private FixtureSystem _fixture;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private ExamineSystemShared _examine;

	private static readonly EntProtoId Blocker = EntProtoId.op_Implicit("RMCDropshipDoorBlocker");

	private readonly List<EntityCoordinates> _toCreate = new List<EntityCoordinates>();

	private EntityQuery<DoorComponent> _doorQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_doorQuery = ((EntitySystem)this).GetEntityQuery<DoorComponent>();
		((EntitySystem)this).SubscribeLocalEvent<DropshipHijackLandedEvent>((EntityEventRefHandler<DropshipHijackLandedEvent>)OnDropshipHijackLanded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCConstructionPreventCollideComponent, PreventCollideEvent>((EntityEventRefHandler<RMCConstructionPreventCollideComponent, PreventCollideEvent>)OnConstructionPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCConstructionItemComponent, UseInHandEvent>((EntityEventRefHandler<RMCConstructionItemComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCConstructionItemComponent, RMCConstructionBuildDoAfterEvent>((EntityEventRefHandler<RMCConstructionItemComponent, RMCConstructionBuildDoAfterEvent>)OnBuildDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCConstructionAttemptEvent>((EntityEventRefHandler<RMCConstructionAttemptEvent>)OnConstructionAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipComponent, DropshipMapInitEvent>((EntityEventRefHandler<DropshipComponent, DropshipMapInitEvent>)OnDropshipMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDropshipBlockedComponent, MapInitEvent>((EntityEventRefHandler<RMCDropshipBlockedComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDropshipBlockedComponent, AnchorAttemptEvent>((EntityEventRefHandler<RMCDropshipBlockedComponent, AnchorAttemptEvent>)OnAnchorAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDropshipBlockedComponent, UserAnchoredEvent>((EntityEventRefHandler<RMCDropshipBlockedComponent, UserAnchoredEvent>)OnUserAnchored, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<RMCConstructionItemComponent>(((EntitySystem)this).Subs, (object)RMCConstructionUiKey.Key, (BuiEventSubscriber<RMCConstructionItemComponent>)delegate(Subscriber<RMCConstructionItemComponent> subs)
		{
			subs.Event<RMCConstructionBuiMsg>((EntityEventRefHandler<RMCConstructionItemComponent, RMCConstructionBuiMsg>)OnConstructionBuiMsg);
		});
	}

	private void OnDropshipHijackLanded(ref DropshipHijackLandedEvent ev)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityQueryEnumerator<RMCReplaceOnHijackLandComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RMCReplaceOnHijackLandComponent>();
		EntityUid uid = default(EntityUid);
		RMCReplaceOnHijackLandComponent comp = default(RMCReplaceOnHijackLandComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			EntProtoId? id = comp.Id;
			if (id.HasValue)
			{
				EntProtoId id2 = id.GetValueOrDefault();
				EntityCoordinates coordinates = _transform.GetMoverCoordinates(uid);
				((EntitySystem)this).Del((EntityUid?)uid);
				((EntitySystem)this).Spawn(EntProtoId.op_Implicit(id2), coordinates);
			}
			else
			{
				((EntitySystem)this).Del((EntityUid?)uid);
			}
		}
	}

	public void OnUseInHand(Entity<RMCConstructionItemComponent> ent, ref UseInHandEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		((HandledEntityEventArgs)args).Handled = true;
		_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCConstructionUiKey.Key, (EntityUid?)user, false);
	}

	private void OnConstructionBuiMsg(Entity<RMCConstructionItemComponent> ent, ref RMCConstructionBuiMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Build(ent, ((BaseBoundUserInterfaceEvent)args).Actor, args.Build, args.Amount);
	}

	public bool Build(Entity<RMCConstructionItemComponent> ent, EntityUid user, ProtoId<RMCConstructionPrototype> protoID, int amount)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return false;
		}
		RMCConstructionPrototype proto = default(RMCConstructionPrototype);
		if (!_prototype.TryIndex<RMCConstructionPrototype>(protoID, ref proto))
		{
			return false;
		}
		TransformComponent transform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(user, ref transform))
		{
			return false;
		}
		EntProtoId<SkillDefinitionComponent>? skill = proto.Skill;
		if (skill.HasValue)
		{
			EntProtoId<SkillDefinitionComponent> skill2 = skill.GetValueOrDefault();
			if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user), skill2, proto.RequiredSkillLevel))
			{
				string message = base.Loc.GetString("rmc-construction-untrained-build");
				_popup.PopupEntity(message, Entity<RMCConstructionItemComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
				return false;
			}
		}
		CivTeamMemberComponent civMember = default(CivTeamMemberComponent);
		if (!string.IsNullOrEmpty(proto.SideId) && (!((EntitySystem)this).TryComp<CivTeamMemberComponent>(user, ref civMember) || !string.Equals(civMember.SideId, proto.SideId, StringComparison.OrdinalIgnoreCase)))
		{
			_popup.PopupEntity(base.Loc.GetString("civ-construction-wrong-side"), Entity<RMCConstructionItemComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			return false;
		}
		Angle localRotation = transform.LocalRotation;
		Direction direction = ((Angle)(ref localRotation)).GetCardinalDir();
		EntityCoordinates coordinates = transform.Coordinates;
		if (proto.PlaceInFront)
		{
			coordinates = ((EntityCoordinates)(ref coordinates)).Offset(DirectionExtensions.ToVec(direction));
		}
		if (!proto.IgnoreBuildRestrictions && !CanBuildAt(coordinates, proto.Name, out string popup, anchoring: false, direction, proto.RestrictedCollisionGroup))
		{
			_popup.PopupEntity(popup, Entity<RMCConstructionItemComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			return false;
		}
		ProtoId<TagPrototype>[] tags = proto.RestrictedTags;
		if (tags != null && _rmcMap.TileHasAnyTag(coordinates, tags))
		{
			string message2 = base.Loc.GetString("rmc-construction-not-proper-surface", (ValueTuple<string, object>)("construction", proto.Name));
			_popup.PopupEntity(message2, Entity<RMCConstructionItemComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			return false;
		}
		if (((EntitySystem)this).HasComp<PubgBlueprintHammerComponent>(Entity<RMCConstructionItemComponent>.op_Implicit(ent)))
		{
			PubgPlaceBlueprintEvent blueprintEv = new PubgPlaceBlueprintEvent(protoID, user, coordinates, direction);
			((EntitySystem)this).RaiseLocalEvent<PubgPlaceBlueprintEvent>(ref blueprintEv);
			return true;
		}
		int? materialCost = proto.MaterialCost;
		if (materialCost.HasValue)
		{
			int materialCost2 = materialCost.GetValueOrDefault();
			StackComponent stack = default(StackComponent);
			if (((EntitySystem)this).TryComp<StackComponent>(ent.Owner, ref stack))
			{
				int totalAmount = amount / proto.Amount;
				int cost = ((amount == proto.Amount) ? materialCost2 : (totalAmount * materialCost2));
				if (stack.Count < cost)
				{
					string message3 = base.Loc.GetString("rmc-construction-more-material", (ValueTuple<string, object>)("material", ent), (ValueTuple<string, object>)("object", proto.Name));
					_popup.PopupEntity(message3, user, user, PopupType.SmallCaution);
					return false;
				}
			}
		}
		RMCConstructionBuildDoAfterEvent ev = new RMCConstructionBuildDoAfterEvent(proto, amount, ((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null), direction);
		int skillMultiplier = (_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user), proto.DelaySkill, 2) ? 1 : 2);
		double doAfterTime = Math.Max((proto.DoAfterTime * skillMultiplier).TotalSeconds, proto.DoAfterTimeMin.TotalSeconds);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, TimeSpan.FromSeconds(doAfterTime), ev, Entity<RMCConstructionItemComponent>.op_Implicit(ent), Entity<RMCConstructionItemComponent>.op_Implicit(ent))
		{
			BreakOnMove = true,
			BreakOnDamage = false,
			MovementThreshold = 0.5f,
			DuplicateCondition = DuplicateConditions.SameEvent,
			CancelDuplicate = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			SpawnConstructionGhost(user, proto.Prototype, coordinates, direction, proto.NoRotate);
		}
		UpdateStackAmountUI(ent);
		return true;
	}

	private void SpawnConstructionGhost(EntityUid user, EntProtoId targetProto, EntityCoordinates coordinates, Direction direction, bool noRotate)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			ClearConstructionGhost(user);
			EntityUid ghost = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(targetProto), coordinates, (ComponentRegistry)null);
			if (!noRotate)
			{
				_transform.SetLocalRotation(ghost, DirectionExtensions.ToAngle(direction), (TransformComponent)null);
			}
			((EntitySystem)this).EnsureComp<RMCConstructionGhostComponent>(ghost);
			((EntitySystem)this).EnsureComp<RMCActiveConstructionGhostComponent>(user).Ghost = ghost;
		}
	}

	private void ClearConstructionGhost(EntityUid user)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		RMCActiveConstructionGhostComponent active = default(RMCActiveConstructionGhostComponent);
		if (_net.IsClient || !((EntitySystem)this).TryComp<RMCActiveConstructionGhostComponent>(user, ref active))
		{
			return;
		}
		EntityUid? ghost = active.Ghost;
		if (ghost.HasValue)
		{
			EntityUid ghost2 = ghost.GetValueOrDefault();
			if (!((EntitySystem)this).Deleted(ghost2, (MetaDataComponent)null))
			{
				((EntitySystem)this).QueueDel((EntityUid?)ghost2);
			}
		}
		((EntitySystem)this).RemCompDeferred<RMCActiveConstructionGhostComponent>(user);
	}

	private void OnConstructionPreventCollide(Entity<RMCConstructionPreventCollideComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		EntityUid? target = ent.Comp.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (!((EntitySystem)this).Deleted(target2, (MetaDataComponent)null))
			{
				if (!(args.OtherEntity != target2))
				{
					if (!_examine.InRangeUnOccluded(ent.Owner, target2, ent.Comp.Range))
					{
						((EntitySystem)this).RemCompDeferred<RMCConstructionPreventCollideComponent>(ent.Owner);
					}
					else
					{
						args.Cancelled = true;
					}
				}
				return;
			}
		}
		((EntitySystem)this).RemCompDeferred<RMCConstructionPreventCollideComponent>(ent.Owner);
	}

	public void MakeConstructionImmuneToCollision(EntityUid construction, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		RMCConstructionPreventCollideComponent constructionComp = ((EntitySystem)this).EnsureComp<RMCConstructionPreventCollideComponent>(construction);
		constructionComp.Target = user;
		((EntitySystem)this).Dirty(construction, (IComponent)(object)constructionComp, (MetaDataComponent)null);
	}

	private void OnBuildDoAfter(Entity<RMCConstructionItemComponent> ent, ref RMCConstructionBuildDoAfterEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		ClearConstructionGhost(args.User);
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled || _net.IsClient)
		{
			return;
		}
		RMCConstructionPrototype entry = args.Prototype;
		EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		((HandledEntityEventArgs)args).Handled = true;
		int totalAmount = args.Amount / entry.Amount;
		int? cost = ((args.Amount == entry.Amount) ? entry.MaterialCost : (totalAmount * entry.MaterialCost));
		StackComponent stack = default(StackComponent);
		if (((EntitySystem)this).TryComp<StackComponent>(ent.Owner, ref stack))
		{
			if (!_stack.Use(ent.Owner, cost ?? 1, stack))
			{
				string message = base.Loc.GetString("rmc-construction-more-material", (ValueTuple<string, object>)("material", ent.Owner), (ValueTuple<string, object>)("object", entry.Name));
				_popup.PopupEntity(message, args.User, args.User, PopupType.SmallCaution);
				return;
			}
		}
		else if (_net.IsServer)
		{
			((EntitySystem)this).QueueDel((EntityUid?)ent.Owner);
		}
		if (!((EntitySystem)this).Deleted(Entity<RMCConstructionItemComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			UpdateStackAmountUI(ent);
		}
		if (args.Amount > 1)
		{
			SpawnMultiple(EntProtoId.op_Implicit(entry.Prototype), args.Amount, coordinates);
			return;
		}
		EntityUid built = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(entry.Prototype), coordinates, (ComponentRegistry)null);
		if (!entry.NoRotate)
		{
			_transform.SetLocalRotation(built, DirectionExtensions.ToAngle(args.Direction), (TransformComponent)null);
		}
		if (!((EntitySystem)this).HasComp<BarricadeComponent>(built))
		{
			MakeConstructionImmuneToCollision(built, args.User);
		}
		RMCConstructionBuiltEvent builtEv = new RMCConstructionBuiltEvent(built, args.User);
		((EntitySystem)this).RaiseLocalEvent<RMCConstructionBuiltEvent>(ref builtEv);
	}

	public List<EntityUid> SpawnMultiple(string entityPrototype, int amount, EntityCoordinates spawnPosition)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return new List<EntityUid>();
		}
		if (amount <= 0)
		{
			((EntitySystem)this).Log.Error($"Attempted to spawn an invalid stack: {entityPrototype}, {amount}. Trace: {Environment.StackTrace}");
			return new List<EntityUid>();
		}
		List<int> list = CalculateSpawns(entityPrototype, amount);
		List<EntityUid> spawnedEnts = new List<EntityUid>();
		foreach (int count in list)
		{
			EntityUid entity = ((EntitySystem)this).SpawnAtPosition(entityPrototype, spawnPosition, (ComponentRegistry)null);
			spawnedEnts.Add(entity);
			_stack.SetCount(entity, count);
		}
		return spawnedEnts;
	}

	public List<int> CalculateSpawns(string entityPrototype, int amount)
	{
		StackComponent stack = default(StackComponent);
		_prototype.Index<EntityPrototype>(entityPrototype).TryGetComponent<StackComponent>(ref stack, base.EntityManager.ComponentFactory);
		int maxCountPerStack = _stack.GetMaxCount(stack);
		List<int> amounts = new List<int>();
		while (amount > 0)
		{
			int countAmount = Math.Min(maxCountPerStack, amount);
			amount -= countAmount;
			amounts.Add(countAmount);
		}
		return amounts;
	}

	private void UpdateStackAmountUI(Entity<RMCConstructionItemComponent> ent)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		RMCConstructionBuiState state = new RMCConstructionBuiState(string.Empty);
		_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCConstructionUiKey.Key, (BoundUserInterfaceState)(object)state);
	}

	private void OnConstructionAttempt(ref RMCConstructionAttemptEvent ev)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!ev.Cancelled && !CanBuildAt(ev.Location, ev.PrototypeName, out string popup, anchoring: false, (Direction)(-1)))
		{
			ev.Popup = popup;
			ev.Cancelled = true;
		}
	}

	private void OnDropshipMapInit(Entity<DropshipComponent> ent, ref DropshipMapInitEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		_toCreate.Clear();
		TransformChildrenEnumerator enumerator = ((EntitySystem)this).Transform(Entity<DropshipComponent>.op_Implicit(ent)).ChildEnumerator;
		EntityUid child = default(EntityUid);
		while (((TransformChildrenEnumerator)(ref enumerator)).MoveNext(ref child))
		{
			if (_doorQuery.HasComp(child))
			{
				_toCreate.Add(child.ToCoordinates());
			}
		}
		foreach (EntityCoordinates toCreate in _toCreate)
		{
			((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(Blocker), toCreate, (ComponentRegistry)null);
		}
	}

	private void OnMapInit(Entity<RMCDropshipBlockedComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(Entity<RMCDropshipBlockedComponent>.op_Implicit(ent), ref physics))
		{
			PhysShapeCircle shape = new PhysShapeCircle(0.49f);
			_fixture.TryCreateFixture(Entity<RMCDropshipBlockedComponent>.op_Implicit(ent), (IPhysShape)(object)shape, ent.Comp.FixtureId, 1f, true, 0, 268435456, 0.4f, 0f, true, (FixturesComponent)null, physics, (TransformComponent)null);
		}
	}

	private void OnAnchorAttempt(Entity<RMCDropshipBlockedComponent> ent, ref AnchorAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !CanBuildAt(ent.Owner.ToCoordinates(), ((EntitySystem)this).Name(Entity<RMCDropshipBlockedComponent>.op_Implicit(ent), (MetaDataComponent)null), out string popup, anchoring: true, (Direction)(-1)))
		{
			_popup.PopupClient(popup, Entity<RMCDropshipBlockedComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnUserAnchored(Entity<RMCDropshipBlockedComponent> ent, ref UserAnchoredEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!CanBuildAt(ent.Owner.ToCoordinates(), ((EntitySystem)this).Name(Entity<RMCDropshipBlockedComponent>.op_Implicit(ent), (MetaDataComponent)null), out string _, anchoring: true, (Direction)(-1)))
		{
			TransformComponent xform = ((EntitySystem)this).Transform(Entity<RMCDropshipBlockedComponent>.op_Implicit(ent));
			_transform.Unanchor(ent.Owner, xform, true);
		}
	}

	public bool CanConstruct(EntityUid? user)
	{
		return !((EntitySystem)this).HasComp<DisableConstructionComponent>(user);
	}

	public bool CanBuildAt(EntityCoordinates coordinates, string? prototypeName, out string? popup, bool anchoring = false, Direction direction = (Direction)(-1), CollisionGroup? collision = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Invalid comparison between Unknown and I4
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		popup = null;
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			if (!_turf.TryGetTileRef(coordinates, out var turf))
			{
				return false;
			}
			if (prototypeName == null)
			{
				prototypeName = base.Loc.GetString("rmc-construction-name");
			}
			if (((EntitySystem)this).HasComp<DropshipComponent>(gridId))
			{
				popup = base.Loc.GetString("rmc-construction-not-proper-surface", (ValueTuple<string, object>)("construction", prototypeName));
				return false;
			}
			MapGridComponent grid2 = default(MapGridComponent);
			if (!((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				return true;
			}
			Vector2i indices = _map.TileIndicesFor(gridId, grid2, coordinates);
			ITileDefinition def = default(ITileDefinition);
			if (!_map.TryGetTileDef(grid2, indices, ref def))
			{
				return true;
			}
			bool invalid = def is ContentTileDefinition contentTileDefinition && contentTileDefinition.BlockConstruction;
			if (anchoring)
			{
				invalid = def is ContentTileDefinition contentTileDefinition2 && contentTileDefinition2.BlockAnchoring;
			}
			if (invalid || _rmcMap.HasAnchoredEntityEnumerator<LadderComponent>(coordinates, (Direction?)null, (DirectionFlag)0))
			{
				popup = base.Loc.GetString("rmc-construction-not-proper-surface", (ValueTuple<string, object>)("construction", prototypeName));
				return false;
			}
			if ((int)direction != -1)
			{
				RMCMapSystem rmcMap = _rmcMap;
				DirectionFlag facing = DirectionExtensions.AsFlag(direction);
				if (rmcMap.HasAnchoredEntityEnumerator<BarricadeComponent>(coordinates, (Direction?)null, facing))
				{
					popup = base.Loc.GetString("rmc-construction-not-barricade-clear");
					return false;
				}
			}
			if (collision.HasValue)
			{
				CollisionGroup collisionGroup = collision.GetValueOrDefault();
				if (_turf.IsTileBlocked(turf.Value, collisionGroup))
				{
					popup = base.Loc.GetString("rmc-construction-not-proper-surface", (ValueTuple<string, object>)("construction", prototypeName));
					return false;
				}
			}
			return true;
		}
		return true;
	}
}
