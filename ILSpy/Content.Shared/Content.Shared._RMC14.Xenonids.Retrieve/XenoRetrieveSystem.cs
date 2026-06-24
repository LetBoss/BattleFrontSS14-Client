using System;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Actions.Components;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Retrieve;

public sealed class XenoRetrieveSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private LineSystem _line;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedRMCEmoteSystem _rmcEmote;

	[Dependency]
	private RMCSizeStunSystem _rmcSize;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoRetrieveComponent, XenoRetrieveActionEvent>((EntityEventRefHandler<XenoRetrieveComponent, XenoRetrieveActionEvent>)OnXenoRetrieveAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRetrieveComponent, XenoRetrieveDoAfterEvent>((EntityEventRefHandler<XenoRetrieveComponent, XenoRetrieveDoAfterEvent>)OnXenoRetrieveDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRetrieveComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoRetrieveComponent, EntityTerminatingEvent>)OnXenoRetrieveTerminating, (Type[])null, (Type[])null);
	}

	private void OnXenoRetrieveAction(Entity<XenoRetrieveComponent> xeno, ref XenoRetrieveActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		EntityUid target = args.Target;
		if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(target)))
		{
			string msg = base.Loc.GetString("rmc-xeno-not-same-hive");
			_popup.PopupClient(msg, Entity<XenoRetrieveComponent>.op_Implicit(xeno), Entity<XenoRetrieveComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		if (xeno.Owner == target)
		{
			string msg2 = base.Loc.GetString("rmc-xeno-retrieve-self");
			_popup.PopupClient(msg2, Entity<XenoRetrieveComponent>.op_Implicit(xeno), Entity<XenoRetrieveComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		if (((EntitySystem)this).Transform(target).Anchored)
		{
			string msg3 = base.Loc.GetString("rmc-xeno-retrieve-anchored");
			_popup.PopupClient(msg3, Entity<XenoRetrieveComponent>.op_Implicit(xeno), Entity<XenoRetrieveComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		if (_rmcSize.TryGetSize(target, out var size) && (int)size > (int)xeno.Comp.SizeLimit && _mobState.IsAlive(target) && !((EntitySystem)this).HasComp<XenoRestingComponent>(target) && !_standing.IsDown(target))
		{
			string msg4 = base.Loc.GetString("rmc-xeno-retrieve-too-big", (ValueTuple<string, object>)("target", target));
			_popup.PopupClient(msg4, Entity<XenoRetrieveComponent>.op_Implicit(xeno), Entity<XenoRetrieveComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(target), xeno.Comp.Range, CollisionGroup.Impassable))
		{
			string msg5 = base.Loc.GetString("rmc-xeno-retrieve-blocked", (ValueTuple<string, object>)("target", target));
			_popup.PopupClient(msg5, Entity<XenoRetrieveComponent>.op_Implicit(xeno), Entity<XenoRetrieveComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		XenoRetrieveDoAfterEvent ev = new XenoRetrieveDoAfterEvent(((EntitySystem)this).GetNetEntity(Entity<ActionComponent>.op_Implicit(args.Action), (MetaDataComponent)null));
		EntityManager entityManager = base.EntityManager;
		EntityUid user = Entity<XenoRetrieveComponent>.op_Implicit(xeno);
		TimeSpan delay = xeno.Comp.Delay;
		EntityUid? eventTarget = Entity<XenoRetrieveComponent>.op_Implicit(xeno);
		EntityUid? target2 = target;
		EntityUid? blocker = null;
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)entityManager, user, delay, ev, eventTarget, target2, blocker)
		{
			BreakOnMove = true,
			DistanceThreshold = xeno.Comp.Range,
			DuplicateCondition = DuplicateConditions.SameEvent
		};
		if (!_doAfter.TryStartDoAfter(doAfter))
		{
			return;
		}
		string selfMsg = base.Loc.GetString("rmc-xeno-retrieve-start-self", (ValueTuple<string, object>)("target", target));
		string othersMsg = base.Loc.GetString("rmc-xeno-retrieve-start-others", (ValueTuple<string, object>)("user", xeno), (ValueTuple<string, object>)("target", target));
		_popup.PopupPredicted(selfMsg, othersMsg, Entity<XenoRetrieveComponent>.op_Implicit(xeno), Entity<XenoRetrieveComponent>.op_Implicit(xeno));
		foreach (EntityUid visual in xeno.Comp.Visuals)
		{
			((EntitySystem)this).QueueDel((EntityUid?)visual);
		}
		xeno.Comp.Visuals.Clear();
		foreach (LineTile tile in _line.DrawLine(xeno.Owner.ToCoordinates(), target.ToCoordinates(), TimeSpan.Zero, xeno.Comp.Range, out blocker))
		{
			xeno.Comp.Visuals.Add(((EntitySystem)this).Spawn(EntProtoId.op_Implicit(xeno.Comp.Visual), tile.Coordinates, (ComponentRegistry)null, default(Angle)));
		}
		ProtoId<EmotePrototype>? emote = xeno.Comp.Emote;
		if (emote.HasValue)
		{
			ProtoId<EmotePrototype> emote2 = emote.GetValueOrDefault();
			_rmcEmote.TryEmoteWithChat(Entity<XenoRetrieveComponent>.op_Implicit(xeno), emote2);
		}
	}

	private void OnXenoRetrieveDoAfter(Entity<XenoRetrieveComponent> xeno, ref XenoRetrieveDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid visual in xeno.Comp.Visuals)
		{
			((EntitySystem)this).QueueDel((EntityUid?)visual);
		}
		xeno.Comp.Visuals.Clear();
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid? action = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(args.Action, ref action))
		{
			return;
		}
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(target2), xeno.Comp.Range, CollisionGroup.Impassable))
		{
			string msg = base.Loc.GetString("rmc-xeno-retrieve-blocked", (ValueTuple<string, object>)("target", target2));
			_popup.PopupClient(msg, Entity<XenoRetrieveComponent>.op_Implicit(xeno), Entity<XenoRetrieveComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		MapCoordinates userCoords = _transform.GetMapCoordinates(Entity<XenoRetrieveComponent>.op_Implicit(xeno), (TransformComponent)null);
		MapCoordinates targetCoords = _transform.GetMapCoordinates(target2, (TransformComponent)null);
		if (!(userCoords.MapId != targetCoords.MapId))
		{
			Vector2 direction = userCoords.Position - targetCoords.Position;
			direction += Vector2Helpers.Normalized(direction);
			PhysicsComponent physics = default(PhysicsComponent);
			if (!(direction == Vector2.Zero) && ((EntitySystem)this).TryComp<PhysicsComponent>(target2, ref physics) && _rmcActions.TryUseAction(Entity<XenoRetrieveComponent>.op_Implicit(xeno), action.Value, target2))
			{
				_rmcPulling.TryStopAllPullsFromAndOn(target2);
				float length = direction.Length();
				float distance = Math.Clamp(length, 0.1f, xeno.Comp.Range);
				direction *= distance / length;
				Vector2 impulse = Vector2Helpers.Normalized(direction) * xeno.Comp.Force * physics.Mass;
				XenoBeingRetrievedComponent retrieved = ((EntitySystem)this).EnsureComp<XenoBeingRetrievedComponent>(target2);
				retrieved.EndTime = _timing.CurTime + TimeSpan.FromSeconds(direction.Length() / xeno.Comp.Force);
				((EntitySystem)this).Dirty(target2, (IComponent)(object)retrieved, (MetaDataComponent)null);
				_physics.ApplyLinearImpulse(target2, impulse, (FixturesComponent)null, physics);
				_physics.SetBodyStatus(target2, physics, (BodyStatus)1, true);
				string selfMsg = base.Loc.GetString("rmc-xeno-retrieve-finish-user", (ValueTuple<string, object>)("target", target2));
				string othersMsg = base.Loc.GetString("rmc-xeno-retrieve-finish-others", (ValueTuple<string, object>)("user", xeno), (ValueTuple<string, object>)("target", target2));
				_popup.PopupPredicted(selfMsg, othersMsg, Entity<XenoRetrieveComponent>.op_Implicit(xeno), Entity<XenoRetrieveComponent>.op_Implicit(xeno));
				_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoRetrieveComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoRetrieveComponent>.op_Implicit(xeno), (AudioParams?)null);
			}
		}
	}

	private void OnXenoRetrieveTerminating(Entity<XenoRetrieveComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid visual in ent.Comp.Visuals)
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(visual, (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(visual))
			{
				((EntitySystem)this).QueueDel((EntityUid?)visual);
			}
		}
		ent.Comp.Visuals.Clear();
	}

	private void StopRetrieve(Entity<XenoBeingRetrievedComponent> retrieved)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(Entity<XenoBeingRetrievedComponent>.op_Implicit(retrieved), ref physics))
		{
			_physics.SetLinearVelocity(Entity<XenoBeingRetrievedComponent>.op_Implicit(retrieved), Vector2.Zero, true, true, (FixturesComponent)null, physics);
			_physics.SetBodyStatus(Entity<XenoBeingRetrievedComponent>.op_Implicit(retrieved), physics, (BodyStatus)0, true);
		}
		((EntitySystem)this).RemCompDeferred<XenoBeingRetrievedComponent>(Entity<XenoBeingRetrievedComponent>.op_Implicit(retrieved));
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoBeingRetrievedComponent> leaping = ((EntitySystem)this).EntityQueryEnumerator<XenoBeingRetrievedComponent>();
		EntityUid uid = default(EntityUid);
		XenoBeingRetrievedComponent comp = default(XenoBeingRetrievedComponent);
		while (leaping.MoveNext(ref uid, ref comp))
		{
			if (!(time < comp.EndTime))
			{
				StopRetrieve(Entity<XenoBeingRetrievedComponent>.op_Implicit((uid, comp)));
			}
		}
	}
}
